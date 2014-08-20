using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CSharp;

namespace GeneSandboxLinqpad
{


    class Program
    {
        static void Main(string[] args)
        {


            Random rnd = new Random();



            List<Animal> generation = Enumerable.Range(0, 16).Select(e => new Animal()).ToList();

            for (int i = 0; i < 1000; i++)
            {
                generation.ForEach(a => a.CalculateFitness());
                //generation.Dump();

                var top = generation.OrderBy(g => g.Fitness)
                        //.ThenBy(g=>g.Display.Length)
                        .Take(generation.Count() / 2).ToList();


                //return;

                top
                    .Select(t => t.Display + ", " + t.Fitness).ToList().ForEach(Console.WriteLine);
                    //.Select (t => t)
                    //.Take()
                    //.Dump();

                List<Animal> nextGen
                    =
                    Enumerable.Range(0, 15).Select(e => top[rnd.Next(top.Count)].MakeChildWith(top[rnd.Next(top.Count)]))
                    .Concat(top.Take(1))
                    .ToList();
                generation = nextGen;


            }
        }


        // Define other methods and classes here
    }
}
