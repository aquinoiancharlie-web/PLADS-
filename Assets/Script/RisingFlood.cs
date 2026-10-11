using UnityEngine;

public class RisingFlood : MonoBehaviour
{
    public static RisingFlood Instance { get; private set; }

    [Header("Fixed Water Level Settings")]
    [Tooltip("I-check ito kung ayaw mong tumaas ang baha habang naglalaro.")]
    [SerializeField] private bool isFixedHeight = true; 
    [SerializeField] private float fixedYPosition = 0f; // Fixed na height ng tubig

    [Header("Dynamic Flood Settings (If Not Fixed)")]
    [SerializeField] private float startHeight = 0f;    // Posisyon ng baha sa simula
    [SerializeField] private float maxHeight = 4f;      // Pinakamataas na antas ng baha
    [SerializeField] private float riseSpeed = 0.05f;   // Bilis ng pagtaas bawat segundo

    [Header("Water Material & Visuals (Optional)")]
    [SerializeField] private Renderer waterRenderer;
    [SerializeField] private Color normalWaterColor = new Color(0.1f, 0.4f, 0.6f, 0.6f);
    [SerializeField] private Color criticalWaterColor = new Color(0.4f, 0.1f, 0.1f, 0.8f);

    [Header("Controls")]
    [SerializeField] private bool isRising = true;

    private bool hasWarnedCriticalLevel = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (waterRenderer == null)
        {
            waterRenderer = GetComponent<Renderer>();
        }

        // 1. Kung Fixed Height ang setting, i-lock agad ang Y position
        if (isFixedHeight)
        {
            Vector3 pos = transform.position;
            pos.y = fixedYPosition;
            transform.position = pos;

            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateWaterLevelUI(fixedYPosition, fixedYPosition);
            }
        }
        else
        {
            // Paunang posisyon kung dynamic/rising ang baha
            Vector3 pos = transform.position;
            pos.y = startHeight;
            transform.position = pos;
        }
    }

    private void Update()
    {
        // Kung naka-fixed height o naka-stop ang baha, huwag nang itaas ang tubig
        if (isFixedHeight || !isRising) return;

        // 2. Dahan-dahang itataas ang Y position ng water object hanggang maabot ang maxHeight
        if (transform.position.y < maxHeight)
        {
            transform.Translate(Vector3.up * riseSpeed * Time.deltaTime, Space.World);

            // 3. I-pass ang current height sa UIManager para sa HUD display
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateWaterLevelUI(transform.position.y, maxHeight);
            }

            // 4. Critical Warning Alert kapag umabot sa 75% ang taas ng baha
            float floodPercentage = transform.position.y / maxHeight;
            if (floodPercentage >= 0.75f && !hasWarnedCriticalLevel)
            {
                hasWarnedCriticalLevel = true;
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowAlert("NDRRMC DANGER: Floodwaters at critical level! Evacuate immediately!", 4f);
                }
            }

            // 5. Palitan ang kulay ng tubig habang lumalalim (pumupula o dumidilim)
            UpdateWaterVisuals(floodPercentage);
        }
        else
        {
            // Kapag naabot na ang maxHeight
            transform.position = new Vector3(transform.position.x, maxHeight, transform.position.z);
        }
    }

    private void UpdateWaterVisuals(float progress)
    {
        if (waterRenderer != null && waterRenderer.material.HasProperty("_Color"))
        {
            waterRenderer.material.color = Color.Lerp(normalWaterColor, criticalWaterColor, progress);
        }
    }

    /// <summary>
    /// Patigilin ang pagtaas ng baha (Tawagin sa Mission Success / Game Over)
    /// </summary>
    public void StopFlood()
    {
        isRising = false;
    }

    /// <summary>
    /// Patakbuhin muli ang pagtaas ng baha
    /// </summary>
    public void StartFlood()
    {
        isRising = true;
    }

    /// <summary>
    /// Kunin ang kasalukuyang taas ng baha
    /// </summary>
    public float GetCurrentWaterHeight()
    {
        return transform.position.y;
    }
}