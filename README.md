# IkarosPC
Custom 16-bit PC

# Instruction Set:
### Info
N refers to any number corresponding to a register.

Any trailing zeros in instructions such as STOP can be assumed to be ignored.

Any numbers in an instruction (e.g. in MOV X, 0x1234) refers to a literal 16-bit value.

### Old Instructions
#### Control
- 0x0000: NOP
- 0x0100: STOP
- 0x02N0: POP X
- 0x03N0: PUSH X
- 0x04N0: CALL subroutine_name
- 0x0500: RET
#### Move
- 0x10NN: MOV X, Y
- 0x11NN: MOV (X), Y
- 0x12NN: MOV X, (Y)
- 0x13N0: MOV 0x1234, X
- 0x14N0: MOV 0x1234, (X)
- 0x1500: MOV 0x1234, (0x4321)
- 0x16NN: MOV (0x1234 + X), Y
- 0x17NN: MOV X, (0x1234 + Y)
#### Arithmatic
- 0x20NN: ADD X, Y
- 0x21N0: ADD X, 0x1234
- 0x22NN: SUB X, Y
- 0x23N0: SUB X, 0x1234
- 0x24N0: SUB 0x1234, X
- 0x25NN: MUL X, Y
- 0x26N0: MUL X, 0x1234
- 0x27N0: DIV X, Y
- 0x28N0: DIV X, 0x1234
- 0x29N0: DIV 0x1234, X
- 0x2ANN: MOD X, Y
- 0x2BN0: MOD X, 0x1234
- 0x2CN0: MOD 0x1234, X
- 0x30NN: AND X, Y
- 0x31N0: AND X, 0x1234
- 0x32NN: OR X, Y
- 0x33N0: OR X, 0x1234
- 0x34NN: XOR X, Y
- 0x35N0: XOR X, 0x1234
#### Bit
S is any number between 0 & 7 inclusive.
- 0x40NS: SLL X, (0-7)
- 0x41NS: SLR X, (0-7)
#### Jump
- 0x50N0: JUMP X
- 0x5100: JUMP 0x1234
- 0x52N0: JZ X
- 0x5300: JZ 0x1234
- 0x54N0: JC X
- 0x5500: JC 0x1234
- 0x56N0: JS X
- 0x5700: JS 0x1234
- 0x58N0: JNZ X
- 0x5900: JNZ 0x1234
- 0x5AN0: JNC X
- 0x5B00: JNC 0x1234
- 0x5CN0: JNS X
- 0x5D00: JNS 0x1234

### New Instructions
insn, source, destination
$ represents register.
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
	- 0x16NN - 2 byte - MOV $X, (i16)
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
	- 0x34N0 - 2 byte - SUB i16, $X
	- 0x35N0 - 1 byte - INC $X
	- 0x36N0 - 1 byte - DEC $X
#### Unsigned Arithmetic
	- 0x40NN - 1 byte - MUL $X, $Y
	- 0x41N0 - 2 byte - MUL $X, i16
	- 0x42NN - 1 byte - DIV $X, $Y
	- 0x43N0 - 2 byte - DIV $X, i16
	- 0x44N0 - 2 byte - DIV i16, $X
	- 0x45NN - 1 byte - MOD $X, $Y
	- 0x46N0 - 2 byte - MOD $X, i16
	- 0x47N0 - 2 byte - MOD i16, $X
#### Signed Arithmetic
	- 0x50NN - 1 byte - MUL $X, $Y
	- 0x51N0 - 2 byte - MUL $X, i16
	- 0x52NN - 1 byte - DIV $X, $Y
	- 0x53N0 - 2 byte - DIV $X, i16
	- 0x54N0 - 2 byte - DIV i16, $X
	- 0x55NN - 1 byte - MOD $X, $Y
	- 0x56N0 - 2 byte - MOD $X, i16
	- 0x50N0 - 2 byte - MOD i16, $X
#### Bit Arithemtic
	- 0x60NN - 1 byte - AND $X, $Y
	- 0x61NN - 2 byte - AND $X, i16
	- 0x62NN - 1 byte - OR $X, $Y
	- 0x63NN - 2 byte - OR $X, i16
	- 0x64NN - 1 byte - XOR $X, $Y
	- 0x65NN - 2 byte - XOR $X, i16
	- 0x66NN - 1 byte - LLS $X, $Y
	- 0x67NN - 2 byte - LLS $X, i16
	- 0x68NN - 1 byte - RLS $X, $Y
	- 0x69NN - 2 byte - RLS $X, i16
	- 0x6ANN - 1 byte - LLSC $X, $Y
	- 0x6BNN - 2 byte - LLSC $X, i16
	- 0x6CNN - 1 byte - RLSC $X, $Y
	- 0x6DNN - 2 byte - RLSC $X, i16
#### Jump
	- 0x8010 - 2 byte - JMP ($X)
	- 0x8100 - 2 byte - JMP (i16)
	- 0x82NN 0xN000 - 2 byte -  JEQ $X, $Y, ($X)
	- 0x82NN 0xN000 - 2 byte - JEQ $X, $Y, (i16)
	- 0x83NN 0xN000 - 2 byte - JNEQ $X, $Y, ($X)
	- 0x84NN - 2 byte - JNEQ $X, $Y, (i16)
	- 0x85N0 - 1 byte - JEZ $X
	- 0x8600 - 2 byte - JEZ (i16)
	- 0x87NF - 1 byte - JF ($X)
	- 0x88NF - 1 byte - JF (i16)
	- 0x89NF - 1 byte - JNF ($X)
	- 0x8ANF - 2 byte - JNF (i16)
