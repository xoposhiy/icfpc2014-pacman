using System;
using System.Collections.Generic;
using System.Linq;

namespace Lib.LMachine
{
	public class LStack<T> where T : class
	{
		private readonly Stack<T> stack;

		public LStack()
		{
			stack = new Stack<T>();
		}

		public bool IsEmpty
		{
			get { return stack.Count == 0; }
		}

		public void Push([NotNull] T value)
		{
			stack.Push(value);
		}

		[NotNull]
		public T Pop()
		{
			if (stack.Count == 0)
				throw new InvalidOperationException("TODO");
			return stack.Pop();
		}

		public override string ToString()
		{
			return string.Join("|", stack.Take(5).Select(i => i.ToString())) + (stack.Count <= 5 ? "|": "|...|");
		}
	}
}