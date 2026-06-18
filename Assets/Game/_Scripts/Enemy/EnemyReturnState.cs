using UnityEngine;

public class EnemyReturnState : IEnemyState
{
    public void EnterState(EnemyBase enemy)
    {
        enemy.Anim.ResetTrigger("Attack");
        enemy.Anim.SetBool("isMoving", true);
        Debug.Log($"<color=white>{enemy.gameObject.name} mất dấu mục tiêu! Quay về vị trí cũ.</color>");
    }

    public void UpdateState(EnemyBase enemy)
    {
        // 1. Nếu trên đường về mà lại thấy người chơi -> Quay lại đuổi tiếp
        if (enemy.DistanceToPlayer() <= enemy.SightRange)
        {
            enemy.ChangeState(enemy.chaseState);
            return;
        }

        // 2. Kiểm tra xem đã về đến điểm ban đầu chưa (chỉ tính khoảng cách trục X cho quái bộ)
        float distanceToSpawn = Mathf.Abs(enemy.SpawnPoint.x - enemy.transform.position.x);

        if (distanceToSpawn <= 0.1f) // Sai số nhỏ để tránh lỗi kẹt vị trí
        {
            enemy.ChangeState(enemy.patrolState); // Đã về đến nơi -> Đi tuần tra lại
            return;
        }

        // 3. Di chuyển về vị trí ban đầu
        float dirX = Mathf.Sign(enemy.SpawnPoint.x - enemy.transform.position.x); // Lấy hướng: 1 (phải) hoặc -1 (trái)

        enemy.FaceTarget(enemy.transform.position.x + dirX);
        enemy.RB.linearVelocity = new Vector2(dirX * enemy.PatrolSpeed, enemy.RB.linearVelocity.y);
    }

    public void ExitState(EnemyBase enemy)
    {
        enemy.RB.linearVelocity = new Vector2(0, enemy.RB.linearVelocity.y);
        enemy.Anim.SetBool("isMoving", false);
    }
}