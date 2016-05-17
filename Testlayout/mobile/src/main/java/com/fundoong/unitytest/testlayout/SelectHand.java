package com.fundoong.unitytest.testlayout;

import android.app.Activity;
import android.content.Intent;
import android.graphics.Color;
import android.os.Bundle;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

public class SelectHand extends Activity {

    Button LHand;
    Button RHand;
    boolean selectStats;
    Button nextButton;
    String nickName;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_selecthand);
        selectStats = false;//false = left, true = right

        Intent intent = getIntent();
        nickName = intent.getStringExtra("nickName");

        setOnClickListenrs();

    }
    public void setOnClickListenrs(){
        LHand = (Button)findViewById(R.id.lhandButton);
        RHand = (Button)findViewById(R.id.rhandButton);
        nextButton = (Button)findViewById(R.id.okButton2);

        LHand.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if(selectStats==true)
                {
                    LHand.setBackgroundResource(R.drawable.lhand_selected);
                    RHand.setBackgroundResource(R.drawable.rhand);
                    selectStats = false;
                }
            }
        });
        RHand.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if(selectStats==false)
                {
                    LHand.setBackgroundResource(R.drawable.lhand);
                    RHand.setBackgroundResource(R.drawable.rhand_selected);
                    selectStats = true ;
                }
            }
        });
        nextButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                //go to list intent
                Intent i = new Intent(SelectHand.this,SelectMenu.class);
                i.putExtra("nickName",nickName);
                i.putExtra("hand",selectStats);
                startActivity(i);
            }
        });
    }

}
