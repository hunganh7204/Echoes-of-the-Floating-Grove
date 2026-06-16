using UnityEngine;
using System.Collections;

public class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField] private int maxHealth = 50;
    private int currentHealth;

    [Header("AI Settings")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float sightRange = 6f;
    [SerializeField] private float attackRange = 1.5f;

    [Header("References (Kéo thả từ Inspector)")]
    [SerializeField] private Transform playerTarget;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Rewards")]
    [SerializeField] private int scoreValue = 10; // Điểm nhận được khi giết quái này

    // FSM Core (Cầm giữ State hiện tại)
    protected IEnemyState currentState;

    // Khởi tạo sẵn các trạng thái
    public EnemyPatrolState patrolState = new EnemyPatrolState();
    public EnemyChaseState chaseState = new EnemyChaseState();
    public EnemyAttackState attackState = new EnemyAttackState();

    public float PatrolSpeed => patrolSpeed;
    public float ChaseSpeed => chaseSpeed;
    public float SightRange => sightRange;
    public float AttackRange => attackRange;
    public Transform PlayerTarget => playerTarget;
    public Rigidbody2D RB => rb;
    public Animator Anim => anim;

    private Color originalColor;

    protected virtual void Start()
    {
        if (spriteRenderer != null) originalColor = spriteRenderer.color;

        currentHealth = maxHealth;
        playerTarget = PlayerController.Instance;
        ChangeState(patrolState);
    }

    protected virtual void OnEnable()
    {
        currentHealth = maxHealth;

        if (spriteRenderer != null && originalColor != default)
        {
            spriteRenderer.color = originalColor;
        }

        if (patrolState != null)
        {
            ChangeState(patrolState);
        }
    }

    protected virtual void Update()
    {
        if (playerTarget == null) return;
        if (currentState != null) currentState.UpdateState(this);
    }

    public void ChangeState(IEnemyState newState)
    {
        if (currentState != null) currentState.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }

    public virtual void TakeDamage(int damageAmount, Vector2 knockbackDirection = default)
    {
        currentHealth -= damageAmount;
        Debug.Log($"<color=orange>{gameObject.name} mất {damageAmount} máu!</color>");

        if (spriteRenderer != null) StartCoroutine(FlashRedEffect());

        if (knockbackDirection != default)
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(knockbackDirection * 5f, ForceMode2D.Impulse);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            if (currentState == patrolState)
            {
                ChangeState(chaseState);
            }
        }
    }

    public void SetPlayerTarget(Transform target)
    {
        playerTarget = target;
    }

    private IEnumerator FlashRedEffect()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        spriteRenderer.color = originalColor;
    }

    protected virtual void Die()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(scoreValue);
        }

        Debug.Log($"<color=red><b>{gameObject.name} đã chết! +{scoreValue} điểm</b></color>");
        gameObject.SetActive(false);
    }
    public float DistanceToPlayer()
    {
        return Vector2.Distance(transform.position, playerTarget.position);
    }

    public void FaceTarget(float targetX)
    {
        if (targetX > transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else if (targetX < transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}