using System;
using System.IO;
using Lib.Game;
using Lib.GMachine;
using Lib.Parsing;
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
				ParseResult = GParser.Parse(File.ReadAllText(KnownPlace.Ghosts + name))
			};
		}

		private Ghost()
		{
		}

		[CanBeNull]
		public ParseResult<GCmd> ParseResult { get; private set; }

		[CanBeNull]
		public Type GhostType { get; private set; }
	}
}