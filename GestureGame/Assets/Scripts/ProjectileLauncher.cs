using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;



public class ProjectileLauncher : MonoBehaviour
{
    XRHandSubsystem handSubsystem;

    private Vector3 indexPosition;
    private Quaternion indexRotation;
    public GameObject LaunchingProjectile;
    public Transform firePoint;
    public float launchForce;

    private PlayerControls controls;

    private void Start()
    {
        handSubsystem = XRGeneralSettings.Instance
            .Manager
            .activeLoader
            .GetLoadedSubsystem<XRHandSubsystem>();
    }
    private void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Fire.performed += ctx => LaunchObject();
    }

    void Update()
    {
        /*Debug.Log(firePoint.position);
        transform.position = firePoint.position;
        transform.rotation = firePoint.rotation;
        */

        if (handSubsystem == null) return;

        XRHand rightHand = handSubsystem.rightHand;

        if (rightHand.isTracked)
        {
            XRHandJoint indexTip = rightHand.GetJoint(XRHandJointID.IndexTip);

            if (indexTip.TryGetPose(out Pose pose))
            {
                indexPosition = pose.position;          // position of fingertip
                indexRotation = pose.rotation;       // rotation of fingertip

                Debug.Log("Right Index Finger Position: " + indexPosition);

                transform.position = indexPosition;
                transform.rotation = indexRotation;             // assign Quaternion directly
            }
        }


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
         GameObject projectile = Instantiate(LaunchingProjectile, indexPosition, indexRotation);

         Rigidbody rb = projectile.GetComponent<Rigidbody>();
         if (rb != null)
         {
            rb.linearVelocity = indexPosition * launchForce;
        }
         else
         {
             Debug.LogWarning("Projectile has no Rigidbody!");
         }
     }
}