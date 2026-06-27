using UnityEngine;

public class FlyingPatrolState : IEnemyState
{
    private Vector2 moveDirection = Vector2.right; 
    private float patrolTimer;
    private float timeToTurn = 2f; 

    public void EnterState(EnemyBase enemy)
    {
        enemy.Anim.ResetTrigger("Attack");
        enemy.Anim.SetBool("isMoving", true);
        patrolTimer = timeToTurn;
        enemy.FaceTarget(enemy.transform.position.x + moveDirection.x);
    }

    public void UpdateState(EnemyBase enemy)
    {
        if (enemy.DistanceToPlayer() <= enemy.SightRange)
        {
            enemy.ChangeState(enemy.chaseState);
            return;
        }

        enemy.RB.linearVelocity = new Vector2(moveDirection.x * enemy.PatrolSpeed, 0f);

        patrolTimer -= Time.deltaTime;
        if (patrolTimer <= 0f)
        {
            moveDirection.x *= -1; 
            enemy.FaceTarget(enemy.transform.position.x + moveDirection.x);
            patrolTimer = timeToTurn;
        }
    }

    public void ExitState(EnemyBase enemy)
    {
        enemy.RB.linearVelocity = Vector2.zero;
        enemy.Anim.SetBool("isMoving", false);
    }
}