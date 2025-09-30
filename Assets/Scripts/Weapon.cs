using System;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.UI.Image;

public class Weapon : MonoBehaviour
{
    public MasterPlayer player; // The omni player
    public PlayerController playerController; //Actual player object

    [SerializeField] private GameObject target;

    [SerializeField] private WeaponStats stats;
    [SerializeField] private PlayerStats playerStats;

    [SerializeField] private float Dexterity;

    [SerializeField] private int Damage;
    [SerializeField] private int Rounds;
    [SerializeField] private float Accuracy; // = dexterity times the accuracy stat of the weapon
    [SerializeField] private int ArmorPiercing;
    [SerializeField] private int Burst;
    
    [SerializeField] private object deviationAngle; // The angle of deviation
    [SerializeField] private int rayDistance;   //Range
    [SerializeField] private LayerMask hitLayers;      // Layers the ray can hit

    void Start()
    {
        player = FindFirstObjectByType<MasterPlayer>();
        playerController = GetComponent<PlayerController>();


        Dexterity = playerStats.dex;

        Damage = stats.dmg;
        Rounds = stats.rnd;
        Accuracy = stats.acc*Dexterity;
        ArmorPiercing = stats.ap;
        Burst = stats.burst;
        rayDistance = stats.raNge;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)){

        }
        if (Input.GetMouseButtonDown(1))
        {
            switchweapons();
        }
    }

    public void shoot()
    {
        target = player.trg;

        Debug.Log("Shooting");
        if (target == null)
        {
            Debug.Log("No target");
            return;
        }
        Vector3 direction = (target.transform.localPosition - playerController.transform.localPosition).normalized;
        Vector3 deviatedDirection = ApplyDeviation(direction, Accuracy);

        // Fire the ray
        if (Physics.Raycast(playerController.transform.localPosition, deviatedDirection, out RaycastHit hit, rayDistance, hitLayers))
        {
            Debug.Log("Hit: " + hit.collider.name);
            Debug.DrawLine(playerController.transform.localPosition, hit.point, Color.red, 2f);
        }
        else
        {
            Debug.DrawRay(playerController.transform.localPosition, deviatedDirection * rayDistance, Color.yellow, 2f);
            Debug.Log("Ray missed.");
        }


    }

    Vector3 ApplyDeviation(Vector3 direction, float Accuracy)
    {
        Debug.Log("math stuff");
        // Get a random direction inside a cone
        return Quaternion.AngleAxis(UnityEngine.Random.Range(-Accuracy, Accuracy), Vector3.up) *
               Quaternion.AngleAxis(UnityEngine.Random.Range(-Accuracy, Accuracy), Vector3.right) * direction;
        
    }


    private void switchweapons()
    {
        
    }
}
