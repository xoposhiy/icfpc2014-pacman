using System;
using System.Collections.Generic;
using System.Linq;

namespace Lib.LispLang
{
	public class SExpr
	{
		private readonly Func<Env, IEnumerable<string>> compile;

		public SExpr(Func<Env, IEnumerable<string>> compile)
		{
			this.compile = compile;
		}

		public SExpr()
			: this(e => Enumerable.Empty<string>())
		{

		}

		public static implicit operator SExpr(int value)
		{
			return new SExpr(env => ConstToCode(value));

		}

		public static implicit operator SExpr(SExpr[] items)
		{
			return new SExpr(env => items.SelectMany(i => i.ToCode(env)));

		}
		public static implicit operator SExpr(string reference)
		{
			return new SExpr(env => ReferenceToCode(env, reference));
		}

		public IEnumerable<string> ToCode(Env env)
		{
			return compile(env);
		}

		private static IEnumerable<string> ConstToCode(int value)
		{
			return new[] { "LDC " + value };
		}

		public static IEnumerable<string> ListToCode(Env env, SExpr[] list)
		{
			return
				list.SelectMany(item => item.ToCode(env))
					.Concat(list.Skip(1).Select(item => "CONS"));
		}

		private static IEnumerable<string> ReferenceToCode(Env env, string reference)
		{
			var index = Array.IndexOf(env.names, reference);
			if (index < 0)
				throw new Exception(reference);
			yield return "LD 0 " + index + "  ; " + reference;
		}
	}
}