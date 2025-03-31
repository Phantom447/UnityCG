using Mirror;
using Mirror.Examples.Basic;

public class drawCard : NetworkBehaviour
{
    private PlayerManager PlayerManager;

    public void onClick(){
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        PlayerManager = networkIdentity.GetComponent<PlayerManager>();
        if (PlayerHelper.instance.clientId == PlayerHelper.instance.hostId){
            PlayerHelper.instance.hostSanity +=1;
        }else {
            PlayerHelper.instance.clientSanity +=1;
        }
        PlayerManager.CmdDealCards(PlayerHelper.instance.isFirst);
    }
}
