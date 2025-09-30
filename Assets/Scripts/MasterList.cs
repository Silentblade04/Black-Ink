using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


//Some help from chatGPT
public class MasterList : MonoBehaviour
{
    [SerializeField] private List<Enemy> enemies = new List<Enemy>();
    [SerializeField] private List<PlayerController> players = new List<PlayerController>();

    [System.Obsolete] //idk it yells at me without this

    void Start()
    {
        enemies.Clear();
        players.Clear();

        foreach (GameObject obj in Object.FindObjectsOfType<GameObject>())
        {
            // Check MyScript
            Enemy E = obj.GetComponent<Enemy>();
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

    void Update()
    {
        
    }
}
