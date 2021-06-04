using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace Yeet {
  public class YeetMod : ModSystem {
    private const string YEET_ONE_CODE = "yeetit";
    private const string YEET_ONE_DESC = "Hard throw item";
    private const string YEET_ALL_CODE = "yeetthem";
    private const string YEET_ALL_DESC = "Hard throw itemstack";
    private const string YEET_HALF_CODE = "yeetsome";
    private const string YEET_HALF_DESC = "Hard throw half stack";

    public override void StartClientSide(ICoreClientAPI capi) {
      base.StartClientSide(capi);

      capi.Input.RegisterHotKey(YEET_ONE_CODE, YEET_ONE_DESC, GlKeys.Y, HotkeyType.CharacterControls);
      capi.Input.RegisterHotKey(YEET_ALL_CODE, YEET_ALL_DESC, GlKeys.Y, HotkeyType.CharacterControls, false, true, false);

      capi.Input.SetHotKeyHandler(YEET_ONE_CODE, (KeyCombination kc) => { return Yeet(capi, false); });
      capi.Input.SetHotKeyHandler(YEET_ALL_CODE, (KeyCombination kc) => { return Yeet(capi, true); });
    }

    private bool Yeet(ICoreClientAPI capi, bool dropAllItems) {
      var invManager = capi.World.Player.InventoryManager;
      return invManager.DropMouseSlotItems(dropAllItems) || invManager.DropItem(invManager.ActiveHotbarSlot, dropAllItems);
    }
  }
}
