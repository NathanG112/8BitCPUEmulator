using _8BitCPUEmulator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _8BitCPUEmulator.Interfaces
{
    public interface IClock
    {
        protected abstract void Update(CPU cpu);
    }
}
