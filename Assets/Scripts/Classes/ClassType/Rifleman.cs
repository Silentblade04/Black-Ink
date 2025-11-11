using UnityEngine;
using UnityEngine.UI;
//with GPT assistance
public class Rifleman : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;
    public Grenade grenadeAbility;            // Assign in Inspector
    public ShootButtonScript shootButton;     // Assign the UI shoot button here
    public MasterPlayer masterPlayer;         // Reference to player that holds Weapon

    void Start()
    {
        // Default to main camera if not set
        if (mainCamera == null)
            mainCamera = Camera.main;

        // Hook up ShootButtonScript so it fires Rifleman’s shoot logic
        if (shootButton != null)
        {
            shootButton.myButton.onClick.RemoveAllListeners();
            shootButton.myButton.onClick.AddListener(FireTwice);
        }
        else
        {
            Debug.LogWarning("Rifleman: No ShootButtonScript assigned.");
        }
    }

    void Update()
    {
        // Press G to use grenade ability
        if (Input.GetKeyDown(KeyCode.G))
        {
            UseGrenade();
        }

        // Optional cancel ability
        if (Input.GetKeyDown(KeyCode.Escape) && grenadeAbility != null && grenadeAbility.IsTargeting())
        {
            grenadeAbility.CancelTargeting();
        }
    }

    public void UseGrenade()
    {
        if (grenadeAbility != null)
        {
            grenadeAbility.ToggleOrConfirm(mainCamera);
        }
        else
        {
            Debug.LogWarning("Rifleman: grenadeAbility not assigned.");
        }
    }

    public void FireTwice()
    {
        if (masterPlayer != null && masterPlayer.ply != null)
        {
            var weapon = masterPlayer.ply.GetComponent<Weapon>();
            if (weapon != null)
            {
                weapon.shoot();
                weapon.shoot();  // Fire second shot
            }
            else
            {
                Debug.LogWarning("Rifleman: No Weapon component found on player.");
            }
        }
        else
        {
            Debug.LogWarning("Rifleman: masterPlayer or ply not assigned.");
        }
    }
}
