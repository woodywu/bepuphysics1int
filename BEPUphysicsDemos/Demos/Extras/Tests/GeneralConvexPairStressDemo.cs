using BEPUphysics.Entities;
using System;
using BEPUphysics.CollisionShapes;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUutilities;
using FixMath.NET;

namespace BEPUphysicsDemos.Demos.Extras.Tests
{
    /// <summary>
    /// Demo showing a wall of blocks stacked up.
    /// </summary>
    public class GeneralConvexPairStressDemo : StandardDemo
    {


        Random random = new Random();
        Fix64 width = 45;
        Fix64 height = 45;
        Fix64 length = 45;
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public GeneralConvexPairStressDemo(DemosGame game)
            : base(game)
        {
            Space.Remove(vehicle.Vehicle);
            //Enable simplex caching.
            ConfigurationHelper.ApplySuperSpeedySettings(Space);

            for (int i = 0; i < 2000; i++)
            {
                EntityShape shape;
                switch (i % 3)
                {
                    case 0:
                        shape = new CylinderShape(0.5m + (Fix64)random.NextDouble() * 1.5m, 0.5m + (Fix64)random.NextDouble() * 1.5m);
                        break;
                    case 1:
                        shape = new ConeShape(0.5m + (Fix64)random.NextDouble() * 1.5m, 0.5m + (Fix64)random.NextDouble() * 1.5m);
                        break;
                    default:
                        shape = new CapsuleShape(0.5m + (Fix64)random.NextDouble() * 1.5m, 0.5m + (Fix64)random.NextDouble() * 1.5m);
                        break;

                }

                var toAdd = new Entity(shape, 2);
                //toAdd.LocalInertiaTensorInverse = new BEPUutilities.Matrix3x3();
                RandomizeEntityState(toAdd);
                Space.Add(toAdd);

            }
            Space.ForceUpdater.Gravity = new Vector3();

            game.Camera.Position = new Vector3(0, 6, 15);
        }

        private void RandomizeEntityState(Entity entity)
        {
            entity.Position = new Vector3(
                (Fix64)random.NextDouble() * width,
                (Fix64)random.NextDouble() * height,
                (Fix64)random.NextDouble() * length);
            entity.Orientation = Quaternion.CreateFromAxisAngle(
                Vector3.Normalize(new Vector3(
                    (Fix64)random.NextDouble() * 2 - 1,
                    (Fix64)random.NextDouble() * 2 - 1,
                    (Fix64)random.NextDouble() * 2 - 1)),
                (Fix64)random.NextDouble() * 50);
            Fix64 linearFactor = 0.5m;
            entity.LinearVelocity = new Vector3(
                ((Fix64)random.NextDouble() * 2 - 1) * linearFactor,
                ((Fix64)random.NextDouble() * 2 - 1) * linearFactor,
                ((Fix64)random.NextDouble() * 2 - 1) * linearFactor);
            Fix64 angularFactor = 0.5m;
            entity.AngularVelocity = new Vector3(
                ((Fix64)random.NextDouble() * 2 - 1) * angularFactor,
                ((Fix64)random.NextDouble() * 2 - 1) * angularFactor,
                ((Fix64)random.NextDouble() * 2 - 1) * angularFactor);
        }

        public override void Update(Fix64 dt)
        {
            for (int i = 0; i < 10; i++)
            {
                RandomizeEntityState(Space.Entities[random.Next(Space.Entities.Count)]);
            }
            base.Update(dt);
        }

        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "General Convex Pair Stress Test"; }
        }
    }
}