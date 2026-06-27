using UnityEngine;

public class FlyingAttackState : IEnemyState
{
    private float attackCooldown = 1.5f;
    private float timer;

    [Header("Hit & Run Settings")]
    private float flyUpDuration = 0.5f;  
    private float flyUpSpeed = 3f;       
    private float diveSpeed = 6f;        

    private enum AttackPhase { Retreat, Hover, Swoop }
    private AttackPhase currentPhase;

    public void EnterState(EnemyBase enemy)
    {
        PerformAttack(enemy);
    }

    public void UpdateState(EnemyBase enemy)
    {
        enemy.FaceTarget(enemy.PlayerTarget.position.x);

        if (enemy.DistanceToPlayer() > enemy.SightRange)
        {
            enemy.ChangeState(enemy.returnState);
            return;
        }

        timer += Time.deltaTime;

        switch (currentPhase)
        {
            case AttackPhase.Retreat:
                enemy.RB.linearVelocity = Vector2.up * flyUpSpeed;

                if (timer >= flyUpDuration)
                {
                    currentPhase = AttackPhase.Hover; 
                }
                break;

            case AttackPhase.Hover:
                enemy.RB.linearVelocity = Vector2.zero;

                if (timer >= attackCooldown)
                {
                    currentPhase = AttackPhase.Swoop; 
                }
                break;

            case AttackPhase.Swoop:
                Vector2 dir = (enemy.PlayerTarget.position - enemy.transform.position).normalized;
                enemy.RB.linearVelocity = dir * diveSpeed;

                if (enemy.DistanceToPlayer() <= enemy.AttackRange)
                {
                    PerformAttack(enemy);
                }
                break;
        }
    }

    private void PerformAttack(EnemyBase enemy)
    {

        enemy.Anim.SetTrigger("Attack");

        Collider2D[] hits = Physics2D.OverlapCircleAll(enemy.transform.position, enemy.AttackRange);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    Vector2 knockbackDir = (hit.transform.position - enemy.transform.position).normalized;
                    knockbackDir.y = 0.5f;

                    int enemyDamage = 15;
                    damageable.TakeDamage(enemyDamage, knockbackDir);
                }
            }
        }
        timer = 0f;
        currentPhase = AttackPhase.Retreat;
    }

    public void ExitState(EnemyBase enemy)
    {
        enemy.RB.linearVelocity = Vector2.zero;
    }
}