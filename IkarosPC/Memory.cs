using System;

namespace IkarosPC;

public class Memory
{
    public const int STACK_SIZE = 0x4000;

    // Cpus registers.
    private readonly Registers _registers;

    private readonly ushort[] _memory;
    private readonly ushort[] _stack;
    private readonly ushort[,] _bank;

    /// <summary>
    /// ~65kb regular memory
    /// ~49kb stack memory
    /// ~8.39mb banked memory
    /// </summary>
    public Memory(Registers registers)
    {
        _registers = registers;

        _memory = new ushort[0x10000];
        _stack = new ushort[STACK_SIZE];
        
        // 255 banks of 0x4000 memory for a total of ~8.39mb of switchable storage. (Including 0th bank - none.)
        _bank = new ushort[0xFF, 0x4000];
    }

    public ushort this[ushort i]
    {
        get
        {
            // Return banked memory.
            if (i >= 0xC000)
            {
                if (_registers.MBC == 0)
                    return _memory[i];

                var address = i - 0xC000;
                var mbc = (_registers.MBC - 1) % _bank.GetLength(0);

                return _bank[mbc, address];
            }
            else return _memory[i];
        }
        set
        {
            // Set banked memory.
            if (i >= 0xC000)
            {
                if (_registers.MBC == 0)
                {
                    _memory[i] = value;
                    return;
                }

                var address = i - 0xB000;
                var mbc = (_registers.MBC - 1) % _bank.GetLength(0);

                _bank[mbc, address] = value;
                return;
            }
            else _memory[i] = value;
        }
    }

    /// <summary>
    /// Pushes a ushort onto the stack.
    /// </summary>
    /// <param name="value"></param>
    public void Push(ushort value)
    {
        _stack[_registers.SP] = value;
        _registers.SP--;

        _registers.StackFrameSize++;
    }

    /// <summary>
    /// Pops a ushort value off the stack.
    /// </summary>
    /// <returns></returns>
    public ushort Pop()
    {
        _registers.StackFrameSize--;

        _registers.SP++;
        return _stack[_registers.SP];
    }

    /// <summary>
    /// Reads the top ushort on the stack.
    /// </summary>
    /// <returns></returns>
    public ushort Peek()
    {
        return _stack[_registers.SP + 1];
    }

    /// <summary>
    /// Pushes a stack frame onto the stack.
    /// The amount of arguments should have been pushed before calling this function.
    /// </summary>
    public void PushStackFrame()
    {
        Push(_registers.A);
        Push(_registers.B);
        Push(_registers.C);
        Push(_registers.D);
        Push(_registers.E);
        Push(_registers.X);
        Push(_registers.Y);
        Push(_registers.Z);

        Push(_registers.Flags);
        Push(_registers.PC);
        Push((ushort)(_registers.StackFrameSize + 1));

        _registers.FP = _registers.SP;
        _registers.StackFrameSize = 0;
    }

    /// <summary>
    /// Restores the previous stack frame.
    /// </summary>
    public void PopStackFrame()
    {
        _registers.SP = _registers.FP;

        _registers.StackFrameSize = Pop();
        var previousStackFrameSize = _registers.StackFrameSize;

        _registers.PC = Pop();
        _registers.Flags = Pop();

        _registers.Z = Pop();
        _registers.Y = Pop();
        _registers.X = Pop();
        _registers.E = Pop();
        _registers.D = Pop();
        _registers.C = Pop();
        _registers.B = Pop();
        _registers.A = Pop();

        var numberOfArguments = Pop();
        for (int i = 0; i < numberOfArguments; i++)
        {
            Pop();
        }

        _registers.FP += previousStackFrameSize;
    }
}
