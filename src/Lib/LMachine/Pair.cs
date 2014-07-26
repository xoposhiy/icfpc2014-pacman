using System.Text;

namespace Lib.LMachine
{
	public class Pair
	{
		public Pair([NotNull] LValue head, [NotNull] LValue tail)
		{
			Head = head;
			Tail = tail;
		}

		[NotNull]
		public LValue Head { get; private set; }

		[NotNull]
		public LValue Tail { get; private set; }

		public bool IsList()
		{
			return Head.Tag == LTag.Int && (Tail.Tag == LTag.Pair && Tail.GetPair().IsList() || Tail.Tag == LTag.Int && Tail.GetValue() == 0);
		}

		public override string ToString()
		{
			if (IsList())
			{
				var result = new StringBuilder();
				result.Append("[");
				var pair = this;
				while (true)
				{
					result.Append(pair.Head);
					if (pair.Tail.Tag == LTag.Int)
						break;
					result.Append(", ");
					pair = pair.Tail.GetPair();
				}
				result.Append("]");
				return result.ToString();
			}
			return string.Format("({0}, {1})", Head, Tail);
		}
	}
}