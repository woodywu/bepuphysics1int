using BEPUutilities;

using FloatMatrix3x3 = BEPUutilitiesFloat.Matrix3x3;
using FloatMatrix = BEPUutilitiesFloat.Matrix;
namespace BEPUtests.util
{
	static class MathConverter
	{
		public static FloatMatrix3x3 Convert(Matrix3x3 matrix)
		{
			FloatMatrix3x3 toReturn;
			Convert(ref matrix, out toReturn);
			return toReturn;
		}

		public static void Convert(ref Matrix3x3 matrix, out FloatMatrix3x3 floatMatrix)
		{
			floatMatrix.M11 = (float)matrix.M11;
			floatMatrix.M12 = (float)matrix.M12;
			floatMatrix.M13 = (float)matrix.M13;

			floatMatrix.M21 = (float)matrix.M21;
			floatMatrix.M22 = (float)matrix.M22;
			floatMatrix.M23 = (float)matrix.M23;

			floatMatrix.M31 = (float)matrix.M31;
			floatMatrix.M32 = (float)matrix.M32;
			floatMatrix.M33 = (float)matrix.M33;
		}

		public static FloatMatrix Convert(Matrix matrix)
		{
			FloatMatrix toReturn;
			Convert(ref matrix, out toReturn);
			return toReturn;
		}

		public static void Convert(ref Matrix matrix, out FloatMatrix floatMatrix)
		{
			floatMatrix.M11 = (float)matrix.M11;
			floatMatrix.M12 = (float)matrix.M12;
			floatMatrix.M13 = (float)matrix.M13;
			floatMatrix.M14 = (float)matrix.M14;

			floatMatrix.M21 = (float)matrix.M21;
			floatMatrix.M22 = (float)matrix.M22;
			floatMatrix.M23 = (float)matrix.M23;
			floatMatrix.M24 = (float)matrix.M24;

			floatMatrix.M31 = (float)matrix.M31;
			floatMatrix.M32 = (float)matrix.M32;
			floatMatrix.M33 = (float)matrix.M33;
			floatMatrix.M34 = (float)matrix.M34;

			floatMatrix.M41 = (float)matrix.M41;
			floatMatrix.M42 = (float)matrix.M42;
			floatMatrix.M43 = (float)matrix.M43;
			floatMatrix.M44 = (float)matrix.M44;
		}

	}
}
