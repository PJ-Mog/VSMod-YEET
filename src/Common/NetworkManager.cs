using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Yeet.Common {
  public class NetworkManager {
    protected YeetSystem System { get; }
    protected IClientNetworkChannel ClientChannel { get; set; }
    protected IServerNetworkChannel ServerChannel { get; set; }

    public NetworkManager(YeetSystem system) {
      System = system;

      var channel = System.Api.Network.RegisterChannel(Constants.MOD_ID);
      channel.RegisterMessageType<YeetEventArgs>();

      if (System.Side == EnumAppSide.Server) {
        ServerChannel = channel as IServerNetworkChannel;
        ServerChannel.SetMessageHandler<YeetEventArgs>(OnServerReceivedYeetEvent);

        system.Event.OnAfterServerHandledEvent += OnAfterServerHandledEvent;
      }
      else {
        ClientChannel = channel as IClientNetworkChannel;
        ClientChannel.SetMessageHandler<YeetEventArgs>(OnClientReceivedYeetEvent);

        system.Event.OnAfterInput += OnAfterInput;
      }
    }

    protected virtual void OnAfterInput(YeetEventArgs eventArgs) {
      if (!eventArgs.Successful) {
        return;
      }

      ClientChannel.SendPacket<YeetEventArgs>(eventArgs);
    }

    protected virtual void OnServerReceivedYeetEvent(IServerPlayer forPlayer, YeetEventArgs eventArgs) {
      eventArgs.ForPlayer = forPlayer;
      System.Event.TriggerServerReceivedYeetEvent(eventArgs);
    }

    protected virtual void OnAfterServerHandledEvent(YeetEventArgs eventArgs) {
      System.ServerChannel.SendPacket<YeetEventArgs>(eventArgs, eventArgs.ForPlayer);
    }

    protected virtual void OnClientReceivedYeetEvent(YeetEventArgs eventArgs) {
      System.Event.TriggerClientReceivedYeetEvent(eventArgs);
    }
  }
}
