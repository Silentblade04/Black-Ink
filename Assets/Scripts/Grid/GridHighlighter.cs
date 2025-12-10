using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // <-- required for NavMesh

/// <summary>
/// GridHighlighter (NavMesh-aware): shows pooled 1x1 tiles on an integer-aligned grid.
/// This variant DOES NOT create/activate tiles on positions that are not backed by the NavMesh.
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

    [Header("Tile Appearance")]
    [Tooltip("Material to use for the highlight tiles.")]
    public Material tileMaterial;

    [Header("NavMesh Validation")]
    [Tooltip("How far to sample the NavMesh from the tile position when validating. Smaller = stricter.")]
    public float navSampleDistance = 0.25f;

    [Tooltip("Height above NavMesh to place highlight tile")]
    public float highlightYOffset = 0.1f; // small offset to avoid z-fighting

    [Tooltip("Which NavMesh area mask to validate against. Defaults to all areas.")]
    public int navAreaMask = -1; // NavMesh.AllAreas

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
    /// Tiles will only be shown if NavMesh.SamplePosition finds a valid nav position near the tile.
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

                // candidateWorld is X/Z of the tile
                Vector3 candidateWorld = new Vector3((centerX + dx) * cellSize, 0.5f, (centerZ + dz) * cellSize); // start ~0.5 units above ground

                NavMeshHit hit;
                if (!NavMesh.SamplePosition(candidateWorld, out hit, navSampleDistance, navAreaMask == -1 ? NavMesh.AllAreas : navAreaMask))
                {
                    Debug.Log($"GridHighlighter: skipped tile at {candidateWorld}, NavMesh invalid.");
                    continue;
                }

                var tile = GetTileFromPool();
                tile.SetActive(true);
                
                // Apply material if assigned
                if (tileMaterial != null)
                {
                    var renderer = tile.GetComponent<Renderer>();
                    if (renderer != null)
                        renderer.material = tileMaterial;
                }

                // Snap X/Z to grid, Y from nav hit, plus highlight offset
                tile.transform.position = new Vector3(candidateWorld.x, hit.position.y + highlightYOffset, candidateWorld.z);
                tile.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                tile.transform.localScale = Vector3.one * cellSize;
            }
        }
    }

    /// <summary>
    /// Show exactly one highlighted tile at the given world position (snaps to grid).
    /// This deactivates any other active pooled tiles so only a single cell is highlighted.
    /// Only shows the tile if NavMesh validates the position.
    /// </summary>
    public void ShowSingleAt(Vector3 worldPosition)
    {
        if (tilePrefab == null)
            return;

        // Snap to integer-aligned grid X/Z
        int x = Mathf.RoundToInt(worldPosition.x / cellSize);
        int z = Mathf.RoundToInt(worldPosition.z / cellSize);
        Vector3 candidateWorld = new Vector3(x * cellSize, 0f, z * cellSize); // Y=0, NavMesh will define height

        NavMeshHit hit;
        if (!NavMesh.SamplePosition(candidateWorld, out hit, navSampleDistance, navAreaMask == -1 ? NavMesh.AllAreas : navAreaMask))
        {
            Debug.Log($"GridHighlighter: ShowSingleAt skipped tile at {candidateWorld}, NavMesh invalid.");
            return;
        }

        var tile = GetTileFromPool();
        tile.SetActive(true);

        // Snap X/Z to grid, Y from NavMesh hit + highlight offset
        tile.transform.position = new Vector3(candidateWorld.x, hit.position.y + highlightYOffset, candidateWorld.z);
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

    /// <summary>
    /// Helper: checks whether the provided grid-aligned world position is on/near the NavMesh.
    /// Uses NavMesh.SamplePosition with navSampleDistance and navAreaMask.
    /// </summary>
    private bool IsPositionOnNavMesh(Vector3 worldPos, out NavMeshHit hit)
    {
        // put a small upward offset so sample works over uneven ground (optional)
        Vector3 samplePos = new Vector3(worldPos.x, worldPos.y + 0.1f, worldPos.z);
        return NavMesh.SamplePosition(samplePos, out hit, navSampleDistance, navAreaMask == -1 ? NavMesh.AllAreas : navAreaMask);
    }
}
