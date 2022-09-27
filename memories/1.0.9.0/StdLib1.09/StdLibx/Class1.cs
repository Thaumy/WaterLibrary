#region 被引用的命名空间
using System;//Base Lib
using System.Text;//Base Lib

using System.IO;//系统IO命名空间(MCL using)
using System.Xml;//系统XML操作命名空间(MCL using)
using System.Collections;//泛型集合类命名空间
using System.Collections.Generic;//非泛型集合命名空间
using System.Security.Cryptography;//hash/md5命名空间
using System.Net;//网页操作命名空间
using System.Drawing;//GDI+命名空间
using MySql.Data.MySqlClient;//MySql数据库命名空间
using System.Data;//ADO.NET类结构访问命名空间
using System.Web.UI;
#endregion

namespace StdLib//StdLib1.09
{
    namespace RevisionControl//版本控制
    {
        /// <summary>
        /// 版本控制限定
        /// </summary>
        public interface ILibInformation
        {
            /// <summary>
            /// 主版本
            /// </summary>
            int verFirst
            {
                get;
            }
            /// <summary>
            /// 次版本
            /// </summary>
            double verSecond
            {
                get;
            }
            /// <summary>
            /// 是否为开发者版本
            /// </summary>
            bool devEdition
            {
                get;
            }
            /// <summary>
            /// 是否为发行版
            /// </summary>
            bool pubEdition
            {
                get;
            }
            /// <summary>
            /// 联网时的最新发行版
            /// </summary>
            string webVer
            {
                get;
            }
            /// <summary>
            /// 联网最新发行版的下载URL
            /// </summary>
            string webURL
            {
                get;
            }
            /// <summary>
            /// 针对最近一次发行版的全局兼容性
            /// </summary>
            bool compatibleLast
            {
                get;
            }
            /// <summary>
            /// 获取到像素化的StdLib_logo
            /// </summary>
            Bitmap logo
            {
                get;
            }
            /// <summary>
            /// 开发者必要留言
            /// </summary>
            string devWords
            {
                get;
            }
        }
        /// <summary>
        /// 用于获取类库信息的类
        /// </summary>
        public sealed class LibInformation : ILibInformation
        {

            /// <summary>
            /// 获取StdLib联网信息
            /// </summary>
            public LibInformation()
            {
                try
                {
                    WebClient Client = new WebClient();//创建WebClient对象
                    Stream stm = Client.OpenRead("http://datarep.thaumy.ml/StdLib/index.html");//访问网址并用一个流对象来接受返回的流
                    StreamReader sr = new StreamReader(stm, Encoding.UTF8);//把流对象转换为StreamReader对象

                    WebInformation = sr.ReadToEnd().Split('$');
                    sr.Close();
                    stm.Close();
                }
                catch
                {
                    WebInformation[0] = "StdLibError ec7540";//报错
                    WebInformation[1] = "StdLibError ec7541";//报错
                }
            }

            /// <summary>
            /// 获取StdLib联网信息(第二重载，不兼容时使用)
            /// </summary>
            /// <param name="InfUrl">URL指定</param>
            public LibInformation(string InfUrl)
            {
                try
                {
                    WebClient Client = new WebClient();
                    Stream stm = Client.OpenRead(InfUrl);
                    StreamReader sr = new StreamReader(stm, Encoding.UTF8);

                    WebInformation = sr.ReadToEnd().Split('$');
                    sr.Close();
                    stm.Close();
                }
                catch
                {
                    WebInformation[0] = "StdLibError ec7480";
                    WebInformation[1] = "StdLibError ec7481";
                }
            }

            #region 类库私有信息

            private string[] WebInformation = new string[2];//用于加载网页信息的临时数组，2个元素
            private const ushort VerFirst = 1;
            private const double VerSecond = 0.19;
            private const bool DevEdition = false;
            private const bool PubEdition = true;
            private const bool CompatibleLast = false;
            private const string DevWords = "StdLib的网络发行版，专注于新型架构的Blog";

            #endregion

            #region 类库信息访问器

            /// <summary>
            /// 主版本
            /// </summary>
            public int verFirst
            {
                get { return VerFirst; }
            }
            /// <summary>
            /// 次版本
            /// </summary>
            public double verSecond
            {
                get { return VerSecond; }
            }
            /// <summary>
            /// 是否为开发者版本
            /// </summary>
            public bool devEdition
            {
                get { return DevEdition; }
            }
            /// <summary>
            /// 是否为发行版
            /// </summary>
            public bool pubEdition
            {
                get { return PubEdition; }
            }
            /// <summary>
            /// 联网时的最新发行版
            /// </summary>
            public string webVer
            {
                get { return WebInformation[0]; }
            }
            /// <summary>
            /// 联网最新发行版的下载URL
            /// </summary>
            public string webURL
            {
                get { return WebInformation[1]; }
            }
            /// <summary>
            /// 针对最近一次发行版的全局兼容性
            /// </summary>
            public bool compatibleLast
            {
                get { return CompatibleLast; }
            }
            /// <summary>
            /// 获取到像素化的StdLib_logo
            /// </summary>
            public Bitmap logo
            {
                get { return StdLibx.Resource1.StdLib_logo; }
            }
            /// <summary>
            /// 开发者必要留言
            /// </summary>
            public string devWords
            {
                get { return DevWords; }
            }

            #endregion
        }

    }

