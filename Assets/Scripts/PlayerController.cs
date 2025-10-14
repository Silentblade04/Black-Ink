using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;

    public int act { get { return actions; } }
    public int actLeft { get { return fluxActions; } }

    [SerializeField] private int health; //This sets the base HP. Also works as the maximum HP
    [SerializeField] private int currentHealth;// As they take damage this number goes down
    [SerializeField] private int actions; //This sets the number of actions they can take each turn
    [SerializeField] private int fluxActions; //This is the number that actually goes down as they take actions
    [SerializeField] private int speed; //The # of tiles that they can move per action point
    [SerializeField] private int strength; //Determins things like carrying capacity 
    [SerializeField] private float dexterity; //Influences accuracy
    [SerializeField] private int perception; //influences range that traps and enemies can bee seen
    [SerializeField] private int charisma; //Influences social skill checks



    void Start()
    {
        health = stats.hp;
        currentHealth = health;
        actions = stats.act;
        fluxActions = actions;
        speed = stats.spd;
        strength = stats.str;
        dexterity = stats.dex;
        perception = stats.precep;
        charisma = stats.chr;
    }
    public void Update()
    {
        
    }
    public void ActionAdd(int actions)
    {
        fluxActions = actions;
    }
    
    public void ActionUsed(int actionCost)
    {
        fluxActions -= actionCost;
    }

    public void Hit(int damage) //This function takes an integer.
    {
        currentHealth -= damage;
        Debug.Log("Took Damage");
        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
            Debug.Log("I am Dead");
        }
    }
    public void heal(int heal) //also takes an integer
    {
        currentHealth += heal;
        if (currentHealth >= health)
        {
            currentHealth = health;
        }

    }
    
}
