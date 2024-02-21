package com.peiyh.pcedpalidict192;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.RandomAccessFile;

import org.apache.http.util.EncodingUtils;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.app.AlertDialog;
import android.app.AlertDialog.Builder;
import android.content.Context;
import android.content.DialogInterface;
import android.os.Bundle;
import android.os.Handler;
import android.os.Environment;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.KeyEvent;
import android.view.View;
import android.view.inputmethod.InputMethodManager;
import android.webkit.WebView;
import android.webkit.JavascriptInterface;

import android.widget.Button;
import android.widget.EditText;

public class MainActivity extends Activity {

	private Handler handler = new Handler();
	WebView webView;
	
	/**
	 * 如果为true表示在当前界面可以返回显示单词列表页面
	 */
	boolean canBack = false;
	
	/**
	 * 缓存单词列表页面Html数据，以便按返回键时可以重新显示单词列表页面
	 */
	String bufferHtml = "";
	
	/**
	 * sdcard路径
	 */
	File sdcDir = new File("");
	

	@SuppressLint("JavascriptInterface")
	@Override
	protected void onCreate(Bundle icicle) {
		super.onCreate(icicle);
		setContentView(R.layout.activity_main);

		//多个可能的sdCard路径
		String[] arrSdcard={
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
		for(String sdcard : arrSdcard)
		{
			File x = new File(sdcard + "/pceddata/index");
			if (x.exists())
			{
				sdcDir = new File(sdcard);
				break;
			}
		}

		//如果词库不存在，则提示安装
		if(sdcDir.getPath().equals(""))
		{
			sdcDialog();
		}

		try{
			File foffset = new File(sdcDir,"/pceddata/itemioff");
			RandomAccessFile rfo = new RandomAccessFile(foffset,"r");
			NUM=(int)(rfo.length()/4);
			rfo.close();


			final EditText etInput=(EditText)findViewById(R.id.editText1);
			etInput.addTextChangedListener(new TextWatcher(){
	        	public void afterTextChanged(Editable s){
	        	}
	        	public void beforeTextChanged(CharSequence s,int start,int count,int after){
	        	}
	        	public void onTextChanged(CharSequence s,int start,int before,int count){
	        		String sWord = s.toString().trim();
	        		if (sWord.equals("")){
	        			webView.loadDataWithBaseURL("about:blank", "<html><head><style type=\"text/css\">body{background-color: white}</style></head><body></body></html>", "text/html", "utf-8", null);
	        			return;
	        		}
	        		
	        		bwordaheadmatch=true;
	        		palihan_cc(sWord, "");
	        		listDc();
	        	}
	        	
	        });
			
			Button btnLookup=(Button)findViewById(R.id.button1);
			btnLookup.setOnClickListener(new Button.OnClickListener(){
				public void onClick(View v){
	        		String sWord = etInput.getText().toString().trim();
	        		if (sWord.equals("")){
	        			return;
	        		}
	        		
					bwordaheadmatch=false;
	        		if (true==palihan_cc(sWord, ""))
	        		{
	        			//取得词典信息数组
	        			String[] scArr=getDicinfo();
	        			String strItem;
	        			StringBuilder b = new StringBuilder("");
	        			for (int z = listFirstNo; z <= listEndNo; z++){
	        				strItem=getexplain(z);
	        				b.append("<br>");
		        			//遍历词典信息数组中每一个元素
		        			for(String sc:scArr){
		        				if(sc.substring(1, 2).equals(strItem.substring(0,1))){
		        					b.append("<span style=\"color: DarkBlue\">");
		        					b.append(sc.substring(3, 28));
		        					b.append("</span>");
		        				}
		        			}	        				
        					b.append("<br>");
        					b.append(strItem.substring(2));
        					b.append("<br>");
	        			}
	        			webView.loadDataWithBaseURL("about:blank", "<html><head><style type=\"text/css\">@font-face{font-family:\"Zawgyi-One\";src:url(\"file:///android_asset/fonts/ZawgyiOne2008.ttf\");}body{font-family:\"Zawgyi-One\"; background-color: white;}</style></head><body>" + b.toString() + "</body></html>", "text/html", "utf-8", null);
	        		}else
	        		{
	        			webView.loadDataWithBaseURL("about:blank", "<html><head><style type=\"text/css\">body{background-color: white}</style></head><body>'<b>" + sWord + "</b>' no found! <br><br>词库里没有这个单词，这个词可能是一个词尾变形词，也可能是一个组合词，建议您删掉词尾的几个字母，从单词列表中寻找并点击一个类似的词形来查词。</body></html>", "text/html", "utf-8", null);
	        		}
	        		 canBack=false;
	        		
					InputMethodManager imm = (InputMethodManager) getSystemService(Context.INPUT_METHOD_SERVICE);
					imm.hideSoftInputFromWindow(v.getWindowToken(), 0);
				}
			});
			
			//webView = new WebView(this);
			webView = (WebView)findViewById(R.id.webView1);
			//webView.getSettings().setCacheMode(WebSettings.LOAD_CACHE_ONLY);
			webView.setScrollBarStyle(View.SCROLLBARS_INSIDE_OVERLAY);
			webView.loadDataWithBaseURL("about:blank", "<html><head><style type=\"text/css\">body{background-color: white}</style></head><body>tip:<br /><a style=\"color:red\">●</a><a style=\"color:green\">●</a><a style=\"color:saddlebrown\">●</a><a style=\"color:midnightblue\">●</a><br />red dot<a style=\"color:red\">●</a>: Chinese<br>green dot<a style=\"color:green\">●</a>: English<br>brown dot<a style=\"color:saddlebrown\">●</a>: Myanmar<br>blue dot<a style=\"color:midnightblue\">●</a>: Viet<br><br>copyright(C) 2014-2019 Alobha<br><br>本程序可自由善意分享<br><br>您在使用中若有问题或建议，<br>可以和作者<br>或“觉悟之路”网站 <br>http://dhamma.sutta.org/ <br>站长Anicca联系。<br><br>程序作者：Alobha<br>邮箱：alobha@hotmail.com <br>QQ 952395695 <br><br>Sādhu! Sādhu! Sādhu!</body></html>", "text/html", "utf-8", null);
	        //webView.loadDataWithBaseURL("about:blank", "<html><head><style type=\"text/css\">@font-face{font-family:\"Zawgyi-One\";src:url(\"file:///android_asset/fonts/ZawgyiOne2008.ttf\");}body{font-family:\"Zawgyi-One\"; background-color: gray;}pre{font-family:\"Zawgyi-One\";}</style></head><a onclick = \"window.lookup.mydata('"+"单词item"+"')\">点击调用java函数</a><br><a onclick = \"window.lookup.mydata('单词item')\">点击调用java函数</a><br><a onclick = \"window.lookup.mydata('单词item')\">点击调用java函数</a><br><a onclick = \"window.lookup.mydata('单词item')\">点击调用java函数</a><br><a onclick = \"window.lookup.mydata('单词item')\">点击调用java函数</a><pre>"+(endMili-startMili)/1.0+"毫秒.#最长单词长度：" +String.valueOf(maxdcLen)+"#第："+String.valueOf(maxdcNum)+"#"+ strInx + "#<br>" + str + "</pre><br>128āīūṅñṭḍṇḷṃṁŋ这是汉字这是日文：無常を常なりとする顛倒这是缅文：အနိစၥ(တိ)这是越文：không bền vững, vô thường。</html>", "text/html", "utf-8", null);
	        //setContentView(webView);

	        
	        webView.getSettings().setJavaScriptEnabled(true);
	        webView.addJavascriptInterface(new Object(){
	        	@JavascriptInterface
	        	public int javaonscroll(final String strItem) {
	        		
	        		handler.post(new Runnable(){
	        			@Override
	        			public void run(){
	        		
	        		InputMethodManager imm = (InputMethodManager) getSystemService(Context.INPUT_METHOD_SERVICE);
					//imm.hideSoftInputFromWindow(etInput.getWindowToken(), 0);

	        			}
	        		});
	        		
	        		return 0;
	        	}
	        	
	        	@JavascriptInterface
	        	public int javaonclick(final String strItem) {
	        		
	        		handler.post(new Runnable(){
	        			@Override
	        			public void run(){
	        		
					bwordaheadmatch=false;
	        		if (true == palihan_cc(strItem, ""))
	        		{
	        			//取得词典信息数组
	        			String[] scArr=getDicinfo();
	        			String item;
	        			StringBuilder b = new StringBuilder("");
	        			for (int z = listFirstNo; z <= listEndNo; z++){
	        				if(ABCtoabc(strItem).equals(ABCtoabc(getword(z))))
	        				{
	        					item=getexplain(z);
	        					b.append("<br>");
			        			//遍历词典信息数组中每一个元素
			        			for(String sc:scArr){
			        				if(sc.substring(1, 2).equals(item.substring(0,1))){
			        					b.append("<span style=\"color: DarkBlue\">");
			        					b.append(sc.substring(3, 28));
			        					b.append("</span>");
			        				}
			        			}
	        					b.append("<br>");
	        					b.append(item.substring(2));
	        					b.append("<br>");
	        				}
	        			}
	        			webView.loadDataWithBaseURL("about:blank", "<html><head><style type=\"text/css\">@font-face{font-family:\"Zawgyi-One\";src:url(\"file:///android_asset/fonts/ZawgyiOne2008.ttf\");}body{font-family:\"Zawgyi-One\"; background-color: white;}</style></head><body>" + b.toString() + "</body></html>", "text/html", "utf-8", null);
	        			canBack=true;
	        		}else
	        		{
	        			webView.loadDataWithBaseURL("about:blank", "<html><head><style type=\"text/css\">body{background-color: white}</style></head><body>error</body></html>", "text/html", "utf-8", null);
	        		}
	        		
	        		InputMethodManager imm = (InputMethodManager) getSystemService(Context.INPUT_METHOD_SERVICE);
					//imm.hideSoftInputFromWindow(etInput.getWindowToken(), 0);

	        			}
	        		});

	        		return 0;

	        	}
	        } , "lookup");
	        
		}

		catch(Exception e){
			e.printStackTrace();
		}
		
	}

	/**
	 * 按行读取词典信息文件dicinfo，取得各词典信息，存入一个字符串数组，并将之返回
	 * @return
	 */
	private String[] getDicinfo(){
		String[] arrDict = {"a"};
		
		//File sdcDir = Environment.getExternalStorageDirectory();
		File filePath = new File(sdcDir,"/pceddata/dicinfo");
		try{
			RandomAccessFile rf = new RandomAccessFile(filePath,"r");
			byte[] buffer;
			int iTemp=0;
			int iOffset=3;
			int iLen=0;
			int j=0;
			int dictNum=0;
			rf.seek(iOffset);
			while(iTemp!=-1){
				iTemp=rf.read();
				iLen++;
				if(iTemp==13){
					iTemp=rf.read();
					iLen++;
					if(iTemp==10){
						j++;
						
						rf.seek(iOffset);
						buffer = new byte[iLen-2];
						rf.read(buffer);
						
						if(j==2){
							dictNum=Integer.parseInt(EncodingUtils.getString(buffer, "UTF-8"));
							arrDict=new String[dictNum];
						}else if(j>2){
							arrDict[j-3]=EncodingUtils.getString(buffer, "UTF-8");
						}else{
							
						}

						iOffset = iOffset + iLen;
						rf.seek(iOffset);
						iLen = 0;
					}
				}
			}
			rf.close();
		}catch (FileNotFoundException e){
			e.printStackTrace();
		}catch(IOException e){
			e.printStackTrace();
		}
		
		return arrDict;
	}
	
	protected void sdcDialog(){
		AlertDialog.Builder builder = new Builder(MainActivity.this);
		builder.setMessage("词库不存在，或者您没有把词库复制到正确的存储卡上或者是词库目录有误，请在存储卡的根目录下建一个/pceddata子目录，然后把词库文件复制进去，如果您的手机有两个存储卡，尝试把/pceddata目录建到内置存储卡上。");
		builder.setTitle("错误提示：");
		builder.setPositiveButton("确定",
				new android.content.DialogInterface.OnClickListener(){
			@Override
			public void onClick(DialogInterface dialog, int which){
				dialog.dismiss();
				MainActivity.this.finish();
			}
		});
		builder.create().show();
	}
	
	protected void exitDialog(){
		AlertDialog.Builder builder = new Builder(MainActivity.this);
		builder.setMessage("are you sure exit this PCED program?");
		builder.setTitle("tip:");
		builder.setPositiveButton("ok",
				new android.content.DialogInterface.OnClickListener(){
			@Override
			public void onClick(DialogInterface dialog, int which){
				dialog.dismiss();
				MainActivity.this.finish();
			}
		});
		builder.setNegativeButton("cancel",
				new android.content.DialogInterface.OnClickListener(){
			@Override
			public void onClick(DialogInterface dialog, int which){
				dialog.dismiss();
			}
		});
		builder.create().show();
	}

	@Override
	public boolean onKeyDown(int keyCode, KeyEvent event){
		if(keyCode == KeyEvent.KEYCODE_BACK && event.getRepeatCount() == 0)
		{
			 if(canBack)
			 {
				 webView.loadDataWithBaseURL("about:blank", "<html><head><style type=\"text/css\">@font-face{font-family:\"Zawgyi-One\";src:url(\"file:///android_asset/fonts/ZawgyiOne2008.ttf\");}body{font-family:\"Zawgyi-One\"; background-color: white;}pre{font-family:\"Zawgyi-One\";}</style></head><body onscroll = \"window.lookup.javaonscroll('"+"javaonscroll"+"')\">" + bufferHtml + "</body></html>", "text/html", "utf-8", null);
				 canBack=false;
				 
				 return false;
			 }
			exitDialog();
			return false;
		}
		return false;
	}
	
	/**
	 * 
	 * @param z
	 * @return index文件中第z+1个单词词目，z从0到NUM-1，NUM为词条数
	 */
	public static String getinxword(byte[] bufferIndex,int[] iArrinxOffset,int[] iArrinxLen,int z){
		byte[] buffer = new byte[iArrinxLen[z]];
		for(int h=0;h<iArrinxLen[z];h++){
			buffer[h]=bufferIndex[iArrinxOffset[z]-3+h];
		}
		return EncodingUtils.getString(buffer, "UTF-8");
	}
	
	/**
	 * 
	 * @param z
	 * @return index文件中第z+1个单词词目，z从0到NUM-1，NUM为词条数
	 */
	public String getword(int z){
		int iOffset;
		int iLen;
		
		//File sdcDir = Environment.getExternalStorageDirectory();
		File findex = new File(sdcDir,"/pceddata/index");
		File finxoff = new File(sdcDir,"/pceddata/inxioff");
		File finxlen = new File(sdcDir,"/pceddata/inxilen");
		
		try{
			RandomAccessFile rindex = new RandomAccessFile(findex,"r");
			RandomAccessFile rfinxoff = new RandomAccessFile(finxoff,"r");
			RandomAccessFile rfinxlen = new RandomAccessFile(finxlen,"r");
			
			rfinxoff.seek(z*4);
			iOffset=rfinxoff.readInt();
			
			rfinxlen.seek(z*4);
			iLen=rfinxlen.readInt();
			
			//byte[] buffer = new byte[iArrinxLen[z]];
			//rindex.seek(iArrinxOffset[z]);
			byte[] buffer = new byte[iLen];
			rindex.seek(iOffset);
			rindex.read(buffer);
			
			rindex.close();
			rfinxoff.close();
			rfinxlen.close();
			
			return EncodingUtils.getString(buffer, "UTF-8");
		}catch (FileNotFoundException e){
			e.printStackTrace();
		}catch(IOException e){
			e.printStackTrace();
		}
		
		return "";
	}
	
	/**
	 * 
	 * @param z
	 * @return cidian文件中第z + 1个词条释义的语言类别：汉C 英E 缅M 越V
	 */
	public char getlanguage(int z)
	{
		switch(getdicsymbol(z))
		{
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
        	return  'V';
        case 'E':
        	return  'V';

        //其它类别
        default:
            return  '0';
		}
	}
	
	/**
	 * 
	 * @param z
	 * @return cidian文件中第z + 1个词条的第一个字符、即词典标识：一个大写英文字母，出错则返回'0'
	 */
	public char getdicsymbol(int z){
		int iOffset;
		char a = '0';

		//File sdcDir = Environment.getExternalStorageDirectory();
		File file = new File(sdcDir,"/pceddata/cidian");
		File foffset = new File(sdcDir,"/pceddata/itemioff");
		try{
			RandomAccessFile rf = new RandomAccessFile(file,"r");
			RandomAccessFile rfo = new RandomAccessFile(foffset,"r");
			
			rfo.seek(z*4);
			iOffset=rfo.readInt();
			
			rf.seek(iOffset);
			a = (char)(rf.read());
			
			rf.close();
			rfo.close();
		}catch (FileNotFoundException e){
			e.printStackTrace();
		}catch(IOException e){
			e.printStackTrace();
		}
		
		return a;
	}

	/**
	 * 
	 * @param z
	 * @return cidian文件中第z+1个词条解释，z从0到NUM-1，NUM为词条数
	 */
	public String getexplain(int z){
		int iOffset;
		int iLen;

		//File sdcDir = Environment.getExternalStorageDirectory();
		File file = new File(sdcDir,"/pceddata/cidian");
		File foffset = new File(sdcDir,"/pceddata/itemioff");
		File flength = new File(sdcDir,"/pceddata/itemilen");
		try{
			RandomAccessFile rf = new RandomAccessFile(file,"r");
			RandomAccessFile rfo = new RandomAccessFile(foffset,"r");
			RandomAccessFile rfl = new RandomAccessFile(flength,"r");
			
			rfo.seek(z*4);
			iOffset=rfo.readInt();
			
			rfl.seek(z*4);
			iLen=rfl.readInt();
			
			byte[] buffer = new byte[iLen];
			rf.seek(iOffset);
			rf.read(buffer);
			
			rf.close();
			rfo.close();
			rfl.close();
			
			return EncodingUtils.getString(buffer, "UTF-8");
		}catch (FileNotFoundException e){
			e.printStackTrace();
		}catch(IOException e){
			e.printStackTrace();
		}
		
		return "";
	}
	
	/**
	 * 当在输入框里输入字母时，随着输入，自动在webview里面列出符合的单词列表，先从列表里面删除重复的词再输出，
	 * 每次最多输出100词，随着列表的被触摸或滚动，再继续追加输出余下的词
	 */
	private void listDc()
	{
		//缺省与页面底色同色、灰色显示
		String CN = "<a style=\"color:white\">•</a>";
		String EN = "<a style=\"color:white\">•</a>";
		String MY = "<a style=\"color:white\">•</a>";
		String VT = "<a style=\"color:white\">•</a>";
		
		if ((listFirstNo < 0)|(listEndNo < 0)){
			webView.loadDataWithBaseURL("about:blank", "<html><head><style type=\"text/css\">body{background-color: white}</style></head><body></body></html>", "text/html", "utf-8", null);
			return;
		}
		if ((listFirstNo >= NUM)|(listEndNo >= NUM)){
			webView.loadDataWithBaseURL("about:blank", "<html><head><style type=\"text/css\">body{background-color: white}</style></head><body></body></html>", "text/html", "utf-8", null);
			return;
		}
		if (listFirstNo > listEndNo){
			webView.loadDataWithBaseURL("about:blank", "<html><head><style type=\"text/css\">body{background-color: white}</style></head><body></body></html>", "text/html", "utf-8", null);
			return;
		}
		
		int n=0;

		String preWord="";
		StringBuilder b = new StringBuilder("<br>");
		for (int z = listFirstNo; z <= listEndNo; z++)
		{
			if(n==101)
			{
				b.append("<div style=\"height:12px\">符合的词较多，现只列出前100条。</div><br>");
				webView.loadDataWithBaseURL("about:blank", "<html><head><style type=\"text/css\">@font-face{font-family:\"Zawgyi-One\";src:url(\"file:///android_asset/fonts/ZawgyiOne2008.ttf\");}body{font-family:\"Zawgyi-One\"; background-color: white;}pre{font-family:\"Zawgyi-One\";}</style></head><body onscroll = \"window.lookup.javaonscroll('"+"javaonscroll"+"')\">" + b.toString() + "</body></html>", "text/html", "utf-8", null);
				
		        bufferHtml=b.toString();
				return;
			}
			
			if(!(ABCtoabc(preWord).equals(ABCtoabc(getword(z)))))
			{
				n++;
				
				if(z > listFirstNo)
				{
					b.append("<div style=\"height:18px; white-space:nowrap\" onmouseover=\"this.style.backgroundColor='gray'\" onmouseout=\"this.style.color='black'\" onclick = \"window.lookup.javaonclick('"+preWord+"')\">");
					b.append(CN);
					b.append(EN);
					b.append(MY);
					b.append(VT);
					b.append(preWord);
					b.append("</div><br>");
				}
				preWord=getword(z);
				CN = "<a style=\"color:white\">•</a>";
				EN = "<a style=\"color:white\">•</a>";
				MY = "<a style=\"color:white\">•</a>";
				VT = "<a style=\"color:white\">•</a>";
			}
			
			switch(getlanguage(z))
			{
            case 'C':
                CN="<a style=\"color:red\">•</a>";
                break;
                
            case 'E':
                EN="<a style=\"color:green\">•</a>";
                break;
                
            case 'M':
                MY="<a style=\"color:saddlebrown\">•</a>";
                break;
                
            case 'V':
                VT="<a style=\"color:midnightblue\">•</a>";
            	break;
                
            default:
                ;
			}
		}
		
		b.append("<div style=\"height:18px; white-space:nowrap\" onmouseover=\"this.style.backgroundColor='gray'\" onmouseout=\"this.style.color='black'\" onclick = \"window.lookup.javaonclick('"+preWord+"')\">");
		b.append(CN);
		b.append(EN);
		b.append(MY);
		b.append(VT);
		b.append(preWord);
		b.append("</div><br>");

        webView.loadDataWithBaseURL("about:blank", "<html><head><style type=\"text/css\">@font-face{font-family:\"Zawgyi-One\";src:url(\"file:///android_asset/fonts/ZawgyiOne2008.ttf\");}body{font-family:\"Zawgyi-One\"; background-color: white;}pre{font-family:\"Zawgyi-One\";}</style></head><body onscroll = \"window.lookup.javaonscroll('"+"javaonscroll"+"')\">" + b.toString() + "</body></html>", "text/html", "utf-8", null);
        
        bufferHtml=b.toString();
        
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
	int	listFirstNo = -1;
	int	listEndNo = -1;
	
    private boolean palihan_cc(String nword, String pword)
    {
        //strPaliWord = pword;

        int n = 0;  //记录查找结果条数
        int sNo = -1, lNo = -1;
        int ibz = -3;

        if (!bwordaheadmatch)
        {
            ibz = edcNo(nword);
            if (ibz == 0)
            {
                sNo = iNo;
                while ((sNo > 0) && !eab(getword(sNo - 1), nword))
                    sNo = sNo - 1;
                lNo = sNo;
                while ((lNo < NUM - 1) && !eab(nword, getword(lNo + 1)))
                    lNo = lNo + 1;
            }
            else
            {
                //if (ziweiCc(pword))
                //    return true;
                //else
                //    return false;
            	return false;
            }
        }
        else
        {
            ibz = edcNo(nword);
            if (ibz == -2)
            {
                //if (ziweiCc(pword))
                //    return true;
                //else
                //    return false;
            }
            if (ibz == 0)
            {
                sNo = iNo;
                while ((sNo > 0) && !eab(getword(sNo - 1), nword))
                    sNo = sNo - 1;
            }
            if (ibz == 1)
            {
                sNo = iNo + 1;
            }
            if (ibz == -1)
            {
                sNo = 0;
            }

            ibz = edcNo(PadRight(nword, 'z', 68));
            if (ibz == -1)
            {
                //if (ziweiCc(pword))
                //    return true;
                //else
                //    return false;
            }
            if (ibz == 0)
            {
                lNo = iNo;
                while ((lNo < NUM - 1) && !eab(PadRight(nword, 'z', 68), getword(lNo + 1)))
                    lNo = lNo + 1;
            }
            if (ibz == 1)
            {
                lNo = iNo;
            }
            if (ibz == -2)
            {
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
	public int edcNo(String d)
    {
        iNo = -1;
        int itmp = 0, min = 0, max = NUM - 1;
        if (eab(d, getword(min)))
            return -1;
        if (eab(getword(max), d))
            return -2;
        do
        {
            itmp = (min + max) / 2;
            if (eab(d, getword(itmp)))
            {
                max = itmp;
            }
            else
            {
                min = itmp;
            }
        } while (max - min > 1);
        if (!eab(d, getword(min)) & !eab(getword(min), d))
        {
            iNo = min;
            return 0;
        }
        else if (!eab(d, getword(max)) & !eab(getword(max), d))
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
	
	/**
	 * 比较英文单词
	 * @param a
	 * @param b
	 * @return 如果a小于b，则返回true
	 */
    public boolean eab(String a, String b)
    {
        int i;
        if (a.length() > b.length())
        {
            i = a.length();
            b = PadRight(b, ' ', i);
        }
        else if (a.length() < b.length())
        {
            i = b.length();
            a = PadRight(a, ' ', i);
        }
        else
            i = a.length();

        for (int j = 0; j < i; j++)
        {
            if (eABC(a.substring(j, j+1).charAt(0)) < eABC(b.substring(j, j+1).charAt(0)))
                return true;
            else if (eABC(a.substring(j, j+1).charAt(0)) > eABC(b.substring(j, j+1).charAt(0)))
                return false;
        }
        return false;
    }
    
    /**
     * 补足字符串长度
     * @param s 需补足的字符串
     * @param a char字符
     * @param length 补足后总长度
     * @return 已补足长度的字符串
     */
    public static String PadRight(String s, char a, int length)
    {
    	StringBuilder b = new StringBuilder(s);
    	while(b.length()<length){
    		b.append(a);
    	}
    	return b.toString();
    }
	
    //英文化模式
    public int eABC(char inabc)
    {
        switch (inabc)
        {
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
     * @param s
     * @return
     */
    public String ABCtoabc(String s)
    {
    	StringBuilder b = new StringBuilder("");
    	for (int i=0; i<s.length(); i++)
    	{
    		b.append(Atoa(s.charAt(i)));
    	}
    	return b.toString();
    }
    
    /**
     * 字母大写转小写
     * @param a
     * @return
     */
    public char Atoa(char a)
    {
        switch (a)
        {
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
