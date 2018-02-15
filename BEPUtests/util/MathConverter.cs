using BEPUutilities;

using FloatMatrix3x3 = BEPUutilitiesFloat.Matrix3x3;
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

	}
}
