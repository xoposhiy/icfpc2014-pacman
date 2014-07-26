using JetBrains.Annotations;
using LMachine.Intructions;

namespace LMachine.Parsing
{
	public class LParseResult
	{
		[NotNull]
		public Instruction[] Program { get; set; }

		public int[] SourceLines { get; set; }
	}
}