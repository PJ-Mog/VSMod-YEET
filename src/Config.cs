using System;
using Newtonsoft.Json;
using Vintagestory.API.Common;

namespace Yeet {
  public class YeetConfig {
    public string YeetForceDescription = "The force of the yeet. [Minimum: 0.5] (No maximum, but it gets crazy quickly...)";
    public double YeetForce = 0.75;

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

      config.YeetForce = Math.Max(0.5, config.YeetForce);
      Save(api, config);
      return config;
    }
    public static void Save(ICoreAPI api, YeetConfig config) {
      api.StoreModConfig(config, filename);
    }
  }
}
