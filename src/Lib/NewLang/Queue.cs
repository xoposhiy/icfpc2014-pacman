using System;

namespace Lib.NewLang
{
	public class TypedSE<T>
	{
		public SE SE { get; set; }
	}
	public class WorldState : TypedSE<WorldState> { }
	public class MapState : TypedSE<WorldState> { }
	public class LambdaManState : TypedSE<WorldState> { }

	public static class WorldStateExtensions
	{
		public static MapState Map(this WorldState state)
		{
			return new MapState { SE = SE.Car(state.SE) };
		}
		public static LambdaManState LambdaManState(this WorldState state)
		{
			return new LambdaManState { SE = SE.Car(SE.Cdr(state.SE)) };
		}
		// ...
	}

	public static class Queue
	{
		public static SE Make(SE inStack, SE outStack)
		{
			return SE.Cons(inStack, outStack);
		}

		public static SE Enqueue(this SE queue, SE value)
		{
			return Make(queue.InStack().PushStack(value), OutStack(queue));
		}

		public static SE Dequeue(this SE queue)
		{
			return SE.Fun(() =>
				SE.If(
					queue.OutStack().IsEmptyStack(),
					queue.TransferInToOut().Dequeue(),
					Make(queue.InStack(), queue.OutStack().PopStack()))
				, queue);
		}

		public static SE PeekQueue(this SE queue)
		{
			return SE.Fun(() =>
				SE.If(queue.OutStack().IsEmptyStack(),
					queue.TransferInToOut().OutStack().PeekStack(),
					queue.OutStack().PeekStack())
				, queue);
		}

		public static SE InStack(this SE queue)
		{
			return SE.Car(queue);
		}

		public static SE OutStack(this SE queue)
		{
			return SE.Car(SE.Cdr(queue));
		}

		public static SE TransferInToOut(this SE queue)
		{
			return SE.Fun(() =>
				SE.If(
					queue.InStack().IsEmptyStack(),
					queue,
					Make(
						queue.InStack().PopStack(),
						queue.OutStack().PushStack(queue.InStack().PeekStack()))
						.TransferInToOut()
					),
				queue);
		}
	}
}