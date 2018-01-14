using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JFS.Libs.Utils.Redis.Extends
{
    public static class Extends
    {
        public static StackExchange.Redis.RedisKey[] ConvertToRedisKeys(this IEnumerable<string> keys)
        {
            if (keys != null && keys.Any())
            {
                return keys.Select(p => (StackExchange.Redis.RedisKey)p).ToArray();
            }

            return null;
        }
    }
}
