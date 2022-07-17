using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Yeet.Common.Network {
  public class MessageManager {
    private YeetSystem System;

    public MessageManager(YeetSystem system) {
      System = system;

      System.Api.Network.RegisterChannel(Constants.MOD_ID)
        .RegisterMessageType(typeof(YeetPacket))
        .RegisterMessageType(typeof(YeetSuccessPacket))
        .RegisterMessageType(typeof(YeetCanceledPacket));
      
      if (System.Side == EnumAppSide.Server) {
        System.ServerChannel.SetMessageHandler<YeetPacket>(OnReceivedYeetRequest);

        System.Event.ItemYeeted += OnItemYeetedServer;
        System.Event.YeetCanceled += OnYeetCanceledServer;
      }
      else {
        System.ClientChannel.SetMessageHandler<YeetSuccessPacket>(OnItemYeetedClient);
        System.ClientChannel.SetMessageHandler<YeetCanceledPacket>(OnYeetCanceledClient);
      }
    }

    public void RequestYeet(IClientPlayer player, EnumYeetType yeetType, EnumYeetSlotType yeetSlot, double force) {
      System.ClientChannel.SendPacket(new YeetPacket(player, yeetType, yeetSlot, force));
      System.Event.StartClientRequestedYeet(player, yeetType, yeetSlot, force);
    }

    public void OnReceivedYeetRequest(IServerPlayer forPlayer, YeetPacket yeetData) {
      System.Event.StartYeetRequestReceived(forPlayer, yeetData);
    }

    public void OnYeetCanceledServer(IPlayer player, string errorCode) {
      var serverPlayer = player as IServerPlayer;
      if (serverPlayer != null) {
        System.ServerChannel.SendPacket(new YeetCanceledPacket(errorCode), serverPlayer);
      }
    }

    public void OnYeetCanceledClient(YeetCanceledPacket packet) {
      System.Event.StartYeetCanceled(System.ClientAPI.World.Player, packet.ErrorCode);
    }

    public void OnItemYeetedServer(IPlayer player) {
      var serverPlayer = player as IServerPlayer;
      if (serverPlayer != null) {
        System.ServerChannel.SendPacket(new YeetSuccessPacket(), serverPlayer);
      }
    }

    public void OnItemYeetedClient(YeetSuccessPacket packet) {
      System.Event.StartItemYeeted(System.ClientAPI.World.Player);
    }
  }
}
