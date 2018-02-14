using BEPUbenchmark.Benchmarks;
using System;

namespace BEPUbenchmark
{
	class Program
	{
		static Benchmark[] benchmarks = { new PathFollowingBenchmark(), new SelfCollidingClothBenchmark(), new PyramidBenchmark() };

		static void Main(string[] args)
		{
			Console.WriteLine("Running benchmarks...\n");

			double runtime = 0;
			foreach (Benchmark b in benchmarks)
			{
				Console.Write(b.GetName()+"... ");
				double time = b.Run();

				Console.WriteLine(Math.Round(time, 2) + "s");
				runtime += time;
			}

			Console.WriteLine("\nCumulative runtime: "+Math.Round(runtime, 2));
		}
	}
}
