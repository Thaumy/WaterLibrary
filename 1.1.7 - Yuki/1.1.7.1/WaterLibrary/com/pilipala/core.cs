namespace WaterLibrary.pilipala
{
    using System;

    using Newtonsoft.Json.Linq;

    using WaterLibrary.MySQL;
    using WaterLibrary.pilipala.Database;
    using WaterLibrary.Utils;


    namespace Database
    {
        /// <summary>
        /// 数据库表集合
        /// 用户表
        /// 元数据表
        /// 主表
        /// 归档表
        /// 评论表
        /// </summary>
        public record PLTables(string User, string Meta, string Stack, string Archive, string Comment);
        /// <summary>
        /// 数据库视图视图集合
        /// 显性联合视图（不包含备份）
        /// 隐性联合视图（包含备份）
        /// </summary>
        public record PLViews(string PosUnion, string NegUnion);

        /// <summary>
        /// 啪啦数据库操作盒
        /// </summary>
        [Obsolete("过时的数据结构")]
        public struct PLDatabase
        {
            /// <summary>
            /// 数据库名
            /// </summary>
            public string Database { get; set; }
            /// <summary>
            /// 数据表
            /// </summary>
            public PLTables Tables { get; set; }
            /// <summary>
            /// 数据视图
            /// </summary>
            public (PLViews CleanViews, PLViews DirtyViews) ViewsSet { get; set; }
            /// <summary>
            /// 数据库管理器实例
            /// </summary>
            public MySqlManager MySqlManager { get; set; }
        }
    }

    /// <summary>
    /// 噼里啪啦配件接口
    /// </summary>
    interface IPLComponent<T>
    {

    }

    /// <summary>
    /// 内核
    /// </summary>
    public class PiliPala
    {
        private static PiliPala Singleton = null;/* 单例 */

        /// <summary>
        /// 核心表结构
        /// </summary>
        public static PLTables Tables { get; private set; }
        /// <summary>
        /// 核心视图结构
        /// </summary>
        public static (PLViews CleanViews, PLViews DirtyViews) ViewsSet { get; private set; }
        /// <summary>
        /// MySql控制器
        /// </summary>
        public static MySqlManager MySqlManager { get; private set; }

        /// <summary>
        /// 初始化内核
        /// </summary>
        /// <param name="PLDatabase">噼里啪啦数据库操作盒</param>
        [Obsolete("请使用重载：INIT(string configJsonString)")]
        public static void INIT(PLDatabase PLDatabase)
        {
            if (Singleton == null)
            {
                Singleton = new();

                MySqlManager = PLDatabase.MySqlManager;
                Tables = PLDatabase.Tables;
                ViewsSet = PLDatabase.ViewsSet;
            }
            else
            {
                throw new Exception("尝试重复加载内核");
            }
        }
        /// <summary>
        /// 初始化内核
        /// </summary>
        /// <param name="configYamlString">配置文件Yaml字符串</param>
        public static void INIT(string configYamlString)
        {
            if (Singleton == null)
            {
                var jsonString = ConvertH.YamlToJson(configYamlString);
                var root = JObject.Parse(jsonString);

                var TablesNode = root["Database"]["Tables"];
                Tables = new(
                    TablesNode.Value<string>("user"),
                    TablesNode.Value<string>("meta"),
                    TablesNode.Value<string>("stack"),
                    TablesNode.Value<string>("archive"),
                    TablesNode.Value<string>("comment"));

                var ViewsSetNode = root["Database"]["ViewsSet"];
                ViewsSet = new(
                    new(
                        ViewsSetNode["CleanViews"].Value<string>("posUnion"),
                        ViewsSetNode["CleanViews"].Value<string>("negUnion")),
                    new(
                        ViewsSetNode["DirtyViews"].Value<string>("posUnion"),
                        ViewsSetNode["DirtyViews"].Value<string>("negUnion")));

                var ConnectionNode = root["Database"]["Connection"];
                var msg = new MySqlConnMsg(
                    ConnectionNode.Value<string>("dataSource"),
                    ConnectionNode.Value<int>("port"),
                    ConnectionNode.Value<string>("usr"),
                    ConnectionNode.Value<string>("pwd")
                    );
                MySqlManager = new MySqlManager
                    (msg,
                    ConnectionNode.Value<string>("schema"),
                    ConnectionNode.Value<uint>("poolSize"));

                Singleton = new();
            }
            else
            {
                throw new Exception("尝试重复加载内核");
            }
        }

        /// <summary>
        /// 私有构造
        /// </summary>
        private PiliPala()
        {
        }
    }
}
