using _8BitCPUEmulator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _8BitCPUEmulator.Models
{
    internal class RegisterFile : IRegisterFile
    {
        private byte[] registers;

        public RegisterFile()
        {
            registers = new byte[0xE];
        }
        public byte Read(byte reg)
        {
            if (reg == 0) return 0x00;

            return registers[reg - 1];
        }

        public void Write(byte reg, byte data)
        {
            if (reg == 0) return;

            registers[reg - 1] = data;
        }
    }
}
