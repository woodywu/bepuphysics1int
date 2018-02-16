using BEPUbenchmark;
using BEPUbenchmark.Benchmarks;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace BEPUtests
{
	public class CrossPlatformDeterminismTests
    {
		private readonly ITestOutputHelper output;

		public CrossPlatformDeterminismTests(ITestOutputHelper output)
		{
			this.output = output;
		}

		//[Fact]
		public void OutputExpectedHashes()
		{
			StringBuilder result = new StringBuilder();

			result.AppendLine(@"
using System.Collections.Generic;
namespace BEPUtests
{
class CrossPlatformDeterminismExpectedHashes
{
public static readonly Dictionary<string, string[]> Hashes = new Dictionary<string, string[]>()
{");

			bool first = true;
			foreach (Benchmark b in AllBenchmarks.Benchmarks)
			{
				if (!first)
					result.Append(",\n");
				result.AppendFormat("{{\"{0}\", new string[] {{", b.GetName());

				first = false;
				b.Initialize();
				for (int i = 0; i < 50; i++)
				{
					string expectedHash = b.RunToNextHash();
					if (i != 0)
						result.Append(",\n");
					result.AppendFormat("\"{0}\"", expectedHash);
				}
				result.AppendLine("}}");
			}
			result.AppendLine("};\n}\n}\n");
			output.WriteLine(result.ToString());
		}

		[Fact]
		public void OutputExpectedHashesForBenchmark()
		{
			Benchmark b = new DiscreteVsContinuousBenchmark();

			StringBuilder result = new StringBuilder();

			result.AppendFormat("{{\"{0}\", new string[] {{", b.GetName());

			b.Initialize();
			for (int i = 0; i < 50; i++)
			{
				string expectedHash = b.RunToNextHash();
				if (i != 0)
					result.Append(",\n");
				result.AppendFormat("\"{0}\"", expectedHash);
			}
			result.AppendLine("}}");
			output.WriteLine(result.ToString());
		}

		[Fact]
		public void DiscreteVsContinuous()
		{
			TestDeterminism(new DiscreteVsContinuousBenchmark());
		}

		[Fact]
		public void Pyramid()
		{
			TestDeterminism(new PyramidBenchmark());
		}

		[Fact]
		public void PathFollowing()
		{
			TestDeterminism(new PathFollowingBenchmark());
		}

		[Fact]
		public void SelfCollidingCloth()
		{
			TestDeterminism(new SelfCollidingClothBenchmark());
		}

		private void TestDeterminism(Benchmark b)
		{
			b.Initialize();
			string[] expectedHashes = CrossPlatformDeterminismExpectedHashes.Hashes[b.GetName()];
			int step = 0;
			foreach (string expectedHash in expectedHashes)
			{
				string actualHash = b.RunToNextHash();
				Assert.True(expectedHash == actualHash, string.Format("Expected {0}, actual {1} in step {2}", expectedHash, actualHash, step));
				step++;
			}
		}
	}
}
