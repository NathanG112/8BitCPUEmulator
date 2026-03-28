using _8BitCPUEmulator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _8BitCPUEmulator.Models
{
    public class CPU
    {
        public Clock CLK { get; private set; }
        public ushort PC { get; set; }
        public IALU _ALU { get; private set; }
        public IRegisterFile REG { get; private set; }
        public ushort[] ROM { get; private set; }
        public CallStack STK { get; private set; }
        public byte[] MEM { get; set; }

        public CPU()
        {
            PC = 0;
            _ALU = new ALU();
            REG = new RegisterFile();
            ROM = new ushort[0x400];
            STK = new CallStack();
            MEM = new byte[0xFF];

            CLK = new Clock(this);
        }

        public CPU(FileStream binFile)
        {
            PC = 0;
            _ALU = new ALU();
            REG = new RegisterFile();
            ROM = new ushort[0x400];

            ushort idx = 0;
            while(binFile.Position < binFile.Length)
            {
                ROM[idx++] = (ushort)(binFile.ReadByte() << 8 | binFile.ReadByte());
            }

            STK = new CallStack();
            MEM = new byte[0xFF];

            CLK = new Clock(this);
        }
    }
}
