using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication1.Pages
{
    public class PostDataModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    [IgnoreAntiforgeryToken]
    public class LoginModel : PageModel
    {
        public void OnGet()
        {
        }
        private int check_token(int uid)
        {
            Classes.Token tk = new Classes.Token("token_db.dat");
            tk.read();
            int t = tk.fetch_token(uid);
            tk.write();
            return t;
        }
        public IActionResult OnPostJson([FromBody] PostDataModel d) {
            Classes.DataBase db = new Classes.DataBase("user_db.dat");
            db.read();
            var uid = db.fetch_uid(d.Username);
            bool stat = false;
            IActionResult res_page;
            if(uid == -1)
            {
                res_page = new JsonResult(new
                {
                    Success = false
                });
                return res_page;
            }
            res_page = new JsonResult(new
            {
                Success = db.user_match(uid, d.Password),
                Token = check_token(uid)
            });
            return res_page;
        }
    }
}
