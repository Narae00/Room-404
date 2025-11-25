using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
public class TeleportationActivator : MonoBehaviour
{
    public XRRayInteractor teleportInteractor;
    public XRRayInteractor rayInteractor;
    public InputActionProperty telepotActivatorAction;

    void Start()
    {
        teleportInteractor.gameObject.SetActive(false);
        telepotActivatorAction.action.performed += Action_performed;
        rayInteractor.uiHoverEntered.AddListener(x => DisableTeleportRay());
    }

    private void Action_performed(InputAction.CallbackContext obj)
    {
        if (rayInteractor && rayInteractor.IsOverUIGameObject())
        {
            return ;
        }
        teleportInteractor.gameObject.SetActive(true);
    }

    public void DisableTeleportRay()
    {
        teleportInteractor.gameObject.SetActive(false);
    }

    void Update()
    {
        if (telepotActivatorAction.action.WasReleasedThisFrame())
        {
            teleportInteractor.gameObject.SetActive(false);
        }
    }
}
