﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form3 : Form
    {
        Image picture;
        public Image Picture
        {
            get
            {
                return picture;
            }
            set
            {
                picture = value;
                pictureBox1.Image = (Image)(new Bitmap(picture, this.Size.Width, this.Size.Height));
            }
        }
        public Form3()
        {
            InitializeComponent();

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
