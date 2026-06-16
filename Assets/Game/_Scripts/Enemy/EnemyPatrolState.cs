using UnityEngine;

public class EnemyPatrolState : IEnemyState
{
    private Vector2 moveDirection = Vector2.right; // Bắt đầu bằng việc đi sang phải
    private float patrolTimer;
    private float timeToTurn = 3f; // Cứ 3 giây thì quay đầu 1 lần

    public void EnterState(EnemyBase enemy)
    {
        // Có thể gọi: enemy.Anim.SetBool("IsMoving", true);
        patrolTimer = timeToTurn;
        enemy.FaceTarget(enemy.transform.position.x + moveDirection.x);
    }

    public void UpdateState(EnemyBase enemy)
    {
        // 1. Logic chuyển trạng thái (Ưu tiên kiểm tra trước)
        if (enemy.DistanceToPlayer() <= enemy.SightRange)
        {
            enemy.ChangeState(enemy.chaseState);
            return;
        }

        // 2. Logic hành động (Đi tuần)
        enemy.RB.linearVelocity = new Vector2(moveDirection.x * enemy.PatrolSpeed, enemy.RB.linearVelocity.y);

        // Đếm thời gian để quay đầu
        patrolTimer -= Time.deltaTime;
        if (patrolTimer <= 0f)
        {
            moveDirection.x *= -1; // Đảo chiều X
            enemy.FaceTarget(enemy.transform.position.x + moveDirection.x);
            patrolTimer = timeToTurn; // Reset timer
        }

        // 💡 Mẹo: Nếu muốn làm AI xịn hơn, sau này bạn có thể thay Timer bằng 
        // Physics2D.Raycast để check đụng tường hoặc check mép vực để tự quay đầu.
    }

    public void ExitState(EnemyBase enemy)
    {
        // Dọn dẹp vận tốc khi bị đánh động và chuyển sang rượt đuổi
        enemy.RB.linearVelocity = new Vector2(0, enemy.RB.linearVelocity.y);
        // enemy.Anim.SetBool("IsMoving", false);
    }
}