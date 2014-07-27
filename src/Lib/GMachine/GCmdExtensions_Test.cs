using System;
using System.IO;
using Lib.AI;
using NUnit.Framework;

namespace Lib.GMachine
{
	[TestFixture]
	public class GCmdExtensions_Test
	{
		[Test]
		public void Chasing_ToGhc()
		{
			var ghc = Ghost.ByProgram("chasing.mghc").ParseResult.Program.ToGhc();
			Console.Out.WriteLine(ghc);
			File.WriteAllText(KnownPlace.Ghosts + "chasing.ghc", ghc);
		}

		[Test]
		public void ToGhc()
		{
			var gCmds = Ghost.ByProgram("chasing.mghc").ParseResult;
			var ghc1 = gCmds.Program.ToGhc();
			var ghc2 = Ghost.ByProgram("chasing.ghc").ParseResult.Program.ToGhc();
			Assert.AreEqual(ghc1, ghc2);
		}
	}
}