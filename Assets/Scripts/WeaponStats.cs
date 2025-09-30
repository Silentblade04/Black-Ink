using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Dev/Weapons")] //Copied almost 1 to 1 from a group 211 project where I already had to do this

public class WeaponStats : ScriptableObject
{

    public int dmg { get {  return damage; } } //this lets other scripts get the damage of this weapon without being able to change it
    public int rnd { get { return rounds; } }
    public float acc { get { return accuracy; } }
    public int ap { get { return armorPiercing; } }
    public int burst { get { return burstNumber; } }
    public int raNge { get { return range; } }


    [SerializeField] protected int damage;          //How much damage it does
    [SerializeField] protected int rounds;        //Amount of ammo before reloading
    [SerializeField] protected float accuracy;      //Likely hood of hitting a target, multiplied by the characters accuracy/precision/dexterity
    [SerializeField] protected int armorPiercing; //level of armor it can go through
    [SerializeField] protected int burstNumber;   //# of rounds fired with each attack
    [SerializeField] protected int range;       //range


}
