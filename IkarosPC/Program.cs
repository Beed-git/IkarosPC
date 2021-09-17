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
        static Registers _registers;

        static void Main(string[] args)
        {
            _registers = new Registers();

            _memory = new Memory(_registers);
            _cpu = new CPU(_memory, _registers);

            var display = new Display(_memory.Vram);

            var screenX = (uint)_memory.Vram.ScreenX;
            var screenY = (uint)_memory.Vram.ScreenY;

            var window = new RenderWindow(new VideoMode(screenX * 4, screenY * 4), "Ikaros");

            // Letter box the window with a view.
            var view = new View(new FloatRect(0, 0, screenX, screenY));
            view.Center = new(view.Size.X / 2, view.Size.Y / 2);
            view = CreateLetterBoxView(view, screenX, screenY);

            var running = true;

            // Window events.
            window.Closed += (s, e) =>
            {
                window.Close();
                running = false;
            };

            window.Resized += (s, e) =>
            {
                view = CreateLetterBoxView(view, e.Width, e.Height);
            };

            var timer = new Stopwatch();
            timer.Start();

            while (running)
            {

                _cpu.Step();

                if (timer.ElapsedMilliseconds > (1 / 60))
                {
                    window.SetView(view);
                    display.Handle(window);

                    timer.Restart();
                }
            }
        }

        /// <summary>
        /// Create a letterboxed view from a regular view.
        /// Code switched to C# from: https://github.com/SFML/SFML/wiki/Source:-Letterbox-effect-using-a-view
        /// </summary>
        /// <param name="view">The view to transform.</param>
        /// <param name="windowWidth">The new width of the window.</param>
        /// <param name="windowHeight">The new height of the window.</param>
        /// <returns>The transformed window.</returns>
        static View CreateLetterBoxView(View view, uint windowWidth, uint windowHeight)
        {
            float windowRatio = windowWidth / (float)windowHeight;
            float viewRatio = view.Size.X / (float)view.Size.Y;

            float sizeX = 1;
            float sizeY = 1;
            float posX = 0;
            float posY = 0;

            if (windowRatio >= viewRatio)
            {
                sizeX = viewRatio / windowRatio;
                posX = (1 - sizeX) / 2.0f;
            }
            else
            {
                sizeY = windowRatio / viewRatio;
                posY = (1 - sizeY) / 2.0f;
            }

            view.Viewport = new(posX, posY, sizeX, sizeY);

            return view;
        }
    }
}
