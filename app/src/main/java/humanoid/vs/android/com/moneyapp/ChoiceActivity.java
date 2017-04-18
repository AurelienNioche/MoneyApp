package humanoid.vs.android.com.moneyapp;
import android.content.Intent;
import android.graphics.Color;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.view.View;
import android.view.WindowManager;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.TextView;

import java.util.HashMap;
import java.util.Map;
import java.util.Objects;

/**
 * An example full-screen activity that shows and hides the system UI (i.e.
 * status bar and navigation/system bar) with user interaction.
 */
public class ChoiceActivity extends AppCompatActivity {

    public String idx;
    public String object_in_hand;
    public String reward_amount;


    public TextView reward_amount_view;
    public ImageView object_in_hand_view;

    public ImageButton button_choice_a;
    public ImageButton button_choice_b;

    public View loading_panel;

    public Map<String, Integer> map_objects;
    public Map<String, Integer> map_choices;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        getWindow().setEnterTransition(null);

        overridePendingTransition(0, 0);

        hideSystemUI();
        getWindow().getDecorView().setBackgroundColor(Color.WHITE);
        getWindow().addFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);

    }

    public void onResume() {

        super.onResume();

        setContentView(R.layout.activity_choice);

        // Get args from previous activity
        Bundle extras = getIntent().getExtras();
        if (extras != null) {
            idx = extras.getString("IDX");
            object_in_hand = extras.getString("OBJECT_IN_HAND");
            reward_amount = extras.getString("REWARD_AMOUNT");
        }

        // Get graphic elements of actual activity
        reward_amount_view = (TextView) findViewById(R.id.reward_amount);
        object_in_hand_view = (ImageView) findViewById(R.id.object_in_hand);
        button_choice_a = (ImageButton) findViewById(R.id.button_choiceA);
        button_choice_b = (ImageButton) findViewById(R.id.button_choiceB);
        loading_panel = findViewById(R.id.loadingPanel);

        // Print elements for debug
        System.out.println("[ChoiceActivity] Id is " + idx);
        System.out.println("[ChoiceActivity] Object in hand is " + object_in_hand);

        // Prepare objects
        mapObjects();
        mapChoices();

        // Set views
        setRewardAmount();
        setObjectInHand();
        setChoices();

    }

    @Override
    public void onBackPressed() {
        //
    }

    public void mapObjects() {

        map_objects = new HashMap<>();
        map_objects.put("0", R.drawable.apple);
        map_objects.put("1", R.drawable.chocolate);
        map_objects.put("2", R.drawable.soap);
    }

    public void mapChoices() {

        map_choices = new HashMap<>();
        map_choices.put("01", R.drawable.apple_chocolate);
        map_choices.put("02", R.drawable.apple_soap);
        map_choices.put("21", R.drawable.soap_chocolate);
        map_choices.put("20", R.drawable.soap_apple);

    }

    public void setRewardAmount() {

        reward_amount_view.setText(reward_amount);

    }

    public void setObjectInHand() {

        object_in_hand_view.setImageResource(map_objects.get(object_in_hand));
    }

    public void setChoices() {

        if (Objects.equals(object_in_hand, "0")) {

            button_choice_a.setBackgroundResource(map_choices.get("01"));
            button_choice_b.setBackgroundResource(map_choices.get("02"));

        } else {

            button_choice_a.setBackgroundResource(map_choices.get("21"));
            button_choice_b.setBackgroundResource(map_choices.get("20"));

        }

    }

    public void myClickHandlerChoiceA(View view){

        handleChoice(0);
    }

    public void myClickHandlerChoiceB(View view){

        handleChoice(1);

    }

    public void handleChoice(int choice){

        showLoadingPanel();

        button_choice_b.setEnabled(false);
        button_choice_a.setEnabled(false);

        System.out.println("[ChoiceActivity] Submitting choice...");

        submitChoiceToServer(choice);

    }

    public void showLoadingPanel() {

        findViewById(R.id.choice).setVisibility(View.INVISIBLE);

        loading_panel.setVisibility(View.VISIBLE);


    }

    public void submitChoiceToServer(int choice) {

        addressDemandToServer(String.format("/set_choice/%s/%s", idx, choice));

    }

    public void handleChoiceSubmitted(String server_msg){

        System.out.println("[ChoiceActivity] " + server_msg);
        startNewActivity();
    }

    public void startNewActivity(){

        // For debug
        System.out.println("[ChoiceActivity] Start ResultsActivity");

        Intent intent = new Intent(getApplicationContext(), ResultsActivity.class);
        intent.putExtra("IDX", idx);
        intent.putExtra("REWARD_AMOUNT", reward_amount);

        startActivity(intent);
    }

    public void addressDemandToServer(String demand) {

        Client client = new Client(this);
        client.ask_server(demand);

    }

    public void handleServerResponse(String server_msg) {

        handleChoiceSubmitted(server_msg);

    }

    private void hideSystemUI() {
        // Set the IMMERSIVE flag.
        // Set the content to appear under the system bars so that the content
        // doesn't resize when the system bars hide and show.
        View decorView = getWindow().getDecorView();
        decorView.setSystemUiVisibility(
                View.SYSTEM_UI_FLAG_IMMERSIVE_STICKY
                        | View.SYSTEM_UI_FLAG_FULLSCREEN // hide status bar
                        | View.SYSTEM_UI_FLAG_HIDE_NAVIGATION // hide nav bar
        );
    }
}


