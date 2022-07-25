package com.peiyh.pcedandcanon;
/////
import android.app.Activity;
import android.app.Application;
import android.content.ClipData;
import android.content.ClipboardManager;
import android.content.Context;
import android.content.res.ColorStateList;
import android.content.res.TypedArray;
import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteException;
import android.graphics.Color;
import android.graphics.Typeface;

import androidx.annotation.IntDef;
import androidx.annotation.NonNull;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import android.text.Html;
import android.text.Spannable;
import android.text.SpannableString;
import android.text.Spanned;
import android.text.style.ForegroundColorSpan;
import android.util.TypedValue;
import android.view.DragEvent;
import android.view.LayoutInflater;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewGroup;
import android.webkit.WebView;
import android.widget.BaseAdapter;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.Spinner;
import android.widget.TextView;
import android.widget.Toast;

import java.io.FileOutputStream;
import java.io.InputStream;
import java.io.OutputStream;
import java.util.Enumeration;
import java.util.HashMap;
import java.util.List;

/////
import java.io.File;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.RandomAccessFile;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;

import org.apache.http.util.EncodingUtils;


import android.widget.AdapterView;
import android.widget.ListView;

import android.annotation.SuppressLint;
import android.app.ActionBar;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.os.Bundle;
import android.os.Handler;
import android.os.Environment;
import androidx.drawerlayout.widget.DrawerLayout;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.ActionMode;
import android.view.Gravity;
import android.view.KeyEvent;
import android.view.Menu;
import android.view.MenuItem;
import android.view.inputmethod.InputMethodManager;
import android.webkit.ValueCallback;
import android.webkit.WebChromeClient;
import android.webkit.WebSettings;
import android.webkit.JavascriptInterface;

import android.widget.Button;
import android.widget.EditText;
import android.widget.ProgressBar;

import com.google.android.material.bottomnavigation.BottomNavigationView;

import java.util.zip.ZipEntry;
import java.util.zip.ZipException;
import java.util.zip.ZipFile;

public class CanonActivity extends Activity {

    //记下三藏目录的父祖ID,以备返回父层目录之用
    public String catalogGrandpaID;
    public String catalogParentID;

    //设置变量显示中文或巴利文
    public String txtcode="chinese";

    //定义html页面的背景和文字颜色，以配合app的主题颜色
    public String htmlColor = "";

    //android颜色整数值转换为十六进制字符串，以在html里使用
    public String toHexEncoding(int color) {
        String R, G, B;
        StringBuffer sb = new StringBuffer();
        R = Integer.toHexString(Color.red(color));
        G = Integer.toHexString(Color.green(color));
        B = Integer.toHexString(Color.blue(color));
        //判断获取到的R,G,B值的长度 如果长度等于1 给R,G,B值的前边添0
        R = R.length() == 1 ? "0" + R : R;
        G = G.length() == 1 ? "0" + G : G;
        B = B.length() == 1 ? "0" + B : B;
        //sb.append("0x");
        sb.append("#");
        sb.append(R);
        sb.append(G);
        sb.append(B);
        return sb.toString();
    }

    //对三个webview进行设置：根本、义注、复注，三个浏览器窗口
    public void webviewTiSet(){
        wvMula.setWebChromeClient(new WebChromeClient() {
            @Override
            public void onProgressChanged(WebView view, int newProgress) {
                super.onProgressChanged(view, newProgress);
                if (newProgress == 100) {
                    prbarmulu.setVisibility(View.INVISIBLE);
                    prbar.setVisibility(View.INVISIBLE);

                    wvMula.evaluateJavascript("javascript:tochapter()", new ValueCallback<String>() {
                        @Override
                        public void onReceiveValue(String value) {
                            //Toast.makeText(CanonActivity.this,value,Toast.LENGTH_SHORT).show();
                        }
                    });

                }
            }
        });
        wvMula.getSettings().setJavaScriptEnabled(true);

        wvAtth.setWebChromeClient(new WebChromeClient() {
            @Override
            public void onProgressChanged(WebView view, int newProgress) {
                super.onProgressChanged(view, newProgress);
                if (newProgress == 100) {
                    prbarmulu.setVisibility(View.INVISIBLE);
                    prbar.setVisibility(View.INVISIBLE);

                    wvAtth.evaluateJavascript("javascript:tochapter()", new ValueCallback<String>() {
                        @Override
                        public void onReceiveValue(String value) {

                        }
                    });
                }
            }
        });
        wvAtth.getSettings().setJavaScriptEnabled(true);

        wvTika.setWebChromeClient(new WebChromeClient() {
            @Override
            public void onProgressChanged(WebView view, int newProgress) {
                super.onProgressChanged(view, newProgress);
                if (newProgress == 100) {
                    prbarmulu.setVisibility(View.INVISIBLE);
                    prbar.setVisibility(View.INVISIBLE);

                    wvTika.evaluateJavascript("javascript:tochapter()", new ValueCallback<String>() {
                        @Override
                        public void onReceiveValue(String value) {

                        }
                    });
                }
            }
        });
        wvTika.getSettings().setJavaScriptEnabled(true);
    }

    public class FruitAdapter extends RecyclerView.Adapter<FruitAdapter.ViewHolder> {

        private List<Fruit> mFruitList;

        public class ViewHolder extends RecyclerView.ViewHolder{
            View fruitView;

            ImageView fruitImage;
            TextView fruitName;
            ImageView inmuluImage;

            public ViewHolder(@NonNull View itemView) {
                super(itemView);

                fruitView = itemView;

                fruitImage=(ImageView)itemView.findViewById(R.id.fruit_image);
                fruitName=(TextView)itemView.findViewById(R.id.fruit_name);
                ///设置字体
                Typeface tf = Typeface.createFromFile(sdcDir+"/ZawgyiOne2008.ttf");
                fruitName.setTypeface(tf);
                ///
                inmuluImage=(ImageView)itemView.findViewById(R.id.inmulu);
            }
        }

        public  FruitAdapter(List<Fruit> fruitList){
            mFruitList=fruitList;
        }

        /////
        private void initFruits(Fruit fruit){

            SQLiteDatabase palicanondb =null;

            try{
                palicanondb=SQLiteDatabase.openDatabase(sdcDir+"/palihtm.db3",null,SQLiteDatabase.OPEN_READONLY);
            }catch (SQLiteException e){
                Toast.makeText(CanonActivity.this,e.getMessage(),Toast.LENGTH_LONG).show();
                return;
            }

            Cursor palidbcursor = palicanondb.rawQuery("SELECT rowid,* FROM mulu WHERE PARENTID ='"+fruit.getID()+"';",null);

            if (palidbcursor.moveToFirst()){

                fruitList.clear();

                do{
                    String s=palidbcursor.getString(palidbcursor.getColumnIndex("txt"));
                    if(s.equals("")){s=palidbcursor.getString(palidbcursor.getColumnIndex("tooltip"));}
                    Fruit apple = new Fruit(palidbcursor.getString(palidbcursor.getColumnIndex("rootid")),palidbcursor.getString(palidbcursor.getColumnIndex("filename")),palidbcursor.getString(palidbcursor.getColumnIndex("txt")),palidbcursor.getString(palidbcursor.getColumnIndex("tooltip")),palidbcursor.getString(palidbcursor.getColumnIndex("ISLEAF")),palidbcursor.getString(palidbcursor.getColumnIndex("PIANPARENT")),palidbcursor.getString(palidbcursor.getColumnIndex("ID")),s,android.R.drawable.ic_menu_compass, -1,R.color.Gainsboro);
                    fruitList.add(apple);

                }while(palidbcursor.moveToNext());

                rvMulu.setAdapter(new FruitAdapter(fruitList));

                catalogGrandpaID=catalogParentID;
                catalogParentID=fruit.getID();
            }

            palidbcursor.close();
        }
        /////
        /////
        private void openBook(Fruit fruit){

            //rootid=1,546,746,904分别属于：根本、义注、复注、其它
            String rootid=fruit.getRootid();

            //‘目录’的isleaf标志设为-1，这些纯粹的都是目录，‘卷’文件中74个有子的isleaf设为0，它们既是目录又是文件，这两种都作为目录处理；‘卷’中143个无子文件isleaf设为1它们是纯书，‘篇’720个是文件书中的节点它们的sileaf设为2
            String bookSign=fruit.getISLEAF();
            //经典htm文件名
            String bookFilename="";
            //htm文件中的章节标记
            String chapterMark="";
            //htm文件开头
            String htmStart="<html>  <head>    <META http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">    <title></title><script>function setHitsVisibility(isVisible){   var c = getStyleClass('hit');  if (isVisible)  {    c.style.backgroundColor = 'blue';    c.style.color = 'white';  }  else  {    c.style.backgroundColor = 'white';    c.style.color = 'black';  }}function setFootnotesVisibility(isVisible){  getStyleClass('note').style.display = (isVisible ? 'inline' : 'none');}function getStyleClass (className) {\tfor (var s = 0; s < document.styleSheets.length; s++)\t{\t\tif(document.styleSheets[s].rules)\t\t{\t\t\tfor (var r = 0; r < document.styleSheets[s].rules.length; r++)\t\t\t{\t\t\t\tif (document.styleSheets[s].rules[r].selectorText == '.' + className)\t\t\t\t{\t\t\t\t\treturn document.styleSheets[s].rules[r];\t\t\t\t}\t\t\t}\t\t}\t\telse if(document.styleSheets[s].cssRules)\t\t{\t\t\tfor (var r = 0; r < document.styleSheets[s].cssRules.length; r++)\t\t\t{\t\t\t\tif (document.styleSheets[s].cssRules[r].selectorText == '.' + className)\t\t\t\t\treturn document.styleSheets[s].cssRules[r];\t\t\t}\t\t}\t}\treturn null;}</script>" + htmlColor + "<style>body {   font-family: \"Times Ext Roman\", \"Indic Times\", \"Doulos SIL\", Tahoma, \"Arial Unicode MS\", Gentium; } span {} .note {color: #0066ff} .bld {font-weight: bold; } .paranum {font-weight: bold; } .hit {background-color: blue; color: white; } .context {background-color: green; color: white; } p {  border-top: 0in; border-bottom: 0in;  padding-top: 0in; padding-bottom: 0in;  margin-top: 0in; margin-bottom: 0.5cm;} .indent { font-size: 12pt; text-indent: 2em; margin-left: 3em;} .bodytext { font-size: 12pt; text-indent: 2em;} .hangnum { font-size: 12pt; text-indent: 2em;} /* Namo tassa, and nitthita -- no unique structural distinction */ .centered { font-size: 12pt; text-align:center;} .unindented { font-size: 12pt;} .book { font-size: 21pt; text-align:center; font-weight: bold;} .chapter { font-size: 18pt; text-align:center; font-weight: bold;} .nikaya { font-size: 24pt; text-align:center; font-weight: bold;} .title { font-size: 12pt; text-align:center; font-weight: bold;} .subhead { font-size: 12pt; text-align:center; font-weight: bold;} .subsubhead { font-size: 12pt; text-align:center; font-weight: bold;} /* Gatha line 1 */ .gatha1 { font-size: 12pt; margin-bottom: 0em; margin-left: 4em;} /* Gatha line 2 */ .gatha2 { font-size: 12pt; margin-bottom: 0em; margin-left: 4em;} /* Gatha line 3 */ .gatha3 { font-size: 12pt; margin-bottom: 0em; margin-left: 4em;} /* Gatha last line */ .gathalast { font-size: 12pt; margin-bottom: 0.5cm; margin-left: 4em;} </style>  </head>  <body>";
            //htm文件结尾
            String htmEnd="</body></html>";
            String minrowid="0";
            String maxrowid="0";

            StringBuilder sb = new StringBuilder(2400256);

            if(bookSign.equals("1")){
                bookFilename=fruit.getFilename()+".htm";
                chapterMark="";
            }else if(bookSign.equals("2")){
                bookFilename=fruit.getPIANPARENT()+".htm";
                chapterMark=fruit.getFilename();
            }else{
                return;
            }

            SQLiteDatabase palicanondb =null;

            try{
                palicanondb=SQLiteDatabase.openDatabase(sdcDir+"/palihtm.db3",null,SQLiteDatabase.OPEN_READONLY);
            }catch (SQLiteException e){
                Toast.makeText(CanonActivity.this,e.getMessage(),Toast.LENGTH_LONG).show();
                return;
            }

            Cursor palidbcursor = palicanondb.rawQuery("SELECT minrowid,maxrowid FROM bookline WHERE filename ='"+bookFilename+"';",null);

            if (palidbcursor.moveToFirst()){
                minrowid=palidbcursor.getString(palidbcursor.getColumnIndex("minrowid"));
                maxrowid=palidbcursor.getString(palidbcursor.getColumnIndex("maxrowid"));
            }

            palidbcursor = palicanondb.rawQuery("SELECT palihtm FROM palihtm WHERE rowid>="+minrowid+" and rowid<="+maxrowid+";",null);

            if (palidbcursor.moveToFirst()){
                sb.append(htmStart);

                do{
                    sb.append(palidbcursor.getString(palidbcursor.getColumnIndex("palihtm")));
                }while(palidbcursor.moveToNext());

                //sb.append(htmEnd);
            }

            palidbcursor.close();

            //prbar.setVisibility(View.VISIBLE);

            //prbarmulu.setVisibility(View.VISIBLE);

            //webviewMainwin.loadDataWithBaseURL("about:blank", sb.toString(), "text/html", "utf-8", null);

            /////

            String s = sb.toString();

            //下面这个替换没考虑带下划线的段号标记
            s = s.replaceAll("-\\d+\"", "\"");
            s = s.replaceAll("<a name=\"([VMPT]\\d+\\.\\d+)\"></a>", "$0<i style=\"color:#3366CC;\">[$1]</i>");
            s = s.replaceAll("name=\"", "id=\"");

            String sfunparadingwei = " function tochapter(){document.getElementById('"+chapterMark+"').scrollIntoView();} "+" function topara(n,m,k){/*三藏经文中段号最大值:3183*/var isok = document.getElementById('para'+n);if(isok==null){n=n-1;if(n>=1){topara(n,m,k);}else{if(m>0){document.getElementById('para'+m).scrollIntoView();}}}else{if(n<=k){document.getElementById('para'+n).scrollIntoView();}else{/*储存下最近的大于k的n值*/m = n;n=n-1;if(n>=1){topara(n,m,k);}}}}";
            s = s + "<script>window.onload=function(){document.getElementById('para365').scrollIntoView();return \"168\";};" + sfunparadingwei + "function ttt(pagenum){document.getElementById(pagenum).scrollIntoView();return \"169\";}function tttop(){scrollTo(0,0);} function gettext(){return window.getSelection().toString();} </script>";

            prbar.setVisibility(View.VISIBLE);
            prbarmulu.setVisibility(View.VISIBLE);

            if((rootid.equals("1"))||(rootid.equals("904"))) {
                openMula();
                navigation.setSelectedItemId(R.id.navitemMula);
                wvMula.findAllAsync("xxxyyyzzz");
                wvMula.setScrollX(0);
                wvMula.setScrollY(0);
                wvMula.loadDataWithBaseURL("about:blank", s, "text/html", "utf-8", null);
            }

            if(rootid.equals("546")) {
                openAtth();
                navigation.setSelectedItemId(R.id.navitemYizhu);
                wvAtth.findAllAsync("xxxyyyzzz");
                wvAtth.setScrollX(0);
                wvAtth.setScrollY(0);
                wvAtth.loadDataWithBaseURL("about:blank", s, "text/html", "utf-8", null);
            }

            if(rootid.equals("746")) {
                openTika();
                navigation.setSelectedItemId(R.id.navitemFuzhu);
                wvTika.findAllAsync("xxxyyyzzz");
                wvTika.setScrollX(0);
                wvTika.setScrollY(0);
                wvTika.loadDataWithBaseURL("about:blank", s, "text/html", "utf-8", null);
            }

            //TextView mainwintitle = (TextView) findViewById(R.id.mainwintitle);
            //mainwintitle.setText(fruit.getTxt());

            /////
        }

        @NonNull
        @Override
        public ViewHolder onCreateViewHolder(@NonNull ViewGroup viewGroup, int i) {
            View view = LayoutInflater.from(viewGroup.getContext()).inflate(R.layout.fruit_item,viewGroup,false);
            final ViewHolder holder = new ViewHolder(view);

            holder.fruitView.setOnClickListener(new View.OnClickListener(){
                @Override
                public void onClick(View v) {
                    int position=holder.getAdapterPosition();
                    Fruit fruit=mFruitList.get(position);

                    //holder.fruitView.setBackgroundColor(getResources().getColor(R.color.material_on_background_emphasis_medium));
                    //////////////////////////////////////////////
                    initFruits(fruit);
                    //////////////////////////////////////////////
                    openBook(fruit);
                    //Toast.makeText(v.getContext(),"View:"+fruit.getName(),Toast.LENGTH_SHORT).show();
                }
            });

            /*
            holder.fruitImage.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    int position=holder.getAdapterPosition();
                    Fruit fruit=mFruitList.get(position);
                    Toast.makeText(v.getContext(),"Image:"+fruit.getName(),Toast.LENGTH_SHORT).show();
                }
            });*/

            return holder;
        }

        @Override
        public void onBindViewHolder(@NonNull ViewHolder viewHolder, int i) {
            Fruit fruit = mFruitList.get(i);

            if(fruit.getBackcolor()==R.color.AliceBlue) {
                viewHolder.fruitView.setBackgroundColor(getResources().getColor(R.color.material_on_background_emphasis_medium));
            }

            viewHolder.fruitName.setTextSize(TypedValue.COMPLEX_UNIT_SP,16);
            if(txtcode.equals("pali")){
                viewHolder.fruitName.setText(fruit.getTxt());
            }else{
                if(fruit.getTooltip().equals("")){
                    viewHolder.fruitName.setText(fruit.getTxt());
                }else{
                    viewHolder.fruitName.setText(fruit.getTooltip());
                }
            }

            if((fruit.getISLEAF().equals("-1"))||(fruit.getISLEAF().equals("0"))){
                viewHolder.fruitImage.setImageResource(R.drawable.ic_baseline_folder_24);
                viewHolder.inmuluImage.setImageResource(R.drawable.ic_baseline_chevron_right_24);
            }else{
                viewHolder.fruitImage.setImageResource(R.drawable.ic_baseline_text_snippet_24);
                viewHolder.inmuluImage.setImageResource(-1);
            }
        }

        @Override
        public int getItemCount() {
            return mFruitList.size();
        }

        public List<Fruit> getList() {
            return mFruitList;
        }
    }

    public class TreeAdapter extends BaseAdapter {
        private Context mcontext;
        private Activity mActivity;
        private List<TreePoint> pointList;
        private String keyword = "";
        private HashMap<String, TreePoint> pointMap = new HashMap<>();

        private int operateMode = ModeSelect;
        //两种操作模式  点击 或者 选择
        private static final int ModeClick = 1;
        private static final int ModeSelect = 2;
/*
        @IntDef({ModeClick,ModeSelect})
        public @interface Mode{

        }*/

        //设置操作模式
        //public void setOperateMode(@Mode int operateMode){
        public void setOperateMode(int operateMode) {
            this.operateMode = operateMode;
        }

        public TreeAdapter(final Context mcontext, List<TreePoint> pointList, HashMap<String, TreePoint> pointMap) {
            this.mcontext = mcontext;
            this.mActivity = (Activity) mcontext;
            this.pointList = pointList;
            this.pointMap = pointMap;
        }

        /**
         * 搜索的时候，先关闭所有的条目，然后，按照条件，找到含有关键字的数据
         * 如果是叶子节点，
         */
        public void setKeyword(String keyword) {
            this.keyword = keyword;
            for (TreePoint treePoint : pointList) {
                treePoint.setExpand(false);
            }
            if (!"".equals(keyword)) {
                for (TreePoint tempTreePoint : pointList) {
                    if (tempTreePoint.getNNAME().contains(keyword)) {
                        if ("0".equals(tempTreePoint.getISLEAF())) {   //非叶子节点
                            tempTreePoint.setExpand(true);
                        }
                        //展开从最顶层到该点的所有节点
                        openExpand(tempTreePoint);
                    }
                }
            }
            this.notifyDataSetChanged();
        }

