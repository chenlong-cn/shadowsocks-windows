using Newtonsoft.Json.Linq;

using Shadowsocks.Common.Model;
using Shadowsocks.Model;
using Shadowsocks.Util;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shadowsocks.Controller.Service
{
    public class OnlineConfigResolver : IService
    {
        public async Task<List<Server>> GetOnline(string url)
        {
            var server_json = await Utils.HttpClient.GetStringAsync(url);
            var servers = server_json.GetServers();
            foreach (var server in servers)
                server.group = url;

            return servers.ToList();
        }
    }

    internal static class OnlineConfigResolverEx
    {
        private static readonly string[] _basic_format = new[] { "server", "server_port", "password", "method" };

        private static readonly IEnumerable<Server> _empty_servers = Array.Empty<Server>();

        internal static IEnumerable<Server> GetServers(this string json) =>
            JToken.Parse(json).SearchJToken().AsEnumerable();

        private static IEnumerable<Server> SearchJArray(JArray array) =>
            array == null ? _empty_servers : array.SelectMany(SearchJToken).ToList();

        private static IEnumerable<Server> SearchJObject(JObject obj)
        {
            if (obj == null)
                return _empty_servers;

            if (_basic_format.All(field => obj.ContainsKey(field)))
                return new[] { obj.ToObject<Server>() };

            var servers = new List<Server>();
            foreach (var kv in obj)
            {
                var token = kv.Value;
                servers.AddRange(SearchJToken(token));
            }
            return servers;
        }

        private static IEnumerable<Server> SearchJToken(this JToken token) =>
            token.Type switch
            {
                JTokenType.Object => SearchJObject(token as JObject),
                JTokenType.Array => SearchJArray(token as JArray),
                _ => Array.Empty<Server>(),
            };
    }
}
