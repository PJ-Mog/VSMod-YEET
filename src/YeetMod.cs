using ProtoBuf;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace Yeet {
  public enum EnumYeetType : int {
    One = 1,
    Half = 2,
    All = 0
  }

  [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
  public class YeetChannelPacket {
    public EnumYeetType YeetType;
    public string InventoryID;
    public int SlotID;
    public Vec3d YeetedFromPos;
    public Vec3d YeetedVelocity;
  }

  public class YeetMod : ModSystem {
    private const GlKeys DEFAULT_YEET_KEY = GlKeys.Y;
    private const string YEET_ONE_CODE = "yeetit";
    private const string YEET_ONE_DESC = "Hard throw item";
    private const string YEET_ALL_CODE = "yeetthem";
    private const string YEET_ALL_DESC = "Hard throw itemstack";
    private const string YEET_HALF_CODE = "yeetsome";
    private const string YEET_HALF_DESC = "Hard throw half stack";
    private const string YEET_CHANNEL_NAME = "yeet-mod";
    public const string YEET_EVENT_STARTED = "sum-been-yeeted";

    public override void Start(ICoreAPI api) {
      base.Start(api);
      api.Network.RegisterChannel(YEET_CHANNEL_NAME).RegisterMessageType(typeof(YeetChannelPacket));
    }

    public override void StartClientSide(ICoreClientAPI capi) {
      base.StartClientSide(capi);

      capi.Input.RegisterHotKey(YEET_ONE_CODE, YEET_ONE_DESC, DEFAULT_YEET_KEY, HotkeyType.CharacterControls);
      capi.Input.RegisterHotKey(YEET_ALL_CODE, YEET_ALL_DESC, DEFAULT_YEET_KEY, HotkeyType.CharacterControls, ctrlPressed: true);
      capi.Input.RegisterHotKey(YEET_HALF_CODE, YEET_HALF_DESC, DEFAULT_YEET_KEY, HotkeyType.CharacterControls, ctrlPressed: true, shiftPressed: true);

      capi.Input.SetHotKeyHandler(YEET_ONE_CODE, (KeyCombination kc) => { return Yeet(capi, EnumYeetType.One); });
      capi.Input.SetHotKeyHandler(YEET_ALL_CODE, (KeyCombination kc) => { return Yeet(capi, EnumYeetType.All); });
      capi.Input.SetHotKeyHandler(YEET_HALF_CODE, (KeyCombination kc) => { return Yeet(capi, EnumYeetType.Half); });
    }

    public override void StartServerSide(ICoreServerAPI sapi) {
      base.StartServerSide(sapi);
      sapi.Network.GetChannel(YEET_CHANNEL_NAME).SetMessageHandler<YeetChannelPacket>((IServerPlayer fromPlayer, YeetChannelPacket packet) => {
        YeetEventHandler(sapi, fromPlayer, packet);
      });
    }


    private bool Yeet(ICoreClientAPI capi, EnumYeetType yeetType) {
      var invManager = capi.World.Player.InventoryManager;
      return Yeet(capi, invManager.MouseItemSlot, yeetType) || Yeet(capi, invManager.ActiveHotbarSlot, yeetType);
    }

    private bool Yeet(ICoreClientAPI capi, ItemSlot slot, EnumYeetType yeetType) {
      if (slot.Empty) { return false; }

      var playerEntity = capi.World.Player.Entity;
      var packet = new YeetChannelPacket();
      packet.YeetType = yeetType;
      packet.InventoryID = slot.Inventory.InventoryID;
      packet.SlotID = slot.Inventory.GetSlotId(slot);
      packet.YeetedFromPos = playerEntity.Pos.XYZ.Add(0, playerEntity.LocalEyePos.Y, 0);
      var radius = 0.75;
      var theta = GameMath.PIHALF + capi.World.Player.CameraYaw;
      var phi = GameMath.PI / 4;
      packet.YeetedVelocity = new Vec3d(radius * GameMath.FastSin(phi) * GameMath.FastSin(theta),
                                        radius * GameMath.FastCos(phi),
                                        radius * GameMath.FastSin(phi) * GameMath.FastCos(theta));
      capi.Network.GetChannel(YEET_CHANNEL_NAME).SendPacket(packet);
      return true;
    }

    private void YeetEventHandler(ICoreServerAPI sapi, IServerPlayer fromPlayer, YeetChannelPacket packet) {
      ItemSlot slot = fromPlayer.InventoryManager.Inventories[packet.InventoryID][packet.SlotID];
      ItemStack yeetedStack;
      switch (packet.YeetType) {
        case EnumYeetType.All:
          yeetedStack = slot.TakeOutWhole();
          break;
        case EnumYeetType.Half:
          yeetedStack = slot.TakeOut(GetHalfStackSizeRoundedUp(slot));
          break;
        case EnumYeetType.One:
        default:
          yeetedStack = slot.TakeOut(1);
          break;
      }
      slot.MarkDirty();
      sapi.World.SpawnItemEntity(yeetedStack, packet.YeetedFromPos, packet.YeetedVelocity);
    }

    private int GetHalfStackSizeRoundedUp(ItemSlot slot) {
      var stackSize = slot?.Itemstack?.StackSize ?? 0;
      return stackSize / 2 + stackSize % 2;
    }
  }
}
