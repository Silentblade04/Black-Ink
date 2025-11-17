using UnityEngine;
//with help from GPT
public class ClassManager : MonoBehaviour
{
    Camera mainCamera; //The main player camera
    public bool needsRay;

    [Header("Abilities")]
    [Tooltip("Assign the Grenade ability component (can be on same GameObject or elsewhere).")]
    public Grenade grenadeAbility; // assign in Inspector

    void Start()
    {
        mainCamera = Camera.main; //assigns the camera
        needsRay = false;
    }

    void Update()
    {
        if (needsRay == true)
        {
            Ray mouseray = mainCamera.ScreenPointToRay(Input.mousePosition);
        }

        // Example: use G to toggle/confirm grenade ability
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (grenadeAbility != null)
            {
                grenadeAbility.ToggleOrConfirm(mainCamera);
            }
            else
            {
                Debug.LogWarning("ClassManager: grenadeAbility not assigned.");
            }
        }

        // Optional: cancel active targeting with Escape (also handled in Grenade.Update)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (grenadeAbility != null && grenadeAbility.IsTargeting())
                grenadeAbility.CancelTargeting();
        }
    }

    // kept for compatibility if you want to call abilities() manually
    public void abilities()
    {
        if (grenadeAbility != null)
            grenadeAbility.ToggleOrConfirm(mainCamera);
    }
}