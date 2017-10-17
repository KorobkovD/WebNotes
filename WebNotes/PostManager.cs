using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections.Specialized;
using System.Windows.Forms;

namespace WebNotes
{
    public class PostManager
    {
        public PostManager()
        { }

        public string Post (string url, NameValueCollection nvc)
        {
            string str = "";
            using (var webClient = new WebClient())
            {
                try
                {
                    var response = webClient.UploadValues(url, nvc);
                    str = System.Text.Encoding.UTF8.GetString(response);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
                }
            }
            return str;
        }
    }
}
