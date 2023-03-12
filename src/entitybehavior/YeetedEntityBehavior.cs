using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace Yeet {
  public class SpawnShockwavesEntityBehavior : EntityBehavior {
    protected long spawnNextRingAt;
    protected int ringsSpawned = 0;
    protected bool active = false;

    public static SpawnShockwavesEntityBehavior Create(EntityItem entityItem) {
      var behavior = new SpawnShockwavesEntityBehavior(entityItem);
      behavior.active = true;
      behavior.spawnNextRingAt = entityItem.World.ElapsedMilliseconds + entityItem.World.Rand.Next(250, 400);
      return behavior;
    }

    private SpawnShockwavesEntityBehavior(Entity entity) : base(entity) { }

    public override string PropertyName() {
      return "yeeted-entity-behavior";
    }

    public override void OnGameTick(float deltaTime) {
      if (!active) {
        return;
      }

      if (!entity.Alive) {
        active = false;
        return;
      }

      var entityItem = entity as EntityItem;
      if (entityItem == null) {
        active = false;
        return;
      }

      if (entity.World.ElapsedMilliseconds < spawnNextRingAt) {
        return;
      }

      SpawnShockwaveRing();
      ringsSpawned++;

      if (ringsSpawned >= 3) {
        active = false;
        return;
      }

      spawnNextRingAt = entity.World.ElapsedMilliseconds + entity.World.Rand.Next(250, 500);
    }

    protected void SpawnShockwaveRing() {
      for (int i = 0; i < 50; i++) {
        var perpendicular = GetRandomVectorPerpendicularTo(entity.SidedPos.Motion);
        var particleProps = GetShockwaveParticleProps(perpendicular);
        entity.World.SpawnParticles(particleProps);
      }
    }

    protected virtual Vec3f GetRandomVectorPerpendicularTo(Vec3d originalVector) {
      var randomVector = new Vec3d(entity.World.Rand.NextDouble() * 12 - 6, entity.World.Rand.NextDouble() * 12 - 6, entity.World.Rand.NextDouble() * 12 - 6);
      return (entity.Pos.Motion.Cross(randomVector).Normalize() * entity.Pos.Motion.Length()).ToVec3f() * 3;
    }

    protected virtual IParticlePropertiesProvider GetShockwaveParticleProps(Vec3f velocity) {
      return new SimpleParticleProperties(1, 1, ColorUtil.ColorFromRgba(255, 255, 255, 50), entity.SidedPos.XYZ, entity.SidedPos.XYZ, velocity, velocity) {
        GravityEffect = 0f,
        ParticleModel = EnumParticleModel.Quad,
        WindAffected = false
      };
    }
  }
}
