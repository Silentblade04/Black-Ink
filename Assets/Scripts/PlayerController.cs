using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;

    [SerializeField] private int health;
    [SerializeField] private int actions;
    [SerializeField] private int speed;
    [SerializeField] private int strength;
    [SerializeField] private float dexterity;
    [SerializeField] private int perception;
    [SerializeField] private int charisma;



    void Start()
    {
        health = stats.hp;
        actions = stats.act;
        speed = stats.spd;
        strength = stats.str;
        dexterity = stats.dex;
        perception = stats.precep;
        charisma = stats.chr;
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
}
