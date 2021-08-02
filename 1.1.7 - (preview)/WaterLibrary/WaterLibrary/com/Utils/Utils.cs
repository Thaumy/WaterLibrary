namespace WaterLibrary.Utils
{
    using System;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;


    /// <summary>
    /// 惰性求值包装器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Lazy<T>
    {
        private readonly Func<T> init;
        private readonly Func<T, T> set;

        /// <summary>
        /// 默认构造
        /// </summary>
        /// <param name="init">初始化函数</param>
        /// <param name="set">set访问器函数</param>
        public Lazy(Func<T> init, Func<T, T> set)
        {
            this.init = init;
            this.set = set;
        }

        private T container = default;

        /// <summary>
        /// 容器值
        /// </summary>
        public T Value
        {
            get
            {
                if (Equals(container, default(T)))
                {
                    container = init.Invoke();
                    return container;
                }
                else
                {
                    return container;
                }
            }
            set
            {
                container = set(value);
            }
        }
    }

    /// <summary>
    /// 可被序列化为Json的
    /// </summary>
    public interface IJsonSerializable
    {
        /// <summary>
        /// 将当前对象序列化到JSON
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject
                (this, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
        }
    }

    /// <summary>
    /// 可索引的
    /// </summary>
    public interface IIndexable<K, V>
    {
        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="key">键</param>
        public V this[K key]
        {
            /* 通过反射获取属性 */
            get
            {
                var FieldName = Convert.ToString(key);

                var ThisType = GetType();
                var FieldInfo = ThisType.GetProperty(FieldName);
                var FieldValue = (V)Convert.ChangeType(FieldInfo.GetValue(this), typeof(V));

                return FieldValue;
            }
            /* 通过反射设置属性 */
            set
            {
                var FieldName = Convert.ToString(key);

                var ThisType = GetType();
                var FieldInfo = ThisType.GetProperty(FieldName);
                var FieldType = FieldInfo.GetValue(this).GetType();
                var FieldValue = Convert.ChangeType(value, FieldType);

                ThisType.GetProperty(FieldName).SetValue(this, FieldValue);
            }
        }
    }
}
