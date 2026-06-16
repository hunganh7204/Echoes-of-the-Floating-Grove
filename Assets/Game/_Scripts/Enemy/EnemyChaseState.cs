using UnityEngine;

public class EnemyChaseState : IEnemyState
{
    public void EnterState(EnemyBase enemy)
    {
        // enemy.Anim.SetBool("IsChasing", true);
        Debug.Log($"<color=yellow>{enemy.gameObject.name} phát hiện mục tiêu! Bắt đầu truy đuổi.</color>");
    }

    public void UpdateState(EnemyBase enemy)
    {
        float dist = enemy.DistanceToPlayer();

        // 1. Logic chuyển trạng thái
        if (dist > enemy.SightRange)
        {
            enemy.ChangeState(enemy.patrolState); // Mục tiêu chạy quá xa -> Quay về đi tuần
            return;
        }
        if (dist <= enemy.AttackRange)
        {
            enemy.ChangeState(enemy.attackState); // Tiến vào tầm chém -> Đổi sang State Tấn công
            return;
        }

        // 2. Logic hành động (Truy đuổi)
        enemy.FaceTarget(enemy.PlayerTarget.position.x);

        // Tính toán hướng đi tới người chơi (chỉ lấy trục X)
        Vector2 direction = (enemy.PlayerTarget.position - enemy.transform.position).normalized;
        enemy.RB.linearVelocity = new Vector2(direction.x * enemy.ChaseSpeed, enemy.RB.linearVelocity.y);
    }

    public void ExitState(EnemyBase enemy)
    {
        // Dừng lại khi mất dấu hoặc khi chuẩn bị ra đòn
        enemy.RB.linearVelocity = new Vector2(0, enemy.RB.linearVelocity.y);
        // enemy.Anim.SetBool("IsChasing", false);
    }
}