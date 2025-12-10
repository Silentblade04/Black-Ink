using UnityEngine;
using UnityEngine.UI;

public class Moving : MonoBehaviour
{
    public Button myButton;
    public MasterPlayer player;
    public GridHighlighter gridHighlighter;

    void Start()
    {
        myButton.onClick.AddListener(OnMoveButtonPressed);
    }

    void OnMoveButtonPressed()
    {
        if (gridHighlighter != null)
            gridHighlighter.Hide();

        player.BeginMovementMode();
    }
}
