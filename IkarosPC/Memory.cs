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

        public ushort TopOfStack => 0xFFFE;

        /// <summary>
        /// ~65kb regular memory
        /// ~49kb stack memory
        /// </summary>
        public Memory()
        {
            _memory = new ushort[0x10000];
            _stack = new ushort[0xC000];
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

                return _memory[i];
            }
            set => _memory[i] = value;
        }

        // Not the biggest fan of this.
        public ushort[] Stack => _stack;

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
