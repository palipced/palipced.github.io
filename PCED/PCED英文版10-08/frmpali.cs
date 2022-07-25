using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace pced
{
    [ComVisible(true)]
    public partial class frmpali : Form, IMessageFilter
    {
        //[DllImport("user32.dll", EntryPoint = "SendMessageA")]
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        public frmpali()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 值 0 标识本篇经文为‘根本’，值 1 标识本篇经文为‘义注’，值 2 标识本篇经文为‘复注’，值3标识本篇经文为‘其它’
        /// </summary>
        public int palilb = 3;

        /// <summary>
        /// 值 0 标识本篇经文为‘律’，值 1 标识本篇经文为‘经’，值 2 标识本篇经文为‘论’
        /// </summary>
        public int sanzanglb = 0;

        public string mulafile = "";
        public string atthafile = "";
        public string tikafile = "";

        /// <summary>
        /// 读经窗口附属信息结构
        /// </summary>
        public paliSt paliStFrm;

        //转到当前经典的第n章节，此功能未完成
        private void zhuanDaoNZhangJie(object sender, EventArgs e)
        {
            //webGenben.Document.GetElementById("para519").ScrollIntoView(true);

            int y = 0;
            Point pt = new Point(0, y);

            HtmlElement hel;

        start:

            y = y + 12;

            if (y > 3000 * 1000 / webBrowser1.Width)
                return;

            pt = new Point(0, y);

            hel = webBrowser1.Document.GetElementFromPoint(webBrowser1.PointToScreen(pt));
            if (hel == null)
            {
                goto start;
            }
            else if (hel.Children.Count > 0)
            {
                if (hel.Children[0].Name.Length >= 4 && hel.Children[0].Name.Substring(0, 4) == "para")
                    MessageBox.Show(hel.Children[0].Name);
                else
                {
                    goto start;
                }
            }
            else
            {
                goto start;
            }

            hel.ScrollIntoView(true);
        }

        private void frmpali_SizeChanged(object sender, EventArgs e)
        {
            if (_isResizeBegin)
                return;

            if (_isfind)
            {
                //webBrowser1.Document.GetElementById(sid).ScrollIntoView(true);
                webBrowser1.Document.Window.ScrollTo(webBrowser1.Document.Body.ScrollLeft, webBrowser1.Document.GetElementById(sid).OffsetRectangle.Y - (int)(fCentValue * (webBrowser1.Document.GetElementById(sid).ScrollRectangle.Height)));
                _isfind = false;
            }

            /*
            if (this.WindowState == FormWindowState.Minimized)
            {
                //this.Hide();
                //this.WindowState = FormWindowState.Normal;
                this.Hide();

                //设置菜单条目颜色，以区分窗口当前状态
                foreach (tsmiWin tsmiw in Program.toolbarform.tsddb.DropDownItems)
                {
                    if (Convert.ToInt32(tsmiw.Tag) == this.Handle.ToInt32())
                    {
                        tsmiw.BackColor = Color.FromKnownColor(KnownColor.ControlDark);
                    }
                }
            }
            else
            {
                //当窗体尺寸改变时，手工改变web的宽度，以避免若设置web.Anchor=Right则最小化及恢复时web会耗费时间调整其显示内容页面的宽度
                //webBrowser1.Width = this.Width - 8;
            }
            */
        }

        private void cmWeb2selectAll_Click(object sender, EventArgs e)
        {
            webBrowser1.Document.ExecCommand("SelectAll", true, null);
            _slAll = true;
        }

        private void cmWeb2copy_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            Form1._isPaliWindowCopy = true;

            //下面执行的每个命令都会发生一次剪贴板事件共三次，每次都会激发自动复制取词函数DisplayClipboardData
            Form1.fzbz = 1;
            webBrowser1.Document.ExecCommand("Copy", true, null);
            Form1.cbdText = Program.mainform.outword_t(Clipboard.GetText(TextDataFormat.UnicodeText));
            if (Form1.cbdText != "") //因为金山词霸在本程序窗口里划词的时候会引发异常，故设此条件判断
                Clipboard.SetText(Form1.cbdText, TextDataFormat.UnicodeText);

            Form1._isPaliWindowCopy = false;
            this.Cursor = Cursors.Default;
        }

        private frmmulu mulamulu;
        private frmmulu atthamulu;
        private frmmulu tikamulu;

        private void frmpali_Load(object sender, EventArgs e)
        {
            if (0 == palilb)
            {
                tsbtnmula.Enabled = false;

                atthamulu = new frmmulu();
                atthamulu.fuhandle = this.Handle.ToInt32();
                atthamulu.FormBorderStyle = FormBorderStyle.SizableToolWindow;
                atthamulu.Width = 300;
                atthamulu.Height = 480;
                atthamulu.StartPosition = FormStartPosition.Manual;
                atthamulu.ShowInTaskbar = false;
                atthamulu.panelmulu.Dock = DockStyle.Fill;
                atthamulu.treeView2.Dock = DockStyle.Fill;
                atthamulu.treeView2.BringToFront();
                atthamulu.treeView2.Nodes.Clear();
                atthamulu._isfrmpali = true;
                atthamulu.rootIndex = 1;
                atthamulu.secondIndex = sanzanglb;
                atthamulu.treeView2.Nodes.Add((TreeNode)(Program.mainform.frmmuluwindow.treeView2.Nodes[1].Nodes[sanzanglb].Clone()));
                atthamulu.treeView2.Nodes[0].Expand();

                tikamulu = new frmmulu();
                tikamulu.fuhandle = this.Handle.ToInt32();
                tikamulu.FormBorderStyle = FormBorderStyle.SizableToolWindow;
                tikamulu.Width = 300;
                tikamulu.Height = 480;
                tikamulu.StartPosition = FormStartPosition.Manual;
                tikamulu.ShowInTaskbar = false;
                tikamulu.panelmulu.Dock = DockStyle.Fill;
                tikamulu.treeView2.Dock = DockStyle.Fill;
                tikamulu.treeView2.BringToFront();
                tikamulu.treeView2.Nodes.Clear();
                tikamulu._isfrmpali = true;
                tikamulu.rootIndex = 2;
                tikamulu.secondIndex = sanzanglb;
                tikamulu.treeView2.Nodes.Add((TreeNode)(Program.mainform.frmmuluwindow.treeView2.Nodes[2].Nodes[sanzanglb].Clone()));
                tikamulu.treeView2.Nodes[0].Expand();
            }
            if (1 == palilb)
            {
                tsbtnattha.Enabled = false;

                mulamulu = new frmmulu();
                mulamulu.fuhandle = this.Handle.ToInt32();
                mulamulu.FormBorderStyle = FormBorderStyle.SizableToolWindow;
                mulamulu.Width = 300;
                mulamulu.Height = 480;
                mulamulu.StartPosition = FormStartPosition.Manual;
                mulamulu.ShowInTaskbar = false;
                mulamulu.panelmulu.Dock = DockStyle.Fill;
                mulamulu.treeView2.Dock = DockStyle.Fill;
                mulamulu.treeView2.BringToFront();
                mulamulu.treeView2.Nodes.Clear();
                mulamulu._isfrmpali = true;
                mulamulu.rootIndex = 0;
                mulamulu.secondIndex = sanzanglb;
                mulamulu.treeView2.Nodes.Add((TreeNode)(Program.mainform.frmmuluwindow.treeView2.Nodes[0].Nodes[sanzanglb].Clone()));
                mulamulu.treeView2.Nodes[0].Expand();

                tikamulu = new frmmulu();
                tikamulu.fuhandle = this.Handle.ToInt32();
                tikamulu.FormBorderStyle = FormBorderStyle.SizableToolWindow;
                tikamulu.Width = 300;
                tikamulu.Height = 480;
                tikamulu.StartPosition = FormStartPosition.Manual;
                tikamulu.ShowInTaskbar = false;
                tikamulu.panelmulu.Dock = DockStyle.Fill;
                tikamulu.treeView2.Dock = DockStyle.Fill;
                tikamulu.treeView2.BringToFront();
                tikamulu.treeView2.Nodes.Clear();
                tikamulu._isfrmpali = true;
                tikamulu.rootIndex = 2;
                tikamulu.secondIndex = sanzanglb;
                tikamulu.treeView2.Nodes.Add((TreeNode)(Program.mainform.frmmuluwindow.treeView2.Nodes[2].Nodes[sanzanglb].Clone()));
                tikamulu.treeView2.Nodes[0].Expand();
            }
            if (2 == palilb)
            {
                tsbtntika.Enabled = false;

                mulamulu = new frmmulu();
                mulamulu.fuhandle = this.Handle.ToInt32();
                mulamulu.FormBorderStyle = FormBorderStyle.SizableToolWindow;
                mulamulu.Width = 300;
                mulamulu.Height = 480;
                mulamulu.StartPosition = FormStartPosition.Manual;
                mulamulu.ShowInTaskbar = false;
                mulamulu.panelmulu.Dock = DockStyle.Fill;
                mulamulu.treeView2.Dock = DockStyle.Fill;
                mulamulu.treeView2.BringToFront();
                mulamulu.treeView2.Nodes.Clear();
                mulamulu._isfrmpali = true;
                mulamulu.rootIndex = 0;
                mulamulu.secondIndex = sanzanglb;
                mulamulu.treeView2.Nodes.Add((TreeNode)(Program.mainform.frmmuluwindow.treeView2.Nodes[0].Nodes[sanzanglb].Clone()));
                mulamulu.treeView2.Nodes[0].Expand();

                atthamulu = new frmmulu();
                atthamulu.fuhandle = this.Handle.ToInt32();
                atthamulu.FormBorderStyle = FormBorderStyle.SizableToolWindow;
                atthamulu.Width = 300;
                atthamulu.Height = 480;
                atthamulu.StartPosition = FormStartPosition.Manual;
                atthamulu.ShowInTaskbar = false;
                atthamulu.panelmulu.Dock = DockStyle.Fill;
                atthamulu.treeView2.Dock = DockStyle.Fill;
                atthamulu.treeView2.BringToFront();
                atthamulu.treeView2.Nodes.Clear();
                atthamulu._isfrmpali = true;
                atthamulu.rootIndex = 1;
                atthamulu.secondIndex = sanzanglb;
                atthamulu.treeView2.Nodes.Add((TreeNode)(Program.mainform.frmmuluwindow.treeView2.Nodes[1].Nodes[sanzanglb].Clone()));
                atthamulu.treeView2.Nodes[0].Expand();
            }
            if (3 == palilb)
            {
                tsbtnmula.Enabled = false;
                tsbtnattha.Enabled = false;
                tsbtntika.Enabled = false;
            }

            tsslPage.Text = "";
            tsslVRIPage.Text = "";
            tsslMyanmarPage.Text = "";
            tsslPTSPage.Text = "";
            tsslThaiPage.Text = "";

            //Application.AddMessageFilter(this);
        }

        private void tsbtnmula_Click(object sender, EventArgs e)
        {
            if (mulafile == "")
            {
                mulamulu.Left = this.Left + 9;
                mulamulu.Top = this.Top + 48;
                mulamulu.Show();
                mulamulu.BringToFront();
            }
            else
            {
                string bookpath = @".\pali\" + mulafile + ".htm";

                if (!(File.Exists(bookpath)))
                {
                    MessageBox.Show("此篇经典文件没找到！您可能没有安装本程序的‘pali经典文件库’或者是删除了文件！\r\n请到‘觉悟之路’网站 http://www.dhamma.org.cn/ 下载本程序的‘pali经典文件库’，\r\n解压缩后将经典文件复制到本程序目录下的 pali\\ 子目录里。");
                    return;
                }

                StreamReader sr = new StreamReader(new FileStream(bookpath, FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
                string strSZ = sr.ReadToEnd();
                sr.Close();








                //改变不同字体大小设置的style风格
                if (!(Program.mainform.fsoriginal.Checked))
                    strSZ = new Regex(@"<style>[\S\s]*</style>", RegexOptions.IgnoreCase).Replace(strSZ, Form1.strf1);










                strSZ = new Regex("<body>", RegexOptions.IgnoreCase).Replace(strSZ, "<body onload = 'window.external.htmonload1()' onmouseup = 'window.external.htmonmouseup()'>" + frmmulu.strJavaScript);
                //引号前后加空格，以避免在浏览器中查找时，被连着引号一起当成一个词，而造成引号边的词查找不出
                strSZ = new Regex("(?<w>‘+|’+)", RegexOptions.None).Replace(strSZ, " ${w} ");

                strSZ = new Regex("<p class=\"nikaya\">", RegexOptions.None).Replace(strSZ, "<p class=\"nikaya\"><a name=\"nikaya\"></a>");
                strSZ = new Regex("<p class=\"book\">", RegexOptions.None).Replace(strSZ, "<p class=\"book\"><a name=\"book\"></a>");
                strSZ = new Regex("<p class=\"title\">", RegexOptions.None).Replace(strSZ, "<p class=\"title\"><a name=\"title\"></a>");
                strSZ = new Regex("<p class=\"chapter\">", RegexOptions.None).Replace(strSZ, "<p class=\"chapter\"><a name=\"chapter\"></a>");

                frmpali frmw = new frmpali();

                foreach (string strName in Program.mainform.alName)
                {
                    if (strName.Substring(0, 20).Trim() == mulafile)
                    {
                        TreeNode trNx;
                        string sNodeIdx = strName.Substring(20);
                        trNx = Program.mainform.frmmuluwindow.treeView2.Nodes[Convert.ToInt32(sNodeIdx.Substring(sNodeIdx.Length - 1 * 2, 1 * 2).Trim())];
                        if (sNodeIdx.Length > 1 * 2)
                        {
                            for (int nx = sNodeIdx.Length - 2 * 2; nx > -1; nx = nx - 2)
                            {
                                trNx = trNx.Nodes[Convert.ToInt32(sNodeIdx.Substring(nx, 2).Trim())];
                            }
                        }

                        //当前章节是根本
                        frmw.palilb = 0;

                        //取得根节点之下一层节点的索引，以确定当前点击的章节是‘律’、‘经’或‘论’
                        TreeNode trN = trNx;
                        while (trN.Level > 1)
                        {
                            trN = trN.Parent;
                        }
                        frmw.sanzanglb = trN.Index;

                        frmw.Text = trNx.Text;
                        frmw.mulafile = ((tvtag)(trNx.Tag)).fnmula;
                        frmw.atthafile = ((tvtag)(trNx.Tag)).fnattha;
                        frmw.tikafile = ((tvtag)(trNx.Tag)).fntika;

                        break;
                    }
                }

                frmw.webBrowser1.ObjectForScripting = frmw;
                frmw.paliStFrm = new paliSt();

                tsmiWin dd = new tsmiWin();
                dd.Text = frmw.Text;
                dd.Tag = frmw.Handle.ToInt32();
                Program.toolbarform.tsddb.DropDownItems.Add(dd);

                frmw.Show();
                Form1.frmpalihandle = frmw.Handle.ToInt32();

                frmw.webBrowser1.DocumentText = strSZ;
            }
        }

        private void tsbtnattha_Click(object sender, EventArgs e)
        {
           if (atthafile == "")
            {
                atthamulu.Left = this.Left + 9 + 60;
                atthamulu.Top = this.Top + 48;
                atthamulu.Show();
                atthamulu.BringToFront();
            }
            else
            {
                string  bookpath = @".\pali\" + atthafile + ".htm";

                if (!(File.Exists(bookpath)))
                {
                    MessageBox.Show("此篇经典文件没找到！您可能没有安装本程序的‘pali经典文件库’或者是删除了文件！\r\n请到‘觉悟之路’网站 http://www.dhamma.org.cn/ 下载本程序的‘pali经典文件库’，\r\n解压缩后将经典文件复制到本程序目录下的 pali\\ 子目录里。");
                    return;
                }

                StreamReader sr = new StreamReader(new FileStream(bookpath, FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
                string strSZ = sr.ReadToEnd();
                sr.Close();








                //改变不同字体大小设置的style风格
                if (!(Program.mainform.fsoriginal.Checked))
                    strSZ = new Regex(@"<style>[\S\s]*</style>", RegexOptions.IgnoreCase).Replace(strSZ, Form1.strf1);









                strSZ = new Regex("<body>", RegexOptions.IgnoreCase).Replace(strSZ, "<body onload = 'window.external.htmonload1()' onmouseup = 'window.external.htmonmouseup()'>" + frmmulu.strJavaScript);
                //引号前后加空格，以避免在浏览器中查找时，被连着引号一起当成一个词，而造成引号边的词查找不出
                strSZ = new Regex("(?<w>‘+|’+)", RegexOptions.None).Replace(strSZ, " ${w} ");

                strSZ = new Regex("<p class=\"nikaya\">", RegexOptions.None).Replace(strSZ, "<p class=\"nikaya\"><a name=\"nikaya\"></a>");
                strSZ = new Regex("<p class=\"book\">", RegexOptions.None).Replace(strSZ, "<p class=\"book\"><a name=\"book\"></a>");
                strSZ = new Regex("<p class=\"title\">", RegexOptions.None).Replace(strSZ, "<p class=\"title\"><a name=\"title\"></a>");
                strSZ = new Regex("<p class=\"chapter\">", RegexOptions.None).Replace(strSZ, "<p class=\"chapter\"><a name=\"chapter\"></a>");

                frmpali frmw = new frmpali();

                foreach (string strName in Program.mainform.alName)
                {
                    if (strName.Substring(0, 20).Trim() == atthafile)
                    {
                        TreeNode trNx;
                        string sNodeIdx = strName.Substring(20);
                        trNx = Program.mainform.frmmuluwindow.treeView2.Nodes[Convert.ToInt32(sNodeIdx.Substring(sNodeIdx.Length - 1 * 2, 1 * 2).Trim())];
                        if (sNodeIdx.Length > 1 * 2)
                        {
                            for (int nx = sNodeIdx.Length - 2 * 2; nx > -1; nx = nx - 2)
                            {
                                trNx = trNx.Nodes[Convert.ToInt32(sNodeIdx.Substring(nx, 2).Trim())];
                            }
                        }

                        //当前章节是义注
                        frmw.palilb = 1;

                        //取得根节点之下一层节点的索引，以确定当前点击的章节是‘律’、‘经’或‘论’
                        TreeNode trN = trNx;
                        while (trN.Level > 1)
                        {
                            trN = trN.Parent;
                        }
                        frmw.sanzanglb = trN.Index;

                        frmw.Text = trNx.Text;
                        frmw.mulafile = ((tvtag)(trNx.Tag)).fnmula;
                        frmw.atthafile = ((tvtag)(trNx.Tag)).fnattha;
                        frmw.tikafile = ((tvtag)(trNx.Tag)).fntika;

                        break;
                    }
                }

                frmw.webBrowser1.ObjectForScripting = frmw;
                frmw.paliStFrm = new paliSt();

                tsmiWin dd = new tsmiWin();
                dd.Text = frmw.Text;
                dd.Tag = frmw.Handle.ToInt32();
                Program.toolbarform.tsddb.DropDownItems.Add(dd);

                frmw.Show();
                Form1.frmpalihandle = frmw.Handle.ToInt32();

                frmw.webBrowser1.DocumentText = strSZ;
            }
        }

        private void tsbtntika_Click(object sender, EventArgs e)
        {
            if (tikafile == "")
            {
                tikamulu.Left = this.Left + 9 + 60 + 60;
                tikamulu.Top = this.Top + 48;
                tikamulu.Show();
                tikamulu.BringToFront();
            }
            else
            {
                string bookpath = @".\pali\" + tikafile + ".htm";

                if (!(File.Exists(bookpath)))
                {
                    MessageBox.Show("此篇经典文件没找到！您可能没有安装本程序的‘pali经典文件库’或者是删除了文件！\r\n请到‘觉悟之路’网站 http://www.dhamma.org.cn/ 下载本程序的‘pali经典文件库’，\r\n解压缩后将经典文件复制到本程序目录下的 pali\\ 子目录里。");
                    return;
                }

                StreamReader sr = new StreamReader(new FileStream(bookpath, FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
                string strSZ = sr.ReadToEnd();
                sr.Close();







                //改变不同字体大小设置的style风格
                if (!(Program.mainform.fsoriginal.Checked))
                    strSZ = new Regex(@"<style>[\S\s]*</style>", RegexOptions.IgnoreCase).Replace(strSZ, Form1.strf1);







                strSZ = new Regex("<body>", RegexOptions.IgnoreCase).Replace(strSZ, "<body onload = 'window.external.htmonload1()' onmouseup = 'window.external.htmonmouseup()'>" + frmmulu.strJavaScript);
                //引号前后加空格，以避免在浏览器中查找时，被连着引号一起当成一个词，而造成引号边的词查找不出
                strSZ = new Regex("(?<w>‘+|’+)", RegexOptions.None).Replace(strSZ, " ${w} ");

                strSZ = new Regex("<p class=\"nikaya\">", RegexOptions.None).Replace(strSZ, "<p class=\"nikaya\"><a name=\"nikaya\"></a>");
                strSZ = new Regex("<p class=\"book\">", RegexOptions.None).Replace(strSZ, "<p class=\"book\"><a name=\"book\"></a>");
                strSZ = new Regex("<p class=\"title\">", RegexOptions.None).Replace(strSZ, "<p class=\"title\"><a name=\"title\"></a>");
                strSZ = new Regex("<p class=\"chapter\">", RegexOptions.None).Replace(strSZ, "<p class=\"chapter\"><a name=\"chapter\"></a>");

                frmpali frmw = new frmpali();

                foreach (string strName in Program.mainform.alName)
                {
                    if (strName.Substring(0, 20).Trim() == tikafile)
                    {
                        TreeNode trNx;
                        string sNodeIdx = strName.Substring(20);
                        trNx = Program.mainform.frmmuluwindow.treeView2.Nodes[Convert.ToInt32(sNodeIdx.Substring(sNodeIdx.Length - 1 * 2, 1 * 2).Trim())];
                        if (sNodeIdx.Length > 1 * 2)
                        {
                            for (int nx = sNodeIdx.Length - 2 * 2; nx > -1; nx = nx - 2)
                            {
                                trNx = trNx.Nodes[Convert.ToInt32(sNodeIdx.Substring(nx, 2).Trim())];
                            }
                        }

                        //当前章节是复注
                        frmw.palilb = 2;

                        //取得根节点之下一层节点的索引，以确定当前点击的章节是‘律’、‘经’或‘论’
                        TreeNode trN = trNx;
                        while (trN.Level > 1)
                        {
                            trN = trN.Parent;
                        }
                        frmw.sanzanglb = trN.Index;

                        frmw.Text = trNx.Text;
                        frmw.mulafile = ((tvtag)(trNx.Tag)).fnmula;
                        frmw.atthafile = ((tvtag)(trNx.Tag)).fnattha;
                        frmw.tikafile = ((tvtag)(trNx.Tag)).fntika;

                        break;
                    }
                }

                frmw.webBrowser1.ObjectForScripting = frmw;
                frmw.paliStFrm = new paliSt();

                tsmiWin dd = new tsmiWin();
                dd.Text = frmw.Text;
                dd.Tag = frmw.Handle.ToInt32();
                Program.toolbarform.tsddb.DropDownItems.Add(dd);

                frmw.Show();
                Form1.frmpalihandle = frmw.Handle.ToInt32();

                frmw.webBrowser1.DocumentText = strSZ;
            }
        }

        private void frmpali_Activated(object sender, EventArgs e)
        {
            //给浏览器窗口设置焦点，使得选中的文字高亮显示
            if (webBrowser1.Document != null)
                webBrowser1.Document.Window.Focus();

            if (mulamulu != null && mulamulu.Visible)
                mulamulu.Hide();

            if (atthamulu != null && atthamulu.Visible)
                atthamulu.Hide();

            if (tikamulu != null && tikamulu.Visible)
                tikamulu.Hide();

            //设定菜单条目颜色，以区分窗口当前状态
            if (Form1._isSelfActivate)
            {
                foreach (tsmiWin tsmiw in Program.toolbarform.tsddb.DropDownItems)
                {
                    if (tsmiw.BackColor == Color.FromKnownColor(KnownColor.ControlLight))
                    {
                        tsmiw.BackColor = Color.FromKnownColor(KnownColor.Control);
                    }

                    if (Convert.ToInt32(tsmiw.Tag) == this.Handle.ToInt32())
                    {
                        tsmiw.BackColor = Color.FromKnownColor(KnownColor.ControlLight);
                    }
                }
            }
        }

        //private void tsbshowmulu_Click(object sender, EventArgs e)
        //{
        //    Program.mainform.frmmuluwindow.Show();
        //    if (FormWindowState.Minimized == Program.mainform.frmmuluwindow.WindowState)
        //        Program.mainform.frmmuluwindow.WindowState = FormWindowState.Normal;
        //    Program.mainform.frmmuluwindow.BringToFront();
        //}

        //private void tsbshowdict_Click(object sender, EventArgs e)
        //{
        //    if (Program.mainform.Visible && !(Program.mainform.WindowState == FormWindowState.Minimized))
        //        Program.mainform.Activate();
        //    else
        //    {
        //        Program.mainform.Visible = true;

        //        SendMessage(Program.mainform.Handle, 0x112, (IntPtr)0xf120, (IntPtr)0); //恢复窗口

        //        Program.mainform.WindowState = FormWindowState.Normal;

        //        Program.mainform.BringToFront();
        //    }
        //}

        //private void btsquit_Click(object sender, EventArgs e)
        //{
            //Form1._isCloseButton = false;
            //Application.Exit();
        //}

        private void frmpali_FormClosing(object sender, FormClosingEventArgs e)
        {
            //从窗口列表里删除
            int its = 0;
            for (int i = 0; i < Program.toolbarform.tsddb.DropDownItems.Count;i++ )
            {
                if (Convert.ToInt32(Program.toolbarform.tsddb.DropDownItems[i].Tag) == this.Handle.ToInt32())
                {
                    its = i;
                    break;
                }
            }
            Program.toolbarform.tsddb.DropDownItems.RemoveAt(its); 
        }

        private void frmpali_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Application.RemoveMessageFilter(this);
        }

        public bool PreFilterMessage(ref Message m)
        {
            //WM_SYSCOMMAND 系统命令消息中的最小化消息
            if (m.WParam.ToInt32() == 0xF020)
            {
                //MessageBox.Show("k");
                //this.Hide();
                //return true;
            }

            //WM_NCLBUTTONDOWN 左击非客户区消息，而且鼠标点击时位于标题栏最小化按钮矩形范围之内
            if (m.Msg == 0x00A1)
            {
                Rectangle vRectangle = new Rectangle(Width - 57, 7, 15, 15);

                Point vPoint = new Point((int)m.LParam);
                vPoint.Offset(-Left, -Top);
                if (vRectangle.Contains(vPoint))
                {
                    //MessageBox.Show(vPoint.ToString());
                    this.Hide();

                    //设定菜单条目颜色，以区分窗口当前状态
                    foreach (tsmiWin tsmiw in Program.toolbarform.tsddb.DropDownItems)
                    {
                        if (Convert.ToInt32(tsmiw.Tag) == this.Handle.ToInt32())
                        {
                            tsmiw.BackColor = Color.FromKnownColor(KnownColor.ControlDark);
                        }
                    }

                    return true;
                }
            }

            //WM_NCLBUTTONDOWN 左击非客户区消息 WM_SYSCOMMAND 系统命令消息
            if (m.Msg == 0x00A1 || (m.Msg == 0x0112 && !(this.WindowState == FormWindowState.Minimized)))
            {
                //webbodyTop = webGenben.Document.Body.ScrollTop;
                //return true;
            }

            /*
            
            if (m.Msg == 0x0100) //键盘消息
            {
                keyValue = LOWORD((int)(m.WParam)); //取得按键的键值

                if (_islistweb && cboxInput.Focused == false && (keyValue == WM_KEYUP || keyValue == WM_KEYDOWN || keyValue == WM_KEYPAGEUP || keyValue == WM_KEYPAGEDOWN || keyValue == WM_KEYHOME || keyValue == WM_KEYEND))
                {
                    cboxInput.Select();
                    return true;
                }
            }

            if (m.Msg == 0x020A)
            {
                delta = HIWORD((int)(m.WParam));

                if (_islistweb)
                {
                    if ((Convert.ToInt16(preId) == listmaxNO && delta > 0) | (Convert.ToInt16(preId) == 1 && delta < 0) | (Convert.ToInt16(preId) > 1 && Convert.ToInt16(preId) < listmaxNO))
                    {
                        webBrowser1.Document.GetElementById(preId).Style = "color:#000000; background-color:#FFFFFF; display:block; width:100%; height:18px;";
                        webBrowser1.Document.GetElementById(preId + "s").Style = "display:none;";
                    }

                    string s1 = "";

                    s1 = (Convert.ToInt32(preId) - delta / 120).ToString();

                    if (Convert.ToInt16(s1) < 1)
                        s1 = "1";

                    if (Convert.ToInt16(s1) > listmaxNO)
                        s1 = listmaxNO.ToString();

                    preId = s1;
                    webBrowser1.Document.GetElementById(s1).Style = "display:block; width:100%; background-color:#efefff; color:#001CFF;";
                    webBrowser1.Document.GetElementById(s1 + "s").Style = "display:block; width:100%; background-color:#efefff; height:18px; cursor:pointer; overflow:hidden;";
                    webBrowser1.Document.Window.ScrollTo(webBrowser1.Document.Body.ScrollLeft, webBrowser1.Document.Body.ScrollTop - delta * 18 / 120);

                    //高亮显示部分单词
                    //textBox3.Text = webBrowser1.Document.GetElementById(s1).InnerText + "\r\n\r\n\r\n\r\n" + webBrowser1.Document.GetElementById(s1).InnerHtml;
                    hlText = new Regex(@"\r\n[\s\S]*", RegexOptions.None).Replace(webBrowser1.Document.GetElementById(s1).InnerText, "").Substring(cbtL);
                    cboxInput.Text = cbT + outword_t(hlText);
                    cboxInput.Select(cbtL, hlText.Length);
                }
                else
                {
                    webBrowser1.Document.Window.ScrollTo(webBrowser1.Document.Body.ScrollLeft, webBrowser1.Document.Body.ScrollTop - delta * 18 / 120);
                }

                return true;
            }

            */

            return false;
        }

        /// <summary>
        /// 标识鼠标移进的是哪一个按钮，如果与前一次一样移进的同一个按钮，则不执行
        /// </summary>
        private int btnBz = 0;

        private void tsbtnmula_MouseEnter(object sender, EventArgs e)
        {
            if (1 != btnBz)
            {
                btnBz = 1;
                //this.Activate();
            }
        }

        private void tsbtnattha_MouseEnter(object sender, EventArgs e)
        {
            if (2 != btnBz)
            {
                btnBz = 2;
                //this.Activate();
            }
        }

        private void tsbtntika_MouseEnter(object sender, EventArgs e)
        {
            if (3 != btnBz)
            {
                btnBz = 3;
                //this.Activate();
            }
        }

        private void tsbShowToolbar_Click(object sender, EventArgs e)
        {
            Program.toolbarform.Show();
            //如不加此句，初始尺寸就会高出几个像素，须用手工移动窗口才消失，不知为何？
            Program.toolbarform.Height = 25;
        }

        /// <summary>
        /// 用正则表达式查找加亮 目前不使用此函数
        /// </summary>
        public void fdword()
        {
            //这一句这样写会否有问题呢？
            //webBrowser1.Document.Focus();
            //为何用大写的F不行？
            //SendKeys.Send("^f");

            //StreamReader sr = new StreamReader(new FileStream(@".\t-vin11.htm", FileMode.Open), System.Text.Encoding.GetEncoding(65001));
            //string strLine = sr.ReadToEnd();
            //sr.Close();
            string strLine = webBrowser1.DocumentText;

            //储存html文件头
            string shtmhead = "";
            MatchCollection mc = new Regex(@"<html>[\s\S]*</head>", RegexOptions.None).Matches(strLine);
            foreach (Match ma in mc)
            {
                shtmhead = ma.Groups[0].Value;
            }

            //去除html文件头，避免在下一步中匹配加亮时出错
            strLine = new Regex(@"<html>[\s\S]*</head>", RegexOptions.None).Replace(strLine, "");

            //清除上一次查询留下的锚点
            strLine = new Regex(@" class= 'hit' id='hit\d+'", RegexOptions.None).Replace(strLine, "");

            //加高亮关键词，加锚点
            //Form1 c = new Form1();
            MatchEvaluator m = new MatchEvaluator(ReplaceCC);
            strLine = new Regex(@"(?<=>[^<>]*)\b" + tstboxWord.Text.Trim() + @"\b(?=[^<>]*<)", RegexOptions.None).Replace(strLine, m);

            //载入
            webBrowser1.DocumentText = shtmhead + strLine;

            //高亮后的html写入磁盘，此段可删除
            //StreamWriter sw = new StreamWriter(@".\p2.htm", false, System.Text.Encoding.GetEncoding(65001));
            //sw.Write(shtmhead + strLine);
            //sw.Close();

            //记录词频
            tslHitsNum.Text = (i + 1).ToString();
            icurHit = 0;
            if ((i + 1) > 0)
            {
                btnFirst.Enabled = true;
                btnPrior.Enabled = true;
                btnNext.Enabled = true;
                btnLast.Enabled = true;
            }
            else
            {
                btnFirst.Enabled = false;
                btnPrior.Enabled = false;
                btnNext.Enabled = false;
                btnLast.Enabled = false;
            }
        }

        public int i = -1;

        // Replace each Regex cc match with the number of the occurrence.
        public string ReplaceCC(Match m)
        {
            i++;
            return "<a class= 'hit' id='hit" + i.ToString() + "'>" + tstboxWord.Text.Trim() + "</a>";
        }

        //已改为在js脚本HighLight函数中hit后加nc来标记，n为数字，表示是第几次调用HighLight函数
        //这样在本窗口中按前后键定位查找到的词时，可以之区分当前的hit标记，根据nc来区分，而不再清除上次的高亮
        /// <summary>
        /// 重置前后键按钮的状态
        /// </summary>
        private void clearHighLight()
        {
            //此段引致查找速度变得特慢，已注释掉
            /*
            for (int z = 0; z < i + 1; z++)
            {
                webBrowser1.Document.GetElementById("hit" + z.ToString()).Style = "";
                webBrowser1.Document.GetElementById("hit" + z.ToString()).Id = "";
            }
            */

            i = -1;
            tslHitsNum.Text = "0";

            btnFirst.Enabled = false;
            btnPrior.Enabled = false;
            btnNext.Enabled = false;
            btnLast.Enabled = false;
        }

        private void tsbCopy_Click(object sender, EventArgs e)
        {
            MessageBox.Show("if you had selected one 'input && output font-code setting:' in dictionary window, \r\n then pali text be copyed out is the same font-code pali text in this window.", "about copy:");
        }

        int d = 0;
        string sid;
        bool _isfind;

        /// <summary>
        /// 网页定位
        /// </summary>
        public void webdw()
        {
            //string sszzzz="";

            //若发现tag为P的段落，值则为true
            _isfind = false;

            int y = -2;
            Point pt;

            HtmlElement hel;

        start:

            pt = new Point(128, y);
            y = y + 8;
            if (y >= webBrowser1.Document.Window.Size.Height * 2)
                return;

            hel = webBrowser1.Document.GetElementFromPoint(pt);
            if (hel == null)
            {
                //sszzzz = sszzzz + "null" + y.ToString()+"\r\n";
                goto start;
            }
            else if (hel.TagName == "P")
            {
                sid = "111111" + (d).ToString();
                hel.Id = sid;
                _isfind = true;
                d++;

                //MessageBox.Show(webBrowser1.Document.GetElementById(sid).ScrollRectangle.Height.ToString() + "#" + webBrowser1.Document.GetElementById(sid).ScrollRectangle.Width.ToString());
                //MessageBox.Show(webBrowser1.Document.Body.ScrollTop.ToString() + "#" + webBrowser1.Document.GetElementById(sid).OffsetRectangle.Y.ToString());

                fCentValue = ((float)(webBrowser1.Document.GetElementById(sid).OffsetRectangle.Y - webBrowser1.Document.Body.ScrollTop)) / ((float)(webBrowser1.Document.GetElementById(sid).ScrollRectangle.Height));

                //sszzzz = sszzzz + "p" + y.ToString() + "\r\n";
            }
            else
            {
                //sszzzz = sszzzz + "f" + y.ToString() + "\r\n";
                goto start;
            }

            //MessageBox.Show(sszzzz.ToString());
        }

        //一个比例值，用于在窗口尺寸变化后，精确定位浏览器里显示内容的位置
        float fCentValue = 0.0f;

        //开始调整尺寸
        bool _isResizeBegin = false;

        private void frmpali_ResizeBegin(object sender, EventArgs e)
        {
            webdw();
            _isResizeBegin = true;
        }

        private void webBrowser1_SizeChanged(object sender, EventArgs e)
        {
            /*
            if (_isfind)
            {
                //webBrowser1.Document.GetElementById(sid).ScrollIntoView(true);
                webBrowser1.Document.Window.ScrollTo(webBrowser1.Document.Body.ScrollLeft, webBrowser1.Document.GetElementById(sid).OffsetRectangle.Y - (int)(fCentValue * (webBrowser1.Document.GetElementById(sid).ScrollRectangle.Height)));
                _isfind = false;
            }
            */
        }

        const int WM_SYSCOMMAND = 0x112;
        const int SC_CLOSE = 0xF060;
        const int SC_MINIMIZE = 0xF020;
        const int SC_MAXIMIZE = 0xF030;
        const int SC_RESTORE = 0xF120;
        const int WM_NCLBUTTONDBLCLK = 0x00A3;

        protected override void WndProc(ref Message m)
        {
            //if (zzcx == "d")
            //{
                //zzcx = "";
              //  return;
            //}


            //按了最大化或恢复按钮
            if (m.Msg == WM_SYSCOMMAND)
            {
                if (m.WParam.ToInt32() == SC_MAXIMIZE || m.WParam.ToInt32() == SC_RESTORE)
                {
                    webdw();
                }

                if (m.WParam.ToInt32() == SC_MINIMIZE)
                {
                    //this.Hide();
                    //this.WindowState = FormWindowState.Normal;
                    this.Hide();

                    //设置菜单条目颜色，以区分窗口当前状态
                    foreach (tsmiWin tsmiw in Program.toolbarform.tsddb.DropDownItems)
                    {
                        if (Convert.ToInt32(tsmiw.Tag) == this.Handle.ToInt32())
                        {
                            tsmiw.BackColor = Color.FromKnownColor(KnownColor.ControlDark);
                        }
                    }
                    return;
                }
            }

            //双击标题栏以最大化或恢复
            if (m.Msg == WM_NCLBUTTONDBLCLK)
            {
                webdw();
            }

            base.WndProc(ref m);
        }

        private void webBrowser1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            webBrowser1.WebBrowserShortcutsEnabled = true;

            if (e.Modifiers == Keys.Control)
            {
                if (e.KeyCode == Keys.A)
                {
                    webBrowser1.Document.ExecCommand("SelectAll", true, null);
                    _slAll = true;

                    webBrowser1.WebBrowserShortcutsEnabled = false;
                }
            }

            if (e.Modifiers == Keys.Control)
            {
                if (e.KeyCode == Keys.C)
                {
                    this.Cursor = Cursors.WaitCursor;
                    Form1._isPaliWindowCopy = true;

                    //下面执行的每个命令都会发生一次剪贴板事件共三次，每次都会激发自动复制取词函数DisplayClipboardData
                    Form1.fzbz = 1;
                    webBrowser1.Document.ExecCommand("Copy", true, null);
                    Form1.cbdText = Program.mainform.outword_t(Clipboard.GetText(TextDataFormat.UnicodeText));
                    if (Form1.cbdText != "") //因为金山词霸在本程序窗口里划词的时候会引发异常，故设此条件判断
                        Clipboard.SetText(Form1.cbdText, TextDataFormat.UnicodeText);

                    webBrowser1.WebBrowserShortcutsEnabled = false;

                    Form1._isPaliWindowCopy = false;
                    this.Cursor = Cursors.Default;
                }
            }
        }

        int numA = -1;
        int numB = -1;
        private void getnumAB(string paraNum)
        {
            string strnumA = "";
            string strnumB = "";

            MatchCollection mc = new Regex("-", RegexOptions.None).Matches(paraNum);
            if (mc.Count > 0)
            {


                MatchCollection mcA = new Regex(@"(?<w>\d+)-(?<w1>\d+)", RegexOptions.None).Matches(paraNum);
                foreach (Match ma in mcA)
                {
                    strnumA = ma.Groups["w"].Value;
                    numA = Convert.ToInt32(strnumA);

                    strnumB = ma.Groups["w1"].Value;
                    if (strnumA.Length > strnumB.Length)
                    {
                        strnumB = strnumA.Substring(0, strnumA.Length - strnumB.Length) + strnumB;

                        numB = Convert.ToInt32(strnumB);
                    }
                    else
                        numB = Convert.ToInt32(strnumB);
                }
            }
            else
            {
                numA = Convert.ToInt32(paraNum);
                numB = numA;
            }
        }

        private void gotopara()
        {
            string paraNum = "";
            int lastNum = -1;
            int paraid = -1;
            string aimparaNum = "";
            aimparaNum = tboxPage.Text;
            int n = -1;
            int z = 0;
            int lastn = -1;
            //记下上次查找段的方向，假如与本次不同，说明方向逆转了，因此也结束查找
            bool buplookup = false;
            n = getcurrpara();
            if (n>-1)
            {
                paraNum = ((HtmlElement)alparas[n]).Name.Substring(4);

            parast:
                getnumAB(paraNum);

                if (numA != numB)
                {

                    if (numA > Convert.ToInt32(aimparaNum))
                    {
                        if (z == 0)
                        {
                            z++;
                            buplookup = true;
                        }
                        else
                        {
                            if (!buplookup)
                                goto lookupend;
                        }
                        n--;
                        if (n >= 0)
                        {
                            paraNum = ((HtmlElement)alparas[n]).Name.Substring(4);
                            paraid = n;

                            lastNum = numA;
                            getnumAB(paraNum);

                            if (numB >= lastNum)
                            {
                                n++;
                                goto lookupend;
                            }

                            goto parast;
                        }
                    }
                    if (numB < Convert.ToInt32(aimparaNum))
                    {
                        if (z == 0)
                        {
                            z++;
                            buplookup = false;
                        }
                        else
                        {
                            if (buplookup)
                                goto lookupend;
                        }
                        n++;
                        if (n < iMaxpara)
                        {
                            paraNum = ((HtmlElement)alparas[n]).Name.Substring(4);
                            paraid = n;

                            lastNum = numB;
                            getnumAB(paraNum);

                            if (numA <= lastNum)
                            {
                                n--;
                                goto lookupend;
                            }

                            goto parast;
                        }
                    }
                }
                else
                {
                    if (numA > Convert.ToInt32(aimparaNum))
                    {
                        if (z == 0)
                        {
                            z++;
                            buplookup = true;
                        }
                        else
                        {
                            if (!buplookup)
                                goto lookupend;
                        }
                        //lastparaNum = paraNum;
                        //lastn = n;
                        n--;
                        if (n >= 0)
                        {
                            paraNum = ((HtmlElement)alparas[n]).Name.Substring(4);
                            paraid = n;

                            lastNum = numA;
                            getnumAB(paraNum);

                            if (numB >= lastNum)
                            {
                                n++;
                                goto lookupend;
                            }

                            goto parast;
                        }
                    }
                    if (numA < Convert.ToInt32(aimparaNum))
                    {
                        if (z == 0)
                        {
                            z++;
                            buplookup = false;
                        }
                        else
                        {
                            if (buplookup)
                                goto lookupend;
                        }
                        //lastparaNum = paraNum;
                        //lastn = n;
                        n++;
                        if (n < iMaxpara)
                        {
                            paraNum = ((HtmlElement)alparas[n]).Name.Substring(4);
                            paraid = n;

                            lastNum = numB;
                            getnumAB(paraNum);

                            if (numA <= lastNum)
                            {
                                n--;
                                goto lookupend;
                            }

                            goto parast;
                        }
                    }
                }
            lookupend:
                if (n < 0)
                    n = 0;
                if (n >= iMaxpara)
                    n = iMaxpara - 1;
                //((HtmlElement)alparas[n]).ScrollIntoView(true);
                webBrowser1.Document.Window.ScrollTo(0, ((HtmlElement)alparas[n]).OffsetRectangle.Y);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(webBrowser1.Document.GetElementById(sid).ScrollRectangle.Height.ToString() + "#" + webBrowser1.Document.GetElementById(sid).ScrollRectangle.Width.ToString());
            //MessageBox.Show(webBrowser1.Document.Body.ScrollTop.ToString() + "#" + webBrowser1.Document.GetElementById(sid).OffsetRectangle.Y.ToString());
            //ssHLword();
        }

        /// <summary>
        /// 取得当前段锚元素
        /// </summary>
        private int getcurrpara()
        {
            //MessageBox.Show(webBrowser1.Document.GetElementById(sid).ScrollRectangle.Height.ToString() + "#" + webBrowser1.Document.GetElementById(sid).ScrollRectangle.Width.ToString());
            //MessageBox.Show(webBrowser1.Document.Body.ScrollTop.ToString() + "#" + webBrowser1.Document.GetElementById(sid).OffsetRectangle.Y.ToString());
            //ssHLword();


            int highdelta = 0;
            int highwin = 0;

            if (iMaxpara == 0)
                return -1;

            try
            {
                //储存当前在浏览器窗口中中所看到的段
                HtmlElement hecurrpara;

                //储存当前在浏览器窗口中中所看到的段在alparas数组中的序号
                int currparaid = -1;

                float f1 = 0.0f;
                float f2 = 0.0f;
                float fcurr = 0.0f;

                f1 = webBrowser1.Document.Body.ScrollTop;
                f2 = webBrowser1.Document.Body.ScrollRectangle.Height;
                fcurr = f1 / f2 * ((float)iMaxpara);

                int pn = (int)fcurr;
                if (pn < 0)
                    pn = 0;
                if (pn >= iMaxpara)
                    pn = iMaxpara - 1;

                highwin = webBrowser1.Document.Window.Size.Height;

                hecurrpara = (HtmlElement)alparas[pn];
                currparaid = pn;

                highdelta = ((HtmlElement)alparas[pn]).OffsetRectangle.Y - this.webBrowser1.Document.Body.ScrollTop;

                if (highdelta < 0)
                {
                    //向页码增大方向查找
                    for (int p = pn + 1; p < alparas.Count; p++)
                    {
                        hecurrpara = (HtmlElement)alparas[p];
                        currparaid = p;

                        if (((HtmlElement)alparas[p]).OffsetRectangle.Y - this.webBrowser1.Document.Body.ScrollTop >= 0)
                        {
                            if (((HtmlElement)alparas[p]).OffsetRectangle.Y - this.webBrowser1.Document.Body.ScrollTop >= highwin)
                            {
                                if ((p - 1) < 0)
                                {
                                    hecurrpara = (HtmlElement)alparas[0];
                                    currparaid = 0;
                                }
                                else
                                {
                                    hecurrpara = (HtmlElement)alparas[p - 1];
                                    currparaid = p - 1;
                                }
                            }
                            else
                            {
                                hecurrpara = (HtmlElement)alparas[p];
                                currparaid = p;
                            }
                            break;
                        }
                    }
                }
                else if (highdelta >= highwin)
                {
                    //向页码减小方向查找
                    for (int p = pn - 1; p > -1; p--)
                    {
                        hecurrpara = (HtmlElement)alparas[p];
                        currparaid = p;

                        if (((HtmlElement)alparas[p]).OffsetRectangle.Y - this.webBrowser1.Document.Body.ScrollTop < highwin)
                        {
                            if (((HtmlElement)alparas[p]).OffsetRectangle.Y - this.webBrowser1.Document.Body.ScrollTop < 0)
                            {
                                if ((p + 1) >= iMaxpara)
                                {
                                    hecurrpara = (HtmlElement)alparas[iMaxpara - 1];
                                    currparaid = iMaxpara - 1;
                                }
                                else
                                {
                                    hecurrpara = (HtmlElement)alparas[p + 1];
                                    currparaid = p + 1;
                                }
                            }
                            else
                            {
                                hecurrpara = (HtmlElement)alparas[p];
                                currparaid = p;
                            }
                            break;
                        }
                    }
                }

                if (hecurrpara == null)
                {
                    return -1;
                }
                else
                {
                    //MessageBox.Show(hecurrpara.Name);
                    return currparaid;
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
                return -1;
            }
        }

        public HtmlElementCollection zzz;
        public ArrayList alVRIPages = new ArrayList();
        public ArrayList alMyanmarPages = new ArrayList();
        public ArrayList alPTSPages = new ArrayList();
        public ArrayList alThaiPages = new ArrayList();
        public int iVRIMaxPage = 0;
        public int iMyanmarMaxPage = 0;
        public int iPTSMaxPage = 0;
        public int iThaiMaxPage = 0;

        /// <summary>
        /// 储存本书htm中所有段号锚元素
        /// </summary>
        public ArrayList alparas = new ArrayList();
        public int iMaxpara = 0;

        /// <summary>
        /// 储存本书htm中所有nikaya锚元素
        /// </summary>
        public ArrayList alnikayas = new ArrayList();
        public int iMaxnikaya = 0;

        /// <summary>
        /// 储存本书htm中所有book锚元素
        /// </summary>
        public ArrayList albooks = new ArrayList();
        public int iMaxbook = 0;

        /// <summary>
        /// 储存本书htm中所有title锚元素
        /// </summary>
        public ArrayList altitles = new ArrayList();
        public int iMaxtitle = 0;

        /// <summary>
        /// 储存本书htm中所有chapter锚元素
        /// </summary>
        public ArrayList alchapters = new ArrayList();
        public int iMaxchapter = 0;

        /// <summary>
        /// 上一次滚动滚动条后的 ScrollTop 值，初始值设为 -5000 ，
        /// 这样基本能保证若某打开篇经文是第一页时，qddqpage函数不会因为滚动距离短而不返回页码
        /// </summary>
        public int lastSTop = -5000;

        string zzcx = "";
        //取得全部页码锚和段号锚元素
        public void qdall()
        {
            zzcx = "d";
            zzz = webBrowser1.Document.GetElementsByTagName("a");

            try
            {

                foreach (HtmlElement heA in zzz)
                {
                    if (heA.Name != "" && heA.Name.Substring(0, 1) == "V")
                    {
                        //ArrayList使用前必须用new初始化，否则运行会出错、而且不提示异常
                        alVRIPages.Add(heA);
                        iVRIMaxPage++;
                    }
                    if (heA.Name != "" && heA.Name.Substring(0, 1) == "M")
                    {
                        alMyanmarPages.Add(heA);
                        iMyanmarMaxPage++;
                    }
                    if (heA.Name != "" && heA.Name.Substring(0, 1) == "P")
                    {
                        alPTSPages.Add(heA);
                        iPTSMaxPage++;
                    }
                    if (heA.Name != "" && heA.Name.Substring(0, 1) == "T")
                    {
                        alThaiPages.Add(heA);
                        iThaiMaxPage++;
                    }
                    
                    try
                    {
                        //取得段号锚元素
                        if (heA.Name != "" && heA.Name.Length > 3)
                        {
                            if (heA.Name.Substring(0, 4) == "para")
                            {
                                MatchCollection mc = new Regex("_", RegexOptions.None).Matches(heA.Name);
                                if (mc.Count == 0)
                                {
                                    alparas.Add(heA);
                                    iMaxpara++;
                                }
                            }
                        }
                    }
                    catch (Exception eee)
                    {
                        MessageBox.Show(eee.ToString());
                    }


                    if ((iMaxnikaya < 50) && (heA.Name == "nikaya"))
                    {
                        alnikayas.Add(heA);

                        ToolStripMenuItem tsmiN = new ToolStripMenuItem(heA.Parent.InnerText);
                        tsmiN.Tag = iMaxnikaya;
                        tsDDbtnNikaya.DropDownItems.Add(tsmiN);
                        iMaxnikaya++;
                    }

                    if ((iMaxbook < 50) && (heA.Name == "book"))
                    {
                        albooks.Add(heA);

                        ToolStripMenuItem tsmiB = new ToolStripMenuItem(heA.Parent.InnerText);
                        tsmiB.Tag = iMaxbook;
                        tsDDbtnBook.DropDownItems.Add(tsmiB);
                        iMaxbook++;
                    }

                    if ((iMaxtitle < 50) && (heA.Name == "title"))
                    {
                        altitles.Add(heA);

                        ToolStripMenuItem tsmiT = new ToolStripMenuItem(heA.Parent.InnerText);
                        tsmiT.Tag = iMaxtitle;
                        tsDDbtnTitle.DropDownItems.Add(tsmiT);
                        iMaxtitle++;
                    }

                    if ((iMaxchapter < 50) && (heA.Name == "chapter"))
                    {
                        alchapters.Add(heA);

                        ToolStripMenuItem tsmiC = new ToolStripMenuItem(heA.Parent.InnerText);
                        tsmiC.Tag = iMaxchapter;
                        tsDDbtnChapter.DropDownItems.Add(tsmiC);
                        iMaxchapter++;
                    }
                    
                    if (tsDDbtnNikaya.HasDropDownItems)
                    {
                        tsDDbtnNikaya.Text = tsDDbtnNikaya.DropDownItems[0].Text;
                        ((ToolStripMenuItem)(tsDDbtnNikaya.DropDownItems[0])).Checked = true;
                    }
                    else
                    {
                        tsDDbtnNikaya.Text = "";
                        //tsDDbtnNikaya.Enabled = false;
                    }

                    if (tsDDbtnBook.HasDropDownItems)
                    {
                        tsDDbtnBook.Text = tsDDbtnBook.DropDownItems[0].Text;
                        ((ToolStripMenuItem)(tsDDbtnBook.DropDownItems[0])).Checked = true;
                    }
                    else
                    {
                        tsDDbtnBook.Text = "";
                        //tsDDbtnBook.Enabled = false;
                    }

                    if (tsDDbtnTitle.HasDropDownItems)
                    {
                        tsDDbtnTitle.Text = tsDDbtnTitle.DropDownItems[0].Text;
                        ((ToolStripMenuItem)(tsDDbtnTitle.DropDownItems[0])).Checked = true;
                    }
                    else
                    {
                        tsDDbtnTitle.Text = "";
                        //tsDDbtnTitle.Enabled = false;
                    }

                    if (tsDDbtnChapter.HasDropDownItems)
                    {
                        tsDDbtnChapter.Text = tsDDbtnChapter.DropDownItems[0].Text;
                        ((ToolStripMenuItem)(tsDDbtnChapter.DropDownItems[0])).Checked = true;
                    }
                    else
                    {
                        tsDDbtnChapter.Text = "";
                        //tsDDbtnChapter.Enabled = false;
                    }
                }
            }
            catch (Exception zcn)
            {
                MessageBox.Show(zcn.ToString());
            }



        }

        /// <summary>
        /// 取得当前页码
        /// </summary>
        public void qddqpage()
        {
            /*
            if (_isSizeC)
            {
                _isSizeC = false;
                return;
            }
            */

            //此注释句中iPTSMaxPage的使用有问题，如为0会出错
            //if (Math.Abs(webBrowser1.Document.Body.ScrollTop - lastSTop) < webBrowser1.Document.Body.ScrollRectangle.Height / iPTSMaxPage / 2)
            //if (Math.Abs(webBrowser1.Document.Body.ScrollTop - lastSTop) < 200)
            if (Math.Abs(webBrowser1.Document.Body.ScrollTop - lastSTop) < webBrowser1.Document.Window.Size.Height * 2 / 3)
                return;
            lastSTop = webBrowser1.Document.Body.ScrollTop;

            getPTSpage(iVRIMaxPage, alVRIPages, "V");
            getPTSpage(iMyanmarMaxPage, alMyanmarPages, "M");
            getPTSpage(iPTSMaxPage, alPTSPages, "P");
            getPTSpage(iThaiMaxPage, alThaiPages, "T");

            if (tsslVRIPage.Text + tsslMyanmarPage.Text + tsslPTSPage.Text + tsslThaiPage.Text == "")
                tsslPage.Text = "";
            else
                tsslPage.Text = "PAGE:";

            int highdelta = 0;
            int highwin = 0;

            highwin = webBrowser1.Document.Window.Size.Height;

            foreach (ToolStripMenuItem tsmiZ in tsDDbtnChapter.DropDownItems)
            {
                tsmiZ.Checked = false;
            }

            for (int h = 0; h < iMaxchapter; h++)
            {

                highdelta = ((HtmlElement)alchapters[h]).OffsetRectangle.Y - this.webBrowser1.Document.Body.ScrollTop - highwin / 2;
                if (highdelta >= 0)
                {
                    if (h > 0)
                    {
                        tsDDbtnChapter.Text = tsDDbtnChapter.DropDownItems[h - 1].Text;
                        ((ToolStripMenuItem)(tsDDbtnChapter.DropDownItems[h - 1])).Checked = true;
                    }
                    else
                    {
                        tsDDbtnChapter.Text = tsDDbtnChapter.DropDownItems[h].Text;
                        ((ToolStripMenuItem)(tsDDbtnChapter.DropDownItems[h])).Checked = true;
                    }
                    break;
                }
                if ((highdelta < 0) && (h == iMaxchapter - 1))
                {
                    tsDDbtnChapter.Text = tsDDbtnChapter.DropDownItems[h].Text;
                    ((ToolStripMenuItem)(tsDDbtnChapter.DropDownItems[h])).Checked = true;
                }
            }

            foreach (ToolStripMenuItem tsmiZ in tsDDbtnBook.DropDownItems)
            {
                tsmiZ.Checked = false;
            }

            for (int h = 0; h < iMaxbook; h++)
            {

                highdelta = ((HtmlElement)albooks[h]).OffsetRectangle.Y - this.webBrowser1.Document.Body.ScrollTop - highwin / 2;
                if (highdelta >= 0)
                {
                    if (h > 0)
                    {
                        tsDDbtnBook.Text = tsDDbtnBook.DropDownItems[h - 1].Text;
                        ((ToolStripMenuItem)(tsDDbtnBook.DropDownItems[h - 1])).Checked = true;
                    }
                    else
                    {
                        tsDDbtnBook.Text = tsDDbtnBook.DropDownItems[h].Text;
                        ((ToolStripMenuItem)(tsDDbtnBook.DropDownItems[h])).Checked = true;
                    }
                    break;
                }
                if ((highdelta < 0) && (h == iMaxbook - 1))
                {
                    tsDDbtnBook.Text = tsDDbtnBook.DropDownItems[h].Text;
                    ((ToolStripMenuItem)(tsDDbtnBook.DropDownItems[h])).Checked = true;
                }
            }

            foreach (ToolStripMenuItem tsmiZ in tsDDbtnTitle.DropDownItems)
            {
                tsmiZ.Checked = false;
            }

            for (int h = 0; h < iMaxtitle; h++)
            {

                highdelta = ((HtmlElement)altitles[h]).OffsetRectangle.Y - this.webBrowser1.Document.Body.ScrollTop - highwin / 2;
                if (highdelta >= 0)
                {
                    if (h > 0)
                    {
                        tsDDbtnTitle.Text = tsDDbtnTitle.DropDownItems[h - 1].Text;
                        ((ToolStripMenuItem)(tsDDbtnTitle.DropDownItems[h - 1])).Checked = true;
                    }
                    else
                    {
                        tsDDbtnTitle.Text = tsDDbtnTitle.DropDownItems[h].Text;
                        ((ToolStripMenuItem)(tsDDbtnTitle.DropDownItems[h])).Checked = true;
                    }
                    break;
                }
                if ((highdelta < 0) && (h == iMaxtitle - 1))
                {
                    tsDDbtnTitle.Text = tsDDbtnTitle.DropDownItems[h].Text;
                    ((ToolStripMenuItem)(tsDDbtnTitle.DropDownItems[h])).Checked = true;
                }
            }

            foreach (ToolStripMenuItem tsmiZ in tsDDbtnNikaya.DropDownItems)
            {
                tsmiZ.Checked = false;
            }

            for (int h = 0; h < iMaxnikaya; h++)
            {

                highdelta = ((HtmlElement)alnikayas[h]).OffsetRectangle.Y - this.webBrowser1.Document.Body.ScrollTop - highwin / 2;
                if (highdelta >= 0)
                {
                    if (h > 0)
                    {
                        tsDDbtnNikaya.Text = tsDDbtnNikaya.DropDownItems[h - 1].Text;
                        ((ToolStripMenuItem)(tsDDbtnNikaya.DropDownItems[h - 1])).Checked = true;
                    }
                    else
                    {
                        tsDDbtnNikaya.Text = tsDDbtnNikaya.DropDownItems[h].Text;
                        ((ToolStripMenuItem)(tsDDbtnNikaya.DropDownItems[h])).Checked = true;
                    }
                    break;
                }
                if ((highdelta < 0) && (h == iMaxnikaya - 1))
                {
                    tsDDbtnNikaya.Text = tsDDbtnNikaya.DropDownItems[h].Text;
                    ((ToolStripMenuItem)(tsDDbtnNikaya.DropDownItems[h])).Checked = true;
                }
            }
        }

        /*
        public void getPTSpage(int iPTSMaxPage, ArrayList alPTSPages, string tsslName)
        {
            if (iPTSMaxPage == 0)
                return;

            try
            {
                string strcurrpage = "";
                float f1 = 0.0f;
                float f2 = 0.0f;
                float fcurr = 0.0f;

                f1 = webBrowser1.Document.Body.ScrollTop;
                f2 = webBrowser1.Document.Body.ScrollRectangle.Height;
                fcurr = f1 / f2 * iPTSMaxPage;

                int pn = (int)fcurr;

                strcurrpage = ((HtmlElement)alPTSPages[pn]).Name;

                if (((HtmlElement)alPTSPages[pn]).Parent.OffsetRectangle.Y + ((HtmlElement)alPTSPages[pn]).OffsetRectangle.Y - this.webBrowser1.Document.Body.ScrollTop < 0)
                {
                    //向页码增大方向查找
                    for (int p = pn + 1; p < alPTSPages.Count; p++)
                    {
                        strcurrpage = ((HtmlElement)alPTSPages[p]).Name;
                        if (((HtmlElement)alPTSPages[p]).Parent.OffsetRectangle.Y + ((HtmlElement)alPTSPages[p]).OffsetRectangle.Y - this.webBrowser1.Document.Body.ScrollTop >= 0)
                        {
                            if (((HtmlElement)alPTSPages[p]).Parent.OffsetRectangle.Y + ((HtmlElement)alPTSPages[p]).OffsetRectangle.Y - this.webBrowser1.Document.Body.ScrollTop >= webBrowser1.Document.Window.Size.Height)
                                strcurrpage = ((HtmlElement)alPTSPages[p - 1]).Name;
                            strcurrpage = ((HtmlElement)alPTSPages[p]).Name;
                            break;
                        }
                    }
                }
                else if (((HtmlElement)alPTSPages[pn]).Parent.OffsetRectangle.Y + ((HtmlElement)alPTSPages[pn]).OffsetRectangle.Y - this.webBrowser1.Document.Body.ScrollTop >= webBrowser1.Document.Window.Size.Height)
                {
                    //向页码减小方向查找
                    for (int p = pn - 1; p > 0; p--)
                    {
                        strcurrpage = ((HtmlElement)alPTSPages[p]).Name;
                        if (((HtmlElement)alPTSPages[p]).Parent.OffsetRectangle.Y + ((HtmlElement)alPTSPages[p]).OffsetRectangle.Y - this.webBrowser1.Document.Body.ScrollTop < webBrowser1.Document.Window.Size.Height)
                        {
                            if (((HtmlElement)alPTSPages[p]).Parent.OffsetRectangle.Y + ((HtmlElement)alPTSPages[p]).OffsetRectangle.Y - this.webBrowser1.Document.Body.ScrollTop < 0)
                                strcurrpage = ((HtmlElement)alPTSPages[p]).Name;
                            strcurrpage = ((HtmlElement)alPTSPages[p]).Name;
                            break;
                        }
                    }
                }

                if (strcurrpage == "")
                {
                    if (tsslName == "V")
                        tsslVRIPage.Text = "VRI *";
                    if (tsslName == "M")
                        tsslMyanmarPage.Text = "Myanmar *";
                    if (tsslName == "P")
                        tsslPTSPage.Text = "PTS *";
                    if (tsslName == "T")
                        tsslThaiPage.Text = "Thai *";
                }
                else
                {
                    if (tsslName == "V")
                        tsslVRIPage.Text = "VRI " + strcurrpage.Substring(1, 1) + "." + strcurrpage.Substring(3).TrimStart('0');
                    if (tsslName == "M")
                        tsslMyanmarPage.Text = "Myanmar " + strcurrpage.Substring(1, 1) + "." + strcurrpage.Substring(3).TrimStart('0');
                    if (tsslName == "P")
                        tsslPTSPage.Text = "PTS " + strcurrpage.Substring(1, 1) + "." + strcurrpage.Substring(3).TrimStart('0');
                    if (tsslName == "T")
                        tsslThaiPage.Text = "Thai " + strcurrpage.Substring(1, 1) + "." + strcurrpage.Substring(3).TrimStart('0');
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }*/
        
        public void getPTSpage(int iPTSMaxPage, ArrayList alPTSPages, string tsslName)
        {
            int highdelta = 0;
            int highwin = 0;

            if (iPTSMaxPage == 0)
                return;

            try
            {
                string strcurrpage = "";
                float f1 = 0.0f;
                float f2 = 0.0f;
                float fcurr = 0.0f;

                f1 = webBrowser1.Document.Body.ScrollTop;
                f2 = webBrowser1.Document.Body.ScrollRectangle.Height;
                fcurr = f1 / f2 * ((float)iPTSMaxPage);

                int pn = (int)fcurr;
                if (pn < 0)
                    pn = 0;
                if (pn >= iPTSMaxPage)
                    pn = iPTSMaxPage - 1;

                highwin = webBrowser1.Document.Window.Size.Height;

                strcurrpage = ((HtmlElement)alPTSPages[pn]).Name;

                highdelta = ((HtmlElement)alPTSPages[pn]).OffsetRectangle.Y - this.webBrowser1.Document.Body.ScrollTop;

                if ( highdelta< 0)
                {
                    //向页码增大方向查找
                    for (int p = pn + 1; p < alPTSPages.Count; p++)
                    {
                        strcurrpage = ((HtmlElement)alPTSPages[p]).Name;
                        if (((HtmlElement)alPTSPages[p]).OffsetRectangle.Y - this.webBrowser1.Document.Body.ScrollTop >= 0)
                        {
                            if (((HtmlElement)alPTSPages[p]).OffsetRectangle.Y - this.webBrowser1.Document.Body.ScrollTop >= highwin)
                            {
                                if ((p - 1) < 0)
                                    strcurrpage = ((HtmlElement)alPTSPages[0]).Name;
                                else
                                    strcurrpage = ((HtmlElement)alPTSPages[p - 1]).Name;
                            }
                            else
                                strcurrpage = ((HtmlElement)alPTSPages[p]).Name;
                            break;
                        }
                    }
                }
                else if (highdelta >= highwin)
                {
                    //向页码减小方向查找
                    for (int p = pn - 1; p > -1; p--)
                    {
                        strcurrpage = ((HtmlElement)alPTSPages[p]).Name;
                        if (((HtmlElement)alPTSPages[p]).OffsetRectangle.Y - this.webBrowser1.Document.Body.ScrollTop < highwin)
                        {
                            if (((HtmlElement)alPTSPages[p]).OffsetRectangle.Y - this.webBrowser1.Document.Body.ScrollTop < 0)
                            {
                                if ((p + 1) >= iPTSMaxPage)
                                    strcurrpage = ((HtmlElement)alPTSPages[iPTSMaxPage - 1]).Name;
                                else
                                    strcurrpage = ((HtmlElement)alPTSPages[p + 1]).Name;
                            }
                            else
                                strcurrpage = ((HtmlElement)alPTSPages[p]).Name;
                            break;
                        }
                    }
                }

                if (strcurrpage == "")
                {
                    if (tsslName == "V")
                        tsslVRIPage.Text = "VRI *";
                    if (tsslName == "M")
                        tsslMyanmarPage.Text = "Myanmar *";
                    if (tsslName == "P")
                        tsslPTSPage.Text = "PTS *";
                    if (tsslName == "T")
                        tsslThaiPage.Text = "Thai *";
                }
                else
                {
                    if (tsslName == "V")
                        tsslVRIPage.Text = "VRI " + strcurrpage.Substring(1, 1) + "." + strcurrpage.Substring(3).TrimStart('0');
                    if (tsslName == "M")
                        tsslMyanmarPage.Text = "Myanmar " + strcurrpage.Substring(1, 1) + "." + strcurrpage.Substring(3).TrimStart('0');
                    if (tsslName == "P")
                        tsslPTSPage.Text = "PTS " + strcurrpage.Substring(1, 1) + "." + strcurrpage.Substring(3).TrimStart('0');
                    if (tsslName == "T")
                        tsslThaiPage.Text = "Thai " + strcurrpage.Substring(1, 1) + "." + strcurrpage.Substring(3).TrimStart('0');
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        public void htmonload()
        {
            //此句应放在下面ScrollIntoView这句之前，否则当打开某篇读经窗口时，ScrollIntoView引发页面滚动事件，
            //引发qddqpage函数执行时，若qdall尚未执行取得a标签元素集合，会导致qddqpage函数异常
            qdall();

            qddqpage();

            ((frmpali)(frmpali.FromHandle((IntPtr)(Form1.frmpalihandle)))).webBrowser1.Document.GetElementById(((frmmulu)(frmmulu.FromHandle((IntPtr)(Form1.frmmuluhandle)))).tnname).ScrollIntoView(true);

            ((frmmulu)(frmmulu.FromHandle((IntPtr)(Form1.frmmuluhandle)))).Cursor = System.Windows.Forms.Cursors.Default;
        }

        public void htmonload1()
        {
            qdall();

            qddqpage();

            ((frmmulu)(frmmulu.FromHandle((IntPtr)(Form1.frmmuluhandle)))).Cursor = System.Windows.Forms.Cursors.Default;
        }

        public void htmonload2()
        {
            this.Cursor = Cursors.WaitCursor;

            qdall();

            //qddqpage();

            //"#ECECEC"银色
            CzWord(sKeyWord, "#008080");

            ssHLword();

            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// 从搜索窗口中点击链接，打开读经窗口，定位到段时，用此函数加亮段中的关键词
        /// </summary>
        public void ssHLword()
        {
            string strHLword = new Regex(@",|\.", RegexOptions.None).Replace(tstboxWord.Text, " ").Trim();
            strHLword = new Regex(@"\s+", RegexOptions.None).Replace(strHLword, @"\b|\b");

            for (int z = icurHit; z < i + 1; z++)
            {
                string ssstitle = webBrowser1.Document.GetElementById("hit" + iczNum.ToString() + "c" + z.ToString()).InnerHtml.Trim();

                string strZZZZ = ssstitle;
                if (ssstitle.Substring(0, 1) != "<")
                    strZZZZ = ">" + strZZZZ;
                if (ssstitle.Substring(ssstitle.Length - 1, 1) != ">")
                    strZZZZ = strZZZZ + "<";

                ArrayList alA = new ArrayList();

                MatchCollection mcA = new Regex(@"(?<w><[^<>]*?>)", RegexOptions.None).Matches(strZZZZ);
                foreach (Match ma in mcA)
                {
                    alA.Add(ma.Groups["w"].Value);
                }

                string strTTTT = "";

                int t = 0;

                MatchCollection mcB = new Regex(@">(?<w>[^<>]*?)<", RegexOptions.None).Matches(strZZZZ);
                foreach (Match ma in mcB)
                {
                    string strB = new Regex(@"(?<w>\b" + strHLword + @"\b)", RegexOptions.IgnoreCase).Replace(ma.Groups["w"].Value, "<span style='BACKGROUND:#ffff66;'>${w}</span>");

                    if (ssstitle.Substring(0, 1) == "<")
                    {
                        strTTTT = strTTTT + alA[t] + strB;
                    }
                    else
                    {
                        if (t < alA.Count)
                        {
                            strTTTT = strTTTT + strB + alA[t];
                        }
                        else
                        {
                            strTTTT = strTTTT + strB;
                        }
                    }
                    t++;
                }

                if (alA.Count > mcB.Count)
                {
                    strTTTT = strTTTT + alA[t];
                }
                webBrowser1.Document.GetElementById("hit" + iczNum.ToString() + "c" + z.ToString()).InnerHtml = strTTTT;
            }
        }

        /// <summary>
        /// 值为true表示在读经窗口中进行了全选，缺省值为false
        /// </summary>
        public static bool _slAll = false;

        public void htmonmouseup()
        {
            this.Cursor = Cursors.WaitCursor;

            if (Program.toolbarform.cboxHccc.Checked && !_slAll)
            {
                Program.mainform.cboxInput.Text = "";

                Form1._ishtmonmouseup = true;

                Form1.currfrmpaliWindowHandle = ActiveForm.Handle.ToInt32();

                Form1._isPaliWeb = true;

                //webBrowser1.Document.ExecCommand("Copy", true, null);
                webBrowser1.Document.InvokeScript("getsltxtlen");
                string sssf = webBrowser1.Document.GetElementById("sltxtlen1207").Name;
                if (Convert.ToInt32(sssf) < 280)
                {
                    webBrowser1.Document.InvokeScript("getsltxt");
                    sssf = webBrowser1.Document.GetElementById("sltxt1207").Name.Trim();
                    string tttf = "";
                    tttf = htminword(sssf);
                    if (tttf == sssf)
                    {
                        Program.mainform.cboxInput.Text = tttf;
                        Program.mainform.cboxInput.Select();

                        Program.mainform.LookupWord();
                    }
                }

                Form1._isPaliWeb = false;
            }

            _slAll = false;

            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// 排除连字符与66个巴利罗马字母之外的字符 排除词间的空格
        /// </summary>
        public static string htminword(string aword)
        {
            char[] charPLABC = { '\'', '-', 'A', 'a', 'Ā', 'ā', 'I', 'i', 'Ī', 'ī', 'U', 'u', 'Ū', 'ū', 'E', 'e', 'O', 'o', 'K', 'k', 'G', 'g', 'C', 'c', 'J', 'j', 'Ñ', 'ñ', 'Ṭ', 'ṭ', 'Ḍ', 'ḍ', 'T', 't', 'D', 'd', 'N', 'n', 'P', 'p', 'B', 'b', 'M', 'm', 'Y', 'y', 'R', 'r', 'L', 'l', 'V', 'v', 'S', 's', 'H', 'h', 'Ṅ', 'ṅ', 'Ṇ', 'ṇ', 'Ḷ', 'ḷ', 'Ṃ', 'ṃ', 'Ṁ', 'ṁ', 'Ŋ', 'ŋ' };
            string saword = "";
            for (int z = 0; z < aword.Length; z++)
            {
                foreach (char cZ in charPLABC)
                {
                    if (cZ.ToString() == aword.Substring(z, 1))
                        saword = saword + aword.Substring(z, 1);
                }
            }
            return saword;
        }

        int ihl = 0;

        private void cwWeb2highLight_Click(object sender, EventArgs e)
        {
            webBrowser1.Document.ExecCommand("BackColor", true, "#0000FF");
            webBrowser1.Document.ExecCommand("ForeColor", true, "#ffffFF");
            webBrowser1.Document.ExecCommand("unselect", true, null);
            ihl++;
        }

        private void cwWeb2cancelHighLight_Click(object sender, EventArgs e)
        {
            webBrowser1.Document.ExecCommand("BackColor", true, "#FFFFFF");
            webBrowser1.Document.ExecCommand("ForeColor", true, "#000000");
            webBrowser1.Document.ExecCommand("unselect", true, null);
            ihl++;
        }

        private void cwWeb2cancelAllBlue_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < ihl*3; i++)
            {
                webBrowser1.Document.ExecCommand("undo", true, null);
            }
            webBrowser1.Document.ExecCommand("unselect", true, null);
            ihl = 0;
        }

        private void tstboxWord_KeyDown(object sender, KeyEventArgs e)
        {
            //快捷键 全选 复制 粘贴 剪切
            if (e.Modifiers == Keys.Control)
            {
                if (e.KeyCode == Keys.A)
                {
                    tstboxWord.SelectAll();
                }

                if (e.KeyCode == Keys.C)
                {
                    Form1.fzbz = -1;
                    Form1.cbdText = Program.mainform.outword_t(tstboxWord.SelectedText);
                    if (Form1.cbdText != "") //因为金山词霸在本程序窗口里划词的时候会引发异常，故设此条件判断
                        Clipboard.SetText(Form1.cbdText, TextDataFormat.UnicodeText);
                }

                if (e.KeyCode == Keys.V)
                {
                    int j = tstboxWord.SelectionStart;
                    //此句删除选中的文本
                    tstboxWord.Text = tstboxWord.Text.Substring(0, j) + tstboxWord.Text.Substring(j + tstboxWord.SelectedText.Length);
                    string scb = Program.mainform.inword_t(Clipboard.GetText(TextDataFormat.UnicodeText));
                    int igh = scb.Length;
                    tstboxWord.Text = tstboxWord.Text.Insert(j, scb);
                    tstboxWord.Select(j + igh, 0);
                    tstboxWord.Select();
                }

                if (e.KeyCode == Keys.X)
                {
                    Form1.fzbz = -1;
                    Form1.cbdText = Program.mainform.outword_t(tstboxWord.SelectedText);
                    if (Form1.cbdText != "") //因为金山词霸在本程序窗口里划词的时候会引发异常，故设此条件判断
                        Clipboard.SetText(Form1.cbdText, TextDataFormat.UnicodeText);

                    int j = tstboxWord.SelectionStart;
                    tstboxWord.Text = tstboxWord.Text.Remove(tstboxWord.SelectionStart, tstboxWord.SelectionLength);
                    tstboxWord.Select(j, 0);
                    tstboxWord.Select();
                }

                e.SuppressKeyPress = true;
            }

            //快捷键 粘贴
            if (e.Modifiers == Keys.Shift)
            {
                if (e.KeyCode == Keys.Insert)
                {
                    int j = tstboxWord.SelectionStart;
                    //此句删除选中的文本，倘若要把‘粘贴’功能改为‘插入’功能，只需注释掉此句即可
                    tstboxWord.Text = tstboxWord.Text.Substring(0, j) + tstboxWord.Text.Substring(j + tstboxWord.SelectedText.Length);
                    string scb = Program.mainform.inword_t(Clipboard.GetText(TextDataFormat.UnicodeText));
                    int igh = scb.Length;
                    tstboxWord.Text = tstboxWord.Text.Insert(j, scb);
                    tstboxWord.Select(j + igh, 0);
                    tstboxWord.Select();

                    e.SuppressKeyPress = true;
                }
            }
        }

        private void tstboxWord_KeyPress(object sender, KeyPressEventArgs e)
        {
            //转换键入的 VriRomanPali 字体编码字母为 Tahoma 字体编码字母
            if (Program.mainform.rbtnVriRomanPali.Checked)
                switch (Convert.ToUInt16(e.KeyChar))
                {
                    case 190:
                        e.KeyChar = 'Ā';
                        break;
                    case 177:
                        e.KeyChar = 'ā';
                        break;
                    case 191:
                        e.KeyChar = 'Ī';
                        break;
                    case 178:
                        e.KeyChar = 'ī';
                        break;
                    case 208:
                        e.KeyChar = 'Ū';
                        break;
                    case 179:
                        e.KeyChar = 'ū';
                        break;
                    case 169:
                        e.KeyChar = 'Ṅ';
                        break;
                    case 170:
                        e.KeyChar = 'ṅ';
                        break;
                    case 221:
                        e.KeyChar = 'Ṭ';
                        break;
                    case 956:
                        e.KeyChar = 'ṭ';
                        break;
                    case 181:
                        e.KeyChar = 'ṭ';
                        break;
                    case 222:
                        e.KeyChar = 'Ḍ';
                        break;
                    case 185:
                        e.KeyChar = 'ḍ';
                        break;
                    case 240:
                        e.KeyChar = 'Ṇ';
                        break;
                    case 186:
                        e.KeyChar = 'ṇ';
                        break;
                    case 253:
                        e.KeyChar = 'Ḷ';
                        break;
                    case 188:
                        e.KeyChar = 'ḷ';
                        break;
                    case 254:
                        e.KeyChar = 'Ṃ';
                        break;
                    case 189:
                        e.KeyChar = 'ṃ';
                        break;
                    default:
                        ;
                        break;
                }

            //转换键入的 Sangayana 字体编码字母为 Tahoma 字体编码字母
            if (Program.mainform.rbtnSangayana.Checked)
                switch (Convert.ToUInt16(e.KeyChar))
                {
                    case 226:
                        e.KeyChar = 'Ā';
                        break;
                    case 224:
                        e.KeyChar = 'ā';
                        break;
                    case 228:
                        e.KeyChar = 'Ī';
                        break;
                    case 227:
                        e.KeyChar = 'ī';
                        break;
                    case 230:
                        e.KeyChar = 'Ū';
                        break;
                    case 229:
                        e.KeyChar = 'ū';
                        break;
                    case 240:
                        e.KeyChar = 'Ṅ';
                        break;
                    case 239:
                        e.KeyChar = 'ṅ';
                        break;
                    case 165:
                        e.KeyChar = 'Ñ';
                        break;
                    case 164:
                        e.KeyChar = 'ñ';
                        break;
                    case 242:
                        e.KeyChar = 'Ṭ';
                        break;
                    case 241:
                        e.KeyChar = 'ṭ';
                        break;
                    case 244:
                        e.KeyChar = 'Ḍ';
                        break;
                    case 243:
                        e.KeyChar = 'ḍ';
                        break;
                    case 246:
                        e.KeyChar = 'Ṇ';
                        break;
                    case 245:
                        e.KeyChar = 'ṇ';
                        break;
                    case 236:
                        e.KeyChar = 'Ḷ';
                        break;
                    case 235:
                        e.KeyChar = 'ḷ';
                        break;
                    case 253:
                        e.KeyChar = 'Ṃ';
                        break;
                    case 252:
                        e.KeyChar = 'ṃ';
                        break;
                    case 167:
                        e.KeyChar = 'ṃ';
                        break;
                    default:
                        ;
                        break;
                }

            if (e.KeyChar == System.Convert.ToChar(13))
            {
                this.Cursor = Cursors.WaitCursor;

                sKeyWord = tstboxWord.Text.Trim();
                CzWord(sKeyWord, "");

                e.Handled = true;

                this.Cursor = Cursors.Default;
            }
        }

        private void cmtselectAll_Click(object sender, EventArgs e)
        {
            tstboxWord.SelectAll();
        }

        private void cmtcopy_Click(object sender, EventArgs e)
        {
            Form1.fzbz = -1;
            Form1.cbdText = Program.mainform.outword_t(tstboxWord.SelectedText);
            if (Form1.cbdText != "") //因为金山词霸在本程序窗口里划词的时候会引发异常，故设此条件判断
                Clipboard.SetText(Form1.cbdText, TextDataFormat.UnicodeText);
        }

        private void cmtpaste_Click(object sender, EventArgs e)
        {
            int j = tstboxWord.SelectionStart;
            //此句删除选中的文本
            tstboxWord.Text = tstboxWord.Text.Substring(0, j) + tstboxWord.Text.Substring(j + tstboxWord.SelectedText.Length);
            string scb = Program.mainform.inword_t(Clipboard.GetText(TextDataFormat.UnicodeText));
            int igh = scb.Length;
            tstboxWord.Text = tstboxWord.Text.Insert(j, scb);
            tstboxWord.Select(j + igh, 0);
            tstboxWord.Select();
        }

        private void cmtcut_Click(object sender, EventArgs e)
        {
            Form1.fzbz = -1;
            Form1.cbdText = Program.mainform.outword_t(tstboxWord.SelectedText);
            if (Form1.cbdText != "") //因为金山词霸在本程序窗口里划词的时候会引发异常，故设此条件判断
                Clipboard.SetText(Form1.cbdText, TextDataFormat.UnicodeText);

            int j = tstboxWord.SelectionStart;
            tstboxWord.Text = tstboxWord.Text.Remove(tstboxWord.SelectionStart, tstboxWord.SelectionLength);
            tstboxWord.Select(j, 0);
            tstboxWord.Select();
        }

        public int icurHit = 0;

        private void btnCz_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            sKeyWord = tstboxWord.Text.Trim();
            CzWord(sKeyWord, "");

            this.Cursor = Cursors.Default;
        }

        public string sKeyWord = "";

        //表示是第几次调用HighLight函数，将之传递给HighLight函数，以标记hit
        int iczNum = 0;

        private void CzWord(string strCzWord, string bgColor)
        {
            if (strCzWord == "")
                return;

            iczNum++;

            if (bgColor == "")
            {
                switch (iczNum % 10)
                {
                    case 1:
                        bgColor = "#FFFF66";
                        break;
                    case 2:
                        bgColor = "#00FF00";
                        break;
                    case 3:
                        bgColor = "#DAA520";
                        break;
                    case 4:
                        bgColor = "#ADFF2F";
                        break;
                    case 5:
                        bgColor = "#FF4500";
                        break;
                    case 6:
                        bgColor = "#98FB98";
                        break;
                    case 7:
                        bgColor = "#DDA0DD";
                        break;
                    case 8:
                        bgColor = "#C0C0C0";
                        break;
                    case 9:
                        bgColor = "#F5DEB3";
                        break;
                    default:
                        bgColor = "#7FFFD4";
                        break;
                }
                //bgColor = "#FFFF66";
            }

            //此函数引致查找速度变得特慢，须修改，已修改
            //改为在js脚本HighLight函数中hit后加nc来标记，n为数字，表示是第几次调用HighLight函数
            //这样在本窗口中按前后键定位查找到的词时，可以区分当前的hit标记，根据nc来区分
            clearHighLight();

            webBrowser1.Document.InvokeScript("HighLight", new String[] { strCzWord, iczNum.ToString(), bgColor });
            if (webBrowser1.Document.GetElementById("hitsnum1207").Name != "")
            {
                i = Convert.ToInt32(webBrowser1.Document.GetElementById("hitsnum1207").Name);
            }

            //记录词频
            tslHitsNum.Text = (i + 1).ToString();
            icurHit = 0;
            if ((i + 1) > 0)
            {
                btnFirst.Enabled = true;
                btnPrior.Enabled = true;
                btnNext.Enabled = true;
                btnLast.Enabled = true;

                webBrowser1.Document.Window.ScrollTo(webBrowser1.Document.Body.ScrollLeft, webBrowser1.Document.GetElementById("hit"+iczNum.ToString()+"c0").OffsetRectangle.Y - 54);

                webBrowser1.Document.ExecCommand("Unselect", true, null);
            }
            else
            {
                btnFirst.Enabled = false;
                btnPrior.Enabled = false;
                btnNext.Enabled = false;
                btnLast.Enabled = false;
            }
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            icurHit = 0;
            try
            {
                //webBrowser1.Document.GetElementById("hit" + icurHit.ToString()).ScrollIntoView(true);
                //定位至关键词后，下移三行
                //webBrowser1.Document.Window.ScrollTo(webBrowser1.Document.Body.ScrollLeft, webBrowser1.Document.Body.ScrollTop - 54);

                webBrowser1.Document.Window.ScrollTo(webBrowser1.Document.Body.ScrollLeft, webBrowser1.Document.GetElementById("hit" +iczNum.ToString()+"c"+ icurHit.ToString()).OffsetRectangle.Y - 54);
            }
            catch
            {
            }
        }

        private void btnPrior_Click(object sender, EventArgs e)
        {
            if (icurHit < 1)
                icurHit = 0;
            else
                icurHit = icurHit - 1;

            try
            {
                webBrowser1.Document.Window.ScrollTo(webBrowser1.Document.Body.ScrollLeft, webBrowser1.Document.GetElementById("hit" + iczNum.ToString() + "c" + icurHit.ToString()).OffsetRectangle.Y - 54);
            }
            catch
            {
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (icurHit >= i)
                icurHit = i;
            else
                icurHit = icurHit + 1;
            try
            {
                webBrowser1.Document.Window.ScrollTo(webBrowser1.Document.Body.ScrollLeft, webBrowser1.Document.GetElementById("hit" + iczNum.ToString() + "c" + icurHit.ToString()).OffsetRectangle.Y - 54);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            icurHit = i;
            try
            {
                webBrowser1.Document.Window.ScrollTo(webBrowser1.Document.Body.ScrollLeft, webBrowser1.Document.GetElementById("hit" + iczNum.ToString() + "c" + icurHit.ToString()).OffsetRectangle.Y - 54);
            }
            catch
            {
            }
        }

        private void btntoPage_Click(object sender, EventArgs e)
        {
            if (tboxPage.Text.Trim() != "")
            {
                string pageBz = "";

                if (cboxPage.SelectedIndex == 0)
                    pageBz = "V";
                if (cboxPage.SelectedIndex == 1)
                    pageBz = "M";
                if (cboxPage.SelectedIndex == 2)
                    pageBz = "P";
                if (cboxPage.SelectedIndex == 3)
                    pageBz = "T";
                if (cboxPage.SelectedIndex == -1)
                    return;

                string s = "";

                if (cboxPage.SelectedIndex == 4)
                    gotopara();
                //s = "para" + tboxPage.Text.Trim();
                else
                {
                    s = pageBz + tboxPage.Text.Trim().Substring(0, 2) + tboxPage.Text.Trim().Substring(2).PadLeft(4, '0');
                    webBrowser1.Document.InvokeScript("dingwei", new String[] { s });
                }
            }
        }

        private void tboxPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == System.Convert.ToChar(13))
            {
                if (tboxPage.Text.Trim() != "")
                {
                    string pageBz="";

                    if (cboxPage.SelectedIndex == 0)
                        pageBz = "V";
                    if (cboxPage.SelectedIndex == 1)
                        pageBz = "M";
                    if (cboxPage.SelectedIndex == 2)
                        pageBz = "P";
                    if (cboxPage.SelectedIndex == 3)
                        pageBz = "T";
                    if (cboxPage.SelectedIndex == -1)
                    {
                        e.Handled = true;
                        return;
                    }

                    string s = "";

                    if (cboxPage.SelectedIndex == 4)
                        gotopara();
                    //s = "para" + tboxPage.Text.Trim();
                    else
                    {
                        s = pageBz + tboxPage.Text.Trim().Substring(0, 2) + tboxPage.Text.Trim().Substring(2).PadLeft(4, '0');
                        webBrowser1.Document.InvokeScript("dingwei", new String[] { s });
                    }
                }

                e.Handled = true;
            }
        }

        private void cwWeb2Lookup_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            webBrowser1.Document.InvokeScript("getsltxtlen");
            string sssf = webBrowser1.Document.GetElementById("sltxtlen1207").Name;
            if (Convert.ToInt32(sssf) < 12800)
            {
                webBrowser1.Document.InvokeScript("getsltxt");
                sKeyWord = webBrowser1.Document.GetElementById("sltxt1207").Name.Trim();

                CzWord(sKeyWord, "");
            }
            else
            {
                this.Cursor = Cursors.Default;

                MessageBox.Show("所选择的文本须不多于12800个字符，可以少选择一点文本再查找。");
            }

            this.Cursor = Cursors.Default;
        }

        private void cwWeb2Search_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            webBrowser1.Document.InvokeScript("getsltxtlen");
            string sssf = webBrowser1.Document.GetElementById("sltxtlen1207").Name;
            if (Convert.ToInt32(sssf) < 12800)
            {
                webBrowser1.Document.InvokeScript("getsltxt");

                string strTmp = webBrowser1.Document.GetElementById("sltxt1207").Name.Trim();

                strTmp = new Regex("\r\n|\\.|’|‘|\"|'|\\?", RegexOptions.None).Replace(strTmp, " ").Trim();

                if (strTmp == "")
                    return;

                Program.ssfrm.tboxSs.Text = strTmp;

                Program.ssfrm.Show();

                if (FormWindowState.Minimized == Program.ssfrm.WindowState)
                    Program.ssfrm.WindowState = FormWindowState.Normal;

                Program.ssfrm.BringToFront();


                if (!(System.IO.Directory.Exists(@".\index")))
                {
                    MessageBox.Show("第一次使用三藏经典全文搜索功能，请先建立索引！");
                    return;
                }

                try
                {
                    Program.ssfrm.searchWord();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                this.Cursor = Cursors.Default;

                MessageBox.Show("所选择的文本须不多于12800个字符，可以少选择一点文本再查找。");
            }

            this.Cursor = Cursors.Default;
        }

        private void cboxPage_SelectionChangeCommitted(object sender, EventArgs e)
        {
            tboxPage.Select();
        }

        private void frmpali_ResizeEnd(object sender, EventArgs e)
        {
            
            if (_isfind)
            {
                //webBrowser1.Document.GetElementById(sid).ScrollIntoView(true);
                webBrowser1.Document.Window.ScrollTo(webBrowser1.Document.Body.ScrollLeft, webBrowser1.Document.GetElementById(sid).OffsetRectangle.Y - (int)(fCentValue * (webBrowser1.Document.GetElementById(sid).ScrollRectangle.Height)));
                _isfind = false;
            }

            _isResizeBegin = false;
        }

        private void tsDDbtnChapter_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            foreach (ToolStripMenuItem tsmiZ in tsDDbtnChapter.DropDownItems)
            {
                tsmiZ.Checked = false;
            }

            ((ToolStripMenuItem)(e.ClickedItem)).Checked = true;
            //((HtmlElement)alchapters[((int)(e.ClickedItem.Tag))]).ScrollIntoView(true);
            webBrowser1.Document.Window.ScrollTo(0, ((HtmlElement)alchapters[((int)(e.ClickedItem.Tag))]).OffsetRectangle.Y);
        }

        private void tsDDbtnBook_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            foreach (ToolStripMenuItem tsmiZ in tsDDbtnBook.DropDownItems)
            {
                tsmiZ.Checked = false;
            }

            ((ToolStripMenuItem)(e.ClickedItem)).Checked = true;
            //((HtmlElement)albooks[((int)(e.ClickedItem.Tag))]).ScrollIntoView(true);
            webBrowser1.Document.Window.ScrollTo(0, ((HtmlElement)albooks[((int)(e.ClickedItem.Tag))]).OffsetRectangle.Y);
        }

        private void tsDDbtnNikaya_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            foreach (ToolStripMenuItem tsmiZ in tsDDbtnNikaya.DropDownItems)
            {
                tsmiZ.Checked = false;
            }

            ((ToolStripMenuItem)(e.ClickedItem)).Checked = true;
            //((HtmlElement)alnikayas[((int)(e.ClickedItem.Tag))]).ScrollIntoView(true);
            webBrowser1.Document.Window.ScrollTo(0, ((HtmlElement)alnikayas[((int)(e.ClickedItem.Tag))]).OffsetRectangle.Y);
        }

        private void tsDDbtnTitle_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            foreach (ToolStripMenuItem tsmiZ in tsDDbtnTitle.DropDownItems)
            {
                tsmiZ.Checked = false;
            }

            ((ToolStripMenuItem)(e.ClickedItem)).Checked = true;
            //((HtmlElement)altitles[((int)(e.ClickedItem.Tag))]).ScrollIntoView(true);
            webBrowser1.Document.Window.ScrollTo(0, ((HtmlElement)altitles[((int)(e.ClickedItem.Tag))]).OffsetRectangle.Y);
        }
    }
}
