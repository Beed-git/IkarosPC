using SFML.Graphics;
using SFML.Window;
using System;
using System.Diagnostics;

namespace IkarosPC
{
    class Program
    {
        static Memory _memory;
        static Registers _registers;

        static CPU _cpu;

        static GPU _gpu;

        static void Main(string[] args)
        {
            _registers = new Registers();

            _memory = new Memory(_registers);
            _cpu = new CPU(_registers, _memory);

            _gpu = new GPU(_registers, _memory);

            // Debug.
            _registers.RSC = 2;

            _memory[0] = 1;
            _memory[1] = 1;
            _memory[2] = 1;
            _memory[3] = 1;
            _memory[32] = 1;
            _memory[33] = 2;
            _memory[35] = 1;

            // X offset
            // -2
            _memory[0xFF00] = 14;
            // Y offset
            _memory[0xFF01] = 4;

            ushort tile = 0x2000;

            for (int i = 0; i < 16; i++)
                for (int y = 0; y < 16; y++)
                {
                    _memory[(ushort)(tile + i + y * 16)] = (ushort)(0xF00F + i * 0x100 + y * 0x10);
                    _memory[(ushort)(tile + 0x100 + i + y * 16)] = 0x7B3F;
                }

            _memory[(ushort)(tile + 0x100 + 3)] = 0x362F;
            _memory[(ushort)(tile + 0x100 + 16 * 1)] = 0x362F;
            _memory[(ushort)(tile + 0x100 + 3 + 16 * 1)] = 0x362F;
            _memory[(ushort)(tile + 0x100 + 1 + 16 * 2)] = 0x362F;
            _memory[(ushort)(tile + 0x100 + 3 + 16 * 2)] = 0x362F;
            _memory[(ushort)(tile + 0x100 + 13 + 16 * 4)] = 0x362F;
            _memory[(ushort)(tile + 0x100 + 13 + 16 * 5)] = 0x362F;
            _memory[(ushort)(tile + 0x100 + 15 + 16 * 5)] = 0x362F;
            _memory[(ushort)(tile + 0x100 + 4 + 16 * 8)] = 0x362F;
            _memory[(ushort)(tile + 0x100 + 2 + 16 * 9)] = 0x362F;
            _memory[(ushort)(tile + 0x100 + 4 + 16 * 9)] = 0x362F;
            _memory[(ushort)(tile + 0x100 + 9 + 16 * 13)] = 0x362F;
            _memory[(ushort)(tile + 0x100 + 7 + 16 * 14)] = 0x362F;
            _memory[(ushort)(tile + 0x100 + 9 + 16 * 14)] = 0x362F;
            _memory[(ushort)(tile + 0x100 + 12 + 16 * 14)] = 0x362F;
            _memory[(ushort)(tile + 0x100 + 7 + 16 * 15)] = 0x362F;
            _memory[(ushort)(tile + 0x100 + 9 + 16 * 15)] = 0x362F;
            _memory[(ushort)(tile + 0x100 + 11 + 16 * 15)] = 0x362F;

            _registers.RSC = 0;

            // END DEBUG CODE

            var screenX = _memory.ScreenX;
            var screenY = _memory.ScreenY;

            var display = new Display(_gpu, screenX, screenY);

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

                if (timer.ElapsedMilliseconds > 1 / 60 * 1000)
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
