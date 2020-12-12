using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;//GDI+命名空间

namespace StdLib
{
    /// <summary>
    /// 用于参与连接数据库的文本结构
    /// </summary>
    public struct connStr
    {
        #region 私有属性

        private string DataSource;
        private string Port;
        private string UserName;
        private string Password;

        #endregion

        #region 属性访问器

        /// <summary>
        /// 被连接的数据源
        /// </summary>
        public string dataSource
        {
            get { return DataSource; }
            set { DataSource = value; }
        }
        /// <summary>
        /// 被连接的端口
        /// </summary>
        public string port
        {
            get { return Port; }
            set { Port = value; }
        }
        /// <summary>
        /// 连接所使用的用户名
        /// </summary>
        public string userName
        {
            get { return UserName; }
            set { UserName = value; }
        }
        /// <summary>
        /// 连接所使用的密码
        /// </summary>
        public string password
        {
            get { return Password; }
            set { Password = value; }
        }

        #endregion
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
    /// 用于定位查询操作对象的文本结构
    /// </summary>
    public struct selectStr
    {
        #region 私有属性

        private string DataBaseName;
        private string TableName;
        private string PrimaryKeyName;
        private string ColumnName;

        #endregion

        #region 属性访问器

        /// <summary>
        /// 被操作表所在的数据库
        /// </summary>
        public string dataBaseName
        {
            get { return DataBaseName; }
            set { DataBaseName = value; }
        }
        /// <summary>
        /// 被操作表
        /// </summary>
        public string tableName
        {
            get { return TableName; }
            set { TableName = value; }
        }
        /// <summary>
        /// 作为索引的主键
        /// </summary>
        public string primaryKeyName
        {
            get { return PrimaryKeyName; }
            set { PrimaryKeyName = value; }
        }
        /// <summary>
        /// 参与更改任务的列         
        /// </summary>
        public string columnName
        {
            get { return ColumnName; }
            set { ColumnName = value; }
        }

        #endregion
    }

    /// <summary>
    /// 用于存储文章概要信息的结构体
    /// </summary>
    public struct postData
    {
        #region 私有属性
        private int pst_ID;
        private string pst_title;
        private string pst_summary;
        private string pst_content;
        private string cvr_link;
        #endregion

        #region 属性访问器
        /// <summary>
        /// 文章ID
        /// </summary>
        public int post_ID
        {
            get { return pst_ID; }
            set { pst_ID = value; }
        }
        /// <summary>
        /// 文章标题
        /// </summary>
        public string post_title
        {
            get { return pst_title; }
            set { pst_title = value; }
        }
        /// <summary>
        /// 文章概要
        /// </summary>
        public string post_summary
        {
            get { return pst_summary; }
            set { pst_summary = value; }
        }
        /// <summary>
        /// 文章内容
        /// </summary>
        public string post_content
        {
            get { return pst_content; }
            set { pst_content = value; }
        }
        /// <summary>
        /// 封面图片url链接
        /// </summary>
        public string cover_link
        {
            get { return cvr_link; }
            set { cvr_link = value; }
        }
        #endregion
    }

    /// <summary>
    /// 用于为通过联网Json获取StdLib信息的类
    /// </summary>
    public class JlinfObject : FrameHandler.IStdLibFrame
    {
        //主要信息
        /// <summary>
        /// 版本号
        /// </summary>
        public int projectVer
        {
            get; set;
        }

        /// <summary>
        /// 版本名字对象
        /// </summary>
        public string projectMoniker
        {
            get; set;
        }

        /// <summary>
        /// 版本类型
        /// </summary>
        public string editionType
        {
            get; set;
        }

        /// <summary>
        /// 步进
        /// </summary>
        public string stepping
        {
            get; set;
        }

        /// <summary>
        /// 类库的目标框架
        /// </summary>
        public string targetFramework
        {
            get; set;
        }

        /// <summary>
        /// 类库的目标框架名字对象
        /// </summary>
        public string targetFrameworkMoniker
        {
            get; set;
        }

        /// <summary>
        /// 针对最近一次发行版的全局兼容性
        /// </summary>
        public bool compat
        {
            get; set;
        }

        /// <summary>
        /// 适用平台
        /// </summary>
        public string platform
        {
            get; set;
        }

        //次要信息
        /// <summary>
        /// 架构名
        /// </summary>
        public string architecture
        {
            get; set;
        }

        /// <summary>
        /// 开发代号
        /// </summary>
        public string developmentCode
        {
            get; set;
        }

        /// <summary>
        /// 版本概要
        /// </summary>
        public string summary
        {
            get; set;
        }

        /// <summary>
        /// 是否为最新pub版本
        /// </summary>
        public bool isNewVer
        {
            get; set;
        }

        /// <summary>
        /// 最新pub版本下载URL
        /// </summary>
        public string newVerURL
        {
            get; set;
        }
    }
}
