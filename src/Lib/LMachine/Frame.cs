using System;
using System.Linq;

namespace Lib.LMachine
{
	public class Frame
	{
		private static int lastFrameId;

		[NotNull]
		public static Frame Dum([CanBeNull] Frame parent, uint size)
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
			Id = ++lastFrameId;
			IsDum = isDum;
			Parent = parent;
			Values = values;
		}

		public int Id { get; private set; }

		public bool IsDum { get; set; }

		[CanBeNull]
		public Frame Parent { get; private set; }

		[NotNull]
		public LValue[] Values { get; private set; }

		[NotNull]
		public LValue GetValue(uint valueIndex)
		{
			if (valueIndex >= Values.Length)
				throw new InvalidOperationException(string.Format("No value {0} in frame {1}", valueIndex, ToString()));
			var value = Values[valueIndex];
			if (value == null)
				throw new InvalidOperationException("TODO");
			return value;
		}

		public void SetValue(uint valueIndex, [NotNull] LValue value)
		{
			if (valueIndex >= Values.Length)
				throw new InvalidOperationException("TODO");
			Values[valueIndex] = value;
		}

		public void SetValues([NotNull] LValue[] values)
		{
			if (Values.Length != values.Length)
				throw new InvalidOperationException("TODO");
			Values = values;
		}

		public override string ToString()
		{
			return string.Format("Id: {0}, IsDum: {1}, Parent: {2}, Values: {3}", Id, IsDum, Parent == null ? "" : Parent.Id.ToString(), string.Join(", ", Values.Select(Convert.ToString)));
		}
	}
}