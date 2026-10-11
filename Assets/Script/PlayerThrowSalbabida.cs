using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(LineRenderer))]
public class PlayerThrowSalbabida : MonoBehaviour
{
    [Header("Throw Settings")]
    [SerializeField] private GameObject salbabidaPrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private float cooldown = 0.5f;

    [Header("Aim Trajectory Settings")]
    [SerializeField] private int trajectoryPoints = 25;
    [SerializeField] private float arcHeight = 3f;

    private float nextThrowTime = 0f;
    private Camera mainCam;
    private LineRenderer aimTrajectoryLine;

    private void Start()
    {
        mainCam = Camera.main;
        aimTrajectoryLine = GetComponent<LineRenderer>();

        if (aimTrajectoryLine != null)
        {
            aimTrajectoryLine.positionCount = trajectoryPoints;
            aimTrajectoryLine.startWidth = 0.1f;
            aimTrajectoryLine.endWidth = 0.1f;
            aimTrajectoryLine.enabled = true;
        }

        if (throwPoint == null) throwPoint = transform;
    }

    private void Update()
    {
        // Laging i-update ang aiming trajectory arc papunta sa mouse position
        UpdateAimTrajectory();

        // Throwing Input (Right Click)
        if (Input.GetMouseButtonDown(1) && Time.time >= nextThrowTime)
        {
            if (EventSystem.current != null && !EventSystem.current.IsPointerOverGameObject())
            {
                ThrowSalbabida();
            }
        }
    }

    private void UpdateAimTrajectory()
    {
        if (mainCam == null) mainCam = Camera.main;
        if (mainCam == null || aimTrajectoryLine == null) return;

        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        Plane waterPlane = new Plane(Vector3.up, throwPoint.position);

        if (waterPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 targetPoint = ray.GetPoint(rayDistance);
            Vector3 startPoint = throwPoint.position;

            aimTrajectoryLine.enabled = true;

            for (int i = 0; i < trajectoryPoints; i++)
            {
                float t = (float)i / (trajectoryPoints - 1);
                Vector3 currentPos = Vector3.Lerp(startPoint, targetPoint, t);

                // Curve height offset sa ere
                float height = Mathf.Sin(t * Mathf.PI) * arcHeight;
                currentPos.y += height;

                aimTrajectoryLine.SetPosition(i, currentPos);
            }
        }
    }

    private void ThrowSalbabida()
    {
        if (salbabidaPrefab == null) return;
        if (mainCam == null) mainCam = Camera.main;

        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        Plane waterPlane = new Plane(Vector3.up, throwPoint.position);

        if (waterPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 targetPoint = ray.GetPoint(rayDistance);
            nextThrowTime = Time.time + cooldown;

            Vector3 spawnPos = throwPoint != null ? throwPoint.position : transform.position;
            GameObject salbabida = Instantiate(salbabidaPrefab, spawnPos, Quaternion.identity);

            SalbabidaProjectile projectile = salbabida.GetComponent<SalbabidaProjectile>();
            if (projectile != null)
            {
                // I-pass ang target point at ang transform ng Player
                projectile.Launch(targetPoint, transform);
            }
        }
    }
}