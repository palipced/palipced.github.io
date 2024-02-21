//制作词典数据库
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;

namespace makedict
{
    class Program
    {
        static void Main(string[] args)
        {
            addCd();

            Console.ReadKey();
        }

        /// <summary>
        /// 值为 0 表示增加巴利语词典，值为 1 表示增加英语词典
        /// </summary>
        public static int iPali = 0;

        public static void save_dc_to_file()
        {
            string dc_en;

            char[] ca = "āīūṅñṭḍṇḷŋĀĪŪṄÑṬḌṆḶŊṁṃṀṂ".ToCharArray();
            char[] cb = "aiunntdnlmAIUNNTDNLMmmMM".ToCharArray();
            int j = 0;
            int i = 0;

            Directory.CreateDirectory(@".\newdict\dc");

            StreamReader sr = new StreamReader(new FileStream(@".\newdict\cd.txt", FileMode.Open), System.Text.Encoding.GetEncoding(65001));
            string strLine = sr.ReadLine();
            //dc_en = strLine.Substring(7);
            while (strLine != null)
            {
                MatchCollection mc = new Regex(@"^\w%(?<w>[^:,]+)[:,].*", RegexOptions.None).Matches(strLine);
                if (mc.Count == 1)
                {
                    foreach (Match ma in mc)
                    {
                        dc_en = ma.Groups["w"].Value;


                        i = 0;
                        foreach (char c in ca)
                        {
                            dc_en = new Regex(c.ToString(), RegexOptions.None).Replace(dc_en, cb[i].ToString());
                            i++;
                        }

                        StreamWriter sw = new StreamWriter(@".\newdict\dc\" + dc_en + ".txt", true, System.Text.Encoding.GetEncoding(65001));
                        sw.WriteLine(strLine);
                        sw.Close();

                        Console.WriteLine(j);
                        j++;
                        //if (1000 == j)
                        //{
                        //    sr.Close();
                        //    return;
                        //}


                    }
                }

                strLine = sr.ReadLine();
                //dc_en = strLine.Substring(7);
            }
            sr.Close();
        }

        public static void getDc()
        {
            StreamWriter sw = new StreamWriter(@".\newdict\cd.txt", false, System.Text.Encoding.GetEncoding(65001));

            Console.WriteLine("GetFiles,please wait...");

            string[] filelist = Directory.GetFiles(@".\newdict\dc\", "*.*", System.IO.SearchOption.TopDirectoryOnly);
            foreach (string s in filelist)
            {
                StreamReader sr = new StreamReader(new FileStream(s, FileMode.Open), System.Text.Encoding.GetEncoding(65001));
                string dc = sr.ReadLine();
                while (dc != null)
                {
                    sw.WriteLine(dc);
                    Console.WriteLine(dc);

                    dc = sr.ReadLine();
                }
                sr.Close();

                File.Delete(s);
            }

            sw.Close();
        }

        /// <summary>
        /// 增加词典
        /// </summary>
        public static void addCd()
        {
            string sInput = "";
            Console.WriteLine("PCED词库制作工具-新版-改进排序-测试版2011-10-05");
            Console.WriteLine(" ");
            Console.WriteLine("增加巴利语词典请按 0 ，增加英语词典请按 1 ，然后按回车键：");
            Console.WriteLine(" ");
        inp:
            sInput = Console.ReadLine().Trim();
            if (sInput == "" || (sInput != "0" && sInput != "1"))
            {
                Console.WriteLine("输入错误！请重新输入：");
                goto inp;
            }
            iPali = Convert.ToInt32(sInput);

            Console.WriteLine(" ");
            Console.WriteLine("正在进行...");
            Console.WriteLine(" ");

            File.Copy(@".\dict\cidian", @".\dict\cidian_t");
            File.Copy(@".\dict\dicinfo", @".\dict\dicinfo_t");
            string[] dictfiles = Directory.GetFiles(@".\dict\dictadd\", "*.txt", System.IO.SearchOption.TopDirectoryOnly);
            foreach (string dictfile in dictfiles)
            {
                makedict_new(dictfile);
            }
            //makedict();

            Console.WriteLine("使用新排序技术(测试,必须在win系统ntfs分区磁盘上进行)请按 0 ，使用旧排序技术请按 1 ，然后按回车键：");
        inpv:
            string szero = Console.ReadLine().Trim();
            if (szero == "" || (szero != "0" && szero != "1"))
            {
                Console.WriteLine("输入错误！请重新输入：");
                goto inpv;
            }
            if (szero == "0")
            {
                save_dc_to_file();
                getDc();
            }

            makeinx();

            Console.WriteLine(@"您现在可以改变词库中各词典的排列顺序，");
            Console.WriteLine(@"现在可以用记事本打开本程序目录下 \newdict\ 子目录下的 dicinfo 文件，");
            Console.WriteLine(@"可以在此 dicinfo 文件中整行整行的调整改变词典行顺序，然后保存 dicinfo 文件，");
            Console.WriteLine(" ");
            Console.WriteLine(@"（注：dicinfo 文件中每一行首字母是词典分组标志，                       ）");
            Console.WriteLine(@"（    在调整词典行顺序时只应在首字母相同的同组行之间进行调整。         ）");
            Console.WriteLine(@"（    并且行与行之间不要留空行。                                       ）");
            Console.WriteLine(@"（    而且每一行的两个 % 号 之间包含空格的总字符数应为25个字符，       ）");
            Console.WriteLine(@"（    如需修改名称，需保持两个 % 号 之间包含空格的总字符数应为25 不变。）");
            Console.WriteLine(" ");
            Console.WriteLine(@"然后 在此 按 回车键 继续！");
            Console.WriteLine(@"假如您不想改变词典顺序，那么不需打开 dicinfo 文件，在此直接 按 回车键 继续即可！");
            Console.ReadLine();

            cdpx();

            cdpx_pali();

            cdabc_read();
            cdpx_dic();

            Console.WriteLine("process complete.");
        }

        public static void makedict_new(string dictfile)
        {
            int iDcNum = 0; //新词库词条数量

            string dictfilename = dictfile.Substring(15, dictfile.Length - 15 - 4);

            if (File.Exists(@".\newdict\cd.txt"))
                File.Copy(@".\newdict\cd.txt", @".\dict\cidian_t");

            if (File.Exists(@".\newdict\dicinfo"))
                File.Copy(@".\newdict\dicinfo", @".\dict\dicinfo_t");

            StreamReader sr = new StreamReader(new FileStream(dictfile, FileMode.Open), System.Text.Encoding.GetEncoding(65001));
            StreamWriter sw = new StreamWriter(@".\dict\" + dictfilename + @"dict_error.txt", false, System.Text.Encoding.GetEncoding(65001));
            StreamReader sr1 = new StreamReader(new FileStream(@".\dict\cidian_t", FileMode.Open), System.Text.Encoding.GetEncoding(65001));
            StreamWriter sw1 = new StreamWriter(@".\newdict\cd.txt", false, System.Text.Encoding.GetEncoding(65001));

            string strCdlxBz = sr.ReadLine(); //词典类型标志
            string strCdBz = sr.ReadLine(); //词典标志
            string strCdjc = sr.ReadLine(); //词典简称
            string strCdqc = sr.ReadLine(); //词典全称

            string strLine1 = sr1.ReadLine();
            while (strLine1 != null)
            {
                sw1.WriteLine(strLine1);
                iDcNum++;
                strLine1 = sr1.ReadLine();
            }
            sr1.Close();

            string strReg;
            if (iPali == 0)
                strReg = "[^ABCDEGHIJKLMNOPRSTUVYĀĪŪṄÑṬḌṆḶṂṀŊabcdeghijklmnoprstuvyāīūṅñṭḍṇḷṃṁŋ° ’'…-]";
            else
                strReg = "[^ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz '-]";

            string strLine = sr.ReadLine();
            while (strLine != null)
            {
                MatchCollection mc = new Regex(@"^(?<w>[^:,]+)[:,].*", RegexOptions.None).Matches(strLine);
                if (mc.Count > 0)
                {
                    foreach (Match ma in mc)
                    {
                        MatchCollection mc1 = new Regex(strReg, RegexOptions.None).Matches(ma.Groups["w"].Value);
                        if (mc1.Count > 0)
                            sw.WriteLine(strLine);
                        else
                        {
                            sw1.WriteLine(strCdBz + "%" + strLine);
                            iDcNum++;
                        }
                    }
                }
                else
                {
                    sw.WriteLine(strLine);
                }
                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
            sw1.Close();

            bool bCdcf = false; //值为true表示新词典标志重复，此标志以前已经使用过，说明此次新增加的此条属于旧有的词典，因此不更新词典说明信息

            int iCdNum = 0; //词库词典数量

            StreamReader srf1 = new StreamReader(new FileStream(@".\dict\dicinfo_t", FileMode.Open), System.Text.Encoding.GetEncoding(65001));
            string strLinef1 = srf1.ReadLine();
            strLinef1 = srf1.ReadLine();
            strLinef1 = srf1.ReadLine();
            while (strLinef1 != null)
            {
                if (strLinef1.Substring(1, 1) == strCdBz)
                {
                    bCdcf = true;
                }
                iCdNum++;
                strLinef1 = srf1.ReadLine();
            }
            srf1.Close();

            if (!bCdcf)
                iCdNum++;

            StreamReader srf = new StreamReader(new FileStream(@".\dict\dicinfo_t", FileMode.Open), System.Text.Encoding.GetEncoding(65001));
            StreamWriter swf = new StreamWriter(@".\newdict\dicinfo", false, System.Text.Encoding.GetEncoding(65001));

            string strLinef = srf.ReadLine();
            swf.WriteLine(iDcNum);

            strLinef = srf.ReadLine();
            swf.WriteLine(iCdNum);

            strLinef = srf.ReadLine();
            while (strLinef != null)
            {
                swf.WriteLine(strLinef);
                strLinef = srf.ReadLine();
            }
            srf.Close();

            if (!bCdcf)
                swf.WriteLine(strCdlxBz + strCdBz + "%" + strCdjc.PadRight(25, ' ') + "%" + strCdqc);

            swf.Close();

            File.Delete(@".\dict\cidian_t");
            File.Delete(@".\dict\dicinfo_t");
        }

        public static void makedict()
        {
            int iDcNum = 0; //新词库词条数量

            StreamReader sr = new StreamReader(new FileStream(@".\dict\dict.txt", FileMode.Open), System.Text.Encoding.GetEncoding(65001));
            StreamWriter sw = new StreamWriter(@".\dict\dict_error.txt", false, System.Text.Encoding.GetEncoding(65001));
            StreamReader sr1 = new StreamReader(new FileStream(@".\dict\cidian", FileMode.Open), System.Text.Encoding.GetEncoding(65001));
            StreamWriter sw1 = new StreamWriter(@".\newdict\cd.txt", false, System.Text.Encoding.GetEncoding(65001));

            string strCdlxBz = sr.ReadLine(); //词典类型标志
            string strCdBz = sr.ReadLine(); //词典标志
            string strCdjc = sr.ReadLine(); //词典简称
            string strCdqc = sr.ReadLine(); //词典全称

            string strLine1 = sr1.ReadLine();
            while (strLine1 != null)
            {
                sw1.WriteLine(strLine1);
                iDcNum++;
                strLine1 = sr1.ReadLine();
            }
            sr1.Close();

            string strReg;
            if (iPali == 0)
                strReg = "[^ABCDEGHIJKLMNOPRSTUVYĀĪŪṄÑṬḌṆḶṂṀŊabcdeghijklmnoprstuvyāīūṅñṭḍṇḷṃṁŋ° ’'…-]";
            else
                strReg = "[^ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz '-]";

            string strLine = sr.ReadLine();
            while (strLine != null)
            {
                MatchCollection mc = new Regex(@"^(?<w>[^:,]+)[:,].*", RegexOptions.None).Matches(strLine);
                if (mc.Count > 0)
                {
                    foreach (Match ma in mc)
                    {
                        MatchCollection mc1 = new Regex(strReg, RegexOptions.None).Matches(ma.Groups["w"].Value);
                        if (mc1.Count > 0)
                            sw.WriteLine(strLine);
                        else
                        {
                            sw1.WriteLine(strCdBz + "%" + strLine);
                            iDcNum++;
                        }
                    }
                }
                else
                {
                    sw.WriteLine(strLine);
                }
                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
            sw1.Close();

            bool bCdcf = false; //值为true表示新词典标志重复，此标志以前已经使用过，说明此次新增加的此条属于旧有的词典，因此不更新词典说明信息

            int iCdNum = 0; //词库词典数量

            StreamReader srf1 = new StreamReader(new FileStream(@".\dict\dicinfo", FileMode.Open), System.Text.Encoding.GetEncoding(65001));
            string strLinef1 = srf1.ReadLine();
            strLinef1 = srf1.ReadLine();
            strLinef1 = srf1.ReadLine();
            while (strLinef1 != null)
            {
                if (strLinef1.Substring(1, 1) == strCdBz)
                {
                    bCdcf = true;
                }
                iCdNum++;
                strLinef1 = srf1.ReadLine();
            }
            srf1.Close();

            if (!bCdcf)
                iCdNum++;

            StreamReader srf = new StreamReader(new FileStream(@".\dict\dicinfo", FileMode.Open), System.Text.Encoding.GetEncoding(65001));
            StreamWriter swf = new StreamWriter(@".\newdict\dicinfo", false, System.Text.Encoding.GetEncoding(65001));

            string strLinef = srf.ReadLine();
            swf.WriteLine(iDcNum);

            strLinef = srf.ReadLine();
            swf.WriteLine(iCdNum);

            strLinef = srf.ReadLine();
            while (strLinef != null)
            {
                swf.WriteLine(strLinef);
                strLinef = srf.ReadLine();
            }
            srf.Close();

            if (!bCdcf)
                swf.WriteLine(strCdlxBz + strCdBz + "%" + strCdjc.PadRight(25, ' ') + "%" + strCdqc);

            swf.Close();
        }

        public static void makeinx()
        {
            StreamReader sr = new StreamReader(new FileStream(@".\newdict\cd.txt", FileMode.Open), System.Text.Encoding.GetEncoding(65001));
            StreamWriter sw = new StreamWriter(@".\newdict\inx.txt", false, System.Text.Encoding.GetEncoding(65001));
            string strLine = sr.ReadLine();
            while (strLine != null)
            {
                MatchCollection mc = new Regex(@"^\w%(?<w>[^:,]+)[:,].*", RegexOptions.None).Matches(strLine);
                foreach (Match ma in mc)
                {
                    sw.WriteLine(ma.Groups["w"].Value);
                }
                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        //词典和索引按英文化模式排序
        public static void cdpx()
        {
            StreamReader srf = new StreamReader(new FileStream(@".\newdict\dicinfo", FileMode.Open), System.Text.Encoding.GetEncoding(65001));
            string strLinef = srf.ReadLine();
            NUM = Convert.ToInt32(strLinef);
            srf.Close();

            sL = new string[NUM];
            strL = new string[NUM];

            readdc(@".\newdict\cd.txt", @".\newdict\inx.txt");

            for (int i = 0; i < NUM; ++i)
            {
                string tempd = sL[i];
                string temps = strL[i];
                int jj = i;
                while ((jj > 0) && eab(tempd, sL[jj - 1]))
                {
                    sL[jj] = sL[jj - 1];
                    strL[jj] = strL[jj - 1];
                    --jj;
                }

                sL[jj] = tempd;
                strL[jj] = temps;

                Console.WriteLine(i);
            }

            StreamWriter sw1 = new StreamWriter(@".\newdict\index", false, System.Text.Encoding.GetEncoding(65001));
            StreamWriter sw2 = new StreamWriter(@".\newdict\cidian", false, System.Text.Encoding.GetEncoding(65001));
            for (int k = 0; k < NUM; k++)
            {
                sw1.WriteLine(sL[k]);
                sw2.WriteLine(strL[k]);
            }
            sw1.Close();
            sw2.Close();

            File.Delete(@".\newdict\cd.txt");
            File.Delete(@".\newdict\inx.txt");
        }

        public static void readdc(string cidianpath, string indexpath)
        {
            string strLine;
            int i = 0, j = 0;

            StreamReader sr = new StreamReader(new FileStream(cidianpath, FileMode.Open), System.Text.Encoding.GetEncoding(65001));
            strLine = sr.ReadLine();
            while (strLine != null)
            {
                strL[i] = strLine;
                i++;
                strLine = sr.ReadLine();
            }
            sr.Close();

            StreamReader sr1 = new StreamReader(new FileStream(indexpath, FileMode.Open), System.Text.Encoding.GetEncoding(65001));
            strLine = sr1.ReadLine();
            while (strLine != null)
            {
                sL[j] = strLine;
                j++;
                strLine = sr1.ReadLine();
            }
            sr1.Close();
        }

        //对英文模式排序相等的词再按巴利文排序
        public static void cdpx_pali()
        {
            File.Copy(@".\newdict\cidian", @".\newdict\cd.txt");
            File.Copy(@".\newdict\index", @".\newdict\inx.txt");
            File.Delete(@".\newdict\cidian");
            File.Delete(@".\newdict\index");

            StreamReader srf = new StreamReader(new FileStream(@".\newdict\dicinfo", FileMode.Open), System.Text.Encoding.GetEncoding(65001));
            string strLinef = srf.ReadLine();
            NUM = Convert.ToInt32(strLinef);
            srf.Close();

            sL = new string[NUM];
            strL = new string[NUM];

            readdc(@".\newdict\cd.txt", @".\newdict\inx.txt");

            string tempd;
            string temps;

            for (int i = 0; i < NUM; ++i)
            {
                tempd = sL[i];
                temps = strL[i];
                int jj = i;

                //对英文模式排序相等的词再按巴利文排序
                while ((jj > 0) && !(eab(tempd, sL[jj - 1])) && !(eab(sL[jj - 1], tempd)) && ab(tempd, sL[jj - 1]))
                {
                    sL[jj] = sL[jj - 1];
                    strL[jj] = strL[jj - 1];
                    --jj;
                }

                sL[jj] = tempd;
                strL[jj] = temps;

                Console.WriteLine(i);
            }

            StreamWriter sw1 = new StreamWriter(@".\newdict\index", false, System.Text.Encoding.GetEncoding(65001));
            StreamWriter sw2 = new StreamWriter(@".\newdict\cidian", false, System.Text.Encoding.GetEncoding(65001));
            for (int k = 0; k < NUM; k++)
            {
                sw1.WriteLine(sL[k]);
                sw2.WriteLine(strL[k]);
            }
            sw1.Close();
            sw2.Close();

            File.Delete(@".\newdict\cd.txt");
            File.Delete(@".\newdict\inx.txt");
        }

        //对以cdpx_pali()函数排好序的词，再以dicinfo文本文件里词典的先后顺序排序
        public static void cdpx_dic()
        {
            File.Copy(@".\newdict\cidian", @".\newdict\cd.txt");
            File.Copy(@".\newdict\index", @".\newdict\inx.txt");
            File.Delete(@".\newdict\cidian");
            File.Delete(@".\newdict\index");

            StreamReader srf = new StreamReader(new FileStream(@".\newdict\dicinfo", FileMode.Open), System.Text.Encoding.GetEncoding(65001));
            string strLinef = srf.ReadLine();
            NUM = Convert.ToInt32(strLinef);
            srf.Close();

            sL = new string[NUM];
            strL = new string[NUM];

            readdc(@".\newdict\cd.txt", @".\newdict\inx.txt");

            string tempd;
            string temps;

            for (int i = 0; i < NUM; ++i)
            {
                tempd = sL[i];
                temps = strL[i];
                int jj = i;

                //对以cdpx_pali()函数排好序的词，再以dicinfo文本文件里词典的先后顺序排序
                while ((jj > 0) && !(ab(tempd, sL[jj - 1])) && !(ab(sL[jj - 1], tempd)) && cdab(temps.Substring(0, 1), strL[jj - 1].Substring(0, 1)))
                {
                    sL[jj] = sL[jj - 1];
                    strL[jj] = strL[jj - 1];
                    --jj;
                }

                sL[jj] = tempd;
                strL[jj] = temps;

                Console.WriteLine(i);
            }

            StreamWriter sw1 = new StreamWriter(@".\newdict\index", false, System.Text.Encoding.GetEncoding(65001));
            StreamWriter sw2 = new StreamWriter(@".\newdict\cidian", false, System.Text.Encoding.GetEncoding(65001));
            for (int k = 0; k < NUM; k++)
            {
                sw1.WriteLine(sL[k]);
                sw2.WriteLine(strL[k]);
            }
            sw1.Close();
            sw2.Close();

            File.Delete(@".\newdict\cd.txt");
            File.Delete(@".\newdict\inx.txt");
        }

        /// <summary>
        /// 储存词典标志字母
        /// </summary>
        public static string s_cdabc = "";

        /// <summary>
        /// 读入dicinfo文本文件中词典标志字母
        /// </summary>
        public static void cdabc_read()
        {
            s_cdabc = "";

            StreamReader sr = new StreamReader(new FileStream(@".\newdict\dicinfo", FileMode.Open), System.Text.Encoding.GetEncoding(65001));
            string strline = sr.ReadLine();
            strline = sr.ReadLine();
            strline = sr.ReadLine();
            while (strline != null)
            {
                s_cdabc = s_cdabc + strline.Substring(1, 1);
                strline = sr.ReadLine();
            }
            sr.Close();
        }

        /// <summary>
        /// 按dicinfo文本文件中词典先后顺序比较
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool cdab(string a, string b)
        {
            for (int j = 0; j < s_cdabc.Length; j++)
            {
                if (a[0] == s_cdabc[j])
                    return true;

                if (b[0] == s_cdabc[j])
                    return false;
            }
            return true;
        }

        //从文本取得所有用到的字母
        public static void getabc()
        {
            string strLine = "";
            StreamReader sr = new StreamReader(new FileStream(@".\index", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@".\abc.txt", false, System.Text.Encoding.GetEncoding("utf-8"));

            int m = 0, p = 0, q = 0;
            string[] VriAbc = new string[200];
            string sT = "";

            strLine = sr.ReadLine();
            while (strLine != null)
            {
                for (int j = 0; j < strLine.Length; j++)
                {
                    sT = strLine.Substring(j, 1);
                    for (int n = 0; n < 200; n++)
                    {
                        if (VriAbc[n] == sT)
                            goto next1;
                    }
                    VriAbc[m] = sT;
                    m++;
                next1:
                    ;
                }
                p++;
                if (p == q + 10000)
                {
                    q = 10000 + q;
                    Console.WriteLine(p);
                }

                strLine = sr.ReadLine();
            }
            sr.Close();

            for (int k = 0; k < 200; k++)
                sw.Write(VriAbc[k]);

            sw.Close();

            Console.WriteLine(m);
        }

        //删除 F 词典中纯英文解释之词条
        public static void delFcheng()
        {
            StreamReader sr = new StreamReader(new FileStream(@".\pali-h\cidian1", FileMode.Open), System.Text.Encoding.GetEncoding(65001));
            StreamReader sr1 = new StreamReader(new FileStream(@".\pali-h\index1", FileMode.Open), System.Text.Encoding.GetEncoding(65001));
            StreamWriter sw = new StreamWriter(@".\pali-h\cidian", false, System.Text.Encoding.GetEncoding(65001));
            StreamWriter sw1 = new StreamWriter(@".\pali-h\index", false, System.Text.Encoding.GetEncoding(65001));
            string strLine = sr.ReadLine();
            string strLine1 = sr1.ReadLine();
            while (strLine != null)
            {
                if (strLine.Substring(0, 1) == "F")
                {
                    MatchCollection mc = new Regex(@"[\u4e00-\u9fa5]", RegexOptions.None).Matches(strLine);
                    if (mc.Count > 0)
                    {
                        sw.WriteLine(strLine);
                        sw1.WriteLine(strLine1);
                    }
                }
                else
                {
                    sw.WriteLine(strLine);
                    sw1.WriteLine(strLine1);
                }
                strLine = sr.ReadLine();
                strLine1 = sr1.ReadLine();
            }
            sr.Close();
            sr1.Close();
            sw.Close();
            sw1.Close();
        }

        public static void ped0()
        {
            StreamReader sr = new StreamReader(new FileStream(@".\ped0.txt", FileMode.Open), System.Text.Encoding.GetEncoding(65001));
            StreamWriter sw = new StreamWriter(@".\ped1.txt", false, System.Text.Encoding.GetEncoding(65001));
            string strLine = sr.ReadToEnd();
            //while (strLine != null)
            //{
            string ptn = @"^(?<w>↓Page.{0,20})";
            MatchCollection mc = new Regex(ptn, RegexOptions.Multiline).Matches(strLine);

            foreach (Match ma in mc)
            {
                sw.WriteLine(ma.Groups["w"].Value);
            }
            //strLine = sr.ReadLine();
            //}
            sr.Close();
            sw.Close();
        }

        public static void ped1()
        {
            StreamReader sr = new StreamReader(new FileStream(@".\ped0.txt", FileMode.Open), System.Text.Encoding.GetEncoding(65001));
            StreamWriter sw = new StreamWriter(@".\ped1.txt", false, System.Text.Encoding.GetEncoding(65001));
            string strLine = sr.ReadLine();
            while (strLine != null)
            {
                if (strLine.Trim() != "")
                    sw.WriteLine(strLine);
                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        public static void ped2()
        {
            StreamReader sr = new StreamReader(new FileStream(@".\ped1.txt", FileMode.Open), System.Text.Encoding.GetEncoding(65001));
            StreamWriter sw = new StreamWriter(@".\ped2.txt", false, System.Text.Encoding.GetEncoding(65001));
            string strLine = sr.ReadLine();
            int n = 0;
            string spage = "";
            while (strLine != null)
            {
                MatchCollection mc = new Regex(@"^(?<w>↓Page\s\d{1,3})$", RegexOptions.None).Matches(strLine);
                if (mc.Count > 0)
                {
                    ////string ptn = @"(?<w>\<[^/<>br]*\>[^<>]*)$";
                    //string ptn = @"(?<w>\<[^>]*)$";
                    //MatchCollection mc = new Regex(ptn, RegexOptions.None).Matches(strLine);

                    //foreach (Match ma in mc)
                    //{
                    //    sw.WriteLine(ma.Groups["w"].Value + n.ToString());
                    //}
                    //n++;
                    spage = strLine.PadRight(9, ' ');
                    //sw.WriteLine(strLine);
                }
                else
                {
                    MatchCollection mc1 = new Regex(@"^(?<w>\<a\>.*\</a\>)", RegexOptions.IgnoreCase).Matches(strLine);
                    if (mc1.Count > 0)
                        sw.WriteLine(spage + strLine);
                    else
                        sw.WriteLine("#" + strLine);
                }
                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        public static void ped3()
        {
            StreamReader sr = new StreamReader(new FileStream(@".\ped2.txt", FileMode.Open), System.Text.Encoding.GetEncoding(65001));
            StreamWriter sw = new StreamWriter(@".\ped3.txt", false, System.Text.Encoding.GetEncoding(65001));
            string strLine = sr.ReadToEnd();
            strLine = new Regex(@"\r\n#", RegexOptions.None).Replace(strLine, " ");
            sw.Write(strLine);
            sr.Close();
            sw.Close();
        }

        public static void ped4()
        {
            StreamReader sr = new StreamReader(new FileStream(@".\ped3.txt", FileMode.Open), System.Text.Encoding.GetEncoding(65001));
            StreamWriter sw = new StreamWriter(@".\ped4.txt", false, System.Text.Encoding.GetEncoding(65001));
            string strLine = sr.ReadLine();
            while (strLine != null)
            {
                MatchCollection mc = new Regex(@"^↓Page\s(?<w>\d{1,3})", RegexOptions.None).Matches(strLine);
                foreach (Match ma in mc)
                {
                    sw.WriteLine(strLine.Substring(9).Trim() + " (Page " + ma.Groups["w"].Value + ")");
                }
                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        public static void ped5()
        {
            string dcjs = "";
            StreamReader sr = new StreamReader(new FileStream(@".\ped4.txt", FileMode.Open), System.Text.Encoding.GetEncoding(65001));
            StreamWriter sw = new StreamWriter(@".\ped5.txt", false, System.Text.Encoding.GetEncoding(65001));
            string strLine = sr.ReadLine();
            while (strLine != null)
            {
                //以此匹配，缺3，4十个特殊情况的词匹配不到
                //MatchCollection mc = new Regex(@"^\<a\>(?<w>[ABCDEGHIJKLMNOPRSTUVYĀĪŪṄÑṬḌṆḶṀabcdeghijklmnoprstuvyāīūṅñṭḍṇḷṁ, ()&<>/\d=°-]*?)\<", RegexOptions.IgnoreCase).Matches(strLine);
                MatchCollection mc = new Regex(@"^\<a\>(?<w>[ABCDEGHIJKLMNOPRSTUVYĀĪŪṄÑṬḌṆḶṀabcdeghijklmnoprstuvyāīūṅñṭḍṇḷṁ°-]*?)(?<w1>[/\d><)(&, =].*)", RegexOptions.IgnoreCase).Matches(strLine);
                foreach (Match ma in mc)
                {
                    //这一段写词典
                    dcjs = ma.Groups["w1"].Value;
                    dcjs = new Regex(@"\<a\>", RegexOptions.IgnoreCase).Replace(dcjs, "");
                    dcjs = new Regex(@"\</a\>", RegexOptions.IgnoreCase).Replace(dcjs, "");
                    dcjs = new Regex(@"\<br\>", RegexOptions.IgnoreCase).Replace(dcjs, "<br><br>");
                    dcjs = dcjs.Trim();
                    if (dcjs.Substring(0, 1) == "," || dcjs.Substring(0, 1) == ":")
                        sw.WriteLine(ma.Groups["w"].Value.Trim() + dcjs);
                    else
                        sw.WriteLine(ma.Groups["w"].Value.Trim() + "," + dcjs);

                    //这一句写单词表
                    //sw.WriteLine(ma.Groups["w"].Value.Trim());
                }
                //写特殊情况的匹配不到的词
                //if (mc.Count==0)
                //sw.WriteLine(strLine);
                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        //从整理好的词库'代名词表.txt'里生成其单词列表
        public static void dmcb_dcb()
        {
            StreamReader sr = new StreamReader(new FileStream(@".\代名词表.txt", FileMode.Open), System.Text.Encoding.GetEncoding(65001));
            StreamWriter sw = new StreamWriter(@".\代名词表_dcb.txt", false, System.Text.Encoding.GetEncoding(65001));
            string strLine = sr.ReadLine();
            while (strLine != null)
            {
                MatchCollection mc = new Regex(@"^(?<w>[^,]+),.*", RegexOptions.None).Matches(strLine);
                foreach (Match ma in mc)
                {
                    sw.WriteLine(ma.Groups["w"].Value);
                }
                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        //ABCDEGHIJKLMNOPRSTUVYĀĪŪṄÑṬḌṆḶṂṀŊ
        //abcdeghijklmnoprstuvyāīūṅñṭḍṇḷṃṁŋ
        //\u4e00-\u9fa5
        //把页码行写为数字
        public static void phcd1()
        {
            StreamReader sr = new StreamReader(new FileStream(@".\pali-h\phcd.txt", FileMode.Open), System.Text.Encoding.GetEncoding(65001));
            StreamWriter sw = new StreamWriter(@".\pali-h\phcd1.txt", false, System.Text.Encoding.GetEncoding(65001));
            string strLine = sr.ReadLine();
            int n = 2;
            while (strLine != null)
            {
                string ptn = @"[ABCDEGHIJKLMNOPRSTUVYĀĪŪṄÑṬḌṆḶṂṀŊ][-ABCDEGHIJKLMNOPRSTUVYĀĪŪṄÑṬḌṆḶṂṀŊabcdeghijklmnoprstuvyāīūṅñṭḍṇḷṃṁŋ]+\s(?<w>\d+)\s[ABCDEGHIJKLMNOPRSTUVYĀĪŪṄÑṬḌṆḶṂṀŊ][-ABCDEGHIJKLMNOPRSTUVYĀĪŪṄÑṬḌṆḶṂṀŊabcdeghijklmnoprstuvyāīūṅñṭḍṇḷṃṁŋ]+";
                MatchCollection mc = new Regex(ptn, RegexOptions.None).Matches(strLine);
                if (mc.Count == 1)
                {
                    foreach (Match ma in mc)
                    {
                        n++;
                        sw.WriteLine(ma.Groups["w"].Value);
                    }
                }
                else
                {
                    sw.WriteLine(strLine);
                }
                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        //把第一个字符是汉字的行前的\r\n替换掉
        public static void phcd2()
        {
            StreamReader sr = new StreamReader(new FileStream(@".\pali-h\phcd1.txt", FileMode.Open), System.Text.Encoding.GetEncoding(65001));
            StreamWriter sw = new StreamWriter(@".\pali-h\phcd2.txt", false, System.Text.Encoding.GetEncoding(65001));
            string strLine = sr.ReadToEnd();
            string ptn = @"(?<w1>[^\d])\r\n(?<w2>[abcdeghijklmnoprstuvyāīūṅñṭḍṇḷṃṁŋ。【=“~（\(°\u4e00-\u9fa5])";
            strLine = new Regex(ptn, RegexOptions.None).Replace(strLine, "${w1}${w2}");
            sw.Write(strLine);
            sr.Close();
            sw.Close();
        }

        //把每一行的开始单词与解释之间的分隔符,号(号前面的空格替换消除掉
        public static void phcd3()
        {
            StreamReader sr = new StreamReader(new FileStream(@".\pali-h\phcd2.txt", FileMode.Open), System.Text.Encoding.GetEncoding(65001));
            StreamWriter sw = new StreamWriter(@".\pali-h\phcd3.txt", false, System.Text.Encoding.GetEncoding(65001));
            string strLine = sr.ReadLine();
            while (strLine != null)
            {
                string ptn = @"^(?<w1>[^,(]+)\s+(?<w2>[,(].*)";
                strLine = new Regex(ptn, RegexOptions.None).Replace(strLine, "${w1}${w2}");
                sw.WriteLine(strLine);
                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        //词典
        //先手工删除201-214之间的12页重复页
        //删除358个数字页码行
        //在每个词条末尾加注原pdf档页码
        //暂时删除内容包括'以上的'或'以下的'行，待以后再整理增加这些词条
        public static void phcd4()
        {
            StreamReader sr = new StreamReader(new FileStream(@".\pali-h\phcd3.txt", FileMode.Open), System.Text.Encoding.GetEncoding(65001));
            StreamWriter sw = new StreamWriter(@".\pali-h\phcd4.txt", false, System.Text.Encoding.GetEncoding(65001));
            string strLine = sr.ReadLine();
            int p = 1;
            while (strLine != null)
            {
                MatchCollection mc = new Regex(@"^\d+", RegexOptions.None).Matches(strLine);
                if (mc.Count == 1)
                {
                    foreach (Match ma in mc)
                    {
                        p = Convert.ToInt16(ma.Value);
                        //sw.WriteLine(ma.Value);
                    }
                }
                else
                {
                    MatchCollection mc1 = new Regex("以上的|以下的", RegexOptions.None).Matches(strLine);
                    if (mc1.Count == 0)
                    {
                        sw.WriteLine(strLine + "(p" + p.ToString() + ")");
                    }
                }
                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        //索引
        //取得单词列表
        public static void phcd5()
        {
            StreamReader sr = new StreamReader(new FileStream(@".\pali-h\phcd4.txt", FileMode.Open), System.Text.Encoding.GetEncoding(65001));
            StreamWriter sw = new StreamWriter(@".\pali-h\phcd5.txt", false, System.Text.Encoding.GetEncoding(65001));
            string strLine = sr.ReadLine();
            while (strLine != null)
            {
                string ptn = @"^(?<w>[^,(]+)[,(].*";
                MatchCollection mc = new Regex(ptn, RegexOptions.None).Matches(strLine);
                foreach (Match ma in mc)
                {
                    sw.WriteLine(ma.Groups["w"].Value);
                }
                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        //删除词典
        public static void delL()
        {
            StreamReader sr = new StreamReader(new FileStream(@".\pali-h\cidian1", FileMode.Open), System.Text.Encoding.GetEncoding(65001));
            StreamReader sr1 = new StreamReader(new FileStream(@".\pali-h\index1", FileMode.Open), System.Text.Encoding.GetEncoding(65001));
            StreamWriter sw = new StreamWriter(@".\pali-h\cidian", false, System.Text.Encoding.GetEncoding(65001));
            StreamWriter sw1 = new StreamWriter(@".\pali-h\index", false, System.Text.Encoding.GetEncoding(65001));
            string strLine = sr.ReadLine();
            string strLine1 = sr1.ReadLine();
            while (strLine != null)
            {
                if (strLine.Substring(0, 1) != "L")
                {
                    sw.WriteLine(strLine);
                    sw1.WriteLine(strLine1);
                }
                strLine = sr.ReadLine();
                strLine1 = sr1.ReadLine();
            }
            sr.Close();
            sr1.Close();
            sw.Close();
            sw1.Close();
        }

        //增加进词库
        public static void addDic()
        {
            StreamReader sr = new StreamReader(new FileStream(@".\pali-h\pedcidian.txt", FileMode.Open), System.Text.Encoding.GetEncoding(65001));
            StreamReader sr1 = new StreamReader(new FileStream(@".\pali-h\pedindex.txt", FileMode.Open), System.Text.Encoding.GetEncoding(65001));
            StreamWriter sw = new StreamWriter(@".\pali-h\cidian", true, System.Text.Encoding.GetEncoding(65001));
            StreamWriter sw1 = new StreamWriter(@".\pali-h\index", true, System.Text.Encoding.GetEncoding(65001));
            string strLine = sr.ReadLine();
            string strLine1 = sr1.ReadLine();
            while (strLine != null)
            {
                sw.WriteLine("P%" + strLine);

                char[] ca = "āīūṅñṭḍṇḷŋṁṃĀĪŪṄÑṬḌṆḶŊṀṂ".ToCharArray();
                char[] cb = "aiunntdnlmmmAIUNNTDNLMMM".ToCharArray();
                int i = 0;
                foreach (char c in ca)
                {
                    strLine1 = new Regex(c.ToString(), RegexOptions.None).Replace(strLine1, cb[i].ToString());
                    i++;
                }

                sw1.WriteLine(strLine1);

                strLine = sr.ReadLine();
                strLine1 = sr1.ReadLine();
            }
            sr.Close();
            sr1.Close();
            sw.Close();
            sw1.Close();
        }

        /// <summary>
        /// 直接编制按字母顺序排列的词目索引库
        /// 索引数据文件里每个信息采用８字节长　第１字节是标志字节，第２、３、４、５字节储存子块地址，第６、７、８字节储存单词地址
        /// </summary>
        static void zjbz1()
        {
            string stext, sword;
            FileStream aFile = new FileStream(@".\62.txt", FileMode.Open);
            StreamReader sr = new StreamReader(aFile, System.Text.Encoding.GetEncoding("utf-8"));
            FileStream aF1 = new FileStream(@".\62cp", FileMode.Open);

            int ist, fdz, zdz, dcdz, maxblockno;//dcdz单词在词典中的地址
            byte[] by1 = new byte[8] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            byte[] b = new byte[256] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            maxblockno = 1;

            aF1.Write(b, 0, 32 * 8);

            int p = 0, q = 0;
            stext = sr.ReadLine();
            while (stext != null)
            {
                //sword = stext.Substring(8);
                sword = stext;
                ist = sword.Length;
                fdz = 0;
                for (int i = 0; i < ist; i++)
                {
                    aF1.Seek(fdz + iABC(sword.Substring(i, 1)) * 8, SeekOrigin.Begin);
                    aF1.Read(by1, 0, 8);
                    dcdz = (int)(by1[5] | by1[6] << 8 | by1[7] << 16 | 0) + 1;
                    if ((int)(by1[0]) == 0)//还没有被使用
                    {
                        //zdz = maxblockno * 31 * 8;
                        zdz = maxblockno * 32 * 8;

                        if (i < ist - 1)
                        {
                            by1[0] = 1;
                            by1[1] = (byte)(zdz);
                            by1[2] = (byte)(zdz >> 8);
                            by1[3] = (byte)(zdz >> 16);
                            by1[4] = (byte)(zdz >> 24);
                            aF1.Seek(fdz + iABC(sword.Substring(i, 1)) * 8, SeekOrigin.Begin);
                            aF1.Write(by1, 0, 8);

                            //fdz = maxblockno * 31 * 8;
                            fdz = maxblockno * 32 * 8;

                            //aF1.Seek(maxblockno * 31 * 8, SeekOrigin.Begin);
                            //aF1.Write(b, 0, 31 * 8);
                            aF1.Seek(maxblockno * 32 * 8, SeekOrigin.Begin);
                            aF1.Write(b, 0, 32 * 8);
                            maxblockno++;
                        }
                        else
                        {
                            dcdz = 1;
                            by1[0] = 2;
                            by1[5] = (byte)(dcdz);
                            by1[6] = (byte)(dcdz >> 8);
                            by1[7] = (byte)(dcdz >> 16);
                            aF1.Seek(fdz + iABC(sword.Substring(i, 1)) * 8, SeekOrigin.Begin);
                            aF1.Write(by1, 0, 8);
                        }
                    }
                    else if ((int)(by1[0]) == 1)//只是路
                    {
                        if (i < ist - 1)
                        {
                            fdz = (int)(by1[1] | by1[2] << 8 | by1[3] << 16 | by1[4] << 24);
                        }
                        else
                        {
                            dcdz = 1;
                            by1[0] = 3;
                            by1[5] = (byte)(dcdz);
                            by1[6] = (byte)(dcdz >> 8);
                            by1[7] = (byte)(dcdz >> 16);
                            aF1.Seek(fdz + iABC(sword.Substring(i, 1)) * 8, SeekOrigin.Begin);
                            aF1.Write(by1, 0, 8);
                        }
                    }
                    else if ((int)(by1[0]) == 2)//只是单词不是路
                    {
                        //zdz = maxblockno * 31 * 8;
                        zdz = maxblockno * 32 * 8;

                        if (i < ist - 1)
                        {
                            by1[0] = 3;
                            by1[1] = (byte)(zdz);
                            by1[2] = (byte)(zdz >> 8);
                            by1[3] = (byte)(zdz >> 16);
                            by1[4] = (byte)(zdz >> 24);
                            aF1.Seek(fdz + iABC(sword.Substring(i, 1)) * 8, SeekOrigin.Begin);
                            aF1.Write(by1, 0, 8);

                            //fdz = maxblockno * 31 * 8;

                            //aF1.Seek(maxblockno * 31 * 8, SeekOrigin.Begin);
                            //aF1.Write(b, 0, 31 * 8);
                            fdz = maxblockno * 32 * 8;

                            aF1.Seek(maxblockno * 32 * 8, SeekOrigin.Begin);
                            aF1.Write(b, 0, 32 * 8);
                            maxblockno++;
                        }
                        else
                        {
                            //dcdz = int.Parse(stext.Substring(0, 7));
                            by1[0] = 2;
                            by1[5] = (byte)(dcdz);
                            by1[6] = (byte)(dcdz >> 8);
                            by1[7] = (byte)(dcdz >> 16);
                            aF1.Seek(fdz + iABC(sword.Substring(i, 1)) * 8, SeekOrigin.Begin);
                            aF1.Write(by1, 0, 8);
                        }
                    }
                    else if ((int)(by1[0]) == 3)//3是路又是单词
                    {
                        if (i < ist - 1)
                        {
                            fdz = (int)(by1[1] | by1[2] << 8 | by1[3] << 16 | by1[4] << 24);
                        }
                        else
                        {
                            //dcdz = int.Parse(stext.Substring(0, 7));
                            by1[0] = 3;
                            by1[5] = (byte)(dcdz);
                            by1[6] = (byte)(dcdz >> 8);
                            by1[7] = (byte)(dcdz >> 16);
                            aF1.Seek(fdz + iABC(sword.Substring(i, 1)) * 8, SeekOrigin.Begin);
                            aF1.Write(by1, 0, 8);
                        }
                    }
                }

                p++;
                if (p == q + 10000)
                {
                    q = 10000 + q;
                    Console.WriteLine(p);
                }

                stext = sr.ReadLine();
            }
            sr.Close();
            aF1.Close();
        }

        //读词典
        static void duci()
        {
            int n = 0;  //记录查找到的结果条数
            string palidc = "";

            FileStream aF1 = new FileStream(@".\62cp", FileMode.Open);
            StreamWriter sw = new StreamWriter(@".\danci.txt", false, System.Text.Encoding.GetEncoding("utf-8"));

            byte[] b = new byte[8] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            string[] paliABC = { "A", "Ā", "I", "Ī", "U", "Ū", "E", "O", "K", "G", "Ṅ", "C", "J", "Ñ", "Ṭ", "Ḍ", "Ṇ", "T", "D", "N", "P", "B", "M", "Y", "R", "L", "V", "S", "H", "Ḷ", "Ŋ" };
            int lu;
            lu = 0;

            int[] alu = new int[220];
            int[] lzimu = new int[220];
            int ch, zimu;
            ch = 0;
            zimu = 1;
            alu[0] = lu;
            do
            {
                aF1.Seek(alu[ch] + zimu * 8, SeekOrigin.Begin);
                aF1.Read(b, 0, 8);
                if ((int)(b[0]) == 0)
                {
                    if (zimu < 31)
                    {
                        if (palidc.Length >= ch)
                            palidc = palidc.Substring(0, ch);
                        zimu++;
                    }
                    else
                    {
                        if (palidc.Length >= ch && ch > 0)
                            palidc = palidc.Substring(0, ch - 1);
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
                    if (palidc.Length >= ch)
                    {
                        palidc = palidc.Substring(0, ch);
                        palidc = palidc + numtoabc(zimu);
                    }
                    lzimu[ch] = zimu;
                    ch++;
                    alu[ch] = (int)(b[1] | b[2] << 8 | b[3] << 16 | b[4] << 24);
                    zimu = 1;
                }
                else if ((int)(b[0]) == 2)
                {

                    if (palidc.Length >= ch)
                    {
                        palidc = palidc.Substring(0, ch);
                        palidc = palidc + numtoabc(zimu);
                    }
                    sw.WriteLine(((int)(b[5] | b[6] << 8 | b[7] << 16 | 0)).ToString().PadLeft(7, '0') + palidc);

                    Console.WriteLine(n++);

                    if (zimu < 31)
                    {
                        if (palidc.Length >= ch)
                            palidc = palidc.Substring(0, ch);
                        zimu++;
                    }
                    else
                    {
                        if (palidc.Length >= ch && ch > 0)
                            palidc = palidc.Substring(0, ch - 1);
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

                    if (palidc.Length >= ch)
                    {
                        palidc = palidc.Substring(0, ch);
                        palidc = palidc + numtoabc(zimu);
                    }
                    sw.WriteLine(((int)(b[5] | b[6] << 8 | b[7] << 16 | 0)).ToString().PadLeft(7, '0') + palidc);

                    Console.WriteLine(n++);
                    lzimu[ch] = zimu;
                    ch++;
                    alu[ch] = (int)(b[1] | b[2] << 8 | b[3] << 16 | b[4] << 24);
                    zimu = 1;
                }
                if (zimu == 32 & ch == 0)
                    break;
            } while (1 == 1);

            aF1.Close();
            sw.Close();
        }

        //按词频排序
        public static void dancipx()
        {
            string strLine;
            int NUM = 943641;
            //int NUM = 1327;
            string[] sDC = new string[NUM];
            int[] iCP = new int[NUM];

            StreamReader sr = new StreamReader(new FileStream(@".\danci.txt", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            strLine = sr.ReadLine();
            int n = 0;
            while (strLine != null)
            {
                sDC[n] = strLine.Substring(7);
                iCP[n] = Convert.ToInt32(strLine.Substring(0, 7));
                n++;
                strLine = sr.ReadLine();
            }
            sr.Close();

            int p = 0, q = 0;
            for (int i = 0; i < NUM; ++i)
            {
                int tempd = iCP[i];
                string temps = sDC[i];
                int jj = i;
                while ((jj > 0) && tempd > iCP[jj - 1])
                {
                    iCP[jj] = iCP[jj - 1];
                    sDC[jj] = sDC[jj - 1];
                    --jj;
                }
                iCP[jj] = tempd;
                sDC[jj] = temps;
                //Console.WriteLine(i);
                p++;
                if (p == q + 10000)
                {
                    q = 10000 + q;
                    Console.WriteLine(p);
                }
            }

            StreamWriter sw = new StreamWriter(@".\dancipx1.txt", false, System.Text.Encoding.GetEncoding("utf-8"));
            for (int k = 0; k < NUM; k++)
            {
                sw.WriteLine(iCP[k].ToString().PadLeft(7, '0') + sDC[k]);
            }
            sw.Close();
        }

        //词频分析
        public static void dancicpfx()
        {
            string strLine;
            int k = 0, n = 0;
            float NUM = 9775862.0f;
            int p = 0, q = 0;

            bool okt = true;

            StreamReader sr = new StreamReader(new FileStream(@".\dancipx.txt", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@".\dancicpfx.txt", false, System.Text.Encoding.GetEncoding("utf-8"));
            strLine = sr.ReadLine();
            while (strLine != null)
            {
                //sDC[n] = strLine.Substring(7);
                k = k + Convert.ToInt32(strLine.Substring(0, 7));
                if (n == 9)
                {
                    sw.Write("前10词%: ");
                    sw.WriteLine(k / NUM * 100);
                }

                if (n == 19)
                {
                    sw.Write("前20词%: ");
                    sw.WriteLine(k / NUM * 100);
                }

                if (n == 49)
                {
                    sw.Write("前50词%: ");
                    sw.WriteLine(k / NUM * 100);
                }

                if (n == 99)
                {
                    sw.Write("前100词%: ");
                    sw.WriteLine(k / NUM * 100);
                }

                if (n == 199)
                {
                    sw.Write("前200词%: ");
                    sw.WriteLine(k / NUM * 100);
                }

                if (n == 999)
                {
                    sw.Write("前一千词%: ");
                    sw.WriteLine(k / NUM * 100);
                }

                if (n == 1999)
                {
                    sw.Write("<> 前两千词%: ");
                    sw.WriteLine(k / NUM * 100);
                }

                if (n == 9999)
                {
                    sw.Write("前一万词%: ");
                    sw.WriteLine(k / NUM * 100);
                }

                if (n == 19999)
                {
                    sw.Write("前两万词%: ");
                    sw.WriteLine(k / NUM * 100);
                }

                if ((n > 45000 && n < 50000) && (okt && k / NUM * 100 > 80))
                {
                    sw.Write("前" + (n + 1) + "词%: ");
                    okt = false;
                    sw.WriteLine(k / NUM * 100);
                }

                if (n == 49999)
                {
                    sw.Write("前五万词%: ");
                    sw.WriteLine(k / NUM * 100);
                }

                if (n == 75749)
                {
                    sw.Write("词频大于9的词%: ");
                    sw.WriteLine(k / NUM * 100);
                }

                if (n == 185472)
                {
                    sw.Write("词频大于3的词%: ");
                    sw.WriteLine(k / NUM * 100);
                }

                if (n == 250719)
                {
                    sw.Write("词频大于2的词%: ");
                    sw.WriteLine(k / NUM * 100);
                }

                if (n == 396197)
                {
                    sw.Write("词频大于1的词%: ");
                    sw.WriteLine(k / NUM * 100);
                    sr.Close();
                    sw.Close();
                    Console.WriteLine(n);
                    return;
                }

                n++;

                p++;
                if (p == q + 10000)
                {
                    q = 10000 + q;
                    Console.WriteLine(p);
                }

                strLine = sr.ReadLine();
            }

            sr.Close();
            sw.Close();
        }

        //连接两个txt文件
        public static void linktxt()
        {
            string strLine, strLine1;

            StreamReader sr = new StreamReader(new FileStream(@".\601.txt", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            StreamReader sr1 = new StreamReader(new FileStream(@".\602.txt", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@".\603.txt", false, System.Text.Encoding.GetEncoding("utf-8"));

            strLine = sr.ReadLine();
            int k = 0;
            while (strLine != null)
            {
                sw.WriteLine(strLine);
                Console.WriteLine(k++);
                strLine = sr.ReadLine();
            }
            sr.Close();

            strLine1 = sr1.ReadLine();
            while (strLine1 != null)
            {
                sw.WriteLine(strLine1);
                Console.WriteLine(k++);
                strLine1 = sr1.ReadLine();
            }
            sr1.Close();

            sw.Close();
        }

        public static void fff()
        {
            string strLine;

            StreamReader sr = new StreamReader(new FileStream(@".\62.txt", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@".\621.txt", false, System.Text.Encoding.GetEncoding("utf-8"));

            strLine = sr.ReadLine();
            while (strLine != null)
            {
                sw.Write("~" + strLine + "%");
                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        public static void fff1()
        {
            string strLine, strLine1;

            StreamReader sr = new StreamReader(new FileStream(@".\62.txt", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            StreamReader sr1 = new StreamReader(new FileStream(@".\621.txt", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@".\622.txt", false, System.Text.Encoding.GetEncoding("utf-8"));

            strLine1 = sr1.ReadToEnd();
            sr1.Close();
            int k = 0;
            //int p = 0, q = 0;
            strLine = sr.ReadLine();
            while (strLine != null)
            {
                MatchCollection mc = new Regex("~" + strLine + "%", RegexOptions.IgnoreCase).Matches(strLine1);
                if (mc.Count > 0)
                    strLine1 = new Regex("~" + strLine + "%", RegexOptions.IgnoreCase).Replace(strLine1, "") + "~" + mc.Count.ToString().PadLeft(4, '0') + strLine + "%";
                Console.WriteLine(k++);
                /*
                p++;
                if (p == q + 10000)
                {
                    q = 10000 + q;
                    Console.WriteLine(p);
                }
                */
                strLine = sr.ReadLine();
            }
            sr.Close();
            strLine1 = new Regex("~", RegexOptions.IgnoreCase).Replace(strLine1, "");
            strLine1 = new Regex("%", RegexOptions.IgnoreCase).Replace(strLine1, "\r\n");
            sw.Write(strLine1);
            sw.Close();
        }

        //统计pali三藏经典里使用的所有词汇,统计的时候,没有包括以[]和()括起来的内容,原因是暂时不清楚括号里是什么,以及括号里是不是都是完整的单词,或者括号里有些是对单词的分解,
        //所以没统计括号里的内容,因此统计会有误差,但应该很小
        //每个词分写为一行
        public static void ppp()
        {
            string strLine;

            StreamReader sr = new StreamReader(new FileStream(@".\603.txt", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@".\61.txt", false, System.Text.Encoding.GetEncoding("utf-8"));

            int p = 0, q = 0;

            strLine = sr.ReadLine();
            while (strLine != null)
            {
                strLine = new Regex(@"\[[^]]+\]", RegexOptions.IgnoreCase).Replace(strLine, " ");
                strLine = new Regex(@"\([^)]+\)", RegexOptions.IgnoreCase).Replace(strLine, " ");
                strLine = new Regex(@"\d+-\d+", RegexOptions.IgnoreCase).Replace(strLine, " ");
                strLine = new Regex(@"\d+", RegexOptions.IgnoreCase).Replace(strLine, " ");
                strLine = new Regex(@"[…,.?;!–+=>'§`ःऋ^]", RegexOptions.IgnoreCase).Replace(strLine, " ");
                strLine = new Regex(@"‘‘", RegexOptions.IgnoreCase).Replace(strLine, " ");
                strLine = new Regex(@"’’", RegexOptions.IgnoreCase).Replace(strLine, " ");
                strLine = new Regex(@"‘", RegexOptions.IgnoreCase).Replace(strLine, " ");
                strLine = new Regex(@"’", RegexOptions.IgnoreCase).Replace(strLine, " ");
                strLine = new Regex(@"-", RegexOptions.IgnoreCase).Replace(strLine, " ");
                strLine = new Regex(@"\\", RegexOptions.IgnoreCase).Replace(strLine, " ");
                strLine = new Regex(@"\(", RegexOptions.IgnoreCase).Replace(strLine, " ");
                strLine = new Regex(@"\)", RegexOptions.IgnoreCase).Replace(strLine, " ");
                strLine = new Regex(@"\[", RegexOptions.IgnoreCase).Replace(strLine, " ");
                strLine = new Regex(@"\]", RegexOptions.IgnoreCase).Replace(strLine, " ");
                strLine = new Regex(@" ", RegexOptions.IgnoreCase).Replace(strLine, "\r\n");
                if (strLine.Trim() != "")
                    sw.WriteLine(strLine);

                p++;
                if (p == q + 10000)
                {
                    q = 10000 + q;
                    Console.WriteLine(p);
                }

                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        //删除空行
        public static void ppp1()
        {
            string strLine;

            StreamReader sr = new StreamReader(new FileStream(@".\61.txt", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@".\62.txt", false, System.Text.Encoding.GetEncoding("utf-8"));

            int p = 0, q = 0;

            strLine = sr.ReadLine();
            while (strLine != null)
            {
                if (strLine.Trim() != "")
                    sw.WriteLine(strLine);

                p++;
                if (p == q + 10000)
                {
                    q = 10000 + q;
                    Console.WriteLine(p);
                }

                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        public static void dccwt()
        {
            string strLine;

            StreamReader sr = new StreamReader(new FileStream(@".\mccw", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@".\mccw1", false, System.Text.Encoding.GetEncoding("utf-8"));

            strLine = sr.ReadLine();
            while (strLine != null)
            {
                MatchCollection mc = new Regex(@"^(?<w>.*)%[^%]*$", RegexOptions.None).Matches(strLine);
                foreach (Match ma in mc)
                {
                    sw.WriteLine(ma.Groups["w"].Value);
                }
                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        public static void dccw1()
        {
            string strLine;

            StreamReader sr = new StreamReader(new FileStream(@".\dccw.txt", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@".\dccw1.txt", false, System.Text.Encoding.GetEncoding("utf-8"));

            strLine = sr.ReadLine();
            while (strLine != null)
            {
                if (strLine != "")
                    sw.WriteLine(strLine.Substring(0, 3) + "    " + strLine.Substring(7));
                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        public static void dccw2()
        {
            string strLine;

            StreamReader sr = new StreamReader(new FileStream(@".\dccw.txt", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@".\dccw2.txt", false, System.Text.Encoding.GetEncoding("utf-8"));

            strLine = sr.ReadLine();
            while (strLine != null)
            {
                if (strLine != "")
                    sw.WriteLine(strLine.Substring(0, 3) + "a   " + strLine.Substring(7));
                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        public static void dccw3()
        {
            string strLine;

            StreamReader sr = new StreamReader(new FileStream(@".\dccw.txt", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@".\dccw3.txt", false, System.Text.Encoding.GetEncoding("utf-8"));

            strLine = sr.ReadLine();
            while (strLine != null)
            {
                if (strLine != "")
                    sw.WriteLine(strLine.Substring(0, 3) + "ati " + strLine.Substring(7));
                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        public static void cldccw()
        {
            string strLine1;
            string[] csLine = new string[441];
            StreamReader sr1 = new StreamReader(new FileStream(@".\dccw4.txt", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            int i = 0;
            strLine1 = sr1.ReadLine();
            while (strLine1 != null)
            {
                csLine[i] = strLine1;
                i++;
                strLine1 = sr1.ReadLine();
            }
            sr1.Close();

            string strLine;
            StreamReader sr = new StreamReader(new FileStream(@".\dccw4.txt", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@".\dccw5.txt", false, System.Text.Encoding.GetEncoding("utf-8"));

            strLine = sr.ReadLine();
            while (strLine != null)
            {
                int n = 0;
                for (int h = 0; h < 441; h++)
                {
                    if (csLine[h].Substring(0, 19) == strLine.Substring(0, 19))
                    {
                        n++;
                        if (n > 1)
                            csLine[h] = "                   " + csLine[h].Substring(19);
                        sw.WriteLine(csLine[h]);
                        csLine[h] = "                   ";
                    }
                }
                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        public static void clmccw()
        {
            string strLine1;
            string[] csLine = new string[832];
            StreamReader sr1 = new StreamReader(new FileStream(@".\mccw1.txt", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            int i = 0;
            strLine1 = sr1.ReadLine();
            while (strLine1 != null)
            {
                csLine[i] = strLine1;
                i++;
                strLine1 = sr1.ReadLine();
            }
            sr1.Close();

            string strLine;
            StreamReader sr = new StreamReader(new FileStream(@".\mccw1.txt", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@".\mccw2.txt", false, System.Text.Encoding.GetEncoding("utf-8"));

            strLine = sr.ReadLine();
            while (strLine != null)
            {
                int n = 0;
                for (int h = 0; h < 832; h++)
                {
                    if (csLine[h].Substring(0, 16) == strLine.Substring(0, 16))
                    {
                        n++;
                        if (n > 1)
                            csLine[h] = "                " + csLine[h].Substring(16);
                        sw.WriteLine(csLine[h]);
                        csLine[h] = "                ";
                    }
                }
                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        public static void cldcmccw1()
        {
            string strLine, s1, s2, s3, s4, s5, b2, b3, b4, b5;
            StreamReader sr = new StreamReader(new FileStream(@".\dccw5.txt", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@".\dccw6.txt", false, System.Text.Encoding.GetEncoding("utf-8"));

            s1 = "";
            s2 = "";
            s3 = "";
            s4 = "";
            s5 = "";
            b2 = "";
            b3 = "";
            b4 = "";
            b5 = "";
            strLine = sr.ReadLine();
            while (strLine != null)
            {
                if (strLine.Substring(2, 1) != " ")
                {
                    sw.WriteLine(s1 + s2 + "%" + s3 + "%" + s4 + "%" + s5);
                    s1 = strLine.Substring(0, 19);

                    MatchCollection mc = new Regex(@"^.{19}(?<w2>[^%]*)%(?<w3>[^%]*)%(?<w4>[^%]*)%(?<w5>[^%]*)$", RegexOptions.None).Matches(strLine);
                    foreach (Match ma in mc)
                    {
                        s2 = ma.Groups["w2"].Value;
                        s3 = ma.Groups["w3"].Value;
                        s4 = ma.Groups["w4"].Value;
                        s5 = ma.Groups["w5"].Value;
                    }
                }
                else
                {
                    MatchCollection mc = new Regex(@"^.{19}(?<w2>[^%]*)%(?<w3>[^%]*)%(?<w4>[^%]*)%(?<w5>[^%]*)$", RegexOptions.None).Matches(strLine);
                    foreach (Match ma in mc)
                    {
                        b2 = ma.Groups["w2"].Value;
                        b3 = ma.Groups["w3"].Value;
                        b4 = ma.Groups["w4"].Value;
                        b5 = ma.Groups["w5"].Value;
                    }

                    MatchCollection mc2 = new Regex(b2, RegexOptions.None).Matches(s2);
                    if (mc2.Count == 0)
                        s2 = s2 + "," + b2;
                    MatchCollection mc3 = new Regex(b3, RegexOptions.None).Matches(s3);
                    if (mc3.Count == 0)
                        s3 = s3 + "," + b3;
                    MatchCollection mc4 = new Regex(b4, RegexOptions.None).Matches(s4);
                    if (mc4.Count == 0)
                        s4 = s4 + "," + b4;
                    MatchCollection mc5 = new Regex(b5, RegexOptions.None).Matches(s5);
                    if (mc5.Count == 0)
                        s5 = s5 + "," + b5;
                }
                strLine = sr.ReadLine();
            }
            sw.WriteLine(s1 + s2 + "%" + s3 + "%" + s4 + "%" + s5);
            sr.Close();
            sw.Close();
        }

        public static void ziwei1()
        {
            string strLine;

            StreamReader sr = new StreamReader(new FileStream(@".\ziwei\ziwei_a.txt", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@".\ziwei\ziwei_a1.txt", false, System.Text.Encoding.GetEncoding("utf-8"));

            strLine = sr.ReadLine();
            while (strLine != null)
            {
                MatchCollection mc = new Regex(@"^-(?<w>[^ ]+) .*", RegexOptions.None).Matches(strLine);
                foreach (Match ma in mc)
                {
                    sw.WriteLine(ma.Groups["w"]);
                }
                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        public static void getC()
        {
            string strLine;
            StreamReader sr = new StreamReader(new FileStream(@".\pali-e\emcidian", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@".\pali-e\t.txt", false, System.Text.Encoding.GetEncoding("utf-8"));
            strLine = sr.ReadLine();
            while (strLine != null)
            {
                /*mf巴汉*/
                //MatchCollection mc = new Regex(@"^(?<w>[^,﹐\s(\d【→/]+)[,﹐\s(\d【→/].*$", RegexOptions.None).Matches(strLine);
                MatchCollection mc = new Regex(@"^(?<w>[^\s,\d=:°]+)[\s,\d=:°].*$", RegexOptions.None).Matches(strLine);
                foreach (Match ma in mc)
                {
                    sw.WriteLine(ma.Groups["w"].Value.Substring(2));
                }
                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        public static void getmostlength()
        {
            string strLine;

            StreamReader sr = new StreamReader(new FileStream(@".\1", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@".\tmp.txt", false, System.Text.Encoding.GetEncoding("utf-8"));

            strLine = sr.ReadLine();
            int i = 0;
            while (strLine != null)
            {
                if (strLine.Length >= 15)
                    sw.WriteLine("(" + strLine.Length + ")" + strLine);
                i++;
                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        //给词典加标志G%
        public static void addbz()
        {
            string strLine;

            StreamReader sr = new StreamReader(new FileStream(@".\巴利新音译简.txt", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@".\21.txt", false, System.Text.Encoding.GetEncoding("utf-8"));

            strLine = sr.ReadLine();
            while (strLine != null)
            {
                sw.WriteLine("M%" + strLine);
                strLine = sr.ReadLine();
            }

            sr.Close();
            sw.Close();
        }

        //生成单词表
        public static void dcb()
        {
            string strLine;

            StreamReader sr = new StreamReader(new FileStream(@"E:\c#\Projects\PaliHan2\六部巴汉\简tahoma\词典.txt", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@"E:\c#\Projects\PaliHan2\六部巴汉\简tahoma\词典dcb.txt", false, System.Text.Encoding.GetEncoding("utf-8"));

            strLine = sr.ReadLine();
            while (strLine != null)
            {
                //MatchCollection mc = new Regex(@"^[aiueokhgcjtdnpbmyrlvsAIUEOKHGCJTDNPBMYRLVSĀāĪīŪūṄṅÑñṬṭḌḍṆṇḶḷŊŋ]+?,(?<w>[aiueokhgcjtdnpbmyrlvsAIUEOKHGCJTDNPBMYRLVSĀāĪīŪūṄṅÑñṬṭḌḍṆṇḶḷŊŋ]+?)(?<w1>[,。].*)$", RegexOptions.None).Matches(strLine);
                //大MatchCollection mc = new Regex(@"^(?<w>[^,。(]+?)[,。(].*$", RegexOptions.None).Matches(strLine);
                //廖MatchCollection mc = new Regex(@"^(?<w>[^(:]+?)[(:].*$", RegexOptions.None).Matches(strLine);
                //玛葛张温MatchCollection mc = new Regex(@"^(?<w>[^:]+?):.*$", RegexOptions.None).Matches(strLine);
                MatchCollection mc = new Regex(@"^(?<w>[^,。(:]+?)[,。(:].*$", RegexOptions.None).Matches(strLine);
                foreach (Match ma in mc)
                {
                    sw.WriteLine(ma.Groups["w"]);
                }
                //strLine = new Regex(@"^(?<w>[aiueokhgcjtdnpbmyrlvsAIUEOKHGCJTDNPBMYRLVSĀāĪīŪūṄṅÑñṬṭḌḍṆṇḶḷŊŋ]+?) 【(?<w1>.*)$", RegexOptions.None).Replace(strLine, "${w},【${w1}");
                //if (mc.Count == 0)
                //  sw.WriteLine(strLine);
                strLine = sr.ReadLine();
            }

            sr.Close();
            sw.Close();
        }

        //拆分马氏词典单词首以,,或,。分割的同义词，需运行两次，以拆分三个同义词
        public static void cftongyi()
        {
            string strLine;

            StreamReader sr = new StreamReader(new FileStream(@"E:\c#\Projects\PaliHan2\五部巴汉\简tahoma\马.txt", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@"E:\c#\Projects\PaliHan2\五部巴汉\简tahoma\马1.txt", false, System.Text.Encoding.GetEncoding("utf-8"));

            strLine = sr.ReadLine();
            while (strLine != null)
            {
                sw.WriteLine(strLine);

                //MatchCollection mc = new Regex(@"^[aiueokhgcjtdnpbmyrlvsAIUEOKHGCJTDNPBMYRLVSĀāĪīŪūṄṅÑñṬṭḌḍṆṇḶḷŊŋ]+?,(?<w>[aiueokhgcjtdnpbmyrlvsAIUEOKHGCJTDNPBMYRLVSĀāĪīŪūṄṅÑñṬṭḌḍṆṇḶḷŊŋ]+?)(?<w1>[,。].*)$", RegexOptions.None).Matches(strLine);
                MatchCollection mc = new Regex(@"^[aiueokhgcjtdnpbmyrlvsAIUEOKHGCJTDNPBMYRLVSĀāĪīŪūṄṅÑñṬṭḌḍṆṇḶḷŊŋ]+?[,。][aiueokhgcjtdnpbmyrlvsAIUEOKHGCJTDNPBMYRLVSĀāĪīŪūṄṅÑñṬṭḌḍṆṇḶḷŊŋ]+?[,。](?<w>[aiueokhgcjtdnpbmyrlvsAIUEOKHGCJTDNPBMYRLVSĀāĪīŪūṄṅÑñṬṭḌḍṆṇḶḷŊŋ]+?)(?<w1>[,。].*)$", RegexOptions.None).Matches(strLine);
                foreach (Match ma in mc)
                {
                    //sw.WriteLine(strLine);
                    sw.WriteLine(ma.Groups["w"].Value + ma.Groups["w1"].Value);
                }

                strLine = sr.ReadLine();
            }

            sr.Close();
            sw.Close();
        }

        //替代马氏词典单词首分割空格
        public static void tdkongge()
        {
            string strLine;

            StreamReader sr = new StreamReader(new FileStream(@"E:\c#\Projects\PaliHan2\五部巴汉\简tahoma\马.txt", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@"E:\c#\Projects\PaliHan2\五部巴汉\简tahoma\马1.txt", false, System.Text.Encoding.GetEncoding("utf-8"));

            strLine = sr.ReadToEnd();
            //stmp = new Regex(@"[\u4e00-\u9fa5]", RegexOptions.IgnoreCase).Replace(strLine, "");

            strLine = new Regex(@" \(", RegexOptions.None).Replace(strLine, @"(");
            strLine = new Regex(@" ,", RegexOptions.None).Replace(strLine, @",");
            strLine = new Regex(@" 。", RegexOptions.None).Replace(strLine, @"。");
            strLine = new Regex(@" 的", RegexOptions.None).Replace(strLine, @"的");
            strLine = new Regex(@", ", RegexOptions.None).Replace(strLine, @",");
            strLine = new Regex(@"。 ", RegexOptions.None).Replace(strLine, @"。");
            strLine = new Regex(@"\( ", RegexOptions.None).Replace(strLine, @"(");
            sw.Write(strLine);

            sr.Close();
            sw.Close();
        }

        public static void tdfont()
        {
            string strLine;

            StreamReader sr = new StreamReader(new FileStream(@"E:\c#\Projects\PaliHan2\五部巴汉\简tahoma\玛.txt", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@"E:\c#\Projects\PaliHan2\五部巴汉\简tahoma\玛1.txt", false, System.Text.Encoding.GetEncoding("utf-8"));

            strLine = sr.ReadToEnd();
            //stmp = new Regex(@"[\u4e00-\u9fa5]", RegexOptions.IgnoreCase).Replace(strLine, "");
            //Sangayana
            char[] ca = "àãåï¤ñóõëüâäæð¥òôöìý".ToCharArray();
            //Tahoma
            char[] cb = "āīūṅñṭḍṇḷṃĀĪŪṄÑṬḌṆḶṂ".ToCharArray();
            int i = 0;
            foreach (char c in ca)
            {
                strLine = new Regex(c.ToString(), RegexOptions.IgnoreCase).Replace(strLine, cb[i].ToString());
                i++;
            }
            sw.Write(strLine);

            sr.Close();
            sw.Close();
        }

        public static void cc(string dc)
        {
            int sNo = -1, lNo = -1;
            int ibz = -3;

            ibz = dcNo(dc, out iNo);
            if (ibz == -2)
            {
                Console.WriteLine("没查到！");
                return;
            }
            if (ibz == 0)
            {
                sNo = iNo;
            }
            if (ibz == 1)
            {
                sNo = iNo + 1;
            }
            if (ibz == -1)
            {
                sNo = 0;
            }

            ibz = dcNo(dc.PadRight(25, 'Ŋ'), out iNo);
            if (ibz == -1)
            {
                Console.WriteLine("没查到！");
                return;
            }
            if (ibz == 0)
            {
                lNo = iNo;
            }
            if (ibz == 1)
            {
                lNo = iNo;
            }
            if (ibz == -2)
            {
                lNo = NUM - 1;
            }

            int No = sNo;
            do
            {
                Console.WriteLine(strL[No]);
                No++;
            } while (No <= lNo);
            Console.WriteLine("" + sNo);
            Console.WriteLine("" + lNo);
        }

        //整理成适合汉查巴利的形式，然后再手工与p8abc处理的结果词条结合
        static void hb()
        {
            string strLine;

            FileStream aFile = new FileStream(@"E:\c#\Projects\PaliHan2\巴汉词典new3.txt", FileMode.Open);
            StreamReader sr = new StreamReader(aFile, System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@"E:\c#\Projects\PaliHan2\汉巴.txt", false, System.Text.Encoding.GetEncoding("utf-8"));
            strLine = sr.ReadLine();
            while (strLine != null)
            {
                MatchCollection mc = new Regex(@"[&@!].*", RegexOptions.IgnoreCase).Matches(strLine);
                if (mc.Count == 0)
                    sw.WriteLine(strLine);
                else
                {
                    MatchCollection mc1 = new Regex(@"[&@!](?<w>[^%]*)%", RegexOptions.IgnoreCase).Matches(strLine);
                    foreach (Match ma1 in mc1)
                    {
                        sw.WriteLine(ma1.Groups["w"].Value);
                    }
                }

                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        //删除巴汉词典pdf中的页码行
        static void p0a()
        {
            string srtn = "", sb = "";
            FileStream bFile = new FileStream(@"E:\c#\Projects\PaliHan\巴汉词典.txt", FileMode.Open);
            StreamReader srb = new StreamReader(bFile, System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw1 = new StreamWriter(@"E:\c#\Projects\PaliHan\巴汉词典y.txt", true, System.Text.Encoding.GetEncoding("utf-8"));
            srtn = srb.ReadLine();
            while (srtn != null)
            {
                sb = "";
                Regex re = new Regex(@"^[^\s]*\b \d{1,} \b[^\s]*\b$", RegexOptions.IgnoreCase);
                MatchCollection mc = re.Matches(srtn);
                foreach (Match ma in mc)
                {
                    sb = "y";
                }
                if (sb != "y")
                    sw1.WriteLine(srtn);
                srtn = srb.ReadLine();
            }
            srb.Close();
            sw1.Close();
        }

        //整理巴汉词典pdf，每个单词写为一行 然后再手工进行一点处理
        static void p0b()
        {
            string[] paliABC = { "A", "Ā", "I", "Ī", "U", "Ū", "E", "O", "K", "G", "Ṅ", "C", "J", "Ñ", "Ṭ", "Ḍ", "Ṇ", "T", "D", "N", "P", "B", "M", "Y", "R", "L", "V", "S", "H", "Ḷ", "Ŋ" };
            string strLine1 = "", strLine2 = "", sB = "";

            FileStream aFile = new FileStream(@"E:\c#\Projects\PaliHan\巴汉词典y.txt", FileMode.Open);
            StreamReader sr = new StreamReader(aFile, System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw1 = new StreamWriter(@"E:\c#\Projects\PaliHan\巴汉词典new.txt", true, System.Text.Encoding.GetEncoding("utf-8"));

            strLine1 = sr.ReadLine();
            strLine2 = sr.ReadLine();

            while (strLine2 != null)
            {
                sB = "";
                foreach (string pABC in paliABC)
                {
                    if (strLine2.Substring(0, 1) == pABC)
                    {
                        sw1.WriteLine(strLine1);
                        sB = "Y";
                    }
                }
                if (sB == "")
                    sw1.Write(strLine1);

                strLine1 = strLine2;
                strLine2 = sr.ReadLine();
            }
            sw1.Close();
            sr.Close();
        }

        //首先手工删除‘巴汉词典new’中重复的V词条
        static void p1()
        {
            //替代“以上的”

            FileStream aFile = new FileStream(@"E:\c#\Projects\PaliHan2\巴汉词典new.txt", FileMode.Open);
            StreamReader sr = new StreamReader(aFile, System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@"E:\c#\Projects\PaliHan2\巴汉词典new0.txt", false, System.Text.Encoding.GetEncoding("utf-8"));

            string a = "", b = "";
            a = sr.ReadLine();
            sw.WriteLine(a);
            b = sr.ReadLine();
            while (b != null)
            {
                string sword = "";
                MatchCollection mc = new Regex(@"^(?<word>.+?)\b(,| |，|\(|【)", RegexOptions.IgnoreCase).Matches(a);
                foreach (Match ma in mc)
                {
                    sword = ma.Groups["word"].Value;
                }
                sw.WriteLine(new Regex(@"以上的", RegexOptions.IgnoreCase).Replace(b, sword + "的"));

                a = b;

                b = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        static void p2()
        {
            //替代“以下的”

            FileStream aFile = new FileStream(@"E:\c#\Projects\PaliHan2\巴汉词典new0.txt", FileMode.Open);
            StreamReader sr = new StreamReader(aFile, System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@"E:\c#\Projects\PaliHan2\巴汉词典new0a.txt", false, System.Text.Encoding.GetEncoding("utf-8"));

            string a = "", b = "";
            a = sr.ReadLine();
            b = sr.ReadLine();
            while (b != null)
            {
                string sword = "";
                MatchCollection mc = new Regex(@"^(?<word>.+?)\b(,| |，|\(|【)", RegexOptions.IgnoreCase).Matches(b);
                foreach (Match ma in mc)
                {
                    sword = ma.Groups["word"].Value;
                }
                sw.WriteLine(new Regex(@"以下的", RegexOptions.IgnoreCase).Replace(a, sword + "的"));
                a = b;
                b = sr.ReadLine();
            }
            sw.WriteLine(a);

            sr.Close();
            sw.Close();
        }

        //在'巴汉词典new'中进行替换，以%号从句号处分割这两种句子：“拜火。~sālā,【阴】火房，桑那浴室。”“价者。agghāpaniya,【中】要被估计的。”，并在此类词条前加&号以区别之
        static void p3()
        {
            string strLine;

            FileStream aFile = new FileStream(@"E:\c#\Projects\PaliHan2\巴汉词典new0a.txt", FileMode.Open);
            StreamReader sr = new StreamReader(aFile, System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@"E:\c#\Projects\PaliHan2\巴汉词典new1.txt", false, System.Text.Encoding.GetEncoding("utf-8"));
            strLine = sr.ReadLine();
            string sWord;
            while (strLine != null)
            {
                sWord = "";
                sWord = new Regex(@"。\s*(?<w>[~aiueokhgcjtdnpbmyrlvsAIUEOKHGCJTDNPBMYRLVSĀāĪīŪūṄṅÑñṬṭḌḍṆṇḶḷŊŋ])", RegexOptions.IgnoreCase).Replace(strLine, "。%${w}");

                MatchCollection mc = new Regex(@".*%.*", RegexOptions.IgnoreCase).Matches(sWord);
                if (mc.Count > 0)
                    sw.WriteLine("&" + sWord);
                else
                    sw.WriteLine(sWord);

                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        static void p4()
        {
            string strLine;

            FileStream aFile = new FileStream(@"E:\c#\Projects\PaliHan2\巴汉词典new1.txt", FileMode.Open);
            StreamReader sr = new StreamReader(aFile, System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@"E:\c#\Projects\PaliHan2\巴汉词典new2.txt", false, System.Text.Encoding.GetEncoding("utf-8"));
            strLine = sr.ReadLine();
            string sWord;
            while (strLine != null)
            {
                if (strLine.Substring(0, 1) == "&")
                    sw.WriteLine(strLine);
                else
                {
                    sWord = "";
                    //[\u4e00-\u9fa5]
                    sWord = new Regex(@"(?<w1>[,。，])\s*(?<w>【[^】]+】\s*[~aiueokhgcjtdnpbmyrlvsAIUEOKHGCJTDNPBMYRLVSĀāĪīŪūṄṅÑñṬṭḌḍṆṇḶḷŊŋ]+)", RegexOptions.IgnoreCase).Replace(strLine, "${w1}%%${w}");

                    MatchCollection mc = new Regex(@".*%%.*", RegexOptions.IgnoreCase).Matches(sWord);
                    if (mc.Count > 0)
                        sw.WriteLine("@" + sWord);
                    else
                        sw.WriteLine(sWord);
                }

                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        static void p5()
        {
            string strLine;

            FileStream aFile = new FileStream(@"E:\c#\Projects\PaliHan2\巴汉词典new2.txt", FileMode.Open);
            StreamReader sr = new StreamReader(aFile, System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@"E:\c#\Projects\PaliHan2\巴汉词典new3.txt", false, System.Text.Encoding.GetEncoding("utf-8"));
            strLine = sr.ReadLine();
            while (strLine != null)
            {
                MatchCollection mc = new Regex(@"@[~ ,.，。()aiueokhgcjtdnpbmyrlvsAIUEOKHGCJTDNPBMYRLVSĀāĪīŪūṄṅÑñṬṭḌḍṆṇḶḷŊŋ]*%.*", RegexOptions.IgnoreCase).Matches(strLine);
                foreach (Match ma in mc)
                {
                    strLine = new Regex(@"%%", RegexOptions.IgnoreCase).Replace(strLine, "");
                    strLine = new Regex(@"(?<w1>@[^\u4e00-\u9fa5]+【[^【】]+】\s*)(?<w2>.*$)", RegexOptions.IgnoreCase).Replace(strLine, "${w1}%%${w2}");
                    sw.WriteLine("!" + strLine.Substring(1));
                }
                if (mc.Count == 0)
                    sw.WriteLine(strLine);

                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        static void p6()
        {
            string strLine;

            FileStream aFile = new FileStream(@"E:\c#\Projects\PaliHan2\巴汉词典new3.txt", FileMode.Open);
            StreamReader sr = new StreamReader(aFile, System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@"E:\c#\Projects\PaliHan2\巴汉词典new4.txt", false, System.Text.Encoding.GetEncoding("utf-8"));
            strLine = sr.ReadLine();
            while (strLine != null)
            {
                MatchCollection mc = new Regex(@"[&@!].*", RegexOptions.IgnoreCase).Matches(strLine);
                foreach (Match ma in mc)
                {
                    sw.WriteLine(ma.Value);
                }

                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        static void p7()
        {
            //替代~
            string strLine;

            FileStream aFile = new FileStream(@"E:\c#\Projects\PaliHan2\巴汉词典new4.txt", FileMode.Open);
            StreamReader sr = new StreamReader(aFile, System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@"E:\c#\Projects\PaliHan2\巴汉词典new5.txt", false, System.Text.Encoding.GetEncoding("utf-8"));
            strLine = sr.ReadLine();
            while (strLine != null)
            {
                string sword = "";

                MatchCollection mc = new Regex(@"[&@!](?<word>.+?)\b(,| |，|\(|【)", RegexOptions.IgnoreCase).Matches(strLine);
                foreach (Match ma in mc)
                {
                    sword = ma.Groups["word"].Value;
                }

                sw.WriteLine(new Regex(@"~", RegexOptions.IgnoreCase).Replace(strLine, sword));

                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        //处理&类词目
        static void p8a()
        {
            string strLine, sword, sjieshi, mid, last;

            FileStream aFile = new FileStream(@"E:\c#\Projects\PaliHan2\巴汉词典new5.txt", FileMode.Open);
            StreamReader sr = new StreamReader(aFile, System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@"E:\c#\Projects\PaliHan2\巴汉词典new6.txt", false, System.Text.Encoding.GetEncoding("utf-8"));
            strLine = sr.ReadLine();
            while (strLine != null)
            {
                sword = "";
                sjieshi = "";
                mid = "";
                last = "";
                MatchCollection mc = new Regex(@"&.*", RegexOptions.IgnoreCase).Matches(strLine);
                if (mc.Count > 0)
                {
                    MatchCollection mcw = new Regex(@"&(?<word>.+?)\b(,| |，|\(|【)", RegexOptions.IgnoreCase).Matches(strLine);
                    foreach (Match maw in mcw)
                    {
                        sword = maw.Groups["word"].Value;
                    }
                    MatchCollection mcj = new Regex(@"(?<=&)[^%]+(?=%)", RegexOptions.IgnoreCase).Matches(strLine);
                    foreach (Match maj in mcj)
                    {
                        sjieshi = maj.Value;
                    }
                    MatchCollection mcm = new Regex(@"(?<=%)[^%]+(?=%)", RegexOptions.IgnoreCase).Matches(strLine);
                    foreach (Match mam in mcm)
                    {
                        mid = mam.Value;
                        MatchCollection mcmh = new Regex(@"^[^\u4e00-\u9fa5]*【[^【】%]+】[^\u4e00-\u9fa5]*$", RegexOptions.IgnoreCase).Matches(mid);
                        foreach (Match mamh in mcmh)
                        {
                            sw.WriteLine(new Regex(@"(?<w1>.*)【(?<w2>.*)", RegexOptions.IgnoreCase).Replace(mamh.Value, "${w1}" + sword + "的【" + "${w2}" + sjieshi));
                        }
                        if (mcmh.Count == 0)
                            sw.WriteLine(mid);
                    }
                    MatchCollection mcl = new Regex(@"(?<=%)[^%]+$", RegexOptions.IgnoreCase).Matches(strLine);
                    foreach (Match mal in mcl)
                    {
                        last = mal.Value;
                        MatchCollection mclh = new Regex(@"^[^\u4e00-\u9fa5]*【[^【】%]+】[^\u4e00-\u9fa5]*$", RegexOptions.IgnoreCase).Matches(last);
                        foreach (Match malh in mclh)
                        {
                            sw.WriteLine(new Regex(@"(?<w1>.*)【(?<w2>.*)", RegexOptions.IgnoreCase).Replace(malh.Value, "${w1}" + sword + "的【" + "${w2}" + sjieshi));
                        }
                        if (mclh.Count == 0)
                            sw.WriteLine(last);
                    }
                }
                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        //处理@类词目，之后再手工删除十多个条目
        static void p8b()
        {
            string strLine, sword, sjieshi, mid, last;

            FileStream aFile = new FileStream(@"E:\c#\Projects\PaliHan2\巴汉词典new5.txt", FileMode.Open);
            StreamReader sr = new StreamReader(aFile, System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@"E:\c#\Projects\PaliHan2\巴汉词典new6.txt", false, System.Text.Encoding.GetEncoding("utf-8"));
            strLine = sr.ReadLine();
            while (strLine != null)
            {
                sword = "";
                sjieshi = "";
                mid = "";
                last = "";
                MatchCollection mc = new Regex(@"@.*", RegexOptions.IgnoreCase).Matches(strLine);
                if (mc.Count > 0)
                {
                    MatchCollection mcw = new Regex(@"@(?<word>.+?)\b(,| |，|\(|【)", RegexOptions.IgnoreCase).Matches(strLine);
                    foreach (Match maw in mcw)
                    {
                        sword = maw.Groups["word"].Value;
                    }
                    MatchCollection mcj = new Regex(@"(?<=@)[^%]+(?=%)", RegexOptions.IgnoreCase).Matches(strLine);
                    foreach (Match maj in mcj)
                    {
                        sjieshi = maj.Value;
                    }
                    MatchCollection mcm = new Regex(@"(?<=%)[^%]+(?=%)", RegexOptions.IgnoreCase).Matches(strLine);
                    foreach (Match mam in mcm)
                    {
                        mid = mam.Value;
                        MatchCollection mcmh = new Regex(@"^[^\u4e00-\u9fa5]*【[^【】%]+】[^\u4e00-\u9fa5]*$", RegexOptions.IgnoreCase).Matches(mid);
                        foreach (Match mamh in mcmh)
                        {
                            sw.WriteLine(new Regex(@"(?<w1>.*【[^【】%]+】\s*)(?<w2>\b.*)", RegexOptions.IgnoreCase).Replace(mamh.Value, "${w2}" + sword + "的" + "${w1}" + sjieshi));
                        }
                        if (mcmh.Count == 0)
                            sw.WriteLine(mid);
                    }
                    MatchCollection mcl = new Regex(@"(?<=%)[^%]+$", RegexOptions.IgnoreCase).Matches(strLine);
                    foreach (Match mal in mcl)
                    {
                        last = mal.Value;
                        MatchCollection mclh = new Regex(@"^[^\u4e00-\u9fa5]*【[^【】%]+】[^\u4e00-\u9fa5]*$", RegexOptions.IgnoreCase).Matches(last);
                        foreach (Match malh in mclh)
                        {
                            sw.WriteLine(new Regex(@"(?<w1>.*【[^【】%]+】\s*)(?<w2>\b.*)", RegexOptions.IgnoreCase).Replace(malh.Value, "${w2}" + sword + "的" + "${w1}" + sjieshi));
                        }
                        if (mclh.Count == 0)
                            sw.WriteLine(last);
                    }
                }
                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        //处理!类词目
        static void p8c()
        {
            string strLine;

            FileStream aFile = new FileStream(@"E:\c#\Projects\PaliHan2\巴汉词典new5.txt", FileMode.Open);
            StreamReader sr = new StreamReader(aFile, System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@"E:\c#\Projects\PaliHan2\巴汉词典new6.txt", false, System.Text.Encoding.GetEncoding("utf-8"));
            strLine = sr.ReadLine();
            while (strLine != null)
            {
                MatchCollection mc = new Regex(@"!.*", RegexOptions.IgnoreCase).Matches(strLine);
                if (mc.Count > 0)
                {
                    MatchCollection mcl = new Regex(@"(?<=%%).*$", RegexOptions.IgnoreCase).Matches(strLine);
                    foreach (Match mal in mcl)
                    {
                        sw.WriteLine(mal.Value);
                    }
                }
                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        //p8abc处理的词典与巴汉词典new合成为new6l,然后再对其进行一些手工修整

        //词典按单词字母顺序排序
        static void zmpx()
        {
            string strLine;
            int z = 0;

            //FileStream aFile = new FileStream(@"E:\c#\Projects\PaliHan2\巴汉词典new6l.txt", FileMode.Open);
            FileStream aFile = new FileStream(@".\Pali-e\pali-e2.txt", FileMode.Open);
            StreamReader sr = new StreamReader(aFile, System.Text.Encoding.GetEncoding("utf-8"));
            strLine = sr.ReadLine();
            while (strLine != null)
            {
                MatchCollection mc = new Regex(@"^(?<w>[^ ,。]*)[ ,。]", RegexOptions.IgnoreCase).Matches(strLine);
                foreach (Match ma in mc)
                {
                    sL[z] = ma.Groups["w"].Value;
                    //strL[z] = strLine.Substring(0, 1).ToUpper() + strLine.Substring(1);
                    strL[z] = strLine;
                    z++;
                }
                strLine = sr.ReadLine();
            }
            sr.Close();

            for (int i = 0; i < NUM; ++i)
            {
                string tempd = sL[i];
                string temps = strL[i];
                int jj = i;
                while ((jj > 0) && ab(tempd, sL[jj - 1]))
                {
                    sL[jj] = sL[jj - 1];
                    strL[jj] = strL[jj - 1];
                    --jj;
                }
                sL[jj] = tempd;
                strL[jj] = temps;
            }
            //StreamWriter sw2 = new StreamWriter(@"E:\c#\Projects\PaliHan2\词典.txt", false, System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw2 = new StreamWriter(@".\Pali-e\cidian", false, System.Text.Encoding.GetEncoding("utf-8"));
            for (int k = 0; k < NUM; k++)
            {
                sw2.WriteLine(strL[k]);
            }
            sw2.Close();
        }

        //给'词典'建立索引文件'词典index'，行首数字表明单词对应'词典'中的行首字节位置，之后用zjbz()函数编制生成'按字母顺序排列的词目索引库indexdat'
        static void makeindex()
        {
            string strLine;
            int i = 0;

            //FileStream aFile = new FileStream(@"E:\c#\Projects\PaliHan2\词典.txt", FileMode.Open);
            FileStream aFile = new FileStream(@".\Pali-e\cidian", FileMode.Open);
            StreamReader sr = new StreamReader(aFile, System.Text.Encoding.GetEncoding("utf-8"));
            //StreamWriter sw = new StreamWriter(@"E:\c#\Projects\PaliHan2\词典index.txt", false, System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@".\Pali-e\index", false, System.Text.Encoding.GetEncoding("utf-8"));

            strLine = sr.ReadLine();

            long lWz = 0;
            string slWz = "";

            while (strLine != null)
            {
                i = i + 1;
                string reg = "", sWord = "";

                reg = @"^(?<word>[^ ,。]+)(,| |。)";
                Regex re = new Regex(reg, RegexOptions.IgnoreCase);
                MatchCollection mc = re.Matches(strLine);
                foreach (Match ma in mc)
                {
                    sWord = ma.Groups["word"].Value;
                }

                slWz = "" + lWz;
                slWz = slWz.PadLeft(7, '0');
                sw.WriteLine(slWz + "$" + sWord);

                StreamWriter sw1 = new StreamWriter(@"E:\c#\Projects\PaliHan2\词典tmp.txt", true, System.Text.Encoding.GetEncoding("utf-8"));
                sw1.WriteLine(strLine);
                sw1.Close();

                FileStream bFile = new FileStream(@"E:\c#\Projects\PaliHan2\词典tmp.txt", FileMode.Open);
                StreamReader srb = new StreamReader(bFile, System.Text.Encoding.GetEncoding("utf-8"));
                lWz = srb.BaseStream.Length;
                srb.Close();

                strLine = sr.ReadLine();
            }

            sr.Close();
            sw.Close();
        }

        static long[] lWord = new long[NUM];
        static string[] strWord = new string[NUM];
        //词典index按单词长度排序，此函数不再使用
        static void px()
        {
            string strLine;
            int j = 0;

            FileStream aFile = new FileStream(@"E:\c#\Projects\PaliHan2\巴汉词典index.txt", FileMode.Open);
            StreamReader sr = new StreamReader(aFile, System.Text.Encoding.GetEncoding("utf-8"));
            strLine = sr.ReadLine();

            while (strLine != null)
            {
                strWord[j] = strLine;
                j++;
                strLine = sr.ReadLine();
            }
            sr.Close();

            for (int i = 0; i < NUM; ++i)
            {
                string temp = strWord[i];
                int jj = i;
                while ((jj > 0) && (strWord[jj - 1].Length < temp.Length))
                {
                    strWord[jj] = strWord[jj - 1];
                    --jj;
                }
                strWord[jj] = temp;
            }

            StreamWriter sw1 = new StreamWriter(@"E:\c#\Projects\PaliHan2\巴汉词典px.txt", true, System.Text.Encoding.GetEncoding("utf-8"));
            for (int k = 0; k < NUM; k++)
            {
                sw1.WriteLine(strWord[k]);
            }
            sw1.Close();
        }

        //把‘巴汉词典px’进行逆向排序为pxn，此函数不再使用
        static void pxn()
        {
            string strLine;
            int j = 0;

            FileStream aFile = new FileStream(@"E:\c#\Projects\PaliHan2\巴汉词典px.txt", FileMode.Open);
            StreamReader sr = new StreamReader(aFile, System.Text.Encoding.GetEncoding("utf-8"));
            strLine = sr.ReadLine();
            while (strLine != null)
            {
                strWord[j] = strLine;
                j++;
                strLine = sr.ReadLine();
            }
            sr.Close();

            StreamWriter sw1 = new StreamWriter(@"E:\c#\Projects\PaliHan2\pxn", true, System.Text.Encoding.GetEncoding("utf-8"));
            for (int k = NUM - 1; k > -1; k--)
            {
                sw1.WriteLine(strWord[k]);
            }
            sw1.Close();
        }

        //把巴汉词典px按词长拆分成以词长命名的多个小px文件，此函数不再采用
        static void pxc()
        {
            string strLine = "", spxno = "";

            FileStream aFile = new FileStream(@"E:\c#\Projects\PaliHan2\巴汉词典px.txt", FileMode.Open);
            StreamReader sr = new StreamReader(aFile, System.Text.Encoding.GetEncoding("utf-8"));
            strLine = sr.ReadLine();

            while (strLine != null)
            {
                spxno = "" + (strLine.Length - 8);
                StreamWriter sw1 = new StreamWriter(@"E:\c#\Projects\PaliHan2\巴汉词典px" + spxno + ".txt", true, System.Text.Encoding.GetEncoding("utf-8"));
                sw1.WriteLine(strLine);
                sw1.Close();

                strLine = sr.ReadLine();
            }
            sr.Close();
        }

        static double bm(string dc)
        {
            int i = 24;
            double d = 0;
            char[] ca = dc.ToCharArray();
            foreach (char c in ca)
            {
                d = d + iABC(c.ToString()) * Math.Pow(32, i);
                i--;
            }
            return d;
        }

        static void sangayana()
        {
            StreamWriter sw = new StreamWriter(@"E:\c#\Projects\PaliHan2\sangayana.txt", false, System.Text.Encoding.GetEncoding("utf-8"));
            char[] sangayanaABC = { 'â', 'ä', 'æ', 'ð', '¥', 'ò', 'ô', 'ö', 'ì', 'ý' };
            char[] sangayanaabc = { 'à', 'ã', 'å', 'ï', '¤', 'ñ', 'ó', 'õ', 'ë', 'ü' };
            char[] bhcdABC = { 'Ā', 'Ī', 'Ū', 'Ṅ', 'Ñ', 'Ṭ', 'Ḍ', 'Ṇ', 'Ḷ', 'Ŋ' };
            char[] bhcdabc = { 'ā', 'ī', 'ū', 'ṅ', 'ñ', 'ṭ', 'ḍ', 'ṇ', 'ḷ', 'ŋ' };
            foreach (char b in sangayanaABC)
            {
                Console.Write(b.ToString() + " ");
                sw.Write(b.ToString() + " ");
            }
            Console.WriteLine();
            sw.Write("\r\n");
            foreach (char a in sangayanaabc)
            {
                Console.Write(a.ToString() + " ");
                sw.Write(a.ToString() + " ");
            }
            Console.WriteLine();
            sw.Write("\r\n");
            foreach (char a in sangayanaabc)
            {
                Console.Write(a.ToString().ToUpper() + " ");
                sw.Write(a.ToString().ToUpper() + " ");
            }
            Console.WriteLine();
            sw.Write("\r\n");
            for (int i = 0; i < 10; i++)
            {
                Console.Write(Convert.ToUInt16(bhcdABC[i]) + " ");
                Console.Write(Convert.ToUInt16(bhcdabc[i]) + "   ");
            }
            Console.WriteLine();
            sw.Close();
            Console.Write(Convert.ToUInt16('Ñ') + "   ");
            Console.Write(Convert.ToUInt16('ñ') + "   ");
        }

        //输出用中文输入法输入的英文字母内码
        static void hzabc()
        {
            char[] abc = new char[52] { 'ａ', 'ｂ', 'ｃ', 'ｄ', 'ｅ', 'ｆ', 'ｇ', 'ｈ', 'ｉ', 'ｊ', 'ｋ', 'ｌ', 'ｍ', 'ｎ', 'ｏ', 'ｐ', 'ｑ', 'ｒ', 'ｓ', 'ｔ', 'ｕ', 'ｖ', 'ｗ', 'ｘ', 'ｙ', 'ｚ', 'Ａ', 'Ｂ', 'Ｃ', 'Ｄ', 'Ｅ', 'Ｆ', 'Ｇ', 'Ｈ', 'Ｉ', 'Ｊ', 'Ｋ', 'Ｌ', 'Ｍ', 'Ｎ', 'Ｏ', 'Ｐ', 'Ｑ', 'Ｒ', 'Ｓ', 'Ｔ', 'Ｕ', 'Ｖ', 'Ｗ', 'Ｘ', 'Ｙ', 'Ｚ' };
            char[] abca = new char[26] { 'ａ', 'ｂ', 'ｃ', 'ｄ', 'ｅ', 'ｆ', 'ｇ', 'ｈ', 'ｉ', 'ｊ', 'ｋ', 'ｌ', 'ｍ', 'ｎ', 'ｏ', 'ｐ', 'ｑ', 'ｒ', 'ｓ', 'ｔ', 'ｕ', 'ｖ', 'ｗ', 'ｘ', 'ｙ', 'ｚ' };
            char[] abcA = new char[26] { 'Ａ', 'Ｂ', 'Ｃ', 'Ｄ', 'Ｅ', 'Ｆ', 'Ｇ', 'Ｈ', 'Ｉ', 'Ｊ', 'Ｋ', 'Ｌ', 'Ｍ', 'Ｎ', 'Ｏ', 'Ｐ', 'Ｑ', 'Ｒ', 'Ｓ', 'Ｔ', 'Ｕ', 'Ｖ', 'Ｗ', 'Ｘ', 'Ｙ', 'Ｚ' };
            foreach (char a in abca)
            {
                Console.Write(Convert.ToUInt16(a) + " ");
            }
            Console.WriteLine();
            foreach (char a in abcA)
            {
                Console.Write(Convert.ToUInt16(a) + " ");
            }
            Console.WriteLine();
            Console.WriteLine("ａ".ToUpper() + "A");
            Console.WriteLine("Ａ".ToLower() + "a");
            Console.WriteLine(Convert.ToUInt16('a'));
            Console.WriteLine(Convert.ToUInt16('A'));
            Console.WriteLine(Convert.ToUInt16('b'));
            Console.WriteLine(Convert.ToUInt16('B'));
        }

        //直接法编制生成'按字母顺序排列的词目索引库indexdat'
        static void zjbz()
        {
            string stext, sword;
            FileStream aFile = new FileStream(@".\index", FileMode.Open);
            StreamReader sr = new StreamReader(aFile, System.Text.Encoding.GetEncoding("utf-8"));
            FileStream aF1 = new FileStream(@".\indexdat", FileMode.Open);

            int ist, fdz, zdz, dcdz, maxblockno;//dcdz单词在词典中的地址
            byte[] by1 = new byte[8] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            byte[] b = new byte[256] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            maxblockno = 1;

            aF1.Write(b, 0, 32 * 8);

            stext = sr.ReadLine();
            while (stext != null)
            {
                sword = stext.Substring(8);
                ist = sword.Length;
                fdz = 0;
                for (int i = 0; i < ist; i++)
                {
                    aF1.Seek(fdz + iABC(sword.Substring(i, 1)) * 8, SeekOrigin.Begin);
                    aF1.Read(by1, 0, 8);
                    if ((int)(by1[0]) == 0)//还没有被使用
                    {
                        zdz = maxblockno * 31 * 8;

                        if (i < ist - 1)
                        {
                            by1[0] = 1;
                            by1[1] = (byte)(zdz);
                            by1[2] = (byte)(zdz >> 8);
                            by1[3] = (byte)(zdz >> 16);
                            by1[4] = (byte)(zdz >> 24);
                            aF1.Seek(fdz + iABC(sword.Substring(i, 1)) * 8, SeekOrigin.Begin);
                            aF1.Write(by1, 0, 8);

                            fdz = maxblockno * 31 * 8;

                            aF1.Seek(maxblockno * 31 * 8, SeekOrigin.Begin);
                            aF1.Write(b, 0, 31 * 8);
                            maxblockno++;
                        }
                        else
                        {
                            dcdz = int.Parse(stext.Substring(0, 7));
                            by1[0] = 2;
                            by1[5] = (byte)(dcdz);
                            by1[6] = (byte)(dcdz >> 8);
                            by1[7] = (byte)(dcdz >> 16);
                            aF1.Seek(fdz + iABC(sword.Substring(i, 1)) * 8, SeekOrigin.Begin);
                            aF1.Write(by1, 0, 8);
                        }
                    }
                    else if ((int)(by1[0]) == 1)//只是路
                    {
                        if (i < ist - 1)
                        {
                            fdz = (int)(by1[1] | by1[2] << 8 | by1[3] << 16 | by1[4] << 24);
                        }
                        else
                        {
                            dcdz = int.Parse(stext.Substring(0, 7));
                            by1[0] = 3;
                            by1[5] = (byte)(dcdz);
                            by1[6] = (byte)(dcdz >> 8);
                            by1[7] = (byte)(dcdz >> 16);
                            aF1.Seek(fdz + iABC(sword.Substring(i, 1)) * 8, SeekOrigin.Begin);
                            aF1.Write(by1, 0, 8);
                        }
                    }
                    else if ((int)(by1[0]) == 2)//只是单词不是路
                    {
                        zdz = maxblockno * 31 * 8;

                        if (i < ist - 1)
                        {
                            by1[0] = 3;
                            by1[1] = (byte)(zdz);
                            by1[2] = (byte)(zdz >> 8);
                            by1[3] = (byte)(zdz >> 16);
                            by1[4] = (byte)(zdz >> 24);
                            aF1.Seek(fdz + iABC(sword.Substring(i, 1)) * 8, SeekOrigin.Begin);
                            aF1.Write(by1, 0, 8);

                            fdz = maxblockno * 31 * 8;

                            aF1.Seek(maxblockno * 31 * 8, SeekOrigin.Begin);
                            aF1.Write(b, 0, 31 * 8);
                            maxblockno++;
                        }
                        else
                        {
                            dcdz = int.Parse(stext.Substring(0, 7));
                            by1[0] = 2;
                            by1[5] = (byte)(dcdz);
                            by1[6] = (byte)(dcdz >> 8);
                            by1[7] = (byte)(dcdz >> 16);
                            aF1.Seek(fdz + iABC(sword.Substring(i, 1)) * 8, SeekOrigin.Begin);
                            aF1.Write(by1, 0, 8);
                        }
                    }
                    else if ((int)(by1[0]) == 3)//3是路又是单词
                    {
                        if (i < ist - 1)
                        {
                            fdz = (int)(by1[1] | by1[2] << 8 | by1[3] << 16 | by1[4] << 24);
                        }
                        else
                        {
                            dcdz = int.Parse(stext.Substring(0, 7));
                            by1[0] = 3;
                            by1[5] = (byte)(dcdz);
                            by1[6] = (byte)(dcdz >> 8);
                            by1[7] = (byte)(dcdz >> 16);
                            aF1.Seek(fdz + iABC(sword.Substring(i, 1)) * 8, SeekOrigin.Begin);
                            aF1.Write(by1, 0, 8);
                        }
                    }
                }
                stext = sr.ReadLine();
            }
            sr.Close();
            aF1.Close();
        }

        //穷举法编制生成'按字母顺序排列的词目索引库indexdat'，笨，速度慢
        static void bzsy()
        {
            /*
            byte[] inputBytes = converter.GetBytes(inputString);
            byte[] bb = new byte[i];
            sr.DiscardBufferedData();
            System.Text.UTF8Encoding converter = new System.Text.UTF8Encoding();
            inputString = converter.GetString(bb);
            */

            /*byte数组与int的转换
            byte[] b = new byte[4] { 0xfe, 0x5a, 0x11, 0xfa };
            uint u = (uint)(b[0] | b[1] << 8 | b[2] << 16 | b[3] << 24);
            b[0] = (byte)(u);
            b[1] = (byte)(u >> 8);
            b[2] = (byte)(u >> 16);
            b[3] = (byte)(u >> 24);
            Console.WriteLine(u);
            */

            string stext;
            FileStream aFile = new FileStream(@"E:\c#\Projects\PaliHan\巴汉词典px4.txt", FileMode.Open);
            StreamReader sr = new StreamReader(aFile, System.Text.Encoding.GetEncoding("utf-8"));
            stext = sr.ReadToEnd();
            sr.Close();

            FileStream aF1 = new FileStream(@"E:\c#\Projects\PaliHan\indexdat", FileMode.Open);
            string[] paliABC = { "A", "Ā", "I", "Ī", "U", "Ū", "E", "O", "K", "G", "Ṅ", "C", "J", "Ñ", "Ṭ", "Ḍ", "Ṇ", "T", "D", "N", "P", "B", "M", "Y", "R", "L", "V", "S", "H", "Ḷ", "Ŋ" };
            byte[] by1 = new byte[5] { 0x00, 0x00, 0x00, 0x00, 0x00 };
            int iFdz1 = 0, iFdz2 = 0, iFdz3 = 0, iFdz4 = 0; //父块地址，进入循环之前赋值
            int iZdz1 = 0, iZdz2 = 0, iZdz3 = 0, iZdz4 = 0; //子块地址
            int n0 = 1, n1 = 0, n2 = 0, n3 = 0, n4 = 0; //记录各层循环当前已查到的字母数

            iFdz1 = 0;
            for (int i = 0; i < 31; i++)
            {
                Regex re1 = new Regex(@"\d{7}\$" + paliABC[i] + @"\w\w\w\b", RegexOptions.IgnoreCase);
                MatchCollection mc1 = re1.Matches(stext);

                if (mc1.Count != 0)
                {
                    iZdz1 = (n0 + n1 + n2 + n3 + n4) * 31 * 5;
                    by1[0] = 1;
                    by1[1] = (byte)(iZdz1);
                    by1[2] = (byte)(iZdz1 >> 8);
                    by1[3] = (byte)(iZdz1 >> 16);
                    by1[4] = (byte)(iZdz1 >> 24);

                    aF1.Seek(iFdz1 + i * 5, SeekOrigin.Begin);//定位
                    aF1.Write(by1, 0, 5);

                    iFdz2 = (n0 + n1 + n2 + n3 + n4) * 31 * 5;
                    n1++;
                    for (int j = 0; j < 31; j++)
                    {
                        Regex re2 = new Regex(@"\d{7}\$" + paliABC[i] + paliABC[j] + @"\w\w\b", RegexOptions.IgnoreCase);
                        MatchCollection mc2 = re2.Matches(stext);

                        if (mc2.Count != 0)
                        {
                            iZdz2 = (n0 + n1 + n2 + n3 + n4) * 31 * 5;
                            by1[0] = 1;
                            by1[1] = (byte)(iZdz2);
                            by1[2] = (byte)(iZdz2 >> 8);
                            by1[3] = (byte)(iZdz2 >> 16);
                            by1[4] = (byte)(iZdz2 >> 24);

                            aF1.Seek(iFdz2 + j * 5, SeekOrigin.Begin);//定位
                            aF1.Write(by1, 0, 5);//写

                            iFdz3 = (n0 + n1 + n2 + n3 + n4) * 31 * 5;
                            n2++;
                            for (int k = 0; k < 31; k++)
                            {
                                Regex re3 = new Regex(@"\d{7}\$" + paliABC[i] + paliABC[j] + paliABC[k] + @"\w\b", RegexOptions.IgnoreCase);
                                MatchCollection mc3 = re3.Matches(stext);

                                if (mc3.Count != 0)
                                {
                                    iZdz3 = (n0 + n1 + n2 + n3 + n4) * 31 * 5;
                                    by1[0] = 1;
                                    by1[1] = (byte)(iZdz3);
                                    by1[2] = (byte)(iZdz3 >> 8);
                                    by1[3] = (byte)(iZdz3 >> 16);
                                    by1[4] = (byte)(iZdz3 >> 24);

                                    aF1.Seek(iFdz3 + k * 5, SeekOrigin.Begin);//定位
                                    aF1.Write(by1, 0, 5);//写

                                    iFdz4 = (n0 + n1 + n2 + n3 + n4) * 31 * 5;
                                    n3++;
                                    for (int l = 0; l < 31; l++)
                                    {
                                        Regex re4 = new Regex(@"\d{7}\$" + paliABC[i] + paliABC[j] + paliABC[k] + paliABC[l] + @"\b", RegexOptions.IgnoreCase);
                                        MatchCollection mc4 = re4.Matches(stext);

                                        if (mc4.Count != 0)
                                        {
                                            //最后一层循环，取得单词在词典中的地址
                                            MatchCollection mcWord = new Regex(@"(?<wIndex>\d{7})\$" + paliABC[i] + paliABC[j] + paliABC[k] + paliABC[l] + @"\b", RegexOptions.IgnoreCase).Matches(stext);
                                            foreach (Match maWord in mcWord)
                                            {
                                                //注：如有两个相同单词，则此处只能取得最后那个单词，因此应再整理一下词典，不允许出现重复的条目
                                                iZdz4 = int.Parse(maWord.Groups["wIndex"].Value);
                                            }

                                            by1[0] = 1;
                                            by1[1] = (byte)(iZdz4);
                                            by1[2] = (byte)(iZdz4 >> 8);
                                            by1[3] = (byte)(iZdz4 >> 16);
                                            by1[4] = (byte)(iZdz4 >> 24);

                                            aF1.Seek(iFdz4 + l * 5, SeekOrigin.Begin);//定位
                                            aF1.Write(by1, 0, 5);//写
                                        }
                                        else
                                        {
                                            by1[0] = 0;
                                            by1[1] = 0;
                                            by1[2] = 0;
                                            by1[3] = 0;
                                            by1[4] = 0;

                                            aF1.Seek(iFdz4 + l * 5, SeekOrigin.Begin);//定位
                                            aF1.Write(by1, 0, 5);
                                        }
                                    }
                                }
                                else
                                {
                                    by1[0] = 0;
                                    by1[1] = 0;
                                    by1[2] = 0;
                                    by1[3] = 0;
                                    by1[4] = 0;

                                    aF1.Seek(iFdz3 + k * 5, SeekOrigin.Begin);//定位
                                    aF1.Write(by1, 0, 5);
                                }
                            }
                        }
                        else
                        {
                            by1[0] = 0;
                            by1[1] = 0;
                            by1[2] = 0;
                            by1[3] = 0;
                            by1[4] = 0;

                            aF1.Seek(iFdz2 + j * 5, SeekOrigin.Begin);//定位
                            aF1.Write(by1, 0, 5);
                        }
                    }
                }
                else
                {
                    by1[0] = 0;
                    by1[1] = 0;
                    by1[2] = 0;
                    by1[3] = 0;
                    by1[4] = 0;

                    aF1.Seek(iFdz1 + i * 5, SeekOrigin.Begin);//定位
                    aF1.Write(by1, 0, 5);
                }
            }
            aF1.Close();
        }

        static void CCD()
        {
            byte[] b = new byte[8] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            int u;
            FileStream aF1 = new FileStream(@"E:\c#\Projects\PaliHan\indexdat", FileMode.Open);

            string[] paliABC = { "A", "Ā", "I", "Ī", "U", "Ū", "E", "O", "K", "G", "Ṅ", "C", "J", "Ñ", "Ṭ", "Ḍ", "Ṇ", "T", "D", "N", "P", "B", "M", "Y", "R", "L", "V", "S", "H", "Ḷ", "Ŋ" };

            string sIn = "akka";

            u = 0;
            for (int i = 0; i < sIn.Length; i++)
            {
                aF1.Seek(u + (iABC(sIn.Substring(i, 1)) - 1) * 8, SeekOrigin.Begin);
                aF1.Read(b, 0, 8);
                if ((int)(b[0]) == 0)
                {
                    Console.WriteLine("没查到！" + i);
                    return;
                }
                else if ((int)(b[0]) == 1)
                {
                    if (i < sIn.Length - 1)
                        u = (int)(b[1] | b[2] << 8 | b[3] << 16 | b[4] << 24);
                    else
                    {
                        Console.WriteLine("没查到1！" + i);
                        return;
                    }
                }
                else if ((int)(b[0]) == 2)
                {
                    if (i < sIn.Length - 1)
                    {
                        Console.WriteLine("没查到2！" + i);
                        return;
                    }
                    else
                        u = (int)(b[5] | b[6] << 8 | b[7] << 16 | 0);
                }
                else if ((int)(b[0]) == 3)
                {
                    if (i < sIn.Length - 1)
                        u = (int)(b[1] | b[2] << 8 | b[3] << 16 | b[4] << 24);
                    else
                        u = (int)(b[5] | b[6] << 8 | b[7] << 16 | 0);
                }

                Console.WriteLine(u);
            }
            FileStream bFile = new FileStream(@"E:\c#\Projects\PaliHan\词典.txt", FileMode.Open);
            StreamReader srb = new StreamReader(bFile, System.Text.Encoding.GetEncoding("utf-8"));
            srb.BaseStream.Seek(u, System.IO.SeekOrigin.Begin);
            Console.WriteLine(srb.ReadLine());
            srb.Close();
            aF1.Close();
        }

        //1-31数字转换成大写巴利字母
        public static string numtoABC(int num)
        {
            switch (num)
            {
                case 1:
                    return "A";

                case 2:
                    return "Ā";

                case 3:
                    return "I";

                case 4:
                    return "Ī";

                case 5:
                    return "U";

                case 6:
                    return "Ū";

                case 7:
                    return "E";

                case 8:
                    return "O";

                case 9:
                    return "K";

                case 10:
                    return "G";

                case 11:
                    return "Ṅ";

                case 12:
                    return "C";

                case 13:
                    return "J";

                case 14:
                    return "Ñ";

                case 15:
                    return "Ṭ";

                case 16:
                    return "Ḍ";

                case 17:
                    return "Ṇ";

                case 18:
                    return "T";

                case 19:
                    return "D";

                case 20:
                    return "N";

                case 21:
                    return "P";

                case 22:
                    return "B";

                case 23:
                    return "M";

                case 24:
                    return "Y";

                case 25:
                    return "R";

                case 26:
                    return "L";

                case 27:
                    return "V";

                case 28:
                    return "S";

                case 29:
                    return "H";

                case 30:
                    return "Ḷ";

                case 31:
                    return "Ṃ";

                default:
                    return " ";
            }
        }

        //1-31数字转换成小写巴利字母
        public static string numtoabc(int num)
        {
            switch (num)
            {
                case 1:
                    return "a";

                case 2:
                    return "ā";

                case 3:
                    return "i";

                case 4:
                    return "ī";

                case 5:
                    return "u";

                case 6:
                    return "ū";

                case 7:
                    return "e";

                case 8:
                    return "o";

                case 9:
                    return "k";

                case 10:
                    return "g";

                case 11:
                    return "ṅ";

                case 12:
                    return "c";

                case 13:
                    return "j";

                case 14:
                    return "ñ";

                case 15:
                    return "ṭ";

                case 16:
                    return "ḍ";

                case 17:
                    return "ṇ";

                case 18:
                    return "t";

                case 19:
                    return "d";

                case 20:
                    return "n";

                case 21:
                    return "p";

                case 22:
                    return "b";

                case 23:
                    return "m";

                case 24:
                    return "y";

                case 25:
                    return "r";

                case 26:
                    return "l";

                case 27:
                    return "v";

                case 28:
                    return "s";

                case 29:
                    return "h";

                case 30:
                    return "ḷ";

                case 31:
                    return "ṃ";

                default:
                    return " ";
            }
        }

        //从文本取得所有用到的字母
        public static void getcichang()
        {
            string strLine = "";
            //StreamReader sr = new StreamReader(new FileStream(@".\620.txt", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            StreamReader sr = new StreamReader(new FileStream(@".\dancipx.txt", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@".\abc.txt", false, System.Text.Encoding.GetEncoding("utf-8"));

            int p = 0, q = 0;
            string[] VriAbc = new string[200];

            strLine = sr.ReadLine();
            while (strLine != null)
            {
                if (strLine.Substring(7).Length > 21)
                    sw.WriteLine(strLine.Substring(7).Length + "%" + strLine);

                p++;
                if (p == q + 10000)
                {
                    q = 10000 + q;
                    Console.WriteLine(p);
                }

                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        public static void pb_em()
        {
            NUM = 26151;
            pbreaddc(@".\pali-h\cidian", @".\pali-h\edcb");

            for (int i = 0; i < NUM; ++i)
            {
                string tempd = sL[i];
                string temps = strL[i];
                int jj = i;
                while ((jj > 0) && eab(tempd, sL[jj - 1]))
                {
                    sL[jj] = sL[jj - 1];
                    strL[jj] = strL[jj - 1];
                    --jj;
                }
                sL[jj] = tempd;
                strL[jj] = temps;
            }

            StreamWriter sw1 = new StreamWriter(@".\Pali-h\emindex", false, System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw2 = new StreamWriter(@".\Pali-h\emcidian", false, System.Text.Encoding.GetEncoding(950));
            for (int k = 0; k < NUM; k++)
            {
                sw1.WriteLine(sL[k]);
                sw2.WriteLine(strL[k]);
            }
            sw1.Close();
            sw2.Close();
        }

        public static void pbreaddc(string cidianpath, string indexpath)
        {
            string strLine;
            int i = 0, j = 0;

            StreamReader sr = new StreamReader(new FileStream(cidianpath, FileMode.Open), System.Text.Encoding.GetEncoding(950));
            strLine = sr.ReadLine();
            while (strLine != null)
            {
                strL[i] = strLine;
                i++;
                strLine = sr.ReadLine();
            }
            sr.Close();

            StreamReader sr1 = new StreamReader(new FileStream(indexpath, FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            strLine = sr1.ReadLine();
            while (strLine != null)
            {
                sL[j] = strLine.Substring(8);
                j++;
                strLine = sr1.ReadLine();
            }
            sr1.Close();
        }

        public static void pb_em_readdc(string cidianpath, string indexpath)
        {
            string strLine;
            int i = 0, j = 0;

            StreamReader sr = new StreamReader(new FileStream(cidianpath, FileMode.Open), System.Text.Encoding.GetEncoding(950));
            strLine = sr.ReadLine();
            while (strLine != null)
            {
                strL[i] = strLine;
                i++;
                strLine = sr.ReadLine();
            }
            sr.Close();

            StreamReader sr1 = new StreamReader(new FileStream(indexpath, FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            strLine = sr1.ReadLine();
            while (strLine != null)
            {
                sL[j] = strLine;
                j++;
                strLine = sr1.ReadLine();
            }
            sr1.Close();
        }

        public static void pb1()
        {
            string strLine;

            FileStream aFile = new FileStream(@".\1.txt", FileMode.Open);
            StreamReader sr = new StreamReader(aFile, System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@".\2.txt", false, System.Text.Encoding.GetEncoding("utf-8"));

            strLine = sr.ReadToEnd();
            //[\u4e00-\u9fa5]
            char[] ca = "āīūṅñṭḍṇḷŋṁṃĀĪŪṄÑṬḌṆḶŊṀṂʨɕʕ½¼⅛⅖☿".ToCharArray();
            char[] cb = "abcdefghijklmnopqrstuvwxyz123456".ToCharArray();
            int i = 0;
            foreach (char c in ca)
            {
                strLine = new Regex(c.ToString(), RegexOptions.IgnoreCase).Replace(strLine, "&" + cb[i].ToString());
                i++;
            }
            sw.Write(strLine);

            sr.Close();
            sw.Close();
        }

        static void pbmakeindex()
        {
            string strLine, strLinec;

            StreamReader sr = new StreamReader(new FileStream(@".\pali-big5\cidian", FileMode.Open), System.Text.Encoding.GetEncoding(950));

            StreamReader src = new StreamReader(new FileStream(@".\pali-big5\gbindex.txt", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));

            StreamWriter sw = new StreamWriter(@".\pali-big5\index", false, System.Text.Encoding.GetEncoding("utf-8"));

            strLine = sr.ReadLine();
            strLinec = src.ReadLine();

            long lWz = 0;
            string slWz = "";

            while (strLine != null)
            {
                slWz = "" + lWz;
                slWz = slWz.PadLeft(7, '0');
                sw.WriteLine(slWz + "$" + strLinec.Substring(8));

                StreamWriter sw1 = new StreamWriter(@".\tmp.txt", true, System.Text.Encoding.GetEncoding(950));
                sw1.WriteLine(strLine);
                sw1.Close();

                StreamReader srb = new StreamReader(new FileStream(@".\tmp.txt", FileMode.Open), System.Text.Encoding.GetEncoding(950));
                lWz = srb.BaseStream.Length;
                srb.Close();

                strLine = sr.ReadLine();
                strLinec = src.ReadLine();
            }

            sr.Close();
            src.Close();
            sw.Close();
        }

        //em英文输入模式
        public static void em2_readdc(string cidianpath, string indexpath)
        {
            string strLine;
            int i = 0, j = 0;

            StreamReader sr = new StreamReader(new FileStream(cidianpath, FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            strLine = sr.ReadLine();
            while (strLine != null)
            {
                strL[i] = strLine;
                i++;
                strLine = sr.ReadLine();
            }
            sr.Close();

            StreamReader sr1 = new StreamReader(new FileStream(indexpath, FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            strLine = sr1.ReadLine();
            while (strLine != null)
            {
                sL[j] = strLine.Substring(2);
                j++;
                strLine = sr1.ReadLine();
            }
            sr1.Close();
        }

        public static void ph_em_px1()
        {
            NUM = 26151;
            em2_readdc(@".\pali-h\词典.txt", @".\pali-h\词典edcb.txt");

            for (int i = 0; i < NUM; ++i)
            {
                string tempd = sL[i];
                string temps = strL[i];
                int jj = i;
                while ((jj > 0) && eab(tempd, sL[jj - 1]))
                {
                    sL[jj] = sL[jj - 1];
                    strL[jj] = strL[jj - 1];
                    --jj;
                }
                sL[jj] = tempd;
                strL[jj] = temps;
            }

            StreamWriter sw1 = new StreamWriter(@".\Pali-h\emindex", false, System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw2 = new StreamWriter(@".\Pali-h\emcidian", false, System.Text.Encoding.GetEncoding("utf-8"));
            for (int k = 0; k < NUM; k++)
            {
                sw1.WriteLine(sL[k]);
                sw2.WriteLine(strL[k]);
            }
            sw1.Close();
            sw2.Close();
        }

        public static void ph_em_1()
        {
            string strLine;

            StreamReader sr = new StreamReader(new FileStream(@".\1.txt", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            strLine = sr.ReadToEnd();
            sr.Close();

            char[] ca = "āīūṅñṭḍṇḷŋĀĪŪṄÑṬḌṆḶŊṁṃṀṂ".ToCharArray();
            char[] cb = "aiunntdnlmAIUNNTDNLMmmMM".ToCharArray();
            int i = 0;
            foreach (char c in ca)
            {
                strLine = new Regex(c.ToString(), RegexOptions.None).Replace(strLine, cb[i].ToString());
                i++;
            }

            StreamWriter sw = new StreamWriter(@".\2.txt", false, System.Text.Encoding.GetEncoding("utf-8"));
            sw.Write(strLine);
            sw.Close();
        }

        public static void ph_em_px2()
        {
            NUM = 40646;
            readdc(@".\emcidian1", @".\emindex1");

            string tempd;
            string temps;
            for (int i = 0; i < NUM; ++i)
            {
                tempd = sL[i];
                temps = strL[i];
                int jj = i;
                while ((jj > 0) && eab(tempd, sL[jj - 1]))
                {
                    sL[jj] = sL[jj - 1];
                    strL[jj] = strL[jj - 1];
                    --jj;
                }
                sL[jj] = tempd;
                strL[jj] = temps;
                Console.WriteLine(i);
            }

            StreamWriter sw1 = new StreamWriter(@".\emindex", false, System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw2 = new StreamWriter(@".\emcidian", false, System.Text.Encoding.GetEncoding("utf-8"));
            //StreamWriter sw2 = new StreamWriter(@".\emcidian", false, System.Text.Encoding.GetEncoding(950));
            for (int k = 0; k < NUM; k++)
            {
                sw1.WriteLine(sL[k]);
                sw2.WriteLine(strL[k]);
            }
            sw1.Close();
            sw2.Close();
        }

        public static void em_readdc(string cidianpath, string indexpath)
        {
            string strLine;
            int i = 0, j = 0;

            StreamReader sr = new StreamReader(new FileStream(cidianpath, FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            strLine = sr.ReadLine();
            while (strLine != null)
            {
                strL[i] = strLine;
                i++;
                strLine = sr.ReadLine();
            }
            sr.Close();

            StreamReader sr1 = new StreamReader(new FileStream(indexpath, FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            strLine = sr1.ReadLine();
            while (strLine != null)
            {
                sL[j] = strLine;
                j++;
                strLine = sr1.ReadLine();
            }
            sr1.Close();
        }

        public static void paliEnglish_englishInputMode_1()
        {
            string strLine;

            StreamReader sr = new StreamReader(new FileStream(@".\Pali-e\index", FileMode.Open), System.Text.Encoding.GetEncoding("utf-8"));
            strLine = sr.ReadToEnd();
            sr.Close();

            char[] ca = "āīūṅñṭḍṇḷŋĀĪŪṄÑṬḌṆḶŊṁṃṀṂ".ToCharArray();
            char[] cb = "aiunntdnlmAIUNNTDNLMmmMM".ToCharArray();
            int i = 0;
            foreach (char c in ca)
            {
                strLine = new Regex(c.ToString(), RegexOptions.None).Replace(strLine, cb[i].ToString());
                i++;
            }

            StreamWriter sw = new StreamWriter(@".\Pali-e\index1.txt", false, System.Text.Encoding.GetEncoding("utf-8"));
            sw.Write(strLine);
            sw.Close();
        }

        public static void paliEnglish_englishInputMode_2()
        {
            NUM = 20787;
            //readdc(@".\pali-e\cidian", @".\pali-e\index1.txt");
            em_readdc(@".\pali-e\cidian", @".\pali-e\index");

            for (int i = 0; i < NUM; ++i)
            {
                string tempd = sL[i];
                string temps = strL[i];
                int jj = i;
                while ((jj > 0) && eab(tempd, sL[jj - 1]))
                {
                    sL[jj] = sL[jj - 1];
                    strL[jj] = strL[jj - 1];
                    --jj;
                }
                sL[jj] = tempd;
                strL[jj] = temps;
            }

            StreamWriter sw1 = new StreamWriter(@".\Pali-e\emindex", false, System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw2 = new StreamWriter(@".\Pali-e\emcidian", false, System.Text.Encoding.GetEncoding("utf-8"));
            for (int k = 0; k < NUM; k++)
            {
                sw1.WriteLine(sL[k]);
                sw2.WriteLine(strL[k]);
            }
            sw1.Close();
            sw2.Close();
        }

        static void pe1()
        {
            string strLine, stmp;

            FileStream aFile = new FileStream(@".\pali-e\pali-e.txt", FileMode.Open);
            StreamReader sr = new StreamReader(aFile, System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@".\pali-e\pali-e1.txt", false, System.Text.Encoding.GetEncoding("utf-8"));
            strLine = sr.ReadLine();
            while (strLine != null)
            {
                stmp = strLine.Trim();
                if (stmp != "")
                {
                    if (stmp.Substring(stmp.Length - 1) == ".")
                    {
                        MatchCollection mc = new Regex(@"^.*\([^\)]*\.\s*$", RegexOptions.IgnoreCase).Matches(strLine);
                        if (mc.Count > 0)
                            sw.Write(strLine.Substring(6));
                        else
                            sw.WriteLine(strLine.Substring(6));
                    }
                    else
                        sw.Write(strLine.Substring(6));
                }
                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        static void pe2()
        {
            string strLine, stmp;

            FileStream aFile = new FileStream(@".\pali-e\pali-e1.txt", FileMode.Open);
            StreamReader sr = new StreamReader(aFile, System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@".\pali-e\pali-e2.txt", false, System.Text.Encoding.GetEncoding("utf-8"));
            strLine = sr.ReadLine();
            while (strLine != null)
            {
                stmp = new Regex(@"\|\|\s*", RegexOptions.IgnoreCase).Replace(strLine, "\r\n");
                sw.WriteLine(stmp.TrimStart(' '));
                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        static void ep0()
        {
            string strLine, stmp;

            FileStream aFile = new FileStream(@".\e-pali\e-pali.txt", FileMode.Open);
            StreamReader sr = new StreamReader(aFile, System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@".\e-pali\e-pali0.txt", false, System.Text.Encoding.GetEncoding("utf-8"));

            strLine = sr.ReadToEnd();
            stmp = new Regex(@"(?<w1>[\w\)?,])\s*(?<w>\r\n^\s{6}[\w- ]*? :)", RegexOptions.Multiline).Replace(strLine, "${w1}.${w}");
            stmp = new Regex(@"\b(?<w>\r\n)", RegexOptions.Multiline).Replace(stmp, " ${w}");
            sw.Write(stmp);

            sr.Close();
            sw.Close();
        }

        static void ep1()
        {
            string strLine, stmp;

            FileStream aFile = new FileStream(@".\e-pali\e-pali0.txt", FileMode.Open);
            StreamReader sr = new StreamReader(aFile, System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@".\e-pali\e-pali1.txt", false, System.Text.Encoding.GetEncoding("utf-8"));
            strLine = sr.ReadLine();
            while (strLine != null)
            {
                stmp = strLine.Trim();
                if (stmp != "")
                {
                    if (stmp.Substring(stmp.Length - 1) == ".")
                    {
                        MatchCollection mc = new Regex(@"^.*\([^\)]*\.\s*$", RegexOptions.IgnoreCase).Matches(strLine);
                        if (mc.Count > 0)
                            sw.Write(strLine.Substring(6));
                        else
                            sw.WriteLine(strLine.Substring(6));
                    }
                    else
                        sw.Write(strLine.Substring(6));
                }
                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        static void ep2()
        {
            string strLine, stmp;

            FileStream aFile = new FileStream(@".\e-pali\e-pali1.txt", FileMode.Open);
            StreamReader sr = new StreamReader(aFile, System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@".\e-pali\e-pali2.txt", false, System.Text.Encoding.GetEncoding("utf-8"));
            strLine = sr.ReadLine();
            while (strLine != null)
            {
                stmp = new Regex(@"\|\|\s*", RegexOptions.IgnoreCase).Replace(strLine, "\r\n");
                sw.WriteLine(stmp.TrimStart(' '));
                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        static void ep3()
        {
            string strLine, stmp;

            FileStream aFile = new FileStream(@".\e-pali\e-pali2.txt", FileMode.Open);
            StreamReader sr = new StreamReader(aFile, System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@".\e-pali\e-pali3.txt", false, System.Text.Encoding.GetEncoding("utf-8"));

            strLine = sr.ReadToEnd();
            stmp = new Regex(@"\. \r\n\(", RegexOptions.IgnoreCase).Replace(strLine, @". (");
            stmp = new Regex(@":", RegexOptions.IgnoreCase).Replace(stmp, @" :");
            stmp = new Regex(@"  ", RegexOptions.IgnoreCase).Replace(stmp, @" ");
            stmp = new Regex(@"\r\n(?<w>[^:]+?\r\n)", RegexOptions.IgnoreCase).Replace(stmp, "${w}");
            stmp = new Regex(@"\r\n(?<w>[^:]+?\r\n)", RegexOptions.IgnoreCase).Replace(stmp, "${w}");
            sw.Write(stmp);

            sr.Close();
            sw.Close();
        }

        static void ep4()
        {
            string strLine, stmp;

            FileStream aFile = new FileStream(@".\e-pali\e-pali3.txt", FileMode.Open);
            StreamReader sr = new StreamReader(aFile, System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@".\e-pali\e-pali4.txt", false, System.Text.Encoding.GetEncoding("utf-8"));

            strLine = sr.ReadLine();
            while (strLine != null)
            {
                stmp = new Regex(@"(?<=[;!])\s*(?<w>[^\(\)]+?)(?=:)", RegexOptions.IgnoreCase).Replace(strLine, "\r\n${w}");
                sw.WriteLine(stmp);
                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        static void ep5()
        {
            string strLine;

            FileStream aFile = new FileStream(@".\e-pali\e-pali4.txt", FileMode.Open);
            StreamReader sr = new StreamReader(aFile, System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@".\e-pali\e-pali5.txt", false, System.Text.Encoding.GetEncoding("utf-8"));
            strLine = sr.ReadLine();
            while (strLine != null)
            {
                MatchCollection mc = new Regex(@"^(?<w>.+?) :", RegexOptions.Multiline).Matches(strLine);
                //MatchCollection mc = new Regex(@"^.*:.*:.*$", RegexOptions.Multiline).Matches(strLine);
                foreach (Match ma in mc)
                {
                    sw.WriteLine(ma.Groups["w"].Value);
                }
                strLine = sr.ReadLine();
            }
            sr.Close();
            sw.Close();
        }

        //词典按单词字母顺序排序
        static void ezmpx()
        {
            int NUM = 20000;
            string[] sL = new string[NUM];
            string[] strL = new string[NUM];
            string strLine;
            int z = 0;

            FileStream aFile = new FileStream(@".\1", FileMode.Open);
            StreamReader sr = new StreamReader(aFile, System.Text.Encoding.GetEncoding("utf-8"));
            strLine = sr.ReadLine();
            while (strLine != null)
            {
                MatchCollection mc = new Regex(@"^(?<w>.+?) :", RegexOptions.IgnoreCase).Matches(strLine);
                foreach (Match ma in mc)
                {
                    sL[z] = ma.Groups["w"].Value;
                    strL[z] = strLine;
                    z++;
                }
                strLine = sr.ReadLine();
            }
            sr.Close();

            for (int i = 0; i < NUM; ++i)
            {
                string tempd = sL[i];
                string temps = strL[i];
                int jj = i;
                while ((jj > 0) && eab(tempd, sL[jj - 1]))
                {
                    sL[jj] = sL[jj - 1];
                    strL[jj] = strL[jj - 1];
                    --jj;
                }
                sL[jj] = tempd;
                strL[jj] = temps;
            }
            StreamWriter sw2 = new StreamWriter(@".\2", false, System.Text.Encoding.GetEncoding("utf-8"));
            for (int k = 0; k < NUM; k++)
            {
                sw2.WriteLine(strL[k]);
            }
            sw2.Close();
        }

        static void emakeindex()
        {
            string strLine;
            int i = 0;

            FileStream aFile = new FileStream(@".\e-pali\cidian", FileMode.Open);
            StreamReader sr = new StreamReader(aFile, System.Text.Encoding.GetEncoding("utf-8"));
            StreamWriter sw = new StreamWriter(@".\e-pali\index", false, System.Text.Encoding.GetEncoding("utf-8"));

            strLine = sr.ReadLine();

            long lWz = 0;
            string slWz = "";

            while (strLine != null)
            {
                i = i + 1;
                string sWord = "";

                Regex re = new Regex(@"^(?<w>.+?) :", RegexOptions.IgnoreCase);
                MatchCollection mc = re.Matches(strLine);
                foreach (Match ma in mc)
                {
                    sWord = ma.Groups["w"].Value;
                }

                slWz = "" + lWz;
                slWz = slWz.PadLeft(7, '0');
                sw.WriteLine(slWz + "$" + sWord);

                StreamWriter sw1 = new StreamWriter(@".\e-pali\tmp.txt", true, System.Text.Encoding.GetEncoding("utf-8"));
                sw1.WriteLine(strLine);
                sw1.Close();

                FileStream bFile = new FileStream(@".\e-pali\tmp.txt", FileMode.Open);
                StreamReader srb = new StreamReader(bFile, System.Text.Encoding.GetEncoding("utf-8"));
                lWz = srb.BaseStream.Length;
                srb.Close();

                strLine = sr.ReadLine();
            }

            sr.Close();
            sw.Close();
        }

        //编制e-pali词目索引库indexdat. -号和空格视为同一字符
        static void ezjbz()
        {
            string stext, sword;
            FileStream aFile = new FileStream(@".\e-pali\index", FileMode.Open);
            StreamReader sr = new StreamReader(aFile, System.Text.Encoding.GetEncoding("utf-8"));
            FileStream aF1 = new FileStream(@".\e-pali\indexdat", FileMode.Open);
            int ist, fdz, zdz, dcdz, maxblockno;//dcdz单词在词典中的地址
            byte[] by1 = new byte[8] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            byte[] b = new byte[216] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            maxblockno = 1;

            aF1.Write(b, 0, 27 * 8);

            stext = sr.ReadLine();
            while (stext != null)
            {
                sword = stext.Substring(8);
                ist = sword.Length;
                fdz = 0;
                for (int i = 0; i < ist; i++)
                {
                    aF1.Seek(fdz + eABC(sword.Substring(i, 1)) * 8, SeekOrigin.Begin);
                    aF1.Read(by1, 0, 8);
                    if ((int)(by1[0]) == 0)//还没有被使用
                    {
                        zdz = maxblockno * 27 * 8;

                        if (i < ist - 1)
                        {
                            by1[0] = 1;
                            by1[1] = (byte)(zdz);
                            by1[2] = (byte)(zdz >> 8);
                            by1[3] = (byte)(zdz >> 16);
                            by1[4] = (byte)(zdz >> 24);
                            aF1.Seek(fdz + eABC(sword.Substring(i, 1)) * 8, SeekOrigin.Begin);
                            aF1.Write(by1, 0, 8);

                            fdz = maxblockno * 27 * 8;

                            aF1.Seek(maxblockno * 27 * 8, SeekOrigin.Begin);
                            aF1.Write(b, 0, 27 * 8);
                            maxblockno++;
                        }
                        else
                        {
                            dcdz = int.Parse(stext.Substring(0, 7));
                            by1[0] = 2;
                            by1[5] = (byte)(dcdz);
                            by1[6] = (byte)(dcdz >> 8);
                            by1[7] = (byte)(dcdz >> 16);
                            aF1.Seek(fdz + eABC(sword.Substring(i, 1)) * 8, SeekOrigin.Begin);
                            aF1.Write(by1, 0, 8);
                        }
                    }
                    else if ((int)(by1[0]) == 1)//只是路
                    {
                        if (i < ist - 1)
                        {
                            fdz = (int)(by1[1] | by1[2] << 8 | by1[3] << 16 | by1[4] << 24);
                        }
                        else
                        {
                            dcdz = int.Parse(stext.Substring(0, 7));
                            by1[0] = 3;
                            by1[5] = (byte)(dcdz);
                            by1[6] = (byte)(dcdz >> 8);
                            by1[7] = (byte)(dcdz >> 16);
                            aF1.Seek(fdz + eABC(sword.Substring(i, 1)) * 8, SeekOrigin.Begin);
                            aF1.Write(by1, 0, 8);
                        }
                    }
                    else if ((int)(by1[0]) == 2)//只是单词不是路
                    {
                        zdz = maxblockno * 27 * 8;

                        if (i < ist - 1)
                        {
                            by1[0] = 3;
                            by1[1] = (byte)(zdz);
                            by1[2] = (byte)(zdz >> 8);
                            by1[3] = (byte)(zdz >> 16);
                            by1[4] = (byte)(zdz >> 24);
                            aF1.Seek(fdz + eABC(sword.Substring(i, 1)) * 8, SeekOrigin.Begin);
                            aF1.Write(by1, 0, 8);

                            fdz = maxblockno * 27 * 8;

                            aF1.Seek(maxblockno * 27 * 8, SeekOrigin.Begin);
                            aF1.Write(b, 0, 27 * 8);
                            maxblockno++;
                        }
                        else
                        {
                            dcdz = int.Parse(stext.Substring(0, 7));
                            by1[0] = 2;
                            by1[5] = (byte)(dcdz);
                            by1[6] = (byte)(dcdz >> 8);
                            by1[7] = (byte)(dcdz >> 16);
                            aF1.Seek(fdz + eABC(sword.Substring(i, 1)) * 8, SeekOrigin.Begin);
                            aF1.Write(by1, 0, 8);
                        }
                    }
                    else if ((int)(by1[0]) == 3)//3是路又是单词
                    {
                        if (i < ist - 1)
                        {
                            fdz = (int)(by1[1] | by1[2] << 8 | by1[3] << 16 | by1[4] << 24);
                        }
                        else
                        {
                            dcdz = int.Parse(stext.Substring(0, 7));
                            by1[0] = 3;
                            by1[5] = (byte)(dcdz);
                            by1[6] = (byte)(dcdz >> 8);
                            by1[7] = (byte)(dcdz >> 16);
                            aF1.Seek(fdz + eABC(sword.Substring(i, 1)) * 8, SeekOrigin.Begin);
                            aF1.Write(by1, 0, 8);
                        }
                    }
                }
                stext = sr.ReadLine();
            }
            sr.Close();
            aF1.Close();
        }

        /// <summary>
        /// 词库词条数量
        /// </summary>
        public static int NUM;

        /// <summary>
        /// 储存词目
        /// </summary>
        public static string[] sL;

        /// <summary>
        /// 储存与词目对应的词条解释内容
        /// </summary>
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
