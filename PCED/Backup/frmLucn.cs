using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;

using System.Text.RegularExpressions;

using System.Runtime.InteropServices;

using System.Collections;

namespace pced
{
    [ComVisible(true)]
    public partial class frmLucn : Form
    {
        public frmLucn()
        {
            InitializeComponent();
        }

        public void createIndex()
        {
            StreamReader sr = new StreamReader(new FileStream(@".\7.txt", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            string s = sr.ReadToEnd();
            sr.Close();

            StreamReader sr1 = new StreamReader(new FileStream(@".\8.txt", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            string s1 = sr1.ReadToEnd();
            sr1.Close();

            Analyzer analyzer = new StandardAnalyzer();
            IndexWriter writer = new IndexWriter("index", analyzer, true);
            writer.SetMaxFieldLength(1000000);
            AddDocument(writer, "", "SQL Server 2008 的发布", "SQL Server 2008 的新特性");
            AddDocument(writer, "", "ASP.Net MVC框架配置与分析", "而今，微软推出了新的MVC开发框架，也就是Microsoft ASP.NET 3.5 Extensions");
            AddDocument(writer, "", "pali piṇḍāya", "samayena aññatarā bhikkhunī sāvatthiyaṃ piṇḍāya caritvā paṭikkamanakāle aññataraṃ bhikkhuṃ passitvā");
            AddDocument(writer, "abhp7", "", s);
            AddDocument(writer, "abhp8", "", s1);
            writer.Optimize();
            writer.Close();
        }

        public void AddDocument(IndexWriter writer, string filename, string title, string content)
        {
            Document document = new Document();
            document.Add(new Field("filename", filename, Field.Store.YES, Field.Index.TOKENIZED));
            document.Add(new Field("title", title, Field.Store.YES, Field.Index.TOKENIZED));
            document.Add(new Field("content", content, Field.Store.YES, Field.Index.TOKENIZED));
            writer.AddDocument(document);
        }

        Hits hits;
        IndexSearcher searcher;

        /// <summary>
        /// 每页输出的第一条在 hits 中的索引值
        /// </summary>
        int iEachStart = 0;

        /// <summary>
        /// 每页输出的最后一条在 hits 中的索引值
        /// </summary>
        int iEachEnd = -1;

        /// <summary>
        /// 每页最多输出的结果条数
        /// </summary>
        int iEachNum = 50;

        string strWord = "";

        public void searchWord()
        {
            strWord = tboxSs.Text.Trim();
            alAurid.Clear();

            Analyzer analyzer = new StandardAnalyzer();
            //IndexSearcher searcher = new IndexSearcher("index");
            //searcher = new IndexSearcher("index");
            MultiFieldQueryParser parser = new MultiFieldQueryParser(new string[] { "title", "content" }, analyzer);
            BooleanQuery.SetMaxClauseCount(Int32.MaxValue);
            Query query = parser.Parse(new Regex(@"\s+", RegexOptions.IgnoreCase).Replace(strWord, " && "));
            //Hits hits = searcher.Search(query);
            hits = searcher.Search(query);

            if (hits.Length() == 0)
            {
                iEachStart = 0;
                iEachEnd = -1;

                webBrowser1.AllowNavigation = true;

                webBrowser1.DocumentText =
                "<html>" +
                "<head>" +
                "<meta http-equiv='Content-Type' content='text/html; charset=utf-8'>" +

                "<style type='text/css'>" +

                "body {font-family:Tahoma; font-size:16px; line-height:22px; padding:0px 0px 0px 10px; margin:0px 0px 0px 0px;}" +
                "b {color:#0000FF; font-weight:bold;}" +

                "</style>" +

                "</head>" +

                "<body>" +

                "<script type='text/javascript'>" +
                "window.onload = function(){window.external.Luonload();};" +
                "</script>" +

                "<b>没有搜索到符合查找条件的经文！<br>您可以减少所输入的关键词数，然后再查找试试。</b>" +
                "<br><br>" +

                "</body>" +

                "</html>";

                return;
            }

            iEachStart = 0;

            listOut();
        }

        public void listOut()
        {
            string sJavaScript = "<script type='text/javascript'>" +
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

            string s = "";

            if ((hits.Length() - 1) < (iEachStart + iEachNum - 1))
                iEachEnd = hits.Length() - 1;
            else
                iEachEnd = iEachStart + iEachNum - 1;

            s = "共找到 " + hits.Length().ToString() + " 段经文，以下是第" + (iEachStart + 1).ToString() + "段到第" + (iEachEnd + 1).ToString() + "段经文。<br><br>";

            //for (int i = 0; i < hits.Length(); i++)
            for (int i = iEachStart; i < iEachEnd + 1; i++)
            {
                Document doc = hits.Doc(i);
                //Console.WriteLine(string.Format("title:{0} content:{1}", doc.Get("title"), doc.Get("content")));

                //由于 标题 里面有 '\' 字符，在 给Luonclick函数 传递参数时，'\'会消失，所以必须先把它替换为别的字符
                string xxxx = doc.Get("title");
                xxxx = new Regex(@"\\", RegexOptions.IgnoreCase).Replace(xxxx, " / ");

                //s = s + "<a style='color:#000000; background:#f0ffff; font-weight:bold; cursor:pointer;'>" + doc.Get("title") + "</a><br>" + doc.Get("content") + "<br><br>";
                s = s + "<a href='#' onclick='window.external.Luonclick(\"" + doc.Get("filename") + "\",\"" + xxxx + "\",\"cnt" + i.ToString() + "\")' id='aur" + i.ToString() + "'>" + xxxx + "</a><br><span id='cnt" + i.ToString() + "'>" + doc.Get("content") + "</span><br><br>";

                //Console.WriteLine(string.Format("title:{0} FieldsCount:{1} hitsLength:{2}", doc.Get("title"), doc.GetFieldsCount(), hits.Length()));
            }

            string strTemp = new Regex(",|\\.|\"|'|\\?", RegexOptions.None).Replace(strWord, " ").Trim();
            string[] allword = new Regex(@"\s+", RegexOptions.None).Replace(strTemp, " ").Split(' ');
            foreach (string oneword in allword)
            {
                s = new Regex(@"(?<w>\b" + oneword + @"\b)", RegexOptions.IgnoreCase).Replace(s, "<b>${w}</b>");
            }

            webBrowser1.AllowNavigation = true;

            webBrowser1.DocumentText =
              "<html>" +
              "<head>" +
              "<meta http-equiv='Content-Type' content='text/html; charset=utf-8'>" +

              "<style type='text/css'>" +

              "body {font-family:Tahoma; font-size:16px; line-height:22px; padding:0px 0px 0px 10px; margin:0px 0px 0px 0px;}" +

              //"a{color:#0000ff; background:#f0ffff; font-weight:bold; text-decoration:none; cursor:pointer;}" +
              "a{color:#0000ff; text-decoration:underline; cursor:pointer;}" +
                //"a:visited {color: #00FF00;}"+
                //"a:hover{color:#000000; text-decoration:underline;}" +
                //".vc {color: #000066;}" +
              "p {color:#000000; padding: 0px 0px 0px 25px; margin: 0px 0px 0px 0px;}" +
              "b {color:#000000; background:#FFFF66; font-weight:bold}" +
                //"b {color:#000000; background:#FFFF66; font-weight:normal}" +

              "i {font-weight:bold;}" +
              "sup {font-weight:bold;}" +
              "em {font-weight:bold; color:#0000FF;}" +

              "</style>" +

              "</head>" +

              "<body onload = 'window.external.Luonload()' onmouseup = 'window.external.htmonmouseup_Lucn()'>" +

              sJavaScript +

              s +

              "<br><br>" +
              "</body>" +

              "</html>";
        }

        public void htmonmouseup_Lucn()
        {
            this.Cursor = Cursors.WaitCursor;

            if (Program.toolbarform.cboxHccc.Checked && !_slAll)
            {
                Program.mainform.cboxInput.Text = "";

                //Form1._ishtmonmouseup = true;

                //Form1.currfrmpaliWindowHandle = ActiveForm.Handle.ToInt32();

                Form1._isPaliWeb = true;

                //webBrowser1.Document.ExecCommand("Copy", true, null);
                webBrowser1.Document.InvokeScript("getsltxtlen");
                string sssf = webBrowser1.Document.GetElementById("sltxtlen1207").Name;
                if (Convert.ToInt32(sssf) < 280)
                {
                    webBrowser1.Document.InvokeScript("getsltxt");
                    sssf = webBrowser1.Document.GetElementById("sltxt1207").Name.Trim();
                    string tttf = "";
                    tttf = frmpali.htminword(sssf);
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

        private void btnPageFirst_Click(object sender, EventArgs e)
        {
            if (0 == iEachStart)
                return;

            iEachStart = 0;

            listOut();
        }

        private void btnPagePrior_Click(object sender, EventArgs e)
        {
            if (0 == iEachStart)
                return;

            iEachStart = iEachStart - iEachNum;

            listOut();

            //if ((hits.Length() - 1) == iEachEnd)
            //  searcher.Close();
        }

        private void btnPageNext_Click(object sender, EventArgs e)
        {
            if (iEachEnd == -1)
                return;

            if ((hits.Length() - 1) == iEachEnd)
                return;

            iEachStart = iEachEnd + 1;

            listOut();
        }

        private void btnPageLast_Click(object sender, EventArgs e)
        {
            if (iEachEnd == -1)
                return;

            int icurrSt = iEachStart;

            if (((hits.Length() / iEachNum) * iEachNum) == hits.Length())
                iEachStart = (hits.Length() / iEachNum - 1) * iEachNum;
            else
                iEachStart = (hits.Length() / iEachNum) * iEachNum;

            if (icurrSt == iEachStart)
                return;

            listOut();
        }

        public void Luonload()
        {
            webBrowser1.AllowNavigation = false;

            //把点击过的链接变为红色显示
            foreach (int uid in alAurid)
            {
                for (int t = iEachStart; t < iEachEnd + 1; t++)
                {
                    if (uid == t)
                        webBrowser1.Document.GetElementById("aur" + uid.ToString()).Style = "color:#ff0000;";
                }
            }
        }

        public void Luonclick(string filename, string title, string cntid)
        {
            this.Cursor = Cursors.WaitCursor;

            string bookpath = @".\pali\" + filename + ".htm";

            if (!(File.Exists(bookpath)))
            {
                MessageBox.Show("此篇经典文件没找到！您可能没有安装本程序的‘pali经典文件库’或者是删除了文件！\r\n请到‘觉悟之路’网站 http://www.dhamma.org.cn/ 重新下载本程序。");
                return;
            }

            StreamReader sr = new StreamReader(new FileStream(bookpath, FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));

            string strHead = "";
            string strLine = "";
            do
            {
                strLine = sr.ReadLine();
                strHead = strHead + strLine + "\r\n";
            } while (strLine.Trim() != "<body>");



            //改变不同字体大小设置的style风格
            if (!(Program.mainform.fsoriginal.Checked))
                strHead = new Regex(@"<style>[\S\s]*</style>", RegexOptions.IgnoreCase).Replace(strHead, Form1.strf1);



            strHead = new Regex("<body>", RegexOptions.IgnoreCase).Replace(strHead, "<body onload = 'window.external.htmonload2()' onmouseup = 'window.external.htmonmouseup()'>" + frmmulu.strJavaScript);


            string strSZ = sr.ReadToEnd();
            sr.Close();

            //引号前后加空格，以避免在浏览器中查找时，被连着引号一起当成一个词，而造成引号边的词查找不出
            strSZ = new Regex("(?<w>‘+|’+)", RegexOptions.None).Replace(strSZ, " ${w} ");

            strSZ = new Regex("<p class=\"nikaya\">", RegexOptions.None).Replace(strSZ, "<p class=\"nikaya\"><a name=\"nikaya\"></a>");
            strSZ = new Regex("<p class=\"book\">", RegexOptions.None).Replace(strSZ, "<p class=\"book\"><a name=\"book\"></a>");
            strSZ = new Regex("<p class=\"title\">", RegexOptions.None).Replace(strSZ, "<p class=\"title\"><a name=\"title\"></a>");
            strSZ = new Regex("<p class=\"chapter\">", RegexOptions.None).Replace(strSZ, "<p class=\"chapter\"><a name=\"chapter\"></a>");

            strSZ = strHead + strSZ;

            frmpali frmw = new frmpali();

            title = new Regex(@"<b>|</b>", RegexOptions.IgnoreCase).Replace(title, "");

            //frmw.Text = title;

            //frmw.palilb = rootIndex;
            //frmw.sanzanglb = secondIndex;
            //frmw.mulafile = ((tvtag)(currnode.Tag)).fnmula;
            //frmw.atthafile = ((tvtag)(currnode.Tag)).fnattha;
            //frmw.tikafile = ((tvtag)(currnode.Tag)).fntika;

            foreach (string strName in Program.mainform.alName)
            {
                if (strName.Substring(0, 20).Trim() == filename)
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

                    //取得根节点的索引，以确定当前点击的章节是根本、义注、复注或其它
                    TreeNode trN = trNx;
                    while (trN.Level > 0)
                    {
                        trN = trN.Parent;
                    }
                    frmw.palilb = trN.Index;

                    //取得根节点之下一层节点的索引，以确定当前点击的章节是‘律’、‘经’或‘论’
                    trN = trNx;
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
            //frmw.sKeyWord = webBrowser1.Document.GetElementById(cntid).InnerText;

            //引号前后加空格，以避免在浏览器中查找时，被连着引号一起当成一个词，而造成引号边的词查找不出
            frmw.sKeyWord = new Regex("(?<w>‘+|’+)", RegexOptions.None).Replace(webBrowser1.Document.GetElementById(cntid).InnerText, " ${w} ");
            frmw.sKeyWord = new Regex(@"\s+", RegexOptions.None).Replace(frmw.sKeyWord, " ").Trim();

            //把点击过的链接变为红色显示
            webBrowser1.Document.GetElementById("aur" + cntid.Substring(3)).Style = "color:#ff0000;";
            //记录下点击过的链接id
            alAurid.Add(Convert.ToInt32(cntid.Substring(3)));

            Form1.frmpalihandle = frmw.Handle.ToInt32();

            frmw.tstboxWord.Text = strWord;
            frmw.webBrowser1.DocumentText = strSZ;

            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// 被点击过的链接id列表
        /// </summary>
        public ArrayList alAurid = new ArrayList();

        IndexWriter writer;

        private void button1_Click(object sender, EventArgs e)
        {
            if (tboxSs.Text.Trim().ToLower() != "index")
            {
                MessageBox.Show("一般不需要重建索引，如需重建索引，请在左边文本框中输入英文'index'，然后再点击此索引按钮！\r\n请注意，如需新建或重建索引，应先把pced程序目录下的index目录删除，然后到‘搜索’窗中重建索引。\r\n注意：在第一次（没建索引前）打开‘搜索’窗口时会出错误，这时按‘继续’就可以了，\r\n然后在搜索窗口里按‘建立或重建索引’按钮重建索引即可，建索引约需2、3个小时，\r\n中间无提示、程序无反应（但可看到index目录下的文件不断变化），不要强行中断程序。\r\n索引建成后程序的 index 子目录下那三个文件即是索引文件。");
                return;
            }


            label1.Visible = true;

            numwait = 0;

            Analyzer analyzer = new StandardAnalyzer();
            writer = new IndexWriter("index", analyzer, true);
            writer.SetMaxFieldLength(1000000);

            TreeNodeCollection nodes = Program.mainform.frmmuluwindow.treeView2.Nodes;
            foreach (TreeNode n in nodes)
            {
                cntopali(n);
            }

            writer.Optimize();
            writer.Close();

            label1.Visible = false;

            MessageBox.Show("索引建立完成！");
        }

        int numwait = 0;

        private void cntopali(TreeNode treeNode)
        {
            //if (numwait< Convert.ToInt32(textBox2.Text) && ((tvtag)(treeNode.Tag)).itag == 2)
            if (((tvtag)(treeNode.Tag)).itag == 2)
            {
                //textBox1.Text = textBox1.Text + treeNode.Name + "##" + treeNode.FullPath + "\r\n";

                string s = "";

                StreamReader sr = new StreamReader(new FileStream(@".\pali\" + treeNode.Name + ".htm", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));

                do
                {
                    s = sr.ReadLine();
                } while (s.Trim() != "<body>");

                s = sr.ReadLine();
                while (s != null)
                {
                    if (s.Trim() != "")
                    {
                        s = new Regex("<[^<>]*>", RegexOptions.IgnoreCase).Replace(s, "");

                        AddDocument(writer, treeNode.Name, treeNode.FullPath, s);
                    }

                    s = sr.ReadLine();
                }

                sr.Close();

                numwait++;
                label1.Text = "已索引" + numwait.ToString() + "个文档";
                label1.Refresh();
            }

            foreach (TreeNode tn in treeNode.Nodes)
            {
                cntopali(tn);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (tboxSs.Text.Trim() == "")
                return;

            if (tboxSs.Text.Trim() == "anicca1120")
                btnDcinx.Visible = true;

            if (!(System.IO.Directory.Exists(@".\index")))
            {
                MessageBox.Show("第一次使用三藏经典全文搜索功能，请先建立索引！");
                return;
            }

            listBox1.Visible = false;

            try
            {
                searchWord();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void frmLucn_Load(object sender, EventArgs e)
        {
            webBrowser1.ObjectForScripting = this;

            searcher = new IndexSearcher("index");
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
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
                if (tboxSs.Text.Trim() == "")
                    return;

                if (!(System.IO.Directory.Exists(@".\index")))
                {
                    MessageBox.Show("第一次使用三藏经典全文搜索功能，请先建立索引！");
                    return;
                }

                listBox1.Visible = false;

                try
                {
                    searchWord();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

                e.Handled = true;
            }
        }

        private void frmLucn_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Form1._isCloseButton)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void frmLucn_FormClosed(object sender, FormClosedEventArgs e)
        {
            searcher.Close();
        }

        private void tboxSs_KeyDown(object sender, KeyEventArgs e)
        {
            //快捷键 全选 复制 粘贴 剪切
            if (e.Modifiers == Keys.Control)
            {
                if (e.KeyCode == Keys.A)
                {
                    tboxSs.SelectAll();
                }

                if (e.KeyCode == Keys.C)
                {
                    Form1.fzbz = -1;
                    Form1.cbdText = Program.mainform.outword_t(tboxSs.SelectedText);
                    if (Form1.cbdText != "") //因为金山词霸在本程序窗口里划词的时候会引发异常，故设此条件判断
                        Clipboard.SetText(Form1.cbdText, TextDataFormat.UnicodeText);
                }

                if (e.KeyCode == Keys.V)
                {
                    int j = tboxSs.SelectionStart;
                    //此句删除选中的文本
                    tboxSs.Text = tboxSs.Text.Substring(0, j) + tboxSs.Text.Substring(j + tboxSs.SelectedText.Length);
                    string scb = Program.mainform.inword_t(Clipboard.GetText(TextDataFormat.UnicodeText));
                    int igh = scb.Length;
                    tboxSs.Text = tboxSs.Text.Insert(j, scb);
                    tboxSs.Select(j + igh, 0);
                    tboxSs.Select();
                }

                if (e.KeyCode == Keys.X)
                {
                    Form1.fzbz = -1;
                    Form1.cbdText = Program.mainform.outword_t(tboxSs.SelectedText);
                    if (Form1.cbdText != "") //因为金山词霸在本程序窗口里划词的时候会引发异常，故设此条件判断
                        Clipboard.SetText(Form1.cbdText, TextDataFormat.UnicodeText);

                    int j = tboxSs.SelectionStart;
                    tboxSs.Text = tboxSs.Text.Remove(tboxSs.SelectionStart, tboxSs.SelectionLength);
                    tboxSs.Select(j, 0);
                    tboxSs.Select();
                }

                e.SuppressKeyPress = true;
            }

            //快捷键 粘贴
            if (e.Modifiers == Keys.Shift)
            {
                if (e.KeyCode == Keys.Insert)
                {
                    int j = tboxSs.SelectionStart;
                    //此句删除选中的文本，倘若要把‘粘贴’功能改为‘插入’功能，只需注释掉此句即可
                    tboxSs.Text = tboxSs.Text.Substring(0, j) + tboxSs.Text.Substring(j + tboxSs.SelectedText.Length);
                    string scb = Program.mainform.inword_t(Clipboard.GetText(TextDataFormat.UnicodeText));
                    int igh = scb.Length;
                    tboxSs.Text = tboxSs.Text.Insert(j, scb);
                    tboxSs.Select(j + igh, 0);
                    tboxSs.Select();

                    e.SuppressKeyPress = true;
                }
            }
        }

        private void cmiPaste_Click(object sender, EventArgs e)
        {
            int j = tboxSs.SelectionStart;
            //此句删除选中的文本
            tboxSs.Text = tboxSs.Text.Substring(0, j) + tboxSs.Text.Substring(j + tboxSs.SelectedText.Length);
            string scb = Program.mainform.inword_t(Clipboard.GetText(TextDataFormat.UnicodeText));
            int igh = scb.Length;
            tboxSs.Text = tboxSs.Text.Insert(j, scb);
            tboxSs.Select(j + igh, 0);
            tboxSs.Select();
        }

        private void cmiSelectAll_Click(object sender, EventArgs e)
        {
            tboxSs.SelectAll();
        }

        private void cmiCopy_Click(object sender, EventArgs e)
        {
            Form1.fzbz = -1;
            Form1.cbdText = Program.mainform.outword_t(tboxSs.SelectedText);
            if (Form1.cbdText != "") //因为金山词霸在本程序窗口里划词的时候会引发异常，故设此条件判断
                Clipboard.SetText(Form1.cbdText, TextDataFormat.UnicodeText);
        }

        private void cmiCut_Click(object sender, EventArgs e)
        {
            Form1.fzbz = -1;
            Form1.cbdText = Program.mainform.outword_t(tboxSs.SelectedText);
            if (Form1.cbdText != "") //因为金山词霸在本程序窗口里划词的时候会引发异常，故设此条件判断
                Clipboard.SetText(Form1.cbdText, TextDataFormat.UnicodeText);

            int j = tboxSs.SelectionStart;
            tboxSs.Text = tboxSs.Text.Remove(tboxSs.SelectionStart, tboxSs.SelectionLength);
            tboxSs.Select(j, 0);
            tboxSs.Select();
        }

        /// <summary>
        /// 值为true表示在读经窗口中进行了全选，缺省值为false
        /// </summary>
        public static bool _slAll = false;

        private void cmwSelectAll_Click(object sender, EventArgs e)
        {
            webBrowser1.Document.ExecCommand("SelectAll", true, null);
            _slAll = true;
        }

        private void cmwCopy_Click(object sender, EventArgs e)
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

        private void btnDcinx_Click(object sender, EventArgs e)
        {
            Analyzer analyzer = new StandardAnalyzer();
            IndexWriter writer = new IndexWriter("dcinx", analyzer, true);
            writer.SetMaxFieldLength(1000000);

            char[] ca = "āīūṅñṭḍṇḷŋĀĪŪṄÑṬḌṆḶŊṁṃṀṂ".ToCharArray();
            char[] cb = "aiunntdnlmAIUNNTDNLMmmMM".ToCharArray();
            int i = 0;
            string dce = "";

            StreamReader sr = new StreamReader(new FileStream(@".\dancienx.txt", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            string s = sr.ReadLine();
            while (s != null)
            {
                if (s.Trim() != "")
                {
                    dce = s.Substring(7);
                    i = 0;
                    foreach (char c in ca)
                    {
                        dce = new Regex(c.ToString(), RegexOptions.None).Replace(dce, cb[i].ToString());
                        i++;
                    }
                    Document document = new Document();
                    document.Add(new Field("dc_en", dce, Field.Store.YES, Field.Index.UN_TOKENIZED));
                    document.Add(new Field("dc_pali", s.Substring(7), Field.Store.YES, Field.Index.NO));
                    document.Add(new Field("dc_num", s.Substring(0, 7).TrimStart('0'), Field.Store.YES, Field.Index.NO));
                    writer.AddDocument(document);
                }
                s = sr.ReadLine();
            }
            sr.Close();

            writer.Optimize();
            writer.Close();

            MessageBox.Show("单词索引建立完成！");
        }

        private void textBox2_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == System.Convert.ToChar(13))
            {
                if (textBox2.Text.Trim().Length < 3)
                {
                    MessageBox.Show("因为查找速度的原因，请至少输入3个字母！");
                    return;
                }

                listBox1.Visible = true;

                string dcWord = textBox2.Text.Trim();

                Analyzer analyzer = new StandardAnalyzer();
                //IndexSearcher searcher = new IndexSearcher("index");
                //searcher = new IndexSearcher("index");
                IndexSearcher searcher = new IndexSearcher("dcinx");
                MultiFieldQueryParser parser = new MultiFieldQueryParser(new string[] { "dc_en" }, analyzer);
                //Query query = parser.Parse(new Regex(@"\s+", RegexOptions.IgnoreCase).Replace(dcWord, " && "));
                BooleanQuery.SetMaxClauseCount(Int32.MaxValue);
                Query query = parser.Parse(dcWord + "*");
                Hits dchits = searcher.Search(query);
                //hits = searcher.Search(query);

                if (dchits.Length() <= 0)
                {
                    listBox1.Items.Clear();
                    listBox1.Items.Add("没有找到符合的词！");
                }
                else
                {
                    listBox1.Items.Clear();
                    //listBox1.Items.Add("共找到 " + dchits.Length().ToString() + " 词，注：单词后的数字表示统计的单词在经典中出现的次数。");
                    //listBox1.Items.Add("共找到 " + dchits.Length().ToString() + " 词，在词条上双击可自动增加进上框。");
                    for (int i = 0; i < dchits.Length(); i++)
                    {
                        if (i >= 100)
                        {
                            listBox1.Items.Add("--------------------------------------------------------------------------------");
                            listBox1.Items.Add(dchits.Length().ToString() + "词，在此只列出前100词。");
                            break;
                        }
                        //listBox1.Items.Add(dchits.Doc(i).Get("dc_pali") + "    :" + dchits.Doc(i).Get("dc_num"));
                        listBox1.Items.Add(dchits.Doc(i).Get("dc_pali"));
                    }
                    listBox1.Focus();
                    listBox1.SelectedIndex = 0;
                }

                e.Handled = true;
            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            tboxSs.Text = tboxSs.Text + listBox1.SelectedItem.ToString() + " ";
            listBox1.Visible = false;
            tboxSs.Focus();
            tboxSs.SelectionStart = tboxSs.Text.Length;
        }

        private void listBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == System.Convert.ToChar(13))
            {
                tboxSs.Text = tboxSs.Text + listBox1.SelectedItem.ToString() + " ";
                listBox1.Visible = false;
                tboxSs.Focus();
                tboxSs.SelectionStart = tboxSs.Text.Length;
                e.Handled = true;
            }
        }

        private void frmLucn_Shown(object sender, EventArgs e)
        {
            textBox2.Focus();
            textBox2.SelectAll();
        }
    }
}
