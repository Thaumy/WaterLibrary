﻿using System;
using System.Collections.Generic;

using System.Data;
using System.Linq;
using System.Reflection;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using MySql.Data.MySqlClient;

namespace WaterLibrary.pilipala
{
    using WaterLibrary.MySQL;
    using WaterLibrary.Utils;
    using WaterLibrary.pilipala.Database;
    using WaterLibrary.pilipala.Entity;

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

    namespace Entity
    {
        public class PostSSK
        {

        }

        /// <summary>
        /// 文章
        /// </summary>
        public class PostRecord : PostSSK
        {
            /// <summary>
            /// 属性索引器
            /// </summary>
            /// <param name="Prop">属性</param>
            /// <returns></returns>
            public object this[string Prop]
            {
                /* 通过反射获取属性 */
                get => GetType().GetProperty(Prop).GetValue(this);
                set
                {
                    /* 通过反射设置属性 */
                    System.Type ThisType = GetType();
                    System.Type KeyType = ThisType.GetProperty(Prop).GetValue(this).GetType();
                    ThisType.GetProperty(Prop).SetValue(this, Convert.ChangeType(value, KeyType));
                }
            }
            /// <summary>
            /// 将当前对象序列化到JSON
            /// </summary>
            /// <returns></returns>
            public string ToJSON()
            {
                return JsonConvert.SerializeObject
                    (this, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
            }


            /// <summary>
            /// 内置读取器引用
            /// </summary>
            private readonly Component.Reader reader;
            /// <summary>
            /// 字段读取器
            /// </summary>
            /// <param name="table">目标表</param>
            /// <param name="field">目标字段</param>
            /// <returns></returns>
            private object GetField(string table, string field)
            {
                var SQL = $"SELECT {field} FROM `{table}` WHERE UUID = ?UUID";
                var para = new MySqlParameter("UUID", UUID);

                var result = reader.MySqlManager.GetRow(SQL, para)[field];
                return Convert.ToString(result);
            }
            /// <summary>
            /// 字段设置器
            /// </summary>
            /// <param name="table">目标表</param>
            /// <param name="field">目标字段</param>
            /// <param name="newValue">新值</param>
            private void SetField(string table, string field, object newValue)
            {
                (string, object) SET = (field, newValue);
                (string, object) WHERE = ("UUID", UUID);

                reader.MySqlManager.ExecuteUpdate(table, SET, WHERE);
            }

            /// <summary>
            /// 默认构造方式（建议使用Reader构造）
            /// </summary>
            /// <param name="UUID">文章记录的UUID</param>
            /// <param name="reader">读取器引用</param>
            public PostRecord(string UUID, Component.Reader reader)
            {
                this.reader = reader;

                this.UUID = UUID;

                PropertyContainer = new();
            }
            

            /// <summary>
            /// 计算由标题、概要、内容签名的MD5
            /// </summary>
            /// <returns></returns>
            public string MD5()
            {
                return MathH.MD5(Title + Summary + Content);
            }
            /// <summary>
            /// 计算由标题、概要、内容签名的MD5，并从首位限定取用长度
            /// </summary>
            /// <param name="length">取用长度</param>
            /// <returns></returns>
            public string MD5(int length)
            {
                return MD5().Substring(0, length);
            }

            /// <summary>
            /// 索引（ID字段不允许更改）
            /// </summary>
            public int ID {
                get => Convert.ToInt32(GetField(reader.StackTable, "PostID"));
            }
            /// <summary>
            /// 全局标识
            /// </summary>
            public string UUID { get; init; }

            /// <summary>
            /// 标题
            /// </summary>
            public string Title
            {
                get => Convert.ToString(GetField(reader.StackTable,"Title"));
                set => SetField(reader.StackTable, "Title", value);
            }
            /// <summary>
            /// 概要
            /// </summary>
            public string Summary {
                get => Convert.ToString(GetField(reader.StackTable, "Summary"));
                set => SetField(reader.StackTable, "Summary", value);
            }
            /// <summary>
            /// 尝试概要
            /// </summary>
            /// <param name="todo">概要为空时的操作</param>
            /// <returns></returns>
            public string TrySummary(Func<string> todo)
            {
                return Summary switch
                {
                    "" => todo(),
                    _ => Summary
                };
            }

            /// <summary>
            /// Content字段原始内容
            /// </summary>
            public string Content
            {
                get => Convert.ToString(GetField(reader.StackTable, "Content"));
                set => SetField(reader.StackTable, "Content", value);
            }
            /// <summary>
            /// 获得Html格式的文章内容，所有Markdown标记均会被转换为等效的Html标记
            /// </summary>
            /// <returns></returns>
            public string HtmlContent()
            {
                return ConvertH.MarkdownToHtml(Content);
            }
            /// <summary>
            /// 获得Html格式的文章内容，所有Markdown标记均会被转换为等效的Html标记，并从首位置限定取用长度
            /// </summary>
            /// <param name="Length">取用长度</param>
            /// <returns></returns>
            public string HtmlContent(int Length) => ConvertH.MarkdownToHtml(Content).Substring(0, Length);
            /// <summary>
            /// 获得纯文本格式的文章内容，过滤掉任何Markdown和Html标记
            /// </summary>
            /// <returns></returns>
            public string TextContent() => ConvertH.HtmlFilter(HtmlContent());
            /// <summary>
            /// 获得纯文本格式的文章内容，过滤掉任何Markdown和Html标记，并从首位置限定取用长度
            /// </summary>
            /// <param name="Length">取用长度</param>
            /// <returns></returns>
            public string TextContent(int Length) => ConvertH.HtmlFilter(HtmlContent()).Substring(0, Length);

            /// <summary>
            /// 封面
            /// </summary>
            public string Cover {
                get => Convert.ToString(GetField(reader.StackTable, "Cover"));
                set => SetField(reader.StackTable, "Cover", value);
            }

            /// <summary>
            /// 归档ID（请使用Archiver设置）
            /// </summary>
            public string ArchiveID {
                get => Convert.ToString(GetField(reader.MetaTable, "ArchiveID"));
            }
            /// <summary>
            /// 归档名（请使用Archiver设置）
            /// </summary>
            public string ArchiveName {
                get => Convert.ToString(GetField(reader.UnionView, "ArchiveName"));
            }

            /// <summary>
            /// 标签
            /// </summary>
            public string Label {
                get => Convert.ToString(GetField(reader.StackTable, "Label"));
                set => SetField(reader.StackTable, "Label", value);
            }
            /// <summary>
            /// 获得标签集合
            /// </summary>
            /// <returns></returns>
            public List<string> LabelList() => ConvertH.StringToList(Label, '$');

            /// <summary>
            /// 文章模式
            /// </summary>
            public string Mode {
                get => Convert.ToString(GetField(reader.MetaTable, "Mode"));
                set => SetField(reader.StackTable, "Mode", value);
            }
            /// <summary>
            /// 文章类型
            /// </summary>
            public string Type {
                get => Convert.ToString(GetField(reader.MetaTable, "Type"));
                set => SetField(reader.StackTable, "Type", value);
            }
            /// <summary>
            /// 归属用户
            /// </summary>
            public string User {
                get => Convert.ToString(GetField(reader.MetaTable, "User"));
                set => SetField(reader.StackTable, "User", value);
            }

            /// <summary>
            /// 创建时间
            /// </summary>
            public DateTime CT {
                get => Convert.ToDateTime(GetField(reader.MetaTable, "CT"));
                set => SetField(reader.MetaTable, "CT", value);
            }
            /// <summary>
            /// 最后修改时间
            /// </summary>
            public DateTime LCT {
                get => Convert.ToDateTime(GetField(reader.StackTable, "LCT"));
                set => SetField(reader.StackTable, "LCT", value);
            }

            /// <summary>
            /// 访问计数
            /// </summary>
            public int UVCount {
                get => Convert.ToInt32(GetField(reader.MetaTable, "UVCount"));
                set => SetField(reader.StackTable, "UVCount", value);
            }
            /// <summary>
            /// 星星计数
            /// </summary>
            public int StarCount {
                get => Convert.ToInt32(GetField(reader.MetaTable, "StarCount"));
                set => SetField(reader.StackTable, "StarCount", value);
            }


            /// <summary>
            /// 属性容器
            /// </summary>
            public Hashtable PropertyContainer { get; set; }
        }
        /// <summary>
        /// 文章栈集
        /// </summary>
        public class PostRecordSet : IEnumerable
        {
            /// <summary>
            /// 文章索引器
            /// </summary>
            /// <param name="UUID">文章UUID</param>
            /// <returns>索引无果返回null</returns>
            public PostRecord this[string UUID]
            {
                /* 通过反射获取属性 */
                get
                {
                    foreach (PostRecord el in PostList)
                    {
                        if (el.UUID == UUID)
                            return el;
                    }
                    return null;
                }
            }
            /// <summary>
            /// 迭代器
            /// </summary>
            /// <returns></returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return PostList.GetEnumerator();
            }

