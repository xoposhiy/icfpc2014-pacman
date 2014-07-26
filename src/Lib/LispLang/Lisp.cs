using System.Collections.Generic;
using System.Linq;

namespace Lib.LispLang
{
	public class Lisp
	{
		public string Compile(params SExpr[] statements)
		{
			var e = new Env();
			var macroAsm = string.Join("\r\n", statements.SelectMany(lst => lst.ToCode(e)));
			return macroAsm;
		}
		public SExpr[] Args(params SExpr[] items)
		{
			return items;
		}

		public SExpr Def(string name, string[] args, SExpr body)
		{
			return new SExpr(env => DefToCode(name, args, body));
		}

		private static IEnumerable<string> DefToCode(string name, string[] args, SExpr body)
		{
			yield return name + ": " + "; fun(" + string.Join(", ", args) + ")";
			foreach (var line in body.ToCode(new Env(args)))
			{
				yield return "  " + line;
			}
			yield return "  RTN";
			yield return ";end " + name;
			yield return "";
		}

		public SExpr Tuple(params SExpr[] items)
		{
			return new SExpr(env => SExpr.TupleToCode(env, items));
		}

		public SExpr List(params SExpr[] items)
		{
			return new SExpr(env => SExpr.ListToCode(env, items));
		}

		public string[] ArgNames(params string[] args)
		{
			return args;
		}

		public SExpr If(SExpr cond, SExpr nonZero, SExpr zero)
		{
			var zeroLabel = NextLabel("zero");
			var nonZeroLabel = NextLabel("nonzero");
			var endLabel = NextLabel("endif");
			return new SExpr(
				env =>
					cond.ToCode(env)
						.Add("SEL " + nonZeroLabel + " " + zeroLabel)
						.Add("  LDC 0")
						.Add("  TSEL " + endLabel + " " + endLabel)
						.Add("  " + nonZeroLabel + ":")
						.Add(nonZero.ToCode(env).Select(l => "    " + l))
						.Add("    JOIN")
						.Add("  " + zeroLabel + ":")
						.Add(zero.ToCode(env).Select(l => "    " + l))
						.Add("    JOIN")
						.Add("  " + endLabel + ":")
				);

		}

		private int lastLabelId ;
		public string NextLabel(string prefix = "label")
		{
			return prefix + "_" + (++lastLabelId);
		}
		public SExpr Cdr(SExpr list)
		{
			return Cmd("CDR", list);
		}

		public SExpr Cons(SExpr head, SExpr tail)
		{
			return Cmd("CONS", new[] { head, tail });
		}

		public static SExpr Cmd(string name, SExpr args)
		{
			return new SExpr(env => args.ToCode(env).Concat(new[] { name }));
		}

		public static SExpr Call(string name, params SExpr[] args)
		{
			SExpr a = args;
			return new SExpr(env => a.ToCode(env).Concat(new[] { "LDF " + name, "AP " + args.Length }));
		}

		public SExpr Car(SExpr list)
		{
			return Cmd("CAR", list);
		}
		public SExpr Sub(SExpr a, SExpr b)
		{
			return Cmd("SUB", new[] { a, b });
		}
	}
}