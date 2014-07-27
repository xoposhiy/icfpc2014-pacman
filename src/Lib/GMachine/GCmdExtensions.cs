using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lib.AI;
using NUnit.Framework;

namespace Lib.GMachine
{
	public static class GCmdExtensions
	{
		[NotNull]
		public static string ToGhc([NotNull] this IEnumerable<GCmd> program)
		{
			return string.Join("\r\n", program.Select(x => x.ToGhc()));
		}
	}

	[TestFixture]
	public class GCmdExtensions_Test
	{
		[Test]
		public void Chasing_ToGhc()
		{
			var ghc = Ghost.ByProgram("chasing.mghc").Program.ToGhc();
			Console.Out.WriteLine(ghc);
			File.WriteAllText(KnownPlace.Ghosts + "chasing.ghc", ghc);
		}
	}
}