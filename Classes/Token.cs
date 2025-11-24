using System.Runtime.CompilerServices;

namespace WebApplication1.Classes
{
    public class Token
    {
        private class token_item
        {
            public int uid;
            public int t;
        }
        private List<token_item> tokens;
        public string path;
        public Token(string path)
        {
            tokens = new List<token_item>();
            this.path = path;
        }
        private int new_token(int uid)
        {
            if(tokens.Count >= 8000)
            {
                throw (new Exception("Overflow!!!"));
            }
            int t;
            while (true)
            {
                var random = new Random();
                t = random.Next(10000000, 99999999);
                bool valid = true;
                foreach (var token in tokens)
                {
                    if (token.t == t) { valid = false; break; }
                }
                if (valid)
                {
                    break;
                }
            }
            token_item nt = new token_item();
            nt.uid = uid;
            nt.t = t;
            tokens.Add(nt);
            this.write();
            return t;
        }
        public int fetch_token(int uid)
        {
            foreach (var token in tokens)
            {
                if (token.uid == uid)
                {
                    return token.t;
                }
            }
            return this.new_token(uid);
        }
        public bool valid(int t)
        {
            foreach(var token in tokens)
            {
                if (token.t == t)
                    return true;
            }
            return false;
        }
        public int getUid(int t)
        {
            foreach (var token in tokens)
            {
                if(token.t == t)
                    return token.uid;
            }
            return -1;
        }
        public void revoke(int t)
        {
            foreach (var token in tokens)
            {
                if (token.t == t)
                {
                    tokens.Remove(token);
                    break;
                }
            }
            this.write();
        }
        public void write()
        {
            using(System.IO.FileStream fs = new System.IO.FileStream(this.path, System.IO.FileMode.Create))
                using(System.IO.BinaryWriter writer = new System.IO.BinaryWriter(fs))
            {
                writer.Write(tokens.Count);
                foreach(var token in tokens)
                {
                    writer.Write(token.uid); 
                    writer.Write(token.t);
                }
            }
        }
        public void read()
        {
            using(System.IO.FileStream fs = new System.IO.FileStream(this.path, System.IO.FileMode.Open))
                using(System.IO.BinaryReader reader = new System.IO.BinaryReader(fs))
            {
                int length = reader.ReadInt32();
                this.tokens = new List<token_item>();
                for(int i = 0; i < length; i++)
                {
                    token_item item = new token_item();
                    item.uid = reader.ReadInt32();
                    item.t = reader.ReadInt32();
                    this.tokens.Add(item);
                }
            }
        }
    }
}
