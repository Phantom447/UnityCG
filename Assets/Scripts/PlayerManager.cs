using System;
using System.Collections.Generic;
using Mirror;
using Mirror.Examples.Basic;
using Steamworks;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    private int count;
    public GameObject card;
    public GameObject p1Bench, p2Bench;
    public GameObject drop;
    private string hostId, clientId;
    private bool coin;
    private PlayerHelper PlayerHelper;

    public override void OnStartClient(){
        base.OnStartClient();
        p1Bench = GameObject.Find("p1Bench");
        p2Bench = GameObject.Find("p2Bench");
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
            GameObject p1Card = Instantiate(card, new Vector2(0,0),Quaternion.identity);
            NetworkServer.Spawn(p1Card, connectionToClient);
            p1Card.AddComponent(typeof(Card));
            p1Card.GetComponent<Card>().id = UnityEngine.Random.Range(0,2);
            p1Card.GetComponent<Card>().init();
            RpcShowCard(p1Card, p1Card.GetComponent<Card>().hp.ToString(), p1Card.GetComponent<Card>().atk.ToString(), p1Card.GetComponent<Card>().cost.ToString());
        }
    }
    [ClientRpc]
    void RpcShowCard(GameObject card, string hp, string atk, string cost){
        card.GetComponentsInChildren<TMP_Text>()[0].text = "hp: "+hp;
        card.GetComponentsInChildren<TMP_Text>()[1].text = "atk: "+atk;
        card.GetComponentsInChildren<TMP_Text>()[2].text = "cost: "+cost;
        card.GetComponent<Card>().hp = int.Parse(hp);
        card.GetComponent<Card>().atk = int.Parse(atk);
        card.GetComponent<Card>().cost = int.Parse(cost);
        card.transform.SetParent(isOwned?p1Bench.transform:p2Bench.transform,false);
    }

    [Command]
    public void CmdPlace(GameObject dropZone, GameObject opposite, GameObject card){
        count--;
        RpcPlace(dropZone, opposite, card);
    }
    [ClientRpc]
    void RpcPlace(GameObject dropZone, GameObject opposite, GameObject card){
        List<GameObject> side = isOwned?PlayerHelper.locs:PlayerHelper.opposite;
        GameObject pos = isOwned?dropZone:opposite;
        card.transform.SetParent(pos.transform, false);
        int index = side.IndexOf(pos);
        if (index%3==0){
            if (side[index+1].transform.childCount>0){
                side[index+1].transform.GetChild(0).SetParent(side[index+2].transform);
            }
        } else if (index%3==2){
            if (side[index-1].transform.childCount>0){
                side[index-1].transform.GetChild(0).SetParent(side[index-2].transform);
            }
        }
    }

    [Command]
    public void CmdAtk(GameObject origin, GameObject target){
        int originHp, originAtk, targetHp, targetAtk;
        originHp = origin.GetComponent<Card>().hp;
        originAtk = origin.GetComponent<Card>().atk;
        targetHp = target.GetComponent<Card>().hp;
        targetAtk = target.GetComponent<Card>().atk;
        RpcAtk(origin, target, originHp, originAtk, targetHp, targetAtk);
    }
    [ClientRpc]
    void RpcAtk(GameObject origin, GameObject target, int originHp, int originAtk, int targetHp, int targetAtk){
        if ((targetHp-originAtk)<=0){
            List<GameObject> side = PlayerHelper.opposite;
            int pos = side.IndexOf(target.transform.parent.gameObject);
            if (pos==-1){
                side = PlayerHelper.locs;
                pos = PlayerHelper.locs.IndexOf(target.transform.parent.gameObject);
            }
            Destroy(target);
            int overflow = (originAtk - targetHp)/2;
            if (pos%3==0){
                if (side[pos+2].transform.childCount>0){
                    GameObject card = side[pos+2].transform.GetChild(0).gameObject;
                    int hp = card.GetComponentInChildren<Card>().hp;
                    if (hp <= overflow){
                        Destroy(card);
                    } else {
                        card.GetComponentsInChildren<TMP_Text>()[0].text = "hp: "+(hp - overflow).ToString();
                        card.GetComponent<Card>().hp -= overflow;
                        Transform parent = side[pos+1].transform;
                        card.transform.SetParent(parent);
                    }
                }
            }
        } else {
            target.GetComponentsInChildren<TMP_Text>()[0].text = "hp: "+(targetHp - originAtk).ToString();
            target.GetComponent<Card>().hp = targetHp - originAtk;
        }
        if ((originHp-targetAtk)<=0){
            Destroy(origin);
        } else {
            origin.GetComponentsInChildren<TMP_Text>()[0].text = "hp: "+(originHp - targetAtk).ToString();
            origin.GetComponent<Card>().hp = originHp - targetAtk;
        }
    }

    [Command]
    public void CmdCoinFlip(){
        RpcCoinFlip(new List<bool> {true, false}[UnityEngine.Random.Range(0,2)]);
    }
    [ClientRpc]
    void RpcCoinFlip(bool result){
        if (clientId == hostId){
            coin = result;
        } else {
            coin = !result;
        }
        PlayerHelper.isFirst = coin;
    }

    [Command]
    public void CmdUpdateClientSanity(int val){
        PlayerHelper.clientSanity += val;
    }

    [Command]
    public void CmdUpdateClientHealth(int val){
        PlayerHelper.clientHealth += val;
    }
}