            private readonly List<PostRecord> PostList = new();
            /// <summary>
            /// 将当前对象序列化到JSON
            /// </summary>
            /// <returns></returns>
            public string ToJSON()
            {
                return JsonConvert
                    .SerializeObject(this, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
            }
            /// <summary>
            /// 当前数据集的文章对象数
            /// </summary>
            public int Count
            {
                get { return PostList.Count; }
            }
            /// <summary>
            /// 取得数据集中的最后一个评论对象
            /// </summary>
            /// <returns></returns>
            public PostRecord Last() => PostList.Last();
            /// <summary>
            /// 添加文章
            /// </summary>
            /// <param name="postRecord">文章对象</param>
            public void Add(PostRecord postRecord) => PostList.Add(postRecord);
            /// <summary>
            /// 对数据集内的每一个对象应用Action
            /// </summary>
            /// <param name="action">Action委托</param>
            /// <returns>返回操作后的数据集</returns>
            public PostRecordSet ForEach(Action<PostRecord> action)
            {
                PostList.ForEach(action);
                return this;
            }


            /// <summary>
            /// 数据集内最近一月(30天内)的文章创建数
            /// </summary>
            /// <returns></returns>
            public int WithinMonthCreateCount() => CreateCounter(-30);
            /// <summary>
            /// 数据集内最近一周(7天内)的文章创建数
            /// </summary>
            /// <returns></returns>
            public int WithinWeekCreateCount() => CreateCounter(-7);
            /// <summary>
            /// 数据集内最近一天(24小时内)的文章创建数
            /// </summary>
            /// <returns></returns>
            public int WithinDayCreateCount() => CreateCounter(-1);
            private int CreateCounter(int Days) => (from el in PostList where el.CT > DateTime.Now.AddDays(Days) select el).Count();

            /// <summary>
            /// 数据集内最近一月(30天内)的文章修改数
            /// </summary>
            /// <returns></returns>
            public int WithinMonthUpdateCount() => UpdateCounter(-30);
            /// <summary>
            /// 数据集内最近一周(7天内)的文章修改数
            /// </summary>
            /// <returns></returns>
            public int WithinWeekUpdateCount() => UpdateCounter(-7);
            /// <summary>
            /// 数据集内最近一天(24小时内)的文章修改数
            /// </summary>
            /// <returns></returns>
            public int WithinDayUpdateCount() => UpdateCounter(-1);
            private int UpdateCounter(int Days) => (from el in PostList where el.CT > DateTime.Now.AddDays(Days) select el).Count();
        }


        /// <summary>
        /// 文章树
        /// </summary>
        public class PostTree : PostSSK
        {
            private List<PostSSK> list = new();
            public int ID = -1;

            public PostTree(int ID)
            {
                this.ID = ID;
            }

            public List<PostSSK> GetNodes()
            {
                return null;
            }
        }
        /// <summary>
        /// 文章栈
        /// </summary>
        public class PostStack : PostSSK
        {
            private List<PostSSK> list = new();
            public int ID = -1;

            public PostStack(int ID)
            {
                this.ID = ID;
            }

            public List<PostSSK> GetNodes()
            {
                return null;
            }
        }

        /// <summary>
        /// 文章属性枚举
        /// </summary>
        public enum PostProp
        {
            /// <summary>
            /// 文章索引
            /// </summary>
            PostID,
            /// <summary>
            /// 文章全局标识
            /// </summary>
            UUID,
            /// <summary>
            /// 文章标题
            /// </summary>
            Title,
            /// <summary>
            /// 文章摘要
            /// </summary>
            Summary,
            /// <summary>
            /// 文章内容
            /// </summary>
            Content,
            /// <summary>
            /// 封面
            /// </summary>
            Cover,
            /// <summary>
            /// 归档名
            /// </summary>
            ArchiveName,
            /// <summary>
            /// 标签
            /// </summary>
            Label,
            /// <summary>
            /// 模式
            /// </summary>
            Mode,
            /// <summary>
            /// 类型
            /// </summary>
            Type,
            /// <summary>
            /// 作者
            /// </summary>
            User,
            /// <summary>
            /// 创建时间
            /// </summary>
            CT,
            /// <summary>
            /// 最后修改时间
            /// </summary>
            LCT,
            /// <summary>
            /// 浏览计数
            /// </summary>
            UVCount,
            /// <summary>
            /// 星星计数
            /// </summary>
            StarCount
        }
    }
    namespace Component
    {
        /// <summary>
        /// 组件工厂
        /// </summary>
        public class ComponentFactory
        {
            /// <summary>
            /// 生成用户组件
            /// </summary>
            /// <returns></returns>
            public User GenUser(string UserAccount, string UserPWD)
            {
                var Tables = CORE.Tables;
                var MySqlManager = CORE.MySqlManager;

                var PWD_MD5 = MathH.MD5(UserPWD).ToLower();
                var para = new MySqlParameter[] { new("UserAccount", UserAccount), new("UserPWD", PWD_MD5) };

                var count = Convert.ToInt32(MySqlManager.GetRow
                    ($"SELECT COUNT(*) FROM {Tables.User} WHERE Account = ?UserAccount AND PWD = ?UserPWD", para)[0]);

                if (count == 1)
                {
                    return new User(Tables, MySqlManager, UserAccount);
                }
                else
                {
                    throw new Exception("构造失败，非法的用户信息。");
                }
            }
            /// <summary>
            /// 生成权限管理组件
            /// </summary>
            /// <returns></returns>
            public Auth GenAuthentication(User User) => new(CORE.Tables, CORE.MySqlManager, User);
            /// <summary>
            /// 生成读组件
            /// </summary>
            /// <param name="ReadMode">读取模式枚举</param>
            /// <param name="WithRawMode">以原始数据读取模式初始化(读取到的数据包含隐性文章)</param>
            /// <returns></returns>
            public Reader GenReader(Reader.ReadMode ReadMode, bool WithRawMode = false)
            {
                return ReadMode switch
                {
                    Reader.ReadMode.CleanRead => WithRawMode switch
                    {
                        false => new(CORE.ViewsSet.CleanViews.PosUnion, CORE.Tables.Meta, CORE.Tables.Stack,  CORE.MySqlManager),
                        true => new(CORE.ViewsSet.CleanViews.NegUnion, CORE.Tables.Meta, CORE.Tables.Stack, CORE.MySqlManager),
                    },
                    Reader.ReadMode.DirtyRead => WithRawMode switch
                    {
                        false => new(CORE.ViewsSet.DirtyViews.PosUnion, CORE.Tables.Meta, CORE.Tables.Stack, CORE.MySqlManager),
                        true => new(CORE.ViewsSet.DirtyViews.NegUnion, CORE.Tables.Meta, CORE.Tables.Stack, CORE.MySqlManager),
                    },
                    _ => throw new NotImplementedException(),
                };
            }
            /// <summary>
            /// 生成写组件
            /// </summary>
            /// <returns></returns>
            public Writer GenWriter() => new(CORE.Tables.Meta, CORE.Tables.Stack, CORE.MySqlManager);
            /// <summary>
            /// 生成计数组件
            /// </summary>
            /// <returns></returns>
            public Counter GenCounter() => new(CORE.Tables.Meta, CORE.Tables.Stack, CORE.MySqlManager);
            /// <summary>
            /// 生成插件组件
            /// </summary>
            /// <returns></returns>
            public Pluginer GenPluginer() => new();
            /// <summary>
            /// 生成归档管理组件
            /// </summary>
            /// <returns></returns>
            public Archiver GenArchiver() => new(CORE.Tables.Archive, CORE.MySqlManager);
            /// <summary>
            /// 生成评论湖组件
            /// </summary>
            /// <returns></returns>
            public CommentLake GenCommentLake() => new(CORE.Tables.Meta, CORE.Tables.Comment, CORE.MySqlManager);
        }

