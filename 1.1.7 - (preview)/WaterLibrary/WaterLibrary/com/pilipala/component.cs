namespace WaterLibrary.pilipala.Component
{
    using System;
    using System.Linq;
    using System.Data;
    using System.Reflection;
    using System.Collections.Generic;

    using MySql.Data.MySqlClient;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    using WaterLibrary.MySQL;
    using WaterLibrary.Utils;
    using WaterLibrary.pilipala.Entity;
    using WaterLibrary.pilipala.Database;


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
            var manager = PiliPala.MySqlManager;

            var PWD_MD5 = MathH.MD5(UserPWD).ToLower();
            var para = new MySqlParameter[] { new("UserAccount", UserAccount), new("UserPWD", PWD_MD5) };

            var count = Convert.ToInt32(manager.GetRow
                ($"SELECT COUNT(*) FROM {PiliPala.Tables.User} WHERE Account = ?UserAccount AND PWD = ?UserPWD", para)[0]);

            if (count == 1)
            {
                return new User(PiliPala.Tables, manager, UserAccount);
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
        public Auth GenAuthentication(User User) => new(PiliPala.Tables, PiliPala.MySqlManager, User);
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
                    false => new(PiliPala.ViewsSet.CleanViews.PosUnion, PiliPala.MySqlManager),
                    true => new(PiliPala.ViewsSet.CleanViews.NegUnion, PiliPala.MySqlManager),
                },
                Reader.ReadMode.DirtyRead => WithRawMode switch
                {
                    false => new(PiliPala.ViewsSet.DirtyViews.PosUnion, PiliPala.MySqlManager),
                    true => new(PiliPala.ViewsSet.DirtyViews.NegUnion, PiliPala.MySqlManager),
                },
                _ => throw new NotImplementedException(),
            };
        }
        /// <summary>
        /// 生成写组件
        /// </summary>
        /// <returns></returns>
        [Obsolete("请使用PostRecord属性访问器以完成对属性的修改")]
        public Writer GenWriter() => new(PiliPala.Tables.Meta, PiliPala.Tables.Stack, PiliPala.MySqlManager);
        /// <summary>
        /// 生成计数组件
        /// </summary>
        /// <returns></returns>
        public Counter GenCounter() => new(PiliPala.Tables.Meta, PiliPala.Tables.Stack, PiliPala.MySqlManager);
        /// <summary>
        /// 生成插件组件
        /// </summary>
        /// <returns></returns>
        public Pluginer GenPluginer() => new();
        /// <summary>
        /// 生成归档管理组件
        /// </summary>
        /// <returns></returns>
        public Archiver GenArchiver() => new(PiliPala.Tables.Archive, PiliPala.MySqlManager);
        /// <summary>
        /// 生成评论湖组件
        /// </summary>
        /// <returns></returns>
        public CommentLake GenCommentLake() => new(PiliPala.Tables.Meta, PiliPala.Tables.Comment, PiliPala.MySqlManager);
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

        internal MySqlManager MySqlManager { get; init; }

        /// <summary>
        /// 默认构造
        /// </summary>
        private Reader() { }
        /// <summary>
        /// 工厂构造
        /// </summary>
        /// <param name="UnionView">联合视图</param>
        /// <param name="MySqlManager">数据库管理器</param>
        /// <returns></returns>
        internal Reader(string UnionView, MySqlManager MySqlManager)
        {
            this.UnionView = UnionView;
            this.MySqlManager = MySqlManager;
        }

        /*/// <summary>
        /// 获取指定文章数据
        /// </summary>
        /// <param name="PostID">目标文章PostID</param>
        /// <returns></returns>
        public PostRecord GetPost(int PostID)
        {
            var SQL = $"SELECT * FROM `{UnionView}` WHERE PostID={PostID}";
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
            var SQL = $"SELECT {Prop} FROM `{UnionView}` WHERE PostID = ?PostID";

            return MySqlManager.GetKey(SQL, new MySqlParameter[]
            {
                    new("PostID", PostID)
            });
        }

        /// <summary>
        /// 获取文章栈
        /// </summary>
        /// <param name="Prop">正则表达式匹配的属性类型</param>
        /// <param name="REGEXP">正则表达式</param>
        /// <returns></returns>
        public PostStackSet GetPostStacks(PostProp Prop, string REGEXP)
        {
            PostStackSet PostStackSet = new();

            var SQL = $"SELECT PostID FROM `{UnionView}` WHERE {Prop} REGEXP ?REGEXP ORDER BY CT DESC";

            var para = new MySqlParameter("REGEXP", REGEXP);
            var Rows = MySqlManager.GetTable(SQL, para).Rows;

            foreach (DataRow Row in Rows)
            {
                PostStackSet.Add(new PostStack(Convert.ToUInt32(Row["PostID"])));
            }

            return PostStackSet;
        }
        /// <summary>
        /// 获取文章栈
        /// </summary>
        /// <param name="Prop">正则表达式匹配的属性类型</param>
        /// <param name="REGEXP">正则表达式</param>
        /// <param name="PostProps">所需属性类型</param>
        /// <returns></returns>
        [Obsolete("应使用PostRecord惰性求值完成对所需属性的筛选")]
        public PostStackSet GetPostStacks(PostProp Prop, string REGEXP, params PostProp[] PostProps)
        {
            PostStackSet PostStackSet = new();

            string KeysStr = ConvertH.ListToString(PostProps, ',');/* 键名字符串格式化 */
            var SQL = $"SELECT PostID,{KeysStr} FROM `{UnionView}` WHERE {Prop} REGEXP ?REGEXP ORDER BY CT DESC";

            var para = new MySqlParameter("REGEXP", REGEXP);
            var Rows = MySqlManager.GetTable(SQL, para).Rows;

            foreach (DataRow Row in Rows)
            {
                PostStack PostStack = new(Convert.ToUInt32(Row["ID"]));

                /*for (int i = 0; i < PostProps.Length; i++)
                {
                    Post[PostProps[i].ToString()] = Row.ItemArray[i];
                }*/

                PostStackSet.Add(PostStack);
            }

            return PostStackSet;
        }
    }
    /// <summary>
    /// 数据修改组件
    /// </summary>
    [Obsolete("请使用PostRecord属性访问器以完成对属性的修改")]
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
        /// 获取指定文章的积极备份的UUID
        /// </summary>
        /// <param name="ID">目标文章PostID</param>
        /// <returns></returns>
        [Obsolete("请使用PostRecord")]
        internal string GetPositiveUUID(int ID)
        {
            return Convert.ToString(MySqlManager.GetKey($"SELECT UUID FROM {MetaTable} WHERE PostID={ID}"));
        }
        /// <summary>
        /// 获取指定文章的消极备份的UUID
        /// </summary>
        /// <param name="ID">目标文章PostID</param>
        /// <returns></returns>
        [Obsolete("请使用PostRecord")]
        internal string GetNegativeUUID(int ID)
        {
            var SQL = string.Format
                (
                "SELECT {1}.UUID FROM {0} JOIN {1} ON {0}.PostID={1}.PostID AND {0}.UUID<>{1}.UUID WHERE {0}.PostID={2}"
                , MetaTable, StackTable, ID
                );
            return Convert.ToString(MySqlManager.GetKey(SQL));
        }

        /// <summary>
        /// 设置文章类型
        /// </summary>
        /// <param name="PostID">文章索引</param>
        /// <param name="TypeState">目标类型</param>
        /// <returns></returns>
        [Obsolete("请使用PostRecord属性访问器")]
        public bool UpdateType(int PostID, PostRecord.TypeState TypeState)
        {
            bool fun(string value)
            {
                var SET = ("Type", value);
                var WHERE = ("PostID", PostID);
                return MySqlManager.ExecuteUpdate(MetaTable, SET, WHERE);
            }
            return TypeState switch
            {
                PostRecord.TypeState.unset => fun("unset"),
                PostRecord.TypeState.note => fun("note"),
                _ => throw new NotImplementedException("模式匹配失败")
            };
        }
        /// <summary>
        /// 设置文章模式
        /// </summary>
        /// <param name="PostID">文章索引</param>
        /// <param name="ModeState">目标模式</param>
        /// <returns></returns>
        [Obsolete("请使用PostRecord属性访问器")]
        public bool UpdateMode(int PostID, PostRecord.ModeState ModeState)
        {
            bool fun(string value)
            {
                var SET = ("Mode", value);
                var WHERE = ("PostID", PostID);
                return MySqlManager.ExecuteUpdate(MetaTable, SET, WHERE);
            };

            return ModeState switch
            {
                PostRecord.ModeState.unset => fun("unset"),
                PostRecord.ModeState.hidden => fun("hidden"),
                PostRecord.ModeState.scheduled => fun("scheduled"),
                PostRecord.ModeState.archived => fun("archived"),
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
        [Obsolete("请使用PostRecord属性访问器")]
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
        [Obsolete("请使用PostRecord属性访问器")]
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
            Assembly assembly = Assembly.LoadFrom(path);

            var type = assembly.GetType(pluginName);
            var inst = Activator.CreateInstance(type, args);
            var pluginUUID = MathH.GenerateUUID(MathH.UUIDFormat.N);

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

    /// <summary>
    /// 评论湖组件
    /// </summary>
    public class CommentLake : IPLComponent<CommentLake>
    {
        private string MetaTable { get; init; }
        private string CommentTable { get; init; }
        /// <summary>
        /// MySql数据库管理器
        /// </summary>
        private MySqlManager MySqlManager { get; init; }

        /// <summary>
        /// 默认构造
        /// </summary>
        private CommentLake() { }
        /// <summary>
        /// 工厂构造
        /// </summary>
        /// <param name="MetaTable">元数据表</param>
        /// <param name="CommentTable">评论表</param>
        /// <param name="MySqlManager">数据库管理器</param>
        internal CommentLake(string MetaTable, string CommentTable, MySqlManager MySqlManager)
        {
            (this.MetaTable, this.CommentTable, this.MySqlManager) = (MetaTable, CommentTable, MySqlManager);
        }

        /// <summary>
        /// 得到最大评论CommentID（私有）
        /// </summary>
        /// <returns>不存在返回1</returns>
        private int GetMaxCommentID()
        {
            string SQL = $"SELECT max(CommentID) FROM {CommentTable}";

            object MaxCommentID = MySqlManager.GetKey(SQL);
            return MaxCommentID == DBNull.Value ? 1 : Convert.ToInt32(MaxCommentID);
        }
        /// <summary>
        /// 获得目标文章下的最大评论楼层（私有）
        /// </summary>
        /// <returns>不存在返回1</returns>
        private int GetMaxFloor(int PostID)
        {
            string SQL = $"SELECT max(Floor) FROM {CommentTable} WHERE PostID = {PostID}";

            object MaxFloor = MySqlManager.GetKey(SQL);
            return MaxFloor == DBNull.Value ? 1 : Convert.ToInt32(MaxFloor);
        }

        /// <summary>
        /// 评论总计数
        /// </summary>
        public int TotalCommentCount
        {
            get
            {
                object Count = MySqlManager.GetKey($"SELECT COUNT(*) FROM {CommentTable}");
                return Count == DBNull.Value ? 0 : Convert.ToInt32(Count);
            }
        }
        /// <summary>
        /// 得到目标文章的评论计数
        /// </summary>
        /// <param name="PostID">目标文章PostID</param>
        /// <returns></returns>
        public int GetCommentCount(int PostID)
        {
            object Count = MySqlManager.GetKey($"SELECT COUNT(*) FROM {CommentTable} WHERE PostID = {PostID}");
            return Count == DBNull.Value ? 0 : Convert.ToInt32(Count);
        }

        /// <summary>
        /// 获得评论属性
        /// </summary>
        /// <param name="CommentID">目标评论CommentID</param>
        /// <param name="Prop">属性类型</param>
        /// <returns></returns>
        public string GetComment(int CommentID, CommentProp Prop)
        {
            /* int类型传入，SQL注入无效 */
            string SQL = $"SELECT {Prop} FROM {CommentTable} WHERE CommentID = {CommentID}";
            return MySqlManager.GetKey(SQL).ToString();
        }

        /// <summary>
        /// 得到被评论文章的PostID列表
        /// </summary>
        /// <returns></returns>
        public List<int> GetCommentedPostID()
        {
            List<int> List = new();

            string SQL = string.Format("SELECT PostID FROM {0} JOIN {1} ON {0}.PostID={1}.PostID GROUP BY {0}.PostID", MetaTable, CommentTable);

            foreach (DataRow Row in MySqlManager.GetTable(SQL).Rows)
            {
                List.Add(Convert.ToInt32(Row[0]));
            }

            return List;
        }
        /// <summary>
        /// 获得目标文章的评论列表
        /// </summary>
        /// <param name="PostID">目标文章PostID</param>
        /// <returns></returns>
        public CommentRecordSet GetComments(int PostID)
        {
            CommentRecordSet CommentRecordSet = new();

            /* 按楼层排序 */
            string SQL = $"SELECT * FROM {CommentTable} WHERE PostID = ?PostID ORDER BY Floor";

            DataTable result = MySqlManager.GetTable(SQL, new MySqlParameter[]
            {
                new("PostID", PostID)
            });

            foreach (DataRow Row in result.Rows)
            {
                CommentRecordSet.Add(new CommentRecord
                {
                    CommentID = Convert.ToInt32(Row["CommentID"]),
                    HEAD = Convert.ToInt32(Row["HEAD"]),
                    PostID = Convert.ToInt32(Row["PostID"]),
                    Floor = Convert.ToInt32(Row["Floor"]),

                    User = Convert.ToString(Row["User"]),
                    Email = Convert.ToString(Row["Email"]),
                    Content = Convert.ToString(Row["Content"]),
                    WebSite = Convert.ToString(Row["WebSite"]),
                    Time = Convert.ToDateTime(Row["Time"]),
                });
            }
            return CommentRecordSet;
        }
        /// <summary>
        /// 获得目标评论的回复列表
        /// </summary>
        /// <param name="CommentID"></param>
        /// <returns></returns>
        public CommentRecordSet GetCommentReplies(int CommentID)
        {
            CommentRecordSet CommentRecordSet = new();

            DataTable result =
                MySqlManager.GetTable($"SELECT * FROM {CommentTable} WHERE HEAD={CommentID} ORDER BY Floor");

            foreach (DataRow Row in result.Rows)
            {
                CommentRecordSet.Add(new CommentRecord
                {
                    /* 数据库中的量 */
                    CommentID = Convert.ToInt32(Row["CommentID"]),
                    HEAD = Convert.ToInt32(Row["HEAD"]),
                    PostID = Convert.ToInt32(Row["PostID"]),
                    Floor = Convert.ToInt32(Row["Floor"]),

                    User = Convert.ToString(Row["User"]),
                    Email = Convert.ToString(Row["Email"]),
                    Content = Convert.ToString(Row["Content"]),
                    WebSite = Convert.ToString(Row["WebSite"]),
                    Time = Convert.ToDateTime(Row["Time"]),
                });
            }
            return CommentRecordSet;
        }

        /// <summary>
        /// 添加评论(CommentID和Time由系统生成，无需传入)
        /// </summary>
        /// <param name="item">评论记录</param>
        /// <returns></returns>
        public bool NewComment(CommentRecord item)
        {
            string SQL = $"INSERT INTO {CommentTable} " +
                        "( CommentID, HEAD, PostID, Floor, User, Email, Content, WebSite, Time) VALUES " +
                        "(?CommentID,?HEAD,?PostID,?Floor,?User,?Email,?Content,?WebSite,?Time)";

            MySqlParameter[] parameters =
            {
                new("CommentID", GetMaxCommentID() + 1 ),
                new("HEAD", item.HEAD),
                new("PostID", item.PostID),
                new("Floor", GetMaxFloor(item.PostID) + 1 ),

                new("User", item.User),
                new("Email", item.Email),
                new("Content", item.Content),
                new("WebSite", item.WebSite),
                new("Time", DateTime.Now),
            };

            return MySqlManager.DoInConnection(conn =>
            {
                return MySqlManager.DoInCommand(conn, cmd =>
                {
                    cmd.CommandText = SQL;
                    cmd.Parameters.AddRange(parameters);

                    return MySqlManager.DoInTransaction(cmd, tx =>
                    {
                        if (cmd.ExecuteNonQuery() == 1)
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
        /// 删除评论(相关回复不会被删除)
        /// </summary>
        /// <param name="CommentID"></param>
        /// <returns></returns>
        public bool DelComment(int CommentID)
        {
            return MySqlManager.DoInConnection(conn =>
            {
                return MySqlManager.DoInCommand(conn, cmd =>
                {
                    cmd.CommandText = $"DELETE FROM {CommentTable} WHERE CommentID = {CommentID}";

                    return MySqlManager.DoInTransaction(cmd, tx =>
                    {
                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            /* 删除1条评论，操作行为1 */
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
    }
}