        /**
         * 从TreePoint开始一直展开到顶部
         *
         * @param treePoint
         */
        private void openExpand(TreePoint treePoint) {
            if ("0".equals(treePoint.getPARENTID())) {
                treePoint.setExpand(true);
            } else {
                pointMap.get(treePoint.getPARENTID()).setExpand(true);
                openExpand(pointMap.get(treePoint.getPARENTID()));
            }
        }


        //第一要准确计算数量
        @Override
        public int getCount() {
            int count = 0;
            for (TreePoint tempPoint : pointList) {
                if ("0".equals(tempPoint.getPARENTID())) {
                    count++;
                } else {
                    if (getItemIsExpand(tempPoint.getPARENTID())) {
                        count++;
                    }
                }
            }
            return count;
        }

        //判断当前Id的tempPoint是否展开了
        private boolean getItemIsExpand(String ID) {
            for (TreePoint tempPoint : pointList) {
                if (ID.equals(tempPoint.getID())) {
                    return tempPoint.isExpand();
                }
            }
            return false;
        }

        @Override
        public Object getItem(int position) {
            return pointList.get(convertPostion(position));
        }

        private int convertPostion(int position) {
            int count = 0;
            for (int i = 0; i < pointList.size(); i++) {
                TreePoint treePoint = pointList.get(i);
                if ("0".equals(treePoint.getPARENTID())) {
                    count++;
                } else {
                    if (getItemIsExpand(treePoint.getPARENTID())) {
                        count++;
                    }
                }
                if (position == (count - 1)) {
                    return i;
                }
            }
            return 0;
        }

        @Override
        public long getItemId(int position) {
            return position;
        }


        @Override
        public View getView(int position, View convertView, ViewGroup parent) {
            ViewHolder holder;
            if (convertView == null) {
                convertView = LayoutInflater.from(mcontext).inflate(R.layout.adapter_treeview, null);
                holder = new ViewHolder();
                holder.text = (TextView) convertView.findViewById(R.id.txtMulu);
                holder.icon = (ImageView) convertView.findViewById(R.id.imgMulu);
                convertView.setTag(holder);
            } else {
                holder = (ViewHolder) convertView.getTag();
            }
            final TreePoint tempPoint = (TreePoint) getItem(position);
            int level = TreeUtils.getLevel(tempPoint, pointMap);
            holder.icon.setPadding(50 * level, holder.icon.getPaddingTop(), 0, holder.icon.getPaddingBottom());
            if ("0".equals(tempPoint.getISLEAF())) {  //如果为父节点
                if (!tempPoint.isExpand()) {    //不展开显示加号
                    holder.icon.setVisibility(View.VISIBLE);
                    holder.icon.setImageResource(R.drawable.open);
                } else {                        //展开显示减号
                    holder.icon.setVisibility(View.VISIBLE);
                    holder.icon.setImageResource(R.drawable.close);
                }
            } else {   //如果叶子节点，不占位显示
                holder.icon.setVisibility(View.INVISIBLE);
            }

            //如果存在搜索关键字
            if (keyword != null && !"".equals(keyword) && tempPoint.getNNAME().contains(keyword)) {
                int index = tempPoint.getNNAME().indexOf(keyword);
                int len = keyword.length();
                Spanned temp = Html.fromHtml(tempPoint.getNNAME().substring(0, index)
                        + "<font color=#FF0000>"
                        + tempPoint.getNNAME().substring(index, index + len) + "</font>"
                        + tempPoint.getNNAME().substring(index + len, tempPoint.getNNAME().length()));

                holder.text.setText(temp);
            } else {
                holder.text.setText(tempPoint.getNNAME());
            }
            holder.text.setCompoundDrawablePadding(DensityUtil.dip2px(mcontext, 10));
            return convertView;
        }

        //收缩打开的节点及其所有的各级子节点
        public void closeNode(TreePoint treePoint) {
            for (TreePoint tempPoint : pointList) {
                if (tempPoint.getPARENTID() == treePoint.getID()) {
                    if ("0".equals(tempPoint.getISLEAF())) {
                        if (tempPoint.isExpand()) {
                            closeNode(tempPoint);
                        }
                    }
                }
            }
            treePoint.setExpand(false);
        }

        public void onItemClick(int position) {
            TreePoint treePoint = (TreePoint) getItem(position);
            if ("1".equals(treePoint.getISLEAF())) {   //点击叶子节点
                //处理回填
                /////
                /*
                new Thread(new Runnable() {
                    @Override
                    public void run() {
                        Message message = new Message();
                        message.what = 1;
                        handler.sendMessage(message);
                    }
                }).start();*/
                try {
                    /*
                    File file = new File(sdcDir, "/pceddata/pali/" + treePoint.getfilename() + ".htm");
                    RandomAccessFile rf = new RandomAccessFile(file, "r");
                    byte[] buffer = new byte[(int) (rf.length())];
                    rf.seek(0);
                    rf.read(buffer);
                    rf.close();
                    String s = EncodingUtils.getString(buffer, "UTF-8");
                    */
                    String s = readHtm(treePoint.getfilename() + ".htm");

                    prbar.setVisibility(View.VISIBLE);

                    prbarmulu.setVisibility(View.VISIBLE);

                    s = s.replaceAll("-\\d+\"", "\"");
                    s = s.replaceAll("<a name=\"([VMPT]\\d+\\.\\d+)\"></a>", "$0<i style=\"color:#3366CC;\">[$1]</i>");
                    s = s.replaceAll("name=\"", "id=\"");
                    //s="<script>"+"window.onload=function ttt(){document.getElementById('para100').scrollIntoView();}"+" </script>"+s;
                    //段落定位函数
                    String sfunparadingwei = "function topara(n,m,k){/*三藏经文中段号最大值:3183*/var isok = document.getElementById('para'+n);if(isok==null){n=n-1;if(n>=1){topara(n,m,k);}else{if(m>0){document.getElementById('para'+m).scrollIntoView();}}}else{if(n<=k){document.getElementById('para'+n).scrollIntoView();}else{/*储存下最近的大于k的n值*/m = n;n=n-1;if(n>=1){topara(n,m,k);}}}}";
                    s = "<script>window.onload=function(){document.getElementById('para365').scrollIntoView();return \"168\";};" + sfunparadingwei + "function ttt(pagenum){document.getElementById(pagenum).scrollIntoView();return \"169\";}function tttop(){scrollTo(0,0);} function gettext(){return window.getSelection().toString();} </script>" + s;

                    //webviewMainwin.destroy();
                    //webviewMainwin.scrollTo(0,0);
                    //webviewMainwin.removeAllViews();
                    //webviewMainwin = (WebView) findViewById(R.id.webviewMain);
/*
                    webviewMainwin.setWebChromeClient(new WebChromeClient(){
                        @Override
                        public void onProgressChanged(WebView view, int newProgress) {
                            super.onProgressChanged(view, newProgress);
                            if(newProgress==100){
                                prbarmulu.setVisibility(View.INVISIBLE);
                                prbar.setVisibility(View.INVISIBLE);
                                /////
                                webviewMainwin.evaluateJavascript("javascript:tttop()", new ValueCallback<String>() {
                                    @Override
                                    public void onReceiveValue(String value) {
                                        //Toast.makeText(CanonActivity.this,value,Toast.LENGTH_SHORT).show();
                                    }
                                });
                                /////
                            }
                        }
                    });
                    */

                    webviewMainwin.findAllAsync("xxxyyyzzz");
                    webviewMainwin.setScrollX(0);
                    webviewMainwin.setScrollY(0);
                    webviewMainwin.loadDataWithBaseURL("about:blank", s, "text/html", "utf-8", null);

                    TextView mainwintitle = (TextView) findViewById(R.id.mainwintitle);
                    mainwintitle.setText(treePoint.getNNAME());

                    openMainwin();

                    //webviewMainwin.loadUrl("http://www.baidu.com");
                    //Toast.makeText(mcontext, "123", Toast.LENGTH_SHORT).show();
                } catch (Exception e) {

                }
                /////
                //Toast.makeText(mcontext, getSubmitResult(treePoint), Toast.LENGTH_SHORT).show();
            } else {  //如果点击的是目录
                if (treePoint.isExpand()) {
                    closeNode(treePoint);
                } else {
                    treePoint.setExpand(true);
                }
            }
            this.notifyDataSetChanged();
        }


        //选择操作
        private void onModeSelect(TreePoint treePoint) {
            if ("1".equals(treePoint.getISLEAF())) {   //选择叶子节点
                //处理回填
                /////

                /////
                treePoint.setSelected(!treePoint.isSelected());
            } else {                                   //选择父节点
                int position = pointList.indexOf(treePoint);
                boolean isSelect = treePoint.isSelected();
                treePoint.setSelected(!isSelect);
                if (position == -1) {
                    return;
                }
                if (position == pointList.size() - 1) {
                    return;
                }
                position++;
                for (; position < pointList.size(); position++) {
                    TreePoint tempPoint = pointList.get(position);
                    if (tempPoint.getPARENTID().equals(treePoint.getPARENTID())) {    //如果找到和自己同级的数据就返回
                        break;
                    }
                    tempPoint.setSelected(!isSelect);
                }
            }
            this.notifyDataSetChanged();
        }

        //选中所有的point
//    private void selectPoint(TreePoint treePoint) {
//        if(){
//
//        }
//    }


        private String getSubmitResult(TreePoint treePoint) {
            StringBuilder sb = new StringBuilder();
            addResult(treePoint, sb);
            String result = sb.toString();
            if (result.endsWith("-")) {
                result = result.substring(0, result.length() - 1);
            }
            return treePoint.getNNAME() + "&" + treePoint.getfilename();
            //return result;
        }

        private void addResult(TreePoint treePoint, StringBuilder sb) {
            if (treePoint != null && sb != null) {
                sb.insert(0, treePoint.getNNAME() + "-");
                if (!"0".equals(treePoint.getPARENTID())) {
                    addResult(pointMap.get(treePoint.getPARENTID()), sb);
                }
            }
        }


        class ViewHolder {
            TextView text;
            ImageView icon;
            ImageButton ib_select;
        }

    }
    //////////

    private Handler handler = new Handler();
    WebView webView, webviewMainwin, webviewmaintxt;
    WebView wvMula,wvAtth,wvTika;
    Button btnmaintxt;
    ProgressBar prbar;
    ProgressBar prbarmulu;
    EditText et1Input;
    EditText sptxt;
    Spinner splist;
    ClipboardManager clip;
    ClipData clipdata;

    /**
     * 如果为true表示在当前界面可以返回显示单词列表页面
     */
    boolean canBack = false;

    /*
      若值为true表示查词指令来自经典阅读窗口中的长按选择文本弹出菜单点击
     */
    boolean isFromCanon = false;

    /**
     * 缓存单词列表页面Html数据，以便按返回键时可以重新显示单词列表页面
     */
    String bufferHtml = "";

    String strKeywords = "";

    /**
     * sdcard路径
     */
    //设为静态，以便在别的类中能够访问
    static File sdcDir = new File("");

    EditText sptxtword;

    @Override
    public void onActionModeStarted(final ActionMode mode) {
        sptxtword = (EditText) findViewById(R.id.sptxtword);
        final EditText edtxtfind = (EditText) findViewById(R.id.edtxtfind);

        super.onActionModeStarted(mode);
        Menu menu = mode.getMenu();
        menu.clear();
        menu.add(getResources().getString(R.string.lookup));
        menu.add(getResources().getString(R.string.copy));
        menu.add(getResources().getString(R.string.paste));
        menu.getItem(0).setOnMenuItemClickListener(new MenuItem.OnMenuItemClickListener() {
            @Override
            public boolean onMenuItemClick(MenuItem item) {
                /////
                if (webView.hasFocus()) {
                    webView.evaluateJavascript("javascript:gettext()", new ValueCallback<String>() {
                        @Override
                        public void onReceiveValue(String value) {
                            //TODO
                            final String s = value.substring(1, value.length() - 1);

                            //Toast.makeText(CanonActivity.this,s,Toast.LENGTH_SHORT).show();
                            DrawerLayout m = (DrawerLayout) findViewById(R.id.drawer_layout);
                            m.openDrawer(Gravity.RIGHT);
                            //把标志置为true
                            isFromCanon = true;
                            if (s.length() > 68) {
                                et1Input.setText(s.substring(0, 68));
                            } else {
                                et1Input.setText(s);
                            }

                        }
                    });
                }
                if (webviewMainwin.hasFocus()) {
                    webviewMainwin.evaluateJavascript("javascript:gettext()", new ValueCallback<String>() {
                        @Override
                        public void onReceiveValue(String value) {
                            //TODO
                            final String s = value.substring(1, value.length() - 1);

                            //Toast.makeText(CanonActivity.this,s,Toast.LENGTH_SHORT).show();
                            DrawerLayout m = (DrawerLayout) findViewById(R.id.drawer_layout);
                            m.openDrawer(Gravity.RIGHT);
                            //把标志置为true
                            isFromCanon = true;
                            if (s.length() > 68) {
                                et1Input.setText(s.substring(0, 68));
                            } else {
                                et1Input.setText(s);
                            }

                        }
                    });
                }
                if (webviewmaintxt.hasFocus()) {
                    webviewmaintxt.evaluateJavascript("javascript:gettext()", new ValueCallback<String>() {
                        @Override
                        public void onReceiveValue(String value) {
                            //TODO
                            final String s = value.substring(1, value.length() - 1);

                            //Toast.makeText(CanonActivity.this,s,Toast.LENGTH_SHORT).show();
                            DrawerLayout m = (DrawerLayout) findViewById(R.id.drawer_layout);
                            m.openDrawer(Gravity.RIGHT);
                            //把标志置为true
                            isFromCanon = true;
                            if (s.length() > 68) {
                                et1Input.setText(s.substring(0, 68));
                            } else {
                                et1Input.setText(s);
                            }

                        }
                    });
                }
                /////
                if (wvMula.hasFocus()) {
                    wvMula.evaluateJavascript("javascript:gettext()", new ValueCallback<String>() {
                        @Override
                        public void onReceiveValue(String value) {
                            //TODO
                            final String s = value.substring(1, value.length() - 1);

                            //Toast.makeText(CanonActivity.this,s,Toast.LENGTH_SHORT).show();
                            DrawerLayout m = (DrawerLayout) findViewById(R.id.drawer_layout);
                            m.openDrawer(Gravity.RIGHT);
                            //把标志置为true
                            isFromCanon = true;
                            if (s.length() > 68) {
                                et1Input.setText(s.substring(0, 68));
                            } else {
                                et1Input.setText(s);
                            }

                        }
                    });
                }
                if (wvAtth.hasFocus()) {
                    wvAtth.evaluateJavascript("javascript:gettext()", new ValueCallback<String>() {
                        @Override
                        public void onReceiveValue(String value) {
                            //TODO
                            final String s = value.substring(1, value.length() - 1);

                            //Toast.makeText(CanonActivity.this,s,Toast.LENGTH_SHORT).show();
                            DrawerLayout m = (DrawerLayout) findViewById(R.id.drawer_layout);
                            m.openDrawer(Gravity.RIGHT);
                            //把标志置为true
                            isFromCanon = true;
                            if (s.length() > 68) {
                                et1Input.setText(s.substring(0, 68));
                            } else {
                                et1Input.setText(s);
                            }

                        }
                    });
                }
                if (wvTika.hasFocus()) {
                    wvTika.evaluateJavascript("javascript:gettext()", new ValueCallback<String>() {
                        @Override
                        public void onReceiveValue(String value) {
                            //TODO
                            final String s = value.substring(1, value.length() - 1);

                            //Toast.makeText(CanonActivity.this,s,Toast.LENGTH_SHORT).show();
                            DrawerLayout m = (DrawerLayout) findViewById(R.id.drawer_layout);
                            m.openDrawer(Gravity.RIGHT);
                            //把标志置为true
                            isFromCanon = true;
                            if (s.length() > 68) {
                                et1Input.setText(s.substring(0, 68));
                            } else {
                                et1Input.setText(s);
                            }

                        }
                    });
                }

                mode.finish();
                return false;
            }
        });
        menu.getItem(1).setOnMenuItemClickListener(new MenuItem.OnMenuItemClickListener() {
            @Override
            public boolean onMenuItemClick(MenuItem item) {
                /////
                if (webView.hasFocus()) {
                    webView.evaluateJavascript("javascript:gettext()", new ValueCallback<String>() {
                        @Override
                        public void onReceiveValue(String value) {
                            if(!value.equals("null")) {
                                value = value.replace("\\n", "\n");
                                clipdata = ClipData.newPlainText("text", value.substring(1, value.length() - 1));
                                clip.setPrimaryClip(clipdata);
                                //Toast.makeText(CanonActivity.this,value,Toast.LENGTH_LONG).show();
                            }
                        }
                    });
                }
                if (webviewMainwin.hasFocus()) {
                    webviewMainwin.evaluateJavascript("javascript:gettext()", new ValueCallback<String>() {
                        @Override
                        public void onReceiveValue(String value) {
                            if(!value.equals("null")) {
                                value = value.replace("\\n", "\n");
                                clipdata = ClipData.newPlainText("text", value.substring(1, value.length() - 1));
                                clip.setPrimaryClip(clipdata);
                                //Toast.makeText(CanonActivity.this,value,Toast.LENGTH_LONG).show();
                            }
                        }
                    });
                }
                if (webviewmaintxt.hasFocus()) {
                    webviewmaintxt.evaluateJavascript("javascript:gettext()", new ValueCallback<String>() {
                        @Override
                        public void onReceiveValue(String value) {
                            if(!value.equals("null")) {
                                value = value.replace("\\n", "\n");
                                clipdata = ClipData.newPlainText("text", value.substring(1, value.length() - 1));
                                clip.setPrimaryClip(clipdata);
                                //Toast.makeText(CanonActivity.this,value,Toast.LENGTH_LONG).show();
                            }
                        }
                    });
                }
                /////
                if (wvMula.hasFocus()) {
                    wvMula.evaluateJavascript("javascript:gettext()", new ValueCallback<String>() {
                        @Override
                        public void onReceiveValue(String value) {
                            if(!value.equals("null")) {
                                value = value.replace("\\n", "\n");
                                clipdata = ClipData.newPlainText("text", value.substring(1, value.length() - 1));
                                clip.setPrimaryClip(clipdata);
                                //Toast.makeText(CanonActivity.this,value,Toast.LENGTH_LONG).show();
                            }
                        }
                    });
                }
                if (wvAtth.hasFocus()) {
                    wvAtth.evaluateJavascript("javascript:gettext()", new ValueCallback<String>() {
                        @Override
                        public void onReceiveValue(String value) {
                            if(!value.equals("null")) {
                                value = value.replace("\\n", "\n");
                                clipdata = ClipData.newPlainText("text", value.substring(1, value.length() - 1));
                                clip.setPrimaryClip(clipdata);
                                //Toast.makeText(CanonActivity.this,value,Toast.LENGTH_LONG).show();
                            }
                        }
                    });
                }
                if (wvTika.hasFocus()) {
                    wvTika.evaluateJavascript("javascript:gettext()", new ValueCallback<String>() {
                        @Override
                        public void onReceiveValue(String value) {
                            if(!value.equals("null")) {
                                value = value.replace("\\n", "\n");
                                clipdata = ClipData.newPlainText("text", value.substring(1, value.length() - 1));
                                clip.setPrimaryClip(clipdata);
                                //Toast.makeText(CanonActivity.this,value,Toast.LENGTH_LONG).show();
                            }
                        }
                    });
                }

                mode.finish();
                return false;
            }
        });
        menu.getItem(2).setOnMenuItemClickListener(new MenuItem.OnMenuItemClickListener() {
            @Override
            public boolean onMenuItemClick(MenuItem item) {
                if(clip.hasPrimaryClip()) {
                    /////
                    if (et1Input.hasFocus()) {
                        String s = clip.getPrimaryClip().getItemAt(0).getText().toString().trim();

                        isFromCanon = true;
                        if (s.length() > 68) {
                            et1Input.setText(s.substring(0, 68));
                        } else {
                            et1Input.setText(s);
                        }
                        //Toast.makeText(CanonActivity.this,s,Toast.LENGTH_LONG).show();
                    }
                    if (sptxtword.hasFocus()) {
                        String s = clip.getPrimaryClip().getItemAt(0).getText().toString().trim();

                        isFromCanon = true;
                        sptxtword.setText(sptxtword.getText().toString().trim() + " " + s);
                        //Toast.makeText(CanonActivity.this,s,Toast.LENGTH_LONG).show();
                    }
                    if (edtxtfind.hasFocus()) {
                        String s = clip.getPrimaryClip().getItemAt(0).getText().toString().trim();

                        isFromCanon = true;
                        edtxtfind.setText(s);
                        //Toast.makeText(CanonActivity.this,s,Toast.LENGTH_LONG).show();
                    }
                    /////
                }
                mode.finish();
                return false;
            }
        });
    }
/*
    @Override
    public void onCreateContextMenu(ContextMenu menu, View v,
                                    ContextMenu.ContextMenuInfo menuInfo) {
        super.onCreateContextMenu(menu, v, menuInfo);
        MenuInflater inflater = getMenuInflater();
        inflater.inflate(R.menu.mainwin, menu);
    }*/

