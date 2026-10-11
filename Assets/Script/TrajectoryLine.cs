using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrajectoryLine : MonoBehaviour
{
    [Header("Line Settings")]
    [SerializeField] private int linePoints = 30;       // Dami ng points sa linya
    [SerializeField] private float arcHeight = 3f;      // Taas ng kurba sa ere

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = linePoints;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
    }

    // Patawagin ito bawat frame para i-update ang kurba sa mouse position
    public void DrawTrajectory(Vector3 startPoint, Vector3 targetPoint)
    {
        if (lineRenderer == null) return;
        lineRenderer.enabled = true;

        for (int i = 0; i < linePoints; i++)
        {
            float t = (float)i / (linePoints - 1);

            // Interpolation mula Start hanggang Target Point
            Vector3 currentPoint = Vector3.Lerp(startPoint, targetPoint, t);

            // Maglagay ng Y-Height offset para sa Parabolic Arc
            float height = Mathf.Sin(t * Mathf.PI) * arcHeight;
            currentPoint.y += height;

            lineRenderer.SetPosition(i, currentPoint);
        }
    }

    public void HideLine()
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
    }
}