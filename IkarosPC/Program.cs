using SFML.Graphics;
using SFML.Window;
using System;
using System.Diagnostics;

namespace IkarosPC
{
    class Program
    {
        static CPU _cpu;
        static Memory _memory;

        static void Main(string[] args)
        {
            _memory = new Memory();
            _cpu = new CPU(_memory);
            
            var display = new Display(_memory.Vram);

            var window = new RenderWindow(new VideoMode((uint)_memory.Vram.ScreenX, (uint)_memory.Vram.ScreenY), "Ikaros");

            var running = true;

            window.Closed += (s, e) =>
            {
                window.Close();
                running = false;
            };

            var timer = new Stopwatch();
            timer.Start();

            while (running)
            {
                if (timer.ElapsedMilliseconds > (1 / 60))
                {
                    display.Handle(window);

                    timer.Restart();
                }
            }
        }
    }
}