        /// <summary>
        /// 权限管理组件
        /// </summary>
        public class Auth : IPLComponent<Auth>
        {
            private PLTables Tables { get; init; }
            private MySqlManager MySqlManager { get; init; }

            private readonly User User;

            /// <summary>
            /// 默认构造
            /// </summary>
            private Auth() { }
            /// <summary>
            /// 工厂构造
            /// </summary>
            /// <param name="Tables">数据库表</param>
            /// <param name="MySqlManager">数据库管理器</param>
            /// <param name="User">用户对象</param>
            internal Auth(PLTables Tables, MySqlManager MySqlManager, User User)
            {
                this.Tables = Tables;
                this.MySqlManager = MySqlManager;
                this.User = User;
            }

            /// <summary>
            /// 权限验证
            /// </summary>
            /// <param name="Token">Token</param>
            /// <param name="todo">行为委托</param>
            /// <returns></returns>
            public T CheckAuth<T>(string Token, Func<T> todo)
            {
                return (DateTime.Now - Convert.ToDateTime(MathH.RSADecrypt(GetPrivateKey(), Token))).TotalSeconds switch
                {
                    < 10 => todo(),
                    _ => default
                };
            }

            /// <summary>
            /// 取得私钥
            /// </summary>
            /// <returns></returns>
            public string GetPrivateKey()
            {
                return MySqlManager.GetKey($"SELECT PrivateKey FROM {Tables.User} WHERE Account = '{User.Account}'").ToString();
            }
            /// <summary>
            /// 设置私钥
            /// </summary>
            /// <returns></returns>
            public bool SetPrivateKey(string PrivateKey)
            {
                var SET = ("PrivateKey", PrivateKey);
                var WHERE = ("Account", User.Account);
                return MySqlManager.ExecuteUpdate(Tables.User, SET, WHERE);
            }
            /// <summary>
            /// 取得最后Token获取时间
            /// </summary>
            /// <returns></returns>
            public DateTime GetTokenTime()
            {
                return Convert.ToDateTime(MySqlManager.GetKey($"SELECT TokenTime FROM {Tables.User} WHERE Account = '{User.Account}'"));
            }
            /// <summary>
            /// 设置最后Token获取时间为当前时间
            /// </summary>
            /// <returns></returns>
            public bool UpdateTokenTime()
            {
                var SET = ("TokenTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                var WHERE = ("Account", User.Account);
                return MySqlManager.ExecuteUpdate(Tables.User, SET, WHERE);
            }
        }
        /// <summary>
        /// 用户组件
        /// </summary>
        public class User : IPLComponent<User>
        {
            internal PLTables Tables { get; init; }
            internal MySqlManager MySqlManager { get; init; }

            /// <summary>
            /// 默认构造
            /// </summary>
            private User() { }
            /// <summary>
            /// 工厂构造
            /// </summary>
            /// <param name="Tables">数据库表</param>
            /// <param name="MySqlManager">数据库管理器</param>
            /// <param name="UserAccount">用户账号</param>
            internal User(PLTables Tables, MySqlManager MySqlManager, string UserAccount)
            {
                this.Tables = Tables;
                this.MySqlManager = MySqlManager;
                Account = UserAccount;
            }

            /// <summary>
            /// 将当前对象序列化到JSON
            /// </summary>
            /// <returns></returns>
            public string ToJSON()
            {
                return JsonConvert.SerializeObject
                    (this, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
            }

            /// <summary>
            /// 用户账号
            /// </summary>
            public string Account { get; internal set; }

            /// <summary>
            /// 检查密码
            /// </summary>
            /// <param name="PWD">等待检查是否正确的密码</param>
            /// <returns></returns>
            public bool CheckPWD(string PWD)
            {
                return MathH.MD5(PWD) == Get("PWD");
            }
            /// <summary>
            /// 设置密码
            /// </summary>
            /// <param name="NewPWD">新的密码</param>
            /// <returns></returns>
            public bool SetPWD(string NewPWD)
            {
                return Set("PWD", NewPWD);
            }

            /// <summary>
            /// 用户名
            /// </summary>
            public string Name { get => Get("Name"); set => Set("Name", value); }
            /// <summary>
            /// 自我介绍
            /// </summary>
            public string Bio { get => Get("Bio"); set => Set("Bio", value); }
            /// <summary>
            /// 组别
            /// </summary>
            public string GroupType { get => Get("GroupType"); set => Set("GroupType", value); }
            /// <summary>
            /// 邮箱
            /// </summary>
            public string Email { get => Get("Email"); set => Set("Email", value); }
            /// <summary>
            /// 头像(链接)
            /// </summary>
            public string Avatar { get => Get("Avatar"); set => Set("Avatar", value); }

            private string Get(string Key)
            {
                return MySqlManager.GetRow($"SELECT {Key} FROM {Tables.User} WHERE Account = '{Account}'")[Key].ToString();
            }
            private bool Set(string Key, string Value)
            {
                var SET = (Key, Value);
                var WHERE = ("Account", Account);
                return MySqlManager.ExecuteUpdate(Tables.User, SET, WHERE);
            }
        }
        /// <summary>
        /// 数据读取组件
        /// </summary>
        public class Reader : IPLComponent<Reader>
        {
            /// <summary>
            /// 读取模式枚举
            /// </summary>
            public enum ReadMode
            {
                /// <summary>
                /// 净读，表示不读取隐藏文章。适用于面向访问者的渲染
                /// </summary>
                CleanRead = 0,
                /// <summary>
                /// 脏读，表示读取隐藏文章。适用于面向管理员的渲染
                /// </summary>
                DirtyRead = 1
            }

