namespace WaterLibrary.Utils
{
    using System;


    /// <summary>
    /// 惰性求值包装器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Lazy<T>
    {
        private Func<T> init;
        private Func<T, T> set;

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
}
