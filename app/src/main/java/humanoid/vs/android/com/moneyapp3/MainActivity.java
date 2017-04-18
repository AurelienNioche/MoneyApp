package humanoid.vs.android.com.moneyapp3;

import android.graphics.Bitmap;
import android.graphics.Color;
import android.os.Bundle;
import android.os.Handler;
import android.support.v7.app.AppCompatActivity;
import android.util.Log;
import android.view.MotionEvent;
import android.view.View;
import android.widget.ImageView;
import android.widget.ProgressBar;
import android.widget.TextView;

import java.util.Objects;
import java.util.Random;

public class MainActivity extends AppCompatActivity implements View.OnTouchListener {

    ImageView logo;
    ImageView welcome;
    ImageView wood_choice;
    ImageView wood_loss;
    ImageView wood_to_wheat;
    ImageView wood_to_stone;
    ImageView stone_choice;
    ImageView stone_loss;
    ImageView stone_to_wheat;
    ImageView stone_to_wood;
    ImageView end;
    ImageView stone_choice_wood;
    ImageView stone_choice_wheat;
    ImageView wood_choice_stone;
    ImageView wood_choice_wheat;


    ProgressBar bottom_progress;
    ProgressBar top_progress;

    TextView wheat_amount;
    TextView plus_one;

    humanoid.vs.android.com.moneyapp3.Client client;

//    HashMap <String, String> red_click = new HashMap<>();
//    HashMap <String, String> green_click = new HashMap<>();

    String in_hand = "wood";
    String demand = "";

    int success = 0;

    int wheat_amount_number = 0;

    int decision_taken = 0;
    int show_result = 0;

    int time_logo_presentation = 1000;
    int time_fake_server_delay = 1000;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        Log.d("MyComments", "I start.");

        hideSystemUI();
        // build_map();

        logo = (ImageView) findViewById(R.id.logo);
        welcome = (ImageView) findViewById(R.id.welcome);
        wood_choice = (ImageView) findViewById(R.id.wood_choice);
        wood_loss = (ImageView) findViewById(R.id.wood_loss);
        wood_to_wheat = (ImageView) findViewById(R.id.wood_to_wheat);
        wood_to_stone = (ImageView) findViewById(R.id.wood_to_stone);
        stone_choice = (ImageView) findViewById(R.id.stone_choice);
        stone_loss = (ImageView) findViewById(R.id.stone_loss);
        stone_to_wheat = (ImageView) findViewById(R.id.stone_to_wheat);
        stone_to_wood = (ImageView) findViewById(R.id.stone_to_wood);
        end = (ImageView) findViewById(R.id.end);
        stone_choice_wood = (ImageView) findViewById(R.id.stone_choice_wood);
        stone_choice_wheat = (ImageView) findViewById(R.id.stone_choice_wheat);
        wood_choice_stone = (ImageView) findViewById(R.id.wood_choice_stone);
        wood_choice_wheat = (ImageView) findViewById(R.id.wood_choice_wheat);

        bottom_progress = (ProgressBar) findViewById(R.id.bottom_progress);
        top_progress = (ProgressBar) findViewById(R.id.top_progress);

        wheat_amount = (TextView) findViewById(R.id.wheat_amount);
        plus_one = (TextView) findViewById(R.id.plus_one);

        welcome.setOnTouchListener(this);
        wood_choice.setOnTouchListener(this);
        wood_loss.setOnTouchListener(this);
        wood_to_wheat.setOnTouchListener(this);
        wood_to_stone.setOnTouchListener(this);
        stone_choice.setOnTouchListener(this);
        stone_loss.setOnTouchListener(this);
        stone_to_wheat.setOnTouchListener(this);
        stone_to_wood.setOnTouchListener(this);

        client = new Client(this);

        final Handler handler = new Handler();
        handler.postDelayed(new Runnable() {
            public void run() {
                // Actions to do after x ms
                getInit();
            }
        }, time_logo_presentation);


    }
    public void getInit() {

//        String android_id = Settings.Secure.getString(this.getContentResolver(), Settings.Secure.ANDROID_ID);
//        addressDemandToServer(String.format("get_init/%s", android_id));
        logo.setVisibility(View.INVISIBLE);
        welcome.setVisibility(View.VISIBLE);


    }


    @Override
    protected void onResume() {
        super.onResume();
        hideSystemUI();
    }

