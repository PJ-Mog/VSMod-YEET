using Yeet.Common.Network;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Yeet.Common {
  public class AnimationManager {
    private YeetSystem System;

    public AnimationManager(YeetSystem system) {
      System = system;
      if (System.Side == EnumAppSide.Client) {
        System.ClientChannel.SetMessageHandler<YeetSuccessPacket>(OnYeetSuccessPacket);
      }
    }

    private void OnYeetSuccessPacket(YeetSuccessPacket packet) {
      ShakeScreen(System.ClientAPI.World.Player);
    }

    public AnimationManager ShakeScreen(IPlayer player) {
      if (System.Side == EnumAppSide.Client) {
        if (System.Config.ScreenShakeIntensity > 0f && (player as IClientPlayer)?.CameraMode == EnumCameraMode.FirstPerson) {
          System.ClientAPI.World.SetCameraShake(System.Config.ScreenShakeIntensity);
        }
      }
      else {
        var serverPlayer = player as IServerPlayer;
        if (System.Config.ScreenShakeIntensity > 0f && serverPlayer != null) {
          System.ServerChannel.SendPacket(new YeetSuccessPacket(), serverPlayer);
        }
      }
      return this;
    }

    public AnimationManager StartWindup(IServerPlayer yeeter) {
      yeeter.Entity.StartAnimation("aim");
      return this;
    }

    public AnimationManager StopWindup(IServerPlayer yeeter) {
      yeeter.Entity.StopAnimation("aim");
      return this;
    }

    public AnimationManager StrongYeet(IServerPlayer yeeter) {
      yeeter.Entity.StartAnimation("throw"); // resets automatically
      yeeter.Entity.StartAnimation("sneakidle");
      System.ServerAPI.World.RegisterCallback((float dt) => { yeeter.Entity.StopAnimation("sneakidle"); }, 600);
      return this;
    }

    public AnimationManager WeakYeet(IServerPlayer yeeter) {
      // TODO
      return this;
    }
  }
}
