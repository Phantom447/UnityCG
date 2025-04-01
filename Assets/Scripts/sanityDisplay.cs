using Mirror;
using TMPro;
using UnityEngine;

public class sanityDisplay : NetworkBehaviour
{
    private PlayerHelper PlayerHelper;
    private TMP_Text sanity;
    private string clientId, hostId;
    private bool isHost, isOwnSanity;
    private int clientSanity;
    private int hostSanity;


    public override void OnStartClient()
    {
        base.OnStartClient();
        PlayerHelper = PlayerHelper.instance;
        clientId = PlayerHelper.clientId;
        hostId = PlayerHelper.hostId;
        isHost = clientId == hostId;
        isOwnSanity = gameObject.name == "sanityDisplay";
        sanity = gameObject.GetComponent<TMP_Text>();
    }

    public void setClientSanity(int val){
        clientSanity = val;
        if ((isHost && isOwnSanity) || (!isHost && !isOwnSanity)){
            sanity.text = "Sanity: "+hostSanity.ToString();
        } else if ((!isHost && isOwnSanity) || (isHost && !isOwnSanity)){
            sanity.text = "Sanity: "+clientSanity.ToString();
        }
    }
    public void setHostSanity(int val){
        hostSanity = val;
        if ((isHost && isOwnSanity) || (!isHost && !isOwnSanity)){
            sanity.text = "Sanity: "+hostSanity.ToString();
        } else if ((!isHost && isOwnSanity) || (isHost && !isOwnSanity)){
            sanity.text = "Sanity: "+clientSanity.ToString();
        }
    }
}