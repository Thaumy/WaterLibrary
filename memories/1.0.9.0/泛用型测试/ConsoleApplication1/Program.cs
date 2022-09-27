using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StdLib.Display.Web;
using StdLib.Data;
using System.Data;
using MySql.Data.MySqlClient;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            MySqlConnectionHandler msch = new MySqlConnectionHandler();

            connStr cs;
            cs.dataSource = "localhost";
            cs.passWord = "1246265280";
            cs.port = "3306";
            cs.userId = "root";
            cs.db_name = "slb_wdb";

            #region 启动数据库服务测试
            if (msch.start(cs)==true)
            {
                Console.WriteLine("数据库服务启动成功");
            }
            else
            {
                Console.WriteLine("数据库服务启动失败");
            }
            if (msch.close() == true)
            {
                Console.WriteLine("数据库服务关闭成功");
            }
            else
            {
                Console.WriteLine("数据库服务关闭失败");
            }
            #endregion

            //数据库单条记录查询测试
            Console.WriteLine("数据库连接实例查询结果：");
            Console.WriteLine(msch.search("select post_title from slb_posts where post_start_date='2017-06-17 03:56:24'"));

            //抛出数据库连接测试
            MySqlConnection n = msch.getConnection(cs);
            Console.WriteLine("数据库连接实例抛出完成");

            //检验被抛出数据库连接的可用性
            Console.WriteLine("被抛出数据库连接实例返回的查询结果：");
            Console.WriteLine(msch.search(n, "select post_title from slb_posts where post_start_date='2017-06-17 03:56:24'"));

            #region 获取数据列测试
            DataTable dt1=msch.getTable("select * from slb_posts");
            foreach (string s in msch.getColumn(dt1, "post_title"))
            {
                Console.WriteLine(s);
            }
            #endregion

            #region 获取文章信息测试
            Post pst = new Post(msch);

            foreach (postData pd in pst.getData())
            {
                Console.WriteLine("文章ID：" + pd.post_ID);
                Console.WriteLine("文章封面图片链接：" + pd.cover_link);

                Console.WriteLine("文章标题：" + pd.post_title);
                Console.WriteLine("文章概要：" + pd.post_summary);
                Console.WriteLine("文章内容：" + pd.post_content);
                Console.WriteLine();
            }
            #endregion

            Console.ReadKey();
            
        }
    }
}
