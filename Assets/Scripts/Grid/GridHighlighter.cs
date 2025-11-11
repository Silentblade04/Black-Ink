using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Turns on a set of highlight tiles around a center cell. Designed for integer-aligned grids.
/// Attach to a scene object and assign a highlight tile prefab (e.g. Quad scaled to 1x1 with transparent material).
/// </summary>
public class GridHighlighter : MonoBehaviour
{
    [Tooltip("Prefab for a single highlighted cell. Should be a flat quad or plane sized to 1x1 (or normalized with cellSize).")]
    public GameObject tilePrefab;

    [Tooltip("Distance between grid cells. Should match your grid cell size (1 for integer-aligned).")]
    public float cellSize = 1f;

    [Tooltip("Parent for pooled tiles (optional).")]
    public Transform poolParent;

    [Tooltip("Maximum number of pooled tiles to create initially.")]
    public int initialPool = 100;

    [Tooltip("Should the highlight use Euclidean radius (round) or Manhattan (diamond) selection?")]
    public bool useEuclidean = true;

    // pooling
    private readonly List<GameObject> pool = new List<GameObject>();

    private void Awake()
    {
        if (poolParent == null)
            poolParent = transform;

        // pre-warm pool
        for (int i = 0; i < initialPool; i++)
            pool.Add(CreatePooledTile(false));
    }

    private GameObject CreatePooledTile(bool active = false)
    {
        if (tilePrefab == null)
        {
            Debug.LogError("GridHighlighter: tilePrefab not set.");
            return null;
        }

        var go = Instantiate(tilePrefab, poolParent);
        go.SetActive(active);
        return go;
    }

    private GameObject GetTileFromPool()
    {
        foreach (var g in pool)
        {
            if (!g.activeSelf)
                return g;
        }

        // none free → create another
        var created = CreatePooledTile(false);
        pool.Add(created);
        return created;
    }

    /// <summary>
    /// Show highlights in a circular area centered on worldPosition.
    /// diameter = number of cells across. For example diameter 10 -> radius 5.
    /// </summary>
    public void ShowAreaAt(Vector3 worldPosition, int diameter)
    {
        if (tilePrefab == null)
            return;

        int radius = Mathf.FloorToInt(diameter / 2f);

        // snap center to integer grid (assumes grid aligned to world integer coordinates)
        int centerX = Mathf.RoundToInt(worldPosition.x / cellSize);
        int centerZ = Mathf.RoundToInt(worldPosition.z / cellSize);

        // deactivate all currently active tiles first (we'll reactivate the ones we need)
        foreach (var t in pool)
        {
            if (t.activeSelf)
                t.SetActive(false);
        }

        // create / activate tiles within radius
        for (int dx = -radius; dx <= radius; dx++)
        {
            for (int dz = -radius; dz <= radius; dz++)
            {
                bool include;
                if (useEuclidean)
                {
                    // use Euclidean distance: circle
                    float dist = Mathf.Sqrt(dx * dx + dz * dz);
                    include = dist <= radius + 0.0001f;
                }
                else
                {
                    // use Manhattan distance: diamond
                    include = Mathf.Abs(dx) + Mathf.Abs(dz) <= radius;
                }

                if (!include) continue;

                var tile = GetTileFromPool();
                tile.SetActive(true);

                Vector3 pos = new Vector3((centerX + dx) * cellSize, 0f, (centerZ + dz) * cellSize);
                tile.transform.position = pos + new Vector3(0f, 1.01f, 0f); // slightly above ground to avoid z-fighting

                // Ensure tile is oriented flat on XZ plane (Quad by default faces +Z normally)
                tile.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

                // scale tile to cellSize (if the prefab is 1x1)
                tile.transform.localScale = Vector3.one * cellSize;
            }
        }
    }

    /// <summary>
    /// Show exactly one highlighted tile at the given world position (snaps to grid).
    /// This deactivates any other active pooled tiles so only a single cell is highlighted.
    /// </summary>
    public void ShowSingleAt(Vector3 worldPosition)
    {
        if (tilePrefab == null)
            return;

        // snap to integer-aligned grid
        int x = Mathf.RoundToInt(worldPosition.x / cellSize);
        int z = Mathf.RoundToInt(worldPosition.z / cellSize);

        // deactivate all tiles first
        foreach (var t in pool)
            t.SetActive(false);

        // get a tile and activate it at the snapped position
        var tile = GetTileFromPool();
        if (tile == null) return;

        tile.SetActive(true);
        Vector3 pos = new Vector3(x * cellSize, 0f, z * cellSize);
        tile.transform.position = pos + new Vector3(0f, 1.01f, 0f);
        tile.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        tile.transform.localScale = Vector3.one * cellSize;
    }

    /// <summary>
    /// Hide all highlights.
    /// </summary>
    public void Hide()
    {
        foreach (var t in pool)
            t.SetActive(false);
    }
}
