namespace Lib.Parsing
{
	public class ParseResult<TProgramItem>
	{
		[NotNull]
		public TProgramItem[] Program { get; set; }

		public int[] SourceLines { get; set; }

		public string[] AddressNames { get; set; }

		public CodeLine[] CodeLines { get; set; }
	}
}