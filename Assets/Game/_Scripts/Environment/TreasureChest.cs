using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class TreasureChest : MonoBehaviour, IInteractable
{
        private bool isOpened = false;
        private SpriteRenderer spriteRenderer;
        private BoxCollider2D triggerCollider;

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            triggerCollider = GetComponent<BoxCollider2D>();

            triggerCollider.isTrigger = true;
        }

        public void Interact()
        {
            if (isOpened) return; 

            isOpened = true;
            spriteRenderer.color = Color.gray;
            Debug.Log("<color=yellow>Nhận được: 100 Vàng & Chìa khóa Boss!</color>");

            HideInteractionPrompt();

            triggerCollider.enabled = false;
        }

        public void ShowInteractionPrompt()
        {
            if (!isOpened)
            {
                spriteRenderer.color = Color.yellow;
                Debug.Log("UI: Nhấn [F] để Mở Rương");
            }
        }

        public void HideInteractionPrompt()
        {
            if (!isOpened)
            {
                spriteRenderer.color = Color.white;
            }
            Debug.Log("UI: Ẩn nhắc nhở Mở Rương");
        }
 }

