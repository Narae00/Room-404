using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
public class BookSlotChecker : MonoBehaviour
{
    public XRSocketInteractor bookSocket;
    public int slotIndex;

    public bookshelfController bookshelfCon;

    void OnEnable()
    {
        bookSocket.selectEntered.AddListener(OnBookPlaced);
        bookSocket.selectExited.AddListener(OnBookRemoved);
    }

    void OnDisable()
    {
        bookSocket.selectEntered.RemoveListener(OnBookPlaced);
        bookSocket.selectExited.RemoveListener(OnBookRemoved);
    }

    private void OnBookPlaced(SelectEnterEventArgs args)
    {
        GameObject book = args.interactableObject.transform.gameObject;
        bookshelfCon.UpdateSlot(slotIndex, book.tag);
    }

    private void OnBookRemoved(SelectExitEventArgs args)
    {
        bookshelfCon.UpdateSlot(slotIndex, "");
    }


}
