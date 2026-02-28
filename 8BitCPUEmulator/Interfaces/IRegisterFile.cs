using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _8BitCPUEmulator.Interfaces
{
    public interface IRegisterFile
    {

        public abstract byte Read(byte reg);
        public abstract void Write(byte reg, byte data);
    }
}
