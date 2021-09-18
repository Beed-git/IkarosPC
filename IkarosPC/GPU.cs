using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkarosPC
{
    public class GPU
    {
        private readonly Registers _registers;
        private readonly Memory _memory;

        public GPU(Registers registers, Memory memory)
        {
            _registers = registers;
            _memory = memory;
        }

        public ushort[] BuildDisplay()
        {
            // Screen is built in seperate stages.
            // First the background tilemap is built.
            // Next is the optional secondary tilemap.
            // After this is the sprite layer which is
            // followed by the foreground layer.

            // Standard tile size is 16x16.

            // Sprites are represented in two places: 
            // First is the SpriteObjectInformation which stores info such as
            // (x pos, y pos), (x scale, y scale), (options) flags (described below), and the 
            // pointer to the sprite graphics.
            // Second is in normal VRAM space, which starts with a word telling 
            // the size of the sprite, then two bytes for the x and y size.
            // This is then followed by the sprite itself.

            // Sprite flags:
            // bit 0: x flip
            // bit 1: y flip
            // bit 2 & 3: sprite layer (0 - behind background, 1 - in between background and secondary,
            // 2 - sprite layer, 3 - on top of foreground layer.)


            // VRAM map:
            // 0 -> 0x100        - ushorts corresponding to the tile number for the first tilemap.
            // 0x100 -> 0x200 is reserved.
            // 0x200 -> 0x300    - ushorts corresponding to the tile number for the second tilemap.
            // 0x300 -> 0x400 is reserved.
            // 0x400 -> 0x500    - ushorts corresponding to the tile number for the foreground tilemap.
            // 0x500 -> 0x600 is reserved.
            // 0x600 -> 0x1000   - ushorts corresponding to the sprite object info.

            // 0x1000 -> 0x8000  - Reserved for 16x16 tiles for a total of 70 tiles loaded at once.
            // 0x8000 -> 0xF000  - Reserved for sprites.
            // (0x8000 should always be the pointer to the next sprite.)

            // 0xF000 - 0xFFFF - Reserved for vram registers.
            // 0xF000 - Global Tilemap Offset (first byte = x offset, second byte = y offset.)
            // 0xF001 - First tilemap offset
            // 0xF002 - Second tilemap offset
            // 0xF003 - Foreground tilemap offset
            var saveRSC = _registers.RSC;

            var screen = new ushort[_memory.ScreenX * _memory.ScreenY];

            // Last step, add display to top of screen.

            // Mix transparancy calculation.
            // https://newbedev.com/how-to-mix-two-argb-pixels

            // Return built screen.
            _registers.RSC = saveRSC;
            return screen;
        }
    }
}
