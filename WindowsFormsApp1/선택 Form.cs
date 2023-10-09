using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics.Eventing.Reader;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        private int sortColumn = -1;
        private String[] listview_columnTitle = { "이름", "성별", "생년월일", "양음력", "비고" };

        Form1 f1;

        public Form2()
        {
            InitializeComponent();
        }

        private void start_ListView()
        {
            // throw new NotImplementedException();

            listView1.View = View.Details;
            listView1.GridLines = true;
            listView1.FullRowSelect = true;

            int listview_width = listView1.ClientSize.Width;
            int col_width = listview_width / 5;

            listView1.Columns.Add(listview_columnTitle[0], -2, HorizontalAlignment.Center);
            listView1.Columns.Add(listview_columnTitle[1], -2, HorizontalAlignment.Center);
            listView1.Columns.Add(listview_columnTitle[2], -2, HorizontalAlignment.Center);
            listView1.Columns.Add(listview_columnTitle[3], -2, HorizontalAlignment.Center);
            listView1.Columns.Add(listview_columnTitle[4], -2, HorizontalAlignment.Center);
        }

        public Form2(Form1 form)
        {
            InitializeComponent();
            //listView1.View = View.Details;
            //listView1.Columns.Add("이름");
            //listView1.Columns.Add("성별");
            //listView1.Columns.Add("생년월일");
            //listView1.Columns.Add("양음력");
            f1 = form;
            start_ListView();
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
                else if (solarlunar == "") f1.radioButton1.Checked = true;
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
            //string[] lines;
            string path = AppDomain.CurrentDomain.BaseDirectory;
            path += "data.csv";

            DialogResult dr = MessageBox.Show("정말로 삭제하시겠습니까?", "삭제 확인", MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK)
            {
                try
                {
                    int c = listView1.FocusedItem.Index;

                    listView1.Items.RemoveAt(listView1.FocusedItem.Index);

                    using (StreamWriter file = new StreamWriter(path))
                    {
                        for (int i = 0; i < listView1.Items.Count; i++)
                        {
                            string strTmp = "";
                            strTmp += listView1.Items[i].SubItems[0].Text + "," +
                                listView1.Items[i].SubItems[1].Text + "," +
                                listView1.Items[i].SubItems[2].Text + "," +
                                listView1.Items[i].SubItems[3].Text + "," +
                                listView1.Items[i].SubItems[4].Text + ",";
                            file.WriteLine(strTmp);
                        }
                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show("지울 리스트가 없습니다");
                }
            }
            //file.Close();
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine whether the column is the same as the last column clicked.
            if (e.Column != sortColumn)
            {
                // Set the sort column to the new column.
                sortColumn = e.Column;
                // Set the sort order to ascending by default.
                listView1.Sorting = SortOrder.Ascending;
                listView1.Columns[sortColumn].Text = listview_columnTitle[sortColumn] + " ▲";
            }
            else
            {
                // Determine what the last sort order was and change it.
                if (listView1.Sorting == SortOrder.Ascending)
                {
                    listView1.Sorting = SortOrder.Descending;
                    listView1.Columns[sortColumn].Text = listview_columnTitle[sortColumn] + " ▼";
                }
                else
                {
                    listView1.Sorting = SortOrder.Ascending;
                    listView1.Columns[sortColumn].Text = listview_columnTitle[sortColumn] + " ▲";

                }
            }

            // Call the sort method to manually sort.
            listView1.Sort();
            // Set the ListViewItemSorter property to a new ListViewItemComparer
            // object.
            this.listView1.ListViewItemSorter = new MyListViewComparer(e.Column, listView1.Sorting);

        }

        private void ListView1_SelectedIndexChanged(object sender, EventArgs e)

        {

            // 선택 안된 경우도 본 메소드가 호출된다.
            // 따라서 예외처리가 있어야 한다.

            if (listView1.SelectedIndices.Count == 0)
            {
                return;
            }

            textBox1.Text = listView1.SelectedItems[0].SubItems[0].Text;
            textBox2.Text = listView1.SelectedItems[0].SubItems[1].Text;
            textBox3.Text = listView1.SelectedItems[0].SubItems[2].Text;
            textBox5.Text = listView1.SelectedItems[0].SubItems[3].Text;
            textBox4.Text = listView1.SelectedItems[0].SubItems[4].Text;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string[] lines;
            string path = AppDomain.CurrentDomain.BaseDirectory;
            path += "data.csv";

            DialogResult dr = MessageBox.Show("정말로 수정하시겠습니까?", "수정 확인", MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK)
            {
                try
                {
                    int c = listView1.FocusedItem.Index;

                    listView1.Items[c].SubItems[0].Text = textBox1.Text;
                    listView1.Items[c].SubItems[1].Text = textBox2.Text;
                    listView1.Items[c].SubItems[2].Text = textBox3.Text;
                    listView1.Items[c].SubItems[3].Text = textBox5.Text;
                    listView1.Items[c].SubItems[4].Text = textBox4.Text;

                    using (StreamReader file = new StreamReader(path))
                        lines = File.ReadAllLines(@path);

                    using (StreamWriter file = new StreamWriter(path))
                    {
                        for (int i = 0; i < listView1.Items.Count; i++)
                        {
                            string data = listView1.Items[i].SubItems[0].Text + "," + listView1.Items[i].SubItems[1].Text + "," + listView1.Items[i].SubItems[2].Text + "," + listView1.Items[i].SubItems[3].Text + "," + listView1.Items[i].SubItems[4].Text+","; // 입력값 가저오기
                            file.WriteLine(data);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("수정 불가능");
                }
            }
            //file.Close();

        }
    }

    class MyListViewComparer : IComparer
    {
        private int col;
        private SortOrder order;
        public MyListViewComparer()
        {
            col = 0;
            order = SortOrder.Ascending;
        }
        public MyListViewComparer(int column, SortOrder order)
        {
            col = column;
            this.order = order;
        }
        public int Compare(object x, object y)
        {
            int returnVal = -1;
            returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text,
                                    ((ListViewItem)y).SubItems[col].Text);
            // Determine whether the sort order is descending.
            if (order == SortOrder.Descending)
                // Invert the value returned by String.Compare.
                returnVal *= -1;
            return returnVal;
        }
    }
}
