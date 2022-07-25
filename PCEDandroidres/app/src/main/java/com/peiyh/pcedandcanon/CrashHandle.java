package com.peiyh.pcedandcanon;

import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.os.Handler;
import android.os.Looper;
import android.os.Process;
import android.util.Log;
import android.view.Gravity;
import android.widget.Toast;

import java.util.Timer;
import java.util.TimerTask;

public class CrashHandle implements Thread.UncaughtExceptionHandler {

    private static CrashHandle Instance = new CrashHandle();
    private Context context;
    private Thread.UncaughtExceptionHandler DefeaultHandler;

    private CrashHandle(){}
    //单例模式
    public static CrashHandle getInstance(){
        return Instance;
    }

    public void init(Context context){
        this.context = context;
        DefeaultHandler = Thread.getDefaultUncaughtExceptionHandler();
        Thread.setDefaultUncaughtExceptionHandler(this);
    }

    @Override
    public void uncaughtException(Thread thread, Throwable ex) {
        // TODO Auto-generated method stub
        if((!HandlerException(ex)) && (this.DefeaultHandler != null)){
            this.DefeaultHandler.uncaughtException(thread, ex);
        }else{
            try {
                Thread.sleep(50000);
            } catch (InterruptedException e) {
                // TODO Auto-generated catch block
                e.printStackTrace();
            }
            Process.killProcess(Process.myPid());
            System.exit(1);
        }

    }

    //自定义错误处理，收集错误信息与发送错误信息
    public boolean HandlerException(final Throwable ex){
        if(ex == null){
            return false;
        }
        new Thread(){
            public void run(){
                Looper.prepare();
                /*//
                AlertDialog.Builder builder = new AlertDialog.Builder(context);
                builder.setMessage("数据库可能损坏了，请重装app！"+ex.getMessage());
                builder.setTitle("提示：");
                builder.setPositiveButton("确定",
                        new android.content.DialogInterface.OnClickListener() {
                            @Override
                            public void onClick(DialogInterface dialog, int which) {
                                dialog.dismiss();
                            }
                        });
                builder.create().show();
                //*/
                Log.e("111111","111111可能是数据库损坏："+ex.getMessage());
                /*//
                Toast toast =Toast.makeText(context, "数据库损坏："+ex.getMessage(), Toast.LENGTH_LONG);
                toast.setGravity(Gravity.CENTER,0,0);
                toast.show();
                //*/

                ///

                /*
                new Handler().postDelayed(new Runnable() {
                    @Override
                    public void run() {
                        toast.cancel();
                    }
                },50000);
                */
                for(int i=0; i<20; i++) {
                    Toast toast = Toast.makeText(context, "本pali app的数据库可能损坏了！请先卸载本应用然后再重装它，可能您的手机当前运行内存不足或者是存储空间不足，请先尝试清理手机的运行内存以及清理手机的存储空间后再重装它！：" + ex.getMessage(), Toast.LENGTH_LONG);
                    toast.setGravity(Gravity.CENTER, 0, 0);
                    toast.show();
                }

                Looper.loop();
            }
        }.start();
        return true;
    }

}
