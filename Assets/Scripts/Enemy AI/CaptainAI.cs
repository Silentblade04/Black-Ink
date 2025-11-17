using UnityEngine;

public class CaptainAI : MonoBehaviour
{
    [SerializeField] private EnemyAI baseAI;


    private void Start()
    {
        baseAI = GetComponent<EnemyAI>();
    }
}
