using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrajectoryPredictor : MonoBehaviour
{
    [Header("Trajectory Settings")]
    [SerializeField] private int linePoints = 30; // Dami ng dots/points sa linya
    [SerializeField] private float timeStep = 0.05f; // Precision ng curve calculation

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Patawagin ito sa Update() habang nag-a-aim ang player
    public void RenderTrajectory(Vector3 startPosition, Vector3 initialVelocity)
    {
        lineRenderer.enabled = true;
        lineRenderer.positionCount = linePoints;

        Vector3 currentPosition = startPosition;
        Vector3 currentVelocity = initialVelocity;

        for (int i = 0; i < linePoints; i++)
        {
            lineRenderer.SetPosition(i, currentPosition);

            // Gravity trajectory formula: p = p0 + v*t + 0.5*g*t^2
            currentPosition += currentVelocity * timeStep + 0.5f * Physics.gravity * (timeStep * timeStep);
            currentVelocity += Physics.gravity * timeStep;
        }
    }

    public void HideTrajectory()
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
    }
}