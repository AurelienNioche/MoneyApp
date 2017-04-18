package humanoid.vs.android.com.moneyapp;

import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.support.v7.app.AppCompatActivity;
import android.view.View;
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
public class ResultsActivity extends AppCompatActivity {

    public String idx;
    public String success;
    public String reward_amount;
    public String object_in_hand;
    public String continue_game;
    public String future_object_in_hand;

    public static int ask_again_delta = 500;

    public ImageView smiley;
    public TextView reward_amount_view;
    public ImageView object_in_hand_view;

    public ImageButton button_next_round;

    public View frame;
    public View counter;
    public View smiley_and_object;

    public Map<String, Integer> map_objects;
    public Map<String, Integer> map_smiley;

    Client client;

    @Override
    protected void onCreate(Bundle savedInstanceState) {

        super.onCreate(savedInstanceState);

        hideSystemUI();

        setContentView(R.layout.activity_results);

        // Get args from previous activity
        Bundle extras = getIntent().getExtras();
        if (extras != null) {
            idx = extras.getString("IDX");
            reward_amount = extras.getString("REWARD_AMOUNT");
        }

        // Find views
        // main_msg = (TextView) findViewById(R.id.main_msg);
        frame = findViewById(R.id.activity_results);
        smiley = (ImageView) findViewById(R.id.smiley);
        button_next_round = (ImageButton) findViewById(R.id.button_next_round);
        counter = findViewById(R.id.counter);
        smiley_and_object = findViewById(R.id.smiley_and_object);
        reward_amount_view = (TextView) findViewById(R.id.reward_amount);
        object_in_hand_view = (ImageView) findViewById(R.id.result_object_in_hand);

        // Set reward amount
        setRewardAmount();

        // Create client for using server
        client = new Client(this);

        mapObjects();
        mapSmiley();
        askForResults();

        frame.setOnTouchListener(new OnSwipeTouchListener(this) {
            @Override
            public void onSwipeLeft() {
                if (Objects.equals(continue_game, "1")) {

                    startNewActivity();
                }
                else {
                    goToEnd();
                }

            }
        });
    }

    public void mapObjects() {

        map_objects = new HashMap<>();
        map_objects.put("0", R.drawable.apple);
        map_objects.put("1", R.drawable.chocolate);
        map_objects.put("2", R.drawable.soap);
    }

    public void mapSmiley() {

        map_smiley = new HashMap<>();
        map_smiley.put("0", R.drawable.smiley_sad);
        map_smiley.put("1", R.drawable.smiley_happy);

    }

    public void askForResults() {

        addressDemandToServer("get_result/" + idx);

    }

    public void addressDemandToServer(String demand) {

        client.ask_server(demand);

    }

    public void handleServerResponse(String server_msg) {
        if (Objects.equals(server_msg, "Try again later...")) {

            final Handler handler = new Handler();
            handler.postDelayed(new Runnable() {
                public void run() {
                    // Actions to do after x ms
                    System.out.println("Wait a little for re-asking results to server.");

                    askForResults();
                }
            }, ask_again_delta);
        }
        else {
            showResults(server_msg);
        }
    }

    public void showResults(String server_msg) {

        String[] parts = server_msg.split("/");

        if (parts.length == 5) {

            continue_game = parts[0];
            future_object_in_hand = parts[1];
            object_in_hand = parts[2];
            success = parts[3];
            reward_amount = parts[4];

            // Print for debug
            System.out.println(
                    String.format("[ResultsActivity] Success: %s", success));
            System.out.println(
                    String.format("[ResultsActivity] Future object: %s", future_object_in_hand));
            System.out.println(
                    String.format("[ResultsActivity] Reward amount: %s", reward_amount));
            System.out.println(
                    String.format("[ResultsActivity] Object in hand: %s", object_in_hand));
            System.out.println(
                    String.format("[ResultsActivity] Continue game: %s", continue_game));

            setObjectInHand();
            setRewardAmount();
            setSmiley();
            setContinueButton();

            displayImages();
        }

        else {
            askForResults();
        }
    }

    public void setSmiley() {

        smiley.setImageResource(map_smiley.get(success));

    }

    public void setRewardAmount() {

        reward_amount_view.setText(reward_amount);

    }

    public void setObjectInHand() {

        object_in_hand_view.setImageResource(map_objects.get(object_in_hand));
        object_in_hand = future_object_in_hand;

    }

    public void setContinueButton(){


        button_next_round.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                if (Objects.equals(continue_game, "1")) {

                    startNewActivity();
                }
                else {
                    goToEnd();
                }

            }
        });
    }

    public void displayImages() {

        findViewById(R.id.loadingPanel).setVisibility(View.GONE);
        counter.setVisibility(View.VISIBLE);
        smiley_and_object.setVisibility(View.VISIBLE);
        button_next_round.setVisibility(View.VISIBLE);
    }

    public void startNewActivity() {
        System.out.println("[ResultsActivity] Start ChoiceActivity");

        Intent intent = new Intent(getApplicationContext(), ChoiceActivity.class);
        intent.putExtra("IDX", idx);
        intent.putExtra("OBJECT_IN_HAND", object_in_hand);
        intent.putExtra("REWARD_AMOUNT", reward_amount);

        startActivity(intent);
    }

    @Override
    public void onBackPressed() {
        // your code.
    }



    public void goToEnd() {

        // For debug
        System.out.println("[ResultsActivity] Start EndActivity");

        Intent intent = new Intent(this, EndActivity.class);
        intent.putExtra("IDX", idx);
        intent.putExtra("REWARD_AMOUNT", reward_amount);

        startActivity(intent);
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