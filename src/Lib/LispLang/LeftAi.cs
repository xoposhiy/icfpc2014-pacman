using Lib.Game;

namespace Lib.LispLang
{
	///ѕример бота, который все врем€ идет направо.
	public class LeftAi : Api
	{
		public static string code = CompileWithLibs(
			Def("main", ArgNames("world", "ghosts"), Cons(42, Fun("step")), Return()),
			Def("step", ArgNames("state", "world"), Cons("state", (int)Direction.Right))
			);

	}
}