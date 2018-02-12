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
		private const bool OUTPUT_HASHES = false;
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
			// Ensure test is robust against changes of the default timestep value
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
				{
					string hash = HashSpace(space);
					if (OUTPUT_HASHES)
						output.WriteLine("\"" + hash + "\",");
					else
						Assert.Equal(expectedHashes[i / 20], HashSpace(space));						
				}

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
			"b3998c7a4eea21660aca459eadd0a22a",
			"bd7a1eecbf9f11c2679ea2d7dc889969",
			"38a7e6fbc12c92d2bbad3bd575fb1504",
			"bf549648ebfe304c7fd56af12969bbfb",
			"dd6c6c887774a714497244afb58d4126",
			"ddb8c9949781b88c6deed8190bd2c5a2",
			"747a72651aaa567a999a241d96d149c4",
			"db129a997dc5c706cd3992356d2b9038",
			"8f517b4a1ce5c18727e0a12e4fa66979",
			"bd6ed6a237c7b7dd387dd8a0995fad38",
			"f8c12daf65c75d41b7b3843d350784da",
			"1dcdb810d9467237ea632187fcfa5bbe",
			"3fd4f75bb4943066aedf810343059950",
			"1f771bea492f3faee0de2a9725170872",
			"fc3cbf7256a88c770134f32fcd3da208",
			"321d460d2981681f3cc63950d5d7155f",
			"04cbb804200a378d28073bc793a732b7",
			"368ac7bd2c3075d3460b02b9f4ee4b5c",
			"ccea1e9860491229bd2803c1b0db264e",
			"b4f838237706bd368009f08f7dbc06d4",
			"72197469fa5e68cf09eb8512f7bf047a",
			"f287a0552730e2a2638498efdf34f22f",
			"4416fa2afe1288bf4b9b988880f0c2a7",
			"76fc306360d256df0460fe4e13a30a4d",
			"9859173caeff6464f0bc6f3e65afe067",
			"efa345168cac32b0eefbb5dce0cc766a",
			"16ce76ba7a20da0af6d8b85d516430b0",
			"79640067c275743a12c1c258560966a1",
			"99f3f5f70c01e46a77bf44f7b393a039",
			"48912303d48988fa38adadf3a45b74b2",
			"bd400e7ffe5c9b08a972be7455369046",
			"2bc3d72ed4f35ce06a2656384809c885",
			"48bacf2fe5e1d92eebecb907cf00c0bf",
			"7e86f7e1be86bab2d18aff1f46451b5b",
			"b730a5cdc773309af7c66d2355c6de22",
			"2e7486cc8a9085bb067d4cc0e6f24dac",
			"d914214b847b94ba447ce860e10d0e37",
			"10cbfe51b869fc4784ef4e5ad908163d",
			"ab3455824a05ebc65ea12abeb6d0e82f",
			"f451909d6d07ec142aa5905b5379e4bd",
			"47a908dfea8c5f735683433b3343f411",
			"b1db056dcb723b6265fea4b522369d65",
			"857f40d52dcbf904c9b327480f6cdbae",
			"77bcd427d92551dab010c6d707359a6f",
			"eb8b92350015e03020ea4b70c583d50d",
			"2954c4cb54df0fa1e81993113dc8aee1",
			"28193442e34037426881445f31fe9bac",
			"19e0d97e358e3177aaa8505786375200",
			"8cdf87064d4ab8335c0e1703756d5040",
			"4cfd551668dfc91f35d317a10e54bc39"
		};
	}
}
