using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Yeet.Common;

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
      var serverSettings = sapi.ModLoader.GetModSystem<YeetConfigurationSystem>()?.ServerSettings ?? new ServerConfig();
      GruntAudibleRange = serverSettings.GruntAudibleRange.Value;
      GruntVolume = serverSettings.GruntVolume.Value;
      WooshAudibleRange = serverSettings.WooshAudibleRange.Value;
      WooshVolume = serverSettings.WooshVolume.Value;
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
