using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkarosPC
{
    public partial class CPU
    {
        void HandleControlFunctions(ushort opcode)
        {
            byte nibble = (byte)(opcode & 0x00FF);

            switch (nibble)
            {
                // Control instructions (0x0NNN).

                // No operation occurs.
                // 1 byte.
                // e.g. NOP
                case 0: break;
                // Stop execution from occuring.
                // 1 byte
                // e.g. STOP
                
                // Pops a value onto the stack.
                // 1 byte.
                // e.g. POP X

                // Pushs a value of the stack.
                // 1 byte.
                // e.g. PUSH X

                // Calls a subroutine. The result of the subroutine should be put into the accumulator. 
                // 2 bytes.
                // e.g. CALL func_name:

                // Returns from a subroutine.
                // 1 byte.
                // e.g. RET
                default: throw new NotImplementedException($"Opcode: { opcode } not implemented or does not exist.");
            }
        }

        void HandleMoveFunctions(ushort opcode)
        {
            byte nibble = (byte)((opcode & 0xFF00) >> 8);

            switch (nibble)
            {
                // Mov instructions. (0x1NNN)
                // In general format is INS_NAME *Area 1* *Area 2*
                // Value of *Area 1* goes into *Area 2*

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

                default: throw new NotImplementedException($"Opcode: { opcode } not implemented or does not exist.");
            }
        }

        void HandleMathFunctions(ushort opcode)
        {
            byte nibble = (byte)((opcode & 0xFF00) >> 8);

            switch (nibble)
            {
                // Math functions. (2NNN & 3NNN)
                // Most functions will store the value in the special Accumulator register.
                // Note: Subtractions take the left value from the right.
                // e.g SUB X Y will subtract x from y.

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

                        var result = literal - _registers[rX];

                        Registers.Zero = ((ushort)result) == 0;
                        Registers.Carry = result < 0;
                        Registers.Negative = true;

                        _registers.Accumulator = (ushort)(literal - _registers[rX]);
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

                        var result = _registers[rX] - literal;

                        Registers.Zero = ((ushort)result) == 0;
                        Registers.Carry = result < 0;
                        Registers.Negative = true;

                        _registers.Accumulator = (ushort)(_registers[rX] - literal);
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
                

                default: throw new NotImplementedException($"Opcode: { opcode } not implemented or does not exist.");
            }
        }

        void HandleJumpFunctions(ushort opcode)
        {
            byte nibble = (byte)((opcode & 0xFF00) >> 8);

            switch (nibble)
            {
                // Sets the program counter to the value stored in the register.
                // 1 byte.
                // e.g. J X

                // Sets the program counter to the literal value.
                // 2 bytes.
                // e.g. J 0xFF00

                // e.g. JZ X
                // e.g. JZ 0xFF00
                // e.g. JC X
                // e.g. JC 0xFF00
                // e.g. JS X
                // e.g. JS 0xFF00
                
                // e.g. JNZ X
                // e.g. JNZ 0xFF00
                // e.g. JNC X
                // e.g. JNC 0xFF00
                // e.g. JNS X
                // e.g. JNS 0xFF00

                default: throw new NotImplementedException($"Opcode: { opcode } not implemented or does not exist.");
            }
        }
    }
    // Template
    //void HandleRegisterFunctions(ushort opcode)
    //{
    //    byte nibble = (byte)(opcode & 0xFF00);

    //    switch (nibble)
    //    {
    //        default: throw new NotImplementedException($"Opcode: { opcode } not implemented or does not exist.");
    //    }
    //}
}
