using System.Collections.Generic;
using System.Linq;

namespace Lib.LispLang
{
	public class Lisp
	{
		public static string Compile(params SExpr[] statements)
		{
			var e = new Env();
			var macroAsm = string.Join("\r\n", statements.SelectMany(lst => lst.ToCode(e)));
			return macroAsm;
		}

		public static SExpr[] Args(params SExpr[] items)
		{
			return items;
		}

		public static SExpr Def(string name, string[] args, params SExpr[] body)
		{
			return new SExpr(env => DefToCode(name, args, body, env));
		}

		private static IEnumerable<string> DefToCode(string name, string[] args, SExpr body, Env env)
		{
			yield return name + ": " + "; fun(" + string.Join(", ", args) + ")";
			foreach (var line in body.ToCode(env.MakeChild(args)))
			{
				yield return "  " + line;
			}
			yield return "  RTN";
			yield return ";end " + name;
			yield return "";
		}

		public static SExpr Tuple(params SExpr[] items)
		{
			return new SExpr(env => SExpr.TupleToCode(env, items));
		}

		public static SExpr List(params SExpr[] items)
		{
			return new SExpr(env => SExpr.ListToCode(env, items));
		}

		public static string[] ArgNames(params string[] args)
		{
			return args;
		}

		public static SExpr If(SExpr cond, SExpr nonZero, SExpr zero)
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

		private static int lastLabelId;
		public static string NextLabel(string prefix = "label")
		{
			return prefix + "_" + (++lastLabelId);
		}

		public static SExpr Cdr(SExpr list)
		{
			return Cmd("CDR", list);
		}

		public static SExpr Atom(SExpr expr)
		{
			return Cmd("ATOM", expr);
		}

		public static SExpr Cons(SExpr head, SExpr tail)
		{
			return Cmd("CONS", new[] { head, tail });
		}

		public static SExpr Ceq(SExpr head, SExpr tail)
		{
			return Cmd("CEQ", new[] { head, tail });
		}

		public static SExpr Cmd(string name, params SExpr[] args)
		{
			return new SExpr(env => ((SExpr)args).ToCode(env).Concat(new[] { name }));
		}

		//public static SExpr DbgView(SExpr e)
		//{
		//	return Cmd("DbgView", e);
		//}

		public static SExpr Call(string name, params SExpr[] args)
		{
			SExpr a = args;
			return new SExpr(env => a.ToCode(env).Concat(new[] { "LDF " + name, "AP " + args.Length }));
		}
		
		public static SExpr CallFunRef(string name, params SExpr[] args)
		{
			SExpr a = args;
			return new SExpr(env => a.ToCode(env).Add(SExpr.ReferenceToCode(env, name)).Add("AP " + args.Length));
		}

		public static SExpr Car(SExpr list)
		{
			return Cmd("CAR", list);
		}
		public static SExpr Max(SExpr a, SExpr b)
		{
			return If(Cgt(a, b), a, b);
		}

		public static SExpr Min(SExpr a, SExpr b)
		{
			return If(Cgt(b, a), a, b);
		}

		public static SExpr X(SExpr point)
		{
			return Car(point);
		}

		public static SExpr Y(SExpr point)
		{
			return Cdr(point);
		}

		public static SExpr Get(int index, SExpr list)
		{
			while (index != 0)
			{
				list = Cdr(list);
				index--;
			}
			return Car(list);
		}

		public static SExpr GetTuple(int index, int tLen, SExpr tuple)
		{
			while (index != 0)
			{
				tuple = Cdr(tuple);
				index --;
				tLen--;
			}
			if (index < tLen - 1)
				return Car(tuple);
			else
				return tuple;
		}

		public static SExpr GetTuple3(int index, SExpr tuple)
		{
			return GetTuple(index, 3, tuple);
		}

		public static SExpr GetTuple4(int index, SExpr tuple)
		{
			return GetTuple(index, 4, tuple);
		}

		public static SExpr Sub(SExpr a, SExpr b)
		{
			return Cmd("SUB", a, b);
		}

		public static SExpr Mul(SExpr a, SExpr b)
		{
			return Cmd("MUL", a, b);
		}

		public static SExpr Add(SExpr a, SExpr b)
		{
			return Cmd("ADD", a, b);
		}

//		public static SExpr Add(params SExpr[] expr)
//		{
//			if (expr.Length == 0)
//				return 0;
//			else if (expr.Length == 1)
//				return expr[0];
//			else
//				return Add(expr[0], expr.Skip(1).ToArray());
//		}

		public static SExpr And(params SExpr[] expr)
		{
			if (expr.Length == 0)
				return 1;
			else
				return If(expr[0], And(expr.Skip(1).ToArray()), 0);
		}

		public static SExpr Or(params SExpr[] expr)
		{
			if (expr.Length == 0)
				return 0;
			else
				return If(expr[0], 1, Or(expr.Skip(1).ToArray()));
		}


		public static SExpr Not(SExpr expr)
		{
			return If(expr, 0, 1);
		}

		public static SExpr Cgt(SExpr a, SExpr b)
		{
			return Cmd("CGT", a, b);
		}

		public static SExpr Fun(string name)
		{
			return Cmd("LDF " + name);
		}		
		
		public static SExpr Return()
		{
			return Cmd("RTN");
		}
	}
}