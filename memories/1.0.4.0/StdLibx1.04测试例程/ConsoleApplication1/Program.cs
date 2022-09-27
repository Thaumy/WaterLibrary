using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using StdLib_GobalnameSpace;
using StdMcl;
using StdSrr;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {

            //StdLib版本信息部分
            StdLib_GobalnameSpace.StdInf inf = new StdInf();
            Console.WriteLine(inf.Base_Safe);
            Console.WriteLine(inf.DevEdition);
            Console.WriteLine(inf.Gobal_Compatible);
            Console.WriteLine(inf.PubEdition);
            Console.WriteLine(inf.Stdver_Deta);
            Console.WriteLine(inf.Stdver_Main);



            //矩阵位移算法部分
            StdSrr.Class_Srr srr = new Class_Srr();
            int[] psw = { 7, 8, 7, 6, 6, 7, 3, 2, 1, 9 };
            int[] dic = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            int[] num = { 1, 2, 4, 6, 2, 6, 5, 2, 8, 0 };
            int[] temp = (srr.Main_Srr(psw, dic, num));//在这里StdSrr的方法中的开发者调试功能帮助输出了矩阵

            for(int i = 0; i < 10; i++)
            {
                if (temp[i] < 10)
                {
                    Console.Write("[" + "0" + temp[i] + "]");//当x值为0时才可能出现输出单数坐标的情况，
                }                                           //单数坐标都小于10，在这里提取单数坐标并在前面加一个0以表示x坐标
                else
                {
                    Console.Write("[" + temp[i] + "]");
                }
            }
            Console.WriteLine();//集体换行


            //输出矩阵部分
            int[,] NumsTemp = srr.NumList[0];
            for (int i2 = 0; i2 < NumsTemp.GetLength(0); i2++)//循环非泛型数组组的每一行
            {
                for(int i3 = 0; i3 < NumsTemp.GetLength(1); i3++)
                {
                    Console.Write(NumsTemp[i2,i3]);
                }
                Console.WriteLine();
            }


            Console.WriteLine();//集体换行


            //MC启动器数据层部分
            StdMcl.Class_Mcl xml = new Class_Mcl();//实例化

            //输出Inf.xml中的信息
            Console.WriteLine(xml.Sys_Copyright);
            Console.WriteLine(xml.Sys_Memory);
            Console.WriteLine(xml.User_Email);
            Console.WriteLine(xml.User_Name);
            Console.WriteLine(xml.User_PassWord);

            xml.Mcl_Save(true, 2233, "hahaha", "faq", "123");//保存更改

            //再次输出检测
            Console.WriteLine(xml.Sys_Copyright);
            Console.WriteLine(xml.Sys_Memory);
            Console.WriteLine(xml.User_Email);
            Console.WriteLine(xml.User_Name);
            Console.WriteLine(xml.User_PassWord);

            Console.ReadKey();
        }
    }
}
