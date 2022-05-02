using System;

namespace IkarosPC;

public partial class CPU
{
    void HandleOpcode(ushort opcode)
    {
        byte nibble = (byte)((opcode & 0xFF00) >> 8);

        switch (nibble)
        {
            //
            // Control
            //

            // No operation occurs.
            // 1 byte.
            // e.g. NOP
            case 0x00:
                break;
            // Stop execution from occuring.
            // 1 byte
            // e.g. STOP
            case 0x01:
                {
                    Stopped = true;
                }
                break;
            // Pushs the value of a register on the stack.
            // 1 byte.
            // e.g. PUSH $X
            case 0x02:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);

                    Memory.Push(Registers[rX]);
                }
                break;
            // Pushs an immediate value on the stack.
            // 2 bytes.
            // e.g. PUSH i16
            case 0x03:
                {
                    var immediate = GetImmediate16();

                    Memory.Push(immediate);
                }
                break;
            // Pops the value of a register off the stack.
            // 1 byte.
            // e.g. POP $X
            case 0x04:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);

                    Registers[rX] = Memory.Pop();
                }
                break;
            // Calls a subroutine from a register
            // 2 bytes.
            // e.g. CALL $X:
            case 0x05:
                {
                    Memory.PushStackFrame();

                    byte rX = (byte)((opcode & 0x00F0) >> 4);
                    Registers.PC = Registers[rX];
                }
                break;
            // Calls a subroutine from an immediate value. 
            // 2 bytes.
            // e.g. CALL i16:
            case 0x06:
                {
                    var immediate = GetImmediate16();

                    // Save state to stack
                    Memory.PushStackFrame();

                    Registers.PC = immediate;
                }
                break;
            // Returns from a subroutine.
            // 1 byte.
            // e.g. RET
            case 0x07:
                {
                    Memory.PopStackFrame();
                }
                break;
            // Moves the value of a general register into a special register.
            // 1 byte.
            // e.g. MOVGS $X, $Y
            case 0x08:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);
                    byte rY = (byte)(opcode & 0x000F);

                    Registers.Special[rY] = Registers[rX];
                }
                break;
            // Moves the value of a special register into a general register.
            // 1 byte.
            // e.g. MOVSG $X, $Y
            case 0x09:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);
                    byte rY = (byte)(opcode & 0x000F);

                    Registers[rY] = Registers.Special[rX];
                }
                break;
            //
            // Move
            //

            // Moves value from first register to second register.
            // 1 byte.
            // e.g. MOV $X, $Y
            case 0x10:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);
                    byte rY = (byte)(opcode & 0x000F);

                    Registers[rY] = Registers[rX];
                }
                break;
            // Moves an immediate value into a specified register.
            // 2 bytes.
            // e.g. MOV i16, $X
            case 0x11:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);

                    var immediate = GetImmediate16();
                    Registers[rX] = immediate;
                }
                break;
            // Moves a value in memory specified by a register into another register.
            // 1 byte.
            // e.g. MOV ($X), $Y
            case 0x12:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);
                    byte rY = (byte)(opcode & 0x000F);

                    Registers[rY] = Memory[Registers[rX]];
                }
                break;
            // Moves a value in memory specified by an immediate value into a register.
            // 2 bytes.
            // e.g. MOV (i16), $X
            case 0x13:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);

                    var immediate = GetImmediate16();
                    Registers[rX] = Memory[immediate];
                }
                break;
            // Moves a value from a register into memory specified by another register.
            // 1 byte.
            // e.g. MOV $X, ($Y)
            case 0x14:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);
                    byte rY = (byte)(opcode & 0x000F);

                    Memory[Registers[rY]] = Registers[rX];
                }
                break;
            // Stores an immediate value in memory specified by a register.
            // 2 bytes.
            // e.g. MOV i16, ($X)
            case 0x15:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);

                    var immediate = GetImmediate16();
                    Memory[Registers[rX]] = immediate;
                }
                break;
            // Moves a value from a register into memory specified by an immediate.
            // 2 bytes.
            // e.g. MOV $X, (i16)
            case 0x16:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);

                    var immediate = GetImmediate16();
                    Memory[immediate] = Registers[rX];
                }
                break;
            // Moves an immediate value into memory specified by an immediate value.
            // 3 bytes.
            // e.g. MOV i16, (i16)
            case 0x17:
                {
                    var immediate = GetImmediate16();
                    var address = GetImmediate16();

                    Memory[address] = immediate;
                }
                break;
            // Moves a value from memory specified by a register into another memory location specified by another register.
            // 1 byte.
            // e.g. MOV ($X), ($Y)
            case 0x18:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);
                    byte rY = (byte)(opcode & 0x000F);

                    Memory[Registers[rY]] = Memory[Registers[rX]];
                }
                break;
            // Moves a value in memory specified by a register into a memory location specified by an immediate value.
            // 2 bytes.
            // e.g. MOV ($X), (i16)
            case 0x19:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);

                    var immediate = GetImmediate16();
                    Memory[immediate] = Memory[Registers[rX]];
                }
                break;
            // Moves a value in memory specified by an immediate value into a memory location specified by a register.
            // 2 bytes.
            // e.g. MOV (i16), ($X)
            case 0x1A:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);

                    var immediate = GetImmediate16();
                    Memory[Registers[rX]] = Memory[immediate];
                }
                break;
            // Moves a value in memory specified by an immediate value into a memory location specified by another immediate value.
            // 3 bytes.
            // e.g. MOV (i16), (i16)
            case 0x1B:
                {
                    var immediate = GetImmediate16();
                    var address = GetImmediate16();

                    Memory[address] = Memory[immediate];
                }
                break;
            // Store a value in memory at an immediate value + a signed offset.
            // 2 bytes.
            // e.g. MOV X, (0x1234 + Y)
            case 0x1C:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);
                    byte rY = (byte)(opcode & 0x000F);

                    var offset = (short)GetImmediate16();

                    var address = (ushort)(offset + Registers[rY]);

                    Memory[address] = Registers[rX];
                }
                break;
            // Stores a value in a register at an immediate value + a signed offset.
            // 2 bytes.
            // e.g. MOV (0x1234 + $X), $Y
            case 0x1D:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);
                    byte rY = (byte)(opcode & 0x000F);

                    var offset = (short)GetImmediate16();

                    var address = (ushort)(offset + Registers[rX]);

                    Registers[rY] = Memory[address];
                }
                break;

            //
            // General Arithmetic
            //

            // Adds the values of the first and second register and puts the result in $ACC.
            // SZCN: 0 Z C 0
            // 1 byte.
            // e.g. ADD $X, $Y
            case 0x20:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);
                    byte rY = (byte)(opcode & 0x000F);

                    var result = Registers[rX] + Registers[rY];

                    Registers.Zero = ((ushort)result) == 0;
                    Registers.Carry = result > ushort.MaxValue;
                    Registers.Negative = false;
                    Registers.Signed = false;

                    Registers.Accumulator = (ushort)result;
                }
                break;
            // Adds the value of the first register and an immediate value and stores the result in $ACC.
            // SZCN: 0 Z C 0
            // 2 bytes.
            // e.g. ADD i16, $X
            case 0x21:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);

                    var immediate = GetImmediate16();

                    var result = Registers[rX] + immediate;

                    Registers.Zero = ((ushort)result) == 0;
                    Registers.Carry = result > ushort.MaxValue;
                    Registers.Negative = false;
                    Registers.Signed = false;

                    Registers.Accumulator = (ushort)(Registers[rX] + immediate);
                }
                break;
            // Subtracts the value of the first register from the second and stores the result in $ACC.
            // SZCN: 0 Z C 1
            // 1 byte.
            // e.g. SUB $X, $Y
            case 0x22:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);
                    byte rY = (byte)(opcode & 0x000F);

                    var result = Registers[rX] - Registers[rY];

                    Registers.Zero = ((ushort)result) == 0;
                    Registers.Carry = result < 0;
                    Registers.Negative = true;
                    Registers.Signed = false;

                    Registers.Accumulator = (ushort)result;
                }
                break;
            // Subtracts the value of the register from the immediate value and stores the result in $ACC.
            // ZCN: 0 Z C 1
            // 2 bytes.
            // e.g. SUB $X, i16
            case 0x23:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);

                    var immediate = GetImmediate16();

                    var result = Registers[rX] - immediate;

                    Registers.Zero = ((ushort)result) == 0;
                    Registers.Carry = result < 0;
                    Registers.Negative = true;
                    Registers.Signed = false;

                    Registers.Accumulator = (ushort)result;
                }
                break;
            // Subtracts the immediate value from the value of the register and stores the result in $ACC.
            // SZCN: 0 Z C 1
            // 2 bytes.
            // e.g. SUB i16, $X
            case 0x24:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);

                    var immediate = GetImmediate16();

                    var result = immediate - Registers[rX];

                    Registers.Zero = ((ushort)result) == 0;
                    Registers.Carry = result < 0;
                    Registers.Negative = true;
                    Registers.Signed = false;

                    Registers.Accumulator = (ushort)result;
                }
                break;
            // Increments the value of the register and stores the result back in the register.
            // SZCN: 0 Z C 0
            // 1 byte.
            // e.g. INC $X
            case 0x25:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);

                    var result = Registers[rX] + 1;

                    Registers.Zero = ((ushort)result) == 0;
                    Registers.Carry = result > ushort.MaxValue;
                    Registers.Negative = false;
                    Registers.Signed = false;

                    Registers[rX] = (ushort)result;
                }
                break;
            // Decrements the value of the register and stores the result back in the register.
            // SZCN: 0 Z C 1
            // 1 byte.
            // e.g. DEC $X
            case 0x26:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);

                    var result = Registers[rX] - 1;

                    Registers.Zero = ((ushort)result) == 0;
                    Registers.Carry = result < 0;
                    Registers.Negative = true;
                    Registers.Signed = false;

                    Registers[rX] = (ushort)result;
                }
                break;

            //
            // Unsigned Arithmetic.
            //

            // Multiplies the values of the two registers and stores the result in $ACC.
            // SZCN: 0 Z C 0
            // 1 byte.
            // e.g. MUL $X, $Y
            case 0x30:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);
                    byte rY = (byte)(opcode & 0x000F);

                    var result = Registers[rX] * Registers[rY];

                    Registers.Zero = ((ushort)result) == 0;
                    Registers.Carry = result > ushort.MaxValue;
                    Registers.Negative = false;
                    Registers.Signed = false;

                    Registers.Accumulator = (ushort)result;
                }
                break;

            //
            // Signed Arithmetic
            //

            //
            // Bit Arithmetic
            //

            // Logical ands two registers and stores the result in $ACC.
            // SZCN: 0 Z 0 0
            // 1 byte.
            // e.g. AND $X, $Y
            case 0x50:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);
                    byte rY = (byte)(opcode & 0x000F);

                    var result = Registers[rX] & Registers[rY];

                    Registers.Zero = ((ushort)result) == 0;
                    Registers.Carry = false;
                    Registers.Negative = false;
                    Registers.Signed = false;

                    Registers.Accumulator = (ushort)result;
                }
                break;
            // Logical ands a register and an immediate and stores the result in $ACC.
            // SZCN: 0 Z 0 0
            // 2 bytes.
            // e.g. AND $X, i16
            case 0x51:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);

                    var result = Registers[rX] & GetImmediate16();

                    Registers.Zero = ((ushort)result) == 0;
                    Registers.Carry = false;
                    Registers.Negative = false;
                    Registers.Signed = false;

                    Registers.Accumulator = (ushort)result;
                }
                break;
            // Logical ors two registers and stores the result in $ACC.
            // SZCN: 0 Z 0 0
            // 1 byte.
            // e.g. OR $X, $Y
            case 0x52:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);
                    byte rY = (byte)(opcode & 0x000F);

                    var result = Registers[rX] | Registers[rY];

                    Registers.Zero = ((ushort)result) == 0;
                    Registers.Carry = false;
                    Registers.Negative = false;
                    Registers.Signed = false;

                    Registers.Accumulator = (ushort)result;
                }
                break;
            // Logical ors a register and an immediate and stores the result in $ACC.
            // SZCN: 0 Z 0 0
            // 2 bytes.
            // e.g. OR $X, i16
            case 0x53:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);

                    var result = Registers[rX] | GetImmediate16();

                    Registers.Zero = ((ushort)result) == 0;
                    Registers.Carry = false;
                    Registers.Negative = false;
                    Registers.Signed = false;

                    Registers.Accumulator = (ushort)result;
                }
                break;
            // Logical xors two registers and stores the result in $ACC.
            // SZCN: 0 Z 0 0
            // 1 byte.
            // e.g. XOR $X, $Y
            case 0x54:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);
                    byte rY = (byte)(opcode & 0x000F);

                    var result = Registers[rX] ^ Registers[rY];

                    Registers.Zero = ((ushort)result) == 0;
                    Registers.Carry = false;
                    Registers.Negative = false;
                    Registers.Signed = false;

                    Registers.Accumulator = (ushort)result;
                }
                break;
            // Logical xors a register and an immediate and stores the result in $ACC.
            // SZCN: 0 Z 0 0
            // 2 bytes.
            // e.g. XOR $X, i16
            case 0x55:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);

                    var result = Registers[rX] ^ GetImmediate16();

                    Registers.Zero = ((ushort)result) == 0;
                    Registers.Carry = false;
                    Registers.Negative = false;
                    Registers.Signed = false;

                    Registers.Accumulator = (ushort)result;
                }
                break;
            // Logical nots a register and stores the result in $ACC.
            // SZCN: 0 Z 0 0
            // 2 bytes.
            // e.g. NOT $X
            case 0x56:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);

                    var result = ~Registers[rX];

                    Registers.Zero = ((ushort)result) == 0;
                    Registers.Carry = false;
                    Registers.Negative = false;
                    Registers.Signed = false;

                    Registers.Accumulator = (ushort)result;
                }
                break;
            // Logical left shift a register by another register and stores the result in $ACC.
            // SZCN: 0 Z C 0
            // 1 byte.
            // e.g. LLS $X, $Y
            case 0x57:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);
                    byte rY = (byte)(opcode & 0x000F);

                    var result = Registers[rX] << Registers[rY];

                    Registers.Zero = ((ushort)result) == 0;
                    Registers.Carry = (result & 0x10000) > 0;
                    Registers.Negative = false;
                    Registers.Signed = false;

                    Registers.Accumulator = (ushort)result;
                }
                break;
            // Logical left shift a register by an immediate and stores the result in $ACC.
            // SZCN: 0 Z C 0
            // 2 bytes.
            // e.g. LLS $X, i16
            case 0x58:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);

                    var result = Registers[rX] << GetImmediate16();

                    Registers.Zero = ((ushort)result) == 0;
                    Registers.Carry = (result & 0x10000) > 0;
                    Registers.Negative = false;
                    Registers.Signed = false;

                    Registers.Accumulator = (ushort)result;
                }
                break;
            // Logical right shift a register by another register and stores the result in $ACC.
            // SZCN: 0 Z 0 0
            // 1 byte.
            // e.g. RLS $X, $Y
            case 0x59:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);
                    byte rY = (byte)(opcode & 0x000F);

                    var x = Registers[rX] + (Registers.Carry ? 0x10000 : 0);
                    var result = x >> Registers[rY];

                    Registers.Zero = ((ushort)result) == 0;
                    Registers.Carry = false;
                    Registers.Negative = false;
                    Registers.Signed = false;

                    Registers.Accumulator = (ushort)result;
                }
                break;
            // Logical right shift a register by an immediate and stores the result in $ACC.
            // SZCN: 0 Z 0 0
            // 2 bytes.
            // e.g. RLS $X, i16
            case 0x5A:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);

                    var x = Registers[rX] + (Registers.Carry ? 0x10000 : 0);
                    var result = x >> GetImmediate16() ;

                    Registers.Zero = ((ushort)result) == 0;
                    Registers.Carry = false;
                    Registers.Negative = false;
                    Registers.Signed = false;

                    Registers.Accumulator = (ushort)result;
                }
                break;
            // Rotate a register left by another register and store the result in $ACC.
            // Flags are not touched.
            // 1 byte.
            // e.g. ROL $X, $Y
            case 0x5B:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);
                    byte rY = (byte)(opcode & 0x000F);

                    var amount = (Registers[rY] % 0x10);

                    var result = (Registers[rX] << amount) | (Registers[rX] >> (0x10 - amount));

                    Registers.Accumulator = (ushort)result;
                }
                break;
            // Rotate a register left by an immediate value and store the result in $ACC.
            // Flags are not touched.
            // 2 bytes.
            // e.g. ROL $X, i16
            case 0x5C:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);

                    var amount = (GetImmediate16() % 0x10);

                    var result = (Registers[rX] << amount) | (Registers[rX] >> (0x10 - amount));

                    Registers.Accumulator = (ushort)result;
                }
                break;
            // Rotate a register right by another register and store the result in $ACC.
            // Flags are not touched.
            // 1 byte.
            // e.g. ROR $X, $Y
            case 0x5D:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);
                    byte rY = (byte)(opcode & 0x000F);

                    var amount = (Registers[rY] % 0x10);

                    var result = (Registers[rX] >> amount) | (Registers[rX] << (0x10 - amount));

                    Registers.Accumulator = (ushort)result;
                }
                break;
            // Rotate a register right by an immediate value and store the result in $ACC.
            // Flags are not touched.
            // 2 bytes.
            // e.g. ROR $X, i16
            case 0x5E:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);

                    var amount = (GetImmediate16() % 0x10);

                    var result = (Registers[rX] >> amount) | (Registers[rX] << (0x10 - amount));

                    Registers.Accumulator = (ushort)result;
                }
                break;
            // Rotate a register left through carry by another register and store the result in $ACC.
            // SZCN: 0 Z C 0
            // 1 byte.
            // e.g. RLC $X, $Y
            case 0x5F:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);
                    byte rY = (byte)(opcode & 0x000F);

                    var amount = Registers[rY] % 0x11;
                    var number = Registers[rX] + (Registers.Carry ? 0x10000 : 0);

                    var result = (number << amount) | (number >> (0x11 - amount));

                    Registers.Zero = ((ushort)result) == 0;
                    Registers.Carry = (result & 0x10000) > 0;
                    Registers.Negative = false;
                    Registers.Signed = false;

                    Registers.Accumulator = (ushort)result;
                }
                break;
            // Rotate a register left through carry by an immediate value and store the result in $ACC.
            // SZCN: 0 Z C 0
            // 2 bytes.
            // e.g. RLC $X, i16
            case 0x60:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);

                    var amount = GetImmediate16() % 0x11;
                    var number = Registers[rX] + (Registers.Carry ? 0x10000 : 0);

                    var result = (number << amount) | (number >> (0x11 - amount));

                    Registers.Zero = ((ushort)result) == 0;
                    Registers.Carry = (result & 0x10000) > 0;
                    Registers.Negative = false;
                    Registers.Signed = false;

                    Registers.Accumulator = (ushort)result;
                }
                break;
            // Rotate a register right through carry by another register and store the result in $ACC.
            // SZCN: 0 Z C 0.
            // 1 byte.
            // e.g. RRC $X, $Y
            case 0x61:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);
                    byte rY = (byte)(opcode & 0x000F);

                    var amount = Registers[rY] % 0x11;
                    var number = Registers[rX] + (Registers.Carry ? 0x10000 : 0);

                    var result = (number >> amount) | (number << (0x11 - amount));

                    Registers.Zero = ((ushort)result) == 0;
                    Registers.Negative = false;
                    Registers.Signed = false;

                    // Convaluted way of setting carry bit. Refactor this in the future.
                    if (amount != 0)
                    {
                        Registers.Carry = (number >> (amount - 1) & 0x1) > 0;
                    }

                    Registers.Accumulator = (ushort)result;
                }
                break;
            // Rotate a register right through carry by an immediate value and store the result in $ACC.
            // SZCN: 0 Z C 0
            // 2 bytes.
            // e.g. RRC $X, i16
            case 0x62:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);

                    var amount = GetImmediate16() % 0x11;
                    var number = Registers[rX] + (Registers.Carry ? 0x10000 : 0);

                    var result = (number >> amount) | (number << (0x11 - amount));

                    Registers.Zero = ((ushort)result) == 0;
                    Registers.Negative = false;
                    Registers.Signed = false;

                    // Convaluted way of setting carry bit. Refactor this in the future.
                    if (amount != 0)
                    {
                        Registers.Carry = (number >> (amount - 1) & 0x1) > 0;
                    }

                    Registers.Accumulator = (ushort)result;
                }
                break;

            //
            //  Jump.
            //

            // Jump to an address specified by register.
            // 1 byte.
            // e.g. JMP $X
            case 0x70:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);

                    Registers.PC = Registers[rX];
                }
                break;
            // Jump to an address specified by register.
            // 2 byte.
            // e.g. JMP i16
            case 0x71:
                {
                    Registers.PC = GetImmediate16();
                }
                break;
            // Jump to an adress at a specified address plus a signed offset.
            // 2 byte.
            // e.g. JMP i16 + $X
            case 0x72:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);

                    var immediate = (short)GetImmediate16();

                    var address = (ushort)(Registers[rX] + immediate);

                    Registers.PC = address;
                }
                break;
            // Jump to an immediate value if two registers are equal.
            // 1 byte.
            // e.g. JEQ $X, $Y, i16
            case 0x73:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);
                    byte rY = (byte)(opcode & 0x000F);

                    var immediate = GetImmediate16();

                    if (Registers[rX] == Registers[rY])
                    {
                        Registers.PC = immediate;
                    }
                }
                break;
            // Check if a register is equal to zero, if so jump to a location in memory specified by another register.
            // 1 byte.
            // e.g. JEQZ $X, $Y
            case 0x74:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);
                    byte rY = (byte)(opcode & 0x000F);

                    if (Registers[rX] == 0)
                    {
                        Registers.PC = Registers[rY];
                    }
                }
                break;
            // Check if a register is equal to zero, if so jump to a location in memory specified by an immediate value.
            // 2 bytes.
            // e.g. JEQZ $X, i16
            case 0x75:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);

                    var immediate = GetImmediate16();

                    if (Registers[rX] == 0)
                    {
                        Registers.PC = immediate;
                    }
                }
                break;
            // Jump to an immediate value if two registers are not equal.
            // 1 byte.
            // e.g. JNEQ $X, $Y, i16
            case 0x76:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);
                    byte rY = (byte)(opcode & 0x000F);

                    var immediate = GetImmediate16();

                    if (Registers[rX] != Registers[rY])
                    {
                        Registers.PC = immediate;
                    }
                }
                break;
            // Check if a register is not equal to zero, if so jump to a location in memory specified by another register.
            // 1 byte.
            // e.g. JNEQZ $X, $Y
            case 0x77:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);
                    byte rY = (byte)(opcode & 0x000F);

                    if (Registers[rX] != 0)
                    {
                        Registers.PC = Registers[rY];
                    }
                }
                break;
            // Check if a register is not equal to zero, if so jump to a location in memory specified by an immediate value.
            // 2 bytes.
            // e.g. JNEQZ $X, i16
            case 0x78:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);

                    var immediate = GetImmediate16();

                    if (Registers[rX] != 0)
                    {
                        Registers.PC = immediate;
                    }
                }
                break;
            // If a specified flag bit is set, jump to a location in memory specified by a register.
            // 1 byte.
            // e.g. JF ?F, i16
            case 0x79:
                {
                    var flag = (byte)((opcode & 0x00F0) >> 4);
                    var rX = (byte)(opcode & 0x000F);

                    var result = ((Registers.Flags >> flag) & 0x1) > 0;

                    if (result)
                    {
                        Registers.PC = Registers[rX];
                    }
                }
                break;
            // If a specified flag bit is set, jump to a location in memory specified by an immediate value.
            // 2 byte.
            // e.g. JF ?F, i16
            case 0x7A:
                {
                    byte flag = (byte)((opcode & 0x00F0) >> 4);

                    var immediate = GetImmediate16();

                    var result = ((Registers.Flags >> flag) & 0x1) > 0;

                    if (result)
                    {
                        Registers.PC = immediate;
                    }
                }
                break;
            // If a specified flag bit is not set, jump to a location in memory specified by a register.
            // 1 byte.
            // e.g. JNF ?F, i16
            case 0x7B:
                {
                    byte flag = (byte)((opcode & 0x00F0) >> 4);
                    byte rX = (byte)(opcode & 0x000F);

                    var result = ((Registers.Flags >> flag) & 0x1) > 0;

                    if (!result)
                    {
                        Registers.PC = Registers[rX];
                    }
                }
                break;
            // If a specified flag bit is not set, jump to a location in memory specified by an immediate value.
            // 2 byte.
            // e.g. JNF ?F, i16
            case 0x7C:
                {
                    byte flag = (byte)((opcode & 0x00F0) >> 4);
                    
                    var immediate = GetImmediate16();

                    var result = ((Registers.Flags >> flag) & 0x1) > 0;

                    if (!result)
                    {
                        Registers.PC = immediate;
                    }
                }
                break;

            //
            //  Jump Signed
            //

            // Jump to an immediate value if the first register is greater than the other.
            // 1 byte.
            // e.g. JG $X, $Y, i16
            case 0x80:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);
                    byte rY = (byte)(opcode & 0x000F);

                    var x = (short)Registers[rX];
                    var y = (short)Registers[rY];

                    var immediate = GetImmediate16();

                    if (x > y)
                    {
                        Registers.PC = immediate;
                    }
                }
                break;
            // Check if a register is greater than zero, if so jump to a location in memory specified by another register.
            // 1 byte.
            // e.g. JGZ $X, $Y
            case 0x81:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);
                    byte rY = (byte)(opcode & 0x000F);

                    var x = (short)Registers[rX];

                    if (x > 0)
                    {
                        Registers.PC = Registers[rY];
                    }
                }
                break;
            // Check if a register is greater than zero, if so jump to a location in memory specified by an immediate value.
            // 2 bytes.
            // e.g. JGZ $X, i16
            case 0x82:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);

                    var x = (short)Registers[rX];

                    var immediate = GetImmediate16();

                    if (x > 0)
                    {
                        Registers.PC = immediate;
                    }
                }
                break;
            // Jump to an immediate value if the first register is less than the other.
            // 1 byte.
            // e.g. JL $X, $Y, i16
            case 0x83:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);
                    byte rY = (byte)(opcode & 0x000F);

                    var x = (short)Registers[rX];
                    var y = (short)Registers[rY];

                    var immediate = GetImmediate16();

                    if (x < y)
                    {
                        Registers.PC = immediate;
                    }
                }
                break;
            // Check if a register is less than zero, if so jump to a location in memory specified by another register.
            // 1 byte.
            // e.g. JLZ $X, $Y
            case 0x84:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);
                    byte rY = (byte)(opcode & 0x000F);

                    var x = (short)Registers[rX];

                    if (x < 0)
                    {
                        Registers.PC = Registers[rY];
                    }
                }
                break;
            // Check if a register is less than zero, if so jump to a location in memory specified by an immediate value.
            // 2 bytes.
            // e.g. JLZ $X, i16
            case 0x85:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);

                    var x = (short)Registers[rX];

                    var immediate = GetImmediate16();

                    if (x < 0)
                    {
                        Registers.PC = immediate;
                    }
                }
                break;
            // Jump to an immediate value if the first register is greater than or equal to the other.
            // 1 byte.
            // e.g. JGE $X, $Y, i16
            case 0x86:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);
                    byte rY = (byte)(opcode & 0x000F);

                    var x = (short)Registers[rX];
                    var y = (short)Registers[rY];

                    var immediate = GetImmediate16();

                    if (x >= y)
                    {
                        Registers.PC = immediate;
                    }
                }
                break;
            // Check if a register is greater than or equal to zero, if so jump to a location in memory specified by another register.
            // 1 byte.
            // e.g. JGEZ $X, $Y
            case 0x87:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);
                    byte rY = (byte)(opcode & 0x000F);

                    var x = (short)Registers[rX];

                    if (x >= 0)
                    {
                        Registers.PC = Registers[rY];
                    }
                }
                break;
            // Check if a register is greater than or equal to zero, if so jump to a location in memory specified by an immediate value.
            // 2 bytes.
            // e.g. JGEZ $X, i16
            case 0x88:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);

                    var x = (short)Registers[rX];

                    var immediate = GetImmediate16();

                    if (x >= 0)
                    {
                        Registers.PC = immediate;
                    }
                }
                break;
            // Jump to an immediate value if the first register is less than or equal to the other.
            // 1 byte.
            // e.g. JLE $X, $Y, i16
            case 0x89:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);
                    byte rY = (byte)(opcode & 0x000F);

                    var x = (short)Registers[rX];
                    var y = (short)Registers[rY];

                    var immediate = GetImmediate16();

                    if (x <= y)
                    {
                        Registers.PC = immediate;
                    }
                }
                break;
            // Check if a register is less than or equal to zero, if so jump to a location in memory specified by another register.
            // 1 byte.
            // e.g. JLEZ $X, $Y
            case 0x8A:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);
                    byte rY = (byte)(opcode & 0x000F);

                    var x = (short)Registers[rX];

                    if (x <= 0)
                    {
                        Registers.PC = Registers[rY];
                    }
                }
                break;
            // Check if a register is less than or equal to zero, if so jump to a location in memory specified by an immediate value.
            // 2 bytes.
            // e.g. JLEZ $X, i16
            case 0x8B:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);

                    var x = (short)Registers[rX];

                    var immediate = GetImmediate16();

                    if (x <= 0)
                    {
                        Registers.PC = immediate;
                    }
                }
                break;
            //
            // Jump Unsigned
            //

            // Check if a register is greater than (above) another register, if so jump to a memory location specified by an immediate value.
            // 2 byte.
            // e.g. JA $X, $Y, i16
            case 0x90:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);
                    byte rY = (byte)(opcode & 0x000F);

                    var x = Registers[rX];
                    var y = Registers[rY];

                    var immediate = GetImmediate16();

                    if (x > y)
                    {
                        Registers.PC = immediate;
                    }
                }
                break;
            // Check if a register is less than (below) another register, if so jump to a memory location specified by an immediate value.
            // 2 byte.
            // e.g. JB $X, $Y, i16
            case 0x91:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);
                    byte rY = (byte)(opcode & 0x000F);

                    var x = Registers[rX];
                    var y = Registers[rY];

                    var immediate = GetImmediate16();

                    if (x < y)
                    {
                        Registers.PC = immediate;
                    }
                }
                break;
            // Check if a register is greater than (above) or equal to another register, if so jump to a memory location specified by an immediate value.
            // 2 byte.
            // e.g. JAE $X, $Y, i16
            case 0x92:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);
                    byte rY = (byte)(opcode & 0x000F);

                    var x = Registers[rX];
                    var y = Registers[rY];

                    var immediate = GetImmediate16();

                    if (x >= y)
                    {
                        Registers.PC = immediate;
                    }
                }
                break;
            // Check if a register is less than (below) or equal to another register, if so jump to a memory location specified by an immediate value.
            // 2 byte.
            // e.g. JBE $X, $Y, i16
            case 0x93:
                {
                    byte rX = (byte)((opcode & 0x00F0) >> 4);
                    byte rY = (byte)(opcode & 0x000F);

                    var x = Registers[rX];
                    var y = Registers[rY];

                    var immediate = GetImmediate16();

                    if (x <= y)
                    {
                        Registers.PC = immediate;
                    }
                }
                break;
            default: throw new NotImplementedException($"Opcode: { opcode } not implemented or does not exist.");
        }
    }
}
