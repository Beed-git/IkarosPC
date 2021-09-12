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
                // No operation occurs.
                // 1 byte.
                // e.g. NOP
                case 0x00: break;
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
                // e.g. PUSH X
                case 0x02:
                    {
                        byte rX = (byte)((opcode & 0x00F0) >> 4);

                        Push(_registers[rX]);
                    }
                    break;
                // Pushs a literal value on the stack.
                // 2 bytes.
                // e.g. PUSH 0x1234
                case 0x03:
                    {
                        Push(_memory[_registers.PC]);

                        _registers.PC++;
                    }
                    break;
                // Pops the value of a register off the stack.
                // 1 byte.
                // e.g. POP X
                case 0x04:
                    {
                        byte rX = (byte)((opcode & 0x00F0) >> 4);

                        _registers[rX] = Pop();
                    }
                    break;
                // Calls a subroutine from a literal value. 
                // 2 bytes.
                // e.g. CALL func_name:
                case 0x05:
                    {
                        var literal = _memory[_registers.PC];
                        _registers.PC++;

                        // Save state to stack
                        SaveStackState();

                        _registers.PC = literal;
                    }
                    break;
                // Calls a subroutine from a register
                // 2 bytes.
                // e.g. CALL X:
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

                // Moves value from first register to second register.
                // 1 byte.
                // e.g. MOV X Y
                case 0x10:
                    {
                        byte rX = (byte)((opcode & 0x00F0) >> 4);
                        byte rY = (byte)(opcode & 0x000F);

                        _registers[rY] = _registers[rX];
                    }
                    break;
                // Move value from memory specified by first register to second register.
                // 1 byte.
                // e.g. MOV (X) Y
                case 0x11:
                    {
                        byte rX = (byte)((opcode & 0x00F0) >> 4);
                        byte rY = (byte)(opcode & 0x000F);

                        _registers[rY] = _memory[_registers[rX]];
                    }
                    break;
                // Move value from first register to memory specified by second register.
                // 1 byte.
                // e.g. MOV X (Y)
                case 0x12:
                    {
                        byte rX = (byte)((opcode & 0x00F0) >> 4);
                        byte rY = (byte)(opcode & 0x000F);

                        _memory[_registers[rY]] = _registers[rX];
                    }
                    break;
                // Store literal value into register.
                // 2 bytes
                // e.g. MOV 0x1234 X
                case 0x13:
                    {
                        byte rX = (byte)((opcode & 0x00F0) >> 4);
                        // Ignore value at 0x000F

                        _registers[rX] = _memory[_registers.PC];
                        _registers.PC++;
                    }
                    break;
                // Store literal value into memory specified by register.
                // 2 bytes
                // e.g. MOV 0x1234 (X)
                case 0x14:
                    {
                        byte rX = (byte)((opcode & 0x00F0) >> 4);
                        // Ignore value at 0x000F

                        _memory[_registers[rX]] = _memory[_registers.PC];
                        _registers.PC++;
                    }
                    break;
                // Adds the values of the first and second register and puts the result in the accumulator.
                // ZCN: Z C 0
                // 1 byte.
                // e.g. ADD X Y
                case 0x20:
                    {
                        byte rX = (byte)((opcode & 0x00F0) >> 4);
                        byte rY = (byte)(opcode & 0x000F);

                        var result = _registers[rX] + _registers[rY];

                        Registers.Zero = ((ushort)result) == 0;
                        Registers.Carry = result > ushort.MaxValue;
                        Registers.Negative = false;

                        _registers.Accumulator = (ushort)(_registers[rX] + _registers[rY]);
                    }
                    break;
                // Adds the value of the first register and a literal value and stores the result in the accumulator.
                // ZCN: Z C 0
                // 2 bytes.
                // e.g. ADD 0x1234 X
                case 0x21:
                    {
                        byte rX = (byte)((opcode & 0x00F0) >> 4);

                        var literal = _memory[_registers.PC];
                        _registers.PC++;

                        var result = _registers[rX] + literal;

                        Registers.Zero = ((ushort)result) == 0;
                        Registers.Carry = result > ushort.MaxValue;
                        Registers.Negative = false;

                        _registers.Accumulator = (ushort)(_registers[rX] + literal);
                    }
                    break;
                // Subtracts the value of the first register from the second and stores the result in the accumulator.
                // ZCN: Z C 1
                // 1 byte.
                // e.g. SUB X Y
                case 0x22:
                    {
                        byte rX = (byte)((opcode & 0x00F0) >> 4);
                        byte rY = (byte)(opcode & 0x000F);

                        var result = _registers[rX] - _registers[rY];

                        Registers.Zero = ((ushort)result) == 0;
                        Registers.Carry = result < 0;
                        Registers.Negative = true;

                        _registers.Accumulator = (ushort)(_registers[rX] - _registers[rY]);
                    }
                    break;
                // Subtracts the value of the register from the literal value and stores the result in the accumulator.
                // ZCN: Z C 1
                // 2 bytes.
                // e.g. SUB X 0x1234
                case 0x23:
                    {
                        byte rX = (byte)((opcode & 0x00F0) >> 4);

                        var literal = _memory[_registers.PC];
                        _registers.PC++;

                        var result = _registers[rX] - literal;

                        Registers.Zero = ((ushort)result) == 0;
                        Registers.Carry = result < 0;
                        Registers.Negative = true;

                        _registers.Accumulator = (ushort)(_registers[rX] - literal);
                    }
                    break;
                // Subtracts the literal value from the value of the register and stores the result in the accumulator.
                // ZCN: Z C 1
                // 2 bytes.
                // e.g. SUB 0x1234 X
                case 0x24:
                    {
                        byte rX = (byte)((opcode & 0x00F0) >> 4);

                        var literal = _memory[_registers.PC];
                        _registers.PC++;

                        var result = literal - _registers[rX];

                        Registers.Zero = ((ushort)result) == 0;
                        Registers.Carry = result < 0;
                        Registers.Negative = true;

                        _registers.Accumulator = (ushort)(literal - _registers[rX]);
                    }
                    break;
                // Multiplies the values of the two registers and stores the result in the accumulator.
                // ZCN: Z C 0
                // 1 byte.
                // e.g. MUL X Y
                case 0x25:
                    {
                        byte rX = (byte)((opcode & 0x00F0) >> 4);
                        byte rY = (byte)(opcode & 0x000F);

                        var result = _registers[rX] * _registers[rY];

                        Registers.Zero = ((ushort)result) == 0;
                        Registers.Carry = result > ushort.MaxValue;
                        Registers.Negative = false;

                        _registers.Accumulator = (ushort)(_registers[rX] * _registers[rY]);
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