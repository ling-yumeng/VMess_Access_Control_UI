namespace WebApplication1.Classes
{
    public class CreateClashConf
    {
        private string template_string { get; set; }
        private string uuid {  get; set; }
        public CreateClashConf(string template_path, string uuid)
        {
            this.template_string = System.IO.File.ReadAllText(template_path);
            this.uuid = uuid;
        }
        public string generateConf()
        {
            int p = template_string.IndexOf("@uuid");
            string result = "";
            result += template_string.Substring(0, p);
            result += uuid;
            result += template_string.Substring(p + ("@uuid").Length, template_string.Length - (p+("@uuid").Length));
            return result;
        }
    }
}
