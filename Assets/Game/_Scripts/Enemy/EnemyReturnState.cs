using UnityEngine;

public class EnemyReturnState : IEnemyState
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

        float distanceToSpawn = Mathf.Abs(enemy.SpawnPoint.x - enemy.transform.position.x);

        if (distanceToSpawn <= 0.1f) 
        {
            enemy.ChangeState(enemy.patrolState); 
            return;
        }

        float dirX = Mathf.Sign(enemy.SpawnPoint.x - enemy.transform.position.x); 

        enemy.FaceTarget(enemy.transform.position.x + dirX);
        enemy.RB.linearVelocity = new Vector2(dirX * enemy.PatrolSpeed, enemy.RB.linearVelocity.y);
    }

    public void ExitState(EnemyBase enemy)
    {
        enemy.RB.linearVelocity = new Vector2(0, enemy.RB.linearVelocity.y);
        enemy.Anim.SetBool("isMoving", false);
    }
}