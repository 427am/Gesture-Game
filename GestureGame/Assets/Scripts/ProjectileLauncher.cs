using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{   
    public GameObject LaunchingProjectile;
    public Transform firePoint;

   //launch force should be the same as velocity
    public float launchForce;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void LaunchObject()
    {
    Rigidbody rb = LaunchingProjectile.GetComponent<Rigidbody>();
    if (rb != null)
        {
            // Launch forward
            rb.AddForce(firePoint.forward * launchForce, ForceMode.Impulse);
        }
        else
        {
            Debug.LogWarning("Projectile has no Rigidbody!");
        }
    }
}
