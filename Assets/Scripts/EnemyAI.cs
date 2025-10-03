using UnityEngine;
using System.Collections;
using static UnityEngine.GraphicsBuffer;

public class EnemyAI : MonoBehaviour, TurnSystem.ITurnActor
{
    public string Name => "Enemy AI";

    [SerializeField] private GameObject target;
    [SerializeField] private GameObject controller;

    //Gives enemies their stats
    [SerializeField] protected int health;
    [SerializeField] protected int actions;
    [SerializeField] protected int speed;
    [SerializeField] protected int strength;
    [SerializeField] protected float dexterity;
    [SerializeField] protected int perception;
    [SerializeField] protected int charisma;

    //Enemy Weapon Stats
    [SerializeField] private int Damage;
    [SerializeField] private int Rounds;
    [SerializeField] private float Accuracy; // = dexterity times the accuracy stat of the weapon
    [SerializeField] private int ArmorPiercing;
    [SerializeField] private int Burst;
    [SerializeField] private int rayDistance;   //Range

    //where the stats come from
    [SerializeField] private PlayerStats characterStats;
    [SerializeField] private WeaponStats stats;

    [SerializeField] private object deviationAngle; // The angle of deviation
    [SerializeField] private LayerMask hitLayers;      // Layers the ray can hit


    private void Start()
    {


        health = characterStats.hp;
        actions = characterStats.act;
        speed = characterStats.spd;
        strength = characterStats.str;
        dexterity = characterStats.dex;
        perception = characterStats.precep;
        charisma = characterStats.chr;
    }

    public void shoot()
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

    public void Hit(int damage) //This function takes an integer.
    {
        health -= damage;
        Debug.Log("Took Damage");
        if (health <= 0)
        {
            gameObject.SetActive(false);
            Debug.Log("I am Dead");
        }
    }

    public void StartTurn()
    {
        Debug.Log($"{Name}'s turn started... thinking.");

        // Start a coroutine to simulate "thinking time"
        StartCoroutine(DoEnemyAction());
    }

    private IEnumerator DoEnemyAction()
    {
        // Simulate a short delay before acting
        yield return new WaitForSeconds(1f);

        int action = Random.Range(1, 6);  // ✅ Produces 1–5 inclusive
        Debug.Log($"{Name} chose action #{action}");

        // End turn after acting
        FindObjectOfType<TurnSystem>().EndTurn();
    }

    public void EndTurn()
    {
        Debug.Log($"{Name}'s turn ended.");
    }
}
