using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkarosAssembler.Parser
{
    public enum ArgType
    {
        None,
        Register,
        Immediate,
        RegisterPointer,
        ImmediatePointer
    }

    public record Instruction(string InstructionType, InstructionArgument[] InstructionArguments);
    public record InstructionArgument(ArgType Type, string Value);

    static partial class Helpers
    {
        public static Instruction BuildInstruction(string instruction, string[] args)
        {
            if (!Data.GetAllKeywords.Contains(instruction))
            {
                throw new Exception($"Instruction '{ instruction }' does not exist.");
            }

            var argCount = args is null ? 0 : args.Length;

            var insnArgs = new InstructionArgument[argCount];

            var builder = new StringBuilder();

            for (int i = 0; i < argCount; i++)
            {
                builder.Clear();

                var argument = args[i].Trim();
                var isPointer = false;

                if (argument.Contains("+"))
                    throw new Exception("0x1CNN and 0x1DNN have not been implemented yet.");

                // Ensure any pointers start and end with brackets.
                if (argument.StartsWith("(") || argument.EndsWith(")"))
                {
                    if (!(argument.StartsWith("(") && argument.EndsWith(")")))
                        throw new Exception("Missing bracket with argument.");

                    isPointer = true;
                    argument = argument.Substring(1, argument.Length - 1);
                }

                // If number ensure number is in hex.
                if (IsNumber(argument))
                {
                    argument = ParseNumberToHexString(argument);
                }

                ArgType argtype;
                if (argument.StartsWith("$"))
                {
                    // If register check its valid.
                    if (!Data.GetRegisters.Contains(argument))
                        throw new Exception($"Unknown register '{ argument }'");

                    argtype = isPointer ? ArgType.RegisterPointer : ArgType.Register; 
                }
                else
                {
                    // We'll be either jumping to a number, a constant, or an address.
                    argtype = isPointer ? ArgType.ImmediatePointer : ArgType.Immediate;
                }

                insnArgs[i] = new(argtype, argument);
            }

            builder.Append(instruction);

            foreach (InstructionArgument arg in insnArgs)
            {
                builder.Append('_');
                
                switch(arg.Type)
                {
                    case ArgType.Register: builder.Append("REG"); break;
                    case ArgType.Immediate: builder.Append("IMM"); break;
                    case ArgType.RegisterPointer: builder.Append("PTR_REG"); break;
                    case ArgType.ImmediatePointer: builder.Append("PTR_IMM"); break;
                    default: throw new Exception($"Unknown argument provided '{ arg }'");
                }
            }

            var insn = builder.ToString();

            return new(insn, insnArgs);
        }
    }
}
