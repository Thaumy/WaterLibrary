using System;//Base Lib
using System.Text;//Base Lib

//using System.Linq;
//using System.Threading.Tasks;

using System.IO;//系统IO命名空间(MCL using)
using System.Xml;//系统XML操作命名空间(MCL using)
using System.Diagnostics;//调用并运行程序所需的命名空间(MCL using)

using System.Collections.Generic;//非泛型集合命名空间(SRR using)

using System.Security.Cryptography;//hash/md5命名空间(ECT using)

using System.Net;//网页操作命名空间


namespace StdLib_GobalnameSpace//StdLib主信息空间
{
    public sealed class StdInf
    {
        public StdInf()
        {
            try
            {
                WebClient Client = new WebClient();//创建WebClient对象
                Stream stm = Client.OpenRead("http://www.thaumy.ml/indexStd/StdWebInf.html");//访问数据网址并用一个流对象来接受返回的流
                StreamReader sr = new StreamReader(stm, Encoding.UTF8);//把流对象转换为StreamReader对象

                newPubs = sr.ReadToEnd().Split('$');
                sr.Close();
                stm.Close();
            }
            catch
            {
                newPubs[0] = "Network or Web Error";//如果出现某些错误，返回这个值
                newPubs[1] = "Network or Web Error";
            }
        }
        public StdInf(string InfUrl)//备用重载
        {
            try
            {
                WebClient Client = new WebClient();
                Stream stm = Client.OpenRead(InfUrl);
                StreamReader sr = new StreamReader(stm, Encoding.UTF8);

                newPubs = sr.ReadToEnd().Split('$');
                sr.Close();
                stm.Close();
            }
            catch
            {
                newPubs[0] = "Network or Web Error";
                newPubs[1] = "Network or Web Error";
            }
        }
        /// <summary>
        /// StdLib的互联网信息
        /// </summary>
        private string[] newPubs = new string[5];//用于缓存网页信息的临时数组变量，目前使用了2个元素，5个元素为支持更多重载提供兼容
        /// <summary>
        /// 主版本
        /// </summary>
        private const ushort Stdver_main = 1;
        /// <summary>
        /// 次版本
        /// </summary>
        private const double Stdver_deta = 0.15;
        /// <summary>
        /// 是否为开发者版本
        /// </summary>
        private const bool devEdition = false;
        /// <summary>
        /// 是否为发行版
        /// </summary>
        private const bool pubEdition = true;
        /// <summary>
        /// 是否采用基本的安全措施
        /// </summary>
        private const bool Base_safe = true;
        /// <summary>
        /// 针对最近一次发行版的全局兼容性
        /// </summary>
        private const bool Gobal_compatible = true;
        

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
        public string Stdver_newPub//最新发行版
        {
            get { return newPubs[0]; }
        }
        public string Stdver_downloadURL//最新发行版的网页下载地址
        {
            get { return newPubs[1]; }
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
    public class Class_Srr
    {
        private int[] ExceptionArray = { 0, 4, 0, 4, 0, 4, 0, 4, 0, 4 };//执行错误所返回的数组
        private ushort i = 0;//循环值，为短整型

        private int[,] buf = new int[10, 10];//初始化矩阵，大小为10x10
        private int[] temp = new int[10];//初始化包含10个坐标的字符串数组
        /// <summary>
        /// 存放生成矩阵的一个非泛型集合类
        /// </summary>
        private List<int[,]> numList = new List<int[,]>();
        

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
            try
            {
                for (int y = 0; y < 10; y++)//y表示纵坐标
                {
                    for (int x = 0; x < dic.Length; x++)//x表示横坐标
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

                numList.Add(buf);/////////////////////////归整泛型

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
            }catch
            {
                return ExceptionArray;
            }
        }//Main_Srr
    }//Class_Srr
}//StdSrr


namespace StdEct//算法加密命名空间(En.cryp.tion)
{
    public static class Class_Hash
    {
        public static string Main_Hash(string InputStr)//散列函数
        {
            try
            {
                var buffer = Encoding.UTF8.GetBytes(InputStr);//将输入字符串转换成字节数组
                var data = SHA1.Create().ComputeHash(buffer);//创建SHA1对象进行散列计算

                var sha = new StringBuilder();//创建一个新的Stringbuilder收集字节
                foreach (var temp in data)//遍历每个字节的散列数据 
                {
                    sha.Append(temp.ToString("X2"));//格式每一个十六进制字符串
                }

                return sha.ToString();//返回
            }catch(Exception e)
            {
                return e.Message;
            }
        }//散列函数
    }//Class_Hash


