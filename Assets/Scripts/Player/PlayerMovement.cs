using UnityEngine;
using UnityEngine.AI;

public class GridClickMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private MasterPlayer mp;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        mp = FindAnyObjectByType<MasterPlayer>();
    }

    void Update()
    {
        if (!mp.isInMovementMode) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (TryGetMovePoint(out Vector3 destination))
            {
                agent.SetDestination(destination);
                mp.requestedMovePoint = destination;
                mp.EndMovementMode();
            }
        }
    }

    bool TryGetMovePoint(out Vector3 result)
    {
        result = Vector3.zero;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit))
            return false;

        // Snap to grid
        Vector3 snapped = new Vector3(
            Mathf.Round(hit.point.x),
            0f,
            Mathf.Round(hit.point.z)
        );

        // Project onto NavMesh
        NavMeshHit navHit;
        if (!NavMesh.SamplePosition(snapped, out navHit, 1.0f, NavMesh.AllAreas))
        {
            Debug.LogWarning($"GridClickMovement: Target {snapped} not on NavMesh.");
            return false;
        }

        result = navHit.position;
        return true;
    }
}
