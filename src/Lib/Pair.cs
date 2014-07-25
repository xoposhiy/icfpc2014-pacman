using JetBrains.Annotations;

namespace Lib
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

		public override string ToString()
		{
			return string.Format("({0}, {1})", Head, Tail);
		}
	}
}