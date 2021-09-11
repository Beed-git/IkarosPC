# IkarosPC
Custom 16-bit PC

# Instruction Set:
### Info
N refers to any number corresponding to a register.

Any trailing zeros in instructions such as STOP can be assumed to be ignored.

Any numbers in an instruction (e.g. in MOV X, 0x1234) refers to a literal 16-bit value.

### Instructions
- 0x0000: NOP
- 0x0100: STOP
- 0x02N0: POP X
- 0x03N0: PUSH X
- 0x04N0: CALL subroutine_name
- 0x0500: RET

- 0x10NN: MOV X, Y
- 0x11NN: MOV (X), Y
- 0x12NN: MOV X, (Y)
- 0x13N0: MOV 0x1234, X
- 0x14N0: MOV 0x1234, (X)

- 0x20NN: ADD X, Y
