using Vintagestory.API.Common;

namespace Yeet.Server {
  public class SoundManager {
    private YeetSystem System;
    private float GruntAudibleRange => System.Config.GruntAudibleRange;
    private float GruntVolume => System.Config.GruntVolume;
    private float WooshAudibleRange => System.Config.WooshAudibleRange;
    private float WooshVolume => System.Config.WooshVolume;

    public SoundManager(YeetSystem system) {
      if (system.Side != EnumAppSide.Server) {
        return;
      }
      System = system;
      System.Event.ItemYeeted += OnItemYeeted;
    }

    private void OnItemYeeted(IPlayer yeeter) {
      StrongYeet(yeeter);
    }

    public void StrongYeet(IPlayer yeeter) {
      System.Api.World.PlaySoundAt(new AssetLocation("game:sounds/player/strike"),
                                         atEntity: yeeter.Entity,
                                         randomizePitch: true,
                                         range: GruntAudibleRange,
                                         volume: GruntVolume);
      System.Api.World.PlaySoundAt(new AssetLocation("game:sounds/player/throw"),
                                         atEntity: yeeter.Entity,
                                         randomizePitch: true,
                                         range: WooshAudibleRange,
                                         volume: WooshVolume);
    }
  }
}
