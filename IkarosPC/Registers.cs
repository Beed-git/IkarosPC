using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkarosPC
{
    public class Registers
    {
        private ushort[] _registers;

        public Registers()
        {
            _registers = new ushort[10];
        }

        public void Reset()
        {
            // Clear registers
            Array.Fill<ushort>(_registers, 0);

            Zero = false;
            Carry = false;
            Negative = false;
        }

        // Get register by value
        public ushort this[byte i]
        {
            get
            {
                if (i > _registers.Length)
                    throw new IndexOutOfRangeException($"Register at {i} does not exist.");
                
                return _registers[i];
            }
            set
            {
                if (i > _registers.Length)
                    throw new IndexOutOfRangeException($"Register at {i} does not exist.");

                _registers[i] = value;
            }
        }

        // Special Registers

        // Accumulator - Most math functions put result in this register.
        public ushort Accumulator
        {
            get => _registers[8];
            set => _registers[8] = value;
        }

        public ushort PC
        {
            get => _registers[9];
            set => _registers[9] = value;
        }

        /// <summary>
        /// Bit map
        /// 0000 0ZCN
        /// </summary>
        /// 
        public ushort Flags
        {
            get
            {
                int u = 0;

                u += Negative ? 0b0001 : 0;
                u += Negative ? 0b0010 : 0;
                u += Negative ? 0b0100 : 0;

                return (ushort)u;
            }
            set
            {
                Zero = (value & 0b0100) > 0;
                Carry = (value & 0b0010) > 0;
                Negative = (value & 0b0001) > 0;
            }
        }

        // Flags
        public bool Zero { get; set; }
        public bool Carry { get; set; }
        public bool Negative { get; set; }

        // General purpose registers
        public ushort A
        {
            get => _registers[0];
            set => _registers[0] = value;
        }
        public ushort B
        {
            get => _registers[1];
            set => _registers[1] = value;
        }
        public ushort C
        {
            get => _registers[2];
            set => _registers[2] = value;
        }
        public ushort D
        {
            get => _registers[3];
            set => _registers[3] = value;
        }
        public ushort E
        {
            get => _registers[4];
            set => _registers[4] = value;
        }
        public ushort X
        {
            get => _registers[5];
            set => _registers[5] = value;
        }
        public ushort Y
        {
            get => _registers[6];
            set => _registers[6] = value;
        }
        public ushort Z
        {
            get => _registers[7];
            set => _registers[7] = value;
        }
    }
}
