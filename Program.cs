using System.Diagnostics;

namespace WebApplication1
{
    class Program
    {
        private static void db_check()
        {
            if(!System.IO.File.Exists("user_db.dat"))
            {
                Classes.DataBase user_db = new Classes.DataBase("user_db.dat");
                user_db.write();
                Console.WriteLine("Database user_db.dat does not exists, creating it in current working directory.");
            }
            if(!System.IO.File.Exists("token_db.dat"))
            {
                Classes.Token tk_db = new Classes.Token("token_db.dat");
                tk_db.write();
                Console.WriteLine("Database token_db.dat does not exists, creating it in current working directory.");
            }
            if(!System.IO.File.Exists("vmessid_db.dat"))
            {
                Classes.VmessId vmid_db = new Classes.VmessId("vmessid_db.dat");
                vmid_db.write();
                Console.WriteLine("Database vmessid_db.dat does not exists, creating it in current working directory.");
            }
        }
        public static int Main(string[] args)
        {
            Program.db_check();
            if (args.Length == 0)
            {
                System.Console.WriteLine("Application Starting");
                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container.
                builder.Services.AddRazorPages();

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

                app.UseHttpsRedirection();

                app.UseRouting();

                app.UseAuthorization();

                app.MapStaticAssets();
                app.MapRazorPages()
                   .WithStaticAssets();

                app.Run();
                return 0;
            }
            else
            {
                string action = args[0];
                if(action == "useradd")
                {
                    string username = args[1];
                    string password = args[2];
                    Classes.DataBase user_db = new Classes.DataBase("user_db.dat");
                    user_db.read();
                    if(user_db.fetch_uid(username) != -1)
                    {
                        Console.WriteLine("Repeated Addition!");
                        return 1;
                    }
                    user_db.data_push(username, password);
                    System.Console.WriteLine($"Successfully Added User {username}");
                    return 0;
                }
                else if(action == "userdel")
                {
                    string username = args[1];
                    Classes.DataBase user_db = new Classes.DataBase("user_db.dat");
                    user_db.read();
                    int uid = user_db.fetch_uid(username);
                    if (uid == -1)
                    {
                        return 1;
                    }
                    user_db.remove(uid);
                    Classes.Token token_db = new Classes.Token("token_db.dat");
                    token_db.read();
                    int t = token_db.fetch_token(uid);
                    token_db.revoke(t);
                    Classes.VmessId vmessid_db = new Classes.VmessId("vmessid_db.dat");
                    vmessid_db.read();
                    vmessid_db.delUuid(vmessid_db.getUuid(uid));
                    Classes.CreateV2RayConfig v2conf_writer = new Classes.CreateV2RayConfig(vmessid_db.getAllUuids());
                    System.IO.File.WriteAllText("v2ray/config.json", v2conf_writer.export());
                    return 0;
                }
                else
                {
                    System.Console.Error.WriteLine($"Unknown action: {action}");
                    return -1;
                }
            }
        }
    }
}
