using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StdLib1_16;
using StdLib1_16.StdEct;
using StdLib1_16.StdDal;

using System.Drawing;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            #region LSS算法测试
            int[] a = { 1, 65, 6, 723, 847, 5874, 884, 041, 6, 76, 1, 847, 3, 79, 715, 09, 46, 235, 886, 45, 87, 122, 98, 15, 01, 4 };
            foreach(int p in Class_LSS.Main_LSS(a))
            {
                Console.WriteLine(p);
            }
            #endregion

            #region ANSW测试

            Class_ANSW aw = new Class_ANSW();

            #region ANSW解码测试
            Console.WriteLine(aw.DeANSW(@"L:\C#项目开发\StdLib1.06\answ"));
            #endregion

            #region ANSW转码测试
            Bitmap bp = aw.ToANSW("4869212069206c6f76652074686520776f726c6421", @"L:\C#项目开发\StdLib1.06\source");
            Image i = bp;
            i.Save(@"L:\C#项目开发\StdLib1.06\answ.bmp");
            #endregion

            //注意：
            //在ANSW解码中，输入流可以为有.bmp后缀的文件名或无后缀的文件名
            //在ANSW转码中，输入与解码规则相同，输出分两种情况如下：

            //1.流错误输出所指定的流必须带.bmp后缀
            //2.正常输出所指定的流可以为有.bmp后缀文件名或无后缀文件名

            #endregion

            Console.ReadKey();
        }
    }

}
