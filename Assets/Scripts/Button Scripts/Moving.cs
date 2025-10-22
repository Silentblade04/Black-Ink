using UnityEngine;
using UnityEngine.UI;

public class Moving : MonoBehaviour
{
    public Button myButton;           // Reference to the UI button
    public MasterPlayer player;       // Reference to player manager (which holds the player object)
    // Add a reference to the GridHighlighter
    public GridHighlighter gridHighlighter;

    void Start()
    {
        // Register button click event ONCE
        myButton.onClick.AddListener(OnMoveButtonPressed);
    }

    void OnMoveButtonPressed()
    {
        // Hide the existing grid highlight
        if (gridHighlighter != null)
        {
            gridHighlighter.Hide();
        }
        else
        {
            Debug.LogWarning("Moving: GridHighlighter not assigned.");
        }
        // Tell the player’s GridClickMovement to start moving
        player.ply.GetComponent<GridClickMovement>().StartMovement();
    }
}
