using NUnit.Framework;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.ProBuilder.Shapes;

public class MasterPlayer : MonoBehaviour
{
    //Got help from chatGPT for this
    Camera mainCamera; //The main player camera

    public GameObject ply { get { return player; } }
    public GameObject trg { get { return target; } }

    public FiringCone firingCone;


    [SerializeField] private GameObject target; //The target of an action like shoot
    [SerializeField] private GameObject player; //The selected player character

    [SerializeField] private PlayerController controller;
    [SerializeField] private EnemyAI enemyAI;


    [SerializeField] private Weapon weapon;
    [SerializeField] private Transform playerTransform;

    [SerializeField] private float rotationSpeed = 5f;


    void Start()
    {
        mainCamera = Camera.main; //assigns the camera
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //pulled from chatGPT
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return; // Skip selection
            }

            Ray mouseray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(mouseray, out RaycastHit hitInfo))
            {
                //debug of what we hit
                Debug.Log("Selected: "+ hitInfo.collider.gameObject.tag);

                if (hitInfo.collider.gameObject.tag == "Environment")
                {
                    Debug.Log("Hit the Environment");

                    return; //Skip
                }
                if (hitInfo.collider.gameObject.tag == "Enemy")
                {
                    if (target != null)
                    {
                        target.GetComponent<EnemyAI>().OutlineOff();
                    }
                    target = hitInfo.collider.gameObject;
                    enemyAI = target.GetComponent<EnemyAI>();
                    enemyAI.Outline();
                    return;
                }
                if(hitInfo.collider.gameObject.tag == "Player")
                {
                    if (player != null)
                    {
                        player.GetComponent<PlayerController>().OutlineOff();
                    }
                    player = hitInfo.collider.gameObject;
                    playerTransform = player.GetComponent<Transform>();
                    weapon = player.GetComponent<Weapon>();
                    controller = player.GetComponent<PlayerController>();
                    GetComponent<GridClickMovement>();
                    controller.Outline();
                    firingCone = player.GetComponent<FiringCone>();
                    return;
                }
            }
        }
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
        }
        else
        {
            if (player != null)
            {
                firingCone.WeaponResting();

            }
        }
    }
}
