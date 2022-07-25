using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using Microsoft.VisualBasic;

using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;

namespace pced
{
    public partial class frmmulu : Form
    {
        public frmmulu()
        {
            InitializeComponent();
        }

        private void frmmulu_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Form1._isCloseButton)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        public string yybz = "";

        private void frmmulu_Load(object sender, EventArgs e)
        {
            if (_isfrmpali)
                this.Width = 300;
            else
                this.Width = 480;
        }

        public void loadMulu(string yybz)
        {
            IFormatter serializer = new BinaryFormatter();

            StreamReader sr = new StreamReader(new FileStream(@".\mulu\count", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            int iMuluCount = Convert.ToInt32(sr.ReadLine());
            sr.Close();

            treeView2.Nodes.Clear();

            for (int q = 0; q < iMuluCount; q++)
            {
                FileStream loadFile = new FileStream(@".\mulu\tr" + yybz + q.ToString() + ".dat", FileMode.Open, FileAccess.Read);
                treeView2.Nodes.Add(serializer.Deserialize(loadFile) as TreeNode);
                loadFile.Close();
            }

            foreach (TreeNode tr in treeView2.Nodes)
            {
                tr.ToolTipText = ((tvtag)(tr.Tag)).stooltip;

                //tr.ToolTipText = tr.Tag.ToString().Substring(1);
                //tr.Tag = tr.Tag.ToString().Substring(0, 1);
            }
        }

        private void treeView2_AfterExpand(object sender, TreeViewEventArgs e)
        {
            foreach (TreeNode tr in e.Node.Nodes)
            {
                if (tr.ToolTipText == "")
                {
                    tr.ToolTipText = ((tvtag)(tr.Tag)).stooltip;
                }

                if (((tvtag)(tr.Tag)).itag == 2)
                {
                    tr.ForeColor = Color.Blue;
                }
            }
        }

        private void rbtnPali_CheckedChanged(object sender, EventArgs e)
        {
            /*
            if (rbtnPali.Checked)
            {
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                label1.Visible = true;

                callcntopali(treeView2);

                if (FAN.Checked)
                    _isBig5 = true;
                else
                    _isBig5 = false;

                label1.Visible = false;
                numwait = 0;

                this.Cursor = System.Windows.Forms.Cursors.Default;
            }
            */
            string yybz = "";
            if (Program.mainform.FAN.Checked)
                yybz = "2";
            else
                yybz = "1";

            loadMulu(yybz);
        }

        private void rbtnCn_CheckedChanged(object sender, EventArgs e)
        {
            /*
            if (rbtnCn.Checked)
            {
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                label1.Visible = true;

                callpalitocn(treeView2);

                if (FAN.Checked)
                    _isBig5 = true;
                else
                    _isBig5 = false;

                label1.Visible = false;
                numwait = 0;

                this.Cursor = System.Windows.Forms.Cursors.Default;
            }
            */
            string yybz = "";
            if (Program.mainform.FAN.Checked)
                yybz = "4";
            else
                yybz = "3";

            loadMulu(yybz);
        }

        private void treeView2_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            cboxTag.Text = Enum.GetName(typeof(muluTag), ((tvtag)(e.Node.Tag)).itag);

            if (e.Node.BackColor.Name == "0")
                cboxColor.Text = "White";
            else
                cboxColor.Text = e.Node.BackColor.Name;

            tboxFileName.Text = e.Node.Name;
            tboxText.Text = e.Node.Text;
            tboxToolTipText.Text = e.Node.ToolTipText;

            tboxmulaName.Text = ((tvtag)(e.Node.Tag)).mulanm;
            tboxatthaName.Text = ((tvtag)(e.Node.Tag)).atthanm;
            tboxtikaName.Text = ((tvtag)(e.Node.Tag)).tikanm;

            tboxmulaNameC.Text = ((tvtag)(e.Node.Tag)).mulanmC;
            tboxatthaNameC.Text = ((tvtag)(e.Node.Tag)).atthanmC;
            tboxtikaNameC.Text = ((tvtag)(e.Node.Tag)).tikanmC;

            tboxmula.Text = ((tvtag)(e.Node.Tag)).fnmula;
            tboxattha.Text = ((tvtag)(e.Node.Tag)).fnattha;
            tboxtika.Text = ((tvtag)(e.Node.Tag)).fntika;
        }

        /// <summary>
        /// 父窗口的句柄，即从中创建了此小目录的读经窗口
        /// </summary>
        public int fuhandle = 0;

        /// <summary>
        /// 值为true标识是在frmpali窗口里单击创建目录的，变量缺省值为false
        /// </summary>
        public bool _isfrmpali = false;

        /// <summary>
        /// 当前打开的书编号
        /// </summary>
        public string tnbook = "";

        /// <summary>
        /// 当前选择的节点名
        /// </summary>
        public string tnname = "";
        //注意此处定义不要加 static ，因为静态(全局)变量不能通过实例化对象调用，只能通过类来调用，相反，以类名也不能调用实例化成员

        /// <summary>
        /// 当前所选择的节点的根节点的索引
        /// </summary>
        public int rootIndex = 3;

        /// <summary>
        /// 当前所选择的节点的根节点之下一层节点的索引，标识是‘律’0、‘经’1或‘论’2。
        /// </summary>
        public int secondIndex = 0;

        /// <summary>
        /// 上一次通过主目录打开的读经窗口的left偏移值，最初值为0
        /// </summary>
        public int preWleftDelta = 0;

        /// <summary>
        /// 上一次通过主目录打开的读经窗口的top值，最初值为30
        /// </summary>
        public int preWtop = 30;

        /// <summary>
        /// 存放js代码
        /// </summary>
        public static string strJavaScript = "<script type='text/javascript'>" +
                "var sltxtlen=0;" +
                "var sltxt='';" +

                //"window.onscroll = function(){alert(document.body.scrollTop);};" +
                "window.onscroll = function(){window.external.qddqpage();};" +
            //"function dingwei(toptop) {document.getElementsByName(toptop)[0].scrollIntoView();}" +
                "function dingwei(toptop) {window.scrollTo(0, document.getElementsByName(toptop)[0].offsetTop);}" +

                "function getsltxtlen()" +
                "{" +
                "sltxtlen= document.selection.createRange().text.length;" +
                "document.getElementById('sltxtlen1207').name=sltxtlen;" +
                "}" +
                "function getsltxt()" +
                "{" +
                "sltxt= document.selection.createRange().text;" +
                "document.getElementById('sltxt1207').name=sltxt;" +
                "}" +

                //strJs +

                "function HighLight(nWord,iczNum,bgColor) {" +
                "var n = -1;" +
                "document.getElementById('hitsnum1207').name = \"\";" +
                "if (nWord != '') {" +
            //nWord = document.selection.createRange().text;
                "var keyword = document.body.createTextRange();" +
                "while (keyword.findText(nWord,1,2)) {" +
                "n++;" +
            //            alert(keyword.htmlText);
            //"keyword.pasteHTML(\"<span style='color:#000000; background:#FFFF66; font-weight:bold' id='hit\" + n + \"'>\" + keyword.htmlText + \"</span>\");"+
            //"keyword.pasteHTML(\"<span style='color:#000000; font-weight:normal; background:\"+bgColor+\";' id='hit\" +iczNum+\"c\"+ n + \"'>\" + keyword.htmlText + \"</span>\");" +
                "keyword.pasteHTML(\"<span style='font-weight:normal; background:\"+bgColor+\";' id='hit\" +iczNum+\"c\"+ n + \"'>\" + keyword.htmlText + \"</span>\");" +
                "keyword.moveStart('character', 1);" +
                "}" +
                "}" +
                "if (n >= 0)" +
                "document.getElementById('hitsnum1207').name = n;" +
                "}" +


                "</script>" +
                "<a id='sltxtlen1207'></a>" +
                "<a id='sltxt1207'></a>" +
                "<a id='hitsnum1207'></a>";

        private void treeView2_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //StreamReader srjs = new StreamReader(new FileStream(@".\set\paliwin.js", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            //string strJs = srjs.ReadToEnd();
            //srjs.Close();

            //用之引用当前所双击的节点。注意，节点展开消息发生在节点双击消息之前。因为节点会自动伸缩弹动，导致有时发出双击消息的节点不是开始所双击的节点。
            TreeNode currnode = treeView2.SelectedNode;

            if (((tvtag)(currnode.Tag)).itag == 1)
            {
                return;
            }

            //如果有子节点，即使此节点是卷，也当目录看待
            if (currnode.GetNodeCount(false) > 0 && ((tvtag)(currnode.Tag)).itag != 3)
            {
                return;
            }

            tnname = currnode.Name;

            Form1.frmmuluhandle = this.Handle.ToInt32();

            if (!_isfrmpali)
            {
                //取得根节点的索引，以确定当前点击的章节是根本、义注、复注或其它
                TreeNode trN = currnode;
                while (trN.Level > 0)
                {
                    trN = trN.Parent;
                }
                rootIndex = trN.Index;

                //取得根节点之下一层节点的索引，以确定当前点击的章节是‘律’、‘经’或‘论’
                trN = currnode;
                while (trN.Level > 1)
                {
                    trN = trN.Parent;
                }
                secondIndex = trN.Index;
            }

            string bookpath = ""; //书文件路径
            string bookbz = ""; //在书htm文件中书名标志

            //此处去掉判断tnbook，允许重复打开一本书
            //if (((tvtag)(currnode.Tag)).itag == 2 && currnode.Name != tnbook)
            if (((tvtag)(currnode.Tag)).itag == 2)
            {
                bookpath = @".\pali\" + currnode.Name + ".htm";

                if (!(File.Exists(bookpath)))
                {
                    MessageBox.Show("此篇经典文件没找到！您可能没有安装本程序的‘pali经典文件库’或者是删除了文件！\r\n请到‘觉悟之路’网站 http://www.dhamma.org.cn/ 下载本程序的‘pali经典文件库’，\r\n解压缩后将经典文件复制到本程序目录下的 pali\\ 子目录里。");
                    return;
                }

                if (currnode.Name.Substring(1, 1) == "-")
                    bookbz = currnode.Name.Substring(2);
                else
                    bookbz = currnode.Name;

                StreamReader sr = new StreamReader(new FileStream(bookpath, FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
                string strSZ = sr.ReadToEnd();
                sr.Close();



                //改变不同字体大小设置的style风格
                if (!(Program.mainform.fsoriginal.Checked))
                    strSZ = new Regex(@"<style>[\S\s]*</style>", RegexOptions.IgnoreCase).Replace(strSZ, Form1.strf1);





                strSZ = new Regex("<a name=\"" + bookbz + "_", RegexOptions.IgnoreCase).Replace(strSZ, "<a id=\"" + bookbz + "_");
                strSZ = new Regex("<body>", RegexOptions.IgnoreCase).Replace(strSZ, "<body onload = 'window.external.htmonload1()' onmouseup = 'window.external.htmonmouseup()'>" + strJavaScript);
                //引号前后加空格，以避免在浏览器中查找时，被连着引号一起当成一个词，而造成引号边的词查找不出
                strSZ = new Regex("(?<w>‘+|’+)", RegexOptions.None).Replace(strSZ, " ${w} ");

                strSZ = new Regex("<p class=\"nikaya\">", RegexOptions.None).Replace(strSZ, "<p class=\"nikaya\"><a name=\"nikaya\"></a>");
                strSZ = new Regex("<p class=\"book\">", RegexOptions.None).Replace(strSZ, "<p class=\"book\"><a name=\"book\"></a>");
                strSZ = new Regex("<p class=\"title\">", RegexOptions.None).Replace(strSZ, "<p class=\"title\"><a name=\"title\"></a>");
                strSZ = new Regex("<p class=\"chapter\">", RegexOptions.None).Replace(strSZ, "<p class=\"chapter\"><a name=\"chapter\"></a>");

                if (currnode.GetNodeCount(false) == 0)
                {
                    MatchCollection mc = new Regex("<a id=\"" + bookbz + "_" + @"(?<id>\d+)" + "\"></a>[^<]*<[^>]*>(?<name>.*)<[^>]*>", RegexOptions.IgnoreCase).Matches(strSZ);
                    foreach (Match ma in mc)
                    {
                        TreeNode tz = new TreeNode();
                        tz.Name = bookbz + "_" + ma.Groups["id"].Value;
                        tz.Text = ma.Groups["name"].Value;

                        tvtag tvt = new tvtag(3);
                        tz.Tag = tvt;

                        currnode.Nodes.Add(tz);
                    }
                }

                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                frmpali frmw = new frmpali();
                if (_isfrmpali)
                {
                    frmw.Left = ((frmpali)(frmpali.FromHandle((IntPtr)(fuhandle)))).Left + 22;
                    frmw.Top = ((frmpali)(frmpali.FromHandle((IntPtr)(fuhandle)))).Top + 50;
                }
                else
                {
                    if (preWleftDelta + 750 > Screen.PrimaryScreen.WorkingArea.Width || preWtop + 500 > Screen.PrimaryScreen.WorkingArea.Height)
                    {
                        preWleftDelta = 0;
                        preWtop = 30;
                    }

                    frmw.Left = Screen.PrimaryScreen.WorkingArea.Width - 750 - preWleftDelta;
                    frmw.Top = preWtop;

                    preWleftDelta = preWleftDelta + 22;
                    preWtop = preWtop + 22;
                    //frmw.StartPosition = FormStartPosition.WindowsDefaultLocation;
                }
                frmw.Text = currnode.Text;
                frmw.palilb = rootIndex;
                frmw.sanzanglb = secondIndex;
                frmw.mulafile = ((tvtag)(currnode.Tag)).fnmula;
                frmw.atthafile = ((tvtag)(currnode.Tag)).fnattha;
                frmw.tikafile = ((tvtag)(currnode.Tag)).fntika;
                frmw.webBrowser1.ObjectForScripting = frmw;
                frmw.paliStFrm = new paliSt();

                tsmiWin dd = new tsmiWin();
                dd.Text = frmw.Text;
                dd.Tag = frmw.Handle.ToInt32();
                Program.toolbarform.tsddb.DropDownItems.Add(dd);

                frmw.Show();
                Form1.frmpalihandle = frmw.Handle.ToInt32();

                frmw.webBrowser1.DocumentText = strSZ;

                tnbook = currnode.Name;
            }

            if (((tvtag)(currnode.Tag)).itag == 3)
            {
                bookpath = @".\pali\" + currnode.Parent.Name + ".htm";

                if (!(File.Exists(bookpath)))
                {
                    MessageBox.Show("此篇经典文件没找到！您可能没有安装本程序的‘pali经典文件库’或者是删除了文件！\r\n请到‘觉悟之路’网站 http://www.dhamma.org.cn/ 下载本程序的‘pali经典文件库’，\r\n解压缩后将经典文件复制到本程序目录下的 pali\\ 子目录里。");
                    return;
                }

                if (currnode.Parent.Name.Substring(1, 1) == "-")
                    bookbz = currnode.Parent.Name.Substring(2);
                else
                    bookbz = currnode.Parent.Name;

                StreamReader sr = new StreamReader(new FileStream(bookpath, FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
                string strSZ = sr.ReadToEnd();
                sr.Close();






                //改变不同字体大小设置的style风格
                if (!(Program.mainform.fsoriginal.Checked))
                    strSZ = new Regex(@"<style>[\S\s]*</style>", RegexOptions.IgnoreCase).Replace(strSZ, Form1.strf1);





                strSZ = new Regex("<a name=\"" + bookbz + "_", RegexOptions.IgnoreCase).Replace(strSZ, "<a id=\"" + bookbz + "_");
                strSZ = new Regex("<body>", RegexOptions.IgnoreCase).Replace(strSZ, "<body onload = 'window.external.htmonload()' onmouseup = 'window.external.htmonmouseup()'>" + strJavaScript);
                //引号前后加空格，以避免在浏览器中查找时，被连着引号一起当成一个词，而造成引号边的词查找不出
                strSZ = new Regex("(?<w>‘+|’+)", RegexOptions.None).Replace(strSZ, " ${w} ");

                strSZ = new Regex("<p class=\"nikaya\">", RegexOptions.None).Replace(strSZ, "<p class=\"nikaya\"><a name=\"nikaya\"></a>");
                strSZ = new Regex("<p class=\"book\">", RegexOptions.None).Replace(strSZ, "<p class=\"book\"><a name=\"book\"></a>");
                strSZ = new Regex("<p class=\"title\">", RegexOptions.None).Replace(strSZ, "<p class=\"title\"><a name=\"title\"></a>");
                strSZ = new Regex("<p class=\"chapter\">", RegexOptions.None).Replace(strSZ, "<p class=\"chapter\"><a name=\"chapter\"></a>");

                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                frmpali frmw = new frmpali();
                if (_isfrmpali)
                {
                    frmw.Left = ((frmpali)(frmpali.FromHandle((IntPtr)(fuhandle)))).Left + 22;
                    frmw.Top = ((frmpali)(frmpali.FromHandle((IntPtr)(fuhandle)))).Top + 50;
                }
                else
                {
                    if (preWleftDelta + 750 > Screen.PrimaryScreen.WorkingArea.Width || preWtop + 500 > Screen.PrimaryScreen.WorkingArea.Height)
                    {
                        preWleftDelta = 0;
                        preWtop = 30;
                    }

                    frmw.Left = Screen.PrimaryScreen.WorkingArea.Width - 750 - preWleftDelta;
                    frmw.Top = preWtop;

                    preWleftDelta = preWleftDelta + 22;
                    preWtop = preWtop + 22;
                    //frmw.StartPosition = FormStartPosition.WindowsDefaultLocation;
                }
                frmw.Text = currnode.Text;
                frmw.palilb = rootIndex;
                frmw.sanzanglb = secondIndex;
                frmw.mulafile = ((tvtag)(currnode.Parent.Tag)).fnmula;
                frmw.atthafile = ((tvtag)(currnode.Parent.Tag)).fnattha;
                frmw.tikafile = ((tvtag)(currnode.Parent.Tag)).fntika;
                frmw.webBrowser1.ObjectForScripting = frmw;
                frmw.paliStFrm = new paliSt();

                tsmiWin dd = new tsmiWin();
                dd.Text = frmw.Text;
                dd.Tag = frmw.Handle.ToInt32();
                Program.toolbarform.tsddb.DropDownItems.Add(dd);

                frmw.Show();
                Form1.frmpalihandle = frmw.Handle.ToInt32();

                frmw.webBrowser1.DocumentText = strSZ;

                tnbook = currnode.Parent.Name;
            }

            if (((tvtag)(currnode.Tag)).itag == 4)
            {
                //用标题判断是否已打开
                if (Program.mainform.cnform.Text != (currnode.Parent.Text + currnode.Text))
                {
                    bookpath = @".\pali_cn\" + currnode.Name + ".htm";

                    if (!(File.Exists(bookpath)))
                    {
                        MessageBox.Show("此篇经典文件没找到！您可能没有安装本程序的‘pali经典文件库’或者是删除了文件！\r\n请到‘觉悟之路’网站 http://www.dhamma.org.cn/ 下载本程序的‘pali经典文件库’，\r\n解压缩后将经典文件复制到本程序目录下的 pali\\ 子目录里。");
                        return;
                    }

                    StreamReader sr = new StreamReader(new FileStream(bookpath, FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
                    string strSZ = sr.ReadToEnd();
                    sr.Close();

                    Program.mainform.cnform.webBrowserCN.DocumentText = strSZ;

                    Program.mainform.cnform.Text = currnode.Parent.Text + currnode.Text;
                }

                Program.mainform.cnform.Show();
                if (!(Program.mainform.cnform.WindowState == FormWindowState.Normal))
                    Program.mainform.cnform.WindowState = FormWindowState.Normal;
                //SendMessage(cnform.Handle, 0x112, (IntPtr)0xf120, (IntPtr)0); //恢复窗口，此消息在此处不起作用？
                Program.mainform.cnform.Activate();
            }

            if (((tvtag)(currnode.Tag)).itag == 5)
            {
                bookpath = @".\pali\" + currnode.Name + ".htm";

                if (!(File.Exists(bookpath)))
                {
                    MessageBox.Show("此篇经典文件没找到！您可能没有安装本程序的‘pali经典文件库’或者是删除了文件！\r\n请到‘觉悟之路’网站 http://www.dhamma.org.cn/ 下载本程序的‘pali经典文件库’，\r\n解压缩后将经典文件复制到本程序目录下的 pali\\ 子目录里。");
                    return;
                }

                StreamReader sr = new StreamReader(new FileStream(bookpath, FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
                string strSZ = sr.ReadToEnd();
                sr.Close();

                strSZ = new Regex("<body>", RegexOptions.IgnoreCase).Replace(strSZ, "<body onload = 'window.external.htmonload1()' onmouseup = 'window.external.htmonmouseup()'>" + strJavaScript);

                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                frmpali frmw = new frmpali();
                frmw.Text = currnode.Text;
                frmw.webBrowser1.ObjectForScripting = frmw;
                frmw.Show();
                frmw.webBrowser1.DocumentText = strSZ;

                tnbook = currnode.Name;
            }

            if (_isfrmpali)
            {
                this.Hide();
            }
        }

        public static string sTvTmp = "";

        /// <summary>
        /// 在转换目录显示时对已转换的节点进行计数
        /// </summary>
        public static int numwait = 0;

        /// <summary>
        /// 值为true表示当前经典目录里是繁体字
        /// </summary>
        public static bool _isBig5 = false;

        private void palitocn(TreeNode treeNode)
        {
            if (treeNode.ToolTipText != "")
            {
                sTvTmp = treeNode.Text;
                //treeNode.Text = treeNode.ToolTipText;
                if (Program.mainform.FAN.Checked)
                {
                    if (_isBig5)
                    {
                        treeNode.Text = treeNode.ToolTipText;
                    }
                    else
                    {
                        treeNode.Text = Strings.StrConv(treeNode.ToolTipText, VbStrConv.TraditionalChinese, 0x0409);
                    }
                }
                else
                {
                    if (_isBig5)
                    {
                        treeNode.Text = Strings.StrConv(treeNode.ToolTipText, VbStrConv.SimplifiedChinese, 0x0409);
                    }
                    else
                    {
                        treeNode.Text = treeNode.ToolTipText;
                    }
                }
                treeNode.ToolTipText = sTvTmp;

                numwait++;
                label1.Text = "正保存：转换" + numwait.ToString() + "节点";
                label1.Refresh();
            }

            foreach (TreeNode tn in treeNode.Nodes)
            {
                palitocn(tn);
            }
        }

        private void callpalitocn(TreeView treeView)
        {
            TreeNodeCollection nodes = treeView.Nodes;
            foreach (TreeNode n in nodes)
            {
                palitocn(n);
            }
        }

        private void cntopali(TreeNode treeNode)
        {
            if (treeNode.ToolTipText != "")
            {
                sTvTmp = treeNode.Text;
                treeNode.Text = treeNode.ToolTipText;
                //treeNode.ToolTipText = sTvTmp;
                if (Program.mainform.FAN.Checked)
                {
                    if (_isBig5)
                    {
                        treeNode.ToolTipText = sTvTmp;
                    }
                    else
                    {
                        treeNode.ToolTipText = Strings.StrConv(sTvTmp, VbStrConv.TraditionalChinese, 0x0409);
                    }
                }
                else
                {
                    if (_isBig5)
                    {
                        treeNode.ToolTipText = Strings.StrConv(sTvTmp, VbStrConv.SimplifiedChinese, 0x0409);
                    }
                    else
                    {
                        treeNode.ToolTipText = sTvTmp;
                    }
                }

                numwait++;
                label1.Text = "正保存：转换" + numwait.ToString() + "节点";
                label1.Refresh();
            }

            foreach (TreeNode tn in treeNode.Nodes)
            {
                cntopali(tn);
            }
        }

        private void callcntopali(TreeView treeView)
        {
            TreeNodeCollection nodes = treeView.Nodes;
            foreach (TreeNode n in nodes)
            {
                cntopali(n);
            }
        }

        //检测重名
        private void pbtnjcname_Click(object sender, EventArgs e)
        {
            if (File.Exists(@".\pali\" + tboxFileName.Text + ".htm") || File.Exists(@".\pali_cn\" + tboxFileName.Text + ".htm"))
                MessageBox.Show("文件名与经藏文件库中的现有文件名重复！");
            else
                MessageBox.Show("文件名可用！");
        }

        //建根节点
        private void pbtnjgjd_Click(object sender, EventArgs e)
        {
            TreeNode tr = new TreeNode();

            tr.Name = "";

            tvtag tvt = new tvtag(1);
            tr.Tag = tvt;

            tr.BackColor = Color.White;
            tr.Text = "";
            tr.ToolTipText = "";

            treeView2.Nodes.Add(tr);

            treeView2.SelectedNode = treeView2.Nodes[treeView2.Nodes.Count - 1];

            tboxFileName.Text = "";
            cboxTag.Text = muluTag.目录.ToString();
            cboxColor.Text = "White";
            tboxText.Text = "";
            tboxToolTipText.Text = "";
        }

        //增加子级节点
        private void pbtnzjjd_Click(object sender, EventArgs e)
        {
            if (treeView2.SelectedNode == null)
            {
                MessageBox.Show("请先选中要修改的节点！");
                return;
            }

            TreeNode tr = new TreeNode();

            tr.Name = "";

            tvtag tvt = new tvtag(1);
            tr.Tag = tvt;

            tr.BackColor = Color.White;
            tr.Text = "";
            tr.ToolTipText = "";

            treeView2.SelectedNode.Nodes.Add(tr);

            treeView2.SelectedNode = treeView2.SelectedNode.LastNode;

            tboxFileName.Text = "";
            cboxTag.Text = muluTag.目录.ToString();
            cboxColor.Text = "White";
            tboxText.Text = "";
            tboxToolTipText.Text = "";
        }

        //修改
        private void pbtnxg_Click(object sender, EventArgs e)
        {
            if (treeView2.SelectedNode == null)
            {
                MessageBox.Show("请先选中要修改的节点！");
                return;
            }

            treeView2.SelectedNode.Name = tboxFileName.Text;

            tvtag tvt = new tvtag(0);
            tvt.itag = (int)(Enum.Parse(typeof(muluTag), cboxTag.Text));
            tvt.stooltip = tboxToolTipText.Text;
            tvt.fnmula = tboxmula.Text;
            tvt.fnattha = tboxattha.Text;
            tvt.fntika = tboxtika.Text;

            tvt.mulanm = tboxmulaName.Text;
            tvt.atthanm = tboxatthaName.Text;
            tvt.tikanm = tboxtikaName.Text;

            tvt.mulanmC = tboxmulaNameC.Text;
            tvt.atthanmC = tboxatthaNameC.Text;
            tvt.tikanmC = tboxtikaNameC.Text;

            treeView2.SelectedNode.Tag = tvt;

            treeView2.SelectedNode.BackColor = Color.FromName(cboxColor.Text);

            treeView2.SelectedNode.Text = tboxText.Text;

            treeView2.SelectedNode.ToolTipText = tboxToolTipText.Text;

            MessageBox.Show("ok " + ((tvtag)(treeView2.SelectedNode.Tag)).stooltip);
        }

        //删除
        private void pbtndel_Click(object sender, EventArgs e)
        {
            if (treeView2.SelectedNode == null)
            {
                MessageBox.Show("请先选中要修改的节点！");
                return;
            }

            if (MessageBox.Show("您确定要删除此节点吗？", "提示：", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                treeView2.SelectedNode.Remove();
            }
        }

        //向上
        private void pbtnup_Click(object sender, EventArgs e)
        {
            if (treeView2.SelectedNode.Index > 0)
            {
                int i = treeView2.SelectedNode.PrevNode.Index;
                if (treeView2.SelectedNode.Level == 0)
                {
                    treeView2.Nodes.Insert(i, (TreeNode)(treeView2.SelectedNode.Clone()));
                    treeView2.SelectedNode = treeView2.Nodes[i];
                    treeView2.SelectedNode.NextNode.NextNode.Remove();
                }
                else
                {
                    treeView2.SelectedNode.Parent.Nodes.Insert(i, (TreeNode)(treeView2.SelectedNode.Clone()));
                    treeView2.SelectedNode = treeView2.SelectedNode.Parent.Nodes[i];
                    treeView2.SelectedNode.NextNode.NextNode.Remove();
                }
            }
        }

        //向下
        private void pbtndown_Click(object sender, EventArgs e)
        {
            int i = treeView2.SelectedNode.Index;
            if (treeView2.SelectedNode.Level == 0)
            {
                if (treeView2.SelectedNode.Index < treeView2.Nodes.Count - 1)
                {
                    treeView2.SelectedNode = treeView2.SelectedNode.NextNode;

                    treeView2.Nodes.Insert(i, (TreeNode)(treeView2.SelectedNode.Clone()));
                    treeView2.SelectedNode = treeView2.Nodes[i + 1];
                    treeView2.SelectedNode.NextNode.Remove();
                }
            }
            else
            {
                if (treeView2.SelectedNode.Index < treeView2.SelectedNode.Parent.Nodes.Count - 1)
                {
                    treeView2.SelectedNode = treeView2.SelectedNode.NextNode;

                    treeView2.SelectedNode.Parent.Nodes.Insert(i, (TreeNode)(treeView2.SelectedNode.Clone()));
                    treeView2.SelectedNode = treeView2.SelectedNode.Parent.Nodes[i + 1];
                    treeView2.SelectedNode.NextNode.Remove();
                }
            }
        }

        string strNI = "";

        //保存更改
        private void pbtnsave_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("您确定要保存对目录的修改吗？", "提示：", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            strNI = "";

            //目录节点背景颜色
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            label1.Visible = true;
            for (int z = 0; z < treeView2.Nodes.Count; z++)
            {
                muluColor(treeView2.Nodes[z]);
            }
            numwait = 0;
            label1.Visible = false;
            this.Cursor = System.Windows.Forms.Cursors.Default;

            //保存
            StreamWriter swNI = new StreamWriter(@".\mulu\nameidx", false, System.Text.Encoding.GetEncoding(65001));
            swNI.Write(strNI);
            swNI.Close();

            //保存
            StreamWriter sw = new StreamWriter(@".\mulu\count", false, System.Text.Encoding.GetEncoding(65001));
            sw.WriteLine(treeView2.Nodes.Count.ToString());
            sw.Close();

            IFormatter serializer = new BinaryFormatter();

            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            label1.Visible = true;

            for (int y = 1; y < 5; y++)
            {
                for (int z = 0; z < treeView2.Nodes.Count; z++)
                {
                    muluPali_GB(treeView2.Nodes[z], y);

                    FileStream saveFile = new FileStream(@".\mulu\tr" + y.ToString() + z.ToString() + ".dat", FileMode.Create, FileAccess.Write);
                    serializer.Serialize(saveFile, treeView2.Nodes[z]);
                    saveFile.Close();
                }
                numwait = 0;
            }

            label1.Visible = false;
            this.Cursor = System.Windows.Forms.Cursors.Default;


            //load
            StreamReader sr = new StreamReader(new FileStream(@".\mulu\count", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            int iMuluCount = Convert.ToInt32(sr.ReadLine());
            sr.Close();

            treeView2.Nodes.Clear();

            for (int q = 0; q < iMuluCount; q++)
            {
                FileStream loadFile = new FileStream(@".\mulu\tr4" + q.ToString() + ".dat", FileMode.Open, FileAccess.Read);
                treeView2.Nodes.Add(serializer.Deserialize(loadFile) as TreeNode);
                loadFile.Close();
            }

            foreach (TreeNode tr in treeView2.Nodes)
                tr.ToolTipText = ((tvtag)(tr.Tag)).stooltip;

            //重新读入书名目录索引
            Program.mainform.alName.Clear();
            StreamReader srNI = new StreamReader(new FileStream(@".\mulu\nameidx", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            string sNitmp = srNI.ReadLine();
            while (sNitmp != null)
            {
                Program.mainform.alName.Add(sNitmp);

                sNitmp = srNI.ReadLine();
            }
            srNI.Close();

            MessageBox.Show("OK 目录已保存！");
        }

        //目录节点背景颜色
        private void muluColor(TreeNode treeNode)
        {
            try
            {
                //保存文件名和节点在目录中的位置，以便于通过文件名直接定位到相应的节点
                //目前目录中节点索引最大是两位数，在保存到文件中时，不足两位的在后面补一个空格
                if (((tvtag)(treeNode.Tag)).itag == 2)
                {
                    strNI = strNI + treeNode.Name.PadRight(20, ' ');
                    TreeNode tnp = treeNode;
                    while (tnp.Level > 0)
                    {
                        strNI = strNI + tnp.Index.ToString().PadRight(2, ' ');
                        tnp = tnp.Parent;
                    }
                    strNI = strNI + tnp.Index.ToString().PadRight(2, ' ') + "\r\n";
                }

                //更改节点颜色
                if (((tvtag)(treeNode.Tag)).itag == 4)
                {
                    TreeNode tnp = treeNode;
                    while (tnp.Level > 0)
                    {
                        tnp.Parent.BackColor = Color.LightCyan;
                        //tnp.Parent.ForeColor = Color.LightSkyBlue;
                        tnp = tnp.Parent;
                    }
                }
            }
            catch (System.InvalidCastException ice)
            {
                MessageBox.Show(ice.Message);
            }

            foreach (TreeNode tn in treeNode.Nodes)
            {
                muluColor(tn);
            }
        }

        /// <summary>
        /// 转化目录text与tooltiptext语言
        /// </summary>
        /// <param name="iYuyanBz">值为1表示：text: pali tag: GB    值为2表示：text: pali tag: Big5    值为3表示：text: GB tag: pali    值为4表示：text: Big5 tag: pali</param>
        private void muluPali_GB(TreeNode treeNode, int iYuyanBz)
        {
            if (treeNode.ToolTipText == "")
                treeNode.ToolTipText = ((tvtag)(treeNode.Tag)).stooltip;

            if (treeNode.ToolTipText != "")
            {
                //text: pali tag: GB
                if (iYuyanBz == 1)
                    ;

                //text: pali tag: Big5
                if (iYuyanBz == 2)
                {
                    tvtag tvt = new tvtag(0);
                    tvt.itag = ((tvtag)(treeNode.Tag)).itag;
                    tvt.stooltip = Strings.StrConv(treeNode.ToolTipText, VbStrConv.TraditionalChinese, 0x0409);
                    tvt.fnmula = ((tvtag)(treeNode.Tag)).fnmula;
                    tvt.fnattha = ((tvtag)(treeNode.Tag)).fnattha;
                    tvt.fntika = ((tvtag)(treeNode.Tag)).fntika;

                    treeNode.Tag = tvt;
                }

                //text: GB tag: pali
                if (iYuyanBz == 3)
                {
                    tvtag tvt = new tvtag(0);
                    tvt.itag = ((tvtag)(treeNode.Tag)).itag;
                    tvt.stooltip = treeNode.Text;
                    tvt.fnmula = ((tvtag)(treeNode.Tag)).fnmula;
                    tvt.fnattha = ((tvtag)(treeNode.Tag)).fnattha;
                    tvt.fntika = ((tvtag)(treeNode.Tag)).fntika;

                    treeNode.Tag = tvt;

                    treeNode.Text = treeNode.ToolTipText;
                }

                //text: Big5 tag: pali
                if (iYuyanBz == 4)
                    treeNode.Text = Strings.StrConv(treeNode.Text, VbStrConv.TraditionalChinese, 0x0409);

                numwait++;
                label1.Text = "正保存：转换" + numwait.ToString() + "节点";
                label1.Refresh();
            }

            foreach (TreeNode tn in treeNode.Nodes)
            {
                muluPali_GB(tn, iYuyanBz);
            }
        }

        private void pbtnXlhCk_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("序列化操作不要与其它词库操作混在一起进行，如果您已经进行了其它的词库操作，\r\n那么建议您重新启动pced程序，然后再进行词库序列化！\r\n\r\n您确定要现在进行词库序列化吗？", "提示：", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            // Get serializer
            IFormatter serializer = new BinaryFormatter();

            // Serialize
            FileStream saveFile1 = new FileStream(@".\pali-h\cidian.dat", FileMode.Create, FileAccess.Write);
            serializer.Serialize(saveFile1, Program.pl_strL);
            saveFile1.Close();

            FileStream saveFile2 = new FileStream(@".\pali-h\index.dat", FileMode.Create, FileAccess.Write);
            serializer.Serialize(saveFile2, Program.pl_sL);
            saveFile2.Close();

            FileStream saveFile3 = new FileStream(@".\e-pali\cidian.dat", FileMode.Create, FileAccess.Write);
            serializer.Serialize(saveFile3, Program.en_strL);
            saveFile3.Close();

            FileStream saveFile4 = new FileStream(@".\e-pali\index.dat", FileMode.Create, FileAccess.Write);
            serializer.Serialize(saveFile4, Program.en_sL);
            saveFile4.Close();

            // Deserialize
            FileStream loadFile = new FileStream(@".\pali-h\cidian.dat", FileMode.Open, FileAccess.Read);
            string[] pls = serializer.Deserialize(loadFile) as string[];
            loadFile.Close();

            MessageBox.Show("ok ! finished.");

            //MessageBox.Show(pls[0]);
            //MessageBox.Show(pls[1]);
            //MessageBox.Show(pls[2]);
        }

        //修改旧的全部tag，置为新tvtag结构，此函数只能在旧的文本tag目录上使用一次
        private void xgoldalltvtag(object sender, EventArgs e)
        {
            for (int z = 0; z < treeView2.Nodes.Count; z++)
            {
                tttttt(treeView2.Nodes[z]);
            }

            MessageBox.Show("ok");
        }

        private void tttttt(TreeNode treeNode)
        {
            if (treeNode.ToolTipText == "")
            {
                tvtag tvt = new tvtag(0);
                tvt.itag = Convert.ToInt32(treeNode.Tag.ToString().Substring(0, 1));
                tvt.stooltip = treeNode.Tag.ToString().Substring(1);

                treeNode.Tag = tvt;

                treeNode.ToolTipText = tvt.stooltip;
            }
            else
            {
                try
                {
                    tvtag tvt = new tvtag(0);
                    tvt.itag = Convert.ToInt32(treeNode.Tag.ToString().Substring(0, 1));
                    tvt.stooltip = treeNode.ToolTipText;

                    treeNode.Tag = tvt;
                }
                catch (Exception ev)
                {
                    MessageBox.Show(ev.ToString() + treeNode.Text);
                }
            }

            foreach (TreeNode tn in treeNode.Nodes)
            {
                tttttt(tn);
            }
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
            //ex = e.X + panel1.Left;
            //ey = e.Y + panel1.Top;

            //bdroppanel = true;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            /*
            if (bdroppanel)
            {
                int exnew = e.X + panel1.Left;
                int eynew = e.Y + panel1.Top;

                int deltax = exnew - ex;
                int deltay = eynew - ey;

                ex = exnew;
                ey = eynew;

                panel1.Left = panel1.Left + deltax;
                panel1.Top = panel1.Top + deltay;
            }
            */
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            //bdroppanel = false;
        }

        private int formmaxwidth = 408;

        private void frmmulu_Resize(object sender, EventArgs e)
        {
            if (this.Width > formmaxwidth)
                this.Width = formmaxwidth;
        }

        private void frmmulu_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
                //this.Left = 0;
                this.Top = 0;
                this.Width = formmaxwidth;
                this.Height = Screen.PrimaryScreen.WorkingArea.Height;
            }

            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (formmaxwidth == 408)
            {
                formmaxwidth = 775;
                this.Width = 775;
                button1.Text = "隐藏管理工具";
            }
            else
            {
                formmaxwidth = 408;
                this.Width = 408;
                button1.Text = "显示管理工具";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new frmCk().Show();
        }

        private void btnLucn_Click(object sender, EventArgs e)
        {
            Program.ssfrm.Show();

            if (FormWindowState.Minimized == Program.ssfrm.WindowState)
                Program.ssfrm.WindowState = FormWindowState.Normal;

            Program.ssfrm.BringToFront();
        }

        private void btnLjgj_Click(object sender, EventArgs e)
        {
            frmYzLj yzljform = new frmYzLj();
            yzljform.Show();
        }

        private void btnSyck_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("您确定要从‘中文词库源档’建立或重建‘词库的中文查词索引’吗？\r\n注意：在建立此索引时，需要先把词库源档cidian和index这两个文件复制到程序的pali-h子目录下，\r\n建立此索引可能需要10几分钟，中间无反应无提示，请不要强行退出程序。", "提示：", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                IndexWriter writer;

                Analyzer analyzer = new StandardAnalyzer();
                writer = new IndexWriter("index_ck", analyzer, true);
                writer.SetMaxFieldLength(1000000);

                StreamReader sr = new StreamReader(new FileStream(@".\pali-h\cidian", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
                string s = sr.ReadLine();
                while (s != null)
                {
                    if (s.Trim() != "")
                    {
                        MatchCollection mc = new Regex(@"[\u4e00-\u9fa5]", RegexOptions.None).Matches(s);
                        if (mc.Count > 0)
                        {
                            Document document = new Document();
                            document.Add(new Field("content", s, Field.Store.YES, Field.Index.TOKENIZED));
                            writer.AddDocument(document);
                        }
                    }
                    s = sr.ReadLine();
                }
                sr.Close();

                writer.Optimize();
                writer.Close();

                MessageBox.Show("中文查词索引建立完成！此索引存放于index_ck子目录。");
            }
        }

        private void foreachmulu(TreeNode treeNode)
        {
            //    if (((tvtag)(treeNode.Tag)).itag == 2)
            {
                StreamReader sr = new StreamReader(new FileStream(@".\nametbl.txt", FileMode.Open), System.Text.Encoding.GetEncoding(65001));
                string strLine = sr.ReadLine();
                while (strLine != null)
                {
                    if (strLine.Substring(0, 10).Trim() == treeNode.Name.Trim())
                    {
                        treeNode.Name = strLine.Substring(10);

                        numwait++;
                        label1.Text = "重命名：" + numwait.ToString() + "节点";
                        label1.Refresh();

                        break;
                    }

                    strLine = sr.ReadLine();
                }
                sr.Close();
            }

            foreach (TreeNode tn in treeNode.Nodes)
            {
                foreachmulu(tn);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            label1.Visible = true;
            for (int z = 0; z < treeView2.Nodes.Count; z++)
            {
                foreachmulu(treeView2.Nodes[z]);
            }
            MessageBox.Show("ok");
        }
    }
}
