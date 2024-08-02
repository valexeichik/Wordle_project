using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace wordle
{
    public partial class EndWindow : Form
    {
        Form1 GameWindow;
        public EndWindow()
        {
            InitializeComponent();
        }

        public EndWindow(bool vict, string word, Form1 form)
        {
            InitializeComponent();

            if (vict)
            {
                label.Text = "You win!";
            }

            label1.Text = word[0].ToString();
            label2.Text = word[1].ToString();
            label3.Text = word[2].ToString();
            label4.Text = word[3].ToString();
            label5.Text = word[4].ToString();

            GameWindow = form;
        }

        private void q_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GameWindow.NewGame();
            this.Hide();
        }

        private void EndWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
