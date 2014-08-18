using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneSandboxLinqpad
{
    class Animal
      
    {
        static Random rnd = new Random();

        public List<int> Genes;
        public string Display;
        public int? Ret;
        public int? Fitness;

        public void SetRandomGenes()
        {
            this.Genes = Enumerable.Range(0, 32).Select(e => rnd.Next(256)).ToList();
        }

        public Animal MakeChildWith(Animal other)
        {
            var a = new Animal();

            {
                int splitPoint = rnd.Next(this.Genes.Count() + 1);
                a.Genes = this.Genes.GetRange(0, splitPoint).Concat(other.Genes.GetRange(splitPoint, other.Genes.Count() - splitPoint)).ToList();
            }

            if (rnd.Next(2) == 0)
                a.Genes[rnd.Next(a.Genes.Count())] = rnd.Next(256);

            if (rnd.Next(5) == 0)
            {
                //roszada
                int splitPoint = rnd.Next(this.Genes.Count() + 1);
                a.Genes = a.Genes.GetRange(splitPoint, a.Genes.Count() - splitPoint).Concat(a.Genes.GetRange(0, splitPoint)).ToList();
            }

            return a;
        }
    }
}