namespace WaterLibrary.pilipala.Entity
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using MySql.Data.MySqlClient;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    using WaterLibrary.MySQL;
    using WaterLibrary.Utils;


    /// <summary>
    /// 文章记录
    /// </summary>
    public class PostRecord
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
        /// 数据结构只读
        /// </summary>
        internal bool ReadOnly { get; init; }
        /// <summary>
        /// 字段缓存
        /// </summary>
        private readonly Dictionary<string, object> fieldCache = new();

        /// <summary>
        /// 字段读取器
        /// </summary>
        /// <param name="table">目标表</param>
        /// <param name="field">目标字段</param>
        /// <returns></returns>
        private object GetFieldByID(string table, string field)
        {
            if (fieldCache.ContainsKey(field))
            {
                return fieldCache[field];
            }
            else
            {
                var SQL = $"SELECT {field} FROM `{table}` WHERE PostID = ?ID";
                var para = new MySqlParameter("ID", ID);

                var result = manager.GetRow(SQL, para)[field];

                fieldCache[field] = result;//写入缓存并返回
                return result;
            }
        }
        /// <summary>
        /// 字段读取器
        /// </summary>
        /// <param name="table">目标表</param>
        /// <param name="field">目标字段</param>
        /// <returns></returns>
        private object GetFieldByUUID(string table, string field)
        {
            if (fieldCache.ContainsKey(field))
            {
                return fieldCache[field];
            }
            else
            {
                var SQL = $"SELECT {field} FROM `{table}` WHERE UUID = ?UUID";
                var para = new MySqlParameter("UUID", UUID);

                var result = manager.GetRow(SQL, para)[field];

                fieldCache[field] = result;//写入缓存并返回
                return result;
            }
        }
        /// <summary>
        /// 字段设置器
        /// </summary>
        /// <param name="table">目标表</param>
        /// <param name="field">目标字段</param>
        /// <param name="newValue">新值</param>
        private void SetFieldByID(string table, string field, object newValue)
        {
            (string, object) SET = (field, newValue);
            (string, object) WHERE = ("PostID", ID);

            if (ReadOnly)
            {
                throw new Exception("该记录只读");
            }
            else
            {
                manager.ExecuteUpdate(table, SET, WHERE);
                fieldCache[field] = newValue;
            }
        }
        /// <summary>
        /// 字段设置器
        /// </summary>
        /// <param name="table">目标表</param>
        /// <param name="field">目标字段</param>
        /// <param name="newValue">新值</param>
        private void SetFieldByUUID(string table, string field, object newValue)
        {
            (string, object) SET = (field, newValue);
            (string, object) WHERE = ("UUID", UUID);

            if (ReadOnly)
            {
                throw new Exception("该记录只读");
            }
            else
            {
                manager.ExecuteUpdate(table, SET, WHERE);
                fieldCache[field] = newValue;
            }
        }


        private readonly string metaTable = CORE.Tables.Meta;
        private readonly string stackTable = CORE.Tables.Stack;
        private readonly string unionView = CORE.ViewsSet.DirtyViews.NegUnion;
        private readonly MySqlManager manager = CORE.MySqlManager;

        /// <summary>
        /// 新建模式
        /// </summary>
        /// <param name="ID">隶属于文章栈的ID</param>
        public PostRecord(uint ID)
        {
            var SQL =
                    $@"INSERT INTO {stackTable}
                       ( PostID, UUID, LCT, Title, Summary, Content, Label, Cover) VALUES
                       (?ID,    ?UUID,?LCT,?Title,?Summary,?Content,?Label,?Cover);";

            var UUID = MathH.GenerateUUID(MathH.UUIDFormat.N);
            var LCT = DateTime.Now;
            var Mode = ModeState.unset;
            var Type = TypeState.unset;

            MySqlParameter[] parameters =
            {
                new("ID", ID),
                new("UUID", UUID),
                new("LCT", LCT),

                new("Mode", Mode),
                new("Type", Type),
                new("User", ""),
                new("ArchiveID", ""),

                new("UVCount", ""),
                new("StarCount", ""),

                new("Title", ""),
                new("Summary", ""),
                new("Content", ""),

                new("Label", ""),
                new("Cover", "")
            };

            manager.DoInConnection(conn =>
            {
                return MySqlManager.DoInCommand(conn, cmd =>
                {
                    cmd.CommandText = SQL;
                    cmd.Parameters.AddRange(parameters);

                    return MySqlManager.DoInTransaction(cmd, tx =>
                    {
                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            /* stack表添加1行数据 */
                            tx.Commit();

                            {//写缓存
                                fieldCache["ID"] = ID;
                                fieldCache["UUID"] = UUID;
                                fieldCache["LCT"] = LCT;

                                fieldCache["Mode"] = Mode;
                                fieldCache["Type"] = Type;

                                fieldCache["User"] = "";
                                fieldCache["ArchiveID"] = "";

                                fieldCache["UVCount"] = "";
                                fieldCache["StarCount"] = "";

                                fieldCache["Title"] = "";
                                fieldCache["Summary"] = "";
                                fieldCache["Content"] = "";

                                fieldCache["Label"] = "";
                                fieldCache["Cover"] = "";
                            }

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
        /// 编辑模式
        /// </summary>
        /// <param name="UUID">文章记录的UUID</param>
        /// <param name="lazy">惰性求值模式（默认为true，仅当需要时才请求数据库获得字段值）</param>
        /// <param name="readOnly">只读模式（默认为true，不允许修改文章栈字段）</param>
        public PostRecord(string UUID, bool lazy = true, bool readOnly = true)
        {
            this.UUID = UUID;

            ReadOnly = readOnly;

            if (!lazy)
            {
                //TODO
            }
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
        public uint ID
        {
            get => Convert.ToUInt32(GetFieldByUUID(CORE.Tables.Stack, "PostID"));
        }
        /// <summary>
        /// 全局标识
        /// </summary>
        [MinLength(32, ErrorMessage = "长度不符")]
        [MaxLength(32, ErrorMessage = "长度不符")]
        public string UUID { get; init; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get => Convert.ToString(GetFieldByUUID(CORE.Tables.Stack, "Title"));
            set => SetFieldByUUID(CORE.Tables.Stack, "Title", value);
        }
        /// <summary>
        /// 概要
        /// </summary>
        public string Summary
        {
            get => Convert.ToString(GetFieldByUUID(CORE.Tables.Stack, "Summary"));
            set => SetFieldByUUID(CORE.Tables.Stack, "Summary", value);
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
            get => Convert.ToString(GetFieldByUUID(CORE.Tables.Stack, "Content"));
            set => SetFieldByUUID(CORE.Tables.Stack, "Content", value);
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
        public string Cover
        {
            get => Convert.ToString(GetFieldByUUID(stackTable, "Cover"));
            set => SetFieldByUUID(stackTable, "Cover", value);
        }

        /// <summary>
        /// 归档ID（请使用Archiver设置）
        /// </summary>
        public uint ArchiveID
        {
            get => Convert.ToUInt32(GetFieldByID(metaTable, "ArchiveID"));
        }
        /// <summary>
        /// 归档名（请使用Archiver设置）
        /// </summary>
        public string ArchiveName
        {
            get => Convert.ToString(GetFieldByID(unionView, "ArchiveName"));
        }

        /// <summary>
        /// 标签
        /// </summary>
        public string Label
        {
            get => Convert.ToString(GetFieldByUUID(stackTable, "Label"));
            set => SetFieldByUUID(stackTable, "Label", value);
        }
        /// <summary>
        /// 获得标签集合
        /// </summary>
        /// <returns></returns>
        public List<string> LabelList() => ConvertH.StringToList(Label, '$');

        /// <summary>
        /// 状态枚举
        /// </summary>
        public enum ModeState
        {
            /// <summary>
            /// 未设置
            /// </summary>
            /// <remarks>默认的文章模式，不带有任何模式特性</remarks>
            unset,
            /// <summary>
            /// 隐藏
            /// </summary>
            /// <remarks>文章被隐藏，此状态下的文章不会被展示</remarks>
            hidden,
            /// <summary>
            /// 计划
            /// </summary>
            /// <remarks>表示文章处于计划状态</remarks>
            scheduled,
            /// <summary>
            /// 归档
            /// </summary>
            /// <remarks>表示文章处于归档状态</remarks>
            archived
        }
        /// <summary>
        /// 状态枚举
        /// </summary>
        public enum TypeState
        {
            /// <summary>
            /// 未设置
            /// </summary>
            /// <remarks>默认的文章类型，不带有任何类型特性</remarks>
            unset,
            /// <summary>
            /// 便签
            /// </summary>
            /// <remarks>表示文章以便签形式展示</remarks>
            note,
        }
        /// <summary>
        /// 文章模式
        /// </summary>
        public ModeState Mode
        {
            get
            {
                var str = Convert.ToString(GetFieldByID(metaTable, "Mode"));
                return (ModeState)Enum.Parse(typeof(ModeState), str);
            }
            set => SetFieldByID(metaTable, "Mode", value);
        }
        /// <summary>
        /// 文章类型
        /// </summary>
        public TypeState Type
        {
            get
            {
                var str = Convert.ToString(GetFieldByID(metaTable, "Type"));
                return (TypeState)Enum.Parse(typeof(TypeState), str);
            }
            set => SetFieldByID(metaTable, "Type", value);
        }
        /// <summary>
        /// 归属用户
        /// </summary>
        public string User
        {
            get => Convert.ToString(GetFieldByID(metaTable, "User"));
            set => SetFieldByID(metaTable, "User", value);
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CT
        {
            get => Convert.ToDateTime(GetFieldByID(metaTable, "CT"));
            set => SetFieldByID(metaTable, "CT", value);
        }
        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime LCT
        {
            get => Convert.ToDateTime(GetFieldByUUID(stackTable, "LCT"));
            set => SetFieldByUUID(stackTable, "LCT", value);
        }

        /// <summary>
        /// 访问计数
        /// </summary>
        public uint UVCount
        {
            get => Convert.ToUInt32(GetFieldByID(metaTable, "UVCount"));
            set => SetFieldByID(metaTable, "UVCount", value);
        }
        /// <summary>
        /// 星星计数
        /// </summary>
        public uint StarCount
        {
            get => Convert.ToUInt32(GetFieldByID(metaTable, "StarCount"));
            set => SetFieldByID(metaTable, "StarCount", value);
        }


        /// <summary>
        /// 属性容器
        /// </summary>
        public readonly Hashtable PropertyContainer = new();
    }
    /// <summary>
    /// 文章栈
    /// </summary>
    public class PostStack : IEnumerable
    {
        /// <summary>
        /// 取得迭代器
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new PostStackEnumerator(UuidList.Value.GetEnumerator());
        }
        /// <summary>
        /// 内部迭代器
        /// </summary>
        protected class PostStackEnumerator : IEnumerator
        {
            private readonly List<string>.Enumerator enumerator;

            /// <summary>
            /// 默认构造
            /// </summary>
            /// <param name="enumerator">UuidList的迭代器</param>
            public PostStackEnumerator(List<string>.Enumerator enumerator)
            {
                this.enumerator = enumerator;
            }

            object IEnumerator.Current => new PostRecord(enumerator.Current);//通过迭代器获取的记录均只读
            bool IEnumerator.MoveNext() => enumerator.MoveNext();
            void IEnumerator.Reset() => throw new NotSupportedException();
        }

        private readonly System.Lazy<List<string>> UuidList;
        private readonly string metaTable = CORE.Tables.Meta;
        private readonly string stackTable = CORE.Tables.Stack;
        private readonly string unionView = CORE.ViewsSet.DirtyViews.NegUnion;
        private readonly MySqlManager manager = CORE.MySqlManager;

        /// <summary>
        /// 文章栈ID
        /// </summary>
        public uint ID { get; init; }

        /// <summary>
        /// 得到最大ID
        /// </summary>
        /// <returns></returns>
        internal uint GetMaxID()
        {
            var SQL = $"SELECT MAX(PostID) FROM {metaTable}";
            var result = manager.GetKey(SQL);
            /* 若取不到ID(没有任何文章时)，返回12000作为初始ID */
            return Convert.ToUInt32(result == DBNull.Value ? 12000 : result);
        }
        /// <summary>
        /// 得到最小ID
        /// </summary>
        /// <returns></returns>
        internal uint GetMinID()
        {
            var SQL = $"SELECT MIN(PostID) FROM {metaTable}";
            var result = manager.GetKey(SQL);
            /* 若取不到ID(没有任何文章时)，返回12000作为初始ID */
            return Convert.ToUInt32(result == DBNull.Value ? 12000 : result);
        }

        /// <summary>
        /// 新建模式
        /// </summary>
        public PostStack()
        {
            ID = GetMaxID() + 1;

            UuidList = new(() =>
            {
                var list = new List<string>();

                var SQL = $"SELECT UUID FROM {stackTable} WHERE PostID = ?ID";
                var para = new MySqlParameter("ID", ID);

                var result = manager.GetTable(SQL, para);
                foreach (DataRow Row in result.Rows)
                {
                    list.Add(Row["UUID"].ToString());
                }

                return list;
            });

            manager.DoInConnection(conn =>
            {
                DateTime t = DateTime.Now;

                var SQL =
                $@"INSERT INTO {metaTable}
                       ( PostID, UUID, CT, Mode, Type, User, UVCount, StarCount, ArchiveID) VALUES
                       (?PostID,?UUID,?CT,?Mode,?Type,?User,?UVCount,?StarCount,?ArchiveID);
                       INSERT INTO {stackTable}
                       ( PostID, UUID, LCT, Title, Summary, Content, Label, Cover) VALUES
                       (?PostID,?UUID,?LCT,?Title,?Summary,?Content,?Label,?Cover);";

                MySqlParameter[] paras =
                {
                    new("PostID", ID),
                    new("UUID", MathH.GenerateUUID(MathH.UUIDFormat.N)),

                    new("CT", t),
                    new("LCT", t),

                    new("User", ""),
                    new("Mode", ""),
                    new("Type", ""),
                    new("ArchiveID", 0),

                    new("UVCount", 0),
                    new("StarCount", 0),

                    new("Title", ""),
                    new("Summary", ""),
                    new("Content", ""),

                    new("Label", ""),
                    new("Cover", "")
                };

                return MySqlManager.DoInCommand(conn, cmd =>
                {
                    cmd.CommandText = SQL;
                    cmd.Parameters.AddRange(paras);

                    return MySqlManager.DoInTransaction(cmd, tx =>
                    {
                        if (cmd.ExecuteNonQuery() == 2)
                        {
                            /* meta表和stack表分别添加1行数据 */
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
        /// 编辑模式
        /// </summary>
        /// <param name="ID"></param>
        public PostStack(uint ID)
        {
            this.ID = ID;

            UuidList = new(() =>
            {
                var list = new List<string>();

                var SQL = $"SELECT UUID FROM {stackTable} WHERE PostID = ?ID";
                var para = new MySqlParameter("ID", ID);

                var result = manager.GetTable(SQL, para);
                foreach (DataRow Row in result.Rows)
                {
                    list.Add(Row["UUID"].ToString());
                }

                return list;
            });
        }

        /// <summary>
        /// 栈计数
        /// </summary>
        public int Count
        {
            get { return UuidList.Value.Count; }
        }

        /// <summary>
        /// 取栈顶
        /// </summary>
        public PostRecord Peek
        {
            get
            {
                var peekUUID = UuidList.Value.Last();
                var peek = new PostRecord(peekUUID, true, false);
                return peek;
            }
        }

        /// <summary>
        /// 弹出PostRecord
        /// </summary>
        /// <returns></returns>
        public void Pop()
        {
            var lastestIndex = UuidList.Value.Count - 1;

            if (lastestIndex == 0)
            {
                throw new Exception("试图删除索引位于0处的记录，文章栈中至少保留一条记录");
            }
            else
            {
                Rollback();
                UuidList.Value.RemoveAt(lastestIndex);
            }
        }
        /// <summary>
        /// 压入PostRecord
        /// </summary>
        /// <param name="item"></param>
        public void Push(PostRecord item)
        {
            if (item.ID == ID)
            {
                var SQL =
                    $@"UPDATE {metaTable} SET 
                       UUID=?UUID, Mode=?Mode, Type=?Type, User=?User, UVCount=?UVCount, StarCount=?StarCount, ArchiveID=?ArchiveID 
                       WHERE PostID=?ID;";

                MySqlParameter[] paras =
                {
                    new("ID", item.ID),
                    new("UUID", item.UUID ),

                    new("Mode", item.Mode),
                    new("Type", item.Type),
                    new("User", item.User),
                    new("ArchiveID", item.ArchiveID),

                    new("UVCount", item.UVCount),
                    new("StarCount", item.StarCount),
                };

                manager.DoInConnection(conn =>
                {
                    return MySqlManager.DoInCommand(conn, cmd =>
                    {
                        cmd.CommandText = SQL;
                        cmd.Parameters.AddRange(paras);

                        return MySqlManager.DoInTransaction(cmd, tx =>
                        {
                            if (cmd.ExecuteNonQuery() == 1)
                            {
                                /* meta表修改1行数据 */
                                tx.Commit();
                                UuidList.Value.Add(item.UUID);
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
            else
            {
                throw new Exception("ID不匹配");
            }
        }

        /// <summary>
        /// 注销文章栈
        /// </summary>
        /// <remarks>
        /// 删除所有拷贝和index指向
        /// </remarks>
        /// <returns></returns>
        public bool Dispose()
        {
            return manager.DoInConnection(conn =>
            {
                return MySqlManager.DoInCommand(conn, cmd =>
                {
                    /* int参数无法用于参数化攻击 */
                    cmd.CommandText = $"DELETE FROM {metaTable} WHERE PostID={ID};DELETE FROM {stackTable} WHERE PostID={ID};";

                    return MySqlManager.DoInTransaction(cmd, tx =>
                    {
                        if (cmd.ExecuteNonQuery() >= 2)
                        {
                            /* 指向表只删除1行数据，拷贝表至少删除1行数据 */
                            tx.Commit();
                            UuidList.Value.Clear();
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
        /// 删除文章记录
        /// </summary>
        /// <remarks>
        /// 删除指定拷贝，且该拷贝不能为当前index指向
        /// </remarks>
        /// <param name="UUID">目标文章的UUID</param>
        /// <returns></returns>
        public bool Delete(string UUID)
        {
            if (UuidList.Value.Contains(UUID))
            {
                return manager.DoInConnection(conn =>
                {
                    var SQL = string.Format
                    (
                    "DELETE {1} FROM {0} INNER JOIN {1} ON {0}.PostID={1}.PostID AND {0}.UUID<>{1}.UUID AND {1}.UUID = ?UUID"
                    , metaTable, stackTable
                    );

                    var para = new MySqlParameter("UUID", UUID);

                    return MySqlManager.DoInCommand(conn, cmd =>
                    {
                        cmd.CommandText = SQL;
                        cmd.Parameters.Add(para);

                        return MySqlManager.DoInTransaction(cmd, tx =>
                        {
                            if (cmd.ExecuteNonQuery() == 1)
                            {
                                /* stack表删除一行数据 */
                                tx.Commit();
                                UuidList.Value.Remove(UUID);
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
            else
            {
                throw new Exception("无法将不属于本栈的文章记录从本栈删除");
            }
        }
        /// <summary>
        /// 替换文章记录
        /// </summary>
        /// <remarks>
        /// 将现有index指向删除（顶出），然后将index指向设置为指定文章记录
        /// </remarks>
        /// <param name="UUID">目标记录的UUID</param>
        /// <returns></returns>
        public bool RePeek(string UUID)
        {
            if (UuidList.Value.Contains(UUID))
            {
                return manager.DoInConnection(conn =>
                {
                    /* 此处，即使SQL注入造成了PostID错误，由于第二步参数化查询的作用，UUID也会造成错误无法成功攻击 */
                    var ID = manager.GetKey($"SELECT PostID FROM {stackTable} WHERE UUID = '{UUID}'");

                    //DELETE FROM {stackTable} WHERE UUID = (SELECT UUID FROM {metaTable} WHERE PostID = ?ID);
                    var SQL =
                    $@"UPDATE {stackTable} SET UUID = ?UUID WHERE PostID = ?ID;";

                    MySqlParameter[] paras =
                    {
                        new("ID", ID),
                        new("UUID", UUID)
                    };

                    return MySqlManager.DoInCommand(conn, cmd =>
                    {
                        cmd.CommandText = SQL;
                        cmd.Parameters.AddRange(paras);

                        return MySqlManager.DoInTransaction(cmd, tx =>
                        {
                            if (cmd.ExecuteNonQuery() == 1)
                            {
                                /* meta表修改1行数据 */
                                tx.Commit();

                                //交换Peek
                                var newPeekIndex = UuidList.Value.FindIndex(
                                    (x) =>
                                    {
                                        if (x == UUID)
                                            return true;
                                        else
                                            return false;
                                    });
                                var newPeekValue = UUID;
                                var nowPeekIndex = UuidList.Value.Count - 1;
                                var nowPeekValue = UuidList.Value.Last();

                                UuidList.Value[newPeekIndex] = nowPeekValue;
                                UuidList.Value[nowPeekIndex] = newPeekValue;

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
            else
            {
                throw new Exception("无法将不属于本栈的文章记录替换到本栈");
            }
        }
        /// <summary>
        /// 释放冗余记录
        /// </summary>
        /// <remarks>
        /// 删除非Peek的所有记录
        /// </remarks>
        /// <returns></returns>
        public bool Clean()
        {
            return manager.DoInConnection(conn =>
            {
                return MySqlManager.DoInCommand(conn, cmd =>
                {
                    cmd.CommandText = string.Format
                    (
                    "DELETE {1} FROM {0} INNER JOIN {1} ON {0}.PostID={1}.PostID AND {0}.UUID<>{1}.UUID AND {0}.PostID={2}"
                    , metaTable, stackTable, ID
                    );

                    return MySqlManager.DoInTransaction(cmd, tx =>
                    {
                        if (cmd.ExecuteNonQuery() >= 0)
                        {
                            /* 删除stack表的所有冗余，不存在冗余时影响行数为0 */
                            tx.Commit();
                            UuidList.Value.RemoveRange(1, UuidList.Value.Count - 1);
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
        /// 回滚文章记录
        /// </summary>
        /// <remarks>
        /// 将现有index指向删除（顶出），然后将index指向设置到另一个最近更新的拷贝
        /// </remarks>
        /// <returns></returns>
        private bool Rollback()
        {
            return manager.DoInConnection(conn =>
            {
                return MySqlManager.DoInCommand(conn, cmd =>
                {
                    cmd.CommandText = string.Format
                    (
                    @"DELETE {1} FROM {0} INNER JOIN {1} ON {0}.UUID={1}.UUID AND {0}.PostID={2};
                        UPDATE {0} SET UUID = (SELECT UUID FROM {1} WHERE PostID={2} ORDER BY LCT DESC LIMIT 0,1) WHERE PostID={2};"
                    , metaTable, stackTable, ID
                    );

                    return MySqlManager.DoInTransaction(cmd, tx =>
                    {
                        var affectedRows = cmd.ExecuteNonQuery();
                        if (affectedRows == 2)
                        {
                            /* meta表修改1行数据，stack表删除1行数据 */
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
        /// 取得具有比当前文章栈的指定属性具有更大的值的文章栈
        /// </summary>
        /// <param name="Prop">指定属性</param>
        /// <returns>查询不到返回null</returns>
        public PostStack GT(PostProp Prop)
        {
            var SQL = (Prop == PostProp.PostID) switch /* 对查询PostID有优化 */
            {
                true => $"SELECT PostID FROM `{unionView}` WHERE PostID=( SELECT min(PostID) FROM `{unionView}` WHERE PostID > {ID})",
                false => string.Format
                (
                $"SELECT PostID FROM `{0}` WHERE {1}=( SELECT min({1}) FROM `{0}` WHERE {1} > ( SELECT {1} FROM `{0}` WHERE PostID = {2} ))"
                , unionView, Prop, ID
                )
            };

            var NextID = manager.GetKey(SQL);

            return NextID == null ? null : new PostStack(Convert.ToUInt32(NextID));
        }
        /// <summary>
        /// 取得具有比当前文章栈文章的指定属性具有更大的值的文章栈
        /// </summary>
        /// <remarks>此方法能根据指定正则表达式对某属性进行筛选</remarks>
        /// <param name="Prop">指定属性</param>
        /// <param name="REGEXP">正则表达式</param>
        /// <param name="FilterProp">被正则表达式筛选的属性</param>
        /// <returns>查询不到返回null</returns>
        public PostStack GT(PostProp Prop, string REGEXP, PostProp FilterProp)
        {
            var SQL = (Prop == PostProp.PostID) switch
            {
                true => string.Format
                (
                "SELECT PostID FROM `{0}` WHERE {1}=( SELECT min({1}) FROM `{0}` WHERE PostID > {2} AND {3} REGEXP ?REGEXP )"
                , unionView, Prop, ID, FilterProp
                ),
                false => string.Format
                (
                "SELECT PostID FROM `{0}` WHERE {1}=( SELECT min({1}) FROM `{0}` WHERE {1} > ( SELECT {1} FROM `{0}` WHERE PostID = ?ID ) AND {2} REGEXP ?REGEXP )"
                , unionView, Prop, FilterProp
                )
            };


            var paras = new MySqlParameter[]
            {
                    new("ID", ID),
                    new("REGEXP", REGEXP)
            };
            object NextID = manager.GetKey(SQL, paras);

            return NextID == null ? null : new PostStack(Convert.ToUInt32(NextID));
        }
        /// <summary>
        /// 取得具有比当前文章栈的指定属性具有更小的值的文章栈
        /// </summary>
        /// <param name="Prop">指定属性</param>
        /// <returns>查询不到返回null</returns>
        public PostStack LT(PostProp Prop)
        {
            var SQL = (Prop == PostProp.PostID) switch /* 对查询PostID有优化 */
            {
                true => $"SELECT PostID FROM `{unionView}` WHERE PostID=( SELECT max(PostID) FROM `{unionView}` WHERE PostID < {ID})",
                false => string.Format
                (
                "SELECT PostID FROM `{0}` WHERE {1}=( SELECT max({1}) FROM `{0}` WHERE {1} < ( SELECT {1} FROM `{0}` WHERE PostID = {2} ))"
                , unionView, Prop, ID
                )
            };

            object PrevID = manager.GetKey(SQL);

            return PrevID == null ? null : new PostStack(Convert.ToUInt32(PrevID));
        }
        /// <summary>
        /// 取得具有比当前文章栈的指定属性具有更小的值的文章栈
        /// </summary>
        /// <param name="Prop">指定属性</param>
        /// <param name="REGEXP">正则表达式</param>
        /// <param name="FilterProp">用于被正则表达式筛选的属性</param>
        /// <returns>查询不到返回null</returns>
        public PostStack LT(PostProp Prop, string REGEXP, PostProp FilterProp)
        {
            var SQL = (Prop == PostProp.PostID) switch
            {
                true => string.Format
                (
                "SELECT PostID FROM `{0}` WHERE {1}=( SELECT max({1}) FROM `{0}` WHERE PostID < {2} AND {3} REGEXP ?REGEXP )"
                , unionView, Prop, ID, FilterProp
                ),
                false => string.Format
                (
                "SELECT PostID FROM `{0}` WHERE {1}=( SELECT max({1}) FROM `{0}` WHERE {1} < ( SELECT {1} FROM `{0}` WHERE PostID = ?ID ) AND {2} REGEXP ?REGEXP )"
                , unionView, Prop, FilterProp
                )
            };

            var paras = new MySqlParameter[]
            {
                    new("ID", ID),
                    new("REGEXP", REGEXP)
            };
            object PrevID = manager.GetKey(SQL, paras);

            return PrevID == null ? null : new PostStack(Convert.ToUInt32(PrevID));
        }
    }
    /// <summary>
    /// 文章栈集
    /// </summary>
    public class PostStackSet : IEnumerable
    {
        /// <summary>
        /// 文章栈索引器
        /// </summary>
        /// <param name="ID">文章栈ID</param>
        /// <returns>索引无果返回null</returns>
        public PostStack this[int ID]
        {
            /* 通过反射获取属性 */
            get
            {
                foreach (PostStack el in PostStackList)
                {
                    if (el.ID == ID)
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
            return PostStackList.GetEnumerator();
        }

        /// <summary>
        /// 默认构造
        /// </summary>
        public PostStackSet()
        {
            PostStackList = new();
        }

        private List<PostStack> PostStackList { get; init; }
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
            get { return PostStackList.Count; }
        }
        /// <summary>
        /// 取得数据集中的最后一个评论对象
        /// </summary>
        /// <returns></returns>
        public PostStack Last() => PostStackList.Last();

        /// <summary>
        /// 添加文章栈
        /// </summary>
        /// <param name="postStack">文章栈对象</param>
        public void Add(PostStack postStack) => PostStackList.Add(postStack);
        /// <summary>
        /// 添加多个文章栈
        /// </summary>
        /// <param name="postStacks">多个文章栈对象</param>
        public void AddRange(IEnumerable<PostStack> postStacks) => PostStackList.AddRange(postStacks);
        /// <summary>
        /// 清空数据集
        /// </summary>
        public void Clear()
        {
            PostStackList.Clear();
        }

        /// <summary>
        /// 将function应用到本数据集的所有元素
        /// </summary>
        /// <remarks>
        /// 函数式API
        /// </remarks>
        /// <param name="function"></param>
        /// <returns>返回操作后的数据集</returns>
        public PostStackSet Map(Action<PostStack> function)
        {
            PostStackList.ForEach(function);
            return this;
        }
        /// <summary>
        /// 过滤得到满足predicate的PostStackSet
        /// </summary>
        /// <remarks>
        /// 函数式API
        /// </remarks>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public PostStackSet Filter(Predicate<PostStack> predicate)
        {
            var list = PostStackList.FindAll(predicate);
            return new PostStackSet() { PostStackList = list };
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
        private int CreateCounter(int Days) => (from el in PostStackList where el.Peek.CT > DateTime.Now.AddDays(Days) select el).Count();

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
        private int UpdateCounter(int Days) => (from el in PostStackList where el.Peek.CT > DateTime.Now.AddDays(Days) select el).Count();
    }


    /*/// <summary>
    /// 文章树
    /// </summary>
    public class PostTree
    {
        private List<string> list = new();
        public int ID = -1;

        public PostTree(int ID)
        {
            this.ID = ID;
        }

        public List<string> GetNodes()
        {
            return null;
        }
    }*/


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
