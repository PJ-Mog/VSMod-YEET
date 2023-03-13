using Newtonsoft.Json;
using ProtoBuf;
using RiceConfig;

namespace Yeet {
  public class YeetConfigurationSystem : ConfigurationSystem<ServerConfig, ClientConfig> {
    public override string ChannelName => "japanhasrice.yeetconfig";

    public override string ServerConfigFilename => "Yeet_ServerConfig.json";

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

    [JsonProperty, JsonConverter(typeof(SettingConverter<float>))]
    public Setting<float> WooshVolume { get; set; } = new Setting<float> {
      Default = 10f,
      Min = 1f,
      Max = 20f,
      Description = "Volume of the 'woosh' noise when an item is yeeted."
    };

    [JsonProperty, JsonConverter(typeof(SettingConverter<float>))]
    public Setting<float> WooshAudibleRange { get; set; } = new Setting<float> {
      Default = 15f,
      Min = 1f,
      Max = 50f,
      Description = "Furthest distance at which the 'woosh' noise can be heard."
    };

    [JsonProperty, JsonConverter(typeof(SettingConverter<float>))]
    public Setting<float> GruntVolume { get; set; } = new Setting<float> {
      Default = 15f,
      Min = 1f,
      Max = 25f,
      Description = "Volume of the player's grunting when yeeting."
    };

    [JsonProperty, JsonConverter(typeof(SettingConverter<float>))]
    public Setting<float> GruntAudibleRange { get; set; } = new Setting<float> {
      Default = 50f,
      Min = 1f,
      Max = 100f,
      Description = "Furthest distance at which the grunting of a yeeting player can be heard."
    };

    [JsonProperty, JsonConverter(typeof(SettingConverter<int>))]
    public Setting<int> MaxShockwaveRingsToSpawn { get; set; } = new Setting<int> {
      Default = 3,
      Min = 0,
      Description = "Maximum number of shockwave particle rings to spawn per yeeted item."
    };

    [JsonProperty, JsonConverter(typeof(SettingConverter<int>))]
    public Setting<int> MinTimeBetweenRingsMillis { get; set; } = new Setting<int> {
      Default = 250,
      Min = 0,
      Description = "Minimum milliseconds to wait before spawning a shockwave particle ring."
    };

    [JsonProperty, JsonConverter(typeof(SettingConverter<int>))]
    public Setting<int> MaxTimeBetweenRingsMillis { get; set; } = new Setting<int> {
      Default = 500,
      Min = 0,
      Description = "Maximum milliseconds to wait before spawning a shockwave particle ring."
    };

    [JsonProperty, JsonConverter(typeof(SettingConverter<int>))]
    public Setting<int> ParticlesPerRing { get; set; } = new Setting<int> {
      Default = 20,
      Min = 0,
      Description = "Number of particles to spawn per shockwave."
    };

    [JsonProperty, JsonConverter(typeof(SettingConverter<float>))]
    public Setting<float> ShockwaveVelocityFactor { get; set; } = new Setting<float> {
      Default = 3.0f,
      Min = 1.0f,
      Description = "Velocity of the shockwave particles compared to the velocity of the yeeted item."
    };
  }
}
