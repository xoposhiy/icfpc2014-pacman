using Lib.LMachine.Intructions;

namespace Lib.LMachine.Parsing
{
	public class LParseResult
	{
		[NotNull]
		public Instruction[] Program { get; set; }

		public int[] SourceLines { get; set; }
		
		public string[] CodeLines { get; set; }
	}
}