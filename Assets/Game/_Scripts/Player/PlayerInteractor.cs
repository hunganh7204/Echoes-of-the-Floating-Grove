using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    private IInteractable currentInteractable;

    private void Update()
    {
        if (InputManager.Instance.InteractPressed && currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IInteractable interactableObject = collision.GetComponent<IInteractable>();

        if (interactableObject != null)
        {
            currentInteractable = interactableObject;
            currentInteractable.ShowInteractionPrompt();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (currentInteractable != null)
        {
            IInteractable interactableObject = collision.GetComponent<IInteractable>();
            if (interactableObject == currentInteractable)
            {
                currentInteractable.HideInteractionPrompt(); 
                currentInteractable = null;
            }
        }
    }
}
