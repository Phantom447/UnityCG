using Mirror;
using TMPro;
using UnityEngine;

public class turnDisplay : MonoBehaviour
{
    private TMP_Text turn;

    void Update()
    {
        turn = gameObject.GetComponent<TMP_Text>();
        turn.text = "Turn: "+PlayerHelper.instance.turn.ToString();
    }
}
