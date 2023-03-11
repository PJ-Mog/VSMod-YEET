using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using Yeet.Common;

namespace Yeet.Server {
  public class YeetHandler {
    protected YeetSystem System { get; }
    protected float SaturationCostPerYeet { get; set; }

    public YeetHandler(YeetSystem system) {
      if (system.Side != EnumAppSide.Server) {
        return;
      }
      System = system;

      LoadServerSettings(system.ServerAPI);

      System.Event.OnServerReceivedYeetEvent += OnServerReceivedYeetEvent;
      System.Event.OnAfterServerHandledEvent += OnAfterServerHandledEvent;
    }

    protected virtual void LoadServerSettings(ICoreServerAPI sapi) {
      var serverSettings = sapi.ModLoader.GetModSystem<YeetConfigurationSystem>()?.ServerSettings ?? new ServerConfig();

      SaturationCostPerYeet = serverSettings.SaturationCostPerYeet.Value;
    }

    protected virtual void OnServerReceivedYeetEvent(YeetEventArgs eventArgs) {
      ItemSlot slot;
      switch (eventArgs.SlotType) {
        case EnumYeetSlotType.Mouse:
          slot = eventArgs.ForPlayer.InventoryManager.MouseItemSlot;
          break;
        case EnumYeetSlotType.Hotbar:
        default:
          slot = eventArgs.ForPlayer.InventoryManager.GetHotbarInventory()[eventArgs.SlotId];
          break;
      }

      if (slot == null || slot.Empty) {
        eventArgs.Successful = false;
        eventArgs.ErrorCode = Constants.ERROR_NOTHING_TO_YEET;
        return;
      }

      ItemStack stackToYeet;
      switch (eventArgs.Quantity) {
        case EnumQuantity.All:
          stackToYeet = slot.TakeOutWhole();
          break;
        case EnumQuantity.Half:
          stackToYeet = slot.TakeOut(GetHalfStackSizeRoundedUp(slot));
          break;
        case EnumQuantity.One:
        default:
          stackToYeet = slot.TakeOut(1);
          break;
      }

      System.ServerAPI.World.SpawnItemEntity(stackToYeet, eventArgs.Pos, eventArgs.Velocity);
      slot.MarkDirty();
      eventArgs.Successful = true;
    }

    protected int GetHalfStackSizeRoundedUp(ItemSlot slot) {
      var stackSize = slot?.Itemstack?.StackSize ?? 0;
      return stackSize / 2 + stackSize % 2;
    }

    protected virtual void OnAfterServerHandledEvent(YeetEventArgs eventArgs) {
      eventArgs.ForPlayer.Entity.GetBehavior<EntityBehaviorHunger>()?.ConsumeSaturation(SaturationCostPerYeet);
    }
  }
}
