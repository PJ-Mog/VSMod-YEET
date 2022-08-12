using ProtoBuf;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

namespace Yeet.Common.Network {
  [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
  public class YeetPacket {
    public EnumYeetType YeetType;
    public EnumYeetSlotType YeetSlotType;
    public int SlotId = -1;
    public Vec3d YeetedFromPos;
    public Vec3d YeetedVelocity;

    private YeetPacket() {}

    public bool ShouldSerializeSlotId() => true;

    public YeetPacket(IClientPlayer player, EnumYeetType yeetType, EnumYeetSlotType yeetSlot, double yeetForce) {
      YeetType = yeetType;
      YeetSlotType = yeetSlot;

      if (YeetSlotType == EnumYeetSlotType.Hotbar) {
        SlotId = player.InventoryManager.ActiveHotbarSlotNumber;
      }

      YeetedFromPos = player.Entity.Pos.XYZ.Add(0, player.Entity.LocalEyePos.Y, 0);
      
      var throwYaw = player.CameraMode == EnumCameraMode.Overhead ? player.Entity.BodyYaw : player.Entity.Pos.Yaw;
      var theta = GameMath.PIHALF + throwYaw; // angle (in radians) to direct the throw on the X-Z (ground) plane, with 0 Rad as north
      YeetedVelocity = new Vec3d(yeetForce * Constants.SIN_PHI * GameMath.FastSin(theta),
                                 yeetForce * Constants.COS_PHI,
                                 yeetForce * Constants.SIN_PHI * GameMath.FastCos(theta));
    }

  }
}
