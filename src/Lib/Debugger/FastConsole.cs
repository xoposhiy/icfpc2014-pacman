using System;
using System.Collections.Generic;

namespace Lib.Debugger
{
	public class FastConsole
	{
		private readonly List<FastConsoleRow> rows = new List<FastConsoleRow>();

		public FastConsoleWriter BeginWrite()
		{
			return new FastConsoleWriter(this);
		}

		public class FastConsoleWriter
		{
			private readonly FastConsole fastConsole;
			private readonly List<FastConsoleRow> rows = new List<FastConsoleRow>();
			private int line = 0;

			public FastConsoleWriter(FastConsole fastConsole)
			{
				ResetColor();
				this.fastConsole = fastConsole;
			}

			public void WriteLine(object value)
			{
				WriteLine(Convert.ToString(value));
			}

			public void WriteLine(string text)
			{
				Write(text);
				WriteLine();
			}

			public ConsoleColor BackgroundColor { get; set; }
			public ConsoleColor ForegroundColor { get; set; }

			public void ResetColor()
			{
				BackgroundColor = ConsoleColor.Black;
				ForegroundColor = ConsoleColor.Gray;
			}

			public void Write(object value)
			{
				Write(Convert.ToString(value));
			}

			public void Write(string text)
			{
				while (rows.Count <= line)
					rows.Add(new FastConsoleRow());
				rows[line].Segments.Add(new FastConsoleRowSegment(ForegroundColor, BackgroundColor, text));
			}

			public void WriteLine()
			{
				line++;
			}

			public void EndWrite()
			{
				for (var i = 0; i < Math.Min(rows.Count, fastConsole.rows.Count); i++)
				{
					if (!rows[i].Equals(fastConsole.rows[i]))
						rows[i].WriteToConsole(i, fastConsole.rows[i].Length);
				}
				for (var i = Math.Min(rows.Count, fastConsole.rows.Count); i < rows.Count; i++)
					rows[i].WriteToConsole(i, 0);
				fastConsole.rows.Clear();
				fastConsole.rows.AddRange(rows);
			}
		}
	}
}