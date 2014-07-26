using System.Collections.Generic;
using System.Linq;

namespace Lib
{
	public static class EnumerableExtensions
	{
		public static IEnumerable<T> Add<T>(this IEnumerable<T> items, T appendix)
		{
			return items.Concat(new[] { appendix });
		}
		public static IEnumerable<T> Add<T>(this IEnumerable<T> items, IEnumerable<T> appendix)
		{
			return items.Concat(appendix);
		}
	}
}