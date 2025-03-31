using System.Collections.Generic;
using Mirror;
using Steamworks;
using TMPro;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    private int count;
    public GameObject card1;
    public GameObject p1, p2;
    public GameObject drop;
    private string hostId, clientId;
    private bool coin;
    private PlayerHelper PlayerHelper;

    public override void OnStartClient(){
        base.OnStartClient();
        p1 = GameObject.Find("p1");
        p2 = GameObject.Find("p2");
        PlayerHelper = PlayerHelper.instance;
        clientId = PlayerHelper.clientId;
    }
    [Server]
    public override void OnStartServer(){
        base.OnStartServer();
        PlayerHelper = PlayerHelper.instance;
        hostId = PlayerHelper.hostId;
    }

    [Command]
    public void CmdDealCards(bool isFirst){
        if (count<5 && PlayerHelper.isTurn(isFirst)){
            count++;
            PlayerHelper.turn++;
            GameObject p1Card = Instantiate(card1, new Vector2(0,0),Quaternion.identity);
            NetworkServer.Spawn(p1Card, connectionToClient);
            RpcShowCard(p1Card, UnityEngine.Random.Range(1,10).ToString(), UnityEngine.Random.Range(1,10).ToString());
        }
    }
    [ClientRpc]
    void RpcShowCard(GameObject card, string hp, string atk){
        card.GetComponentsInChildren<TMP_Text>()[0].text = hp;
        card.GetComponentsInChildren<TMP_Text>()[1].text = atk;
        card.transform.SetParent(isOwned?p1.transform:p2.transform,false);
    }

    [Command]
    public void CmdPlace(GameObject dropZone, GameObject opposite, GameObject card){
        count--;
        RpcPlace(dropZone, opposite, card);
    }
    [ClientRpc]
    void RpcPlace(GameObject dropZone, GameObject opposite, GameObject card){
        card.transform.SetParent(isOwned?dropZone.transform:opposite.transform, false);
    }

    [Command]
    public void CmdAtk(GameObject origin, GameObject target){
        int originHp, originAtk, targetHp, targetAtk;
        originHp = int.Parse(origin.GetComponentsInChildren<TMP_Text>()[0].text);
        originAtk = int.Parse(origin.GetComponentsInChildren<TMP_Text>()[1].text);
        targetHp = int.Parse(target.GetComponentsInChildren<TMP_Text>()[0].text);
        targetAtk = int.Parse(target.GetComponentsInChildren<TMP_Text>()[1].text);
        RpcAtk(origin, target, originHp, originAtk, targetHp, targetAtk);
    }
    [ClientRpc]
    void RpcAtk(GameObject origin, GameObject target, int originHp, int originAtk, int targetHp, int targetAtk){
        target.GetComponentsInChildren<TMP_Text>()[0].text = (targetHp - originAtk).ToString();
        if ((targetHp-originAtk)<=0){Destroy(target);}
        origin.GetComponentsInChildren<TMP_Text>()[0].text = (originHp - targetAtk).ToString();
        if ((originHp-targetAtk)<=0){Destroy(origin);}
    }

    [Command]
    public void CmdSetTurn(){
        RpcSetTurn(new List<bool> {true, false}[UnityEngine.Random.Range(0,2)]);
    }
    [ClientRpc]
    void RpcSetTurn(bool result){
        if (clientId == hostId){
            coin = result;
        } else {
            coin = !result;
        }
        PlayerHelper.isFirst = coin;
    }
}