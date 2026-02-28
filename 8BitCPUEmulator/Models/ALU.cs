using _8BitCPUEmulator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _8BitCPUEmulator.Models
{
    internal class ALU : IALU
    {
        public bool Zero { get; private set; }
        public bool Carry { get; private set; }
        public byte Add(byte A, byte B)
        {
            Carry = A + B > 0xFF;
            Zero = (A | B) == 0;
            return (byte)(A + B);
        }

        public byte And(byte A, byte B)
        {
            Zero = (A & B) == 0;
            return (byte)(A & B);
        }

        public byte Bsr(byte A)
        {
            bool Zero = A >> 1 == 0;
            return (byte)(A >> 1);
        }

        public byte Nor(byte A, byte B)
        {
            Zero = ~(A | B) == 0;
            return (byte)~(A | B);
        }

        public byte Sub(byte A, byte B)
        {
            Zero = A == B;
            return (byte)(A - B);
        }

        public byte Xor(byte A, byte B)
        {
            Zero = (A ^ B) == 0;
            return (byte)(A ^ B);
        }
    }
}
