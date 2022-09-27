using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StdLib1_17;
using StdLib1_17.StdLib_GobalnameSpace;
using StdLib1_17.StdEct;
using StdLib1_17.StdDal;
using System.Drawing;
using System.Windows.Forms;

namespace 泛用型控制台测试
{
    class Program
    {
        static void Main(string[] args)
        {

            #region ANSW TEST
            /*
            string[,] strArray = new string[100, 100];
            string _hex = "57656c636f6d6520746f2075736520414e53573264436f64652c2020697473206120766572792075736566756c20636f64652c202069742063616e207472616e736c61746520796f757220696e666f726d6174696f6e20696e746f20612073616665203264436f646520424d5020746f2068656c7020796f752073617665207468696e67732157656c636f6d6520746f2075736520414e53573264436f64652c2020697473206120766572792075736566756c20636f64652c202069742063616e207472616e736c61746520796f757220696e666f726d6174696f6e20696e746f20612073616665203264436f646520424d5020746f2068656c7020796f752073617665207468696e67732157656c636f6d6520746f2075736520414e53573264436f64652c2020697473206120766572792075736566756c20636f64652c202069742063616e207472616e736c61746520796f757220696e666f726d6174696f6e20696e746f20612073616665203264436f646520424d5020746f2068656c7020796f752073617665207468696e67732157656c636f6d6520746f2075736520414e53573264436f64652c2020697473206120766572792075736566756c20636f64652c202069742063616e207472616e736c61746520796f757220696e666f726d6174696f6e20696e746f20612073616665203264436f646520424d5020746f2068656c7020796f752073617665207468696e67732157656c636f6d6520746f2075736520414e53573264436f64652c2020697473206120766572792075736566756c20636f64652c202069742063616e207472616e736c61746520796f757220696e666f726d6174696f6e20696e746f20612073616665203264436f646520424d5020746f2068656c7020796f752073617665207468696e67732157656c636f6d6520746f2075736520414e53573264436f64652c2020697473206120766572792075736566756c20636f64652c202069742063616e207472616e736c61746520796f757220696e666f726d6174696f6e20696e746f20612073616665203264436f646520424d5020746f2068656c7020796f752073617665207468696e67732157656c636f6d6520746f2075736520414e53573264436f64652c2020697473206120766572792075736566756c20636f64652c202069742063616e207472616e736c61746520796f757220696e666f726d6174696f6e20696e746f20612073616665203264436f646520424d5020746f2068656c7020796f752073617665207468696e67732157656c636f6d6520746f2075736520414e53573264436f64652c2020697473206120766572792075736566756c20636f64652c202069742063616e207472616e736c61746520796f757220696e666f726d6174696f6e20696e746f20612073616665203264436f646520424d5020746f2068656c7020796f752073617665207468696e677321";
            string text = _hex;
            int len = _hex.Length;
            #region 字符串转矩阵处理
            int i = 0;
            for (int a = 1; a < len; a++)
            {
                i++;
                _hex = _hex.Insert(a + i - 1, " ");
            }
            i = 0;
            for (int y = 0; y < 100; y++)
            {
                for (int x = 0; x < 100; x++)
                {
                    strArray[x, y] = null;
                }
            }
            for (int y = 0; y < 100; y++)
            {
                for (int x = 0; x < 100; x++)
                {

                    if (i < _hex.Split(' ').Length)
                    {
                        strArray[x, y] = _hex.Split(' ')[i];
                    }
                    i++;
                }
            }
            #endregion

            object obj1 = text;
            object obj2 = _hex;
            object obj3 = strArray;

            Class_ANSW aw = new Class_ANSW();

            Image img1 = aw.ToANSW(text, @"D:\source.bmp");
            img1.Save(@"D:\img1.bmp");

            Image img2 = aw.ToANSW(obj1, @"D:\source.bmp", "text");
            img2.Save(@"D:\img2.bmp");

            Image img3 = aw.ToANSW(obj2, @"D:\source.bmp", "_hex");
            img3.Save(@"D:\img3.bmp");

            Image img4 = aw.ToANSW(obj3, @"D:\source.bmp", "ary");
            img4.Save(@"D:\img4.bmp");
            */
            #endregion

            #region XmlCreater测试
            /*
            XmlInf inf1;
            inf1.path = "//StdLib";
            inf1.nodeName = "MyNode";
            inf1.attName = "att";
            inf1.attValue = "Att233";
            inf1.innerText = "This is my innerText!";

            XmlInf inf2;
            inf2.path = "//StdLib";
            inf2.nodeName = "XN1";
            inf2.attName = "att";
            inf2.attValue = null;
            inf2.innerText = "Type";

            XmlInf inf3;
            inf3.path = "//StdLib";
            inf3.nodeName = "NO_0";

            XmlInf inf4;
            inf4.path = "//StdLib//NO_0";
            inf4.nodeName = "NO_1";

            XmlInf inf5;
            inf5.path = "//StdLib//NO_1";
            inf5.nodeName = "NO_2";

            XmlInf inf6;
            inf6.path = "//StdLib//NO_1";
            inf6.nodeName = "Node233";
            inf6.attName = "att";
            inf6.attValue = null;
            inf6.innerText = "innerText";

            XmlInf inf7;
            inf7.path = "//StdLib";
            inf7.nodeName = "goodXN";
            inf7.attName = "att";
            inf7.attValue = null;
            inf7.innerText = "Type";

            XmlInf inf8;
            inf8.path = "//StdLib";
            inf8.nodeName = "badXN";
            inf8.attName = "att";
            inf8.attValue = null;
            inf8.innerText = "Type";

            XmlInf inf9;
            inf9.path = "//StdLib//NO_0//NO_1//Node233";
            inf9.type = "_name";

            XmlInf inf10;
            inf10.path = "//StdLib//goodXN";
            inf10.type = "_value";

            XmlInf inf11;
            inf11.path = "//StdLib//MyNode";
            inf11.attName = "att";

            XmlInf sys;
            sys.path = @"D:\";
            sys.xmlName = "Xml_1";
            sys.rootName = "StdLib";

            //XML初始化测试
            Class_XmlCreater.CreateXml(sys);
            Class_XmlCreater.reStream(@"D:\Xml_1.xml");

            //Xml写入测试
            Class_XmlCreater.AddRealNode(inf1);

            //节点重名测试
            Class_XmlCreater.AddRealNode(inf2);
            Class_XmlCreater.AddRealNode(inf2);

            //节点嵌套测试
            Class_XmlCreater.AddEmptyNode(inf3);
            Class_XmlCreater.AddEmptyNode(inf4);
            Class_XmlCreater.AddEmptyNode(inf5);

            //添加嵌套里的有多个参数的节点测试
            Class_XmlCreater.AddRealNode(inf6);

            //节点删除测试
            Class_XmlCreater.AddRealNode(inf7);
            Class_XmlCreater.AddRealNode(inf8);
            Class_XmlCreater.RemoveNode(inf8);
            //被嵌套节点删除测试
            Class_XmlCreater.RemoveNode(inf5);

            //节点信息读取测试
            Console.WriteLine(Class_XmlCreater.ReadNode(inf9));
            Console.WriteLine(Class_XmlCreater.ReadNode(inf10));
            Console.WriteLine(Class_XmlCreater.ReadAtt(inf11));
            */
            #endregion

            #region IStdInf接口测试

            IStdInf I = new StdInf();
            Console.WriteLine(I.Stdver_newPub);

            #endregion

            #region HASH/MD5异步测试

            Console.WriteLine(Class_MD5.Main_MD5(@"jgadsklfhjkfgnjdd5sf@#$%^&*())ag416h4gsf6h3g4j56s74gs54h!@#$%^&*()h"));
            Console.WriteLine(Class_Hash.Main_Hash(@"j!@~)(_+)/sklfgnjkgnhnjfdsfag4575g3$#%^)(*s6y4456w68f$#%^g434dsf4"));

            Console.WriteLine(Class_MD5.Main_MD5(@""));
            Console.WriteLine(Class_Hash.Main_Hash(@""));

            Console.WriteLine(Class_MD5.Main_MD5(null));
            Console.WriteLine(Class_Hash.Main_Hash(null));

            #endregion

            #region 输出资源测试
            /*
            Image i = I.Lib_logo;
            i.Save(@"D:\interface_Lib_logo.bmp");

            StdInf inf = new StdInf();
            i = inf.Lib_logo;
            i.Save(@"D:\virtual_Lib_logo.bmp");
            */
            #endregion

            #region 二分法找值测试
            int[] a1 = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            Console.WriteLine(_BinarySrch.BinSrch(7, a1));
            Console.WriteLine(_BinarySrch.BinSrch(10, a1));

            double[] a2 = { 1.1, 1.3, 1.5, 1.7 };
            Console.WriteLine(_BinarySrch.BinSrch(1.7, a2));
            Console.WriteLine(_BinarySrch.BinSrch(5.6, a2));
            #endregion

            Console.ReadKey();

        }
    }
}
