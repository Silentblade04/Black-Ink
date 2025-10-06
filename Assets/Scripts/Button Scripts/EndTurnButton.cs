using UnityEngine;
using UnityEngine.UI;
using static Unity.Collections.AllocatorManager;


public class EndTurnButton : MonoBehaviour
{
    public Button myButton;
    public TurnSystem turnSystem;

    void Start()
    {
        myButton = GetComponent<Button>();
        myButton.onClick.AddListener(MyFunction);
    }

    void MyFunction()
    {
        turnSystem.EndTurn();

    }
}
