namespace Lib.LMachine.Parsing
{
	public class CodeLine
	{
		public CodeLine([NotNull] string line)
		{
			var command = SkipConstants(SkipLabel(SkipComments(line)));
			Command = command;
		}

		public string Label { get; set; }
		public string Command { get; set; }
		public string Comment { get; set; }
		public string Constant { get; set; }

		public override string ToString()
		{
			return string.Format("{0}{1}{2}{3}", Label, Constant, Command, Comment);
		}

		[NotNull]
		private string SkipComments([NotNull] string line)
		{
			var commentIndex = line.IndexOf(';');
			if (commentIndex >= 0)
			{
				Comment = line.Substring(commentIndex);
				return line.Substring(0, commentIndex);
			}
			return line;
		}

		[NotNull]
		private string SkipLabel([NotNull] string line)
		{
			var labelIndex = line.IndexOf(':');
			if (labelIndex >= 0)
			{
				Label = line.Substring(0, labelIndex + 1);
				return line.Substring(labelIndex + 1);
			}
			return line;
		}

		[NotNull]
		private string SkipConstants([NotNull] string line)
		{
			var labelIndex = line.IndexOf('=');
			if (labelIndex >= 0)
			{
				Constant = line;
				return "";
			}
			return line;
		}
	}
}