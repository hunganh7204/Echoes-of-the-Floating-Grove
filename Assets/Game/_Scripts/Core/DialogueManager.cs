using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Text;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("API Config")]
    [SerializeField] private string apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-3.1-flash-lite-preview:generateContent";
    [SerializeField] private string apiKey = "YOUR_API_KEY_HERE";

    private NPCAI currentNPC;
    private string internalChatHistory = ""; // Lịch sử ngầm gửi cho AI
    private bool isWaitingForAI = false;

    public static bool IsChatting { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void StartDialogue(NPCAI npc)
    {
        currentNPC = npc;
        internalChatHistory = "";
        isWaitingForAI = false;

        IsChatting = true;     // Bật cờ Khóa Input
        Time.timeScale = 0f;   // Đóng băng thế giới vật lý

        DialogueUI.Instance.ShowDialogueBox(npc.npcName, npc.greetingMessage);
        internalChatHistory += $"NPC: {npc.greetingMessage}\n";
    }

    public void ProcessUserQuestion(string userText)
    {
        if (isWaitingForAI || currentNPC == null) return;

        string lowerText = userText.ToLower();

        // 1. KIỂM TRA KỊCH BẢN CỨNG TRƯỚC (Ưu tiên số 1)
        foreach (var fixedRes in currentNPC.fixedResponses)
        {
            if (lowerText.Contains(fixedRes.keyword.ToLower()))
            {
                DialogueUI.Instance.DisplayAnswer(fixedRes.answer);
                internalChatHistory += $"Player: {userText}\nNPC: {fixedRes.answer}\n";
                return; // Ngắt luôn, KHÔNG GỌI API
            }
        }

        // 2. NẾU KHÔNG CÓ TRONG KỊCH BẢN -> GỌI AI
        internalChatHistory += $"Player: {userText}\n";
        DialogueUI.Instance.ShowLoadingIndicator();
        StartCoroutine(CallLLM_API(userText));
    }

    private IEnumerator CallLLM_API(string prompt)
    {
        isWaitingForAI = true;

        // Bơm Persona và Lore vào System Prompt
        string systemInstruction = $"Bạn là {currentNPC.npcName}. Tính cách: {currentNPC.npcPersona}. " +
                                   $"Cốt truyện thế giới: {currentNPC.npcLore}. " +
                                   $"Quy tắc: Luôn nhập vai, trả lời ngắn gọn bằng tiếng Việt.";

        string combinedText = $"[HỆ THỐNG]: {systemInstruction}\n\n[LỊCH SỬ CHAT]:\n{internalChatHistory}\nNPC trả lời:";

        string safeText = combinedText.Replace("\"", "\\\"").Replace("\n", "\\n");

        string jsonPayload = $@"{{
            ""contents"": [
                {{
                    ""parts"": [
                        {{""text"": ""{safeText}""}}
                    ]
                }}
            ]
        }}";

        string requestUrl = $"{apiUrl}?key={apiKey}";

        UnityWebRequest request = new UnityWebRequest(requestUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            DialogueUI.Instance.DisplayAnswer("Ta đang mệt, tín hiệu vũ trụ bị đứt đoạn...");
        }
        else
        {
            string responseJson = request.downloadHandler.text;
            string aiAnswer = MockExtractJson(responseJson);

            internalChatHistory += $"NPC: {aiAnswer}\n";
            DialogueUI.Instance.DisplayAnswer(aiAnswer);
        }

        isWaitingForAI = false;
    }

    private string MockExtractJson(string rawJson)
    {
        try
        {
            var jsonObj = Newtonsoft.Json.Linq.JObject.Parse(rawJson);
            return jsonObj["candidates"][0]["content"]["parts"][0]["text"].ToString();
        }
        catch
        {
            return "Lỗi suy nghĩ...";
        }
    }

    public void EndDialogue()
    {
        DialogueUI.Instance.HideDialogueBox();
        currentNPC = null;

        IsChatting = false; 
        Time.timeScale = 1f;
    }
}