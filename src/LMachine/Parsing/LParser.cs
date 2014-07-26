using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using LMachine.Intructions;

namespace LMachine.Parsing
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
			var lines = source.Split(new[] { "\r\n" }, StringSplitOptions.None);
			var program = new List<Instruction>();
			var sourceLines = new List<int>();
			var labels = ExtractLabels(lines);
			var constants = ExtractConstants(lines);
			var sourceLineToAddress = GetSourceLineToAddressMap(lines);
			for (var sourceLine = 0; sourceLine < lines.Length; sourceLine++)
			{
				var originalLine = lines[sourceLine];
				var line = SkipConstants(SkipLabel(SkipComments(originalLine)));
				var split = line.Split(whitespaces, StringSplitOptions.RemoveEmptyEntries);
				if (split.Length > 0)
				{
					Type instructionType;
					if (!instructionTypes.TryGetValue(split[0], out instructionType))
						throw new InvalidOperationException(string.Format("Instruction '{0}' is not supported. Line {1}: {2}", split[0], sourceLine, originalLine));
					split = split.Skip(1).ToArray();
					var constructors = instructionType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
					if (constructors.Length != 1)
						throw new InvalidOperationException(string.Format("Instruction '{0}' has no single public constructor.", instructionType));
					var constructor = constructors[0];
					var parameterInfos = constructor.GetParameters();
					var parameters = new object[parameterInfos.Length];
					if (split.Length != parameters.Length)
						throw new InvalidOperationException(string.Format("Invalid parameter count of instruction '{0}'. Expected parameters: '{1}'. Line {2}: {3}", instructionType.Name, string.Join(",", parameterInfos.Select(x => x.Name + " (" + x.ParameterType.Name + ")")), sourceLine, originalLine));
					for (var i = 0; i < parameterInfos.Length; i++)
					{
						if (parameterInfos[i].ParameterType == typeof(int))
						{
							int value;
							if (!constants.TryGetValue(split[i], out value) && !int.TryParse(split[i], out value))
								throw new InvalidOperationException(string.Format("Invalid parameter #{0} ({1}) of instruction '{2}'. Expected parameters: '{3}'. Line {4}: {5}", i, split[i], instructionType.Name, string.Join(",", parameterInfos.Select(x => x.Name + " (" + x.ParameterType.Name + ")")), sourceLine, originalLine));
							parameters[i] = value;
						}
						else if (parameterInfos[i].ParameterType == typeof(uint))
						{
							uint value;
							int labelSourceLine;
							if (labels.TryGetValue(split[i], out labelSourceLine))
								value = sourceLineToAddress[labelSourceLine];
							else if (!uint.TryParse(split[i], out value))
								throw new InvalidOperationException(string.Format("Invalid parameter #{0} ({1}) of instruction '{2}'. Expected parameters: '{3}'. Line {4}: {5}", i, split[i], instructionType.Name, string.Join(",", parameterInfos.Select(x => x.Name + " (" + x.ParameterType.Name + ")")), sourceLine, originalLine));
							parameters[i] = value;
						}
						else
							throw new InvalidOperationException(string.Format("Invalid constructor parameter '{0}' type ({1}) of Instruction '{2}'", parameterInfos[i].Name, parameterInfos[i].ParameterType, instructionType));
					}
					program.Add((Instruction)Activator.CreateInstance(instructionType, parameters));
					sourceLines.Add(sourceLine + 1);
				}
			}
			return new LParseResult
			{
				Program = program.ToArray(),
				SourceLines = sourceLines.ToArray(),
			};
		}

		[NotNull]
		private static Dictionary<int, uint> GetSourceLineToAddressMap([NotNull] string[] lines)
		{
			uint currentAddress = 0;
			var sourceLineToAddress = new Dictionary<int, uint>();
			for (var sourceLine = 0; sourceLine < lines.Length; sourceLine++)
			{
				var originalLine = lines[sourceLine];
				var line = SkipConstants(SkipLabel(SkipComments(originalLine)));
				var split = line.Split(whitespaces, StringSplitOptions.RemoveEmptyEntries);
				sourceLineToAddress[sourceLine] = currentAddress;
				if (split.Length > 0)
					currentAddress++;
			}
			return sourceLineToAddress;
		}

		[NotNull]
		private static Dictionary<string, int> ExtractConstants([NotNull] string[] lines)
		{
			var constants = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
			for (var sourceLine = 0; sourceLine < lines.Length; sourceLine++)
			{
				var originalLine = lines[sourceLine];
				var line = SkipLabel(SkipComments(originalLine));
				if (line.IndexOf('=') >= 0)
				{
					var split = line.Split(whitespaces.Concat(new[] { '=' }).ToArray(), 3, StringSplitOptions.RemoveEmptyEntries);
					if (split.Length != 2)
						throw new InvalidOperationException(string.Format("Invalid constant deinition. Expected '<name>=<value (Int32)>'. Line {0}: {1}", sourceLine, originalLine));
					var constantName = split[0];
					var constantValueString = split[1];
					if (constants.ContainsKey(constantName))
						throw new InvalidOperationException(string.Format("Duplicate definition of constant '{0}'. Line {1}: {2}", constantName, sourceLine, originalLine));
					int value;
					if (!int.TryParse(constantValueString, out value))
						throw new InvalidOperationException(string.Format("Invalid constant value '{0}'. Expected Int32. Line {1}: {2}", constantValueString, sourceLine, originalLine));
					constants.Add(constantName, value);
				}
			}
			return constants;
		}

		[NotNull]
		private static Dictionary<string, int> ExtractLabels([NotNull] string[] lines)
		{
			var labels = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
			for (var sourceLine = 0; sourceLine < lines.Length; sourceLine++)
			{
				var originalLine = lines[sourceLine];
				var line = SkipConstants(SkipComments(originalLine));
				var labelIndex = line.IndexOf(':');
				if (labelIndex >= 0)
				{
					var labelName = line.Substring(0, labelIndex).Trim(whitespaces);
					if (labels.ContainsKey(labelName))
						throw new InvalidOperationException(string.Format("Duplicate definition of label '{0}'. Line {1}: {2}", labelName, sourceLine, originalLine));
					labels.Add(labelName, sourceLine);
				}
			}
			return labels;
		}

		[NotNull]
		private static string SkipComments([NotNull] string line)
		{
			var commentIndex = line.IndexOf(';');
			return commentIndex >= 0 ? line.Substring(0, commentIndex) : line;
		}

		[NotNull]
		private static string SkipLabel([NotNull] string line)
		{
			var labelIndex = line.IndexOf(':');
			return labelIndex >= 0 ? line.Substring(labelIndex + 1) : line;
		}

		[NotNull]
		private static string SkipConstants([NotNull] string line)
		{
			var labelIndex = line.IndexOf('=');
			return labelIndex >= 0 ? "" : line;
		}
	}
}