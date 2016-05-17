package com.fundoong.unitytest.testlayout;

import android.app.Activity;
import android.content.Intent;
import android.graphics.Color;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

public class NickNameInput extends Activity {

    String nickName;
    EditText editText;
    Button okButton;
    Boolean editBool = false;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_nickname);
        editText = (EditText)findViewById(R.id.EditNickName);
        okButton = (Button)findViewById(R.id.okButton);

        editText.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {

            }

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {

            }

            @Override
            public void afterTextChanged(Editable s) {
                if(s.toString().length()>0&&s.toString().charAt(0)!=' ')
                {
                    okButton.setBackgroundColor(Color.rgb(45,221,0));
                    nickName = s.toString();
                    editBool = true;
                }
                else if (s.toString().length()==0)
                {
                    okButton.setBackgroundColor(Color.rgb(255,0,140));
                    editBool = false;
                }
                else if(s.toString().charAt(0)==' ')
                {
                    okButton.setBackgroundColor(Color.rgb(255,0,140));
                    Toast.makeText(getApplicationContext(), "공백을 입력하시면 안됩니다.", Toast.LENGTH_SHORT);
                    editBool = false;
                }
            }
        });

        okButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                if(editBool == true)
                {
                    Intent i = new Intent(NickNameInput.this,SelectHand.class);
                    i.putExtra("nickName",nickName);
                    startActivity(i);
                }

            }
        });
    }
}
