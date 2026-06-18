using UnityEngine;

public class EnemyAttackState : IEnemyState
{
    private float attackCooldown = 1.5f; // Quái đánh 1.5 giây 1 lần
    private float lastAttackTime;

    public void EnterState(EnemyBase enemy)
    {
        lastAttackTime = Time.time - attackCooldown;
        enemy.RB.linearVelocity = new Vector2(0, enemy.RB.linearVelocity.y);
        enemy.Anim.SetBool("isMoving", false);
    }

    public void UpdateState(EnemyBase enemy)
    {
        // Luôn luôn nhìn theo người chơi dù đang đứng yên
        enemy.FaceTarget(enemy.PlayerTarget.position.x);

        // 1. Logic hành động (Tấn công)
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            PerformAttack(enemy);
            lastAttackTime = Time.time;
        }

        // 2. Logic chuyển trạng thái
        // Nếu người chơi Lướt (Dash) hoặc chạy thoát khỏi tầm đánh -> Lại đổi sang Chase đuổi tiếp
        if (enemy.DistanceToPlayer() > enemy.AttackRange)
        {
            enemy.ChangeState(enemy.chaseState);
        }
    }

    private void PerformAttack(EnemyBase enemy)
    {
        Debug.Log($"<color=red>{enemy.gameObject.name} vung vũ khí tấn công!</color>");

        enemy.Anim.SetTrigger("Attack");

        // Quét một vòng tròn quanh quái vật bằng với Tầm Đánh (AttackRange)
        Collider2D[] hits = Physics2D.OverlapCircleAll(enemy.transform.position, enemy.AttackRange);

        foreach (Collider2D hit in hits)
        {
            // Kiểm tra xem thứ bị chém trúng có phải là Player không
            if (hit.CompareTag("Player"))
            {
                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    // Tính hướng đẩy lùi (Từ quái hất về phía người chơi)
                    Vector2 knockbackDir = (hit.transform.position - enemy.transform.position).normalized;
                    knockbackDir.y = 0.5f; // Hất bổng nhẹ lên không trung

                    int enemyDamage = 15; // (Bạn có thể chuyển biến này sang EnemyBase để chỉnh sửa cho từng quái)
                    damageable.TakeDamage(enemyDamage, knockbackDir);
                }
            }
        }
    }

    public void ExitState(EnemyBase enemy)
    {
        // Thoát khỏi trạng thái tấn công
    }
}
