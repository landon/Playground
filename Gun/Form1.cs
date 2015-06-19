using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gun
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;

            var computer = new Computer();
            computer.Dock = DockStyle.Fill;
            Controls.Add(computer);

            //SayIntro();

            var number = PickNumber();
            computer.ShowNumber(number.ToString());
        }

        void NewGame()
        {
        }

        int PickNumber()
        {
            var rng = new Random(DateTime.Now.Millisecond);
            return rng.Next(-21, 101);
        }

        void SayIntro()
        {
            Say(@"C:\Users\landon\Documents\GitHub\Playground\Gun\intro.wav");
        }
        void SayYes()
        {
            var sp = new SoundPlayer(@"C:\Users\landon\Documents\GitHub\Playground\Gun\yes.wav");
            sp.Play();
        }
        void SayNo()
        {
            var sp = new SoundPlayer(@"C:\Users\landon\Documents\GitHub\Playground\Gun\no.wav");
            sp.Play();
        }
        void SayWin()
        {
            var sp = new SoundPlayer(@"C:\Users\landon\Documents\GitHub\Playground\Gun\win.wav");
            sp.Play();
        }

        void Say(string file)
        {
            var sp = new SoundPlayer(file);
            sp.Play();
        }
    }
}
