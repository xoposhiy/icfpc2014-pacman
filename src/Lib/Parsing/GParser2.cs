using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Lib.Parsing.GParsing;
using NUnit.Framework;
using Sprache;

namespace Lib.Parsing
{
	public class AssemblerLine : IPositionAware<AssemblerLine>
	{
		public readonly string Label;
		public readonly string InstructionName;
		public readonly string ConstantName;
		public readonly string ConstantValue;
		public readonly string[] Operands;
		public readonly string Comment;
		public int Addr;
		public int Length { get; set; }
		public Position StartPos { get; set; }

		public AssemblerLine(string label, string instructionName, string[] operands, string constantName, string constantValue, string comment)
		{
			Label = label;
			InstructionName = instructionName == null ? null : instructionName.ToUpper();
			Operands = operands;
			Comment = comment;
			ConstantName = constantName;
			ConstantValue = constantValue;
		}

		public AssemblerLine SetPos(Position startPos, int length)
		{
			StartPos = startPos;
			Length = length;
			return this;
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			if (Label != null) sb.Append(Label + ": ");
			if (InstructionName != null) sb.Append(InstructionName + " " + string.Join(", ", Operands));
			if (ConstantName != null) sb.Append(ConstantName + " = " + ConstantValue);
			if (Comment != null) sb.Append(";" + Comment);
			return sb.ToString();
		}
		
		public string ToFullString()
		{
			return ToString() + " ; at " + StartPos + ", Addr " + Addr;
		}
	}


	public class GParser2
	{
		public static Parser<string> Identifier = Parse.AnyChar.Except(Parse.WhiteSpace.XOr(Parse.Chars(",[];:"))).AtLeastOnce().Text();

		public static Parser<T> AsmToken<T>(Parser<T> parser)
		{
			var whitespaces = Parse.WhiteSpace.Except(Parse.LineEnd).Many();
			return from leading in whitespaces
				from item in parser
				from trailing in whitespaces
				select item;
		}

		public static Parser<string> Operand =
			Identifier.XOr(
				from open in Parse.Char('[')
				from id in Identifier
				from close in Parse.Char(']')
				select open + id + close);

		public static Parser<Tuple<string, string[]>> Instruction =
			from instructionName in AsmToken(Identifier)
			from operands in AsmToken(Operand).XDelimitedBy(Parse.Char(',')).Optional()
			select Tuple.Create(instructionName, (operands.GetOrDefault() ?? Enumerable.Empty<string>()).ToArray());

		public static Parser<Tuple<string, string>> ConstantDefinition =
			from name in AsmToken(Identifier)
			from equalitySign in Parse.Char('=')
			from value in AsmToken(Parse.Number)
			select Tuple.Create(name, value);

		public static Parser<string> Comment =
			AsmToken(Parse.EndOfLineComment(";"));

		public static Parser<string> Label =
			from labelName in AsmToken(Identifier)
			from colon in AsmToken(Parse.Char(':'))
			select labelName;

		public static readonly Parser<IEnumerable<AssemblerLine>> Assembler = (
			from space in Parse.WhiteSpace.Except(Parse.LineEnd).Many()
			from label in Label.Optional()
			from constantDef in ConstantDefinition.Optional()
			from instruction in Instruction.Optional()
			from comment in Comment.Optional()
			from lineTerminator in Parse.LineTerminator
			select new AssemblerLine(
				label.GetOrDefault(),
				instruction.IsEmpty ? null : instruction.Get().Item1,
				instruction.IsEmpty ? null : instruction.Get().Item2,
				constantDef.IsEmpty ? null : constantDef.Get().Item1,
				constantDef.IsEmpty ? null : constantDef.Get().Item2,
				comment.GetOrDefault()
				)
			).Positioned().XMany().End();

		public static AssemblerLine[] ToGhc(string macro)
		{
			var lines = Assembler.Parse(macro).ToList();
			var addr = 0;
			var consts = new Dictionary<string, int>();
			foreach (var line in lines)
			{
				line.Addr = addr;
				if (line.InstructionName != null)
					addr++;
				// ReSharper disable AccessToForEachVariableInClosure
				RegisterConstant(line, line.ConstantName, () => int.Parse(line.ConstantValue), consts);
				RegisterConstant(line, line.Label, () => line.Addr, consts);
				// ReSharper restore AccessToForEachVariableInClosure
			}
			return lines
				.Where(line => line.InstructionName != null)
				.Select(line => ReplaceConsts(line, consts))
				.ToArray();
		}

		private static void RegisterConstant(AssemblerLine line, string name, Func<int> value, Dictionary<string, int> consts)
		{
			if (name == null) return;
			if (consts.ContainsKey(name))
				throw new InvalidOperationException("Duplicate definition in [" + line.ToFullString() + "]");
			consts.Add(name, value());
		}

		private static AssemblerLine ReplaceConsts(AssemblerLine line, Dictionary<string, int> labels)
		{
			return new AssemblerLine(
				null,
				line.InstructionName,
				line.Operands.Select(op => ReplaceConst(op, labels)).ToArray(),
				null,
				null,
				null)
			{
				Addr = line.Addr,
				StartPos = line.StartPos,
				Length = line.Length
			};
		}

		private static string ReplaceConst(string operand, Dictionary<string, int> consts)
		{
			if (operand.StartsWith("["))
			{
				if (!operand.EndsWith("]"))
					throw new InvalidOperationException(operand);
				return "[" + ReplaceConst(operand.Substring(1, operand.Length - 2), consts) + "]";
			}
			int res;
			if (!int.TryParse(operand, out res) && !consts.TryGetValue(operand, out res))
				return operand.ToUpper();
			return ((res + 256) % 256).ToString();
		}
	}

	[TestFixture]
	public class GParser2_Tests
	{
		[Test]
		public void Compare()
		{
			var prog = File.ReadAllText(KnownPlace.Ghosts + "chasing2.mghc");
			var oldProg = GParser.Parse(prog).Program.Select(i => i.ToString()).ToArray();
			var newProg = GParser2.ToGhc(prog).Select(i => i.ToString()).ToArray();
			CollectionAssert.AreEqual(oldProg, newProg);
		}

		[Test]
		public void Test()
		{
			var instructions = GParser2.ToGhc(@"
 tmp = 123
  pi = 3
 mov   a,	b;db
label:
;comment
mov a
mov
mov x, b; helloworld  
;comment
 mov   a,	b;db
  ;hw
label2:
	mov [tmp], pi
MOV [62], 255
");
			foreach (var instruction in instructions)
			{
				Console.WriteLine(instruction.ToFullString());
			}
		}
	}
}