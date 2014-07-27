using System.Linq;
using NUnit.Framework;

namespace Lib.LMachine
{
	[TestFixture]
	public class LStack_Test
	{
		[Test]
		public void TestEnumerable()
		{
			var stack = new LStack<string>();
			stack.Push("lalala");
			stack.Push("bububu");
			Assert.That(stack.ToList(), Is.EqualTo(new[]{"bububu", "lalala"}));
			Assert.That(stack.Pop(), Is.EqualTo("bububu"));
			Assert.That(stack.Pop(), Is.EqualTo("lalala"));
		}
	}
}