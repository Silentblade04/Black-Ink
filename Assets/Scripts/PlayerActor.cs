using UnityEngine;

public class PlayerActor : MonoBehaviour, TurnSystem.ITurnActor
{
    public string Name => "Player";

    public void StartTurn()
    {
        Debug.Log("Player’s turn started: enable input here");
    }

    public void EndTurn()
    {
        Debug.Log("Player’s turn ended");
    }
}