    namespace Math//运算
    {
        /// <summary>
        /// 加密算法类
        /// </summary>
        public sealed class Encryptor
        {
            private delegate string MD5Handler(string Str);//声明用于toMD5的委托
            /// <summary>
            /// MD5方法
            /// </summary>
            /// <param name="str">被加密的字符串</param>
            /// <returns>通常返回MD5加密结果，报错则返回错误信息</returns>
            public string md5(string str)
            {
                MD5Handler Mh = new MD5Handler(toMD5);
                IAsyncResult result = Mh.BeginInvoke(str, null, null);

                return Mh.EndInvoke(result);
            }//兼容方法
            private string toMD5(string input_str)
            {
                try
                {
                    var buffer = Encoding.Default.GetBytes(input_str);
                    var data = MD5.Create().ComputeHash(buffer);

                    var md5 = new StringBuilder();
                    foreach (var temp in data)
                    {
                        md5.Append(temp.ToString("X2"));
                    }

                    return md5.ToString();//返回
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }

            private delegate string HashHandler(string Str);//声明用于toHash的委托
            /// <summary>
            /// 散列方法
            /// </summary>
            /// <param name="str">被加密的字符串</param>
            /// <returns>通常返回散列加密结果，报错则返回错误信息</returns>
            public string hash(string str)
            {
                HashHandler Hh = new HashHandler(toHash);
                IAsyncResult result = Hh.BeginInvoke(str, null, null);

                return Hh.EndInvoke(result);
            }//兼容方法
            private string toHash(string input_str)
            {
                try
                {
                    var buffer = Encoding.UTF8.GetBytes(input_str);//将输入字符串转换成字节数组
                    var data = SHA1.Create().ComputeHash(buffer);//创建SHA1对象进行散列计算

                    var sha = new StringBuilder();//创建一个新的Stringbuilder收集字节
                    foreach (var temp in data)//遍历每个字节的散列数据 
                    {
                        sha.Append(temp.ToString("X2"));//格式每一个十六进制字符串
                    }

                    return sha.ToString();//返回
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }
        }

        /// <summary>
        /// 矩阵算法类
        /// </summary>
        public sealed class ArrayAlgorithm
        {
            private int[] ExceptionValue = { 2390 };//执行错误所返回的数组

            private int[,] buf = new int[10, 10];//初始化矩阵，大小为10x10
            private int[] temp = new int[10];//初始化包含10个坐标的字符串数组

            /// <summary>
            /// 存放生成的矩阵
            /// </summary>
            private List<int[,]> NumArray = new List<int[,]>();

            /// <summary>
            /// 矩阵的引用访问器
            /// </summary>
            public List<int[,]> numArray
            {
                get { return NumArray; }//如果算法还没处理就直接取用的话取到的是空的List
            }

            /// <summary>
            /// 提取矩阵有效信息索引的方法
            /// </summary>
            /// <param name="psw">密码(int):共10个整数</param>
            /// <param name="dic">int[] dic = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };:字典集，一共10个字符</param>
            /// <param name="num">int[] num = { 8, 4, 5, 5, 5, 3, 7, 9, 2, 9 };:被加密的10个整数</param>
            /// <returns>通常返回索引数组，报错则返回内容为-2的数组</returns>
            public int[] getIndex(int[] psw, int[] dic, int[] num)
            {
                ushort i = 0;//循环值
                try
                {
                    #region 生成矩阵
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
                    #endregion

                    NumArray.Add(buf);//给NumArray添加元素

                    #region 循环以检索矩阵中符合要求的值
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
                    #endregion

                    return temp;//返回temp数组

                    //假设其中一个值为29，那么它的坐标即为(2,9)，假如出现了单数，设其值为6，则坐标为(0,6)
                }
                catch
                {
                    return ExceptionValue;
                }
            }//Main_Srr

        }

        /// <summary>
        /// PixelGraphic转码类
        /// </summary>
        public sealed class PixelGraphic
        {
            //异步多线程解码
            private delegate string dePixelsHandler(string stream);//声明用于dePixels委托
            /// <summary>
            /// PixelGraphic解码方法
            /// </summary>
            /// <param name="stream">文件流指定</param>
            /// <returns>通常返回解码结果，报错则返回"StdLibError ec4580"</returns>
            public string dePixels(string stream)
            {
                dePixelsHandler deHandler = new dePixelsHandler(de);//建立委托deHandler
                IAsyncResult result = deHandler.BeginInvoke(stream, null, null);//异步请求结果

                return deHandler.EndInvoke(result);//返回异步结果
            }//兼容方法

            //异步多线程编码
            private delegate Bitmap toPixelsHandler_normal(string hex, string stream);//声明用于hexToBMP的委托
            /// <summary>
            /// PixelGraphic加密方法:(第一重载)注意hex至少由4个16进制字符组成
            /// </summary>
            /// <param name="hex">被加密的16进制text(不带空格)</param>
            /// <param name="stream">bmp模板流</param>
            /// <returns>通常返回编译后的bmp，报错则返回null</returns>
            public Bitmap toPixels(string hex, string stream)
            {
                toPixelsHandler_normal toHandler = new toPixelsHandler_normal(hexToBMP);//建立委托toHandler
                IAsyncResult result = toHandler.BeginInvoke(hex, stream, null, null);

                return toHandler.EndInvoke(result);
            }//兼容方法

            private delegate Bitmap toPixelsHandler_pro(object obj, string stream, string type);//声明用于_allToBMP的委托
            /// <summary>
            /// PixelGraphic加密方法:(第二重载)注意hex至少由4个16进制字符组成
            /// </summary>
            /// <param name="obj">被加密的16进制obj:text(不带空格)或者_hex(带空格)或者str[,](hex矩阵)/</param>
            /// <param name="stream">bmp模板流</param>
            /// <param name="type"></param>
            /// <returns>通常返回编译后的bmp，报错则返回null</returns>
            public Bitmap toPixels(object obj, string stream, string type)
            {
                toPixelsHandler_pro toHandler = new toPixelsHandler_pro(allToBMP);//建立委托toHandler
                IAsyncResult result = toHandler.BeginInvoke(obj, stream, type, null, null);

                return toHandler.EndInvoke(result);
            }//兼容方法


            //用于对图像解码的方法
            /// <summary>
            /// 解码ANSW图像
            /// </summary>
            /// <param name="stream">包含待解码BMP图像的物理路径</param>
            /// <returns>返回解码后的16进制数据，报错则返回"StdLibError ec4580"</returns>
            private string de(string stream)
            {
                try
                {
                    Bitmap bp = new Bitmap(stream);//从文件流获取bmp
                    Color c = new Color();
                    string hex = "";

                    #region 解码部分
                    for (int y = 0; y < 100; y++)
                    {
                        for (int x = 0; x < 100; x++)
                        {
                            c = bp.GetPixel(x, y);
                            switch (c.R)
                            {
                                case 000: hex += "0"; break;
                                case 010: hex += "1"; break;
                                case 020: hex += "2"; break;
                                case 030: hex += "3"; break;
                                case 040: hex += "4"; break;
                                case 050: hex += "5"; break;
                                case 060: hex += "6"; break;
                                case 070: hex += "7"; break;
                                case 080: hex += "8"; break;
                                case 090: hex += "9"; break;
                                case 100: hex += "a"; break;
                                case 110: hex += "b"; break;
                                case 120: hex += "c"; break;
                                case 130: hex += "d"; break;
                                case 140: hex += "e"; break;
                                case 150: hex += "f"; break;
                            }
                        }
                    }
                    #endregion

                    return hex;
                }
                catch
                {
                    return "StdLibError ec4580";
                }
            }

            //用于编译图像的方法
            /// <summary>
            /// 通过选择根据方式来编译ANSW
            /// </summary>
            /// <param name="obj">含有16进制数据的实例</param>
            /// <param name="stream">初始BMP图片模板的物理路径</param>
            /// <param name="type">编译类型，可选填项有："hex"（根据不带空格的16进制文本编译ANSW）、"_hex"（根据带有空格的16进制文本编译ANSW）、"array"（根据16进制文本数据矩阵编译ANSW）</param>
            /// <returns>返回Bitmap实例，错误则返回null</returns>
            private Bitmap allToBMP(object obj, string stream, string type)
            {
                try
                {
                    switch (type)
                    {
                        case "hex": return hexToBMP(obj.ToString(), stream);//根据不带空格的16进制文本编译ANSW
                        case "_hex": return _hexToBMP(obj.ToString(), stream);//根据带有空格的16进制文本编译ANSW
                        case "array": return arrayToBMP((string[,])obj, stream);//根据16进制文本数据矩阵编译ANSW
                        default: return null;
                    }
                }
                catch
                {
                    return null;
                }
            }
            /// <summary>
            /// 根据不带空格的16进制文本编译ANSW
            /// </summary>
            /// <param name="hex">不带空格的16进制文本</param>
            /// <param name="stream">初始BMP图片模板的物理路径</param>
            /// <returns>返回Bitmap实例，错误则返回null</returns>
            private Bitmap hexToBMP(string hex, string stream)
            {
                try
                {
                    //初始化
                    Bitmap outputBP = new Bitmap(stream);
                    ushort i = 0;//临时变量
                    int len = hex.Length;
                    string[,] hexArray = new string[100, 100];

                    #region 将矩阵记空
                    for (int y = 0; y < 100; y++)
                    {
                        for (int x = 0; x < 100; x++)
                        {
                            hexArray[x, y] = null;//循环记空
                        }
                    }
                    #endregion

                    #region 将字符串加空格
                    if (len <= 10000)
                    {
                        for (int a = 1; a < len; a++)
                        {
                            i++;
                            hex = hex.Insert(a + i - 1, " ");
                        }
                    }
                    else
                    {
                        for (int a = 1; a < 10000; a++)
                        {
                            i++;
                            hex = hex.Insert(a + i - 1, " ");
                        }
                    }

                    #endregion

                    #region 把空格字符串变为矩阵
                    i = 0;//临时变量归零
                    for (int y = 0; y < 100; y++)
                    {
                        for (int x = 0; x < 100; x++)
                        {

                            if (i < hex.Split(' ').Length)
                            {
                                hexArray[x, y] = hex.Split(' ')[i];
                            }
                            i++;
                        }
                    }
                    #endregion

                    #region 根据矩阵的值生成bmp图像
                    for (int y = 0; y < 100; y++)
                    {

                        for (int x = 0; x < 100; x++)
                        {
                            switch (hexArray[x, y])
                            {
                                case "0": outputBP.SetPixel(x, y, Color.FromArgb(000, 0, 0)); break;
                                case "1": outputBP.SetPixel(x, y, Color.FromArgb(010, 0, 0)); break;
                                case "2": outputBP.SetPixel(x, y, Color.FromArgb(020, 0, 0)); break;
                                case "3": outputBP.SetPixel(x, y, Color.FromArgb(030, 0, 0)); break;
                                case "4": outputBP.SetPixel(x, y, Color.FromArgb(040, 0, 0)); break;
                                case "5": outputBP.SetPixel(x, y, Color.FromArgb(050, 0, 0)); break;
                                case "6": outputBP.SetPixel(x, y, Color.FromArgb(060, 0, 0)); break;
                                case "7": outputBP.SetPixel(x, y, Color.FromArgb(070, 0, 0)); break;
                                case "8": outputBP.SetPixel(x, y, Color.FromArgb(080, 0, 0)); break;
                                case "9": outputBP.SetPixel(x, y, Color.FromArgb(090, 0, 0)); break;
                                case "a": outputBP.SetPixel(x, y, Color.FromArgb(100, 0, 0)); break;
                                case "b": outputBP.SetPixel(x, y, Color.FromArgb(110, 0, 0)); break;
                                case "c": outputBP.SetPixel(x, y, Color.FromArgb(120, 0, 0)); break;
                                case "d": outputBP.SetPixel(x, y, Color.FromArgb(130, 0, 0)); break;
                                case "e": outputBP.SetPixel(x, y, Color.FromArgb(140, 0, 0)); break;
                                case "f": outputBP.SetPixel(x, y, Color.FromArgb(150, 0, 0)); break;
                                case "A": outputBP.SetPixel(x, y, Color.FromArgb(100, 0, 0)); break;
                                case "B": outputBP.SetPixel(x, y, Color.FromArgb(110, 0, 0)); break;
                                case "C": outputBP.SetPixel(x, y, Color.FromArgb(120, 0, 0)); break;
                                case "D": outputBP.SetPixel(x, y, Color.FromArgb(130, 0, 0)); break;
                                case "E": outputBP.SetPixel(x, y, Color.FromArgb(140, 0, 0)); break;
                                case "F": outputBP.SetPixel(x, y, Color.FromArgb(150, 0, 0)); break;
                            }
                        }
                    }
                    #endregion

                    return outputBP;
                }
                catch
                {
                    return null;
                }
            }
            /// <summary>
            /// 根据带有空格的16进制文本编译ANSW
            /// </summary>
            /// <param name="_hex">带有空格的16进制文本</param>
            /// <param name="stream">初始BMP图片模板的物理路径</param>
            /// <returns>返回Bitmap实例，错误则返回null</returns>
            private Bitmap _hexToBMP(string _hex, string stream)
            {
                try
                {
                    //初始化
                    Bitmap outputBP = new Bitmap(stream);
                    ushort i = 0;
                    string[,] hexArray = new string[100, 100];

                    #region 将矩阵记空
                    for (int y = 0; y < 100; y++)
                    {
                        for (int x = 0; x < 100; x++)
                        {
                            hexArray[x, y] = null;//循环记空
                        }
                    }
                    #endregion

                    #region 把空格字符串变为矩阵
                    for (int y = 0; y < 100; y++)
                    {
                        for (int x = 0; x < 100; x++)
                        {

                            if (i < _hex.Split(' ').Length)
                            {
                                hexArray[x, y] = _hex.Split(' ')[i];
                            }
                            i++;
                        }
                    }
                    #endregion

                    #region 根据矩阵的值生成bmp图像
                    for (int y = 0; y < 100; y++)
                    {
                        for (int x = 0; x < 100; x++)
                        {
                            switch (hexArray[x, y])
                            {
                                case "0": outputBP.SetPixel(x, y, Color.FromArgb(000, 0, 0)); break;
                                case "1": outputBP.SetPixel(x, y, Color.FromArgb(010, 0, 0)); break;
                                case "2": outputBP.SetPixel(x, y, Color.FromArgb(020, 0, 0)); break;
                                case "3": outputBP.SetPixel(x, y, Color.FromArgb(030, 0, 0)); break;
                                case "4": outputBP.SetPixel(x, y, Color.FromArgb(040, 0, 0)); break;
                                case "5": outputBP.SetPixel(x, y, Color.FromArgb(050, 0, 0)); break;
                                case "6": outputBP.SetPixel(x, y, Color.FromArgb(060, 0, 0)); break;
                                case "7": outputBP.SetPixel(x, y, Color.FromArgb(070, 0, 0)); break;
                                case "8": outputBP.SetPixel(x, y, Color.FromArgb(080, 0, 0)); break;
                                case "9": outputBP.SetPixel(x, y, Color.FromArgb(090, 0, 0)); break;
                                case "a": outputBP.SetPixel(x, y, Color.FromArgb(100, 0, 0)); break;
                                case "b": outputBP.SetPixel(x, y, Color.FromArgb(110, 0, 0)); break;
                                case "c": outputBP.SetPixel(x, y, Color.FromArgb(120, 0, 0)); break;
                                case "d": outputBP.SetPixel(x, y, Color.FromArgb(130, 0, 0)); break;
                                case "e": outputBP.SetPixel(x, y, Color.FromArgb(140, 0, 0)); break;
                                case "f": outputBP.SetPixel(x, y, Color.FromArgb(150, 0, 0)); break;
                                case "A": outputBP.SetPixel(x, y, Color.FromArgb(100, 0, 0)); break;
                                case "B": outputBP.SetPixel(x, y, Color.FromArgb(110, 0, 0)); break;
                                case "C": outputBP.SetPixel(x, y, Color.FromArgb(120, 0, 0)); break;
                                case "D": outputBP.SetPixel(x, y, Color.FromArgb(130, 0, 0)); break;
                                case "E": outputBP.SetPixel(x, y, Color.FromArgb(140, 0, 0)); break;
                                case "F": outputBP.SetPixel(x, y, Color.FromArgb(150, 0, 0)); break;
                            }
                        }
                    }
                    #endregion

                    return outputBP;
                }
                catch
                {
                    return null;
                }
            }
            /// <summary>
            /// 根据16进制文本数据矩阵编译ANSW
            /// </summary>
            /// <param name="hexArray">16进制文本数据矩阵</param>
            /// <param name="stream">初始BMP图片模板的物理路径</param>
            /// <returns>返回Bitmap实例，错误则返回null</returns>
            private Bitmap arrayToBMP(string[,] hexArray, string stream)
            {
                try
                {
                    Bitmap outputBP = new Bitmap(stream);//初始化

                    //根据矩阵的值生成bmp图像
                    for (int y = 0; y < 100; y++)
                    {
                        for (int x = 0; x < 100; x++)
                        {
                            switch (hexArray[x, y])
                            {
                                case "0": outputBP.SetPixel(x, y, Color.FromArgb(000, 0, 0)); break;
                                case "1": outputBP.SetPixel(x, y, Color.FromArgb(010, 0, 0)); break;
                                case "2": outputBP.SetPixel(x, y, Color.FromArgb(020, 0, 0)); break;
                                case "3": outputBP.SetPixel(x, y, Color.FromArgb(030, 0, 0)); break;
                                case "4": outputBP.SetPixel(x, y, Color.FromArgb(040, 0, 0)); break;
                                case "5": outputBP.SetPixel(x, y, Color.FromArgb(050, 0, 0)); break;
                                case "6": outputBP.SetPixel(x, y, Color.FromArgb(060, 0, 0)); break;
                                case "7": outputBP.SetPixel(x, y, Color.FromArgb(070, 0, 0)); break;
                                case "8": outputBP.SetPixel(x, y, Color.FromArgb(080, 0, 0)); break;
                                case "9": outputBP.SetPixel(x, y, Color.FromArgb(090, 0, 0)); break;
                                case "a": outputBP.SetPixel(x, y, Color.FromArgb(100, 0, 0)); break;
                                case "b": outputBP.SetPixel(x, y, Color.FromArgb(110, 0, 0)); break;
                                case "c": outputBP.SetPixel(x, y, Color.FromArgb(120, 0, 0)); break;
                                case "d": outputBP.SetPixel(x, y, Color.FromArgb(130, 0, 0)); break;
                                case "e": outputBP.SetPixel(x, y, Color.FromArgb(140, 0, 0)); break;
                                case "f": outputBP.SetPixel(x, y, Color.FromArgb(150, 0, 0)); break;
                                case "A": outputBP.SetPixel(x, y, Color.FromArgb(100, 0, 0)); break;
                                case "B": outputBP.SetPixel(x, y, Color.FromArgb(110, 0, 0)); break;
                                case "C": outputBP.SetPixel(x, y, Color.FromArgb(120, 0, 0)); break;
                                case "D": outputBP.SetPixel(x, y, Color.FromArgb(130, 0, 0)); break;
                                case "E": outputBP.SetPixel(x, y, Color.FromArgb(140, 0, 0)); break;
                                case "F": outputBP.SetPixel(x, y, Color.FromArgb(150, 0, 0)); break;
                            }
                        }
                    }
                    return outputBP;
                }
                catch
                {
                    return null;
                }
            }

        }

        /// <summary>
        /// 排序算法类
        /// </summary>
        public class Sorter
        {
            /// <summary>
            /// 排序方法
            /// </summary>
            /// <param name="array">被排序的数组</param>
            /// <returns>通常返回有序数组(由小到大)，报错则返回null</returns>
            public T[] easySort<T>(T[] array) where T : IComparable
            {
                try
                {
                    for (int path = 0; path < array.Length; path++)//正被有序的起始位
                    {
                        for (int i = 0; i < array.Length; i++)//临近元素排序
                        {
                            if (i + 1 < array.Length)//元素交换
                            {
                                T tmp; ;
                                if (array[i].CompareTo(array[i + 1]) > 0)
                                {
                                    tmp = array[i];
                                    array[i] = array[i + 1];
                                    array[i + 1] = tmp;
                                }
                            }

                        }
                    }
                    return array;
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 检索类
        /// </summary>
        public class Searcher
        {
            /// <summary>
            /// 二分法检索(第一重载),适用于整型检索
            /// </summary>
            /// <param name="value">被检索值</param>
            /// <param name="array">数组,顺序由小到大</param>
            /// <returns>若数组存在被检索值,则返回值在数组中的位置,若不存在则返回-1,报错则返回"StdLibError ec5880"</returns>
            public int binarySearch(int value, int[] array)
            {
                try//二分法主体
                {
                    int low = 0;
                    int high = array.Length - 1;
                    while (low <= high)
                    {
                        int mid = (low + high) / 2;

                        if (value == array[mid])
                        {
                            return mid;
                        }
                        if (value > array[mid])
                        {
                            low = mid + 1;
                        }
                        if (value < array[mid])
                        {
                            high = mid - 1;
                        }
                    }
                    return -1;
                }
                catch
                {
                    return 5880;
                }
            }

            /// <summary>
            /// 二分法检索(第二重载),适用于双精度浮点检索
            /// </summary>
            /// <param name="value">被检索值</param>
            /// <param name="array">数组，顺序由小到大</param>
            /// <returns>若数组存在被检索值,则返回值在数组中的位置,若不存在则返回-1,报错则返回"StdLibError ec5881"</returns>
            public double binarySearch(double value, double[] array)
            {
                try//二分法主体
                {
                    double low = 0;
                    double high = array.Length - 1;
                    while (low <= high)
                    {
                        int mid = (int)(low + high) / 2;

                        if (value == array[mid])
                        {
                            return mid;
                        }
                        if (value > array[mid])
                        {
                            low = mid + 1;
                        }
                        if (value < array[mid])
                        {
                            high = mid - 1;
                        }
                    }
                    return -1;
                }
                catch
                {
                    return 5881;
                }
            }
        }

    }
     
    namespace Data//数据
    {
        /// <summary>
        /// 程序启动器
        /// </summary>
        public static class ProgramLoader
        {

            /// <summary>
            /// 第一重载，启动1个应用
            /// </summary>
            /// <param name="file1">应用目录</param>
            /// <returns>通常返回true，报错则返回false</returns>
            public static bool run(string file1)
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
            /// <summary>
            /// 第二重载，启动2个应用
            /// </summary>
            /// <param name="file1">应用目录1</param>
            /// <param name="file2">应用目录2</param>
            /// <returns>通常返回true，报错则返回false</returns>
            public static bool run(string file1, string file2)
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


        }

        /// <summary>
        /// 用于判定Xml输出类型的结构体
        /// </summary>
        public struct xmlStr
        {

            #region 私有属性

            private string Path;
            private string FileName;
            private string XmlName;
            private string RootName;
            private string NodeName;
            private string AttName;
            private string AttValue;
            private string InnerText;
            private string Type;

            #endregion

            #region 属性访问器

            /// <summary>
            /// 节点地址，如父节点、实节点、子节点的地址，用于XmlCreater类中除reStream、CreateXml方法外的所有方法
            /// </summary>
            public string path
            {
                get { return Path; }
                set { Path = value; }
            }
            /// <summary>
            /// 被创建的Xml文档的文件地址，用于XmlCreater类的CreateXml方法
            /// </summary>
            public string fileName
            {
                get { return FileName; }
                set { FileName = value; }
            }
            /// <summary>
            /// 被创建的Xml文档的文件名，用于XmlCreater类的CreateXml方法
            /// </summary>
            public string xmlName
            {
                get { return XmlName; }
                set { XmlName = value; }
            }
            /// <summary>
            /// 被创建的Xml文档的根元素名，用于XmlCreater类的CreateXml方法
            /// </summary>
            public string rootName
            {
                get { return RootName; }
                set { RootName = value; }
            }
            /// <summary>
            /// 节点名，可表示子节点、父节点、新建空\实节点名，用于XmlCreater类的AddRealNode、AddEmptyNode、RemoveNode方法
            /// </summary>
            public string nodeName
            {
                get { return NodeName; }
                set { NodeName = value; }
            }
            /// <summary>
            /// 节点的属性名，用于XmlCreater类的AddRealNode、ReadAtt方法
            /// </summary>
            public string attName
            {
                get { return AttName; }
                set { AttName = value; }
            }
            /// <summary>
            /// 节点的属性值，用于XmlCreater类的AddRealNode方法
            /// </summary>
            public string attValue
            {
                get { return AttValue; }
                set { AttValue = value; }
            }
            /// <summary>
            /// 节点的子文本，用于XmlCreater类的AddRealNode方法
            /// </summary>
            public string innerText
            {
                get { return InnerText; }
                set { InnerText = value; }
            }
            /// <summary>
            /// 读取类型，可选值有"_name"、"_value"，用于XmlCreater类的ReadNode方法
            /// </summary>
            public string type
            {
                get { return Type; }
                set { Type = value; }
            }

            #endregion

        }

        /// <summary>
        /// 组标准Xml数据格式读写类
        /// </summary>
        public sealed class XmlCreater
        {
            //xml文档地址，默认为运行目录的"StdLibx.xml"
            private string xpath = Directory.GetCurrentDirectory() + @"\StdLibx.xml";
            static XmlDocument xDoc = new XmlDocument();

            /// <summary>
            /// 指定流的方法
            /// </summary>
            /// <param name="xStream">文件流地址</param>
            /// <returns>通常返回true，报错则返回false</returns>
            public bool reStream(string xStream)
            {
                try
                {
                    xpath = xStream;
                    xDoc.Load(xpath);
                    return true;
                }
                catch
                {
                    return false;
                }
            }


            /// <summary>
            /// 创建Xml文档的方法（第一重载）
            /// </summary>
            /// <param name="fileName">Xml文档被创建的目录</param>
            /// <param name="xmlName">Xml文档名</param>
            /// <param name="rootName">根节点名</param>
            /// <returns>通常返回true，报错则返回false</returns>
            public bool createXml(string fileName, string xmlName, string rootName)
            {
                try
                {
                    XmlDocument newDoc = new XmlDocument();//doc模式读写
                    XmlNode node_xml = newDoc.CreateXmlDeclaration("1.0", "utf-8", "");
                    newDoc.AppendChild(node_xml);
                    XmlNode root = newDoc.CreateElement(rootName);//创建根节点
                    newDoc.AppendChild(root);//添加根节点

                    newDoc.Save(fileName + @"\" + xmlName + ".xml");
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            /// <summary>
            /// 创建Xml文档的方法（第二重载）
            /// </summary>
            /// <param name="inf">Xml文档信息通用结构体</param>
            /// <returns>通常返回true，报错则返回false</returns>
            public bool createXml(xmlStr xmlStr)
            {
                try
                {
                    XmlDocument newDoc = new XmlDocument();//doc模式读写
                    XmlNode node_xml = newDoc.CreateXmlDeclaration("1.0", "utf-8", "");
                    newDoc.AppendChild(node_xml);
                    XmlNode root = newDoc.CreateElement(xmlStr.rootName);//创建根节点
                    newDoc.AppendChild(root);//添加根节点

                    newDoc.Save(xmlStr.fileName + @"\" + xmlStr.xmlName + ".xml");
                    return true;
                }
                catch
                {
                    return false;
                }
            }


            /// <summary>
            /// 添加实节点的方法（第一重载）
            /// </summary>
            /// <param name="path">被指定的父节点</param>
            /// <param name="nodeName">新建的节点名</param>
            /// <param name="attName">节点的属性</param>
            /// <param name="attValue">节点的属性值</param>
            /// <param name="innerText">节点的子文本</param>
            /// <returns>通常返回true，报错则返回false</returns>
            public bool addRealNode(string path, string nodeName, string attName, string attValue, string innerText)
            {
                try
                {
                    XmlNode parentNode = xDoc.SelectSingleNode(path);//父节点指定
                    XmlNode newNode = xDoc.CreateElement(nodeName);//创建新的子节点
                    XmlAttribute newAtt = xDoc.CreateAttribute(attName);//创建用于新的子节点的一个属性

                    newAtt.Value = attValue;//属性的值指定
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
            /// <summary>
            /// 添加实节点的方法（第二重载）
            /// </summary>
            /// <param name="inf">Xml文档信息通用结构体</param>
            /// <returns>通常返回true，报错则返回false</returns>
            public bool addRealNode(xmlStr xmlStr)
            {
                try
                {
                    XmlNode parentNode = xDoc.SelectSingleNode(xmlStr.path);//父节点指定
                    XmlNode newNode = xDoc.CreateElement(xmlStr.nodeName);//创建新的子节点
                    XmlAttribute newAtt = xDoc.CreateAttribute(xmlStr.attName);//创建用于新的子节点的一个属性

                    newAtt.Value = xmlStr.attValue;//属性的值指定
                    newNode.Attributes.Append(newAtt);//添加属性到节点

                    newNode.InnerText = xmlStr.innerText;

                    parentNode.AppendChild(newNode);//在父节点上添加该节点
                    xDoc.Save(xpath);//保存到xpath
                    return true;
                }
                catch
                {
                    return false;
                }
            }


            /// <summary>
            /// 添加空节点的方法（第一重载）
            /// </summary>
            /// <param name="path">被指定的父节点</param>
            /// <param name="nodeName">新建的空节点名</param>
            /// <returns>通常返回true，报错则返回false</returns>
            public bool addEmptyNode(string path, string nodeName)
            {
                try
                {
                    XmlNode pxn = xDoc.SelectSingleNode(path);
                    XmlNode nxn = xDoc.CreateElement(nodeName);
                    pxn.AppendChild(nxn);
                    xDoc.Save(xpath);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            /// <summary>
            /// 添加空节点的方法（第二重载）
            /// </summary>
            /// <param name="inf">Xml文档信息通用结构体</param>
            /// <returns>通常返回true，报错则返回false</returns>
            public bool addEmptyNode(xmlStr xmlStr)
            {
                try
                {
                    XmlNode Pxn = xDoc.SelectSingleNode(xmlStr.path);
                    XmlNode Cxn = xDoc.CreateElement(xmlStr.nodeName);
                    Pxn.AppendChild(Cxn);
                    xDoc.Save(xpath);
                    return true;
                }
                catch
                {
                    return false;
                }
            }


            /// <summary>
            /// 删除被指定的父节点下子节点的方法（第一重载）
            /// </summary>
            /// <param name="path">被指定的父节点</param>
            /// <param name="nodeName">被删的子节点名</param>
            /// <returns>通常返回true，报错则返回false</returns>
            public bool removeNode(string path, string nodeName)
            {
                try
                {
                    XmlNode baseNode = xDoc.SelectSingleNode(path);//指定父节点
                    XmlNodeList xnList = baseNode.ChildNodes;//初始化父节点的子节点列
                    foreach (XmlNode n in xnList)//遍历每一个节点
                    {
                        if (n.Name == nodeName)//判断节点名
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
            /// <summary>
            /// 删除被指定的父节点下子节点的方法（第二重载）
            /// </summary>
            /// <param name="inf">Xml文档信息通用结构体</param>
            /// <returns>通常返回true，报错则返回false</returns>
            public bool removeNode(xmlStr xmlStr)
            {
                try
                {
                    XmlNode baseNode = xDoc.SelectSingleNode(xmlStr.path);//指定父节点
                    XmlNodeList xnList = baseNode.ChildNodes;//初始化父节点的子节点列
                    foreach (XmlNode n in xnList)//遍历每一个节点
                    {
                        if (n.Name == xmlStr.nodeName)//判断节点名
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


            /// <summary>
            /// 读取被指定的实节点的信息的方法（第一重载）
            /// </summary>
            /// <param name="path">被指定的实节点</param>
            /// <param name="type">被读取的信息类型</param>
            /// <returns>通常返回被读取的信息，传递未知的type返回"StdLibError ec1120"，报错则返回"StdLibError ec1121"</returns>
            public string readInformation(string path, string type)
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
                            return "StdLibError ec1120";
                    }
                }
                catch
                {
                    return "StdLibError ec1121";
                }
            }
            /// <summary>
            /// 读取被指定的实节点的信息的方法（第二重载）
            /// </summary>
            /// <param name="inf">Xml文档信息通用结构体</param>
            /// <returns>通常返回被读取的信息，传递未知的type返回"StdLibError ec1127"，报错则返回"StdLibError ec1128"</returns>
            public string readInformation(xmlStr xmlStr)
            {
                try
                {
                    XmlNode xn = xDoc.SelectSingleNode(xmlStr.path);
                    switch (xmlStr.type)
                    {
                        case "_name":
                            return xn.Name;
                        case "_value":
                            return xn.InnerText;
                        default:
                            return "StdLibError ec1127";
                    }
                }
                catch
                {
                    return "StdLibError ec1128";
                }
            }


            /// <summary>
            /// 读取被指定的实节点的属性值的方法（第一重载）
            /// </summary>
            /// <param name="path">被指定的实节点</param>
            /// <param name="attName">被读值的属性名</param>
            /// <returns>通常返回被读取属性的值，报错则返回"StdLibError ec1440"</returns>
            public string readAttribute(string path, string attName)
            {
                try
                {
                    XmlNode xn = xDoc.SelectSingleNode(path);
                    return xn.Attributes[attName].Value;
                }
                catch
                {
                    return "StdLibError ec1440";
                }
            }
            /// <summary>
            /// 读取被指定的实节点的属性值的方法（第二重载）
            /// </summary>
            /// <param name="inf">Xml文档信息通用结构体</param>
            /// <returns>通常返回被读取属性的值，报错则返回"StdLibError ec1441"</returns>
            public string readAttribute(xmlStr xmlStr)
            {
                try
                {
                    XmlNode xn = xDoc.SelectSingleNode(xmlStr.path);
                    return xn.Attributes[xmlStr.attName].Value;
                }
                catch
                {
                    return "StdLibError ec1441";
                }
            }

        }

        /// <summary>
        /// 数据库连接文本结构体
        /// </summary>
        public struct connStr
        {
            #region 私有属性

            private string Db_name;
            private string DataSource;
            private string Port;
            private string UserId;
            private string PassWord;

            #endregion

            #region 属性访问器

            /// <summary>
            /// 数据库名
            /// </summary>
            public string db_name
            {
                get { return Db_name; }
                set { Db_name = value; }
            }
            /// <summary>
            /// 数据源
            /// </summary>
            public string dataSource
            {
                get { return DataSource; }
                set { DataSource = value; }
            }
            /// <summary>
            /// 端口
            /// </summary>
            public string port
            {
                get { return Port; }
                set { Port = value; }
            }
            /// <summary>
            /// 用户名
            /// </summary>
            public string userId
            {
                get { return UserId; }
                set { UserId = value; }
            }
            /// <summary>
            /// 密码
            /// </summary>
            public string passWord
            {
                get { return PassWord; }
                set { PassWord = value; }
            }
            
            #endregion
        }

        /// <summary>
        /// MySql数据库操作类
        /// </summary>
        public sealed class MySqlConnectionHandler
        {
            private MySqlConnection MainConnection;

            /// <summary>
            /// 打开连接（第一重载）
            /// </summary>
            /// <param name="db_name">数据库名</param>
            /// <param name="dataSource">数据源</param>
            /// <param name="port">端口</param>
            /// <param name="userName">用户名</param>
            /// <param name="passWord">密码</param>
            /// <returns>返回true，错误则返回null</returns>
            public bool start(string db_name, string dataSource, string port, string userName, string passWord)
            {
                //组建连接信息并创建连接
                MainConnection = new MySqlConnection
                    (
                    "Database=" + db_name + "; Data source=" + dataSource+ ";port="
                    + port + ";User Id=" + userName + ";Password=" + passWord + ";"
                    );
                #region 安全开启服务措施
                try
                {
                    switch (MainConnection.State)
                    {
                        case ConnectionState.Closed:
                            MainConnection.Open();
                            if (MainConnection.State == ConnectionState.Open)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }

                        case ConnectionState.Open:
                            return false;

                        default:
                            return false;
                    }
                }
                catch
                {
                    if (MainConnection.State == ConnectionState.Open)
                    {
                        MainConnection.Close();
                        return false;
                    }
                    else
                    {
                        return false;
                    }

                }
                #endregion
            }
            /// <summary>
            /// 打开连接（第二重载）
            /// </summary>
            /// <param name="obj">MySql连接信息通用结构体</param>
            /// <returns>返回true，错误则返回null</returns>
            public bool start(connStr connStr)
            {
                //组建连接信息并创建连接
                MainConnection = new MySqlConnection
                    (
                    "Database=" + connStr.db_name + "; Data source=" + connStr.dataSource+ ";port=" + 
                    connStr.port + ";User Id=" + connStr.userId + ";Password=" + connStr.passWord + ";"
                    );
                #region 安全开启服务措施
                try
                {
                    switch (MainConnection.State)
                    {
                        case ConnectionState.Closed:
                            MainConnection.Open();
                            if (MainConnection.State == ConnectionState.Open)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }

                        case ConnectionState.Open:
                            return false;

                        default:
                            return false;
                    }                   
                }
                catch
                {
                    if (MainConnection.State == ConnectionState.Open)
                    {
                        MainConnection.Close();
                        return false;
                    }
                    else
                    {
                        return false;
                    }
                    
                }
                #endregion
            }

            /// <summary>
            /// 关闭连接
            /// </summary>
            public bool close()
            {
                try
                {
                    switch (MainConnection.State)
                    {
                        case ConnectionState.Open:
                            MainConnection.Close();
                            if (MainConnection.State == ConnectionState.Closed)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }

                        case ConnectionState.Closed:
                            return true;

                        default:
                            return false;
                    }
                }
                catch
                {
                    return false;
                }
            }

            /// <summary>
            /// 执行一个查询（第一重载）
            /// </summary>
            /// <param name="connection">链接的实例</param>
            /// <param name="sql">SQL语句</param>
            /// <returns>返回查询结果，错误则返回Exception e的文本信息</returns>
            public string search(string sql)
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sql, MainConnection);
                    cmd.Connection.Open();
                    return cmd.ExecuteScalar().ToString();//返回查询结果
                }
                catch (Exception e)
                {
                    return e.ToString();//抛出错误
                }
                finally
                {
                    if (MainConnection.State == ConnectionState.Open)
                    {
                        MainConnection.Close();
                    }
                }
            }
            /// <summary>
            /// 执行一个查询（第二重载）
            /// </summary>
            /// <param name="connection">链接的实例</param>
            /// <param name="sql">SQL语句</param>
            /// <returns>返回查询结果，错误则返回Exception e的文本信息</returns>
            public string search(MySqlConnection connectionObj, string sql)
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sql, connectionObj);
                    cmd.Connection.Open();
                    return cmd.ExecuteScalar().ToString();//返回查询结果
                }
                catch (Exception e)
                {
                    return e.ToString();//抛出错误
                }
                finally
                {
                    if (connectionObj.State == ConnectionState.Open)
                    {
                        connectionObj.Close();
                    }
                }
            }

            /// <summary>
            /// 用于抛出一个MySql连接
            /// </summary>
            /// <param name="db_name">数据库名</param>
            /// <param name="dataSource">数据源</param>
            /// <param name="port">端口</param>
            /// <param name="userName">用户名</param>
            /// <param name="passWord">密码</param>
            /// <returns>返回一个MySqlConnection对象，错误则返回null</returns>
            public MySqlConnection getConnection(string db_name, string dataSource, string port, string userName, string passWord)
            {
                try
                {
                    //组建连接信息并创建连接
                    MySqlConnection conncetion = new MySqlConnection
                        (
                        "Database=" + db_name + "; Data source=" + dataSource+ ";port="
                        + port + ";User Id=" + userName + ";Password=" + passWord + ";"
                       );
                    return conncetion;
                }
                catch
                {
                    return null;
                }
            }
            /// <summary>
            /// 用于抛出一个MySql连接
            /// </summary>
            /// <param name="db_name">数据库名</param>
            /// <param name="dataSource">数据源</param>
            /// <param name="port">端口</param>
            /// <param name="userName">用户名</param>
            /// <param name="passWord">密码</param>
            /// <returns>返回一个MySqlConnection对象，错误则返回null</returns>
            public MySqlConnection getConnection(connStr connStr)
            {
                try
                {
                    //组建连接信息并创建连接
                    MySqlConnection conncetion = new MySqlConnection
                        (
                        "Database=" + connStr.db_name + "; Data source=" + connStr.dataSource+ ";port=" + 
                       connStr.port + ";User Id=" + connStr.userId + ";Password=" + connStr.passWord + ";"
                       );
                    return conncetion;
                }
                catch
                {
                    return null;
                }
            }

            /// <summary>
            /// 获取一张数据表（第一重载）
            /// </summary>
            /// <param name="db_name">数据库名</param>
            /// <param name="dataSource">数据源</param>
            /// <param name="port">端口</param>
            /// <param name="userName">用户名</param>
            /// <param name="passWord">密码</param>
            /// <param name="sql">用于查询数据表的SQL语句</param>
            /// <returns>返回一个DataTable对象，错误则返回null</returns>
            public DataTable getTable(string db_name, string dataSource, string port, string userName, string passWord, string sql)
            {
                List<string> list = new List<string>();

                //组建连接信息并创建连接
                MySqlConnection conn = new MySqlConnection
                    (
                    "Database=" + db_name + "; Data source=" + dataSource+ ";port=" 
                    + port + ";User Id=" + userName + ";Password=" + passWord + ";"
                    );
                try
                {       
                    MySqlDataAdapter mda = new MySqlDataAdapter(sql, conn);
                    DataTable table = new DataTable();
                    mda.Fill(table);//填充数据到dt
              
                    return table;
                }
                catch
                {
                    return null;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();//关闭连接
                    }
                }

            }
            /// <summary>
            /// 获取一张数据表（第二重载）
            /// </summary>
            /// <param name="connStr">MySql连接信息通用结构体</param>
            /// <param name="sql">用于查询数据表的SQL语句</param>
            /// <returns>返回一个DataTable对象，错误则返回null</returns>
            public DataTable getTable(connStr connStr,string sql)
            {
                List<string> list = new List<string>();

                //组建连接信息并创建连接
                MySqlConnection conn = new MySqlConnection
                    (
                    "Database=" + connStr.db_name + "; Data source=" + connStr.dataSource+ ";port=" +
                    connStr.port + ";User Id=" + connStr.userId + ";Password=" + connStr.passWord + ";"
                    );
                try
                {
                    MySqlDataAdapter mda = new MySqlDataAdapter(sql, conn);
                    DataTable table = new DataTable();

                    mda.Fill(table);//填充数据到dt

                    return table;
                }
                catch
                {
                    return null;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();//关闭连接
                    }
                }

            }
            /// <summary>
            /// 获取一张数据表（第三重载）（需要事先调用start()方法）
            /// </summary>
            /// <param name="sql">用于查询数据表的SQL语句</param>
            /// <returns>返回一个DataTable对象，错误则返回null</returns>
            public DataTable getTable(string sql)
            {
                try
                {
                    
                    if (MainConnection.State == ConnectionState.Closed)
                    {
                        MainConnection.Open();
                    }

                    MySqlDataAdapter mda = new MySqlDataAdapter(sql, MainConnection);
                    DataTable table = new DataTable();
                    mda.Fill(table);//填充数据到dt             

                    return table;
                }
                catch
                {
                    return null;
                }
                finally
                {
                    if (MainConnection.State == ConnectionState.Open)
                    {
                        MainConnection.Close();
                    }
                }

            }

            /// <summary>
            /// 获得数据列
            /// </summary>
            /// <param name="DataTable">数据表实例</param>
            /// <param name="columnName">列名</param>
            /// <returns>返回非泛型List<object>实例，错误则返回null</returns>
            public List<object> getColumn(DataTable DataTable, string columnName)
            {
                try
                {
                    List<object> list = new List<object>();
                    foreach (DataRow DataRow in DataTable.Rows)
                    {
                        list.Add(DataRow[columnName]);//将数据表中columnName列的所有行数据依次添加到list中
                    }

                    return list;
                }
                catch
                {
                    return null;
                }

            }

        }
    }

    namespace Display
    {
        namespace Web
        {
            public struct postData
            {
                #region 私有属性
                private int ID;
                private string pst_title;
                private string pst_summary;
                private string pst_content;
                private string cvr_link;
                #endregion

                #region 属性访问器
                public int post_ID
                {
                    get { return ID; }
                    set { ID = value; }
                }
                public string post_title
                {
                    get { return pst_title; }
                    set { pst_title = value; }
                }
                public string post_summary
                {
                    get { return pst_summary; }
                    set { pst_summary = value; }
                }
                public string post_content
                {
                    get { return pst_content; }
                    set { pst_content = value; }
                }
                public string cover_link
                {
                    get { return cvr_link; }
                    set { cvr_link = value; }
                }
                #endregion
            }
            public sealed class Post : Page
            {
                private Data.MySqlConnectionHandler MySqlConnectionHandler;
                /// <summary>
                /// 初始化Post对象（第一构造重载）
                /// </summary>
                /// <param name="MySqlConnectionHandler">用于获取Post信息的MySqlConnectionHandler实例</param>
                public Post(Data.MySqlConnectionHandler MySqlConnectionHandler)
                {
                    this.MySqlConnectionHandler = MySqlConnectionHandler;
                }
                /// <summary>
                /// 初始化Post对象（第二构造重载）
                /// </summary>
                /// <param name="connStr">用于创建Post实例的数据库连接文本结构体</param>
                public Post(Data.connStr connStr)
                {
                    MySqlConnectionHandler.start(connStr);
                }

                /// <summary>
                /// 获取文章信息
                /// </summary>
                /// <returns>返回带有文章信息的非泛型集合类，报错则返回null</returns>
                public List<postData> getData()
                {
                    try
                    {
                        List<postData> LstPd = new List<postData>();
                        DataTable dt = MySqlConnectionHandler.getTable("select * from slb_posts");

                        foreach (DataRow DataRow in dt.Rows)
                        {
                            postData pd = new postData();
                            pd.post_ID = int.Parse(DataRow["ID"].ToString());
                            pd.post_title = DataRow["post_title"].ToString();
                            pd.post_summary = DataRow["post_summary"].ToString();
                            pd.post_content = DataRow["post_content"].ToString();
                            pd.cover_link = DataRow["cover_link"].ToString();

                            LstPd.Add(pd);//将数据表中columnName列的所有行数据依次添加到list中
                        }
                        return LstPd;
                    }
                    catch
                    {
                        return null;
                    }
                    finally
                    {
                        MySqlConnectionHandler.close();
                    }
                }

                /// <summary>
                /// 根据ID输出文章标题
                /// </summary>
                /// <param name="LstPd">带有文章信息的非泛型集合类</param>
                /// <param name="post_ID">文章ID</param>
                /// <returns>返回文章标题，报错则返回null</returns>
                public string showPostTitle(List<postData> LstPd,int post_ID)
                {
                    foreach(postData pd in LstPd)
                    {
                        if (pd.post_ID == post_ID)
                        {
                            Response.Write(pd.post_title);
                            return pd.post_title;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    return null;
                }

                /// <summary>
                /// 根据ID输出文章概要
                /// </summary>
                /// <param name="LstPd">带有文章信息的非泛型集合类</param>
                /// <param name="post_ID">文章ID</param>
                /// <returns>返回文章概要，报错则返回null</returns>
                public string showPostSummary(List<postData> LstPd, int post_ID)
                {
                    foreach (postData pd in LstPd)
                    {
                        if (pd.post_ID == post_ID)
                        {
                            Response.Write(pd.post_summary);
                            return pd.post_summary;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    return null;
                }

                /// <summary>
                /// 根据ID输出文章内容
                /// </summary>
                /// <param name="LstPd">带有文章信息的非泛型集合类</param>
                /// <param name="post_ID">文章ID</param>
                /// <returns>返回文章内容，报错则返回null</returns>
                public string showPostContent(List<postData> LstPd, int post_ID)
                {
                    foreach (postData pd in LstPd)
                    {
                        if (pd.post_ID == post_ID)
                        {
                            Response.Write(pd.post_summary);
                            return pd.post_summary;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    return null;
                }

                /// <summary>
                /// 根据ID输出文章封面链接
                /// </summary>
                /// <param name="LstPd">带有文章信息的非泛型集合类</param>
                /// <param name="post_ID">文章ID</param>
                /// <returns>返回文章封面链接，报错则返回null</returns>
                public string showCoverLink(List<postData> LstPd, int post_ID)
                {
                    foreach (postData pd in LstPd)
                    {
                        if (pd.post_ID == post_ID)
                        {
                            Response.Write(pd.cover_link);
                            return pd.cover_link;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    return null;
                }
                


            }
        }
    }
}

