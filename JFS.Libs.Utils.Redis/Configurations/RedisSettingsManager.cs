using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JFS.Libs.Utils.Redis.Configurations
{
    public sealed class RedisSettingsManager
    {
        #region "Fields"
        static RedisSettingsConfiguration _config = System.Configuration.ConfigurationManager.GetSection("redisSettings") as RedisSettingsConfiguration;

        internal static readonly int CNINT_DEFAULT_PORT = 6379;
        internal static readonly int CNINT_DEFAULT_CONNECTTIMEOUT = 5000;
        #endregion

        #region "Properties"
        public static IEnumerable<RedisServer> Servers
        {
            get
            {
                var lst = new List<RedisServer>();

                if (_config.Servers != null)
                {
                    foreach (RedisSettingsElement item in _config.Servers)
                    {
                        if (item.Enabled)
                        {
                            lst.Add(new RedisServer(item));
                        }
                    }
                }

                return lst;
            }
        }

        public static string ClientName { get { return _config.ClientName; } }

        public static string Password { get { return _config.Password; } }

        public static bool AllowAdmin { get { return _config.AllowAdmin; } }

        public static bool AuthRequired { get { return !string.IsNullOrWhiteSpace(_config.Password); } }

        public static int ConnectTimeout { get { return _config.ConnectTimeout; } }
        #endregion
    }

    public sealed class RedisServer
    {
        #region "Constructs"
        internal RedisServer(RedisSettingsElement element)
        {
            this.Server = element.Server;
            this.Port = element.Port;
            this.Enabled = element.Enabled;
        }
        #endregion

        #region "Properties"
        public string Server { get; set; }

        public uint Port { get; set; }

        public bool Enabled { get; set; }
        #endregion
    }

    #region "Configuration Settings"
    class RedisSettingsConfiguration : System.Configuration.ConfigurationSection
    {
        #region "Properties"
        [ConfigurationProperty("", DefaultValue = null, IsDefaultCollection = true)]
        public RedisSettingsElements Servers { get { return base[""] as RedisSettingsElements; } set { base[""] = value; } }

        [ConfigurationProperty("clientName")]
        public string ClientName { get { return Convert.ToString(base["clientName"]); } set { base["clientName"] = value; } }

        [ConfigurationProperty("password")]
        public string Password { get { return Convert.ToString(base["password"]); } set { base["password"] = value; } }

        [ConfigurationProperty("allowAdmin", DefaultValue = true)]
        public bool AllowAdmin { get { return !string.IsNullOrWhiteSpace(base["allowAdmin"].ToString()) ? Convert.ToBoolean(base["allowAdmin"]) : true; } set { base["allowAdmin"] = value; } }

        [ConfigurationProperty("connectTimeout", DefaultValue = 1000)]
        public int ConnectTimeout { get { return !string.IsNullOrWhiteSpace(base["connectTimeout"].ToString()) ? Convert.ToInt32(base["connectTimeout"]) : RedisSettingsManager.CNINT_DEFAULT_CONNECTTIMEOUT; } set { base["connectTimeout"] = value; } }
        #endregion
    }

    [System.Configuration.ConfigurationCollection(typeof(RedisSettingsElement), CollectionType = System.Configuration.ConfigurationElementCollectionType.AddRemoveClearMap, AddItemName = "add")]
    class RedisSettingsElements : System.Configuration.ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new RedisSettingsElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as RedisSettingsElement).Server;
        }
    }

    class RedisSettingsElement : System.Configuration.ConfigurationElement
    {
        #region "Properties"
        [ConfigurationProperty("server", IsKey = true, IsRequired = true)]
        public string Server { get { return Convert.ToString(base["server"]); } set { base["server"] = value; } }

        [ConfigurationProperty("port", DefaultValue = (uint)6379)]
        public uint Port { get { return !string.IsNullOrWhiteSpace(base["port"].ToString()) ? Convert.ToUInt32(base["port"]) : (uint)RedisSettingsManager.CNINT_DEFAULT_PORT; } set { base["port"] = value; } }

        [ConfigurationProperty("enabled", DefaultValue = true)]
        public bool Enabled { get { return !string.IsNullOrWhiteSpace(base["enabled"].ToString()) ? Convert.ToBoolean(base["enabled"]) : true; } set { base["enabled"] = value; } }
        #endregion
    }
    #endregion
}
