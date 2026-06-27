using UnityEngine;
using System.Collections; // Bắt buộc phải có để dùng Coroutine
using System.IO;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameResultPanel;
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameResultUI gameResultUIScript;
    [SerializeField] private GameObject gameplayHUD;

    [Header("UI Transition Settings")]
    [SerializeField] private float fadeDuration = 0.25f; 

    private GameObject currentActivePanel;
    private GameObject previousPanel;

    [Header("Gameplay References")]
    [SerializeField] private GameObject player;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        CloseAllPanelsInstant(); 
        ShowMainMenu();
    }

    private void Update()
    {
        if (InputManager.Instance.PausePressed)
        {     
            if (DialogueManager.IsChatting)
            {
                DialogueManager.Instance.EndDialogue();
                return;
            }
            if (currentActivePanel == settingsPanel)
            {
                CloseSettings();
            }
            else if (currentActivePanel == null || currentActivePanel == pauseMenuPanel)
            {
                TogglePauseMenu();
            }
        }
    }



    public void ShowMainMenu()
    {
        TransitionToPanel(mainMenuPanel);
        if (gameplayHUD != null && gameplayHUD.activeSelf) StartCoroutine(FadeCanvasGroup(gameplayHUD, 0f, false));
        Time.timeScale = 0f;
    }

    public void StartGameplay()
    {
        TransitionToPanel(null); 
        if (gameplayHUD != null) StartCoroutine(FadeCanvasGroup(gameplayHUD, 1f, true));
        Time.timeScale = 1f;
    }

    public void ShowGameResult(bool isVictory)
    {
        TransitionToPanel(gameResultPanel);
        if (gameplayHUD != null && gameplayHUD.activeSelf) StartCoroutine(FadeCanvasGroup(gameplayHUD, 0f, false));

        int finalScore = ScoreManager.Instance != null ? ScoreManager.Instance.CurrentScore : 0;
        int currentLevel = LevelGenerator.Instance != null ? LevelGenerator.Instance.CurrentLevel : 1;

        if (gameResultUIScript != null) gameResultUIScript.SetupPanel(isVictory, currentLevel, finalScore);

        Time.timeScale = 0f;
    }

    public void TogglePauseMenu()
    {
        if (currentActivePanel == pauseMenuPanel)
        {
            StartGameplay();
        }
        else
        {
            TransitionToPanel(pauseMenuPanel);
            if (gameplayHUD != null && gameplayHUD.activeSelf) StartCoroutine(FadeCanvasGroup(gameplayHUD, 0f, false));
            Time.timeScale = 0f;
        }
    }

    public void OpenSettings()
    {
        previousPanel = currentActivePanel;
        TransitionToPanel(settingsPanel);
    }

    public void CloseSettings()
    {
        if (previousPanel != null) TransitionToPanel(previousPanel);
        else ShowMainMenu();
    }

    public void ReturnToMainMenu()
    {
        if (LevelGenerator.Instance != null) LevelGenerator.Instance.ClearMapToPool();
        if (player != null) player.SetActive(false);
        ShowMainMenu();
    }



    private void TransitionToPanel(GameObject newPanel)
    {
        if (currentActivePanel == newPanel) return;

        if (currentActivePanel != null)
        {
            StartCoroutine(FadeCanvasGroup(currentActivePanel, 0f, false));
        }

        currentActivePanel = newPanel;

        if (currentActivePanel != null)
        {
            StartCoroutine(FadeCanvasGroup(currentActivePanel, 1f, true));
        }
    }

    private IEnumerator FadeCanvasGroup(GameObject panel, float targetAlpha, bool stateAtEnd)
    {
        if (panel == null) yield break;

        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null) cg = panel.AddComponent<CanvasGroup>();

        if (stateAtEnd)
        {
            panel.SetActive(true);
            if (cg.alpha > 0.9f) cg.alpha = 0f; 
        }

        float startAlpha = cg.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            yield return null;
        }

        cg.alpha = targetAlpha;

        if (!stateAtEnd) panel.SetActive(false); 
    }

    private void CloseAllPanelsInstant()
    {
        mainMenuPanel.SetActive(false);
        gameResultPanel.SetActive(false);
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        if (gameplayHUD != null) gameplayHUD.SetActive(false);
        currentActivePanel = null;
    }


    public void StartNewGameFromMenu()
    {
        if (LevelGenerator.Instance != null) LevelGenerator.Instance.ClearMapToPool();
        if (ScoreManager.Instance != null) ScoreManager.Instance.ResetScore();
        ResetPlayerState();
        StartGameplay();
        if (LevelGenerator.Instance != null) LevelGenerator.Instance.LoadLevelAndSpawnPlayer(1);
    }

    public void ContinueGameFromMenu()
    {
        PlayerData savedData = DataManager.Instance.LoadGame();
        if (savedData != null)
        {
            StartGameplay();
            if (ScoreManager.Instance != null) ScoreManager.Instance.ResetScore();
            Vector2 checkpointPos = new Vector2(savedData.positionX, savedData.positionY);
            LevelGenerator.Instance.LoadLevelAndSpawnPlayer(savedData.currentLevel, checkpointPos);

            ResetPlayerState();

            var stats = player.GetComponent<PlayerStats>();
            if (stats != null)
            {
                stats.LoadSavedStats(savedData.currentHealth, savedData.currentMana);
            }
        }
        else StartNewGameFromMenu();
    }

    public void RestartLevel(bool useCheckpoint)
    {
        StartGameplay();
        if (ScoreManager.Instance != null) ScoreManager.Instance.ResetScore();

        int currentLevel = LevelGenerator.Instance.CurrentLevel;

        if (useCheckpoint)
        {
            PlayerData savedData = DataManager.Instance.LoadGame();
            if (savedData != null && savedData.currentLevel == currentLevel)
            {
                Vector2 checkpointPos = new Vector2(savedData.positionX, savedData.positionY);
                player.transform.position = checkpointPos;
                ResetPlayerState();
                return;
            }
            else useCheckpoint = false;
        }

        if (!useCheckpoint)
        {
            string saveFilePath = Application.persistentDataPath + "/savegame.json";
            if (File.Exists(saveFilePath)) File.Delete(saveFilePath);

            LevelGenerator.Instance.ClearMapToPool();
            LevelGenerator.Instance.LoadLevelAndSpawnPlayer(currentLevel);
            ResetPlayerState();
        }
    }

    private void ResetPlayerState()
    {
        if (player == null) return;
        player.SetActive(true);

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = Color.white;

        var stats = player.GetComponent<PlayerStats>();
        if (stats != null) stats.ResetStats();

        var gravity = player.GetComponent<PlayerGravity>();
        if (gravity != null) gravity.ResetGravity();
    }


}