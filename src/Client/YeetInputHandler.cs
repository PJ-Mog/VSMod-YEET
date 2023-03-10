using Yeet.Common;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace Yeet.Client {
  public class YeetInputHandler {
    protected YeetSystem System;
    protected IClientPlayer Player => System.ClientAPI.World.Player;
    protected bool ShouldTryMouseCursorSlot { get; set; }
    protected float SaturationRequired { get; set; }
    protected double YeetForce { get; set; }

    public YeetInputHandler(YeetSystem system) {
      if (system.Side != EnumAppSide.Client) { return; };
      System = system;

      LoadServerSettings(system.Api);
      LoadClientSettings(system.Api);
      RegisterHotKeys(system.ClientAPI.Input, system.Event);
    }

    protected virtual void LoadServerSettings(ICoreAPI api) {
      var configSystem = api.ModLoader.GetModSystem<YeetConfigurationSystem>();
      if (configSystem == null) {
        api.Logger.Error("[{0}] {1} was not loaded. Using defaults.", nameof(YeetInputHandler), nameof(YeetConfigurationSystem));
        LoadServerSettings(new ServerConfig());
        return;
      }

      configSystem.ServerSettingsReceived += LoadServerSettings;
      if (configSystem.ServerSettings != null) {
        LoadServerSettings(configSystem.ServerSettings);
      }
    }

    protected virtual void LoadServerSettings(ServerConfig serverSettings) {
      SaturationRequired = serverSettings.SaturationCostPerYeet.Value;
      YeetForce = serverSettings.YeetForce.Value;
    }

    protected virtual void LoadClientSettings(ICoreAPI api) {
      var clientSettings = api.ModLoader.GetModSystem<YeetConfigurationSystem>()?.ClientSettings;
      if (clientSettings == null) {
        api.Logger.Error("[{0}] The {1} was not loaded. Using default settings.", nameof(YeetInputHandler), nameof(ClientConfig));
        clientSettings = new ClientConfig();
      }

      ShouldTryMouseCursorSlot = clientSettings.EnableMouseCursorItemYeet.Value;
    }

    protected virtual void RegisterHotKeys(IInputAPI inputAPI, EventApi eventApi) {
      inputAPI.RegisterHotKey(Constants.YEET_ONE_CODE, Constants.YEET_ONE_DESC, Constants.DEFAULT_YEET_KEY);
      inputAPI.SetHotKeyHandler(Constants.YEET_ONE_CODE, OnHotkeyYeetOne);

      inputAPI.RegisterHotKey(Constants.YEET_HALF_CODE, Constants.YEET_HALF_DESC, Constants.DEFAULT_YEET_KEY, shiftPressed: true, ctrlPressed: true);
      inputAPI.SetHotKeyHandler(Constants.YEET_HALF_CODE, OnHotkeyYeetHalf);

      inputAPI.RegisterHotKey(Constants.YEET_ALL_CODE, Constants.YEET_ALL_DESC, Constants.DEFAULT_YEET_KEY, ctrlPressed: true);
      inputAPI.SetHotKeyHandler(Constants.YEET_ALL_CODE, OnHotkeyYeetAll);
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
