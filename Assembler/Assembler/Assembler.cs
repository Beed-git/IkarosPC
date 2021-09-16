using IkarosAssembler.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IkarosAssembler.Data;

namespace IkarosAssembler.Assemble
{
    public static class Assembler
    {
        static Dictionary<string, int> _addressLookup;

        public static byte[] Assemble(List<Instruction> instructions)
        {
            _addressLookup = new();

            int count = 0;

            foreach (var insn in instructions)
            {
                if (insn.InstructionType == "LABEL")
                {
                    _addressLookup.Add(insn.InstructionArguments[0].Value, count);
                    continue;
                }

                var instructionOpcode = GetInstructionOpcodes.Where(i => i.Instruction == insn.InstructionType).First();

                count += instructionOpcode.Size;
            }

            List<byte> data = new();

            foreach (var insn in instructions)
            {
                if (insn.InstructionType == "LABEL")
                    continue;

                var instructionOpcode = GetInstructionOpcodes.Where(i => i.Instruction == insn.InstructionType).First();

                // Build opcode.
                var arr = BuildOpcode(instructionOpcode, insn.InstructionArguments);

                foreach (var opcode in arr)
                {
                    data.Add(opcode);
                }
            }

            return data.ToArray();
        }

        private static byte[] BuildOpcode(InstructionOpcode instruction, InstructionArgument[] arguments)
        {
            byte opcodeStart = instruction.Opcode;

            List<byte> words = new();

            if (arguments is null)
            {
                return new byte[] { opcodeStart, 0x00 };
            }

            var offset = 4;
            byte registers = 0;

            foreach (var arg in arguments)
            {
                if (offset < 0)
                    throw new Exception("Too many register arguments.");

                if (arg.Type is ArgType.Register or ArgType.RegisterPointer)
                {
                    var registerValue = Array.IndexOf(GetRegisters, arg.Value);

                    registers |= (byte)(registerValue << offset);

                    offset -= 4;
                }
                else if (arg.Type is ArgType.Immediate or ArgType.ImmediatePointer)
                {
                    ushort full;

                    if (arg.Value.EndsWith(":"))
                    {
                        full = (ushort)_addressLookup[arg.Value];
                    }
                    else
                    {
                        full = ushort.Parse(arg.Value, System.Globalization.NumberStyles.HexNumber);
                    }

                    var upper = (byte)((full & 0xFF00) >> 8);
                    var lower = (byte)(full & 0x00FF);

                    words.Add(upper);
                    words.Add(lower);
                }
            }

            var array = new byte[words.Count() + 2];

            array[0] = opcodeStart;
            array[1] = registers;
            words.CopyTo(array, 2);

            return array;
        }
    }
}
