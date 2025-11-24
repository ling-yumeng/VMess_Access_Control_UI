using System.Text.Json;
using System.Text.Json.Nodes;

namespace WebApplication1.Classes
{
    public class CreateV2RayConfig
    {
        private string[] uuids;
        public CreateV2RayConfig(string[] uuids)
        {
            this.uuids = uuids;
        }

        private class v2temp
        {
            public class Log_t
            {
                public string loglevel { get; set; }
            }

            public class Client_t
            {
                public string id { get; set; }
                public int level { get; set; } = 0;
                public int alterId { get; set; } = 64;
            }

            public class Ws_settings_t
            {
                public string path { get; set; }
            }

            public class Stream_settings_t
            {
                public string network { get; set; } = "ws";
                // 改为属性
                public Ws_settings_t wsSettings { get; set; } = new Ws_settings_t
                {
                    path = "/stream"
                };
            }

            public class Settings_t
            {
                public Client_t[] clients { get; set; }
            }

            public class Inbound_t
            {
                public int port { get; set; }
                public string listen { get; set; }
                public string protocol { get; set; }
                public Settings_t settings { get; set; }
                public Stream_settings_t streamSettings { get; set; }
            }

            public class Outbound_t
            {
                // 改为属性
                public string protocol { get; set; }
                // 改为属性
                public object settings { get; set; } = new { };
            }

            // 改为公共属性
            public Log_t log { get; set; } = new Log_t
            {
                loglevel = "warning"
            };

            // 改为公共属性
            public Inbound_t inbound { get; set; } = new Inbound_t
            {
                port = 10086,
                listen = "0.0.0.0",
                protocol = "vmess",
                settings = new Settings_t
                {
                    clients = new Client_t[0] // 初始化为空数组
                },
                streamSettings = new Stream_settings_t()
            };

            // 改为公共属性
            public Outbound_t outbound { get; set; } = new Outbound_t
            {
                protocol = "freedom",
                settings = new { }
            };

            public v2temp(Client_t[] clients)
            {
                inbound.settings.clients = clients;
            }
        }

        public string export()
        {
            List<v2temp.Client_t> clients = new List<v2temp.Client_t>();
            foreach (var i in this.uuids)
            {
                clients.Add(new v2temp.Client_t
                {
                    id = i,
                    level = 0,
                    alterId = 64
                });
            }
            v2temp t = new v2temp(clients.ToArray());

            // 添加序列化选项以确保正确序列化
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase // 可选：使用驼峰命名
            };

            string JSONConfig = JsonSerializer.Serialize(t, options);
            return JSONConfig;
        }
    }
}