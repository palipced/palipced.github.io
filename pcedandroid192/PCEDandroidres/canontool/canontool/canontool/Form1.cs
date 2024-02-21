using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Diagnostics;
using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;

namespace canontool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static void getDc()
        {
            //书编号
            int bookid = 0;
            //每本书中段的编号
            int paraid = 0;
            string[] filelist = Directory.GetFiles(@".\palitxt\", "*.txt", System.IO.SearchOption.TopDirectoryOnly);
            foreach (string txtfile in filelist)
            {
                bookid = bookid + 1;
                StreamReader sr = new StreamReader(new FileStream(txtfile, FileMode.Open), System.Text.Encoding.GetEncoding(65001));
                string row = sr.ReadLine();
                while (row != null)
                {
                    if (row != "")
                    {
                        paraid = paraid + 1;
                    }

                    row = sr.ReadLine();
                }
                sr.Close();

            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //textBox1.Text = "";

            //SQLiteConnection.CreateFile(@".\testDB.db");
            //SQLiteConnection dbConnection = new SQLiteConnection(@"data source = .\pali.db3");
            SQLiteConnection dbConnection = new SQLiteConnection(@"data source = d:\pali.db3");
            dbConnection.Open();

            //string[] colNames = new string[] { "ID", "Name", "Age", "Email" };

            SQLiteCommand dbCommand = dbConnection.CreateCommand();
            //dbCommand.CommandText = "CREATE TABLE IF NOT EXISTS table2(ID INTEGER,Name TEXT,Age INTEGER,Email TEXT)";
            //dbCommand.ExecuteNonQuery();

            dbCommand.CommandText = "CREATE VIRTUAL TABLE IF NOT EXISTS canons USING fts4(BOOKID INTEGER,PARAID INTEGER,PALI TEXT,filename TEXT,A TEXT,B TEXT,C TEXT,BOOK TEXT);";
            dbCommand.ExecuteNonQuery();

            string filename = "";
            string A = "";
            string B = "";
            string C = "";
            string BOOK = "";

            //书编号
            int bookid = 0;
            //每本书中段的编号
            int paraid;
            string txtname = "";
            string[] filelist = Directory.GetFiles(@".\palitxt\", "*.txt", System.IO.SearchOption.TopDirectoryOnly);
            foreach (string txtfile in filelist)
            {

                ////////////////////
                txtname=txtfile.Substring(10);

                StreamReader sr1 = new StreamReader(new FileStream(@".\filecata.txt", FileMode.Open), System.Text.Encoding.GetEncoding(65001));
                string row1 = sr1.ReadLine();
                while (row1 != null)
                {
                    ////
                    MatchCollection mc = new Regex("^(?<w0>.*?)#(?<w1>.*?)#(?<w2>.*?)#(?<w3>.*?)#(?<w4>.*?)$", RegexOptions.None).Matches(row1);
                    foreach (Match ma in mc)
                    {
                        filename = ma.Groups["w0"].Value;
                        A = ma.Groups["w1"].Value;
                        B = ma.Groups["w2"].Value;
                        C = ma.Groups["w3"].Value;
                        BOOK = ma.Groups["w4"].Value;
                    }
                    ////
                    if (txtname == filename+".htm.txt") {
                        break;
                    }

                    row1 = sr1.ReadLine();
                }
                sr1.Close();
                //////////////////////

                bookid = bookid + 1;
                StreamReader sr = new StreamReader(new FileStream(txtfile, FileMode.Open), System.Text.Encoding.GetEncoding(65001));
                paraid = 0;
                string row = sr.ReadLine();
                while (row != null)
                {
                    if (row != "")
                    {
                        paraid = paraid + 1;

                        dbCommand.CommandText = "insert into canons (BOOKID, PARAID, PALI,filename,A,B,C,BOOK) values(" + bookid + "," + paraid + ",'" + row + "','"+filename+ "','" + A + "','" + B + "','" + C + "','" + BOOK + "');";
                        dbCommand.ExecuteNonQuery();

                    }

                    row = sr.ReadLine();
                }
                sr.Close();

                Debug.WriteLine(bookid.ToString());
            }


            /*
            dbCommand.CommandText = "SELECT * FROM canons WHERE PALI MATCH 'abc巴利āīūṅñṭḍṇḷṃṁŋ123 1233 10086 123';";
            SQLiteDataReader dataReader = dbCommand.ExecuteReader();
            while (dataReader.Read())
            {

            textBox1.Text = textBox1.Text + "\r\n" + dataReader.GetInt32(0).ToString() + " # " + dataReader.GetInt32(1).ToString() + " # " + dataReader.GetString(2);
            }
            dataReader.Close();*/

            dbConnection.Close();

            MessageBox.Show("ok");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //-,;</>.?‘’(384519)27–…60ः!=\+`^]&ऋ
            //webBrowser1.Document.GetElementById(s1).InnerText
            //webBrowser1.Document.Body.ScrollTop
            int i;
            string[] filelist = Directory.GetFiles(@".\pali\", "*.htm", System.IO.SearchOption.TopDirectoryOnly);
            foreach (string s in filelist)
            {
                i = 0;
                StreamWriter sw = new StreamWriter(@".\palitxt\" + s.Substring(7) + ".txt", false, System.Text.Encoding.GetEncoding(65001));

                StreamReader sr = new StreamReader(new FileStream(s, FileMode.Open), System.Text.Encoding.GetEncoding(65001));
                string dc = sr.ReadToEnd();

                sr.Close();

                //dc = new Regex("<body>", RegexOptions.IgnoreCase).Replace(dc, "");
                //dc = new Regex("</body>", RegexOptions.IgnoreCase).Replace(dc, "");
                //if (s== (@".\pali\"+ "vin02m4.mul.htm")) {
                //webBrowser1.DocumentText = dc;

                //}

                //MatchCollection mc1 = new Regex("<p class=\"bodytext\">", RegexOptions.None).Matches(dc);
                //MatchCollection mc2 = new Regex("<p class=\"gatha1\">", RegexOptions.None).Matches(dc);
                //MatchCollection mc3 = new Regex("<p class=\"gathalast\">", RegexOptions.None).Matches(dc);
                //gatha缺一：此情况不存在，即是说1和last必然同有或同无
                //if (((mc2.Count <= 0) & (mc3.Count > 0))| ((mc2.Count > 0) & (mc3.Count <= 0)))
                //三者全无：此情况不存在
                //if ((mc1.Count <= 0) & (mc2.Count <= 0) & (mc3.Count <= 0))
                //{
                //MessageBox.Show(s);
                //}

                dc = new Regex("<p class=\"gatha1\">[\\s\\S]*?<p class=\"gathalast\">", RegexOptions.IgnoreCase).Replace(dc, delegate (Match m) { return new Regex("(\r\n){2,}", RegexOptions.IgnoreCase).Replace(m.Value, "\r\n"); });
                dc = new Regex("^[\\s\\S]*?<body>", RegexOptions.IgnoreCase).Replace(dc, "");
                dc = new Regex("<[\\s\\S]*?>", RegexOptions.IgnoreCase).Replace(dc, "");
                dc = new Regex("\\[[\\s\\S]*?\\]", RegexOptions.IgnoreCase).Replace(dc, "");
                dc = new Regex("'", RegexOptions.IgnoreCase).Replace(dc, "’");

                dc = new Regex(@"[\]\\\-0123456789().,;–</>?‘’=+!`^&…ःऋ]", RegexOptions.IgnoreCase).Replace(dc, delegate (Match m) { return " "+m.Value+" "; });

                dc = new Regex("\r\n", RegexOptions.IgnoreCase).Replace(dc, "<br />");
                dc = new Regex("(<br />){2,}", RegexOptions.IgnoreCase).Replace(dc, "\r\n");
                dc = new Regex("<br />", RegexOptions.IgnoreCase).Replace(dc, " <br /> ");

                sw.Write(dc);

                sw.Close();

                //File.Delete(s);
            }
            MessageBox.Show("ok");
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            return;
            StreamWriter sw = new StreamWriter(@".\palitxt\vin02m4.mul.htm.txt", false, System.Text.Encoding.GetEncoding(65001));


            sw.Write(webBrowser1.Document.Body.InnerText);

            sw.Close();
            MessageBox.Show("ok1");
        }

        private void btnseekdb_Click(object sender, EventArgs e)
        {
            SQLiteConnection dbConnection = new SQLiteConnection(@"data source = R:\pali.db3");
            dbConnection.Open();

            SQLiteCommand dbCommand = dbConnection.CreateCommand();

            string filename = "";
            string A = "";
            string B = "";
            string C = "";
            string BOOK = "";

            //书编号
            int bookid = 0;
            //每本书中段的编号
            int paraid;

            bookid = bookid + 1;
            StreamReader sr = new StreamReader(new FileStream(@"R:\cidian", FileMode.Open), System.Text.Encoding.GetEncoding(65001));
            paraid = 0;
            string row = sr.ReadLine();
            while (row != null)
            {
                if (row != "")
                {
                    paraid = paraid + 1;
                    BOOK = row.Substring(0, 1);
                    //row = new Regex("'", RegexOptions.None).Replace(row, @"''");
                    row = new Regex("'", RegexOptions.None).Replace(row, @"’");
                    //繁简转换
                    row = ChineseConverter.Convert(row, ChineseConversionDirection.TraditionalToSimplified);
                    row = new Regex("[\u4e00-\u9fa5]", RegexOptions.IgnoreCase).Replace(row, delegate (Match m) { return " " + m.Value + " "; });
                    row = new Regex(" +", RegexOptions.IgnoreCase).Replace(row, " ");
                    //dbCommand.CommandText = "insert into canons (BOOKID, PARAID, PALI,filename,A,B,C,BOOK) values(0,0,'" + row.Substring(4) + "',' ',' ',' ',' ','" + BOOK + "');";
                    dbCommand.CommandText = "insert into canons (BOOKID, PARAID, PALI,filename,A,B,C,BOOK) values(0,0,'" + row + "',' ',' ',' ',' ','" + BOOK + "');";
                    dbCommand.ExecuteNonQuery();

                }

                /*//
                if (paraid>=1000) {
                    sr.Close();
                    dbConnection.Close();

                    MessageBox.Show("ok");
                    return;

                }
                //*/
                if (paraid % 100 == 0)
                {
                    Debug.WriteLine(paraid.ToString());
                }

                row = sr.ReadLine();
            }
            sr.Close();

            dbConnection.Close();

            MessageBox.Show("ok");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SQLiteConnection dbConnection = new SQLiteConnection(@"data source = d:\palihtm.db3");
            dbConnection.Open();

            SQLiteCommand dbCommand = dbConnection.CreateCommand();

            dbCommand.CommandText = "CREATE TABLE IF NOT EXISTS palihtm(bookid INTEGER,filename TEXT,arow TEXT,classname TEXT,paranum TEXT,a TEXT,a2 TEXT,palihtm TEXT)";
            dbCommand.ExecuteNonQuery();

            string arow = "";
            string a = "";
            string a2 = "";
            string classname = "";
            string paranum = "";

            //书编号
            int bookid = 0;
            string htmname = "";
            string[] filelist = Directory.GetFiles(@".\pali\", "*.htm", System.IO.SearchOption.TopDirectoryOnly);
            foreach (string htmfile in filelist)
            {
                htmname = htmfile.Substring(7);

                bookid = bookid + 1;
                StreamReader sr = new StreamReader(new FileStream(htmfile, FileMode.Open), System.Text.Encoding.GetEncoding(65001));
                string row = sr.ReadLine();
                while (row != null)
                {
                    row = row.Trim();
                    row = new Regex("'", RegexOptions.None).Replace(row, "’");
                    if (row.Length >= 2)
                    {
                        arow = "";
                        classname = "";
                        paranum = "";
                        a = "";
                        a2 = "";
                        if (row.Substring(0, 2) == "<a")
                        {
                            
                            ///
                            MatchCollection mca = new Regex("^<a name=\"(?<ar>[\\w\\d_]+?)\"></a>", RegexOptions.None).Matches(row);
                            foreach (Match ma in mca)
                            {
                                arow = ma.Groups["ar"].Value;
                            }
                            ///
                            dbCommand.CommandText = "insert into palihtm(bookid, filename, arow,classname, paranum,a, a2,palihtm) values(" + bookid + ",'" + htmname + "','"+arow+"','" + classname + "','" + paranum + "','" + a + "','" + a2 + "','" + row + "');";
                            dbCommand.ExecuteNonQuery();
                            /*
                            if (row.Substring(0, 8) != "<a name=")
                            {
                                Debug.WriteLine(row);
                            }*/
                        }

                        if (row.Substring(0, 2) == "<p")
                        {

                            ///
                            MatchCollection mc = new Regex("^<p class=\"(?<w>[\\w\\d]*?)\">", RegexOptions.None).Matches(row);
                            foreach (Match ma in mc)
                            {
                                classname = ma.Groups["w"].Value;
                            }
                            ///
                            ///
                            MatchCollection mc1 = new Regex("<span class=\"paranum\">(?<d>[\\d-]+?)</span>", RegexOptions.None).Matches(row);
                            foreach (Match ma in mc1)
                            {
                                paranum = ma.Groups["d"].Value;
                            }
                            ///
                            ///
                            MatchCollection mc3 = new Regex("<a name=\"(?<a>para[\\d-]+?)\">.*</a>", RegexOptions.None).Matches(row);
                            foreach (Match ma in mc3)
                            {
                                a = ma.Groups["a"].Value;
                            }
                            ///
                            ///
                            MatchCollection mc2 = new Regex("<a name=\"para[\\d-]+?\">.*</a><a name=\"(?<a2>para[\\w\\d-_]+?)\">", RegexOptions.None).Matches(row);
                            foreach (Match ma in mc2)
                            {
                                a2 = ma.Groups["a2"].Value;
                            }
                            ///
                            dbCommand.CommandText = "insert into palihtm(bookid, filename, classname, paranum,a, a2,palihtm) values(" + bookid + ",'" + htmname + "','"+classname+"','"+paranum+"','"+a+"','"+a2+"','" + row + "');";
                            dbCommand.ExecuteNonQuery();
                        }
                        /*
                        if ((row.Substring(0, 1) == "<"))
                        {
                            if ((row.Substring(0, 2) != "<a") && (row.Substring(0, 2) != "<p"))
                            {
                                Debug.WriteLine(row);
                            }
                        }*/
                    }

                    row = sr.ReadLine();
                }
                sr.Close();
                
                Debug.WriteLine(bookid.ToString());
                //if (bookid == 10) { break; }
            }
            dbConnection.Close();

            MessageBox.Show("ok");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string dc = new Regex("[\u4e00-\u9fa5]", RegexOptions.IgnoreCase).Replace("覚った, 目                    覚めたる, 覚知せる; 覚者, 仏陀, 仏. -ānubuddha 仏に隨って覚れる. -ānussati 仏随念, 念仏. -āpadāniya 仏前生譚. -ārammaṇa 仏所縁. -āsana 仏座. -uppāda 仏の出世. -khetta 仏剎土, 仏国. -gata 仏に向けたる. -cakkhu 仏眼. -ñāṇa 仏智. -thūpa 仏塔. -paṭimā, -bimba 仏像. -pūjā 仏供養. -bhāsita 仏所説", delegate (Match m) { return " " + m.Value + " "; });
            dc = new Regex(" +", RegexOptions.IgnoreCase).Replace(dc, " ");
            MessageBox.Show(dc);
        }
    }
}