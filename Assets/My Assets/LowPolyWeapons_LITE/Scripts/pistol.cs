using UnityEngine;

public class pistol : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed;
    public float bulletLifeTime;

    // public AudioClip clip;
    // private AudioSource source;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        // source = GetComponent<AudioSource>();
    }

    public void FireBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        // source.PlayOneShot(clip);

        if (rb != null)
        {
            rb.linearVelocity = firePoint.forward * bulletSpeed;
        }

        Destroy(bullet, bulletLifeTime);
    }

}
