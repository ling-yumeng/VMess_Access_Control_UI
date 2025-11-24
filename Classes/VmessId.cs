namespace WebApplication1.Classes
{
    public class VmessId
    {
        private class VmessId_item
        {
            public int uid;
            public string uuid;
        }
        List<VmessId_item> data;
        public string path;
        public VmessId(string path)
        {
            this.data = new List<VmessId_item>();
            this.path = path;
        }
        public string[] getAllUuids()
        {
            List<string> uuids = new List<string>();
            foreach (var item in this.data)
            {
                uuids.Add(item.uuid);
            }
            return uuids.ToArray();
        }
        public string getUuid(int uid)
        {
            foreach(var item in this.data)
            {
                if (item.uid == uid)
                    return item.uuid;
            }
            return genUuid(uid);
        }
        public void delUuid(string uuid)
        {
            foreach(var item in this.data)
            {
                if(item.uuid == uuid)
                {
                    this.data.Remove(item);
                    break;
                }
            }
            this.write();
        }
        private string genUuid(int uid)
        {
            System.Guid uuid = System.Guid.NewGuid();
            VmessId_item item = new VmessId_item();
            item.uid = uid;
            item.uuid = uuid.ToString();
            this.data.Add(item);
            this.write();
            return item.uuid;
        }
        public void write()
        {
            using (System.IO.FileStream fs = new System.IO.FileStream(this.path, System.IO.FileMode.Create))
                using (System.IO.BinaryWriter writer = new System.IO.BinaryWriter(fs))
            {
                writer.Write(this.data.Count);
                foreach(var item in this.data)
                {
                    writer.Write(item.uid);
                    writer.Write(item.uuid);
                }
            }
        }
        public void read()
        {
            using(System.IO.FileStream fs = new System.IO.FileStream(this.path, System.IO.FileMode.Open))
                using(System.IO.BinaryReader reader = new System.IO.BinaryReader(fs))
            {
                int length = reader.ReadInt32();
                this.data = new List<VmessId_item>();
                for(int i = 0; i < length; i++)
                {
                    VmessId_item item = new VmessId_item();
                    item.uid = reader.ReadInt32();
                    item.uuid = reader.ReadString();
                    this.data.Add(item);
                }
            }
        }
    }
}
