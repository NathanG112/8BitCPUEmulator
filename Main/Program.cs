using _8BitCPUEmulator.Interfaces;
using _8BitCPUEmulator.Models;

namespace _8BitCPUEmulator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please select the .bin file to attach as the program ROM");
            bool validFile = false;
            string path;
            do
            {
                path = Console.ReadLine();
                if (File.Exists(path)) validFile = true;
                else Console.WriteLine("Not a valid file");
            } while (!validFile);

            CPU cpu = new CPU(File.OpenRead(path));


            Console.WriteLine("cpu initialized");
        }
    }
}
