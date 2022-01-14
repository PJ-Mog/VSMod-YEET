using Yeet.Common;

namespace Yeet.Client {
  public class YeetAllInputHandler : YeetInputHandler {
    protected override EnumYeetType YeetType => EnumYeetType.All;
    protected override string HotkeyCode => Constants.YEET_ALL_CODE;

    public YeetAllInputHandler(YeetSystem system) : base(system) {}

    protected override void RegisterHotkey()
      => System.ClientAPI.Input.RegisterHotKey(HotkeyCode, Constants.YEET_ALL_DESC, Constants.DEFAULT_YEET_KEY, ctrlPressed: true);
  }
}
