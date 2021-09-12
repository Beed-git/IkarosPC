using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkarosPC
{
    public class Memory
    {
        private ushort[] _memory;
        private ushort[] _stack;
        private Vram _vram;

        // Not the biggest fan of this.
        public ushort[] Stack => _stack;
        public Vram Vram => _vram;

        /// <summary>
        /// ~65kb regular memory
        /// ~49kb stack memory
        /// </summary>
        public Memory()
        {
            _memory = new ushort[0x10000];
            _stack = new ushort[0xC000];

            _vram = new Vram();
        }

        public ushort this[ushort i]
        {
            get
            {
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
            set => _memory[i] = value;
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
