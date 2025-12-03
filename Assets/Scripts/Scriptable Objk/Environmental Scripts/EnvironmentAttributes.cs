using UnityEngine;

[CreateAssetMenu(fileName = "EnvironmentAttribute", menuName = "Dev/EnvironmentalAttribute")] //Copied almost 1 to 1 from a group 211 project where I already had to do this

public class EnvironmentAttributes : ScriptableObject
{
    public float mass { get { return mKG; } }
    public int hrdness { get { return hardness; } }
    public bool breakable { get { return destructable; } }
    public bool cthrugh { get { return transperency; } }


    [Tooltip("Mass of an object in kilograms")]
    [SerializeField] private float mKG; //mass in kilograms

    [Tooltip("Strength of an object")]
    [SerializeField] private int hardness; //will act as the strength required to break

    [Tooltip("Can the object be destroyed")]
    [SerializeField] private bool destructable; //can the object be destroyed

    [Tooltip("Is the object seethrough")]
    [SerializeField] private bool transperency; //can the object be seen through
}
