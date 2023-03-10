using Newtonsoft.Json;
using ProtoBuf;
using RiceConfig;

namespace Yeet {
  public class YeetConfigurationSystem : ConfigurationSystem<ServerConfig, ClientConfig> {
    public override string ChannelName => "japanhasrice.yeetconfig";

    public override string ServerConfigFilename => "Yeet_ServerConfig";

    public override string ClientConfigFilename => "Yeet_ClientConfig";
  }

  public class ClientConfig : RiceConfig.ClientConfig {
    [JsonProperty, JsonConverter(typeof(SettingConverter<bool>))]
    public Setting<bool> EnableMouseCursorItemYeet { get; set; } = new Setting<bool> {
      Default = true,
      Description = "Whether the yeet action works for items held in the mouse slot."
    };

    [JsonProperty, JsonConverter(typeof(SettingConverter<bool>))]
    public Setting<float> ScreenShakeIntensity { get; set; } = new Setting<float> {
      Default = 0.5f,
      Min = 0f,
      Max = 1f,
      Description = "How much the screen shakes when you yeet."
    };
  }

  [ProtoContract]
  public class ServerConfig : RiceConfig.ServerConfig {

    [ProtoMember(1)]
    [JsonProperty, JsonConverter(typeof(SettingConverter<float>))]
    public Setting<float> SaturationCostPerYeet { get; set; } = new Setting<float> {
      Default = 5f,
      Min = 0f,
      Description = "The amount of satiety lost when yeeting."
    };

    [ProtoMember(2)]
    [JsonProperty, JsonConverter(typeof(SettingConverter<float>))]
    public Setting<float> YeetForce { get; set; } = new Setting<float> {
      Default = 0.9f,
      Min = 0.5f,
      Description = "The force of the yeet."
    };

    [JsonProperty, JsonConverter(typeof(SettingConverter<bool>))]
    public Setting<float> WooshVolume { get; set; } = new Setting<float> {
      Default = 10f,
      Min = 1f,
      Max = 20f,
      Description = "Volume of the 'woosh' noise when an item is yeeted."
    };

    [JsonProperty, JsonConverter(typeof(SettingConverter<bool>))]
    public Setting<float> WooshAudibleRange { get; set; } = new Setting<float> {
      Default = 15f,
      Min = 1f,
      Max = 50f,
      Description = "Furthest distance at which the 'woosh' noise can be heard."
    };

    [JsonProperty, JsonConverter(typeof(SettingConverter<bool>))]
    public Setting<float> GruntVolume { get; set; } = new Setting<float> {
      Default = 15f,
      Min = 1f,
      Max = 25f,
      Description = "Volume of the player's grunting when yeeting."
    };

    [JsonProperty, JsonConverter(typeof(SettingConverter<bool>))]
    public Setting<float> GruntAudibleRange { get; set; } = new Setting<float> {
      Default = 50f,
      Min = 1f,
      Max = 100f,
      Description = "Furthest distance at which the grunting of a yeeting player can be heard."
    };
  }
}
