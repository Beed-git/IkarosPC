using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkarosAssembler
{
    public class Data
    {
        public static string[] GetAssemblerKeywords => _assemblerKeywords;
        public static string[] GetInstructionKeywords => _instructionKeywords;
        public static string[] GetRegisters => _registers;
        public static InstructionOpcode[] GetInstructionOpcodes => _instructionOpcodes;

        public static string[] GetAllKeywords => _assemblerKeywords.Concat(_instructionKeywords).ToArray();

        private static readonly string[] _assemblerKeywords = new string[]
        {
            // Special assembler keywords
            "DEF", "SIZEOF"
        };

        private static readonly string[] _instructionKeywords = new string[]
        {
            // Regular keywords
            "NOP", "STOP", "PUSH", "POP", "CALL", "RET",
            "MOV",
            "ADD", "SUB", "INC", "DEC",
            "MUL", "DIV", "MOD",
            "AND", "OR", "XOR", "NOT",
            "LLS", "LRS", "ROL", "ROR", "RLC", "RRC",
            "JMP", "JEQ", "JNEQZ", "JF", "JNF",
            "JG", "JGZ", "JL", "JLZ", "JGE", "JGEZ", "JLE", "JLEZ"
        };

        private static readonly string[] _instructionFormats = new string[]
        {
            "REG",
            "IMM",
            "PTR_REG",
            "PTR_IMM",
            "REGOFF",
            "FLAG"
        };

        private static readonly string[] _registers = new string[]
        {
            // General registers.
            "$A", "$B", "$C", "$D", "$E", "$X", "$Y", "$Z",
            // Special registers.
            "$ACC", "F",
            "$PC", "$SP", "$FP"
        };

        public record InstructionOpcode(string Instruction, byte Opcode, int Size);

        private static readonly InstructionOpcode[] _instructionOpcodes = new InstructionOpcode[]
        {
            // Control
            new("NOP", 0x00, 1),
            new("STOP", 0x01, 1),
            new("PUSH_REG", 0x02, 1),
            new("PUSH_IMM", 0x03, 2),
            new("POP_REG", 0x04, 1),
            new("CALL_REG", 0x05, 1),
            new("CALL_IMM", 0x06, 2),
            new("RET", 0x07, 1),
            // Move instructions.
            new("MOV_REG_REG", 0x10, 1),
            new("MOV_IMM_REG", 0x11, 2),
            new("MOV_PTR_REG_REG", 0x12, 1),
            new("MOV_PTR_IMM_REG", 0x13, 2),
            new("MOV_REG_PTR_REG", 0x14, 1),
            new("MOV_IMM_PTR_REG", 0x15, 2),
            new("MOV_REG_PTR_IMM", 0x16, 2),
            new("MOV_IMM_PTR_IMM", 0x17, 3),
            new("MOV_PTR_REG_PTR_REG", 0x18, 1),
            new("MOV_PTR_REG_PTR_IMM", 0x19, 2),
            new("MOV_PTR_IMM_PTR_REG", 0x1A, 2),
            new("MOV_PTR_IMM_PTR_IMM", 0x1B, 3),

            new("MOV_REG_IMM_REGOFF", 0x1C, 2),
            new("MOV_IMM_REGOFF_REG", 0x1D, 2),
            // General arithmetic.
            new("ADD_REG_REG", 0x20, 1),
            new("ADD_REG_IMM", 0x21, 2),
            new("ADD_IMM_REG", 0x21, 2),
            new("SUB_REG_REG", 0x22, 1),
            new("SUB_REG_IMM", 0x23, 2),
            new("SUB_IMM_REG", 0x24, 2),
            new("INC_REG", 0x25, 1),
            new("DEC_REG", 0x26, 1),
            // Unsigned arithmetic.
            new("MUL_REG_REG", 0x30, 1),
            new("MUL_REG_IMM", 0x31, 2),
            new("MUL_IMM_REG", 0x31, 2),
            new("DIV_REG_REG", 0x32, 1),
            new("DIV_REG_IMM", 0x33, 2),
            new("DIV_IMM_REG", 0x34, 2),
            new("MOD_REG_REG", 0x35, 1),
            new("MOD_REG_IMM", 0x36, 2),
            new("MOD_IMM_REG", 0x37, 2),
            // Signed arithmetic.
            new("SMUL_REG_REG", 0x40, 1),
            new("SMUL_REG_IMM", 0x41, 2),
            new("SMUL_IMM_REG", 0x41, 2),
            new("SDIV_REG_REG", 0x42, 1),
            new("SDIV_REG_IMM", 0x43, 2),
            new("SDIV_IMM_REG", 0x44, 2),
            new("SMOD_REG_REG", 0x45, 1),
            new("SMOD_REG_IMM", 0x46, 2),
            new("SMOD_IMM_REG", 0x47, 2),
            // Bitmath
            new("AND_REG_REG", 0x50, 1),
            new("AND_REG_IMM", 0x51, 2),
            new("AND_IMM_REG", 0x51, 2),
            new("OR_REG_REG", 0x52, 1),
            new("OR_REG_IMM", 0x53, 2),
            new("OR_IMM_REG", 0x53, 2),
            new("XOR_REG_REG", 0x54, 1),
            new("XOR_REG_IMM", 0x55, 2),
            new("XOR_IMM_REG", 0x55, 2),
            new("NOT_REG", 0x56, 1),
            new("LLS_REG_REG", 0x57, 1),
            new("LLS_REG_IMM", 0x58, 2),
            new("LRS_REG_REG", 0x59, 1),
            new("LRS_REG_IMM", 0x5A, 2),
            new("ROL_REG_REG", 0x5B, 1),
            new("ROL_REG_IMM", 0x5C, 2),
            new("ROR_REG_REG", 0x5D, 1),
            new("ROR_REG_IMM", 0x5E, 2),
            new("RLC_REG_REG", 0x5F, 1),
            new("RLC_REG_IMM", 0x60, 2),
            new("RRC_REG_REG", 0x61, 1),
            new("RRC_REG_IMM", 0x62, 2),
            // Jump
            new("JMP_REG", 0x70, 1),
            new("JMP_IMM", 0x71, 2),
            new("JMP_IMM_REGOFF", 0x72, 2),
            new("JEQ_REG_REG_IMM", 0x73, 2),
            new("JEQZ_REG_PTR_REG", 0x74, 1),
            new("JEQZ_REG_PTR_IMM", 0x75, 2),
            new("JNEQ_REG_REG_IMM", 0x76, 2),
            new("JNEQZ_REG_PTR_REG", 0x77, 1),
            new("JNEQZ_REG_PTR_IMM", 0x78, 2),
            new("JF_FLAG_PTR_REG", 0x79, 1),
            new("JF_FLAG_PTR_IMM", 0x7A, 2),
            new("JNF_FLAG_PTR_REG", 0x7B, 1),
            new("JNF_FLAG_PTR_IMM", 0x7C, 2),
            // Jump extended.
            new("JG_REG_REG_IMM", 0x80, 2),
            new("JGZ_REG_PTR_REG", 0x81, 1),
            new("JGZ_REG_PTR_IMM", 0x82, 2),
            new("JL_REG_REG_IMM", 0x83, 2),
            new("JLZ_REG_PTR_REG", 0x84, 1),
            new("JLZ_REG_PTR_IMM", 0x85, 2),
            new("JGE_REG_REG_IMM", 0x86, 2),
            new("JGEZ_REG_PTR_REG", 0x87, 1),
            new("JGEZ_REG_PTR_IMM", 0x88, 2),
            new("JLE_REG_REG_IMM", 0x89, 2),
            new("JLEZ_REG_PTR_REG", 0x8A, 1),
            new("JLEZ_REG_PTR_IMM", 0x8B, 2),
        };
    }
}
