using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JFS.Libs.Utils.Redis.Interfaces
{
    public interface IRedisUtils
    {
        #region "Methods: Utils"
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        void Delete(string key);

        void Delete(IEnumerable<string> keys);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expiredIn"></param>
        void Expire(string key, TimeSpan? expiredIn = null);

        void Expire(string key, DateTime? expiredAt = null);

        void Expire(IEnumerable<string> keys, TimeSpan? expiredIn = null);

        void Expire(IEnumerable<string> keys, DateTime? expiredAt = null);

        void Persist(string key);

        void Persist(IEnumerable<string> keys);
        #endregion

        #region "Methods: Get / Set"
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string Get(string key);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T Get<T>(string key) where T : struct;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T GetObject<T>(string key) where T : class;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiredIn"></param>
        /// <returns></returns>
        string Set(string key, string value, TimeSpan? expiredIn = null);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiredIn"></param>
        /// <returns></returns>
        T Set<T>(string key, T value, TimeSpan? expiredIn = null) where T : struct;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiredIn"></param>
        /// <returns></returns>
        T SetObject<T>(string key, T value, TimeSpan? expiredIn = null) where T : class; 
        #endregion
    }
}
