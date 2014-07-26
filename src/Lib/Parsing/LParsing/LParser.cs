using System;
using System.Collections.Generic;
using System.Reflection;
using Lib.LMachine.Intructions;

namespace Lib.Parsing.LParsing
{
	public class LParser : ParserBase<Instruction>
	{
		private LParser()
			: base(new[] { ' ', '\t' })
		{
		}

		[NotNull]
		public static ParseResult<Instruction> Parse([NotNull] string source)
		{
			var res = new LParser().DoParse(source);
			for (int i = 0; i < res.Program.Length; i++)
			{
				res.Program[i].SourceLineNo = res.SourceLines[i];
			}
			return res;
		}

		protected override bool TryGetParameterValue([NotNull] string argString, [NotNull] ParameterInfo parameterInfo, [NotNull] Type programItemType, [NotNull] Dictionary<string, int> labels, [NotNull] Dictionary<int, uint> sourceLineToAddress, [NotNull] Dictionary<string, int> constants, out object parameter)
		{
			parameter = null;
			if (parameterInfo.ParameterType == typeof(int))
			{
				int value;
				if (!constants.TryGetValue(argString, out value) && !int.TryParse(argString, out value))
					return false;
				parameter = value;
				return true;
			}
			if (parameterInfo.ParameterType == typeof(uint))
			{
				uint value;
				int labelSourceLine;
				if (labels.TryGetValue(argString, out labelSourceLine))
					value = sourceLineToAddress[labelSourceLine];
				else if (!uint.TryParse(argString, out value))
					return false;
				parameter = value;
				return true;
			}
			throw new InvalidOperationException(string.Format("Invalid constructor parameter '{0}' type ({1}) of Instruction '{2}'", parameterInfo.Name, parameterInfo.ParameterType, programItemType));
		}
	}
}