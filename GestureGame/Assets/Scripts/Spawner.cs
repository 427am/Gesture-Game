
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform followTarget;
    private Vector3 offset;
    public float interval = 3f;
    private float timer;
    public GameObject ObjectToBeSpawned;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        offset = transform.position - followTarget.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + offset;
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            SpawnBallon();
            //The function to spawn
            timer = 0f; // reset
        }
    }
    void SpawnBallon()
    {
        Instantiate(ObjectToBeSpawned, transform.position, transform.rotation);

    }
}
