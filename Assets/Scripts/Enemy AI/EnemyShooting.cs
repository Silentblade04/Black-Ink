using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class EnemyShooting : MonoBehaviour
{
    [SerializeField] private MasterList list;
    [SerializeField] private PlayerController target;
    [SerializeField] private Transform controller;
    [SerializeField] private GameObject player;

    [SerializeField] private List<PlayerController> targetList = new List<PlayerController>();

    //Enemy AI base
    [SerializeField] private EnemyAI aibase;

    //Enemy stats for weapons
    [SerializeField] private int dexterity;

    //Enemy Weapon Stats
    [SerializeField] private WeaponStats weaponstats;
    [SerializeField] private int Damage;
    [SerializeField] private int Rounds;
    [SerializeField] private float Accuracy; // = dexterity times the accuracy stat of the weapon
    [SerializeField] private int ArmorPiercing;
    [SerializeField] private int Burst;
    [SerializeField] private int rayDistance;   //Range

    [SerializeField] private object deviationAngle; // The angle of deviation
    [SerializeField] private LayerMask hitLayers;      // Layers the ray can hit
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<Transform>();
        aibase = GetComponent<EnemyAI>();

        Damage = weaponstats.dmg;
        Rounds = weaponstats.rnd;
        Accuracy = weaponstats.acc * aibase.dex;
        ArmorPiercing = weaponstats.ap;
        Burst = weaponstats.burst;
        rayDistance = weaponstats.raNge;
    }

    /*public void Update()
    {
        foreach (PlayerController pc in list.players)
        {
            Debug.Log("Got the list of players");
            if (Physics.Raycast(pc.transform.position, (pc.transform.position - controller.transform.position).normalized, out RaycastHit hit, rayDistance, hitLayers))
            {
                Debug.Log("Shooting Raycasts");
                if (hit.collider.CompareTag("Player"))
                {
                    PlayerController pc2 = hit.collider.GetComponent<PlayerController>();
                    targetList.Add(pc2);
                    Debug.Log(pc2.name);
                }
                else
                {
                    Debug.Log("Didn't hit the target");
                    return;
                }
            }
        }
    }*/
    public void callshoot()
    {
        //int RandIndex = Random.Range(0, targetList.Count);
        //shoot(targetList[RandIndex]);
        shoot(target);
    }

    private void shoot(PlayerController target)
    {

        Debug.Log("Shooting");
        if (target == null)
        {
            Debug.Log("No target");
            return;
        }
        Vector3 direction = (target.transform.position - controller.transform.position).normalized;

        Vector3 deviatedDirection = ApplyDeviation(direction, Accuracy);

        // Fire the ray
        if (Physics.Raycast(controller.transform.position, deviatedDirection, out RaycastHit hit, rayDistance, hitLayers))
        {
            Debug.DrawLine(controller.transform.position, hit.point, Color.red, 15f);

            if (hit.collider.CompareTag("Player")) //For when they shoot at us
            {
                Debug.Log("Hit an enemy!");
                PlayerController player = hit.collider.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.Hit(Damage);
                }

            }
            else if (hit.collider.CompareTag("Environment"))
            {
                Debug.Log("Hit an Environment!");
            }

        }
        else
        {
            Debug.DrawRay(controller.transform.position, deviatedDirection * rayDistance, Color.yellow, 15f);
            Debug.Log("Ray missed.");
        }


    }

    Vector3 ApplyDeviation(Vector3 direction, float Accuracy)
    {
        Debug.Log("math stuff");
        // Get a random direction inside a cone
        return Quaternion.AngleAxis(UnityEngine.Random.Range(-Accuracy, Accuracy), controller.transform.up) *
       Quaternion.AngleAxis(UnityEngine.Random.Range(-Accuracy, Accuracy), controller.transform.right) *
       direction;

    }
}
