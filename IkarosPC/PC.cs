using System;

namespace IkarosPC;

internal class PC
{
    private CPU _cpu;
    private Registers _registers;
    private Memory _memory;

    public PC()
    {
        _registers = new Registers();
        _memory = new Memory(_registers);

        _cpu = new CPU(_registers, _memory);
    }

    public void Run()
    {
        while (!_cpu.Stopped)
        {
            _cpu.Step();
        }
    }
}
