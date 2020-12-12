using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StdLib_GobalnameSpace;
using StdEct;

namespace StdLibx1._05测试例程
{
    class Program
    {
        static void Main(string[] args)
        {
            {
                //StdLib_GobalnameSpace.StdInf inf = new StdLib_GobalnameSpace.StdInf("http://www.thaumy.ml/Std_index/StdWebInf.html");
                StdLib_GobalnameSpace.StdInf inf = new StdLib_GobalnameSpace.StdInf();
                Console.WriteLine(inf.Stdver_newPub);
                Console.WriteLine(inf.Stdver_downloadURL);
            }
            {
                string s = Console.ReadLine();
                Console.WriteLine(Class_Hash.Main_Hash(s));//输出Hash加密后的s
                Console.WriteLine(Class_MD5.Main_MD5(s));//输出MD5加密后的s
            }

            {
                //bool b;
                //bool b2;
                /*应用程序单重载运行，成功返回true，失败返回false*/
                //b=StdDal.Class_Run.Run(@"Y:\AdobeSoft\Adobe Photoshop CS6 (64 Bit)\Adobe Photoshop CS6 (64 Bit)\Photoshop.exe");
                /*应用程序双重载运行,同上*/
                //b2=StdDal.Class_Run.Run(@"Y:\AdobeSoft\Adobe Flash CS6\Adobe Flash CS6\Flash.exe", @"Y:\AdobeSoft\Adobe Illustrator CS6\Support Files\Contents\Windows\Illustrator.exe");
                //Console.WriteLine(b.ToString()+b2.ToString());
            }
            {
                StdDal.Class_Xml.CreateXml(@"D:\", "Xml_1","StdLib");
                
                StdDal.Class_Xml StdXml = new StdDal.Class_Xml(@"D:\Xml_1.xml");

                StdXml.AddNode("//StdLib", "MyNode", "att", "Att233", "This is my innerText!");//Xml写入测试

                StdXml.AddNode("//StdLib", "XN1", "att", null, "Type");//节点重名测试
                StdXml.AddNode("//StdLib", "XN1", "att", null, "Type");
                
                StdXml.AddChild("//StdLib", "NO_0");//节点嵌套测试
                StdXml.AddChild("//StdLib//NO_0", "NO_1");
                StdXml.AddChild("//StdLib//NO_1", "NO_2");

                StdXml.AddNode("//StdLib//NO_1", "Node233", "att", null, "innerText");//添加嵌套里的有多个参数的节点测试

                {
                    StdXml.AddNode("//StdLib", "goodXN", "att", null, "Type");//节点删除测试
                    StdXml.AddNode("//StdLib", "badXN", "att", null, "Type");
                    StdXml.RemoveNode("//StdLib", "badXN");//删除节点测试
                    StdXml.RemoveNode("//StdLib//NO_1", "NO_2");//被嵌套节点删除测试
                }
                
                Console.WriteLine(StdXml.ReadNode("//StdLib//NO_0//NO_1//Node233", "_name"));
                Console.WriteLine(StdXml.ReadNode("//StdLib//goodXN", "_value"));
                Console.WriteLine(StdXml.ReadNode("//StdLib//MyNode", "_att", "att"));
            }
            
            Console.ReadKey();
        }
    }

}
