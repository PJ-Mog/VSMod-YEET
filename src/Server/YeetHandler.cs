using Yeet.Common;
using Yeet.Common.Network;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace Yeet.Server {
  public class YeetHandler {
    private YeetSystem System;

    public YeetHandler(YeetSystem system) {
      if (system.Side != EnumAppSide.Server) {
        return;
      }
      System = system;

      System.Event.YeetRequestReceived += OnYeetRequestReceived;
    }

    private void OnYeetRequestReceived(IServerPlayer player, YeetPacket packet) {
      ItemSlot slot;
      switch (packet.YeetSlotType) {
        case EnumYeetSlotType.Mouse:
          slot = player.InventoryManager.MouseItemSlot;
          break;
        case EnumYeetSlotType.Hotbar:
        default:
          slot = player.InventoryManager.GetHotbarInventory()[packet.SlotId];
          break;
      }

      if (slot == null || slot.Empty) {
        System.Event.StartYeetCanceled(player, Constants.ERROR_NOTHING_TO_YEET);
        return;
      }

      ItemStack stackToYeet;
      switch (packet.YeetType) {
        case EnumYeetType.All:
          stackToYeet = slot.TakeOutWhole();
          break;
        case EnumYeetType.Half:
          stackToYeet = slot.TakeOut(GetHalfStackSizeRoundedUp(slot));
          break;
        case EnumYeetType.One:
        default:
          stackToYeet = slot.TakeOut(1);
          break;
      }

      System.ServerAPI.World.SpawnItemEntity(stackToYeet, packet.YeetedFromPos, packet.YeetedVelocity);
      slot.MarkDirty();

      player.Entity.GetBehavior<EntityBehaviorHunger>()?.ConsumeSaturation(System.Config.SaturationCostPerYeet);

      System.Event.StartItemYeeted(player);
    }

    private int GetHalfStackSizeRoundedUp(ItemSlot slot) {
      var stackSize = slot?.Itemstack?.StackSize ?? 0;
      return stackSize / 2 + stackSize % 2;
    }
  }
}
