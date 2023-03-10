using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Yeet.Server {
  public class SoundManager {
    private YeetSystem System { get; }
    private float GruntAudibleRange { get; set; }
    private float GruntVolume { get; set; }
    private float WooshAudibleRange { get; set; }
    private float WooshVolume { get; set; }

    public SoundManager(YeetSystem system) {
      if (system.Side != EnumAppSide.Server) {
        return;
      }
      System = system;

      LoadServerSettings(system.ServerAPI);

      System.Event.ItemYeeted += OnItemYeeted;
    }

    protected virtual void LoadServerSettings(ICoreServerAPI sapi) {
      var serverSettings = sapi.ModLoader.GetModSystem<YeetConfigurationSystem>()?.ServerSettings ?? new ServerConfig();
      GruntAudibleRange = serverSettings.GruntAudibleRange.Value;
      GruntVolume = serverSettings.GruntVolume.Value;
      WooshAudibleRange = serverSettings.WooshAudibleRange.Value;
      WooshVolume = serverSettings.WooshVolume.Value;
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
