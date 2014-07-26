namespace Lib.LMachine
{
	public class Closure
	{
		public Closure(uint address, [CanBeNull] Frame frame)
		{
			Address = address;
			Frame = frame;
		}

		public uint Address { get; private set; }

		[CanBeNull]
		public Frame Frame { get; private set; }

		public override string ToString()
		{
			return string.Format("{{{0}: Frame_{1}}}", Address, Frame.Id);
		}
	}
}