using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace pced
{
    public partial class frmCk : Form
    {
        public frmCk()
        {
            InitializeComponent();
        }

        private void lvCdxx_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                tboxCdlb.Text = lvCdxx.FocusedItem.Text;

                tboxCdbz.Text = lvCdxx.FocusedItem.SubItems[1].Text;

                tboxCdName.Text = lvCdxx.FocusedItem.SubItems[2].Text;

                tboxCdshm.Text = lvCdxx.FocusedItem.SubItems[3].Text;
            }
        }

        /// <summary>
        /// 词库中的词汇总数
        /// </summary>
        string sWordNum;

        /// <summary>
        /// 词库中的词典总数
        /// </summary>
        string sDictNum;

        private void ViewlvZxxx()
        {
            ListViewItem lvi;
            ListViewItem.ListViewSubItem lvsi;

            this.lvCdxx.Items.Clear();

            this.lvCdxx.BeginUpdate();



            string s;
            int i = 0;
            StreamReader sr = new StreamReader(new FileStream(@".\pali-h\dicinfo", FileMode.Open), System.Text.Encoding.GetEncoding(65001));
            sWordNum = sr.ReadLine();
            sDictNum = sr.ReadLine();
            s = sr.ReadLine();
            while (s != null)
            {


                // Create the main ListViewItem
                lvi = new ListViewItem();
                lvi.Tag = i.ToString();
                lvi.Text = s.Substring(0, 1);
                //lvi.ImageIndex = 0;

                lvsi = new ListViewItem.ListViewSubItem();
                lvsi.Text = s.Substring(1, 1);
                lvi.SubItems.Add(lvsi);

                lvsi = new ListViewItem.ListViewSubItem();
                lvsi.Text = s.Substring(3, 25).Trim();
                lvi.SubItems.Add(lvsi);

                lvsi = new ListViewItem.ListViewSubItem();
                lvsi.Text = s.Substring(29).Trim();
                lvi.SubItems.Add(lvsi);

                //lvsi = new ListViewItem.ListViewSubItem();
                //lvsi.Text = s.Substring(0, 1);
                //lvi.SubItems.Add(lvsi);

                // Add the ListViewItem to the Items collection of the ListView
                this.lvCdxx.Items.Add(lvi);

                i++;

                s = sr.ReadLine();
            }
            sr.Close();

            this.lvCdxx.EndUpdate();
        }

        private void frmCk_Shown(object sender, EventArgs e)
        {
            ViewlvZxxx();
        }

        //确定修改词典信息
        private void button1_Click(object sender, EventArgs e)
        {
            lvCdxx.FocusedItem.Text = tboxCdlb.Text;
            lvCdxx.FocusedItem.SubItems[1].Text = tboxCdbz.Text;
            lvCdxx.FocusedItem.SubItems[2].Text = tboxCdName.Text;
            lvCdxx.FocusedItem.SubItems[3].Text = tboxCdshm.Text;

            saveDictInfo(@".\pali-h\newdict");

            MessageBox.Show(@"修改后的词库信息文件dicinfo已保存在本程序的.\pali-h\newdict\目录下！");
        }

        /// <summary>
        /// 保存词典信息
        /// </summary>
        private void saveDictInfo(string directorypath)
        {
            Directory.CreateDirectory(directorypath);
            StreamWriter sw = new StreamWriter(directorypath + @"\dicinfo", false, System.Text.Encoding.GetEncoding(65001));
            sw.WriteLine(sWordNum);
            sw.WriteLine(sDictNum);

            foreach (ListViewItem it in lvCdxx.Items)
                sw.WriteLine(it.SubItems[0].Text + it.SubItems[1].Text + "%" + it.SubItems[2].Text.PadRight(25, ' ') + "%" + it.SubItems[3].Text);

            sw.Close();
        }

        //从词库中导出所选词典
        private void button2_Click(object sender, EventArgs e)
        {
            Directory.CreateDirectory(@".\pali-h\newdict");
            StreamWriter sw = new StreamWriter(@".\pali-h\newdict\" + lvCdxx.FocusedItem.SubItems[2].Text.Trim() + "dict.txt", false, System.Text.Encoding.GetEncoding(65001));
            StreamWriter sw2 = new StreamWriter(@".\pali-h\newdict\" + lvCdxx.FocusedItem.SubItems[2].Text.Trim() + "index.txt", false, System.Text.Encoding.GetEncoding(65001));
            sw.WriteLine(lvCdxx.FocusedItem.Text.Trim());
            sw.WriteLine(lvCdxx.FocusedItem.SubItems[1].Text.Trim());
            sw.WriteLine(lvCdxx.FocusedItem.SubItems[2].Text.Trim());
            sw.WriteLine(lvCdxx.FocusedItem.SubItems[3].Text.Trim());

            for (int i = 0; i < Program.NUM; i++)
            {
                if (Program.strL[i].Substring(0, 1) == lvCdxx.FocusedItem.SubItems[1].Text.Trim())
                {
                    sw.WriteLine(Program.strL[i].Substring(2));
                    sw2.WriteLine(Program.sL[i]);
                }
            }

            sw.Close();
            sw2.Close();

            MessageBox.Show(@"所选词典已保存在本程序的.\pali-h\newdict\目录下！即文件名以所选词典名称开头的dict与index两个文本文件");
        }

        //旧 导出（从中删除所选词典）为管理用词库
        private void button3_Click(object sender, EventArgs e)
        {
            int n = 0;

            Directory.CreateDirectory(@".\pali-h\newdict");
            StreamWriter sw = new StreamWriter(@".\pali-h\newdict\cidian", false, System.Text.Encoding.GetEncoding(65001));
            StreamWriter sw2 = new StreamWriter(@".\pali-h\newdict\index", false, System.Text.Encoding.GetEncoding(65001));

            for (int i = 0; i < Program.NUM; i++)
            {
                if (Program.strL[i].Substring(0, 1) != lvCdxx.FocusedItem.SubItems[1].Text.Trim())
                {
                    sw.WriteLine(Program.strL[i]);
                    sw2.WriteLine(Program.sL[i]);
                    n++;
                }
            }

            sw.Close();
            sw2.Close();

            lvCdxx.FocusedItem.Remove();

            sWordNum = n.ToString();
            sDictNum = lvCdxx.Items.Count.ToString();

            saveDictInfo(@".\pali-h\newdict");

            MessageBox.Show("新生成的管理用词库（其中已删除刚刚所选的词典）已保存在本程序的\\pali-h\\newdict目录下，\r\n即文件名为cidian与index的两个文件，您可以再对其进行序列化。");
        }

        //导出全部词汇为词库源档
        private void button4_Click(object sender, EventArgs e)
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            int n = 0;

            Directory.CreateDirectory(@".\pali-h\词库源档\pali-h");
            StreamWriter sw = new StreamWriter(@".\pali-h\词库源档\pali-h\cidian", false, System.Text.Encoding.GetEncoding(65001));
            StreamWriter sw2 = new StreamWriter(@".\pali-h\词库源档\pali-h\index", false, System.Text.Encoding.GetEncoding(65001));

            for (int i = 0; i < Program.NUM; i++)
            {
                for (int z = 0; z < lvCdxx.Items.Count; z++)
                {
                    if (Program.strL[i].Substring(0, 1) == lvCdxx.Items[z].SubItems[1].Text.Trim())
                    {
                        sw.WriteLine(Program.strL[i]);
                        sw2.WriteLine(Program.sL[i]);
                        n++;
                        break;
                    }
                }
            }

            sw.Close();
            sw2.Close();

            //lvCdxx.FocusedItem.Remove();

            sWordNum = n.ToString();
            sDictNum = lvCdxx.Items.Count.ToString();

            saveDictInfo(@".\pali-h\词库源档\pali-h");



            if (File.Exists(@".\pali-h\词库源档\pali-h\cidian"))
            {
                StreamReader srPLDic = new StreamReader(new FileStream(@".\pali-h\词库源档\pali-h\dicinfo", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
                Program.pl_NUM = Convert.ToInt32(srPLDic.ReadLine());
                srPLDic.Close();
                Program.pl_sL = new string[Program.pl_NUM];
                Program.pl_strL = new string[Program.pl_NUM];
                Program.mainform.pl_readdc(@".\pali-h\词库源档\pali-h\cidian", @".\pali-h\词库源档\pali-h\index");
            }

            if (!(Program.mainform.menuosEnglishPali.Checked))
            {
                Program.NUM = Program.pl_NUM;
                Program.sL = Program.pl_sL;
                Program.strL = Program.pl_strL;
            }



            // Get serializer
            IFormatter serializer = new BinaryFormatter();

            // Serialize
            FileStream saveFile1 = new FileStream(@".\pali-h\词库源档\pali-h\cidian.dat", FileMode.Create, FileAccess.Write);
            serializer.Serialize(saveFile1, Program.pl_strL);
            saveFile1.Close();

            FileStream saveFile2 = new FileStream(@".\pali-h\词库源档\pali-h\index.dat", FileMode.Create, FileAccess.Write);
            serializer.Serialize(saveFile2, Program.pl_sL);
            saveFile2.Close();

            this.Cursor = System.Windows.Forms.Cursors.Default;

            MessageBox.Show("生成的新的词库已保存在本程序的\\pali-h\\词库源档\\pali-h目录下，\r\n即文件名为cidian与index的两个文件及dicinfo文件。");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            lvCdxx.FocusedItem.Remove();
            //MessageBox.Show("已删除");
            //MessageBox.Show(lvCdxx.Items[1].SubItems[3].Text.Trim());
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            int i = 0;
            i = lvCdxx.FocusedItem.Index;
            if (i == 0)
            {
                lvCdxx.Focus();
                return;
            }
            lvCdxx.Items[i - 1].Focused = true;
            lvCdxx.Items[i - 1].Selected = true;
            lvCdxx.Focus();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            int i = 0;
            i = lvCdxx.FocusedItem.Index;
            if (i == lvCdxx.Items.Count-1)
            {
                lvCdxx.Focus();
                return;
            }
            lvCdxx.Items[i + 1].Focused = true;
            lvCdxx.Items[i + 1].Selected = true;
            lvCdxx.Focus();
        }
    }
}
