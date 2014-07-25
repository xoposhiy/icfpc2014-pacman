using System;
using NUnit.Framework;

namespace Lib
{
	public class WorldTests
	{
		[Test]
		public void ToLValue()
		{
			var s = new World(MapUtils.LoadFromKnownLocation("small.txt")).ToLValue().ToString();
			Console.WriteLine(s);
			Assert.AreEqual("(((0, (0, (0, (0, (0, (0, (0, (0, (0, (0, (0, 0))))))))))), ((0, (5, (2, (4, (2, (2, (2, (3, (6, (2, (0, 0))))))))))), ((0, (0, (0, (0, (0, (0, (0, (0, (0, (0, (0, 0))))))))))), 0))), ((0, ((1, 1), (0, (3, 0)))), (((0, ((8, 1), 0)), 0), 0)))", s);
		}
	}
}