using UnityEngine;

public class FlyingChaseState : IEnemyState
{
    public void EnterState(EnemyBase enemy)
    {
        enemy.Anim.ResetTrigger("Attack");
        enemy.Anim.SetBool("isMoving", true);
        Debug.Log($"<color=cyan>{enemy.gameObject.name} bay tới mục tiêu!</color>");
    }

    public void UpdateState(EnemyBase enemy)
    {
        float dist = enemy.DistanceToPlayer();

        if (dist > enemy.SightRange)
        {
            enemy.ChangeState(enemy.returnState);
            return;
        }
        if (dist <= enemy.AttackRange)
        {
            enemy.ChangeState(enemy.attackState);
            return;
        }

        enemy.FaceTarget(enemy.PlayerTarget.position.x);

        // Lấy hướng bay thẳng đến chỗ Player (tính cả X và Y)
        Vector2 direction = (enemy.PlayerTarget.position - enemy.transform.position).normalized;
        enemy.RB.linearVelocity = direction * enemy.ChaseSpeed;
    }

    public void ExitState(EnemyBase enemy)
    {
        enemy.RB.linearVelocity = Vector2.zero;
        enemy.Anim.SetBool("isMoving", false);
    }
}