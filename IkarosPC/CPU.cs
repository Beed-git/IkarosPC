namespace IkarosPC;

public partial class CPU
{
    // 4mhz clock.
    public const int CLOCKS_PER_SECOND = 4000000;

    public CPU(Registers registers, Memory memory)
    {
        Registers = registers;
        Memory = memory;

        Registers.Reset();

        Stopped = true;
    }

    public Memory Memory { get; init; }
    public Registers Registers { get; init; }
    public bool Stopped { get; private set; }

    public void Step()
    {
        if (Stopped)
            return;

        var opcode = GetImmediate16();

        HandleOpcode(opcode);
    }

    // Instruction helpers
    public ushort GetImmediate16()
    {
        var immediate = Memory[Registers.PC];
        Registers.PC++;

        return immediate;
    }
}
