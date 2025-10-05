using System.Collections.Generic;         // Allows us to use List<T> to store actors
using UnityEngine;
using UnityEngine.InputSystem;            // Needed to use the new Input System (Keyboard class)

public class TurnSystem : MonoBehaviour
{
    // Assistance from ChatGPT
    // -----------------------------------------------------------
    // INTERFACE: ITurnActor
    // -----------------------------------------------------------
    // Any class that wants to participate in the turn system must
    // implement this interface, guaranteeing it has a Name and
    // knows how to start and end its turn.
    public interface ITurnActor
    {
        string Name { get; }               // Used for logging/debugging the actor’s name
        void StartTurn();                  // Called when this actor’s turn begins
        void EndTurn();                    // Called when this actor’s turn ends
    }

    // -----------------------------------------------------------
    // INSPECTOR FIELDS
    // -----------------------------------------------------------

    // This list appears in the Inspector. You drag in any GameObjects
    // that have a component which implements ITurnActor.
    [SerializeField] private List<MonoBehaviour> actorComponents = new List<MonoBehaviour>();

    // This is the internal list that stores all participants as ITurnActor.
    // Using the interface type lets us treat all actors uniformly
    // whether they’re players, enemies, or AI.
    private readonly List<ITurnActor> turnOrder = new List<ITurnActor>();

    // Tracks which actor’s turn it currently is (index in turnOrder list).
    private int currentTurnIndex = 0;

    // -----------------------------------------------------------
    // UNITY LIFECYCLE METHODS
    // -----------------------------------------------------------

    // Awake() is called when the GameObject is first loaded.
    // Here we convert any MonoBehaviours in actorComponents into ITurnActor
    // and populate the turnOrder list.
    private void Awake()
    {
        foreach (var comp in actorComponents)
        {
            // Check if this component implements ITurnActor
            if (comp is ITurnActor actor)
                turnOrder.Add(actor);      // Add valid actor to the turn order
            else
                Debug.LogWarning($"{comp.name} does not implement ITurnActor and was skipped.");
        }
    }

    // Start() is called before the first frame update.
    // We check that we have participants and then begin the first turn.
    private void Start()
    {
        if (turnOrder.Count == 0)
        {
            Debug.LogError("TurnSystem has no participants!");
            return;                         // Stops the game if no actors were added
        }

        currentTurnIndex = 0;               // Start with the first actor in the list
        BeginCurrentTurn();                 // Kick off the first turn
    }

    // Update() is called every frame.
    // We use it to check for input to manually end a turn with the SPACEBAR.
    private void Update()
    {
        //New Input System way to detect SPACEBAR being pressed in the current frame
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            EndTurn();                      // Manually end the current turn
        }
    }

    public bool IsPlayerTurn()
    {
        ITurnActor current = turnOrder[currentTurnIndex];

        // Option 1: Check by tag (if your player GameObject has a "Player" tag)
        if (current is MonoBehaviour mb && mb.CompareTag("Player"))
            return true;

        // Option 2: Check by interface, if you make a special IPlayerActor marker interface
        // return current is IPlayerActor;

        return false;
    }

    // -----------------------------------------------------------
    // TURN FLOW METHODS
    // -----------------------------------------------------------

    // BeginCurrentTurn() notifies the current actor that it’s their turn.
    private void BeginCurrentTurn()
    {
        ITurnActor current = turnOrder[currentTurnIndex]; // Get the current actor
        Debug.Log($"--- {current.Name}'s Turn ---");       // Log for debugging
        current.StartTurn();                               // Tell the actor to start their turn
    }

    // EndTurn() is called to end the current actor’s turn.
    // It’s public so it can also be triggered by player input,
    // AI scripts, or UI buttons.
    public void EndTurn()
    {
        ITurnActor current = turnOrder[currentTurnIndex]; // Get the current actor
        current.EndTurn();                                 // Notify them their turn has ended

        // Advance to the next actor in the list.
        // The modulo (%) wraps back to 0 after reaching the last actor.
        currentTurnIndex = (currentTurnIndex + 1) % turnOrder.Count;

        // Start the next actor’s turn.
        BeginCurrentTurn();
    }
}
