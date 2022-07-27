using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using System;

namespace Yeet.Common {
  public class EntityYeetedItem : EntityItem {
    private float accum = 0f;
    private int ringCount = 0;
    public static SimpleParticleProperties shockwaveRing = new SimpleParticleProperties(1, 1, ColorUtil.ColorFromRgba(255, 255, 255, 50), new Vec3d(), new Vec3d(), new Vec3f(), new Vec3f(), lifeLength: 1, gravityEffect: 0, model: EnumParticleModel.Quad);

    public override void OnEntitySpawn() {
      base.OnEntitySpawn();
      World.Logger.Debug("we yeet it!");
    }

    // 12 particles per ring, rotated so side is same angle as a line tangent to circle centered on yeetedItem. Stretch as expanding. Die quickly.
    // watch out for dual calls
    // DISABLE ON LOAD, CURRENTLY STARTS OVER FOR ALL ITEMS ON RELOAD
    // public override void OnGameTick(float deltaTime) {
    //   base.OnGameTick(deltaTime);
    //   if (ringCount >= 3) { return; }
    //   accum += deltaTime;
    //   if (accum > 0.5f) {
    //     Api.Logger.Debug("yeeted item spawn ring");
    //     Api.Logger.Debug("yeeted motion: " + SidedPos.Motion.ToString());
    //     Api.Logger.Debug("yeeted pos: " + SidedPos.OnlyPosToString());
    //     float speed = (float)SidedPos.Motion.Length();
    //     var rotX = (float)GameMath.Asin(SidedPos.Motion.Y / speed);
    //     var rotY = (float)GameMath.PI + (float)Math.Atan2(SidedPos.Motion.X / speed, SidedPos.Motion.Z / speed);
    //     var quat = Quaternionf.Create();
    //     quat[0] = 0f;
    //     quat[1] = 0f;
    //     quat[2] = 1f;
    //     quat[3] = 0f;
    //     if (rotX != 0f) { Quaternionf.RotateX(quat, quat, rotX); }
    //     if (rotY != 0f) { Quaternionf.RotateY(quat, quat, rotY); }
    //     shockwaveRing.MinPos = SidedPos.XYZ;
    //     World.SpawnParticles(shockwaveRing);
    //     accum = 0f;
    //     ringCount++;
    //   }
    // }
  }
}
