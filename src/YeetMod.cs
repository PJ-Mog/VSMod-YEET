using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace Yeet {
  public class YeetMod : ModSystem {
    public const string YEET_EVENT_STARTED = "sum-been-yeeted";
    public static double YeetForce {
      get { return radius; }
      set { radius = YeetForce; }
    }
    internal static YeetConfig Config;
    private const GlKeys DEFAULT_YEET_KEY = GlKeys.Y;
    private const string YEET_ONE_CODE = "yeetit";
    private const string YEET_ONE_DESC = "Hard throw item";
    private const string YEET_ALL_CODE = "yeetthem";
    private const string YEET_ALL_DESC = "Hard throw itemstack";
    private const string YEET_HALF_CODE = "yeetsome";
    private const string YEET_HALF_DESC = "Hard throw half stack";
    private const string YEET_CHANNEL_NAME = "yeet-mod";
    private const string YEET_ERROR_HUNGER = "toohungrytoyeet";
    private const string YEET_ERROR_HUNGER_TEXT = "You're too hungry and cannot muster the energy.";
    private const double PHI = GameMath.PI / 4.0; // angle (in radians) between the ground plane and the Y-axis, always 45 degrees for maximum distance
    private static double SIN_PHI = GameMath.Sin(PHI);
    private static double COS_PHI = GameMath.Cos(PHI);
    private static double radius = 0.75; // the magnitude of the velocity vector
    private static bool ShouldTryMouseCursorSlot {
      get { return Config?.EnableMouseCursorItemYeet ?? YeetConfig.DEFAULT_MOUSE_YEET; }
    }

    public override void Start(ICoreAPI api) {
      base.Start(api);

      Config = YeetConfig.Load(api);
      radius = Config.YeetForce;

      api.Network.RegisterChannel(YEET_CHANNEL_NAME)
        .RegisterMessageType(typeof(YeetAttemptedPacket))
        .RegisterMessageType(typeof(YeetSuccessPacket));
    }

    public override void StartClientSide(ICoreClientAPI capi) {
      base.StartClientSide(capi);

      capi.Input.RegisterHotKey(YEET_ONE_CODE, YEET_ONE_DESC, DEFAULT_YEET_KEY, HotkeyType.CharacterControls);
      capi.Input.RegisterHotKey(YEET_ALL_CODE, YEET_ALL_DESC, DEFAULT_YEET_KEY, HotkeyType.CharacterControls, ctrlPressed: true);
      capi.Input.RegisterHotKey(YEET_HALF_CODE, YEET_HALF_DESC, DEFAULT_YEET_KEY, HotkeyType.CharacterControls, ctrlPressed: true, shiftPressed: true);

      capi.Input.SetHotKeyHandler(YEET_ONE_CODE, (KeyCombination kc) => { return TryToYeet(capi, EnumYeetType.One); });
      capi.Input.SetHotKeyHandler(YEET_ALL_CODE, (KeyCombination kc) => { return TryToYeet(capi, EnumYeetType.All); });
      capi.Input.SetHotKeyHandler(YEET_HALF_CODE, (KeyCombination kc) => { return TryToYeet(capi, EnumYeetType.Half); });

      capi.Network.GetChannel(YEET_CHANNEL_NAME)
        .SetMessageHandler<YeetSuccessPacket>((YeetSuccessPacket packet) => {
          OnSuccessfulYeet(capi);
        });
    }

    public override void StartServerSide(ICoreServerAPI sapi) {
      base.StartServerSide(sapi);
      sapi.Network.GetChannel(YEET_CHANNEL_NAME)
        .SetMessageHandler<YeetAttemptedPacket>((IServerPlayer fromPlayer, YeetAttemptedPacket packet) => {
          YeetAttemptHandler(sapi, fromPlayer, packet);
        });
    }

    public bool TryToYeet(ICoreClientAPI capi, EnumYeetType yeetType) {
      var invManager = capi.World.Player.InventoryManager;
      return (ShouldTryMouseCursorSlot && TryToYeet(capi, invManager.MouseItemSlot, yeetType))
             || TryToYeet(capi, invManager.ActiveHotbarSlot, yeetType);
    }

    private bool TryToYeet(ICoreClientAPI capi, ItemSlot slot, EnumYeetType yeetType) {
      if (slot.Empty) { return false; }

      var playerEntity = capi.World.Player.Entity;
      var attemptPacket = new YeetAttemptedPacket();
      attemptPacket.YeetType = yeetType;
      attemptPacket.InventoryID = slot.Inventory.InventoryID;
      attemptPacket.SlotID = slot.Inventory.GetSlotId(slot);
      attemptPacket.YeetedFromPos = playerEntity.Pos.XYZ.Add(0, playerEntity.LocalEyePos.Y, 0);
      var playerYaw = capi.World.Player.CameraMode == EnumCameraMode.Overhead ? playerEntity.BodyYaw : capi.World.Player.CameraYaw;
      var theta = GameMath.PIHALF + playerYaw; // angle (in radians) to direct the throw on the X-Z (ground) plane, with 0 Rad as north
      attemptPacket.YeetedVelocity = new Vec3d(radius * SIN_PHI * GameMath.FastSin(theta),
                                        radius * COS_PHI,
                                        radius * SIN_PHI * GameMath.FastCos(theta));
      capi.Network.GetChannel(YEET_CHANNEL_NAME).SendPacket(attemptPacket);
      return true;
    }

    private void YeetAttemptHandler(ICoreServerAPI sapi, IServerPlayer fromPlayer, YeetAttemptedPacket packet) {
      OnBeforeYeetAttempt(sapi, fromPlayer);

      if (!HasEnoughSaturation(fromPlayer)) {
        OnFailedYeet(sapi, fromPlayer);
        return;
      }

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
      OnSuccessfulYeet(sapi, fromPlayer);
    }

    private void OnBeforeYeetAttempt(ICoreServerAPI sapi, IServerPlayer fromPlayer) {
      // TODO: begin holding player's position until yeet completes
      fromPlayer.Entity.StartAnimation("aim");
    }

    private void OnFailedYeet(ICoreServerAPI sapi, IServerPlayer yeeter) {
      // Currently, the only reason a yeet might fail server-side is if the yeeter doesn't have enough satiety/saturation.
      yeeter.SendIngameError(YEET_ERROR_HUNGER, Lang.GetIfExists($"yeet:ingameerror-{YEET_ERROR_HUNGER}") ?? YEET_ERROR_HUNGER_TEXT);
      yeeter.Entity.StopAnimation("aim");
    }

    private void OnSuccessfulYeet(ICoreServerAPI sapi, IServerPlayer fromPlayer) {
      AnimateSuccessfulYeet(sapi, fromPlayer);
      PlaySuccessfulYeetSounds(sapi, fromPlayer.Entity);
      fromPlayer.Entity.GetBehavior<EntityBehaviorHunger>().ConsumeSaturation(Config.SaturationCostPerYeet);
    }

    private void AnimateSuccessfulYeet(ICoreServerAPI sapi, IServerPlayer yeeter) {
      sapi.Network.GetChannel(YEET_CHANNEL_NAME).SendPacket(new YeetSuccessPacket(), yeeter);
      yeeter.Entity.StopAnimation("aim");
      yeeter.Entity.StartAnimation("throw");
      yeeter.Entity.StartAnimation("sneakidle");
      sapi.World.RegisterCallback((float timePassed) => { yeeter.Entity.StopAnimation("sneakidle"); }, 600);
    }

    private void PlaySuccessfulYeetSounds(ICoreServerAPI sapi, Entity yeetingEntity) {
      sapi.World.PlaySoundAt(new AssetLocation("game:sounds/player/strike"), yeetingEntity, randomizePitch: true, range: Config.GruntAudibleRange, volume: Config.GruntVolume);
      sapi.World.PlaySoundAt(new AssetLocation("game:sounds/player/throw"), yeetingEntity, randomizePitch: true, range: Config.WooshAudibleRange, volume: Config.WooshVolume);
    }

    private void OnSuccessfulYeet(ICoreClientAPI capi) {
      if (capi.World.Player.CameraMode == EnumCameraMode.FirstPerson && Config.ScreenShakeIntensity > 0f) {
        capi.World.SetCameraShake(Config.ScreenShakeIntensity);
      }
    }

    private bool HasEnoughSaturation(IServerPlayer player) {
      return player.Entity.GetBehavior<EntityBehaviorHunger>().Saturation >= Config.SaturationCostPerYeet;
    }

    private int GetHalfStackSizeRoundedUp(ItemSlot slot) {
      var stackSize = slot?.Itemstack?.StackSize ?? 0;
      return stackSize / 2 + stackSize % 2;
    }
  }
}
