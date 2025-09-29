using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;  // ✅ Add this for the new Input System

public class TurnSystem : MonoBehaviour
{
    public interface ITurnActor
    {
        string Name { get; }
        void StartTurn();
        void EndTurn();
    }

    [SerializeField] private List<MonoBehaviour> actorComponents = new List<MonoBehaviour>();
    private readonly List<ITurnActor> turnOrder = new List<ITurnActor>();
    private int currentTurnIndex = 0;

    private void Awake()
    {
        foreach (var comp in actorComponents)
        {
            if (comp is ITurnActor actor)
                turnOrder.Add(actor);
            else
                Debug.LogWarning($"{comp.name} does not implement ITurnActor and was skipped.");
        }
    }

    private void Start()
    {
        if (turnOrder.Count == 0)
        {
            Debug.LogError("TurnSystem has no participants!");
            return;
        }

        currentTurnIndex = 0;
        BeginCurrentTurn();
    }

    private void Update()
    {
        // ✅ New Input System way to detect SPACEBAR
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            EndTurn();
        }
    }

    private void BeginCurrentTurn()
    {
        ITurnActor current = turnOrder[currentTurnIndex];
        Debug.Log($"--- {current.Name}'s Turn ---");
        current.StartTurn();
    }

    public void EndTurn()
    {
        ITurnActor current = turnOrder[currentTurnIndex];
        current.EndTurn();

        currentTurnIndex = (currentTurnIndex + 1) % turnOrder.Count;
        BeginCurrentTurn();
    }
}
