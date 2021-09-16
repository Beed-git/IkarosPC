using IkarosAssembler.Parser;
using IkarosAssembler.Assemble;
using System;
using System.IO;

namespace IkarosAssembler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Please enter at least 1 file name.");
                //return;

                args = new[] { "test.asm" };
            }

            foreach (string arg in args)
            {
                //try
                //{
                    var text = File.ReadAllLines(arg);

                    var parsed = AssemblyParser.Parse(text);
                    var result = Assembler.Assemble(parsed);

                    var filename = arg + ".bsm";

                    File.WriteAllBytes(filename, result);
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine(ex.ToString());
                //    return;
                //}
            }

            //var complexAsm = new string[]
            //{
            //    ".export main",
            //    "",
            //    "def TILE_START = 0x8000",
            //    "",
            //    "main:",
            //    "   MOV 0x4500, $A",
            //    "   MOV 0b0110_1101_0010_1111, $B",
            //    "   MOV 15, $C",
            //    "   # Comment on its own.",
            //    "   ADD      $B,    $C      # Comment on line.",
            //    "",
            //    ".data",
            //    "counter_array:",
            //    "   .word 0x1000    # Counter starting at 0x1000.",
            //    "   .word 0x2000    # Counter starting at 0x2000."
            //};
        }
    }
}
