using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Rendering.HableCurve;
using static UnityEngine.UI.Image;

public class Weapon : MonoBehaviour
{
    public MasterPlayer player; // The omni player
    public PlayerController playerController; //Actual player object

    [SerializeField] private GameObject target;

    //firing cone stuff
    public FiringCone firingCone;
    public int rng { get { return rayDistance; } }
    public float acc { get { return Accuracy; } } 

    //stats
    [SerializeField] private WeaponStats stats;
    [SerializeField] private PlayerStats playerStats;

    [SerializeField] private float Dexterity;

    public int actions;

    [SerializeField] private int Damage;
    [SerializeField] private int Rounds;
    [SerializeField] private float Accuracy; // = dexterity times the accuracy stat of the weapon
    [SerializeField] private int ArmorPiercing;
    [SerializeField] private int Burst;
    [SerializeField] private int rayDistance;   //Range

    //accuracy stuff
    [SerializeField] private object deviationAngle; // The angle of deviation
    [SerializeField] private LayerMask hitLayers;      // Layers the ray can hit

    
    void Start()
    {
        player = FindFirstObjectByType<MasterPlayer>();
        playerController = GetComponent<PlayerController>();
        firingCone = GetComponent<FiringCone>();


        Dexterity = playerStats.dex;

        Damage = stats.dmg;
        Rounds = stats.rnd;
        Accuracy = stats.acc*Dexterity;
        ArmorPiercing = stats.ap;
        Burst = stats.burst;
        rayDistance = stats.raNge;

        firingCone.WeaponSwap();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            switchweapons();
        }
        actions = playerController.actLeft;
        target = player.trg;
    }

    public void shoot()
    {
        if (actions == 0)
        {
            Debug.Log("Out of actions");
            return;
        }
        else
        {
            playerController.ActionUsed(1);
        }
        Debug.Log("Shooting");
        if (target == null)
        {
            Debug.Log("No target");
            return;
        }
        Vector3 direction = (target.transform.position - playerController.transform.position).normalized;

        Vector3 deviatedDirection = ApplyDeviation(direction, Accuracy);

        // Fire the ray
        if (Physics.Raycast(playerController.transform.position, deviatedDirection, out RaycastHit hit, rayDistance, hitLayers))
        {
            Debug.DrawLine(playerController.transform.position, hit.point, Color.red, 15f);

            if (hit.collider.CompareTag("Enemy")) //For when we shoot enemies
            {
                Debug.Log("Hit an enemy!");
                EnemyAI enemy = hit.collider.GetComponent<EnemyAI>();
                if (enemy != null)
                {
                    enemy.Hit(Damage, firingCone);
                }

            }
            else if (hit.collider.CompareTag("Environment"))
            {
                Debug.Log("Hit an Environment!");
            }
        }
        else
        {
            Debug.DrawRay(playerController.transform.position, deviatedDirection * rayDistance, Color.yellow, 15f);
            Debug.Log("Ray missed.");
        }

    }

    Vector3 ApplyDeviation(Vector3 direction, float Accuracy)
    {
        Debug.Log("math stuff");
        // Get a random direction inside a cone
        return Quaternion.AngleAxis(UnityEngine.Random.Range(-Accuracy, Accuracy), playerController.transform.up) *
       Quaternion.AngleAxis(UnityEngine.Random.Range(-Accuracy, Accuracy), playerController.transform.right) *
       direction;
    }

    

    private void switchweapons()
    {
        
    }
}