            internal string UnionView { get; init; }
            internal string MetaTable { get; init; }
            internal string StackTable { get; init; }
            
            internal MySqlManager MySqlManager { get; init; }

            /// <summary>
            /// 默认构造
            /// </summary>
            private Reader() { }
            /// <summary>
            /// 工厂构造
            /// </summary>
            /// <param name="UnionView">联合视图</param>
            /// <param name="MetaTable">元信息表</param>
            /// <param name="StackTable">栈表</param>
            /// <param name="MySqlManager">数据库管理器</param>
            /// <returns></returns>
            internal Reader(string UnionView,string MetaTable, string StackTable, MySqlManager MySqlManager)
            {
                this.UnionView = UnionView;
                this.MetaTable = MetaTable;
                this.StackTable = StackTable;
                this.MySqlManager = MySqlManager;
            }

            /*/// <summary>
            /// 获取指定文章数据
            /// </summary>
            /// <param name="PostID">目标文章PostID</param>
            /// <returns></returns>
            public PostRecord GetPost(int PostID)
            {
                string SQL = $"SELECT * FROM `{UnionView}` WHERE PostID={PostID}";
                DataRow Row = MySqlManager.GetRow(SQL);

                return new PostRecord(Convert.ToString(Row["UUID"]), this)
                {
                    CT = Convert.ToDateTime(Row["CT"]),
                    LCT = Convert.ToDateTime(Row["LCT"]),
                    Title = Convert.ToString(Row["Title"]),
                    Summary = Convert.ToString(Row["Summary"]),
                    Content = Convert.ToString(Row["Content"]),

                    ArchiveName = Convert.ToString(Row["ArchiveName"]),
                    Label = Convert.ToString(Row["Label"]),
                    Cover = Convert.ToString(Row["Cover"]),

                    Mode = Convert.ToString(Row["Mode"]),
                    Type = Convert.ToString(Row["Type"]),
                    User = Convert.ToString(Row["User"]),

                    UVCount = Convert.ToInt32(Row["UVCount"]),
                    StarCount = Convert.ToInt32(Row["StarCount"])
                };
            }*/
            /// <summary>
            /// 取得指定文章属性
            /// </summary>
            /// <param name="PostID">目标文章PostID</param>
            /// <param name="Prop">目标属性类型</param>
            /// <returns></returns>
            public object GetPostProp(int PostID, PostProp Prop)
            {
                string SQL = $"SELECT {Prop} FROM `{UnionView}` WHERE PostID = ?PostID";

                return MySqlManager.GetKey(SQL, new MySqlParameter[]
                {
                    new("PostID", PostID)
                });
            }

            /*/// <summary>
            /// 获取文章数据
            /// </summary>
            /// <param name="Prop">正则表达式匹配的属性类型</param>
            /// <param name="REGEXP">正则表达式</param>
            /// <returns></returns>
            public PostRecordSet GetPost(PostProp Prop, string REGEXP)
            {
                string SQL = $"SELECT * FROM `{UnionView}` WHERE {Prop} REGEXP ?REGEXP ORDER BY CT DESC";

                PostRecordSet PostRecordSet = new();

                foreach (DataRow Row in MySqlManager.GetTable(SQL, new MySqlParameter[]
                {
                    new("REGEXP", REGEXP)
                }).Rows)
                {
                    PostRecordSet.Add(new PostRecord(Convert.ToString(Row["UUID"]), this)
                    {
                        ID = Convert.ToInt32(Row["PostID"]),

                        CT = Convert.ToDateTime(Row["CT"]),
                        LCT = Convert.ToDateTime(Row["LCT"]),
                        Title = Convert.ToString(Row["Title"]),
                        Summary = Convert.ToString(Row["Summary"]),
                        Content = Convert.ToString(Row["Content"]),

                        ArchiveName = Convert.ToString(Row["ArchiveName"]),
                        Label = Convert.ToString(Row["Label"]),
                        Cover = Convert.ToString(Row["Cover"]),

                        Mode = Convert.ToString(Row["Mode"]),
                        Type = Convert.ToString(Row["Type"]),
                        User = Convert.ToString(Row["User"]),

                        UVCount = Convert.ToInt32(Row["UVCount"]),
                        StarCount = Convert.ToInt32(Row["StarCount"])
                    });
                }


                return PostRecordSet;
            }*/
            /// <summary>
            /// 获取文章数据
            /// </summary>
            /// <param name="Prop">正则表达式匹配的属性类型</param>
            /// <param name="REGEXP">正则表达式</param>
            /// <param name="PostProps">所需属性类型</param>
            /// <returns></returns>
            public PostRecordSet GetPost(PostProp Prop, string REGEXP, params PostProp[] PostProps)
            {
                /* 键名字符串格式化 */
                string KeysStr = ConvertH.ListToString(PostProps, ',');
                string SQL = $"SELECT {KeysStr} FROM `{UnionView}` WHERE {Prop} REGEXP ?REGEXP ORDER BY CT DESC";

                PostRecordSet PostRecordSet = new();

                foreach (DataRow Row in MySqlManager.GetTable(SQL, new MySqlParameter[]
                {
                    new("REGEXP", REGEXP)
                }).Rows)
                {
                    PostRecord PostRecord = new("", this);//TODO，构造时必须使用UUID

                    for (int i = 0; i < PostProps.Length; i++)
                    {
                        PostRecord[PostProps[i].ToString()] = Row.ItemArray[i];
                    }

                    PostRecordSet.Add(PostRecord);
                }

                return PostRecordSet;
            }

