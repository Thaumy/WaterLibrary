using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StdLib;
using StdLib.DataHandler;

namespace ConsoleApplication1
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


            #region HConnection测试 true

            Console.WriteLine("HConnection初始化启动");
            Console.WriteLine(msch.start(cS));

            Console.WriteLine("HConnection close后restart");
            Console.WriteLine(msch.HConnection_Close());
            Console.WriteLine(msch.HConnection_restart());

            Console.WriteLine("HConnection dispose后restart");
            Console.WriteLine(msch.HConnection_Dispose());
            Console.WriteLine(msch.HConnection_restart());

            Console.WriteLine("HConnection dispose后null");
            Console.WriteLine(msch.HConnection_Close());
            Console.WriteLine(msch.HConnection_null());
            Console.WriteLine(msch.start(cS));

            Console.WriteLine("HConnection dispose后null");
            Console.WriteLine(msch.HConnection_Dispose());
            Console.WriteLine(msch.HConnection_null());
            Console.WriteLine(msch.start(cS));

            Console.WriteLine("HConnection null后start");
            Console.WriteLine(msch.HConnection_null());
            Console.WriteLine(msch.start(cS));

            #endregion
            #region HConnection测试 false

            Console.WriteLine("HConnection null后close");
            Console.WriteLine(msch.HConnection_null());
            Console.WriteLine(msch.HConnection_Close());
            Console.WriteLine(msch.start(cS));

            Console.WriteLine("HConnection null后dispose");
            Console.WriteLine(msch.HConnection_null());
            Console.WriteLine(msch.HConnection_Dispose());
            Console.WriteLine(msch.start(cS));

            Console.WriteLine("HConnection null后restart");
            Console.WriteLine(msch.HConnection_null());
            Console.WriteLine(msch.HConnection_restart());



            #endregion

            #region HCommand测试 true
            Console.WriteLine(msch.HCommand_set(""));
            Console.WriteLine("HCommand dispose后null");
            Console.WriteLine(msch.HCommand_Dispose());
            Console.WriteLine(msch.HCommand_null());

            #endregion
            #region HCommand测试 false
            Console.WriteLine(msch.HCommand_set(""));
            Console.WriteLine("HCommand null后dispose");
            Console.WriteLine(msch.HCommand_null());
            Console.WriteLine(msch.HCommand_Dispose());

            #endregion



            Console.ReadKey();
        }
    }
}
