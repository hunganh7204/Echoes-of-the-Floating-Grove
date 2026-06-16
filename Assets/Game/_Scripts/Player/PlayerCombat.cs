using System;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 0.5f;

    [Header("Hitbox")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.8f;
    [SerializeField] private LayerMask enemyLayers;

    private float nextAttackTime = 0f;
    [SerializeField] private Animator anim;

    private void Start()
    {
        //anim = GetComponent<Animator>();
        //if (anim == null) Debug.LogError("PlayerCombat: No Animator component found on player!");
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time >= nextAttackTime)
        {
            if(InputManager.Instance.AttackPressed)
            {
                Attack();
                Debug.Log("Player attacked!");
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    private void Attack()
    {
        if (anim != null) anim.SetTrigger("2_Attack");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            IDamageable damageable = enemy.GetComponent<IDamageable>();
            if (damageable != null)
            {
                Vector2 knockbackDir = (enemy.transform.position - transform.position).normalized;
                knockbackDir.y = 0.5f;
                damageable.TakeDamage(attackDamage, knockbackDir);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
