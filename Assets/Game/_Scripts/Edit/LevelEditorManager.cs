using UnityEngine;
using UnityEngine.EventSystems;

public class LevelEditorManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LevelGenerator levelGenerator;
    [SerializeField] private GameObject testPlayer;

    [Header("Visuals")]
    [SerializeField] private Transform hoverCursor;

    [Header("Settings")]
    [SerializeField] private Vector2Int mapSize = new Vector2Int(50, 30);
    [SerializeField] private float cameraMoveSpeed = 15f;
    [SerializeField] private float cameraZoomSpeed = 2f;

    public string levelNameToLoad = "Level_1";

    [Header("Cameras")]
    [SerializeField] private Camera editorCamera;
    [SerializeField] private GameObject gameplayCamera;

    private int currentBrush = 1;
    private bool isTesting = false;

    public bool IsTesting => isTesting;

    void Start()
    {
        testPlayer.SetActive(false);
        InitializeBlankMap();
        CenterCameraToGrid();
    }

    private void CenterCameraToGrid()
    {
        if (editorCamera != null)
        {
            float centerX = (mapSize.x * levelGenerator.tileSize) / 2f;
            float centerY = (mapSize.y * levelGenerator.tileSize) / 2f;
            editorCamera.transform.position = new Vector3(centerX, centerY, -10f);
        }
    }

    public void InitializeBlankMap()
    {
        DataManager.Instance.CreateBlankMapData(mapSize.x, mapSize.y);
        levelGenerator.BuildLevelFromData(DataManager.Instance.CurrentEditingData);
    }

    public void LoadMapFromFile()
    {
        LevelData data = DataManager.Instance.LoadMapForEditing(levelNameToLoad);
        if (data != null)
        {
            levelGenerator.BuildLevelFromData(data);
            Debug.Log("Editor: Đã hiển thị map " + levelNameToLoad);
        }
    }

    void Update()
    {
        if (isTesting) return;

        HandleCameraMovement();

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            if (hoverCursor != null) hoverCursor.gameObject.SetActive(false);
            return;
        }

        Vector3 mouseWorldPos = editorCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        int x = Mathf.RoundToInt(mouseWorldPos.x / levelGenerator.tileSize);
        int y = Mathf.RoundToInt(mouseWorldPos.y / levelGenerator.tileSize);

        if (x >= 0 && x < mapSize.x && y >= 0 && y < mapSize.y)
        {
            if (hoverCursor != null)
            {
                hoverCursor.gameObject.SetActive(true);
                hoverCursor.position = new Vector3(x * levelGenerator.tileSize, y * levelGenerator.tileSize, 0f);
            }

            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                PaintTile(x, y, Input.GetMouseButton(1));
            }
        }
        else
        {
            if (hoverCursor != null) hoverCursor.gameObject.SetActive(false);
        }
    }

    private void HandleCameraMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        editorCamera.transform.position += new Vector3(h, v, 0) * cameraMoveSpeed * Time.deltaTime;

        if (Input.GetMouseButton(2))
        {
            float mouseX = Input.GetAxisRaw("Mouse X");
            float mouseY = Input.GetAxisRaw("Mouse Y");
            editorCamera.transform.position -= new Vector3(mouseX, mouseY, 0) * cameraMoveSpeed * Time.deltaTime;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            editorCamera.orthographicSize -= scroll * cameraZoomSpeed;
            editorCamera.orthographicSize = Mathf.Clamp(editorCamera.orthographicSize, 2f, 20f);
        }
    }

    private void PaintTile(int x, int y, bool isErase)
    {
        int targetBrush = isErase ? 99 : currentBrush;

        bool isChanged = DataManager.Instance.UpdateTileData(x, y, targetBrush);

        if (isChanged)
        {
            levelGenerator.BuildLevelFromData(DataManager.Instance.CurrentEditingData);
        }
    }

    public void SetBrushType(int brushType)
    {
        currentBrush = brushType;
    }

    public void ToggleTestPlay()
    {
        if (!isTesting && !levelGenerator.HasStartPosition())
        {
            Debug.LogWarning("Hệ thống: Vui lòng đặt Tile 'Điểm Bắt Đầu' trước khi Test!");
            return;
        }

        isTesting = !isTesting;
        if (isTesting)
        {
            if (hoverCursor != null) hoverCursor.gameObject.SetActive(false);

            editorCamera.gameObject.SetActive(false);
            gameplayCamera.SetActive(true);

            testPlayer.transform.position = levelGenerator.GetStartPosition();

            Rigidbody2D rb = testPlayer.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;

            testPlayer.SetActive(true);
        }
        else
        {
            editorCamera.gameObject.SetActive(true);
            gameplayCamera.SetActive(false);
            testPlayer.SetActive(false);

            levelGenerator.BuildLevelFromData(DataManager.Instance.CurrentEditingData);
        }
    }

    public void SaveMap()
    {
        DataManager.Instance.SaveCurrentEditedMap();
    }
}