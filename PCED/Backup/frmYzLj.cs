using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace pced
{
    public partial class frmYzLj : Form
    {
        public frmYzLj()
        {
            InitializeComponent();
        }

        private void frmYzLj_Load(object sender, EventArgs e)
        {
            loadMulu("1");
        }

        public void loadMulu(string yybz)
        {
            IFormatter serializer = new BinaryFormatter();

            StreamReader sr = new StreamReader(new FileStream(@".\mulu\count", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            int iMuluCount = Convert.ToInt32(sr.ReadLine());
            sr.Close();

            treeView1.Nodes.Clear();

            for (int q = 0; q < iMuluCount; q++)
            {
                FileStream loadFile = new FileStream(@".\mulu\tr" + yybz + q.ToString() + ".dat", FileMode.Open, FileAccess.Read);
                treeView1.Nodes.Add(serializer.Deserialize(loadFile) as TreeNode);
                loadFile.Close();
            }

            foreach (TreeNode tr in treeView1.Nodes)
            {
                tr.ToolTipText = ((tvtag)(tr.Tag)).stooltip;

                //tr.ToolTipText = tr.Tag.ToString().Substring(1);
                //tr.Tag = tr.Tag.ToString().Substring(0, 1);
            }
        }

        private void treeView1_AfterExpand(object sender, TreeViewEventArgs e)
        {
            foreach (TreeNode tr in e.Node.Nodes)
            {
                if (tr.ToolTipText == "")
                {
                    tr.ToolTipText = ((tvtag)(tr.Tag)).stooltip;
                }
                if (((tvtag)(tr.Tag)).itag == 2)
                {
                    tr.BackColor = Color.Teal;
                }
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //cboxTag.Text = Enum.GetName(typeof(muluTag), ((tvtag)(e.Node.Tag)).itag);

            //取得当前节点的根节点，以确定是根本、义注、复注或其它
            TreeNode tr = e.Node;
            while (tr.Level > 0)
                tr = tr.Parent;

            //如果标识是2 表示这是一本书 一个单独的htm档
            if (((tvtag)(e.Node.Tag)).itag == 2)
            {
                if (tr.Index == 0)
                {
                    Program.mainform.frmmuluwindow.tboxmula.Text = e.Node.Name;
                    Program.mainform.frmmuluwindow.tboxmulaName.Text = e.Node.Text;
                    Program.mainform.frmmuluwindow.tboxmulaNameC.Text = e.Node.ToolTipText;
                }
                if (tr.Index == 1)
                {
                    Program.mainform.frmmuluwindow.tboxattha.Text = e.Node.Name;
                    Program.mainform.frmmuluwindow.tboxatthaName.Text = e.Node.Text;
                    Program.mainform.frmmuluwindow.tboxatthaNameC.Text = e.Node.ToolTipText;
                }
                if (tr.Index == 2)
                {
                    Program.mainform.frmmuluwindow.tboxtika.Text = e.Node.Name;
                    Program.mainform.frmmuluwindow.tboxtikaName.Text = e.Node.Text;
                    Program.mainform.frmmuluwindow.tboxtikaNameC.Text = e.Node.ToolTipText;
                }

                //打开这本书
                string bookpath = @".\pali\" + e.Node.Name + ".htm";
                StreamReader sr = new StreamReader(new FileStream(bookpath, FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
                string strSZ = sr.ReadToEnd();
                sr.Close();
                webBrowser1.DocumentText = strSZ;
            }
            else
            {
                webBrowser1.DocumentText = "这是一个目录或书中的一篇！不是经文htm库中一个单独的htm文档。";
            }
        }
    }
}
