using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Yeet.Common.Network {
  public class MessageManager {
    private YeetSystem System;

    public MessageManager(YeetSystem system) {
      System = system;

      System.Api.Network.RegisterChannel(Constants.MOD_ID)
        .RegisterMessageType<YeetEventArgs>();

      if (System.Side == EnumAppSide.Server) {
        System.ServerChannel.SetMessageHandler<YeetEventArgs>(OnServerReceivedYeetEvent);

        System.Event.OnAfterServerHandledEvent += OnAfterServerHandledEvent;
      }
      else {
        System.ClientChannel.SetMessageHandler<YeetEventArgs>(OnClientReceivedYeetEvent);

        System.Event.OnAfterInput += OnAfterInput;
      }
    }

    protected virtual void OnAfterInput(YeetEventArgs eventArgs) {
      if (!eventArgs.Successful) {
        return;
      }

      System.ClientChannel.SendPacket<YeetEventArgs>(eventArgs);
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
