using BEPUphysics;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUutilities;
using FixMath.NET;
using Xunit;
using System.Security.Cryptography;
using System.Text;
using Xunit.Abstractions;

namespace BEPUtests
{
	public class CrossPlatformDeterminism
    {
		private readonly ITestOutputHelper output;

		public CrossPlatformDeterminism(ITestOutputHelper output)
		{
			this.output = output;
		}

		[Fact]
		public void RunsDeterministically()
		{
			Space space = new Space();
			space.ForceUpdater.Gravity = new Vector3(0, (Fix64)(-9.81m), 0); //If left unset, the default value is (0,0,0).
			// Ensure test is robust against changes of the default value
			space.TimeStepSettings.TimeStepDuration = 1 / 60m;

			Fix64 boxSize = 2;
			int boxCount = 20;
			Fix64 platformLength = MathHelper.Min(50, boxCount * boxSize + 10);
			space.Add(new Box(new Vector3(0, -.5m, 0), boxCount * boxSize + 20, 1,
							  platformLength));

			for (int i = 0; i < boxCount; i++)
			{
				for (int j = 0; j < boxCount - i; j++)
				{
					space.Add(new Box(
								  new Vector3(
									  -boxCount * boxSize / 2 + boxSize / 2 * i + j * (boxSize),
									  (boxSize / 2) + i * boxSize,
									  0),
								  boxSize, boxSize, boxSize, 20));
				}
			}
			//Down here are the 'destructors' used to blow up the pyramid.

			Sphere pow = new Sphere(new Vector3(-25, 5, 70), 2, 40);
			pow.LinearVelocity = new Vector3(0, 10, -100);
			space.Add(pow);
			pow = new Sphere(new Vector3(-15, 10, 70), 2, 40);
			pow.LinearVelocity = new Vector3(0, 10, -100);
			space.Add(pow);
			pow = new Sphere(new Vector3(-5, 15, 70), 2, 40);
			pow.LinearVelocity = new Vector3(0, 10, -100);
			space.Add(pow);
			pow = new Sphere(new Vector3(5, 15, 70), 2, 40);
			pow.LinearVelocity = new Vector3(0, 10, -100);
			space.Add(pow);
			pow = new Sphere(new Vector3(15, 10, 70), 2, 40);
			pow.LinearVelocity = new Vector3(0, 10, -100);
			space.Add(pow);
			pow = new Sphere(new Vector3(25, 5, 70), 2, 40);
			pow.LinearVelocity = new Vector3(0, 10, -100);
			space.Add(pow);
						
			for (int i = 0; i < 1000; i++)
			{
				space.Update();
				if (i % 20 == 0)
					Assert.Equal(expectedHashes[i / 20], HashSpace(space));
			}
		}
		
		private string HashSpace(Space space)
		{
			const int valuesPerEntity = 3 + 4 + 3;
			byte[] state = new byte[space.Entities.Count * valuesPerEntity * sizeof(long)];
			int offset = 0;
			foreach (Entity e in space.Entities)
			{
				Fix64IntoByteArray(e.Position.X, offset++, state);
				Fix64IntoByteArray(e.Position.Y, offset++, state);
				Fix64IntoByteArray(e.Position.Z, offset++, state);

				Fix64IntoByteArray(e.Orientation.X, offset++, state);
				Fix64IntoByteArray(e.Orientation.Y, offset++, state);
				Fix64IntoByteArray(e.Orientation.Z, offset++, state);
				Fix64IntoByteArray(e.Orientation.W, offset++, state);

				Fix64IntoByteArray(e.LinearVelocity.X, offset++, state);
				Fix64IntoByteArray(e.LinearVelocity.Y, offset++, state);
				Fix64IntoByteArray(e.LinearVelocity.Z, offset++, state);
			}
			HashAlgorithm md5 = MD5.Create();
			byte[] hash = md5.ComputeHash(state);

			StringBuilder hex = new StringBuilder(hash.Length * 2);
			foreach (byte b in hash)
				hex.AppendFormat("{0:x2}", b);

			return hex.ToString();
		}

		private void Fix64IntoByteArray(Fix64 value, int offset, byte[] destination)
		{
			offset *= sizeof(long);
			long raw = value.RawValue;
			destination[offset++] = (byte)(raw & 0xFF);
			destination[offset++] = (byte)((raw >> 8) & 0xFF);
			destination[offset++] = (byte)((raw >> 16) & 0xFF);
			destination[offset++] = (byte)((raw >> 24) & 0xFF);
		}

		private readonly string[] expectedHashes = {
			"76f05a961f08670e31c855731ff2f0a9",
			"e630ee102bfa6bb297b9ec3b771bdf6a",
			"9e493b2f429329ea953271ea02217ba9",
			"6e51d99d274326de70f5954efe8ac467",
			"fb2762c4ec1ad16e4e965555ee994978",
			"40dfd25d501cb779c725462efcacd80e",
			"65186b03561102594c06196b0310d68d",
			"14facef79ee2982ec9770f1df0bad583",
			"2a96c83a19245388e7a4986b7ca10a8a",
			"3652a67ce7fa54a55d4a5a4be2d88976",
			"aaabc6a21c2f417f0a9c4fba8189a85f",
			"514861cc4e17c05e6d5359add6f4ee93",
			"321323123bdcc6fee6f551407691087f",
			"70dcb2be67927d101142947619db7dbb",
			"68b94aa2bed4534b946ef5d03d1ab9f6",
			"467ff9f3fcb61c561226248019d89e4b",
			"9974ff4b5eec37fa2b0ac586be914c15",
			"7790c317c3d507f46d12bba1a622914b",
			"673326b7ac5367e57ae7b8dc256fcece",
			"d9b3fc25d1aac9f361b2cfe9af5b7e81",
			"7d666d4c8492a5012c4ba9c95665ea48",
			"c3220fea99590cf5e16a2c64d7208e31",
			"76f010d89f0c99d5f6c0ad16501e6d26",
			"0c490b6756f3dfd5ef3383fb81e7f6f4",
			"4b75179d5b9041919c2d95c28983f0c7",
			"fc419cbb7180ecad67a23be88a7fa759",
			"f5cfe9928489d43c3cf2d3c733dd44ad",
			"b80713a15a4d1f9f457be3ea28434699",
			"c1968564b83627db03218dba84841f97",
			"3ac845033ffd256df960ff09fc9a5ea5",
			"a1bbae3ecef8850c3364719d58c5c1dc",
			"63e65ecf3904825a50149d8a6b836b73",
			"0a69050b6fe7146bac6bbec204387d19",
			"bec4d782ec7c85cc9f7829f60a49c94b",
			"7d2b2957e39c32efde90ff0f6b81d3cc",
			"d1d22a943385068fa7bc4bc4b93f33db",
			"17ab068cbf28da9468193ac4fc379760",
			"5b925bf85ff0072777c1541ea1c1cdd3",
			"46b10dd8fd57c529e843f56e34ff8b61",
			"81d79b0a15a084a4a04a5020a3c5f547",
			"30d0290a68ea0282777ea0e18c6e7ab2",
			"f7564fed1edc3a8c64c96d503673624a",
			"4280e4315c2c64c5efc074433f5839fa",
			"807de4a222043984ec41551abceb887b",
			"5df4baba4dedfad8ac2b281e68ac1083",
			"8808eabcb0ea042eff25e4cb50af0d8b",
			"28e8cf4844b468441b328c142b15b645",
			"58404d2d4be623843a6536ce5b1eb08e",
			"f74fb1b743e38886c81f673e399e91c9",
			"885204aa7570f75c3fafefb5024a7cb0"
		};
	}
}
