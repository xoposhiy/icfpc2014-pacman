using System;

namespace Lib.LMachine.Intructions
{
	public abstract class Instruction
	{
		protected Instruction(InstructionType type)
		{
			Type = type;
		}

		public InstructionType Type { get; private set; }

		public abstract void Execute([NotNull] LMachineState state);

		[NotNull]
		protected static LValue[] PopArgs([NotNull] LMachineState state, uint argsCount)
		{
			var args = new LValue[argsCount];
			for (var i = (int)argsCount - 1; i >= 0; i--)
				args[i] = state.DataStack.Pop();
			return args;
		}

		[NotNull]
		protected static Frame GetParentFrame([CanBeNull] Frame currentFrame, uint frameIndex)
		{
			var fp = currentFrame;
			for (var i = 0; i < frameIndex; i++)
			{
				if (fp == null)
					throw new InvalidOperationException("TODO");
				fp = fp.Parent;
			}
			if (fp == null)
				throw new InvalidOperationException("TODO");
			if (fp.IsDum)
				throw new InvalidOperationException("TODO");
			return fp;
		}
	}
}