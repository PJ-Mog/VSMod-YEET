using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Yeet.Common;

namespace Yeet.Client {
  public class InputHandler {
    protected YeetSystem System { get; }
    protected bool IsMouseSlotYeetEnabled { get; set; }
    protected float SatietyCost { get; set; }
    protected double YeetForce { get; set; }

    protected IClientPlayer Player => System.ClientAPI.World.Player;
    protected bool IsMouseItemSlotEmpty => Player.InventoryManager.MouseItemSlot.Empty;
    protected bool IsActiveHotbarSlotEmpty => Player.InventoryManager.ActiveHotbarSlot.Empty;
    // Current saturation(sic) does not utilize EntityBehaviorHunger because that behavior is server-side only
    protected float CurrentSatiety => Player.Entity.WatchedAttributes.GetTreeAttribute("hunger")?.TryGetFloat("currentsaturation") ?? SatietyCost;

    public InputHandler(YeetSystem system) {
      if (system?.Side != EnumAppSide.Client) {
        return;
      }

      System = system;

      LoadServerSettings(system.Api);
      LoadClientSettings(system.Api);
      RegisterHotKeys(system.ClientAPI.Input, system.Event);

      system.Event.OnYeetHotkeyPressed += OnYeetHotkeyPressed;
    }

    protected virtual void LoadServerSettings(ICoreAPI api) {
      SatietyCost = api.World.Config.GetFloat("yeet-SatietyCost", 5.0f);
      YeetForce = api.World.Config.GetFloat("yeet-Force", 0.9f);
    }

    protected virtual void LoadClientSettings(ICoreAPI api) {
      var clientSettings = api.ModLoader.GetModSystem<YeetConfigurationSystem>()?.ClientSettings;
      if (clientSettings == null) {
        api.Logger.Error("[{0}] The {1} was not loaded. Using default settings.", nameof(InputHandler), nameof(ClientConfig));
        clientSettings = new ClientConfig();
      }

      IsMouseSlotYeetEnabled = clientSettings.EnableMouseCursorItemYeet.Value;
    }

    protected virtual void RegisterHotKeys(IInputAPI inputAPI, EventApi eventApi) {
      inputAPI.RegisterHotKey(Constants.YEET_ONE_CODE, Constants.YEET_ONE_DESC, Constants.DEFAULT_YEET_KEY);
      inputAPI.SetHotKeyHandler(Constants.YEET_ONE_CODE, eventApi.TriggerYeetOneKeyPressed);

      inputAPI.RegisterHotKey(Constants.YEET_HALF_CODE, Constants.YEET_HALF_DESC, Constants.DEFAULT_YEET_KEY, shiftPressed: true, ctrlPressed: true);
      inputAPI.SetHotKeyHandler(Constants.YEET_HALF_CODE, eventApi.TriggerYeetHalfKeyPressed);

      inputAPI.RegisterHotKey(Constants.YEET_ALL_CODE, Constants.YEET_ALL_DESC, Constants.DEFAULT_YEET_KEY, ctrlPressed: true);
      inputAPI.SetHotKeyHandler(Constants.YEET_ALL_CODE, eventApi.TriggerYeetAllKeyPressed);
    }

    protected virtual void OnYeetHotkeyPressed(YeetEventArgs eventArgs) {
      if (!CanYeet(eventArgs)) {
        eventArgs.Successful = false;
        return;
      }

      eventArgs.Pos = Player.Entity.Pos.XYZ + Player.Entity.LocalEyePos;
      eventArgs.Velocity = GetYeetVelocity();
      eventArgs.Successful = true;
      return;
    }

    protected virtual bool CanYeet(YeetEventArgs eventArgs) {
      return HasEnoughSatiety(eventArgs)
             && HasSomethingToYeet(eventArgs);
    }

    protected virtual bool HasEnoughSatiety(YeetEventArgs eventArgs) {
      if (SatietyCost == 0f) {
        return true;
      }

      if (CurrentSatiety < SatietyCost) {
        eventArgs.ErrorCode = Constants.ERROR_HUNGER;
        return false;
      }
      return true;
    }

    protected virtual bool HasSomethingToYeet(YeetEventArgs eventArgs) {
      return CanYeetMouseSlot(eventArgs)
             || CanYeetHotbarSlot(eventArgs);
    }

    protected virtual bool CanYeetMouseSlot(YeetEventArgs eventArgs) {
      if (!IsMouseSlotYeetEnabled) {
        eventArgs.ErrorCode = "Yeeting of mouse-held items is disabled.";
        return false;
      }

      if (IsMouseItemSlotEmpty) {
        eventArgs.ErrorCode = Constants.ERROR_NOTHING_TO_YEET;
        return false;
      }

      eventArgs.SlotType = EnumYeetSlotType.Mouse;
      return true;
    }

    protected virtual bool CanYeetHotbarSlot(YeetEventArgs eventArgs) {
      if (IsActiveHotbarSlotEmpty) {
        eventArgs.ErrorCode = Constants.ERROR_NOTHING_TO_YEET;
        return false;
      }

      eventArgs.SlotType = EnumYeetSlotType.Hotbar;
      eventArgs.SlotId = Player.InventoryManager.ActiveHotbarSlotNumber;
      return true;
    }

    protected virtual Vec3d GetYeetVelocity() {
      var yaw = Player.CameraMode == EnumCameraMode.Overhead ? Player.Entity.BodyYaw : Player.Entity.Pos.Yaw;
      var theta = yaw;
      return new Vec3d(YeetForce * Constants.SIN_PHI * GameMath.FastSin(theta),
                       YeetForce * Constants.COS_PHI,
                       YeetForce * Constants.SIN_PHI * GameMath.FastCos(theta));
    }
  }
}
