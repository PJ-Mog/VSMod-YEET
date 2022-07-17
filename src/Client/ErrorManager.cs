using Vintagestory.API.Config;
using Vintagestory.API.Common;

namespace Yeet.Common {
  public class ErrorManager {
    private YeetSystem System { get; }
    private readonly string langPrefix = $"{Constants.MOD_ID}:";

    public ErrorManager(YeetSystem system) {
      if (system.Side == EnumAppSide.Server) { return; }
      System = system;

      System.Event.YeetFailedToStart += OnFailedToStart;
      System.Event.YeetCanceled += OnYeetCanceled;
    }

    private void OnFailedToStart(string errorCode) {
      TriggerFromClient(errorCode);
    }
    private void OnYeetCanceled(IPlayer yeeter, string errorCode) {
      TriggerFromClient(errorCode);
    }

    public void TriggerFromClient(string errorCode, params object[] args) {
      System.ClientAPI?.TriggerIngameError(System, errorCode, GetErrorText(errorCode, args));
    }

    public string GetErrorText(string errorCode, params object[] args) {
      string prefixedCode = errorCode.StartsWith(langPrefix) ? errorCode : $"{langPrefix}{errorCode}";
      string displayMessage = Lang.GetMatching(prefixedCode, args).Replace(langPrefix, "");
      return displayMessage;
    }
  }
}
