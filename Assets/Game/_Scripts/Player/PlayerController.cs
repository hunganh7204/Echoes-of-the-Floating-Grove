using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static Transform Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this.transform)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this.transform;
    }
}