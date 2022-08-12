using ProtoBuf;

namespace Yeet.Common.Network {
  [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
  public class YeetCanceledPacket {
    public string ErrorCode;

    private YeetCanceledPacket() {}

    public YeetCanceledPacket(string errorCode) {
      ErrorCode = errorCode;
    }
  }
}
