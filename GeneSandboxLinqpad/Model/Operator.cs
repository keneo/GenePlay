using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneSandboxLinqpad.Model
{
    class Operator
    {
        public readonly string CSCode;

        public Operator(int[] genes, ref int pos)
        {
            var gene = genes[pos++]%5;
            switch (gene)
            {
                case 0:
                    this.CSCode = "+";
                    break;
                case 1:
                    this.CSCode = "-";
                    break;
                case 2:
                    this.CSCode = "*";
                    break;
                case 3:
                    this.CSCode = "/";
                    break;
                case 4:
                    this.CSCode = "%";
                    break;
                default:
                    throw new Exception("should not happen");

            }
        }
    }
}
