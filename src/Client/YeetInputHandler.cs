using Yeet.Common;
using Yeet.Common.Network;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace Yeet.Client {
  public abstract class YeetInputHandler {
    protected YeetSystem System;

    protected abstract EnumYeetType YeetType { get; }
    protected abstract string HotkeyCode { get; }
    protected IClientPlayer Player => System.ClientAPI.World.Player;
    protected bool ShouldTryMouseCursorSlot = Constants.DEFAULT_MOUSE_YEET;
    protected float SaturationRequired = Constants.DEFAULT_SATURATION_COST;
    protected double YeetForce = Constants.DEFAULT_YEET_FORCE;

    public YeetInputHandler(YeetSystem system) {
      if (system.Side != EnumAppSide.Client) { return; };
      System = system;

      RegisterHotkey();
      System.ClientAPI.Input.SetHotKeyHandler(HotkeyCode, OnTryToYeet);

      if (System.Config?.EnableMouseCursorItemYeet != null) {
        ShouldTryMouseCursorSlot = System.Config.EnableMouseCursorItemYeet;
      }
      if (System.Config?.SaturationCostPerYeet != null) {
        SaturationRequired = System.Config.SaturationCostPerYeet;
      }
      if (System.Config?.YeetForce != null) {
        YeetForce = System.Config.YeetForce;
      }
    }

    protected abstract void RegisterHotkey();

    private bool OnTryToYeet(KeyCombination kc) {
      if (CanYeet(out EnumYeetSlotType yeetSlot, out string errorCode)) {
        var packet = new YeetPacket(Player, YeetType, yeetSlot, YeetForce);
        System.ClientChannel.SendPacket(packet);
      }
      else {
        System.Error.TriggerFromClient(errorCode);
      }
      return true;
    }

    private bool CanYeet(out EnumYeetSlotType yeetSlot, out string errorCode) {
      yeetSlot = EnumYeetSlotType.Hotbar;
      return HasEnoughSaturation(out errorCode)
             && HasSomethingToYeet(out yeetSlot, out errorCode);
    }

    private bool HasEnoughSaturation(out string errorCode) {
      errorCode = "";
      if (!(SaturationRequired > 0)) { return true; }
      var currentSaturation = Player.Entity.WatchedAttributes.GetTreeAttribute("hunger")?.TryGetFloat("currentsaturation");
      bool enoughSaturation = (currentSaturation == null ? true : currentSaturation >= SaturationRequired);
      if (!enoughSaturation) {
        errorCode = System.Error.GetErrorText(Constants.ERROR_HUNGER);
      }
      return enoughSaturation;
    }

    private bool HasSomethingToYeet(out EnumYeetSlotType yeetSlot, out string errorCode) {
      yeetSlot = EnumYeetSlotType.Hotbar;
      errorCode = "";
      if (CanYeetMouseSlot()) {
        yeetSlot = EnumYeetSlotType.Mouse;
        return true;
      }
      if (CanYeetHotbarSlot(out errorCode)) {
        yeetSlot = EnumYeetSlotType.Hotbar;
        return true;
      }
      return false;
    }

    private bool CanYeetMouseSlot() {
      return ShouldTryMouseCursorSlot
             && !Player.InventoryManager.MouseItemSlot.Empty;
    }

    private bool CanYeetHotbarSlot(out string errorCode) {
      errorCode = "";
      if (Player.InventoryManager.ActiveHotbarSlot.Empty) {
        errorCode = System.Error.GetErrorText(Constants.ERROR_NOTHING_TO_YEET);
        return false;
      }
      return true;
    }
  }
}
