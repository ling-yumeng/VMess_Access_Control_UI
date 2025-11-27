using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication1.Pages
{
    public class ApiGenYamlModel : PageModel
    {
        [BindProperty(SupportsGet=true)]
        public int token { get; set; }
        public IActionResult OnGet()
        {
            Classes.Token tk = new Classes.Token("token_db.dat");
            tk.read();
            int uid = tk.getUid(token);
            Classes.VmessId vmuidb = new Classes.VmessId("vmessid_db.dat");
            vmuidb.read();
            string vmuuid = vmuidb.getUuid(uid);
            string template = System.IO.File.ReadAllText("template.yaml");
            string res = "";
            int index_of_uuid = template.IndexOf("@uuid");
            res += template.Substring(0, index_of_uuid);
            res += vmuuid;
            res += template.Substring(index_of_uuid + "@uuid".Length, template.Length - index_of_uuid - "@uuid".Length);
            return Content(res);
        }
    }
}
