using System.Collections.Generic;
using UnityEngine;

public class MasterAI : MonoBehaviour
{
    [SerializeField] private List<CaptainAI> captains = new List<CaptainAI>();

    [System.Obsolete]
    void Start()
    {
        captains.Clear();

        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            // Check for enemy scripts
            CaptainAI C = obj.GetComponent<CaptainAI>();
            if (C != null)
                captains.Add(C);
        }
        Debug.Log("Found " + captains.Count + " captain components.");


    }
}
