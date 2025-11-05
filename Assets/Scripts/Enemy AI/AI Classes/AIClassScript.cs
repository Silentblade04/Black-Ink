using UnityEngine;

[CreateAssetMenu(fileName = "AI", menuName = "Dev/AI")] //Copied almost 1 to 1 from a group 211 project where I already had to do this

public class AIClassScript : ScriptableObject
{

   [SerializeField] private Rank rank;
}
