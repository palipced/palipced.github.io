using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace makedict
{
    class Program
    {
        static void Main(string[] args)
        {
            vvvz();

            Console.ReadKey();
        }

        public static void vvvz()
        {
            if (!(Directory.Exists(@".\new")))
                Directory.CreateDirectory(@".\new");

            string strLine = "";
            string ptn = @".hangnum { font-size: 12pt; position:relative; left:0px; top:38px; z-index:-1; text-indent: 2em;}";
            int i = 0;
            foreach (string sfilename in Directory.GetFiles(@".\pali\"))
            {
                StreamReader sr = new StreamReader(new FileStream(sfilename, FileMode.Open), System.Text.Encoding.GetEncoding(65001));
                StreamWriter sw = new StreamWriter(@".\new\" + sfilename.Substring(7), false, System.Text.Encoding.GetEncoding(65001));
                strLine = sr.ReadToEnd();
                strLine = new Regex(ptn, RegexOptions.None).Replace(strLine, ".hangnum { font-size: 12pt; text-indent: 2em;}");
                sw.Write(strLine);
                sr.Close();
                sw.Close();

                i++;
                Console.WriteLine(i.ToString() + "#   " + sfilename);
                Console.WriteLine("       " + @".\new\" + sfilename.Substring(7));
            }

            Console.WriteLine("OK.");
            Console.WriteLine("新的保存在‘new’目录下，请把它的名字改为‘pali’，而把旧的‘pali’目录移走、做好备份即可。");
            Console.WriteLine("按回车键退出。");
        }
    }
}
