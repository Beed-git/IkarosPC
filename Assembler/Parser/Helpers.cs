using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkarosAssembler.Parser
{
    static partial class Helpers
    {
        public static bool IsNumber(string line)
        {
            if (line.StartsWith("0x"))
                return true;
            if (line.StartsWith("0b"))
                return true;
            if (char.IsDigit(line[0]))
                return true;

            return false;
        }

        public static string ParseNumberToHexString(string number)
        {
            // Hex.
            if (number.StartsWith("0x"))
            {
                var hex = number.Substring(2);

                return ushort.Parse(hex, System.Globalization.NumberStyles.HexNumber)
                             .ToString("X4");
            }
            // Binary.
            if (number.StartsWith("0b"))
            {
                var binary = number.Substring(2)
                                   .Replace("_", "");

                return Convert.ToUInt16(binary, 2)
                              .ToString("X4");
            }
            // Dec10.
            return ushort.Parse(number).ToString("X4");
        }

        public static bool CheckValidInsnType(string isns)
        {
            if (isns.StartsWith('.'))
                return true;

            if (isns.EndsWith(':'))
                return true;

            var instruction = isns.ToUpper();

            return Data.GetAllKeywords.Contains(instruction);

        }
    }
}
