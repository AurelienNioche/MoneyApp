package humanoid.vs.android.com.moneyapp;

import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.view.View;
import android.view.WindowManager;
import android.widget.ImageView;
import android.widget.TextView;

import java.util.HashMap;
import java.util.Map;


public class EndActivity extends AppCompatActivity {


    public String idx;
    public String reward_amount;

    public TextView reward_amount_view;
    public ImageView consumption_object;

    public Map<String, Integer> map_objects;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        setContentView(R.layout.activity_end);

        getWindow().addFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);

        Bundle extras = getIntent().getExtras();
        if (extras != null) {
            idx = extras.getString("IDX");
            reward_amount = extras.getString("REWARD_AMOUNT");
        }


        reward_amount_view = (TextView) findViewById(R.id.reward_amount);
        consumption_object = (ImageView) findViewById(R.id.consumption_object);

        mapObjects();
        showFinalResult();

    }

    public void mapObjects() {

        map_objects = new HashMap<>();
        map_objects.put("0", R.drawable.apple);
        map_objects.put("1", R.drawable.chocolate);
        map_objects.put("2", R.drawable.soap);
    }

    public void showFinalResult() {

        reward_amount_view.setText(reward_amount);
        consumption_object.setImageResource(map_objects.get("1"));
        findViewById(R.id.final_screen).setVisibility(View.VISIBLE);

    }

    public void myClickHandler(View view) {

        finishAffinity();
    }
}