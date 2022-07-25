//查词程序
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;

namespace pced
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]

        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //检测安装字体
            /*
            int n = 0;
            if (!File.Exists(Environment.SystemDirectory.Substring(0, 1) + @":\WINDOWS\Fonts\SangayanaPlzd.ttf"))
            {
                File.Copy(@".\SangayanaPlzd.ttf", Environment.SystemDirectory.Substring(0, 1) + @":\WINDOWS\Fonts\SangayanaPlzd.ttf");
                n++;
            }
            if (!File.Exists(Environment.SystemDirectory.Substring(0, 1) + @":\WINDOWS\Fonts\VriRomanPlzd.ttf"))
            {
                File.Copy(@".\VriRomanPlzd.ttf", Environment.SystemDirectory.Substring(0, 1) + @":\WINDOWS\Fonts\VriRomanPlzd.ttf");
                n++;
            }
            if (n > 0)
            {
                System.Diagnostics.Process.Start("explorer.exe", Environment.SystemDirectory.Substring(0, 1) + @":\WINDOWS\Fonts");

                formfont = new Form();
                formfont.Height = 210;
                formfont.MaximizeBox = false;
                formfont.SizeGripStyle = SizeGripStyle.Hide;
                formfont.Text = "PCED";
                formfont.StartPosition = FormStartPosition.CenterScreen;
                formfont.TopMost = true;
                formfont.FormClosed += new System.Windows.Forms.FormClosedEventHandler(formfont_FormClosed);

                TextBox tboxfont = new TextBox();
                tboxfont.Multiline = true;
                tboxfont.BackColor = System.Drawing.Color.LightGoldenrodYellow;
                tboxfont.Dock = DockStyle.Top;
                tboxfont.Height = 130;
                tboxfont.TabIndex = 1;
                tboxfont.Text = "第一次运行,安装了所需之Pali字体,倘若第一次运行时Sangayana和VriRoman字体不能正常显示，请重新运行程序或重启计算机！" + "\r\n\r\n" + Strings.StrConv("第一次运行,安装了所需之Pali字体,倘若第一次运行时Sangayana和VriRoman字体不能正常显示，请重新运行程序或重启计算机！", VbStrConv.TraditionalChinese, 0x0409) + "\r\n\r\n" + "first running, installed Pali fonts!";
                tboxfont.Parent = formfont;

                Button btnFont = new Button();
                btnFont.Parent = formfont;
                btnFont.Top = 150;
                btnFont.Left = 100;
                btnFont.Text = "确定";
                btnFont.TabIndex = 0;
                btnFont.Click += new System.EventHandler(btnFont_Click);

                formfont.AcceptButton = btnFont;
                formfont.CancelButton = btnFont;
                formfont.ShowDialog();

                return;
            }
            else
            {
                */
                //禁止启动多个实例
                Boolean createdNew; //返回是否赋予了使用线程的互斥体初始所属权
                System.Threading.Mutex instance = new System.Threading.Mutex(true, "MutexName", out createdNew); //同步基元变量
                if (createdNew) //赋予了线程初始所属权，也就是首次使用互斥体
                {
                    instance.ReleaseMutex();
                }
                else
                {
                    MessageBox.Show("您已经启动了一个PCED辞典程序，请看看PCED的图标是否出现\r\n在“任务栏”里或屏幕右下角的“系统托盘”区，点击它！\r\n\r\n请按确定！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                    return;
                }
            //}

            flashform = new Form();
            flashform.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            flashform.Width = 400;
            flashform.Height = 278;
            flashform.BackgroundImage = System.Drawing.Image.FromFile("ft.jpg");
            flashform.StartPosition = FormStartPosition.CenterScreen;
            flashform.ShowInTaskbar = false;

            lblNamo = new Label();
            lblNamo.Font = new System.Drawing.Font("Tahoma", 10, System.Drawing.FontStyle.Bold);
            lblNamo.BackColor = System.Drawing.Color.CornflowerBlue;
            lblNamo.ForeColor = System.Drawing.Color.Red;
            lblNamo.AutoSize = true;
            lblNamo.Text = "Namo tassa Bhagavato Arahato Sammāsambuddhassa";
            lblNamo.Parent = flashform;
            lblNamo.Top = 3;
            lblNamo.Left = 7;

            lblff = new Label();
            lblff.Font = new System.Drawing.Font("Tahoma", 10);
            lblff.BackColor = System.Drawing.Color.CornflowerBlue;
            lblff.ForeColor = System.Drawing.Color.White;
            lblff.AutoSize = true;
            lblff.Text = "reading dictionary data, please wait a moment ...";
            lblff.Parent = flashform;
            lblff.Top = 251;
            lblff.Left = 50;

            flashform.Show();
            lblNamo.Refresh();
            lblff.Refresh();

            //Application.Run(new Form1());
            mainform = new Form1();
            toolbarform = new frmtool();
            Application.Run(mainform);
        }

        public static Form1 mainform;

        public static frmtool toolbarform;

        public static frmLucn ssfrm;

        private static void formfont_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Restart();
        }

        private static void btnFont_Click(object sender, EventArgs e)
        {
            formfont.Close();
        }

        public static Form cnform;
        public static Form formfont;
        public static Form flashform;
        public static Label lblNamo;
        public static Label lblff;

        //NUM词库词条数量
        //sL[]储存词目
        //strL[]储存与sL词目对应的词条和解释内容
        //en_ 储存 英-巴 词库
        //pl_ 储存 巴-中英 词库

        public static int en_NUM;
        public static string[] en_sL;
        public static string[] en_strL;

        public static int pl_NUM;
        public static string[] pl_sL;
        public static string[] pl_strL;

        public static int NUM;
        public static string[] sL;
        public static string[] strL;

        public static int iNo;

        public static int dcNo(string d, out int iNo)
        {
            iNo = -1;
            int itmp = 0, min = 0, max = NUM - 1;
            if (ab(d, sL[min]))
                return -1;
            if (ab(sL[max], d))
                return -2;
            do
            {
                itmp = (min + max) / 2;
                if (ab(d, sL[itmp]))
                {
                    max = itmp;
                }
                else
                {
                    min = itmp;
                }
            } while (max - min > 1);
            if (!ab(d, sL[min]) & !ab(sL[min], d))
            {
                iNo = min;
                return 0;
            }
            else if (!ab(d, sL[max]) & !ab(sL[max], d))
            {
                iNo = max;
                return 0;
            }
            else
            {
                iNo = min;
                return 1;
            }
        }

        //比较巴利单词
        public static bool ab(string a, string b)
        {
            int i;
            if (a.Length >= b.Length)
                i = a.Length;
            else
                i = b.Length;
            a = a.PadRight(i, ' ');
            b = b.PadRight(i, ' ');
            for (int j = 0; j < i; j++)
            {
                if (iABC(a.Substring(j, 1)) < iABC(b.Substring(j, 1)))
                    return true;
                else if (iABC(a.Substring(j, 1)) > iABC(b.Substring(j, 1)))
                    return false;
            }
            return false;
        }

        //巴利字母转换成1-31数字
        public static int iABC(string inabc)
        {
            switch (inabc.ToUpper())
            {
                case "A":
                    return 1;

                case "Ā":
                    return 2;

                case "I":
                    return 3;

                case "Ī":
                    return 4;

                case "U":
                    return 5;

                case "Ū":
                    return 6;

                case "E":
                    return 7;

                case "O":
                    return 8;

                case "K":
                    return 9;

                case "G":
                    return 10;

                case "Ṅ":
                    return 11;

                case "C":
                    return 12;

                case "J":
                    return 13;

                case "Ñ":
                    return 14;

                case "Ṭ":
                    return 15;

                case "Ḍ":
                    return 16;

                case "Ṇ":
                    return 17;

                case "T":
                    return 18;

                case "D":
                    return 19;

                case "N":
                    return 20;

                case "P":
                    return 21;

                case "B":
                    return 22;

                case "M":
                    return 23;

                case "Y":
                    return 24;

                case "R":
                    return 25;

                case "L":
                    return 26;

                case "V":
                    return 27;

                case "S":
                    return 28;

                case "H":
                    return 29;

                case "Ḷ":
                    return 30;

                case "Ṁ":
                    return 31;

                case "Ṃ":
                    return 31;

                case "Ŋ":
                    return 31;

                default:
                    return 0;
            }
        }

        public static int edcNo(string d, out int iNo)
        {
            iNo = -1;
            int itmp = 0, min = 0, max = NUM - 1;
            if (eab(d, sL[min]))
                return -1;
            if (eab(sL[max], d))
                return -2;
            do
            {
                itmp = (min + max) / 2;
                if (eab(d, sL[itmp]))
                {
                    max = itmp;
                }
                else
                {
                    min = itmp;
                }
            } while (max - min > 1);
            if (!eab(d, sL[min]) & !eab(sL[min], d))
            {
                iNo = min;
                return 0;
            }
            else if (!eab(d, sL[max]) & !eab(sL[max], d))
            {
                iNo = max;
                return 0;
            }
            else
            {
                iNo = min;
                return 1;
            }
        }

        //比较英文单词
        public static bool eab(string a, string b)
        {
            int i;
            if (a.Length > b.Length)
            {
                i = a.Length;
                b = b.PadRight(i, ' ');
            }
            else if (a.Length < b.Length)
            {
                i = b.Length;
                a = a.PadRight(i, ' ');
            }
            else
                i = a.Length;

            for (int j = 0; j < i; j++)
            {
                if (eABC(a.Substring(j, 1)) < eABC(b.Substring(j, 1)))
                    return true;
                else if (eABC(a.Substring(j, 1)) > eABC(b.Substring(j, 1)))
                    return false;
            }
            return false;
        }

        //英文化模式
        public static int eABC(string inabc)
        {
            switch (inabc)
            {
                case "a":
                    return 1;

                case "b":
                    return 2;

                case "c":
                    return 3;

                case "d":
                    return 4;

                case "e":
                    return 5;

                case "f":
                    return 6;

                case "g":
                    return 7;

                case "h":
                    return 8;

                case "i":
                    return 9;

                case "j":
                    return 10;

                case "k":
                    return 11;

                case "l":
                    return 12;

                case "m":
                    return 13;

                case "n":
                    return 14;

                case "o":
                    return 15;

                case "p":
                    return 16;

                case "q":
                    return 17;

                case "r":
                    return 18;

                case "s":
                    return 19;

                case "t":
                    return 20;

                case "u":
                    return 21;

                case "v":
                    return 22;

                case "w":
                    return 23;

                case "x":
                    return 24;

                case "y":
                    return 25;

                case "z":
                    return 26;

                case "ā":
                    return 1;
                case "ī":
                    return 9;
                case "ū":
                    return 21;
                case "ṅ":
                    return 14;
                case "ñ":
                    return 14;
                case "ṭ":
                    return 20;
                case "ḍ":
                    return 4;
                case "ṇ":
                    return 14;
                case "ḷ":
                    return 12;
                case "ŋ":
                    return 13;
                case "ṁ":
                    return 13;
                case "ṃ":
                    return 13;

                case "A":
                    return 1;

                case "B":
                    return 2;

                case "C":
                    return 3;

                case "D":
                    return 4;

                case "E":
                    return 5;

                case "F":
                    return 6;

                case "G":
                    return 7;

                case "H":
                    return 8;

                case "I":
                    return 9;

                case "J":
                    return 10;

                case "K":
                    return 11;

                case "L":
                    return 12;

                case "M":
                    return 13;

                case "N":
                    return 14;

                case "O":
                    return 15;

                case "P":
                    return 16;

                case "Q":
                    return 17;

                case "R":
                    return 18;

                case "S":
                    return 19;

                case "T":
                    return 20;

                case "U":
                    return 21;

                case "V":
                    return 22;

                case "W":
                    return 23;

                case "X":
                    return 24;

                case "Y":
                    return 25;

                case "Z":
                    return 26;

                case "Ā":
                    return 1;
                case "Ī":
                    return 9;
                case "Ū":
                    return 21;
                case "Ṅ":
                    return 14;
                case "Ñ":
                    return 14;
                case "Ṭ":
                    return 20;
                case "Ḍ":
                    return 4;
                case "Ṇ":
                    return 14;
                case "Ḷ":
                    return 12;
                case "Ŋ":
                    return 13;
                case "Ṁ":
                    return 13;
                case "Ṃ":
                    return 13;

                default:
                    return 0;
            }
        }
    }
}