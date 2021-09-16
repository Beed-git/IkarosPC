# IkarosPC
IkarosPC is a custom 16-bit computer I am creating in C# which will eventually turn into a fantasy console. 

### Info
Any trailing zeros in instructions after the first 2 digits such as MOV i16, (i16) can be assumed to be ignored.

### Registers
- $PC = Program counter.
- $SP = Stack pointer.
- $FP = Frame pointer. (stack)
- $ACC = Accumulator.
- $FLAG = Flags.
- $A, $B, $C, $D, $E, $X, $Y, $Z = general purpose registers.

### Instruction Set
insn, source, destination
- $ = A register. $X and $Y below mean any general register or $ACC.
- () = Brackets surrounding a value mean the address in memory.
- i16 = An immediate 16-bit value.
- N = Any general register or $ACC.
- F = Bit corresponding to bit in $FLAG .
#### Control
	- 0x0000 - 1 byte - NOP
	- 0x0100 - 1 byte - STOP
	- 0x02N0 - 1 byte - PUSH $X
	- 0x0300 - 2 byte - PUSH i16
	- 0x04N0 - 1 byte - POP $X
	- 0x05N0 - 1 byte - CALL $X
	- 0x0600 - 2 byte - CALL i16
	- 0x0700 - 1 byte - RET
#### Move
	- 0x10NN - 1 byte - MOV $X, $Y
	- 0x11N0 - 2 byte - MOV i16, $Y
	- 0x12NN - 1 byte - MOV ($X), $Y
	- 0x13N0 - 2 byte - MOV (i16), $Y
	- 0x14NN - 1 byte - MOV $X, ($Y)
	- 0x15N0 - 2 byte - MOV i16, ($Y)
	- 0x16N0 - 2 byte - MOV $X, (i16)
	- 0x1700 - 3 byte - MOV i16, (i16)
	- 0x18NN - 1 byte - MOV ($X), ($Y)
	- 0x19N0 - 2 byte - MOV ($X), (i16)
	- 0x1AN0 - 2 byte - MOV (i16), ($X)
	- 0x1B00 - 3 byte - MOV (i16), (i16)
	- 0x1CNN - 2 byte - MOV $X, (i16 + $Y)
	- 0x1DNN - 2 byte - MOV (i16 + $X), $Y
#### General Arithmetic
	- 0x20NN - 1 byte - ADD $X, $Y
	- 0x21N0 - 2 byte - ADD $X, i16
	- 0x22NN - 1 byte - SUB $X, $Y
	- 0x23N0 - 2 byte - SUB $X, i16
	- 0x24N0 - 2 byte - SUB i16, $X
	- 0x25N0 - 1 byte - INC $X
	- 0x26N0 - 1 byte - DEC $X
#### Unsigned Arithmetic
	- 0x30NN - 1 byte - MUL $X, $Y
	- 0x31N0 - 2 byte - MUL $X, i16
	- 0x32NN - 1 byte - DIV $X, $Y
	- 0x33N0 - 2 byte - DIV $X, i16
	- 0x34N0 - 2 byte - DIV i16, $X
	- 0x35NN - 1 byte - MOD $X, $Y
	- 0x36N0 - 2 byte - MOD $X, i16
	- 0x37N0 - 2 byte - MOD i16, $X
#### Signed Arithmetic
	- 0x40NN - 1 byte - MUL $X, $Y
	- 0x41N0 - 2 byte - MUL $X, i16
	- 0x42NN - 1 byte - DIV $X, $Y
	- 0x43N0 - 2 byte - DIV $X, i16
	- 0x44N0 - 2 byte - DIV i16, $X
	- 0x45NN - 1 byte - MOD $X, $Y
	- 0x46N0 - 2 byte - MOD $X, i16
	- 0x47N0 - 2 byte - MOD i16, $X
#### Bit Arithmetic
	- 0x50NN - 1 byte - AND $X, $Y
	- 0x51N0 - 2 byte - AND $X, i16
	- 0x52NN - 1 byte - OR $X, $Y
	- 0x53N0 - 2 byte - OR $X, i16
	- 0x54NN - 1 byte - XOR $X, $Y
	- 0x55N0 - 2 byte - XOR $X, i16
	- 0x56N0 - 1 byte - NOT $X
	- 0x57NN - 1 byte - LLS $X, $Y
	- 0x58N0 - 2 byte - LLS $X, i16
	- 0x59NN - 1 byte - RLS $X, $Y
	- 0x5AN0 - 2 byte - RLS $X, i16
	- 0x5BNN - 1 byte - LLSC $X, $Y
	- 0x5CN0 - 2 byte - LLSC $X, i16
	- 0x5DNN - 1 byte - RLSC $X, $Y
	- 0x5EN0 - 2 byte - RLSC $X, i16
#### Jump
	- 0x70N0 - 1 byte - JMP $X
	- 0x7100 - 2 byte - JMP i16
	- 0x72N0 - 2 byte - JMP i16 + $X
	- 0x73NN - 2 byte - JEQ  $X, $Y, i16
	- 0x74NN - 1 byte - JEQZ  $X, $Y
	- 0x75N0 - 2 byte - JEQZ  $X, i16
	- 0x76NN - 2 byte - JNEQ  $X, $Y, i16
	- 0x77NN - 1 byte - JNEQZ  $X, $Y
	- 0x78N0 - 2 byte - JNEQZ  $X, i16
	- 0x79F0 - 2 byte - JF ?F, i16
	- 0x7AFN - 1 byte - JF ?F, $X
	- 0x7BF0 - 2 byte - JNF ?F, i16
	- 0x7CFN - 1 byte - JNF ?F, $X
#### Jump Signed:
	- 0x80NN - 2 byte - JG  $X, $Y, i16
	- 0x81NN - 1 byte - JGZ  $X, $Y
	- 0x82N0 - 2 byte - JGZ  $X, i16
	- 0x83NN - 2 byte - JL  $X, $Y, i16
	- 0x84NN - 1 byte - JLZ  $X, $Y
	- 0x85N0 - 2 byte - JLZ  $X, i16
	- 0x86NN - 2 byte - JGE  $X, $Y, i16
	- 0x87NN - 1 byte - JGEZ  $X, $Y
	- 0x88N0 - 2 byte - JGEZ  $X, i16
	- 0x89NN - 2 byte - JLE  $X, $Y, i16
	- 0x8ANN - 1 byte - JLEZ  $X, $Y
	- 0x8BN0 - 2 byte - JLEZ  $X, i16
#### Jump Unsigned
	- 0x90NN - 2 byte - JA  $X, $Y, i16
	- 0x91NN - 2 byte - JB  $X, $Y, i16
	- 0x92NN - 2 byte - JAE  $X, $Y, i16
	- 0x93NN - 2 byte - JBE  $X, $Y, i16
