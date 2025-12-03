using UnityEngine;
using System.Collections;
using static UnityEngine.GraphicsBuffer;
using Unity.VisualScripting;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour, TurnSystem.ITurnActor
{
    // -----------------------------------------------------------
    // BASIC PROPERTIES
    // -----------------------------------------------------------

    // The name of this actor, required by ITurnActor interface.
    // Used mostly for logging/debugging.
    public int act { get { return actions; } }

    public string Name => "Enemy AI";

    [SerializeField] private MasterList list;
    [SerializeField] private List<PlayerController> playerList;

    //Sends the enemy stats
    public int hp { get { return health; } } //this lets other scripts get the damage of this weapon without being able to change it
    public int ap { get { return actions; } }
    public float spd { get { return speed; } }
    public int stgth { get { return strength; } }
    public float dex { get { return dexterity; } }
    public int perc { get { return perception; } }
    public int chr { get { return charisma; } }

    //Gives enemies their stats
    [SerializeField] protected int health;
    [SerializeField] protected int actions;
    [SerializeField] protected int speed;
    [SerializeField] protected int strength;
    [SerializeField] protected float dexterity;
    [SerializeField] protected int perception;
    [SerializeField] protected int charisma;

    //where the stats come from
    [SerializeField] private PlayerStats characterStats;
    [SerializeField] private WeaponStats stats;

    //Scripts
    [SerializeField] private EnemyMovement enemyMovement;
    [SerializeField] private EnemyShooting enemyShooting;
    [SerializeField] private EnemyStates enemyStates;


    //shooting stuff
    [SerializeField] Material[] mats;
    [SerializeField] private Renderer rend;

    [SerializeField] private TurnSystem turnsystem;


    private void Start()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        enemyShooting = GetComponent<EnemyShooting>();
        enemyStates = GetComponent<EnemyStates>();

        health = characterStats.hp;
        actions = characterStats.act;
        speed = characterStats.spd;
        strength = characterStats.str;
        dexterity = characterStats.dex;
        perception = characterStats.precep;
        charisma = characterStats.chr;

        rend = GetComponent<Renderer>();
        // Make sure the object has a renderer
        if (rend != null)
        {
            // Get all materials on this object
            mats = rend.materials;

            // Example: Print all material names
            foreach (Material m in mats)
            {
                Debug.Log("Material: " + m.name);
            }
        }
        else
        {
            Debug.LogWarning("No Renderer found on " + gameObject.name);
        }

    }

    public void Update()
    {
        
    }

    public void Hit(int damage, FiringCone cone) //This function takes an integer.
    {
        cone.WeaponResting();
        health -= damage;
        Debug.Log("Took Damage");
        if (health <= 0)
        {
            Debug.Log("I am Dead");
            Destroy(gameObject);
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
        if (gameObject != null)
        {
            StartCoroutine(DoEnemyAction());
        }
        else
        {
            EndTurn();
        }
        
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
        if (action == 1)
        {
            enemyShooting.callshoot();
        }
        if (action == 2)
        {
            enemyShooting.callshoot();
        }

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
    public void Outline()
    {
        Debug.Log("Calling Outline");

        Material outlineMat = mats[1];
        Debug.Log(mats[1].name + " turned on");

        outlineMat.SetFloat("_Outline_Thickness", 0.01f);
        rend.materials = mats;
    }
    public void OutlineOff()
    {
        Material outlineMat = mats[1];
        outlineMat.SetFloat("_Outline_Thickness", 0f);
    }
}
