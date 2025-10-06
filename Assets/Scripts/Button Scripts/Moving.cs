using UnityEngine;
using UnityEngine.UI;

public class Moving : MonoBehaviour
{
    public Button myButton;
    public MasterPlayer player;
    private GridClickMovement GridClickMovement;

    void Start()
    {
        myButton = GetComponent<Button>();
        myButton.onClick.AddListener(MyFunction);
    }

    void MyFunction()
    {
        player.ply.GetComponent<GridClickMovement>().MoveAlongPath();
    }
}
