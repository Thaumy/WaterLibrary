namespace WaterLibrary.Utils
{
    using System.IO;
    using System.Reflection;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using Markdig;
    using YamlDotNet.Serialization;


    /// <summary>
    /// 转换管理器
    /// </summary>
    public class ConvertH
    {
        /// <summary>
        /// 私有构造
        /// </summary>
        private ConvertH() { }

        /// <summary>
        /// 将字符串按照分隔符字符切割
        /// </summary>
        /// <param name="str">待切割字符串</param>
        /// <param name="Delimiter">分隔符</param>
        /// <returns>返回切割结果集</returns>
        public static List<string> StringToList(string str, char Delimiter)
        {
            List<string> StringList = new List<string>();
            foreach (string el in str.Split(Delimiter))
            {
                StringList.Add(el);
            }
            return StringList;
        }
        /// <summary>
        /// 将可迭代对象的元素值按照分隔符合并为一个字符串
        /// </summary>
        /// <param name="List">可遍历对象，其中的元素需能转换为字符串</param>
        /// <param name="Delimiter">分隔符</param>
        /// <returns></returns>
        public static string ListToString(dynamic List, char Delimiter)
        {
            string Result = "";
            foreach (dynamic temp in List)
            {
                Result += temp.ToString() + Delimiter;
            }
            return Result[0..^1];
        }
        /// <summary>
        /// 将可迭代对象的元素值按照分隔符合并为一个字符串
        /// </summary>
        /// <param name="List">可遍历对象，其中的元素需能通过指定属性获取值</param>
        /// <param name="PropertyName">属性名</param>
        /// <param name="Delimiter">分隔符</param>
        /// <returns></returns>
        public static string ListToString(dynamic List, string PropertyName, char Delimiter)
        {
            string Result = "";
            PropertyInfo info = List[0].GetType().GetPostProperty(PropertyName);
            foreach (dynamic temp in List)
            {
                Result += info.GetValue(temp) + Delimiter;
            }
            return Result[0..^1];
        }
        /// <summary>
        /// 将Yaml字符串转为Json字符串
        /// </summary>
        /// <param name="yaml">Yaml字符串</param>
        /// <returns></returns>
        public static string YamlToJson(string yaml)
        {
            var input = new StringReader(yaml);
            var deserializer = new DeserializerBuilder().Build();
            var deserialized = deserializer.Deserialize(input);

            var serializer = new SerializerBuilder()
                .JsonCompatible()
                .Build();

            var json = serializer.Serialize(deserialized);

            return json;
        }

        /// <summary>
        /// Html过滤器
        /// </summary>
        /// <param name="HtmlText">Html文本</param>
        /// <returns></returns>
        public static string HtmlFilter(string HtmlText)
        {
            if (string.IsNullOrEmpty(HtmlText))
            {
                return "";
            }

            HtmlText = Regex.Replace(HtmlText, @"<script[^>]*>(.|\n)*?</script>", "");//脚本标签
            HtmlText = Regex.Replace(HtmlText, @"<style>(.|\n)*</style>", "");//样式标签
            HtmlText = Regex.Replace(HtmlText, @"<([^>]|\n)+>", "");//其他标签
            HtmlText = Regex.Replace(HtmlText, @"^\s*\n", "");//空行去除（不考虑最后一行，后续逻辑会将其去除）
            HtmlText = Regex.Replace(HtmlText, @"&#*\w+;", " ");//将转义替换为空格
            HtmlText = Regex.Replace(HtmlText, @"[\s\n]+", " ");//多空白合并
            HtmlText = Regex.Replace(HtmlText, @"^ | $", "");//首尾去空格 

            return HtmlText.Trim();
        }
        /// <summary>
        /// Markdown转Html
        /// </summary>
        /// <param name="MarkdownText">Markdown文本</param>
        /// <returns></returns>
        public static string MarkdownToHtml(string MarkdownText)
        {
            var builder = new MarkdownPipelineBuilder();

            builder.Extensions.Add(new Markdig.Extensions.Tables.PipeTableExtension());//表格
            builder.Extensions.Add(new Markdig.Extensions.EmphasisExtras.EmphasisExtraExtension());//额外的强调
            builder.Extensions.Add(new Markdig.Extensions.Mathematics.MathExtension());//LaTeX
            builder.Extensions.Add(new Markdig.Extensions.TaskLists.TaskListExtension());//任务列表

            var pipeline = builder.Build();

            return Markdown.ToHtml(MarkdownText, pipeline);
        }
    }
}
