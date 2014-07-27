namespace Lib.LMachine
{
	public static class LStackExtensions
	{
		public static int PopValue([NotNull] this LStack<LValue> stack)
		{
			var value = stack.Peek();
			var result = value.GetValue();
			stack.Pop();
			return result;
		}
	}
}