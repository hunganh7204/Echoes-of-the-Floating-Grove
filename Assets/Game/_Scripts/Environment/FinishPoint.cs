using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class FinishPoint : MonoBehaviour
{
    private void Start()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("<color=green>Player đã chạm đích! Hoàn thành màn chơi.</color>");

            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowGameResult(true);
            }
        }
    }
}