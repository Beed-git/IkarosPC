using NUnit.Framework;
using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkarosPC.Tests.MiniProgramTests
{
    class FillScreenTest
    {
        Registers _registers;

        Memory _memory;
        CPU _cpu;

        [SetUp]
        public void Setup()
        {
            _registers = new Registers();

            _memory = new Memory(_registers);
            _cpu = new CPU(_memory, _registers);
        }

        [Test]
        public void FillScreen()
        {
            _memory.SetInitialMemory(new ushort[]
            {
                // Address of ram-switcher.
                0x1300, 0xFFFF,
                // Screen size.
                0x1310, 0x5A00,
                // Yellow
                0x1320, 0xFF00,
                // Store 2 in ram-switch
                0x1400, 0x0002,
                // Write yellow at address
                0x1221,
                // B -= 1
                0x2310, 0x0001,
                0x1081,
                // Branch if not zero
                0xF010, 0x0008,
                // Final write yellow at address
                0x1221,
                // Switch back to normal ram.
                0x1400, 0x0000,
                // Stop
                0x0100
            });

            var display = new Display(_memory.Vram);

            var window = new RenderWindow(new VideoMode((uint)_memory.Vram.ScreenX, (uint)_memory.Vram.ScreenY), "Ikaros");

            var running = true;

            window.Closed += (s, e) =>
            {
                window.Close();
                running = false;
            };

            var timer = new System.Diagnostics.Stopwatch();
            timer.Start();

            while (running)
            {
                _cpu.Step();

                if (timer.ElapsedMilliseconds > (1 / 60))
                {
                    if (_memory.GetRam(_cpu.Registers.PC - 1) == 0x0100 && _memory.GetRam(_cpu.Registers.PC) == 0)
                        running = false;

                    display.Handle(window);

                    timer.Restart();
                }
            }

            Assert.IsTrue(_memory.Vram.GetScreen().All(s => s == 0xFF00));
        }
    }
}
