package com.fundoong.unitytest.testlayout;

import android.content.Intent;
import android.util.Log;

import com.google.android.gms.wearable.MessageEvent;
import com.google.android.gms.wearable.WearableListenerService;

/**
 * Created by Wasim on 08-05-2015.
 */
public class DataLayerListenrService extends WearableListenerService {

    public static String SERVICE_CALLED_WEAR = "WearListClicked";

    @Override
    public void onMessageReceived(MessageEvent messageEvent) {
        super.onMessageReceived(messageEvent);
        MyApplication myApp = (MyApplication)getApplicationContext();

        String event = messageEvent.getPath();
        myApp.setData(event);
        Log.d("Listclicked", event);

        String [] message = event.split(",");
        Log.d("Messages : ", event);
        //if (message[0].equals(SERVICE_CALLED_WEAR)) {

          //  SelectMenu.getInstance().receiveData = event.toString();
            //startActivity(new Intent((Intent) Listactivity.getInstance().tutorials.get(message[1]))
              //      .addFlags(Intent.FLAG_ACTIVITY_NEW_TASK));
       // }
    }
}
