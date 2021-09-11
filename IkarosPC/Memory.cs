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

        public Memory()
        {
            _memory = new ushort[65536];
        }

        public ushort this[ushort i]
        {
            get => _memory[i];
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
