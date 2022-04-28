namespace IkarosPC;

public partial class CPU
{
    private readonly Memory _memory;
    private readonly Registers _registers;

    private bool _stopped;

    public bool Stopped => _stopped;

    public CPU(Registers registers, Memory memory)
    {
        _registers = registers;
        _memory = memory;

        _registers.Reset();

        _stopped = false;
    }

    public void Reset()
    {
        _registers.Reset();
        // _memory.Reset();
    }

    public void Step()
    {
        if (_stopped)
            return;

        var opcode = GetImmediate16();

        HandleOpcode(opcode);
    }

    // Instruction helpers
    public ushort GetImmediate16()
    {
        var immediate = _memory[_registers.PC];
        _registers.PC++;

        return immediate;
    }
}
