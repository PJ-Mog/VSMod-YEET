using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Yeet.Client;
using Yeet.Common;
using Yeet.Server;

namespace Yeet {
  public class YeetSystem : ModSystem {
    public ICoreAPI Api { get; protected set; }
    public EnumAppSide Side => Api.Side;
    public ErrorManager Error { get; protected set; }
    public EventApi Event { get; protected set; } = new EventApi();
    public NetworkManager NetworkManager { get; protected set; }

    public ICoreClientAPI ClientAPI => Api as ICoreClientAPI;
    public YeetInputHandler YeetInputHandler;

    public ICoreServerAPI ServerAPI => Api as ICoreServerAPI;
    public IServerNetworkChannel ServerChannel => ServerAPI?.Network?.GetChannel(Constants.MOD_ID);
    public YeetHandler YeetHandler;
    public Yeet.Common.AnimationManager Animation;
    public SoundManager Sound;

    public override void Start(ICoreAPI api) {
      base.Start(api);
      Api = api;

      NetworkManager = new NetworkManager(this);
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