    ///
    private static final int BUFF_SIZE = 1024 * 1024; // 1M Byte
    /**
     * 解压缩一个文件
     *
     * @param zipFile 压缩文件
     * @param folderPath 解压缩的目标目录
     * @throws IOException 当解压缩过程出错时抛出
     */
    public static void unZipFile(File zipFile, String folderPath) throws ZipException, IOException {
        File desDir = new File(folderPath);
        if (!desDir.exists()) {
            desDir.mkdirs();
        }
        ZipFile zf = new ZipFile(zipFile);
        for (Enumeration<?> entries = zf.entries(); entries.hasMoreElements();) {
            ZipEntry entry = ((ZipEntry)entries.nextElement());
            InputStream in = zf.getInputStream(entry);
            String str = folderPath + File.separator + entry.getName();
            str = new String(str.getBytes("8859_1"), "GB2312");
            File desFile = new File(str);
            if (!desFile.exists()) {
                File fileParentDir = desFile.getParentFile();
                if (!fileParentDir.exists()) {
                    fileParentDir.mkdirs();
                }
                desFile.createNewFile();
            }
            OutputStream out = new FileOutputStream(desFile);
            byte buffer[] = new byte[BUFF_SIZE];
            int realLength;
            while ((realLength = in.read(buffer)) > 0) {
                out.write(buffer, 0, realLength);
            }
            in.close();
            out.close();
        }
    }
    ///

    public static void unZipFile2(File zipFile, String folderPath) throws ZipException, IOException {
        File desDir = new File(folderPath);
        if (!desDir.exists()) {
            desDir.mkdirs();
        }
        ZipFile zf = new ZipFile(zipFile);
        for (Enumeration<?> entries = zf.entries(); entries.hasMoreElements();) {
            ZipEntry entry = ((ZipEntry)entries.nextElement());
            InputStream in = zf.getInputStream(entry);
            String str = folderPath + File.separator + entry.getName();
            str = new String(str.getBytes("8859_1"), "GB2312");
            //str = new String(str.getBytes("utf8"), "utf8");
            File desFile = new File(str);
            if (!desFile.exists()) {
                File fileParentDir = desFile.getParentFile();
                if (!fileParentDir.exists()) {
                    fileParentDir.mkdirs();
                }
                desFile.createNewFile();
            }

            //OutputStream out = new FileOutputStream(desFile);
            //File foffset = new File(sdcDir,"/pceddata/�ʿ�1/itemioff");
            File foffset = new File(folderPath + File.separator + entry.getName());
            RandomAccessFile wfo = new RandomAccessFile(foffset,"rw");

            byte buffer[] = new byte[BUFF_SIZE];
            int realLength;
            while ((realLength = in.read(buffer)) > 0) {

                //out.write(buffer, 0, realLength);
                //wfo.writeInt(iOff);
                wfo.write(buffer, 0, realLength);

            }
            in.close();

            //out.close();
            wfo.close();

        }
    }

    public void copyData3(String filename)
    {
        InputStream in = null;
        FileOutputStream out = null;
        String path = getExternalFilesDir(Environment.DIRECTORY_DCIM) + "/"+filename; // data/data目录
        File file = new File(path);
        if (!file.exists()) {
            try
            {
                in = this.getAssets().open(filename); // 从assets目录下复制
                //out = new FileOutputStream(file);
                File foffset = new File(path);
                RandomAccessFile wfo = new RandomAccessFile(foffset,"rw");
                int length = -1;
                byte[] buf = new byte[1024000];
                while ((length = in.read(buf)) != -1)
                {
                    //out.write(buf, 0, length);
                    wfo.write(buf, 0, length);
                }
                //out.flush();
                wfo.close();
            }
            catch (Exception e)
            {
                e.printStackTrace();
            }
            finally{
                ///
                ///
                if (in != null)
                {
                    try {
                        in.close();
                    } catch (IOException e1) {
                        e1.printStackTrace();
                    }
                }

                if (out != null)
                {
                    try {
                        out.close();
                    } catch (IOException e1) {
                        e1.printStackTrace();
                    }
                }
            }
        }
    }

    //复制压缩文件并解压删除
    public void copyData2(String filename)
    {
        //如果数据库文件已存在则退出
        if(new File(getExternalFilesDir(Environment.DIRECTORY_DCIM) + "/"+"pali.db3").exists()){
            return;
        }

        InputStream in = null;
        FileOutputStream out = null;
        String path = getExternalFilesDir(Environment.DIRECTORY_DCIM) + "/"+filename; // data/data目录
        File file = new File(path);
        if (!file.exists()) {
            try
            {
                in = this.getAssets().open(filename); // 从assets目录下复制
                out = new FileOutputStream(file);
                int length = -1;
                byte[] buf = new byte[1024000];
                while ((length = in.read(buf)) != -1)
                {
                    //out = new FileOutputStream(file);
                    out.write(buf, 0, length);
                    //out.flush();
                }
                out.flush();
            }
            catch (Exception e)
            {
                e.printStackTrace();
            }
            finally{
                ///
                ///
                if (in != null)
                {
                    try {
                        in.close();
                    } catch (IOException e1) {
                        e1.printStackTrace();
                    }
                }

                if (out != null)
                {
                    try {
                        out.close();
                    } catch (IOException e1) {
                        e1.printStackTrace();
                    }
                }
            }
        }

        //解压
        try {
            unZipFile2(new File(getExternalFilesDir(Environment.DIRECTORY_DCIM) + "/"+"pali.db3.zip"),getExternalFilesDir(Environment.DIRECTORY_DCIM).toString());
        } catch (IOException e) {
            e.printStackTrace();
        }

        //删除
        new File(getExternalFilesDir(Environment.DIRECTORY_DCIM) + "/"+"pali.db3.zip").delete();
    }

    public void copyData(String filename)
    {
        /*
        String s = readHtm("vin13t.nrf.htm");
        AlertDialog.Builder builder = new AlertDialog.Builder(CanonActivity.this);
        builder.setMessage("词"+s.substring(9850,10150));
        builder.setTitle("提示：");
        builder.setPositiveButton("确定",
                new android.content.DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        dialog.dismiss();
                    }
                });
        builder.create().show();
        */

        InputStream in = null;
        FileOutputStream out = null;
        String path = getExternalFilesDir(Environment.DIRECTORY_DCIM) + "/"+filename; // data/data目录
        File file = new File(path);
        ///fileok文件当数据库文件复制完毕后建立以作为数据库文件完好的标志
        File fileok = new File(path+".ok");
        ///
        if (!fileok.exists()) {
            Toast toast=null;
            try
            {
                in = this.getAssets().open(filename); // 从assets目录下复制
                out = new FileOutputStream(file);
                int length = -1;
                byte[] buf = new byte[1024000];
                while ((length = in.read(buf)) != -1)
                {
                    out.write(buf, 0, length);
                }
                out.flush();
            }
            catch (Exception e)
            {
                toast=Toast.makeText(CanonActivity.this,"#1#搜索数据库可能没能复制成功！可能您的手机当前运行内存不足或者是存储空间不足，请先尝试清理手机的运行内存以及清理手机的存储空间，之后点击search按钮再次复制它！："+e.getMessage(),Toast.LENGTH_LONG);
                toast.setGravity(Gravity.CENTER,0,0);
                for(int i=0;i<20;i++){
                    toast.show();
                }
                e.printStackTrace();
            }
            finally{
                ///
                ///
                if (in != null)
                {
                    try {
                        in.close();
                    } catch (IOException e) {
                        toast=Toast.makeText(CanonActivity.this,"#2#搜索数据库可能没能复制成功！可能您的手机当前运行内存不足或者是存储空间不足，请先尝试清理手机的运行内存以及清理手机的存储空间，之后点击search按钮再次复制它！："+e.getMessage(),Toast.LENGTH_LONG);
                        toast.setGravity(Gravity.CENTER,0,0);
                        for(int i=0;i<20;i++){
                            toast.show();
                        }
                        e.printStackTrace();
                    }
                }

                if (out != null)
                {
                    try {
                        out.close();
                    } catch (IOException e) {
                        toast=Toast.makeText(CanonActivity.this,"#3#搜索数据库可能没能复制成功！可能您的手机当前运行内存不足或者是存储空间不足，请先尝试清理手机的运行内存以及清理手机的存储空间，之后点击search按钮再次复制它！："+e.getMessage(),Toast.LENGTH_LONG);
                        toast.setGravity(Gravity.CENTER,0,0);
                        for(int i=0;i<20;i++){
                            toast.show();
                        }
                        e.printStackTrace();
                    }
                }
            }
            try {
                fileok.createNewFile();
            }catch (Exception e){
                Toast.makeText(CanonActivity.this,"#4font#"+e.getMessage(),Toast.LENGTH_LONG).show();
            }
        }
    }

    public void copyFont(String filename)
    {
        InputStream in = null;
        FileOutputStream out = null;
        String path = getExternalFilesDir(Environment.DIRECTORY_DCIM) + "/"+filename; // data/data目录
        File file = new File(path);
        ///fileok文件当数据库文件复制完毕后建立以作为数据库文件完好的标志
        File fileok = new File(path+".ok");
        ///
        if (!fileok.exists()) {
            Toast toast=null;
            try
            {
                in = this.getAssets().open("fonts/"+filename); // 从assets目录下复制
                //is = this.getAssets().open("pali/"+filename); // 从assets目录下复制
                out = new FileOutputStream(file);
                int length = -1;
                byte[] buf = new byte[1024000];
                while ((length = in.read(buf)) != -1)
                {
                    out.write(buf, 0, length);
                }
                out.flush();
            }
            catch (Exception e)
            {
                toast=Toast.makeText(CanonActivity.this,"#1font#搜索数据库可能没能复制成功！可能您的手机当前运行内存不足或者是存储空间不足，请先尝试清理手机的运行内存以及清理手机的存储空间，之后点击search按钮再次复制它！："+e.getMessage(),Toast.LENGTH_LONG);
                toast.setGravity(Gravity.CENTER,0,0);
                for(int i=0;i<20;i++){
                    toast.show();
                }
                e.printStackTrace();
            }
            finally{
                ///
                ///
                if (in != null)
                {
                    try {
                        in.close();
                    } catch (IOException e) {
                        toast=Toast.makeText(CanonActivity.this,"#2font#搜索数据库可能没能复制成功！可能您的手机当前运行内存不足或者是存储空间不足，请先尝试清理手机的运行内存以及清理手机的存储空间，之后点击search按钮再次复制它！："+e.getMessage(),Toast.LENGTH_LONG);
                        toast.setGravity(Gravity.CENTER,0,0);
                        for(int i=0;i<20;i++){
                            toast.show();
                        }
                        e.printStackTrace();
                    }
                }

                if (out != null)
                {
                    try {
                        out.close();
                    } catch (IOException e) {
                        toast=Toast.makeText(CanonActivity.this,"#3font#搜索数据库可能没能复制成功！可能您的手机当前运行内存不足或者是存储空间不足，请先尝试清理手机的运行内存以及清理手机的存储空间，之后点击search按钮再次复制它！："+e.getMessage(),Toast.LENGTH_LONG);
                        toast.setGravity(Gravity.CENTER,0,0);
                        for(int i=0;i<20;i++){
                            toast.show();
                        }
                        e.printStackTrace();
                    }
                }
            }
            try {
                fileok.createNewFile();
            }catch (Exception e){
                Toast.makeText(CanonActivity.this,"#4font#"+e.getMessage(),Toast.LENGTH_LONG).show();
            }
        }
    }

    public  void openMula(){
        wvMula.setVisibility(View.VISIBLE);
        wvAtth.setVisibility(View.INVISIBLE);
        wvTika.setVisibility(View.INVISIBLE);
        webviewMainwin.setVisibility(View.INVISIBLE);
        webviewmaintxt.setVisibility(View.INVISIBLE);
    }

    public  void openAtth(){
        wvMula.setVisibility(View.INVISIBLE);
        wvAtth.setVisibility(View.VISIBLE);
        wvTika.setVisibility(View.INVISIBLE);
        webviewMainwin.setVisibility(View.INVISIBLE);
        webviewmaintxt.setVisibility(View.INVISIBLE);
    }

    public  void openTika(){
        wvMula.setVisibility(View.INVISIBLE);
        wvAtth.setVisibility(View.INVISIBLE);
        wvTika.setVisibility(View.VISIBLE);
        webviewMainwin.setVisibility(View.INVISIBLE);
        webviewmaintxt.setVisibility(View.INVISIBLE);
    }

    public void openMainwin(){
        wvMula.setVisibility(View.INVISIBLE);
        wvAtth.setVisibility(View.INVISIBLE);
        wvTika.setVisibility(View.INVISIBLE);
        webviewMainwin.setVisibility(View.VISIBLE);
        webviewmaintxt.setVisibility(View.INVISIBLE);
    }

    public  void openmaintxt(){
        wvMula.setVisibility(View.INVISIBLE);
        wvAtth.setVisibility(View.INVISIBLE);
        wvTika.setVisibility(View.INVISIBLE);
        webviewMainwin.setVisibility(View.INVISIBLE);
        webviewmaintxt.setVisibility(View.VISIBLE);
        //btnmaintxt.setText("切换主/副读经窗口（当前为主窗口）");
        /*//
        SpannableString spanString = new SpannableString("切换主/副读经窗口（当前为主窗口）");
        //再构造一个改变字体颜色的Span
        ForegroundColorSpan span = new ForegroundColorSpan(Color.RED);
        //将这个Span应用于指定范围的字体
        spanString.setSpan(span, 13, 14, Spannable.SPAN_EXCLUSIVE_INCLUSIVE);
        //设置给EditText显示出来
        btnmaintxt.setText(spanString);
        //*/
    }

    //把副窗口输出的内容关键词高亮
    public String htmlwin2(String s) {

        if(!strKeywords.equals("")) {

            String[] split = strKeywords.split(" ");

            for (String keyword : split) {
                if (keyword.substring(keyword.length() - 1).equals("*")) {
                    s = s.replaceAll("(?i)\\b" + keyword.trim().substring(0, keyword.trim().length() - 1), "<span style='color:red; background:yellow;'>" + keyword.trim().substring(0, keyword.trim().length() - 1) + "</span>");
                } else if (keyword.substring(keyword.length() - 1).matches("[AaĀāIiĪīUuŪūEeOoKkGgCcJjÑñṬṭḌḍTtDdNnPpBbMmYyRrLlVvSsHhṄṅṆṇḶḷṂṃṀṁŊŋ]")) {
                    s = s.replaceAll("(?i)\\b" + keyword.trim() + "\\b", "<span style='color:red; background:yellow;'>" + keyword.trim() + "</span>");
                } else {
                    s = s.replaceAll(keyword.trim(), "<span style='color:red; background:yellow;'>" + keyword.trim() + "</span>");
                }
            }
        }

        return s;
    }

    public void opensecondwin(String docid) {
        webviewMainwin.setVisibility(View.INVISIBLE);
        webviewmaintxt.setVisibility(View.VISIBLE);
        ///
        SpannableString spanString = new SpannableString("切换主/副读经窗口（当前为副窗口）");
        //再构造一个改变字体颜色的Span
        ForegroundColorSpan span = new ForegroundColorSpan(Color.BLUE);
        //将这个Span应用于指定范围的字体
        spanString.setSpan(span, 13, 14, Spannable.SPAN_EXCLUSIVE_INCLUSIVE);
        //设置给EditText显示出来
        btnmaintxt.setText(spanString);
        ///
        //Toast.makeText(CanonActivity.this,a+"#"+b,Toast.LENGTH_LONG).show();
        /////////////////////////////////////
        SQLiteDatabase palicanondb = null;
        try {
            palicanondb = SQLiteDatabase.openDatabase(sdcDir + "/pali.db3", null, SQLiteDatabase.OPEN_READONLY);
        } catch (SQLiteException e) {
            Toast.makeText(CanonActivity.this, e.getMessage(), Toast.LENGTH_LONG).show();
            return;
        }
        Cursor palidbcursor=null;
        Integer idocid=Integer.parseInt(docid);
        ///
        String palitop = "";
        for(int i=idocid-1; (i>=idocid-25)&&(i>0); i--) {
            palidbcursor = palicanondb.rawQuery("SELECT docid,* FROM canons WHERE docid=" + String.valueOf(i), null);
            if (palidbcursor.moveToFirst()) {
                palitop ="<span style='color:white; background:gray;'>"+ palidbcursor.getString(palidbcursor.getColumnIndex("docid"))+"</span>" + " " + htmlwin2(palidbcursor.getString(palidbcursor.getColumnIndex("PALI")))+"<br /><br />"+palitop;
                //do {

                //} while (palidbcursor.moveToNext()) ;
            }
            palidbcursor.close();
        }
        ///
        ///
        palidbcursor = palicanondb.rawQuery("SELECT docid,* FROM canons WHERE docid="+docid, null);
        String palione="";
        if (palidbcursor.moveToFirst()) {
            palione="<span style='color:white; background:blue;'>"+palidbcursor.getString(palidbcursor.getColumnIndex("docid"))+"</span>";
            palione=palione+" <a id='palione'></a>"+htmlwin2(palidbcursor.getString(palidbcursor.getColumnIndex("PALI")));
            //do {

            //} while (palidbcursor.moveToNext()) ;
        }
        palidbcursor.close();
        ///
        ///
        String palibottom = "";
        for(int i=idocid+1; (i<=idocid+25)&&(i<=415754); i++) {
            palidbcursor = palicanondb.rawQuery("SELECT docid,* FROM canons WHERE docid=" + String.valueOf(i), null);
            if (palidbcursor.moveToFirst()) {
                palibottom =palibottom+"<br /><br />"+"<span style='color:white; background:gray;'>"+ palidbcursor.getString(palidbcursor.getColumnIndex("docid"))+"</span>" + " " + htmlwin2(palidbcursor.getString(palidbcursor.getColumnIndex("PALI")));
                //do {

                //} while (palidbcursor.moveToNext()) ;
            }
            palidbcursor.close();
        }
        ///

        webviewmaintxt.getSettings().setJavaScriptEnabled(true);
        webviewmaintxt.setWebChromeClient(new WebChromeClient() {
            @Override
            public void onProgressChanged(WebView view, int newProgress) {
                super.onProgressChanged(view, newProgress);
                if (newProgress == 100) {
                    webviewmaintxt.evaluateJavascript("javascript:scrollonetoview()", new ValueCallback<String>() {
                        @Override
                        public void onReceiveValue(String value) {
                            //Toast.makeText(CanonActivity.this,value,Toast.LENGTH_SHORT).show();
                        }
                    });
                }
            }
        });
        webviewmaintxt.findAllAsync("xxxyyyzzz");
        webviewmaintxt.loadDataWithBaseURL("about:blank", "<script>function scrollonetoview(){document.getElementById(\"palione\").scrollIntoView(true);} function tttop(){scrollTo(0,0);} function gettext(){return window.getSelection().toString();}</script><html><head><style type=\"text/css\">.cl{color:#0066ff} .cl:visited{color:#ff0000;} body{ word-wrap:break-word;}</style>" + htmlColor + "<script>function gettext(){return window.getSelection().toString();}</script></head><body><span style='color:red; background:yellow;'>"+""+"</span>---------------------------------------------------------------------------<br /><br />" + palitop + palione + palibottom + "<br /><br />---------------------------------------------------------------------------</body></html>", "text/html", "utf-8", null);
        /////////////////////////////////////
    }

    /*
    @SuppressLint("JavascriptInterface")
    @Override
    protected void onCreate(Bundle icicle) {
        super.onCreate(icicle);
        setContentView(R.layout.activity_canon);

        SQLiteDatabase palicanondb = SQLiteDatabase.openDatabase(sdcDir + "/pali.db3", null, SQLiteDatabase.OPEN_READONLY);
    }*/

