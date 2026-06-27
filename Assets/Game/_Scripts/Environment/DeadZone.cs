using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DeadZone : MonoBehaviour
{
    private void Start()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("<color=red>Player đã rơi vào Deadzone!</color>");

            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(99999);
            }
            else
            {
                collision.gameObject.SetActive(false);
                if (UIManager.Instance != null) UIManager.Instance.ShowGameResult(false);
            }
        }
        else
        {
            collision.gameObject.SetActive(false);
        }
    }
}