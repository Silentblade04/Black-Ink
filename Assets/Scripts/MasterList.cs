using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


//Some help from chatGPT
public class MasterList : MonoBehaviour
{
    [SerializeField] private List<EnemyAI> enemies = new List<EnemyAI>();
    [SerializeField] private List<PlayerController> players = new List<PlayerController>();

    [SerializeField] private int actionAmount;

    [SerializeField] private TurnSystem turnSystem;

    [System.Obsolete] //idk it yells at me without this

    void Start()
    {
        enemies.Clear();
        players.Clear();

        foreach (GameObject obj in Object.FindObjectsOfType<GameObject>())
        {
            // Check MyScript
            EnemyAI E = obj.GetComponent<EnemyAI>();
            if (E != null)
                enemies.Add(E);

            // Check OtherScript
            PlayerController PC = obj.GetComponent<PlayerController>();
            if (PC != null)
                players.Add(PC);
        }
        Debug.Log("Found " + enemies.Count + " Enemy components.");
        Debug.Log("Found " + players.Count + " Player components.");
    }

    public void OnTurnStart()
    {

        foreach (PlayerController pc in players)
        {
            pc.ActionAdd(pc.act); 
        }
    }
}
