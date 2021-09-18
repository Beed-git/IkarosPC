using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkarosPC
{
    public class Display
    {
        private readonly GPU _gpu;

        private readonly uint _screenX;
        private readonly uint _screenY;

        private Image _image;
        private Texture _texture;
        private Sprite _sprite;

        public Display(GPU gpu, uint screenX, uint screenY)
        {
            _gpu = gpu;

            _screenX = screenX;
            _screenY = screenY;
        }

        public void Handle(RenderWindow window)
        {
            // Dispatch all events
            window.DispatchEvents();

            window.Clear(Color.Black);

            _image = new Image(_screenX, _screenY, ConvertScreenToSFML());

            _texture = new Texture(_image, new IntRect(0, 0, (int)_screenX, (int)_screenY));

            _sprite = new Sprite(_texture);

            window.Draw(_sprite);

            window.Display();
        }

        public byte[] ConvertScreenToSFML()
        {
            var screen = _gpu.BuildDisplay();

            var pixels = new byte[_screenX * _screenY * 4];

            for (ushort i = 0; i < screen.Length; i++)
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
