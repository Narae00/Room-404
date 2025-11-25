using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace NavKeypad
{
    public class Keypad : MonoBehaviour
    {
        // 🔥🔥 이동 관련 변수들을 최상단으로 이동
        [Header("Move Objects On Success (Speed-based)")]
        [SerializeField] private Transform[] objectsToMove;
        [SerializeField] private Vector3 moveOffset = new Vector3(0, 0, 2f);  // 이동 거리
        [SerializeField] private float moveSpeed = 2f; // m/s


        [Header("Events")]
        [SerializeField] private UnityEvent onAccessGranted;
        [SerializeField] private UnityEvent onAccessDenied;

        [Header("Combination Code (9 Numbers Max)")]
        [SerializeField] private int keypadCombo = 12345;

        public UnityEvent OnAccessGranted => onAccessGranted;
        public UnityEvent OnAccessDenied => onAccessDenied;

        [Header("Settings")]
        [SerializeField] private string accessGrantedText = "Granted";
        [SerializeField] private string accessDeniedText = "Denied";

        [Header("Visuals")]
        [SerializeField] private float displayResultTime = 1f;
        [Range(0, 5)]
        [SerializeField] private float screenIntensity = 2.5f;

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
            panelMesh.material.SetVector("_EmissionColor", screenNormalColor * screenIntensity);
        }


        public void AddInput(string input)
        {
            audioSource.PlayOneShot(buttonClickedSfx);

            if (displayingResult || accessWasGranted)
                return;

            switch (input)
            {
                case "enter":
                    CheckCombo();
                    break;

                default:
                    if (currentInput != null && currentInput.Length == 9)
                        return;

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

                if (!displayingResult)
                    StartCoroutine(DisplayResultRoutine(granted));
            }
            else
            {
                Debug.LogWarning("Couldn't process input!");
            }
        }


        private IEnumerator DisplayResultRoutine(bool granted)
        {
            displayingResult = true;

            if (granted)
                AccessGranted();
            else
                AccessDenied();

            yield return new WaitForSeconds(displayResultTime);

            displayingResult = false;

            if (granted)
                yield break;

            ClearInput();
            panelMesh.material.SetVector("_EmissionColor", screenNormalColor * screenIntensity);
        }


        private void AccessDenied()
        {
            keypadDisplayText.text = accessDeniedText;
            onAccessDenied?.Invoke();
            panelMesh.material.SetVector("_EmissionColor", screenDeniedColor * screenIntensity);
            audioSource.PlayOneShot(accessDeniedSfx);
        }


        private void ClearInput()
        {
            currentInput = "";
            keypadDisplayText.text = currentInput;
        }


        private void AccessGranted()
        {
            accessWasGranted = true;
            keypadDisplayText.text = accessGrantedText;
            onAccessGranted?.Invoke();
            panelMesh.material.SetVector("_EmissionColor", screenGrantedColor * screenIntensity);
            audioSource.PlayOneShot(accessGrantedSfx);

            // 🔥 성공 시 이동 시작
            StartCoroutine(MoveObjectsRoutine());
        }


        // 🔥 속도 기반 이동
        private IEnumerator MoveObjectsRoutine()
        {
            Vector3[] startPositions = new Vector3[objectsToMove.Length];
            Vector3[] endPositions = new Vector3[objectsToMove.Length];
            float[] durations = new float[objectsToMove.Length];

            // 시작 위치 + 목표 위치 + 개별 이동시간 계산
            for (int i = 0; i < objectsToMove.Length; i++)
            {
                startPositions[i] = objectsToMove[i].position;
                endPositions[i] = startPositions[i] + moveOffset;

                float distance = Vector3.Distance(startPositions[i], endPositions[i]);
                durations[i] = distance / moveSpeed;   // 🔥 m/s 속도 기반 이동시간
            }

            float elapsed = 0f;
            float maxDuration = Mathf.Max(durations);

            while (elapsed < maxDuration)
            {
                elapsed += Time.deltaTime;

                for (int i = 0; i < objectsToMove.Length; i++)
                {
                    float t = Mathf.Clamp01(elapsed / durations[i]);
                    objectsToMove[i].position =
                        Vector3.Lerp(startPositions[i], endPositions[i], t);
                }

                yield return null;
            }
        }
    }
}
