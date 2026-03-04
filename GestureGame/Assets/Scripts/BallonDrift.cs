using UnityEngine;

public class BallonDrift : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float verticalSpeed = 0.5f;
    public float horizontalAmount = 0.5f;
    public float horizontalSpeed = 1.5f;

    private float randomOffsetX;
    private float randomOffsetZ;
    private float direction;

    void Start()
    {
        // Randomly choose up or down
        direction = Random.value > 0.5f ? 1f : -1f;

        // Give each balloon a different wobble pattern
        randomOffsetX = Random.Range(0f, 100f);
        randomOffsetZ = Random.Range(0f, 100f);
    }

    void Update()
    {
        // Vertical drift
        Vector3 verticalMove = Vector3.up * direction * verticalSpeed * Time.deltaTime;

        // Smooth side-to-side movement
        float x = Mathf.Sin(Time.time * horizontalSpeed + randomOffsetX) * horizontalAmount;
        float z = Mathf.Cos(Time.time * horizontalSpeed + randomOffsetZ) * horizontalAmount;

        Vector3 horizontalMove = new Vector3(x, 0f, z);

        transform.position += verticalMove + horizontalMove * Time.deltaTime;
    }
}