    private List<Fruit> fruitList=new ArrayList<>();

    private void initFruits(){
        catalogGrandpaID="0";
        catalogParentID="0";

        fruitList.clear();

        Fruit apple = new Fruit("1","tipitaka","Tipiṭaka(Mūla)","三藏(根本)","-1","","1","三藏(根本)",android.R.drawable.ic_menu_gallery, android.R.drawable.ic_media_play,R.color.Gainsboro);
        fruitList.add(apple);

        apple = new Fruit("546","atth","Aṭṭhakathā","《义注》","-1","","546","《义注》",android.R.drawable.ic_menu_gallery, android.R.drawable.ic_media_play,R.color.Gainsboro);
        fruitList.add(apple);

        apple = new Fruit("746","tika","ṭīkā","《复注》","-1","","746","《复注》",android.R.drawable.ic_menu_gallery, android.R.drawable.ic_media_play,R.color.Gainsboro);
        fruitList.add(apple);

        apple = new Fruit("904","anna","Añña","《其他典籍》","-1","","904","《其他典藉》",android.R.drawable.ic_menu_gallery, android.R.drawable.ic_media_play,R.color.Gainsboro);
        fruitList.add(apple);

        rvMulu=(RecyclerView) findViewById(R.id.rvMulu);
        //必须有 三种：线性 网格 瀑布流
        LinearLayoutManager layoutManager=new LinearLayoutManager(this);
        rvMulu.setLayoutManager(layoutManager);
        FruitAdapter a=new FruitAdapter(fruitList);
        rvMulu.setAdapter(a);
        //让滚动条一直显示
        rvMulu.setScrollbarFadingEnabled(false);
    }

    /////
    private void backAboveLevelCatalog(){
        initFruits(catalogGrandpaID);

    }

    private void initFruits(String pID){

        if(catalogParentID.equals("0")){
            return;
        }

        //返回前先记下父ID
        String fuID=catalogParentID;

        SQLiteDatabase palicanondb =null;

        try{
            palicanondb=SQLiteDatabase.openDatabase(sdcDir+"/palihtm.db3",null,SQLiteDatabase.OPEN_READONLY);
        }catch (SQLiteException e){
            Toast.makeText(CanonActivity.this,e.getMessage(),Toast.LENGTH_LONG).show();
            return;
        }

        Cursor palidbcursor = palicanondb.rawQuery("SELECT rowid,* FROM mulu WHERE PARENTID ='"+pID+"';",null);

        if (palidbcursor.moveToFirst()){

            fruitList.clear();

            do{
                String s=palidbcursor.getString(palidbcursor.getColumnIndex("txt"));
                if(s.equals("")){s=palidbcursor.getString(palidbcursor.getColumnIndex("tooltip"));}

                Fruit apple=null;

                if(palidbcursor.getString(palidbcursor.getColumnIndex("ID")).equals(fuID)){
                    apple = new Fruit(palidbcursor.getString(palidbcursor.getColumnIndex("rootid")),palidbcursor.getString(palidbcursor.getColumnIndex("filename")),palidbcursor.getString(palidbcursor.getColumnIndex("txt")),palidbcursor.getString(palidbcursor.getColumnIndex("tooltip")),palidbcursor.getString(palidbcursor.getColumnIndex("ISLEAF")),palidbcursor.getString(palidbcursor.getColumnIndex("PIANPARENT")),palidbcursor.getString(palidbcursor.getColumnIndex("ID")),s,android.R.drawable.ic_menu_compass, -1,R.color.AliceBlue);
                }else{
                    apple = new Fruit(palidbcursor.getString(palidbcursor.getColumnIndex("rootid")),palidbcursor.getString(palidbcursor.getColumnIndex("filename")),palidbcursor.getString(palidbcursor.getColumnIndex("txt")),palidbcursor.getString(palidbcursor.getColumnIndex("tooltip")),palidbcursor.getString(palidbcursor.getColumnIndex("ISLEAF")),palidbcursor.getString(palidbcursor.getColumnIndex("PIANPARENT")),palidbcursor.getString(palidbcursor.getColumnIndex("ID")),s,android.R.drawable.ic_menu_compass, -1,R.color.Gainsboro);
                }

                fruitList.add(apple);

            }while(palidbcursor.moveToNext());

            rvMulu.setAdapter(new FruitAdapter(fruitList));

            catalogParentID=catalogGrandpaID;

            /////
            if(!(catalogGrandpaID.equals("0"))){


                palidbcursor = palicanondb.rawQuery("SELECT PARENTID FROM mulu WHERE ID ='"+catalogGrandpaID+"';",null);

                if (palidbcursor.moveToFirst()){
                    catalogGrandpaID =palidbcursor.getString(palidbcursor.getColumnIndex("PARENTID"));
                }
            }
            /////
        }

        palidbcursor.close();
    }
    /////

    BottomNavigationView navigation=null;

