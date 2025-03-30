using Mirror;

public class drawCard : NetworkBehaviour
{
    private PlayerManager PlayerManager;

    public void onClick(){
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        PlayerManager = networkIdentity.GetComponent<PlayerManager>();
        PlayerManager.CmdDealCards(PlayerHelper.instance.isFirst);
    }
}
