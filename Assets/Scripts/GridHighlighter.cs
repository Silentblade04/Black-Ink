using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// With assistance from ChatGPT
/// Turns on a set of highlight tiles around a center cell. Designed for integer-aligned grids.
/// Attach to a scene object and assign highlight tile prefabs (e.g. Quad scaled to 1x1 with transparent material).
/// </summary>
public class GridHighlighter : MonoBehaviour
{
    [Tooltip("Prefab for a single moveable highlighted cell. Should be a flat quad or plane sized to 1x1 (or normalized with cellSize).")]
    [SerializeField] public GameObject moveTilePrefab;
    [Tooltip("Prefab for an actionable single highlighted cell. Should be a flat quad or plane sized to 1x1 (or normalized with cellSize).")]
    [SerializeField] public GameObject actionTilePrefab;

    [Tooltip("Distance between grid cells. Should match your grid cell size (1 for integer-aligned).")]
    public float cellSize = 1f;

    [Tooltip("Parent for pooled tiles (optional).")]
    public Transform poolParent;

    [Tooltip("Maximum number of pooled tiles to create initially (per prefab).")]
    public int initialPool = 100;

    [Tooltip("Should the highlight use Euclidean radius (round) or Manhattan (diamond) selection?")]
    public bool useEuclidean = true;

    // Pools for each prefab
    private readonly List<GameObject> movePool = new List<GameObject>();
    private readonly List<GameObject> actionPool = new List<GameObject>();

    public enum HighlightType { Move, Action }

    private void Awake()
    {
        if (poolParent == null)
            poolParent = transform;

        // pre-warm pools (creates initialPool for each prefab)
        for (int i = 0; i < initialPool; i++)
            movePool.Add(CreatePooledTile(moveTilePrefab, false));

        for (int i = 0; i < initialPool; i++)
            actionPool.Add(CreatePooledTile(actionTilePrefab, false));
    }

    // Create pooled tile for given prefab and return the GameObject (inactive by default unless active==true)
    private GameObject CreatePooledTile(GameObject prefab, bool active = false)
    {
        if (prefab == null)
        {
            Debug.LogError("GridHighlighter: requested prefab is null.");
            return null;
        }

        var go = Instantiate(prefab, poolParent);
        go.SetActive(active);
        return go;
    }

    // Get the pool list that corresponds to the prefab type
    private List<GameObject> GetPoolForType(HighlightType type)
    {
        return (type == HighlightType.Move) ? movePool : actionPool;
    }

    // Convenience to get prefab by type
    private GameObject GetPrefabForType(HighlightType type)
    {
        return (type == HighlightType.Move) ? moveTilePrefab : actionTilePrefab;
    }

    private GameObject GetTileFromPool(HighlightType type)
    {
        var pool = GetPoolForType(type);
        // find inactive
        foreach (var g in pool)
        {
            if (g != null && !g.activeSelf)
                return g;
        }

        // none free → create another
        var prefab = GetPrefabForType(type);
        var created = CreatePooledTile(prefab, false);
        pool.Add(created);
        return created;
    }

    /// <summary>
    /// Show highlights in a circular area centered on worldPosition.
    /// diameter = number of cells across. For example diameter 10 -> radius 5.
    /// Choose which prefab/pool to use with 'type'.
    /// </summary>
    public void ShowAreaAt(Vector3 worldPosition, int diameter, HighlightType type = HighlightType.Move)
    {
        var prefab = GetPrefabForType(type);
        if (prefab == null)
            return;

        int radius = Mathf.FloorToInt(diameter / 2f);

        // snap center to integer grid (assumes grid aligned to world integer coordinates)
        int centerX = Mathf.RoundToInt(worldPosition.x / cellSize);
        int centerZ = Mathf.RoundToInt(worldPosition.z / cellSize);

        // deactivate all currently active tiles in the selected pool (we'll reactivate the ones we need)
        var pool = GetPoolForType(type);
        foreach (var t in pool)
        {
            if (t != null && t.activeSelf)
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
                    float dist = Mathf.Sqrt(dx * dx + dz * dz);
                    include = dist <= radius + 0.0001f;
                }
                else
                {
                    include = Mathf.Abs(dx) + Mathf.Abs(dz) <= radius;
                }

                if (!include) continue;

                var tile = GetTileFromPool(type);
                if (tile == null) continue;
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
    /// Hide all highlights (both move and action pools).
    /// </summary>
    public void Hide()
    {
        foreach (var t in movePool)
            if (t != null) t.SetActive(false);

        foreach (var t in actionPool)
            if (t != null) t.SetActive(false);
    }
}
