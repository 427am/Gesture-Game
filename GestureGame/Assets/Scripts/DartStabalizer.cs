using UnityEngine;

public class DartStabilizer : MonoBehaviour
{
    private Rigidbody rb;
    public float rotationSpeed = 10f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(rb.linearVelocity);

            rb.MoveRotation(
                Quaternion.Slerp(
                    rb.rotation,
                    targetRotation,
                    rotationSpeed * Time.fixedDeltaTime
                )
            );
        }
    }
}