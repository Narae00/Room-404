using UnityEngine;

public class KeyholeTrigger : MonoBehaviour
{
    public Transform insertPoint;   // 키가 정렬될 위치
    public string keyTag = "Key";   // 키의 태그

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(keyTag))
        {
            var key = other.GetComponent<KeyBehavior>();
            if (key != null)
            {
                key.InsertIntoKeyhole(insertPoint);
            }
        }
    }
}
