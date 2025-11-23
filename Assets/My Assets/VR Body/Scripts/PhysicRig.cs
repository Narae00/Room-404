using UnityEngine;

public class PhysicRig : MonoBehaviour
{
    public Transform playerHead;
    public CapsuleCollider playerCollider;

    public float bodyHeightMin = 0.5f;
    public float bodyHeightMax = 2;


    void FixedUpdate()
    {
        playerCollider.height = Mathf.Clamp(playerHead.localPosition.y, bodyHeightMin, bodyHeightMax);
        playerCollider.center = new Vector3(playerHead.localPosition.x, 
            playerCollider.height / 2, playerHead.localPosition.z);
    }
}
