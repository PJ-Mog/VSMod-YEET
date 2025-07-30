using Newtonsoft.Json;
using RiceConfig;

namespace Yeet {
  public class YeetConfigurationSystem : ClientOnlyConfigurationSystem<ClientConfig> {
    public override string ChannelName => "japanhasrice.yeetconfig";

    public override string ClientConfigFilename => "Yeet_ClientConfig.json";
  }

  public class ClientConfig : RiceConfig.ClientConfig {
    [JsonProperty, JsonConverter(typeof(SettingConverter<bool>))]
    public Setting<bool> EnableMouseCursorItemYeet { get; set; } = new Setting<bool> {
      Default = true,
      Description = "Whether the yeet action works for items held in the mouse slot."
    };

    [JsonProperty, JsonConverter(typeof(SettingConverter<float>))]
    public Setting<float> ScreenShakeIntensity { get; set; } = new Setting<float> {
      Default = 0.5f,
      Min = 0f,
      Max = 1f,
      Description = "How much the screen shakes when you yeet."
    };
  }
}
