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

		public static IEnumerable<string> TupleToCode(Env env, SExpr[] tuple)
		{
			return
				tuple.SelectMany(item => item.ToCode(env))
					.Concat(tuple.Skip(1).Select(item => "CONS"));
		}

		public static IEnumerable<string> ListToCode(Env env, SExpr[] list)
		{
			return TupleToCode(env, list.Concat(new SExpr[]{0}).ToArray());
		}

		public static IEnumerable<string> ReferenceToCode(Env env, string reference)
		{
			var addr = env.GetVariableAddr(reference);
			yield return "LD " + addr.Item1 + " " + addr.Item2 + "  ; " + reference;
		}
	}
}