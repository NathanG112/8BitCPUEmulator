using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _8BitCPUEmulator.Models
{
    public class CallStack
    {
        private const byte capacity = 0xF;
        private CStackNode? top;

        public CallStack()
        {
            top = null;
        }

        public void Push(ushort addr)
        {
            if(top == null)
            {
                top = new CStackNode(addr);
                return;
            }

            CStackNode newNode = new CStackNode(addr, top);
            top = newNode;
        }
        public ushort Pop()
        {
            if (top == null) return 0;

            ushort addr = top.addr;
            top = top.next;
            return addr;
        }
    }

    public class CStackNode
    {
        public ushort addr;
        public CStackNode next;

        public CStackNode(ushort addr, CStackNode next = null)
        {
            this.addr = addr;
            this.next = next;
        }
    }
}
