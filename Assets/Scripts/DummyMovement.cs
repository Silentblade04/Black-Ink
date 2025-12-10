using UnityEngine;
using UnityEngine.AI;

public class DummyMovement : MonoBehaviour
{
    [SerializeField]
    private Camera Camera;
    private NavMeshAgent Agent;

    private RaycastHit[] Hits = new RaycastHit[1];

    // Movement lock toggle
    private bool movementLocked = false;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        // Toggle lock on right click
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            movementLocked = !movementLocked;
            Debug.Log("Movement Locked: " + movementLocked);
        }

        // If locked, ignore movement inputs
        if (movementLocked)
            return;

        // Normal movement on left click
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Ray ray = Camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.RaycastNonAlloc(ray, Hits) > 0)
            {
                Agent.SetDestination(Hits[0].point);
            }
        }
    }
}
