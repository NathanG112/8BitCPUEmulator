using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _8BitCPUEmulator.Interfaces
{
    public interface IALU
    {
        public bool Zero { get; }
        public bool Carry { get; }
        public abstract byte Add(byte A, byte B);
        public abstract byte Sub(byte A, byte B);
        public abstract byte And(byte A, byte B);
        public abstract byte Nor(byte A, byte B);
        public abstract byte Xor(byte A, byte B);
        public abstract byte Bsr(byte A);
    }
}
