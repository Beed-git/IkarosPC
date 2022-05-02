using System;

namespace IkarosPC;

public class Registers
{
    private readonly ushort[] _registers;
    private readonly ushort[] _specialRegisters;

    private ushort _stackFrameSize;

    public Registers()
    {
        // Represents the 8 general-purpose registers plus the accumulator.
        _registers = new ushort[9];
        // $PC, $SP, $FP, $MBC, $RSC
        _specialRegisters = new ushort[5];
    }

    public void Reset()
    {
        // Clear general registers.
        Array.Fill<ushort>(_registers, 0);

        // Clear special registers.
        PC = 0;
        SP = 0;
        FP = 0;

        // Set stack to top of memory.
        SP = Memory.STACK_SIZE;
        FP = Memory.STACK_SIZE;

        // Reset stack frame pointer.
        StackFrameSize = 0;

        Zero = false;
        Carry = false;
        Negative = false;
    }

    // Get register by value
    public ushort this[byte i]
    {
        get
        {
            if (i > _registers.Length)
                throw new IndexOutOfRangeException($"Register at {i} does not exist.");
            
            return _registers[i];
        }
        set
        {
            if (i > _registers.Length)
                throw new IndexOutOfRangeException($"Register at {i} does not exist.");

            _registers[i] = value;
        }
    }

    // Get special register by value.
    public ushort[] Special => _specialRegisters;

    // Special Registers

    // Accumulator - Most math functions put result in this register.
    public ushort Accumulator
    {
        get => _registers[8];
        set => _registers[8] = value;
    }

    // Program counter.
    public ushort PC
    {
        get => _specialRegisters[0];
        set => _specialRegisters[0] = value;
    }

    // Stack pointer.
    public ushort SP
    {
        get => _specialRegisters[1];
        set => _specialRegisters[1] = value;
    }

    // Frame pointer.
    public ushort FP
    {
        get => _specialRegisters[2];
        set => _specialRegisters[2] = value;
    }

    // Memory bank controller
    public ushort MBC
    {
        get => _specialRegisters[3];
        set => _specialRegisters[3] = value;
    }

    // Tracks the size of the current stack frame. Should never be touched by the user. Shouldn't be used outside of Pop, Push, LoadState, and SaveState.
    public ushort StackFrameSize
    {
        get => _stackFrameSize;
        set => _stackFrameSize = value;
    }

    /// <summary>
    /// Flags register.
    /// 0000 SZCN
    /// </summary>
    public ushort Flags
    {
        get
        {
            int u = 0;

            u += Signed ? 0b1000 : 0;
            u += Zero ? 0b0100 : 0;
            u += Carry ? 0b0010 : 0;
            u += Negative ? 0b0001 : 0;

            return (ushort)u;
        }
        set
        {
            Signed = (value & 0b1000) > 0;
            Zero = (value & 0b0100) > 0;
            Carry = (value & 0b0010) > 0;
            Negative = (value & 0b0001) > 0;
        }
    }

    // Flags
    public bool Zero { get; set; }
    public bool Carry { get; set; }
    public bool Negative { get; set; }
    public bool Signed { get; set; }

    // General purpose registers
    public ushort A
    {
        get => _registers[0];
        set => _registers[0] = value;
    }
    public ushort B
    {
        get => _registers[1];
        set => _registers[1] = value;
    }
    public ushort C
    {
        get => _registers[2];
        set => _registers[2] = value;
    }
    public ushort D
    {
        get => _registers[3];
        set => _registers[3] = value;
    }
    public ushort E
    {
        get => _registers[4];
        set => _registers[4] = value;
    }
    public ushort X
    {
        get => _registers[5];
        set => _registers[5] = value;
    }
    public ushort Y
    {
        get => _registers[6];
        set => _registers[6] = value;
    }
    public ushort Z
    {
        get => _registers[7];
        set => _registers[7] = value;
    }
}
