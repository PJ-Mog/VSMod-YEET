using System;
using ProtoBuf;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace Yeet.Common {
  public enum EnumQuantity {
    One,
    Half,
    All
  }

  public enum EnumYeetSlotType {
    Mouse,
    Hotbar
  }

  [ProtoContract]
  public class YeetEventArgs : EventArgs {
    public KeyCombination KeyCombination { get; private set; }

    [ProtoMember(1)]
    public EnumQuantity Quantity { get; private set; }

    [ProtoMember(2)]
    public EnumYeetSlotType SlotType { get; set; }

    [ProtoMember(3)]
    public int SlotId { get; set; } = -1;

    [ProtoMember(4)]
    public Vec3d Pos { get; set; }

    [ProtoMember(5)]
    public Vec3d Velocity { get; set; }

    [ProtoMember(6)]
    public bool Successful { get; set; } = false;

    public IServerPlayer ForPlayer { get; set; }

    public string ErrorCode { get; set; } = "";

    public object[] ErrorArgs { get; set; } = new object[0];

    public YeetEventArgs(KeyCombination keyCombination, EnumQuantity quantity) {
      KeyCombination = keyCombination;
      Quantity = quantity;
    }

    private YeetEventArgs() { }
  }

  public class EventApi {
    public event Action<YeetEventArgs> OnYeetHotkeyPressed;
    public bool TriggerYeetOneKeyPressed(KeyCombination keyCombination) {
      var eventArgs = new YeetEventArgs(keyCombination, EnumQuantity.One);
      OnYeetHotkeyPressed?.Invoke(eventArgs);
      TriggerAfterInput(eventArgs);
      return eventArgs.Successful;
    }

    public bool TriggerYeetHalfKeyPressed(KeyCombination keyCombination) {
      var eventArgs = new YeetEventArgs(keyCombination, EnumQuantity.Half);
      OnYeetHotkeyPressed?.Invoke(eventArgs);
      TriggerAfterInput(eventArgs);
      return eventArgs.Successful;
    }

    public bool TriggerYeetAllKeyPressed(KeyCombination keyCombination) {
      var eventArgs = new YeetEventArgs(keyCombination, EnumQuantity.All);
      OnYeetHotkeyPressed?.Invoke(eventArgs);
      TriggerAfterInput(eventArgs);
      return eventArgs.Successful;
    }

    public event Action<YeetEventArgs> OnAfterInput;
    public void TriggerAfterInput(YeetEventArgs eventArgs) {
      OnAfterInput?.Invoke(eventArgs);
    }

    public event Action<YeetEventArgs> OnServerReceivedYeetEvent;
    public void TriggerServerReceivedYeetEvent(YeetEventArgs eventArgs) {
      OnServerReceivedYeetEvent?.Invoke(eventArgs);
      TriggerAfterServerHandledEvent(eventArgs);
    }

    public event Action<YeetEventArgs> OnAfterServerHandledEvent;
    public void TriggerAfterServerHandledEvent(YeetEventArgs eventArgs) {
      OnAfterServerHandledEvent?.Invoke(eventArgs);
    }

    public event Action<YeetEventArgs> OnClientReceivedYeetEvent;
    public void TriggerClientReceivedYeetEvent(YeetEventArgs eventArgs) {
      OnClientReceivedYeetEvent?.Invoke(eventArgs);
    }
  }
}
