using Mirror;
using TMPro;
using UnityEngine;

public class turnDisplay : MonoBehaviour
{
    public static turnDisplay instance = null;
    private TMP_Text turn;
    public void Start()
    {
        if (instance == null){
            instance = this;
        }
    }
    public void updateTurn(){
        turn = gameObject.GetComponent<TMP_Text>();
        turn.text = "Turn: "+PlayerHelper.instance.turn.ToString();
    }
}
