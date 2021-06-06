using ProtoBuf;
using Vintagestory.API.MathTools;

namespace Yeet {
  [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
  public class YeetChannelPacket {
    public EnumYeetType YeetType;
    public string InventoryID;
    public int SlotID;
    public Vec3d YeetedFromPos;
    public Vec3d YeetedVelocity;
  }
}
