using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HwModule.Utils
{
    public static class ASCII
    {
        public const char STX = (char)0x02;
        public const char ETX = (char)0x03;
        public const char EOT = (char)0x04;
        public const char ENQ = (char)0x05;
        public const char NAK = (char)0x15;
        public const char FS = (char)0x1C;
        public const char LF = (char)0x0A;
        public const char CR = (char)0x0D;
        public const char ESC = (char)0x1B;
        public const char GS = (char)0x1D;
    }
}
