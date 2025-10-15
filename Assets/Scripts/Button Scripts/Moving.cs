using UnityEngine;
using UnityEngine.UI;

public class Moving : MonoBehaviour
{
    public Button myButton;           // Reference to the UI button
    public MasterPlayer player;       // Reference to player manager (which holds the player object)

    void Start()
    {
        // Register button click event ONCE
        myButton.onClick.AddListener(OnMoveButtonPressed);
    }

    void OnMoveButtonPressed()
    {
        // Tell the player’s GridClickMovement to start moving
        player.ply.GetComponent<GridClickMovement>().StartMovement();
    }
}
