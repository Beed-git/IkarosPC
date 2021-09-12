using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkarosPC
{
    class Display
    {
        private Vram _vram;

        private Image _image;
        private Texture _texture;
        private Sprite _sprite;

        public Display(Vram vram)
        {
            _vram = vram;
        }

        public void Handle(RenderWindow window)
        {
            // Dispatch all events
            window.DispatchEvents();

            // If you see blue on screen, somethings gone wrong.
            window.Clear(Color.Blue);

            _image = new Image((uint)_vram.ScreenX, (uint)_vram.ScreenY, ConvertScreenToSFML());

            _texture = new Texture(_image, new IntRect(0, 0, _vram.ScreenX, _vram.ScreenY));

            _sprite = new Sprite(_texture);

            window.Draw(_sprite);

            window.Display();
        }

        public byte[] ConvertScreenToSFML()
        {
            var screen = _vram.GetScreen();
            var pixels = new byte[_vram.ScreenX * _vram.ScreenY * 4];

            for (int i = 0; i < screen.Length; i++)
            {
                var pixel = screen[i];

                var red = (byte)((pixel & 0xF000) >> 12);
                var green = (byte)((pixel & 0x0F00) >> 8);
                var blue = (byte)((pixel & 0x00F0) >> 4);

                pixels[i * 4] = (byte)(red * (255 / 0xF));
                pixels[i * 4 + 1] = (byte)(green * (255 / 0xF));
                pixels[i * 4 + 2] = (byte)(blue * (255 / 0xF));
                pixels[i * 4 + 3] = 255;
            }

            return pixels;
        }
    }
}
