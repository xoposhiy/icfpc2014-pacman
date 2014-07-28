# ICFP Contest 2014

## Hack the loop team
  * Alexey Buslavyev
  * Ivan Dashkevich
  * Pavel Egorov
  * Andrew Kostousov
  * Alexey Kungurtsev
  * Ksenia Zhagorina

All from SKB Kontur [http://www.kontur.ru] and [Ural Federal University](http://www.urfu.ru)

Used language: C#

## Technical info

### Macroassembler

Both Ghost CPU and Lambda-Man CPU has macroassembler with constants and labels.

Source code:
  * Abstract macroassembler [ParsrBase.cs](src/Lib/Parsing/ParserBase.cs)
  * GHC macroassembler [GParser.cs](src/Lib/Parsing/GParsing/GParser.cs)
  * GCC macroassembler [LParser.cs](src/Lib/Parsing/LParsing/LParser.cs)

###Ghost AI

Due to small allowed program size we write ghost AIs in macroassembler without any higher level languages:

  * Final ghost AI [chasing2](ghosts/chasing2.mghc)
  * Previous one [chasing](ghosts/chasing.mghc)

Interesting part in host AI is the way to emulate functions with the help of PC-register and variable [return] for storing return address.

Sample from chasing2:
```nasm
mod_dif:
;In: a, b
;Out: a = |a-b|
	jgt mod_dif_gt, a, b
		sub b, a
		mov a, b
		mov pc, end_mod_dif
	mod_dif_gt:
		sub a, b
end_mod_dif:
	add [return], 2
	mov pc, [return]
```

And the call:
```nasm
; a = mod_dif(2, 5)
  mov a, 2
  mov b, 5
  mov [return], pc
  mov pc, mod_dif
  
; execution will continue from this line
```

#### Algorithm components

* **Random walk**. In all cases it has no more clever decisions AI uses random walk. It searches first correct direction starting from given direction. It stores last chosen direction and by default next time starts search direction from different one.
* **Chasing**. Select direction to make ghost a bit closer (in fright mode - further) to lambda-man.

* **Ghost collaoration**. If ghost is near another ghost with smaller index it falls back into random walk. It make higher-index ghosts to spread while smaller-index ghost still chasing the lambda-man.

* **Horizontal and vertical preference during chasing**. During chasing odd ghosts prefere to select horizontal directions, while even ghosts - vertical. It helps them to surround the lambda-man.

* **Fright mode far enough**. If ghost is far enough from the lambda-man in fright mode, it just falls back into random walk.

### Lambda AI

Macroassembler for GCC considered to be too low level and we build lisp-like language on top of it.

Lisp-like language was build as Internal DSL in C#:

```csharp
SExpr main = Def("main", ArgNames("world", "ghosts"), Cons(42, Fun("step")));
SExpr step = Def("step", ArgNames("state", "world"), Cons("state", (int)Direction.Right));

string gccCode = Compile(main, step);
```

Where SExpr — is something who know how to translate it into macroassembler gcc-code.

Neigher tail recursion optimization, nor any other optimizations. Stupid simple.

Everything refered to the lisp-translator lives here [src/Lib/LispLang](src/Lib/LispLang).

Final lambda-man AI is here:
* Lisp-like source code [GreedyLambdaMan_Lisp.cs](src/Lib/AI/GreedyLambdaMan_Lisp.cs)
* The same in macroassembler [GreedyLM.mgcc](gcc-samples/GreedyLM.mgcc)
* The same in pure gcc [GreedyLM.gcc](gcc-samples/GreedyLM.gcc)


### Debugger

Cool visual debugger was made for both macroassemblers. See [src/Lib/Debugger] for details.
