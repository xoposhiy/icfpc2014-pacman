using System;
using System.IO;
using Lib.Game;
using Lib.GMachine;
using Lib.Parsing.GParsing;

namespace Lib.AI
{
	public class Ghost
	{
		[NotNull]
		public static Ghost ByType<TGhost>() where TGhost : IGMachine
		{
			return new Ghost
			{
				GhostType = typeof(TGhost)
			};
		}

		[NotNull]
		public static Ghost ByProgram([NotNull] string name)
		{
			return new Ghost
			{
				Program = GParser.Parse(File.ReadAllText(KnownPlace.Ghosts + name)).Program
			};
		}

		private Ghost()
		{
		}

		[CanBeNull]
		public GCmd[] Program { get; private set; }

		[CanBeNull]
		public Type GhostType { get; private set; }
	}
}