    public static class Class_MD5
    {
        public static string Main_MD5(string InputStr)//MD5
        {
            try
            {
                var buffer = Encoding.Default.GetBytes(InputStr);
                var data = MD5.Create().ComputeHash(buffer);

                var md5 = new StringBuilder();
                foreach (var temp in data)
                {
                    md5.Append(temp.ToString("X2"));
                }

                return md5.ToString();//返回
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }//MD5
    }//Class_MD5
}//StdEct


namespace StdDal//数据层命名空间
{
    public struct OutPar//用于判定Xml输出类型的结构体
    {
        string path;
        const string name = "op#name";
        const string innerText = "op#value";
        const string att = "op#att";

    }

    public sealed class Class_Mcl//MC启动器数据层
    {

        /// <summary>
        /// 正盗版参数
        /// </summary>
        private bool Sys_copyright;
        /// <summary>
        /// 游戏运行所需的内存值
        /// </summary>
        private uint Sys_memory;
        /// <summary>
        /// 游戏运行的玩家名
        /// </summary>
        private string User_name;
        /// <summary>
        /// 游戏邮箱
        /// </summary>
        private string User_email;
        /// <summary>
        /// 游戏密码
        /// </summary>
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

    public static class Class_Run
    {
        public static bool Run(string file1)//1个重载，启动2个应用
        {
            try
            {
                System.Diagnostics.Process.Start(file1);
                return true;//成功返回true
            }
            catch
            {
                return false;
            }
        }
        public static bool Run(string file1, string file2)//2个重载，启动2个应用
        {
            try
            {
                System.Diagnostics.Process.Start(file1);
                System.Diagnostics.Process.Start(file2);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }//Class_Run

    public sealed class Class_Xml//组标准Xml数据格式读写类
    {
        private string xpath = null;//xml文档地址
        XmlDocument xDoc = new XmlDocument();

        public Class_Xml(string inStream)//使用构造函数初始流指定
        {
            xpath = inStream;
            xDoc.Load(xpath);
        }

        public static bool CreateXml(string file,string name,string rootName)//静态创建Xml表
        {
            try
            {
                XmlDocument newDoc = new XmlDocument();//doc模式读写
                XmlNode node_xml = newDoc.CreateXmlDeclaration("1.0", "utf-8", "");
                newDoc.AppendChild(node_xml);
                XmlNode root = newDoc.CreateElement(rootName);//创建根节点
                newDoc.AppendChild(root);//添加根节点

                newDoc.Save(file + @"\" + name + ".xml");
                return true;
            }
            catch
            {
                return false;
            }
        } 

        public void Stream(string inStream)//后期按需指定流的方法
        {
            xpath = inStream;
            xDoc.Load(xpath);
        }

        public bool AddNode(string path,string nodeName,string AttName,string AttValue,string innerText)//增加实节点方法，path表示被指定的父节点
        {
            try
            {
                XmlNode parentNode = xDoc.SelectSingleNode(path);//父节点指定
                XmlNode newNode = xDoc.CreateElement(nodeName);//创建新的子节点
                XmlAttribute newAtt = xDoc.CreateAttribute(AttName);//创建用于新的子节点的一个属性，名称为Value的值

                newAtt.Value = AttValue;//属性的值指定
                newNode.Attributes.Append(newAtt);//添加属性到节点

                newNode.InnerText = innerText;

                parentNode.AppendChild(newNode);//在父节点上添加该节点
                xDoc.Save(xpath);//保存到xpath
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool AddChild(string parentNode_Name,string childNode_Name)//添加子节点的方法
        {
            try
            {
                XmlNode pxn = xDoc.SelectSingleNode(parentNode_Name);
                XmlNode nxn = xDoc.CreateElement(childNode_Name);
                pxn.AppendChild(nxn);
                xDoc.Save(xpath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool RemoveNode(string path,string nodeID)//删除某节点下指定子节点的方法，path表示被指定的父节点
        {
            try
            {
                XmlNode baseNode = xDoc.SelectSingleNode(path);//指定父节点
                XmlNodeList xnList = baseNode.ChildNodes;//初始化父节点的子节点列
                foreach(XmlNode n in xnList)//遍历每一个节点
                {
                    if (n.Name == nodeID)//判断节点名
                    {
                        baseNode.RemoveChild(n);

                        xDoc.Save(xpath);
                        break;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string ReadNode(string path,string type)//根据type的类型判断要获取的值
        {
            try
            {
                XmlNode xn = xDoc.SelectSingleNode(path);
                switch (type)
                {
                    case "_name":
                        return xn.Name;
                    case "_value":
                        return xn.InnerText;
                    default:
                        return "Main_ERROR";
                }
            }
            catch
            {
                return "XML_ERROR";
            }
        }

        public string ReadNode(string path, string type, string attName)
        {
            try
            {
                XmlNode xn = xDoc.SelectSingleNode(path);
                if (type == "_att")
                {
                    return xn.Attributes[attName].Value;
                }
                else
                {
                    return "Main_ERROR";
                }
            }
            catch
            {
                return "XML_ERROR";
            }
        }
    }//Class_Xml
}//StdDBA