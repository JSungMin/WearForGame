package com.fundoong.unitytest.testlayout;

import android.app.Application;

/**
 * Created by 032 on 2016-01-25.
 */
public class MyApplication extends Application {
    private String data;

    @Override
    public void onCreate(){
        super.onCreate();
        data = "";
    }
    @Override
    public void onTerminate(){
        super.onTerminate();
    }
    public void setData(String da){
        data = da;
    }
    public String getData(){
        return data;
    }
}
