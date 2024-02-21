//查词程序
using System;
using System.IO;
using System.Net;
using System.Data;            // Use ADO.NET namespace
using System.Data.SqlClient;  // Use SQL Server data provider namespace
using System.Globalization;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Drawing.Text;    // 字体
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Collections;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

using System.Xml;

using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;

namespace pced
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public partial class Form1 : Form, IMessageFilter
    {
        //此处CharSet = CharSet.Auto在其它语言系统平台下会否有问题？
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int16 GetKeyState(Int32 nVirtKey);

        [DllImport("User32.dll")]
        protected static extern int SetClipboardViewer(int hWndNewViewer);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        //[DllImport("user32.dll", EntryPoint = "SendMessageA")]
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        IntPtr nextClipboardViewer;

        ///<summary>复合词查词模式开关 false关闭 true启动</summary>
        public static bool bFhcSwitch = false;

        ///<summary>查词模式标志 1表示只开启单独词查词模式 2表示开启单独词 变形词两种查词模式
        /// 3表示开启单独词 变形词 多种变形词三种查词模式
        /// 多种变形词功能耗费时间，效果微小，且未必正确，应废止多种变形词功能
        ///</summary>
        public static int ccmsbz = 1;

        public static string srCtext; //储存索引文件px的内容
        public static string strCxjg = ""; //储存查询结果(以词典的核心字母编码)
        public static string[] s = new string[60];

        ///<summary>是否使用前部匹配的模式查词</summary>
        bool bwordaheadmatch = true;

        /// <summary>
        /// 储存改变经文文本字体大小时对应的style风格字符串
        /// </summary>
        public static string strf1 = "";

        /// <summary>
        /// 巴利语词典数量
        /// </summary>
        public static int iPaliDictNum;

        /// <summary>
        /// 巴利语词典信息：标志、简称、说明
        /// </summary>
        public static string[] arStrPaliDictInfo;

        /// <summary>
        /// 巴利语查词输出，与出处词典对应
        /// </summary>
        public static string[] arStrPaliCcsc;

        /// <summary>
        /// 巴利语词典树
        /// </summary>
        public static TreeNode[] arTreeNodePali;

        /// <summary>
        /// 英语词典数量
        /// </summary>
        public static int iEnglishDictNum;

        /// <summary>
        /// 英语词典信息：标志、简称、说明
        /// </summary>
        public static string[] arStrEnglishDictInfo;

        /// <summary>
        /// 英语查词输出，与出处词典对应
        /// </summary>
        public static string[] arStrEnglishCcsc;

        /// <summary>
        /// 英语词典树
        /// </summary>
        public static TreeNode[] arTreeNodeEnglish;

        public Form1()
        {
            InitializeComponent();

            string strLinefan;
            int f = 0;
            StreamReader srfan = new StreamReader(new FileStream(@".\set\fan", FileMode.Open), System.Text.Encoding.GetEncoding(65001));
            strLinefan = srfan.ReadLine();
            while (strLinefan != null)
            {
                s[f] = Strings.StrConv(strLinefan, VbStrConv.TraditionalChinese, 0x0409);
                f++;
                strLinefan = srfan.ReadLine();
            }
            srfan.Close();

            nextClipboardViewer = (IntPtr)SetClipboardViewer((int)this.Handle);
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            // defined in winuser.h
            const int WM_DRAWCLIPBOARD = 0x308;
            const int WM_CHANGECBCHAIN = 0x030D;

            //因在繁体版windows系统中，与繁体版word里复制可能有冲突，所以设此开关来关闭或打开自动复制粘贴查词功能。
            if (!(menuosCopyAutoCc.Checked))
            {
                base.WndProc(ref m);
                return;
            }

            switch (m.Msg)
            {
                case WM_DRAWCLIPBOARD:
                    DisplayClipboardData();
                    SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    break;

                case WM_CHANGECBCHAIN:
                    if (m.WParam == nextClipboardViewer)
                        nextClipboardViewer = m.LParam;
                    else
                        SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        /// <summary>
        /// 复制标识，值为 1 表示复制命令是在词典窗口中查词结果显示框里发出，会激发三次DisplayClipboardData函数，
        /// 执行函数Clipboard.SetText(cbdText, TextDataFormat.UnicodeText);会激发两次DisplayClipboardData函数，
        /// 所以用值 -1 来标识它，不让DisplayClipboardData函数把复制内容自动往cboxInput框里粘帖
        /// </summary>
        public static int fzbz = 0;

        /// <summary>
        /// 剪贴板事件发生次数标志，因为copy_Click函数的运行会连续激发3次剪贴板事件，所以用它进行控制
        /// </summary>
        int cbN = 0;

        void DisplayClipboardData()
        {
            //MessageBox.Show("sss");
            try
            {
                if (fzbz == -1 && cbN < 1)
                {
                    cbN++;
                }
                else if (fzbz == -1 && cbN == 1)
                {
                    cbN = 0;
                    fzbz = 0;
                }
                else if (fzbz == 1 && cbN < 2)
                {
                    cbN++;
                }
                else if (fzbz == 1 && cbN == 2)
                {
                    cbN = 0;
                    fzbz = 0;
                }
                else
                {
                    IDataObject iData = new DataObject();
                    iData = Clipboard.GetDataObject();

                    if (iData.GetDataPresent(DataFormats.UnicodeText))
                    {
                        if (((string)iData.GetData(DataFormats.UnicodeText)).Length < 280)
                        {
                            cboxInput.Text = inword_t((string)iData.GetData(DataFormats.UnicodeText));
                            {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失

                            if (menuosCopyAutoCc.Checked)
                                btnLookup_Click(this, null);
                        }
                    }
                    /*
                else if (iData.GetDataPresent(DataFormats.Text))
                {
                    textBox3.Text = "TEXT";
                    if (((string)iData.GetData(DataFormats.Text)).Length < 280)
                    {
                        cboxInput.Text = (string)iData.GetData(DataFormats.UnicodeText);
                        {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失
                    }
                }
                    */
                    else if (iData.GetDataPresent(DataFormats.Rtf))
                    {
                        if (((string)iData.GetData(DataFormats.Rtf)).Length < 280)
                        {
                            richTextBox1.Rtf = (string)iData.GetData(DataFormats.Rtf);
                            cboxInput.Text = inword_t(richTextBox1.Text);
                            {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失

                            if (menuosCopyAutoCc.Checked)
                                btnLookup_Click(this, null);
                        }
                    }
                }
            }
            catch
            //catch (Exception e)
            {
                //在金山词霸划词取词时，会出异常，暂屏蔽之
                //MessageBox.Show(e.ToString());
            }
        }

        private void syCC()
        {
            string sbword = "";
            strCxjg = "";

            DateTime startD, endD;
            startD = DateTime.Now;

            if (cboxInput.Text.Trim() == "")
                return;

            int n = 0;  //记录查找到的结果条数
            string nword = "", saword = "";

            nword = inword();
            saword = nword;

            if (nword == "")
            {
                webBrowser1.DocumentText = "您输入的单词字符不是正确的巴利文罗马字母，请重新输入！";
                return;
            }

            if (nword.Length > 68)
            {
                webBrowser1.DocumentText = "本词典最长单词为68个字母，您输入的单词 " + nword + " 过长！\r\n请重新输入！";
                return;
            }

            webBrowser1.DocumentText = "looking up the word: " + nword + " ...";
            strCxjg = nword + "  " + "word translate:" + "\r\n\r\n";

            FileStream bFile = new FileStream(@".\pali-h\cidian", FileMode.Open);
            StreamReader srb = new StreamReader(bFile, System.Text.Encoding.GetEncoding("utf-8"));
            FileStream aF1m = new FileStream(@".\pali-h\indexdat", FileMode.Open);

            if (nword.Length < 4)
            {
                sbword = nword;

                byte[] bm = new byte[8] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                int um = 0;

                for (int im = 0; im < sbword.Length; im++)
                {
                    aF1m.Seek(um + Program.iABC(sbword.Substring(im, 1)) * 8, SeekOrigin.Begin);
                    aF1m.Read(bm, 0, 8);
                    if ((int)(bm[0]) == 0)
                    {
                        goto kkk;
                    }
                    else if ((int)(bm[0]) == 1)
                    {
                        if (im < sbword.Length - 1)
                            um = (int)(bm[1] | bm[2] << 8 | bm[3] << 16 | bm[4] << 24);
                        else
                        {
                            goto kkk;
                        }
                    }
                    else if ((int)(bm[0]) == 2)
                    {
                        if (im < sbword.Length - 1)
                        {
                            goto kkk;
                        }
                        else
                            um = (int)(bm[5] | bm[6] << 8 | bm[7] << 16 | 0);
                    }
                    else if ((int)(bm[0]) == 3)
                    {
                        if (im < sbword.Length - 1)
                            um = (int)(bm[1] | bm[2] << 8 | bm[3] << 16 | bm[4] << 24);
                        else
                            um = (int)(bm[5] | bm[6] << 8 | bm[7] << 16 | 0);
                    }
                }

                srb.DiscardBufferedData();//对于StreamReader来说，在使用ReadLine()后，如再要seek()则这一句很必要，但对于FileStream的使用，似乎就没有这个问题
                srb.BaseStream.Seek(um, System.IO.SeekOrigin.Begin);

                Regex re1m = new Regex(@"^[^, 。]+(,| |。)(?<w>.*)", RegexOptions.IgnoreCase);
                MatchCollection mc1m = re1m.Matches(srb.ReadLine());
                foreach (Match ma1m in mc1m)
                {
                     strCxjg = strCxjg + sbword.PadRight(nword.Length, ' ') + " " + ma1m.Groups["w"].Value + "\r\n";
                    n++;
                }

            kkk:
                ;
            }

            for (int i = 0; i < nword.Length; i++)
            {
                for (int j = saword.Length; j > 3; j--)
                {
                    sbword = saword.Substring(0, j);

                    byte[] bm = new byte[8] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    int um = 0;

                    for (int im = 0; im < sbword.Length; im++)
                    {
                        aF1m.Seek(um + Program.iABC(sbword.Substring(im, 1)) * 8, SeekOrigin.Begin);
                        aF1m.Read(bm, 0, 8);
                        if ((int)(bm[0]) == 0)
                        {
                            goto kkk;
                        }
                        else if ((int)(bm[0]) == 1)
                        {
                            if (im < sbword.Length - 1)
                                um = (int)(bm[1] | bm[2] << 8 | bm[3] << 16 | bm[4] << 24);
                            else
                            {
                                goto kkk;
                            }
                        }
                        else if ((int)(bm[0]) == 2)
                        {
                            if (im < sbword.Length - 1)
                            {
                                goto kkk;
                            }
                            else
                                um = (int)(bm[5] | bm[6] << 8 | bm[7] << 16 | 0);
                        }
                        else if ((int)(bm[0]) == 3)
                        {
                            if (im < sbword.Length - 1)
                                um = (int)(bm[1] | bm[2] << 8 | bm[3] << 16 | bm[4] << 24);
                            else
                                um = (int)(bm[5] | bm[6] << 8 | bm[7] << 16 | 0);
                        }
                    }

                    srb.DiscardBufferedData();//对于StreamReader来说，在使用ReadLine()后，如再要seek()则这一句很必要，但对于FileStream的使用，似乎就没有这个问题
                    srb.BaseStream.Seek(um, System.IO.SeekOrigin.Begin);

                    sbword = sbword.PadLeft(sbword.Length + i, ' ');
                    Regex re1m = new Regex(@"^[^, 。]+(,| |。)(?<w>.*)", RegexOptions.IgnoreCase);
                    MatchCollection mc1m = re1m.Matches(srb.ReadLine());
                    foreach (Match ma1m in mc1m)
                    {
                        strCxjg = strCxjg + sbword.PadRight(nword.Length, ' ') + " " + ma1m.Groups["w"].Value + "\r\n";
                        n++;
                    }

                kkk:
                    ;
                }
                saword = saword.Substring(1);
            }
            srb.Close();
            aF1m.Close();

            endD = DateTime.Now;
            System.TimeSpan ts = endD.Subtract(startD);
            strCxjg = strCxjg + "( " + ts.TotalMilliseconds.ToString() + " milliseconds.)";
            webBrowser1.DocumentText = outword(strCxjg);

            if (n == 0)
                webBrowser1.DocumentText = "\r\n没找到。";

            {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失
        }

        /// <summary>
        /// 分析pali单词的查找情况
        /// </summary>
        private void palitongji()
        {
            string strLine;
            string cp = "0";
            int p = 0, q = 0;

            StreamReader sr = new StreamReader(new FileStream(@".\2000word.txt", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@".\2000wordfx.txt", false, System.Text.Encoding.GetEncoding("utf-8"));
            strLine = sr.ReadLine();
            while (strLine != null)
            {
                cp = strLine.Substring(0, 7);
                strLine = strLine.Substring(7);
                palitongji_cz(strLine);

                sw.WriteLine(palitongji_zhbz + "%" + palitongji_bz + "%" + cp + "%" + strLine);

                p++;
                if (p == q + 10)
                {
                    q = q + 10;
                    textBox3.Text = p.ToString();
                    textBox3.Refresh();
                }

                strLine = sr.ReadLine();
            }

            sr.Close();
            sw.Close();
        }

        /// <summary>
        /// 对于查到的非组合词 1表示在词典里直接就查到的词 2表示词尾变形词
        /// </summary>
        public static string palitongji_bz = "0";

        /// <summary>
        /// 标志 0表示没有查到 1表示非组合词 2表示组合词
        /// </summary>
        public static string palitongji_zhbz = "0";

        /// <summary>
        /// 分析pali单词的查找情况，查词代码是复制的 旧palihan_ccFHC 函数
        /// </summary>
        /// <param name="strinword"></param>
        private void palitongji_cz(string strinword)
        {
            palitongji_zhbz = "0";
            palitongji_bz = "0";
            string pword = "", nword = "";
            pword = strinword;
            nword = palitongji_einword(strinword);

            if (pword.Length != nword.Length)
            {
                webBrowser1.DocumentText = "error! 请联系程序作者.";
                return;
            }

            strCxjg = "";

            swjgstr = "";
            for (int g = 0; g < 256; g++)
            {
                swjg[g] = "";
            }

            ccmsbz = 2; //设定为单独词 变形词两种查词模式

            if (palihan_cc(nword, pword))
                palitongji_zhbz = "1";
            else
            {
                if (pword.Length != nword.Length)
                    return;
                else
                {
                    int i;
                    int k = 0;
                    int jst = 0;
                    for (i = 0; i < pword.Length; i++)
                    {
                        if (pword.Length - i < 16)
                            jst = pword.Length - i - 1;
                        else
                            jst = 15 - 1;

                        for (int j = jst; j > -1; j--)
                        {
                            if (j + i == pword.Length - 1)
                                ccmsbz = 2;
                            else
                                ccmsbz = 1;

                            if (palihan_cc(nword.Substring(i, j + 1), pword.Substring(i, j + 1)))
                            {
                                swjgstr = swjgstr + i.ToString().PadLeft(3, '0') + (j + 1).ToString().PadLeft(3, '0') + "-" + pword.Substring(i, j + 1) + "\r\n";
                                swjg[i] = pword.Substring(i, j + 1);
                                if (pword.Substring(i, j + 1).Length > 2)
                                {
                                    if (k == 0)
                                    {
                                        i = i + pword.Substring(i, j + 1).Length - 2 - 1;
                                        k = 3;
                                    }
                                }
                                break;
                            }
                        }

                        if (k > 0)
                            k--;
                    }

                    if (swjgstr != "")
                    {
                        string strswjg = "";
                        if (swjg[0] != "")
                        {
                            for (int g = 0; g < pword.Length; g++)
                            {
                                if (swjg[g] != "")
                                {
                                    if (g == 0)
                                        strswjg = swjg[g];
                                    else
                                        strswjg = strswjg + "-" + swjg[g];
                                    g = g + swjg[g].Length - 1;
                                }
                                else
                                {
                                    strswjg = "";
                                    break;
                                }
                            }
                        }

                        if (strswjg == "")
                        {
                            strCxjg = "";
                        }
                        else
                        {
                            palitongji_zhbz = "2";
                            strCxjg = strswjg;
                        }
                    }
                }
            }
        }

        //用于 palitongji_cz 函数
        string palitongji_einword(string strinword)
        {
            string aword;
            aword = strinword;

            char[] ca = "āīūṅñṭḍṇḷŋĀĪŪṄÑṬḌṆḶŊṁṃṀṂ".ToCharArray();
            char[] cb = "aiunntdnlmAIUNNTDNLMmmMM".ToCharArray();
            int i = 0;
            foreach (char c in ca)
            {
                aword = new Regex(c.ToString(), RegexOptions.None).Replace(aword, cb[i].ToString());
                i++;
            }

            return aword;
        }

        //采用全部循环的方式，并且查找所有分词的多种变形（这种做法不正确），极为费时，不再使用
        private void palihan_ccFHC_allxhcz()
        {
            if (cboxInput.Text.Trim() == "")
                return;

            if (cboxInput.Text.Trim().Length > 250)
            {
                webBrowser1.DocumentText = "本词典最长单词为250个字母，您输入的单词过长！\r\n请重新输入！";
                return;
            }

            string nword = "", pword = "";
            pword = inword();
            nword = einword();
            if (nword == "")
            {
                webBrowser1.DocumentText = "您输入的单词字符不是正确的巴利文罗马字母，请重新输入！\r\n可能您font设置有误,请试试选择其它font再查!";
                return;
            }

            webBrowser1.DocumentText = "正在查找单词： " + pword + " ...";

            strCxjg = "";
            string cxa = "", cxb = "";
            if (palihan_cc(nword, pword))
                webBrowser1.DocumentText = outword(strCxjg);
            else
            {
                //词长大于40的单词不用组合查找，因速度会非常慢，词长<=40的词已经占了三藏注疏单词量的99%,总词量的99.9%
                if (pword.Length < 41)
                {
                    if (pword.Length != nword.Length)
                    {
                        webBrowser1.DocumentText = "您输入的单词字符不是正确的巴利文罗马字母，请重新输入！\r\n可能您font设置有误,请试试选择其它font再查!";
                        return;
                    }
                    else
                    {
                        int numcxjg = 0;//结果词条限定
                        int numword = 0, t = 0;
                        int dcl; //单词长度
                        dcl = pword.Length;
                        numword = 1000;
                        string[] strP = new string[numword];
                        string[] strpWord = new string[dcl];
                        string[] strnWord = new string[dcl];
                        string[] strTmp = new string[numword];

                        string[] strDC = new string[numword]; //单词
                        bool bcq = false, bch = false;//单词的前后两半段是否查到

                        string ysc; //原始词
                        string[] strCw = new string[3]; //词尾
                        for (int i = 0; i < 3; i++)
                        {
                            strCw[i] = "";
                        }
                        bool bCw = false; //词尾重复标志
                        string[] strPcwtmp = new string[dcl]; //词尾
                        string[] strNcwtmp = new string[dcl]; //词尾
                        bool[] strCwczbz = new bool[dcl]; //词尾查找结果标志,值为true表示已经查找过1次,不必再次查找   
                        int czbz = 0; //查找结果标志 0表示没查到 1表示查到
                        int v = 0; //单词定位索引
                        int[,] iDCJS = new int[dcl, dcl]; //单词解释状态 0表示还没有查过 1表示查到了 2表示没查到,下标是在原始词里的开始与结束位置
                        string[,] strDCJS = new string[dcl, dcl]; //单词解释,下标是在原始词里的开始与结束位置
                        for (int i = 0; i < dcl; i++)
                        {
                            for (int j = 0; j < dcl; j++)
                            {
                                strDCJS[i, j] = "";
                                iDCJS[i, j] = 0;
                            }
                        }

                        t = 0;
                        for (int m = 0; m < dcl; m++)
                        {
                            strpWord[m] = "";
                            strnWord[m] = "";
                            strPcwtmp[m] = "";
                            strNcwtmp[m] = "";
                            strCwczbz[m] = false;
                        }

                        ysc = pword;
                        strpWord[0] = pword;
                        strnWord[0] = nword;
                    xl:
                        czbz = 0;
                        for (int m = 0; m < dcl; m++)
                        {
                            if (strpWord[m] == "")
                                break;
                            pword = strpWord[m];
                            nword = strnWord[m];
                            for (int z = pword.Length - 3; z > 0; z--)
                            {
                                strCxjg = "";
                                v = dcl - pword.Length;
                                if (iDCJS[v, v + z] == 1)
                                {
                                    bcq = true;
                                    strCxjg = strDCJS[v, v + z];
                                }
                                else if (iDCJS[v, v + z] == 2)
                                {
                                    bcq = false;
                                }
                                else
                                {
                                    bcq = palihan_cc(nword.Substring(0, z + 1), pword.Substring(0, z + 1));
                                }
                                if (bcq)
                                {
                                    cxa = strCxjg;

                                    strDCJS[v, v + z] = cxa;
                                    iDCJS[v, v + z] = 1;
                                    strCxjg = "";

                                    if (iDCJS[v + z + 1, dcl - 1] == 1)
                                    {
                                        bch = true;
                                        strCxjg = strDCJS[v + z + 1, dcl - 1];
                                    }
                                    else if (iDCJS[v + z + 1, dcl - 1] == 2)
                                    {
                                        bch = false;
                                    }
                                    else
                                    {
                                        bch = palihan_cc(nword.Substring(z + 1), pword.Substring(z + 1));
                                    }

                                    if (bch)
                                    {
                                        cxb = strCxjg;

                                        strDCJS[v + z + 1, dcl - 1] = cxb;
                                        iDCJS[v + z + 1, dcl - 1] = 1;

                                        //储存找到的且不重复的词尾,最多3个
                                        bCw = false;
                                        for (int i = 0; i < 3; i++)
                                        {
                                            if (strCw[i] == pword.Substring(z + 1))
                                            {
                                                bCw = true;
                                                break;
                                            }
                                        }
                                        if (!bCw)
                                        {
                                            for (int i = 0; i < 3; i++)
                                            {
                                                if (strCw[i] == "")
                                                {
                                                    strCw[i] = pword.Substring(z + 1);
                                                    break;
                                                }
                                            }
                                        }

                                        if (++numcxjg == 3)
                                            goto scjg;
                                    }
                                    else
                                    {
                                        iDCJS[v + z + 1, dcl - 1] = 2;
                                        czbz = 1;

                                        strPcwtmp[v + z + 1] = pword.Substring(z + 1);
                                        strNcwtmp[v + z + 1] = nword.Substring(z + 1);
                                    }
                                }
                                else
                                {
                                    iDCJS[v, v + z] = 2;
                                }
                            }
                        }

                        if (czbz == 0)
                        {
                            goto scjg;
                        }
                        else
                        {
                            //清空数组
                            for (int m = 0; m < dcl; m++)
                            {
                                strpWord[m] = "";
                                strnWord[m] = "";
                            }

                            t = 0;
                            for (int m = 0; m < dcl; m++)
                            {
                                if (strPcwtmp[m] != "")
                                {
                                    strpWord[t] = strPcwtmp[m];
                                    strnWord[t] = strNcwtmp[m];
                                    t++;

                                    //清空临时数组
                                    strPcwtmp[m] = "";
                                    strNcwtmp[m] = "";
                                }
                            }
                            goto xl;
                        }

                        //输出结果
                    scjg:
                        if (numcxjg == 0)
                        {
                            webBrowser1.DocumentText = "没找到!";
                            return;
                        }

                        string strscjg = ""; //输出结果

                        int iscct = 0; //输出词条数限制
                        string stxt = "";
                        string stxttmp = "";
                        string swordtmp = "";
                        string swordztmp = "";
                        stxt = strCw[0].PadLeft(250, ' ') + strCw[0].PadLeft(500, ' ');
                    jw:
                        for (int i = 0; i < stxt.Length; i = i + 750)
                        {
                            swordtmp = stxt.Substring(i, 250).TrimStart();
                            swordztmp = stxt.Substring(i + 250, 500).TrimStart();

                            int ijw = 0;//单词结尾索引

                            ijw = ysc.Length - swordtmp.Length - 1;

                            for (int j = ijw; j > -1; j--)
                            {
                                if (iDCJS[j, ijw] == 1)
                                {
                                    if (j > 0)
                                        stxttmp = (ysc.Substring(j, ijw + 1 - j) + swordtmp).PadLeft(250, ' ') + (ysc.Substring(j, ijw + 1 - j) + "-" + swordztmp).PadLeft(500, ' ') + stxttmp;
                                    else
                                    {
                                        if (iscct > 0)
                                            strscjg = strscjg + "\r\n" + ysc.Substring(j, ijw + 1 - j) + "-" + swordztmp;
                                        else
                                            strscjg = ysc.Substring(j, ijw + 1 - j) + "-" + swordztmp;

                                        if (++iscct == 3)
                                        {
                                            strCxjg = strscjg;
                                            textBox3.Text = strCxjg;
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                        if (stxttmp != "")
                        {
                            stxt = stxttmp;
                            stxttmp = "";
                            goto jw;
                        }
                        strCxjg = strscjg;
                        textBox3.Text = strCxjg;
                    }
                }
                else
                    webBrowser1.DocumentText = "没找到!";
            }
        }

        string e_inword_lianyin(string word)
        {
            string PLabc = "āīūṅñṭḍṇḷŋĀĪŪṄÑṬḌṆḶŊṁṃṀṂ";
            string ENabc = "aiunntdnlmAIUNNTDNLMmmMM";
            for (int i = 0; i < 24; i++)
            {
                if (word == PLabc[i].ToString())
                    return ENabc[i].ToString();
            }
            return word;
        }

        //处理连音
        private int lianyin(string nword, string pword, int i, int jbz, string word)
        {
            //int iarrsl = 57;
            int iarrsl = 72;
            string[] arrsLianYina = new string[iarrsl];
            string[] arrsLianYinb = new string[iarrsl];
            string[] arrsLianYinc = new string[iarrsl];
            int iarr = 0;
            string strLine = "";
            StreamReader sr = new StreamReader(new FileStream(@".\ziwei\lianyin.txt", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            strLine = sr.ReadLine();
            while (strLine != null)
            {
                arrsLianYina[iarr] = strLine.Substring(0, 1);
                arrsLianYinb[iarr] = strLine.Substring(1, 1);
                arrsLianYinc[iarr] = strLine.Substring(2);
                iarr++;
                strLine = sr.ReadLine();
            }
            sr.Close();

            int jst, jstb = 0;
            string snword = "", spword = "";

            if (pword.Length < 16)
                jst = pword.Length - 1;
            else
                jst = 15 - 1;

            //for (int j = jst; j > 2; j--)
            for (int j = jst; j > 1; j--)
            {
                //1前除后同2前除后长3前同后除4前长后除5前后相合
                for (int k = 0; k < 45; k++)
                {
                    //if (pword.Substring(j, 1) == "ā" && i + j < word.Length - 1)
                    if (pword.Substring(j, 1) == arrsLianYinc[k] && i + j < word.Length - 1)
                    {
                        snword = nword.Substring(0, j) + e_inword_lianyin(arrsLianYina[k]);
                        spword = pword.Substring(0, j) + arrsLianYina[k];
                        if (palihan_cc(snword, spword))
                        {
                            if (pword.Length - j - 1 < 15)
                                jstb = pword.Length - j - 1;
                            else
                                jstb = 15 - 1;
                            for (int n = jstb; n > 1; n--)
                            {
                                if (i + j + n == word.Length - 1)
                                    ccmsbz = 2;
                                else
                                    ccmsbz = 1;
                                if (palihan_cc(e_inword_lianyin(arrsLianYinb[k]) + nword.Substring(j + 1, n), arrsLianYinb[k] + pword.Substring(j + 1, n)))
                                {
                                    swjg[i] = spword;
                                    swjglen[i] = spword.Length;

                                    swjg[i + j + 1] = arrsLianYinb[k] + pword.Substring(j + 1, n);
                                    swjglen[i + j + 1] = pword.Substring(j + 1, n).Length;

                                    swjgstr = swjgstr + i.ToString().PadLeft(3, '0') + swjglen[i].ToString().PadLeft(3, '0') + "-" + swjg[i] + "\r\n";
                                    swjgstr = swjgstr + (i + j + 1).ToString().PadLeft(3, '0') + swjglen[i + j + 1].ToString().PadLeft(3, '0') + "-" + swjg[i + j + 1] + "\r\n";


                                    webBrowser1.DocumentText = e_inword_lianyin(arrsLianYinb[k]) + nword.Substring(j + 1, n) + "#" + arrsLianYinb[k] + pword.Substring(j + 1, n) + "\r\n";
                                    webBrowser1.DocumentText = webBrowser1.DocumentText + i.ToString() + "#" + (i + j + 1).ToString() + "\r\n";
                                    webBrowser1.DocumentText = webBrowser1.DocumentText + swjg[i] + "#" + swjg[i + j + 1] + "\r\n";
                                    webBrowser1.DocumentText = webBrowser1.DocumentText + swjglen[i].ToString() + "#" + swjglen[i + j + 1].ToString() + "\r\n";

                                    return j + n;
                                }
                            }
                        }
                    }
                }
                //
                //6前半母后同7前半母后长
                for (int k = 45; k < 57; k++)
                {
                    if (i + j < word.Length - 2)
                    {
                        if (pword.Substring(j, 2) == arrsLianYinc[k])
                        {
                            snword = nword.Substring(0, j) + e_inword_lianyin(arrsLianYina[k]);
                            spword = pword.Substring(0, j) + arrsLianYina[k];
                            if (palihan_cc(snword, spword))
                            {
                                if (pword.Length - j - 1 - 1 < 15)
                                    jstb = pword.Length - j - 1 - 1;
                                else
                                    jstb = 15 - 1;
                                for (int n = jstb; n > 1; n--)
                                {
                                    if (i + j + n == word.Length - 1)
                                        ccmsbz = 2;
                                    else
                                        ccmsbz = 1;
                                    if (palihan_cc(e_inword_lianyin(arrsLianYinb[k]) + nword.Substring(j + 2, n), arrsLianYinb[k] + pword.Substring(j + 2, n)))
                                    {
                                        swjg[i] = spword;
                                        swjglen[i] = spword.Length;

                                        swjg[i + j + 1] = arrsLianYinb[k] + pword.Substring(j + 2, n);
                                        swjglen[i + j + 1] = n + 1;

                                        swjgstr = swjgstr + i.ToString().PadLeft(3, '0') + swjglen[i].ToString().PadLeft(3, '0') + "-" + swjg[i] + "\r\n";
                                        swjgstr = swjgstr + (i + j + 1).ToString().PadLeft(3, '0') + swjglen[i + j + 1].ToString().PadLeft(3, '0') + "-" + swjg[i + j + 1] + "\r\n";


                                        webBrowser1.DocumentText = e_inword_lianyin(arrsLianYinb[k]) + nword.Substring(j + 2, n) + "#" + arrsLianYinb[k] + pword.Substring(j + 2, n) + "\r\n";
                                        webBrowser1.DocumentText = webBrowser1.DocumentText + i.ToString() + "#" + (i + j + 1).ToString() + "\r\n";
                                        webBrowser1.DocumentText = webBrowser1.DocumentText + swjg[i] + "#" + swjg[i + j + 1] + "\r\n";
                                        webBrowser1.DocumentText = webBrowser1.DocumentText + swjglen[i].ToString() + "#" + swjglen[i + j + 1].ToString() + "\r\n";

                                        return j + n + 1;
                                    }
                                }
                            }
                        }
                    }
                }
                //
                //第[6]大类连音：鼻音＋子音，－ṃ ＋ 子音－，-ṃ变成该群鼻音。整理自《巴利语入门》
                for (int k = 57; k < 72; k++)
                {
                    if (i + j < word.Length - 2)
                    {
                        if (pword.Substring(j, 2) == arrsLianYinc[k])
                        {
                            snword = nword.Substring(0, j) + e_inword_lianyin(arrsLianYina[k]);
                            spword = pword.Substring(0, j) + arrsLianYina[k];
                            if (palihan_cc(snword, spword))
                            {
                                if (pword.Length - j - 1 - 1 < 15)
                                    jstb = pword.Length - j - 1 - 1;
                                else
                                    jstb = 15 - 1;
                                for (int n = jstb; n > 1; n--)
                                {
                                    if (i + j + n == word.Length - 1)
                                        ccmsbz = 2;
                                    else
                                        ccmsbz = 1;
                                    if (palihan_cc(e_inword_lianyin(arrsLianYinb[k]) + nword.Substring(j + 2, n), arrsLianYinb[k] + pword.Substring(j + 2, n)))
                                    {
                                        swjg[i] = spword;
                                        swjglen[i] = spword.Length;

                                        swjg[i + j + 1] = arrsLianYinb[k] + pword.Substring(j + 2, n);
                                        swjglen[i + j + 1] = n + 1;

                                        swjgstr = swjgstr + i.ToString().PadLeft(3, '0') + swjglen[i].ToString().PadLeft(3, '0') + "-" + swjg[i] + "\r\n";
                                        swjgstr = swjgstr + (i + j + 1).ToString().PadLeft(3, '0') + swjglen[i + j + 1].ToString().PadLeft(3, '0') + "-" + swjg[i + j + 1] + "\r\n";


                                        webBrowser1.DocumentText = e_inword_lianyin(arrsLianYinb[k]) + nword.Substring(j + 2, n) + "#" + arrsLianYinb[k] + pword.Substring(j + 2, n) + "\r\n";
                                        webBrowser1.DocumentText = webBrowser1.DocumentText + i.ToString() + "#" + (i + j + 1).ToString() + "\r\n";
                                        webBrowser1.DocumentText = webBrowser1.DocumentText + swjg[i] + "#" + swjg[i + j + 1] + "\r\n";
                                        webBrowser1.DocumentText = webBrowser1.DocumentText + swjglen[i].ToString() + "#" + swjglen[i + j + 1].ToString() + "\r\n";

                                        return j + n + 1;
                                    }
                                }
                            }
                        }
                    }
                }
                //
            }
            swjg[i] = word.Substring(i, jbz + 1);
            swjglen[i] = jbz + 1;
            swjgstr = swjgstr + i.ToString().PadLeft(3, '0') + (jbz + 1).ToString().PadLeft(3, '0') + "-" + swjg[i] + "\r\n";
            return jbz;
        }

        string swjgstr = "";
        string[] swjg = new string[256];
        int[] swjglen = new int[256];

        string[] arrStrCcls = new string[50];

        /// <summary>
        /// 历史词条定位
        /// </summary>
        int iCclsDw = 0;

        /// <summary>
        /// 按的回车键或是点击的‘查词’按钮，只有在此两种情况下，才把词记录入查词历史
        /// </summary>
        bool _isEnter = false;

        private void palihan_ccFHC(string cboxInputWord)
        {
            if (cboxInputWord.Trim() == "")
                return;

            if (cboxInputWord.Trim().Length > 256)
            {
                webBrowser1.DocumentText = "本词典最长单词为256个字母，您输入的单词过长！<br>请重新输入！";
                return;
            }

            string nword = "", pword = "";
            pword = inword(cboxInputWord);
            nword = einword(cboxInputWord);
            if (nword == "")
            {
                webBrowser1.DocumentText = "您输入的单词字符不是正确的巴利文罗马字母，请重新输入！\r\n可能您font设置有误,请试试选择其它font再查!";
                return;
            }

            //记录查词历史
            if (_isEnter && iCclsDw == 0)
            {
                _isEnter = false;

                string[] arrStrCcls_Tmp = new string[50];
                for (int t = 0; t < 50; t++)
                {
                    if (arrStrCcls[t] == pword)
                        arrStrCcls[t] = "";
                    arrStrCcls_Tmp[t] = arrStrCcls[t];
                }
                for (int t = 0; t < 50; t++)
                    arrStrCcls[t] = "";
                arrStrCcls[0] = pword;
                int c_Tmp = 1;
                for (int t = 0; t < 50; t++)
                {
                    if (c_Tmp < 50 && arrStrCcls_Tmp[t] != "")
                    {
                        arrStrCcls[c_Tmp] = arrStrCcls_Tmp[t];
                        c_Tmp++;
                    }
                }
            }
            //

            webBrowser1.DocumentText = "正在查找单词： " + pword + " ...";

            strCxjg = "";
            swjgstr = "";

            for (int g = 0; g < 256; g++)
            {
                swjg[g] = "";
                swjglen[g] = 0;
            }

            //ccmsbz = 2; //设定为单独词 变形词两种查词模式

            if (palihan_cc(nword, pword))
            {
                //webBrowser1.DocumentText = outword(strCxjg);
                closefhctextbox_t();
                return;
            }
            else
            {
                strCxjg = ""; //把 strCxjg 清空是为了防止 htmlListOut() 函数脚本出错，strCxjg为空则不运行htmlListOut() 函数

                if (bFhcSwitch)
                {
                    if (pword.Length != nword.Length)
                    {
                        _isInputError = true;
                        return;
                    }
                    else
                    {
                        int i;
                        int jst = 0;
                        for (i = 0; i < pword.Length; i++)
                        {
                            if (pword.Length - i < 16)
                                jst = pword.Length - i - 1;
                            else
                                jst = 15 - 1;

                            for (int j = jst; j > -1; j--)
                            {
                                if (j + i == pword.Length - 1)
                                    ccmsbz = 2;
                                else
                                    ccmsbz = 1;

                                if (palihan_cc(nword.Substring(i, j + 1), pword.Substring(i, j + 1)))
                                {
                                    //处理连音
                                    int arrtmp = 0;
                                    if (pword.Substring(i, j + 1).Length < 4)
                                    {
                                        arrtmp = lianyin(nword.Substring(i), pword.Substring(i), i, j, pword);
                                        if (arrtmp > 0)
                                        {
                                            j = arrtmp;
                                            i = i + j;

                                            break;
                                        }
                                        else
                                            continue;
                                    }
                                    //
                                    swjgstr = swjgstr + i.ToString().PadLeft(3, '0') + (j + 1).ToString().PadLeft(3, '0') + "-" + pword.Substring(i, j + 1) + "\r\n";
                                    swjg[i] = pword.Substring(i, j + 1);
                                    swjglen[i] = pword.Substring(i, j + 1).Length;

                                    i = i + j;
                                    break;
                                }
                            }
                        }

                        if (swjgstr != "")
                        {
                            string strswjg = "";
                            if (swjg[0] != "")
                            {
                                for (int g = 0; g < pword.Length; g++)
                                {
                                    if (swjg[g] != "")
                                    {
                                        if (g == 0)
                                            strswjg = swjg[g];
                                        else
                                            strswjg = strswjg + "-" + swjg[g];
                                        g = g + swjglen[g] - 1;
                                    }
                                    else
                                    {
                                        strswjg = "";
                                        break;
                                    }
                                }
                            }
                            if (strswjg == "")
                                textBox3.Text = swjgstr;
                            else
                                textBox3.Text = "000000-" + strswjg + "\r\n" + swjgstr;

                            showfhctextbox();

                            _isFhc = true;
                        }
                    }
                    strCxjg = "";
                }
                else
                {
                    //把 strCxjg 清空是为了防止 htmlListOut() 函数脚本出错，strCxjg为空则不运行htmlListOut() 函数
                    strCxjg = "";
                }
            }
        }

        /// <summary>
        /// 当查词结果是复合词时，值为true
        /// </summary>
        public static bool _isFhc = false;

        /// <summary>
        /// 值为true表示输入错误，初始值为false
        /// </summary>
        public static bool _isInputError = false;

        private bool palihan_cc(string nword, string pword)
        {
            treeView1.Nodes["trRoot"].Nodes.Clear();

            strPaliWord = pword;

            int n = 0;  //记录查找结果条数
            int sNo = -1, lNo = -1;
            int ibz = -3;

            if (!bwordaheadmatch)
            {
                ibz = Program.edcNo(nword, out Program.iNo);
                if (ibz == 0)
                {
                    sNo = Program.iNo;
                    while ((sNo > 0) && !Program.eab(Program.sL[sNo - 1], nword))
                        sNo = sNo - 1;
                    lNo = sNo;
                    while ((lNo < Program.NUM - 1) && !Program.eab(nword, Program.sL[lNo + 1]))
                        lNo = lNo + 1;
                }
                else
                {
                    if (ziweiCc(pword))
                        return true;
                    else
                        return false;
                }
            }
            else
            {
                ibz = Program.edcNo(nword, out Program.iNo);
                if (ibz == -2)
                {
                    if (ziweiCc(pword))
                        return true;
                    else
                        return false;
                }
                if (ibz == 0)
                {
                    sNo = Program.iNo;
                    while ((sNo > 0) && !Program.eab(Program.sL[sNo - 1], nword))
                        sNo = sNo - 1;
                }
                if (ibz == 1)
                {
                    sNo = Program.iNo + 1;
                }
                if (ibz == -1)
                {
                    sNo = 0;
                }

                ibz = Program.edcNo(nword.PadRight(68, 'z'), out Program.iNo);
                if (ibz == -1)
                {
                    if (ziweiCc(pword))
                        return true;
                    else
                        return false;
                }
                if (ibz == 0)
                {
                    lNo = Program.iNo;
                    while ((lNo < Program.NUM - 1) && !Program.eab(nword.PadRight(68, 'z'), Program.sL[lNo + 1]))
                        lNo = lNo + 1;
                }
                if (ibz == 1)
                {
                    lNo = Program.iNo;
                }
                if (ibz == -2)
                {
                    lNo = Program.NUM - 1;
                }
            }

            string sBz, pword_tmp;

            for (int v = 0; v < iPaliDictNum; v++)
            {
                arStrPaliCcsc[v] = "";
            }

            sBz = "";

            pword_tmp = "";

            webBrowser1.DocumentText = "";

            int No = 0;

            if (!_ishtmlListOut)  //如果查词请求不是 htmlListOut 发出的
            {
                No = sNo;
                while (No <= lNo)
                {
                    sBz = Program.strL[No].Substring(0, 1);
                    if (!menuosBlurinputmode.Checked)
                    {
                        MatchCollection mc = new Regex(@"\w%" + pword + ".*", RegexOptions.IgnoreCase).Matches(Program.strL[No]);
                        foreach (Match ma in mc)
                        {
                            for (int v = 0; v < iPaliDictNum; v++)
                            {
                                if (sBz == arStrPaliDictInfo[v].Substring(1, 1))
                                    arStrPaliCcsc[v] = arStrPaliCcsc[v] + "<> " + Program.strL[No].Substring(2) + "$\r\n";
                            }
                            //if (sBz == "D")
                            //sD = sD + "<> " + Program.strL[No].Substring(2) + "$\r\n";
                            n++;
                        }

                        MatchCollection mcp = new Regex("ṃ", RegexOptions.IgnoreCase).Matches(pword);
                        if (mcp.Count > 0)
                        {
                            pword_tmp = pword;
                            pword_tmp = new Regex("ṃ", RegexOptions.IgnoreCase).Replace(pword_tmp, "ṁ");
                            MatchCollection mc2 = new Regex(@"\w%" + pword_tmp + ".*", RegexOptions.IgnoreCase).Matches(Program.strL[No]);
                            foreach (Match ma in mc2)
                            {
                                for (int v = 0; v < iPaliDictNum; v++)
                                {
                                    if (sBz == arStrPaliDictInfo[v].Substring(1, 1))
                                        arStrPaliCcsc[v] = arStrPaliCcsc[v] + "<> " + Program.strL[No].Substring(2) + "$\r\n";
                                }
                                //if (sBz == "D")
                                //sD = sD + "<> " + Program.strL[No].Substring(2) + "$\r\n";
                                n++;
                            }
                            pword_tmp = pword;
                            pword_tmp = new Regex("ṃ", RegexOptions.IgnoreCase).Replace(pword_tmp, "ŋ");
                            MatchCollection mc3 = new Regex(@"\w%" + pword_tmp + ".*", RegexOptions.IgnoreCase).Matches(Program.strL[No]);
                            foreach (Match ma in mc3)
                            {
                                for (int v = 0; v < iPaliDictNum; v++)
                                {
                                    if (sBz == arStrPaliDictInfo[v].Substring(1, 1))
                                        arStrPaliCcsc[v] = arStrPaliCcsc[v] + "<> " + Program.strL[No].Substring(2) + "$\r\n";
                                }
                                //if (sBz == "D")
                                //sD = sD + "<> " + Program.strL[No].Substring(2) + "$\r\n";
                                n++;
                            }
                        }
                    }
                    else
                    {
                        for (int v = 0; v < iPaliDictNum; v++)
                        {
                            if (sBz == arStrPaliDictInfo[v].Substring(1, 1))
                                arStrPaliCcsc[v] = arStrPaliCcsc[v] + "<> " + Program.strL[No].Substring(2) + "$\r\n";
                        }
                        //if (sBz == "D")
                        //sD = sD + "<> " + Program.strL[No].Substring(2) + "$\r\n";
                        n++;
                    }
                    No++;
                }
            }
            else
            {
                //储存以下值，以备 listBatchOut() 函数 输出时使用
                listFirstNo = sNo;
                listEndNo = lNo;
                return true;
            }

            for (int v = 0; v < iPaliDictNum; v++)
            {
                if (arStrPaliCcsc[v] != "")
                {
                    arStrPaliCcsc[v] = "<font id='" + arStrPaliDictInfo[v].Substring(1, 1) + "' color=#4169E1>" + arStrPaliDictInfo[v].Substring(3, 25).Trim() + "</font><br>\r\n" + arStrPaliCcsc[v] + "<br>\r\n";
                    arTreeNodePali[v] = new TreeNode(outword(arStrPaliDictInfo[v].Substring(3, 25).Trim()));
                    arTreeNodePali[v].Name = "nod" + arStrPaliDictInfo[v].Substring(1, 1);
                    arTreeNodePali[v].ToolTipText = outword(arStrPaliDictInfo[v].Substring(29));
                    if (arStrPaliDictInfo[v].Substring(0, 1) == "E")
                        arTreeNodePali[v].BackColor = Color.LightYellow;
                    treeView1.Nodes["trRoot"].Nodes.Add(arTreeNodePali[v]);
                }
            }
            /*
            if (sD != "")
            {
                sD = "<font id='D' color=#4169E1>《巴汉词典》</font><br>\r\n" + sD + "<br>\r\n";
                TreeNode trNodD = new TreeNode(outword("《巴汉词典》"));
                trNodD.Name = "nodD";
                trNodD.ToolTipText = outword("Mahāñāṇo Bhikkhu编著");
                treeView1.Nodes["trRoot"].Nodes.Add(trNodD);
            }
            */
            if (EN.Checked)
            {
                for (int v = 0; v < iPaliDictNum; v++)
                {
                    if (arStrPaliDictInfo[v].Substring(0, 1) == "E")
                        strCxjg = strCxjg + arStrPaliCcsc[v];
                }
                for (int v = 0; v < iPaliDictNum; v++)
                {
                    if (arStrPaliDictInfo[v].Substring(0, 1) == "C")
                        strCxjg = strCxjg + arStrPaliCcsc[v];
                }
                //strCxjg = strCxjg + sN + sP + sC + sM + sD + sF + sW + sL + sG + sZ + sX;
            }
            else
            {
                for (int v = 0; v < iPaliDictNum; v++)
                {
                    if (arStrPaliDictInfo[v].Substring(0, 1) == "C")
                        strCxjg = strCxjg + arStrPaliCcsc[v];
                }
                for (int v = 0; v < iPaliDictNum; v++)
                {
                    if (arStrPaliDictInfo[v].Substring(0, 1) == "E")
                        strCxjg = strCxjg + arStrPaliCcsc[v];
                }
                //strCxjg = strCxjg + sM + sD + sF + sW + sL + sG + sZ + sX + sN + sP + sC;
            }

            //如果tsmiBxc菜单项没有被选中
            if (!(tsmiBxc.Checked))
            {
                //对于在词库里直接查找不到的词，进行词尾变形查找
                if (n == 0)
                {
                    if (ziweiCc(pword))
                        return true;
                    else
                        return false;
                }
                else
                {
                    palitongji_bz = "1";
                    return true;
                }
            }
            else
            {
                //即使是在词库里直接就查找到的词，也进行词尾变形查找
                if (ziweiCc(pword))
                    return true;
                else
                {
                    if (n == 0)
                    {
                        return false;
                    }
                    else
                    {
                        palitongji_bz = "1";
                        return true;
                    }
                }
            }
        }

        private void en_pali_cc(string strInputWord)
        {
            strCxjg = "";

            if (strInputWord.Trim() == "")
                return;

            if (strInputWord.Trim().Length > 34)
            {
                webBrowser1.DocumentText = " inputed characters too more! <br>\r\n the most long word is 34 characters in this english-pali dictionary, <br>\r\n please re-input.";
                return;
            }

            int n = 0;  //记录查找到的结果条数
            string nword = "";
            nword = ewinword(strInputWord);

            if (nword == "")
            {
                webBrowser1.DocumentText = " inputed character is wrong character! <br>\r\n please re-input.";
                return;
            }

            treeView1.Nodes["trRoot"].Nodes.Clear();
            strPaliWord = nword;

            webBrowser1.DocumentText = "looking up the word: " + nword + " ...";

            int sNo = -1, lNo = -1;
            int ibz = -3;

            if (bwordaheadmatch)
            {
                ibz = Program.edcNo(nword, out Program.iNo);
                if (ibz == -2)
                {
                    strCxjg = "No found.";
                    return;
                }
                if (ibz == 0)
                {
                    sNo = Program.iNo;
                }
                if (ibz == 1)
                {
                    sNo = Program.iNo + 1;
                }
                if (ibz == -1)
                {
                    sNo = 0;
                }

                ibz = Program.edcNo(nword.PadRight(34, 'z'), out Program.iNo);
                if (ibz == -1)
                {
                    strCxjg = "No found.";
                    return;
                }
                if (ibz == 0)
                {
                    lNo = Program.iNo;
                }
                if (ibz == 1)
                {
                    lNo = Program.iNo;
                }
                if (ibz == -2)
                {
                    lNo = Program.NUM - 1;
                }
            }
            else
            {
                ibz = Program.edcNo(nword, out Program.iNo);
                if (ibz == 0)
                {
                    sNo = Program.iNo;
                    while ((sNo > 0) && !Program.eab(Program.sL[sNo - 1], nword))
                        sNo = sNo - 1;
                    lNo = sNo;
                    while ((lNo < Program.NUM - 1) && !Program.eab(nword, Program.sL[lNo + 1]))
                        lNo = lNo + 1;
                }
                else
                {
                    strCxjg = "No found.";
                    return;
                }
            }

            string sBz = "";

            for (int v = 0; v < iEnglishDictNum; v++)
            {
                arStrEnglishCcsc[v] = "";
            }

            int No = 0;

            if (!_ishtmlListOut)  //如果查词请求不是 htmlListOut 发出的
            {
                No = sNo;
                while (No <= lNo)
                {
                    sBz = Program.strL[No].Substring(0, 1);

                    for (int v = 0; v < iEnglishDictNum; v++)
                    {
                        if (sBz == arStrEnglishDictInfo[v].Substring(1, 1))
                            arStrEnglishCcsc[v] = arStrEnglishCcsc[v] + "<> " + Program.strL[No].Substring(2) + "$\r\n\r\n";
                    }

                    //if (sBz == "E")
                    //sE = sE + "<> " + Program.strL[No].Substring(2) + "$\r\n\r\n";

                    n++;
                    No++;
                }
            }
            else
            {
                //储存以下值，以备 listBatchOut() 函数 输出时使用
                listFirstNo = sNo;
                listEndNo = lNo;
                return;
            }

            for (int v = 0; v < iEnglishDictNum; v++)
            {
                if (arStrEnglishCcsc[v] != "")
                {
                    arStrEnglishCcsc[v] = "<font id='" + arStrEnglishDictInfo[v].Substring(1, 1) + "' color=#4169E1>" + arStrEnglishDictInfo[v].Substring(29) + "</font><br>\r\n" + arStrEnglishCcsc[v] + "<br>\r\n";
                    arTreeNodeEnglish[v] = new TreeNode(arStrEnglishDictInfo[v].Substring(3, 25).Trim());
                    arTreeNodeEnglish[v].Name = "nod" + arStrEnglishDictInfo[v].Substring(1, 1);
                    arTreeNodeEnglish[v].ToolTipText = arStrEnglishDictInfo[v].Substring(29);
                    treeView1.Nodes["trRoot"].Nodes.Add(arTreeNodeEnglish[v]);
                }
            }
            /*
            if (sE != "")
            {
                sE = "<font id='E' color=#4169E1>English-Pali Dictionary</font><br>\r\n" + sE + "<br>\r\n";
                TreeNode trNodE = new TreeNode("E-P Dict");
                trNodE.Name = "nodE";
                trNodE.ToolTipText = "by Metta Net, Sri Lanka";
                treeView1.Nodes["trRoot"].Nodes.Add(trNodE);
            }*/

            for (int v = 0; v < iEnglishDictNum; v++)
            {
                strCxjg = strCxjg + arStrEnglishCcsc[v];
            }

            //strCxjg = strCxjg + sY + sE;

            if (n == 0)
                strCxjg = "No found.";
        }

        public string cc_zw(string sc)
        {
            string nword, pword;
            int n = 0;
            pword = sc;
            nword = einword_zw(sc);

            int sNo = -1, lNo = -1;
            int ibz = -3;

            ibz = Program.edcNo(nword, out Program.iNo);
            if (ibz == 0)
            {
                sNo = Program.iNo;
                while ((sNo > 0) && !Program.eab(Program.sL[sNo - 1], nword))
                    sNo = sNo - 1;
                lNo = sNo;
                while ((lNo < Program.NUM - 1) && !Program.eab(nword, Program.sL[lNo + 1]))
                    lNo = lNo + 1;
            }
            else
                return "";

            char[] ca = "āīūṅñṭḍṇḷŋṁṃĀĪŪṄÑṬḌṆḶŊṀṂʨɕʕ½¼⅛⅖☿".ToCharArray();
            char[] cb = "abcdefghijklmnopqrstuvwxyz123456".ToCharArray();

            string sBz, sTmp, pword_tmp;
            for (int v = 0; v < iPaliDictNum; v++)
            {
                arStrPaliCcsc[v] = "";
            }
            sBz = "";
            sTmp = "";
            pword_tmp = "";
            int No = sNo;
            while (No <= lNo)
            {
                sBz = Program.strL[No].Substring(0, 1);
                sTmp = Program.strL[No].Substring(2);

                if (!menuosBlurinputmode.Checked)
                {
                    MatchCollection mc = new Regex(@"^" + pword + ".*", RegexOptions.IgnoreCase).Matches(sTmp);
                    foreach (Match ma in mc)
                    {
                        sTmp = "<b>" + sTmp.Substring(0, pword.Length) + "</b>" + sTmp.Substring(pword.Length);

                        for (int v = 0; v < iPaliDictNum; v++)
                        {
                            if (sBz == arStrPaliDictInfo[v].Substring(1, 1))
                                arStrPaliCcsc[v] = arStrPaliCcsc[v] + "<br>" + sTmp + "\r\n";
                        }

                        //if (sBz == "D")
                        //sD = sD + "<br>" + sTmp + "\r\n";

                        n++;
                    }

                    MatchCollection mcp = new Regex("ṃ", RegexOptions.IgnoreCase).Matches(pword);
                    if (mcp.Count > 0)
                    {
                        pword_tmp = pword;
                        pword_tmp = new Regex("ṃ", RegexOptions.IgnoreCase).Replace(pword_tmp, "ṁ");
                        MatchCollection mc2 = new Regex(@"^" + pword_tmp + ".*", RegexOptions.IgnoreCase).Matches(sTmp);
                        foreach (Match ma in mc2)
                        {
                            sTmp = "<b>" + sTmp.Substring(0, pword_tmp.Length) + "</b>" + sTmp.Substring(pword_tmp.Length);

                            for (int v = 0; v < iPaliDictNum; v++)
                            {
                                if (sBz == arStrPaliDictInfo[v].Substring(1, 1))
                                    arStrPaliCcsc[v] = arStrPaliCcsc[v] + "<br>" + sTmp + "\r\n";
                            }

                            //if (sBz == "D")
                            //sD = sD + "<br>" + sTmp + "\r\n";

                            n++;
                        }
                        pword_tmp = pword;
                        pword_tmp = new Regex("ṃ", RegexOptions.IgnoreCase).Replace(pword_tmp, "ŋ");
                        MatchCollection mc1 = new Regex(@"^" + pword_tmp + ".*", RegexOptions.IgnoreCase).Matches(sTmp);
                        foreach (Match ma in mc1)
                        {
                            sTmp = "<b>" + sTmp.Substring(0, pword_tmp.Length) + "</b>" + sTmp.Substring(pword_tmp.Length);

                            for (int v = 0; v < iPaliDictNum; v++)
                            {
                                if (sBz == arStrPaliDictInfo[v].Substring(1, 1))
                                    arStrPaliCcsc[v] = arStrPaliCcsc[v] + "<br>" + sTmp + "\r\n";
                            }

                            //if (sBz == "D")
                            //sD = sD + "<br>" + sTmp + "\r\n";

                            n++;
                        }
                    }
                }
                else
                {
                    sTmp = "<b>" + sTmp.Substring(0, pword.Length) + "</b>" + sTmp.Substring(pword.Length);

                    for (int v = 0; v < iPaliDictNum; v++)
                    {
                        if (sBz == arStrPaliDictInfo[v].Substring(1, 1))
                            arStrPaliCcsc[v] = arStrPaliCcsc[v] + "<br>" + sTmp + "\r\n";
                    }

                    //if (sBz == "D")
                    //sD = sD + "<br>" + sTmp + "\r\n";

                    n++;
                }
                No++;
            }

            for (int v = 0; v < iPaliDictNum; v++)
            {
                if (arStrPaliCcsc[v] != "")
                {
                    arStrPaliCcsc[v] = "<br><font size='2' color=#4169E1>" + arStrPaliDictInfo[v].Substring(29) + "</font>\r\n" + arStrPaliCcsc[v] + "<br>\r\n";
                }
            }

            //if (sD != "")
            //sD = "<br><font size='2' color=#4169E1>《巴汉词典》Mahāñāṇo Bhikkhu编著</font>\r\n" + sD + "<br>\r\n";

            if (n == 0)
                return "";
            else
            {
                string rtnStrCxjg = "";

                if (EN.Checked)
                {
                    for (int v = 0; v < iPaliDictNum; v++)
                    {
                        if (arStrPaliDictInfo[v].Substring(0, 1) == "E")
                            rtnStrCxjg = rtnStrCxjg + arStrPaliCcsc[v];
                    }
                    for (int v = 0; v < iPaliDictNum; v++)
                    {
                        if (arStrPaliDictInfo[v].Substring(0, 1) == "C")
                            rtnStrCxjg = rtnStrCxjg + arStrPaliCcsc[v];
                    }
                    //return sN + sP + sC + sM + sD + sF + sW + sL + sG + sZ + sX;
                }
                else
                {
                    for (int v = 0; v < iPaliDictNum; v++)
                    {
                        if (arStrPaliDictInfo[v].Substring(0, 1) == "C")
                            rtnStrCxjg = rtnStrCxjg + arStrPaliCcsc[v];
                    }
                    for (int v = 0; v < iPaliDictNum; v++)
                    {
                        if (arStrPaliDictInfo[v].Substring(0, 1) == "E")
                            rtnStrCxjg = rtnStrCxjg + arStrPaliCcsc[v];
                    }
                    //return sM + sD + sF + sW + sL + sG + sZ + sX + sN + sP + sC;
                }

                return rtnStrCxjg;
            }
        }

        //变形词 变换名词动词词尾查词
        public bool ziweiCc(string sc)
        {
            if (ccmsbz != 2 && ccmsbz != 3)
                return false;

            int n = 0;  //记录查找到的结果条数

            string[] sCS = new string[753];
            string[] sMccw = new string[753];
            string[] sZiwei = new string[753];
            string[] sZiweiC = new string[753];
            string strLine, szw3;
            int i = 0;

            StreamReader sr1 = new StreamReader(new FileStream(@".\ziwei\dmcw", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            strLine = sr1.ReadLine();
            while (strLine != null)
            {
                sCS[i] = strLine.Substring(0, 2).TrimEnd();
                sMccw[i] = strLine.Substring(3, 4).TrimEnd();
                sZiwei[i] = strLine.Substring(8, 10).TrimEnd();
                sZiweiC[i] = strLine.Substring(19);
                i++;
                strLine = sr1.ReadLine();
            }
            sr1.Close();

            char[] ca = "āīūṅñṭḍṇḷŋṁṃĀĪŪṄÑṬḌṆḶŊṀṂʨɕʕ½¼⅛⅖☿".ToCharArray();
            char[] cb = "abcdefghijklmnopqrstuvwxyz123456".ToCharArray();

            string strC, strB = "";
            strC = sc;
            string sZW;

            for (int h = 0; h < 753; h++)
            {
                sZW = sZiwei[h];
                if (strC.Length > sZW.Length + sCS[h].Length)
                {
                    if (strC.Substring(0, sCS[h].Length) == sCS[h])
                    {
                        if (strC.Substring(strC.Length - sZW.Length) == sZW)
                        {
                            strB = strC.Substring(sCS[h].Length, strC.Length - sZW.Length - sCS[h].Length) + sMccw[h];

                            szw3 = cc_zw(strB);
                            if (szw3 == "")
                                continue;
                            else
                            {
                                if (sCS[h] == "")
                                    strCxjg = strCxjg + "<br><span style='color:#000000; background:#FFFF66; font-weight:bold'>" + strC + " = ( " + strB + " + " + sZW + " )</span>" + "  ";
                                else
                                    strCxjg = strCxjg + "<br><span style='color:#000000; background:#FFFF66; font-weight:bold'>" + strC + " = ( " + sCS[h] + " + " + strB + " + " + sZW + " )</span>" + "  ";
                                strCxjg = strCxjg + "\r\n字尾: " + sMccw[h] + "<>" + sZiwei[h] + "  " + sZiweiC[h] + "\r\n\r\n";
                                strCxjg = strCxjg + "<br>" + szw3;
                                n++;
                            }
                        }
                    }
                }
            }

            if (n == 0 && (strC.Substring(strC.Length - 1) == "ṃ" || strC.Substring(strC.Length - 1) == "ṁ" || strC.Substring(strC.Length - 1) == "ŋ"))
            {
                strB = strC.Substring(0, strC.Length - 1);

                szw3 = cc_zw(strB);
                string yuanci = "";
                if (szw3 == "")
                {
                    yuanci = strC;
                    strC = strC.Substring(0, strC.Length - 1);
                    for (int h = 0; h < 753; h++)
                    {
                        sZW = sZiwei[h];

                        if (strC.Length > sZW.Length + sCS[h].Length)
                        {
                            if (strC.Substring(0, sCS[h].Length) == sCS[h])
                            {
                                if (strC.Substring(strC.Length - sZW.Length) == sZW)
                                {
                                    strB = strC.Substring(sCS[h].Length, strC.Length - sZW.Length - sCS[h].Length) + sMccw[h];

                                    szw3 = cc_zw(strB);
                                    if (szw3 == "")
                                        continue;
                                    else
                                    {
                                        if (sCS[h] == "")
                                            strCxjg = strCxjg + "<br><span style='color:#000000; background:#FFFF66; font-weight:bold'>" + yuanci + " = ( " + strB + " + " + sZW + " + " + yuanci.Substring(yuanci.Length - 1) + " )</span>" + "  ";
                                        else
                                            strCxjg = strCxjg + "<br><span style='color:#000000; background:#FFFF66; font-weight:bold'>" + yuanci + " = ( " + sCS[h] + " + " + strB + " + " + sZW + " + " + yuanci.Substring(yuanci.Length - 1) + " )</span>" + "  ";

                                        strCxjg = strCxjg + "\r\n字尾: " + sMccw[h] + "<>" + sZiwei[h] + "  " + sZiweiC[h] + "\r\n\r\n";
                                        strCxjg = strCxjg + "<br>" + szw3;
                                        n++;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    strCxjg = strCxjg + "<br><span style='color:#000000; background:#FFFF66; font-weight:bold'>" + strC + " = ( " + strB + " + " + strC.Substring(strC.Length - 1) + " )</span>" + "  ";
                    strCxjg = strCxjg + "\r\n字尾: " + strC.Substring(strC.Length - 1) + "\r\n\r\n";
                    strCxjg = strCxjg + "<br>" + szw3;
                    n++;
                }
            }

            if (n == 0)
            {
                if (cwCc(sc))
                {
                    palitongji_bz = "2";
                    return true;
                }
                else
                    return false;
            }
            else
            {
                palitongji_bz = "2";
                return true;
            }
        }

        /// <summary>
        /// 储存查词结果列表第一个单词在词库数组里的编号，注：词库是按字母顺序排放的，因此查词结果第一与最后编号之间的所有单词就是
        /// 要输出的内容。定义这个变量是为了要在列表输出的时候使用，因为有时候结果词条很多，一次性输出太慢，所以要在用户滚动
        /// 列表时逐次分批输出，所以才定义这两个变量。
        /// </summary>
        public static int listFirstNo = 0;

        /// <summary>
        /// 储存查词结果列表最后一个单词在词库数组里的编号，注：词库是按字母顺序排放的，因此查词结果第一与最后编号之间的所有单词就是
        /// 要输出的内容。定义这个变量是为了要在列表输出的时候使用，因为有时候结果词条很多，一次性输出太慢，所以要在用户滚动
        /// 列表时逐次分批输出，所以才定义这两个变量。
        /// </summary>
        public static int listEndNo = 0;

        /// <summary>
        /// 储存palihan_cc函数所查找的单词pword，在htmlout与htmllistout函数中使用
        /// </summary>
        public static string strPaliWord = "";

        /// <summary>
        /// 值为true表明查词结果已经全部输出
        /// </summary>
        public static bool _isOutputAll = false;

        /// <summary>
        /// 控制每次输出结果条数，一次输出两页的数量
        /// </summary>
        public static int listOutNum = 40;

        //储存上一个词目，以用于对比当前词目是否与其相同（巴利模式词目）
        //此变量不能在BatchOut中置空，因为需要记住上次BOut的最后一个词，以避免重复输出此词
        string sLTmp = "";

        /// <summary>
        /// 逐批输出查词结果
        /// </summary>
        private void listBatchOut()
        {
            int n = 0;  //记录查找结果条数
            int sNo = listFirstNo, lNo = listEndNo;
            string pword = strPaliWord;

            string sBz = "", pword_tmp = "";

            int No = 0;

            No = sNo;
            while (No <= lNo && n < listOutNum)
            {
                if (Program.sL[No].ToLower() != sLTmp)
                {
                    sLTmp = Program.sL[No].ToLower();

                    sBz = Program.strL[No].Substring(0, 1);
                    /*
                    if (!menuosBlurinputmode.Checked)
                    {
                        MatchCollection mcp = new Regex("[ṁṃŋ]", RegexOptions.IgnoreCase).Matches(pword);
                        if (mcp.Count > 0)
                        {
                            pword_tmp = pword;
                            char[] ca1 = "ṁṃ".ToCharArray();
                            foreach (char c in ca1)
                            {
                                pword_tmp = new Regex(c.ToString(), RegexOptions.IgnoreCase).Replace(pword_tmp, "ŋ");
                            }
                            MatchCollection mc1 = new Regex(@"\w%" + pword_tmp + ".*", RegexOptions.IgnoreCase).Matches(Program.strL[No]);
                            foreach (Match ma in mc1)
                            {
                                strCxjg = strCxjg + "<> " + Program.strL[No].Substring(2) + "$\r\n";
                                n++;
                            }
                            pword_tmp = pword;
                            char[] ca2 = "ŋṃ".ToCharArray();
                            foreach (char c in ca2)
                            {
                                pword_tmp = new Regex(c.ToString(), RegexOptions.IgnoreCase).Replace(pword_tmp, "ṁ");
                            }
                            MatchCollection mc2 = new Regex(@"\w%" + pword_tmp + ".*", RegexOptions.IgnoreCase).Matches(Program.strL[No]);
                            foreach (Match ma in mc2)
                            {
                                strCxjg = strCxjg + "<> " + Program.strL[No].Substring(2) + "$\r\n";
                                n++;
                            }
                            pword_tmp = pword;
                            char[] ca3 = "ŋṁ".ToCharArray();
                            foreach (char c in ca3)
                            {
                                pword_tmp = new Regex(c.ToString(), RegexOptions.IgnoreCase).Replace(pword_tmp, "ṃ");
                            }
                            MatchCollection mc3 = new Regex(@"\w%" + pword_tmp + ".*", RegexOptions.IgnoreCase).Matches(Program.strL[No]);
                            foreach (Match ma in mc3)
                            {
                                strCxjg = strCxjg + "<> " + Program.strL[No].Substring(2) + "$\r\n";
                                n++;
                            }
                        }
                        else
                        {
                            MatchCollection mc = new Regex(@"\w%" + pword + ".*", RegexOptions.IgnoreCase).Matches(Program.strL[No]);
                            foreach (Match ma in mc)
                            {
                                strCxjg = strCxjg + "<> " + Program.strL[No].Substring(2) + "$\r\n";
                                n++;
                            }
                        }
                    }
                    else
                    {
                        strCxjg = strCxjg + "<> " + Program.strL[No].Substring(2) + "$\r\n";
                        n++;
                    }*/
                    //注释掉上面这一段，list时不再使用pali模式，而永远是英文化模糊模式
                    //同时，把blur-mode复选框设置为空，并且把它禁用掉，这样，除了list之外的地方，都永远是pali查词模式！
                    strCxjg = strCxjg + "<> " + Program.strL[No].Substring(2) + "$\r\n";
                    n++;
                }
                No++;
            }
            if (n == 0)
                strCxjg = ""; //把 strCxjg 清空是为了防止 htmlListOut() 函数脚本出错，strCxjg为空则不运行htmlListOut() 函数
            
            if (No - 1 == listEndNo)
                _isOutputAll = true;
            else
            {
                _isOutputAll = false;
                listFirstNo = No - 1;
            }
        }

        private void psyCC()
        {
            string sbword = "";
            strCxjg = "";

            DateTime startD, endD;
            startD = DateTime.Now;

            if (cboxInput.Text.Trim() == "")
                return;

            if (cboxInput.Text.Trim().Length > 68)
            {
                webBrowser1.DocumentText = "本词典最长单词为68个字母，您输入的单词过长！\r\n请重新输入！";
                return;
            }

            int n = 0;  //记录查找到的结果条数
            string nword = "", saword = "";

            nword = inword();
            saword = nword;

            if (nword == "")
            {
                webBrowser1.DocumentText = "您输入的单词字符不是正确的巴利文罗马字母，请重新输入！";
                return;
            }

            webBrowser1.DocumentText = "looking up the word: " + nword + " ...";
            strCxjg = nword + "  " + "word translate:" + "\r\n\r\n";

            FileStream bFile = new FileStream(@".\pali-e\cidian", FileMode.Open);
            StreamReader srb = new StreamReader(bFile, System.Text.Encoding.GetEncoding("utf-8"));
            FileStream aF1m = new FileStream(@".\pali-e\indexdat", FileMode.Open);

            if (nword.Length < 4)
            {
                sbword = nword;

                byte[] bm = new byte[8] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                int um = 0;

                for (int im = 0; im < sbword.Length; im++)
                {
                    aF1m.Seek(um + Program.iABC(sbword.Substring(im, 1)) * 8, SeekOrigin.Begin);
                    aF1m.Read(bm, 0, 8);
                    if ((int)(bm[0]) == 0)
                    {
                        goto kkk;
                    }
                    else if ((int)(bm[0]) == 1)
                    {
                        if (im < sbword.Length - 1)
                            um = (int)(bm[1] | bm[2] << 8 | bm[3] << 16 | bm[4] << 24);
                        else
                        {
                            goto kkk;
                        }
                    }
                    else if ((int)(bm[0]) == 2)
                    {
                        if (im < sbword.Length - 1)
                        {
                            goto kkk;
                        }
                        else
                            um = (int)(bm[5] | bm[6] << 8 | bm[7] << 16 | 0);
                    }
                    else if ((int)(bm[0]) == 3)
                    {
                        if (im < sbword.Length - 1)
                            um = (int)(bm[1] | bm[2] << 8 | bm[3] << 16 | bm[4] << 24);
                        else
                            um = (int)(bm[5] | bm[6] << 8 | bm[7] << 16 | 0);
                    }
                }

                //对于StreamReader来说，在使用ReadLine()后，如再要seek()则这一句很必要，但对于FileStream的使用，似乎就没有这个问题
                srb.DiscardBufferedData();
                srb.BaseStream.Seek(um, System.IO.SeekOrigin.Begin);

                Regex re1m = new Regex(@"^[^, 。]+(,| |。)(?<w>.*)", RegexOptions.IgnoreCase);
                MatchCollection mc1m = re1m.Matches(srb.ReadLine());
                foreach (Match ma1m in mc1m)
                {
                    strCxjg = strCxjg + sbword.PadRight(nword.Length, ' ') + " " + ma1m.Groups["w"].Value + "\r\n";
                    n++;
                }

            kkk:
                ;
            }

            for (int i = 0; i < nword.Length; i++)
            {
                for (int j = saword.Length; j > 3; j--)
                {
                    sbword = saword.Substring(0, j);

                    byte[] bm = new byte[8] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    int um = 0;

                    for (int im = 0; im < sbword.Length; im++)
                    {
                        aF1m.Seek(um + Program.iABC(sbword.Substring(im, 1)) * 8, SeekOrigin.Begin);
                        aF1m.Read(bm, 0, 8);
                        if ((int)(bm[0]) == 0)
                        {
                            goto kkk;
                        }
                        else if ((int)(bm[0]) == 1)
                        {
                            if (im < sbword.Length - 1)
                                um = (int)(bm[1] | bm[2] << 8 | bm[3] << 16 | bm[4] << 24);
                            else
                            {
                                goto kkk;
                            }
                        }
                        else if ((int)(bm[0]) == 2)
                        {
                            if (im < sbword.Length - 1)
                            {
                                goto kkk;
                            }
                            else
                                um = (int)(bm[5] | bm[6] << 8 | bm[7] << 16 | 0);
                        }
                        else if ((int)(bm[0]) == 3)
                        {
                            if (im < sbword.Length - 1)
                                um = (int)(bm[1] | bm[2] << 8 | bm[3] << 16 | bm[4] << 24);
                            else
                                um = (int)(bm[5] | bm[6] << 8 | bm[7] << 16 | 0);
                        }
                    }

                    srb.DiscardBufferedData();
                    srb.BaseStream.Seek(um, System.IO.SeekOrigin.Begin);

                    sbword = sbword.PadLeft(sbword.Length + i, ' ');
                    Regex re1m = new Regex(@"^[^, 。]+(,| |。)(?<w>.*)", RegexOptions.IgnoreCase);
                    MatchCollection mc1m = re1m.Matches(srb.ReadLine());
                    foreach (Match ma1m in mc1m)
                    {
                        strCxjg = strCxjg + sbword.PadRight(nword.Length, ' ') + " " + ma1m.Groups["w"].Value + "\r\n";
                        n++;
                    }

                kkk:
                    ;
                }
                saword = saword.Substring(1);
            }
            srb.Close();
            aF1m.Close();

            endD = DateTime.Now;
            System.TimeSpan ts = endD.Subtract(startD);
            strCxjg = strCxjg + "( " + ts.TotalMilliseconds.ToString() + " milliseconds.)";
            webBrowser1.DocumentText = outword(strCxjg);

            if (n == 0)
                webBrowser1.DocumentText = "\r\nNo found.";

            {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失
        }

        private void pbjcc()
        {
            strCxjg = "";

            DateTime startD, endD;
            startD = DateTime.Now;

            if (cboxInput.Text.Trim() == "")
                return;

            if (cboxInput.Text.Trim().Length > 68)
            {
                webBrowser1.DocumentText = "本词典最长单词为68个字母，您输入的单词过长！\r\n请重新输入！";
                return;
            }

            int n = 0;  //记录查找到的结果条数
            string nword = "";
            nword = inword();
            if (nword == "")
            {
                webBrowser1.DocumentText = "您输入的单词字符不是正确的巴利文罗马字母，请重新输入！";
                return;
            }

            webBrowser1.DocumentText = "looking up the word: " + nword + " ...";
            strCxjg = nword + "$  " + "word translate:" + "\r\n\r\n";


            Program.NUM = 20787;
            pl_readdc(@".\pali-e\cidian", @".\pali-e\index");
            int sNo = -1, lNo = -1;
            int ibz = -3;

            ibz = Program.dcNo(nword, out Program.iNo);
            if (ibz == -2)
            {
                webBrowser1.DocumentText = "\r\nNo found.";
                return;
            }
            if (ibz == 0)
            {
                sNo = Program.iNo;
            }
            if (ibz == 1)
            {
                sNo = Program.iNo + 1;
            }
            if (ibz == -1)
            {
                sNo = 0;
            }

            ibz = Program.dcNo(nword.PadRight(25, 'Ŋ'), out Program.iNo);
            if (ibz == -1)
            {
                webBrowser1.DocumentText = "\r\nNo found.";
                return;
            }
            if (ibz == 0)
            {
                lNo = Program.iNo;
            }
            if (ibz == 1)
            {
                lNo = Program.iNo;
            }
            if (ibz == -2)
            {
                lNo = Program.NUM - 1;
            }

            int No = sNo;
            while (No <= lNo)
            {
                strCxjg = strCxjg + Program.strL[No] + "\r\n";
                n++;
                No++;
            }

            endD = DateTime.Now;
            System.TimeSpan ts = endD.Subtract(startD);
            strCxjg = strCxjg + "( " + ts.TotalMilliseconds.ToString() + " milliseconds.)";

            webBrowser1.DocumentText = outword(strCxjg);

            if (n == 0)
                webBrowser1.DocumentText = "\r\nNo found.";

            {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失
        }

        private void readme()
        {
            string flpath = @".\set\readme.htm";
            if (EN.Checked)
                flpath = @".\set\readmeen.htm";

            StreamReader sr = new StreamReader(new FileStream(flpath, FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            strCxjg = sr.ReadToEnd();
            sr.Close();

            webBrowser1.DocumentText = outword("<span>If you encounter the problem that the cursor disappears in the word input box, please click the small box in front of 'abc' in this window, and then click the mouse to enter the input box, and the cursor will be displayed.</span><br /><br /><span>如果您遇到单词输入框里光标消失的问题，请用鼠标点击一下本窗口里‘小写’前面的小方框，然后再用鼠标点击进入输入框，光标就会显示。</span><br /><br />" + strCxjg);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Application.RemoveMessageFilter(this);

            FileStream cfFile = new FileStream(@".\set\cfdat", FileMode.Open);
            byte[] bwin = new byte[1] { 0x00 };
            byte[] bcf = new byte[4] { 0x00, 0x00, 0x00, 0x00 };
            byte[] bmatch = new byte[1] { 0x00 };
            byte[] b_Font_Dictionary_Inputmode = new byte[3] { 0x00, 0x00, 0x00 };
            byte[] blanguage = new byte[1] { 0x00 };
            byte[] bonlydict = new byte[1] { 0x00 };

            if (menuosSetWindowInFront.Checked == false)
                bwin[0] = 0x00;
            else
                bwin[0] = 0x01;
            cfFile.Seek(0, SeekOrigin.Begin);
            cfFile.Write(bwin, 0, 1);

            int ileft = 100, itop = 50;
            if (this.Left >= 0)
                ileft = this.Left;
            if (this.Top >= 0)
                itop = this.Top;
            bcf[0] = (byte)(ileft);
            bcf[1] = (byte)(ileft >> 8);
            bcf[2] = (byte)(itop);
            bcf[3] = (byte)(itop >> 8);
            cfFile.Seek(1, SeekOrigin.Begin);
            cfFile.Write(bcf, 0, 4);

            if (!bwordaheadmatch == true)
                bmatch[0] = 0x00;
            else
                bmatch[0] = 0x01;
            cfFile.Seek(5, SeekOrigin.Begin);
            cfFile.Write(bmatch, 0, 1);

            if (rbtnVriRomanPali.Checked)
                b_Font_Dictionary_Inputmode[0] = 0x01;
            if (rbtnSangayana.Checked)
                b_Font_Dictionary_Inputmode[0] = 0x02;
            if (rbtnTahoma.Checked)
                b_Font_Dictionary_Inputmode[0] = 0x03;

            if (menuosEnglishPali.Checked)
                b_Font_Dictionary_Inputmode[1] = 0x04;

            if (menuosBlurinputmode.Checked)
                b_Font_Dictionary_Inputmode[2] = 0x02;

            cfFile.Seek(6, SeekOrigin.Begin);
            cfFile.Write(b_Font_Dictionary_Inputmode, 0, 3);

            if (FAN.Checked)
                blanguage[0] = 0x01;
            else if (HAN.Checked)
                blanguage[0] = 0x02;
            else
                blanguage[0] = 0x03;
            cfFile.Seek(9, SeekOrigin.Begin);
            cfFile.Write(blanguage, 0, 1);

            if (frmmuluwindow.Visible || Program.toolbarform.Visible)
                bonlydict[0] = 0x01;
            cfFile.Seek(11, SeekOrigin.Begin);
            cfFile.Write(bonlydict, 0, 1);

            cfFile.Close();

            //写入查词历史记录
            StreamWriter swCbi = new StreamWriter(@".\set\ccls", false, System.Text.Encoding.GetEncoding("utf-8"));
            for (int g = 0; g < 50; g++)
            {
                if (arrStrCcls[g] == "")
                    break;
                swCbi.WriteLine(arrStrCcls[g]);
            }
            swCbi.Close();
            //

            searcher.Close();

            //xml设置
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("set.xml");
            foreach (XmlElement bxc in xmlDoc.DocumentElement.ChildNodes)
            {
                if (tsmiBxc.Checked)
                {
                    bxc.SelectSingleNode("open").InnerText = "1";
                }
                else
                {
                    bxc.SelectSingleNode("open").InnerText = "0";
                }
            }
            xmlDoc.Save("set.xml");
        }

        private void menuosBlurinputmode_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("‘模糊匹配’功能说明：\r\n\r\n" +
                           "若选中‘模糊匹配’，则可以直接输入英文字符，查字符类似的巴利单词，\r\n" +
                           "如，\r\n" +
                           "无论输入英文字符'ara'或是输入巴利字符'ārā'，\r\n" +
                           "都可以查到如下巴利单词：\r\n" +
                           "‘ara,【中】 轮辐。\r\n" +
                           "āra,【阳】 针。\r\n" +
                           "ārā,【无】 在远处，远离，遥远的。’\r\n\r\n" +

                           "倘若不选中‘模糊匹配’，则只进行精确查词，\r\n" +
                           "如，\r\n" +
                           "输入'ara'，只查到：\r\n" +
                           "‘ara,【中】 轮辐。’\r\n\r\n" +

                           "输入'ārā'，只查到：\r\n" +
                           "‘ārā,【无】 在远处，远离，遥远的。中】 轮辐。’", this.menuosBlurinputmode);
        }

        private void menuosEnglishPali_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("功能说明：\r\n" +
                            "若选中此‘英语’复选框，即可设置成‘英语 => 巴利文’查词模式，\r\n" +
                            "可以输入英文单词，查找其巴利文之解释。\r\n\r\n" +
                            "若取消选中，即设置成‘巴利文 => 汉语、巴利文 => 英语’查词模式。", this.menuosEnglishPali);
        }

        private void btnLookup_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("在‘巴利文 -> 汉语、英语’查词模式下，\r\n" +
                            "也可以输入‘中文’，从词库中反查其巴利文之解释，以供参考。\r\n\r\n" +
                            "注：要正常进行此种查找，必须在菜单中先进行正确的‘语言设置’：\r\n" +
                            "若您的windows系统是‘繁体’版，则在‘语言设置’中应选‘繁体’，\r\n" +
                            "若您的windows系统是‘简体’版，则在‘语言设置’中应选‘简体’。", this.btnLookup);
        }

        private void btnshowfhc_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("点击此侧边按钮，可显示复合词分析结果文本框。", this.btnshowfhc);
        }

        ToolTip toolTip1;

        /// <summary>
        /// 用此数组储存.\mulu\nameidx 文件里的每一行字符串，此字符串表明每一本书的磁盘htm文件与经典目录中节点的对应关系，其中数字即是节点索引
        /// </summary>
        public ArrayList alName;

        private void Form1_Load(object sender, EventArgs e)
        {
            toolTip1 = new ToolTip();
            toolTip1.AutoPopDelay = 25000;
            toolTip1.InitialDelay = 50;
            toolTip1.ReshowDelay = 50;
            toolTip1.ShowAlways = true;

            webBrowser1.AllowWebBrowserDrop = false;
            //webBrowser1.IsWebBrowserContextMenuEnabled = false;
            webBrowser1.WebBrowserShortcutsEnabled = false;
            webBrowser1.ObjectForScripting = this;
            // Uncomment the following line when you are finished debugging.
            webBrowser1.ScriptErrorsSuppressed = true;

            FileStream cfFile = new FileStream(@".\set\cfdat", FileMode.Open);
            byte[] bcf = new byte[12] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            cfFile.Seek(0, SeekOrigin.Begin);
            cfFile.Read(bcf, 0, 12);
            cfFile.Close();
            if ((int)(bcf[0]) == 0)
            {
                menuosSetWindowInFront.Checked = false;
                this.TopMost = false;
            }
            else if ((int)(bcf[0]) == 1)
            {
                menuosSetWindowInFront.Checked = true;
                this.TopMost = true;
            }

            if ((int)(bcf[5]) == 0)
                bwordaheadmatch = false;
            else if ((int)(bcf[5]) == 1)
                bwordaheadmatch = true;

            int ileft = 0, itop = 0;
            if ((int)(bcf[1] | bcf[2] << 8) > (Screen.PrimaryScreen.Bounds.Width - 15))
                ileft = Screen.PrimaryScreen.Bounds.Width - this.Width - 15;
            else
                ileft = (int)(bcf[1] | bcf[2] << 8);
            if ((int)(bcf[3] | bcf[4] << 8) > (Screen.PrimaryScreen.Bounds.Height - 50))
                itop = 20;
            else
                itop = (int)(bcf[3] | bcf[4] << 8);

            this.SetDesktopLocation(ileft, itop);

            if ((int)(bcf[6]) == 1)
            {
                rbtnVriRomanPali.Checked = true;
                rbtnVriRomanPali.ForeColor = Color.RoyalBlue;
                cboxInput.Font = new System.Drawing.Font("Tahoma", 12);
                //cboxInput.Font = new System.Drawing.Font("VriRomanPlzd", 12);
            }
            if ((int)(bcf[6]) == 2)
            {
                rbtnSangayana.Checked = true;
                rbtnSangayana.ForeColor = Color.RoyalBlue;
                cboxInput.Font = new System.Drawing.Font("Tahoma", 12);
                //cboxInput.Font = new System.Drawing.Font("SangayanaPlzd", 12);
            }
            if ((int)(bcf[6]) == 3)
            {
                rbtnTahoma.Checked = true;
                rbtnTahoma.ForeColor = Color.RoyalBlue;
                cboxInput.Font = new System.Drawing.Font("Tahoma", 12);

                button11.Enabled = true;
                button12.Enabled = true;
            }
            textBox3.Font = new System.Drawing.Font("Tahoma", 10);

            if ((int)(bcf[7]) == 1)
            {
            }
            if ((int)(bcf[7]) == 2)
            {
            }
            if ((int)(bcf[7]) == 3)
            {
            }

            if ((int)(bcf[7]) == 4)
                menuosEnglishPali.Checked = true;
            else
                menuosEnglishPali.Checked = false;

            if ((int)(bcf[8]) == 2)
                menuosBlurinputmode.Checked = true;

            /*
            if ((int)(bcf[9]) == 1)
            {
                FAN.Checked = true;
                language_fan();
            }
            if ((int)(bcf[9]) == 2)
            {
                HAN.Checked = true;
                language_han();
            }
            if ((int)(bcf[9]) == 3)
            {
                EN.Checked = true;
                language_en();
            }
            */

            if ((int)(bcf[11]) == 0)
            {
                _onlyShowDict = true;
            }

            //hotkey.ShortcutKeys = Keys.Control | (Keys)bcf[10];



            //读经窗口经文文本字体大小设置
            StreamReader srfs = new StreamReader(new FileStream(@".\set\fs", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            string strfs = srfs.ReadLine();
            srfs.Close();

            if (strfs == "1")
                fsplus.Checked = true;
            else if (strfs == "2")
                fsplustwo.Checked = true;
            else if (strfs == "3")
                fsplusthree.Checked = true;
            else
            {
                strfs = "0";
                fsoriginal.Checked = true;
            }

            if (strfs != "0")
            {
                StreamReader srf1 = new StreamReader(new FileStream(@".\set\f" + strfs, FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
                strf1 = srf1.ReadToEnd();
                srf1.Close();
            }





            //读入查词历史记录
            for (int g = 0; g < 50; g++)
                arrStrCcls[g] = "";
            int p = 0;
            string strCbi = "";
            StreamReader srCbi = new StreamReader(new FileStream(@".\set\ccls", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            strCbi = srCbi.ReadLine();
            while (strCbi != null)
            {
                arrStrCcls[p] = strCbi;
                p++;
                cboxInput.Items.Add(strCbi);
                strCbi = srCbi.ReadLine();
            }
            srCbi.Close();
            //

            bjLCID = System.Globalization.CultureInfo.CurrentCulture.LCID;

            initpro();

            alName = new ArrayList();
            StreamReader srNI = new StreamReader(new FileStream(@".\mulu\nameidx", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            string sNitmp = srNI.ReadLine();
            while (sNitmp != null)
            {
                alName.Add(sNitmp);

                sNitmp = srNI.ReadLine();
            }
            srNI.Close();

            Program.ssfrm = new frmLucn();
            if ((int)(bcf[9]) == 1)
            {
                FAN.Checked = true;
                language_fan();
            }
            if ((int)(bcf[9]) == 2)
            {
                HAN.Checked = true;
                language_han();
            }
            if ((int)(bcf[9]) == 3)
            {
                EN.Checked = true;
                language_en();
            }

            searcher = new IndexSearcher("index_ck");

            //xml设置
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("set.xml");
            foreach (XmlElement bxc in xmlDoc.DocumentElement.ChildNodes)
            {
                //string open = bxc.SelectSingleNode("open").InnerText;
                //MessageBox.Show("Test" + open);
                //string price = bxc.SelectSingleNode("price").InnerText;
                if (bxc.SelectSingleNode("open").InnerText == "1")
                {
                    tsmiBxc.Checked = true;
                }
                else
                {
                    tsmiBxc.Checked = false;
                }
            }

            //Application.AddMessageFilter(this);
        }

        /// <summary>
        /// 值为true标识启动程序时只显示词典，缺省值为false
        /// </summary>
        public static bool _onlyShowDict = false;

        /// <summary>
        /// 本机系统localeID
        /// </summary>
        public static int bjLCID = 2052;

        public frmmulu frmmuluwindow;

        public FormCN cnform;

        private void initpro()
        {
            frmmuluwindow = new frmmulu();
            //为目录窗口load经典目录
            if (frmmuluwindow.rbtnPali.Checked)
            {
                if (Program.mainform.FAN.Checked)
                    frmmuluwindow.yybz = "2";
                else
                    frmmuluwindow.yybz = "1";
            }
            if (frmmuluwindow.rbtnCn.Checked)
            {
                if (Program.mainform.FAN.Checked)
                    frmmuluwindow.yybz = "4";
                else
                    frmmuluwindow.yybz = "3";
            }
            frmmuluwindow.loadMulu(frmmuluwindow.yybz);

            treeView1.ExpandAll();

            readme();

            //读入巴利语、英语词典信息，读入词库，初始化词典树
            StreamReader srPLDic = new StreamReader(new FileStream(@".\pali-h\dicinfo", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            string strPaliDicInfo = srPLDic.ReadLine();
            Program.pl_NUM = Convert.ToInt32(strPaliDicInfo);

            iPaliDictNum = Convert.ToInt32(srPLDic.ReadLine());
            arStrPaliDictInfo = new string[iPaliDictNum];
            arStrPaliCcsc = new string[iPaliDictNum];
            arTreeNodePali = new TreeNode[iPaliDictNum];

            int iPNum = 0;
            strPaliDicInfo = srPLDic.ReadLine();
            while (strPaliDicInfo != null)
            {
                arStrPaliDictInfo[iPNum] = strPaliDicInfo;
                arStrPaliCcsc[iPNum] = "";
                iPNum++;
                strPaliDicInfo = srPLDic.ReadLine();
            }
            srPLDic.Close();

            StreamReader srENDic = new StreamReader(new FileStream(@".\e-pali\dicinfo", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            string strENDicInfo = srENDic.ReadLine();
            Program.en_NUM = Convert.ToInt32(strENDicInfo);

            iEnglishDictNum = Convert.ToInt32(srENDic.ReadLine());
            arStrEnglishDictInfo = new string[iEnglishDictNum];
            arStrEnglishCcsc = new string[iEnglishDictNum];
            arTreeNodeEnglish = new TreeNode[iEnglishDictNum];

            int iENum = 0;
            strENDicInfo = srENDic.ReadLine();
            while (strENDicInfo != null)
            {
                arStrEnglishDictInfo[iENum] = strENDicInfo;
                arStrEnglishCcsc[iENum] = "";
                iENum++;
                strENDicInfo = srENDic.ReadLine();
            }
            srENDic.Close();

            //读入义注段落关系表
            readparamark(@".\pali\paramark.csv");

            //读入词库
            IFormatter serializer = new BinaryFormatter();

            //pali词库，如果词典原档存在则读取原档，如果不存在则读取序列化档
            if (File.Exists(@".\pali-h\cidian"))
            {
                Program.pl_sL = new string[Program.pl_NUM];
                Program.pl_strL = new string[Program.pl_NUM];
                pl_readdc(@".\pali-h\cidian", @".\pali-h\index");
            }
            else
            {
                FileStream loadFile1 = new FileStream(@".\pali-h\cidian.dat", FileMode.Open, FileAccess.Read);
                Program.pl_strL = serializer.Deserialize(loadFile1) as string[];
                loadFile1.Close();

                FileStream loadFile2 = new FileStream(@".\pali-h\index.dat", FileMode.Open, FileAccess.Read);
                Program.pl_sL = serializer.Deserialize(loadFile2) as string[];
                loadFile2.Close();
            }

            //en词库，如果词典原档存在则读取原档，如果不存在则读取序列化档
            if (File.Exists(@".\e-pali\cidian"))
            {
                Program.en_sL = new string[Program.en_NUM];
                Program.en_strL = new string[Program.en_NUM];
                en_readdc(@".\e-pali\cidian", @".\e-pali\index");
            }
            else
            {
                FileStream loadFile3 = new FileStream(@".\e-pali\cidian.dat", FileMode.Open, FileAccess.Read);
                Program.en_strL = serializer.Deserialize(loadFile3) as string[];
                loadFile3.Close();

                FileStream loadFile4 = new FileStream(@".\e-pali\index.dat", FileMode.Open, FileAccess.Read);
                Program.en_sL = serializer.Deserialize(loadFile4) as string[];
                loadFile4.Close();
            }

            if (menuosEnglishPali.Checked)
            {
                Program.NUM = Program.en_NUM;
                Program.sL = Program.en_sL;
                Program.strL = Program.en_strL;
            }
            else
            {
                Program.NUM = Program.pl_NUM;
                Program.sL = Program.pl_sL;
                Program.strL = Program.pl_strL;
            }

            //初始化词典树
            treeView1.Nodes["trRoot"].Nodes.Clear();
            for (int v = 0; v < iPaliDictNum; v++)
            {
                arTreeNodePali[v] = new TreeNode(outword(arStrPaliDictInfo[v].Substring(3, 25).Trim()));
                arTreeNodePali[v].Name = "nod" + arStrPaliDictInfo[v].Substring(1, 1);
                arTreeNodePali[v].ToolTipText = outword(arStrPaliDictInfo[v].Substring(29));
                if (arStrPaliDictInfo[v].Substring(0, 1) == "E")
                    arTreeNodePali[v].BackColor = Color.LightYellow;
                treeView1.Nodes["trRoot"].Nodes.Add(arTreeNodePali[v]);
            }
            for (int v = 0; v < iEnglishDictNum; v++)
            {
                arTreeNodeEnglish[v] = new TreeNode(arStrEnglishDictInfo[v].Substring(3, 25).Trim());
                arTreeNodeEnglish[v].Name = "nod" + arStrEnglishDictInfo[v].Substring(1, 1);
                arTreeNodeEnglish[v].ToolTipText = arStrEnglishDictInfo[v].Substring(29);
                if (arStrEnglishDictInfo[v].Substring(0, 1) == "P")
                    arTreeNodeEnglish[v].BackColor = Color.Lavender;
                treeView1.Nodes["trRoot"].Nodes.Add(arTreeNodeEnglish[v]);
            }
            treeView1.Nodes["trRoot"].ExpandAll();

            cboxInput.Text = "";
            cboxInput.Focus();
            cboxInput.Select();

            Program.flashform.Close();
            Program.flashform.Dispose();

            cnform = new FormCN();

            notifyIcon1.Visible = true;

            //隐藏复合词文本框，调整列表行数
            panelfhc.Visible = false;
            lineNum = (this.Height - 306 + 126) / 18;
            webBrowser1.Height = lineNum * 18;
            treeView1.Height = webBrowser1.Height;
        }

        public void en_readdc(string cidianpath, string indexpath)
        {
            string strLine;
            int i = 0, j = 0;

            StreamReader sr = new StreamReader(new FileStream(cidianpath, FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));

            int p = 0, q = 0; //计数显示

            strLine = sr.ReadLine();
            while (strLine != null)
            {
                Program.en_strL[i] = strLine;
                i++;
                strLine = sr.ReadLine();

                p++;
                if (p == q + 1000)
                {
                    q = 1000 + q;
                    //textBox3.Text = "reading dictionary " + p.ToString();
                    //textBox3.Refresh();
                    Program.lblff.Text = "reading dictionary " + p.ToString();
                    Program.lblff.Refresh();
                }

            }
            sr.Close();

            StreamReader sr1 = new StreamReader(new FileStream(indexpath, FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));

            p = 0;
            q = 0;

            strLine = sr1.ReadLine();
            while (strLine != null)
            {
                Program.en_sL[j] = strLine;
                j++;
                strLine = sr1.ReadLine();

                p++;
                if (p == q + 1000)
                {
                    q = 1000 + q;
                    //textBox3.Text = "reading dictionary " + p.ToString();
                    //textBox3.Refresh();
                    Program.lblff.Text = "reading dictionary " + p.ToString();
                    Program.lblff.Refresh();
                }

            }
            sr1.Close();
        }

        //读入义注章节标记对应表
        public void readparamark(string paramarkpath)
        {
            string strLine;
            int i = 0;

            StreamReader sr = new StreamReader(new FileStream(paramarkpath, FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));

            //略去第一行标题
            strLine = sr.ReadLine();
            strLine = sr.ReadLine();
            while (strLine != null)
            {
                Regex rex = new Regex(@"^(?<w0>.*?),(?<w1>.*?),(?<w2>.*?),(?<w3>.*)", RegexOptions.IgnoreCase);
                MatchCollection mc = rex.Matches(strLine);
                foreach (Match ma in mc)
                {
                    Program.paramark[i, 0] = ma.Groups["w0"].Value;
                    Program.paramark[i, 1] = ma.Groups["w1"].Value;
                    Program.paramark[i, 2] = ma.Groups["w2"].Value;
                    Program.paramark[i, 3] = ma.Groups["w3"].Value;
                }

                i++;
                strLine = sr.ReadLine();
            }
            sr.Close();
        }

        public void pl_readdc(string cidianpath, string indexpath)
        {
            string strLine;
            int i = 0, j = 0;

            StreamReader sr = new StreamReader(new FileStream(cidianpath, FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));

            int p = 0, q = 0;

            strLine = sr.ReadLine();
            while (strLine != null)
            {
                Program.pl_strL[i] = strLine;
                i++;
                strLine = sr.ReadLine();

                p++;
                if (p == q + 1000)
                {
                    q = 1000 + q;
                    //textBox3.Text = "reading dictionary " + p.ToString();
                    //textBox3.Refresh();
                    Program.lblff.Text = "reading dictionary " + p.ToString();
                    Program.lblff.Refresh();
                }

            }
            sr.Close();

            StreamReader sr1 = new StreamReader(new FileStream(indexpath, FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));

            p = 0;
            q = 0;

            strLine = sr1.ReadLine();
            while (strLine != null)
            {
                Program.pl_sL[j] = strLine;
                j++;
                strLine = sr1.ReadLine();

                p++;
                if (p == q + 1000)
                {
                    q = 1000 + q;
                    //textBox3.Text = "reading dictionary " + p.ToString();
                    //textBox3.Refresh();
                    Program.lblff.Text = "reading dictionary " + p.ToString();
                    Program.lblff.Refresh();
                }

            }
            sr1.Close();
        }

        //索引查词，前部匹配，然而如此遍历索引表速度太慢，不用
        private void syCCqbpp()
        {
            string outtext = "";

            DateTime startD, endD;
            startD = DateTime.Now;

            if (cboxInput.Text.Trim() == "")
                return;

            int n = 0;  //记录查找到的结果条数
            string nword = "";

            nword = inword();

            if (nword == "")
            {
                webBrowser1.DocumentText = "您输入的单词字符不是正确的巴利文罗马字母，请重新输入！";
                return;
            }

            if (nword.Length > 68)
            {
                webBrowser1.DocumentText = "本词典最长单词为68个字母，您输入的单词 " + nword + " 过长！\r\n请重新输入！";
                return;
            }

            webBrowser1.DocumentText = "looking up the word: " + nword + " ...";

            FileStream bFile = new FileStream("cidian", FileMode.Open);
            StreamReader srb = new StreamReader(bFile, System.Text.Encoding.GetEncoding("utf-8"));
            FileStream aF1 = new FileStream("indexdat", FileMode.Open);

            byte[] b = new byte[8] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            string[] paliABC = { "A", "Ā", "I", "Ī", "U", "Ū", "E", "O", "K", "G", "Ṅ", "C", "J", "Ñ", "Ṭ", "Ḍ", "Ṇ", "T", "D", "N", "P", "B", "M", "Y", "R", "L", "V", "S", "H", "Ḷ", "Ŋ" };
            int lu, dc;
            lu = 0;
            dc = 0;
            for (int i = 0; i < nword.Length; i++)
            {
                aF1.Seek(lu + Program.iABC(nword.Substring(i, 1)) * 8, SeekOrigin.Begin);
                aF1.Read(b, 0, 8);
                if ((int)(b[0]) == 0)
                {
                    //没查到！
                    goto nnn;
                }
                else if ((int)(b[0]) == 1)
                {
                    lu = (int)(b[1] | b[2] << 8 | b[3] << 16 | b[4] << 24);
                }
                else if ((int)(b[0]) == 2)
                {
                    if (i < nword.Length - 1)
                    {
                        //没查到2！
                        goto nnn;
                    }
                    else
                    {
                        dc = (int)(b[5] | b[6] << 8 | b[7] << 16 | 0);
                        srb.DiscardBufferedData();
                        srb.BaseStream.Seek(dc, System.IO.SeekOrigin.Begin);
                        outtext = outtext + srb.ReadLine() + "\r\n";
                        n++;
                        goto nnn;
                    }
                }
                else if ((int)(b[0]) == 3)
                {
                    if (i < nword.Length - 1)
                        lu = (int)(b[1] | b[2] << 8 | b[3] << 16 | b[4] << 24);
                    else
                    {
                        lu = (int)(b[1] | b[2] << 8 | b[3] << 16 | b[4] << 24);
                        dc = (int)(b[5] | b[6] << 8 | b[7] << 16 | 0);
                        srb.DiscardBufferedData();
                        srb.BaseStream.Seek(dc, System.IO.SeekOrigin.Begin);
                        outtext = outtext + srb.ReadLine() + "\r\n";
                        n++;
                    }
                }
            }


            int[] alu = new int[25];
            int[] lzimu = new int[31];
            int ch, zimu;
            ch = 0;
            zimu = 1;
            alu[0] = lu;
            do
            {
                aF1.Seek(alu[ch] + (zimu - 1) * 8, SeekOrigin.Begin);
                aF1.Read(b, 0, 8);
                if ((int)(b[0]) == 0)
                {
                    if (zimu < 31)
                    {
                        zimu++;
                    }
                    else
                    {
                        do
                        {
                            if (ch > 0)
                            {
                                ch--;
                                zimu = lzimu[ch] + 1;
                            }
                            else
                                zimu++;
                        } while (ch > 0 & zimu == 32);
                    }
                }
                else if ((int)(b[0]) == 1)
                {
                    lzimu[ch] = zimu;
                    ch++;
                    alu[ch] = (int)(b[1] | b[2] << 8 | b[3] << 16 | b[4] << 24);
                    zimu = 1;
                }
                else if ((int)(b[0]) == 2)
                {
                    srb.DiscardBufferedData();
                    srb.BaseStream.Seek((int)(b[5] | b[6] << 8 | b[7] << 16 | 0), System.IO.SeekOrigin.Begin);
                    outtext = outtext + srb.ReadLine() + "\r\n";
                    n++;

                    if (zimu < 31)
                    {
                        zimu++;
                    }
                    else
                    {
                        do
                        {
                            if (ch > 0)
                            {
                                ch--;
                                zimu = lzimu[ch] + 1;
                            }
                            else
                                zimu++;
                        } while (ch > 0 & zimu == 32);
                    }
                }
                else if ((int)(b[0]) == 3)
                {
                    srb.DiscardBufferedData();
                    srb.BaseStream.Seek((int)(b[5] | b[6] << 8 | b[7] << 16 | 0), System.IO.SeekOrigin.Begin);
                    outtext = outtext + srb.ReadLine() + "\r\n";
                    n++;
                    lzimu[ch] = zimu;
                    ch++;
                    alu[ch] = (int)(b[1] | b[2] << 8 | b[3] << 16 | b[4] << 24);
                    zimu = 1;
                }
                if (zimu == 32 & ch == 0)
                    break;
            } while (1 == 1);

        nnn:
            srb.Close();
            aF1.Close();

            webBrowser1.DocumentText = webBrowser1.DocumentText + outtext;

            if (n == 0)
                webBrowser1.DocumentText = "\r\nNo found.";

            endD = DateTime.Now;
            System.TimeSpan ts = endD.Subtract(startD);
            webBrowser1.DocumentText = webBrowser1.DocumentText + "( " + ts.TotalMilliseconds.ToString() + " milliseconds.)";
        }

        /// <summary>
        /// 对输入的字符串进行简繁转换后输出 程序中类似功能的用于转换输出的函数有两个 outword() , outword_t()
        /// </summary>
        /// <param name="intext"></param>
        /// <returns></returns>
        string outword(string intext)
        {
            if (intext == "")
                return intext;

            if (FAN.Checked)
                return Strings.StrConv(intext, VbStrConv.TraditionalChinese, 0x0409);
            else
                return intext;
        }

        /// <summary>
        /// 值为true表示是在pali读经窗口中进行复制而启动的转换，因为没有汉字，所以不需进行繁简转换，缺省值为false
        /// </summary>
        public static bool _isPaliWindowCopy = false;

        /// <summary>
        /// 对输入的字符串进行转换后输出，同时进行两种转换 1 转换pali字母编码 2 简繁转换 程序中类似功能的用于转换输出的函数有两个 outword() , outword_t()
        /// </summary>
        /// <param name="intext"></param>
        /// <returns></returns>
        public string outword_t(string intext)
        {
            if (intext == "")
                return intext;
            string sOut = "", sTmp = "";
            int i = 0;
            if (rbtnTahoma.Checked == true)
            {
                if (FAN.Checked && !_isPaliWindowCopy)
                    return Strings.StrConv(intext, VbStrConv.TraditionalChinese, 0x0409);
                else
                    return intext;
            }
            else
            {
                do
                {
                    if (intext.Length - i > 2000)
                        sTmp = intext.Substring(i, 2000);
                    else
                        sTmp = intext.Substring(i);
                    i = i + 2000;
                    string aword = "";
                    char[] cText = sTmp.ToCharArray();
                    //输出VriRomanPali字母编码
                    if (rbtnVriRomanPali.Checked == true)
                    {
                        foreach (char cB in cText)
                        {
                            if (Convert.ToUInt16(cB) < 7790)
                            {
                                switch (Convert.ToUInt16(cB))
                                {
                                    case 256:
                                        aword = aword + "¾";
                                        break;
                                    case 257:
                                        aword = aword + "±";
                                        break;
                                    case 298:
                                        aword = aword + "¿";
                                        break;
                                    case 299:
                                        aword = aword + "²";
                                        break;
                                    case 362:
                                        aword = aword + "Ð";
                                        break;
                                    case 363:
                                        aword = aword + "³";
                                        break;
                                    case 7748:
                                        aword = aword + "©";
                                        break;
                                    case 7749:
                                        aword = aword + "ª";
                                        break;
                                    case 209:
                                        aword = aword + "Ñ";
                                        break;
                                    case 241:
                                        aword = aword + "ñ";
                                        break;
                                    case 7788:
                                        aword = aword + "Ý";
                                        break;
                                    case 7789:
                                        aword = aword + "μ";
                                        break;
                                    case 7692:
                                        aword = aword + "Þ";
                                        break;
                                    case 7693:
                                        aword = aword + "¹";
                                        break;
                                    case 7750:
                                        aword = aword + "ð";
                                        break;
                                    case 7751:
                                        aword = aword + "º";
                                        break;
                                    case 7734:
                                        aword = aword + "ý";
                                        break;
                                    case 7735:
                                        aword = aword + "¼";
                                        break;
                                    case 330:
                                        aword = aword + "þ";
                                        break;
                                    case 331:
                                        aword = aword + "½";
                                        break;
                                    case 7744:
                                        aword = aword + "þ";
                                        break;
                                    case 7745:
                                        aword = aword + "½";
                                        break;
                                    case 7746:
                                        aword = aword + "þ";
                                        break;
                                    case 7747:
                                        aword = aword + "½";
                                        break;
                                    default:
                                        aword = aword + cB.ToString();
                                        break;
                                }
                            }
                            else
                                aword = aword + cB.ToString();
                        }
                    }
                    //输出Sangayana字母编码
                    if (rbtnSangayana.Checked == true)
                    {
                        foreach (char cB in cText)
                        {
                            if (Convert.ToUInt16(cB) < 7790)
                            {
                                switch (Convert.ToUInt16(cB))
                                {
                                    case 256:
                                        aword = aword + "â";
                                        break;
                                    case 257:
                                        aword = aword + "à";
                                        break;
                                    case 298:
                                        aword = aword + "ä";
                                        break;
                                    case 299:
                                        aword = aword + "ã";
                                        break;
                                    case 362:
                                        aword = aword + "æ";
                                        break;
                                    case 363:
                                        aword = aword + "å";
                                        break;
                                    case 7748:
                                        aword = aword + "ð";
                                        break;
                                    case 7749:
                                        aword = aword + "ï";
                                        break;
                                    case 209:
                                        aword = aword + "¥";
                                        break;
                                    case 241:
                                        aword = aword + "¤";
                                        break;
                                    case 7788:
                                        aword = aword + "ò";
                                        break;
                                    case 7789:
                                        aword = aword + "ñ";
                                        break;
                                    case 7692:
                                        aword = aword + "ô";
                                        break;
                                    case 7693:
                                        aword = aword + "ó";
                                        break;
                                    case 7750:
                                        aword = aword + "ö";
                                        break;
                                    case 7751:
                                        aword = aword + "õ";
                                        break;
                                    case 7734:
                                        aword = aword + "ì";
                                        break;
                                    case 7735:
                                        aword = aword + "ë";
                                        break;
                                    case 330:
                                        aword = aword + "ý";
                                        break;
                                    case 331:
                                        aword = aword + "ü";
                                        break;
                                    case 7744:
                                        aword = aword + "ý";
                                        break;
                                    case 7745:
                                        aword = aword + "§";
                                        break;
                                    case 7746:
                                        aword = aword + "ý";
                                        break;
                                    case 7747:
                                        aword = aword + "ü";
                                        break;
                                    default:
                                        aword = aword + cB.ToString();
                                        break;
                                }
                            }
                            else
                                aword = aword + cB.ToString();
                        }
                    }
                    sOut = sOut + aword;
                } while (intext.Length - i > 0);
                if (FAN.Checked && !_isPaliWindowCopy)
                    return Strings.StrConv(sOut, VbStrConv.TraditionalChinese, 0x0409);
                else
                    return sOut;
            }
        }

        /// <summary>
        /// 在中文查词的时候显示等待信息，在等待页面load完成后再由此页面的body调用han_pali_cc函数查词，然后在han_pali_cc函数的开始部分用webBrowser1.Update更新显示，不然等待信息显示不出來
        /// </summary>
        private void han_pali_waitHtm(string cWord)
        {
            webBrowser1.DocumentText =
                "<html>" +

                "<head>" +

                "<meta http-equiv='Content-Type' content='text/html; charset=utf-8'>" +

                "<style type='text/css'>" +

                //"body {  font-family:Tahoma; font-size:12px; line-height:18px; padding:0px 0px 0px 10px; margin:0px 0px 0px 0px;}" +
                //"body {font-family:Zawgyi-One,Tahoma; padding:0px 0px 0px 10px; margin:0px 0px 0px 0px;}" +
                "body {font-family:Tahoma; padding:0px 0px 0px 10px; margin:0px 0px 0px 0px;}" +
                //"p {color: #4169E1;}" +
                "p {color: #666666;}" +

                "</style>" +

                "</head>" +

                "<body onload = \"window.external.han_pali_cc('" + cWord + "')\">" +

                "<p><br><br>正在查找中文 '" + cWord + "' ...<br><br>请稍候...<br><br>假如等待时间超过<b>一秒</b>仍未显示查词结果，那么可能是由于您输入的字符中包含了一些特殊的字符而导致查词出错，请不必再等待，您可以修改所输入的字符或重新输入其它的字符继续查找。建议在进行中文反查时，只输入汉字（汉字字词间可试以英文的' '空格或','逗号分割）。</p>" +
                "<p>中文查词说明：由于中文查词暂时是从汉巴词典中反查pali单词，所以结果只是提供一个参考。</p>" +

                "</body>" +

                "</html>";
        }

        public void han_pali_cc(string cbWord)
        {
            if (cbWord.Length > 50)
            {
                webBrowser1.DocumentText = "您输入的字符过多！<br>在输入中文反查巴利词条时，最多可输入50个字符。<br>您可以减少输入的字符数量之后再重新查找！";
                return;
            }

            webBrowser1.Update();

            DateTime startD, endD;
            startD = DateTime.Now;

            strCxjg = "";

            int m = 0; //控制最多输出词条数

            //注释掉的这一段代码本来是用来查找中文的，但是此种查找方式速度太慢，从1.81版始不再使用它，改用下面的Lucene索引查找方式
            /*
            for (int n = 0; n < Program.NUM; n++)
            {
                if (Program.strL[n].Substring(0, 1) != "C" && Program.strL[n].Substring(0, 1) != "P")
                {
                    MatchCollection mc = new Regex(@"^(?<w>.*" + cbWord + ".*)$", RegexOptions.None).Matches(Program.strL[n].Substring(2));
                    foreach (Match ma in mc)
                    {
                        strCxjg = strCxjg + "<b style='font-size:20px; color:#006080;'>词：</b>" + ma.Groups["w"].Value + "<br><br>";
                        m++;
                    }
                    //if (m == 20)
                    if (m == -1) //取消输出限制
                        break;
                }
            }
            */

            //1.81版始使用下面的 searchCNWord函数 来查找中文，此函数采用Lucene索引查找，极大的提高了中文查找速度
            strCxjg = searchCNWord(cbWord);

            if (strCxjg == "")
                m = 0;
            else
                m = 1;
            //

            if (m == 0)
                strCxjg = "<br><br><br><p>程序词库里暂时没有符合的内容，您可以换个词试试！</p>";
            else
                strCxjg = new Regex(cbWord, RegexOptions.IgnoreCase).Replace(strCxjg, "<strong>" + cbWord + "</strong>");

            webBrowser1.DocumentText =
                "<html>" +

                "<head>" +

                "<meta http-equiv='Content-Type' content='text/html; charset=utf-8'>" +

                "<style type='text/css'>" +
                //字体全比原来大4px
                //"body {font-family:Zawgyi-One,Tahoma; font-size:16px; line-height:22px; padding:0px 0px 0px 10px; margin:0px 0px 0px 0px;}" +
                "body {font-family:Tahoma; font-size:16px; line-height:22px; padding:0px 0px 0px 10px; margin:0px 0px 0px 0px;}" +
                "p {color: #000000; padding: 0px 0px 0px 25px; margin: 0px 0px 0px 0px;}" +
                //上面颜色原为#666666
                "strong {color: #FF6600;}" +

                "</style>" +

                "</head>" +

                "<body>" +

                strHtmloutJS +

                outword(strCxjg) +
                "<br><br><br><br><br><br><br><br><br><br><br><br><br><br>" +

                "</body>" +

                "</html>";

            endD = DateTime.Now;
            System.TimeSpan ts = endD.Subtract(startD);

            toollbltimes.Text = " " + ts.TotalSeconds.ToString() + " s.";

            {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失
        }

        //新版
        string inword()
        {
            string aword = "";

            char[] cText = cboxInput.Text.Trim().ToCharArray();
            foreach (char cB in cText)
            {
                switch (Convert.ToUInt16(cB))
                {
                    case 330:
                        aword = aword + "Ṃ";
                        break;
                    case 331:
                        aword = aword + "ṃ";
                        break;
                    case 7744:
                        aword = aword + "Ṃ";
                        break;
                    case 7745:
                        aword = aword + "ṃ";
                        break;
                    default:
                        aword = aword + cB.ToString();
                        break;
                }
            }

            //排除连字符与66个巴利罗马字母之外的字符
            char[] charPLABC = { '\'', '-', ' ', 'A', 'a', 'Ā', 'ā', 'I', 'i', 'Ī', 'ī', 'U', 'u', 'Ū', 'ū', 'E', 'e', 'O', 'o', 'K', 'k', 'G', 'g', 'C', 'c', 'J', 'j', 'Ñ', 'ñ', 'Ṭ', 'ṭ', 'Ḍ', 'ḍ', 'T', 't', 'D', 'd', 'N', 'n', 'P', 'p', 'B', 'b', 'M', 'm', 'Y', 'y', 'R', 'r', 'L', 'l', 'V', 'v', 'S', 's', 'H', 'h', 'Ṅ', 'ṅ', 'Ṇ', 'ṇ', 'Ḷ', 'ḷ', 'Ṃ', 'ṃ', 'Ṁ', 'ṁ', 'Ŋ', 'ŋ' };
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

        //旧版
        /*
        string inword()
        {
            string aword = "";

            char[] cText = cboxInput.Text.Trim().ToCharArray();
            foreach (char cB in cText)
            {
                //处理Vri印度内观研究所版巴利三藏pdf档所使用的字母编码
                if (rbtnVriRomanPali.Checked == true)
                    switch (Convert.ToUInt16(cB))
                    {
                        case 190:
                            aword = aword + "Ā";
                            break;
                        case 177:
                            aword = aword + "ā";
                            break;
                        case 191:
                            aword = aword + "Ī";
                            break;
                        case 178:
                            aword = aword + "ī";
                            break;
                        case 208:
                            aword = aword + "Ū";
                            break;
                        case 179:
                            aword = aword + "ū";
                            break;
                        case 169:
                            aword = aword + "Ṅ";
                            break;
                        case 170:
                            aword = aword + "ṅ";
                            break;
                        case 221:
                            aword = aword + "Ṭ";
                            break;
                        case 956:
                            aword = aword + "ṭ";
                            break;
                        case 181:
                            aword = aword + "ṭ";
                            break;
                        case 222:
                            aword = aword + "Ḍ";
                            break;
                        case 185:
                            aword = aword + "ḍ";
                            break;
                        case 240:
                            aword = aword + "Ṇ";
                            break;
                        case 186:
                            aword = aword + "ṇ";
                            break;
                        case 253:
                            aword = aword + "Ḷ";
                            break;
                        case 188:
                            aword = aword + "ḷ";
                            break;
                        case 254:
                            aword = aword + "Ṃ";
                            break;
                        case 189:
                            aword = aword + "ṃ";
                            break;
                        default:
                            aword = aword + cB.ToString();
                            break;
                    }

                //处理Sangayana字母编码
                if (rbtnSangayana.Checked == true)
                    switch (Convert.ToUInt16(cB))
                    {
                        case 226:
                            aword = aword + "Ā";
                            break;
                        case 224:
                            aword = aword + "ā";
                            break;
                        case 228:
                            aword = aword + "Ī";
                            break;
                        case 227:
                            aword = aword + "ī";
                            break;
                        case 230:
                            aword = aword + "Ū";
                            break;
                        case 229:
                            aword = aword + "ū";
                            break;
                        case 240:
                            aword = aword + "Ṅ";
                            break;
                        case 239:
                            aword = aword + "ṅ";
                            break;
                        case 165:
                            aword = aword + "Ñ";
                            break;
                        case 164:
                            aword = aword + "ñ";
                            break;
                        case 242:
                            aword = aword + "Ṭ";
                            break;
                        case 241:
                            aword = aword + "ṭ";
                            break;
                        case 244:
                            aword = aword + "Ḍ";
                            break;
                        case 243:
                            aword = aword + "ḍ";
                            break;
                        case 246:
                            aword = aword + "Ṇ";
                            break;
                        case 245:
                            aword = aword + "ṇ";
                            break;
                        case 236:
                            aword = aword + "Ḷ";
                            break;
                        case 235:
                            aword = aword + "ḷ";
                            break;
                        case 253:
                            aword = aword + "Ṃ";
                            break;
                        case 252:
                            aword = aword + "ṃ";
                            break;
                        case 167:
                            aword = aword + "ṃ";
                            break;
                        default:
                            aword = aword + cB.ToString();
                            break;
                    }
                if (rbtnTahoma.Checked == true)
                    switch (Convert.ToUInt16(cB))
                    {
                        case 330:
                            aword = aword + "Ṃ";
                            break;
                        case 331:
                            aword = aword + "ṃ";
                            break;
                        case 7744:
                            aword = aword + "Ṃ";
                            break;
                        case 7745:
                            aword = aword + "ṃ";
                            break;
                        default:
                            aword = aword + cB.ToString();
                            break;
                    }
            }

            //排除连字符与66个巴利罗马字母之外的字符
            char[] charPLABC = { '\'', '-', ' ', 'A', 'a', 'Ā', 'ā', 'I', 'i', 'Ī', 'ī', 'U', 'u', 'Ū', 'ū', 'E', 'e', 'O', 'o', 'K', 'k', 'G', 'g', 'C', 'c', 'J', 'j', 'Ñ', 'ñ', 'Ṭ', 'ṭ', 'Ḍ', 'ḍ', 'T', 't', 'D', 'd', 'N', 'n', 'P', 'p', 'B', 'b', 'M', 'm', 'Y', 'y', 'R', 'r', 'L', 'l', 'V', 'v', 'S', 's', 'H', 'h', 'Ṅ', 'ṅ', 'Ṇ', 'ṇ', 'Ḷ', 'ḷ', 'Ṃ', 'ṃ', 'Ṁ', 'ṁ', 'Ŋ', 'ŋ' };
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
        */

        //新版
        string einword()
        {
            string aword = "";
            aword = cboxInput.Text.Trim();

            char[] ca = "āīūṅñṭḍṇḷŋĀĪŪṄÑṬḌṆḶŊṁṃṀṂ".ToCharArray();
            char[] cb = "aiunntdnlmAIUNNTDNLMmmMM".ToCharArray();
            int i = 0;
            foreach (char c in ca)
            {
                aword = new Regex(c.ToString(), RegexOptions.None).Replace(aword, cb[i].ToString());
                i++;
            }

            //排除42个英文字母和,'-',' ','\''之外的任何字符
            char[] charEABC = { '\'', '-', ' ', 'a', 'b', 'c', 'd', 'e', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'r', 's', 't', 'u', 'v', 'y', 'A', 'B', 'C', 'D', 'E', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'R', 'S', 'T', 'U', 'V', 'Y' };
            string saword = "";
            for (int z = 0; z < aword.Length; z++)
            {
                foreach (char c in charEABC)
                {
                    if (c.ToString() == aword.Substring(z, 1))
                        saword = saword + aword.Substring(z, 1);
                }
            }
            return saword;
        }

        //旧版
        /*
        string einword()
        {
            string aword = "";
            aword = cboxInput.Text.Trim();

            if (rbtnTahoma.Checked == true)
            {
                char[] ca = "āīūṅñṭḍṇḷŋĀĪŪṄÑṬḌṆḶŊṁṃṀṂ".ToCharArray();
                char[] cb = "aiunntdnlmAIUNNTDNLMmmMM".ToCharArray();
                int i = 0;
                foreach (char c in ca)
                {
                    aword = new Regex(c.ToString(), RegexOptions.None).Replace(aword, cb[i].ToString());
                    i++;
                }
            }

            if (rbtnSangayana.Checked == true)
            {
                char[] ca = "àãåï¤ñóõëü§âäæð¥òôöìý".ToCharArray();
                char[] cb = "aiunntdnlmmAIUNNTDNLM".ToCharArray();
                int i = 0;
                foreach (char c in ca)
                {
                    aword = new Regex(c.ToString(), RegexOptions.None).Replace(aword, cb[i].ToString());
                    i++;
                }
            }

            if (rbtnVriRomanPali.Checked == true)
            {
                char[] ca = "±²³ªñμµ¹º¼½¾¿Ð©ÑÝÞðýþ".ToCharArray();
                char[] cb = "aiunnttdnlmAIUNNTDNLM".ToCharArray();
                int i = 0;
                foreach (char c in ca)
                {
                    aword = new Regex(c.ToString(), RegexOptions.None).Replace(aword, cb[i].ToString());
                    i++;
                }
            }
            //排除42个英文字母和,'-',' ','\''之外的任何字符
            char[] charEABC = { '\'', '-', ' ', 'a', 'b', 'c', 'd', 'e', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'r', 's', 't', 'u', 'v', 'y', 'A', 'B', 'C', 'D', 'E', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'R', 'S', 'T', 'U', 'V', 'Y' };
            string saword = "";
            for (int z = 0; z < aword.Length; z++)
            {
                foreach (char c in charEABC)
                {
                    if (c.ToString() == aword.Substring(z, 1))
                        saword = saword + aword.Substring(z, 1);
                }
            }
            return saword;
        }
        */

        //新版
        string inword(string cboxInputWord)
        {
            string aword = "";

            char[] cText = cboxInputWord.Trim().ToCharArray();
            foreach (char cB in cText)
            {
                switch (Convert.ToUInt16(cB))
                {
                    case 330:
                        aword = aword + "Ṃ";
                        break;
                    case 331:
                        aword = aword + "ṃ";
                        break;
                    case 7744:
                        aword = aword + "Ṃ";
                        break;
                    case 7745:
                        aword = aword + "ṃ";
                        break;
                    default:
                        aword = aword + cB.ToString();
                        break;
                }
            }

            //排除连字符与66个巴利罗马字母之外的字符
            char[] charPLABC = { '\'', '-', ' ', 'A', 'a', 'Ā', 'ā', 'I', 'i', 'Ī', 'ī', 'U', 'u', 'Ū', 'ū', 'E', 'e', 'O', 'o', 'K', 'k', 'G', 'g', 'C', 'c', 'J', 'j', 'Ñ', 'ñ', 'Ṭ', 'ṭ', 'Ḍ', 'ḍ', 'T', 't', 'D', 'd', 'N', 'n', 'P', 'p', 'B', 'b', 'M', 'm', 'Y', 'y', 'R', 'r', 'L', 'l', 'V', 'v', 'S', 's', 'H', 'h', 'Ṅ', 'ṅ', 'Ṇ', 'ṇ', 'Ḷ', 'ḷ', 'Ṃ', 'ṃ', 'Ṁ', 'ṁ', 'Ŋ', 'ŋ' };
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

        //旧版
        /*
        string inword(string cboxInputWord)
        {
            string aword = "";

            char[] cText = cboxInputWord.Trim().ToCharArray();
            foreach (char cB in cText)
            {
                //处理Vri印度内观研究所版巴利三藏pdf档所使用的字母编码
                if (rbtnVriRomanPali.Checked && !_isPaliWeb)
                    switch (Convert.ToUInt16(cB))
                    {
                        case 190:
                            aword = aword + "Ā";
                            break;
                        case 177:
                            aword = aword + "ā";
                            break;
                        case 191:
                            aword = aword + "Ī";
                            break;
                        case 178:
                            aword = aword + "ī";
                            break;
                        case 208:
                            aword = aword + "Ū";
                            break;
                        case 179:
                            aword = aword + "ū";
                            break;
                        case 169:
                            aword = aword + "Ṅ";
                            break;
                        case 170:
                            aword = aword + "ṅ";
                            break;
                        case 221:
                            aword = aword + "Ṭ";
                            break;
                        case 956:
                            aword = aword + "ṭ";
                            break;
                        case 181:
                            aword = aword + "ṭ";
                            break;
                        case 222:
                            aword = aword + "Ḍ";
                            break;
                        case 185:
                            aword = aword + "ḍ";
                            break;
                        case 240:
                            aword = aword + "Ṇ";
                            break;
                        case 186:
                            aword = aword + "ṇ";
                            break;
                        case 253:
                            aword = aword + "Ḷ";
                            break;
                        case 188:
                            aword = aword + "ḷ";
                            break;
                        case 254:
                            aword = aword + "Ṃ";
                            break;
                        case 189:
                            aword = aword + "ṃ";
                            break;
                        default:
                            aword = aword + cB.ToString();
                            break;
                    }

                //处理Sangayana字母编码
                if (rbtnSangayana.Checked && !_isPaliWeb)
                    switch (Convert.ToUInt16(cB))
                    {
                        case 226:
                            aword = aword + "Ā";
                            break;
                        case 224:
                            aword = aword + "ā";
                            break;
                        case 228:
                            aword = aword + "Ī";
                            break;
                        case 227:
                            aword = aword + "ī";
                            break;
                        case 230:
                            aword = aword + "Ū";
                            break;
                        case 229:
                            aword = aword + "ū";
                            break;
                        case 240:
                            aword = aword + "Ṅ";
                            break;
                        case 239:
                            aword = aword + "ṅ";
                            break;
                        case 165:
                            aword = aword + "Ñ";
                            break;
                        case 164:
                            aword = aword + "ñ";
                            break;
                        case 242:
                            aword = aword + "Ṭ";
                            break;
                        case 241:
                            aword = aword + "ṭ";
                            break;
                        case 244:
                            aword = aword + "Ḍ";
                            break;
                        case 243:
                            aword = aword + "ḍ";
                            break;
                        case 246:
                            aword = aword + "Ṇ";
                            break;
                        case 245:
                            aword = aword + "ṇ";
                            break;
                        case 236:
                            aword = aword + "Ḷ";
                            break;
                        case 235:
                            aword = aword + "ḷ";
                            break;
                        case 253:
                            aword = aword + "Ṃ";
                            break;
                        case 252:
                            aword = aword + "ṃ";
                            break;
                        case 167:
                            aword = aword + "ṃ";
                            break;
                        default:
                            aword = aword + cB.ToString();
                            break;
                    }

                if (rbtnTahoma.Checked || _isPaliWeb)
                    switch (Convert.ToUInt16(cB))
                    {
                        case 330:
                            aword = aword + "Ṃ";
                            break;
                        case 331:
                            aword = aword + "ṃ";
                            break;
                        case 7744:
                            aword = aword + "Ṃ";
                            break;
                        case 7745:
                            aword = aword + "ṃ";
                            break;
                        default:
                            aword = aword + cB.ToString();
                            break;
                    }
            }

            //排除连字符与66个巴利罗马字母之外的字符
            char[] charPLABC = { '\'', '-', ' ', 'A', 'a', 'Ā', 'ā', 'I', 'i', 'Ī', 'ī', 'U', 'u', 'Ū', 'ū', 'E', 'e', 'O', 'o', 'K', 'k', 'G', 'g', 'C', 'c', 'J', 'j', 'Ñ', 'ñ', 'Ṭ', 'ṭ', 'Ḍ', 'ḍ', 'T', 't', 'D', 'd', 'N', 'n', 'P', 'p', 'B', 'b', 'M', 'm', 'Y', 'y', 'R', 'r', 'L', 'l', 'V', 'v', 'S', 's', 'H', 'h', 'Ṅ', 'ṅ', 'Ṇ', 'ṇ', 'Ḷ', 'ḷ', 'Ṃ', 'ṃ', 'Ṁ', 'ṁ', 'Ŋ', 'ŋ' };
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
        */

        /// <summary>
        /// 将输入的VriRomanPali或Sangayana字体编码的字符串转换为Tahoma(unicode)字体编码的字符串，此函数是从旧版 string inword(string cboxInputWord) 函数修改而得
        /// </summary>
        public string inword_t(string s)
        {
            string aword = "";

            char[] cText = s.ToCharArray();

            //处理Vri印度内观研究所版巴利三藏pdf档所使用的字母编码
            if (rbtnVriRomanPali.Checked && !_isPaliWeb)
                foreach (char cB in cText)
                {
                    switch (Convert.ToUInt16(cB))
                    {
                        case 190:
                            aword = aword + "Ā";
                            break;
                        case 177:
                            aword = aword + "ā";
                            break;
                        case 191:
                            aword = aword + "Ī";
                            break;
                        case 178:
                            aword = aword + "ī";
                            break;
                        case 208:
                            aword = aword + "Ū";
                            break;
                        case 179:
                            aword = aword + "ū";
                            break;
                        case 169:
                            aword = aword + "Ṅ";
                            break;
                        case 170:
                            aword = aword + "ṅ";
                            break;
                        case 221:
                            aword = aword + "Ṭ";
                            break;
                        case 956:
                            aword = aword + "ṭ";
                            break;
                        case 181:
                            aword = aword + "ṭ";
                            break;
                        case 222:
                            aword = aword + "Ḍ";
                            break;
                        case 185:
                            aword = aword + "ḍ";
                            break;
                        case 240:
                            aword = aword + "Ṇ";
                            break;
                        case 186:
                            aword = aword + "ṇ";
                            break;
                        case 253:
                            aword = aword + "Ḷ";
                            break;
                        case 188:
                            aword = aword + "ḷ";
                            break;
                        case 254:
                            aword = aword + "Ṃ";
                            break;
                        case 189:
                            aword = aword + "ṃ";
                            break;
                        default:
                            aword = aword + cB.ToString();
                            break;
                    }
                }

            //处理Sangayana字母编码
            if (rbtnSangayana.Checked && !_isPaliWeb)
                foreach (char cB in cText)
                {
                    switch (Convert.ToUInt16(cB))
                    {
                        case 226:
                            aword = aword + "Ā";
                            break;
                        case 224:
                            aword = aword + "ā";
                            break;
                        case 228:
                            aword = aword + "Ī";
                            break;
                        case 227:
                            aword = aword + "ī";
                            break;
                        case 230:
                            aword = aword + "Ū";
                            break;
                        case 229:
                            aword = aword + "ū";
                            break;
                        case 240:
                            aword = aword + "Ṅ";
                            break;
                        case 239:
                            aword = aword + "ṅ";
                            break;
                        case 165:
                            aword = aword + "Ñ";
                            break;
                        case 164:
                            aword = aword + "ñ";
                            break;
                        case 242:
                            aword = aword + "Ṭ";
                            break;
                        case 241:
                            aword = aword + "ṭ";
                            break;
                        case 244:
                            aword = aword + "Ḍ";
                            break;
                        case 243:
                            aword = aword + "ḍ";
                            break;
                        case 246:
                            aword = aword + "Ṇ";
                            break;
                        case 245:
                            aword = aword + "ṇ";
                            break;
                        case 236:
                            aword = aword + "Ḷ";
                            break;
                        case 235:
                            aword = aword + "ḷ";
                            break;
                        case 253:
                            aword = aword + "Ṃ";
                            break;
                        case 252:
                            aword = aword + "ṃ";
                            break;
                        case 167:
                            aword = aword + "ṃ";
                            break;
                        default:
                            aword = aword + cB.ToString();
                            break;
                    }
                }

            if (rbtnTahoma.Checked || _isPaliWeb)
                aword = s;

            return aword;
        }

        //新版
        string einword(string cboxInputWord)
        {
            string aword = "";
            aword = cboxInputWord.Trim();

            char[] ca = "āīūṅñṭḍṇḷŋĀĪŪṄÑṬḌṆḶŊṁṃṀṂ".ToCharArray();
            char[] cb = "aiunntdnlmAIUNNTDNLMmmMM".ToCharArray();
            int i = 0;
            foreach (char c in ca)
            {
                aword = new Regex(c.ToString(), RegexOptions.None).Replace(aword, cb[i].ToString());
                i++;
            }

            //排除42个英文字母与-'空格符之外的任何字符
            char[] charEABC = { '\'', '-', ' ', 'a', 'b', 'c', 'd', 'e', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'r', 's', 't', 'u', 'v', 'y', 'A', 'B', 'C', 'D', 'E', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'R', 'S', 'T', 'U', 'V', 'Y' };
            string saword = "";
            for (int z = 0; z < aword.Length; z++)
            {
                foreach (char c in charEABC)
                {
                    if (c.ToString() == aword.Substring(z, 1))
                        saword = saword + aword.Substring(z, 1);
                }
            }
            return saword;
        }

        //旧版
        /*
        string einword(string cboxInputWord)
        {
            string aword = "";
            aword = cboxInputWord.Trim();

            if (rbtnTahoma.Checked || _isPaliWeb)
            {
                char[] ca = "āīūṅñṭḍṇḷŋĀĪŪṄÑṬḌṆḶŊṁṃṀṂ".ToCharArray();
                char[] cb = "aiunntdnlmAIUNNTDNLMmmMM".ToCharArray();
                int i = 0;
                foreach (char c in ca)
                {
                    aword = new Regex(c.ToString(), RegexOptions.None).Replace(aword, cb[i].ToString());
                    i++;
                }
            }

            if (rbtnSangayana.Checked && !_isPaliWeb)
            {
                char[] ca = "àãåï¤ñóõëü§âäæð¥òôöìý".ToCharArray();
                char[] cb = "aiunntdnlmmAIUNNTDNLM".ToCharArray();
                int i = 0;
                foreach (char c in ca)
                {
                    aword = new Regex(c.ToString(), RegexOptions.None).Replace(aword, cb[i].ToString());
                    i++;
                }
            }

            if (rbtnVriRomanPali.Checked && !_isPaliWeb)
            {
                char[] ca = "±²³ªñμµ¹º¼½¾¿Ð©ÑÝÞðýþ".ToCharArray();
                char[] cb = "aiunnttdnlmAIUNNTDNLM".ToCharArray();
                int i = 0;
                foreach (char c in ca)
                {
                    aword = new Regex(c.ToString(), RegexOptions.None).Replace(aword, cb[i].ToString());
                    i++;
                }
            }

            //排除42个英文字母与-'空格符之外的任何字符
            char[] charEABC = { '\'', '-', ' ', 'a', 'b', 'c', 'd', 'e', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'r', 's', 't', 'u', 'v', 'y', 'A', 'B', 'C', 'D', 'E', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'R', 'S', 'T', 'U', 'V', 'Y' };
            string saword = "";
            for (int z = 0; z < aword.Length; z++)
            {
                foreach (char c in charEABC)
                {
                    if (c.ToString() == aword.Substring(z, 1))
                        saword = saword + aword.Substring(z, 1);
                }
            }
            return saword;
        }
        */

        string einwordfhc(string txtword)
        {
            string aword = "";
            aword = txtword.Trim();

            char[] ca = "āīūṅñṭḍṇḷŋĀĪŪṄÑṬḌṆḶŊṁṃṀṂ".ToCharArray();
            char[] cb = "aiunntdnlmAIUNNTDNLMmmMM".ToCharArray();
            int i = 0;
            foreach (char c in ca)
            {
                aword = new Regex(c.ToString(), RegexOptions.None).Replace(aword, cb[i].ToString());
                i++;
            }

            //排除42个英文字母与-'空格符之外的任何字符
            char[] charEABC = { '\'', '-', ' ', 'a', 'b', 'c', 'd', 'e', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'r', 's', 't', 'u', 'v', 'y', 'A', 'B', 'C', 'D', 'E', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'R', 'S', 'T', 'U', 'V', 'Y' };
            string saword = "";
            for (int z = 0; z < aword.Length; z++)
            {
                foreach (char c in charEABC)
                {
                    if (c.ToString() == aword.Substring(z, 1))
                        saword = saword + aword.Substring(z, 1);
                }
            }
            return saword;
        }

        string einwordfhc_old(string txtword)
        {
            string aword = "";
            aword = txtword.Trim();

            if (rbtnTahoma.Checked == true)
            {
                char[] ca = "āīūṅñṭḍṇḷŋĀĪŪṄÑṬḌṆḶŊṁṃṀṂ".ToCharArray();
                char[] cb = "aiunntdnlmAIUNNTDNLMmmMM".ToCharArray();
                int i = 0;
                foreach (char c in ca)
                {
                    aword = new Regex(c.ToString(), RegexOptions.None).Replace(aword, cb[i].ToString());
                    i++;
                }
            }

            if (rbtnSangayana.Checked == true)
            {
                char[] ca = "àãåï¤ñóõëü§âäæð¥òôöìý".ToCharArray();
                char[] cb = "aiunntdnlmmAIUNNTDNLM".ToCharArray();
                int i = 0;
                foreach (char c in ca)
                {
                    aword = new Regex(c.ToString(), RegexOptions.None).Replace(aword, cb[i].ToString());
                    i++;
                }
            }

            if (rbtnVriRomanPali.Checked == true)
            {
                char[] ca = "±²³ªñμµ¹º¼½¾¿Ð©ÑÝÞðýþ".ToCharArray();
                char[] cb = "aiunnttdnlmAIUNNTDNLM".ToCharArray();
                int i = 0;
                foreach (char c in ca)
                {
                    aword = new Regex(c.ToString(), RegexOptions.None).Replace(aword, cb[i].ToString());
                    i++;
                }
            }
            //排除47个英文字母,'-',' '之外的任何字符
            char[] charEABC = { '\'', '-', ' ', 'a', 'b', 'c', 'd', 'e', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'r', 's', 't', 'u', 'v', 'y', 'A', 'B', 'C', 'D', 'E', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'R', 'S', 'T', 'U', 'V', 'Y' };
            string saword = "";
            for (int z = 0; z < aword.Length; z++)
            {
                foreach (char c in charEABC)
                {
                    if (c.ToString() == aword.Substring(z, 1))
                        saword = saword + aword.Substring(z, 1);
                }
            }
            return saword;
        }

        //英文输入
        string ewinword(string strInputWord)
        {
            string aword = "", saword = "";
            aword = strInputWord.Trim();

            //排除52个英文字母,'-',' '之外的任何字符
            char[] charEABC = { '\'', '-', ' ', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            for (int z = 0; z < aword.Length; z++)
            {
                foreach (char c in charEABC)
                {
                    if (c.ToString() == aword.Substring(z, 1))
                        saword = saword + aword.Substring(z, 1);
                }
            }
            return saword;
        }

        //不再使用
        public void esyCC()
        {
            string sbword = "";
            strCxjg = "";

            DateTime startD, endD;
            startD = DateTime.Now;

            if (cboxInput.Text.Trim() == "")
                return;

            if (cboxInput.Text.Trim().Length > 35)
            {
                webBrowser1.DocumentText = "本词典最长单词为35个字母，您输入的单词过长！\r\n请重新输入！";
                return;
            }

            int n = 0;  //记录查找到的结果条数
            string nword = "", saword = "";

            nword = einword();
            saword = nword;

            if (nword == "")
            {
                webBrowser1.DocumentText = "输入的字符不是正确的英文字母，请重新输入！";
                return;
            }

            webBrowser1.DocumentText = "looking up the word: " + nword + " ...";
            strCxjg = nword + "  " + "word translate:" + "\r\n\r\n";

            FileStream bFile = new FileStream(@".\e-pali\cidian", FileMode.Open);
            StreamReader srb = new StreamReader(bFile, System.Text.Encoding.GetEncoding("utf-8"));
            FileStream aF1m = new FileStream(@".\e-pali\indexdat", FileMode.Open);

            if (nword.Length < 4)
            {
                sbword = nword;

                byte[] bm = new byte[8] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                int um = 0;

                for (int im = 0; im < sbword.Length; im++)
                {
                    aF1m.Seek(um + Program.eABC(sbword.Substring(im, 1)) * 8, SeekOrigin.Begin);
                    aF1m.Read(bm, 0, 8);
                    if ((int)(bm[0]) == 0)
                    {
                        goto kkk;
                    }
                    else if ((int)(bm[0]) == 1)
                    {
                        if (im < sbword.Length - 1)
                            um = (int)(bm[1] | bm[2] << 8 | bm[3] << 16 | bm[4] << 24);
                        else
                        {
                            goto kkk;
                        }
                    }
                    else if ((int)(bm[0]) == 2)
                    {
                        if (im < sbword.Length - 1)
                        {
                            goto kkk;
                        }
                        else
                            um = (int)(bm[5] | bm[6] << 8 | bm[7] << 16 | 0);
                    }
                    else if ((int)(bm[0]) == 3)
                    {
                        if (im < sbword.Length - 1)
                            um = (int)(bm[1] | bm[2] << 8 | bm[3] << 16 | bm[4] << 24);
                        else
                            um = (int)(bm[5] | bm[6] << 8 | bm[7] << 16 | 0);
                    }
                }

                srb.DiscardBufferedData();//对于StreamReader来说，在使用ReadLine()后，如再要seek()则这一句很必要，但对于FileStream的使用，似乎就没有这个问题
                srb.BaseStream.Seek(um, System.IO.SeekOrigin.Begin);

                Regex re1m = new Regex(@"^(.+?) :(?<w>.*)", RegexOptions.IgnoreCase);
                MatchCollection mc1m = re1m.Matches(srb.ReadLine());
                foreach (Match ma1m in mc1m)
                {
                    strCxjg = strCxjg + sbword.PadRight(nword.Length, ' ') + " :" + ma1m.Groups["w"].Value + "\r\n";
                    n++;
                }

            kkk:
                ;
            }

            for (int i = 0; i < nword.Length; i++)
            {
                for (int j = saword.Length; j > 3; j--)
                {
                    sbword = saword.Substring(0, j);

                    byte[] bm = new byte[8] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    int um = 0;

                    for (int im = 0; im < sbword.Length; im++)
                    {
                        aF1m.Seek(um + Program.eABC(sbword.Substring(im, 1)) * 8, SeekOrigin.Begin);
                        aF1m.Read(bm, 0, 8);
                        if ((int)(bm[0]) == 0)
                        {
                            goto kkk;
                        }
                        else if ((int)(bm[0]) == 1)
                        {
                            if (im < sbword.Length - 1)
                                um = (int)(bm[1] | bm[2] << 8 | bm[3] << 16 | bm[4] << 24);
                            else
                            {
                                goto kkk;
                            }
                        }
                        else if ((int)(bm[0]) == 2)
                        {
                            if (im < sbword.Length - 1)
                            {
                                goto kkk;
                            }
                            else
                                um = (int)(bm[5] | bm[6] << 8 | bm[7] << 16 | 0);
                        }
                        else if ((int)(bm[0]) == 3)
                        {
                            if (im < sbword.Length - 1)
                                um = (int)(bm[1] | bm[2] << 8 | bm[3] << 16 | bm[4] << 24);
                            else
                                um = (int)(bm[5] | bm[6] << 8 | bm[7] << 16 | 0);
                        }
                    }

                    srb.DiscardBufferedData();
                    srb.BaseStream.Seek(um, System.IO.SeekOrigin.Begin);

                    sbword = sbword.PadLeft(sbword.Length + i, ' ');
                    Regex re1m = new Regex(@"^(.+?) :(?<w>.*)", RegexOptions.IgnoreCase);
                    MatchCollection mc1m = re1m.Matches(srb.ReadLine());
                    foreach (Match ma1m in mc1m)
                    {
                        strCxjg = strCxjg + sbword.PadRight(nword.Length, ' ') + " :" + ma1m.Groups["w"].Value + "\r\n";
                        n++;
                    }

                kkk:
                    ;
                }
                saword = saword.Substring(1);
            }
            srb.Close();
            aF1m.Close();

            endD = DateTime.Now;
            System.TimeSpan ts = endD.Subtract(startD);
            strCxjg = strCxjg + "( " + ts.TotalMilliseconds.ToString() + " milliseconds.)";
            webBrowser1.DocumentText = outword(strCxjg);

            if (n == 0)
                webBrowser1.DocumentText = "\r\nNo found.";

            {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失
        }

        private void language_fan()
        {
            FAN.Checked = true;
            HAN.Checked = false;
            EN.Checked = false;

            treeView1.Nodes["trRoot"].Text = outword("词典导航：");

            /*
            if (treeView1.Nodes["trRoot"].Nodes.ContainsKey("nodD"))
            {
                treeView1.Nodes["trRoot"].Nodes["nodD"].Text = outword("《巴汉词典》");
                treeView1.Nodes["trRoot"].Nodes["nodD"].ToolTipText = outword("Mahāñāṇo Bhikkhu编著");
            }
            if (treeView1.Nodes["trRoot"].Nodes.ContainsKey("nodF"))
            {
                treeView1.Nodes["trRoot"].Nodes["nodF"].Text = outword("《巴汉词典》增订版");
                treeView1.Nodes["trRoot"].Nodes["nodF"].ToolTipText = outword("明法比丘增订");
            }
            if (treeView1.Nodes["trRoot"].Nodes.ContainsKey("nodL"))
            {
                treeView1.Nodes["trRoot"].Nodes["nodL"].Text = outword("巴汉辞典");
                treeView1.Nodes["trRoot"].Nodes["nodL"].ToolTipText = outword("编者：(台湾斗六)廖文灿");
            }
            if (treeView1.Nodes["trRoot"].Nodes.ContainsKey("nodM"))
            {
                treeView1.Nodes["trRoot"].Nodes["nodM"].Text = outword("巴利语汇解");
                treeView1.Nodes["trRoot"].Nodes["nodM"].ToolTipText = outword("&巴利新音译 玛欣德尊者");
            }
            if (treeView1.Nodes["trRoot"].Nodes.ContainsKey("nodG"))
            {
                treeView1.Nodes["trRoot"].Nodes["nodG"].Text = outword("字汇");
                treeView1.Nodes["trRoot"].Nodes["nodG"].ToolTipText = outword("四念住课程开示集要(葛印卡)");
            }
            if (treeView1.Nodes["trRoot"].Nodes.ContainsKey("nodW"))
            {
                treeView1.Nodes["trRoot"].Nodes["nodW"].Text = outword("巴英术语汇编");
                treeView1.Nodes["trRoot"].Nodes["nodW"].ToolTipText = outword("《法的医疗》附 温宗堃");
            }
            if (treeView1.Nodes["trRoot"].Nodes.ContainsKey("nodZ"))
            {
                treeView1.Nodes["trRoot"].Nodes["nodZ"].Text = outword("巴汉佛学辞汇");
                treeView1.Nodes["trRoot"].Nodes["nodZ"].ToolTipText = outword("巴利文-汉文佛学名相辞汇 翻译:张文明居士");
            }
            */

            menuLanguageSet.Text = s[6];
            toollbltimes.Text = "";
            this.Text = Strings.StrConv("巴利三藏电子辞典 版本", VbStrConv.TraditionalChinese, 0x0409) + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            menupalimulu.Text = s[5];
            menuosEnglishPali.Text = s[40];
            gboxFontSet.Text = s[41];
            menuOtherset.Text = s[58];
            menuosBlurinputmode.Text = s[44];
            btnLookup.Text = s[50];
            menuosSetWindowInFront.Text = s[54];

            /*
            if (menuosEnglishPali.Checked)
                toollbl.Text = outword("当前词典设置：英语-->巴利文");
            else
                toollbl.Text = outword("当前词典设置：巴利文-->汉语、英语；汉语-->巴利文");
            */
            copy.Text = s[56];
            selectAll.Text = s[57];
            menuhReadme.Text = s[51];
            menuHelp.Text = s[22];
            menuhFofa.Text = s[24];
            menuhJwzl.Text = s[25];
            menuhHowto.Text = s[30];
            cboxABC.Text = s[48];
            lblFhc.Text = s[49];

            frmmuluwindow.Text = "巴利三藏目錄";
            frmmuluwindow.rbtnPali.Text = "巴利語";
            frmmuluwindow.rbtnCn.Text = "中文";
            frmmuluwindow.btnLucn.Text = "搜索";

            Program.toolbarform.tsbmulu.Text = "三藏";
            Program.toolbarform.tsbSearch.Text = "搜索";
            Program.toolbarform.tsbdict.Text = "詞典";
            Program.toolbarform.tsddb.Text = "窗口";
            Program.toolbarform.tsddbWindow.Text = "管理";
            Program.toolbarform.tsbHide.Text = "隱藏";
            Program.toolbarform.tsbquit.Text = "退出";
            Program.toolbarform.cboxHccc.Text = "劃詞查詞";

            Program.ssfrm.Text = "巴利三藏全文搜索";
            Program.ssfrm.button2.Text = "搜索";
            Program.ssfrm.button1.Text = "索引重建";
            Program.ssfrm.btnPageFirst.Text = "|<最前";
            Program.ssfrm.btnPagePrior.Text = "<前一";
            Program.ssfrm.btnPageNext.Text = "下一>";
            Program.ssfrm.btnPageLast.Text = "最後>|";
            Program.ssfrm.textBox2.Text = "在此以英文字母輸入詞的前部分，然後按Enter鍵、按Enter鍵、按Enter鍵";
        }

        private void language_han()
        {
            HAN.Checked = true;
            FAN.Checked = false;
            EN.Checked = false;

            treeView1.Nodes["trRoot"].Text = "词典导航：";

            /*
            if (treeView1.Nodes["trRoot"].Nodes.ContainsKey("nodD"))
            {
                treeView1.Nodes["trRoot"].Nodes["nodD"].Text = "《巴汉词典》";
                treeView1.Nodes["trRoot"].Nodes["nodD"].ToolTipText = "Mahāñāṇo Bhikkhu编著";
            }
            if (treeView1.Nodes["trRoot"].Nodes.ContainsKey("nodF"))
            {
                treeView1.Nodes["trRoot"].Nodes["nodF"].Text = "《巴汉词典》增订版";
                treeView1.Nodes["trRoot"].Nodes["nodF"].ToolTipText = "明法比丘增订";
            }
            if (treeView1.Nodes["trRoot"].Nodes.ContainsKey("nodL"))
            {
                treeView1.Nodes["trRoot"].Nodes["nodL"].Text = "巴汉辞典";
                treeView1.Nodes["trRoot"].Nodes["nodL"].ToolTipText = "编者：(台湾斗六)廖文灿";
            }
            if (treeView1.Nodes["trRoot"].Nodes.ContainsKey("nodM"))
            {
                treeView1.Nodes["trRoot"].Nodes["nodM"].Text = "巴利语汇解";
                treeView1.Nodes["trRoot"].Nodes["nodM"].ToolTipText = "&巴利新音译 玛欣德尊者";
            }
            if (treeView1.Nodes["trRoot"].Nodes.ContainsKey("nodG"))
            {
                treeView1.Nodes["trRoot"].Nodes["nodG"].Text = "字汇";
                treeView1.Nodes["trRoot"].Nodes["nodG"].ToolTipText = "四念住课程开示集要(葛印卡)";
            }
            if (treeView1.Nodes["trRoot"].Nodes.ContainsKey("nodW"))
            {
                treeView1.Nodes["trRoot"].Nodes["nodW"].Text = "巴英术语汇编";
                treeView1.Nodes["trRoot"].Nodes["nodW"].ToolTipText = "《法的医疗》附 温宗堃";
            }
            if (treeView1.Nodes["trRoot"].Nodes.ContainsKey("nodZ"))
            {
                treeView1.Nodes["trRoot"].Nodes["nodZ"].Text = "巴汉佛学辞汇";
                treeView1.Nodes["trRoot"].Nodes["nodZ"].ToolTipText = "巴利文-汉文佛学名相辞汇 翻译:张文明居士";
            }
            */

            menuLanguageSet.Text = "语言设置";
            toollbltimes.Text = "";
            this.Text = "巴利三藏电子辞典 版本" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            menupalimulu.Text = "巴利三藏";
            menuosEnglishPali.Text = "英语";
            gboxFontSet.Text = "输入输出字体编码设置：";
            menuOtherset.Text = "其它设置";
            menuosBlurinputmode.Text = "模糊匹配";
            btnLookup.Text = "查    词";
            menuosSetWindowInFront.Text = "设置本窗体总在最前面";
            /*
            if (menuosEnglishPali.Checked)
                toollbl.Text = "当前词典设置：英语-->巴利文";
            else
                toollbl.Text = "当前词典设置：巴利文-->汉语、英语；汉语-->巴利文";
            */
            copy.Text = "复制";
            selectAll.Text = "全选";
            menuhReadme.Text = "使用说明";
            menuHelp.Text = "帮助";
            menuhFofa.Text = "《佛陀的启示》";
            menuhJwzl.Text = "'觉悟之路'网站简介";
            menuhHowto.Text = "怎样输入巴利音标字母";
            cboxABC.Text = "小写";
            lblFhc.Text = "复合词分析结果(在此框里双击拆分结果词自动查找其解释):";

            frmmuluwindow.Text = "巴利三藏目录";
            frmmuluwindow.rbtnPali.Text = "巴利语";
            frmmuluwindow.rbtnCn.Text = "中文";
            frmmuluwindow.btnLucn.Text = "搜索";

            Program.toolbarform.tsbmulu.Text = "三藏";
            Program.toolbarform.tsbSearch.Text = "搜索";
            Program.toolbarform.tsbdict.Text = "词典";
            Program.toolbarform.tsddb.Text = "窗口";
            Program.toolbarform.tsddbWindow.Text = "管理";
            Program.toolbarform.tsbHide.Text = "隐藏";
            Program.toolbarform.tsbquit.Text = "退出";
            Program.toolbarform.cboxHccc.Text = "划词查词";

            Program.ssfrm.Text = "巴利三藏全文搜索";
            Program.ssfrm.button2.Text = "搜索";
            Program.ssfrm.button1.Text = "索引重建";
            Program.ssfrm.btnPageFirst.Text = "|<最前";
            Program.ssfrm.btnPagePrior.Text = "<前一";
            Program.ssfrm.btnPageNext.Text = "下一>";
            Program.ssfrm.btnPageLast.Text = "最后>|";
            Program.ssfrm.textBox2.Text = "在此以英文字母输入词的前部分，然后按Enter键、按Enter键、按Enter键";
        }

        private void language_en()
        {
            EN.Checked = true;
            FAN.Checked = false;
            HAN.Checked = false;

            treeView1.Nodes["trRoot"].Text = "Dictionary navigation:";

            /*
            if (treeView1.Nodes["trRoot"].Nodes.ContainsKey("nodD"))
            {
                treeView1.Nodes["trRoot"].Nodes["nodD"].Text = "《巴汉词典》";
                treeView1.Nodes["trRoot"].Nodes["nodD"].ToolTipText = "Mahāñāṇo Bhikkhu编著";
            }
            if (treeView1.Nodes["trRoot"].Nodes.ContainsKey("nodF"))
            {
                treeView1.Nodes["trRoot"].Nodes["nodF"].Text = "《巴汉词典》增订版";
                treeView1.Nodes["trRoot"].Nodes["nodF"].ToolTipText = "明法比丘增订";
            }
            if (treeView1.Nodes["trRoot"].Nodes.ContainsKey("nodL"))
            {
                treeView1.Nodes["trRoot"].Nodes["nodL"].Text = "巴汉辞典";
                treeView1.Nodes["trRoot"].Nodes["nodL"].ToolTipText = "编者：(台湾斗六)廖文灿";
            }
            if (treeView1.Nodes["trRoot"].Nodes.ContainsKey("nodM"))
            {
                treeView1.Nodes["trRoot"].Nodes["nodM"].Text = "巴利语汇解";
                treeView1.Nodes["trRoot"].Nodes["nodM"].ToolTipText = "&巴利新音译 玛欣德尊者";
            }
            if (treeView1.Nodes["trRoot"].Nodes.ContainsKey("nodG"))
            {
                treeView1.Nodes["trRoot"].Nodes["nodG"].Text = "字汇";
                treeView1.Nodes["trRoot"].Nodes["nodG"].ToolTipText = "四念住课程开示集要(葛印卡)";
            }
            if (treeView1.Nodes["trRoot"].Nodes.ContainsKey("nodW"))
            {
                treeView1.Nodes["trRoot"].Nodes["nodW"].Text = "巴英术语汇编";
                treeView1.Nodes["trRoot"].Nodes["nodW"].ToolTipText = "《法的医疗》附 温宗堃";
            }
            if (treeView1.Nodes["trRoot"].Nodes.ContainsKey("nodZ"))
            {
                treeView1.Nodes["trRoot"].Nodes["nodZ"].Text = "巴汉佛学辞汇";
                treeView1.Nodes["trRoot"].Nodes["nodZ"].ToolTipText = "巴利文-汉文佛学名相辞汇 翻译:张文明居士";
            }
            */

            menuLanguageSet.Text = "Language set";
            toollbltimes.Text = "";
            this.Text = "Pali Tipitaka Dictionary " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            menupalimulu.Text = "pali canon";
            menuosEnglishPali.Text = "en-pali";
            gboxFontSet.Text = "input && output font-code setting:";
            menuOtherset.Text = "Other set";
            menuosBlurinputmode.Text = "blur-input";
            btnLookup.Text = "look    up";
            menuosSetWindowInFront.Text = "set This form is always at the front";
            /*
            if (menuosEnglishPali.Checked)
                toollbl.Text = "now dictionary set: English-->Pali";
            else
                toollbl.Text = "now dictionary set: Pali-->English ,Chinese ; Chinese-->Pali";
            */
            copy.Text = "copy";
            selectAll.Text = "select all";
            menuhReadme.Text = "readme";
            menuHelp.Text = "help";
            menuhFofa.Text = "What The Buddha Taught";
            menuhJwzl.Text = "about 'www.dhamma.org.cn'";
            menuhHowto.Text = "how to input pali letter";
            cboxABC.Text = "abc";
            lblFhc.Text = "complex-word Box(in this textbox, double-click the word, auto look up.):";

            frmmuluwindow.Text = "pali tipitaka catalog";
            frmmuluwindow.rbtnPali.Text = "pāli";
            frmmuluwindow.rbtnCn.Text = "chinese";
            frmmuluwindow.btnLucn.Text = "search";

            Program.toolbarform.tsbmulu.Text = "tipitaka";
            Program.toolbarform.tsbSearch.Text = "search";
            Program.toolbarform.tsbdict.Text = "dictionary";
            Program.toolbarform.tsddb.Text = "window";
            Program.toolbarform.tsddbWindow.Text = "manage";
            Program.toolbarform.tsbHide.Text = "hide";
            Program.toolbarform.tsbquit.Text = "quit";
            Program.toolbarform.cboxHccc.Text = "select word";

            Program.ssfrm.Text = "pāli tipiṭaka full text search";
            Program.ssfrm.button2.Text = "search";
            Program.ssfrm.button1.Text = "remade Index";
            Program.ssfrm.btnPageFirst.Text = "|<First";
            Program.ssfrm.btnPagePrior.Text = "<Prior";
            Program.ssfrm.btnPageNext.Text = "Next>";
            Program.ssfrm.btnPageLast.Text = "Last>|";
            Program.ssfrm.textBox2.Text = "here, input ahead of the word in english, then press Enter, press Enter, press Enter";
        }

        private void FAN_Click(object sender, EventArgs e)
        {
            if (!FAN.Checked)
                language_fan();
        }

        private void HAN_Click(object sender, EventArgs e)
        {
            if (!HAN.Checked)
                language_han();
        }

        private void EN_Click(object sender, EventArgs e)
        {
            if (!EN.Checked)
                language_en();
        }

        int key_h = 0;
        //int key_k = 0;
        /*
        private void hotkey_Click(object sender, EventArgs e)
        {
            //hotkey.ShortcutKeys = Keys.Control + Keys.Y;
            if (key_k == 0)
            {
                key_k = 1;
                key_h = 1;
                {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失
            }
            else
            {
                key_k = 0;
                key_h = 0;
            }
        }
         */

        int shortkey = 0;
        /*
        private void setkey_Click(object sender, EventArgs e)
        {
            shortkey = 1;

            string skey;
            Encoding ekey;
            if (FAN.Checked)
            {
                skey = @".\set\keybig5.txt";
                ekey = System.Text.Encoding.GetEncoding(950);
            }
            else
            {
                skey = @".\set\key.txt";
                ekey = System.Text.Encoding.GetEncoding("utf-8");
            }
            StreamReader sr = new StreamReader(new FileStream(skey, FileMode.Open), ekey);
            strCxjg = sr.ReadToEnd();
            sr.Close();
            strCxjg = strCxjg + "\r\nĀ    Ī    Ū    Ṅ    Ñ   Ṭ    Ḍ    Ṇ    Ḷ    Ṃ   Ṁ";
            strCxjg = strCxjg + "\r\nA    I    U    V    B    T    D    N    L    J    M";
            webBrowser1.DocumentText = outword(strCxjg);

            cboxInput.Text = "";
            {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失
        }
         */

        /// <summary>
        /// 多种变形词 复合动词尾查词2 此功能耗费时间，效果微小，且未必正确，应废止
        /// </summary>
        /// <param name="sc">要查找的单词</param>
        /// <returns>返回true表示查到</returns>
        public bool cwCc(string sc)
        {
            if (ccmsbz != 3)
                return false;

            int n = 0;  //记录查找到的结果条数
            int p = 0;  //记录sCT数组下标

            string[] sCTou = new string[19];
            string[] sCT = new string[19];
            string[] sCWei = new string[19];
            string[] sCS = new string[116];
            string[] sMccw = new string[116];
            string[] sZiwei = new string[116];
            string[] sZiweiC = new string[116];
            string strLine, szw3;
            int i = 0;

            StreamReader sr1 = new StreamReader(new FileStream(@".\ziwei\cw", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            strLine = sr1.ReadLine();
            while (strLine != null)
            {
                sCS[i] = strLine.Substring(0, 2).TrimEnd();
                sMccw[i] = strLine.Substring(3, 4).TrimEnd();
                sZiwei[i] = strLine.Substring(8, 10).TrimEnd();
                sZiweiC[i] = strLine.Substring(19);
                i++;
                strLine = sr1.ReadLine();
            }
            sr1.Close();

            string strC, strB = "";
            strC = sc;
            string sZW;

            for (int h = 0; h < 116; h++)
            {
                sZW = sZiwei[h];
                if (strC.Length > sZW.Length + sCS[h].Length)
                {
                    if (strC.Substring(0, sCS[h].Length) == sCS[h])
                    {
                        if (strC.Substring(strC.Length - sZW.Length) == sZW)
                        {
                            if (strC.Substring(sCS[h].Length, strC.Length - sZW.Length - sCS[h].Length).Length > 2)
                            {
                                sCTou[p] = sCS[h];
                                sCT[p] = strC.Substring(sCS[h].Length, strC.Length - sZW.Length - sCS[h].Length);
                                sCWei[p] = sZW;
                                p++;
                            }
                        }
                    }
                }
            }

            for (int k = 0; k < p; k++)
            {
                for (int h = 0; h < 116; h++)
                {
                    sZW = sZiwei[h];

                    strB = sCS[h] + sCT[k] + sZiwei[h];

                    szw3 = cc_zw(strB);
                    if (szw3 == "")
                        continue;
                    else
                    {
                        if (sCTou[k] == "")
                            strCxjg = strCxjg + "<br><span style='color:#000000; background:#FFFF66; font-weight:bold'>" + sc + " = ( " + sCT[k] + " + " + sCWei[k] + " )</span>" + "  \r\n";
                        else
                            strCxjg = strCxjg + "<br><span style='color:#000000; background:#FFFF66; font-weight:bold'>" + sc + " = ( " + sCTou[k] + " + " + sCT[k] + " + " + sCWei[k] + " )</span>" + "  \r\n";

                        if (sCS[h] == "")
                            strCxjg = strCxjg + "<br><span style='color:#000000; background:#FFFF66; font-weight:bold'>" + strB + " = ( " + sCT[k] + " + " + sZiwei[h] + " )</span>" + "  ";
                        else
                            strCxjg = strCxjg + "<br><span style='color:#000000; background:#FFFF66; font-weight:bold'>" + strB + " = ( " + sCS[h] + " + " + sCT[k] + " + " + sZiwei[h] + " )</span>" + "  ";

                        strCxjg = strCxjg + "\r\n字尾: " + sMccw[h] + "<>" + sZiwei[h] + "  " + sZiweiC[h] + "\r\n\r\n";
                        strCxjg = strCxjg + szw3;
                        n++;
                    }
                }
            }

            if (n == 0)
                return false;
            else
                return true;
        }

        string einword_zw(string s)
        {
            int cvbz;
            string aword = "", tmp = "";

            tmp = s.Trim();
            char[] ca = "āīūṅñṭḍṇḷŋĀĪŪṄÑṬḌṆḶŊṁṃṀṂ".ToCharArray();
            char[] cb = "aiunntdnlmAIUNNTDNLMmmMM".ToCharArray();
            int h = 0;
            foreach (char c in ca)
            {
                tmp = new Regex(c.ToString(), RegexOptions.None).Replace(tmp, cb[h].ToString());
                h++;
            }
            char[] cText = tmp.ToCharArray();
            foreach (char c in cText)
            {
                cvbz = 0;
                //处理简体中文输入法所输入的 26 个英文字母
                for (int i = 65313; i < 65339; i++)
                {
                    if (Convert.ToUInt16(c) == i)
                    {
                        aword = aword + ((char)(i - 65248)).ToString();
                        cvbz = 1;
                    }
                    if (Convert.ToUInt16(c) == i + 32)
                    {
                        aword = aword + ((char)(i + 32 - 65248)).ToString();
                        cvbz = 1;
                    }
                }
                if (cvbz == 0)
                    aword = aword + c.ToString();
            }

            //排除52个英文字母,'-',' '之外的任何字符
            char[] charEABC = { '\'', '-', ' ', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            string saword = "";
            for (int z = 0; z < aword.Length; z++)
            {
                foreach (char c in charEABC)
                {
                    if (c.ToString() == aword.Substring(z, 1))
                        saword = saword + aword.Substring(z, 1);
                }
            }
            return saword;
        }

        string tahomaabc = "āīūṅñṭḍṇḷṃṁŋ";
        string tahomaABC = "ĀĪŪṄÑṬḌṆḶṂṀŊ";
        string sangayanaabc = "àãåï¤ñóõëü§h";
        string sangayanaABC = "âäæð¥òôöìýMH";
        string vriabc = "±²³ªñμ¹º¼½mh";
        string vriABC = "¾¿Ð©ÑÝÞðýþMH";

        private void kABC(int i)
        {
            string c = "";
            //以下几句注释后变为只输入Tahoma字符
            //if (rbtnTahoma.Checked)
            c = tahomaABC.Substring(i, 1);
            //if (rbtnSangayana.Checked)
            //c = sangayanaABC.Substring(i, 1);
            //if (rbtnVriRomanPali.Checked)
            //c = vriABC.Substring(i, 1);

            int j = cboxInput.SelectionStart;
            //此句删除选中的文本
            cboxInput.Text = cboxInput.Text.Substring(0, j) + cboxInput.Text.Substring(j + cboxInput.SelectedText.Length);

            cboxInput.Text = cboxInput.Text.Insert(j, c);
            cboxInput.Select(j + 1, 0);
            {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失
            //

            if (cboxInput.Text.Trim().Length > 0)
            {
                if (inputError())
                    return;

                ccmsbz = 1; //设定为单独词查词模式
                bFhcSwitch = false;

                idNo = 1;

                _ishtmlListOut = true;

                bwordaheadmatch = true;

                DateTime startD, endD;
                startD = DateTime.Now;

                //iCclsDw = -1;

                if (menuosEnglishPali.Checked)
                    en_pali_cc(cboxInput.Text);
                else
                    palihan_ccFHC(cboxInput.Text);

                listBatchOut();

                if (strCxjg != "")
                    htmlListOut();
                else
                {
                    if (HAN.Checked)
                        webBrowser1.DocumentText = "没查到！请直接按 回车 或 '查词'按钮，利用变形词和复合词分析功能来分析这个pali单词！";
                    if (FAN.Checked)
                        webBrowser1.DocumentText = Strings.StrConv("没查到！请直接按 回车 或 '查词'按钮，利用变形词和复合词分析功能来分析这个pali单词！", VbStrConv.TraditionalChinese, 0x0409);
                    if (EN.Checked)
                        webBrowser1.DocumentText = "No found!please press Enter.";
                }

                _ishtmlListOut = false;

                endD = DateTime.Now;
                System.TimeSpan ts = endD.Subtract(startD);

                toollbltimes.Text = " " + ts.TotalSeconds.ToString() + " s.";

                {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失

                cbT = cboxInput.Text;
                cbtL = cboxInput.Text.Length;
            }
        }

        private void kabc(int i)
        {
            string c = "";
            //以下几句注释后变为只输入Tahoma字符
            //if (rbtnTahoma.Checked)
            c = tahomaabc.Substring(i, 1);
            //if (rbtnSangayana.Checked)
            //c = sangayanaabc.Substring(i, 1);
            //if (rbtnVriRomanPali.Checked)
            //c = vriabc.Substring(i, 1);

            int j = cboxInput.SelectionStart;
            //此句删除选中的文本
            cboxInput.Text = cboxInput.Text.Substring(0, j) + cboxInput.Text.Substring(j + cboxInput.SelectedText.Length);

            cboxInput.Text = cboxInput.Text.Insert(j, c);
            cboxInput.Select(j + 1, 0);
            {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失
            //

            if (cboxInput.Text.Trim().Length > 0)
            {
                if (inputError())
                    return;

                ccmsbz = 1; //设定为单独词查词模式
                bFhcSwitch = false;

                idNo = 1;

                _ishtmlListOut = true;

                bwordaheadmatch = true;

                DateTime startD, endD;
                startD = DateTime.Now;

                //iCclsDw = -1;

                if (menuosEnglishPali.Checked)
                    en_pali_cc(cboxInput.Text);
                else
                    palihan_ccFHC(cboxInput.Text);

                listBatchOut();

                if (strCxjg != "")
                    htmlListOut();
                else
                {
                    if (HAN.Checked)
                        webBrowser1.DocumentText = "没查到！请直接按 回车 或 '查词'按钮，利用变形词和复合词分析功能来分析这个pali单词！";
                    if (FAN.Checked)
                        webBrowser1.DocumentText = Strings.StrConv("没查到！请直接按 回车 或 '查词'按钮，利用变形词和复合词分析功能来分析这个pali单词！", VbStrConv.TraditionalChinese, 0x0409);
                    if (EN.Checked)
                        webBrowser1.DocumentText = "No found!please press Enter.";
                }

                _ishtmlListOut = false;

                endD = DateTime.Now;
                System.TimeSpan ts = endD.Subtract(startD);

                toollbltimes.Text = " " + ts.TotalSeconds.ToString() + " s.";

                {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失

                cbT = cboxInput.Text;
                cbtL = cboxInput.Text.Length;
            }
        }

        //找出不重复的单词
        private void cf()
        {
            string strLine, strLine1, strLine2;

            StreamReader sr = new StreamReader(new FileStream(@".\pali-e\c.txt", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            StreamReader sr1 = new StreamReader(new FileStream(@".\pali-e\emcidian", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            StreamReader sr2 = new StreamReader(new FileStream(@".\pali-e\emindex", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw1 = new StreamWriter(@".\pali-e\n1.txt", false, System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw2 = new StreamWriter(@".\pali-e\n2.txt", false, System.Text.Encoding.GetEncoding("utf-8"));

            strLine = sr.ReadLine();
            strLine1 = sr1.ReadLine();
            strLine2 = sr2.ReadLine();
            int i = 0;
            while (strLine != null)
            {
                if (cc_zw(strLine) == "")
                {
                    sw1.WriteLine(strLine1);
                    sw2.WriteLine(strLine2);
                }
                webBrowser1.DocumentText = "" + i;
                i++;
                strLine = sr.ReadLine();
                strLine1 = sr1.ReadLine();
                strLine2 = sr2.ReadLine();
            }
            sr.Close();
            sr1.Close();
            sr2.Close();
            sw1.Close();
            sw2.Close();
        }

        private void textBox3_DoubleClick(object sender, EventArgs e)
        {
            int i, ks = 0, js = 0, FirstCharIndexOfCurrentLine;
            string s;

            FirstCharIndexOfCurrentLine = textBox3.GetFirstCharIndexOfCurrentLine();
            i = textBox3.SelectionStart - textBox3.GetFirstCharIndexOfCurrentLine();
            s = textBox3.Lines[textBox3.GetLineFromCharIndex(textBox3.SelectionStart)];

            if (s[i] == '-')
                return;

            for (int j = i; j > -1; j--)
            {
                if (s[j] == '-')
                {
                    ks = j + 1;
                    break;
                }
                if (j == 0)
                    ks = j;
            }

            for (int j = i; j < s.Length; j++)
            {
                if (s[j] == '-')
                {
                    js = j - 1;
                    break;
                }
                if (j == s.Length - 1)
                    js = j;
            }

            textBox3.Select(FirstCharIndexOfCurrentLine + ks, js - ks + 1);
            int jh = 0; //减号记数
            if (s.Substring(0, 6) == "000000")
            {
                for (int h = 0; h < textBox3.SelectionStart; h++)
                {
                    if (textBox3.Text.Substring(h, 1) == "-")
                        jh++;
                }
                cboxInput.Select(textBox3.SelectionStart - jh - 6, js - ks + 1);
            }
            else
                cboxInput.Select(Convert.ToByte(textBox3.Text.Substring(FirstCharIndexOfCurrentLine, 3)), Convert.ToByte(textBox3.Text.Substring(FirstCharIndexOfCurrentLine + 3, 3)));

            DateTime startD, endD;
            startD = DateTime.Now;

            string nword = "", pword = "";
            pword = textBox3.SelectedText;
            nword = einwordfhc(textBox3.SelectedText);
            if (nword == "")
            {
                webBrowser1.DocumentText = "您输入的单词字符不是正确的巴利文罗马字母，请重新输入！\r\n可能您font设置有误,请试试选择其它font再查!";
                return;
            }

            webBrowser1.DocumentText = "正在查找单词： " + pword + " ...";

            strCxjg = "";
            if (palihan_cc(nword, pword))
                webBrowser1.DocumentText = outword(strCxjg);
            else
            {
                if (pword.Length < 257)
                {
                    if (pword.Length != nword.Length)
                    {
                        webBrowser1.DocumentText = "您输入的单词字符不是正确的巴利文罗马字母，请重新输入！\r\n可能您font设置有误,请试试选择其它font再查!";
                        return;
                    }
                    else
                    {
                        for (int ic = 0; ic < pword.Length; ic++)
                        {
                            if (pword.Length - ic - 1 < 15)
                            {
                                for (int j = pword.Length - ic - 1; j > 0; j--)
                                {
                                    if (palihan_cc(nword.Substring(ic, j + 1), pword.Substring(ic, j + 1)))
                                    {
                                        textBox3.AppendText(pword.Substring(ic, j + 1) + "\r\n");
                                        if (pword.Substring(ic, j + 1).Length > 2)
                                            ic = ic + pword.Substring(ic, j + 1).Length - 2 - 1;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                for (int j = 14; j > 0; j--)
                                {
                                    if (palihan_cc(nword.Substring(ic, j + 1), pword.Substring(ic, j + 1)))
                                    {
                                        textBox3.AppendText(pword.Substring(ic, j + 1) + "\r\n");
                                        if (pword.Substring(ic, j + 1).Length > 2)
                                            ic = ic + pword.Substring(ic, j + 1).Length - 2 - 1;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                    webBrowser1.DocumentText = "输入的单词长度不应大于256个字母!";
            }

            htmlout();

            endD = DateTime.Now;
            System.TimeSpan ts = endD.Subtract(startD);

            toollbltimes.Text = "times: " + ts.TotalSeconds.ToString() + " s.";
        }

        private void cms3selectAll_Click(object sender, EventArgs e)
        {
            cboxInput.SelectAll();
        }

        private void cms3copy_Click(object sender, EventArgs e)
        {
            fzbz = -1;
            cbdText = outword_t(cboxInput.SelectedText);
            if (cbdText != "") //因为金山词霸在本程序窗口里划词的时候会引发异常，故设此条件判断
                Clipboard.SetText(cbdText, TextDataFormat.UnicodeText);
        }

        private void cms3paste_Click(object sender, EventArgs e)
        {
            int j = cboxInput.SelectionStart;
            //此句删除选中的文本
            cboxInput.Text = cboxInput.Text.Substring(0, j) + cboxInput.Text.Substring(j + cboxInput.SelectedText.Length);
            string scb = inword_t(Clipboard.GetText(TextDataFormat.UnicodeText));
            int igh = scb.Length;
            cboxInput.Text = cboxInput.Text.Insert(j, scb);
            cboxInput.Select(j + igh, 0);
            {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失
        }

        private void cms3cut_Click(object sender, EventArgs e)
        {
            fzbz = -1;
            cbdText = outword_t(cboxInput.SelectedText);
            if (cbdText != "") //因为金山词霸在本程序窗口里划词的时候会引发异常，故设此条件判断
                Clipboard.SetText(cbdText, TextDataFormat.UnicodeText);

            int j = cboxInput.SelectionStart;
            cboxInput.Text = cboxInput.Text.Remove(cboxInput.SelectionStart, cboxInput.SelectionLength);
            cboxInput.Select(j, 0);
            {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失
        }

        private void cboxInput_KeyDown(object sender, KeyEventArgs e)
        {
            //快捷键 全选 复制 粘贴 剪切
            if (e.Modifiers == Keys.Control)
            {
                if (e.KeyCode == Keys.A)
                {
                    cboxInput.SelectAll();
                }

                if (e.KeyCode == Keys.C)
                {
                    fzbz = -1;
                    cbdText = outword_t(cboxInput.SelectedText);
                    if (cbdText != "") //因为金山词霸在本程序窗口里划词的时候会引发异常，故设此条件判断
                        Clipboard.SetText(cbdText, TextDataFormat.UnicodeText);
                }

                if (e.KeyCode == Keys.V)
                {
                    int j = cboxInput.SelectionStart;
                    //此句删除选中的文本
                    cboxInput.Text = cboxInput.Text.Substring(0, j) + cboxInput.Text.Substring(j + cboxInput.SelectedText.Length);
                    string scb = inword_t(Clipboard.GetText(TextDataFormat.UnicodeText));
                    int igh = scb.Length;
                    cboxInput.Text = cboxInput.Text.Insert(j, scb);
                    cboxInput.Select(j + igh, 0);
                    {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失
                }

                if (e.KeyCode == Keys.X)
                {
                    fzbz = -1;
                    cbdText = outword_t(cboxInput.SelectedText);
                    if (cbdText != "") //因为金山词霸在本程序窗口里划词的时候会引发异常，故设此条件判断
                        Clipboard.SetText(cbdText, TextDataFormat.UnicodeText);

                    int j = cboxInput.SelectionStart;
                    cboxInput.Text = cboxInput.Text.Remove(cboxInput.SelectionStart, cboxInput.SelectionLength);
                    cboxInput.Select(j, 0);
                    {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失
                }

                e.SuppressKeyPress = true;
            }

            //快捷键 粘贴
            if (e.Modifiers == Keys.Shift)
            {
                if (e.KeyCode == Keys.Insert)
                {
                    int j = cboxInput.SelectionStart;
                    //此句删除选中的文本，倘若要把‘粘贴’功能改为‘插入’功能，只需注释掉此句即可
                    cboxInput.Text = cboxInput.Text.Substring(0, j) + cboxInput.Text.Substring(j + cboxInput.SelectedText.Length);
                    string scb = inword_t(Clipboard.GetText(TextDataFormat.UnicodeText));
                    int igh = scb.Length;
                    cboxInput.Text = cboxInput.Text.Insert(j, scb);
                    cboxInput.Select(j + igh, 0);
                    {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失

                    e.SuppressKeyPress = true;
                }
            }

            if (e.Modifiers == Keys.Alt)
            {
                if (GetKeyState(Convert.ToInt32(Keys.Capital)) == 0)
                {
                    if (e.KeyCode == Keys.A)
                    {
                        kabc(0);
                    }
                    if (e.KeyCode == Keys.I)
                    {
                        kabc(1);
                    }
                    if (e.KeyCode == Keys.U)
                    {
                        kabc(2);
                    }
                    if (e.KeyCode == Keys.V)
                    {
                        kabc(3);
                    }
                    if (e.KeyCode == Keys.B)
                    {
                        kabc(4);
                    }
                    if (e.KeyCode == Keys.T)
                    {
                        kabc(5);
                    }
                    if (e.KeyCode == Keys.D)
                    {
                        kabc(6);
                    }
                    if (e.KeyCode == Keys.N)
                    {
                        kabc(7);
                    }
                    if (e.KeyCode == Keys.L)
                    {
                        kabc(8);
                    }
                    if (e.KeyCode == Keys.J)
                    {
                        kabc(9);
                    }
                    if (e.KeyCode == Keys.M)
                    {
                        kabc(10);
                    }
                    if (e.KeyCode == Keys.H)
                    {
                        kabc(11);
                    }
                }
                else
                {
                    if (e.KeyCode == Keys.A)
                    {
                        kABC(0);
                    }
                    if (e.KeyCode == Keys.I)
                    {
                        kABC(1);
                    }
                    if (e.KeyCode == Keys.U)
                    {
                        kABC(2);
                    }
                    if (e.KeyCode == Keys.V)
                    {
                        kABC(3);
                    }
                    if (e.KeyCode == Keys.B)
                    {
                        kABC(4);
                    }
                    if (e.KeyCode == Keys.T)
                    {
                        kABC(5);
                    }
                    if (e.KeyCode == Keys.D)
                    {
                        kABC(6);
                    }
                    if (e.KeyCode == Keys.N)
                    {
                        kABC(7);
                    }
                    if (e.KeyCode == Keys.L)
                    {
                        kABC(8);
                    }
                    if (e.KeyCode == Keys.J)
                    {
                        kABC(9);
                    }
                    if (e.KeyCode == Keys.M)
                    {
                        kABC(10);
                    }
                    if (e.KeyCode == Keys.H)
                    {
                        kABC(11);
                    }
                }
                e.Handled = true;
            }

            if (e.Modifiers == (Keys.Alt | Keys.Shift))
            {
                if (GetKeyState(Convert.ToInt32(Keys.Capital)) == 0)
                {
                    if (e.KeyCode == Keys.A)
                    {
                        kABC(0);
                    }
                    if (e.KeyCode == Keys.I)
                    {
                        kABC(1);
                    }
                    if (e.KeyCode == Keys.U)
                    {
                        kABC(2);
                    }
                    if (e.KeyCode == Keys.V)
                    {
                        kABC(3);
                    }
                    if (e.KeyCode == Keys.B)
                    {
                        kABC(4);
                    }
                    if (e.KeyCode == Keys.T)
                    {
                        kABC(5);
                    }
                    if (e.KeyCode == Keys.D)
                    {
                        kABC(6);
                    }
                    if (e.KeyCode == Keys.N)
                    {
                        kABC(7);
                    }
                    if (e.KeyCode == Keys.L)
                    {
                        kABC(8);
                    }
                    if (e.KeyCode == Keys.J)
                    {
                        kABC(9);
                    }
                    if (e.KeyCode == Keys.M)
                    {
                        kABC(10);
                    }
                    if (e.KeyCode == Keys.H)
                    {
                        kABC(11);
                    }
                }
                else
                {
                    if (e.KeyCode == Keys.A)
                    {
                        kabc(0);
                    }
                    if (e.KeyCode == Keys.I)
                    {
                        kabc(1);
                    }
                    if (e.KeyCode == Keys.U)
                    {
                        kabc(2);
                    }
                    if (e.KeyCode == Keys.V)
                    {
                        kabc(3);
                    }
                    if (e.KeyCode == Keys.B)
                    {
                        kabc(4);
                    }
                    if (e.KeyCode == Keys.T)
                    {
                        kabc(5);
                    }
                    if (e.KeyCode == Keys.D)
                    {
                        kabc(6);
                    }
                    if (e.KeyCode == Keys.N)
                    {
                        kabc(7);
                    }
                    if (e.KeyCode == Keys.L)
                    {
                        kabc(8);
                    }
                    if (e.KeyCode == Keys.J)
                    {
                        kabc(9);
                    }
                    if (e.KeyCode == Keys.M)
                    {
                        kabc(10);
                    }
                    if (e.KeyCode == Keys.H)
                    {
                        kabc(11);
                    }
                }
                e.Handled = true;
            }

            //当滚动鼠标滚轮时，也会分别触发 Keys.Up 或 Keys.Down 消息
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up || e.KeyCode == Keys.PageDown || e.KeyCode == Keys.PageUp)
            {
                if (e.KeyCode == Keys.Down)
                    delta = -120;
                else if (e.KeyCode == Keys.Up)
                    delta = 120;
                else if (e.KeyCode == Keys.PageDown)
                    delta = -120 * (lineNum - 1); //之所以乘(lineNum-1)，而非lineNum，原因是可见列表里高亮词条占两行高，实际一页只显示了(lineNum-1)行
                else
                    delta = 120 * (lineNum - 1);

                if (_islistweb)
                {
                    topSpace = webBrowser1.Document.GetElementById(preId).OffsetRectangle.Y - webBrowser1.Document.Body.ScrollTop;

                    //此四组行为正确，但有时词库里词错位导致出错，　又，preid n值是否无错？
                    if (topSpace <= -36 || topSpace >= 18 * lineNum)
                    {
                        webBrowser1.Document.GetElementById(preId).Style = "color:#000000; background-color:#FFFFFF; display:block; width:100%; height:18px;";
                        webBrowser1.Document.GetElementById(preId + "s").Style = "display:none;";

                        if (topSpace <= -36)
                        {
                            if (delta < 0)
                            {
                                preId = (webBrowser1.Document.Body.ScrollTop / 18).ToString();

                                if (Convert.ToInt16(preId) > listmaxNO)
                                    preId = listmaxNO.ToString();

                                webBrowser1.Document.GetElementById(preId).ScrollIntoView(true);

                                webBrowser1.Document.GetElementById(preId).Style = "display:block; width:100%; background-color:#efefff; color:#001CFF;";
                                webBrowser1.Document.GetElementById(preId + "s").Style = "display:block; width:100%; background-color:#efefff; height:18px; cursor:pointer; overflow:hidden;";
                            }
                            else
                            {
                                preId = (webBrowser1.Document.Body.ScrollTop / 18 + lineNum).ToString();

                                if (Convert.ToInt16(preId) > listmaxNO)
                                    preId = listmaxNO.ToString();

                                webBrowser1.Document.GetElementById(preId).Style = "display:block; width:100%; background-color:#efefff; color:#001CFF;";
                                webBrowser1.Document.GetElementById(preId + "s").Style = "display:block; width:100%; background-color:#efefff; height:18px; cursor:pointer; overflow:hidden;";

                                webBrowser1.Document.GetElementById(preId).ScrollIntoView(false);
                            }
                        }
                        else
                        {
                            if (delta < 0)
                            {
                                preId = (webBrowser1.Document.Body.ScrollTop / 18 + 1).ToString();

                                if (Convert.ToInt16(preId) > listmaxNO)
                                    preId = listmaxNO.ToString();

                                webBrowser1.Document.GetElementById(preId).ScrollIntoView(true);

                                webBrowser1.Document.GetElementById(preId).Style = "display:block; width:100%; background-color:#efefff; color:#001CFF;";
                                webBrowser1.Document.GetElementById(preId + "s").Style = "display:block; width:100%; background-color:#efefff; height:18px; cursor:pointer; overflow:hidden;";
                            }
                            else
                            {
                                preId = (webBrowser1.Document.Body.ScrollTop / 18 + lineNum + 1).ToString();

                                if (Convert.ToInt16(preId) > listmaxNO)
                                    preId = listmaxNO.ToString();

                                webBrowser1.Document.GetElementById(preId).Style = "display:block; width:100%; background-color:#efefff; color:#001CFF;";
                                webBrowser1.Document.GetElementById(preId + "s").Style = "display:block; width:100%; background-color:#efefff; height:18px; cursor:pointer; overflow:hidden;";

                                webBrowser1.Document.GetElementById(preId).ScrollIntoView(false);
                            }
                        }

                        //高亮显示部分单词
                        hlText = new Regex(@"\r\n[\s\S]*", RegexOptions.None).Replace(webBrowser1.Document.GetElementById(preId).InnerText, "").Substring(cbtL);
                        //不再转换编码
                        //cboxInput.Text = cbT + hlText;
                        cboxInput.Text = new Regex(@"\r\n[\s\S]*", RegexOptions.None).Replace(webBrowser1.Document.GetElementById(preId).InnerText, "");
                        //cboxInput.Text = cbT + outword_t(hlText);
                        cboxInput.Select(cbtL, hlText.Length);
                    }
                    else
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

                        webBrowser1.Document.GetElementById(preId).Style = "display:block; width:100%; background-color:#efefff; color:#001CFF;";
                        webBrowser1.Document.GetElementById(preId + "s").Style = "display:block; width:100%; background-color:#efefff; height:18px; cursor:pointer; overflow:hidden;";

                        if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up) //按行浏览
                        {
                            //首条对齐
                            if ((preId == (webBrowser1.Document.Body.ScrollTop / 18 + 1).ToString() && delta > 0))
                            {
                                webBrowser1.Document.GetElementById(preId).ScrollIntoView(true);
                            }
                            //如果当前高亮词条是列表可见部分的首条且按的是向上键才滚动
                            if ((preId == (webBrowser1.Document.Body.ScrollTop / 18).ToString() && delta > 0))
                            {
                                webBrowser1.Document.Window.ScrollTo(webBrowser1.Document.Body.ScrollLeft, webBrowser1.Document.Body.ScrollTop - delta * 18 / 120);
                            }
                            //如果当前高亮词条是列表可见部分的末条且按的是向下键才滚动
                            if ((preId == (webBrowser1.Document.Body.ScrollTop / 18 + lineNum).ToString() && delta < 0))
                            {
                                webBrowser1.Document.Window.ScrollTo(webBrowser1.Document.Body.ScrollLeft, webBrowser1.Document.Body.ScrollTop - delta * 18 / 120);
                                webBrowser1.Document.GetElementById(preId).ScrollIntoView(false);
                            }
                        }
                        else //按页浏览，对之不处理顶行对齐
                        {
                            webBrowser1.Document.Window.ScrollTo(webBrowser1.Document.Body.ScrollLeft, webBrowser1.Document.Body.ScrollTop - delta * 18 / 120);
                        }


                        //高亮显示部分单词
                        hlText = new Regex(@"\r\n[\s\S]*", RegexOptions.None).Replace(webBrowser1.Document.GetElementById(s1).InnerText, "").Substring(cbtL);
                        //不再转换编码
                        //cboxInput.Text = cbT + hlText;
                        //输入框控件似乎本身存在问题，当在列表中按向上按钮直到最上一个词，词首大写字母却显示为小写！如果再按一次向上，则显示出正常的大写。如在词尾（测试）加上一个字符，则也显示正确。
                        cboxInput.Text = new Regex(@"\r\n[\s\S]*", RegexOptions.None).Replace(webBrowser1.Document.GetElementById(s1).InnerText, "");
                        //cboxInput.Text = cbT + outword_t(hlText);
                        cboxInput.Select(cbtL, hlText.Length);
                    }
                }
                else
                {
                    webBrowser1.Document.Window.ScrollTo(webBrowser1.Document.Body.ScrollLeft, webBrowser1.Document.Body.ScrollTop - delta * 18 / 120);
                }

                e.Handled = true;
            }
        }

        /// <summary>
        /// 定义htmllist单词列表框中可见的最大行数，当前窗口设计尺寸下的行数初始值：13
        /// </summary>
        int lineNum = 13;

        /// <summary>
        /// 当将滚动浏览htmllist单词列表的时候，当前高亮词条元素顶端与浏览器窗口顶端的距离
        /// </summary>
        int topSpace = 0;

        /// <summary>
        /// 当滚动浏览htmllist单词列表的时候，需要在cboxInput框里高亮显示的文本
        /// </summary>
        string hlText = "";

        private void cboxInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            //转换键入的 VriRomanPali 字体编码字母为 Tahoma 字体编码字母
            if (rbtnVriRomanPali.Checked)
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
            if (rbtnSangayana.Checked)
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

            //此段已暂不起作用，因为key_h值为0
            if (key_h == 1)
            {
                if (rbtnTahoma.Checked)
                    switch (Convert.ToUInt16(e.KeyChar))
                    {
                        case 65:
                            e.KeyChar = 'Ā';
                            break;
                        case 97:
                            e.KeyChar = 'ā';
                            break;
                        case 73:
                            e.KeyChar = 'Ī';
                            break;
                        case 105:
                            e.KeyChar = 'ī';
                            break;
                        case 85:
                            e.KeyChar = 'Ū';
                            break;
                        case 117:
                            e.KeyChar = 'ū';
                            break;
                        case 86:
                            e.KeyChar = 'Ṅ';
                            break;
                        case 118:
                            e.KeyChar = 'ṅ';
                            break;
                        case 66:
                            e.KeyChar = 'Ñ';
                            break;
                        case 98:
                            e.KeyChar = 'ñ';
                            break;
                        case 84:
                            e.KeyChar = 'Ṭ';
                            break;
                        case 116:
                            e.KeyChar = 'ṭ';
                            break;
                        case 68:
                            e.KeyChar = 'Ḍ';
                            break;
                        case 100:
                            e.KeyChar = 'ḍ';
                            break;
                        case 78:
                            e.KeyChar = 'Ṇ';
                            break;
                        case 110:
                            e.KeyChar = 'ṇ';
                            break;
                        case 76:
                            e.KeyChar = 'Ḷ';
                            break;
                        case 108:
                            e.KeyChar = 'ḷ';
                            break;
                        case 74:
                            e.KeyChar = 'Ṃ';
                            break;
                        case 106:
                            e.KeyChar = 'ṃ';
                            break;
                        case 77:
                            e.KeyChar = 'Ṁ';
                            break;
                        case 109:
                            e.KeyChar = 'ṁ';
                            break;
                        default:
                            ;
                            break;
                    }
                if (rbtnSangayana.Checked)
                    switch (Convert.ToUInt16(e.KeyChar))
                    {
                        case 65:
                            e.KeyChar = 'â';
                            break;
                        case 97:
                            e.KeyChar = 'à';
                            break;
                        case 73:
                            e.KeyChar = 'ä';
                            break;
                        case 105:
                            e.KeyChar = 'ã';
                            break;
                        case 85:
                            e.KeyChar = 'æ';
                            break;
                        case 117:
                            e.KeyChar = 'å';
                            break;
                        case 86:
                            e.KeyChar = 'ð';
                            break;
                        case 118:
                            e.KeyChar = 'ï';
                            break;
                        case 66:
                            e.KeyChar = '¥';
                            break;
                        case 98:
                            e.KeyChar = '¤';
                            break;
                        case 84:
                            e.KeyChar = 'ò';
                            break;
                        case 116:
                            e.KeyChar = 'ñ';
                            break;
                        case 68:
                            e.KeyChar = 'ô';
                            break;
                        case 100:
                            e.KeyChar = 'ó';
                            break;
                        case 78:
                            e.KeyChar = 'ö';
                            break;
                        case 110:
                            e.KeyChar = 'õ';
                            break;
                        case 76:
                            e.KeyChar = 'ì';
                            break;
                        case 108:
                            e.KeyChar = 'ë';
                            break;
                        case 74:
                            e.KeyChar = 'ý';
                            break;
                        case 106:
                            e.KeyChar = 'ü';
                            break;
                        case 77:
                            ;
                            break;
                        case 109:
                            e.KeyChar = '§';
                            break;
                        default:
                            ;
                            break;
                    }
                if (rbtnVriRomanPali.Checked)
                    switch (Convert.ToUInt16(e.KeyChar))
                    {
                        case 65:
                            e.KeyChar = '¾';
                            break;
                        case 97:
                            e.KeyChar = '±';
                            break;
                        case 73:
                            e.KeyChar = '¿';
                            break;
                        case 105:
                            e.KeyChar = '²';
                            break;
                        case 85:
                            e.KeyChar = 'Ð';
                            break;
                        case 117:
                            e.KeyChar = '³';
                            break;
                        case 86:
                            e.KeyChar = '©';
                            break;
                        case 118:
                            e.KeyChar = 'ª';
                            break;
                        case 66:
                            e.KeyChar = 'Ñ';
                            break;
                        case 98:
                            e.KeyChar = 'ñ';
                            break;
                        case 84:
                            e.KeyChar = 'Ý';
                            break;
                        case 116:
                            e.KeyChar = 'μ';
                            break;
                        case 68:
                            e.KeyChar = 'Þ';
                            break;
                        case 100:
                            e.KeyChar = '¹';
                            break;
                        case 78:
                            e.KeyChar = 'ð';
                            break;
                        case 110:
                            e.KeyChar = 'º';
                            break;
                        case 76:
                            e.KeyChar = 'ý';
                            break;
                        case 108:
                            e.KeyChar = '¼';
                            break;
                        case 74:
                            e.KeyChar = 'þ';
                            break;
                        case 106:
                            e.KeyChar = '½';
                            break;
                        case 77:
                            ;
                            break;
                        case 109:
                            ;
                            break;
                        default:
                            ;
                            break;
                    }
            }

            if (e.KeyChar == System.Convert.ToChar(13))
            {
                string c_Text = cboxInput.Text.Trim();
                if (c_Text == "")
                {
                    e.Handled = true;
                    return;
                }

                if (c_Text == "anicca1120")
                {
                    frmmuluwindow.Show();
                    frmmuluwindow.WindowState = FormWindowState.Normal;
                    frmmuluwindow.button1.Visible = true;
                    cboxInput.Text = "^_^请看目录窗口！";
                    frmmuluwindow.BringToFront();
                    e.Handled = true;
                    return;
                }

                //如果输入的第一个非空格字符不是巴利罗马字母
                MatchCollection mc = new Regex(c_Text.Substring(0, 1), RegexOptions.None).Matches(pali_letter);
                if (mc.Count == 0)
                {
                    if (ifCNchar(c_Text))
                    {
                        han_pali_waitHtm(c_Text);
                        return;
                    }
                }

                if (inputError())
                    return;

                ccmsbz = 2;
                bFhcSwitch = true;
                bwordaheadmatch = false;

                DateTime startD, endD;
                startD = DateTime.Now;

                _isEnter = true;
                iCclsDw = 0;

                if (menuosEnglishPali.Checked)
                    en_pali_cc(cboxInput.Text);
                else
                    palihan_ccFHC(cboxInput.Text);

                htmlout();

                endD = DateTime.Now;
                System.TimeSpan ts = endD.Subtract(startD);

                toollbltimes.Text = " " + ts.TotalSeconds.ToString() + " s.";

                {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失

                e.Handled = true;
            }
        }

        private void cboxInput_KeyUp(object sender, KeyEventArgs e)
        {
            cbxSS = cboxInput.SelectionStart;
            cbxSL = cboxInput.SelectedText.Length;

            if (shortkey == 1)
            {
                int[] qkey = new int[48];
                for (int n = 0; n < 26; n++)
                {
                    qkey[n] = n + 65;
                }
                for (int n = 26; n < 36; n++)
                {
                    qkey[n] = n + 22;
                }
                for (int n = 36; n < 48; n++)
                {
                    qkey[n] = n + 76;
                }
                foreach (int k in qkey)
                {
                    if (e.KeyValue == k)
                    {
                        //hotkey.ShortcutKeys = Keys.Control | (Keys)e.KeyValue;

                        FileStream F = new FileStream(@".\set\cfdat", FileMode.Open);
                        byte[] b = new byte[1] { 0x00 };
                        b[0] = (byte)e.KeyValue;
                        F.Seek(10, SeekOrigin.Begin);
                        F.Write(b, 0, 1);
                        F.Close();

                        if (FAN.Checked)
                            MessageBox.Show(s[20] + "Ctrl + " + (Keys)e.KeyValue);
                        else
                            MessageBox.Show("巴利音标键盘输入法快捷键已设定为:" + "Ctrl + " + (Keys)e.KeyValue);
                        shortkey = 0;
                    }
                }
                if (shortkey == 1)
                {
                    if (FAN.Checked)
                        cboxInput.Text = s[21];
                    else
                        cboxInput.Text = "请按键: F1--F12; 或: A--Z; 或: 0--9.";
                    cboxInput.SelectAll();
                }
            }
        }

        /// <summary>
        /// 当点击输入下拉框右边的下拉箭头时，会首先执行下拉事件再执行单击事件，用这个标记来避免在下拉时执行单击事件里的方法
        /// </summary>
        public int cbiDc = 0;

        private void cboxInput_DropDown(object sender, EventArgs e)
        {
            //cbiDc = 1;

            //显示查词历史记录
            cboxInput.Items.Clear();
            for (int i = 0; i < 50; i++)
            {
                if (arrStrCcls[i] == "")
                    break;
                cboxInput.Items.Add(arrStrCcls[i]);
            }
            //
        }

        Hits hits;
        IndexSearcher searcher;

        //1.81版始使用此函数来查找中文，此函数采用Lucene索引查找，极大的提高了中文查找速度
        public string searchCNWord(string strWord)
        {
            Analyzer analyzer = new StandardAnalyzer();
            //IndexSearcher searcher = new IndexSearcher("index");
            //searcher = new IndexSearcher("index");
            MultiFieldQueryParser parser = new MultiFieldQueryParser(new string[] { "content" }, analyzer);
            Query query = parser.Parse(new Regex(@"\s+", RegexOptions.IgnoreCase).Replace(strWord, " && "));
            //Hits hits = searcher.Search(query);

            if (menuosZwfcCx.Checked)
                hits = searcher.Search(query, new Sort("content"));
            else
                hits = searcher.Search(query);

            string s = "";

            if (hits.Length() <= 200)
            {
                for (int i = 0; i < hits.Length(); i++)
                {
                    Document doc = hits.Doc(i);
                    s = s + "<b style='font-size:20px; color:#006080;'>词：</b>" + doc.Get("content").Substring(2) + "<br><br>";
                }
            }
            else
            {
                for (int i = 0; i < 200; i++)
                {
                    Document doc = hits.Doc(i);
                    s = s + "<b style='font-size:20px; color:#006080;'>词：</b>" + doc.Get("content").Substring(2) + "<br><br>";
                }

                s = s + "共查到了 <b>" + hits.Length().ToString() + "</b> 个词，但为了不至于因有时词数太多而导致输出速度变得过慢，在此，只输出了前 <b>200</b> 个词，您可以多输入几个字符再查找，以减少符合的词数。";
            }

            return s;
        }

        /// <summary>
        /// 判断字符是否是中文字符，如果是，则返回true
        /// </summary>
        public bool ifCNchar(string c_Text)
        {
            //以下判断输入的第一个字符是否汉字，如果是，则进行中文查词
            int lcp = 0;
            if (bjLCID == 1028)
                lcp = 950;
            else
                lcp = 936;

            System.Text.Encoding chs = System.Text.Encoding.GetEncoding(lcp); //用简体中文编码把字符串解码为字节流(从ansi格式记事本文本里保存复制出來的正是简体码字符串)
            byte[] bytes = chs.GetBytes(c_Text);
            MemoryStream ms = new MemoryStream(bytes);

            int cp = 0;
            if (bjLCID == 1028)
                cp = 950;
            else
                cp = 936;

            StreamReader sr = new StreamReader(ms, System.Text.Encoding.GetEncoding(cp)); //用繁体big5编码把流转换为字符串
            c_Text = sr.ReadToEnd();
            sr.Close();

            //c_Text = Strings.StrConv(c_Text, VbStrConv.TraditionalChinese, 0x0404);//Locale ID Chinese - Taiwan把此格式的字符串转换为utf-8编码的繁体中文
            c_Text = Strings.StrConv(c_Text, VbStrConv.TraditionalChinese, 0x0409);//Locale ID English - United States 效果同上
            //c_Text = Strings.StrConv(c_Text, VbStrConv.SimplifiedChinese, 0x0804);//Locale ID Chinese - China 再转为简体字
            c_Text = Strings.StrConv(c_Text, VbStrConv.SimplifiedChinese, 0x0409);//Locale ID English - United States 效果同上

            string cz = c_Text.Substring(0, 1);
            int cm = Strings.AscW(cz);
            if (cm >= 19968 && cm <= 40869)
                return true;
            else
                return false;
        }

        private void btnLookup_Click(object sender, EventArgs e)
        {
            LookupWord();
        }

        /// <summary>
        /// 原 btnLookup_Click 里的代码
        /// </summary>
        public void LookupWord()
        {
            string c_Text = cboxInput.Text.Trim();

            if (c_Text == "")
                return;

            if (c_Text == "anicca1120")
            {
                frmmuluwindow.Show();
                frmmuluwindow.WindowState = FormWindowState.Normal;
                frmmuluwindow.button1.Visible = true;
                cboxInput.Text = "^_^请看目录窗口！";
                frmmuluwindow.BringToFront();
                return;
            }

            MatchCollection mc = new Regex(c_Text.Substring(0, 1), RegexOptions.None).Matches(pali_letter);
            if (mc.Count == 0)//如果输入的第一个非空格字符不是巴利罗马字母及英文字母
            {
                if (ifCNchar(c_Text))
                {
                    han_pali_waitHtm(c_Text);
                    return;
                }
            }

            if (inputError())
                return;

            ccmsbz = 2;
            bFhcSwitch = true;
            bwordaheadmatch = false;

            DateTime startD, endD;
            startD = DateTime.Now;

            _isEnter = true;
            iCclsDw = 0;

            if (menuosEnglishPali.Checked)
                en_pali_cc(c_Text);
            else
                palihan_ccFHC(c_Text);

            htmlout();

            endD = DateTime.Now;
            System.TimeSpan ts = endD.Subtract(startD);

            toollbltimes.Text = " " + ts.TotalSeconds.ToString() + " s.";

            //{}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失

            //激活词典窗口
            if (this.Visible && !(this.WindowState == FormWindowState.Minimized))
                //this.Activate();
                ;
            else
            {
                this.Visible = true;

                this.WindowState = FormWindowState.Normal;
                //SendMessage(this.Handle, 0x112, (IntPtr)0xf120, (IntPtr)0); //恢复窗口

                if (_ishtmonmouseup)
                {
                    ((frmpali)(frmpali.FromHandle((IntPtr)(currfrmpaliWindowHandle)))).Activate();
                    _ishtmonmouseup = false;
                }
            }
        }

        /// <summary>
        /// 三种编码的巴利罗马化音标字母大小写、26个英文字母大小写、-'号，以utf-8编码存储的字符串，无空格符，但多出巴利文里没有的 fqwxz 这5个英文字母之大小写
        /// </summary>
        public static string pali_letter = "āīūṅñṭḍṇḷṃṁŋĀĪŪṄÑṬḌṆḶṂṀŊàãåï¤ñóõëü§âäæð¥òôöìý±²³ªñμµ¹º¼½¾¿Ð©ÑÝÞðýþ-'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        string tahoma = "āīūṅñṭḍṇḷṃṁŋĀĪŪṄÑṬḌṆḶṂṀŊ";
        string sangayana = "àãåï¤ñóõëü§âäæð¥òôöìý";
        string vri = "±²³ªñμµ¹º¼½¾¿Ð©ÑÝÞðýþ";
        string eng = "- 'abcdeghijklmnoprstuvyABCDEGHIJKLMNOPRSTUVY"; //没有的 fqwxz 这5个英文字母之大小写

        /// <summary>
        /// 返回true，则表示输入字符错误
        /// </summary>
        private bool inputError()
        {
            string abc = tahoma + eng + "fqwxzFQWXZ";
            string fontName = "Tahoma(unicode)";

            if (rbtnSangayana.Checked && !_isPaliWeb)
            {
                //abc = sangayana + eng + "fqwxzFQWXZ";
                fontName = "Sangayana";
            }

            if (rbtnVriRomanPali.Checked && !_isPaliWeb)
            {
                //abc = vri + eng + "fqwxzFQWXZ";
                fontName = "VriRomanPali";
            }

            char[] ca = cboxInput.Text.Trim().ToCharArray();
            foreach (char c in ca)
            {
                MatchCollection mc = new Regex(c.ToString(), RegexOptions.None).Matches(abc);
                if (mc.Count == 0)
                {
                    webBrowser1.DocumentText = "当使用seek按钮查找时，勿理会以下文字：<br />When using the seek button to find, ignore the following text:<br /><br /><font face='Tahoma'><font color='#ff0000'>提示：输入错误！</font>  您输入的单词中有一些字符不是正确的巴利文<font color='#0000ff'><b>" + fontName + "</b>字体（当前设置）编码字母</font>或与之同一系列的罗马化字母，请重新输入！<br>可能是您<font color='#ff0000'>‘输入输出字体编码’</font>设置有误,请试试选择设置为其它字体编码系列再查!<br>请注意：程序只接受<font color='#0000ff'>如下pali文罗马化字符、英文字符</font>和<font color='#0000ff'>所有汉字</font>的输入：<br>āīūṅñṭḍṇḷṃṁŋĀĪŪṄÑṬḌṆḶṂṀŊ<br>- '（即英文字符中的-减号 空格符与'单引号）<br>abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ</font>";
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 存放htmlout函数输出查词结果页里的js脚本代码
        /// </summary>
        public static string strHtmloutJS = "<script type='text/javascript'>" +
                "var sltxtlen=0;" +
                "var sltxt='';" +

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

                "</script>" +
                "<a id='sltxtlen1207'></a>" +
                "<a id='sltxt1207'></a>";


        //字体全比原来增大4px
        private void htmlout()
        {
            string strOutText = "";

            strCxjg = new Regex(@"<> (?<w>" + ".{" + strPaliWord.Length.ToString() + "}" + @")(?<w1>[^,:]*?)(?<w2>[,:][^。\$]*?)[。\$](?<w3>.*?)[$\r]", RegexOptions.IgnoreCase).Replace(strCxjg, "<b style='font-size:20px;'>${w}</b>${w1}<br>${w2}。<br><p>${w3}</p>");

            if (_isFhc)
            {
                _isFhc = false;
                strOutText = "<font color='#ff0000'>提示：</font>  <font color='#0000ff'>这个单词可能是一个<u>复合词</u>，请看下面的<u>‘复合词分析结果’文本框</u></font>，这是一个自动分析拆解的结果，以供参考，在‘复合词分析结果’文本框中，用鼠标<font color='#ff0000'>双击</font>拆分后的词，在这里会自动显示其解释！复合词分析结果仅供参考，可能有一些词分析拆解得不正确，可能有另外一些词目前尚不存在于在本程序的词库中。<br>在‘复合词分析结果’文本框中的数字是程序所使用的标志，不用管它。";
            }
            else
                strOutText = strCxjg;

            if (_isInputError)
            {
                _isInputError = false;
                strOutText = "您输入的单词字符不是正确的巴利文罗马字母，请重新输入！<br>可能您font设置有误,请试试选择其它font再查!";
            }

            webBrowser1.DocumentText =
                "<html>" +
                "<head>" +
                "<meta http-equiv='Content-Type' content='text/html; charset=utf-8'>" +

                "<style type='text/css'>" +

                //"body {font-family:Zawgyi-One,Tahoma; font-size:16px; line-height:22px; padding:0px 0px 0px 10px; margin:0px 0px 0px 0px;}" +
                "body {font-family:Tahoma; font-size:16px; line-height:22px; padding:0px 0px 0px 10px; margin:0px 0px 0px 0px;}" +
                "p {color:#000000; padding: 0px 0px 0px 25px; margin: 0px 0px 0px 0px;}" +
                //上面颜色原为#666666
                //下面这句原无
                "b {color:#008080;}" +
                "i {font-weight:bold;}" +
                "sup {font-weight:bold;}" +
                "em {font-weight:bold; color:#0000FF;}" +

                "</style>" +

                "</head>" +

                "<body>" +

                strHtmloutJS +

                //outword(strCxjg) +
                outword(strOutText) +

                "<br><br><br><br><br><br><br><br><br><br><br><br>" +
                "</body>" +

                "</html>";
        }

        /// <summary>
        /// htmlist列表里分批输出时每批词条里第一个词条的id编号，如1-10 11-20 21-30，其中1，11，21就是每一批次idNo的值
        /// </summary>
        public static int idNo = 1;

        private void htmlListOut()
        {
            MatchCollection mc = new Regex(@"<> (?<w>" + ".{" + strPaliWord.Length.ToString() + "}" + @")(?<w1>[^,:]*?)(?<w2>[,:][^。\$]*?)[。\$](?<w3>.*?)[$\r]", RegexOptions.IgnoreCase).Matches(strCxjg);

            strCxjg = "";
            int n = idNo;
            string sn = "";

            if (mc.Count > 0)
            {
                foreach (Match ma in mc)
                {

                    sn = n.ToString();
                    strCxjg = strCxjg + "<a id='" + sn + "' class='tooltips' " + "onclick=\"window.external.oClick('" + ma.Groups["w"].Value + ma.Groups["w1"].Value + "')\"" + "onmouseover=\"window.external.omOver('" + sn + "')\"" + "onmouseleave=\"window.external.omLeave('" + sn + "')\">" + "<b>" + ma.Groups["w"].Value + "</b>" + ma.Groups["w1"].Value + "<br><span id='" + sn + "s'>" + ma.Groups["w2"].Value + "。</span></a>";
                    n++;

                }
            }

            listmaxNO = n - 1;
            idNo = n;

            webBrowser1.DocumentText =
                "<html>" +
                "<head>" +
                "<style type='text/css'>" +
                //"body {font-family:Zawgyi-One,Tahoma; font-size:14px; padding:0px 0px 0px 0px; margin:0px 0px 0px 0px;}" +
                "body {font-family:Tahoma; font-size:14px; padding:0px 0px 0px 0px; margin:0px 0px 0px 0px;}" +
                //以上字体原为12px
                ".tooltips {position:relative; display:block; padding:0px 0px 0px 10px; margin:0px 0px 0px 0px; line-height:18px; width:100%; text-decoration:none; color:#000000; cursor:pointer;}" +
                //            ".tooltips:hover{color:#001CFF; background:#efefff;}" +
                ".tooltips span{display:none;}" +
                //            ".tooltips:hover span{display:block; position:static; width:100%; height:18px; overflow:hidden; cursor:pointer; background-color:#efefff; color:#001CFF;}" +
                "</style>" +
                "</head>" +
                "<body onload = 'window.external.htmLoad()' onunload = 'window.external.htmunLoad()' onscroll = 'window.external.htmScroll()'>" +
                //"<a id='1' class='tooltips' href='#tooltips'  " + "onclick=\"window.external.oClick('1')\"" + "onmouseover=\"window.external.omOver('1')\"" + "onmouseleave=\"window.external.omLeave('1')\">" + "  <b>arahant 1</b><br><span id='1s'>:阿拉漢，巴利語的音譯譯音譯音譯音譯音譯音譯音譯音譯音譯譯音譯音譯音譯音譯音譯音譯音譯音譯譯音譯音譯音譯音譯音譯音譯音譯音譯譯音譯音譯音譯音譯音譯音譯音譯音譯譯音譯音譯音譯音譯音譯音譯音譯音譯譯音譯音譯音譯音譯音譯音譯音譯音譯譯音譯音譯音譯音譯音譯音譯音譯音譯譯音譯音譯音譯音譯音譯音譯音譯音譯譯音譯音譯音譯音譯音譯音譯音譯音譯。1</span></a>" +
                //"<a id='2' class='tooltips' href='#tooltips'  " + "onclick=\"window.external.oClick('2')\"" + "onmouseover=\"window.external.omOver('2')\"" + "onmouseleave=\"window.external.omLeave('2')\">" + "  <b>arahant 2</b><br><span id='2s'>:阿拉漢，巴利語的音譯。2</span></a>" +
                outword(strCxjg) +
                "</body>" +
                "</html>";
        }

        /// <summary>
        /// 值为true表明当前是单词列表页，可以在列表里滚动
        /// </summary>
        bool _islistweb = false;

        string preId = "1";

        public void htmLoad()
        {
            _islistweb = true;
            preId = "1";
            webBrowser1.Document.GetElementById(preId).Style = "color:#001CFF;";
            webBrowser1.Document.GetElementById(preId + "s").Style =
                "display:block; position:static; width:100%; height:18px; overflow:hidden; cursor:pointer; background-color:#ffffff; color:#001CFF;";
        }

        public void htmunLoad()
        {
            _islistweb = false;
        }

        public void htmScroll()
        {
            if (!_isOutputAll)
            {
                listBatchOut();

                MatchCollection mc = new Regex(@"<> (?<w>" + ".{" + strPaliWord.Length.ToString() + "}" + @")(?<w1>[^,:]*?)(?<w2>[,:][^。\$]*?)[。\$](?<w3>.*?)[$\r]", RegexOptions.IgnoreCase).Matches(strCxjg);

                strCxjg = "";
                int n = idNo;
                string sn = "";
                foreach (Match ma in mc)
                {
                    sn = n.ToString();
                    strCxjg = strCxjg + "<a id='" + sn + "' class='tooltips' " + "onclick=\"window.external.oClick('" + ma.Groups["w"].Value + ma.Groups["w1"].Value + "')\"" + "onmouseover=\"window.external.omOver('" + sn + "')\"" + "onmouseleave=\"window.external.omLeave('" + sn + "')\">" + "<b>" + ma.Groups["w"].Value + "</b>" + ma.Groups["w1"].Value + "<br><span id='" + sn + "s'>" + ma.Groups["w2"].Value + "。</span></a>";
                    n++;
                }
                listmaxNO = n - 1;
                idNo = n;


                if (webBrowser1.Document != null)
                {
                    HtmlElement htmE = webBrowser1.Document.CreateElement("div");
                    htmE.InnerHtml = outword(strCxjg);
                    webBrowser1.Document.Body.AppendChild(htmE);
                }
            }
            {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失
        }

        /// <summary>
        /// 用于查找htmllist列表里的单词，在点选htmllist列表里的单词时被浏览器调用
        /// </summary>
        /// <param name="listword">所点选的单词</param>
        public void oClick(string listword)
        {
            ccmsbz = 1;
            bFhcSwitch = false;
            bwordaheadmatch = false;

            DateTime startD, endD;
            startD = DateTime.Now;

            iCclsDw = 0;

            if (menuosEnglishPali.Checked)
                en_pali_cc(listword);
            else
                palihan_ccFHC(listword);
            //palihan_ccFHC(outword_t(listword));

            htmlout();

            endD = DateTime.Now;
            System.TimeSpan ts = endD.Subtract(startD);

            toollbltimes.Text = " " + ts.TotalSeconds.ToString() + " s.";

            //以下两个方法都会引起输入框里光标消失，注释掉就好了，现在在输入框里直接回车焦点不变，如果用鼠标点击下方单词列表中的单词，则失去焦点
            //cboxInput.Focus();
            //{}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失
        }

        private void cboxInput_MouseClick(object sender, MouseEventArgs e)
        {
            /*
            if (cbiDc==1)
            {
                cbiDc = 0;
                return;
            }

            //cboxInput.Select()经常会造成输入框中光标消失，用如下代码来处理这个问题
            //先离开
            cboxABC.Select();

            //再回来 即可
            cboxInput.Focus();
            //cboxInput.Select(cboxInput.Text.Length, 0);
            cboxInput.SelectAll();
            */
        }

        public void omOver(String id)
        {
            webBrowser1.Document.GetElementById(preId).Style = "color:#000000; background-color:#FFFFFF; display:block; width:100%; height:18px;";
            webBrowser1.Document.GetElementById(preId + "s").Style = "display:none;";

            webBrowser1.Document.GetElementById(id).Style = "color:#001CFF; background-color:#efefff;";
            webBrowser1.Document.GetElementById(id + "s").Style =
                "display:block; position:static; width:100%; height:18px; overflow:hidden; cursor:pointer; background-color:#efefff; color:#001CFF;";

            preId = id;
        }

        public void omLeave(String id)
        {
            webBrowser1.Document.GetElementById(preId).Style = "color:#000000; background-color:#FFFFFF; display:block; width:100%; height:18px;";
            webBrowser1.Document.GetElementById(preId + "s").Style = "display:none;";

            webBrowser1.Document.GetElementById(id).Style = "color:#001CFF; background-color:#ffffff;";
            webBrowser1.Document.GetElementById(id + "s").Style =
                "display:block; position:static; width:100%; height:18px; overflow:hidden; cursor:pointer; background-color:#ffffff; color:#001CFF;";

            preId = id;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (iCclsDw < cboxInput.Items.Count - 1)
            {
                ccmsbz = 2;
                bFhcSwitch = true;
                bwordaheadmatch = false;

                DateTime startD, endD;
                startD = DateTime.Now;

                iCclsDw++;
                cboxInput.Text = arrStrCcls[iCclsDw];
                cboxInput.Refresh();

                palihan_ccFHC(cboxInput.Text);

                htmlout();

                endD = DateTime.Now;
                System.TimeSpan ts = endD.Subtract(startD);

                toollbltimes.Text = " " + ts.TotalSeconds.ToString() + " s.";

                {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失
            }
        }

        private void btnForward_Click(object sender, EventArgs e)
        {
            if (iCclsDw > 0)
            {
                ccmsbz = 2;
                bFhcSwitch = true;
                bwordaheadmatch = false;

                DateTime startD, endD;
                startD = DateTime.Now;

                iCclsDw--;
                cboxInput.Text = arrStrCcls[iCclsDw];
                cboxInput.Refresh();

                palihan_ccFHC(cboxInput.Text);

                htmlout();

                endD = DateTime.Now;
                System.TimeSpan ts = endD.Subtract(startD);

                toollbltimes.Text = " " + ts.TotalSeconds.ToString() + " s.";

                {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失
            }
        }

        private void cboxInput_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ccmsbz = 2;
            bFhcSwitch = true;
            bwordaheadmatch = false;

            DateTime startD, endD;
            startD = DateTime.Now;

            iCclsDw = cboxInput.SelectedIndex;
            cboxInput.Text = arrStrCcls[iCclsDw];
            cboxInput.Refresh();

            palihan_ccFHC(cboxInput.Text);

            htmlout();

            endD = DateTime.Now;
            System.TimeSpan ts = endD.Subtract(startD);

            toollbltimes.Text = " " + ts.TotalSeconds.ToString() + " s.";

            {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失
        }

        /// <summary>
        /// 值为true表明是被htmlListOut调用，在这种情况下，查到的单词将按词库里的顺序列出，而不是按单词出处词典的种类列出
        /// </summary>
        bool _ishtmlListOut = false;

        /// <summary>
        /// 记录下每次cboxInput框里输入单词文本变化后的长度，以在滚动浏览htmllist列表时使用
        /// </summary>
        int cbtL = 0;

        /// <summary>
        /// 记录下每次cboxInput框里输入的单词文本，以在滚动浏览htmllist列表时使用
        /// </summary>
        string cbT = "";

        private void cboxInput_TextUpdate(object sender, EventArgs e)
        {
            //将此变量置空，以避免listout中或有少输入第一个词
            sLTmp = "";
            if (cboxInput.Text.Trim().Length > 0)
            {
                if (ifCNchar(cboxInput.Text.Trim()))
                    return;

                if (inputError())
                    return;

                ccmsbz = 1; //设定为单独词查词模式
                bFhcSwitch = false;

                idNo = 1;

                _ishtmlListOut = true;

                bwordaheadmatch = true;

                DateTime startD, endD;
                startD = DateTime.Now;

                //iCclsDw = -1;

                if (menuosEnglishPali.Checked)
                    en_pali_cc(cboxInput.Text);
                else
                    palihan_ccFHC(cboxInput.Text);

                listBatchOut();

                if (strCxjg != "")
                    htmlListOut();
                else
                {
                    if (HAN.Checked)
                        webBrowser1.DocumentText = "没查到！请直接按 回车 或 '查词'按钮，利用变形词和复合词分析功能来分析这个pali单词！";
                    if (FAN.Checked)
                        webBrowser1.DocumentText = Strings.StrConv("没查到！请直接按 回车 或 '查词'按钮，利用变形词和复合词分析功能来分析这个pali单词！", VbStrConv.TraditionalChinese, 0x0409);
                    if (EN.Checked)
                        webBrowser1.DocumentText = "No found! please press Enter or click 'look up' button.";
                }

                _ishtmlListOut = false;

                endD = DateTime.Now;
                System.TimeSpan ts = endD.Subtract(startD);

                toollbltimes.Text = " " + ts.TotalSeconds.ToString() + " s.";

                {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失

                cbT = cboxInput.Text;
                cbtL = cboxInput.Text.Length;
            }
        }

        private void menuhFofa_Click(object sender, EventArgs e)
        {
            StreamReader sr = new StreamReader(new FileStream(@".\book\What The Buddha Taught.htm", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            webBrowser1.DocumentText = outword(sr.ReadToEnd());
            sr.Close();
        }

        private void menuhJwzl_Click(object sender, EventArgs e)
        {
            StreamReader sr = new StreamReader(new FileStream(@".\book\jwzl.htm", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            webBrowser1.DocumentText = outword(sr.ReadToEnd());
            sr.Close();
        }

        private void menuhReadme_Click(object sender, EventArgs e)
        {
            readme();

            {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失
        }

        private void menuhHowto_Click(object sender, EventArgs e)
        {
            StreamReader sr = new StreamReader(new FileStream(@".\set\htinput.htm", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            strCxjg = sr.ReadToEnd();
            sr.Close();

            webBrowser1.DocumentText = outword(strCxjg);

            {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失
        }

        private void menuosSetWindowInFront_Click(object sender, EventArgs e)
        {
            if (!menuosSetWindowInFront.Checked)
            {
                menuosSetWindowInFront.Checked = true;
                this.TopMost = true;
            }
            else
            {
                menuosSetWindowInFront.Checked = false;
                this.TopMost = false;
            }

            {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                for (int v = 0; v < iPaliDictNum; v++)
                {
                    if ("nod" + arStrPaliDictInfo[v].Substring(1, 1) == e.Node.Name)
                    {
                        webBrowser1.Document.GetElementById(arStrPaliDictInfo[v].Substring(1, 1)).ScrollIntoView(true);
                        break;
                    }
                }
                for (int v = 0; v < iEnglishDictNum; v++)
                {
                    if ("nod" + arStrEnglishDictInfo[v].Substring(1, 1) == e.Node.Name)
                    {
                        webBrowser1.Document.GetElementById(arStrEnglishDictInfo[v].Substring(1, 1)).ScrollIntoView(true);
                        break;
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// 取低位
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static int LOWORD(int i)
        {
            return i & 0xFFFF;
        }

        /// <summary>
        /// 取高位
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static int HIWORD(int i)
        {
            return i >> 16;
        }

        /// <summary>
        /// 鼠标滚轮偏移量
        /// </summary>
        int delta;

        /// <summary>
        /// 查到的单词结果列表里的最末词条编号
        /// </summary>
        int listmaxNO;

        int keyValue = 0;

        int WM_KEYUP = 38, WM_KEYDOWN = 40, WM_KEYPAGEUP = 33, WM_KEYPAGEDOWN = 34, WM_KEYHOME = 36, WM_KEYEND = 35;

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
                    //this.Hide();
                    //return true;
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
                    {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失
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

        private void cboxInput_Leave(object sender, EventArgs e)
        {
            if (_islistweb)
                {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失
        }

        //WebBrowserShortcutsEnabled = false;
        //bool _isFirstKeyDown = true; 避免只按一次键却发生两次事件的bug，然而在按Ctrl+ Alt+字母组合键时这个bug不会出现
        private void webBrowser1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.Modifiers == Keys.Control)
            {
                if (e.KeyCode == Keys.A)
                {
                    webBrowser1.Document.ExecCommand("SelectAll", true, null);
                }
            }

            if (e.Modifiers == Keys.Control)
            {
                if (e.KeyCode == Keys.C)
                {
                    fzbz = 1;
                    webBrowser1.Document.ExecCommand("Copy", true, null);
                    cbdText = outword_t(Clipboard.GetText(TextDataFormat.UnicodeText));
                    if (cbdText != "") //因为金山词霸在本程序窗口里划词的时候会引发异常，故设此条件判断
                        Clipboard.SetText(cbdText, TextDataFormat.UnicodeText);
                }
            }

            //_isFirstKeyDown = !_isFirstKeyDown;
        }

        /// <summary>
        /// 经转换后往剪贴板粘帖的文本
        /// </summary>
        public static string cbdText = "";

        //IsWebBrowserContextMenuEnabled = false;
        //ContextMenuStrip = contextMenuStrip1;
        private void copy_Click(object sender, EventArgs e)
        {
            /*
            fzbz = 1;
            //下面的命令会激发三次自动复制取词函数DisplayClipboardData
            webBrowser1.Document.ExecCommand("Copy", true, null);
            cbdText = outword_t(Clipboard.GetText(TextDataFormat.UnicodeText));
            Clipboard.SetText(cbdText, TextDataFormat.UnicodeText);
            */

            webBrowser1.Document.InvokeScript("getsltxt");
            cbdText = outword_t(webBrowser1.Document.GetElementById("sltxt1207").Name.Trim());
            fzbz = -1;
            Clipboard.SetText(cbdText, TextDataFormat.UnicodeText);
        }

        private void lookupsctword_Click(object sender, EventArgs e)
        {
            webBrowser1.Document.InvokeScript("getsltxtlen");
            string sssf = webBrowser1.Document.GetElementById("sltxtlen1207").Name;
            if (Convert.ToInt32(sssf) < 280)
            {
                webBrowser1.Document.InvokeScript("getsltxt");
                cboxInput.Text = webBrowser1.Document.GetElementById("sltxt1207").Name.Trim();
                {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失

                LookupWord();
            }
        }

        private void selectAll_Click(object sender, EventArgs e)
        {
            webBrowser1.Document.ExecCommand("SelectAll", true, null);
        }

        private void menuhSly_Click(object sender, EventArgs e)
        {
            StreamReader sr = new StreamReader(new FileStream(@".\set\sly.htm", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            strCxjg = sr.ReadToEnd();
            sr.Close();

            webBrowser1.DocumentText = outword(strCxjg);

            {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失
        }

        /// <summary>
        /// 记录下窗体最小化前的状态 0 表示正常 1 表示最大化
        /// </summary>
        int frmState = 0;

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            //若为最小化则直接返回，不进行处理
            if (this.WindowState == FormWindowState.Minimized)
                return;

            //if (this.WindowState == FormWindowState.Normal)
            //    frmState = 0;
            //if (this.WindowState == FormWindowState.Maximized)
            //    frmState = 1;

            //当窗体尺寸改变时，取得列表框能够显示的可见行数
            //lineNum = 13;
            if (panelfhc.Visible)
                lineNum = (this.Height - 306) / 18;
            else
                lineNum = (this.Height - 306 + 126) / 18;

            //当窗体尺寸改变时，保持webBrowser1的高度为行高18px的整倍数，以免在滚动列表时其行为不正确
            webBrowser1.Height = lineNum * 18;
            webBrowser1.Width = panel2.Width - treeView1.Width - 12;
            treeView1.Height = webBrowser1.Height;

            //textBox3.Top = webBrowser1.Top + webBrowser1.Height + 8;
            //textBox3.Width = this.Width - 37;
            //treeView1.Height = webBrowser1.Height;

            //当窗体尺寸改变时，改变每次输出结果条数
            listOutNum = lineNum * 2;

            //当窗体尺寸改变时，增加当前显示的词条数量，以免在窗体变大后列表不能把结果词条显示全
            if (_islistweb)
                htmScroll();
        }

        /// <summary>
        /// 记录下浏览器中网页的显示坐标，以便在从窗体隐藏状态恢复后定位网页的位置
        /// </summary>
        int webbodyTop = 0;

        private void menuosBlurinputmode_CheckedChanged(object sender, EventArgs e)
        {
            {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失
        }

        private void menuosEnglishPali_CheckedChanged(object sender, EventArgs e)
        {
            if (menuosEnglishPali.Checked)
            {
                //for (int i = 0; i < Program.en_NUM; i++)
                //{
                //    Program.NUM = Program.en_NUM;
                //    Program.sL[i] = Program.en_sL[i];
                //    Program.strL[i] = Program.en_strL[i];
                //}
                Program.NUM = Program.en_NUM;
                Program.sL = Program.en_sL;
                Program.strL = Program.en_strL;
                if (HAN.Checked)
                    toollbl.Text = "当前词典设置：英语-->巴利文";
                if (FAN.Checked)
                    toollbl.Text = outword("当前词典设置：英语-->巴利文");
                if (EN.Checked)
                    toollbl.Text = "now dictionary set: English-->Pali";
            }
            else
            {
                //for (int i = 0; i < Program.pl_NUM; i++)
                //{
                //    Program.NUM = Program.pl_NUM;
                //    Program.sL[i] = Program.pl_sL[i];
                //    Program.strL[i] = Program.pl_strL[i];
                //}
                Program.NUM = Program.pl_NUM;
                Program.sL = Program.pl_sL;
                Program.strL = Program.pl_strL;
                if (HAN.Checked)
                    toollbl.Text = "当前词典设置：巴利文-->汉语、英语；汉语-->巴利文";
                if (FAN.Checked)
                    toollbl.Text = outword("当前词典设置：巴利文-->汉语、英语；汉语-->巴利文");
                if (EN.Checked)
                    toollbl.Text = "now dictionary set: Pali-->English ,Chinese ; Chinese-->Pali";
            }

            {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失
        }

        /// <summary>
        /// 值为true表示查词命令是从Pali经典显示Web页中发出的
        /// </summary>
        public static bool _isPaliWeb = false;

        /// <summary>
        /// 当前目录窗口的句柄，全局变量
        /// </summary>
        public static int frmmuluhandle = 0;

        /// <summary>
        /// 当前正在打开的读经窗口的句柄，全局变量
        /// </summary>
        public static int frmpalihandle = 0;

        /// <summary>
        /// 当前读经窗口的句柄
        /// </summary>
        public static int currfrmpaliWindowHandle = 0;

        /// <summary>
        /// 值为true表示查词命令是htmonmouseup函数发出的，缺省值为false
        /// </summary>
        public static bool _ishtmonmouseup = false;

        /*
        public void htmonload()
        {
            ((frmpali)(frmpali.FromHandle((IntPtr)(frmpalihandle)))).webBrowser1.Document.GetElementById(((frmmulu)(frmmulu.FromHandle((IntPtr)(frmmuluhandle)))).tnname).ScrollIntoView(true);
        }
        */

        private void btnTj_Click(object sender, EventArgs e)
        {
            palitongji();
        }

        private void inputpali(int i)
        {
            string c = "";

            //以下条件语句内注释后变成只输入tahoma字符
            if (cboxABC.Checked)
            {
                //if (rbtnTahoma.Checked)
                c = tahomaabc.Substring(i, 1);
                //if (rbtnSangayana.Checked)
                //c = sangayanaabc.Substring(i, 1);
                //if (rbtnVriRomanPali.Checked)
                //c = vriabc.Substring(i, 1);
            }
            else
            {
                //if (rbtnTahoma.Checked)
                c = tahomaABC.Substring(i, 1);
                //if (rbtnSangayana.Checked)
                //c = sangayanaABC.Substring(i, 1);
                //if (rbtnVriRomanPali.Checked)
                //c = vriABC.Substring(i, 1);
            }
            //int j = cboxInput.SelectionStart;
            int j = cbxSS;

            //此句删除选中的文本
            //cboxInput.Text = cboxInput.Text.Substring(0, j) + cboxInput.Text.Substring(j + cboxInput.SelectedText.Length);
            cboxInput.Text = cboxInput.Text.Substring(0, j) + cboxInput.Text.Substring(j + cbxSL);

            cboxInput.Text = cboxInput.Text.Insert(j, c);
            cboxInput.Select(j + 1, 0);

            cbxSS = cboxInput.SelectionStart;
            cbxSL = cboxInput.SelectedText.Length;

            {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失
            cboxInput.Select(j + 1, 0);

            if (cboxInput.Text.Trim().Length > 0)
            {
                if (inputError())
                    return;

                ccmsbz = 1; //设定为单独词查词模式
                bFhcSwitch = false;

                idNo = 1;

                _ishtmlListOut = true;

                bwordaheadmatch = true;

                DateTime startD, endD;
                startD = DateTime.Now;

                //iCclsDw = -1;

                if (menuosEnglishPali.Checked)
                    en_pali_cc(cboxInput.Text);
                else
                    palihan_ccFHC(cboxInput.Text);

                listBatchOut();

                if (strCxjg != "")
                    htmlListOut();
                else
                {
                    if (HAN.Checked)
                        webBrowser1.DocumentText = "没查到！请直接按 回车 或 '查词'按钮，利用变形词和复合词分析功能来分析这个pali单词！";
                    if (FAN.Checked)
                        webBrowser1.DocumentText = Strings.StrConv("没查到！请直接按 回车 或 '查词'按钮，利用变形词和复合词分析功能来分析这个pali单词！", VbStrConv.TraditionalChinese, 0x0409);
                    if (EN.Checked)
                        webBrowser1.DocumentText = "No found!please press Enter.";
                }

                _ishtmlListOut = false;

                endD = DateTime.Now;
                System.TimeSpan ts = endD.Subtract(startD);

                toollbltimes.Text = " " + ts.TotalSeconds.ToString() + " s.";

                {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失

                cbT = cboxInput.Text;
                cbtL = cboxInput.Text.Length;
            }
        }

        public static int cbxSS = 0;
        public static int cbxSL = 0;

        private void button1_Click(object sender, EventArgs e)
        {
            inputpali(0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            inputpali(1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            inputpali(2);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            inputpali(3);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            inputpali(4);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            inputpali(5);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            inputpali(6);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            inputpali(7);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            inputpali(8);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            inputpali(9);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            inputpali(10);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            inputpali(11);
        }

        private void cboxInput_MouseUp(object sender, MouseEventArgs e)
        {
            cbxSS = cboxInput.SelectionStart;
            cbxSL = cboxInput.SelectedText.Length;
        }

        private void rbtnTahoma_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnTahoma.Checked)
            {
                rbtnTahoma.ForeColor = Color.RoyalBlue;
                rbtnSangayana.ForeColor = Color.Black;
                rbtnVriRomanPali.ForeColor = Color.Black;

                cboxInput.Font = new System.Drawing.Font("Tahoma", 12);

                button11.Enabled = true;
                button12.Enabled = true;

                {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失
            }
        }

        private void rbtnSangayana_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnSangayana.Checked)
            {
                rbtnTahoma.ForeColor = Color.Black;
                rbtnSangayana.ForeColor = Color.RoyalBlue;
                rbtnVriRomanPali.ForeColor = Color.Black;

                cboxInput.Font = new System.Drawing.Font("Tahoma", 12);
                //cboxInput.Font = new System.Drawing.Font("SangayanaPlzd", 12);

                button11.Enabled = false;
                button12.Enabled = false;

                {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失
            }
        }

        private void rbtnVriRomanPali_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnVriRomanPali.Checked)
            {
                rbtnTahoma.ForeColor = Color.Black;
                rbtnSangayana.ForeColor = Color.Black;
                rbtnVriRomanPali.ForeColor = Color.RoyalBlue;

                cboxInput.Font = new System.Drawing.Font("Tahoma", 12);
                //cboxInput.Font = new System.Drawing.Font("VriRomanPlzd", 12);

                button11.Enabled = false;
                button12.Enabled = false;

                {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失
            }
        }

        private void menuQuit_Click(object sender, EventArgs e)
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

        private void stQuit_Click(object sender, EventArgs e)
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

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isSelfActivate = false;

                if (Program.toolbarform.btoolwin)
                {
                    Program.toolbarform.Show();
                    Program.toolbarform.btoolwin = false;
                }
                if (Program.toolbarform.bmuluwin)
                {
                    frmmuluwindow.Show();
                    Program.toolbarform.bmuluwin = false;
                }

                if (this.Visible && !(this.WindowState == FormWindowState.Minimized))
                    this.Activate();
                else
                {
                    //if (Program.toolbarform.bmainwin)
                    //{
                    this.Visible = true;

                    SendMessage(this.Handle, 0x112, (IntPtr)0xf120, (IntPtr)0); //恢复窗口

                    //webGenben.Document.Window.ScrollTo(webGenben.Document.Body.ScrollLeft, webbodyTop);
                    //}
                }

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
                    if (tsmiw.BackColor == Color.FromKnownColor(KnownColor.Control))
                    {
                        ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).Show();
                    }
                }
                if (haslight)
                {
                    if (((frmpali)(frmpali.FromHandle((IntPtr)(lightWin)))).Visible)
                    {
                        ((frmpali)(frmpali.FromHandle((IntPtr)(lightWin)))).Show();
                        this.Activate();
                    }
                    else
                    {
                        ((frmpali)(frmpali.FromHandle((IntPtr)(lightWin)))).Show();
                        ((frmpali)(frmpali.FromHandle((IntPtr)(lightWin)))).BringToFront();
                    }
                }

                _isSelfActivate = true;
            }
        }

        private void stShow_Click(object sender, EventArgs e)
        {
            _isSelfActivate = false;

            if (Program.toolbarform.btoolwin)
            {
                Program.toolbarform.Show();
                Program.toolbarform.btoolwin = false;
            }
            if (Program.toolbarform.bmuluwin)
            {
                frmmuluwindow.Show();
                Program.toolbarform.bmuluwin = false;
            }

            if (this.Visible && !(this.WindowState == FormWindowState.Minimized))
                this.Activate();
            else
            {
                this.Visible = true;

                SendMessage(this.Handle, 0x112, (IntPtr)0xf120, (IntPtr)0); //恢复窗口
            }

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
                if (tsmiw.BackColor == Color.FromKnownColor(KnownColor.Control))
                {
                    ((frmpali)(frmpali.FromHandle((IntPtr)(Convert.ToInt32(tsmiw.Tag))))).Show();
                }
            }
            if (haslight)
            {
                if (((frmpali)(frmpali.FromHandle((IntPtr)(lightWin)))).Visible)
                {
                    ((frmpali)(frmpali.FromHandle((IntPtr)(lightWin)))).Show();
                    this.Activate();
                }
                else
                {
                    ((frmpali)(frmpali.FromHandle((IntPtr)(lightWin)))).Show();
                    ((frmpali)(frmpali.FromHandle((IntPtr)(lightWin)))).BringToFront();
                }
            }

            _isSelfActivate = true;
        }

        /// <summary>
        /// 值为true表示是读经窗口自身激活的，为false表示是从系统托盘按钮发出命令所激活的
        /// </summary>
        public static bool _isSelfActivate = true;

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_isCloseButton)
            {
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
                this.Hide();
            }
        }

        /// <summary>
        /// 值为true表示按下的是窗口栏上的‘关闭’按钮，初始值为true
        /// </summary>
        public static bool _isCloseButton = true;

        private void pbtnfhcclose_Click(object sender, EventArgs e)
        {
            closefhctextbox();
        }

        /// <summary>
        /// 关闭复合词文本框
        /// </summary>
        private void closefhctextbox()
        {
            panelfhc.Visible = false;
            btnshowfhc.Visible = true;

            lineNum = (this.Height - 306 + 126) / 18;

            //当窗体尺寸改变时，保持webBrowser1的高度为行高18px的整倍数，以免在滚动列表时其行为不正确
            webBrowser1.Height = lineNum * 18;
            treeView1.Height = webBrowser1.Height;

            //当窗体尺寸改变时，改变每次输出结果条数
            listOutNum = lineNum * 2;

            //当窗体尺寸改变时，增加当前显示的词条数量，以免在窗体变大后列表不能把结果词条显示全
            if (_islistweb)
                htmScroll();

            {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失
        }

        /// <summary>
        /// 关闭复合词文本框2 不调用htmScroll函数
        /// </summary>
        private void closefhctextbox_t()
        {
            panelfhc.Visible = false;
            btnshowfhc.Visible = true;

            lineNum = (this.Height - 306 + 126) / 18;

            //当窗体尺寸改变时，保持webBrowser1的高度为行高18px的整倍数，以免在滚动列表时其行为不正确
            webBrowser1.Height = lineNum * 18;
            treeView1.Height = webBrowser1.Height;

            //当窗体尺寸改变时，改变每次输出结果条数
            listOutNum = lineNum * 2;

            //当窗体尺寸改变时，增加当前显示的词条数量，以免在窗体变大后列表不能把结果词条显示全
            //if (_islistweb)
            //  htmScroll();

            //{}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失
        }

        private void menuhabout_Click(object sender, EventArgs e)
        {
            aboutpced aboutform = new aboutpced();
            aboutform.TopMost = true;
            aboutform.ShowDialog();
        }

        private void btnshowfhc_Click(object sender, EventArgs e)
        {
            showfhctextbox();
        }

        /// <summary>
        /// 显示复合词文本框
        /// </summary>
        private void showfhctextbox()
        {
            panelfhc.Visible = true;
            btnshowfhc.Visible = false;

            lineNum = (this.Height - 306) / 18;

            //当窗体尺寸改变时，保持webBrowser1的高度为行高18px的整倍数，以免在滚动列表时其行为不正确
            webBrowser1.Height = lineNum * 18;
            treeView1.Height = webBrowser1.Height;

            //当窗体尺寸改变时，改变每次输出结果条数
            listOutNum = lineNum * 2;

            {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失
        }

        private void menupalimulu_Click(object sender, EventArgs e)
        {
            Program.toolbarform.Show();
            //如不加此句，初始尺寸就会高出几个像素，须用手工移动窗口才消失，不知为何？
            Program.toolbarform.Height = 25;

            frmmuluwindow.Show();
            if (FormWindowState.Minimized == frmmuluwindow.WindowState)
                frmmuluwindow.WindowState = FormWindowState.Normal;
            frmmuluwindow.BringToFront();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            if (!_onlyShowDict)
            {
                frmmuluwindow.Show();
                Program.toolbarform.Show();
                //如不加此句，初始尺寸就会高出几个像素，须用手工移动窗口才消失，不知为何？
                Program.toolbarform.Height = 25;
                //Program.toolbarform.BringToFront();
            }
            this.Activate();

            if (File.Exists(@".\pali\a-vam1.htm"))
            {
                if (MessageBox.Show(@"程序升级，需要修改本程序目录\pali子目录下的文件名称，确定吗？", "提示：", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    this.Cursor = Cursors.WaitCursor;

                    StreamReader sr = new StreamReader(new FileStream(@".\nametbl.txt", FileMode.Open), System.Text.Encoding.GetEncoding(65001));
                    string strLine = sr.ReadLine();
                    while (strLine != null)
                    {
                        if (File.Exists(@".\pali\" + strLine.Substring(0, 10).Trim() + ".htm"))
                        {
                            File.Copy(@".\pali\" + strLine.Substring(0, 10).Trim() + ".htm", @".\pali\" + strLine.Substring(10).Trim() + ".htm", true);
                            File.Delete(@".\pali\" + strLine.Substring(0, 10).Trim() + ".htm");
                        }
                        strLine = sr.ReadLine();
                    }
                    sr.Close();

                    this.Cursor = Cursors.Default;

                    MessageBox.Show("文件名称修改完成！");
                }
            }
        }

        private void stShowToolBar_Click(object sender, EventArgs e)
        {
            Program.toolbarform.Show();
            //如不加此句，初始尺寸就会高出几个像素，须用手工移动窗口才消失，不知为何？
            Program.toolbarform.Height = 25;
        }

        private void btnSeek_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("此功能从词典库的词条解释中进行搜索，可输入任何合规字符进行搜索。\r\n" +
                            "出于性能考虑，最多只输出50条结果。\r\n" +
                            "This function searches from the entry interpretation of the dictionary,\r\n" +
                            "and you can enter any compliance character to search.\r\n" +
                            "considering performance, only up to 50 results are output.", this.btnSeek);
        }

        private void btnSeek_Click(object sender, EventArgs e)
        {
            int j = 0;
            strCxjg = "";
            String vseek = "";
            String cdBzseek = "";
            String strSeek = cboxInput.Text;
            strSeek = new Regex(@"[ṃṁŋ]", RegexOptions.IgnoreCase).Replace(strSeek, "[ṃṁŋ]");
            //strSeek = "(?<w>" + strSeek + ")";
            Regex regseek = new Regex(strSeek, RegexOptions.IgnoreCase);
            for (int i = 0; i < Program.NUM; i++)
            {
                if (j>=50) 
                {
                    break;
                }

                cdBzseek = Program.strL[i].Substring(0, 1);

                MatchCollection mcseek = regseek.Matches(Program.strL[i].Substring(Program.sL[i].Length + 3));
                //foreach (Match maseek in mcseek)
                if(mcseek.Count>0)
                {
                    for (int v = 0; v < iPaliDictNum; v++)
                    {
                        if (cdBzseek == arStrPaliDictInfo[v].Substring(1, 1))
                            strCxjg = strCxjg + "<span style='color:blue;'>" + arStrPaliDictInfo[v].Substring(29) + "</span><br />";
                    }

                    //vseek = regseek.Replace(Program.strL[i].Substring(Program.sL[i].Length + 3), "<span style='color:red; background:yellow;'>"+ maseek.Groups["w"].Value + "</span>");
                    vseek = regseek.Replace(Program.strL[i].Substring(Program.sL[i].Length + 3), delegate(Match m){ return "<span style='color:red; background:yellow;'>" + m.Value + "</span>"; });
                    strCxjg = strCxjg + "<b>" + Program.sL[i] + "</b>" + "<br />" + vseek + "<br />" + "<br />";
                    j = j + 1;
                }
            }
            htmlout();
        }

        private void fsoriginal_Click(object sender, EventArgs e)
        {
            if (!fsoriginal.Checked)
            {
                fsoriginal.Checked = true;
                fsplus.Checked = false;
                fsplustwo.Checked = false;
                fsplusthree.Checked = false;

                StreamWriter swfs = new StreamWriter(@".\set\fs", false, System.Text.Encoding.GetEncoding("utf-8"));
                swfs.WriteLine("0");
                swfs.Close();
            }

            {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失
        }

        private void fsplus_Click(object sender, EventArgs e)
        {
            if (!fsplus.Checked)
            {
                fsoriginal.Checked = false;
                fsplus.Checked = true;
                fsplustwo.Checked = false;
                fsplusthree.Checked = false;

                StreamWriter swfs = new StreamWriter(@".\set\fs", false, System.Text.Encoding.GetEncoding("utf-8"));
                swfs.WriteLine("1");
                swfs.Close();

                StreamReader srf1 = new StreamReader(new FileStream(@".\set\f1", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
                strf1 = srf1.ReadToEnd();
                srf1.Close();
            }

            {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失
        }

        private void fsplustwo_Click(object sender, EventArgs e)
        {
            if (!fsplustwo.Checked)
            {
                fsoriginal.Checked = false;
                fsplus.Checked = false;
                fsplustwo.Checked = true;
                fsplusthree.Checked = false;

                StreamWriter swfs = new StreamWriter(@".\set\fs", false, System.Text.Encoding.GetEncoding("utf-8"));
                swfs.WriteLine("2");
                swfs.Close();

                StreamReader srf1 = new StreamReader(new FileStream(@".\set\f2", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
                strf1 = srf1.ReadToEnd();
                srf1.Close();
            }

            {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失
        }

        private void fsplusthree_Click(object sender, EventArgs e)
        {
            if (!fsplusthree.Checked)
            {
                fsoriginal.Checked = false;
                fsplus.Checked = false;
                fsplustwo.Checked = false;
                fsplusthree.Checked = true;

                StreamWriter swfs = new StreamWriter(@".\set\fs", false, System.Text.Encoding.GetEncoding("utf-8"));
                swfs.WriteLine("3");
                swfs.Close();

                StreamReader srf1 = new StreamReader(new FileStream(@".\set\f3", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
                strf1 = srf1.ReadToEnd();
                srf1.Close();
            }

            {}//cboxInput.Select();//此文档共49处全部替换成{}因此句代码造成编辑框中光标消失
        }

        private void splitContainer2_SplitterMoved(object sender, SplitterEventArgs e)
        {
            webBrowser1.Width = panel2.Width - treeView1.Width - 12;
        }
        /*private void splitContainer2_SplitterMoved(object sender, SplitterEventArgs e)
        {
            webBrowser1.Width = splitContainer2.Width - treeView1.Width - 12;
        }*/
    }

    /// <summary>
    /// 节点类型
    /// </summary>
    enum muluTag
    {
        目录 = 1,
        卷 = 2,
        篇 = 3,
        译本 = 4,
        巴利经文 = 5
    }

    /// <summary>
    /// 经名目录节点项的tag结构 注意：结构中的‘书名’‘书中文名’那六个变量是1.8版中新增加的，如果有什么问题，可以先删除它们，
    /// 并且使用目录D:\PCED\PCED\bin\Debug\mulubak\mulu20100302下面的原1.79版的目录mulu数据，将之复制到\Debug\mulu\下即可
    /// </summary>
    [Serializable]
    public struct tvtag
    {
        // 节点tag数字标识，标识节点类型
        public int itag;

        // 节点的tooltip文本
        public string stooltip;

        // 根本 磁盘文件名
        // 义注 磁盘文件名
        // 复注 磁盘文件名
        public string fnmula;
        public string fnattha;
        public string fntika;

        //书名
        public string mulanm;
        public string atthanm;
        public string tikanm;

        //书中文名
        public string mulanmC;
        public string atthanmC;
        public string tikanmC;

        /// <summary>
        /// 初始化结构体
        /// </summary>
        /// <param name="initag">节点tag数字标识，标识节点类型</param>
        public tvtag(int initag)
        {
            itag = initag;

            stooltip = "";
            fnmula = "";
            fnattha = "";
            fntika = "";

            mulanm = "";
            atthanm = "";
            tikanm = "";

            mulanmC = "";
            atthanmC = "";
            tikanmC = "";
        }
    }

    /// <summary>
    /// 结构：读经窗口附属信息
    /// </summary>
    [Serializable]
    public struct paliSt
    {
        /// <summary>
        /// 执行层叠/平铺/最大化之前窗体的状态是否是最大化，缺省为false
        /// </summary>
        public bool preisMax;

        /// <summary>
        /// 执行层叠/平铺/最大化之前窗体的 Left 值
        /// </summary>
        public int preLeft;

        /// <summary>
        /// 执行层叠/平铺/最大化之前窗体的 Top 值
        /// </summary>
        public int preTop;

        /// <summary>
        /// 执行层叠/平铺/最大化之前窗体的 Width 值
        /// </summary>
        public int preWidth;

        /// <summary>
        /// 执行层叠/平铺/最大化之前窗体的 Height 值
        /// </summary>
        public int preHeight;
    }

    public class FormCN : Form
    {
        public WebBrowser webBrowserCN;

        public FormCN()
        {
            webBrowserCN = new System.Windows.Forms.WebBrowser();
            //webBrowserCN.Dock = System.Windows.Forms.DockStyle.Fill;
            webBrowserCN.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            webBrowserCN.Location = new System.Drawing.Point(0, 0);
            webBrowserCN.MinimumSize = new System.Drawing.Size(20, 20);
            webBrowserCN.Size = new System.Drawing.Size(678, 401);
            webBrowserCN.TabIndex = 0;

            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(678, 401);
            this.Controls.Add(webBrowserCN);

            this.Text = "FormCN";
            if (File.Exists(@".\set\pced16.ico"))
                this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(@".\set\pced16.ico");
            this.WindowState = FormWindowState.Minimized;
            //this.ShowInTaskbar = false;
            this.Visible = false;
            this.Load += new System.EventHandler(this.FormCN_Load);
            this.Shown += new System.EventHandler(this.FormCN_Shown);
            this.SizeChanged += new System.EventHandler(this.FormCN_SizeChanged);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormCN_FormClosing);
        }

        private void FormCN_SizeChanged(object sender, EventArgs e)
        {
            //若为最小化则直接返回，不进行处理
            if (this.WindowState == FormWindowState.Minimized)
                return;

            //当窗体尺寸改变时，手工改变webBrowserCN的宽度，以避免若设置webBrowserCN.Anchor=Right则最小化及恢复时webBrowserCN会耗费时间调整其显示内容页面的宽度
            webBrowserCN.Width = this.Width - 8;
        }

        private void FormCN_Load(object sender, EventArgs e)
        {
            //Disable.DisableFormSysMenuCloseButton(this);
        }

        private void FormCN_Shown(object sender, EventArgs e)
        {

        }

        private void FormCN_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Form1._isCloseButton)
            {
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
                this.Hide();
            }
        }
    }

    /// <summary>
    ///使窗体右上角的X按钮失效
    ///利用API函数GetSystemMenu得到系统菜单的句柄
    ///X按钮是系统菜单的一菜单项，然后用RemoveMenu函数
    ///删去这一菜单项，也就是使X按钮失效了。
    /// </summary>
    public class Disable
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetSystemMenu(IntPtr hwnd, bool bRevert);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetMenuItemCount(IntPtr hMenu);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int RemoveMenu(IntPtr hMenu, int uPosition, int uFlags);

        private const int MF_BYPOSITION = 0x00000400;

        /// <summary>
        /// 取消窗体标题栏关闭按钮-使其失效变成灰色
        /// 传个Form名字进去就行了
        /// 此函数应该在窗体自身的 Load 事件中调用，如果在别的窗体中调用可能会有bug
        /// </summary>
        /// <param name="form">窗体名</param>
        public static void DisableFormSysMenuCloseButton(Form form)
        {
            IntPtr hWindow = form.Handle;
            IntPtr hMenu = GetSystemMenu(hWindow, false);
            int count = GetMenuItemCount(hMenu);
            RemoveMenu(hMenu, count - 1, MF_BYPOSITION);
            RemoveMenu(hMenu, count - 2, MF_BYPOSITION);
        }
    }
}