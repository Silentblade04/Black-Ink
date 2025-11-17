using UnityEngine;

//with GPT assistance
/// <summary>
/// Assigns a player class script (e.g., Rifleman) to the player object at the start of the game.
/// Future classes can be added easily to the switch or via a list.
/// </summary>
public class ClassCheck : MonoBehaviour
{
    [Header("Player Setup")]
    [Tooltip("Assign the player GameObject to receive the class script.")]
    public GameObject playerObject;

    [Header("Class Selection")]
    [Tooltip("Select which class to assign at start.")]
    public string selectedClass = "Rifleman";

    [Header("Shared References")]
    public Camera mainCamera;
    public Grenade grenadeAbility;
    public ShootButtonScript shootButton;
    public MasterPlayer masterPlayer;

    void Start()
    {
        if (playerObject == null)
        {
            Debug.LogError("ClassCheck: No player object assigned!");
            return;
        }

        AssignClass(selectedClass);
    }

    void AssignClass(string className)
    {
        switch (className)
        {
            case "Rifleman":
                Rifleman rifleman = playerObject.AddComponent<Rifleman>();
                rifleman.mainCamera = mainCamera != null ? mainCamera : Camera.main;
                rifleman.grenadeAbility = grenadeAbility;
                rifleman.shootButton = shootButton;
                rifleman.masterPlayer = masterPlayer;
                Debug.Log("ClassCheck: Rifleman class assigned.");
                break;

            // Future classes can be added here
            // case "Medic":
            //     playerObject.AddComponent<Medic>();
            //     break;

            default:
                Debug.LogWarning($"ClassCheck: Unknown class '{className}'. No class assigned.");
                break;
        }
    }
}
