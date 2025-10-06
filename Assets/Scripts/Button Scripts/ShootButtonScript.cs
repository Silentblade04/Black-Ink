using UnityEngine;
using UnityEngine.UI;
using static Unity.Collections.AllocatorManager;

public class ShootButtonScript : MonoBehaviour
{
    public Button myButton;
    public MasterPlayer masterPlayer;

    void Start()
    {
        myButton = GetComponent<Button>();
        myButton.onClick.AddListener(MyFunction);
    }

    void MyFunction()
    {
        masterPlayer.ply.GetComponent<Weapon>().shoot();
        
    }
}
