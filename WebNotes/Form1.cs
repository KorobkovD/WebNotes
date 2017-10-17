using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace WebNotes
{
    public partial class Form1 : Form
    {
        PostManager mng;
        string oldNoteValue = "";
        public static int uid;

        public Form1 ()
        {
            InitializeComponent();
            mng = new PostManager();
            uid = -1;
        }

        private void Form1_Load (object sender, EventArgs e)
        {
            textBoxServer.Text = Properties.Settings.Default.Server;
            if (textBoxServer.Text.Length > 0)
            {
                uid = -1;
                FormAuth fa = new FormAuth();
                fa.ShowDialog();
                if (uid > 0)
                    SelectAll();
            }
            else
                MessageBox.Show("Укажите адрес сервера заметок в поле \"Сервер\" и нажмите \"Авторизоваться\"");
        }

        /// <summary>
        /// Запрашивает все заметки с сервера
        /// </summary>
        public void SelectAll()
        {
            dataGridView1.Rows.Clear();
            var pars = new NameValueCollection();
            pars.Add("Cmd", "select");
            pars.Add("UID", uid.ToString());
            string ans = mng.Post(String.Format("http://{0}/api.php", textBoxServer.Text), pars);
            if (ans.Length != 0)
            {
                try
                {
                    var response = JsonConvert.DeserializeObject<List<InnerClasses.SelectResponse>>(ans);
                    foreach (InnerClasses.SelectResponse resp in response)
                    {
                        dataGridView1.Rows.Add(resp.ID, Convert.ToDateTime(resp.DateTime).ToString("d"), resp.Data);
                    }
                }
                catch (Exception exc)
                {
                    richTextBoxMain.Text = ans;
                }
            }
        }

        // Внесение изменений в заметку
        private void dataGridView1_CellEndEdit (object sender, DataGridViewCellEventArgs e)
        {
            int col = dataGridView1.CurrentCellAddress.X;
            int row = dataGridView1.CurrentCellAddress.Y;
            if (col == 2 & String.Compare(dataGridView1.CurrentCell.Value.ToString(), oldNoteValue) != 0)
            {
                if (MessageBox.Show("Применить изменения?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var pars = new NameValueCollection();
                    pars.Add("Cmd", "update");
                    pars.Add("UID", uid.ToString());
                    pars.Add("Text", dataGridView1.CurrentCell.Value.ToString());
                    pars.Add("ID", dataGridView1.Rows[row].Cells[0].Value.ToString());
                    string ans = mng.Post(String.Format("http://{0}/api.php", textBoxServer.Text), pars);
                    if (ans.Length != 0)
                        MessageBox.Show(ans);
                    SelectAll();
                }
            }
            oldNoteValue = "";
        }

        private void dataGridView1_CellBeginEdit (object sender, DataGridViewCellCancelEventArgs e)
        {
            oldNoteValue = dataGridView1.CurrentCell.Value.ToString();
        }

        // создание текста заметки
        private void buttonMain_Click (object sender, EventArgs e)
        {
            var pars = new NameValueCollection();
            pars.Add("Cmd", "insert");
            pars.Add("UID", uid.ToString());
            pars.Add("Text", richTextBoxMain.Text);
            string ans = mng.Post(String.Format("http://{0}/api.php", textBoxServer.Text), pars);
            if (ans.Length != 0)
                MessageBox.Show(ans);
            richTextBoxMain.Clear();
            SelectAll();
        }

        private void удалитьЗаписьToolStripMenuItem_Click (object sender, EventArgs e)
        {
            if (MessageBox.Show("Удалить выбранную заметку?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string id = dataGridView1.Rows[dataGridView1.CurrentCellAddress.Y].Cells[0].Value.ToString();
                var pars = new NameValueCollection();
                pars.Add("Cmd", "delete");
                pars.Add("UID", uid.ToString());
                pars.Add("ID", id);
                string ans = mng.Post(String.Format("http://{0}/api.php", textBoxServer.Text), pars);
                if (ans.Length != 0)
                    MessageBox.Show(ans);
                SelectAll();
            }
        }

        private void dataGridView1_CellMouseClick (object sender, DataGridViewCellMouseEventArgs e)
        {

        }

        // переподключиться
        private void button1_Click (object sender, EventArgs e)
        {
            SelectAll();
        }

        private void textBoxServer_TextChanged (object sender, EventArgs e)
        {
            Properties.Settings.Default.Server = textBoxServer.Text;
            Properties.Settings.Default.Save();
        }

        private void buttonAuth_Click (object sender, EventArgs e)
        {
            FormAuth fa = new FormAuth();
            fa.ShowDialog();
            if (uid > 0)
                SelectAll();
        }

        private void richTextBoxMain_KeyDown (object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter&&e.Shift)
            {
                buttonMain.PerformClick();
            }
        }
    }
}
