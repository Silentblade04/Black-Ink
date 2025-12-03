using UnityEngine;

public class Environment : MonoBehaviour
{
    [SerializeField] private EnvironmentAttributes attributes;
    public int hrd { get { return hardness; } }

    [SerializeField] private Weapon wpn;

    [SerializeField] private float mass;
    [SerializeField] private int hardness;
    [SerializeField] private bool destructuble;
    [SerializeField] private bool transparent;

    private void Start()
    {
        mass = attributes.mass;
        hardness = attributes.hrdness;
        destructuble = attributes.breakable;
        transparent = attributes.cthrugh;
    }

    public void passthrough(int piercing, Weapon weapon, RaycastHit hit, Vector3 deviatedDirection)
    {
        if (piercing > hardness)
        {
            weapon.passthrough(hit, deviatedDirection);
        }
    }
    public void hit(int piercing)
    {
        if(piercing >= hardness && destructuble == true)
        {
           Destroy(gameObject);
        }
        else
        {
            Debug.Log("Unbreakable Object");
        }
    }
}
