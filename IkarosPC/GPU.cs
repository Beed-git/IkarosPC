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

        ///// <summary>
        ///// The top left X coordinate from which the screen starts rendering.
        ///// Assumes the program is already in VRAM.
        ///// </summary>
        //private ushort GlobalTilemapXOffset
        //{
        //    get => _memory[0xFF00];
        //    set => _memory[0xFF00] = value;
        //}

        ///// <summary>
        ///// The top left Y coordinate from which the screen starts rendering.
        ///// Assumes the program is already in VRAM.
        ///// </summary>
        //private ushort GlobalTilemapYOffset
        //{
        //    get => _memory[0xFF01];
        //    set => _memory[0xFF01] = value;
        //}

        //public ushort[] BuildDisplay()
        //{
        //    // Screen is built in seperate stages.
        //    // First the background tilemap is built.
        //    // Next is the optional secondary tilemap.
        //    // After this is the sprite layer which is
        //    // followed by the foreground layer.

        //    // Standard tile size is 16x16.

        //    // Sprites are represented in two places: 
        //    // First is the SpriteObjectInformation which stores info such as
        //    // (x pos, y pos), (x scale, y scale), (options) flags (described below), and the 
        //    // pointer to the sprite graphics.
        //    // Second is in normal VRAM space, which starts with a word telling 
        //    // the size of the sprite, then two bytes for the x and y size.
        //    // This is then followed by the sprite itself.

        //    // Sprite flags:
        //    // bit 0: x flip
        //    // bit 1: y flip
        //    // bit 2 & 3: sprite layer (0 - behind background, 1 - in between background and secondary,
        //    // 2 - sprite layer, 3 - on top of foreground layer.)


        //    // VRAM map:
        //    // 0 -> 0x400        - ushorts corresponding to the tile number for the first tilemap.
        //    // 0x400 -> 0x500 is reserved.
        //    // 0x500 -> 0x900    - ushorts corresponding to the tile number for the second tilemap.
        //    // 0x900 -> 0x1000 is reserved.
        //    // 0x1000 -> 0x1400    - ushorts corresponding to the tile number for the foreground tilemap.
        //    // 0x1400 -> 0x1500 is reserved.
        //    // 0x1500 -> 0x2000   - ushorts corresponding to the sprite object info.

        //    // 0x2000 -> 0x9000  - Reserved for 16x16 tiles for a total of 70 tiles loaded at once.
        //    // 0x9000 -> 0xF000  - Reserved for sprites.
        //    // (0x9000 should always be the pointer to the next sprite.)

        //    // 0xFF00 - 0xFFFF - Reserved for vram registers.
        //    // 0xFF00 - Global Tilemap X Offset
        //    // 0xFF01 - Global Tilemap Y Offset
        //    // 0xFF02 - Enable Global Tilemap wrapping.
        //    //              If enabled, when going offscreen the tilemap will wrap around to the otherside.
        //    //              If disabled, when going offscreen the tilemap will extend the previous column/row.

        //    var saveRSC = _registers.RSC;

        //    var tileMapRam = 0x2000;
        //    var tileByteSize = 0x100;

        //    var screenX = _memory.ScreenX;
        //    var screenY = _memory.ScreenY;

        //    var screen = new ushort[screenX * screenY];

        //    _registers.RSC = 2;

        //    // TODO: 
        //    // First set of sprites here.

        //    var tileMapSize = 32;
        //    var tileSize = 16;

        //    var backgroundTiles = new ushort[tileMapSize * tileMapSize];
        //    var foregroundTiles = new ushort[tileMapSize * tileMapSize];

        //    // Build first tilesets.
        //    for (ushort i = 0; i < 256; i++)
        //    {
        //        backgroundTiles[i] = _memory[i];
        //        foregroundTiles[i] = _memory[(ushort)(0x500 + i)];
        //    }

        //    // Display tiles
        //    for (ushort y = 0; y < screenY; y++)
        //    {
        //        for (ushort x = 0; x < screenX; x++)
        //        {
        //            // TODO: 
        //            // Fix offset.
        //            // Tilemap needs to wrap around or continue extending the last tile if 
        //            // tileAddress < 0 || tileAddress > tileMapSize.
                    
        //            // Get address of tile.
        //            var tileX = x / tileSize;
        //            var tileY = y / tileSize * tileMapSize;

        //            var tileXOffset = GlobalTilemapXOffset / tileSize;
        //            var tileYOffset = GlobalTilemapYOffset / tileSize * tileMapSize;

        //            var tileAddress = (ushort)(tileX + tileY +
        //                                       tileXOffset + tileYOffset);

        //            // Start drawing from background tile.
        //            // Get background tile.
        //            var backgroundTile = _memory[tileAddress];

        //            var backgroundTileStart = (ushort)(tileMapRam + backgroundTile * tileByteSize);

        //            //var _ = (ushort)(backgroundTileStart
        //            //                + (GlobalTilemapXOffset + x) % tileSize
        //            //                + (GlobalTilemapYOffset + y) % tileSize)

        //            var localTilePos = x % tileSize + y % tileSize * tileSize;

        //            var tilePixel = (ushort)(backgroundTileStart + localTilePos);

        //            var tilePos = x + y * screenX;
        //                //- GlobalTilemapXOffset % tileSize
        //                //- (GlobalTilemapYOffset % tileSize * screenX);

        //            if (tilePos >= 0 && tilePos < screen.Length)
        //                screen[tilePos] = _memory[tilePixel];
        //        }
        //    }

        //    // Last step, add display to top of screen.

        //    // Mix transparancy calculation.
        //    // https://newbedev.com/how-to-mix-two-argb-pixels

        //    // Return built screen.
        //    _registers.RSC = saveRSC;
        //    return screen;
        //}
    }
}
