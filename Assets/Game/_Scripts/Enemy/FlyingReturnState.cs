using UnityEngine;

public class FlyingReturnState : IEnemyState
{
    public void EnterState(EnemyBase enemy)
    {
        enemy.Anim.ResetTrigger("Attack");
        enemy.Anim.SetBool("isMoving", true);
    }

    public void UpdateState(EnemyBase enemy)
    {
        if (enemy.DistanceToPlayer() <= enemy.SightRange)
        {
            enemy.ChangeState(enemy.chaseState);
            return;
        }

        float distanceToSpawn = Vector2.Distance(enemy.transform.position, enemy.SpawnPoint);

        if (distanceToSpawn <= 0.1f)
        {
            enemy.ChangeState(enemy.patrolState);
            return;
        }

        Vector2 dir = (enemy.SpawnPoint - (Vector2)enemy.transform.position).normalized;
        enemy.FaceTarget(enemy.transform.position.x + dir.x);
        enemy.RB.linearVelocity = dir * enemy.PatrolSpeed;
    }

    public void ExitState(EnemyBase enemy)
    {
        enemy.RB.linearVelocity = Vector2.zero;
        enemy.Anim.SetBool("isMoving", false);
    }
}