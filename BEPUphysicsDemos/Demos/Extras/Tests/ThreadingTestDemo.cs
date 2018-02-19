using BEPUphysics.Entities.Prefabs;
using System.Diagnostics;
using System;
using BEPUutilities;
using BEPUphysics.Constraints;
using FixMath.NET;

namespace BEPUphysicsDemos.Demos.Extras.Tests
{
    /// <summary>
    /// Demo showing a wall of blocks stacked up.
    /// </summary>
    public class ThreadingTestDemo : StandardDemo
    {
        public void InitializeSimulation(bool threaded)
        {
            //            Space.Dispose();
            //            SolverSettings.DefaultMinimumIterations = 10;
            //            Space = new BEPUphysics.Space();

            //            if (threaded)
            //            {
            //#if XBOX360
            //            //Note that not all four available hardware threads are used.
            //            //Currently, BEPUphysics will allocate an equal amount of work to each thread on the xbox360.
            //            //If two threads are put on one core, it will bottleneck the engine and run significantly slower than using 3 hardware threads.
            //            Space.ThreadManager.AddThread(delegate { Thread.CurrentThread.SetProcessorAffinity(new[] { 1 }); }, null);
            //            Space.ThreadManager.AddThread(delegate { Thread.CurrentThread.SetProcessorAffinity(new[] { 3 }); }, null);
            //            Space.ThreadManager.AddThread(delegate { Thread.CurrentThread.SetProcessorAffinity(new[] { 5 }); }, null);

            //#else
            //                if (Environment.ProcessorCount > 1)
            //                {
            //                    for (int i = 0; i < Environment.ProcessorCount; i++)
            //                    {
            //                        Space.ThreadManager.AddThread();
            //                    }
            //                }
            //#endif
            //            }

            for (int i = Space.Entities.Count - 1; i >= 0; i--)
            {
                Space.Remove(Space.Entities[i]);
            }
            SolverSettings.DefaultMinimumIterationCount = 100;

            int width = 15;
            int height = 15;
            Fix64 blockWidth = 2;
            Fix64 blockHeight = 1;
            Fix64 blockLength = 1;



            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var toAdd =
                        new Box(
                            new Vector3(
                                i * blockWidth + .5m * blockWidth * (j % 2) - width * blockWidth * .5m,
                                blockHeight * .5m + j * (blockHeight),
                                0),
                            blockWidth, blockHeight, blockLength, 10);
                    toAdd.ActivityInformation.IsAlwaysActive = true;
                    Space.Add(toAdd);
                }
            }
            Box ground = new Box(new Vector3(0, -.5m, 0), 50, 1, 50);
            Space.Add(ground);

            GC.Collect();
        }

        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public ThreadingTestDemo(DemosGame game)
            : base(game)
        {

            int numRuns = 10;
            int numFrames = 100;


            //SINGLE THREADED
            //singleThreadedTime = 0;
            //for (int i = 0; i < numRuns; i++)
            //{
            //    InitializeSimulation(0);
            //    double startTime = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;
            //    for (int j = 0; j < numFrames; j++)
            //    {
            //        Space.Update();
            //    }
            //    double endTime = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;
            //    singleThreadedTime += endTime - startTime;
            //}
            //singleThreadedTime /= (numRuns * numFrames);

            //MULTI THREADED
            multiThreadedTime = 0;
            for (int i = 0; i < numRuns; i++)
            {
                InitializeSimulation(true);
                double startTime = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;
                for (int j = 0; j < numFrames; j++)
                {
                    Space.Update();
                }
                double endTime = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;
                multiThreadedTime += endTime - startTime;
            }
            multiThreadedTime /= (numRuns * numFrames);


        }


        double singleThreadedTime;
        double multiThreadedTime;


        public override void DrawUI()
        {
            base.DrawUI();
            Game.DataTextDrawer.Draw("Time per SingleThreaded:    ", singleThreadedTime * 1e6, new Microsoft.Xna.Framework.Vector2(50, 50));
            Game.DataTextDrawer.Draw("Time per Multithreaded:    ", multiThreadedTime * 1e6, new Microsoft.Xna.Framework.Vector2(50, 80));

        }



        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Test4"; }
        }

    }
}