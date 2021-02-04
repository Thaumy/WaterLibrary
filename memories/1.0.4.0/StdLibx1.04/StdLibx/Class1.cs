using System;
using System.Text;

//using System.Linq;
//using System.Threading.Tasks;


using System.IO;//系统IO命名空间
using System.Xml;//系统XML操作命名空间
using System.Collections.Generic;//非泛型集合命名空间

namespace StdLib_GobalnameSpace//StdLib主空间
{
    public sealed class StdInf
    {
        private const ushort Stdver_main = 1;//主版本
        private const double Stdver_deta= 0.14;//次版本

        private const bool devEdition = false;//是否为开发者版本
        private const bool pubEdition = true;//是否为发行版

        private const bool Base_safe = true;//是否采用基本的安全措施
        private const bool Gobal_compatible = false;//针对最近一次发行版的全局兼容性

        public int Stdver_Main//主版本
        {
            get { return Stdver_main; }
        }
        public double Stdver_Deta//次版本
        {
            get { return Stdver_deta; }
        }
        public bool DevEdition//开发者版本
        {
            get { return devEdition; }
        }
        public bool PubEdition//发行版
        {
            get { return pubEdition; }
        }
        public bool Base_Safe//基本安全
        {
            get { return Base_safe; }
        }
        public bool Gobal_Compatible//全局兼容
        {
            get { return Gobal_compatible; }
        }

    }//StdInf
}//StdLib_GobalMain



namespace StdSrr//矩阵位移算法命名空间
{
    public sealed class Class_Srr
    {
        private ushort i = 0;//循环值，为短整型

        private int[,] buf = new int[10, 10];//初始化矩阵，大小为10x10
        private int[] temp = new int[10];//初始化包含10个坐标的字符串数组

        private List<int[,]> numList = new List<int[,]>();//一个非泛型集合类用于存放矩阵

        public List<int[,]> NumList//矩阵的访问器
        {
            get { return numList; }//如果算法还没处理就直接取用的话取不到
        }


        public int[] Main_Srr//算法主方法，返回值为temp数组
            (
            int[] psw/*密码，10个数*/,
            int[] dic/*int[] dic = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };字典集，一共10个字符*/,
            int[] num/*int[] num = { 8, 4, 5, 5, 5, 3, 7, 9, 2, 9 }; 被加密的10个整数*/
            )
            //传参部分
        {
            for (int y = 0; y < 10; y++)//y表示纵坐标
            {
                for(int x= 0;x < dic.Length; x++)//x表示横坐标
                {
                    if (x > 10 - psw[y]) { i++; }

                    if (x < 10 - psw[y])
                    {
                        buf[y, x] = dic[x + psw[y]];

                        Console.Write(buf[y, x]);//逐个输出位移数
                    }
                    else
                    {
                        buf[y, x] = dic[i];

                        Console.Write(buf[y, x]);//逐个输出位移数
                    }
                }
                i = 0;

                Console.WriteLine();//满10换行
            }
            numList.Add(buf);//归整泛型

            //循环以检索矩阵中符合要求的值
            for (int p = 0; p < 10; p++)
            {
                for (i = 0; i < 10; i++)
                {
                    if (buf[p, i] == num[p])
                    {
                        temp[p] = i * 10 + p;//把横纵坐标联合构成单坐标
                    }
                }
            }
            return temp;//返回temp数组
                        //假设其中一个值为29，那么它的坐标即为(2,9)，假如出现了单数，设其值为6，则坐标为(0,6)
        }//Main_Srr
    }//Class_Srr
}//StdSrr



namespace StdMcl//MC启动器数据层命名空间
{
    public sealed class Class_Mcl
    {

        private bool Sys_copyright;
        private uint Sys_memory;

        private string User_name;
        private string User_email;
        private string User_passWord;

        public Class_Mcl()//用构造函数初始化运行数据
        {
            XmlDocument runInf = new XmlDocument();//使用DOM方式读写xml文件

            runInf.Load(Directory.GetCurrentDirectory() + @"\Inf.xml");//加载Inf.xml
            //提取运行信息
            XmlNode copyright = runInf.SelectSingleNode("//SystemInf//copyright");
            XmlNode memory = runInf.SelectSingleNode("//SystemInf//memory");
            //提取用户信息
            XmlNode name = runInf.SelectSingleNode("//UserInf//name");
            XmlNode email = runInf.SelectSingleNode("//UserInf//email");
            XmlNode passWord = runInf.SelectSingleNode("//UserInf//passWord");

            if (copyright.InnerText == "false")//InnerText除了0以外Sys_copyright都是true
            {
                Sys_copyright = false;
            }
            else
            {
                Sys_copyright = true;
            }

            Sys_memory = Convert.ToUInt16(memory.InnerText);//初始化运行内存
            User_name = name.InnerText;//初始化玩家名
            User_email = email.InnerText;//初始化邮箱
            User_passWord = passWord.InnerText;//初始化密码
        }

        //信息访问器
        public bool Sys_Copyright//返回版权参数
        {
            get { return Sys_copyright; }
        }
        public int Sys_Memory//返回运行内存
        {
            get { return Convert.ToInt16(Sys_memory); }
        }
        public string User_Name//返回玩家名
        {
            get { return User_name; }
        }
        public string User_Email//返回邮箱名
        {
            get { return User_email; }
        }
        public string User_PassWord//返回密码
        {
            get { return User_passWord; }
        }


        public void Mcl_Save( bool Saved_copyright, ushort Saved_memory, string Saved_name, string Saved_email, string Saved_passWord )
        {
            XmlDocument runInf = new XmlDocument();//使用DOM方式读写xml文件

            runInf.Load(Directory.GetCurrentDirectory() + @"\Inf.xml");//加载Inf.xml

            //将各数据源位置确定
            XmlNode copyright = runInf.SelectSingleNode("//SystemInf//copyright");
            XmlNode memory = runInf.SelectSingleNode("//SystemInf//memory");
            XmlNode name = runInf.SelectSingleNode("//UserInf//name");
            XmlNode email = runInf.SelectSingleNode("//UserInf//email");
            XmlNode passWord = runInf.SelectSingleNode("//UserInf//passWord");

            //更改各数据源
            copyright.InnerText = Saved_copyright.ToString();//版权
            memory.InnerText = Saved_memory.ToString();//内存
            name.InnerText = Saved_name;//玩家名
            email.InnerText = Saved_email;//邮箱
            passWord.InnerText = Saved_passWord;//密码
            
            //更新参数信息
            Sys_copyright = Saved_copyright;
            Sys_memory = Saved_memory;

            //更新用户信息
            User_name = Saved_name;
            User_email = Saved_email;
            User_passWord = Saved_passWord;

            runInf.Save(System.IO.Directory.GetCurrentDirectory() + @"\Inf.xml");//保存

        }//Mcl_Save
    }//Class_Mcl
}//StdMcl