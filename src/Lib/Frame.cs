using JetBrains.Annotations;

namespace Lib
{
	public class Frame
	{
		[NotNull]
		public static Frame Dum([CanBeNull] Frame parent, int size)
		{
			return new Frame(true, parent, new LValue[size]);
		}

		[NotNull]
		public static Frame ForFunctionCall([CanBeNull] Frame parent, [NotNull] LValue[] values)
		{
			return new Frame(false, parent, values);
		}

		private Frame(bool isDum, [CanBeNull] Frame parent, [NotNull] LValue[] values)
		{
			IsDum = isDum;
			Parent = parent;
			Values = values;
		}

		public bool IsDum { get; private set; }

		[CanBeNull]
		public Frame Parent { get; private set; }

		[NotNull]
		public LValue[] Values { get; private set; }
	}
}