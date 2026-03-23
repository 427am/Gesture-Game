using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;

public class ProjectileLauncher : MonoBehaviour
{
    private XRHandSubsystem handSubsystem;

    [Header("Projectile Settings")]
    public GameObject launchingProjectile;
    public float launchForce = 8f;

    private PlayerControls controls;

    // Current fingertip pose (WORLD SPACE)
    private Vector3 fingerPosition;
    private Quaternion fingerRotation;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Player.Fire.performed += ctx => LaunchObject();
    }

    void Start()
    {
        var manager = XRGeneralSettings.Instance?.Manager;

        if (manager?.activeLoader != null)
        {
            handSubsystem = manager.activeLoader.GetLoadedSubsystem<XRHandSubsystem>();
        }

        if (handSubsystem == null)
        {
            Debug.LogError("XRHandSubsystem not found! Make sure XR Hands is enabled.");
        }
    }

    void Update()
    {
        if (handSubsystem == null) return;

        XRHand rightHand = handSubsystem.rightHand;

        if (!rightHand.isTracked) return;

        XRHandJoint indexTip = rightHand.GetJoint(XRHandJointID.IndexTip);

        if (indexTip.TryGetPose(out Pose pose))
        {
            // ? WORLD SPACE (do NOT convert)
            fingerPosition = pose.position;
            fingerRotation = pose.rotation;

            // Move this object to follow the fingertip
            transform.SetPositionAndRotation(fingerPosition, fingerRotation);
        }
    }

    public void LaunchObject()
    {
        if (launchingProjectile == null) return;

        GameObject projectile = Instantiate(
            launchingProjectile,
            fingerPosition,
            fingerRotation
        );

        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // Forward direction of finger
            rb.linearVelocity = fingerRotation * Vector3.forward * launchForce;
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

    // For trajectory / line renderer scripts
    public Vector3 GetLaunchPosition()
    {
        return fingerPosition;
    }

    public Vector3 GetLaunchVelocity()
    {
        return fingerRotation * Vector3.forward * launchForce;
    }
}