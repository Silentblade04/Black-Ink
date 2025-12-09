using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// With GPT Assistance
/// Does not compute movement or ability ranges —
/// Displays whatever tiles you request.
/// </summary>
public class GridHighlighter : MonoBehaviour
{
    // ------------------------------
    //      PUBLIC SETTINGS
    // ------------------------------
    
    [Header("Tile Settings")]
    public float cellSize = 1f;

    [Header("Grid Height")]
public float highlightHeight = 0.02f;   // height offset for all highlight tiles

    [Tooltip("Prefab for highlight tiles (must be a flat quad or similar).")]
    public GameObject highlightTilePrefab;

    [Header("Tile Colors / Materials")]
    public Material moveMaterial;
    public Material abilityMaterial;
    public Material selectedMaterial;
    public Material dangerMaterial;
    public Material actionMaterial;

    // ------------------------------
    //      INTERNAL POOLS
    // ------------------------------

    public enum HighlightType
    {
        Move,
        Ability,
        Selected,
        Danger,
        Action
    }

    private class TilePool
    {
        public readonly List<GameObject> tiles = new List<GameObject>();
        public int activeCount = 0;
    }

    private Dictionary<HighlightType, TilePool> pools = new Dictionary<HighlightType, TilePool>();

    // ------------------------------
    //      UNITY LIFECYCLE
    // ------------------------------

    private void Awake()
    {
        foreach (HighlightType type in System.Enum.GetValues(typeof(HighlightType)))
            pools[type] = new TilePool();
    }

    // ------------------------------
    //      PUBLIC HIGHLIGHT API
    // ------------------------------

    /// <summary>
    /// Highlights all tiles in the provided list (movement, etc.)
    /// </summary>
    public void ShowMoveRange(IEnumerable<Vector3> cells)
    {
        ShowTiles(cells, HighlightType.Move);
    }

    /// <summary>
    /// Highlights a single selected tile.
    /// </summary>
    public void ShowSelectedCell(Vector3 cell)
    {
        ShowTiles(new List<Vector3> { cell }, HighlightType.Selected);
    }

    /// <summary>
    /// Shows a diameter-based AoE around a center point (Grid snapped).
    /// </summary>
    public void ShowAreaAt(Vector3 worldCenter, int diameter, HighlightType type)
    {
        float half = diameter / 2f;

        List<Vector3> cells = new List<Vector3>();

        int radius = Mathf.FloorToInt(half);

        Vector3 snapped = SnapToCell(worldCenter);

        for (int x = -radius; x <= radius; x++)
        {
            for (int z = -radius; z <= radius; z++)
            {
                Vector3 pos = snapped + new Vector3(x * cellSize, 0, z * cellSize);
                cells.Add(pos);
            }
        }

        ShowTiles(cells, type);
    }

    /// <summary>
    /// Hides everything.
    /// </summary>
    public void Hide()
    {
        foreach (var pool in pools.Values)
            DeactivatePool(pool);
    }

    /// <summary>
    /// Hides only one type of highlight.
    /// </summary>
    public void HideType(HighlightType type)
    {
        DeactivatePool(pools[type]);
    }

    // ------------------------------
    //      INTERNAL UTILITY
    // ------------------------------

    private void ShowTiles(IEnumerable<Vector3> worldCells, HighlightType type)
    {
        TilePool pool = pools[type];
        DeactivatePool(pool);

        foreach (var cell in worldCells)
        {
            GameObject tile = GetTile(pool);
            tile.transform.position = SnapToCell(cell);
            tile.SetActive(true);
        }
    }

    private Vector3 SnapToCell(Vector3 pos)
{
    int x = Mathf.RoundToInt(pos.x / cellSize);
    int z = Mathf.RoundToInt(pos.z / cellSize);

    return new Vector3(
        x * cellSize,
        highlightHeight,   // force consistent tile height
        z * cellSize
    );
}

    private GameObject GetTile(TilePool pool)
{
    if (pool.activeCount >= pool.tiles.Count)
    {
        GameObject obj = Instantiate(highlightTilePrefab, transform);
        pool.tiles.Add(obj);

        // Disable physics so tiles never push players
        var col = obj.GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;   // no collision force
        }

        var rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Destroy(rb);            // tiles should NEVER have rigidbodies
        }

        // Assign material
        var renderer = obj.GetComponentInChildren<Renderer>();
        if (renderer != null)
            renderer.material = GetMaterialForPool(pool);

        pool.activeCount++;
        return obj;
    }
    else
    {
        GameObject obj = pool.tiles[pool.activeCount];
        pool.activeCount++;
        return obj;
    }
}

    private Material GetMaterialForPool(TilePool pool)
    {
        foreach (var kv in pools)
            if (kv.Value == pool)
                return GetMaterialForType(kv.Key);

        return null;
    }

    private Material GetMaterialForType(HighlightType type)
    {
        return type switch
        {
            HighlightType.Move => moveMaterial,
            HighlightType.Ability => abilityMaterial,
            HighlightType.Selected => selectedMaterial,
            HighlightType.Danger => dangerMaterial,
            HighlightType.Action => actionMaterial,
            _ => abilityMaterial
        };
    }

    private void DeactivatePool(TilePool pool)
    {
        for (int i = 0; i < pool.tiles.Count; i++)
            pool.tiles[i].SetActive(false);

        pool.activeCount = 0;
    }
}