//    private void build_map() {
//        red_click.put("stone", "wood");
//        red_click.put("wood", "wheat");
//        green_click.put("stone", "wheat");
//        green_click.put("wood", "stone");
//    }

    public void hideSystemUI() {

        // Hide navigation bar
        View decorView = getWindow().getDecorView();
        decorView.setSystemUiVisibility(
                View.SYSTEM_UI_FLAG_IMMERSIVE_STICKY
                        | View.SYSTEM_UI_FLAG_FULLSCREEN // hide status bar
                        | View.SYSTEM_UI_FLAG_HIDE_NAVIGATION // hide nav bar
        );
    }

    @Override
    public void onBackPressed() {
        // Do nothing.
    }

    public boolean onTouch (View v, MotionEvent ev) {
        final int action = ev.getAction();
        // (1)
        final int evX = (int) ev.getX();
        final int evY = (int) ev.getY();
        switch (action) {
            case MotionEvent.ACTION_DOWN :
                Log.d("MyComments", "Clicked down.");
            case MotionEvent.ACTION_UP :
                Log.d("MyComments", "Clicked up.");
                // On the UP, we do the click action.
                // The hidden image (image_areas) has three different hotspots on it.
                // The colors are red, blue, and yellow.
                // Use image_areas to determine which region the user touched.
                // (2)
                int touchColor = getHotspotColor (R.id.clickable_areas, evX, evY);
                // Compare the touchColor to the expected values.
                // Switch to a different image, depending on what color was touched.
                // Note that we use a Color Tool object to test whether the
                // observed color is close enough to the real color to
                // count as a match. We do this because colors on the screen do
                // not match the map exactly because of scaling and
                // varying pixel density.
                ColorTool ct = new ColorTool ();
                int tolerance = 25;

                if (ct.closeMatch (Color.RED, touchColor, tolerance)) {

                    if (decision_taken == 0) {
                        // Do the action associated with the RED region
                        Log.d("MyComments", "Clicked on red.");
                        if (v == wood_choice || v == stone_choice) {
                            Log.d("MyComments", "It's choice situation.");
                            clickOnRed(v);
                        }

                    }

                } else if (ct.closeMatch (Color.GREEN, touchColor, tolerance)){

                    if (decision_taken == 0) {
                        Log.d("MyComments", "Clicked on green.");
                        if (v == wood_choice || v == stone_choice) {
                            Log.d("MyComments", "It's choice situation.");
                            clickOnGreen(v);

                        }
                    }
                } else if (ct.closeMatch (Color.BLUE, touchColor, tolerance)){
                    Log.d("MyComments", "Clicked on blue.");
                    if (v == welcome) {
                        Log.d("MyComments", "It's welcome.");
                        nextOnWelcome();
                    } else if (show_result == 1) {
                        nextOnResult(v);
                        Log.d("MyComments", "It's wood_loss.");
                    }
                }
                break;
        } // end switch
//        if (nextImage > 0) {
//            imageView.setImageResource (nextImage);
//            imageView.setTag (nextImage);
//        }
        return true;
    }

    public void nextOnWelcome() {
        welcome.setVisibility(View.INVISIBLE);
        wheat_amount.setVisibility(View.VISIBLE);
        showChoice();
    }

    public void showChoice() {

        plus_one.setVisibility(View.INVISIBLE);
        wheat_amount.setText(String.format("%d", wheat_amount_number));

        if (Objects.equals(in_hand, "wood")) {
            wood_choice.setVisibility(View.VISIBLE);
        } else {
            stone_choice.setVisibility(View.VISIBLE);
        }
        decision_taken = 0;
    }

    public void nextOnResult(View view) {
        show_result = 0;
        decision_taken = 0;
        view.setVisibility(View.INVISIBLE);
        showChoice();
    }

    public void clickOnRed(View view) {

        // Red is top
        decision_taken = 1;

        if (view == wood_choice) {
            demand = "wheat";
            wood_choice_wheat.setVisibility(View.VISIBLE);
        } else {
            demand = "wood";
            stone_choice_wood.setVisibility(View.VISIBLE);
        }

        Log.d("MyComments", String.format("Demand is %s.", demand));

        view.setVisibility(View.INVISIBLE);
        bottom_progress.setVisibility(View.VISIBLE);

        // For test
        final Handler handler = new Handler();
        handler.postDelayed(new Runnable() {
            public void run() {
                // Actions to do after x ms
                showResult();
            }
        }, time_fake_server_delay);
    }

    public void clickOnGreen(View view) {

        decision_taken = 1;
        // Green is bottom

        if (Objects.equals(in_hand, "wood")) {
            demand = "stone";
            wood_choice_stone.setVisibility(View.VISIBLE);
        } else {
            demand = "wheat";
            stone_choice_wheat.setVisibility(View.VISIBLE);
        }

        Log.d("MyComments", String.format("Demand is %s.", demand));

        view.setVisibility(View.INVISIBLE);
        top_progress.setVisibility(View.VISIBLE);

        // For test
        final Handler handler = new Handler();
        handler.postDelayed(new Runnable() {
            public void run() {
                // Actions to do after x ms
                showResult();
            }
        }, time_fake_server_delay);
    }
