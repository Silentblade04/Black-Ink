using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
public class GridClickMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;          // Units per second for movement speed
    public float stopDistance = 0.05f;    // Distance threshold to consider a step reached

    private Rigidbody rb;                 // Rigidbody component reference
    private Queue<Vector3> pathQueue;     // Queue storing all positions along the path
    private Vector3 currentTarget;        // Current position player is moving toward
    private bool isMoving = false;        // Whether the player is currently moving

    [SerializeField] int actions;
    [SerializeField] PlayerController controller;
    void Start()
    {
        rb = GetComponent<Rigidbody>();  // Cache Rigidbody component
        pathQueue = new Queue<Vector3>(); // Initialize path queue
        currentTarget = transform.position; // Start with current position as target
        controller = GetComponent<PlayerController>();
    }

    void Update()
    {
        actions = controller.actLeft;
        HandleMouseClick();               // Check for player input every frame
        
    }


    void FixedUpdate()
    {
        // Only move if movement is active
        if (isMoving)
        {
            ContinueMovement();
        }
    }
    /// <summary>
    /// Detects left mouse clicks and calculates path to clicked grid location
    /// </summary>
    void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0))  // Check for left mouse button
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return; // Skip selection
            }
            // Cast a ray from the camera through the mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit)) // Check if ray hits any collider
            {
                // Round clicked position to nearest integer grid for grid-based movement
                Vector3 targetPos = new Vector3(
                    Mathf.Round(hit.point.x),
                    transform.position.y,   // Keep player's current Y position
                    Mathf.Round(hit.point.z)
                );

                
                // Generate path using diagonal first, then straight moves
                GeneratePath(transform.position, targetPos);
            }
        }
    }



    /// <summary>
    /// Generates a path from start to end using diagonal (45°) moves first, then straight (90°)
    /// </summary>
    void GeneratePath(Vector3 start, Vector3 end)
    {
        pathQueue.Clear(); // Clear any previous path

        int currentX = Mathf.RoundToInt(start.x); // Start X on integer grid
        int currentZ = Mathf.RoundToInt(start.z); // Start Z on integer grid

        int targetX = Mathf.RoundToInt(end.x);    // Target X on integer grid
        int targetZ = Mathf.RoundToInt(end.z);    // Target Z on integer grid

        // Continue adding steps until we reach the target
        while (currentX != targetX || currentZ != targetZ)
        {
            int stepX = 0;
            int stepZ = 0;

            // Diagonal move if both X and Z need adjustment
            if (currentX != targetX && currentZ != targetZ)
            {
                stepX = targetX > currentX ? 1 : -1; // Move toward target X
                stepZ = targetZ > currentZ ? 1 : -1; // Move toward target Z
            }
            // Straight horizontal move if only X needs adjustment
            else if (currentX != targetX)
            {
                stepX = targetX > currentX ? 1 : -1;
            }
            // Straight vertical move if only Z needs adjustment
            else if (currentZ != targetZ)
            {
                stepZ = targetZ > currentZ ? 1 : -1;
            }

            // Update current position
            currentX += stepX;
            currentZ += stepZ;

            // Add this step to the path queue
            pathQueue.Enqueue(new Vector3(currentX, start.y, currentZ));
        }

        // Set the first step as the immediate target
        if (pathQueue.Count > 0)
            currentTarget = pathQueue.Dequeue();
    }

    /// <summary>
    /// Called by UI Button to start moving along the queued path
    /// </summary>
    public void StartMovement()
    {
        if (actions == 0)
        {
            Debug.Log("No Actions Left");
            return;
        }

        if (pathQueue.Count > 0)
        {
            isMoving = true; // Begin continuous movement
            controller.ActionUsed(1);
        }
    }

    /// <summary>
    /// Handles actual per-frame movement logic while isMoving = true
    /// </summary>
    void ContinueMovement()
    {
        Vector3 direction = (currentTarget - transform.position).normalized;
        direction.y = 0;

        float distance = Vector3.Distance(transform.position, currentTarget);

        // When close enough to the current step, snap to it
        if (distance < stopDistance)
        {
            transform.position = currentTarget;

            // Move to next target if available
            if (pathQueue.Count > 0)
            {
                currentTarget = pathQueue.Dequeue();
            }
            else
            {
                // End of path reached
                isMoving = false;
                return;
            }
        }

        // Move and face direction
        rb.MovePosition(transform.position + direction * moveSpeed * Time.fixedDeltaTime);

        if (direction != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.fixedDeltaTime * 10f);
        }
    }
}
