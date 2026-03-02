using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineManager : MonoBehaviour
{
    public ProjectileLauncher launcher;
    public int MaxPositions = 60;
    public float Drag = 0f;

    private LineRenderer line;

    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    public Vector3[] GetTrajectoryPredictionPoints(
        Vector3 startPos,
        Vector3 startVelocity,
        float drag,
        int steps)
    {
        Vector3 pos = startPos;
        Vector3 vel = startVelocity;
        Vector3 acc = Physics.gravity;

        float dt = Time.fixedDeltaTime;
        Vector3[] points = new Vector3[steps];
        points[0] = startPos;

        for (int i = 1; i < steps; i++)
        {
            vel += acc * dt;
            vel *= 1f / (1f + drag * dt);
            pos += vel * dt;

            points[i] = pos;
        }

        return points;
    }
    void Update()
    {
        if (launcher == null) return;

        Vector3 startPosition = launcher.firePoint.position;
        Vector3 startVelocity = launcher.firePoint.forward * launcher.launchForce;

        Vector3[] trajectory = GetTrajectoryPredictionPoints(
            startPosition,
            startVelocity,
            Drag,
            MaxPositions
        );

        line.positionCount = trajectory.Length;
        line.SetPositions(trajectory);
    }
}