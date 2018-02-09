using BEPUphysics.Entities.Prefabs;
using BEPUutilities;
using FixMath.NET;

namespace BEPUphysicsDemos.Demos.Extras.Tests
{
    /// <summary>
    /// Demo showing a wall of blocks stacked up.
    /// </summary>
    public class StackDemo : StandardDemo
    {
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public StackDemo(DemosGame game)
            : base(game)
        {
            kapow.PositionUpdateMode = BEPUphysics.PositionUpdating.PositionUpdateMode.Continuous;
            int height = 50;
            Fix64 blockWidth = 3;
            Fix64 blockHeight = 1;
            Fix64 blockLength = 3;

            for (int i = 0; i < height; i++)
            {
                    var toAdd =
                        new Box(
                            new Vector3(
                                0,
                                blockHeight * .5m + i * (blockHeight),
                                0),
                            blockWidth, blockHeight, blockLength, 10);
                    Space.Add(toAdd);
            }

            Box ground = new Box(new Vector3(0, -.5m, 0), 50, 1, 50);
            Space.Add(ground);

            game.Camera.Position = new Vector3(0, 6, 15);
        }

        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Stack"; }
        }
    }
}