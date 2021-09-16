using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkarosAssembler.Parser
{
    static class AssemblyParser
    {
        public static List<Instruction> Parse(string[] content)
        {
            List<Instruction> instructions = new();

            for (int i = 0; i < content.Length; i++)
            {
                var line = content[i].Trim();

                var noComments = RemoveComments(line);

                if (string.IsNullOrWhiteSpace(noComments))
                    continue;

                var insn = GetFirstWord(noComments);

                if (insn.firstWord.StartsWith('.'))
                {
                    Console.WriteLine($"Directives are not yet implemented.\n{ line }");
                    continue;
                }

                if (insn.firstWord.EndsWith(':'))
                {
                    instructions.Add(new("LABEL", new InstructionArgument[] { new(ArgType.None, insn.firstWord) }));

                    if (insn.restOfString is null)
                        continue;

                    // If any remaining instruction on same line, add it.
                    insn = GetFirstWord(insn.restOfString.Trim());
                }

                if (insn.restOfString is null)
                {
                    instructions.Add(Helpers.BuildInstruction(insn.firstWord, null));
                    continue;
                }

                var args = ParseArguments(insn.restOfString);

                instructions.Add(Helpers.BuildInstruction(insn.firstWord, args));
            }

            return instructions;
        }

        private static string RemoveComments(string line)
        {
            if (!line.Contains('#'))
                return line;

            var index = line.IndexOf('#');

            var noComments = line.Substring(0, index)
                                 .Trim();

            if (string.IsNullOrWhiteSpace(noComments))
                return null;

            return noComments;
        }

        private record SplitInstruction(string firstWord, string restOfString);

        private static SplitInstruction GetFirstWord(string line)
        {
            if (!line.Contains(' '))
            {
                return new(line, null);
            }
            var index = line.IndexOf(' ');

            var firstWord = line.Substring(0, index)
                                .Trim();

            var restOfString = line.Substring(index);

            return new(firstWord, restOfString);
        }

        private static string[] ParseArguments(string line)
        {
            var lineSplit = line.Split(',');
            var lineSplitClean = new List<string>();

            for (int i = 0; i < lineSplit.Length; i++)
            {
                if (string.IsNullOrEmpty(lineSplit[i]))
                    continue;

                lineSplitClean.Add(lineSplit[i]);
            }

            var args = new string[lineSplitClean.Count];

            for (int i = 0; i < lineSplitClean.Count; i++)
            {
                var argument = lineSplitClean[i].Trim();

                args[i] = argument;
            }

            return args;
        }
    }
}
