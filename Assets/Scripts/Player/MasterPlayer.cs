using UnityEngine;
using UnityEngine.EventSystems;
using System;   // optional

public class MasterPlayer : MonoBehaviour
//GPT Assistance
{
    Camera mainCamera;

    public GameObject ply { get { return player; } }
    public GameObject trg { get { return target; } }

    public FiringCone firingCone;
    [SerializeField] private bool fired;

    [SerializeField] private GameObject target;
    [SerializeField] private GameObject player;

    [SerializeField] private PlayerController controller;
    [SerializeField] private EnemyAI enemyAI;

    [SerializeField] private GridHighlighter gridHighlighter;
    [SerializeField] private int highlightDiameter = 10;

    [SerializeField] private Weapon weapon;
    [SerializeField] private Transform playerTransform;

    [SerializeField] private float rotationSpeed = 5f;

    //  NEW: Movement state
    public bool isInMovementMode = false;
    public Vector3 requestedMovePoint;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isInMovementMode)
        {
            HandleSelection();
        }

        HandleRotationAndWeapons();
    }

    // ------------------------------------------
    // SELECTION HANDLING 
    // ------------------------------------------
    void HandleSelection()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        Ray mouseray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(mouseray, out RaycastHit hitInfo))
        {
            Debug.Log("Selected: "+ hitInfo.collider.gameObject.tag);

            if (hitInfo.collider.CompareTag("Environment"))
            {
                //SetMoveTarget(hitInfo.point);
                return;
            }
                

            if (hitInfo.collider.CompareTag("Enemy"))
            {
                if (target != null)
                    target.GetComponent<EnemyAI>().OutlineOff();

                target = hitInfo.collider.gameObject;
                enemyAI = target.GetComponent<EnemyAI>();
                enemyAI.Outline();
                return;
            }

            if (hitInfo.collider.CompareTag("Player"))
            {
                if (player != null)
                    player.GetComponent<PlayerController>().OutlineOff();

                player = hitInfo.collider.gameObject;
                playerTransform = player.transform;
                weapon = player.GetComponent<Weapon>();
                controller = player.GetComponent<PlayerController>();
                controller.Outline();
                firingCone = player.GetComponent<FiringCone>();

                if (gridHighlighter != null)
                    gridHighlighter.ShowAreaAt(player.transform.position, highlightDiameter);

                return;
            }
        }
    }

    // ------------------------------------------
    // ROTATION & WEAPON HANDLER 
    // ------------------------------------------
    void HandleRotationAndWeapons()
    {
        if (target != null && player != null)
        {
            Vector3 roationDirection = target.transform.position - player.transform.position;
            if (roationDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(roationDirection);
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            firingCone.WeaponSwap();
            firingCone.WeaponAiming();
            return;
        }

        if (player != null && target == null)
        {
            firingCone.WeaponResting();
        }
    }

    // ------------------------------------------
    //  NEW: Called by Move button
    // ------------------------------------------
    public void BeginMovementMode()
    {
        isInMovementMode = true;
        gridHighlighter.Hide();
    }

    public void EndMovementMode()
    {
        isInMovementMode = false;
    }
    public void DeselectPlayer()
    {
        if (player != null)
        {
            var pc = player.GetComponent<PlayerController>();
            if (pc != null) pc.OutlineOff();

            player = null;
            controller = null;
            playerTransform = null;
            weapon = null;
            firingCone = null;

            if (gridHighlighter != null)
                gridHighlighter.Hide();
        }

        if (target != null)
        {
            var enemy = target.GetComponent<EnemyAI>();
            if (enemy != null) enemy.OutlineOff();
            target = null;
            enemyAI = null;
        }

        // Reset movement mode to allow new selections
        isInMovementMode = false;
    }
    // void SetMoveTarget(Vector3 worldPoint)
    // {
    //     // Sample NavMesh
    //     UnityEngine.AI.NavMeshHit navHit;
    //     if (UnityEngine.AI.NavMesh.SamplePosition(worldPoint, out navHit, 1f, UnityEngine.AI.NavMesh.AllAreas))
    //     {
    //         requestedMovePoint = navHit.position;
    //         isInMovementMode = true;

    //         // Highlight single tile
    //         if (gridHighlighter != null)
    //             gridHighlighter.ShowSingleAt(navHit.position);

    //         Debug.Log($"Movement target set to {requestedMovePoint}");
    //     }
    // }
    
}
