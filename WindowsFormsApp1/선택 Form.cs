using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        Form1 f1;

        public Form2()
        {
            InitializeComponent();
        }

        public Form2(Form1 form)
        {
            InitializeComponent();
            listView1.View = View.Details;
            listView1.Columns.Add("이름");
            listView1.Columns.Add("성별");
            listView1.Columns.Add("생년월일");
            listView1.Columns.Add("양음력");
            f1 = form;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 0)
            {
                int SelectRow = listView1.SelectedItems[0].Index;

                string name = listView1.Items[SelectRow].SubItems[0].Text;
                string gender = listView1.Items[SelectRow].SubItems[1].Text;
                string date = listView1.Items[SelectRow].SubItems[2].Text;
                string solarlunar = listView1.Items[SelectRow].SubItems[3].Text;

                f1.textBox6.Text = name;
                if (solarlunar == "양력") f1.radioButton1.Checked = true;
                else if (solarlunar == "음력") f1.radioButton2.Checked = true;
                else  f1.radioButton3.Checked = true;

                if (gender == "남") f1.radioButton4.Checked = true;
                else f1.radioButton5.Checked = true;

                string[] substr = date.Split('_');
                f1.dateTimePicker1.Value = new DateTime(Int32.Parse(substr[0]), Int32.Parse(substr[1]), Int32.Parse(substr[2]), Int32.Parse(substr[3].Substring(0,2)), Int32.Parse(substr[3].Substring(2, 2)),0);
                //f1.textBox1.Text = substr[0];
                //f1.textBox2.Text = substr[1];
                //f1.textBox3.Text = substr[2];
                //f1.textBox4.Text = substr[3];

                //string date = textBox1.Text + textBox2.Text + textBox3.Text + textBox4.Text + "\n"; // 입력값 
            }
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string[] lines;
            string path = AppDomain.CurrentDomain.BaseDirectory;
            path += "data.txt";

            try
            {
                int c = listView1.FocusedItem.Index;

                listView1.Items.RemoveAt(listView1.FocusedItem.Index);

                using (StreamReader file = new StreamReader(path))
                    lines = File.ReadAllLines(@path);


                using (StreamWriter file = new StreamWriter(path))
                { 
                    for (int i = 0; i<lines.Length; i++)
                    {
                        if(i!= c)
                            file.WriteLine(lines[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("지울 리스트가 없습니다");
            }
            //file.Close();
        }
    }
}
