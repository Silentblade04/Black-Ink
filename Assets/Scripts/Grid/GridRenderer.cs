using UnityEngine;

/// <summary>
/// Renders a visible square grid using LineRenderers, aligned to integer coordinates.
/// The grid stays invisible until explicitly enabled and can be recentered dynamically.
/// </summary>
public class GridRenderer : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float cellSize = 1f;
    public Color gridColor = Color.green;
    public float gridY = 0.01f; // Slightly above terrain for visibility

    private bool gridGenerated = false;
    private Vector3 lastCenterPosition; // To remember where grid was centered

    /// <summary>
    /// Centers and generates the grid around a given world position.
    /// </summary>
    public void CenterGridOn(Vector3 centerPosition)
    {
        ClearGrid(); // Remove any existing lines first

        // Remember this position so we can regenerate later
        lastCenterPosition = centerPosition;

        // Compute grid origin so player is roughly centered
        Vector3 gridOrigin = new Vector3(
            Mathf.Round(centerPosition.x - (gridWidth / 2f) * cellSize),
            gridY,
            Mathf.Round(centerPosition.z - (gridHeight / 2f) * cellSize)
        );

        float width = gridWidth * cellSize;
        float height = gridHeight * cellSize;

        // Draw vertical lines (along Z-axis)
        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 start = gridOrigin + new Vector3(x * cellSize, 0, 0);
            Vector3 end = start + new Vector3(0, 0, height);
            CreateLine(start, end);
        }

        // Draw horizontal lines (along X-axis)
        for (int z = 0; z <= gridHeight; z++)
        {
            Vector3 start = gridOrigin + new Vector3(0, 0, z * cellSize);
            Vector3 end = start + new Vector3(width, 0, 0);
            CreateLine(start, end);
        }

        gridGenerated = true;
    }

    /// <summary>
    /// Destroys all child line renderers (removes the grid).
    /// </summary>
    public void ClearGrid()
    {
        foreach (Transform child in transform)
        {
            if (Application.isPlaying)
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject);
        }
        gridGenerated = false;
    }

    /// <summary>
    /// Creates a single visible grid line.
    /// </summary>
    private void CreateLine(Vector3 start, Vector3 end)
    {
        GameObject lineObj = new GameObject("GridLine");
        lineObj.transform.parent = transform;

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);

        lr.startWidth = 0.02f;
        lr.endWidth = 0.02f;

        lr.material = new Material(Shader.Find("Unlit/Color"));
        lr.material.color = gridColor;
        lr.startColor = gridColor;
        lr.endColor = gridColor;

        lr.useWorldSpace = true;
    }

    /// <summary>
    /// When this grid object becomes enabled again, regenerate if we previously had one.
    /// </summary>
    private void OnEnable()
    {
        if (gridGenerated && lastCenterPosition != Vector3.zero)
        {
            CenterGridOn(lastCenterPosition);
        }
    }

    /// <summary>
    /// Optional: Clear grid when disabled to save performance and memory.
    /// </summary>
    private void OnDisable()
    {
        // Comment this out if you want the grid to persist invisibly between activations.
        ClearGrid();
    }
}
