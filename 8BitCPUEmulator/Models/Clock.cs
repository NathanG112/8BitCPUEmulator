using _8BitCPUEmulator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace _8BitCPUEmulator.Models
{

    public class Clock : IClock
    {
        private System.Timers.Timer CLK;
        private bool PCJump;
        CPU _CPU { get; set; }

        public Clock(CPU cpu) 
        { 
            _CPU = cpu;
            PCJump = false;

            CLK = new System.Timers.Timer(20);
            CLK.Elapsed += Pulse;
            CLK.AutoReset = true;
            CLK.Enabled = true;
        }

        private void Pulse(object source, ElapsedEventArgs e) => Update(_CPU);

        public void Update(CPU cpu)
        {
            Console.WriteLine("Pulse");
            ushort inst = cpu.ROM[cpu.PC];

            switch((byte)(inst >> 12))
            {
                //NOP
                case 0b0000:
                    break;
                //END
                case 0b0001:
                    CLK.Enabled = false;
                    break;
                //ADD
                case 0b0010:
                    cpu.REG.Write((byte)(inst & 0x000F), cpu._ALU.Add(cpu.REG.Read((byte)((inst & 0x0F00) >> 8)), cpu.REG.Read((byte)((inst & 0x00F0) >> 4))));
                    break;
                //SUB
                case 0b0011:
                    cpu.REG.Write((byte)(inst & 0x000F), cpu._ALU.Sub(cpu.REG.Read((byte)((inst & 0x0F00) >> 8)), cpu.REG.Read((byte)((inst & 0x00F0) >> 4))));
                    break;
                //NOR
                case 0b0100:
                    cpu.REG.Write((byte)(inst & 0x000F), cpu._ALU.Nor(cpu.REG.Read((byte)((inst & 0x0F00) >> 8)), cpu.REG.Read((byte)((inst & 0x00F0) >> 4))));
                    break;
                //XOR
                case 0b0101:
                    cpu.REG.Write((byte)(inst & 0x000F), cpu._ALU.Xor(cpu.REG.Read((byte)((inst & 0x0F00) >> 8)), cpu.REG.Read((byte)((inst & 0x00F0) >> 4))));
                    break;
                //AND
                case 0b0110:
                    cpu.REG.Write((byte)(inst & 0x000F), cpu._ALU.And(cpu.REG.Read((byte)((inst & 0x0F00) >> 8)), cpu.REG.Read((byte)((inst & 0x00F0) >> 4))));
                    break;
                //BSR
                case 0b0111:
                    cpu.REG.Write((byte)(inst & 0x000F), cpu._ALU.Bsr(cpu.REG.Read((byte)((inst & 0x00F0) >> 4))));
                    break;
                //LDI
                case 0b1000:
                    cpu.REG.Write((byte)((inst & 0x0F00) >> 8), (byte)(inst & 0x00FF));
                    break;
                //ADI
                case 0b1001:
                    cpu.REG.Write((byte)((inst & 0x0F00) >> 8), cpu._ALU.Add(cpu.REG.Read((byte)((inst & 0x0F00) >> 8)), (byte)(inst & 0x00FF)));
                    break;
                //JMP
                case 0b1010:
                    cpu.PC = (ushort)((inst & 0x03FF) - 1);
                    break;
                //JMC
                case 0b1011:
                    switch((byte)((inst & 0x0C00) >> 10)) {
                        //Zero Flag True, ==
                        case 0b00:
                            if (cpu._ALU.Zero) cpu.PC = (ushort)((inst & 0x03FF));
                            break;
                        //Zero Flag False, !=
                        case 0b01:
                            if (!cpu._ALU.Zero) cpu.PC = (ushort)((inst & 0x03FF));
                            break;
                        //Carry Flag True, >=
                        case 0b10:
                            if (cpu._ALU.Carry) cpu.PC = (ushort)((inst & 0x03FF));
                            break;
                        //Carry Flag False, <
                        case 0b11:
                            if (!cpu._ALU.Carry) cpu.PC = (ushort)((inst & 0x03FF));
                            break;
                    }
                    PCJump = true;
                    break;
                //CAL
                case 0b1100:
                    cpu.STK.Push(cpu.PC);
                    cpu.PC = (ushort)((inst & 0x03FF));
                    PCJump = true;
                    break;
                //RET
                case 0b1101:
                    cpu.PC = cpu.STK.Pop();
                    break;
                //LOD
                case 0b1110:
                    cpu.REG.Write((byte)((inst & 0x00F0) >> 4), cpu.MEM[cpu.REG.Read((byte)((inst & 0x0F00) >> 8)) + (byte)((inst & 0x000F) - 8)]);
                    break;
                //STR
                case 0b1111:
                    cpu.MEM[cpu.REG.Read((byte)((inst & 0x0F00) >> 8)) + (byte)((inst & 0x000F) - 8)] = cpu.REG.Read((byte)((inst & 0x00F0) >> 4));
                    break;
            }

            if (!PCJump) cpu.PC++;
            else PCJump = false;
        }

        public bool ClockEnabled() => CLK.Enabled;
    }
}
