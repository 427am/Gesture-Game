using UnityEngine;

public class Balloon : MonoBehaviour
{
    public GameObject Confetti;
    public float interval = 3f;
    private float timer;
    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            Destroy(gameObject);
            timer = 0f; // reset
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the colliding object is a projectile
        if (collision.gameObject.CompareTag("projectile"))
        {
            // Spawn the confetti at the balloon's position
            Instantiate(Confetti, transform.position, Quaternion.identity);
            Destroy(gameObject);

            // Optional: Destroy the balloon after popping
            // Destroy(gameObject);
        }
    }


}