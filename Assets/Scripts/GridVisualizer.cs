using UnityEngine;

/// <summary>
/// Displays a visible grid overlay aligned to world-space integer coordinates.
/// Works best with movement systems that snap to integer grid positions.
/// </summary>
[ExecuteAlways] // Allows visualization both in Play Mode and Edit Mode
public class GridVisualizer : MonoBehaviour
{
    [Header("Grid Settings")]
    [Tooltip("How many units wide the grid should be.")]
    public int gridWidth = 20;

    [Tooltip("How many units tall the grid should be.")]
    public int gridHeight = 20;

    [Tooltip("Distance between grid lines. Set to 1 for integer alignment.")]
    public float cellSize = 1f;

    [Tooltip("Color of the grid lines.")]
    public Color gridColor = Color.white;

    [Tooltip("Optional offset of the grid in world space.")]
    public Vector3 gridOrigin = Vector3.zero;

    [Header("Debug Settings")]
    [Tooltip("Enable to show grid even in Edit Mode.")]
    public bool showInEditor = true;

    private void OnDrawGizmos()
    {
        if (!showInEditor && !Application.isPlaying)
            return;

        Gizmos.color = gridColor;

        // Calculate half extents for centering the grid
        float width = gridWidth * cellSize;
        float height = gridHeight * cellSize;

        // Draw vertical lines (X direction)
        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 start = gridOrigin + new Vector3(x * cellSize, 0, 0);
            Vector3 end = start + new Vector3(0, 0, height);
            Gizmos.DrawLine(start, end);
        }

        // Draw horizontal lines (Z direction)
        for (int z = 0; z <= gridHeight; z++)
        {
            Vector3 start = gridOrigin + new Vector3(0, 0, z * cellSize);
            Vector3 end = start + new Vector3(width, 0, 0);
            Gizmos.DrawLine(start, end);
        }
    }
}
