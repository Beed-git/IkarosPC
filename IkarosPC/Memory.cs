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

        public ushort this[ushort i]
        {
            get
            {
                switch (_registers.RSC)
                {
                    case 0:
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

                            return _memory[i];
                        }
                    case 1:
                        {
                            return _stack[i];
                        }
                    case 2:
                        {
                            return _vram[i];
                        }
                    default: throw new ArgumentOutOfRangeException($"Attempted to access invalid memory at ${ _registers.MBC }.");
                }
            }
            set
            {
                switch (_registers.RSC)
                {
                    case 0:
                        {
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

                            _memory[i] = value;
                        }
                        return;
                    case 1:
                        {
                            _stack[i] = value;
                        }
                        return;
                    case 2:
                        {
                            _vram[i] = value;
                        }
                        return; 
                    default: throw new ArgumentOutOfRangeException($"Attempted to access invalid memory at ${ _registers.MBC }.");
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
