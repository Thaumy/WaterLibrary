

namespace WaterLibrary.pilipala
{
    using System.Collections.Generic;

    using WaterLibrary.MySQL;
    using WaterLibrary.pilipala.Database;


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
    /// pilipala内核
    /// </summary>
    public class CORE
    {
        private static CORE Singleton = null;/* 单例 */

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
        /// 插件表名键值对
        /// </summary>
        public static Dictionary<string, string> TableCache;
        /// <summary>
        /// 插件表名键值对
        /// </summary>
        public static Dictionary<string, string> ViewCache;

        /// <summary>
        /// 初始化内核
        /// </summary>
        /// <param name="PLDatabase">噼里啪啦数据库操作盒</param>
        public static void INIT(PLDatabase PLDatabase)
        {
            if (Singleton == null)
            {
                Singleton = new(PLDatabase);
            }
        }
        /// <summary>
        /// 初始化pilipala内核
        /// </summary>
        /// <param name="PLDatabase">pilipala数据库信息</param>
        private CORE(PLDatabase PLDatabase)
        {
            MySqlManager = PLDatabase.MySqlManager;
            Tables = PLDatabase.Tables;
            ViewsSet = PLDatabase.ViewsSet;
        }
    }
}
