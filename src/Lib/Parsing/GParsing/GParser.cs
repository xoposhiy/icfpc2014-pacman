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

		protected override bool IsValidConstantName(string constantName)
		{
			return !IsRegisterName(constantName);
		}

		protected override bool TryGetParameterValue([NotNull] string argString, [NotNull] ParameterInfo parameterInfo, [NotNull] Type programItemType, [NotNull] Dictionary<string, int> labels, [NotNull] Dictionary<int, uint> sourceLineToAddress, [NotNull] Dictionary<string, int> constants, out object parameter)
		{
			if (parameterInfo.ParameterType == typeof(byte))
			{
				parameter = null;
				int address;
				if (labels.TryGetValue(argString, out address))
				{
					parameter = (byte)sourceLineToAddress[address];
					return true;
				}
				if (int.TryParse(argString, out address))
				{
					parameter = (byte)address;
					return true;
				}
				return false;
			}
			if (parameterInfo.ParameterType == typeof(int))
			{
				parameter = null;
				int value;
				if (constants.TryGetValue(argString, out value) || int.TryParse(argString, out value))
				{
					parameter = (byte)value;
					return true;
				}
				return false;
			}
			if (parameterInfo.ParameterType == typeof(GArg))
			{
				parameter = null;
				if (argString.StartsWith("["))
				{
					if (!argString.EndsWith("]"))
						return false;
					argString = argString.Substring(1, argString.Length - 2).Trim();
					int address;
					if (constants.TryGetValue(argString, out address) || int.TryParse(argString, out address))
					{
						parameter = GArg.Data((byte)address);
						return true;
					}
					if (IsRegisterName(argString))
					{
						parameter = GArg.IndirectReg((byte)(char.ToLower(argString[0]) - 'a'));
						return true;
					}
					return false;
				}
				int value;
				if (constants.TryGetValue(argString, out value) || int.TryParse(argString, out value))
				{
					parameter = GArg.Const((byte)value);
					return true;
				}
				if (labels.TryGetValue(argString, out value))
				{
					parameter = GArg.Const((byte)sourceLineToAddress[value]);
					return true;
				}
				if (argString.Length == 1 && char.ToLower(argString[0]) >= 'a' && char.ToLower(argString[0]) <= 'h')
				{
					parameter = GArg.Reg((byte)(char.ToLower(argString[0]) - 'a'));
					return true;
				}
				return false;
			}
			throw new InvalidOperationException(string.Format("Invalid constructor parameter '{0}' type ({1}) of Instruction '{2}'", parameterInfo.Name, parameterInfo.ParameterType, programItemType));
		}

		private static bool IsRegisterName([NotNull] string argString)
		{
			return argString.Length == 1 && char.ToLower(argString[0]) >= 'a' && char.ToLower(argString[0]) <= 'h';
		}
	}
}