package com.peiyh.PaliTipitakaDict;

import android.app.Application;

public class MyApplication extends Application {

    private static  MyApplication myApplication;
    public void onCreate(){
        super.onCreate();

        CrashHandle crash = CrashHandle.getInstance();
        crash.init(getApplicationContext());
    }

    public static MyApplication getMyApplication(){
        if(myApplication == null){
            myApplication = new MyApplication();
        }
        return myApplication;
    }

}
