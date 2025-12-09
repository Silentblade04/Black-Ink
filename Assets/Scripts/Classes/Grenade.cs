using UnityEngine;
// with help from GPT
/// <summary>
/// Grenade ability: enter a targeting mode where GridHighlighter follows the mouse.
/// First activation starts targeting; second activation confirms and triggers the ability.
/// </summary>
public class Grenade : MonoBehaviour
{
    [Header("Highlighting")]
    [Tooltip("Assign the GridHighlighter that will show the affected tiles. If left null, the script will try to FindObjectOfType<GridHighlighter>() at Start.")]
    public GridHighlighter gridHighlighter;

    [Tooltip("Number of cells across the highlighted area (diameter). For a 2-block area set this to 2.")]
    public int diameter = 2;

    [Header("Targeting")]
    [Tooltip("Max raycast distance when selecting the target point.")]
    public float maxRange = 50f;

    [Tooltip("Optional prefab to spawn when the grenade is confirmed (visual/explosion). Leave null if not used).")]
    public GameObject grenadeEffectPrefab;

    [Tooltip("Height above ground to spawn the effect (so it sits on ground).")]
    public float spawnYOffset = 0.01f;

    // runtime
    private bool isTargeting = false;
    private Camera targetingCamera = null;
    private Vector3 lastTargetPoint = Vector3.zero;
    private bool hasValidTarget = false; // true if the last frame's raycast hit something valid

    private void Start()
    {
        if (gridHighlighter == null)
        {
            gridHighlighter = FindObjectOfType<GridHighlighter>();
            if (gridHighlighter == null)
                Debug.LogWarning("Grenade: No GridHighlighter found in scene. Assign one in the inspector to enable highlighting.");
        }
    }

    private void Update()
    {
        // while we're in targeting mode, update highlight to follow mouse
        if (!isTargeting) return;

        if (targetingCamera == null)
        {
            Debug.LogWarning("Grenade: Targeting camera is null. Cancelling targeting.");
            CancelTargeting();
            return;
        }

        // cancel on escape key (optional)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelTargeting();
            return;
        }

        // follow the mouse ray each frame
        Ray ray = targetingCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, maxRange))
        {
            lastTargetPoint = hitInfo.point;
            hasValidTarget = true;

            // make the grid highlighter show area centered on the hit point
            if (gridHighlighter != null)
            {
                gridHighlighter.ShowAreaAt(lastTargetPoint, diameter, GridHighlighter.HighlightType.Action);
            }
        }
        else
        {
            // If ray misses, hide highlights so player knows target is invalid
            hasValidTarget = false;
            if (gridHighlighter != null)
                gridHighlighter.Hide();
        }
    }

    /// <summary>
    /// Called by ClassManager (or other caller) to begin/confirm ability.
    /// If not currently targeting -> start targeting mode.
    /// If already targeting -> confirm and execute ability at last mouse location.
    /// </summary>
    public void ToggleOrConfirm(Camera cam)
    {
        if (!isTargeting)
        {
            StartTargeting(cam);
        }
        else
        {
            ConfirmTargeting();
        }
    }

    /// <summary>
    /// Start continuous targeting. Highlights will follow the mouse automatically.
    /// </summary>
    public void StartTargeting(Camera cam)
    {
        if (cam == null)
        {
            Debug.LogError("Grenade.StartTargeting: camera is null.");
            return;
        }

        if (gridHighlighter == null)
        {
            gridHighlighter = FindObjectOfType<GridHighlighter>();
            if (gridHighlighter == null)
            {
                Debug.LogWarning("Grenade.StartTargeting: No GridHighlighter found. Cannot start targeting.");
                return;
            }
        }

        isTargeting = true;
        targetingCamera = cam;
        hasValidTarget = false;

        // Immediately update once so highlight appears without waiting for next frame
        Ray ray = targetingCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, maxRange))
        {
            lastTargetPoint = hitInfo.point;
            hasValidTarget = true;
            gridHighlighter.ShowAreaAt(lastTargetPoint, diameter, GridHighlighter.HighlightType.Action);
        }
        else
        {
            // show nothing until mouse is over something valid
            gridHighlighter.Hide();
        }

        Debug.Log("Grenade: Targeting started.");
    }

    /// <summary>
    /// Confirm target and execute the grenade effect. Hides highlights afterwards.
    /// </summary>
    public void ConfirmTargeting()
    {
        if (!isTargeting)
        {
            Debug.LogWarning("Grenade.ConfirmTargeting called when not targeting.");
            return;
        }

        isTargeting = false;

        // If we don't have a last valid point, warn and abort.
        if (!hasValidTarget)
        {
            Debug.LogWarning("Grenade.ConfirmTargeting: No valid target selected. Cancelling.");
            if (gridHighlighter != null) gridHighlighter.Hide();
            targetingCamera = null;
            hasValidTarget = false;
            return;
        }

        // Snap to grid cell center (consistent with GridHighlighter behaviour)
        float cs = (gridHighlighter != null) ? gridHighlighter.cellSize : 1f;
        int centerX = Mathf.RoundToInt(lastTargetPoint.x / cs);
        int centerZ = Mathf.RoundToInt(lastTargetPoint.z / cs);
        Vector3 spawnPos = new Vector3(centerX * cs, lastTargetPoint.y + spawnYOffset, centerZ * cs);

        // Execute effect: spawn prefab (if provided) or just log
        if (grenadeEffectPrefab != null)
        {
            Instantiate(grenadeEffectPrefab, spawnPos, Quaternion.identity);
        }

        // Here you would put your gameplay logic (apply damage, spawn explosion VFX, etc).
        Debug.Log($"Grenade: confirmed at {spawnPos}. Executing effect for diameter {diameter}.");

        // Hide highlights after confirming
        if (gridHighlighter != null)
            gridHighlighter.Hide();

        // Reset camera / target state
        targetingCamera = null;
        hasValidTarget = false;
    }

    /// <summary>
    /// Cancel targeting and hide highlights.
    /// </summary>
    public void CancelTargeting()
    {
        isTargeting = false;
        targetingCamera = null;
        hasValidTarget = false;
        if (gridHighlighter != null)
            gridHighlighter.Hide();

        Debug.Log("Grenade: Targeting cancelled.");
    }

    /// <summary>
    /// Optional public helper to check if currently in targeting mode.
    /// </summary>
    public bool IsTargeting()
    {
        return isTargeting;
    }
}
