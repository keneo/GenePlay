using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneSandboxLinqpad.Model
{
    class Expression
    {
        private readonly List<Expression> _children;
        public readonly string CSCode;
        private readonly Operator _operator;

        public Expression(int[]genes, ref int pos)
        {
            var gene = genes[pos++];

            var genCode = gene%3;
            switch (genCode)
            {
                case 0:
                    _children = new List<Expression>() {new Expression(genes, ref pos)};
                    this.CSCode = "(" + _children[0].CSCode + ")";
                    break;
                case 1:
                    _operator = new Operator(genes, ref pos);
                    _children = new List<Expression>() { new Expression(genes, ref pos), new Expression(genes, ref pos) };
                    this.CSCode = _children[0].CSCode + _operator.CSCode + _children[1].CSCode;
                    break;
                case 2:
                    this.CSCode = (gene%10).ToString();
                    break;
                default:
                    throw new Exception("should not happen");

            }
        }

        public Expression(Expression other)
        {
            
        }
    }
}
