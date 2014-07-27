using System.Collections.Generic;
using System.Linq;

namespace Lib.GMachine
{
	public static class GCmdExtensions
	{
		[NotNull]
		public static string ToGhc([NotNull] this IEnumerable<GCmd> program)
		{
			return string.Join("\r\n", program.Select(x => x.ToGhc()));
		}
	}
}