using Yeet.Common;

namespace Yeet.Client {
  public class YeetOneInputHandler : YeetInputHandler {
    protected override EnumYeetType YeetType => EnumYeetType.One;
    protected override string HotkeyCode => Constants.YEET_ONE_CODE;

    public YeetOneInputHandler(YeetSystem system) : base(system) {}

    protected override void RegisterHotkey()
      => System.ClientAPI.Input.RegisterHotKey(HotkeyCode, Constants.YEET_ONE_DESC, Constants.DEFAULT_YEET_KEY);
  }
}
