using UnityEngine;

public class FiringCone : MonoBehaviour
{
    [SerializeField] private GameObject firingCone;
    [SerializeField] private int range;
    [SerializeField] private float accuracy;

    [SerializeField] private Weapon weapon;
    [SerializeField] private PlayerController playerController;

    [SerializeField] private Material mat;
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        Material mat = firingCone.GetComponent<Material>();
        mat.SetFloat("_Alpha", 0);
        
    }

    public void WeaponSwap()
    {
        weapon = GetComponent<Weapon>();
        if (weapon != null) {
            accuracy = 0;
            range = 0;
        }
        else
        {
            accuracy = weapon.acc;
            range = weapon.rng;
        }
    }

    public void WeaponAiming()
    {
        mat.SetFloat("_Alpha", 0.1f);
    }

    public void WeaponResting()
    {
        mat.SetFloat("_Alpha", 0);
    }

    void Update()
    {
        firingCone.transform.localPosition = Vector3.zero;
        firingCone.transform.localRotation = Quaternion.identity;

        float endRadius = Mathf.Tan(accuracy * Mathf.Deg2Rad) * range;
        firingCone.transform.localScale = new Vector3(endRadius * 2, range, endRadius * 2);


    }
}
