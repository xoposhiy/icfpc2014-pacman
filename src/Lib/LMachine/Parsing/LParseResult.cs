using Lib.LMachine.Intructions;

namespace Lib.LMachine.Parsing
{
	public class LParseResult
	{
		[NotNull]
		public Instruction[] Program { get; set; }

		public int[] SourceLines { get; set; }

		public string[] AddressNames { get; set; }

		public CodeLine[] CodeLines { get; set; }
	}
}