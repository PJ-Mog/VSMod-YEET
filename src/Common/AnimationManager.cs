using Yeet.Common.Network;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Yeet.Common {
  public class AnimationManager {
    private YeetSystem System;

    public AnimationManager(YeetSystem system) {
      System = system;

      System.Event.ClientRequestedYeet += OnClientRequestedYeet;
      System.Event.YeetRequestReceived += OnYeetRequestReceived;
      System.Event.YeetCanceled += OnYeetCanceled;
      System.Event.ItemYeeted += OnItemYeeted;
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
      if (System.Side == EnumAppSide.Client
          && System.Config.ScreenShakeIntensity > 0f
          && System.ClientAPI.World.Player.CameraMode == EnumCameraMode.FirstPerson) {
        System.ClientAPI.World.SetCameraShake(System.Config.ScreenShakeIntensity);
      }
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
