namespace Lib.NewLang
{
	public static class Stack
	{
		public static SE PushStack(this SE stack, SE value)
		{
			return SE.Cons(value, stack);
		}

		public static SE IsEmptyStack(this SE stack)
		{
			return SE.IsInt(stack);
		}

		public static SE PopStack(this SE stack)
		{
			return SE.Cdr(stack);
		}

		public static SE PeekStack(this SE stack)
		{
			return SE.Car(stack);
		}

		public static SE CreateEmptyStack()
		{
			return SE.Val(0);
		}
	}
}