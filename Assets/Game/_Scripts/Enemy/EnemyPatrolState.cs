using UnityEngine;

public class EnemyPatrolState : IEnemyState
{
    private Vector2 moveDirection = Vector2.right; 
    private float patrolTimer;
    private float timeToTurn = 3f; 

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

        enemy.RB.linearVelocity = new Vector2(moveDirection.x * enemy.PatrolSpeed, enemy.RB.linearVelocity.y);

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
        enemy.RB.linearVelocity = new Vector2(0, enemy.RB.linearVelocity.y);
        enemy.Anim.SetBool("isMoving", false);
    }
}