package com.example.app1;
//webview使用与字体设置

import android.os.Bundle;
import android.app.Activity;
import android.view.Menu;
import android.content.res.AssetManager;
import android.graphics.Typeface;
import android.view.ViewGroup;
import android.widget.LinearLayout;
import android.widget.TextView;
import android.webkit.WebView;

import android.os.Environment;
import android.util.Log;
import java.io.File;
import java.io.InputStream;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.RandomAccessFile;
import java.io.FileNotFoundException;
import java.io.IOException;
import org.apache.http.util.EncodingUtils;



public class MainActivity extends Activity {
	
	final int WRAP_CONTENT = ViewGroup.LayoutParams.WRAP_CONTENT;
	private LinearLayout linearLayout;
	private LinearLayout.LayoutParams linearLayoutParams; 
	private TextView tv;
	/*
	public void constructTexeView(){
		tv = new TextView(this);
	}
	*/
	WebView webView;
	

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        
        setContentView(R.layout.activity_main);
        
        Log.e("123",String.valueOf(Environment.getExternalStorageDirectory()));
        Log.e("123",Environment.getExternalStorageDirectory().getPath());
   		Log.e("123",Environment.MEDIA_MOUNTED);
   		
   		long startMili=System.currentTimeMillis();
	
        //linearLayout=new LinearLayout(this);
        //linearLayout.setOrientation(LinearLayout.VERTICAL);
        //setContentView(linearLayout);
        //linearLayoutParams=new LinearLayout.LayoutParams(WRAP_CONTENT,WRAP_CONTENT);
        
        //constructTexeView();
        //AssetManager mgr=getAssets();
        //Typeface tf=Typeface.createFromAsset(mgr, "fonts/ZawgyiOne2008.ttf");
        //tv.setTypeface(tf);
        
        //tv.setText("128āīūṅñṭḍṇḷṃṁŋ这是汉字这是日文：無常を常なりとする顛倒这是缅文：အနိစၥ(တိ)这是越文：không bền vững, vô thường。");
        
        //linearLayout.addView(tv,linearLayoutParams);
        
		//RandomAccessFile的使用，读入：
		File sdcDir = Environment.getExternalStorageDirectory();
		
		File file = new File(sdcDir,"/pceddata/词库1/cidian");
		
		File foffset = new File(sdcDir,"/pceddata/词库1/itemioff");
		File flength = new File(sdcDir,"/pceddata/词库1/itemilen");
		
		if (foffset.exists()) foffset.delete();
		if (flength.exists()) flength.delete();
		
		try{
			RandomAccessFile rf = new RandomAccessFile(file,"r");
			
			RandomAccessFile wfo = new RandomAccessFile(foffset,"rw");
			RandomAccessFile wfl = new RandomAccessFile(flength,"rw");

			//byte[] buffer = new byte[(int)(rf.length())];

			byte[] buffer;
			String str="";

			int j=0;
			int iTemp=0;
			int iOff=3;
			int iLen=0;
			rf.seek(iOff);
			while(iTemp!=-1){
				iTemp=rf.read();
				iLen++;
				if(iTemp==13){
					iTemp=rf.read();
					iLen++;
					if(iTemp==10){
						j++;
						//if (j>100) break;
						
						wfo.writeInt(iOff);
						wfl.writeInt(iLen-2);
						
						rf.seek(iOff);
						
						buffer = new byte[iLen-2];
						rf.read(buffer);
						str = EncodingUtils.getString(buffer, "UTF-8");
						//if (j%1000==0)
						Log.e(String.valueOf(j)+"#"+String.valueOf(str.length()),str.toString());
						iOff=iOff+iLen;
						rf.seek(iOff);
						iLen=0;
						}
					}
			}
			
			wfo.close();
			wfl.close();
			
			rf.close();
			
			long endMili=System.currentTimeMillis();
			
			//String dcstr[]=str.split("\r\n");
			//for(int i=0;i<dcstr.length;i++){
			//	Log.e(String.valueOf(i),dcstr[i]);
			//}
			
			//byte[] dcbytes=dcstr[5].getBytes("UTF-8");
			//Log.e("dcbytes",String.valueOf(dcbytes.length));
			
	        webView = new WebView(this);
	        webView.loadDataWithBaseURL("about:blank", "<html><head><style type=\"text/css\">@font-face{font-family:\"Zawgyi-One\";src:url(\"file:///android_asset/fonts/ZawgyiOne2008.ttf\");}body{font-family:\"Zawgyi-One\";color:red;}pre{font-family:\"Zawgyi-One\";color:green;}</style></head><pre>" +(endMili-startMili)/1000.0+"秒."+ str + "</pre>128āīūṅñṭḍṇḷṃṁŋ这是汉字这是日文：無常を常なりとする顛倒这是缅文：အနိစၥ(တိ)这是越文：không bền vững, vô thường。</html>", "text/html", "utf-8", null);
	        setContentView(webView);
			
		}catch (FileNotFoundException e){
			e.printStackTrace();
		}catch(IOException e){
			e.printStackTrace();
		}
        


    }


    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.main, menu);
        return true;
    }
    
    
}
