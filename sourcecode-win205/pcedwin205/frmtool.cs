using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace pced
{
    public partial class frmtool : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        public frmtool()
        {
            InitializeComponent();
        }

        public bool bdroppanel = false;

        /// <summary>
        /// 光标相对于窗口原点的 x 坐标
        /// </summary>
        public int ex = 0;

        /// <summary>
        /// 光标相对于窗口原点的 y 坐标
        /// </summary>
        public int ey = 0;

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            ex = e.X + panel1.Left + this.Left;
            ey = e.Y + panel1.Top + this.Top;

            bdroppanel = true;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (bdroppanel)
            {
                int exnew = e.X + panel1.Left + this.Left;
                int eynew = e.Y + panel1.Top + this.Top;

                int deltax = exnew - ex;
                int deltay = eynew - ey;

                ex = exnew;
                ey = eynew;

                //panel1.Left = panel1.Left + deltax;
                //panel1.Top = panel1.Top + deltay;
                this.Left = this.Left + deltax;
                this.Top = this.Top + deltay;
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            bdroppanel = false;
        }

        private void tsbquit_Click(object sender, EventArgs e)
        {
            //当前可见的读经窗口数目
            int n = 0;
            foreach (tsmiWin tsmiw in Program.toolbarform.tsddb.DropDownItems)
            {
                if (((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).Visible)
                {
                    n++;
                }
            }

            if (n > 0)
            {

                if (MessageBox.Show("您确定要退出本程序吗(Are you sure to exit this program)？", "提示(Tip)：", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Form1._isCloseButton = false;
                    Application.Exit();
                }
            }
            else
            {
                Form1._isCloseButton = false;
                Application.Exit();
            }
        }

        private void tsbmulu_Click(object sender, EventArgs e)
        {
            Program.mainform.frmmuluwindow.Show();
            if (FormWindowState.Minimized == Program.mainform.frmmuluwindow.WindowState)
                Program.mainform.frmmuluwindow.WindowState = FormWindowState.Normal;
            Program.mainform.frmmuluwindow.BringToFront();
        }

        private void tsbdict_Click(object sender, EventArgs e)
        {
            if (Program.mainform.Visible && !(Program.mainform.WindowState == FormWindowState.Minimized))
                Program.mainform.Activate();
            else
            {
                Program.mainform.Visible = true;

                SendMessage(Program.mainform.Handle, 0x112, (IntPtr)0xf120, (IntPtr)0); //恢复窗口

                Program.mainform.WindowState = FormWindowState.Normal;

                Program.mainform.BringToFront();
            }
        }

        private void tsbHide_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void toolStrip1_MouseEnter(object sender, EventArgs e)
        {
            this.Activate();
        }

        public bool btoolwin = false;
        public bool bmuluwin = false;
        //public bool bmainwin = false;

        private void tsmiHideAllWindows_Click(object sender, EventArgs e)
        {
            btoolwin = Program.toolbarform.Visible;
            bmuluwin = Program.mainform.frmmuluwindow.Visible;
            //bmainwin = Program.mainform.Visible;

            foreach (tsmiWin tsmiw in Program.toolbarform.tsddb.DropDownItems)
            {
                ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).Hide();
            }

            this.Hide();
            Program.mainform.frmmuluwindow.Hide();
            Program.mainform.Hide();
        }

        private void tsmiHideAllPaliWindows_Click(object sender, EventArgs e)
        {
            foreach (tsmiWin tsmiw in this.tsddb.DropDownItems)
            {
                ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).Hide();
                //把菜单条目颜色设置为深色，表示对应的读经窗口被隐藏
                tsmiw.BackColor = Color.FromKnownColor(KnownColor.ControlDark);
            }
        }

        //层叠
        private void tsmiCascadeWindows_Click(object sender, EventArgs e)
        {
            int w;//每个窗口的宽度
            int h;//每个窗口的高度
            int xdelta = 22;//每个窗口相对于前一个窗口的x坐标偏移量
            int ydelta = 22;//每个窗口相对于前一个窗口的y坐标偏移量
            w = Screen.PrimaryScreen.WorkingArea.Width - 11 * xdelta;
            h = Screen.PrimaryScreen.WorkingArea.Height - 11 * ydelta;

            //以下进行窗口定位

            int p = 0;//可见的读经窗口索引，从0开始

            foreach (tsmiWin tsmiw in Program.toolbarform.tsddb.DropDownItems)
            {
                if (((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).Visible)
                {
                    //储存旧值
                    if (((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).WindowState == FormWindowState.Maximized)
                        ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).paliStFrm.preisMax = true;
                    else
                        ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).paliStFrm.preisMax = false;

                    ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).paliStFrm.preLeft = ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).Left;
                    ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).paliStFrm.preTop = ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).Top;
                    ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).paliStFrm.preWidth = ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).Width;
                    ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).paliStFrm.preHeight = ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).Height;

                    //赋新值
                    ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).webdw();

                    if (((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).WindowState == FormWindowState.Maximized)
                    {
                        ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).WindowState = FormWindowState.Normal;

                        //此处必须得再一次执行此函数，因如果是从最大化状态转为平铺，则窗口状态改变了两次
                        ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).webdw();
                    }

                    ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).Left = p % 12 * xdelta;
                    ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).Top = p % 12 * ydelta;
                    ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).Width = w;
                    ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).Height = h;

                    ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).BringToFront();

                    p++;
                }
            }
        }

        //平铺
        private void tsmiTileWindows_Click(object sender, EventArgs e)
        {
            //可见的读经窗口数目
            int n = 0;
            foreach (tsmiWin tsmiw in Program.toolbarform.tsddb.DropDownItems)
            {
                if (((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).Visible)
                {
                    n++;
                }
            }
            if (n == 0)
                return;

            //记下当前活动窗口的窗口句柄
            int lightWin = 0;
            bool haslight = false;
            foreach (tsmiWin tsmiw in Program.toolbarform.tsddb.DropDownItems)
            {
                if (tsmiw.BackColor == Color.FromKnownColor(KnownColor.ControlLight))
                {
                    lightWin = Convert.ToInt32(tsmiw.Tag);
                    haslight = true;
                }
            }

            int x;//横向排放窗口数目
            int y;//竖向排放窗口数目
            int xn;//横向第n个窗口，编号从0开始
            int yn;//竖向第n个窗口，编号从0开始
            int w;//每个窗口的宽度
            int h;//每个窗口的高度
            int q;

            q = (int)(Math.Sqrt(n));
            x = q;
            y = q;
            if (x * y < n)
                x = q + 1;
            if (x * y < n)
                y = q + 1;

            w = Screen.PrimaryScreen.WorkingArea.Width / x;
            h = Screen.PrimaryScreen.WorkingArea.Height / y;

            //以下进行窗口定位

            int p = 0;//从0开始的所有可见窗口序号

            foreach (tsmiWin tsmiw in Program.toolbarform.tsddb.DropDownItems)
            {
                if (((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).Visible)
                {
                    //储存旧值
                    if (((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).WindowState == FormWindowState.Maximized)
                        ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).paliStFrm.preisMax = true;
                    else
                        ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).paliStFrm.preisMax = false;

                    ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).paliStFrm.preLeft = ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).Left;
                    ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).paliStFrm.preTop = ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).Top;
                    ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).paliStFrm.preWidth = ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).Width;
                    ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).paliStFrm.preHeight = ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).Height;

                    //赋新值

                    xn = p % x;
                    yn = p / x;

                    ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).webdw();

                    if (((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).WindowState == FormWindowState.Maximized)
                    {
                        ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).WindowState = FormWindowState.Normal;

                        //此处必须得再一次执行此函数，因如果是从最大化状态转为平铺，则窗口状态改变了两次
                        ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).webdw();
                    }

                    ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).Left = xn * w;
                    ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).Top = yn * h;
                    ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).Width = w;
                    ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).Height = h;

                    ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).BringToFront();

                    p++;
                }
            }

            //激活原活动窗口
            if (haslight)
                ((frmpali)(frmpali.FromHandle((IntPtr)(lightWin)))).Activate();
        }

        //最大化所有可见的读经窗口
        private void tsmiMaxAllVisiblePaliWindows_Click(object sender, EventArgs e)
        {
            //记下当前活动窗口的窗口句柄
            int lightWin = 0;
            bool haslight = false;
            foreach (tsmiWin tsmiw in Program.toolbarform.tsddb.DropDownItems)
            {
                if (tsmiw.BackColor == Color.FromKnownColor(KnownColor.ControlLight))
                {
                    lightWin = Convert.ToInt32(tsmiw.Tag);
                    haslight = true;
                }
            }

            foreach (tsmiWin tsmiw in Program.toolbarform.tsddb.DropDownItems)
            {
                if (((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).Visible)
                {
                    //储存旧值
                    if (((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).WindowState == FormWindowState.Maximized)
                        ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).paliStFrm.preisMax = true;
                    else
                        ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).paliStFrm.preisMax = false;

                    ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).paliStFrm.preLeft = ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).Left;
                    ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).paliStFrm.preTop = ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).Top;
                    ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).paliStFrm.preWidth = ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).Width;
                    ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).paliStFrm.preHeight = ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).Height;

                    //赋新值
                    ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).webdw();

                    ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).WindowState = FormWindowState.Maximized;
                }
            }

            //激活原活动窗口
            if (haslight)
                ((frmpali)(frmpali.FromHandle((IntPtr)(lightWin)))).Activate();
        }

        //撤销层叠/平铺/最大化
        private void tsmiRestore_Click(object sender, EventArgs e)
        {
            //记下当前活动窗口的窗口句柄
            int lightWin = 0;
            bool haslight = false;
            foreach (tsmiWin tsmiw in Program.toolbarform.tsddb.DropDownItems)
            {
                if (tsmiw.BackColor == Color.FromKnownColor(KnownColor.ControlLight))
                {
                    lightWin = Convert.ToInt32(tsmiw.Tag);
                    haslight = true;
                }
            }

            //复原
            foreach (tsmiWin tsmiw in Program.toolbarform.tsddb.DropDownItems)
            {
                if (((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).Visible)
                {
                    ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).webdw();

                    if (((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).paliStFrm.preisMax)
                        ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).WindowState = FormWindowState.Maximized;
                    else
                        ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).WindowState = FormWindowState.Normal;

                    ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).Left = ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).paliStFrm.preLeft;
                    ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).Top = ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).paliStFrm.preTop;
                    ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).Width = ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).paliStFrm.preWidth;
                    ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).Height = ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).paliStFrm.preHeight;
                }
            }

            //激活原活动窗口
            if (haslight)
                ((frmpali)(frmpali.FromHandle((IntPtr)(lightWin)))).Activate();
        }

        ToolTip toolTip1;

        private void frmtool_Load(object sender, EventArgs e)
        {
            toolTip1 = new ToolTip();
            toolTip1.AutoPopDelay = 25000;
            toolTip1.InitialDelay = 50;
            toolTip1.ReshowDelay = 50;
            toolTip1.ShowAlways = true;
        }

        private void cboxHccc_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("开启或关闭‘划词查词’功能：\n\n当此复选框处于选中状态时，在读经窗口巴利文本中用鼠标划取或双击选定单词，\n即自动查词并弹出词典窗口；\n\n但请注意：\n如果所划选的内容中包括有标点符号，则不查词；\n\n另外，根据需要：\n可把词典窗口设置为‘总在最前面’显示，这可以在词典的‘其它设置’菜单中设置。", this.cboxHccc);
        }

        private void tsbSearch_Click(object sender, EventArgs e)
        {
            Program.ssfrm.Show();

            if (FormWindowState.Minimized == Program.ssfrm.WindowState)
                Program.ssfrm.WindowState = FormWindowState.Normal;

            Program.ssfrm.BringToFront();
        }
    }

    /// <summary>
    /// 自定义的一个类，继承自 ToolStripMenuItem ，在其中定义了一个 click 事件的处理
    /// </summary>
    public class tsmiWin : ToolStripMenuItem
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        public tsmiWin()
        {
            this.Click += new System.EventHandler(this.tsMenuItem_Click);
        }

        private void tsMenuItem_Click(object sender, EventArgs e)
        {
            if (((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(this.Tag))))).Visible == false)
                ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(this.Tag))))).Visible = true;
            if (((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(this.Tag))))).WindowState == FormWindowState.Minimized)
            {
                SendMessage((IntPtr)(Convert.ToInt32(this.Tag)), 0x112, (IntPtr)0xf120, (IntPtr)0); //恢复窗口
                //((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(this.Tag))))).WindowState = FormWindowState.Normal;
            }
            ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(this.Tag))))).BringToFront();
        }
    }
}
