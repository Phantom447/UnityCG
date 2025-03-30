using System.Collections.Generic;
using Mirror;

public class GameManager : NetworkManager
{
    public static GameManager instance = null;
    private int count;
    private PlayerManager PlayerManager;

    public override void Awake()
    {
        if (instance == null){
            instance = this;
        }
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        count = NetworkServer.connections.Count;
        if (count==2){
            NetworkIdentity networkIdentity = NetworkClient.connection.identity;
            PlayerManager = networkIdentity.GetComponent<PlayerManager>();
            PlayerManager.CmdSetTurn();
        }
    }

}
