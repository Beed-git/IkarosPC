using SFML.Graphics;
using SFML.Window;
using System;
using System.Diagnostics;

namespace IkarosPC
{
    class Program
    {
        static void Main(string[] args)
        {
            var vram = new Vram();
            var display = new Display(vram);

            var window = new RenderWindow(new VideoMode((uint)vram.ScreenX, (uint)vram.ScreenY), "Ikaros");

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
