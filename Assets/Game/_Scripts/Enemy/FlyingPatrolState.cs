using UnityEngine;

public class FlyingPatrolState : IEnemyState
{
    private Vector2 moveDirection = Vector2.right; // Bắt đầu bằng việc bay sang phải
    private float patrolTimer;
    private float timeToTurn = 2f; // Cứ 2 giây thì quay đầu bay ngược lại 1 lần

    public void EnterState(EnemyBase enemy)
    {
        enemy.Anim.ResetTrigger("Attack");
        enemy.Anim.SetBool("isMoving", true);
        patrolTimer = timeToTurn;
        enemy.FaceTarget(enemy.transform.position.x + moveDirection.x);
    }

    public void UpdateState(EnemyBase enemy)
    {
        // Quét thấy người chơi -> Truy đuổi
        if (enemy.DistanceToPlayer() <= enemy.SightRange)
        {
            enemy.ChangeState(enemy.chaseState);
            return;
        }

        // Bay ngang: Trục X dùng tốc độ di chuyển, Trục Y giữ bằng 0 để bay trên một đường thẳng
        enemy.RB.linearVelocity = new Vector2(moveDirection.x * enemy.PatrolSpeed, 0f);

        // Đếm ngược để đảo chiều
        patrolTimer -= Time.deltaTime;
        if (patrolTimer <= 0f)
        {
            moveDirection.x *= -1; // Đảo ngược hướng X (phải thành trái, trái thành phải)
            enemy.FaceTarget(enemy.transform.position.x + moveDirection.x);
            patrolTimer = timeToTurn;
        }
    }

    public void ExitState(EnemyBase enemy)
    {
        // Khi thoát trạng thái bay tuần tra thì dừng lại
        enemy.RB.linearVelocity = Vector2.zero;
        enemy.Anim.SetBool("isMoving", false);
    }
}