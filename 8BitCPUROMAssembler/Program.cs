using Microsoft.Win32.SafeHandles;
using System.Formats.Asn1;
using System.Linq.Expressions;

namespace _8BitCPUROMAssembler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string filePath;
            bool validFile = false;
            Dictionary<string, byte> opcodes = new Dictionary<string, byte>
            {
                {"NOP", 0b0000 },
                {"END", 0b0001 },
                {"ADD", 0b0010 },
                {"SUB", 0b0011 },
                {"NOR", 0b0100 },
                {"XOR", 0b0101 },
                {"AND", 0b0110 },
                {"BSR", 0b0111 },
                {"LDI", 0b1000 },
                {"ADI", 0b1001 },
                {"JMP", 0b1010 },
                {"JMC", 0b1011 },
                {"CAL", 0b1100 },
                {"RET", 0b1101 },
                {"LOD", 0b1110 },
                {"STR", 0b1111 }

            };

            Console.WriteLine("Please enter the filepath to the file you want to assemble\n(The .bin file will be made in the same location)");
            do
            {
                filePath = Console.ReadLine();
                if(!File.Exists(filePath))
                {
                    Console.WriteLine("Invalid file path. Please try Again.");
                }
                else validFile = true;

            } while (!validFile);

            string binPath = Path.GetDirectoryName(filePath) + "\\" + Path.GetFileNameWithoutExtension(filePath) + ".bin";

            using (FileStream asStream = File.OpenRead(filePath))
            using (FileStream binStream = new FileStream(binPath, FileMode.Create))
            {
                StreamReader asReader = new StreamReader(asStream);

                Dictionary<string, byte> defs = new Dictionary<string, byte>();
                Dictionary<string, ushort> labels = new Dictionary<string, ushort>();

                List<string> lines = new List<string>();
                //First Pass -- Pseudoinstructions into actual opcodes, defining .def statements, comment trimming;
                while (!asReader.EndOfStream)
                {
                    string? line = asReader.ReadLine();
                    string[] arguments = line.Split(' ');

                    switch (arguments[0])
                    {
                        case ".def":
                            defs[arguments[1]] = byte.Parse(arguments[2]);
                            break;
                        case "BSL":
                            lines.Add($"ADD {arguments[1]} {arguments[1]} {arguments[2]}");
                            break;
                        case "INC":
                            lines.Add($"ADI {arguments[1]} 1");
                            break;
                        case "DEC":
                            lines.Add($"ADI {arguments[1]} 255");
                            break;
                        case "NOT":
                            lines.Add($"NOR {arguments[1]} r0 {arguments[2]}");
                            break;
                        case "OR":
                            lines.Add($"NOR {arguments[1]} {arguments[2]} {arguments[3]}");
                            lines.Add($"NOR {arguments[3]} r0 {arguments[3]}");
                            break;
                        case "XNOR":
                            lines.Add($"XOR {arguments[1]} {arguments[2]} {arguments[3]}");
                            lines.Add($"NOR {arguments[3]} r0 {arguments[3]}");
                            break;
                        case "NAND":
                            lines.Add($"AND {arguments[1]} {arguments[2]} {arguments[3]}");
                            lines.Add($"NOR {arguments[3]} r0 {arguments[3]}");
                            break;
                        case "CMP":
                            lines.Add($"SUB {arguments[1]} {arguments[2]} r0");
                            break;
                        case "MOV":
                            lines.Add($"ADD {arguments[1]} r0 {arguments[2]}");
                            break;
                        default:
                            if (!(arguments[0][0] == '/' && arguments[0][1] == '/')) lines.Add(line);
                            break;
                    }
                }
                asReader.Close();

                //Second Pass -- defining .label markers
                for (int i = 0; i < lines.Count; i++)
                {
                    string[] arguments = lines[i].Split(' ');
                    if (arguments[0] == ".label")
                    {
                        labels[arguments[1]] = (ushort)i;
                        string newInst = "";
                        for (int j = 2; j < arguments.Length; j++)
                        {
                            newInst += arguments[j];
                            if (j + 1 < arguments.Length) newInst += " ";
                        }
                        lines[i] = newInst;
                    }
                }

                //Third Pass - Instructions into machine code, write to binfile
                foreach (string line in lines)
                {
                    string[] arguments = line.Split(' ');
                    ushort instruction = 0;
                    switch (arguments[0])
                    {
                        case "NOP":
                            binStream.WriteByte(0);
                            binStream.WriteByte(0);
                            break;
                        case "END":
                            binStream.WriteByte(0b00010000);
                            binStream.WriteByte(0);
                            break;
                        case "ADD":
                            instruction += 0b0010 << 12;
                            instruction += (ushort)(ParseRegister(arguments[1]) << 8);
                            instruction += (ushort)(ParseRegister(arguments[2]) << 4);
                            instruction += (ushort)(ParseRegister(arguments[3]));

                            binStream.WriteByte((byte)(instruction >> 8));
                            binStream.WriteByte((byte)(instruction & 0xFF));
                            break;
                        case "SUB":
                            instruction += 0b0011 << 12;
                            instruction += (ushort)(ParseRegister(arguments[1]) << 8);
                            instruction += (ushort)(ParseRegister(arguments[2]) << 4);
                            instruction += (ushort)(ParseRegister(arguments[3]));

                            binStream.WriteByte((byte)(instruction >> 8));
                            binStream.WriteByte((byte)(instruction & 0xFF));
                            break;
                        case "NOR":
                            instruction += 0b0100 << 12;
                            instruction += (ushort)(ParseRegister(arguments[1]) << 8);
                            instruction += (ushort)(ParseRegister(arguments[2]) << 4);
                            instruction += (ushort)(ParseRegister(arguments[3]));

                            binStream.WriteByte((byte)(instruction >> 8));
                            binStream.WriteByte((byte)(instruction & 0xFF));
                            break;
                        case "XOR":
                            instruction += 0b0101 << 12;
                            instruction += (ushort)(ParseRegister(arguments[1]) << 8);
                            instruction += (ushort)(ParseRegister(arguments[2]) << 4);
                            instruction += (ushort)(ParseRegister(arguments[3]));

                            binStream.WriteByte((byte)(instruction >> 8));
                            binStream.WriteByte((byte)(instruction & 0xFF));
                            break;
                        case "AND":
                            instruction += 0b0110 << 12;
                            instruction += (ushort)(ParseRegister(arguments[1]) << 8);
                            instruction += (ushort)(ParseRegister(arguments[2]) << 4);
                            instruction += (ushort)(ParseRegister(arguments[3]));

                            binStream.WriteByte((byte)(instruction >> 8));
                            binStream.WriteByte((byte)(instruction & 0xFF));
                            break;
                        case "BSR":
                            instruction += 0b0111 << 12;
                            instruction += (ushort)(ParseRegister(arguments[1]) << 4);
                            instruction += (ushort)(ParseRegister(arguments[2]));

                            binStream.WriteByte((byte)(instruction >> 8));
                            binStream.WriteByte((byte)(instruction & 0xFF));
                            break;
                        case "LDI":
                            instruction += 0b1000 << 12;
                            instruction += (ushort)(ParseRegister(arguments[1]) << 8);
                            if (defs.ContainsKey(arguments[2])) instruction += defs[arguments[2]];
                            else instruction += byte.Parse(arguments[2]);

                            binStream.WriteByte((byte)(instruction >> 8));
                            binStream.WriteByte((byte)(instruction & 0xFF));
                            break;
                        case "ADI":
                            instruction += 0b1001 << 12;
                            instruction += (ushort)(ParseRegister(arguments[1]) << 8);
                            if (defs.ContainsKey(arguments[2])) instruction += defs[arguments[2]];
                            else instruction += byte.Parse(arguments[2]);

                            binStream.WriteByte((byte)(instruction >> 8));
                            binStream.WriteByte((byte)(instruction & 0xFF));
                            break;
                        case "JMP":
                            instruction += 0b1010 << 12;
                            if (labels.ContainsKey(arguments[1])) instruction += labels[arguments[1]];
                            else instruction += ushort.Parse(arguments[1]);

                            binStream.WriteByte((byte)(instruction >> 8));
                            binStream.WriteByte((byte)(instruction & 0xFF));
                            break;
                        case "JMC":
                            instruction += 0b1011 << 12;
                            switch (arguments[1])
                            {
                                case "00":
                                case "equals":
                                case "e":
                                case "==":
                                    break;
                                case "01":
                                case "not equals":
                                case "ne":
                                case "!=":
                                    instruction += 0b01 << 10;
                                    break;
                                case "10":
                                case "greater than":
                                case "ge":
                                case ">=":
                                    instruction += 0b10 << 10;
                                    break;
                                case "11":
                                case "less than":
                                case "le":
                                case "<":
                                    instruction += 0b11 << 10;
                                    break;
                            }
                            if (labels.ContainsKey(arguments[2])) instruction += labels[arguments[2]];
                            else instruction += ushort.Parse(arguments[2]);

                            binStream.WriteByte((byte)(instruction >> 8));
                            binStream.WriteByte((byte)(instruction & 0xFF));
                            break;
                        case "CAL":
                            instruction += 0b1100 << 12;
                            if (labels.ContainsKey(arguments[1])) instruction += labels[arguments[1]];
                            else instruction += ushort.Parse(arguments[1]);

                            binStream.WriteByte((byte)(instruction >> 8));
                            binStream.WriteByte((byte)(instruction & 0xFF));
                            break;
                        case "RET":
                            instruction += 0b1101 << 12;

                            binStream.WriteByte((byte)(instruction >> 8));
                            binStream.WriteByte(0);
                            break;
                        case "LOD":
                            instruction += 0b1110 << 12;
                            instruction += (ushort)(ParseRegister(arguments[1]) << 8);
                            instruction += (ushort)(ParseRegister(arguments[2]) << 4);
                            instruction += byte.Parse(arguments[3]);

                            binStream.WriteByte((byte)(instruction >> 8));
                            binStream.WriteByte((byte)(instruction & 0xFF));
                            break;
                        case "STR":
                            instruction += 0b1111 << 12;
                            instruction += (ushort)(ParseRegister(arguments[1]) << 8);
                            instruction += (ushort)(ParseRegister(arguments[2]) << 4);
                            instruction += byte.Parse(arguments[3]);

                            binStream.WriteByte((byte)(instruction >> 8));
                            binStream.WriteByte((byte)(instruction & 0xFF));
                            break;
                    }
                }

            }
        }

        static ushort ParseRegister(string reg)
        {
            switch(reg)
            {
                case "r1":
                    return 1;
                case "r2":
                    return 2;
                case "r3":
                    return 3;
                case "r4":
                    return 4;
                case "r5":
                    return 5;
                case "r6":
                    return 6;
                case "r7":
                    return 7;
                case "r8":
                    return 8;
                case "r9":
                    return 9;
                case "r10":
                    return 10;
                case "r11":
                    return 11;
                case "r12":
                    return 12;
                case "r13":
                    return 13;
                case "r14":
                    return 14;
                case "r15":
                    return 15;
                default:
                    return 0;
            }
        }
    }
}
