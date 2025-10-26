using UnityEngine;

public class FiringCone : MonoBehaviour
{
    [SerializeField] private GameObject firingCone;
    [SerializeField] private int range;
    [SerializeField] private float accuracy;

    [SerializeField] private Weapon weapon;
    [SerializeField] private PlayerController playerController;

    [SerializeField] private GameObject player; //must be manually assigned

    [SerializeField] Material[] mats;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        WeaponResting();
    }



    public void WeaponSwap()
    {
        weapon = GetComponent<Weapon>();
        if (weapon == null) {
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
        Material coneMaterial = mats[0];
        coneMaterial.SetFloat("_Alpha", 0.1f);
        WeaponSwap();

    }

    public void WeaponResting()
    {
        Material coneMaterial = mats[0];
        coneMaterial.SetFloat("_Alpha", 0f);
    }

    void Update()
    {
        firingCone.transform.localPosition = new Vector3(0, 0, range/2);

        float endRadius = Mathf.Tan(accuracy * Mathf.Deg2Rad) * range;
        firingCone.transform.localScale = new Vector3(endRadius * 2, range, endRadius * 2);


    }

}
