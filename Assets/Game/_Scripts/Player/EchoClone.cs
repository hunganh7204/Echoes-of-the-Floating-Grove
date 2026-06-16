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
    [SerializeField] private int attackDamage = 10; // Có thể cho bóng đánh yếu hơn người thật (VD: 5)

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

            // 1. Lặp lại vị trí và hướng xoay (Quay đầu, Đảo trọng lực)
            transform.position = frame.position;
            transform.rotation = frame.rotation;

            // 2. Lặp lại chính xác frame Animation đang chạy
            if (anim != null)
            {
                anim.Play(frame.animNameHash, 0, frame.animNormalizedTime);
            }

            // 3. Nếu frame này Player có bấm chém -> Bóng cũng vung kiếm gây sát thương
            if (frame.isAttacking)
            {
                PerformAttack();
            }

            // 4. Nếu frame này Player có bấm tương tác -> Bóng cũng check xung quanh
            if (frame.isInteracting)
            {
                PerformInteract();
            }

            currentFrameIndex++;
        }
        else
        {
            Destroy(gameObject); // Chạy hết thì biến mất
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
                // Hất tung quái vật giống hệt người thật
                Vector2 knockbackDir = (enemy.transform.position - transform.position).normalized;
                knockbackDir.y = 0.5f;
                damageable.TakeDamage(attackDamage, knockbackDir);
            }
        }
    }

    private void PerformInteract()
    {
        // Tạo 1 vòng quét xung quanh bóng để tìm NPC hoặc Cửa, Rương
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, interactRange);
        foreach (Collider2D obj in hitObjects)
        {
            IInteractable interactable = obj.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
                break; // Chỉ tương tác với 1 vật gần nhất
            }
        }
    }

    // Vẽ vòng đỏ/xanh trên Scene để bạn dễ căn chỉnh tầm đánh của bóng
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