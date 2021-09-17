using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkarosPC
{
    public class Memory
    {
        // Cpus registers.
        private readonly Registers _registers;

        private readonly ushort[] _memory;
        private readonly ushort[] _stack;
        private readonly ushort[,] _bank;
        private readonly Vram _vram;

        // Not the biggest fan of this.
        public ushort GetRam(int index)
        {
            // Return banked memory.
            if (index >= 0xB000 && index < 0xF000)
            {
                if (_registers.MBC == 0)
                    return _memory[index];

                var address = index - 0xB000;
                var mbc = (_registers.MBC - 1) % _bank.GetLength(0);

                return _bank[mbc, address];
            }

            return _memory[index];
        }

        public void SetRam(ushort value, int index)
        {
            // Set banked memory.
            if (index >= 0xB000 && index < 0xF000)
            {
                if (_registers.MBC == 0)
                {
                    _memory[index] = value;
                    return;
                }

                var address = index - 0xB000;
                var mbc = (_registers.MBC - 1) % _bank.GetLength(0);

                _bank[mbc, address] = value;
                return;
            }

            _memory[index] = value;
        }

        public ushort[] Stack => _stack;
        public Vram Vram => _vram;

        /// <summary>
        /// ~65kb regular memory
        /// ~49kb stack memory
        /// ~8.39mb banked memory
        /// </summary>
        public Memory(Registers registers)
        {
            _registers = registers;

            _memory = new ushort[0x10000];
            _stack = new ushort[0xC000];
            // 255 banks of 0x4000 memory for a total of ~8.39mb of switchable storage. (Including 0th bank - none.)
            _bank = new ushort[0xFF, 0x4000];

            _vram = new Vram();
        }

        public void Reset()
        {
            _memory[0xFFFF] = 0;
        }

        public ushort this[ushort i]
        {
            get
            {
                // Return banked memory.
                if (i >= 0xB000 && i < 0xF000)
                {
                    if (_registers.MBC == 0)
                        return _memory[i];

                    var address = i - 0xB000;
                    var mbc = (_registers.MBC - 1) % _bank.GetLength(0);

                    return _bank[mbc, address];
                }

                // 0xFFFF will always return from the 0xFFFF register (memory control switch, will change between 'cartridges', regular ram, vram, and stack ram.
                if (i == 0xFFFF)
                {
                    return _memory[i];
                }

                switch (_memory[0xFFFF])
                {
                    case 0x0: return _memory[i];
                    case 0x1: return _stack[i];
                    case 0x2: return _vram[i];
                    default: throw new ArgumentOutOfRangeException("Memory at this address does not exist.");
                }
            }
            set
            {
                if (i == 0xFFFF)
                {
                    _memory[i] = value;
                }

                // Set banked memory.
                if (i >= 0xB000 && i < 0xF000)
                {
                    if (_registers.MBC == 0)
                    {
                        _memory[i] = value;
                        return;
                    }

                    var address = i - 0xB000;
                    var mbc = (_registers.MBC - 1) % _bank.GetLength(0);

                    _bank[mbc, address] = value;
                    return;
                }

                switch (_memory[0xFFFF])
                {
                    case 0x0: _memory[i] = value; break;
                    case 0x1: _stack[i] = value; break;
                    case 0x2: _vram[i] = value; break;
                    default: throw new ArgumentOutOfRangeException("Memory at this address does not exist.");
                }
            }
        }

        /// <summary>
        /// Fills memory from the 0th byte to the length of data.
        /// </summary>
        /// <param name="data">The set of opcodes to put in the initial memory.</param>
        public void SetInitialMemory(ushort[] data)
        {
            for (ushort i = 0; i < data.Length; i++)
            {
                _memory[i] = data[i];
            }
        }

        public void SetSubroutineAtAddress(ushort offset, ushort[] data)
        {
            for (ushort i = 0; i < data.Length; i++)
            {
                _memory[i + offset] = data[i];
            }
        }
    }
}
