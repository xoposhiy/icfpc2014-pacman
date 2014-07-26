using System.Collections.Generic;
using System.Linq;

namespace Lib.LMachine.Intructions
{
	public static class InstructionExtensions
	{
		[NotNull]
		public static string ToGcc([NotNull] this IEnumerable<Instruction> program)
		{
			var notDebugInstructions = program.Where(x =>x.Type != InstructionType.DbgView);
			return string.Join("\r\n", notDebugInstructions.Select(ToGcc));
		}

		[NotNull]
		public static string ToGcc([NotNull] this Instruction instruction)
		{
			var args = string.Empty;
			switch (instruction.Type)
			{
				case InstructionType.Ldc:
					args = ((Ldc)instruction).Value.ToString();
					break;
				case InstructionType.Ld:
					var ld = ((Ld)instruction);
					args = ld.FrameIndex + " " + ld.ValueIndex;
					break;
				case InstructionType.Sel:
					var sel = ((Sel)instruction);
					args = sel.TrueAddress + " " + sel.FalseAddress;
					break;
				case InstructionType.Ldf:
					args = ((Ldf)instruction).Address.ToString();
					break;
				case InstructionType.Ap:
					args = ((Ap)instruction).FrameSize.ToString();
					break;
				case InstructionType.Dum:
					args = ((Dum)instruction).FrameSize.ToString();
					break;
				case InstructionType.Rap:
					args = ((Rap)instruction).FrameSize.ToString();
					break;
				case InstructionType.Tap:
					args = ((Tap)instruction).FrameSize.ToString();
					break;
				case InstructionType.Trap:
					args = ((Trap)instruction).FrameSize.ToString();
					break;
				case InstructionType.Tsel:
					var tsel = ((Tsel)instruction);
					args = tsel.TrueAddress + " " + tsel.FalseAddress;
					break;
				case InstructionType.St:
					var st = ((St)instruction);
					args = st.FrameIndex + " " + st.ValueIndex;
					break;
			}
			return string.Format("{0} {1}", instruction.Type.ToString().ToUpper(), args);
		}
	}
}