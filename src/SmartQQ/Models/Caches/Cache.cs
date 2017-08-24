using System;
using System.Collections.Generic;
using System.Threading;

namespace SmartQQ.Models
{
    /// <summary>
    /// 
    /// </summary>
    internal abstract class Cache
    {
        protected readonly Timer Timer;
        protected bool IsValid;

        /// <summary>
        ///     初始化一个缓存对象。
        /// </summary>
        /// <param name="timeout">表示缓存的超时时间。</param>
        protected Cache(TimeSpan timeout)
        {
            Timeout = timeout;
            Timer = new Timer(_ => Clear(), null, Timeout, System.Threading.Timeout.InfiniteTimeSpan);
        }

        public TimeSpan Timeout { get; set; }

        protected object Value { get; set; }

        /// <summary>
        ///     尝试取得缓存的值。
        /// </summary>
        /// <param name="target">值的赋值目标。</param>
        /// <returns>值是否有效。</returns>
        public bool TryGetValue<T>(out T target)
        {
            target = (T)Value;
            return IsValid;
        }

        /// <summary>
        ///     设置缓存的值并重置过期计时器。
        /// </summary>
        /// <param name="target">值</param>
        public void SetValue(object target)
        {
            Value = target;
            IsValid = true;
            Timer.Change(Timeout, System.Threading.Timeout.InfiniteTimeSpan);
        }

        public void Clear()
        {
            IsValid = false;
            Value = null;
        }
    }


    /// <summary>
    ///     缓存数据。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class Cache<T> : Cache where T : class
    {
        /// <summary>
        ///     初始化一个缓存对象。
        /// </summary>
        /// <param name="timeout">表示缓存的超时时间。</param>
        public Cache(TimeSpan timeout) : base(timeout)
        {
        }

        /// <summary>
        ///     尝试取得缓存的值。
        /// </summary>
        /// <param name="target">值的赋值目标。</param>
        /// <returns>值是否有效。</returns>
        public bool TryGetValue(out T target)
        {
            target = Value as T;
            return IsValid;
        }

        /// <summary>
        ///     设置缓存的值并重置过期计时器。
        /// </summary>
        /// <param name="target">值</param>
        public void SetValue(T target)
        {
            Value = target;
            IsValid = true;
            Timer.Change(Timeout, System.Threading.Timeout.InfiniteTimeSpan);
        }
    }


    /// <summary>
    ///     放一堆不同类型的东西的缓存的字典。
    /// </summary>
    internal class CacheDepot
    {
        private readonly Dictionary<string, Cache> _dic = new Dictionary<string, Cache>();

        public CacheDepot(TimeSpan timeout)
        {
            Timeout = timeout;
        }

        public TimeSpan Timeout { get; set; }

        public Cache GetCache<T>() where T : class
        {
            if (!_dic.ContainsKey(typeof(T).FullName))
                _dic.Add(typeof(T).FullName, new Cache<T>(Timeout));
            return _dic[typeof(T).FullName];
        }

        public void Clear()
        {
            _dic.Clear();
        }
    }


    /// <summary>
    ///     缓存词典（会定时清空内容）。
    /// </summary>
    /// <typeparam name="TKey">键的类型。</typeparam>
    /// <typeparam name="TValue">值的类型。</typeparam>
    internal class CacheDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly Timer _timer;

        private TimeSpan _timeout;

        public CacheDictionary(TimeSpan timeout)
        {
            _timeout = timeout;
            _timer = new Timer(_ => Clear(), null, timeout, timeout);
        }

        public TimeSpan Timeout
        {
            get { return _timeout; }
            set
            {
                _timeout = value;
                _timer.Change(value, value);
            }
        }
    }
}
