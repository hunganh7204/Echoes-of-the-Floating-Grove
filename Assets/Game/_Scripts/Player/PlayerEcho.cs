using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct EchoFrame
{
    public Vector2 position;
    public Quaternion rotation;

    public int animNameHash;
    public float animNormalizedTime;

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
    [SerializeField] private Animator anim; 

    private List<EchoFrame> recordedData = new List<EchoFrame>();
    private bool isRecording = false;
    private float recordTimer = 0f;
    private GameObject currentClone;

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

            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

            recordedData.Add(new EchoFrame
            {
                position = transform.position,
                rotation = transform.rotation,
                animNameHash = stateInfo.shortNameHash,         
                animNormalizedTime = stateInfo.normalizedTime,  
                isAttacking = frameAttackInput,                 
                isInteracting = frameInteractInput              
            });

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