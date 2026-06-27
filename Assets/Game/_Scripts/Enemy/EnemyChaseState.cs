using UnityEngine;

public class EnemyChaseState : IEnemyState
{
    public void EnterState(EnemyBase enemy)
    {
        enemy.Anim.ResetTrigger("Attack");
        enemy.Anim.SetBool("isMoving", true);

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

        Vector2 direction = (enemy.PlayerTarget.position - enemy.transform.position).normalized;
        enemy.RB.linearVelocity = new Vector2(direction.x * enemy.ChaseSpeed, enemy.RB.linearVelocity.y);
    }

    public void ExitState(EnemyBase enemy)
    {
        enemy.RB.linearVelocity = new Vector2(0, enemy.RB.linearVelocity.y);
        enemy.Anim.SetBool("isMoving", false);
    }
}