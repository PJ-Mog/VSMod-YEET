using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace Yeet {
  public class YeetMod : ModSystem {
    public const string YEET_EVENT_STARTED = "sum-been-yeeted";
    public static double YeetForce {
      get { return radius; }
      set { radius = YeetForce; }
    }
    private const GlKeys DEFAULT_YEET_KEY = GlKeys.Y;
    private const string YEET_ONE_CODE = "yeetit";
    private const string YEET_ONE_DESC = "Hard throw item";
    private const string YEET_ALL_CODE = "yeetthem";
    private const string YEET_ALL_DESC = "Hard throw itemstack";
    private const string YEET_HALF_CODE = "yeetsome";
    private const string YEET_HALF_DESC = "Hard throw half stack";
    private const string YEET_CHANNEL_NAME = "yeet-mod";
    private const double PHI = GameMath.PI / 4.0; // angle (in radians) between the ground plane and the Y-axis, always 45 degrees for maximum distance
    private static double SIN_PHI = GameMath.Sin(PHI);
    private static double COS_PHI = GameMath.Cos(PHI);
    private static double radius = 0.75; // the magnitude of the velocity vector

    public override void Start(ICoreAPI api) {
      base.Start(api);

      radius = YeetConfig.Load(api).YeetForce;

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
      var theta = GameMath.PIHALF + capi.World.Player.CameraYaw; // angle (in radians) to direct the throw on the X-Z (ground) plane, with 0 Rad as north
      packet.YeetedVelocity = new Vec3d(radius * SIN_PHI * GameMath.FastSin(theta),
                                        radius * COS_PHI,
                                        radius * SIN_PHI * GameMath.FastCos(theta));
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

      sapi.World.PlaySoundAt(new AssetLocation("game:sounds/player/throw"), fromPlayer.Entity, randomizePitch: true, range: 10, volume: 10);
      sapi.World.PlaySoundAt(new AssetLocation("game:sounds/player/strike"), fromPlayer.Entity, randomizePitch: true, range: 50, volume: 15);
      sapi.World.SpawnItemEntity(yeetedStack, packet.YeetedFromPos, packet.YeetedVelocity);
    }

    private int GetHalfStackSizeRoundedUp(ItemSlot slot) {
      var stackSize = slot?.Itemstack?.StackSize ?? 0;
      return stackSize / 2 + stackSize % 2;
    }
  }
}
