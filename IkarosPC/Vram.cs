using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkarosPC
{
    class Vram
    {
        private int _screenX = 160;
        private int _screenY = 144;

        private ushort[] _vram;

        public int ScreenX => _screenX;
        public int ScreenY => _screenY;

        /// <summary>
        /// Vram has a 160x144 pixel screen, with 1 16-bit value per pixel. 
        /// Each 16-bit value is split into RRRRGGGGBBBB----
        /// First 23040 ushorts corrosponds to the screen output.
        /// </summary>
        public Vram()
        {
            _vram = new ushort[65536];

            // Fill with random
            var rand = new Random();

            for (int i = 0; i < _screenX * _screenY; i++)
            {
                _vram[i] = (ushort)i;
            }
        }

        public ushort this[ushort i]
        {
            get => _vram[i];
            set => _vram[i] = value;
        }

        public ushort[] GetScreen()
        {
            return _vram.Take(_screenX * _screenY).ToArray();
        }
    }
}
