using UnityEngine;
using UnityEngine.InputSystem;

public class ProjectileLauncher : MonoBehaviour
{
    public GameObject LaunchingProjectile;
    public Transform firePoint;
    public float launchForce;

    private PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Fire.performed += ctx => LaunchObject();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

     public void LaunchObject()
     {
         GameObject projectile = Instantiate(LaunchingProjectile, firePoint.position, firePoint.rotation);

         Rigidbody rb = projectile.GetComponent<Rigidbody>();
         if (rb != null)
         {
            rb.linearVelocity = firePoint.forward * launchForce;
        }
         else
         {
             Debug.LogWarning("Projectile has no Rigidbody!");
         }
     }
}