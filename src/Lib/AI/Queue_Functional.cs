using System.Collections.Generic;
using Lib.LMachine;
using NUnit.Framework;

namespace Lib.AI
{
	public class Queue_Functional
	{
		private LValue twoStacks;
		private static readonly LValue nil = LValue.FromInt(846534165);

		public Queue_Functional()
		{
			twoStacks = LValue.FromPair(nil, nil);
		}

		public bool IsEmpty()
		{
			return twoStacks.Pair.Tail.Tag != LTag.Pair
			       && twoStacks.Pair.Head.Tag != LTag.Pair;
		}

		public void Enqueue(LValue value)
		{
			twoStacks = LValue.FromPair(
				LValue.FromPair(value, twoStacks.Pair.Head),
				twoStacks.Pair.Tail);
		}

		public LValue Dequeue()
		{
			if (twoStacks.Pair.Tail.Tag != LTag.Pair)
				throwLeftStackToRight();
			if (twoStacks.Pair.Tail.Tag != LTag.Pair)
				return null;
			var result = twoStacks.Pair.Tail .Pair.Head;
			twoStacks = LValue.FromPair(
				twoStacks.Pair.Head,
				twoStacks.Pair.Tail .Pair.Tail);
			return result;
		}

		private void throwLeftStackToRight()
		{
			LValue rightStack = nil;
			while (twoStacks.Pair.Head.Tag == LTag.Pair)
			{
				var curr = twoStacks.Pair.Head.Pair.Head;
				rightStack = LValue.FromPair(curr, rightStack);
				twoStacks = LValue.FromPair(
					twoStacks.Pair.Head.Pair.Tail,
					twoStacks.Pair.Tail);
			}
			twoStacks = LValue.FromPair(nil, rightStack);
		}
	}

	public class TestQueueFunctional
	{
		private Queue_Functional func;
		private Queue<int> coll;

		[Test]
		public void TestQueue()
		{
			func = new Queue_Functional();
			coll = new Queue<int>();

			Push(1);
			Push(2);
			Push(3);
			Pop();
			Pop();
			Pop();
			Pop();
			Push(4);
			Push(5);
			Push(6);
			Pop();
			Pop();
			Push(7);
			Pop();
			Pop();
			Pop();
		}

		private void Push(int val)
		{
			func.Enqueue(val);
			coll.Enqueue(val);
		}

		private void Pop()
		{
			Assert.That(func.IsEmpty(), Is.EqualTo(coll.Count == 0));
			if (coll.Count > 0)
			{
				int val1 = coll.Dequeue();
				int? val2 = func.Dequeue().Value;
				Assert.That(val2.HasValue);
				Assert.That(val1, Is.EqualTo(val2.Value));
			}
		}
	}
}