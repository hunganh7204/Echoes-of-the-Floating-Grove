using UnityEngine;
using System.Collections.Generic;

public class EchoClone : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator anim;
    [SerializeField] private Transform attackPoint;

    [Header("Clone Combat Settings")]
    [SerializeField] private float attackRange = 0.8f;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private int attackDamage = 10;

    [Header("Clone Interact Settings")]
    [SerializeField] private float interactRange = 1.5f;

    private List<EchoFrame> framesToPlay;
    private int currentFrameIndex = 0;

    public void InitData(List<EchoFrame> data)
    {
        framesToPlay = data;
        currentFrameIndex = 0;
    }

    private void FixedUpdate()
    {
        if (framesToPlay == null || framesToPlay.Count == 0) return;

        if (currentFrameIndex < framesToPlay.Count)
        {
            EchoFrame frame = framesToPlay[currentFrameIndex];

            transform.position = frame.position;
            transform.rotation = frame.rotation;

            if (anim != null)
            {
                anim.Play(frame.animNameHash, 0, frame.animNormalizedTime);
            }

            if (frame.isAttacking)
            {
                PerformAttack();
            }

            if (frame.isInteracting)
            {
                PerformInteract();
            }

            currentFrameIndex++;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void PerformAttack()
    {
        if (attackPoint == null) return;

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

    private void PerformInteract()
    {
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, interactRange);
        foreach (Collider2D obj in hitObjects)
        {
            IInteractable interactable = obj.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
                break; 
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}