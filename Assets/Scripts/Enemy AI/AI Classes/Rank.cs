using UnityEngine;

[CreateAssetMenu(fileName = "Rank", menuName = "Dev/AI")] //Copied almost 1 to 1 from a group 211 project where I already had to do this

public class Rank : ScriptableObject
{
    public string rnk { get { return rank; } } 

    [SerializeField] private string rank;
}
