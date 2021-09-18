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
        private readonly Registers _registers;
        private readonly Memory _memory;

        private Image _image;
        private Texture _texture;
        private Sprite _sprite;

        public Display(Registers registers, Memory memory)
        {
            _registers = registers;
            _memory = memory;
        }

        public void Handle(RenderWindow window)
        {
            // Dispatch all events
            window.DispatchEvents();

            window.Clear(Color.Black);

            _image = new Image(_memory.ScreenX, _memory.ScreenY, ConvertScreenToSFML());

            _texture = new Texture(_image, new IntRect(0, 0, (int)_memory.ScreenX, (int)_memory.ScreenY));

            _sprite = new Sprite(_texture);

            window.Draw(_sprite);

            window.Display();
        }

        public byte[] ConvertScreenToSFML()
        {
            var saveRSC = _registers.RSC;
            _registers.RSC = 3;

            var screenSize = _memory.ScreenX * _memory.ScreenY;

            var pixels = new byte[_memory.ScreenX * _memory.ScreenY * 4];

            for (ushort i = 0; i < screenSize; i++)
            {
                var pixel = _memory[i];

                var red = (byte)((pixel & 0xF000) >> 12);
                var green = (byte)((pixel & 0x0F00) >> 8);
                var blue = (byte)((pixel & 0x00F0) >> 4);

                pixels[i * 4] = (byte)(red * (255 / 0xF));
                pixels[i * 4 + 1] = (byte)(green * (255 / 0xF));
                pixels[i * 4 + 2] = (byte)(blue * (255 / 0xF));
                pixels[i * 4 + 3] = 255;
            }

            _registers.RSC = saveRSC;

            return pixels;
        }
    }
}
