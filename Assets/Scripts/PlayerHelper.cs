using System.Collections.Generic;
using System.IO;
using Mirror;
using Steamworks;
using UnityEngine;

public class PlayerHelper : NetworkBehaviour
{
    public static PlayerHelper instance = null;
    public List<GameObject> locs = new List<GameObject>();
    public List<GameObject> opposite = new List<GameObject>();
    public List<sanityDisplay> sanityDisplays = new List<sanityDisplay>();
    public bool isFirst;
    public string hostId, clientId;
    [SyncVar (hook = nameof(newTurn))]
    public int turn;
    [SyncVar (hook = nameof(updateHostSanity))]
    public int hostSanity;
    [SyncVar (hook = nameof(updateClientSanity))]
    public int clientSanity;
    public string[] cards;


    public override void OnStartClient()
    {
        base.OnStartClient();
        clientId = SteamFriends.GetPersonaName();
        cards = File.ReadAllLines("Assets/Scripts/test.csv");
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        hostId = SteamFriends.GetPersonaName();
    }

    void Awake()
    {
        if (instance == null){
            instance = this;
        }
    }

    //start from turn 1
    public bool isTurn(bool goFirst){
        return (turn%2==1 && goFirst) || (turn%2==0 && !goFirst);
    }

    public void newTurn(int oldTurn, int newTurn){
        hostSanity = newTurn;
        clientSanity = newTurn;
        foreach (sanityDisplay display in sanityDisplays){
            display.setClientSanity(newTurn);
            display.setHostSanity(newTurn);
        }
        turnDisplay.instance.updateTurn();
    }
    public void updateClientSanity(int oldClientSanity, int newClientSanity){
        foreach (sanityDisplay display in sanityDisplays){
            display.setClientSanity(newClientSanity);
        }
    }
    public void updateHostSanity(int oldHostSanity, int newHostSanity){
        foreach (sanityDisplay display in sanityDisplays){
            display.setHostSanity(newHostSanity);
        }
    }
}
