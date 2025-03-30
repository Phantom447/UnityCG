using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerHelper : NetworkBehaviour
{
    public static PlayerHelper instance = null;
    public List<GameObject> locs = new List<GameObject>();
    public List<GameObject> opposite = new List<GameObject>();
    [SyncVar]
    public int turn;
    public bool isFirst;


    void Awake()
    {
        if (instance == null){
            print(turn);
            instance = this;
        }
    }
    //start from turn 1
    public bool isTurn(bool goFirst){
        return (turn%2==1 && goFirst) || (turn%2==0 && !goFirst);
    }
}
