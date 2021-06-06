using System;
using Newtonsoft.Json;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace Yeet {
  public class YeetConfig {
    public string PhysicsSectionTitle = "=== Gameplay Settings ===";

    private const double DEFAULT_YEET_FORCE = 0.75;
    private const double MIN_YEET_FORCE = 0.5;
    public string YeetForceDescription = $"The force of the yeet. [Default: {DEFAULT_YEET_FORCE}, Minimum: {MIN_YEET_FORCE}, Maximum: none (but it gets crazy quickly...)]";
    public double YeetForce = DEFAULT_YEET_FORCE;

    private const bool DEFAULT_MOUSE_YEET = true;
    public string EnableMouseCursorItemYeetDescription = $"Whether the yeet action works for items held in the mouse slot. [Default: {DEFAULT_MOUSE_YEET}]";
    public bool EnableMouseCursorItemYeet = DEFAULT_MOUSE_YEET;


    public string AudioSectionTitle = "=== Audio Settings ===";

    private const float DEFAULT_WOOSH_VOLUME = 10;
    private const float MIN_WOOSH_VOLUME = 1;
    private const float MAX_WOOSH_VOLUME = 20;
    public string WooshVolumeDescription = $"Volume of the 'woosh' noise when an item is yeeted. [Default: {DEFAULT_WOOSH_VOLUME}, Minimum: {MIN_WOOSH_VOLUME}, Maximum: {MAX_WOOSH_VOLUME}]";
    public float WooshVolume = DEFAULT_WOOSH_VOLUME;


    private const float DEFAULT_WOOSH_RANGE = 15;
    private const float MIN_WOOSH_RANGE = 1;
    private const float MAX_WOOSH_RANGE = 50;
    public string WooshAudibleRangeDescription = $"Furthest distance at which the 'woosh' noise can be heard. [Default: {DEFAULT_WOOSH_RANGE}, Minimum: {MIN_WOOSH_RANGE}, Maximum: {MAX_WOOSH_RANGE}]";
    public float WooshAudibleRange = DEFAULT_WOOSH_RANGE;


    private const float DEFAULT_GRUNT_VOLUME = 15;
    private const float MIN_GRUNT_VOLUME = 1;
    private const float MAX_GRUNT_VOLUME = 25;
    public string GruntVolumeDescription = $"Volume of the player's grunting when yeeting. [Default: {DEFAULT_GRUNT_VOLUME}, Minimum: {MIN_GRUNT_VOLUME}, Maximum: {MAX_GRUNT_VOLUME}]";
    public float GruntVolume = DEFAULT_GRUNT_VOLUME;


    private const float DEFAULT_GRUNT_RANGE = 50;
    private const float MIN_GRUNT_RANGE = 1;
    private const float MAX_GRUNT_RANGE = 100;
    public string GruntAudibleRangeDescription = $"Furthest distance at which the grunting of a yeeting player can be heard. [Default: {DEFAULT_GRUNT_RANGE}, Minimum: {MIN_GRUNT_RANGE}, Maximum: {MAX_GRUNT_RANGE}]";
    public float GruntAudibleRange = DEFAULT_GRUNT_RANGE;


    public string VideoSectionTitle = "=== Video Settings ===";

    private const float DEFAULT_SHAKE_INTENSITY = 0.5f;
    private const float MIN_SHAKE_INTENSITY = 0;
    private const float MAX_SHAKE_INTENSITY = 1;
    public string ScreenShakeIntensityDescription = $"How much the screen shakes when you yeet. [Default: {DEFAULT_SHAKE_INTENSITY}, Minimum: {MIN_SHAKE_INTENSITY}, Maximum: {MAX_SHAKE_INTENSITY}, Reference: throwing a spear is 0.25]";
    public float ScreenShakeIntensity = DEFAULT_SHAKE_INTENSITY;


    public static string filename = "YeetConfig.json";
    public static YeetConfig Load(ICoreAPI api) {
      YeetConfig config = null;
      try {
        config = api.LoadModConfig<YeetConfig>(filename);
      }
      catch (JsonReaderException e) {
        api.Logger.Error($"[YeetMod] Unable to parse config JSON. Correct syntax errors and retry, or delete {filename} and load the world again to generate a new configuration file with default settings.");
        throw e;
      }
      catch (Exception e) {
        api.Logger.Error($"[YeetMod] I don't know what happened. Delete {filename} in the config folder and try again.");
        throw e;
      }

      if (config == null) {
        api.Logger.Notification($"[YeetMod] Configuration file not found. Generating ${filename} with default settings.");
        config = new YeetConfig();
      }

      config.YeetForce = Math.Max(MIN_YEET_FORCE, config.YeetForce);

      config.WooshVolume = GameMath.Clamp(config.WooshVolume, MIN_WOOSH_VOLUME, MAX_WOOSH_VOLUME);
      config.WooshAudibleRange = GameMath.Clamp(config.WooshAudibleRange, MIN_WOOSH_RANGE, MAX_WOOSH_RANGE);

      config.GruntVolume = GameMath.Clamp(config.GruntVolume, MIN_GRUNT_VOLUME, MAX_GRUNT_VOLUME);
      config.GruntAudibleRange = GameMath.Clamp(config.GruntAudibleRange, MIN_GRUNT_RANGE, MAX_GRUNT_RANGE);

      config.ScreenShakeIntensity = GameMath.Clamp(config.ScreenShakeIntensity, MIN_SHAKE_INTENSITY, MAX_SHAKE_INTENSITY);

      Save(api, config);
      return config;
    }
    public static void Save(ICoreAPI api, YeetConfig config) {
      api.StoreModConfig(config, filename);
    }
  }
}
