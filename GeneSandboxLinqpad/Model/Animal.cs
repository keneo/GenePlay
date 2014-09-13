using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;

namespace GeneSandboxLinqpad
{
    class Animal
      
    {
        static Random rnd = new Random();

        public readonly List<int> Genes;

        public string Display;
        public int? Ret;
        public int? Fitness;

        public Animal()
        {
            this.Genes = Enumerable.Range(0, 32).Select(e => rnd.Next(256)).ToList();
        }

        public Animal(IEnumerable<int> genes)
        {
            this.Genes = new List<int>(genes);
        }

        public Animal MakeChildWith(Animal other)
        {
            int splitPoint = rnd.Next(this.Genes.Count() + 1);
            var newGenes = this.Genes.GetRange(0, splitPoint).Concat(other.Genes.GetRange(splitPoint, other.Genes.Count() - splitPoint)).ToList();

            var a = new Animal(newGenes);

            var m = a.Mutated();

            return m;
        }

        private Animal Mutated()
        {
            //copy
            List<int> newGenes = new List<int>(this.Genes);

            while (rnd.Next(2) == 0)
            {
                newGenes[rnd.Next(newGenes.Count())] = rnd.Next(256);
            }


            while (rnd.Next(5) == 0)
            {


                int srcIndex = rnd.Next(newGenes.Count());
                int dstIndex = rnd.Next(newGenes.Count());
                int maxLen = (newGenes.Count() - Math.Max(srcIndex, dstIndex));
                int len = rnd.Next(maxLen + 1);

                for (int i = 0; i < len; i++)
                    newGenes[i + dstIndex] = newGenes[i + srcIndex];
            }

            while (rnd.Next(5) == 0)
            {
                //roszada
                int splitPoint = rnd.Next(newGenes.Count() + 1);
                newGenes =
                    newGenes.GetRange(splitPoint, newGenes.Count() - splitPoint).Concat(newGenes.GetRange(0, splitPoint)).ToList();
            }

            while (rnd.Next(5) == 0)
            {
                //del
                int index = rnd.Next(newGenes.Count() + 1);
                int len = rnd.Next(newGenes.Count() - index);
                newGenes.RemoveRange(index, len);
                newGenes.AddRange(Enumerable.Range(0, len).Select(x => rnd.Next(256)));
            }

            return new Animal(newGenes);
        }


        public void CalculateFitness()
        {
            var expr = BuildExprInt(this.Genes);

            this.Display = expr;

            var fit = CachedGetFitnessByEffectiveCode(expr);

            this.Fitness = fit;
        }

        static Dictionary<string, int> cache = new Dictionary<string, int>();

        int CachedGetFitnessByEffectiveCode(string expr)
        {
            if (cache.ContainsKey(expr))
                return cache[expr];
            else
            {
                var f = GetFitnessByEffectiveCode(expr);
                cache[expr] = f;
                return f;
            }
        }

        static int GetFitnessByEffectiveCode(string expr)
        {
            var method = CompileMethod(expr);

            if (method == null)
            {
                return int.MaxValue;
            }

            try
            {
                int fit = 0;

                int[] testCaseArgs = Enumerable.Range(0, 10).ToArray();

                for (int x = 0; x < 10; x++)
                {
                    int ret = (int)method.Invoke(null, new object[] { x });

                    int expected = (int)x * x / 2 + x - 3;

                    var diff = ret - expected;
                    if (diff < 0)
                        diff = -diff;
                    
                    fit += diff;
                }

                return fit;
            }
            catch (Exception ex)
            {
                if (!(ex.InnerException is DivideByZeroException))
                    throw;

                return int.MaxValue;
            }
        }

        private static MethodInfo CompileMethod(string expr)
        {
            CompilerResults res;

            CodeDomProvider compiler;
            compiler = CSharpCodeProvider.CreateProvider("CSharp");
            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;
            parameters.GenerateExecutable = false;
            parameters.IncludeDebugInformation = false;
            //parameters.OutputAssembly = Path.Combine(@"C:\Users\bartek\AppData\Local\Temp\", "animal" + Guid.NewGuid() + ".dll");

            res = compiler.CompileAssemblyFromSource(parameters,
                "public static class C {public static int F(int a){return " + expr + ";}}");

            MethodInfo method;

            if (res.Errors.Count > 0) //div by zero
            {
                if (res.Errors.Cast<CompilerError>().Any(ce => ce.ErrorNumber == "CS0020")) //div by zero
                {
                    method = null;
                }
                else
                {
                    throw new Exception("fixme");
                }
            }
            else
            {
                method = res.CompiledAssembly.GetType("C").GetMethod("F");
            }
            return method;
        }

        string BuildExprInt(List<int> genes)
        {
            int i = 0;
            return BuildExprInt(genes, ref i);
        }


        private static string BuildExprInt(List<int> genes, ref int index)
        {
            if (index >= genes.Count())
                return "1";

            var gene = genes[index];

            var c = gene % 4;

            index++;

            switch (c)
            {
                case 0:
                    return BuildExprInt(genes, ref index) + BuildOper(genes, ref index) + BuildExprInt(genes, ref index);
                case 1:
                    return "(" + BuildExprInt(genes, ref index) + ")";
                case 2:
                    return (gene%10).ToString();
                case 3:
                    return "a";
                //case 5:
                //	return "((("+BuildExprInt(genes, ref index)+")==0)?("+BuildExprInt(genes, ref index)+"):("+BuildExprInt(genes, ref index)+"))"     ;
                default:
                    throw new Exception("should not happen");
            }
        }

        private static string BuildOper(List<int> genes, ref int index)
        {
            if (index >= genes.Count())
                return "+";

            var gene = genes[index] % 5;

            index++;

            switch (gene)
            {
                case 0:
                    return "/";
                case 1:
                    return "-";
                case 2:
                    return "*";
                case 3:
                    return "+";
                case 4:
                    return "%";
                default:
                    throw new Exception("should not happen");
            }
        }
    }
}