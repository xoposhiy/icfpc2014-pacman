using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lib.LMachine.Intructions;

namespace Lib.LMachine.Parsing
{
	public static class LParser
	{
		private static readonly char[] whitespaces = { ' ', '\t' };
		private static readonly Dictionary<string, Type> instructionTypes;

		static LParser()
		{
			instructionTypes = typeof(Instruction).Assembly.GetTypes().Where(x => typeof(Instruction).IsAssignableFrom(x) && x != typeof(Instruction)).ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
		}

		[NotNull]
		public static LParseResult Parse([NotNull] string source)
		{
			var lines = source.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
			var codeLines = GetCodeLines(lines);
			var program = new List<Instruction>();
			var sourceLines = new List<int>();
			var labels = ExtractLabels(codeLines);
			var constants = ExtractConstants(codeLines);
			var sourceLineToAddress = GetSourceLineToAddressMap(codeLines);
			var addressNames = new List<string>();
			var addressToName = labels.GroupBy(x => x.Value).ToDictionary(x => sourceLineToAddress[x.Key], x => x.Any() ? x.First().Key : null);
			for (var sourceLine = 0; sourceLine < codeLines.Length; sourceLine++)
			{
				var codeLine = codeLines[sourceLine];
				var split = codeLine.Command.Split(whitespaces, StringSplitOptions.RemoveEmptyEntries);
				if (split.Length > 0)
				{
					Type instructionType;
					if (!instructionTypes.TryGetValue(split[0], out instructionType))
						throw new InvalidOperationException(string.Format("Instruction '{0}' is not supported. Line {1}: {2}", split[0], sourceLine, codeLine));
					split = split.Skip(1).ToArray();
					var constructors = instructionType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
					if (constructors.Length != 1)	
						throw new InvalidOperationException(string.Format("Instruction '{0}' has no single public constructor.", instructionType));
					var constructor = constructors[0];
					var parameterInfos = constructor.GetParameters();
					var parameters = new object[parameterInfos.Length];
					if (split.Length != parameters.Length)
						throw new InvalidOperationException(string.Format("Invalid parameter count of instruction '{0}'. Expected parameters: '{1}'. Line {2}: {3}", instructionType.Name, string.Join(",", parameterInfos.Select(x => x.Name + " (" + x.ParameterType.Name + ")")), sourceLine, codeLine));
					for (var i = 0; i < parameterInfos.Length; i++)
					{
						if (parameterInfos[i].ParameterType == typeof(int))
						{
							int value;
							if (!constants.TryGetValue(split[i], out value) && !int.TryParse(split[i], out value))
								throw new InvalidOperationException(string.Format("Invalid parameter #{0} ({1}) of instruction '{2}'. Expected parameters: '{3}'. Line {4}: {5}", i, split[i], instructionType.Name, string.Join(",", parameterInfos.Select(x => x.Name + " (" + x.ParameterType.Name + ")")), sourceLine, codeLine));
							parameters[i] = value;
						}
						else if (parameterInfos[i].ParameterType == typeof(uint))
						{
							uint value;
							int labelSourceLine;
							if (labels.TryGetValue(split[i], out labelSourceLine))
								value = sourceLineToAddress[labelSourceLine];
							else if (!uint.TryParse(split[i], out value))
								throw new InvalidOperationException(string.Format("Invalid parameter #{0} ({1}) of instruction '{2}'. Expected parameters: '{3}'. Line {4}: {5}", i, split[i], instructionType.Name, string.Join(",", parameterInfos.Select(x => x.Name + " (" + x.ParameterType.Name + ")")), sourceLine, codeLine));
							parameters[i] = value;
						}
						else
							throw new InvalidOperationException(string.Format("Invalid constructor parameter '{0}' type ({1}) of Instruction '{2}'", parameterInfos[i].Name, parameterInfos[i].ParameterType, instructionType));
					}
					program.Add((Instruction)Activator.CreateInstance(instructionType, parameters));
					sourceLines.Add(sourceLine + 1);
					string addressName;
					addressToName.TryGetValue((uint)addressNames.Count, out addressName);
					addressNames.Add(addressName);
				}
			}
			return new LParseResult
			{
				Program = program.ToArray(),
				SourceLines = sourceLines.ToArray(),
				CodeLines = codeLines,
				AddressNames = addressNames.ToArray(),
			};
		}

		[NotNull]
		private static Dictionary<int, uint> GetSourceLineToAddressMap([NotNull] CodeLine[] codeLines)
		{
			uint currentAddress = 0;
			var sourceLineToAddress = new Dictionary<int, uint>();
			for (var i = 0; i < codeLines.Length; i++)
			{
				sourceLineToAddress[i] = currentAddress;
				if (!string.IsNullOrWhiteSpace(codeLines[i].Command))
					currentAddress++;
			}
			return sourceLineToAddress;
		}

		[NotNull]
		private static CodeLine[] GetCodeLines([NotNull] string[] lines)
		{
			var codeLines = new CodeLine[lines.Length];
			for (var i = 0; i < lines.Length; i++)
				codeLines[i] = new CodeLine(lines[i]);
			return codeLines;
		}

		[NotNull]
		private static Dictionary<string, int> ExtractConstants([NotNull] CodeLine[] codeLines)
		{
			var constants = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
			for (var i = 0; i < codeLines.Length; i++)
			{
				var constantDef = codeLines[i].Constant;
				if (!string.IsNullOrWhiteSpace(constantDef))
				{
					var split = constantDef.Split(whitespaces.Concat(new[] { '=' }).ToArray(), 3, StringSplitOptions.RemoveEmptyEntries);
					if (split.Length != 2)
						throw new InvalidOperationException(string.Format("Invalid constant deinition. Expected '<name>=<value (Int32)>'. Line {0}: {1}", i, codeLines[i]));
					var constantName = split[0];
					var constantValueString = split[1];
					if (constants.ContainsKey(constantName))
						throw new InvalidOperationException(string.Format("Duplicate definition of constant '{0}'. Line {1}: {2}", constantName, i, codeLines[i]));
					int value;
					if (!int.TryParse(constantValueString, out value))
						throw new InvalidOperationException(string.Format("Invalid constant value '{0}'. Expected Int32. Line {1}: {2}", constantValueString, i, codeLines[i]));
					constants.Add(constantName, value);
				}
			}
			return constants;
		}

		[NotNull]
		private static Dictionary<string, int> ExtractLabels([NotNull] CodeLine[] codeLines)
		{
			var labels = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
			for (var i = 0; i < codeLines.Length; i++)
			{
				var labelDef = codeLines[i].Label;
				if (!string.IsNullOrWhiteSpace(labelDef))
				{
					var labelIndex = labelDef.IndexOf(':');
					var labelName = labelDef.Substring(0, labelIndex).Trim(whitespaces);
					if (labels.ContainsKey(labelName))
						throw new InvalidOperationException(string.Format("Duplicate definition of label '{0}'. Line {1}: {2}", labelName, i, codeLines[i]));
					labels.Add(labelName, i);
				}
			}
			return labels;
		}
	}
}