using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour, TurnSystem.ITurnActor
{
    public string Name => "Enemy AI";

    public void StartTurn()
    {
        Debug.Log($"{Name}'s turn started... thinking.");

        // Start a coroutine to simulate "thinking time"
        StartCoroutine(DoEnemyAction());
    }

    private IEnumerator DoEnemyAction()
    {
        // Simulate a short delay before acting
        yield return new WaitForSeconds(1f);

        int action = Random.Range(1, 6);  // ✅ Produces 1–5 inclusive
        Debug.Log($"{Name} chose action #{action}");

        // End turn after acting
        FindObjectOfType<TurnSystem>().EndTurn();
    }

    public void EndTurn()
    {
        Debug.Log($"{Name}'s turn ended.");
    }
}
