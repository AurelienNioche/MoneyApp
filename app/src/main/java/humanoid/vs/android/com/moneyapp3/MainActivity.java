package humanoid.vs.android.com.moneyapp3;

import android.graphics.Bitmap;
import android.graphics.Color;
import android.os.AsyncTask;
import android.os.Bundle;
import android.os.Handler;
import android.provider.Settings;
import android.support.v7.app.AppCompatActivity;
import android.util.Log;
import android.view.MotionEvent;
import android.view.View;
import android.widget.ImageView;
import android.widget.ProgressBar;
import android.widget.TextView;

import java.io.BufferedReader;
import java.io.DataOutputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.InetSocketAddress;
import java.net.Socket;
import java.net.UnknownHostException;
import java.util.Objects;

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
    TextView wheat_amount_end;
    TextView plus_one;

    View choice_made_view;
    View result_view;
    ProgressBar progress;

    String server_ip;
    int server_port;

    String idx = "";
    String initial_state = "";
    String in_hand = "wood";
    String demand = "";
    String server_demand = "";

    int success = 0;
    int consumption = 0;

    int wheat_amount_number = 0;

    int decision_taken = 0;
    int show_result = 0;
    int continue_game = 1;

    int got_init_information = 0;
    int logo_presentation = 1;

    int time_logo_presentation = 1000;
    int time_retry_after_failure = 200;
    int socket_time_out = 500;

    // ------------------------------------------------------------------------------ //
    // ------------------ OVERRIDED METHODS ----------------------------------------- //
    // ------------------------------------------------------------------------------ //

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        Log.e("MainActivity", "I start.");

        hideSystemUI();

        server_ip = getResources().getString(R.string.server_ip);
        server_port = Integer.parseInt(getResources().getString(R.string.server_port));

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
        wheat_amount_end = (TextView) findViewById(R.id.wheat_amount_end);
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

        getInitInformation();

        final Handler handler = new Handler();
        handler.postDelayed(new Runnable() {
            public void run() {
                // Actions to do after x ms
                endLogoPresentation();
            }
        }, time_logo_presentation);


    }

    @Override
    protected void onResume() {
        super.onResume();
        hideSystemUI();
    }

    @Override
    public void onBackPressed() {
        // Do nothing.
    }

    // ------------------------------------------------------------------------------ //
    // ------------------ REMOVE BUTTONS, ETC. -------------------------------------- //
    // ------------------------------------------------------------------------------ //

    public void hideSystemUI() {

        // Hide navigation bar
        View decorView = getWindow().getDecorView();
        decorView.setSystemUiVisibility(
                View.SYSTEM_UI_FLAG_IMMERSIVE_STICKY
                        | View.SYSTEM_UI_FLAG_FULLSCREEN // hide status bar
                        | View.SYSTEM_UI_FLAG_HIDE_NAVIGATION // hide nav bar
        );
    }

    // ------------------------------------------------------------------------------ //
    // ------------------ BEGINNING ------------------------------------------------- //
    // ------------------------------------------------------------------------------ //

    public void endLogoPresentation() {
        logo_presentation = 0;
    }

    public void getInitInformation() {

        String android_id =
                Settings.Secure.getString(this.getContentResolver(), Settings.Secure.ANDROID_ID);

        // DEMAND TO SERVER
        askServer(String.format("get_init/%s", android_id));

    }

    // ------------------------------------------------------------------------------ //
    // ------------------------------------ SHOW  ----------------------------------- //
    // ------------------------------------------------------------------------------ //

    public void showWelcome() {

        logo.setVisibility(View.INVISIBLE);
        welcome.setVisibility(View.VISIBLE);
    }

    public void showWheatAmount() {
        wheat_amount.setText(Integer.toString(wheat_amount_number));
        wheat_amount.setVisibility(View.VISIBLE);
    }

    public void showChoice(View previous_view) {

        previous_view.setVisibility(View.INVISIBLE);

        plus_one.setVisibility(View.INVISIBLE);
        wheat_amount.setText(Integer.toString(wheat_amount_number));

        if (Objects.equals(in_hand, "wood")) {
            wood_choice.setVisibility(View.VISIBLE);
        } else {
            stone_choice.setVisibility(View.VISIBLE);
        }
        decision_taken = 0;
    }

    public void showMadeChoice(View previous_view) {

        previous_view.setVisibility(View.INVISIBLE);
        progress.setVisibility(View.VISIBLE);
        choice_made_view.setVisibility(View.VISIBLE);
    }

    public void showEnd(View previous_view) {

        previous_view.setVisibility(View.INVISIBLE);

        plus_one.setVisibility(View.INVISIBLE);
        wheat_amount.setVisibility(View.INVISIBLE);
        wheat_amount_end.setText(Integer.toString(wheat_amount_number));
        wheat_amount_end.setVisibility(View.VISIBLE);

        end.setVisibility(View.VISIBLE);

    }

    public void showResult() {

        choice_made_view.setVisibility(View.INVISIBLE);
        progress.setVisibility(View.INVISIBLE);

        result_view.setVisibility(View.VISIBLE);
        if (consumption == 1) {
            plus_one.setVisibility(View.VISIBLE);
        }

    }

    // ------------------------------------------------------------------------------ //
    // ------------------ HANDLE 'NEXT' --------------------------------------------- //
    // ------------------------------------------------------------------------------ //


    public void nextOnWelcome() {

        showWheatAmount();

        if (Objects.equals(initial_state, "choice")) {
            showChoice(welcome);
        } else {
            showMadeChoice(welcome);
            // DEMAND TO SERVER
            askServer(String.format("set_choice/%s/%s", idx, demand));
        }
    }

    public void nextOnResult(View view) {

        view.setVisibility(View.INVISIBLE);

        show_result = 0;
        decision_taken = 0;

        if (success == 1 && Objects.equals(demand, "wheat")) {
            in_hand = "wood";
            ++ wheat_amount_number;
        } else {
            in_hand = demand;
        }
        if (continue_game == 1) {
            showChoice(view);
        } else {
            showEnd(view);
        }
    }

    // ------------------------------------------------------------------------------ //
    // ------------------ HANDLE CHOICE --------------------------------------------- //
    // ------------------------------------------------------------------------------ //

    public void handleInit() {

        if (logo_presentation == 0 && got_init_information == 1) {

            if (Objects.equals(initial_state, "ask_result")) {
                handleAskResult();
            }

            showWelcome();
        } else {
            final Handler handler = new Handler();
            handler.postDelayed(new Runnable() {
                public void run() {
                    // Actions to do after x ms
                    handleInit();
                }
            }, 10);  // Here are the x ms

        }

    }

    public void handleAskResult() {
        if (Objects.equals(in_hand, "wood") && Objects.equals(demand, "wheat")) {
            choice_made_view = wood_choice_wheat;
            progress = bottom_progress;
        } else if (Objects.equals(in_hand, "wood") && Objects.equals(demand, "stone")) {
            choice_made_view = wood_choice_stone;
            progress = top_progress;
        } else if (Objects.equals(in_hand, "stone") && Objects.equals(demand, "wheat")) {
            choice_made_view = stone_choice_wheat;
            progress = top_progress;
        } else if (Objects.equals(in_hand, "stone") && Objects.equals(demand, "wood")) {
            progress = bottom_progress;
            choice_made_view = stone_choice_wood;
        }
    }

    public void handleChoice(View view, String position) {

        decision_taken = 1;

        if (Objects.equals(position, "top")) {

            if (Objects.equals(in_hand, "wood")) {
                demand = "wheat";
                choice_made_view = wood_choice_wheat;
            } else {
                demand = "wood";
                choice_made_view = stone_choice_wood;
            }

            progress = bottom_progress;

        } else {

            if (Objects.equals(in_hand, "wood")) {
                demand = "stone";
                choice_made_view = wood_choice_stone;
            } else {
                demand = "wheat";
                choice_made_view = stone_choice_wheat;
            }

            progress = top_progress;

        }

        Log.e("MainActivity", String.format("Demand is %s.", demand));

        showMadeChoice(view);

        // DEMAND TO SERVER
        askServer(String.format("set_choice/%s/%s", idx, demand));
    }

    public void handleResults() {

        consumption = 0;

        if (success == 0) {
            if (Objects.equals(in_hand, "wood")) {
                result_view = wood_loss;
            } else {
                result_view = stone_loss;
            }
        } else {

            if (Objects.equals(in_hand, "wood")) {
                if (Objects.equals(demand, "stone")) {
                    result_view = wood_to_stone;
                } else {
                    result_view = wood_to_wheat;
                }

            } else {
                if (Objects.equals(demand, "wood")) {
                    result_view = stone_to_wood;
                } else {
                    result_view = stone_to_wheat;
                }
            }

            if (Objects.equals(demand, "wheat")) {
                consumption = 1;
            }
        }

        show_result = 1;
        showResult();
    }

    // ------------------------------------------------------------------------------ //
    // ------------------ TREAT SERVER REPLY ---------------------------------------- //
    // ------------------------------------------------------------------------------ //

    public void treatServerReply(String... parts) {

        if (Objects.equals(parts[1], "init")) {
            Log.e("MainApplication", "Got init information.");
            idx = parts[2];
            in_hand = parts[3];
            demand = parts[4];
            wheat_amount_number = Integer.parseInt(parts[5]);
            initial_state = parts[6];
            got_init_information = 1;
            handleInit();
        } else if (Objects.equals(parts[1], "result")) {
            Log.e("MainApplication", "Got result.");
            success = Integer.parseInt(parts[2]);
            continue_game = Integer.parseInt(parts[3]);  // Will have an impact in 'nextOnResult'
            handleResults();
        } else {
            Log.e("MainApplication", "ERROR!");
        }
    }

    // ----------------------------------------------------------------------------------------- //
    // ---------------------------- COMMUNICATION WITH THE SERVER ------------------------------ //
    // ----------------------------------------------------------------------------------------- //

    void askServer(String demand) {
        Log.e("MainActivity", String.format("Ask the server: '%s'.", demand));
        server_demand = demand;
        Communicate com = new Communicate();
        com.execute(demand);
    }

    void handleServerResponse(String response) {

        if (response != null) {
            Log.e("MainActivity", String.format("Received from server: '%s'", response));
            String[] parts = response.split("/");
            if (parts.length > 1 && Objects.equals(parts[0], "reply")) {
                Log.e("MainActivity", "Server response is in correct shape.");
                treatServerReply(parts);
            } else {
                retryDemand(response);
            }
        } else {
            retryDemand("No response.");
        }
    }

    void retryDemand(String server_response) {
        Log.e("MainActivity",
                String.format("Server response is in bad shape: '%s'. Retry the same demand.",
                        server_response));
        final Handler handler = new Handler();
        handler.postDelayed(new Runnable() {
            public void run() {
                // Actions to do after x ms
                askServer(server_demand);
            }
        }, time_retry_after_failure);
    }

    private class Communicate extends AsyncTask<String, Integer, String> {
        protected String doInBackground(String... demand) {
            return communicate(demand[0]);
        }

        protected void onPostExecute(String result) {
            handleServerResponse(result);
        }
    }

    String communicate(String demand) {
        try {

            Socket s = new Socket();
            s.connect(new InetSocketAddress(server_ip, server_port), socket_time_out);

            DataOutputStream out = new DataOutputStream(s.getOutputStream());
            BufferedReader input = new BufferedReader(new InputStreamReader(s.getInputStream()));

            out.writeBytes(demand + '\n');

            String st = input.readLine();

            s.close();

            return st;


        } catch (UnknownHostException e) {
            // TODO Auto-generated catch block
            // e.printStackTrace();
            return String.valueOf(e);
        } catch (IOException e) {
            // TODO Auto-generated catch block
            // e.printStackTrace();
            return String.valueOf(e);
        } catch (RuntimeException e) {
            return String.valueOf(e);

        }
    }

    // ----------------------------------------------------------------------------------------- //
    // ---------------------------- CLICK HANDLER ---------------------------------------------- //
    // ----------------------------------------------------------------------------------------- //

    public boolean onTouch (View v, MotionEvent ev) {
        final int action = ev.getAction();
        // (1)
        final int evX = (int) ev.getX();
        final int evY = (int) ev.getY();
        switch (action) {
            case MotionEvent.ACTION_DOWN :
                Log.e("MainActivity", "Clicked down.");
            case MotionEvent.ACTION_UP :
                Log.e("MainActivity", "Clicked up.");
                // The hidden image (clickable_areas) has three different hotspots on it.
                // The colors are red, blue, and green.
                // Use image_areas to determine which region the user touched.
                int touchColor = getHotspotColor (R.id.clickable_areas, evX, evY);
                // Compare the touchColor to the expected values.
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
                        Log.e("MainActivity", "Clicked on red (that is at the TOP).");
                        if (v == wood_choice || v == stone_choice) {
                            Log.e("MainActivity", "It's choice situation.");
                            handleChoice(v, "top"); // Red is at the top.
                        }

                    }

                } else if (ct.closeMatch (Color.GREEN, touchColor, tolerance)){

                    if (decision_taken == 0) {
                        Log.e("MainActivity", "Clicked on green (that is at the BOTTOM).");
                        if (v == wood_choice || v == stone_choice) {
                            Log.e("MainActivity", "It's choice situation.");
                            handleChoice(v, "bottom"); // Green is at the bottom.

                        }
                    }
                } else if (ct.closeMatch (Color.BLUE, touchColor, tolerance)){

                    if (v == welcome) {
                        Log.e("MainActivity", "Next on welcome.");
                        nextOnWelcome();
                    } else if (show_result == 1) {
                        nextOnResult(v);
                        Log.e("MainActivity", "Next on results.");
                    }
                }
                break;
        }
        return true;
    }

    public int getHotspotColor (int hotspotId, int x, int y) {
        ImageView img = (ImageView) findViewById (hotspotId);
        img.setDrawingCacheEnabled(true);
        Bitmap hotspots = Bitmap.createBitmap(img.getDrawingCache());
        img.setDrawingCacheEnabled(false);
        return hotspots.getPixel(x, y);
    }
}
