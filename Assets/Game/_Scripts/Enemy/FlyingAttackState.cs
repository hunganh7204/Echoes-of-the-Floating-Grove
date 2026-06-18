using UnityEngine;

public class FlyingAttackState : IEnemyState
{
    private float attackCooldown = 1.5f; // Thời gian chờ giữa 2 lần cắn
    private float timer;

    [Header("Hit & Run Settings")]
    private float flyUpDuration = 0.5f;  // Thời gian bay lùi lên trên (0.5 giây)
    private float flyUpSpeed = 3f;       // Tốc độ bay lên
    private float diveSpeed = 6f;        // Tốc độ lao xuống cắn (nhanh hơn bình thường)

    // Khai báo 3 hành động trong lúc tấn công
    private enum AttackPhase { Retreat, Hover, Swoop }
    private AttackPhase currentPhase;

    public void EnterState(EnemyBase enemy)
    {
        // Vừa vào tầm đánh là thực hiện cắn ngay lập tức
        PerformAttack(enemy);
    }

    public void UpdateState(EnemyBase enemy)
    {
        // Luôn nhìn theo người chơi
        enemy.FaceTarget(enemy.PlayerTarget.position.x);

        // Nếu người chơi chạy quá xa khỏi tầm nhìn -> Bỏ qua hồi chiêu, quay về vị trí cũ
        if (enemy.DistanceToPlayer() > enemy.SightRange)
        {
            enemy.ChangeState(enemy.returnState);
            return;
        }

        timer += Time.deltaTime;

        // Xử lý các giai đoạn
        switch (currentPhase)
        {
            case AttackPhase.Retreat:
                // Bay ngược lên trên một đoạn
                enemy.RB.linearVelocity = Vector2.up * flyUpSpeed;

                if (timer >= flyUpDuration)
                {
                    currentPhase = AttackPhase.Hover; // Bay đủ thời gian thì dừng lại lơ lửng
                }
                break;

            case AttackPhase.Hover:
                // Đứng im trên không chờ hồi chiêu
                enemy.RB.linearVelocity = Vector2.zero;

                if (timer >= attackCooldown)
                {
                    currentPhase = AttackPhase.Swoop; // Hồi chiêu xong -> Chuyển sang bổ nhào
                }
                break;

            case AttackPhase.Swoop:
                // Lao thẳng vào người chơi
                Vector2 dir = (enemy.PlayerTarget.position - enemy.transform.position).normalized;
                enemy.RB.linearVelocity = dir * diveSpeed;

                // Khi tiến vào đủ gần (vào tầm chém) -> Thực hiện cắn tiếp
                if (enemy.DistanceToPlayer() <= enemy.AttackRange)
                {
                    PerformAttack(enemy);
                }
                break;
        }
    }

    private void PerformAttack(EnemyBase enemy)
    {
        Debug.Log($"<color=red>{enemy.gameObject.name} bổ nhào cắn mục tiêu!</color>");

        // Kích hoạt Animation
        enemy.Anim.SetTrigger("Attack");

        // Quét và gây sát thương
        Collider2D[] hits = Physics2D.OverlapCircleAll(enemy.transform.position, enemy.AttackRange);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    Vector2 knockbackDir = (hit.transform.position - enemy.transform.position).normalized;
                    knockbackDir.y = 0.5f;

                    int enemyDamage = 15;
                    damageable.TakeDamage(enemyDamage, knockbackDir);
                }
            }
        }

        // Đánh xong thì reset thời gian và lập tức chuyển sang pha bay lên (Retreat)
        timer = 0f;
        currentPhase = AttackPhase.Retreat;
    }

    public void ExitState(EnemyBase enemy)
    {
        // Reset vận tốc khi thoát khỏi trạng thái đánh
        enemy.RB.linearVelocity = Vector2.zero;
    }
}