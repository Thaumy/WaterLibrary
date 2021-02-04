using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StdLib;
using StdLib_inf;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            StdLib.SrrLib e = new SrrLib();

            int[] i1 = { 7, 8, 9, 3, 2, 1, 6, 5, 4, 7 };
            int[] i2 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
            int[] i3 = { 1, 2, 4, 6, 2, 6, 5, 2, 8, 0 };
            
            Console.WriteLine(e.srr(i1, i2, i3));

            StdLib_inf.inf i = new StdLib_inf.inf();
            Console.WriteLine(i.Libver_total.ToString()+ i.Libver_deta.ToString() + i.gobal_Safe.ToString() + i.gobal_Past.ToString() + i.Test.ToString() + i.devInf);

            Console.ReadKey();


            

        }
    }
}