            /// <summary>
            /// 取得具有比目标文章的指定属性具有更大的值的文章PostID
            /// </summary>
            /// <param name="PostID">目标文章的PostID</param>
            /// <param name="Prop">指定属性</param>
            /// <returns>不存在符合要求的PostID时，返回-1</returns>
            public int Bigger(int PostID, PostProp Prop)
            {
                string SQL = (Prop == PostProp.PostID) switch /* 对查询PostID有优化 */
                {
                    true => $"SELECT PostID FROM `{UnionView}` WHERE PostID=( SELECT min(PostID) FROM `{UnionView}` WHERE PostID > {PostID})",
                    false => string.Format
                    (
                    $"SELECT PostID FROM `{0}` WHERE {1}=( SELECT min({1}) FROM `{0}` WHERE {1} > ( SELECT {1} FROM `{0}` WHERE PostID = {2} ))"
                    , UnionView, Prop, PostID
                    )
                };

                object NextPostID = MySqlManager.GetKey(SQL);

                return NextPostID == null ? -1 : Convert.ToInt32(NextPostID);

            }
            /// <summary>
            /// 取得具有比目标文章的指定属性具有更大的值的文章PostID
            /// </summary>
            /// <remarks>此方法能根据指定正则表达式对某属性进行筛选</remarks>
            /// <param name="PostID">目标文章的PostID</param>
            /// <param name="Prop">指定属性</param>
            /// <param name="REGEXP">正则表达式</param>
            /// <param name="FilterProp">被正则表达式筛选的属性</param>
            /// <returns>不存在符合要求的PostID时，返回-1</returns>
            public int Bigger(int PostID, PostProp Prop, string REGEXP, PostProp FilterProp)
            {
                string SQL = (Prop == PostProp.PostID) switch
                {
                    true => string.Format
                    (
                    "SELECT PostID FROM `{0}` WHERE {1}=( SELECT min({1}) FROM `{0}` WHERE PostID > {2} AND {3} REGEXP ?REGEXP )"
                    , UnionView, Prop, PostID, FilterProp
                    ),
                    false => string.Format
                    (
                    "SELECT PostID FROM `{0}` WHERE {1}=( SELECT min({1}) FROM `{0}` WHERE {1} > ( SELECT {1} FROM `{0}` WHERE PostID = ?PostID ) AND {2} REGEXP ?REGEXP )"
                    , UnionView, Prop, FilterProp
                    )
                };

                object NextPostID = MySqlManager.GetKey(SQL, new MySqlParameter[]
                {
                    new("PostID", PostID),
                    new("REGEXP", REGEXP)
                });

                return NextPostID == null ? -1 : Convert.ToInt32(NextPostID);
            }
            /// <summary>
            /// 取得具有比目标文章的指定属性具有更小的值的文章PostID
            /// </summary>
            /// <param name="PostID">目标文章的PostID</param>
            /// <param name="Prop">指定属性</param>
            /// <returns>不存在符合要求的PostID时，返回-1</returns>
            public int Smaller(int PostID, PostProp Prop)
            {
                string SQL = (Prop == PostProp.PostID) switch /* 对查询PostID有优化 */
                {
                    true => $"SELECT PostID FROM `{UnionView}` WHERE PostID=( SELECT max(PostID) FROM `{UnionView}` WHERE PostID < {PostID})",
                    false => string.Format
                    (
                    "SELECT PostID FROM `{0}` WHERE {1}=( SELECT max({1}) FROM `{0}` WHERE {1} < ( SELECT {1} FROM `{0}` WHERE PostID = {2} ))"
                    , UnionView, Prop, PostID
                    )
                };

                object PrevPostID = MySqlManager.GetKey(SQL);

                return PrevPostID == null ? -1 : Convert.ToInt32(PrevPostID);
            }
            /// <summary>
            /// 取得具有比目标文章的指定属性具有更小的值的文章PostID
            /// </summary>
            /// <param name="PostID">目标文章的PostID</param>
            /// <param name="Prop">指定属性</param>
            /// <param name="REGEXP">正则表达式</param>
            /// <param name="FilterProp">用于被正则表达式筛选的属性</param>
            /// <returns>不存在符合要求的PostID时，返回-1</returns>
            public int Smaller(int PostID, PostProp Prop, string REGEXP, PostProp FilterProp)
            {
                string SQL = (Prop == PostProp.PostID) switch
                {
                    true => string.Format
                    (
                    "SELECT PostID FROM `{0}` WHERE {1}=( SELECT max({1}) FROM `{0}` WHERE PostID < {2} AND {3} REGEXP ?REGEXP )"
                    , UnionView, Prop, PostID, FilterProp
                    ),
                    false => string.Format
                    (
                    "SELECT PostID FROM `{0}` WHERE {1}=( SELECT max({1}) FROM `{0}` WHERE {1} < ( SELECT {1} FROM `{0}` WHERE PostID = ?PostID ) AND {2} REGEXP ?REGEXP )"
                    , UnionView, Prop, FilterProp
                    )
                };

                object PrevPostID = MySqlManager.GetKey(SQL, new MySqlParameter[]
                {
                    new("PostID", PostID),
                    new("REGEXP", REGEXP)
                });

                return PrevPostID == null ? -1 : Convert.ToInt32(PrevPostID);
            }
        }
        /// <summary>
        /// 数据修改组件
        /// </summary>
        public class Writer : IPLComponent<Writer>
        {
            private string MetaTable { get; init; }
            private string StackTable { get; init; }
            private MySqlManager MySqlManager { get; init; }

            /// <summary>
            /// 默认构造
            /// </summary>
            private Writer() { }
            /// <summary>
            /// 工厂构造
            /// </summary>
            /// <param name="MetaTable">元数据表</param>
            /// <param name="StackTable">主表</param>
            /// <param name="MySqlManager">数据库管理器</param>
            /// <returns></returns>
            internal Writer(string MetaTable, string StackTable, MySqlManager MySqlManager)
            {
                this.MetaTable = MetaTable;
                this.StackTable = StackTable;
                this.MySqlManager = MySqlManager;
            }

            /// <summary>
            /// 得到最大PostID（私有）
            /// </summary>
            /// <returns></returns>
            internal int GetMaxPostID()
            {
                string SQL = $"SELECT MAX(PostID) FROM {MetaTable}";
                var result = MySqlManager.GetKey(SQL);
                /* 若取不到最大PostID(没有任何文章时)，返回12000作为初始PostID */
                return Convert.ToInt32(result == DBNull.Value ? 12000 : result);
            }
            /// <summary>
            /// 得到最小PostID（私有）
            /// </summary>
            /// <returns>错误则返回-1</returns>
            internal int GetMinPostID()
            {
                string SQL = $"SELECT MIN(PostID) FROM {MetaTable}";
                var result = MySqlManager.GetKey(SQL);
                /* 若取不到最大PostID(没有任何文章时)，返回12000作为初始PostID */
                return Convert.ToInt32(result == DBNull.Value ? 12000 : result);
            }
            /// <summary>
            /// 获取指定文章的积极备份的UUID
            /// </summary>
            /// <param name="PostID">目标文章PostID</param>
            /// <returns></returns>
            internal string GetPositiveUUID(int PostID)
            {
                return Convert.ToString(MySqlManager.GetKey($"SELECT UUID FROM {MetaTable} WHERE PostID={PostID}"));
            }
            /// <summary>
            /// 获取指定文章的消极备份的UUID
            /// </summary>
            /// <param name="PostID">目标文章PostID</param>
            /// <returns></returns>
            internal string GetNegativeUUID(int PostID)
            {
                return Convert.ToString(MySqlManager.GetKey(
                    string.Format
                    (
                    "SELECT {1}.UUID FROM {0} JOIN {1} ON {0}.PostID={1}.PostID AND {0}.UUID<>{1}.UUID WHERE {0}.PostID={2}"
                    , MetaTable, StackTable, PostID
                    )
                    ));
            }

