package com.fundoong.unitytest.testlayout;

import android.app.Activity;
import android.content.Context;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.os.Bundle;
import android.support.wearable.view.WatchViewStub;
import android.support.wearable.view.WearableListView;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.ViewGroup;
import android.view.WindowManager;
import android.widget.TextView;
import android.widget.Toast;

import com.google.android.gms.common.ConnectionResult;
import com.google.android.gms.common.api.GoogleApiClient;
import com.google.android.gms.common.api.ResultCallback;
import com.google.android.gms.wearable.MessageApi;
import com.google.android.gms.wearable.Node;
import com.google.android.gms.wearable.NodeApi;
import com.google.android.gms.wearable.Wearable;

import java.util.ArrayList;

public class MainActivity extends Activity implements GoogleApiClient.ConnectionCallbacks, GoogleApiClient.OnConnectionFailedListener,WearableListView.ClickListener,SensorEventListener {
    private float waitTime = 0.02f;
    private float lastTime = 0;
    private WearableListView mListView;

    private ArrayList<String> listItems;

    Node mNode; // the connected device to send the message to
    GoogleApiClient mGoogleApiClient;
    private boolean mResolvingError=false;

    public static String SERVICE_CALLED_WEAR = "WearListClicked";
    public static String TAG = "WearListActivity";

    public float[] accelData = new float[3];
    public float[] magnaticData = new float[3];

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_list);
        getWindow().addFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);
        //Connect the GoogleApiClient
        mGoogleApiClient = new GoogleApiClient.Builder(this)
                .addApi(Wearable.API)
                .addConnectionCallbacks(this)
                .addOnConnectionFailedListener(this)
                .build();

        initializeListItems();
        final WatchViewStub stub = (WatchViewStub) findViewById(R.id.watch_view_stub);
        stub.setOnLayoutInflatedListener(new WatchViewStub.OnLayoutInflatedListener() {
            @Override
            public void onLayoutInflated(WatchViewStub stub) {
                mListView = (WearableListView) stub.findViewById(R.id.listView1);

                mListView.setAdapter(new MyAdapter(MainActivity.this, listItems));
                mListView.setClickListener(MainActivity.this);
                getSensor();
            }
        });
    }
    private void getSensor(){
        SensorManager mSensorManager = (SensorManager)getSystemService(SENSOR_SERVICE);
        Sensor mAccelSensor = mSensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER);
        Sensor mMagneticSensor = mSensorManager.getDefaultSensor(Sensor.TYPE_MAGNETIC_FIELD);

        mSensorManager.registerListener(this,mAccelSensor,50000);
        mSensorManager.registerListener(this,mMagneticSensor,50000);

    }
    private void initializeListItems() {

        listItems = new ArrayList<>();
        listItems.add("Async File Read");
        listItems.add("Battery Status");
        listItems.add("Volume Setting");
        listItems.add("Frame Animation");
        listItems.add("Video Player");
        listItems.add("Circular Image View");
        listItems.add("Track User Location");
        listItems.add("Take Image");
        listItems.add("Image Grid View");
        listItems.add("Image Switcher");
        listItems.add("Tabs with Toolbar");
        listItems.add("Icon Tabs with Toolbar");
        listItems.add("Push Notification");


    }

    @Override
    protected void onStart() {
        super.onStart();
        if (!mResolvingError) {
            mGoogleApiClient.connect();
        }
    }

    /**
     * Resolve the node = the connected device to send the message to
     */
    private void resolveNode() {

        Wearable.NodeApi.getConnectedNodes(mGoogleApiClient)
                .setResultCallback(new ResultCallback<NodeApi.GetConnectedNodesResult>() {
                    @Override
                    public void onResult(NodeApi.GetConnectedNodesResult nodes) {
                        for (Node node : nodes.getNodes()) {
                            mNode = node;
                        }
                    }
                });
    }

    @Override
    public void onConnected(Bundle bundle) {
        resolveNode();
    }

    @Override
    public void onConnectionSuspended(int i) {

    }

    @Override
    public void onConnectionFailed(ConnectionResult connectionResult) {

    }

    /**
     * Send message to mobile handheld
     */
    private void sendMessage(String Key) {

        if (mNode != null && mGoogleApiClient!= null && mGoogleApiClient.isConnected()) {
            Wearable.MessageApi.sendMessage(
                    mGoogleApiClient, mNode.getId(),Key, null).setResultCallback(

                    new ResultCallback<MessageApi.SendMessageResult>() {
                        @Override
                        public void onResult(MessageApi.SendMessageResult sendMessageResult) {

                            if (!sendMessageResult.getStatus().isSuccess()) {
                            }
                        }
                    }
            );
        }

    }

    @Override
    public void onClick(WearableListView.ViewHolder viewHolder) {
        TextView view = (TextView) viewHolder.itemView.findViewById(R.id.row_tv_name);
        String Key = view.getText().toString();

        sendMessage(Key);
    }

    @Override
    public void onTopEmptyRegionClick() {
        Toast.makeText(this, "You tapped on Top empty area", Toast.LENGTH_SHORT).show();
    }

    @Override
    public void onSensorChanged(SensorEvent event) {
        if(event.sensor.getType()==Sensor.TYPE_ACCELEROMETER){
        accelData = event.values;
        }
        if(event.sensor.getType()==Sensor.TYPE_MAGNETIC_FIELD){
        magnaticData = event.values;
        }
        if(accelData!=null&&magnaticData!=null){
            float R[] = new float[9];
            boolean success = SensorManager.getRotationMatrix(R,null,accelData,magnaticData);
            if(success){
                float orientation[] = new float[3];
                SensorManager.getOrientation(R,orientation);
                try{
                    String key = SERVICE_CALLED_WEAR+","+orientation[0]+","+orientation[1]+","+orientation[2];

                    sendMessage(key);
                }catch (Exception e){

                }
            }
        }
    }

    @Override
    public void onAccuracyChanged(Sensor sensor, int accuracy) {

    }


    private class MyAdapter extends WearableListView.Adapter {
        private final LayoutInflater mInflater;
        private ArrayList<String> data;

        private MyAdapter(Context context, ArrayList<String> listItems) {
            mInflater = LayoutInflater.from(context);
            data = listItems;
        }

        @Override
        public WearableListView.ViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
            return new WearableListView.ViewHolder(
                    mInflater.inflate(R.layout.row_wear_list, null));
        }

        @Override
        public void onBindViewHolder(WearableListView.ViewHolder holder, int position) {
            TextView view = (TextView) holder.itemView.findViewById(R.id.row_tv_name);
            view.setText(data.get(position));
            holder.itemView.setTag(position);
        }

        @Override
        public int getItemCount() {
            return data.size();
        }
    }
}
