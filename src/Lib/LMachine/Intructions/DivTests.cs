using NUnit.Framework;

namespace Lib.LMachine.Intructions
{
	public class DivTests
	{
		[Test]
		public void TestNegatives()
		{
			Assert.AreEqual(1, Div.div(3, 2));
			Assert.AreEqual(1, Div.div(3, 3));
			Assert.AreEqual(0, Div.div(3, 4));
			Assert.AreEqual(-2, Div.div(-3, 2));
			Assert.AreEqual(-1, Div.div(-3, 3));
			Assert.AreEqual(-1, Div.div(3, -3));
			Assert.AreEqual(-2, Div.div(3, -2));
			Assert.AreEqual(-3, Div.div(3, -1));
			Assert.AreEqual(3, Div.div(-3, -1));
			Assert.AreEqual(1, Div.div(-3, -2));
			Assert.AreEqual(1, Div.div(-3, -3));
			Assert.AreEqual(0, Div.div(-3, -4));
			Assert.AreEqual(-1, Div.div(-3, 4));
			Assert.AreEqual(0, Div.div(3, 4));
		}
	}
}