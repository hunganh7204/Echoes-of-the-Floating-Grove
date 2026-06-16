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
        // Nếu là Player rớt xuống
        if (collision.CompareTag("Player"))
        {
            Debug.Log("<color=red>Player đã rơi vào Deadzone!</color>");

            // Ép chết ngay lập tức bằng cách truyền vào một lượng sát thương khổng lồ
            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(99999);
            }
            else
            {
                // Phòng hờ nếu Player mất script máu, vẫn ép thua
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