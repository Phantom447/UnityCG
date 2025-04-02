using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class dragDrop : NetworkBehaviour
{
    private bool isDragging;
    private bool overDrop;
    private GameObject dropZone;
    private int count;
    private bool canDrag, isPlaced;
    private List<GameObject> locs = new List<GameObject>();
    private List<GameObject> opposite = new List<GameObject>();
    private GameObject Canvas;
    private GameObject startParent;
    private PlayerManager PlayerManager;
    private PlayerHelper PlayerHelper;
    private GameObject target;

    public void Awake(){
        Canvas = GameObject.Find("Main Canvas");
    }
    void Start(){
        canDrag = isOwned;
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        PlayerManager = networkIdentity.GetComponent<PlayerManager>();
        PlayerHelper = PlayerHelper.instance;
        locs = PlayerHelper.locs;
        opposite = PlayerHelper.opposite;
    }
    void Update(){
        if (isDragging){
            transform.position = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
            transform.SetParent(Canvas.transform, true);
        }
    }

    private bool isFront(GameObject pos, bool isEnemy){
        if (isEnemy){
            if (opposite.IndexOf(pos)%2==1){
                return opposite[opposite.IndexOf(pos)-1].transform.childCount<1;
            }
        } else {
            if (locs.IndexOf(pos)%2==1){
                return locs[locs.IndexOf(pos)-1].transform.childCount<1;
            }
        }
        return true;
    }
    private GameObject getTarget(GameObject origin){
        GameObject target = null;
        int pos = (int)Math.Floor((double)locs.IndexOf(origin)/2);
        for (int i = pos*2;i<(pos*2)+2;i++){
            target = opposite[i];
            if (target != null && target.transform.childCount>0){return target;}
        }
        return target;
    }

    private void OnCollisionEnter2D(Collision2D collision){
        count++;
        overDrop = true;
        dropZone = collision.gameObject;
    }
    private void OnCollisionExit2D(Collision2D collision){
        count--;
        if (count==0){
            overDrop = false;
            dropZone = null;
        }
    }

    public void startDrag(){
        if (PlayerHelper.instance.isTurn(PlayerHelper.instance.isFirst)){
            if (isPlaced){
                if (isFront(transform.parent.gameObject, false)){
                    startParent = transform.parent.gameObject;
                    target = getTarget(transform.parent.gameObject);
                    if (target.transform.childCount>=1){
                        target.transform.GetChild(0).gameObject.GetComponent<Outline>().enabled = true;
                        isDragging = true;
                    }
                }
            } else {
                if (canDrag && ((PlayerHelper.clientId == PlayerHelper.hostId)?PlayerHelper.hostSanity:PlayerHelper.clientSanity) >= gameObject.GetComponent<Card>().cost){
                    startParent = transform.parent.gameObject;
                    isDragging = true;
                }
            }
        }
    }
    public void endDrag(){
        if (isDragging){
            isDragging = false;
            if (isPlaced){
                target.transform.GetChild(0).gameObject.GetComponent<Outline>().enabled = false;
                if (overDrop && dropZone==target){
                    PlayerManager.CmdAtk(transform.gameObject, target.transform.GetChild(0).gameObject);
                    target = null;
                }
                transform.SetParent(startParent.transform, false);
            } else{
                if (overDrop && opposite.IndexOf(dropZone)==-1){
                    if (dropZone.transform.childCount<1){
                        canDrag = false;
                        isPlaced = true;
                        if (isOwned){
                            if (PlayerHelper.clientId == PlayerHelper.hostId){
                                PlayerHelper.hostSanity -= gameObject.GetComponent<Card>().cost;
                            } else {
                                //using Cmd because SyncVar has to be updated on host
                                PlayerManager.CmdUpdateClientSanity(-gameObject.GetComponent<Card>().cost);
                            }
                        }
                        PlayerManager.CmdPlace(dropZone, opposite[locs.IndexOf(dropZone)], gameObject);
                    } else {
                        transform.SetParent(startParent.transform, false);
                    }
                } else {
                    transform.SetParent(startParent.transform, false);
                }
            }
        }
    }
}
