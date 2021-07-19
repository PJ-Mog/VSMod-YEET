using Yeet.Common;

namespace Yeet.Client {
  public class YeetHalfInputHandler : YeetInputHandler {
    protected override EnumYeetType YeetType => EnumYeetType.Half;
    protected override string HotkeyCode => Constants.YEET_HALF_CODE;

    public YeetHalfInputHandler(YeetSystem system) : base(system) {}

    protected override void RegisterHotkey()
      => System.ClientAPI.Input.RegisterHotKey(HotkeyCode, Constants.YEET_HALF_DESC, Constants.DEFAULT_YEET_KEY, shiftPressed: true, ctrlPressed: true);
  }
}
