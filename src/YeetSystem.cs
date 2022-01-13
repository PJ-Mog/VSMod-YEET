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

    public ICoreClientAPI ClientAPI { get; private set; }
    public IClientNetworkChannel ClientChannel => ClientAPI?.Network?.GetChannel(Constants.MOD_ID);
    public YeetInputHandler YeetOneInputHandler;
    public YeetInputHandler YeetAllInputHandler;
    public YeetInputHandler YeetHalfInputHandler;

    public ICoreServerAPI ServerAPI { get; private set; }
    public IServerNetworkChannel ServerChannel => ServerAPI?.Network?.GetChannel(Constants.MOD_ID);
    public YeetHandler YeetHandler;
    public Yeet.Common.AnimationManager Animation;
    public SoundManager Sound;

    public override void Start(ICoreAPI api) {
      base.Start(api);
      Api = api;
      api.Network.RegisterChannel(Constants.MOD_ID)
        .RegisterMessageType(typeof(YeetPacket))
        .RegisterMessageType(typeof(YeetSuccessPacket));
      
      Config = YeetConfig.Load(api);
      Error = new ErrorManager(this);
    }

    public override void StartClientSide(ICoreClientAPI api) {
      base.StartClientSide(api);

      ClientAPI = api;
      Animation = new Yeet.Common.AnimationManager(this);
      YeetOneInputHandler = new YeetOneInputHandler(this);
      YeetAllInputHandler = new YeetAllInputHandler(this);
      YeetHalfInputHandler = new YeetHalfInputHandler(this);
    }

    public override void StartServerSide(ICoreServerAPI api) {
      base.StartServerSide(api);

      ServerAPI = api;
      Animation = new Yeet.Common.AnimationManager(this);
      YeetHandler = new YeetHandler(this);
      Sound = new SoundManager(this);
    }
  }
}
