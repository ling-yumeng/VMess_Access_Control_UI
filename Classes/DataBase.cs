using System.Security.Cryptography;
using System.Text;

namespace WebApplication1.Classes
{
    public class DataBase
    {
        private class Auth
        {
            public string name = "";
            public int uid;
            public string password_hash;
        }
        private int id_counter = 0;
        private List<Auth> data;
        public string path;
        public DataBase(string path)
        {
            this.path = path;
            this.data = new List<Auth>();
        }
        private static string sha512_gen(string str)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(str);
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] hashByte = sha512.ComputeHash(dataBytes);
                StringBuilder hashString = new StringBuilder();
                foreach (byte b in hashByte)
                {
                    hashString.Append(b.ToString("x2"));
                }
                return hashString.ToString();
            }
        }
        public void data_push(string name, string password)
        {
            Auth new_id = new Auth();
            new_id.name = name;
            new_id.password_hash = DataBase.sha512_gen(password);
            new_id.uid = this.id_counter++;
            this.data.Add(new_id);
            this.write();
        }
        public int fetch_uid(string name)
        {
            int uid = -1;
            foreach(var user in data)
            {
                if( user.name == name )
                {
                    uid = user.uid;
                    break;
                }
            }
            return uid;
        }
        public string usernameof(int uid)
        {
            foreach(var user in data)
            {
                if( user.uid == uid )
                {
                    return user.name; 
                }
            }
            return "";
        }
        public bool user_match(int uid, string password)
        {
            string hashString = DataBase.sha512_gen(password);
            bool match = false;
            foreach(var user in data)
            {
                if (user.uid == uid)
                    if (user.password_hash == hashString)
                    {
                        match = true;
                        break;
                    }
            }
            return match;
        }
        public void remove(int uid)
        {
            foreach(var user in this.data)
            {
                if(user.uid == uid)
                {
                    this.data.Remove(user);
                    break;
                }
            }
            this.write();
        }
        public void write()
        {
            using(System.IO.FileStream fs = new System.IO.FileStream(this.path, FileMode.Create)) using (BinaryWriter writer = new BinaryWriter(fs))
            {
                writer.Write(this.id_counter);
                writer.Write(this.data.Count);
                foreach(var user in this.data)
                {
                    writer.Write(user.uid);
                    writer.Write(user.name);
                    writer.Write(user.password_hash);
                }
            }
        }
        public void read()
        {
            using(FileStream fs = new FileStream(this.path, FileMode.Open)) using(BinaryReader reader = new BinaryReader(fs))
            {
                this.id_counter = reader.ReadInt32();
                int length = reader.ReadInt32();
                for(int i = 0; i < length; i++)
                {
                    Auth user = new Auth();
                    user.uid = reader.ReadInt32();
                    user.name = reader.ReadString();
                    user.password_hash = reader.ReadString();
                    this.data.Add(user);
                }
            }
        }
    }
}
