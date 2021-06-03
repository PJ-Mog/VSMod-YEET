using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace Yeet {
  public class YeetMod : ModSystem {

    public override void StartClientSide(ICoreClientAPI capi)
    {
      base.StartClientSide(capi);

      capi.Input.RegisterHotKey("yeetit", "Hard throw item", GlKeys.Y, HotkeyType.CharacterControls);
      capi.Input.RegisterHotKey("yeetthem", "Hard throw itemstack", GlKeys.Y, HotkeyType.CharacterControls, false, true, false);

      capi.Input.SetHotKeyHandler("yeetit", (KeyCombination kc) => { return Yeet(capi, false); });
      capi.Input.SetHotKeyHandler("yeetthem", (KeyCombination kc) => { return Yeet(capi, true); });
    }

    private bool Yeet(ICoreClientAPI capi, bool dropAllItems) {
      var invManager = capi.World.Player.InventoryManager;
      return invManager.DropMouseSlotItems(dropAllItems) || invManager.DropItem(invManager.ActiveHotbarSlot, dropAllItems);
    }
  }
}
