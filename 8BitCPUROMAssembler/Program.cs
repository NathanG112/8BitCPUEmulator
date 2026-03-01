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

            string binPath = Path.GetDirectoryName(filePath) + Path.GetFileNameWithoutExtension(filePath) + ".bin";

            if(File.Exists(binPath)) using (File.Create(binPath)) { }
            
            using (FileStream asStream = File.OpenRead(filePath))
            {
                FileStream binStream = File.OpenWrite(binPath);
                
                StreamReader asReader = new StreamReader(asStream);
                StreamWriter binWriter = new StreamWriter(binStream);

                Dictionary<string, byte> defs = new Dictionary<string, byte>();
                Dictionary<string, ushort> labels = new Dictionary<string, ushort>();

                List<string> lines = new List<string>();
                //First Pass -- Pseudoinstructions into actual opcodes, defining .def statements, comment trimming;
                while(!asReader.EndOfStream)
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
                        default:
                            if (!(arguments[0][0] == '/' && arguments[0][1] == '/')) lines.Add(line);
                            break;
                    }
                }
                asReader.Close();
                
                //Second Pass -- defining .label markers
                for(int i = 0; i < lines.Count; i++)
                {
                    string[] arguments = lines[i].Split(' ');
                    if(arguments[0] == ".label")
                    {
                        labels[arguments[1]] = (ushort)i;
                        string newInst = "";
                        for (int j = 2; i < arguments.Length; i++)
                        {
                            newInst += arguments[j];
                            if (j + 1 < arguments.Length) newInst += " ";
                        }
                        lines[i] = newInst;
                    }
                }

                //Third Pass - Instructions into machine code

            }
        }
    }
}
