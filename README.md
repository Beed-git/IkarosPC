# IkarosPC
Custom 16-bit PC

# Instruction Set:
### Info
N refers to any number corresponding to a register.

Any trailing zeros in instructions such as STOP can be assumed to be ignored.

Any numbers in an instruction (e.g. in MOV X, 0x1234) refers to a literal 16-bit value.

### Instructions
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
#### Jump
- 0x40N0: JUMP X
- 0x4100: JUMP 0x1234
- 0x42N0: JZ X
- 0x4300: JZ 0x1234
- 0x44N0: JC X
- 0x4500: JC 0x1234
- 0x46N0: JS X
- 0x4700: JS 0x1234
- 0x48N0: JNZ X
- 0x4900: JNZ 0x1234
- 0x4AN0: JNC X
- 0x4B00: JNC 0x1234
- 0x4CN0: JNS X
- 0x4D00: JNS 0x1234
