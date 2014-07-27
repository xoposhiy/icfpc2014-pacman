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
			CompileAndSave("chasing");
		}

		[Test]
		public void Chasing2_ToGhc()
		{
			CompileAndSave("chasing2");
		}

		private static void CompileAndSave(string prog)
		{
			var ghc = Ghost.ByProgram(prog + ".mghc").ParseResult.Program.ToGhc();
			Console.Out.WriteLine(ghc);
//			Clipboard.SetText(ghc);
			File.WriteAllText(KnownPlace.Ghosts + prog + ".ghc", ghc);
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