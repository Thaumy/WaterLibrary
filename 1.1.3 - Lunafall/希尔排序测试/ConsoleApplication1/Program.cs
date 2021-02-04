using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StdLib.LogicHandler;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            Sorter st = new Sorter();//初始化排序类
            int[] a = { 332, 152, 85, 76, 3232, 09, 765, 1780, 87, 554, 88, 32 };
            a = st.shellSort(a);//调用排序
            foreach (int p in a)
            {
                Console.Write(p + " ");
            }
            Console.ReadKey();
        }
    }
}