    @SuppressLint({"JavascriptInterface", "SetJavaScriptEnabled"})
    @Override
    protected void onCreate(Bundle icicle) {
        super.onCreate(icicle);
        setContentView(R.layout.activity_canon);

        navigation = (BottomNavigationView)findViewById(R.id.nav_view);
        navigation.setLabelVisibilityMode(1);
        //navigation.setItemRippleColor(ColorStateList.valueOf(Color.RED));
        navigation.setSelectedItemId(R.id.navitemSousuo);

        navigation.setOnNavigationItemSelectedListener(new BottomNavigationView.OnNavigationItemSelectedListener() {
            @Override
            public boolean onNavigationItemSelected(@NonNull MenuItem menuItem) {
                switch (menuItem.getItemId()) {
                    case R.id.navitemMula:
                        wvMula.setVisibility(View.VISIBLE);
                        wvAtth.setVisibility(View.INVISIBLE);
                        wvTika.setVisibility(View.INVISIBLE);
                        webviewMainwin.setVisibility(View.INVISIBLE);
                        webviewmaintxt.setVisibility(View.INVISIBLE);
                        break;
                    case R.id.navitemYizhu:
                        wvMula.setVisibility(View.INVISIBLE);
                        wvAtth.setVisibility(View.VISIBLE);
                        wvTika.setVisibility(View.INVISIBLE);
                        webviewMainwin.setVisibility(View.INVISIBLE);
                        webviewmaintxt.setVisibility(View.INVISIBLE);
                        break;
                    case R.id.navitemFuzhu:
                        wvMula.setVisibility(View.INVISIBLE);
                        wvAtth.setVisibility(View.INVISIBLE);
                        wvTika.setVisibility(View.VISIBLE);
                        webviewMainwin.setVisibility(View.INVISIBLE);
                        webviewmaintxt.setVisibility(View.INVISIBLE);
                        break;
                    case R.id.navitemSousuo:
                        wvMula.setVisibility(View.INVISIBLE);
                        wvAtth.setVisibility(View.INVISIBLE);
                        wvTika.setVisibility(View.INVISIBLE);
                        webviewMainwin.setVisibility(View.VISIBLE);
                        webviewmaintxt.setVisibility(View.INVISIBLE);
                        break;
                    case R.id.navitemXiangqing:
                        wvMula.setVisibility(View.INVISIBLE);
                        wvAtth.setVisibility(View.INVISIBLE);
                        wvTika.setVisibility(View.INVISIBLE);
                        webviewMainwin.setVisibility(View.INVISIBLE);
                        webviewmaintxt.setVisibility(View.VISIBLE);
                        //Toast.makeText(CanonActivity.this, "详情", Toast.LENGTH_SHORT).show();
                        break;
                }
                return true;
            }
        });


        ///
        Typeface tf = Typeface.createFromAsset(CanonActivity.this.getAssets(),"fonts/ZawgyiOne2008.ttf");
        ((EditText)findViewById(R.id.sptxtword)).setTypeface(tf);
        ((EditText)findViewById(R.id.edtxtfind)).setTypeface(tf);
        ((EditText)findViewById(R.id.editTextcanon)).setTypeface(tf);
        ///

        ///
        initFruits();

        //rvMulu.setScrollBarFadeDuration(0);
        //setTheme(android.R.style.Theme_Black);

        /////
        TypedArray array = getTheme().obtainStyledAttributes(new int[]{
                android.R.attr.colorBackground,
                android.R.attr.textColorPrimary,
                //android.R.attr.colorPrimary,
                //android.R.attr.colorPrimaryDark,
                //android.R.attr.colorAccent,
        });
        int backgroundColor = array.getColor(0, 0xFF00FF);
        int textColor = array.getColor(1, 0xFF00FF);
        //int colorPrimary = array.getColor(2, getResources().getColor(R.color.colorPrimary));
        //int colorPrimaryDark = array.getColor(3, getResources().getColor(R.color.colorPrimaryDark));
        //int colorAccent = array.getColor(4, getResources().getColor(R.color.colorAccent));
        array.recycle();

        htmlColor="<style type=\"text/css\">body{color:"+toHexEncoding(textColor)+";background-color:"+toHexEncoding(backgroundColor)+";}</style>";

        copyData("cidian");
        copyData("index");
        copyData("dicinfo");
        copyData("inxilen");
        copyData("inxioff");
        copyData("itemilen");
        copyData("itemioff");

        copyFont("ZawgyiOne2008.ttf");

        //init();
        //addListener();

        ///
        wvMula = (WebView) findViewById(R.id.webviewMula);
        wvAtth = (WebView) findViewById(R.id.webviewAtth);
        wvTika = (WebView) findViewById(R.id.webviewTika);
        webviewMainwin = (WebView) findViewById(R.id.webviewMain);
        webviewmaintxt = (WebView) findViewById(R.id.webviewMaintxt);
        ///
        //设置webview窗口初始背景，避免启动时白色窗口一闪而过
        wvMula.setBackgroundColor(backgroundColor);
        wvAtth.setBackgroundColor(backgroundColor);
        wvTika.setBackgroundColor(backgroundColor);
        webviewMainwin.setBackgroundColor(backgroundColor);
        webviewmaintxt.setBackgroundColor(backgroundColor);
        ///
        prbar = (ProgressBar) findViewById(R.id.jiazaibar);
        prbarmulu = (ProgressBar) findViewById(R.id.jiazaibarmulu);

        webviewTiSet();

        webviewMainwin.setWebChromeClient(new WebChromeClient() {
            @Override
            public void onProgressChanged(WebView view, int newProgress) {
                super.onProgressChanged(view, newProgress);
                if (newProgress == 100) {
                    prbarmulu.setVisibility(View.INVISIBLE);
                    prbar.setVisibility(View.INVISIBLE);
                    /*////放在此处导致切换主副窗口时有时自动滚回顶端
                    webviewMainwin.evaluateJavascript("javascript:tttop()", new ValueCallback<String>() {
                        @Override
                        public void onReceiveValue(String value) {
                            //Toast.makeText(CanonActivity.this,value,Toast.LENGTH_SHORT).show();
                        }
                    });
                    ////*/
                }
            }
        });
        webviewMainwin.getSettings().setJavaScriptEnabled(true);

        //避免焦点落在段落输入框中导致输入法弹出
        webviewMainwin.requestFocusFromTouch();
        //registerForContextMenu(webviewMainwin);

        webView = (WebView) findViewById(R.id.webViewcanon);
        webView.setWebChromeClient(new WebChromeClient() {
            @Override
            public void onProgressChanged(WebView view, int newProgress) {
                super.onProgressChanged(view, newProgress);
                if (newProgress == 100) {
                    webView.evaluateJavascript("javascript:tttop()", new ValueCallback<String>() {
                        @Override
                        public void onReceiveValue(String value) {

                        }
                    });
                }
            }
        });

        et1Input = (EditText) findViewById(R.id.editTextcanon);

        sptxt = (EditText) findViewById(R.id.sptxt);
        splist = (Spinner) findViewById(R.id.splist);

        clip = (ClipboardManager) getSystemService(CLIPBOARD_SERVICE);

        /*
        LinearLayout ll = findViewById(R.id.rootll);
        ll.removeAllViewsInLayout();

        LayoutInflater inflater = getLayoutInflater();
        View vv = inflater.inflate(R.layout.newone, null);
        ll.addView(vv);*/

        ActionBar actionBar = getActionBar();
        if (actionBar != null) {
            actionBar.hide();
        }

        //主窗口工具栏定位段落页码按钮
        Button imgto = (Button) findViewById(R.id.imgto);
        imgto.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                String inputstr = sptxt.getText().toString();
                if(inputstr.trim().equals("")){
                    return;
                }

                //Toast.makeText(CanonActivity.this,String.valueOf(splist.getSelectedItemPosition()),Toast.LENGTH_SHORT).show();
                String spre = "";
                int ipre = splist.getSelectedItemPosition();
                if (ipre == 0) {
                    spre = "V";
                }
                if (ipre == 1) {
                    spre = "M";
                }
                if (ipre == 2) {
                    spre = "P";
                }
                if (ipre == 3) {
                    spre = "T";
                }

                WebView wvPali=null;
                if(wvMula.getVisibility()==0) {
                    wvPali=wvMula;
                }else if(wvAtth.getVisibility()==0) {
                    wvPali=wvAtth;
                }else if(wvTika.getVisibility()==0) {
                    wvPali=wvTika;
                }else{
                    return;
                }

                if (ipre == 4) {
                    wvPali.evaluateJavascript("javascript:topara(3183,0," + inputstr + ")", new ValueCallback<String>() {
                        @Override
                        public void onReceiveValue(String value) {

                        }
                    });
                } else {
                    wvPali.evaluateJavascript("javascript:ttt('" + spre + inputstr + "')", new ValueCallback<String>() {
                        @Override
                        public void onReceiveValue(String value) {

                        }
                    });
                }

                //openMainwin();
            }
        });

        Button btnCodeChinese = (Button) findViewById(R.id.btncodechinese);
        btnCodeChinese.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                txtcode="chinese";
                initFruits();
            }
        });

        Button btnCodePali = (Button) findViewById(R.id.btncodepali);
        btnCodePali.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                txtcode="pali";
                initFruits();
            }
        });

        Button btnBackAbove = (Button) findViewById(R.id.btnBackAbove);
        btnBackAbove.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                backAboveLevelCatalog();

            }
        });

        Button ivcatalog = (Button) findViewById(R.id.catalog);
        ivcatalog.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                DrawerLayout dl = (DrawerLayout) findViewById(R.id.drawer_layout);
                dl.openDrawer(Gravity.LEFT);

                copyData("palihtm.db3");

            }
        });

        Button ivdict = (Button) findViewById(R.id.dict);
        ivdict.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                DrawerLayout dldict = (DrawerLayout) findViewById(R.id.drawer_layout);
                dldict.openDrawer(Gravity.RIGHT);
                ((EditText) findViewById(R.id.editTextcanon)).requestFocus();
            }
        });

        btnmaintxt = (Button) findViewById(R.id.btnmaintxt);
        ///
        SpannableString spanString = new SpannableString("切换主/副读经窗口（当前为主窗口）");
        //再构造一个改变字体颜色的Span
        ForegroundColorSpan span = new ForegroundColorSpan(Color.RED);
        //将这个Span应用于指定范围的字体
        spanString.setSpan(span, 13, 14, Spannable.SPAN_EXCLUSIVE_INCLUSIVE);
        //设置给EditText显示出来
        btnmaintxt.setText(spanString);
        ///
        btnmaintxt.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if(btnmaintxt.getText().toString().equals("切换主/副读经窗口（当前为主窗口）")){
                    webviewMainwin.setVisibility(View.INVISIBLE);
                    webviewmaintxt.setVisibility(View.VISIBLE);
                    //btnmaintxt.setText("切换主/副读经窗口（当前为副窗口）");
                    ///
                    SpannableString spanString = new SpannableString("切换主/副读经窗口（当前为副窗口）");
                    //再构造一个改变字体颜色的Span
                    ForegroundColorSpan span = new ForegroundColorSpan(Color.BLUE);
                    //将这个Span应用于指定范围的字体
                    spanString.setSpan(span, 13, 14, Spannable.SPAN_EXCLUSIVE_INCLUSIVE);
                    //设置给EditText显示出来
                    btnmaintxt.setText(spanString);
                    ///
                }else{
                    webviewmaintxt.setVisibility(View.INVISIBLE);
                    webviewMainwin.setVisibility(View.VISIBLE);
                    //btnmaintxt.setText("切换主/副读经窗口（当前为主窗口）");
                    ///
                    SpannableString spanString = new SpannableString("切换主/副读经窗口（当前为主窗口）");
                    //再构造一个改变字体颜色的Span
                    ForegroundColorSpan span = new ForegroundColorSpan(Color.RED);
                    //将这个Span应用于指定范围的字体
                    spanString.setSpan(span, 13, 14, Spannable.SPAN_EXCLUSIVE_INCLUSIVE);
                    //设置给EditText显示出来
                    btnmaintxt.setText(spanString);
                    ///
                }
            }
        });

        final EditText edtxtfind = (EditText) findViewById(R.id.edtxtfind);

        Button btnfind = (Button)findViewById(R.id.btnfind);
        btnfind.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if(webviewMainwin.getVisibility()==0) {
                    webviewMainwin.findAllAsync(edtxtfind.getText().toString().trim());
                }
                if(webviewmaintxt.getVisibility()==0) {
                    webviewmaintxt.findAllAsync(edtxtfind.getText().toString().trim());
                }
                if(wvMula.getVisibility()==0) {
                    wvMula.findAllAsync(edtxtfind.getText().toString().trim());
                }
                if(wvAtth.getVisibility()==0) {
                    wvAtth.findAllAsync(edtxtfind.getText().toString().trim());
                }
                if(wvTika.getVisibility()==0) {
                    wvTika.findAllAsync(edtxtfind.getText().toString().trim());
                }
            }
        });

        Button btnprev = (Button)findViewById(R.id.btnprev);
        btnprev.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if(webviewMainwin.getVisibility()==0) {
                    webviewMainwin.findNext(false);
                }
                if(webviewmaintxt.getVisibility()==0) {
                    webviewmaintxt.findNext(false);
                }
                if(wvMula.getVisibility()==0) {
                    wvMula.findNext(false);
                }
                if(wvAtth.getVisibility()==0) {
                    wvAtth.findNext(false);
                }
                if(wvTika.getVisibility()==0) {
                    wvTika.findNext(false);
                }
            }
        });

        final EditText scpaliword = (EditText) findViewById(R.id.sptxtword);
        scpaliword.requestFocus();
        Button btnsearch = (Button) findViewById(R.id.btnsearch);
        btnsearch.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                ///
                navigation.setSelectedItemId(R.id.navitemSousuo);
                ///
                strKeywords=scpaliword.getText().toString().trim();
                //AaĀāIiĪīUuŪūEeOoKkGgCcJjÑñṬṭḌḍTtDdNnPpBbMmYyRrLlVvSsHhṄṅṆṇḶḷṂṃṀṁŊŋ
                strKeywords = strKeywords.replaceAll("[^AaĀāIiĪīUuŪūEeOoKkGgCcJjÑñṬṭḌḍTtDdNnPpBbMmYyRrLlVvSsHhṄṅṆṇḶḷṂṃṀṁŊŋ\\* \\u4e00-\\u9fa5]", " ");
                strKeywords = strKeywords.replaceAll("(?<=[^\\u4e00-\\u9fa5])(?=[\\u4e00-\\u9fa5])|(?<=[\\u4e00-\\u9fa5])(?=[^\\u4e00-\\u9fa5])", " ");
                strKeywords = " "+strKeywords+" ";
                strKeywords = strKeywords.replaceAll("(?<=[AaĀāIiĪīUuŪūEeOoKkGgCcJjÑñṬṭḌḍTtDdNnPpBbMmYyRrLlVvSsHhṄṅṆṇḶḷṂṃṀṁŊŋ])\\*(?= )", "#");
                strKeywords = strKeywords.replaceAll("\\*", "");
                strKeywords = strKeywords.replaceAll("#", "*");
                strKeywords = strKeywords.trim().replaceAll(" {2,}", " ");

                if(strKeywords.equals("")){
                    return;
                }

                copyData("pali.db3");

                String[] split = strKeywords.split(" ");

                Integer iresultnum=0;
                String scresult="";
                //PaliCanonDbSqliteHelper palidbSH = new PaliCanonDbSqliteHelper(CanonActivity.this,sdcDir+"/pali.db3",null,1);
                //SQLiteDatabase palicanondb = palidbSH.getReadableDatabase();
                ///
                SQLiteDatabase palicanondb =null;

                try{
                    //palicanondb = palidbSH.getWritableDatabase();
                    palicanondb=SQLiteDatabase.openDatabase(sdcDir+"/pali.db3",null,SQLiteDatabase.OPEN_READONLY);
                }catch (SQLiteException e){
                    Toast.makeText(CanonActivity.this,e.getMessage(),Toast.LENGTH_LONG).show();
                    return;
                }
                ///
                //Gaḷeyya yaṃ pitvā pate papātaṃ, sobbhaṃ guhaṃ candaniyoḷigallaṃ
                Cursor palidbcursor = palicanondb.rawQuery("SELECT docid,* FROM canons WHERE PALI MATCH '"+strKeywords.replaceAll("(?<=[\\u4e00-\\u9fa5])(?=[\\u4e00-\\u9fa5])", " NEAR/0 ")+"';",null);
                if (palidbcursor.moveToFirst()){
                    String linkdocid="";
                    String bookname="";
                    String prepali="";
                    String pali="";
                    do{
                        linkdocid="";
                        if(!(palidbcursor.getString(palidbcursor.getColumnIndex("BOOKID")).equals("0"))) {
                            linkdocid = "<a class='cl' href='#docid" + palidbcursor.getString(palidbcursor.getColumnIndex("docid")) + "' onclick='window.openinnewwin.openwin(\"" + palidbcursor.getString(palidbcursor.getColumnIndex("docid")) +  "\")'>点击此处在上下文窗口中打开其上下文</a><br />";
                        }
                        bookname="";
                        iresultnum=iresultnum+1;
                        if(palidbcursor.getString(palidbcursor.getColumnIndex("C")).equals(" ")) {
                            bookname = "<span style='color:blue; background:Gainsboro;'>" + palidbcursor.getString(palidbcursor.getColumnIndex("A")) + " / " + palidbcursor.getString(palidbcursor.getColumnIndex("B")) + " / " +  palidbcursor.getString(palidbcursor.getColumnIndex("BOOK")) + "</span><br />";
                        }else{
                            bookname = "<span style='color:blue; background:Gainsboro;'>" + palidbcursor.getString(palidbcursor.getColumnIndex("A")) + " / " + palidbcursor.getString(palidbcursor.getColumnIndex("B")) + " / " + palidbcursor.getString(palidbcursor.getColumnIndex("C")) + " / " + palidbcursor.getString(palidbcursor.getColumnIndex("BOOK")) + "</span><br />";
                        }
                        prepali = "<span style='color:white; background:blue;'>"+palidbcursor.getString(palidbcursor.getColumnIndex("docid")) + "</span>"+"　";
                        pali = palidbcursor.getString(palidbcursor.getColumnIndex("PALI"));
                        pali = pali.replaceAll("(?<=[\\u4e00-\\u9fa5]) {1,}(?=[\\u4e00-\\u9fa5])", "");

                        for (String keyword : split) {
                            //Toast.makeText(CanonActivity.this,keyword.trim().substring(0,keyword.trim().length()-1),Toast.LENGTH_LONG).show();
                            if(keyword.substring(keyword.length()-1).equals("*")){
                                pali = pali.replaceAll("(?i)\\b" + keyword.trim().substring(0,keyword.trim().length()-1), "<span style='color:red; background:yellow;'>" + keyword.trim().substring(0,keyword.trim().length()-1) + "</span>");
                            }else if(keyword.substring(keyword.length()-1).matches("[AaĀāIiĪīUuŪūEeOoKkGgCcJjÑñṬṭḌḍTtDdNnPpBbMmYyRrLlVvSsHhṄṅṆṇḶḷṂṃṀṁŊŋ]")){
                                pali = pali.replaceAll("(?i)\\b" + keyword.trim()+"\\b", "<span style='color:red; background:yellow;'>" + keyword.trim() + "</span>");
                            }else{
                                pali = pali.replaceAll(keyword.trim(), "<span style='color:red; background:yellow;'>" + keyword.trim() + "</span>");
                            }
                        }

                        //数据库里词典解释里有错误的htm标记，须替换为正确的/
                        pali = pali.replaceAll("< i >", "<i>");
                        pali = pali.replaceAll("< / i >", "</i>");
                        pali = pali.replaceAll("< sup >", "<sup>");
                        pali = pali.replaceAll("< / sup >", "</sup>");
                        pali = pali.replaceAll("< em >", "<em>");
                        pali = pali.replaceAll("< / em >", "</em>");
                        pali = pali.replaceAll("< br >", "<br />");
                        ///

                        //Toast.makeText(CanonActivity.this,pali,Toast.LENGTH_LONG).show();
                        scresult = scresult + linkdocid + bookname + prepali + pali+"<br /><br />";
                    }while(palidbcursor.moveToNext() && (iresultnum<100));

                    if(iresultnum>=100){
                        scresult = scresult + "<br /><br />搜索结果在100条以上，只输出前100条。<br />Search for more than 100 results, only the top 100 output.";
                    }
                    webviewMainwin.getSettings().setJavaScriptEnabled(true);
                    ///
                    webviewMainwin.addJavascriptInterface(new Object() {
                        @JavascriptInterface
                        public int openwin(final String docid) {

                            handler.post(new Runnable() {
                                @Override
                                public void run() {
                                    opensecondwin(docid);
                                    navigation.setSelectedItemId(R.id.navitemXiangqing);
                                }
                            });

                            return 0;
                        }
                    }, "openinnewwin");
                    webviewMainwin.findAllAsync("xxxyyyzzz");
                    webviewMainwin.setScrollX(0);
                    webviewMainwin.setScrollY(0);
                    webviewMainwin.loadDataWithBaseURL("about:blank", "<script>function tttop(){scrollTo(0,0);}</script><html><head><style type=\"text/css\">.cl{color:#0066ff} .cl:visited{color:#ff0000;} body{ word-wrap:break-word;}</style>"+htmlColor+"<script>function gettext(){return window.getSelection().toString();}</script></head><body><span style='color:red; background:yellow;'>"+strKeywords+"</span><br /><br />"+scresult+"</body></html>", "text/html", "utf-8", null);
                }else{
                    webviewMainwin.loadDataWithBaseURL("about:blank", "<html>"+htmlColor+"<body><span style='color:red; background:yellow;'>"+strKeywords+"</span><br /><br />"+"没找到包含以上关键词的段落！</body></html>", "text/html", "utf-8", null);
                }
                palidbcursor.close();
                openMainwin();

                /*
                webviewMainwin.evaluateJavascript("javascript:tttop()", new ValueCallback<String>() {
                    @Override
                    public void onReceiveValue(String value) {

                    }
                });*/
            }
        });

        final EditText etxtdocid = (EditText) findViewById(R.id.etxtdocid);

        Button btnget = (Button) findViewById(R.id.btnget);
        btnget.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                navigation.setSelectedItemId(R.id.navitemSousuo);

                String strdocid=etxtdocid.getText().toString();
                if(strdocid.equals("")){
                    return;
                }

                copyData("pali.db3");

                String scresult="";
                PaliCanonDbSqliteHelper palidbSH = new PaliCanonDbSqliteHelper(CanonActivity.this,sdcDir+"/pali.db3",null,1);
                SQLiteDatabase palicanondb = palidbSH.getReadableDatabase();

                Cursor palidbcursor = palicanondb.rawQuery("SELECT docid,* FROM canons WHERE docid = "+strdocid+";",null);
                if (palidbcursor.moveToFirst()){
                    String linkdocid="";
                    if(!(palidbcursor.getString(palidbcursor.getColumnIndex("BOOKID")).equals("0"))) {
                        linkdocid = "<a class='cl' href='#docid" + palidbcursor.getString(palidbcursor.getColumnIndex("docid")) +  "' onclick='window.openinnewwin.openwin(\"" + palidbcursor.getString(palidbcursor.getColumnIndex("docid")) + "\")'>点击此处在上下文窗口中打开其上下文</a><br />";
                    }
                    String bookname="";
                    if(palidbcursor.getString(palidbcursor.getColumnIndex("C")).equals(" ")) {
                        bookname = "<span style='color:blue; background:Gainsboro;'>" + palidbcursor.getString(palidbcursor.getColumnIndex("A")) + " / " + palidbcursor.getString(palidbcursor.getColumnIndex("B")) + " / " +  palidbcursor.getString(palidbcursor.getColumnIndex("BOOK")) + "</span><br />";
                    }else{
                        bookname = "<span style='color:blue; background:Gainsboro;'>" + palidbcursor.getString(palidbcursor.getColumnIndex("A")) + " / " + palidbcursor.getString(palidbcursor.getColumnIndex("B")) + " / " + palidbcursor.getString(palidbcursor.getColumnIndex("C")) + " / " + palidbcursor.getString(palidbcursor.getColumnIndex("BOOK")) + "</span><br />";
                    }
                    String prepali = "<span style='color:white; background:blue;'>"+palidbcursor.getString(palidbcursor.getColumnIndex("docid"))+ "</span>"+"　";
                    String pali =palidbcursor.getString(palidbcursor.getColumnIndex("PALI"));

                    webviewMainwin.getSettings().setJavaScriptEnabled(true);
                    webviewMainwin.addJavascriptInterface(new Object() {
                        @JavascriptInterface
                        public int openwin(final String docid) {

                            handler.post(new Runnable() {
                                @Override
                                public void run() {
                                    opensecondwin(docid);
                                    navigation.setSelectedItemId(R.id.navitemXiangqing);
                                }
                            });

                            return 0;
                        }
                    }, "openinnewwin");
                    webviewMainwin.findAllAsync("xxxyyyzzz");
                    webviewMainwin.loadDataWithBaseURL("about:blank", "<html><head><style type=\"text/css\">.cl{color:#0066ff} .cl:visited{color:#ff0000;} </style>"+htmlColor+"<script>function gettext(){return window.getSelection().toString();}</script></head><body>"+linkdocid+bookname+prepali+pali+"</body></html>", "text/html", "utf-8", null);
                }else{
                    webviewMainwin.loadDataWithBaseURL("about:blank", "<html>"+htmlColor+"<body>此段落不存在！</body></html>", "text/html", "utf-8", null);
                }
                palidbcursor.close();
                strKeywords = "";
                openMainwin();
            }
        });

        ImageView ivmuluclose = (ImageView) findViewById(R.id.muluclose);
        ivmuluclose.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                DrawerLayout dl = (DrawerLayout) findViewById(R.id.drawer_layout);
                dl.closeDrawer(Gravity.LEFT);
            }
        });

        ImageView ivdictclose = (ImageView) findViewById(R.id.dictclose);
        ivdictclose.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                DrawerLayout dl = (DrawerLayout) findViewById(R.id.drawer_layout);
                dl.closeDrawer(Gravity.RIGHT);
            }
        });

        //多个可能的sdCard路径
        String[] arrSdcard = {
                Environment.getExternalStorageDirectory().getPath(),
                "/mnt/sdcard",
                "/storage/sdcard0",
                "/mnt/extSdCard",
                "/mnt/sdcard2",
                "/mnt/sdcard-ext",
                "/mnt/ext_sdcard",
                "/mnt/sdcard/SD_CARD",
                "/mnt/sdcard/extra_sd",
                "/mnt/extrasd_bind",
                "/mnt/sdcard/ext_sd",
                "/mnt/sdcard/external_SD",
                "/storage/sdcard1",
                "/storage/extSdCard"
        };

        //如果某一路径是正确的，则保存此一路径
        for (String sdcard : arrSdcard) {
            File x = new File(sdcard + "/pceddata/index");
            if (x.exists()) {
                sdcDir = new File(sdcard);
                break;
            }
        }

        sdcDir = CanonActivity.this.getExternalFilesDir(Environment.DIRECTORY_DCIM);

        //如果词库不存在，则提示安装
        if (sdcDir.getPath().equals("")) {
            sdcDialog();
        }

        /////
        //try{
            /*
            File file = new File(sdcDir, "/pceddata/pali/abh01a.att.htm");
            RandomAccessFile rf = new RandomAccessFile(file, "r");
            byte[] buffer = new byte[(int)(rf.length())];
            rf.seek(0);
            rf.read(buffer);
            rf.close();
            String s=EncodingUtils.getString(buffer, "UTF-8");

            final ProgressBar prbar=(ProgressBar)findViewById(R.id.jiazaibar);
            prbar.setVisibility(View.VISIBLE);
*/
        /////
        //s="<a name=\"para413\"></a><a name=\"para409-412\"></a><a name=\"V0.0297\"></a>";
        //TextView mainwintitle=(TextView)findViewById(R.id.mainwintitle);
        //mainwintitle.setText("#"+s+"#");

        /////

        webviewMainwin.loadDataWithBaseURL("about:blank", "<html><head>"+htmlColor+"<script>function gettext(){return window.getSelection().toString();}</script></head><body>copyright(C) 2014-2021 alobha<br><br>在经文中长按单词选择查词！<br />in pali text, Long press word look up!<br /><br />本程序里的‘巴利三藏经典库’是从印度“内观研究所”制作的电子版“Chattha Sangayana Tipitaka 4.0”中完全复制取得，在此，随喜赞叹印度“内观研究所”之功德善行！<br />随喜本程序内各个词典作者之功德善行！<br />本程序可自由善意分享<br /><br />您在使用中若有问题或建议，<br>可以和作者联系。<br><br>程序作者：alobha<br>邮箱：alobha@hotmail.com <br>QQ 952395695 <br><br>Sādhu! Sādhu! Sādhu!</body></html>", "text/html", "utf-8", null);
        webviewmaintxt.loadDataWithBaseURL("about:blank", "<html><head>"+htmlColor+"</head><body>这是搜索结果条目上下文窗口！</body></html>", "text/html", "utf-8", null);
        //webviewMainwin.loadUrl("https://www.baidu.com/");

            /*
            webviewMainwin.setWebChromeClient(new WebChromeClient(){
                @Override
                public void onProgressChanged(WebView view, int newProgress) {
                    super.onProgressChanged(view, newProgress);
                    if(newProgress==100){
                        prbar.setVisibility(View.INVISIBLE);
                    }
                }
            });*/


        //}catch(Exception e){

        //}
        /////

        try {
            File foffset = new File(sdcDir, "/itemioff");
            RandomAccessFile rfo = new RandomAccessFile(foffset, "r");
            NUM = (int) (rfo.length() / 4);
            rfo.close();

            final EditText etInput = (EditText) findViewById(R.id.editTextcanon);
            etInput.addTextChangedListener(new TextWatcher() {
                public void afterTextChanged(Editable s) {
                }

                public void beforeTextChanged(CharSequence s, int start, int count, int after) {
                }

                public void onTextChanged(CharSequence s, int start, int before, int count) {

                    String sWord = s.toString().trim();
                    if (sWord.equals("")) {
                        webView.loadDataWithBaseURL("about:blank", "<html><head>"+htmlColor+"</head><body></body></html>", "text/html", "utf-8", null);
                        return;
                    }

                    bwordaheadmatch = true;

                    if (isFromCanon) {
                        palihan_cc_c(sWord, "");
                        isFromCanon = false;
                    } else {
                        palihan_cc(sWord, "");
                    }

                    listDc();

                }

            });

            /*
            Button btnLookup = (Button) findViewById(R.id.buttoncanon);
            btnLookup.setOnClickListener(new Button.OnClickListener() {
                public void onClick(View v) {
                    String sWord = etInput.getText().toString().trim();
                    if (sWord.equals("")) {
                        return;
                    }

                    bwordaheadmatch = false;
                    if (true == palihan_cc(sWord, "")) {
                        //取得词典信息数组
                        String[] scArr = getDicinfo();
                        String strItem;
                        StringBuilder b = new StringBuilder("");
                        for (int z = listFirstNo; z <= listEndNo; z++) {
                            strItem = getexplain(z);
                            b.append("<br>");
                            //遍历词典信息数组中每一个元素
                            for (String sc : scArr) {
                                if (sc.substring(1, 2).equals(strItem.substring(0, 1))) {
                                    b.append("<span style=\"color: #3366CC\">");
                                    b.append(sc.substring(3, 28));
                                    b.append("</span>");
                                }
                            }
                            b.append("<br>");
                            b.append(strItem.substring(2));
                            b.append("<br>");
                        }
                        webView.loadDataWithBaseURL("about:blank", "<html><head><style type=\"text/css\">@font-face{font-family:\"Zawgyi-One\";src:url(\"file:///android_asset/fonts/ZawgyiOne2008.ttf\");}body{font-family:\"Zawgyi-One\"; background-color: white;}</style></head><body>" + b.toString() + "</body></html>", "text/html", "utf-8", null);
                    } else {
                        webView.loadDataWithBaseURL("about:blank", "<html><head><style type=\"text/css\">body{background-color: white}</style></head><body>'<b>" + sWord + "</b>' no found! <br><br>词库里没有这个单词，这个词可能是一个词尾变形词，也可能是一个组合词，建议您删掉词尾的几个字母，从单词列表中寻找并点击一个类似的词形来查词。</body></html>", "text/html", "utf-8", null);
                    }
                    canBack = false;

                    InputMethodManager imm = (InputMethodManager) getSystemService(Context.INPUT_METHOD_SERVICE);
                    imm.hideSoftInputFromWindow(v.getWindowToken(), 0);
                }
            });*/

            //webView = new WebView(this);
            //webView = (WebView) findViewById(R.id.webViewcanon);
            //webView.getSettings().setCacheMode(WebSettings.LOAD_CACHE_ONLY);
            webView.setScrollBarStyle(View.SCROLLBARS_INSIDE_OVERLAY);
            webView.loadDataWithBaseURL("about:blank", "<html><head>"+htmlColor+"</head><body>tip:<br /><a style=\"color:red\">●</a><a style=\"color:green\">●</a><a style=\"color:saddlebrown\">●</a><a style=\"color:gray\">●</a><br />red dot<a style=\"color:red\">●</a>: Chinese<br>green dot<a style=\"color:green\">●</a>: English<br>brown dot<a style=\"color:saddlebrown\">●</a>: Myanmar<br>gray dot<a style=\"color:gray\">●</a>: Viet<br></body></html>", "text/html", "utf-8", null);
            //webView.loadDataWithBaseURL("about:blank", "<html><head><style type=\"text/css\">@font-face{font-family:\"Zawgyi-One\";src:url(\"file:///android_asset/fonts/ZawgyiOne2008.ttf\");}body{font-family:\"Zawgyi-One\"; background-color: gray;}pre{font-family:\"Zawgyi-One\";}</style></head><a onclick = \"window.lookup.mydata('"+"单词item"+"')\">点击调用java函数</a><br><a onclick = \"window.lookup.mydata('单词item')\">点击调用java函数</a><br><a onclick = \"window.lookup.mydata('单词item')\">点击调用java函数</a><br><a onclick = \"window.lookup.mydata('单词item')\">点击调用java函数</a><br><a onclick = \"window.lookup.mydata('单词item')\">点击调用java函数</a><pre>"+(endMili-startMili)/1.0+"毫秒.#最长单词长度：" +String.valueOf(maxdcLen)+"#第："+String.valueOf(maxdcNum)+"#"+ strInx + "#<br>" + str + "</pre><br>128āīūṅñṭḍṇḷṃṁŋ这是汉字这是日文：無常を常なりとする顛倒这是缅文：အနိစၥ(တိ)这是越文：không bền vững, vô thường。</html>", "text/html", "utf-8", null);
            //setContentView(webView);

            webView.getSettings().setJavaScriptEnabled(true);
            webView.addJavascriptInterface(new Object() {
                @JavascriptInterface
                public int javaonscroll(final String strItem) {

                    handler.post(new Runnable() {
                        @Override
                        public void run() {

                            InputMethodManager imm = (InputMethodManager) getSystemService(Context.INPUT_METHOD_SERVICE);
                            //imm.hideSoftInputFromWindow(etInput.getWindowToken(), 0);

                        }
                    });

                    return 0;
                }

                @JavascriptInterface
                public int javaonclick(final String strItem) {

                    handler.post(new Runnable() {
                        @Override
                        public void run() {

                            bwordaheadmatch = false;
                            if (true == palihan_cc(strItem, "")) {
                                //取得词典信息数组
                                String[] scArr = getDicinfo();
                                String item;
                                StringBuilder b = new StringBuilder("");
                                for (int z = listFirstNo; z <= listEndNo; z++) {
                                    if (ABCtoabc(strItem).equals(ABCtoabc(getword(z)))) {
                                        item = getexplain(z);
                                        b.append("<br>");
                                        //遍历词典信息数组中每一个元素
                                        for (String sc : scArr) {
                                            if (sc.substring(1, 2).equals(item.substring(0, 1))) {
                                                b.append("<span style=\"color: #3366CC\">");
                                                b.append(sc.substring(3, 28));
                                                b.append("</span>");
                                            }
                                        }
                                        b.append("<br>");
                                        b.append(item.substring(2));
                                        b.append("<br>");
                                    }
                                }
                                webView.loadDataWithBaseURL("about:blank", "<script>function tttop(){scrollTo(0,0);}</script>" + "<html><head><style type=\"text/css\">@font-face{font-family:\"Zawgyi-One\";src:url(\"file:///android_asset/fonts/ZawgyiOne2008.ttf\");}body{font-family:\"Zawgyi-One\"; }</style>"+htmlColor+"</head><script>function gettext(){return window.getSelection().toString();}</script><body>" + b.toString() + "</body></html>", "text/html", "utf-8", null);
                                canBack = true;
                            } else {
                                webView.loadDataWithBaseURL("about:blank", "<html><head>"+htmlColor+"</head><body>error</body></html>", "text/html", "utf-8", null);
                            }

                            InputMethodManager imm = (InputMethodManager) getSystemService(Context.INPUT_METHOD_SERVICE);
                            //imm.hideSoftInputFromWindow(etInput.getWindowToken(), 0);

                        }
                    });

                    return 0;

                }
            }, "lookup");

        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    //目录///

    private TreeAdapter adapter;
    private ListView lvMulu;
    private RecyclerView rvMulu;
    private EditText et_filter;
    private List<TreePoint> pointList = new ArrayList<>();
    private HashMap<String, TreePoint> pointMap = new HashMap<>();

    public void init() {
        adapter = new TreeAdapter(this, pointList, pointMap);
        lvMulu = findViewById(R.id.lvMulu);
        lvMulu.setAdapter(adapter);
        initData();
    }

    //初始化数据
    //数据特点：TreePoint 之间的关系特点   id是任意唯一的。    如果为根节点 PARENTID  为"0"   如果没有子节点，也就是本身是叶子节点的时候ISLEAF = "1"
    //  DISPLAY_ORDER 是同一级中 显示的顺序
    //如果需要做选中 单选或者多选，只需要给TreePoint增加一个选中的属性，在ReasonAdapter中处理就好了
    private void initData() {
        pointList.clear();

        pointList.add(new TreePoint("1", "Tipiṭaka(Mūla)", "", "0", "0", 1));
        {
            pointList.add(new TreePoint("5", "Vinayapiṭaka", "", "1", "0", 1));
            {
                pointList.add(new TreePoint("101", "Pārājikapāḷi", "vin01m.mul", "5", "1", 1));
                pointList.add(new TreePoint("102", "Pācittiyapāḷi", "vin02m1.mul", "5", "1", 2));
                pointList.add(new TreePoint("103", "Mahāvaggapāḷi", "vin02m2.mul", "5", "1", 3));
                pointList.add(new TreePoint("104", "Cūḷavaggapāḷi", "vin02m3.mul", "5", "1", 4));
                pointList.add(new TreePoint("105", "Parivārapāḷi", "vin02m4.mul", "5", "1", 5));
            }
            pointList.add(new TreePoint("6", "Suttapiṭaka", "", "1", "0", 2));
            {
                pointList.add(new TreePoint("23", "Dīghanikāya", "", "6", "0", 1));
                {
                    pointList.add(new TreePoint("106", "Sīlakkhandhavaggapāḷi", "s0101m.mul", "23", "1", 1));
                    pointList.add(new TreePoint("107", "Mahāvaggapāḷi", "s0102m.mul", "23", "1", 2));
                    pointList.add(new TreePoint("108", "Pāthikavaggapāḷi", "s0103m.mul", "23", "1", 3));
                }
                pointList.add(new TreePoint("24", "Majjhimanikāya", "", "6", "0", 2));
                {
                    pointList.add(new TreePoint("109", "Mūlapaṇṇāsapāḷi", "s0201m.mul", "24", "1", 1));
                    pointList.add(new TreePoint("110", "Majjhimapaṇṇāsapāḷi", "s0202m.mul", "24", "1", 2));
                    pointList.add(new TreePoint("111", "Uparipaṇṇāsapāḷi", "s0203m.mul", "24", "1", 3));
                }
                pointList.add(new TreePoint("25", "Saṃyuttanikāya", "", "6", "0", 3));
                {
                    pointList.add(new TreePoint("112", "Sagāthāvaggapāḷi", "s0301m.mul", "25", "1", 1));
                    pointList.add(new TreePoint("113", "Nidānavaggapāḷi", "s0302m.mul", "25", "1", 2));
                    pointList.add(new TreePoint("114", "Khandhavaggapāḷi", "s0303m.mul", "25", "1", 3));
                    pointList.add(new TreePoint("115", "Saḷāyatanavaggapāḷi", "s0304m.mul", "25", "1", 4));
                    pointList.add(new TreePoint("116", "Mahāvaggapāḷi", "s0305m.mul", "25", "1", 5));
                }
                pointList.add(new TreePoint("26", "Aṅguttaranikāya", "", "6", "0", 4));
                {
                    pointList.add(new TreePoint("117", "Ekakanipātapāḷi", "s0401m.mul", "26", "1", 1));
                    pointList.add(new TreePoint("118", "Dukanipātapāḷi", "s0402m1.mul", "26", "1", 2));
                    pointList.add(new TreePoint("119", "Tikanipātapāḷi", "s0402m2.mul", "26", "1", 3));
                    pointList.add(new TreePoint("120", "Catukkanipātapāḷi", "s0402m3.mul", "26", "1", 4));
                    pointList.add(new TreePoint("121", "Pañcakanipātapāḷi", "s0403m1.mul", "26", "1", 5));
                    pointList.add(new TreePoint("122", "Chakkanipātapāḷi", "s0403m2.mul", "26", "1", 6));
                    pointList.add(new TreePoint("123", "Sattakanipātapāḷi", "s0403m3.mul", "26", "1", 7));
                    pointList.add(new TreePoint("124", "Aṭṭhakādinipātapāḷi", "s0404m1.mul", "26", "1", 8));
                    pointList.add(new TreePoint("125", "Navakanipātapāḷi", "s0404m2.mul", "26", "1", 9));
                    pointList.add(new TreePoint("126", "Dasakanipātapāḷi", "s0404m3.mul", "26", "1", 10));
                    pointList.add(new TreePoint("127", "Ekādasakanipātapāḷi", "s0404m4.mul", "26", "1", 11));
                }
                pointList.add(new TreePoint("27", "Khuddakanikāya", "", "6", "0", 5));
                {
                    pointList.add(new TreePoint("128", "Khuddakapāṭhapāḷi", "s0501m.mul", "27", "1", 1));
                    pointList.add(new TreePoint("129", "Dhammapadapāḷi", "s0502m.mul", "27", "1", 2));
                    pointList.add(new TreePoint("130", "Udānapāḷi", "s0503m.mul", "27", "1", 3));
                    pointList.add(new TreePoint("131", "Itivuttakapāḷi", "s0504m.mul", "27", "1", 4));
                    pointList.add(new TreePoint("132", "Suttanipātapāḷi", "s0505m.mul", "27", "1", 5));
                    pointList.add(new TreePoint("133", "Vimānavatthupāḷi", "s0506m.mul", "27", "1", 6));
                    pointList.add(new TreePoint("134", "Petavatthupāḷi", "s0507m.mul", "27", "1", 7));
                    pointList.add(new TreePoint("135", "Theragāthāpāḷi", "s0508m.mul", "27", "1", 8));
                    pointList.add(new TreePoint("136", "Therīgāthāpāḷi", "s0509m.mul", "27", "1", 9));
                    pointList.add(new TreePoint("137", "Apadānapāḷi-1", "s0510m1.mul", "27", "1", 10));
                    pointList.add(new TreePoint("138", "Apadānapāḷi-2", "s0510m2.mul", "27", "1", 11));
                    pointList.add(new TreePoint("139", "Buddhavaṃsapāḷi", "s0511m.mul", "27", "1", 12));
                    pointList.add(new TreePoint("140", "Cariyāpiṭakapāḷi", "s0512m.mul", "27", "1", 13));
                    pointList.add(new TreePoint("141", "Jātakapāḷi-1", "s0513m.mul", "27", "1", 14));
                    pointList.add(new TreePoint("142", "Jātakapāḷi-2", "s0514m.mul", "27", "1", 15));
                    pointList.add(new TreePoint("143", "Mahāniddesapāḷi", "s0515m.mul", "27", "1", 16));
                    pointList.add(new TreePoint("144", "Cūḷaniddesapāḷi", "s0516m.mul", "27", "1", 17));
                    pointList.add(new TreePoint("145", "Paṭisambhidāmaggapāḷi", "s0517m.mul", "27", "1", 18));
                    pointList.add(new TreePoint("146", "Nettippakaraṇapāḷi", "s0519m.mul", "27", "1", 19));
                    pointList.add(new TreePoint("147", "Milindapañhapāḷi", "s0518m.nrf", "27", "1", 20));
                    pointList.add(new TreePoint("148", "Peṭakopadesapāḷi", "s0520m.nrf", "27", "1", 21));
                }
            }
            pointList.add(new TreePoint("7", "Abhidhammapiṭaka", "", "1", "0", 3));
            {
                pointList.add(new TreePoint("149", "Dhammasaṅgaṇīpāḷi", "abh01m.mul", "7", "1", 1));
                pointList.add(new TreePoint("150", "Vibhaṅgapāḷi", "abh02m.mul", "7", "1", 2));
                pointList.add(new TreePoint("151", "Dhātukathāpāḷi", "abh03m1.mul", "7", "1", 3));
                pointList.add(new TreePoint("152", "Puggalapaññattipāḷi", "abh03m2.mul", "7", "1", 4));
                pointList.add(new TreePoint("153", "Kathāvatthupāḷi", "abh03m3.mul", "7", "1", 5));
                pointList.add(new TreePoint("154", "Yamakapāḷi-1", "abh03m4.mul", "7", "1", 6));
                pointList.add(new TreePoint("155", "Yamakapāḷi-2", "abh03m5.mul", "7", "1", 7));
                pointList.add(new TreePoint("156", "Yamakapāḷi-3", "abh03m6.mul", "7", "1", 8));
                pointList.add(new TreePoint("157", "Paṭṭhānapāḷi-1", "abh03m7.mul", "7", "1", 9));
                pointList.add(new TreePoint("158", "Paṭṭhānapāḷi-2", "abh03m8.mul", "7", "1", 10));
                pointList.add(new TreePoint("159", "Paṭṭhānapāḷi-3", "abh03m9.mul", "7", "1", 11));
                pointList.add(new TreePoint("160", "Paṭṭhānapāḷi-4", "abh03m10.mul", "7", "1", 12));
                pointList.add(new TreePoint("161", "Paṭṭhānapāḷi-5", "abh03m11.mul", "7", "1", 13));
            }
        }

        pointList.add(new TreePoint("2", "Aṭṭhakathā", "", "0", "0", 2));
        {
            pointList.add(new TreePoint("8", "Vinayapiṭaka (aṭṭhakathā)", "", "2", "0", 1));
            {
                pointList.add(new TreePoint("162", "Pārājikakaṇḍa-aṭṭhakathā", "vin01a.att", "8", "1", 1));
                pointList.add(new TreePoint("163", "Pācittiya-aṭṭhakathā", "vin02a1.att", "8", "1", 2));
                pointList.add(new TreePoint("164", "Mahāvagga-aṭṭhakathā", "vin02a2.att", "8", "1", 3));
                pointList.add(new TreePoint("165", "Cūḷavagga-aṭṭhakathā", "vin02a3.att", "8", "1", 4));
                pointList.add(new TreePoint("166", "Parivāra-aṭṭhakathā", "vin02a4.att", "8", "1", 5));
            }
            pointList.add(new TreePoint("9", "Suttapiṭaka (aṭṭhakathā)", "", "2", "0", 2));
            {
                pointList.add(new TreePoint("28", "Dīgha nikāya (aṭṭhakathā)", "", "9", "0", 1));
                {
                    pointList.add(new TreePoint("167", "Sīlakkhandhavagga-aṭṭhakathā", "s0101a.att", "28", "1", 1));
                    pointList.add(new TreePoint("168", "Mahāvagga-aṭṭhakathā", "s0102a.att", "28", "1", 2));
                    pointList.add(new TreePoint("169", "Pāthikavagga-aṭṭhakathā", "s0103a.att", "28", "1", 3));
                }
                pointList.add(new TreePoint("29", "Majjhimanikāya (aṭṭhakathā)", "", "9", "0", 2));
                {
                    pointList.add(new TreePoint("170", "Mūlapaṇṇāsa-aṭṭhakathā", "s0201a.att", "29", "1", 1));
                    pointList.add(new TreePoint("171", "Majjhimapaṇṇāsa-aṭṭhakathā", "s0202a.att", "29", "1", 2));
                    pointList.add(new TreePoint("172", "Uparipaṇṇāsa-aṭṭhakathā", "s0203a.att", "29", "1", 3));
                }
                pointList.add(new TreePoint("30", "Saṃyuttanikāya (aṭṭhakathā)", "", "9", "0", 3));
                {
                    pointList.add(new TreePoint("173", "Sagāthāvagga-aṭṭhakathā", "s0301a.att", "30", "1", 1));
                    pointList.add(new TreePoint("174", "Nidānavagga-aṭṭhakathā", "s0302a.att", "30", "1", 2));
                    pointList.add(new TreePoint("175", "Khandhavagga-aṭṭhakathā", "s0303a.att", "30", "1", 3));
                    pointList.add(new TreePoint("176", "Saḷāyatanavagga-aṭṭhakathā", "s0304a.att", "30", "1", 4));
                    pointList.add(new TreePoint("177", "Mahāvagga-aṭṭhakathā", "s0305a.att", "30", "1", 5));
                }
                pointList.add(new TreePoint("31", "Aṅguttaranikāya (aṭṭhakathā)", "", "9", "0", 4));
                {
                    pointList.add(new TreePoint("178", "Ekakanipāta-aṭṭhakathā", "s0401a.att", "31", "1", 1));
                    pointList.add(new TreePoint("179", "Duka-tika-catukkanipāta-aṭṭhakathā", "s0402a.att", "31", "1", 2));
                    pointList.add(new TreePoint("180", "Pañcaka-chakka-sattakanipāta-aṭṭhakathā", "s0403a.att", "31", "1", 3));
                    pointList.add(new TreePoint("181", "Aṭṭhakādinipāta-aṭṭhakathā", "s0404a.att", "31", "1", 4));
                }
                pointList.add(new TreePoint("32", "Khuddakanikāya (aṭṭhakathā)", "", "9", "0", 5));
                {
                    pointList.add(new TreePoint("182", "Khuddakapāṭha-aṭṭhakathā", "s0501a.att", "32", "1", 1));
                    pointList.add(new TreePoint("183", "Dhammapada-aṭṭhakathā", "s0502a.att", "32", "1", 2));
                    pointList.add(new TreePoint("184", "Udāna-aṭṭhakathā", "s0503a.att", "32", "1", 3));
                    pointList.add(new TreePoint("185", "Itivuttaka-aṭṭhakathā", "s0504a.att", "32", "1", 4));
                    pointList.add(new TreePoint("186", "Suttanipāta-aṭṭhakathā", "s0505a.att", "32", "1", 5));
                    pointList.add(new TreePoint("187", "Vimānavatthu-aṭṭhakathā", "s0506a.att", "32", "1", 6));
                    pointList.add(new TreePoint("188", "Petavatthu-aṭṭhakathā", "s0507a.att", "32", "1", 7));
                    pointList.add(new TreePoint("189", "Theragāthā-aṭṭhakathā-1", "s0508a1.att", "32", "1", 8));
                    pointList.add(new TreePoint("190", "Theragāthā-aṭṭhakathā-2", "s0508a2.att", "32", "1", 9));
                    pointList.add(new TreePoint("191", "Therīgāthā-aṭṭhakathā", "s0509a.att", "32", "1", 10));
                    pointList.add(new TreePoint("192", "Apadāna-aṭṭhakathā", "s0510a.att", "32", "1", 11));
                    pointList.add(new TreePoint("193", "Buddhavaṃsa-aṭṭhakathā", "s0511a.att", "32", "1", 12));
                    pointList.add(new TreePoint("194", "Cariyāpiṭaka-aṭṭhakathā", "s0512a.att", "32", "1", 13));
                    pointList.add(new TreePoint("195", "Jātaka-aṭṭhakathā-1", "s0513a1.att", "32", "1", 14));
                    pointList.add(new TreePoint("196", "Jātaka-aṭṭhakathā-2", "s0513a2.att", "32", "1", 15));
                    pointList.add(new TreePoint("197", "Jātaka-aṭṭhakathā-3", "s0513a3.att", "32", "1", 16));
                    pointList.add(new TreePoint("198", "Jātaka-aṭṭhakathā-4", "s0513a4.att", "32", "1", 17));
                    pointList.add(new TreePoint("199", "Jātaka-aṭṭhakathā-5", "s0514a1.att", "32", "1", 18));
                    pointList.add(new TreePoint("200", "Jātaka-aṭṭhakathā-6", "s0514a2.att", "32", "1", 19));
                    pointList.add(new TreePoint("201", "Jātaka-aṭṭhakathā-7", "s0514a3.att", "32", "1", 20));
                    pointList.add(new TreePoint("202", "Mahāniddesa-aṭṭhakathā", "s0515a.att", "32", "1", 21));
                    pointList.add(new TreePoint("203", "Cūḷaniddesa-aṭṭhakathā", "s0516a.att", "32", "1", 22));
                    pointList.add(new TreePoint("204", "Paṭisambhidāmagga-aṭṭhakathā", "s0517a.att", "32", "1", 23));
                    pointList.add(new TreePoint("205", "Nettippakaraṇa-aṭṭhakathā", "s0519a.att", "32", "1", 24));
                }
            }
            pointList.add(new TreePoint("10", "Abhidhammapiṭaka (aṭṭhakathā)", "", "2", "0", 3));
            {
                pointList.add(new TreePoint("206", "Dhammasaṅgaṇi-aṭṭhakathā", "abh01a.att", "10", "1", 1));
                pointList.add(new TreePoint("207", "Sammohavinodanī-aṭṭhakathā", "abh02a.att", "10", "1", 2));
                pointList.add(new TreePoint("208", "Pañcapakaraṇa-aṭṭhakathā", "abh03a.att", "10", "1", 3));
            }
        }

        pointList.add(new TreePoint("3", "ṭīkā", "", "0", "0", 3));
        {
            pointList.add(new TreePoint("11", "Vinayapiṭaka (ṭīkā)", "", "3", "0", 1));
            {
                pointList.add(new TreePoint("209", "Sāratthadīpanī-ṭīkā-1", "vin01t1.tik", "11", "1", 1));
                pointList.add(new TreePoint("210", "Sāratthadīpanī-ṭīkā-2", "vin01t2.tik", "11", "1", 2));
                pointList.add(new TreePoint("211", "Sāratthadīpanī-ṭīkā-3", "vin02t.tik", "11", "1", 3));
                pointList.add(new TreePoint("212", "Dvemātikāpāḷi", "vin04t.nrf", "11", "1", 4));
                pointList.add(new TreePoint("213", "Vinayasaṅgaha-aṭṭhakathā", "vin05t.nrf", "11", "1", 5));
                pointList.add(new TreePoint("214", "Vajirabuddhi-ṭīkā", "vin06t.nrf", "11", "1", 6));
                pointList.add(new TreePoint("215", "Vimativinodanī-ṭīkā", "vin07t.nrf", "11", "1", 7));
                pointList.add(new TreePoint("216", "Vinayālaṅkāra-ṭīkā", "vin08t.nrf", "11", "1", 8));
                pointList.add(new TreePoint("217", "Kaṅkhāvitaraṇīpurāṇa-ṭīkā", "vin09t.nrf", "11", "1", 9));
                pointList.add(new TreePoint("218", "Vinayavinicchaya-uttaravinicchaya", "vin10t.nrf", "11", "1", 10));
                pointList.add(new TreePoint("219", "Vinayavinicchaya-ṭīkā", "vin11t.nrf", "11", "1", 11));
                pointList.add(new TreePoint("220", "Pācityādiyojanāpāḷi", "vin12t.nrf", "11", "1", 12));
                pointList.add(new TreePoint("221", "Khuddasikkhā-mūlasikkhā", "vin13t.nrf", "11", "1", 13));
            }
            pointList.add(new TreePoint("12", "Suttapiṭaka (ṭīkā)", "", "3", "0", 2));
            {
                pointList.add(new TreePoint("33", "Dīghanikāya (ṭīkā)", "", "12", "0", 1));
                {
                    pointList.add(new TreePoint("222", "Sīlakkhandhavagga-ṭīkā", "s0101t.tik", "33", "1", 1));
                    pointList.add(new TreePoint("223", "Mahāvagga-ṭīkā", "s0102t.tik", "33", "1", 2));
                    pointList.add(new TreePoint("224", "Pāthikavagga-ṭīkā", "s0103t.tik", "33", "1", 3));
                    pointList.add(new TreePoint("225", "Sīlakkhandhavagga-abhinavaṭīkā-1", "s0104t.nrf", "33", "1", 4));
                    pointList.add(new TreePoint("226", "Sīlakkhandhavagga-abhinavaṭīkā-2", "s0105t.nrf", "33", "1", 5));
                }
                pointList.add(new TreePoint("34", "Majjhimanikāya (ṭīkā)", "", "12", "0", 2));
                {
                    pointList.add(new TreePoint("227", "Mūlapaṇṇāsa-ṭīkā", "s0201t.tik", "34", "1", 1));
                    pointList.add(new TreePoint("228", "Majjhimapaṇṇāsa-ṭīkā", "s0202t.tik", "34", "1", 2));
                    pointList.add(new TreePoint("229", "Uparipaṇṇāsa-ṭīkā", "s0203t.tik", "34", "1", 3));
                }
                pointList.add(new TreePoint("35", "Saṃyuttanikāya (ṭīkā)", "", "12", "0", 3));
                {
                    pointList.add(new TreePoint("230", "Sagāthāvagga-ṭīkā", "s0301t.tik", "35", "1", 1));
                    pointList.add(new TreePoint("231", "Nidānavagga-ṭīkā", "s0302t.tik", "35", "1", 2));
                    pointList.add(new TreePoint("232", "Khandhavagga-ṭīkā", "s0303t.tik", "35", "1", 3));
                    pointList.add(new TreePoint("233", "Saḷāyatanavagga-ṭīkā", "s0304t.tik", "35", "1", 4));
                    pointList.add(new TreePoint("234", "Mahāvagga-ṭīkā", "s0305t.tik", "35", "1", 5));
                }
                pointList.add(new TreePoint("36", "Aṅguttaranikāya (ṭīkā)", "", "12", "0", 4));
                {
                    pointList.add(new TreePoint("235", "Ekakanipāta-ṭīkā", "s0401t.tik", "36", "1", 1));
                    pointList.add(new TreePoint("236", "Duka-tika-catukkanipāta-ṭīkā", "s0402t.tik", "36", "1", 2));
                    pointList.add(new TreePoint("237", "Pañcaka-chakka-sattakanipāta-ṭīkā", "s0403t.tik", "36", "1", 3));
                    pointList.add(new TreePoint("238", "Aṭṭhakādinipāta-ṭīkā", "s0404t.tik", "36", "1", 4));
                }
                pointList.add(new TreePoint("37", "Khuddakanikāya (ṭīkā)", "", "12", "0", 5));
                {
                    pointList.add(new TreePoint("239", "Nettippakaraṇa-ṭīkā", "s0519t.tik", "37", "1", 1));
                    pointList.add(new TreePoint("240", "Nettivibhāvinī", "s0501t.nrf", "37", "1", 2));
                }
            }
            pointList.add(new TreePoint("13", "Abhidhammapiṭaka (ṭīkā)", "", "3", "0", 3));
            {
                pointList.add(new TreePoint("241", "Dhammasaṅgaṇī-mūlaṭīkā", "abh01t.tik", "13", "1", 1));
                pointList.add(new TreePoint("242", "Vibhaṅga-mūlaṭīkā", "abh02t.tik", "13", "1", 2));
                pointList.add(new TreePoint("243", "Pañcapakaraṇa-mūlaṭīkā", "abh03t.tik", "13", "1", 3));
                pointList.add(new TreePoint("244", "Dhammasaṅgaṇī-anuṭīkā", "abh04t.nrf", "13", "1", 4));
                pointList.add(new TreePoint("245", "Pañcapakaraṇa-anuṭīkā", "abh05t.nrf", "13", "1", 5));
                pointList.add(new TreePoint("246", "Abhidhammāvatāro-nāmarūpaparicchedo", "abh06t.nrf", "13", "1", 6));
                pointList.add(new TreePoint("247", "Abhidhammatthasaṅgaho", "abh07t.nrf", "13", "1", 7));
                pointList.add(new TreePoint("248", "Abhidhammāvatāra-purāṇaṭīkā", "abh08t.nrf", "13", "1", 8));
                pointList.add(new TreePoint("249", "Abhidhammamātikāpāḷi", "abh09t.nrf", "13", "1", 9));
            }
        }

        pointList.add(new TreePoint("4", "Añña", "", "0", "0", 4));
        {
            pointList.add(new TreePoint("14", "Visuddhimagga", "", "4", "0", 1));
            {
                pointList.add(new TreePoint("250", "Visuddhimagga-1", "e0101n.mul", "14", "1", 1));
                pointList.add(new TreePoint("251", "Visuddhimagga-2", "e0102n.mul", "14", "1", 2));
                pointList.add(new TreePoint("252", "Visuddhimagga-mahāṭīkā-1", "e0103n.att", "14", "1", 3));
                pointList.add(new TreePoint("253", "Visuddhimagga-mahāṭīkā-2", "e0104n.att", "14", "1", 4));
                pointList.add(new TreePoint("254", "Visuddhimagga nidānakathā", "e0105n.nrf", "14", "1", 5));
            }
            pointList.add(new TreePoint("15", "Saṅgāyana-puccha vissajjanā", "", "4", "0", 2));
            {
                pointList.add(new TreePoint("255", "Dīghanikāya (pu-vi)", "e0901n.nrf", "15", "1", 1));
                pointList.add(new TreePoint("256", "Majjhimanikāya (pu-vi)", "e0902n.nrf", "15", "1", 2));
                pointList.add(new TreePoint("257", "Saṃyuttanikāya (pu-vi)", "e0903n.nrf", "15", "1", 3));
                pointList.add(new TreePoint("258", "Aṅguttaranikāya (pu-vi)", "e0904n.nrf", "15", "1", 4));
                pointList.add(new TreePoint("259", "Vinayapiṭaka (pu-vi)", "e0905n.nrf", "15", "1", 5));
                pointList.add(new TreePoint("260", "Abhidhammapiṭaka (pu-vi)", "e0906n.nrf", "15", "1", 6));
                pointList.add(new TreePoint("261", "Aṭṭhakathā (pu-vi)", "e0907n.nrf", "15", "1", 7));
            }
            pointList.add(new TreePoint("16", "Leḍī sayāḍo gantha-saṅgaho", "", "4", "0", 3));
            {
                pointList.add(new TreePoint("262", "Niruttidīpanī", "e0201n.nrf", "16", "1", 1));
                pointList.add(new TreePoint("263", "Paramatthadīpanī Saṅgahamahāṭīkāpāṭha", "e0301n.nrf", "16", "1", 2));
                pointList.add(new TreePoint("264", "Anudīpanīpāṭha", "e0401n.nrf", "16", "1", 3));
                pointList.add(new TreePoint("265", "Paṭṭhānuddesa dīpanīpāṭha", "e0501n.nrf", "16", "1", 4));
            }
            pointList.add(new TreePoint("17", "Buddha-vandanā gantha-saṅgaho", "", "4", "0", 4));
            {
                pointList.add(new TreePoint("266", "Namakkāraṭīkā", "e0601n.nrf", "17", "1", 1));
                pointList.add(new TreePoint("267", "Mahāpaṇāmapāṭha", "e0602n.nrf", "17", "1", 2));
                pointList.add(new TreePoint("268", "Lakkhaṇāto buddhathomanāgāthā", "e0603n.nrf", "17", "1", 3));
                pointList.add(new TreePoint("269", "Sutavandanā", "e0604n.nrf", "17", "1", 4));
                pointList.add(new TreePoint("270", "Jinālaṅkāra", "e0605n.nrf", "17", "1", 5));
                pointList.add(new TreePoint("271", "Kamalāñjali", "e0606n.nrf", "17", "1", 6));
                pointList.add(new TreePoint("272", "Pajjamadhu", "e0607n.nrf", "17", "1", 7));
                pointList.add(new TreePoint("273", "Buddhaguṇagāthāvalī", "e0608n.nrf", "17", "1", 8));
            }
            pointList.add(new TreePoint("18", "Vaṃsa-gantha-saṅgaho", "", "4", "0", 5));
            {
                pointList.add(new TreePoint("274", "Cūḷaganthavaṃsa", "e0701n.nrf", "18", "1", 1));
                pointList.add(new TreePoint("275", "Sāsanavaṃsa", "e0702n.nrf", "18", "1", 2));
                pointList.add(new TreePoint("276", "Mahāvaṃsa", "e0703n.nrf", "18", "1", 3));
            }
            pointList.add(new TreePoint("19", "Byākaraṇa gantha-saṅgaho", "", "4", "0", 6));
            {
                pointList.add(new TreePoint("277", "Moggallānabyākaraṇaṃ", "e0801n.nrf", "19", "1", 1));
                pointList.add(new TreePoint("278", "Kaccāyanabyākaraṇaṃ", "e0802n.nrf", "19", "1", 2));
                pointList.add(new TreePoint("279", "Saddanītippakaraṇaṃ (padamālā)", "e0803n.nrf", "19", "1", 3));
                pointList.add(new TreePoint("280", "Saddanītippakaraṇaṃ (dhātumālā)", "e0804n.nrf", "19", "1", 4));
                pointList.add(new TreePoint("281", "Padarūpasiddhi", "e0805n.nrf", "19", "1", 5));
                pointList.add(new TreePoint("282", "Mogallānapañcikā", "e0806n.nrf", "19", "1", 6));
                pointList.add(new TreePoint("283", "Payogasiddhipāṭha", "e0807n.nrf", "19", "1", 7));
                pointList.add(new TreePoint("284", "Vuttodayapāṭha", "e0808n.nrf", "19", "1", 8));
                pointList.add(new TreePoint("285", "Abhidhānappadīpikāpāṭha", "e0809n.nrf", "19", "1", 9));
                pointList.add(new TreePoint("286", "Abhidhānappadīpikāṭīkā", "e0810n.nrf", "19", "1", 10));
                pointList.add(new TreePoint("287", "Subodhālaṅkārapāṭha", "e0811n.nrf", "19", "1", 11));
                pointList.add(new TreePoint("288", "Subodhālaṅkāraṭīkā", "e0812n.nrf", "19", "1", 12));
                pointList.add(new TreePoint("289", "Bālāvatāra gaṇṭhipadatthavinicchayasāra", "e0813n.nrf", "19", "1", 13));
            }
            pointList.add(new TreePoint("20", "Nīti-gantha-saṅgaho", "", "4", "0", 7));
            {
                pointList.add(new TreePoint("290", "Kavidappaṇanīti", "e1001n.nrf", "20", "1", 1));
                pointList.add(new TreePoint("291", "Nītimañjarī", "e1002n.nrf", "20", "1", 2));
                pointList.add(new TreePoint("292", "Dhammanīti", "e1003n.nrf", "20", "1", 3));
                pointList.add(new TreePoint("293", "Mahārahanīti", "e1004n.nrf", "20", "1", 4));
                pointList.add(new TreePoint("294", "Lokanīti", "e1005n.nrf", "20", "1", 5));
                pointList.add(new TreePoint("295", "Suttantanīti", "e1006n.nrf", "20", "1", 6));
                pointList.add(new TreePoint("296", "Sūrassatinīti", "e1007n.nrf", "20", "1", 7));
                pointList.add(new TreePoint("297", "Cāṇakyanīti", "e1008n.nrf", "20", "1", 8));
                pointList.add(new TreePoint("298", "Naradakkhadīpanī", "e1009n.nrf", "20", "1", 9));
                pointList.add(new TreePoint("299", "Caturārakkhadīpanī", "e1010n.nrf", "20", "1", 10));
            }
            pointList.add(new TreePoint("21", "Pakiṇṇaka-gantha-saṅgaho", "", "4", "0", 8));
            {
                pointList.add(new TreePoint("300", "Rasavāhinī", "e1101n.nrf", "21", "1", 1));
                pointList.add(new TreePoint("301", "Sīmavisodhanīpāṭha", "e1102n.nrf", "21", "1", 2));
                pointList.add(new TreePoint("302", "Vessantaragīti", "e1103n.nrf", "21", "1", 3));
            }
            pointList.add(new TreePoint("22", "Sihaḷa-gantha-saṅgaho", "", "4", "0", 9));
            {
                pointList.add(new TreePoint("303", "Moggallāna vuttivivaraṇapañcikā", "e1201n.nrf", "22", "1", 1));
                pointList.add(new TreePoint("304", "Thūpavaṃsa", "e1202n.nrf", "22", "1", 2));
                pointList.add(new TreePoint("305", "Dāṭhāvaṃsa", "e1203n.nrf", "22", "1", 3));
                pointList.add(new TreePoint("306", "Dhātupāṭhavilāsiniyā", "e1204n.nrf", "22", "1", 4));
                pointList.add(new TreePoint("307", "Dhātuvaṃsa", "e1205n.nrf", "22", "1", 5));
                pointList.add(new TreePoint("308", "Hatthavanagallavihāravaṃsa", "e1206n.nrf", "22", "1", 6));
                pointList.add(new TreePoint("309", "Jinacaritaya", "e1207n.nrf", "22", "1", 7));
                pointList.add(new TreePoint("310", "Jinavaṃsadīpaṃ", "e1208n.nrf", "22", "1", 8));
                pointList.add(new TreePoint("311", "Telakaṭāhagāthā", "e1209n.nrf", "22", "1", 9));
                pointList.add(new TreePoint("312", "Milidaṭīkā", "e1210n.nrf", "22", "1", 10));
                pointList.add(new TreePoint("313", "Padamañjarī", "e1211n.nrf", "22", "1", 11));
                pointList.add(new TreePoint("314", "Padasādhanaṃ", "e1212n.nrf", "22", "1", 12));
                pointList.add(new TreePoint("315", "Saddabindupakaraṇaṃ", "e1213n.nrf", "22", "1", 13));
                pointList.add(new TreePoint("316", "Kaccāyanadhātumañjusā", "e1214n.nrf", "22", "1", 14));
                pointList.add(new TreePoint("317", "Sāmantakūṭavaṇṇanā", "e1215n.nrf", "22", "1", 15));
            }
        }

        //打乱集合中的数据
        Collections.shuffle(pointList);
        //对集合中的数据重新排序
        updateData();
    }

    public void addListener() {
        lvMulu.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                adapter.onItemClick(position);
            }
        });
    }

    private void searchAdapter(Editable s) {
        adapter.setKeyword(s.toString());
    }

    //对数据排序 深度优先
    private void updateData() {
        for (TreePoint treePoint : pointList) {
            pointMap.put(treePoint.getID(), treePoint);
        }
        Collections.sort(pointList, new Comparator<TreePoint>() {
            @Override
            public int compare(TreePoint lhs, TreePoint rhs) {
                int llevel = TreeUtils.getLevel(lhs, pointMap);
                int rlevel = TreeUtils.getLevel(rhs, pointMap);
                if (llevel == rlevel) {
                    if (lhs.getPARENTID().equals(rhs.getPARENTID())) {  //左边小
                        return lhs.getDISPLAY_ORDER() > rhs.getDISPLAY_ORDER() ? 1 : -1;
                    } else {  //如果父辈id不相等
                        //同一级别，不同父辈
                        TreePoint ltreePoint = TreeUtils.getTreePoint(lhs.getPARENTID(), pointMap);
                        TreePoint rtreePoint = TreeUtils.getTreePoint(rhs.getPARENTID(), pointMap);
                        return compare(ltreePoint, rtreePoint);  //父辈
                    }
                } else {  //不同级别
                    if (llevel > rlevel) {   //左边级别大       左边小
                        if (lhs.getPARENTID().equals(rhs.getID())) {
                            return 1;
                        } else {
                            TreePoint lreasonTreePoint = TreeUtils.getTreePoint(lhs.getPARENTID(), pointMap);
                            return compare(lreasonTreePoint, rhs);
                        }
                    } else {   //右边级别大   右边小
                        if (rhs.getPARENTID().equals(lhs.getID())) {
                            return -1;
                        }
                        TreePoint rreasonTreePoint = TreeUtils.getTreePoint(rhs.getPARENTID(), pointMap);
                        return compare(lhs, rreasonTreePoint);
                    }
                }
            }
        });
        adapter.notifyDataSetChanged();
    }
    /////

    //从读经窗口中发出查词指令调用此方法
    private boolean palihan_cc_c(String nword, String pword) {

        palihan_cc(nword, pword);
        if ((((listFirstNo >= 0) && (listFirstNo <= NUM - 1)) && ((listEndNo >= 0) && (listEndNo <= NUM - 1))) && ((listEndNo - listFirstNo) >= 0)) {
            return true;
        } else {
            if (nword.length() >= 2) {
                palihan_cc_c(nword.substring(0, nword.length() - 1), pword);
            } else {
                return false;
            }
        }
        return true;
    }

    /**
     * 按行读取词典信息文件dicinfo，取得各词典信息，存入一个字符串数组，并将之返回
     *
     * @return
     */
    private String[] getDicinfo() {
        String[] arrDict = {"a"};

        //File sdcDir = Environment.getExternalStorageDirectory();
        File filePath = new File(sdcDir, "/dicinfo");
        try {
            RandomAccessFile rf = new RandomAccessFile(filePath, "r");
            byte[] buffer;
            int iTemp = 0;
            int iOffset = 3;
            int iLen = 0;
            int j = 0;
            int dictNum = 0;
            rf.seek(iOffset);
            while (iTemp != -1) {
                iTemp = rf.read();
                iLen++;
                if (iTemp == 13) {
                    iTemp = rf.read();
                    iLen++;
                    if (iTemp == 10) {
                        j++;

                        rf.seek(iOffset);
                        buffer = new byte[iLen - 2];
                        rf.read(buffer);

                        if (j == 2) {
                            dictNum = Integer.parseInt(EncodingUtils.getString(buffer, "UTF-8"));
                            arrDict = new String[dictNum];
                        } else if (j > 2) {
                            arrDict[j - 3] = EncodingUtils.getString(buffer, "UTF-8");
                        } else {

                        }

                        iOffset = iOffset + iLen;
                        rf.seek(iOffset);
                        iLen = 0;
                    }
                }
            }
            rf.close();
        } catch (FileNotFoundException e) {
            e.printStackTrace();
        } catch (IOException e) {
            e.printStackTrace();
        }

        return arrDict;
    }

    protected void sdcDialog() {
        AlertDialog.Builder builder = new AlertDialog.Builder(CanonActivity.this);
        builder.setMessage("词库不存在，或者您没有把词库复制到正确的存储卡上或者是词库目录有误，请在存储卡的根目录下建一个/pceddata子目录，然后把词库文件复制进去，如果您的手机有两个存储卡，尝试把/pceddata目录建到内置存储卡上。");
        builder.setTitle("错误提示：");
        builder.setPositiveButton("确定",
                new android.content.DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        dialog.dismiss();
                        CanonActivity.this.finish();
                    }
                });
        builder.create().show();
    }

    protected void exitDialog() {
        AlertDialog.Builder builder = new AlertDialog.Builder(CanonActivity.this);
        builder.setMessage("Exit this Pali program?");
        builder.setTitle("tip:");
        builder.setPositiveButton("ok",
                new android.content.DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        dialog.dismiss();
                        CanonActivity.this.finish();
                    }
                });
        builder.setNegativeButton("cancel",
                new android.content.DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        dialog.dismiss();
                    }
                });
        builder.create().show();
    }

    @Override
    public boolean onKeyDown(int keyCode, KeyEvent event) {
        if (keyCode == KeyEvent.KEYCODE_BACK && event.getRepeatCount() == 0) {
            if (canBack) {
                webView.loadDataWithBaseURL("about:blank", "<script>function tttop(){scrollTo(0,0);}</script>" + "<html><head><style type=\"text/css\">@font-face{font-family:\"Zawgyi-One\";src:url(\"file:///android_asset/fonts/ZawgyiOne2008.ttf\");}body{font-family:\"Zawgyi-One\";}pre{font-family:\"Zawgyi-One\";}</style>"+htmlColor+"</head><body onscroll = \"window.lookup.javaonscroll('" + "javaonscroll" + "')\">" + bufferHtml + "</body></html>", "text/html", "utf-8", null);
                canBack = false;

                return false;
            }
            exitDialog();
            return false;
        }
        return false;
    }

    /**
     * @param z
     * @return index文件中第z+1个单词词目，z从0到NUM-1，NUM为词条数
     */
    public static String getinxword(byte[] bufferIndex, int[] iArrinxOffset, int[] iArrinxLen, int z) {
        byte[] buffer = new byte[iArrinxLen[z]];
        for (int h = 0; h < iArrinxLen[z]; h++) {
            buffer[h] = bufferIndex[iArrinxOffset[z] - 3 + h];
        }
        return EncodingUtils.getString(buffer, "UTF-8");
    }

    /**
     * @param z
     * @return index文件中第z+1个单词词目，z从0到NUM-1，NUM为词条数
     */
    public String getword(int z) {
        int iOffset;
        int iLen;

        //File sdcDir = Environment.getExternalStorageDirectory();
        File findex = new File(sdcDir, "/index");
        File finxoff = new File(sdcDir, "/inxioff");
        File finxlen = new File(sdcDir, "/inxilen");

        try {
            RandomAccessFile rindex = new RandomAccessFile(findex, "r");
            RandomAccessFile rfinxoff = new RandomAccessFile(finxoff, "r");
            RandomAccessFile rfinxlen = new RandomAccessFile(finxlen, "r");

            rfinxoff.seek(z * 4);
            iOffset = rfinxoff.readInt();

            rfinxlen.seek(z * 4);
            iLen = rfinxlen.readInt();

            //byte[] buffer = new byte[iArrinxLen[z]];
            //rindex.seek(iArrinxOffset[z]);
            byte[] buffer = new byte[iLen];
            rindex.seek(iOffset);
            rindex.read(buffer);

            rindex.close();
            rfinxoff.close();
            rfinxlen.close();

            return EncodingUtils.getString(buffer, "UTF-8");
        } catch (FileNotFoundException e) {
            e.printStackTrace();
        } catch (IOException e) {
            e.printStackTrace();
        }

        return "";
    }

    /**
     * @param z
     * @return cidian文件中第z + 1个词条释义的语言类别：汉C 英E 缅M 越V
     */
    public char getlanguage(int z) {
        switch (getdicsymbol(z)) {
            //汉、日文类别
            case 'H':
                return 'C';
            case 'T':
                return 'C';
            case 'S':
                return 'C';
            case 'A':
                return 'C';
            case 'J':
                return 'C';
            case 'M':
                return 'C';
            case 'D':
                return 'C';
            case 'F':
                return 'C';
            case 'G':
                return 'C';
            case 'W':
                return 'C';
            case 'Z':
                return 'C';
            case 'X':
                return 'C';

            //英文类别
            case 'N':
                return 'E';
            case 'C':
                return 'E';
            case 'P':
                return 'E';
            case 'V':
                return 'E';
            case 'I':
                return 'E';

            //缅文类别
            case 'B':
                return 'M';
            case 'K':
                return 'M';
            case 'O':
                return 'M';
            case 'R':
                return 'M';

            //越文类别
            case 'U':
                return 'V';
            case 'Q':
                return 'V';
            case 'E':
                return 'V';

            //其它类别
            default:
                return '0';
        }
    }

    /**
     * @param z
     * @return cidian文件中第z + 1个词条的第一个字符、即词典标识：一个大写英文字母，出错则返回'0'
     */
    public char getdicsymbol(int z) {
        int iOffset;
        char a = '0';

        //File sdcDir = Environment.getExternalStorageDirectory();
        File file = new File(sdcDir, "/cidian");
        File foffset = new File(sdcDir, "/itemioff");
        try {
            RandomAccessFile rf = new RandomAccessFile(file, "r");
            RandomAccessFile rfo = new RandomAccessFile(foffset, "r");

            rfo.seek(z * 4);
            iOffset = rfo.readInt();

            rf.seek(iOffset);
            a = (char) (rf.read());

            rf.close();
            rfo.close();
        } catch (FileNotFoundException e) {
            e.printStackTrace();
        } catch (IOException e) {
            e.printStackTrace();
        }

        return a;
    }

    public String readHtm(String filename)
    {
    /*
            AssetManager am = null;
            am = getAssets();
            InputStream is =am.open("cidian");


            int lenght = is.available();
            byte[]  buffer1 = new byte[lenght];
            is.read(buffer1);
*/

        //builder.setMessage(String.valueOf(lenght)+"词"+CanonActivity.this.getExternalCacheDir());
        //builder.setMessage("词"+CanonActivity.this.getExternalFilesDir(Environment.DIRECTORY_DCIM));
        try {
            InputStream is = this.getAssets().open("pali/"+filename); // 从assets目录下复制

            int lengh = is.available();
            byte[] buffer = new byte[lengh];
            is.read(buffer);

            //String s = new String(buffer,"UTF-8");
            String s = EncodingUtils.getString(buffer, "UTF-8");

            return s;

        }catch(Exception e){
            return "";
        }
    }

    /**
     * @param z
     * @return cidian文件中第z+1个词条解释，z从0到NUM-1，NUM为词条数
     */
    public String getexplain(int z) {
        int iOffset;
        int iLen;

        //File sdcDir = Environment.getExternalStorageDirectory();
        File file = new File(sdcDir, "/cidian");
        File foffset = new File(sdcDir, "/itemioff");
        File flength = new File(sdcDir, "/itemilen");
        try {
            RandomAccessFile rf = new RandomAccessFile(file, "r");
            RandomAccessFile rfo = new RandomAccessFile(foffset, "r");
            RandomAccessFile rfl = new RandomAccessFile(flength, "r");

            rfo.seek(z * 4);
            iOffset = rfo.readInt();

            rfl.seek(z * 4);
            iLen = rfl.readInt();

            byte[] buffer = new byte[iLen];
            rf.seek(iOffset);
            rf.read(buffer);

            rf.close();
            rfo.close();
            rfl.close();

            return EncodingUtils.getString(buffer, "UTF-8");
        } catch (FileNotFoundException e) {
            e.printStackTrace();
        } catch (IOException e) {
            e.printStackTrace();
        }

        return "";
    }

    /**
     * 当在输入框里输入字母时，随着输入，自动在webview里面列出符合的单词列表，先从列表里面删除重复的词再输出，
     * 每次最多输出100词，随着列表的被触摸或滚动，再继续追加输出余下的词
     */
    private void listDc() {
        //缺省与页面底色同色、灰色显示
        String CN = "<a style=\"color:transparent\">•</a>";
        String EN = "<a style=\"color:transparent\">•</a>";
        String MY = "<a style=\"color:transparent\">•</a>";
        String VT = "<a style=\"color:transparent\">•</a>";

        if ((listFirstNo < 0) | (listEndNo < 0)) {
            webView.loadDataWithBaseURL("about:blank", "<html><head>"+htmlColor+"</head><body></body></html>", "text/html", "utf-8", null);
            return;
        }
        if ((listFirstNo >= NUM) | (listEndNo >= NUM)) {
            webView.loadDataWithBaseURL("about:blank", "<html><head>"+htmlColor+"</head><body></body></html>", "text/html", "utf-8", null);
            return;
        }
        if (listFirstNo > listEndNo) {
            webView.loadDataWithBaseURL("about:blank", "<html><head>"+htmlColor+"</head><body></body></html>", "text/html", "utf-8", null);
            return;
        }

        int n = 0;

        String preWord = "";
        StringBuilder b = new StringBuilder("<br>");
        for (int z = listFirstNo; z <= listEndNo; z++) {
            if (n == 101) {
                b.append("<div style=\"height:12px\">符合的词较多，现只列出前100条。</div><br>");
                webView.loadDataWithBaseURL("about:blank", "<script>function tttop(){scrollTo(0,0);}</script>"+"<html><head><style type=\"text/css\">@font-face{font-family:\"Zawgyi-One\";src:url(\"file:///android_asset/fonts/ZawgyiOne2008.ttf\");}body{font-family:\"Zawgyi-One\"; }pre{font-family:\"Zawgyi-One\";}</style>"+htmlColor+"</head><body onscroll = \"window.lookup.javaonscroll('" + "javaonscroll" + "')\">" + b.toString() + "</body></html>", "text/html", "utf-8", null);

                bufferHtml = b.toString();
                return;
            }

            if (!(ABCtoabc(preWord).equals(ABCtoabc(getword(z))))) {
                n++;

                if (z > listFirstNo) {
                    b.append("<div style=\"height:18px; white-space:nowrap\" onmouseover=\"this.style.backgroundColor='#3366CC'\" onmouseout=\"this.style.color='black'\" onclick = \"window.lookup.javaonclick('" + preWord + "')\">");
                    b.append(CN);
                    b.append(EN);
                    b.append(MY);
                    b.append(VT);
                    b.append(preWord);
                    b.append("</div><br>");
                }
                preWord = getword(z);
                CN = "<a style=\"color:transparent\">•</a>";
                EN = "<a style=\"color:transparent\">•</a>";
                MY = "<a style=\"color:transparent\">•</a>";
                VT = "<a style=\"color:transparent\">•</a>";
            }

            switch (getlanguage(z)) {
                case 'C':
                    CN = "<a style=\"color:red\">•</a>";
                    break;

                case 'E':
                    EN = "<a style=\"color:green\">•</a>";
                    break;

                case 'M':
                    MY = "<a style=\"color:saddlebrown\">•</a>";
                    break;

                case 'V':
                    VT = "<a style=\"color:gray\">•</a>";
                    break;

                default:
                    ;
            }
        }

        b.append("<div style=\"height:18px; white-space:nowrap\" onmouseover=\"this.style.backgroundColor='#3366CC'\" onmouseout=\"this.style.color='black'\" onclick = \"window.lookup.javaonclick('" + preWord + "')\">");
        b.append(CN);
        b.append(EN);
        b.append(MY);
        b.append(VT);
        b.append(preWord);
        b.append("</div><br>");

        webView.loadDataWithBaseURL("about:blank", "<script>function tttop(){scrollTo(0,0);}</script>"+"<html><head><style type=\"text/css\">@font-face{font-family:\"Zawgyi-One\";src:url(\"file:///android_asset/fonts/ZawgyiOne2008.ttf\");}body{font-family:\"Zawgyi-One\";}pre{font-family:\"Zawgyi-One\";}</style>"+htmlColor+"</head><body onscroll = \"window.lookup.javaonscroll('" + "javaonscroll" + "')\">" + b.toString() + "</body></html>", "text/html", "utf-8", null);

        bufferHtml = b.toString();

    }

    /**
     * 是否使用前部匹配的模式查词，当在输入框里输入字母时，随着输入的变化，在下面webview里面显示
     * 前面几个字母符合的单词列表
     */
    boolean bwordaheadmatch = true;

    /**
     * 当在输入框里输入前面几个字母时，自动从词库词目index文件里查出符合的一列词，
     * listFirstNo是这列词的起始编号
     * listEndNo是这列词的结束编号，取得此两个编号，以用之在webview里面输出列表
     */
    int listFirstNo = -1;
    int listEndNo = -1;

    private boolean palihan_cc(String nword, String pword) {
        //strPaliWord = pword;
        /////复原其值，旧版词典（可能电脑版也是）未如此做，是否可能出错？
        listFirstNo = -1;
        listEndNo = -1;
        /////

        int n = 0;  //记录查找结果条数
        int sNo = -1, lNo = -1;
        int ibz = -3;

        if (!bwordaheadmatch) {
            ibz = edcNo(nword);
            if (ibz == 0) {
                sNo = iNo;
                while ((sNo > 0) && !eab(getword(sNo - 1), nword))
                    sNo = sNo - 1;
                lNo = sNo;
                while ((lNo < NUM - 1) && !eab(nword, getword(lNo + 1)))
                    lNo = lNo + 1;
            } else {
                //if (ziweiCc(pword))
                //    return true;
                //else
                //    return false;
                return false;
            }
        } else {
            ibz = edcNo(nword);
            if (ibz == -2) {
                //if (ziweiCc(pword))
                //    return true;
                //else
                //    return false;
            }
            if (ibz == 0) {
                sNo = iNo;
                while ((sNo > 0) && !eab(getword(sNo - 1), nword))
                    sNo = sNo - 1;
            }
            if (ibz == 1) {
                sNo = iNo + 1;
            }
            if (ibz == -1) {
                sNo = 0;
            }

            ibz = edcNo(PadRight(nword, 'z', 68));
            if (ibz == -1) {
                //if (ziweiCc(pword))
                //    return true;
                //else
                //    return false;
            }
            if (ibz == 0) {
                lNo = iNo;
                while ((lNo < NUM - 1) && !eab(PadRight(nword, 'z', 68), getword(lNo + 1)))
                    lNo = lNo + 1;
            }
            if (ibz == 1) {
                lNo = iNo;
            }
            if (ibz == -2) {
                lNo = NUM - 1;
            }
        }

        //储存以下值，以备 listBatchOut() 函数 输出时使用
        listFirstNo = sNo;
        listEndNo = lNo;
        return true;
    }

    int iNo;

    /**
     * 词库词条数量
     */
    int NUM;

    //sL[]储存词目
    //String[] sL;

    //public static int edcNo(String d, out int iNo)
    public int edcNo(String d) {
        iNo = -1;
        int itmp = 0, min = 0, max = NUM - 1;
        if (eab(d, getword(min)))
            return -1;
        if (eab(getword(max), d))
            return -2;
        do {
            itmp = (min + max) / 2;
            if (eab(d, getword(itmp))) {
                max = itmp;
            } else {
                min = itmp;
            }
        } while (max - min > 1);
        if (!eab(d, getword(min)) & !eab(getword(min), d)) {
            iNo = min;
            return 0;
        } else if (!eab(d, getword(max)) & !eab(getword(max), d)) {
            iNo = max;
            return 0;
        } else {
            iNo = min;
            return 1;
        }
    }

    /**
     * 比较英文单词
     *
     * @param a
     * @param b
     * @return 如果a小于b，则返回true
     */
    public boolean eab(String a, String b) {
        int i;
        if (a.length() > b.length()) {
            i = a.length();
            b = PadRight(b, ' ', i);
        } else if (a.length() < b.length()) {
            i = b.length();
            a = PadRight(a, ' ', i);
        } else
            i = a.length();

        for (int j = 0; j < i; j++) {
            if (eABC(a.substring(j, j + 1).charAt(0)) < eABC(b.substring(j, j + 1).charAt(0)))
                return true;
            else if (eABC(a.substring(j, j + 1).charAt(0)) > eABC(b.substring(j, j + 1).charAt(0)))
                return false;
        }
        return false;
    }

    /**
     * 补足字符串长度
     *
     * @param s      需补足的字符串
     * @param a      char字符
     * @param length 补足后总长度
     * @return 已补足长度的字符串
     */
    public static String PadRight(String s, char a, int length) {
        StringBuilder b = new StringBuilder(s);
        while (b.length() < length) {
            b.append(a);
        }
        return b.toString();
    }

    //英文化模式
    public int eABC(char inabc) {
        switch (inabc) {
            case 'a':
                return 1;

            case 'b':
                return 2;

            case 'c':
                return 3;

            case 'd':
                return 4;

            case 'e':
                return 5;

            case 'f':
                return 6;

            case 'g':
                return 7;

            case 'h':
                return 8;

            case 'i':
                return 9;

            case 'j':
                return 10;

            case 'k':
                return 11;

            case 'l':
                return 12;

            case 'm':
                return 13;

            case 'n':
                return 14;

            case 'o':
                return 15;

            case 'p':
                return 16;

            case 'q':
                return 17;

            case 'r':
                return 18;

            case 's':
                return 19;

            case 't':
                return 20;

            case 'u':
                return 21;

            case 'v':
                return 22;

            case 'w':
                return 23;

            case 'x':
                return 24;

            case 'y':
                return 25;

            case 'z':
                return 26;

            case 'ā':
                return 1;
            case 'ī':
                return 9;
            case 'ū':
                return 21;
            case 'ṅ':
                return 14;
            case 'ñ':
                return 14;
            case 'ṭ':
                return 20;
            case 'ḍ':
                return 4;
            case 'ṇ':
                return 14;
            case 'ḷ':
                return 12;
            case 'ŋ':
                return 13;
            case 'ṁ':
                return 13;
            case 'ṃ':
                return 13;

            case 'A':
                return 1;

            case 'B':
                return 2;

            case 'C':
                return 3;

            case 'D':
                return 4;

            case 'E':
                return 5;

            case 'F':
                return 6;

            case 'G':
                return 7;

            case 'H':
                return 8;

            case 'I':
                return 9;

            case 'J':
                return 10;

            case 'K':
                return 11;

            case 'L':
                return 12;

            case 'M':
                return 13;

            case 'N':
                return 14;

            case 'O':
                return 15;

            case 'P':
                return 16;

            case 'Q':
                return 17;

            case 'R':
                return 18;

            case 'S':
                return 19;

            case 'T':
                return 20;

            case 'U':
                return 21;

            case 'V':
                return 22;

            case 'W':
                return 23;

            case 'X':
                return 24;

            case 'Y':
                return 25;

            case 'Z':
                return 26;

            case 'Ā':
                return 1;
            case 'Ī':
                return 9;
            case 'Ū':
                return 21;
            case 'Ṅ':
                return 14;
            case 'Ñ':
                return 14;
            case 'Ṭ':
                return 20;
            case 'Ḍ':
                return 4;
            case 'Ṇ':
                return 14;
            case 'Ḷ':
                return 12;
            case 'Ŋ':
                return 13;
            case 'Ṁ':
                return 13;
            case 'Ṃ':
                return 13;

            default:
                return 0;
        }
    }

    /**
     * 字符串大写转小写
     *
     * @param s
     * @return
     */
    public String ABCtoabc(String s) {
        StringBuilder b = new StringBuilder("");
        for (int i = 0; i < s.length(); i++) {
            b.append(Atoa(s.charAt(i)));
        }
        return b.toString();
    }

    /**
     * 字母大写转小写
     *
     * @param a
     * @return
     */
    public char Atoa(char a) {
        switch (a) {
            case 'A':
                return 'a';

            case 'B':
                return 'b';

            case 'C':
                return 'c';

            case 'D':
                return 'd';

            case 'E':
                return 'e';

            case 'F':
                return 'f';

            case 'G':
                return 'g';

            case 'H':
                return 'h';

            case 'I':
                return 'i';

            case 'J':
                return 'j';

            case 'K':
                return 'k';

            case 'L':
                return 'l';

            case 'M':
                return 'm';

            case 'N':
                return 'n';

            case 'O':
                return 'o';

            case 'P':
                return 'p';

            case 'Q':
                return 'q';

            case 'R':
                return 'r';

            case 'S':
                return 's';

            case 'T':
                return 't';

            case 'U':
                return 'u';

            case 'V':
                return 'v';

            case 'W':
                return 'w';

            case 'X':
                return 'x';

            case 'Y':
                return 'y';

            case 'Z':
                return 'z';

            case 'Ā':
                return 'ā';

            case 'Ī':
                return 'ī';

            case 'Ū':
                return 'ū';

            case 'Ṅ':
                return 'ṅ';

            case 'Ñ':
                return 'ñ';

            case 'Ṭ':
                return 'ṭ';

            case 'Ḍ':
                return 'ḍ';

            case 'Ṇ':
                return 'ṇ';

            case 'Ḷ':
                return 'ḷ';

            case 'Ŋ':
                return 'ŋ';

            case 'Ṁ':
                return 'ṁ';

            case 'Ṃ':
                return 'ṃ';

            default:
                return a;
        }
    }
}
