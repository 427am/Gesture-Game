using UnityEngine;

public class Balloon : MonoBehaviour
{
    public GameObject Confetti;

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the colliding object is a projectile
        if (collision.gameObject.CompareTag("projectile"))
        {
            // Spawn the confetti at the balloon's position
            Instantiate(Confetti, transform.position, Quaternion.identity);

            // Optional: Destroy the balloon after popping
            // Destroy(gameObject);
        }
    }
}