using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Dev/PlayerStats")] //Copied almost 1 to 1 from a group 211 project where I already had to do this

public class PlayerStats : ScriptableObject
{

    public int hp { get { return health; } } //this lets other scripts get the health without other scripts modfying it.
    public int act { get { return actions; } }
    public int spd { get { return speed; } }
    public int str { get { return strength; } }
    public float dex { get { return dexterity; } }
    public int precep { get { return preception; } }
    public int chr { get { return charisma; } }


    [SerializeField] protected int health;          //How much damage you can take
    [SerializeField] protected int actions;        //# of actions you can take in a turn
    [SerializeField] protected int speed;      //Amount of tiles you can move in one movement
    [SerializeField] protected int strength; //Dictates things like carry weight
    [SerializeField] protected float dexterity;   //Dictates things like accuracy
    [SerializeField] protected int preception; //Dictates how close to things like mines you have to be to see them
    [SerializeField] protected int charisma; //Dictates likelyhood of social actions working
}
