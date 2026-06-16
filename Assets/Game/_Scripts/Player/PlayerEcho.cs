using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct EchoFrame
{
    public Vector2 position;
    public Quaternion rotation;

    // Lưu lại trạng thái chuẩn xác của Animator
    public int animNameHash;
    public float animNormalizedTime;

    // Lưu lại hành động
    public bool isAttacking;
    public bool isInteracting;
}

public class PlayerEcho : MonoBehaviour
{
    [Header("Echo Settings")]
    [SerializeField] private float maxRecordTime = 5f;
    [SerializeField] private GameObject echoClonePrefab;
    [SerializeField] private int echoManaCost = 20;

    [Header("References")]
    [SerializeField] private Animator anim; // Kéo Animator của Player vào đây

    private List<EchoFrame> recordedData = new List<EchoFrame>();
    private bool isRecording = false;
    private float recordTimer = 0f;
    private GameObject currentClone;

    // Biến tạm để bắt Input (Vì FixedUpdate có thể lỡ mất phím bấm nhanh)
    private bool frameAttackInput = false;
    private bool frameInteractInput = false;

    private void Update()
    {
        if (InputManager.Instance.RecordEchoPressed)
        {
            if (!isRecording && recordedData.Count == 0 && currentClone == null) StartRecording();
            else if (isRecording) StopRecording();
        }

        if (InputManager.Instance.ReplayEchoPressed && recordedData.Count > 0 && currentClone == null) ReplayEcho();
        if (InputManager.Instance.CancelEchoPressed && currentClone != null) CancelEcho();

        // NẾU ĐANG GHI HÌNH -> BẮT CÁC PHÍM BẤM HÀNH ĐỘNG
        if (isRecording)
        {
            if (InputManager.Instance.AttackPressed) frameAttackInput = true;
            if (InputManager.Instance.InteractPressed) frameInteractInput = true;
        }
    }

    private void FixedUpdate()
    {
        if (isRecording)
        {
            recordTimer += Time.fixedDeltaTime;
            if (recordTimer >= maxRecordTime)
            {
                StopRecording();
                return;
            }

            // Lấy thông tin animation hiện tại (Đang chạy Anim nào, chạy đến frame thứ mấy)
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

            recordedData.Add(new EchoFrame
            {
                position = transform.position,
                rotation = transform.rotation,
                animNameHash = stateInfo.shortNameHash,         // Lưu mã Anim
                animNormalizedTime = stateInfo.normalizedTime,  // Lưu tiến trình Anim
                isAttacking = frameAttackInput,                 // Lưu hành động chém
                isInteracting = frameInteractInput              // Lưu hành động tương tác
            });

            // Reset biến tạm cho frame tiếp theo
            frameAttackInput = false;
            frameInteractInput = false;
        }
    }

    private void StartRecording()
    {
        if (!PlayerStats.Instance.UseMana(echoManaCost)) return;
        isRecording = true;
        recordTimer = 0f;
        recordedData.Clear();
    }

    private void StopRecording() { isRecording = false; }

    private void ReplayEcho()
    {
        currentClone = Instantiate(echoClonePrefab, recordedData[0].position, recordedData[0].rotation);
        EchoClone cloneScript = currentClone.GetComponent<EchoClone>();
        cloneScript.InitData(new List<EchoFrame>(recordedData));
        recordedData.Clear();
    }

    private void CancelEcho()
    {
        if (currentClone != null) Destroy(currentClone);
        recordedData.Clear();
    }
}