            /// <summary>
            /// 注册文章
            /// </summary>
            /// <remarks>
            /// 新建一个拷贝，并将index指向该拷贝
            /// </remarks>
            /// <param name="Post">文章数据（其中的PostID、UUID、CT、LCT、User由系统生成）</param>
            /// <returns>返回受影响的行数</returns>
            public bool Reg(PostRecord Post)
            {
                return MySqlManager.DoInConnection(conn =>
                {
                    DateTime t = DateTime.Now;

                    string SQL =
                    $@"INSERT INTO {MetaTable}
                       ( PostID, UUID, CT, Mode, Type, User, UVCount, StarCount, ArchiveID) VALUES
                       (?PostID,?UUID,?CT,?Mode,?Type,?User,?UVCount,?StarCount,?ArchiveID);
                       INSERT INTO {StackTable}
                       ( PostID, UUID, LCT, Title, Summary, Content, Label, Cover) VALUES
                       (?PostID,?UUID,?LCT,?Title,?Summary,?Content,?Label,?Cover);";

                    MySqlParameter[] parameters =
                    {
                    new("PostID", GetMaxPostID() + 1 ),
                    new("UUID", MathH.GenerateUUID("N") ),

                    new("CT", t),
                    new("LCT", t),

                    new("User", Post.User),/* 指定用户账号 */

                    /* 可传参数 */
                    new("Mode", Post.Mode),
                    new("Type", Post.Type),
                    new("ArchiveID",0),

                    new("UVCount", Post.UVCount),
                    new("StarCount", Post.StarCount),

                    new("Title", Post.Title),
                    new("Summary", Post.Summary ),
                    new("Content", Post.Content ),

                    new("Label", Post.Label ),
                    new("Cover", Post.Cover )
                    };

                    return MySqlManager.DoInCommand(conn, cmd =>
                    {
                        cmd.CommandText = SQL;
                        cmd.Parameters.AddRange(parameters);

                        return MySqlManager.DoInTransaction(cmd, tx =>
                        {
                            if (cmd.ExecuteNonQuery() == 2)
                            {
                                /* 指向表和拷贝表分别添加1行数据 */
                                tx.Commit();
                                return true;
                            }
                            else
                            {
                                tx.Rollback();
                                return false;
                            }
                        });
                    });
                });
            }
            /// <summary>
            /// 注销文章
            /// </summary>
            /// <remarks>
            /// 删除所有拷贝和index指向
            /// </remarks>
            /// <param name="PostID">目标文章PostID</param>
            /// <returns></returns>
            public bool Dispose(int PostID)
            {
                return MySqlManager.DoInConnection(conn =>
                {
                    return MySqlManager.DoInCommand(conn, cmd =>
                    {
                        /* int参数无法用于参数化攻击 */
                        cmd.CommandText = $"DELETE FROM {MetaTable} WHERE PostID={PostID};DELETE FROM {StackTable} WHERE PostID={PostID};";

                        return MySqlManager.DoInTransaction(cmd, tx =>
                        {
                            if (cmd.ExecuteNonQuery() >= 2)
                            {
                                /* 指向表只删除1行数据，拷贝表至少删除1行数据 */
                                tx.Commit();
                                return true;
                            }
                            else
                            {
                                tx.Rollback();
                                return false;
                            }
                        });
                    });
                });
            }
            /// <summary>
            /// 更新文章
            /// </summary>
            /// <remarks>
            /// 此方法会新建一个拷贝，并将index更改为指向该拷贝
            /// </remarks>
            /// <param name="Post">文章数据</param>
            /// <returns></returns>
            public bool Update(PostRecord Post)
            {
                return MySqlManager.DoInConnection(conn =>
                {
                    string SQL =
                    $@"UPDATE {MetaTable} SET UUID=?UUID, Mode=?Mode, Type=?Type, User=?User, UVCount=?UVCount, StarCount=?StarCount, ArchiveID=?ArchiveID WHERE PostID=?PostID;
                       INSERT INTO {StackTable}
                       ( PostID, UUID, LCT, Title, Summary, Content, Label, Cover) VALUES
                       (?PostID,?UUID,?LCT,?Title,?Summary,?Content,?Label,?Cover);";

                    MySqlParameter[] parameters =
                    {
                    new("UUID", MathH.GenerateUUID("N") ),
                    new("LCT", DateTime.Now ),

                    new("User", Post.User),/* 指定用户账号 */

                    /* 可传参数 */
                    new("PostID", Post.ID),

                    new("Mode", Post.Mode),
                    new("Type", Post.Type),
                    new("ArchiveID",0),

                    new("UVCount", Post.UVCount),
                    new("StarCount", Post.StarCount),

                    new("Title", Post.Title),
                    new("Summary", Post.Summary),
                    new("Content", Post.Content),

                    new("Label", Post.Label),
                    new("Cover", Post.Cover)
                    };

                    return MySqlManager.DoInCommand(conn, cmd =>
                    {
                        cmd.CommandText = SQL;
                        cmd.Parameters.AddRange(parameters);

                        return MySqlManager.DoInTransaction(cmd, tx =>
                        {
                            if (cmd.ExecuteNonQuery() == 2)
                            {
                                /* 指向表修改1行数据，拷贝表添加1行数据 */
                                tx.Commit();
                                return true;
                            }
                            else
                            {
                                tx.Rollback();
                                return false;
                                /* 由于UUID更新，影响行始终为2，若出现其他情况则一定为错误 */
                            }
                        });
                    });
                });
            }

            /// <summary>
            /// 删除拷贝
            /// </summary>
            /// <remarks>
            /// 删除指定拷贝，且该拷贝不能为当前index指向
            /// </remarks>
            /// <param name="UUID">目标文章的UUID</param>
            /// <returns></returns>
            public bool Delete(string UUID)
            {
                return MySqlManager.DoInConnection(conn =>
                {
                    string SQL = string.Format
                    (
                    "DELETE {1} FROM {0} INNER JOIN {1} ON {0}.PostID={1}.PostID AND {0}.UUID<>{1}.UUID AND {1}.UUID = ?UUID"
                    , MetaTable, StackTable
                    );

                    MySqlParameter[] parameters = { new("UUID", UUID) };

                    return MySqlManager.DoInCommand(conn, cmd =>
                    {
                        cmd.CommandText = SQL;
                        cmd.Parameters.AddRange(parameters);

                        return MySqlManager.DoInTransaction(cmd, tx =>
                        {
                            if (cmd.ExecuteNonQuery() == 1)
                            {
                                /* 拷贝表删除一行数据 */
                                tx.Commit();
                                return true;
                            }
                            else
                            {
                                tx.Rollback();
                                return false;
                            }
                        });
                    });
                });
            }
            /// <summary>
            /// 应用拷贝
            /// </summary>
            /// <remarks>
            /// 将现有index指向删除（顶出），然后将index指向设置为指定文章拷贝
            /// </remarks>
            /// <param name="UUID">目标拷贝的UUID</param>
            /// <returns></returns>
            public bool Apply(string UUID)
            {
                return MySqlManager.DoInConnection(conn =>
                {
                    /* 此处，即使SQL注入造成了PostID错误，由于第二步参数化查询的作用，UUID也会造成错误无法成功攻击 */
                    object PostID = MySqlManager.GetKey($"SELECT PostID FROM {StackTable} WHERE UUID = '{UUID}'");

                    string SQL =
                    $@"DELETE FROM {StackTable} WHERE UUID = (SELECT UUID FROM {MetaTable} WHERE PostID = ?PostID);
                       UPDATE {StackTable} SET UUID = ?UUID WHERE PostID = ?PostID;";

                    MySqlParameter[] parameters =
                    { new("PostID", PostID), new("UUID", UUID) };

                    return MySqlManager.DoInCommand(conn, cmd =>
                    {
                        cmd.CommandText = SQL;
                        cmd.Parameters.AddRange(parameters);

                        return MySqlManager.DoInTransaction(cmd, tx =>
                        {
                            if (cmd.ExecuteNonQuery() == 2)
                            {
                                /* 指向表修改1行数据，拷贝表删除一行数据 */
                                tx.Commit();
                                return true;
                            }
                            else
                            {
                                tx.Rollback();
                                return false;
                            }
                        });
                    });
                });
            }
            /// <summary>
            /// 回滚拷贝
            /// </summary>
            /// <remarks>
            /// 将现有index指向删除（顶出），然后将index指向设置到另一个最近更新的拷贝
            /// </remarks>
            /// <param name="PostID">目标文章的PostID</param>
            /// <returns></returns>
            public bool Rollback(int PostID)
            {
                return MySqlManager.DoInConnection(conn =>
                {
                    return MySqlManager.DoInCommand(conn, cmd =>
                    {
                        cmd.CommandText = string.Format
                        (
                        @"DELETE {1} FROM {0} INNER JOIN {1} ON {0}.UUID={1}.UUID AND {0}.PostID={2};
                        UPDATE {0} SET UUID = (SELECT UUID FROM {1} WHERE PostID={2} ORDER BY LCT DESC LIMIT 0,1) WHERE PostID={2};"
                        , MetaTable, StackTable, PostID
                        );

                        return MySqlManager.DoInTransaction(cmd, tx =>
                        {
                            if (cmd.ExecuteNonQuery() == 2)
                            {
                                /* 指向表修改1行数据，拷贝表删除1行数据 */
                                tx.Commit();
                                return true;
                            }
                            else
                            {
                                tx.Rollback();
                                return false;
                            }
                        });
                    });
                });
            }
            /// <summary>
            /// 释放拷贝
            /// </summary>
            /// <remarks>删除非当前index指向的所有拷贝
            /// </remarks>
            /// <param name="PostID">目标文章的PostID</param>
            /// <returns></returns>
            public bool Release(int PostID)
            {
                return MySqlManager.DoInConnection(conn =>
                {
                    return MySqlManager.DoInCommand(conn, cmd =>
                    {
                        cmd.CommandText = string.Format
                        (
                        "DELETE {1} FROM {0} INNER JOIN {1} ON {0}.PostID={1}.PostID AND {0}.UUID<>{1}.UUID AND {0}.PostID={2}"
                        , MetaTable, StackTable, PostID
                        );

                        return MySqlManager.DoInTransaction(cmd, tx =>
                        {
                            if (cmd.ExecuteNonQuery() >= 0)
                            {
                                /* 删除拷贝表的所有冗余，不存在冗余时影响行数为0 */
                                tx.Commit();
                                return true;
                            }
                            else
                            {
                                tx.Rollback();
                                return false;
                            }
                        });
                    });
                });
            }

