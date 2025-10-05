using UnityEngine;

public class PlayerActor : MonoBehaviour, TurnSystem.ITurnActor
{
    // -----------------------------------------------------------
    // BASIC PROPERTIES
    // -----------------------------------------------------------

    // The name of this actor.
    // This satisfies the ITurnActor interface requirement
    // and is mainly used for debugging/logging.
    public string Name => "Player";

    // -----------------------------------------------------------
    // TURN FLOW METHODS
    // -----------------------------------------------------------

    // Called by the TurnSystem whenever the player’s turn begins.
    public void StartTurn()
    {
        // This is where you’d enable player input controls,
        // highlight selectable units, or show the player's UI.
        Debug.Log("Player’s turn started: enable input here");
    }

    // Called by the TurnSystem whenever the player’s turn ends.
    public void EndTurn()
    {
        // This is where you could disable input controls,
        // hide the UI, or trigger end-of-turn effects.
        Debug.Log("Player’s turn ended");
    }
}
