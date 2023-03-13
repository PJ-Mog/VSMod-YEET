using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Yeet.Common;

namespace Yeet.Client {
  public class ErrorManager {
    protected YeetSystem System { get; }
    protected readonly string langPrefix = $"{Constants.MOD_ID}:";

    public ErrorManager(YeetSystem system) {
      if (system.Side == EnumAppSide.Server) { return; }
      System = system;

      System.Event.OnAfterInput += TriggerClientError;
      System.Event.OnClientReceivedYeetEvent += TriggerClientError;
    }

    protected virtual void TriggerClientError(YeetEventArgs eventArgs) {
      if (eventArgs.Successful) {
        return;
      }

      System.ClientAPI?.TriggerIngameError(System, eventArgs.ErrorCode, GetErrorText(eventArgs.ErrorCode, eventArgs.ErrorArgs));
    }

    protected virtual string GetErrorText(string errorCode, params object[] args) {
      string prefixedCode = errorCode.StartsWith(langPrefix) ? errorCode : $"{langPrefix}{errorCode}";
      string displayMessage = Lang.GetMatching(prefixedCode, args).Replace(langPrefix, "");
      return displayMessage;
    }
  }
}
