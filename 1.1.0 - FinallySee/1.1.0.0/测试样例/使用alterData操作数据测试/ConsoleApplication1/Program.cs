using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StdLib;
using StdLib.DataHandler;
using StdLib.FrameHandler;
using MySql;


namespace ConsoleApplication1//使用alterData操作数据测试
{
    class Program
    {
        static void Main(string[] args)
        {
            connStr cS = new connStr();
            cS.dataSource = "localhost";
            cS.password = "1246265280";
            cS.port = "3306";
            cS.userName = "root";

            MySqlConnectionHandler msch = new MySqlConnectionHandler();
            Console.WriteLine(msch.start(cS));

            selectStr sS = new selectStr();
            sS.dataBaseName = "slb_wdb";
            sS.primaryKeyName = "ID";
            sS.tableName = "slb_posts";
            sS.columnName = "post_summary";

            bool b = msch.alterData(sS, "3", "吃惊");
            bool c = msch.alterData(sS, "3", "吃");
            Console.WriteLine(b);
            Console.WriteLine(c);

            Console.ReadKey();
        }
    }
}
