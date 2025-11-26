using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class TeleportationActivator : MonoBehaviour
{
    public XRRayInteractor teleportInteractor;
    public XRRayInteractor rayInteractor;
    public InputActionProperty telepotActivatorAction;

    // Start 대신 OnEnable에서 액션을 활성화하고 구독합니다.
    void OnEnable()
    {
        // Null 방어 (혹시 Inspector 연결을 깜빡했을 경우 대비)
        if (telepotActivatorAction.action == null) return;

        // 1. 액션을 켜고 구독합니다. (액션 사용 전 필수!)
        telepotActivatorAction.action.Enable();
        telepotActivatorAction.action.performed += Action_performed;
        
        // 2. 다른 Interactor의 UI 호버링 리스너는 그대로 유지
        if (rayInteractor != null)
        {
            rayInteractor.uiHoverEntered.AddListener(x => DisableTeleportRay());
        }

        // 3. 텔레포트 레이는 시작 시 꺼둡니다.
        if (teleportInteractor != null)
        {
            teleportInteractor.gameObject.SetActive(false);
        }
    }

    // 오브젝트가 비활성화되거나 씬을 떠날 때 액션을 해제합니다.
    void OnDisable()
    {
        if (telepotActivatorAction.action == null) return;
        
        // 1. 구독 해제 (메모리 누수 방지)
        telepotActivatorAction.action.performed -= Action_performed;
        
        // 2. 액션을 끕니다.
        telepotActivatorAction.action.Disable();

        // 3. 다른 Interactor의 UI 호버링 리스너 해제
        if (rayInteractor != null)
        {
            rayInteractor.uiHoverEntered.RemoveListener(x => DisableTeleportRay());
        }
    }
    
    // Action Logic
    private void Action_performed(InputAction.CallbackContext obj)
    {
        // UI 위에 레이저가 있다면 텔레포트 막기
        if (rayInteractor && rayInteractor.IsOverUIGameObject())
        {
            return;
        }
        // 텔레포트 레이 켜기
        teleportInteractor.gameObject.SetActive(true);
    }

    public void DisableTeleportRay()
    {
        if (teleportInteractor != null)
        {
            teleportInteractor.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Action이 해제(버튼 놓기)될 때 레이저 끄기
        if (telepotActivatorAction.action.WasReleasedThisFrame())
        {
            if (teleportInteractor != null)
            {
                teleportInteractor.gameObject.SetActive(false);
            }
        }
    }
}