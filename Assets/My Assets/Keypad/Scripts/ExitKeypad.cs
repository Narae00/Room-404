using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace NavKeypad
{
    public class KeypadExit : MonoBehaviour
    {
        [Header("1. Desk Slide SFX")]
        [SerializeField] private AudioSource ambientSfxSource;
        [SerializeField] private AudioClip wallSlideSfx;

        [Header("2. Door Open SFX")]
        [SerializeField] private AudioSource doorSfxSource;
        [SerializeField] private AudioClip doorOpenSfx;

        [Header("Move Objects Settings")]
        [SerializeField] private Transform[] objectsToMove;
        [SerializeField] private Vector3 moveOffset = new Vector3(0, 0, 2f);
        [SerializeField] private float moveSpeed = 2f;

        // ▼▼▼ [추가된 기능: 딜레이 & 전광판] ▼▼▼
        [Header("Exit Sequence Settings")]
        [Tooltip("책상 이동이 끝나고 문이 열리기 전 대기 시간 (초)")]
        public float exitDelay = 0.5f; // ★ 여기서 딜레이 조절 가능!

        [Header("Exit Sign Emission (전광판)")]
        public Renderer exitSignRenderer; // Exit 전광판 모델
        public int exitSignMatIndex = 0;  // 전광판 재질 순서 (보통 0)
        [ColorUsage(true, true)]
        public Color exitSignOnColor = Color.green * 3f; // 켜질 때 색상 (빛나게)
        // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

        [Header("Events")]
        [SerializeField] private UnityEvent onAccessGranted; // 즉시 실행
        [SerializeField] private UnityEvent onExitSequence;  // 나중 실행 (문 열기 등)
        [SerializeField] private UnityEvent onAccessDenied;

        [Header("Combination Code")]
        [SerializeField] private int keypadCombo = 12345;

        // ... (기존 변수들: 건드리지 않음) ...
        [Header("Settings")][SerializeField] private string accessGrantedText = "Granted";
        [SerializeField] private string accessDeniedText = "Denied";
        [Header("Visuals")][SerializeField] private float displayResultTime = 1f;
        [Range(0, 5)][SerializeField] private float screenIntensity = 2.5f;
        [Header("Colors")]
        [SerializeField] private Color screenNormalColor = new Color(0.98f, 0.50f, 0.032f, 1f);
        [SerializeField] private Color screenDeniedColor = new Color(1f, 0f, 0f, 1f);
        [SerializeField] private Color screenGrantedColor = new Color(0f, 0.62f, 0.07f);
        [Header("SoundFx")]
        [SerializeField] private AudioClip buttonClickedSfx;
        [SerializeField] private AudioClip accessDeniedSfx;
        [SerializeField] private AudioClip accessGrantedSfx;
        [Header("Component References")]
        [SerializeField] private Renderer panelMesh;
        [SerializeField] private TMP_Text keypadDisplayText;
        [SerializeField] private AudioSource audioSource;

        private string currentInput;
        private bool displayingResult = false;
        private bool accessWasGranted = false;

        private void Awake()
        {
            ClearInput();
            if (panelMesh != null) panelMesh.material.SetVector("_EmissionColor", screenNormalColor * screenIntensity);

            // 시작할 때 전광판 끄기 (검은색)
            if (exitSignRenderer != null)
            {
                exitSignRenderer.materials[exitSignMatIndex].SetColor("_EmissionColor", Color.black);
            }
        }

        // ... (AddInput, CheckCombo 등 기존 함수는 그대로 유지) ...
        public void AddInput(string input)
        {
            if (audioSource != null && buttonClickedSfx != null) audioSource.PlayOneShot(buttonClickedSfx);
            if (displayingResult || accessWasGranted) return;
            switch (input)
            {
                case "enter": CheckCombo(); break;
                default:
                    if (currentInput != null && currentInput.Length >= 9) return;
                    currentInput += input;
                    keypadDisplayText.text = currentInput;
                    break;
            }
        }

        public void CheckCombo()
        {
            if (int.TryParse(currentInput, out var currentKombo))
            {
                bool granted = currentKombo == keypadCombo;
                if (!displayingResult) StartCoroutine(DisplayResultRoutine(granted));
            }
            else { ClearInput(); }
        }

        private IEnumerator DisplayResultRoutine(bool granted)
        {
            displayingResult = true;
            if (granted) AccessGranted();
            else AccessDenied();
            yield return new WaitForSeconds(displayResultTime);
            displayingResult = false;
            if (granted) yield break;
            ClearInput();
            if (panelMesh != null) panelMesh.material.SetVector("_EmissionColor", screenNormalColor * screenIntensity);
        }

        private void AccessDenied()
        {
            keypadDisplayText.text = accessDeniedText;
            onAccessDenied?.Invoke();
            if (panelMesh != null) panelMesh.material.SetVector("_EmissionColor", screenDeniedColor * screenIntensity);
            if (audioSource != null && accessDeniedSfx != null) audioSource.PlayOneShot(accessDeniedSfx);
        }

        private void ClearInput()
        {
            currentInput = "";
            if (keypadDisplayText != null) keypadDisplayText.text = currentInput;
        }

        private void AccessGranted()
        {
            accessWasGranted = true;
            keypadDisplayText.text = accessGrantedText;

            // 1. 즉시 실행 (키패드 반응)
            onAccessGranted?.Invoke();
            if (panelMesh != null) panelMesh.material.SetVector("_EmissionColor", screenGrantedColor * screenIntensity);
            if (audioSource != null && accessGrantedSfx != null) audioSource.PlayOneShot(accessGrantedSfx);

            // 2. 책상 미는 소리 시작
            if (ambientSfxSource != null && wallSlideSfx != null)
            {
                ambientSfxSource.clip = wallSlideSfx;
                ambientSfxSource.loop = true;
                ambientSfxSource.Play();
            }

            // 3. 시퀀스 시작
            if (objectsToMove != null && objectsToMove.Length > 0)
            {
                StartCoroutine(MoveSequenceRoutine());
            }
        }

        private IEnumerator MoveSequenceRoutine()
        {
            // --- [책상 이동 로직] ---
            Vector3[] startPositions = new Vector3[objectsToMove.Length];
            Vector3[] endPositions = new Vector3[objectsToMove.Length];
            float maxDuration = 0f;

            for (int i = 0; i < objectsToMove.Length; i++)
            {
                if (objectsToMove[i] == null) continue;
                startPositions[i] = objectsToMove[i].position;
                endPositions[i] = startPositions[i] + moveOffset;
                float duration = Vector3.Distance(startPositions[i], endPositions[i]) / moveSpeed;
                if (duration > maxDuration) maxDuration = duration;
            }

            float elapsed = 0f;
            while (elapsed < maxDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / maxDuration);
                for (int i = 0; i < objectsToMove.Length; i++)
                {
                    if (objectsToMove[i] != null)
                        objectsToMove[i].position = Vector3.Lerp(startPositions[i], endPositions[i], t);
                }
                yield return null;
            }

            // 위치 확정
            for (int i = 0; i < objectsToMove.Length; i++)
                if (objectsToMove[i] != null) objectsToMove[i].position = endPositions[i];

            // 이동 소리 끄기
            if (ambientSfxSource != null) ambientSfxSource.Stop();

            // --- [대기 시간] ---
            // ★ 여기서 설정한 시간만큼 기다립니다!
            yield return new WaitForSeconds(exitDelay);

            // --- [문 열기 & 전광판 켜기] ---

            // 1. 전광판 Emission 켜기
            if (exitSignRenderer != null)
            {
                exitSignRenderer.materials[exitSignMatIndex].SetColor("_EmissionColor", exitSignOnColor);
            }

            // 2. 문 소리 재생
            if (doorSfxSource != null && doorOpenSfx != null) doorSfxSource.PlayOneShot(doorOpenSfx);
            else if (ambientSfxSource != null && doorOpenSfx != null) ambientSfxSource.PlayOneShot(doorOpenSfx);

            // 3. 문 열기 이벤트 실행
            onExitSequence?.Invoke();
        }
    }
}