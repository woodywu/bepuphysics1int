using BEPUbenchmark.Benchmarks;

namespace BEPUbenchmark
{
	public static class AllBenchmarks
	{
		public static Benchmark[] Benchmarks = { new PathFollowingBenchmark(), new SelfCollidingClothBenchmark(), new PyramidBenchmark() };
	}
}
