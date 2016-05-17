package com.fundoong.unitytest.testlayout;

import android.app.Service;
import android.content.Intent;
import android.os.Handler;
import android.os.IBinder;
import android.util.Log;

public class MyService extends Service {

    private final Handler handler = new Handler();


    private Runnable runnable = new Runnable() {
        @Override
        public void run() {
            MyApplication myApp = (MyApplication)getApplicationContext();
            Intent sendIntent = new Intent();
            sendIntent.setFlags(Intent.FLAG_ACTIVITY_NO_ANIMATION|Intent.FLAG_FROM_BACKGROUND|Intent.FLAG_INCLUDE_STOPPED_PACKAGES  );
            sendIntent.setAction("android.intent.action.SUPERSK");
            sendIntent.putExtra("Data", myApp.getData());
            sendBroadcast(sendIntent);
            Log.i("Send : ", myApp.getData());
            handler.removeCallbacks(this);
            handler.postDelayed(this,50);
        }
    };

    @Override
 public void onStart(Intent intent, int startid) {
        handler.removeCallbacks(runnable);
        handler.postDelayed(runnable,1000);
    }

    @Override
    public IBinder onBind(Intent intent) {
        return null;
    }
}