//
//    public void clickOnBlue(View view) {
//
//    }

    public void showResult() {

        Random random = new Random();
        success = random.nextInt(2);
        // success = 1;

        Log.d("MyComments", String.format("success: %d", success));

        show_result = 1;

        stone_choice_wheat.setVisibility(View.INVISIBLE);
        stone_choice_wood.setVisibility(View.INVISIBLE);

        wood_choice_wheat.setVisibility(View.INVISIBLE);
        wood_choice_stone.setVisibility(View.INVISIBLE);

        bottom_progress.setVisibility(View.INVISIBLE);
        top_progress.setVisibility(View.INVISIBLE);

        if (success == 0) {
            if (Objects.equals(in_hand, "wood")) {
                wood_loss.setVisibility(View.VISIBLE);
            } else {
                stone_loss.setVisibility(View.VISIBLE);
            }
        } else {

            if (Objects.equals(in_hand, "wood")) {
                if (Objects.equals(demand, "stone")) {
                    wood_to_stone.setVisibility(View.VISIBLE);
                } else {
                    wood_to_wheat.setVisibility(View.VISIBLE);
                }

            } else {
                if (Objects.equals(demand, "wood")) {
                    stone_to_wood.setVisibility(View.VISIBLE);
                } else {
                    stone_to_wheat.setVisibility(View.VISIBLE);
                }
            }

            if (Objects.equals(demand, "wheat")) {
                in_hand = "wood";
                plus_one.setVisibility(View.VISIBLE);
                ++ wheat_amount_number;
            } else {
                in_hand = demand;
            }
        }

    }

    public int getHotspotColor (int hotspotId, int x, int y) {
        ImageView img = (ImageView) findViewById (hotspotId);
        img.setDrawingCacheEnabled(true);
        Bitmap hotspots = Bitmap.createBitmap(img.getDrawingCache());
        img.setDrawingCacheEnabled(false);
        return hotspots.getPixel(x, y);
    }

    public void addressDemandToServer(String demand) {

        client.ask_server(demand);

    }

    public void handleServerResponse(String server_msg) {

        String[] parts = server_msg.split("/");

        if (parts.length > 1) {

//            this.idx = parts[0];
//            this.object_in_hand = parts[1];
//            this.reward_amount = parts[2];
//
//            System.out.println(
//                    String.format(
//                            "[MainActivity] Idx is %s, " +
//                                    "object in hand is %s, reward amount is %s.",
//                            this.idx, this.object_in_hand, this.reward_amount)
//            );
//
//            welcome.setVisibility(View.INVISIBLE);
//            schema.setVisibility(View.VISIBLE);


        } else {

//            getInit();

        }

    }
}
