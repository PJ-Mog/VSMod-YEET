using Yeet.Common;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace Yeet.Client {
  public class YeetInputHandler {
    protected YeetSystem System;
    protected IClientPlayer Player => System.ClientAPI.World.Player;
    protected bool ShouldTryMouseCursorSlot => System.Config.EnableMouseCursorItemYeet;
    protected float SaturationRequired => System.Config.SaturationCostPerYeet;
    protected double YeetForce => System.Config.YeetForce;

    public YeetInputHandler(YeetSystem system) {
      if (system.Side != EnumAppSide.Client) { return; };
      System = system;

      System.ClientAPI.Input.RegisterHotKey(Constants.YEET_ONE_CODE, Constants.YEET_ONE_DESC, Constants.DEFAULT_YEET_KEY);
      System.ClientAPI.Input.SetHotKeyHandler(Constants.YEET_ONE_CODE, OnHotkeyYeetOne);

      System.ClientAPI.Input.RegisterHotKey(Constants.YEET_HALF_CODE, Constants.YEET_HALF_DESC, Constants.DEFAULT_YEET_KEY, shiftPressed: true, ctrlPressed: true);
      System.ClientAPI.Input.SetHotKeyHandler(Constants.YEET_HALF_CODE, OnHotkeyYeetHalf);

      System.ClientAPI.Input.RegisterHotKey(Constants.YEET_ALL_CODE, Constants.YEET_ALL_DESC, Constants.DEFAULT_YEET_KEY, ctrlPressed: true);
      System.ClientAPI.Input.SetHotKeyHandler(Constants.YEET_ALL_CODE, OnHotkeyYeetAll);
    }

    private bool OnHotkeyYeetOne(KeyCombination kc) {
      return TryToYeet(EnumYeetType.One);
    }

    private bool OnHotkeyYeetHalf(KeyCombination kc) {
      return TryToYeet(EnumYeetType.Half);
    }

    private bool OnHotkeyYeetAll(KeyCombination kc) {
      return TryToYeet(EnumYeetType.All);
    }

    private bool TryToYeet(EnumYeetType yeetType) {
      if (CanYeet(out EnumYeetSlotType yeetSlot, out string errorCode)) {
        System.MessageManager.RequestYeet(Player, yeetType, yeetSlot, YeetForce);
        return true;
      }
      System.Event.StartYeetFailedToStart(errorCode);
      return false;
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
