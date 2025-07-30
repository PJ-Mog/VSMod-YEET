using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Yeet.Server {
  public class SoundManager {
    protected YeetSystem System { get; }
    protected float GruntAudibleRange { get; set; }
    protected float GruntVolume { get; set; }
    protected float WooshAudibleRange { get; set; }
    protected float WooshVolume { get; set; }

    public SoundManager(YeetSystem system) {
      if (system.Side != EnumAppSide.Server) {
        return;
      }
      System = system;

      LoadServerSettings(system.ServerAPI);

      System.Event.OnAfterServerHandledEvent += OnAfterServerHandledEvent;
    }

    protected virtual void LoadServerSettings(ICoreServerAPI sapi) {
      GruntAudibleRange = sapi.World.Config.GetFloat("yeet-GruntAudibleRange", 50.0f);
      GruntVolume = sapi.World.Config.GetFloat("yeet-GruntVolume", 15.0f);
      WooshAudibleRange = sapi.World.Config.GetFloat("yeet-WooshAudibleRange", 15.0f);
      WooshVolume = sapi.World.Config.GetFloat("yeet-WooshVolume", 10.0f);
    }

    protected virtual void OnAfterServerHandledEvent(YeetEventArgs eventArgs) {
      if (!eventArgs.Successful) {
        return;
      }

      StrongYeet(eventArgs.ForPlayer);
    }

    protected virtual void StrongYeet(IPlayer yeeter) {
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
