using BEPUphysics;
using BEPUutilities;
using BEPUutilities.Threading;
using FixMath.NET;
using System;

namespace BEPUbenchmark
{
	abstract class Benchmark
	{
		protected Space space;
		
		protected abstract void InitializeSpace();
		protected virtual void Step()
		{
		}

		public double Run()
		{			
			space = new Space();
			space.ForceUpdater.Gravity = new Vector3(0, (Fix64)(-9.81m), 0);
			space.TimeStepSettings.TimeStepDuration = 1 / 60m;

			InitializeSpace();

			Console.WriteLine("");
			long startTime = DateTime.Now.Ticks;
			long opStartTime = DateTime.Now.Ticks;
			int opCount = 0;
			for (int i = 0; i < 1000; i++)
			{
				Step();
				space.Update();
				opCount++;
				long time = DateTime.Now.Ticks - opStartTime;
				if (time > TimeSpan.TicksPerSecond)
				{
					Console.Write(string.Format("\rAvg. duration per Step: {0}ms                ", (time/TimeSpan.TicksPerMillisecond)/opCount));
					opCount = 0;
					opStartTime = DateTime.Now.Ticks;
				}					
			}

			long runTime = (DateTime.Now.Ticks - startTime);

			Console.Write("\r                                                                          \r");
			return (double)runTime / TimeSpan.TicksPerSecond;
		}

		public string GetName()
		{
			return this.GetType().Name;
		}
	}
}
