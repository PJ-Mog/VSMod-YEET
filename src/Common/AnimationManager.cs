using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Yeet.Common.Network;

namespace Yeet.Common {
  public class AnimationManager {
    private YeetSystem System { get; }
    private float ScreenShakeIntensity { get; }

    public AnimationManager(YeetSystem system) {
      System = system;

      System.Event.ClientRequestedYeet += OnClientRequestedYeet;
      System.Event.YeetRequestReceived += OnYeetRequestReceived;
      System.Event.YeetCanceled += OnYeetCanceled;
      System.Event.ItemYeeted += OnItemYeeted;

      if (system.Api is ICoreClientAPI capi) {
        ScreenShakeIntensity = capi.ModLoader.GetModSystem<YeetConfigurationSystem>()?.ClientSettings?.ScreenShakeIntensity.Value ?? new ClientConfig().ScreenShakeIntensity.Value;
      }
    }

    protected virtual void LoadClientSettings(ICoreClientAPI capi) {
    }

    private void OnClientRequestedYeet(IClientPlayer yeeter, EnumYeetType type, EnumYeetSlotType slot, double force) {
      StartWindup(yeeter.Entity);
    }

    private void OnYeetRequestReceived(IServerPlayer yeeter, YeetPacket yeetData) {
      StartWindup(yeeter.Entity);
    }

    private void OnYeetCanceled(IPlayer yeeter, string errorCode) {
      StopWindup(yeeter.Entity);
    }

    private void OnItemYeeted(IPlayer yeeter) {
      ShakeScreen();
      StopWindup(yeeter.Entity);
      StrongYeet(yeeter.Entity);
    }

    public void ShakeScreen() {
      if (ShouldShakeScreen()) {
        System.ClientAPI.World.SetCameraShake(ScreenShakeIntensity);
      }
    }

    private bool ShouldShakeScreen() {
      return System.Side == EnumAppSide.Client
             && ScreenShakeIntensity > 0f
             && System.ClientAPI.World.Player.CameraMode == EnumCameraMode.FirstPerson;
    }

    public void StartWindup(EntityPlayer yeeter) {
      yeeter.StartAnimation("aim");
    }

    public void StopWindup(EntityPlayer yeeter) {
      yeeter.StopAnimation("aim");
    }

    public void StrongYeet(EntityPlayer yeeter) {
      yeeter.StartAnimation("throw"); // resets automatically
      yeeter.StartAnimation("sneakidle");
      System.Api.World.RegisterCallback((float dt) => { yeeter.StopAnimation("sneakidle"); }, 600);
    }
  }
}
