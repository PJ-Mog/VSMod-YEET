using Yeet.Common.Network;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Yeet.Common {
  public class EventApi {
    // 1. Client presses hotkey => local checks performed.
    
    // 2a. Client has verified prerequisites. Yeet request was sent to server.
    public event ClientRequestedYeetHandler ClientRequestedYeet;
    public delegate void ClientRequestedYeetHandler(IClientPlayer yeeter, EnumYeetType yeetType, EnumYeetSlotType yeetSlot, double force);
    public void StartClientRequestedYeet(IClientPlayer yeeter, EnumYeetType yeetType, EnumYeetSlotType yeetSlot, double force) {
      ClientRequestedYeet?.Invoke(yeeter, yeetType, yeetSlot, force);
    }

    // 2b. Client failed prerequisites.
    public event YeetFailedToStartHandler YeetFailedToStart;
    public delegate void YeetFailedToStartHandler(string errorCode);
    public void StartYeetFailedToStart(string errorCode) {
      YeetFailedToStart?.Invoke(errorCode);
    }

    // 3. Server received yeet request data.
    public event YeetRequestReceivedHandler YeetRequestReceived;
    public delegate void YeetRequestReceivedHandler(IServerPlayer forPlayer, YeetPacket yeetData);
    public void StartYeetRequestReceived(IServerPlayer forPlayer, YeetPacket yeetData) {
      YeetRequestReceived?.Invoke(forPlayer, yeetData);
    }

    // 4a. Server has verified prerequisites and item was launched.
    public event ItemYeetedHandler ItemYeeted;
    public delegate void ItemYeetedHandler(IPlayer yeeter);
    public void StartItemYeeted(IPlayer yeeter) {
      ItemYeeted?.Invoke(yeeter);
    }

    // 4b. Server failed prerequisites.
    public event YeetCanceledHandler YeetCanceled;
    public delegate void YeetCanceledHandler(IPlayer wouldBeYeeter, string errorCode);
    public void StartYeetCanceled(IPlayer wouldbeYeeter, string errorCode) {
      YeetCanceled?.Invoke(wouldbeYeeter, errorCode);
    }
  }
}
