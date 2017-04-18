package humanoid.vs.android.com.moneyapp;

import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.provider.Settings.Secure;
import android.support.v7.app.AppCompatActivity;
import android.view.View;
import android.widget.ImageButton;


public class MainActivity extends AppCompatActivity {

    public String idx;
    public String object_in_hand;
    public String reward_amount;

    public static int sending_delta = 200;

    public View welcome;
    public View schema;

    public ImageButton start_button;

    @Override
    protected void onCreate(Bundle savedInstanceState) {

        super.onCreate(savedInstanceState);
        hideSystemUI();

        setContentView(R.layout.activity_main);

        welcome = findViewById(R.id.welcome);
        start_button = (ImageButton) findViewById(R.id.start_button);
        schema = findViewById(R.id.schema);

        final Handler handler = new Handler();
        handler.postDelayed(new Runnable() {
            public void run() {
                // Actions to do after x ms
                getInit();
            }
        }, sending_delta);
    }

    public void myClickHandler(View view) {

        View loading_panel = findViewById(R.id.loadingPanel);
        loading_panel.setVisibility(View.VISIBLE);

        startNewActivity();

    }
    
    public void startNewActivity(){

        Intent intent = new Intent(this, ChoiceActivity.class);
        intent.putExtra("IDX", idx);
        intent.putExtra("OBJECT_IN_HAND", object_in_hand);
        intent.putExtra("REWARD_AMOUNT", reward_amount);
        startActivity(intent);
    }

    private void getInit() {

        String android_id = Secure.getString(this.getContentResolver(), Secure.ANDROID_ID);
        addressDemandToServer(String.format("get_init/%s", android_id));

    }

    public void addressDemandToServer(String demand) {

        Client client = new Client(this);
        client.ask_server(demand);

    }

    public void handleServerResponse(String server_msg) {

        String[] parts = server_msg.split("/");

        if (parts.length > 1) {

            this.idx = parts[0];
            this.object_in_hand = parts[1];
            this.reward_amount = parts[2];

            System.out.println(
                    String.format(
                            "[MainActivity] Idx is %s, " +
                                    "object in hand is %s, reward amount is %s.",
                            this.idx, this.object_in_hand, this.reward_amount)
            );

            welcome.setVisibility(View.INVISIBLE);
            schema.setVisibility(View.VISIBLE);


        } else {

            getInit();

        }

    }

    @Override
    public void onBackPressed() {
        // Do nothing.
    }

    private void hideSystemUI() {
        
        // Hide navigation bar
        View decorView = getWindow().getDecorView();
        decorView.setSystemUiVisibility(
                View.SYSTEM_UI_FLAG_IMMERSIVE_STICKY
                        | View.SYSTEM_UI_FLAG_FULLSCREEN // hide status bar
                        | View.SYSTEM_UI_FLAG_HIDE_NAVIGATION // hide nav bar
                );
    }
}