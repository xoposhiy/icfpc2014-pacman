using System;
using System.Text;

namespace Lib.NewLang
{
	public class CompilationWriter
	{
		public CompilationWriter(string indent = "")
		{
			this.indent = indent;
		}

		private StringBuilder program = new StringBuilder();
		private StringBuilder functions = new StringBuilder();
		private readonly string indent;

		public void Indented(Action<CompilationWriter> compile)
		{
			var indentedWriter = new CompilationWriter(indent + "\t");
			compile(indentedWriter);
			program.Append(indentedWriter.program);
			functions.Append(indentedWriter.functions);
		}

		public void Write(string format, params object[] args)
		{
			program.AppendLine(indent + string.Format(format, args));
		}

		public override string ToString()
		{
			return program.ToString();
		}

		public void HelperWriter(Action<CompilationWriter> compile)
		{
			var helperWriter = new CompilationWriter(indent);
			compile(helperWriter);
			functions.Append(helperWriter.program);
			functions.Append(helperWriter.functions);
		}

		public void FlushFunctions()
		{
			program.Append(functions);
			functions.Clear();
		}
	}
}