using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineManager : MonoBehaviour
{
    public ProjectileLauncher launcher;

    public int steps = 60;
    public float timeStep = 0.05f;

    private LineRenderer line;

    void Awake()
    {
        line = GetComponent<LineRenderer>();

        // You REQUIRE this off
        line.useWorldSpace = false;
    }

    void Update()
    {
        if (launcher == null) return;

        Vector3 startPos = launcher.GetLaunchPosition();   // world space
        Vector3 velocity = launcher.GetLaunchVelocity();   // world space

        DrawTrajectory(startPos, velocity);
    }

    void DrawTrajectory(Vector3 startPos, Vector3 velocity)
    {
        Vector3 pos = startPos;
        Vector3 vel = velocity;

        line.positionCount = steps;

        for (int i = 0; i < steps; i++)
        {
            // ✅ CONVERT WORLD → LOCAL SPACE
            Vector3 localPos = transform.InverseTransformPoint(pos);

            line.SetPosition(i, localPos);

            vel += Physics.gravity * timeStep;
            pos += vel * timeStep;
        }
    }
}