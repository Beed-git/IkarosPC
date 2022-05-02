using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace IkarosPC;

internal class EmuWindow
{
    private CPU _cpu;

    private RenderWindow _window;
    private Stopwatch _frameTimer;
    private Stopwatch _timer;

    private const int FPS = 60;
    private const int CLOCKS_PER_FRAME = CPU.CLOCKS_PER_SECOND / FPS;
    private const float FRAME_TIME_MS = 1.0f / FPS * 1000;

    public EmuWindow()
    {
    }

    public void Start()
    {
        var reg = new Registers();
        var mem = new Memory(reg);

        _cpu = new CPU(reg, mem);

        _window = new RenderWindow(new VideoMode(800, 600), "IkarosPC");
        _frameTimer = new Stopwatch();
        _timer = new Stopwatch();

        _window.Closed += (s, e) =>
        {
            _window.Close();
        };

        _window.KeyPressed += Window_KeyPressed;

        _frameTimer.Start();
        _timer.Start();

        bool hasSimulatedFrame = false;

        while (_window.IsOpen)
        {
            if (!hasSimulatedFrame)
            {
                for (int i = 0; i < CLOCKS_PER_FRAME; i++)
                {
                    _cpu.Step();
                }
                hasSimulatedFrame = true;
            }

            if (hasSimulatedFrame && _frameTimer.ElapsedMilliseconds > FRAME_TIME_MS)
            {
                hasSimulatedFrame = false;

                _frameTimer.Restart();

                _window.DispatchEvents();

                _window.Clear(new Color(150, 20, 40));
                _window.Display();
            }
        }
    }

    private void Window_KeyPressed(object sender, KeyEventArgs e)
    {
        if (e.Code == Keyboard.Key.Escape)
        {
            _window.Close();
        }
    }
}
