using System;
using System.Collections.Generic;
using System.Reflection;
using Lib.GMachine;

namespace Lib.Parsing.GParsing
{
	public class GParser : ParserBase<GCmd>
	{
		private GParser()
			: base(new[] { ',' })
		{
		}

		[NotNull]
		public static ParseResult<GCmd> Parse([NotNull] string source)
		{
			return new GParser().DoParse(source);
		}

		protected override bool TryGetParameterValue([NotNull] string argString, [NotNull] ParameterInfo parameterInfo, [NotNull] Type programItemType, [NotNull] Dictionary<string, int> labels, [NotNull] Dictionary<int, uint> sourceLineToAddress, [NotNull] Dictionary<string, int> constants, out object parameter)
		{
			if (parameterInfo.ParameterType != typeof(GArg))
				throw new InvalidOperationException(string.Format("Invalid constructor parameter '{0}' type ({1}) of Instruction '{2}'", parameterInfo.Name, parameterInfo.ParameterType, programItemType));
			parameter = null;
			if (argString.StartsWith("["))
			{
				if (!argString.EndsWith("]"))
					return false;
				argString = argString.Substring(1, argString.Length - 2).Trim();
				int address;
				if (labels.TryGetValue(argString, out address) || int.TryParse(argString, out address))
				{
					parameter = GArg.Data((byte)address);
					return true;
				}
				if (argString.Length == 1 && char.ToLower(argString[0]) >= 'a' && char.ToLower(argString[0]) <= 'h')
				{
					parameter = GArg.IndirectReg((byte)(char.ToLower(argString[0]) - 'a'));
					return true;
				}
			}
			int value;
			if (constants.TryGetValue(argString, out value) || int.TryParse(argString, out value))
			{
				parameter = GArg.Const((byte)value);
				return true;
			}
			if (argString.Length == 1 && char.ToLower(argString[0]) >= 'a' && char.ToLower(argString[0]) <= 'h')
			{
				parameter = GArg.Reg((byte)(char.ToLower(argString[0]) - 'a'));
				return true;
			}
			return false;
		}
	}
}