            /// <summary>
            /// 状态枚举
            /// </summary>
            public enum ModeStates
            {
                /// <summary>
                /// 未设置
                /// </summary>
                /// <remarks>默认的文章模式，不带有任何模式特性</remarks>
                Unset,
                /// <summary>
                /// 隐藏
                /// </summary>
                /// <remarks>文章被隐藏，此状态下的文章不会被展示</remarks>
                Hidden,
                /// <summary>
                /// 计划
                /// </summary>
                /// <remarks>表示文章处于计划状态</remarks>
                Scheduled,
                /// <summary>
                /// 归档
                /// </summary>
                /// <remarks>表示文章处于归档状态</remarks>
                Archived
            }
            /// <summary>
            /// 状态枚举
            /// </summary>
            public enum TypeStates
            {
                /// <summary>
                /// 未设置
                /// </summary>
                /// <remarks>默认的文章类型，不带有任何类型特性</remarks>
                Unset,
                /// <summary>
                /// 便签
                /// </summary>
                /// <remarks>表示文章以便签形式展示</remarks>
                Note,
            }
            /// <summary>
            /// 设置文章类型
            /// </summary>
            /// <param name="PostID">文章索引</param>
            /// <param name="TypeState">目标类型</param>
            /// <returns></returns>
            public bool UpdateType(int PostID, TypeStates TypeState)
            {
                bool fun(string value)
                {
                    var SET = ("Type", value);
                    var WHERE = ("PostID", PostID);
                    return MySqlManager.ExecuteUpdate(MetaTable, SET, WHERE);
                }
                return TypeState switch
                {
                    TypeStates.Unset => fun(""),
                    TypeStates.Note => fun("note"),
                    _ => throw new NotImplementedException("模式匹配失败")
                };
            }
            /// <summary>
            /// 设置文章模式
            /// </summary>
            /// <param name="PostID">文章索引</param>
            /// <param name="ModeState">目标模式</param>
            /// <returns></returns>
            public bool UpdateMode(int PostID, ModeStates ModeState)
            {
                bool fun(string value)
                {
                    var SET = ("Mode", value);
                    var WHERE = ("PostID", PostID);
                    return MySqlManager.ExecuteUpdate(MetaTable, SET, WHERE);
                };

                return ModeState switch
                {
                    ModeStates.Unset => fun(""),
                    ModeStates.Hidden => fun("hidden"),
                    ModeStates.Scheduled => fun("scheduled"),
                    ModeStates.Archived => fun("archived"),
                    _ => throw new NotImplementedException("模式匹配失败")
                };
            }
        }
        /// <summary>
        /// 计数管理组件
        /// </summary>
        public class Counter : IPLComponent<Counter>
        {
            private string MetaTable { get; init; }
            private string StackTable { get; init; }
            private MySqlManager MySqlManager { get; init; }

            /// <summary>
            /// 默认构造
            /// </summary>
            private Counter() { }
            /// <summary>
            /// 工厂构造
            /// </summary>
            /// <param name="MetaTable">元数据表</param>
            /// <param name="StackTable">主表</param>
            /// <param name="MySqlManager">数据库管理器</param>
            /// <returns></returns>
            internal Counter(string MetaTable, string StackTable, MySqlManager MySqlManager)
            {
                this.MetaTable = MetaTable;
                this.StackTable = StackTable;
                this.MySqlManager = MySqlManager;
            }

            /// <summary>
            /// 文章计数
            /// </summary>
            public int TotalPostCount
            {
                get => GetPostCountByMode("^");
            }
            /// <summary>
            /// 拷贝计数
            /// </summary>
            public int StackCount
            {
                get => GetStackCount();
            }

            /// <summary>
            /// 隐藏文章计数
            /// </summary>
            public int HiddenCount
            {
                get => GetPostCountByMode("^hidden$");
            }
            /// <summary>
            /// 展示中文章计数
            /// </summary>
            public int OnDisplayCount
            {
                get => GetPostCountByMode("^$");
            }
            /// <summary>
            /// 归档中文章计数
            /// </summary>
            public int ArchivedCount
            {
                get => GetPostCountByMode("^archived$");
            }
            /// <summary>
            /// 计划中文章计数
            /// </summary>
            public int ScheduledCount
            {
                get => GetPostCountByMode("^scheduled$");
            }

            /// <summary>
            /// 设置星星计数
            /// </summary>
            /// <param name="PostID">文章ID</param>
            /// <param name="Value">值</param>
            /// <returns></returns>
            public bool SetStarCount(int PostID, int Value)
            {
                var SET = ("StarCount", Value);
                var WHERE = ("PostID", PostID);
                return MySqlManager.ExecuteUpdate(MetaTable, SET, WHERE);
            }
            /// <summary>
            /// 设置浏览计数
            /// </summary>
            /// <param name="PostID">文章ID</param>
            /// <param name="Value">值</param>
            /// <returns></returns>
            public bool SetUVCount(int PostID, int Value)
            {
                var SET = ("UVCount", Value);
                var WHERE = ("PostID", PostID);
                return MySqlManager.ExecuteUpdate(MetaTable, SET, WHERE);
            }

