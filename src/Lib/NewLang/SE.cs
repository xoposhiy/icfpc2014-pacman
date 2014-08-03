using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Lib.NewLang
{
	public class SE
	{
		private readonly Action<CompilationWriter> compile;

		public SE(Action<CompilationWriter> compile)
		{
			this.compile = compile;
		}

		public static SE Val(int v)
		{
			return new SE(w => w.Write("LdC {0}", v));
		}

		public static SE Car(SE list)
		{
			return Cmd("Car", list);
		}
		
		public static SE Cdr(SE list)
		{
			return Cmd("Cdr", list);
		}

		public static SE Cons(SE head, SE tail)
		{
			return Cmd("Cons", head, tail);
		}

		public static SE IsInt(SE e)
		{
			return Cmd("Atom", e);
		}

		public static SE If(SE condiiton, SE trueExpr, SE falseExpr)
		{
			return new SE(w =>
			{
				var trueLabel = "if_true_" + ++lastLabelIndex;
				var falseLabel = "if_false_" + ++lastLabelIndex;
				w.Write("SEL {0} {1}", trueLabel, falseLabel);
				WriteIfCase(trueExpr, w, trueLabel);
				WriteIfCase(falseExpr, w, falseLabel);
			});
		}

		private static void WriteIfCase(SE expr, CompilationWriter w, string label)
		{
			w.HelperWriter(caseWriter =>
			{
				caseWriter.Write("{0}:", label);
				caseWriter.Indented(expr.Compile);
				caseWriter.Write("JOIN");
			});
		}

		public static int lastLabelIndex = -1;

		public static SE ArgRef(int index)
		{
			return Cmd("LD 0 " + index);
		}

		public static implicit operator SE(int value)
		{
			return Val(value);
		}

		public static implicit operator SE(SE[] list)
		{
			throw new InvalidOperationException("TODO");
		}

		private static SE Cmd(string instruction, params SE[] args)
		{
			return new SE(
				w =>
				{
					foreach (var arg in args)
						arg.Compile(w);
					w.Write(instruction);
				});
		}

		private static SE Call(string fun, params SE[] args)
		{
			return new SE(
				w =>
				{
					foreach (var arg in args)
						arg.Compile(w);
					w.Write("LDF {0}", fun);
					w.Write("AP {0}", args.Length);
				});
		}

		public void Compile(CompilationWriter w)
		{
			compile(w);
		}

		public static SE Fun(Func<SE> func, params SE[] args)
		{
			var function = new StackTrace().GetFrame(1).GetMethod();
			if (function.GetParameters().Length != args.Length)
				throw new InvalidOperationException("Wrong arguments number!");
			var name = GetFunctionName(function);
			if (FunctionToDefinition != name)
				return Call(name, args);
			else
			{
				FunctionToDefinition = null;
				return func();
			}
		}

		private static string GetFunctionName(MethodBase function)
		{
			return function.Name;
		}

		private static string FunctionToDefinition;

		public static string Compile(params Type[] types)
		{
			var w = new CompilationWriter();
			foreach (var type in types)
			{
				var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
				foreach (var m in methods)
				{
					var name = GetFunctionName(m);
					FunctionToDefinition = name;
					// ReSharper disable once CoVariantArrayConversion
					var se = (SE)m.Invoke(null, m.GetParameters().Select((info, i) => ArgRef(i)).ToArray());
					if (FunctionToDefinition == null)
					{
						w.Write("{0}:", name);
						w.Indented(se.Compile);
						w.Write("RTN ; End of {0}", name);
						w.FlushFunctions();
					}
				}
			}
			return w.ToString();
		}
	}
}