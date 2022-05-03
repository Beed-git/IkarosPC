namespace IkarosPC;

internal class PC
{
    public CPU CPU { get; init; }
    public Registers Registers { get; init; }
    public Memory Memory {  get; init;  }

    public PC()
    {
        Registers = new Registers();
        Memory = new Memory(Registers);

        CPU = new CPU(Registers, Memory);
    }

    public void Step()
    {
        if (!CPU.Stopped)
        {
            CPU.Step();
        }
    }
}
