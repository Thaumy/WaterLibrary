using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Xml;

namespace StdLib
{
    public sealed class SrrLib
    {
        private int x;
        private int y = -1;
        private int i = 0;

        private int[,] buf = new int[10, 10];//初始化矩阵，大小为10x10
        private String[] temp = new string[10];//初始化包含10个坐标的字符串数组
        private string outputs = null;//初始化坐标数列

        public string srr
            (
            int[] sar/*密码，10个数*/,
            int[] dic/*int[] dic = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };字典集，一共10个字符*/,
            int[] num/*int[] num = { 8, 4, 5, 5, 5, 3, 7, 9, 2, 9 }; 被加密的10个整数*/
            )

        {
            

            for (x = 0; x < 10; x++)
            {
                foreach (int d in dic)
                {

                    y++;
                    if (y > 10 - sar[x])
                    {
                        i++;
                    }
                    if (y < 10 - sar[x])
                    {
                        buf[x, y] = dic[y + sar[x]];
                        Console.Write(buf[x, y]);//逐个输出位移数
                    }
                    else
                    {
                        buf[x, y] = dic[i];
                        Console.Write(buf[x, y]);//逐个输出位移数
                    }
                }
                i = 0;
                y = -1;
                Console.WriteLine();//满10换行
            }

            //单坐标生成↓////////////////////////////////////////////////////////////////////////////////////////////////////////////

            for (int p = 0; p < 10; p++)
            {
                for (i = 0; i < 10; i++)
                {
                    if (buf[p, i] == num[p])
                    {

                        temp[p] = "[" + p.ToString() + i.ToString() + "]";//单坐标构成组坐标

                    }
                }
            }

            //组坐标处理↓
            

            for (int p = 0; p < 10; p++)
            {
                outputs = outputs + temp[p];
            }

            return outputs;//返回坐标数列
        }



    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public sealed class Getit
    {
        public class Get
        {
            /*正盗版判断*/
            public bool b()
            {
                string l = this.GetLines(4);
                if (l == "1")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            /*正盗版判断*/

            public string GetLines(int i)
            {
                XmlDocument settings = new XmlDocument();
                settings.Load(Directory.GetCurrentDirectory()+@"\settings.xml");//加载配置文件

                XmlNode name = settings.SelectSingleNode("//usersDATA//NAME");
                XmlNode mail = settings.SelectSingleNode("//usersDATA//MAIL");
                XmlNode password = settings.SelectSingleNode("//usersDATA//PASSWORD");
                XmlNode ram = settings.SelectSingleNode("//runDATA//RAM");
                XmlNode me = settings.SelectSingleNode("//ME");
                
                if (i == 0)
                {
                    return name.InnerText;
                }
                if (i == 1)
                {
                    
                    return ram.InnerText;
                }
                if (i == 2)
                {
                    return password.InnerText;
                }
                if (i == 3)
                {
                    return mail.InnerText;
                }
                if (i == 4)
                {
                    return me.InnerText;
                }
                else
                {
                    return null;
                }
                //line[0]代表游戏名称
                //line[1]代表运行内存
                //line[2]代表正版密码
                //line[3]代表正版邮箱

                //line[4]代表是否选择正版<!--注意:仅限内部类访问-->
            }

            public void save(string name, string ram, string password, string mail, string me)
            {
                XmlDocument settings = new XmlDocument();
                settings.Load(System.IO.Directory.GetCurrentDirectory() + @"\settings.xml");//加载配置文件

                XmlNode Xml_name = settings.SelectSingleNode("//usersDATA//NAME");
                XmlNode Xml_mail = settings.SelectSingleNode("//usersDATA//MAIL");
                XmlNode Xml_password = settings.SelectSingleNode("//usersDATA//PASSWORD");
                XmlNode Xml_ram = settings.SelectSingleNode("//runDATA//RAM");
                XmlNode Xml_me = settings.SelectSingleNode("//ME");

                Xml_name.InnerText = name;
                Xml_mail.InnerText = mail;
                Xml_password.InnerText = password;
                Xml_ram.InnerText = ram;
                Xml_me.InnerText = me;  

                settings.Save(System.IO.Directory.GetCurrentDirectory() + @"\settings.xml");
            }
        }
    }
}
namespace StdLib_inf
{
    public sealed class inf
    {
        private const int libver_total = 1;//总版本
        public const double libver_deta = 0.13;//发行版本（副版本）
        //
        public int Libver_total
        {
            get { return libver_total; }
        }
        public double Libver_deta
        {
            get { return libver_deta; }
        }
        //

        private const bool test = false;//是否为内部测试版
        public bool Test
        {
            get { return test; }
        }
        /////////////////////////////////////////////////////////////////版本

        private const bool gobal_safe = true;//是否全局安全
        private const bool gobal_past = true;//是否对最后一次发行版兼容
        
        public bool gobal_Safe
        {
            get { return gobal_safe; }
        }
        public bool gobal_Past
        {
            get { return gobal_past; }
        }
        ////////////////////////////////////////////////////////////////兼容性

        private const string devinf = "ArcaneinterstellarStudios";//开发者信息

        public string devInf
        {
            get { return devinf; }
        }
        //////////////////////////////////////////////////////////////其他信息
    }
}