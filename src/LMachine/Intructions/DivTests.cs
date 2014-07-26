using NUnit.Framework;

namespace LMachine.Intructions
{
	public class DivTests
	{
		[Test]
		public void TestNegatives()
		{
			Assert.AreEqual(1, Div.floor_div2(3, 2));
			Assert.AreEqual(1, Div.floor_div2(3, 3));
			Assert.AreEqual(0, Div.floor_div2(3, 4));
			Assert.AreEqual(-2, Div.floor_div2(-3, 2));
			Assert.AreEqual(-1, Div.floor_div2(-3, 3));
			Assert.AreEqual(-1, Div.floor_div2(3, -3));
			Assert.AreEqual(-2, Div.floor_div2(3, -2));
			Assert.AreEqual(-3, Div.floor_div2(3, -1));
			Assert.AreEqual(3, Div.floor_div2(-3, -1));
			Assert.AreEqual(1, Div.floor_div2(-3, -2));
			Assert.AreEqual(1, Div.floor_div2(-3, -3));
			Assert.AreEqual(0, Div.floor_div2(-3, -4));
			Assert.AreEqual(-1, Div.floor_div2(-3, 4));
			Assert.AreEqual(0, Div.floor_div2(3, 4));
		}
	}
}