namespace Lib.LMachine
{
	public class LMachineState
	{
		public LMachineState()
		{
			DataStack = new LStack<LValue>();
			ControlStack = new LStack<ControlStackItem>();
		}

		public uint CurrentAddress { get; set; }

		[CanBeNull]
		public Frame CurrentFrame { get; set; }

		public bool Stopped { get; set; }

		[NotNull]
		public LStack<LValue> DataStack { get; private set; }

		[NotNull]
		public LStack<ControlStackItem> ControlStack { get; private set; }
	}
}