            private int GetPostCountByMode(string REGEXP)
            {
                object Count = MySqlManager.GetKey($"SELECT Count(*) FROM {MetaTable} WHERE Mode REGEXP '{REGEXP}';");
                return Count == DBNull.Value ? 0 : Convert.ToInt32(Count);
            }
            private int GetStackCount()
            {
                object Count = MySqlManager.GetKey(string.Format("SELECT COUNT(*) FROM {0},{1} WHERE {0}.PostID={1}.PostID AND {0}.UUID<>{1}.UUID;", MetaTable, StackTable));
                return Count == DBNull.Value ? 0 : Convert.ToInt32(Count);
            }
        }
        /// <summary>
        /// 归档管理组件
        /// </summary>
        public class Archiver : IPLComponent<Archiver>
        {
            private string ArchiveTable { get; set; }
            private string MetaTable { get; set; }
            private MySqlManager MySqlManager { get; init; }
            private readonly Dictionary<string, int> ArchiveCache = new();//归档表缓存
            private void RefreshCache()
            {
                ArchiveCache.Clear();
                //重建缓存
                foreach (DataRow Row in MySqlManager.GetTable($"SELECT * FROM {ArchiveTable}").Rows)
                {
                    ArchiveCache.Add(Row["Name"].ToString(), Convert.ToInt32(Row["ArchiveID"]));
                }
            }


            /// <summary>
            /// 默认构造
            /// </summary>
            private Archiver() { }
            /// <summary>
            /// 工厂构造
            /// </summary>
            /// <param name="ArchiveTable">归档表</param>
            /// <param name="MySqlManager">数据库管理器</param>
            /// <returns></returns>
            internal Archiver(string ArchiveTable, MySqlManager MySqlManager)
            {
                this.ArchiveTable = ArchiveTable;
                this.MySqlManager = MySqlManager;

                RefreshCache();//建立缓存
            }

            /// <summary>
            /// 得到最大ArchiveID（私有）
            /// </summary>
            /// <returns></returns>
            internal int GetMaxArchiveID()
            {
                /* 始终具有最大ArchiveID，因为0是无归档状态 */
                return ArchiveCache.Values.OrderBy(x => x).Last();
            }

            /// <summary>
            /// 创建归档
            /// </summary>
            /// <param name="ArchiveName">归档名</param>
            /// <returns></returns>
            public bool NewArchive(string ArchiveName)
            {
                int ArchiveID = GetMaxArchiveID() + 1;

                return MySqlManager.DoInConnection(conn =>
                {
                    return MySqlManager.DoInCommand(conn, cmd =>
                    {
                        return MySqlManager.DoInTransaction(cmd, tx =>
                        {
                            int affectedArchives = cmd.ExecuteInsert(ArchiveTable,
                                new("ArchiveID", ArchiveID),
                                new("Name", ArchiveName));
                            if (affectedArchives == 1)
                            {
                                tx.Commit();//归档被删除时提交
                                ArchiveCache.Add(ArchiveName, ArchiveID);//同步到缓存
                                return true;
                            }
                            else
                            {
                                tx.Rollback();
                                return false;
                            }
                        });
                    });
                });
            }
            /// <summary>
            /// 注销归档
            /// </summary>
            /// <remarks>不可注销归档"无"，否则将返回false</remarks>
            /// <param name="ArchiveName">归档名</param>
            /// <returns>受影响的文章数</returns>
            public int DelArchive(string ArchiveName)
            {
                if (ArchiveName != "无")
                {
                    var SET = ("ArchiveID", 0);
                    var OldValue = ArchiveCache[ArchiveName];
                    var result = MySqlManager.DoInConnection(conn =>
                    {
                        return MySqlManager.DoInCommand(conn, cmd =>
                        {
                            return MySqlManager.DoInTransaction(cmd, tx =>
                            {
                                int affectedArchives = cmd.ExecuteDelete(ArchiveTable, ("Name", ArchiveName));//删除归档
                                int affectedPosts = cmd.ExecuteUpdate(MetaTable, SET, OldValue);//将该归档内的文章设为无归档
                                if (affectedArchives == 1)
                                {
                                    tx.Commit();//归档被删除时提交
                                    ArchiveCache.Remove(ArchiveName);//同步到缓存
                                    return affectedPosts;
                                }
                                else
                                {
                                    tx.Rollback();
                                    return 0;//影响行数异常，返回0
                                }
                            });
                        });
                    });
                    return result;
                }
                else return 0;
            }

            /// <summary>
            /// 将文章移入归档
            /// </summary>
            /// <param name="ArchiveName">归档名</param>
            /// <param name="PostID">目标文章PostID</param>
            /// <returns></returns>
            public bool ArchiveIn(string ArchiveName, int PostID)
            {
                var SET = ("ArchiveID", ArchiveCache[ArchiveName]);
                var WHERE = ("PostID", PostID);
                return MySqlManager.ExecuteUpdate(MetaTable, SET, WHERE);
            }
            /// <summary>
            /// 将文章设为无归档
            /// </summary>
            /// <param name="PostID">目标文章PostID</param>
            /// <returns></returns>
            public bool ArchiveOut(int PostID)
            {
                var SET = ("ArchiveID", 0);
                var WHERE = ("PostID", PostID);
                return MySqlManager.ExecuteUpdate(MetaTable, SET, WHERE);
            }

            /// <summary>
            /// 归档下的文章计数
            /// </summary>
            /// <param name="ArchiveName">归档名</param>
            /// <returns></returns>
            public int Count(string ArchiveName)
            {
                return Convert.ToInt32(
                    MySqlManager.GetKey($"SELECT COUNT(*) FROM {ArchiveTable} WHERE Name = ?Name",
                    new MySqlParameter[] { new("Name", ArchiveName) }));
            }
        }
        /// <summary>
        /// 插件管理组件
        /// </summary>
        public class Pluginer : IPLComponent<Pluginer>
        {
            /// <summary>
            /// 工厂构造
            /// </summary>
            /// <param name="pluginList">预加载插件信息列表</param>
            /// <returns></returns>
            internal Pluginer()
            {

            }


            private readonly Dictionary<string, (Type type, object obj)> PluginPool = new();//插件池

            /// <summary>
            /// 加载插件
            /// </summary>
            /// <param name="path">插件dll所在目录</param>
            /// <param name="pluginName">插件名(格式如namesapace.pluginName)</param>
            /// <param name="args">构造参数表</param>
            /// <returns>返回插件UUID</returns>
            public string LoadPlugin(string path, string pluginName, params object[] args)
            {
                System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFrom(path);
                var type = assembly.GetType(pluginName);
                var inst = Activator.CreateInstance(type, args);
                var pluginUUID = MathH.GenerateUUID("N");
                PluginPool.Add(pluginUUID, (type, inst));
                return pluginUUID;//返回生成的插件UUID作为插件池键值
            }
            /// <summary>
            /// 卸载插件
            /// </summary>
            /// <param name="pluginUUID">插件UUID</param>
            public void UnloadPlugin(string pluginUUID)
            {
                PluginPool.Remove(pluginUUID);
            }
            /// <summary>
            /// 执行插件方法
            /// </summary>
            /// <param name="pluginUUID">插件UUID</param>
            /// <param name="methodName">方法名</param>
            /// <param name="args">方法参数表</param>
            /// <returns></returns>
            public object Invoke(string pluginUUID, string methodName, params object[] args)
            {
                var (inst, obj) = PluginPool[pluginUUID];
                MethodInfo method = inst.GetMethod(methodName);
                return method.Invoke(obj, args);
            }
        }
    }
}
