namespace Lib.LMachine
{
	public class ControlStackItem
	{
		[NotNull]
		public static ControlStackItem ForJoin(uint address)
		{
			return new ControlStackItem
			{
				Tag = CTag.Join,
				Address = address,
			};
		}

		[NotNull]
		public static ControlStackItem ForRet(uint address)
		{
			return new ControlStackItem
			{
				Tag = CTag.Ret,
				Address = address,
			};
		}

		[NotNull]
		public static ControlStackItem ForFrame([CanBeNull] Frame frame)
		{
			return new ControlStackItem
			{
				Tag = CTag.Frame,
				Frame = frame,
			};
		}

		private ControlStackItem()
		{
		}

		public CTag Tag { get; private set; }

		public uint Address { get; private set; }

		[CanBeNull]
		public Frame Frame { get; private set; }
	}
}