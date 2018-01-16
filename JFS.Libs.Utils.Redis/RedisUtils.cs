using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JFS.Libs.Utils.Redis.Extends;
using JFS.Libs.Utils.Redis.Interfaces;

namespace JFS.Libs.Utils.Redis
{
    public sealed class RedisUtils : Interfaces.IRedisUtils
    {
        #region "Fields"
        static Lazy<Interfaces.IRedisUtils> _inst = new Lazy<IRedisUtils>(() => { return new RedisUtils(); });
        static object _lock = new object();
        #endregion

        #region "Fields"
        StackExchange.Redis.ConnectionMultiplexer _conn = null;
        #endregion

        #region "Constructs"
        private RedisUtils()
        {
            _conn = StackExchange.Redis.ConnectionMultiplexer.Connect(BuildRedisConfiguration());
        }
        #endregion

        #region "Properties"
        public static Interfaces.IRedisUtils Instance { get { return _inst.Value; } } 
        
        StackExchange.Redis.IDatabase RedisDB { get { return _conn.GetDatabase(); } }
        #endregion

        #region "Interfaces: Interfaces.IRedisUtils"

        #region "Methods: Utils"
        void IRedisUtils.Delete(string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                this.RedisDB.KeyDelete(key);
            }
        }

        void IRedisUtils.Delete(IEnumerable<string> keys)
        {
            if (keys != null && keys.Any())
            {
                this.RedisDB.KeyDelete(keys.ConvertToRedisKeys());
            }
        }

        void IRedisUtils.Expire(string key, TimeSpan? expiredIn)
        {
            if (!string.IsNullOrWhiteSpace(key) && this.RedisDB.KeyExists(key))
            {
                this.RedisDB.KeyExpire(key, expiredIn);
            }
        }

        void IRedisUtils.Expire(string key, DateTime? expiredAt)
        {
            if (!string.IsNullOrWhiteSpace(key) && this.RedisDB.KeyExists(key))
            {
                this.RedisDB.KeyExpire(key, expiredAt);
            }
        }

        void IRedisUtils.Expire(IEnumerable<string> keys, TimeSpan? expiredIn)
        {
            if (keys != null && keys.Any())
            {
                var db = this.RedisDB;

                foreach(var key in keys)
                {
                    db.KeyExpire(key, expiredIn);
                }
            }
        }

        void IRedisUtils.Expire(IEnumerable<string> keys, DateTime? expiredAt)
        {
            if (keys != null && keys.Any())
            {
                foreach (var key in keys)
                {
                    (this as IRedisUtils).Expire(key, expiredAt);
                }
            }
        }

        void IRedisUtils.Persist(string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                this.RedisDB.KeyPersist(key);
            }
        }

        void IRedisUtils.Persist(IEnumerable<string> keys)
        {
            if (keys != null && keys.Any())
            {
                foreach(var key in keys)
                {
                    (this as IRedisUtils).Persist(key);
                }
            }
        }
        #endregion

        #region "Methods: Get / Set"
        string IRedisUtils.Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return default(string);
            }

            var value = this.RedisDB.StringGet(key);

            if (value != default(StackExchange.Redis.RedisValue) || value.HasValue)
            {
                return value.ToString();
            }

            return default(string);
        }

        T IRedisUtils.Get<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return default(T);
            }

            var value = this.RedisDB.StringGet(key);

            if (value != default(StackExchange.Redis.RedisValue) || value.HasValue)
            {
                var t = typeof(T);

                switch (t.FullName.ToLower())
                {
                    case "system.float":
                        {
                            return (T)Convert.ChangeType(Convert.ToSingle(value), typeof(T));
                        }
                    case "system.double":
                        {
                            return (T)Convert.ChangeType(Convert.ToDouble(value), typeof(T));
                        }
                    case "system.decimal":
                        {
                            return (T)Convert.ChangeType(Convert.ToDecimal(value), typeof(T));
                        }                                            
                    case "system.guid":
                        {
                            return (T)Convert.ChangeType(Guid.Parse(value.ToString()), typeof(T));
                        }
                    default:
                        {
                            return (T)Convert.ChangeType(value, typeof(T));
                        }
                }                                
            }

            return default(T);
        }

        T IRedisUtils.GetObject<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return default(T);
            }

            var value = this.RedisDB.StringGet(key);

            if (value != default(StackExchange.Redis.RedisValue) || value.HasValue)
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value.ToString());
            }

            return default(T);
        }

        string IRedisUtils.Set(string key, string value, TimeSpan? expiredIn)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return value;
            }

            this.RedisDB.StringSet(key, value, expiredIn);

            return value;
        }

        T IRedisUtils.Set<T>(string key, T value, TimeSpan? expiredIn)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                //this.RedisDB.StringSet(key, (StackExchange.Redis.RedisValue)value, expiredIn);
            }
            
            return value;
        }

        T IRedisUtils.SetObject<T>(string key, T value, TimeSpan? expiredIn)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return value;
            }

            this.RedisDB.StringSet(key, value != default(T) ? Newtonsoft.Json.JsonConvert.SerializeObject(value) : string.Empty, expiredIn);

            return value;
        }
        #endregion

        #region "Methods: SGet / SSet"
        string SSet(string key, string value)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                this.RedisDB.SetAdd(key, value);
            }

            return value;
        }

        T SSet<T>(string key, T value, TimeSpan? expiredIn)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                //this.RedisDB.SetAdd(key, value, expiredIn);
            }

            return value;
        }
        #endregion

        #region "Methods: Pub / Sub"
        long IRedisUtils.Pub(string channel, string msg)
        {
            if (string.IsNullOrWhiteSpace(channel) || string.IsNullOrWhiteSpace(msg))
            {
                return 0;
            }

            return this.RedisDB.Publish(channel, msg);
        }

        long IRedisUtils.Pub<T>(string channel, T msg)
        {
            if (string.IsNullOrWhiteSpace(channel))
            {
                return 0;
            }

            return this.RedisDB.Publish(channel, msg.ToString());
        }

        long IRedisUtils.PubObject<T>(string channel, T msg)
        {
            if (string.IsNullOrWhiteSpace(channel) || msg == null)
            {
                return 0;
            }

            return this.RedisDB.Publish(channel, Newtonsoft.Json.JsonConvert.SerializeObject(msg));
        }

        void IRedisUtils.Sub(string channel)
        {           
            throw new NotImplementedException();
        } 
        #endregion

        #endregion

        #region "Common Utils"
        static StackExchange.Redis.ConfigurationOptions BuildRedisConfiguration()
        {
            var options = new StackExchange.Redis.ConfigurationOptions();

            foreach (var server in Configurations.RedisSettingsManager.Servers)
            {
                options.EndPoints.Add(server.Server, (int)server.Port);
            }

            options.ClientName = Configurations.RedisSettingsManager.ClientName;
            options.AllowAdmin = Configurations.RedisSettingsManager.AllowAdmin;
            options.ConnectTimeout = Configurations.RedisSettingsManager.ConnectTimeout;
            options.AbortOnConnectFail = false;
            options.ReconnectRetryPolicy = new StackExchange.Redis.LinearRetry(Configurations.RedisSettingsManager.ConnectTimeout);

            if (Configurations.RedisSettingsManager.AuthRequired)
            {
                options.Password = Configurations.RedisSettingsManager.Password;
            }

            return options;
        }        
        #endregion
    }
}
