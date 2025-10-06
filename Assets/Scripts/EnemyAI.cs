using UnityEngine;
using System.Collections;
using static UnityEngine.GraphicsBuffer;

public class EnemyAI : MonoBehaviour, TurnSystem.ITurnActor
{
    // -----------------------------------------------------------
    // BASIC PROPERTIES
    // -----------------------------------------------------------

    // The name of this actor, required by ITurnActor interface.
    // Used mostly for logging/debugging.
    public int act { get { return actions; } }

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
    // -----------------------------------------------------------
    // TURN FLOW METHODS
    // -----------------------------------------------------------

    // Called by TurnSystem at the start of this actor’s turn.
    public void StartTurn()
    {
        Debug.Log($"{Name}'s turn started... thinking.");

        // Start a coroutine to simulate some "thinking time"
        // before the AI takes its action.
        StartCoroutine(DoEnemyAction());
    }

    // Coroutine to simulate AI delay and perform an action.
    private IEnumerator DoEnemyAction()
    {
        // This is the section that Would be changed as we want to change what the AI does during their turn
        // Wait for 1 second to simulate calculation or animation delay
        yield return new WaitForSeconds(1f);

        // Pick a random number between 1 and 5 inclusive
        int action = Random.Range(1, 6);  // ✅ Produces 1–5 inclusive
        Debug.Log($"{Name} chose action #{action}");

        // Notify the TurnSystem that the enemy has finished its turn.
        // Using the modern method to find the TurnSystem in the scene.
        var turnSystem = FindFirstObjectByType<TurnSystem>();
        if (turnSystem != null)
        {
            turnSystem.EndTurn();  // End the AI’s turn and pass control to next actor
        }
        else
        {
            Debug.LogError("TurnSystem not found in the scene!");
        }
    }

    // Called by TurnSystem at the end of this actor’s turn.
    public void EndTurn()
    {
        Debug.Log($"{Name}'s turn ended.");
    }
}
