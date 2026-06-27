using UnityEngine;

public class EnemyAttackState : IEnemyState
{
    private float attackCooldown = 1.5f; 
    private float lastAttackTime;

    public void EnterState(EnemyBase enemy)
    {
        lastAttackTime = Time.time - attackCooldown;
        enemy.RB.linearVelocity = new Vector2(0, enemy.RB.linearVelocity.y);
        enemy.Anim.SetBool("isMoving", false);
    }

    public void UpdateState(EnemyBase enemy)
    {
        enemy.FaceTarget(enemy.PlayerTarget.position.x);

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            PerformAttack(enemy);
            lastAttackTime = Time.time;
        }
        if (enemy.DistanceToPlayer() > enemy.AttackRange)
        {
            enemy.ChangeState(enemy.chaseState);
        }
    }

    private void PerformAttack(EnemyBase enemy)
    {
        Debug.Log($"<color=red>{enemy.gameObject.name} vung vũ khí tấn công!</color>");

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
    }

    public void ExitState(EnemyBase enemy)
    {
    }
}
