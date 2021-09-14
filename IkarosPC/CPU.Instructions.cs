using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkarosPC
{
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
                        _stopped = true;
                    }
                    break;
                // Pushs the value of a register on the stack.
                // 1 byte.
                // e.g. PUSH $X
                case 0x02:
                    {
                        byte rX = (byte)((opcode & 0x00F0) >> 4);

                        Push(_registers[rX]);
                    }
                    break;
                // Pushs an immediate value on the stack.
                // 2 bytes.
                // e.g. PUSH i16
                case 0x03:
                    {
                        var immediate = GetImmediate16();

                        Push(immediate);
                    }
                    break;
                // Pops the value of a register off the stack.
                // 1 byte.
                // e.g. POP $X
                case 0x04:
                    {
                        byte rX = (byte)((opcode & 0x00F0) >> 4);

                        _registers[rX] = Pop();
                    }
                    break;
                // Calls a subroutine from an immediate value. 
                // 2 bytes.
                // e.g. CALL i16:
                case 0x05:
                    {
                        var immediate = GetImmediate16();

                        // Save state to stack
                        SaveStackState();

                        _registers.PC = immediate;
                    }
                    break;
                // Calls a subroutine from a register
                // 2 bytes.
                // e.g. CALL $X:
                case 0x06:
                    {
                        SaveStackState();

                        byte rX = (byte)((opcode & 0x00F0) >> 4);
                        _registers.PC = _registers[rX];
                    }
                    break;
                // Returns from a subroutine.
                // 1 byte.
                // e.g. RET
                case 0x07:
                    {
                        LoadStackState();
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

                        _registers[rY] = _registers[rX];
                    }
                    break;
                // Moves an immediate value into a specified register.
                // 2 bytes.
                // e.g. MOV i16, $X
                case 0x11:
                    {
                        byte rX = (byte)((opcode & 0x00F0) >> 4);

                        var immediate = GetImmediate16();
                        _registers[rX] = immediate;
                    }
                    break;
                // Moves a value in memory specified by a register into another register.
                // 1 byte.
                // e.g. MOV ($X), $Y
                case 0x12:
                    {
                        byte rX = (byte)((opcode & 0x00F0) >> 4);
                        byte rY = (byte)(opcode & 0x000F);

                        _registers[rY] = _memory[_registers[rX]];
                    }
                    break;
                // Moves a value in memory specified by an immediate value into a register.
                // 2 bytes.
                // e.g. MOV (i16), $X
                case 0x13:
                    {
                        byte rX = (byte)((opcode & 0x00F0) >> 4);

                        var immediate = GetImmediate16();
                        _registers[rX] = _memory[immediate];
                    }
                    break;
                // Moves a value from a register into memory specified by another register.
                // 1 byte.
                // e.g. MOV $X, ($Y)
                case 0x14:
                    {
                        byte rX = (byte)((opcode & 0x00F0) >> 4);
                        byte rY = (byte)(opcode & 0x000F);

                        _memory[_registers[rY]] = _registers[rX];
                    }
                    break;
                // Stores an immediate value in memory specified by a register.
                // 2 bytes.
                // e.g. MOV i16, ($X)
                case 0x15:
                    {
                        byte rX = (byte)((opcode & 0x00F0) >> 4);

                        var immediate = GetImmediate16();
                        _memory[_registers[rX]] = immediate;
                    }
                    break;
                // Moves a value from a register into memory specified by an immediate.
                // 2 bytes.
                // e.g. MOV $X, (i16)
                case 0x16:
                    {
                        byte rX = (byte)((opcode & 0x00F0) >> 4);

                        var immediate = GetImmediate16();
                        _memory[immediate] = _registers[rX];
                    }
                    break;
                // Moves an immediate value into memory specified by an immediate value.
                // 3 bytes.
                // e.g. MOV i16, (i16)
                case 0x17:
                    {
                        var immediate = GetImmediate16();
                        var address = GetImmediate16();

                        _memory[address] = immediate;
                    }
                    break;
                // Moves a value from memory specified by a register into another memory location specified by another register.
                // 1 byte.
                // e.g. MOV ($X), ($Y)
                case 0x18:
                    {
                        byte rX = (byte)((opcode & 0x00F0) >> 4);
                        byte rY = (byte)(opcode & 0x000F);

                        _memory[_registers[rY]] = _memory[_registers[rX]];
                    }
                    break;
                // Moves a value in memory specified by a register into a memory location specified by an immediate value.
                // 2 bytes.
                // e.g. MOV ($X), (i16)
                case 0x19:
                    {
                        byte rX = (byte)((opcode & 0x00F0) >> 4);

                        var immediate = GetImmediate16();
                        _memory[immediate] = _memory[_registers[rX]];
                    }
                    break;
                // Moves a value in memory specified by an immediate value into a memory location specified by a register.
                // 2 bytes.
                // e.g. MOV (i16), ($X)
                case 0x1A:
                    {
                        byte rX = (byte)((opcode & 0x00F0) >> 4);

                        var immediate = GetImmediate16();
                        _memory[_registers[rX]] = _memory[immediate];
                    }
                    break;
                // Moves a value in memory specified by an immediate value into a memory location specified by another immediate value.
                // 3 bytes.
                // e.g. MOV (i16), (i16)
                case 0x1B:
                    {
                        var immediate = GetImmediate16();
                        var address = GetImmediate16();

                        _memory[address] = _memory[immediate];
                    }
                    break;
                // Store a value in memory at an immediate value + an offset.
                // 2 bytes.
                // e.g. MOV X, (0x1234 + Y)
                case 0x1C:
                    {
                        byte rX = (byte)((opcode & 0x00F0) >> 4);
                        byte rY = (byte)(opcode & 0x000F);

                        var offset = GetImmediate16();

                        var address = (ushort)(offset + _registers[rY]);

                        _memory[address] = _registers[rX];
                    }
                    break;
                // Stores a value in a register at an immediate value + an offset.
                // 2 bytes.
                // e.g. MOV (0x1234 + $X), $Y
                case 0x1D:
                    {
                        byte rX = (byte)((opcode & 0x00F0) >> 4);
                        byte rY = (byte)(opcode & 0x000F);

                        var offset = GetImmediate16();

                        var address = (ushort)(offset + _registers[rX]);

                        _registers[rY] = _memory[address];
                    }
                    break;

                //
                // General Arithmetic
                //

                // Adds the values of the first and second register and puts the result in the accumulator.
                // SZCN: 0 Z C 0
                // 1 byte.
                // e.g. ADD $X, $Y
                case 0x20:
                    {
                        byte rX = (byte)((opcode & 0x00F0) >> 4);
                        byte rY = (byte)(opcode & 0x000F);

                        var result = _registers[rX] + _registers[rY];

                        Registers.Zero = ((ushort)result) == 0;
                        Registers.Carry = result > ushort.MaxValue;
                        Registers.Negative = false;
                        Registers.Signed = false;

                        _registers.Accumulator = (ushort)result;
                    }
                    break;
                // Adds the value of the first register and an immediate value and stores the result in the accumulator.
                // SZCN: 0 Z C 0
                // 2 bytes.
                // e.g. ADD i16, $X
                case 0x21:
                    {
                        byte rX = (byte)((opcode & 0x00F0) >> 4);

                        var immediate = GetImmediate16();

                        var result = _registers[rX] + immediate;

                        Registers.Zero = ((ushort)result) == 0;
                        Registers.Carry = result > ushort.MaxValue;
                        Registers.Negative = false;
                        Registers.Signed = false;

                        _registers.Accumulator = (ushort)(_registers[rX] + immediate);
                    }
                    break;
                // Subtracts the value of the first register from the second and stores the result in the accumulator.
                // SZCN: 0 Z C 1
                // 1 byte.
                // e.g. SUB $X, $Y
                case 0x22:
                    {
                        byte rX = (byte)((opcode & 0x00F0) >> 4);
                        byte rY = (byte)(opcode & 0x000F);

                        var result = _registers[rX] - _registers[rY];

                        Registers.Zero = ((ushort)result) == 0;
                        Registers.Carry = result < 0;
                        Registers.Negative = true;
                        Registers.Signed = false;

                        _registers.Accumulator = (ushort)result;
                    }
                    break;
                // Subtracts the value of the register from the immediate value and stores the result in the accumulator.
                // ZCN: 0 Z C 1
                // 2 bytes.
                // e.g. SUB $X, i16
                case 0x23:
                    {
                        byte rX = (byte)((opcode & 0x00F0) >> 4);

                        var immediate = GetImmediate16();

                        var result = _registers[rX] - immediate;

                        Registers.Zero = ((ushort)result) == 0;
                        Registers.Carry = result < 0;
                        Registers.Negative = true;
                        Registers.Signed = false;

                        _registers.Accumulator = (ushort)result;
                    }
                    break;
                // Subtracts the immediate value from the value of the register and stores the result in the accumulator.
                // SZCN: 0 Z C 1
                // 2 bytes.
                // e.g. SUB i16, $X
                case 0x24:
                    {
                        byte rX = (byte)((opcode & 0x00F0) >> 4);

                        var immediate = GetImmediate16();

                        var result = immediate - _registers[rX];

                        Registers.Zero = ((ushort)result) == 0;
                        Registers.Carry = result < 0;
                        Registers.Negative = true;
                        Registers.Signed = false;

                        _registers.Accumulator = (ushort)result;
                    }
                    break;
                // Increments the value of the register and stores the result back in the register.
                // SZCN: 0 Z C 0
                // 1 byte.
                // e.g. INC $X
                case 0x25:
                    {
                        byte rX = (byte)((opcode & 0x00F0) >> 4);

                        var result = _registers[rX] + 1;

                        Registers.Zero = ((ushort)result) == 0;
                        Registers.Carry = result > ushort.MaxValue;
                        Registers.Negative = false;
                        Registers.Signed = false;

                        _registers[rX] = (ushort)result;
                    }
                    break;
                // Decrements the value of the register and stores the result back in the register.
                // SZCN: 0 Z C 1
                // 1 byte.
                // e.g. INC $X
                case 0x26:
                    {
                        byte rX = (byte)((opcode & 0x00F0) >> 4);

                        var result = _registers[rX] - 1;

                        Registers.Zero = ((ushort)result) == 0;
                        Registers.Carry = result < 0;
                        Registers.Negative = true;
                        Registers.Signed = false;

                        _registers.Accumulator = (ushort)result;
                    }
                    break;

                //
                // Unsigned Arithmetic.
                //

                // Multiplies the values of the two registers and stores the result in the accumulator.
                // ZCN: Z C 0
                // 1 byte.
                // e.g. MUL X Y
                case 0x30:
                    {
                        byte rX = (byte)((opcode & 0x00F0) >> 4);
                        byte rY = (byte)(opcode & 0x000F);

                        var result = _registers[rX] * _registers[rY];

                        Registers.Zero = ((ushort)result) == 0;
                        Registers.Carry = result > ushort.MaxValue;
                        Registers.Negative = false;
                        Registers.Signed = false;

                        _registers.Accumulator = (ushort)result;
                    }
                    break;
                // Multiplies the value of a register and a literal and stores the result in the accumulator.
                // ZCN: Z C 0
                // 2 bytes.
                // e.g. MUL X 0x1234

                // Divides the value of the first register by the value of the second and stores the result in the accumulator.
                // ZCN: Z C 1
                // 1 byte.
                // e.g. DIV X Y

                // Divides the value of the register by the literal value and stores the result in the accumulator.
                // ZCN: Z C 1
                // 2 bytes.
                // e.g. DIV X 0x1234

                // Divides the literal value by the value of the register and stores the result in the accumulator.
                // ZCN: Z C 1
                // 2 bytes.
                // e.g. DIV 0x1234 X

                // Modulos the value of the first register by the second register and stores the result in the accumulator.
                // ZCN: Z C 1
                // 1 byte.
                // e.g. MOD X Y

                // Modulos the value of the register by the literal value and stores the result in the accumulator.
                // ZCN: Z C 1
                // 2 bytes
                // e.g. MOD X 0x1234

                // Modulos the literal value by the value of the register and stores the result in the accumulator.
                // ZCN: Z C 1
                // 2 bytes.
                // e.g. MOD 0x1234 X

                // Increment register. Value gets stored in the same register which was incremented.
                // ZCN: Z C 1
                // 1 byte.
                // e.g. INC X

                // Decrement register. Value gets stored in the same register which as decremented.
                // ZCN: Z C 0
                // 1 byte.
                // e.g. DEC X

                // start from 0x30

                // Ands the values of the two registers and stores the result in the accumulator.
                // ZCN: Z 0 0
                // 1 byte.
                // e.g. AND X Y

                // Ands the literal value with the value in the specified register and stores the result in the accumulator.
                // ZCN: Z 0 0
                // 2 bytes.
                // e.g. AND X 0x1234

                // Ors the values of the two registers and stores the result in the accumulator.
                // ZCN: Z 0 0
                // 1 byte.
                // e.g. OR X Y

                // Ors the literal value with the value in the specified register and stores the result in the accumulator.
                // ZCN: Z 0 0
                // 2 bytes.
                // e.g. OR X 0x1234

                // Xors the values of the two registers and stores the result in the accumulator.
                // ZCN: Z 0 0
                // 1 byte.
                // e.g. XOR X Y

                // Xors the literal value with the value in the specified register and stores the result in the accumulator.
                // ZCN: Z 0 0
                // 2 bytes.
                // e.g. XOR X 0x1234

                // Sets the program counter to the value stored in the register.
                // 1 byte.
                // e.g. J X

                // Sets the program counter to the literal value.
                // 2 bytes.
                // e.g. JUMP 0xFF00
                // e.g. JZ X
                // e.g. JZ 0xFF00
                // e.g. JC X
                // e.g. JC 0xFF00

                // e.g. JS X (maybe not this?)
                // e.g. JS 0xFF00

                // e.g. JNZ X
                // e.g. JNZ 0xFF00
                // e.g. JNC X
                // e.g. JNC 0xFF00

                // e.g. JNS X
                // e.g. JNS 0xFF00

                // Jump if the two registers are equal.
                // 1 byte.
                // e.g. JEQ X, Y

                // e.g. JEQ X, 0x1234
                case 0xF0:
                    {
                        // REMOVE ME
                        byte rX = (byte)((opcode & 0x00F0) >> 4);

                        var literal = _memory.Ram[_registers.PC];
                        _registers.PC++;

                        if (_registers[rX] != 0)
                            _registers.PC = literal;
                    }
                    break;
                // e.g. JNEQ X, Y
                // e.g. JNEQ X, 0x12340

                // Bit shift left (through carry)
                // Bit shift right (through carry)
                // Rotate left (through carry)
                // Rotate right (through carry)

                default: throw new NotImplementedException($"Opcode: { opcode } not implemented or does not exist.");
            }
        }
    }
}