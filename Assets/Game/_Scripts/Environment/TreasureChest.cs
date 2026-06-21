using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class TreasureChest : MonoBehaviour, IInteractable
{
        [Header("References")]
        [SerializeField] private Animator anim;
        private bool isOpened = false;
        private SpriteRenderer spriteRenderer;
        private BoxCollider2D triggerCollider;

    [Header("Rewards")]
    [Tooltip("Số điểm hoặc tiền nhận được khi mở rương này")]
    [SerializeField] private int rewardPoints = 100;

    private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            triggerCollider = GetComponent<BoxCollider2D>();

            triggerCollider.isTrigger = true;
        }

    private void OnEnable()
    {
        isOpened = false;

        if (triggerCollider != null) triggerCollider.enabled = true;

    }

    public void Interact()
        {
            if (isOpened) return; 

            isOpened = true;
            if (anim != null)
                {
                     anim.SetTrigger("Open");
                }
            ScoreManager.Instance.AddScore(rewardPoints);

            HideInteractionPrompt();

            triggerCollider.enabled = false;
        }

        public void ShowInteractionPrompt()
        {
            if (isOpened) return;

            if (InteractionPromptUI.Instance != null)
                InteractionPromptUI.Instance.ShowPrompt("Mở rương", transform);
    }

        public void HideInteractionPrompt()
        {
        if (InteractionPromptUI.Instance != null)
            InteractionPromptUI.Instance.HidePrompt();
    }
 }

