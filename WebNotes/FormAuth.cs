using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WebNotes
{
    public partial class FormAuth : Form
    {
        PostManager mng;

        public FormAuth ()
        {
            InitializeComponent();
            mng = new PostManager();
        }

        private void buttonAuth_Click (object sender, EventArgs e)
        {
            Properties.Settings.Default.Login = textBoxLogin.Text;
            Properties.Settings.Default.Save();
            var pars = new NameValueCollection();
            pars.Add("Cmd", "login");
            pars.Add("UID", "0");
            pars.Add("login", textBoxLogin.Text);
            pars.Add("password", textBoxPass.Text);
            string ans = mng.Post(String.Format("http://{0}/api.php", Properties.Settings.Default.Server), pars);
            bool check = false;
            if (ans.Length != 0)
            {
                switch (ans)
                {
                    case "-1":
                        MessageBox.Show("Неправильный логин или пароль");
                        break;
                    case "0":
                        MessageBox.Show("Ошибка 0");
                        break;
                    default:
                        Form1.uid = Convert.ToInt32(ans);
                        check = true;
                        break;
                }
            }
            if (check)
                Close();
        }

        private void buttonClose_Click (object sender, EventArgs e)
        {
            Close();
        }

        private void FormAuth_Load (object sender, EventArgs e)
        {
            textBoxLogin.Text = Properties.Settings.Default.Login;
        }
    }
}
