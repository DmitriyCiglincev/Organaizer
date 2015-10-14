using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Organaizer
{
    public partial class DialogForm : Form
    {
        string title;
        public DialogForm(string _title)
        {
            InitializeComponent();
            title = _title;
        }

        public DialogForm(string _title,string _textNode)
        {
            InitializeComponent();
            title = _title;
            textBox1.Text = _textNode;
            textBox1.Focus();
        }

        string text;

        public string getText()
        {
            return text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            text = textBox1.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void DialogForm_Load(object sender, EventArgs e)
        {
            this.Text = title;
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
           
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1.PerformClick();
            }
        }
    }
}
