using Mirror;
using Mirror.Examples.Basic;
using TMPro;
using UnityEngine;

public class sanityDisplay : NetworkBehaviour
{
    private PlayerHelper PlayerHelper;
    private TMP_Text sanity;
    private string clientId, hostId;
    private bool isHost, isOwnSanity;

    public override void OnStartClient()
    {
        base.OnStartClient();
        PlayerHelper = PlayerHelper.instance;
        clientId = PlayerHelper.clientId;
        hostId = PlayerHelper.hostId;
        isHost = clientId == hostId;
        isOwnSanity = gameObject.name == "sanityDisplay";
        print(gameObject);
        print(isHost);
        print(isOwnSanity);
    }
    void Update()
    {
        sanity = gameObject.GetComponent<TMP_Text>();
        if ((isHost && isOwnSanity) || (!isHost && !isOwnSanity)){
            sanity.text = "Sanity: "+PlayerHelper.hostSanity.ToString();
        } else {
            sanity.text = "Sanity: "+PlayerHelper.clientSanity.ToString();
        }
    }
}
