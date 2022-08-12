using Yeet.Client;
using Yeet.Common;
using Yeet.Common.Network;
using Yeet.Server;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Yeet {
  public class YeetSystem : ModSystem {
    public ICoreAPI Api { get; private set; }
    public EnumAppSide Side => Api.Side;
    public YeetConfig Config { get; private set; }
    public ErrorManager Error { get; private set; }
    public EventApi Event { get; private set; } = new EventApi();
    public MessageManager MessageManager { get; private set; }

    public ICoreClientAPI ClientAPI => Api as ICoreClientAPI;
    public IClientNetworkChannel ClientChannel => ClientAPI?.Network?.GetChannel(Constants.MOD_ID);
    public YeetInputHandler YeetInputHandler;

    public ICoreServerAPI ServerAPI => Api as ICoreServerAPI;
    public IServerNetworkChannel ServerChannel => ServerAPI?.Network?.GetChannel(Constants.MOD_ID);
    public YeetHandler YeetHandler;
    public Yeet.Common.AnimationManager Animation;
    public SoundManager Sound;

    public override void Start(ICoreAPI api) {
      base.Start(api);
      Api = api;
      
      Config = YeetConfig.Load(api);

      MessageManager = new MessageManager(this);
      Animation = new Yeet.Common.AnimationManager(this);
    }

    public override void StartClientSide(ICoreClientAPI api) {
      base.StartClientSide(api);

      Error = new ErrorManager(this);

      YeetInputHandler = new YeetInputHandler(this);
    }

    public override void StartServerSide(ICoreServerAPI api) {
      base.StartServerSide(api);

      YeetHandler = new YeetHandler(this);
      Sound = new SoundManager(this);
    }
  }
}
