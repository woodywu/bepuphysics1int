using FixMath.NET;
using Xunit;
using Xunit.Abstractions;
using System.Collections.Generic;
using BEPUutilities;
using FloatMatrix3x3 = BEPUutilitiesFloat.Matrix3x3;
using BEPUtests.util;
using System.Linq;
using System.Diagnostics;
using System;

namespace BEPUtests
{
	public class Matrix3x3Tests
    {
		Matrix3x3[] testCases = {
			Matrix3x3.Identity,
			new Matrix3x3(6770.833m, 0, 0, 0, 13500, 0, 0, 0, 6770.833m),
			new Matrix3x3(0.6770833m, 0, 0, 0, 1.35m, 0, 0, 0, 0.6770833m),
			new Matrix3x3(0, 0, 0, 0, 13500, 0, 0, 0, 6770.833m)
		};

		private readonly ITestOutputHelper output;
		
		public Matrix3x3Tests(ITestOutputHelper output)
		{
			if (output == null)
				output = new ConsoleTestOutputHelper();
			this.output = output;
		}


		[Fact]
		public void Invert()
		{
			var maxDelta = 0.00001m;

			var deltas = new List<decimal>();

			// Scalability and edge cases
			foreach (var m in testCases)
			{
				Matrix3x3 testCase = m;

				FloatMatrix3x3 floatMatrix = MathConverter.Convert(testCase);
				FloatMatrix3x3 expected;
				FloatMatrix3x3.Invert(ref floatMatrix, out expected);

				Matrix3x3 actual;
				if (float.IsInfinity(expected.M11) || float.IsNaN(expected.M11))
				{
					Assert.Throws<ArgumentException>(() => Matrix3x3.Invert(ref testCase, out actual));
				}
				else
				{
					Matrix3x3.Invert(ref testCase, out actual);
					bool success = true;
					foreach (decimal delta in GetDeltas(expected, actual))
					{
						deltas.Add(delta);
						success &= delta <= maxDelta;

					}
					Assert.True(success, string.Format("Precision: Matrix3x3Invert({0}): Expected {1} Actual {2}", testCase, expected, actual));
				}
			}
			output.WriteLine("Max error: {0} ({1} times precision)", deltas.Max(), deltas.Max() / Fix64.Precision);
			output.WriteLine("Average precision: {0} ({1} times precision)", deltas.Average(), deltas.Average() / Fix64.Precision);
		}

		[Fact]
		public void BenchmarkInvert()
		{
			var swf = new Stopwatch();
			var swd = new Stopwatch();

			var deltas = new List<decimal>();

			foreach (var m in testCases)
			{
				Matrix3x3 testCase = m;

				for (int i = 0; i < 10000; i++)
				{
					FloatMatrix3x3 floatMatrix = MathConverter.Convert(testCase);
					FloatMatrix3x3 expected;
					swf.Start();
					FloatMatrix3x3.Invert(ref floatMatrix, out expected);
					swf.Stop();

					Matrix3x3 actual;
					swd.Start();
					if (float.IsInfinity(expected.M11) || float.IsNaN(expected.M11))
					{
						Assert.Throws<ArgumentException>(() => Matrix3x3.Invert(ref testCase, out actual));
						swd.Stop();
						continue;
					}
					else
						Matrix3x3.Invert(ref testCase, out actual);
					swd.Stop();

					foreach (decimal delta in GetDeltas(expected, actual))
						deltas.Add(delta);
				}
			}
			output.WriteLine("Max error: {0} ({1} times precision)", deltas.Max(), deltas.Max() / Fix64.Precision);
			output.WriteLine("Average precision: {0} ({1} times precision)", deltas.Average(), deltas.Average() / Fix64.Precision);
			output.WriteLine("Fix64.Invert time = {0}ms, float.Invert time = {1}ms", swf.ElapsedMilliseconds, swd.ElapsedMilliseconds);
		}

		[Fact]
		public void AdaptiveInvert()
		{
			var maxDelta = 0.00001m;

			var deltas = new List<decimal>();

			// Scalability and edge cases
			foreach (var m in testCases)
			{
				Matrix3x3 testCase = m;

				FloatMatrix3x3 floatMatrix = MathConverter.Convert(testCase);
				FloatMatrix3x3 expected;
				FloatMatrix3x3.AdaptiveInvert(ref floatMatrix, out expected);
				
				Matrix3x3 actual;
				Matrix3x3.AdaptiveInvert(ref testCase, out actual);

				bool success = true;
				foreach (decimal delta in GetDeltas(expected, actual))
				{
					deltas.Add(delta);
					success &= delta <= maxDelta;
					
				}
				Assert.True(success, string.Format("Precision: Matrix3x3Invert({0}): Expected {1} Actual {2}", testCase, expected, actual));
			}
			output.WriteLine("Max error: {0} ({1} times precision)", deltas.Max(), deltas.Max() / Fix64.Precision);
			output.WriteLine("Average precision: {0} ({1} times precision)", deltas.Average(), deltas.Average() / Fix64.Precision);
		}

		[Fact]
		public void BenchmarkAdaptiveInvert()
		{
			var swf = new Stopwatch();
			var swd = new Stopwatch();

			var deltas = new List<decimal>();

			foreach (var m in testCases)
			{
				Matrix3x3 testCase = m;

				for (int i = 0; i < 10000; i++)
				{
					FloatMatrix3x3 floatMatrix = MathConverter.Convert(testCase);
					FloatMatrix3x3 expected;
					swf.Start();
					FloatMatrix3x3.AdaptiveInvert(ref floatMatrix, out expected);
					swf.Stop();


					Matrix3x3 actual;
					swd.Start();
					Matrix3x3.AdaptiveInvert(ref testCase, out actual);
					swd.Stop();

					foreach (decimal delta in GetDeltas(expected, actual))
						deltas.Add(delta);
				}
			}
			output.WriteLine("Max error: {0} ({1} times precision)", deltas.Max(), deltas.Max() / Fix64.Precision);
			output.WriteLine("Average precision: {0} ({1} times precision)", deltas.Average(), deltas.Average() / Fix64.Precision);
			output.WriteLine("Fix64.AdaptiveInvert time = {0}ms, float.AdaptiveInvert time = {1}ms", swf.ElapsedMilliseconds, swd.ElapsedMilliseconds);
		}

		decimal[] GetDeltas(FloatMatrix3x3 expected, Matrix3x3 actual)
		{
			decimal[] result = new decimal[9];
			int i = 0;
			result[i++] = (decimal)actual.M11 - (decimal)expected.M11;
			result[i++] = (decimal)actual.M12 - (decimal)expected.M12;
			result[i++] = (decimal)actual.M13 - (decimal)expected.M13;

			result[i++] = (decimal)actual.M21 - (decimal)expected.M21;
			result[i++] = (decimal)actual.M22 - (decimal)expected.M22;
			result[i++] = (decimal)actual.M23 - (decimal)expected.M23;

			result[i++] = (decimal)actual.M31 - (decimal)expected.M31;
			result[i++] = (decimal)actual.M32 - (decimal)expected.M32;
			result[i++] = (decimal)actual.M33 - (decimal)expected.M33;

			return result;
		}
	}
}
