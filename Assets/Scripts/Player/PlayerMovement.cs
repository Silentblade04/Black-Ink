using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class GridClickMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float stopDistance = 0.05f;
    public float maxMoveDistance = 5f;

    private Rigidbody rb;
    private NavMeshAgent navAgent;
    private Queue<Vector3> pathQueue;
    private Vector3 currentTarget;
    private bool isMoving = false;

    [SerializeField] int actions;
    [SerializeField] PlayerController controller;

    [Header("References")]
    public GridHighlighter gridHighlighter;

    [Header("Layer Masks")]
    public LayerMask groundMask;      // Must EXCLUDE Highlight
    private int highlightLayer;       

    private Vector3? selectedCell = null;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        navAgent = GetComponent<NavMeshAgent>();
        pathQueue = new Queue<Vector3>();
        currentTarget = transform.position;

        controller = GetComponent<PlayerController>();

        if (gridHighlighter == null)
            gridHighlighter = FindObjectOfType<GridHighlighter>();

        highlightLayer = LayerMask.NameToLayer("Highlight");
    }

    void Update()
    {
        actions = controller.actLeft;
        HandleMouseHover();
        HandleMouseClick();
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            ContinueMovement();
        }
    }

    #region Mouse Input & Selection

    private void HandleMouseHover()
    {
        if (isMoving) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        // STEP 1 — First raycast INCLUDING highlight layer to detect hover.
        if (!Physics.Raycast(ray, out hit, 200f))
        {
            ClearHover();
            return;
        }

        Vector3 worldPoint = hit.point;

        bool hitHighlight = (hit.collider.gameObject.layer == highlightLayer);

        // STEP 2 — If we hit highlight, raycast downward to find ground.
        if (hitHighlight)
        {
            if (Physics.Raycast(hit.point + Vector3.up * 2f, Vector3.down, out RaycastHit groundHit, 10f, groundMask))
            {
                worldPoint = groundHit.point;
            }
            else
            {
                ClearHover();
                return;
            }
        }
        else
        {
            // STEP 3 — If hit normal ground, ensure it is a navmesh-valid point.
            if (Physics.Raycast(hit.point + Vector3.up * 2f, Vector3.down, out RaycastHit groundHit, 10f, groundMask))
            {
                worldPoint = groundHit.point;
            }
            else
            {
                ClearHover();
                return;
            }
        }

        // STEP 4 — Convert hover point to grid
        Vector3 cell = SnapToGrid(worldPoint);

        // STEP 5 — Check navmesh reachability
        if (IsCellReachable(cell))
        {
            selectedCell = cell;
            gridHighlighter?.ShowSelectedCell(cell);
        }
        else
        {
            ClearHover();
        }
    }

    private void ClearHover()
    {
        selectedCell = null;
        gridHighlighter?.HideType(GridHighlighter.HighlightType.Selected);
    }

    private void HandleMouseClick()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        if (selectedCell == null) return;

        GeneratePath(transform.position, selectedCell.Value);
    }

    #endregion

    #region Path Generation

    private void GeneratePath(Vector3 start, Vector3 end)
    {
        pathQueue.Clear();

        int currentX = Mathf.RoundToInt(start.x);
        int currentZ = Mathf.RoundToInt(start.z);
        int targetX = Mathf.RoundToInt(end.x);
        int targetZ = Mathf.RoundToInt(end.z);

        while (currentX != targetX || currentZ != targetZ)
        {
            int stepX = 0, stepZ = 0;

            if (currentX != targetX && currentZ != targetZ)
            {
                stepX = targetX > currentX ? 1 : -1;
                stepZ = targetZ > currentZ ? 1 : -1;
            }
            else if (currentX != targetX)
            {
                stepX = targetX > currentX ? 1 : -1;
            }
            else if (currentZ != targetZ)
            {
                stepZ = targetZ > currentZ ? 1 : -1;
            }

            currentX += stepX;
            currentZ += stepZ;

            Vector3 stepPos = new Vector3(currentX, start.y, currentZ);

            if (IsCellReachable(stepPos))
                pathQueue.Enqueue(stepPos);
            else
                break;
        }

        if (pathQueue.Count > 0)
            currentTarget = pathQueue.Dequeue();
    }

    #endregion

    #region Movement Execution

    public void StartMovement()
    {
        if (actions == 0)
        {
            Debug.Log("No Actions Left");
            return;
        }

        if (pathQueue.Count > 0)
        {
            isMoving = true;
            controller.ActionUsed(1);
            gridHighlighter?.Hide(); // Clear highlight immediately
        }
    }

    private void ContinueMovement()
    {
        Vector3 direction = (currentTarget - transform.position).normalized;
        direction.y = 0;

        float distance = Vector3.Distance(transform.position, currentTarget);

        if (distance < stopDistance)
        {
            transform.position = currentTarget;

            if (pathQueue.Count > 0)
                currentTarget = pathQueue.Dequeue();
            else
            {
                isMoving = false;
                return;
            }
        }

        rb.MovePosition(transform.position + direction * moveSpeed * Time.fixedDeltaTime);

        if (direction != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.fixedDeltaTime * 10f);
        }
    }

    #endregion

    #region Utility

    private Vector3 SnapToGrid(Vector3 pos)
    {
        int x = Mathf.RoundToInt(pos.x);
        int z = Mathf.RoundToInt(pos.z);
        return new Vector3(x, transform.position.y, z);
    }

    private bool IsCellReachable(Vector3 cell)
    {
        if (Vector3.Distance(transform.position, cell) > maxMoveDistance)
            return false;

        NavMeshPath path = new NavMeshPath();
        if (!navAgent.CalculatePath(cell, path) || path.status != NavMeshPathStatus.PathComplete)
            return false;

        return true;
    }

    #endregion
}
