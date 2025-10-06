using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MasterPlayer : MonoBehaviour
{
    //Got help from chatGPT for this
    Camera mainCamera; //The main player camera

    public GameObject ply { get { return player; } }
    public GameObject trg { get { return target; } }

    [SerializeField] private GameObject target; //The target of an action like shoot
    [SerializeField] private GameObject player; //The selected player character



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
                    target = hitInfo.collider.gameObject;
                    return;
                }
                if(hitInfo.collider.gameObject.tag == "Player")
                {
                    
                    player = hitInfo.collider.gameObject;
                    GetComponent<Weapon>();
                    GetComponent<PlayerController>();
                    GetComponent<GridClickMovement>();    
                    return;
                }
            }
        }
    }
}
