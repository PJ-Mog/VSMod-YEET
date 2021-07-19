using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Yeet.Server {
  public class SoundManager {
    private YeetSystem System;
    private float GruntAudibleRange;
    private float GruntVolume;
    private float WooshAudibleRange;
    private float WooshVolume;

    public SoundManager(YeetSystem system) {
      if (system.Side != EnumAppSide.Server) {
        return;
      }
      System = system;
      GruntAudibleRange = System.Config.GruntAudibleRange;
      GruntVolume = System.Config.GruntVolume;
      WooshAudibleRange = System.Config.WooshAudibleRange;
      WooshVolume = System.Config.WooshVolume;
    }

    public void StrongYeet(IServerPlayer yeeter) {
      System.ServerAPI.World.PlaySoundAt(new AssetLocation("game:sounds/player/strike"),
                                         atEntity: yeeter.Entity,
                                         randomizePitch: true,
                                         range: GruntAudibleRange,
                                         volume: GruntVolume);
      System.ServerAPI.World.PlaySoundAt(new AssetLocation("game:sounds/player/throw"),
                                         atEntity: yeeter.Entity,
                                         randomizePitch: true,
                                         range: WooshAudibleRange,
                                         volume: WooshVolume);
    }

    public void WeakYeet(IServerPlayer yeeter) {
      // TODO
    }
  }
}
