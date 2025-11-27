using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication1.Pages
{
    [IgnoreAntiforgeryToken]
    public class IndexModel : PageModel
    {
        public bool login_stat { get; set; } = false;
        public string username { get; set; } = String.Empty;

        [BindProperty(SupportsGet = true)]
        public int token { get; set;  }
        [BindProperty(SupportsGet = true)]
        public bool token_valid { get; set; } = false;
        public string uuid { get; set; } = String.Empty;
        public int uid { get; set; }
        public void OnGet()
        {
            if(token_valid)
            {
                Classes.Token tk = new Classes.Token("token_db.dat");
                tk.read();
                if (tk.valid(token))
                {
                    login_stat = true;
                    Classes.DataBase user_db = new Classes.DataBase("user_db.dat");
                    user_db.read();
                    uid = tk.getUid(token);
                    username = user_db.usernameof(tk.getUid(token));
                }
            }
        }
        public class PostConfigData
        {
            public int token { get; set; }
        }
        public IActionResult OnPostConfig([FromBody] PostConfigData d)
        {
            Classes.Token tk = new Classes.Token("token_db.dat");
            tk.read();
            if(!tk.valid(d.token))
                return NotFound();
            int uid = tk.getUid(d.token);
            Classes.VmessId vmid = new Classes.VmessId("vmessid_db.dat");
            vmid.read();
            string vmess_id = vmid.getUuid(uid);
            Classes.CreateClashConf clashconf = new Classes.CreateClashConf("template.yaml", vmess_id);
            string config_text = clashconf.generateConf();
            var vmess_generater = new Classes.CreateV2RayConfig(vmid.getAllUuids());
            string v2conf = vmess_generater.export();
            string v2conf_old;
            if (System.IO.File.Exists("v2ray/config.json"))
            {
                v2conf_old = System.IO.File.ReadAllText("v2ray/config.json");
            }
            else
            {
                v2conf_old = "";
            }
            if (v2conf != v2conf_old)
            {
                System.IO.File.WriteAllText("v2ray/config.json", v2conf);
		        System.Diagnostics.Process process = new System.Diagnostics.Process();
		        process.StartInfo.FileName = "systemctl";
		        process.StartInfo.Arguments = "restart v2ray";
		        process.StartInfo.UseShellExecute = false;
		        process.StartInfo.RedirectStandardOutput = false;
		        process.StartInfo.RedirectStandardError = false;
		        process.StartInfo.CreateNoWindow = true;
                try
                {
                    process.Start();
                }
                catch
                {
                    Console.WriteLine("WARNING: Unable to restart v2ray service using systemctl");
                }
            }
            return Content(config_text);
        }
    }
}
