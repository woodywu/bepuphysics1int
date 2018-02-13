using BEPUphysics;
using BEPUutilities;
using FixMath.NET;
using System;

namespace BEPUbenchmark
{
	abstract class Benchmark
	{
		protected Space space;
		
		protected abstract void InitializeSpace();
		protected void Step()
		{
		}

		public double Run()
		{
			space = new Space();
			space.ForceUpdater.Gravity = new Vector3(0, (Fix64)(-9.81m), 0);
			space.TimeStepSettings.TimeStepDuration = 1 / 60m;

			InitializeSpace();

			long startTime = DateTime.Now.Ticks;
			for (int i = 0; i < 1000; i++)
			{
				Step();
				space.Update();
			}

			long runTime = (DateTime.Now.Ticks - startTime);

			return (double)runTime / TimeSpan.TicksPerSecond;
		}

		public string GetName()
		{
			return this.GetType().Name;
		}
	}
}
