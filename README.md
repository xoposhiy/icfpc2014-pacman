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
  * Abstract macroassembler[ParsrBase.cs](src/Lib/Parsing/ParserBase.cs)
  * GHC macroassembler[GParser.cs](src/Lib/Parsing/GParsing/GParser.cs)
  * GCC macroassembler[LParser.cs](src/Lib/Parsing/LParsing/LParser.cs)

###Ghost AI

Due to small allowed program size we write ghost AIs in macroassembler without any higher level languages:

  * Final ghost AI [chasing2](ghosts/chasing2.mghc)
  * Previous one [chasing](ghosts/chasing.mghc)

Interesting part in host AI is the way to emulate functions with the help of PC-register and variable [return] for storing return address.

Sample from chasing2:
```assembly
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

#### Algorithm description

TODO

### Lambda AI

TODO
