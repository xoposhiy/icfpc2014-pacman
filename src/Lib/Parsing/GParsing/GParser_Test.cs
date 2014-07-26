using System;
using System.IO;
using System.Linq;
using Lib.GMachine;
using NUnit.Framework;

namespace Lib.Parsing.GParsing
{
	[TestFixture]
	public class GParser_Test
	{
		private static void AssertInstructionEquals([NotNull] GCmd expected, [CanBeNull] GCmd actual)
		{
			Assert.IsNotNull(actual);
			Assert.That(actual.Type, Is.EqualTo(expected.Type));
			Assert.That(actual.GetType(), Is.EqualTo(expected.GetType()));
			foreach (var property in expected.GetType().GetProperties())
				Assert.That(property.GetGetMethod().Invoke(actual, new object[0]), Is.EqualTo(property.GetGetMethod().Invoke(expected, new object[0])), property.Name);
		}

		[Test]
		public void ParseCommandWithIntParameter()
		{
			var parseResult = GParser.Parse("mov 1, [10]");
			AssertInstructionEquals(new Mov(GArg.Const(1), GArg.Data(10)), parseResult.Program.Single());
			Assert.AreEqual(1, parseResult.SourceLines.Single());
		}

		[Test]
		public void ParseCommandWithRegParameter()
		{
			var parseResult = GParser.Parse("add A, [b]");
			AssertInstructionEquals(new Add(GArg.Reg(0), GArg.IndirectReg(1)), parseResult.Program.Single());
			Assert.AreEqual(1, parseResult.SourceLines.Single());
		}

		[Test]
		public void ParseConstants()
		{
			var parseResult = GParser.Parse("myconst = 1 \r\n int myconst");
			AssertInstructionEquals(new Int(1), parseResult.Program.Single());
			Assert.AreEqual(2, parseResult.SourceLines.Single());
		}

		[Test]
		public void ParseLabel()
		{
			var parseResult = GParser.Parse("jeq lalala, a, b \r\n  lalala: \r\n int 0");
			AssertInstructionEquals(new Jeq(1, GArg.Reg(0), GArg.Reg(1)), parseResult.Program[0]);
			AssertInstructionEquals(new Int(0), parseResult.Program[1]);
			Assert.AreEqual(1, parseResult.SourceLines[0]);
			Assert.AreEqual(3, parseResult.SourceLines[1]);
		}

		[Test]
		public void ParseLabelIndirect()
		{
			Assert.Throws<InvalidOperationException>(() => GParser.Parse("mov [lalala], a \r\n  lalala: \r\n int 0"));
		}

		[Test]
		public void ParseForbiddenConstants()
		{
			Assert.Throws<InvalidOperationException>(() => GParser.Parse("a=10"));
		}
	}
}