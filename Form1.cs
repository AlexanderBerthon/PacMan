using System.Text.RegularExpressions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace PacMan {
    /* TAG KEY
        _______________________________________________
        TAG         -  DEFINITION
        _______________________________________________
        "AI1 orb"   -  Ghost one, needs orb replacement
        "AI0"       -  Ghost zero, no orb replacement
        "Player"    -  The player / pacman
        "0"         -  Empty space / no orb
        "1"         -  Full space / orb
        "3"         -  Special orb / power up
        _______________________________________________
    */
    public partial class Form1 : Form {
        //highscore variables
        Highscore[] highScores;

        //player variables
        int currentIndex;
        int trajectory;

        //game variables
        int score;
        int orbs;
        int AIVulnerable;
        Ghost[] ghosts;
        Button[] btnArray;

        //clock and timer
        Boolean gameOver;
        Boolean skip;
        Boolean animation;
        int despawn;
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer animationTimer = new System.Windows.Forms.Timer();

        public Form1() {
            InitializeComponent();

            //Initialize highscore variables
            string[] inputData;
            highScores = new Highscore[5];

            //create the highscore file if it doesn't exist
            if (!File.Exists("C:\\Users\\" + Environment.UserName + "\\Desktop\\Pacman_Highscores.txt")) {
                string[] temp = { "Jeff 0", "Kenny 0", "Taylor 0", "Alex 0", "Martin 0" };
                File.WriteAllLines("C:\\Users\\" + Environment.UserName + "\\Desktop\\Pacman_Highscores.txt", temp); //creates files and populates with dummy data
            }

            inputData = System.IO.File.ReadAllLines("C:\\Users\\" + Environment.UserName + "\\Desktop\\Pacman_Highscores.txt");

            if (inputData.Length > 0) {
                for(int i = 0; i<inputData.Length; i++) {
                    string[] split = new string[2];
                    split = inputData[i].Split(" ");
                    highScores[i] = new Highscore(split[0], int.Parse(split[1]));
                }
            }

            //Initialize player variables
            currentIndex = 120;
            trajectory = 0;

            //Initialize game variables
            score = 0;
            orbs = 110;
            AIVulnerable = 0;
            ghosts = new Ghost[6]; //[6] = number of ghosts
            btnArray = new Button[256];

            flowLayoutPanel1.Controls.CopyTo(btnArray, 0);

            //clock and timer
            gameOver = false;
            skip = true;
            animation = true;
            despawn = 0;

            timer.Interval = 300;
            timer.Tick += new EventHandler(TimerEventProcessor);

            animationTimer.Interval = 150;
            animationTimer.Tick += new EventHandler(TimerEventProcessor2);

            //ghost creation
            ghosts[0] = new Ghost(71, 10); //(start index, delay / # of game tick cycles)
            ghosts[1] = new Ghost(72, 0);
            ghosts[2] = new Ghost(73, 20);
            ghosts[3] = new Ghost(87, 30);
            ghosts[4] = new Ghost(88, 40);
            ghosts[5] = new Ghost(89, 50);

            //distinct labels
            for (int i = 0; i < ghosts.Length; i++) {
                switch (i) {
                    case 0:
                        btnArray[ghosts[i].getIndex()].BackgroundImage = Properties.Resources.Ghost1;
                        btnArray[ghosts[i].getIndex()].Tag = "AI0";
                        break;
                    case 1:
                        btnArray[ghosts[i].getIndex()].BackgroundImage = Properties.Resources.Ghost2;
                        btnArray[ghosts[i].getIndex()].Tag = "AI1";
                        break;
                    case 2:
                        btnArray[ghosts[i].getIndex()].BackgroundImage = Properties.Resources.Ghost3;
                        btnArray[ghosts[i].getIndex()].Tag = "AI2";
                        break;
                    case 3:
                        btnArray[ghosts[i].getIndex()].BackgroundImage = Properties.Resources.Ghost4;
                        btnArray[ghosts[i].getIndex()].Tag = "AI3";
                        break;
                    case 4:
                        btnArray[ghosts[i].getIndex()].BackgroundImage = Properties.Resources.Ghost5;
                        btnArray[ghosts[i].getIndex()].Tag = "AI4";
                        break;
                    case 5:
                        btnArray[ghosts[i].getIndex()].BackgroundImage = Properties.Resources.Ghost6;
                        btnArray[ghosts[i].getIndex()].Tag = "AI5";
                        break;
                }
            }

            //begin
            timer.Start();
            animationTimer.Start();
        }


        //movement clock
        private void TimerEventProcessor(Object anObject, EventArgs eventArgs) {

            //Orb check / verification
            if(orbs == 5 || orbs == 25 || orbs == 50 || orbs == 75) {
                int temp = 0;
                foreach (Button btn in btnArray) {
                    if (btn.Tag == "1" || btn.Tag.ToString().Contains("orb")) {
                    temp++;
                    }
                }

                if (orbs != temp) {
                    orbs = temp;
                }
            }
            
            if (!gameOver) {
                move();
            }

            if (AIVulnerable <= 0) {
                AIMove();
            }
            else {
                if (skip) {
                    skip = false;
                }
                else {
                    skip = true;
                    AIMove();
                    AIVulnerable--;
                }
            }
            //added condition so this function is only called 1 time rather than every time the clock ticks after the game has ended
            if (gameOver && GameOverLabel.Visible == false) {
                endGame();
            }
        }

        //animation clock
        private void TimerEventProcessor2(Object anObject, EventArgs eventArgs) {
            if (animation) {
                switch (trajectory) {
                    case 1:
                        btnArray[currentIndex].BackgroundImage = Properties.Resources.Right;
                        break;
                    case -1:
                        btnArray[currentIndex].BackgroundImage = Properties.Resources.Left;
                        break;
                    case 16:
                        btnArray[currentIndex].BackgroundImage = Properties.Resources.Down;
                        break;
                    case -16:
                        btnArray[currentIndex].BackgroundImage = Properties.Resources.Up;
                        break;
                    default:
                        btnArray[currentIndex].BackgroundImage = Properties.Resources.Right;
                        break;
                }
                animation = false;
            }
            else {
                btnArray[currentIndex].BackgroundImage = Properties.Resources.Closed;
                animation = true;
            }
            if (gameOver) { //despawn pacman animation
                if(despawn < 8) {
                    despawn++;
                }
                switch (despawn) {
                    case 1:
                        btnArray[currentIndex].BackgroundImage = Properties.Resources.Right;
                        break;
                    case 2:
                        btnArray[currentIndex].BackgroundImage = Properties.Resources.despawn1;
                        break;
                    case 3:
                        btnArray[currentIndex].BackgroundImage = Properties.Resources.despawn2;
                        break;
                    case 4:
                        btnArray[currentIndex].BackgroundImage = Properties.Resources.despawn3;
                        break;
                    case 5:
                        btnArray[currentIndex].BackgroundImage = Properties.Resources.despawn4;
                        break;
                    case 6:
                        btnArray[currentIndex].BackgroundImage = Properties.Resources.despawn5;
                        break;
                    case 7:
                        btnArray[currentIndex].BackgroundImage = Properties.Resources.despawn6;
                        break;
                    case 8:
                        btnArray[currentIndex].BackgroundImage = Properties.Resources.despawn4;
                        break;
                }
            }
        }

        /// This function is responsible for the AI Logic and movement
        /// Essentially this function will calculate all moves available for each ghosts and randomly choose a valid move
        /// Maximum of 3 choices, often only 1 choice. Calculated every tick of the movement clock for each of the active ghosts / ghosts who have a delay = 
        /// Movement also encompasses animation, background image swapping, and orb maintenance.
        private void AIMove() {
            Random random = new Random();
            List<int> validMoves = new List<int>();
            Boolean replaceOrb = false;
            Boolean replaceSpecialOrb = false;

            for (int i = 0; i < ghosts.Length; i++) {
                //logic
                if (ghosts[i].getDelay() == 0) {
                    //check possible moves
                    if (btnArray[ghosts[i].getIndex() + 1].BackColor != Color.DarkSlateBlue &&
                        !(btnArray[ghosts[i].getIndex() + 1].Tag.ToString().Contains("AI")) &&
                        ghosts[i].getTrajectory() != -1) {
                        validMoves.Add(1);
                    }

                    if (btnArray[ghosts[i].getIndex() - 1].BackColor != Color.DarkSlateBlue &&
                        !(btnArray[ghosts[i].getIndex() - 1].Tag.ToString().Contains("AI")) &&
                        ghosts[i].getTrajectory() != +1) {
                        validMoves.Add(-1);
                    }

                    if (btnArray[ghosts[i].getIndex() + 16].BackColor != Color.DarkSlateBlue &&
                        !(btnArray[ghosts[i].getIndex() + 16].Tag.ToString().Contains("AI")) &&
                        ghosts[i].getTrajectory() != -16) {
                        validMoves.Add(16);
                    }

                    if (btnArray[ghosts[i].getIndex() - 16].BackColor != Color.DarkSlateBlue &&
                        !(btnArray[ghosts[i].getIndex() - 16].Tag.ToString().Contains("AI")) &&
                        ghosts[i].getTrajectory() != +16) {
                        validMoves.Add(-16);
                    }

                    //unstuck if needed - this location is important. runs on next move, not current move. 
                    if (ghosts[i].stuck()) {
                        if (btnArray[ghosts[i].getIndex() + 1].BackColor != Color.DarkSlateBlue &&
                        !(btnArray[ghosts[i].getIndex() + 1].Tag.ToString().Contains("AI"))) {
                            ghosts[i].stuck(false);
                            validMoves.Add(1);
                        }

                        if (btnArray[ghosts[i].getIndex() - 1].BackColor != Color.DarkSlateBlue &&
                            !(btnArray[ghosts[i].getIndex() - 1].Tag.ToString().Contains("AI"))) {
                            ghosts[i].stuck(false);
                            validMoves.Add(-1);
                        }

                        if (btnArray[ghosts[i].getIndex() + 16].BackColor != Color.DarkSlateBlue &&
                            !(btnArray[ghosts[i].getIndex() + 16].Tag.ToString().Contains("AI"))) {
                            ghosts[i].stuck(false);
                            validMoves.Add(16);
                        }

                        if (btnArray[ghosts[i].getIndex() - 16].BackColor != Color.DarkSlateBlue &&
                            !(btnArray[ghosts[i].getIndex() - 16].Tag.ToString().Contains("AI"))) {
                            ghosts[i].stuck(false);
                            validMoves.Add(-16);
                        }
                    }

                    if (validMoves.Count < 1) {
                        ghosts[i].stuck(true);
                    }

                    if (!ghosts[i].stuck()) {
                        //replace
                        if (btnArray[ghosts[i].getIndex()].Tag.ToString().Contains("orb")) {
                            btnArray[ghosts[i].getIndex()].BackgroundImage = Properties.Resources.orb;
                            btnArray[ghosts[i].getIndex()].Tag = "1";
                        }
                        else if (btnArray[ghosts[i].getIndex()].Tag.ToString().Contains("special")) {
                            btnArray[ghosts[i].getIndex()].Tag = "3";
                            btnArray[ghosts[i].getIndex()].BackgroundImage = Properties.Resources.orb2;
                        }
                        else {
                            btnArray[ghosts[i].getIndex()].Tag = "0";
                            btnArray[ghosts[i].getIndex()].BackgroundImage = null;
                        }

                        //move
                        int choice = random.Next(0, validMoves.Count);
                        if (btnArray[ghosts[i].getIndex() + validMoves[choice]].Tag == "1") {
                            replaceOrb = true;
                        }
                        else if (btnArray[ghosts[i].getIndex() + validMoves[choice]].Tag == "3") {
                            replaceSpecialOrb = true;
                        }

                        if (btnArray[ghosts[i].getIndex()].Tag == "Player" || btnArray[ghosts[i].getIndex() + validMoves[choice]].Tag == "Player") {
                            if (AIVulnerable > 0) {
                                if (btnArray[ghosts[i].getIndex()].Tag.ToString().Contains("orb")) {
                                    orbs--;
                                }
                                destroyGhost(i);
                                validMoves.Clear();
                                score += 25;
                            }
                            else {
                                gameOver = true;
                            }
                        }
                        else {
                            ghosts[i].update(validMoves[choice]);
                        }
                    }
                }

                //animation
                if (!ghosts[i].stuck()) {
                    btnArray[ghosts[i].getIndex()].Tag = "AI" + i;

                    if (AIVulnerable > 0) {
                        btnArray[ghosts[i].getIndex()].BackgroundImage = Properties.Resources.Ghost7;
                    }
                    else {
                        switch (i) {
                            case 0:
                                btnArray[ghosts[i].getIndex()].BackgroundImage = Properties.Resources.Ghost1;
                                break;
                            case 1:
                                btnArray[ghosts[i].getIndex()].BackgroundImage = Properties.Resources.Ghost2;
                                break;
                            case 2:
                                btnArray[ghosts[i].getIndex()].BackgroundImage = Properties.Resources.Ghost3;
                                break;
                            case 3:
                                btnArray[ghosts[i].getIndex()].BackgroundImage = Properties.Resources.Ghost4;
                                break;
                            case 4:
                                btnArray[ghosts[i].getIndex()].BackgroundImage = Properties.Resources.Ghost5;
                                break;
                            case 5:
                                btnArray[ghosts[i].getIndex()].BackgroundImage = Properties.Resources.Ghost6;
                                break;
                        }
                    }
                }
                    
                if (replaceOrb) {
                    btnArray[ghosts[i].getIndex()].Tag += " orb";
                }
                else if (replaceSpecialOrb) {
                    btnArray[ghosts[i].getIndex()].Tag += " special";
                }
                validMoves.Clear();
                replaceOrb = false;
                replaceSpecialOrb = false;
            }

            //reduce delay each tick
            for (int i = 0; i < ghosts.Length; i++) {
                if(ghosts[i].getDelay() > 0){
                    ghosts[i].reduceDelay();
                }
            }
        }

        /// This function is responsible for the logic behind player movement and collision
        private void move() {
            
            if (trajectory != 0) {
                btnArray[currentIndex].BackgroundImage = null;
                btnArray[currentIndex].Tag = "0";
            }

            if (btnArray[currentIndex + trajectory].BackColor == Color.DarkSlateBlue) {
                trajectory = 0;
            }
            else if (btnArray[currentIndex + trajectory].Tag.ToString().Contains("AI")) {
                if (AIVulnerable > 0) {
                    //ghost capture
                    if(btnArray[currentIndex + trajectory].Tag.ToString().Contains("orb")){
                        orbs--;
                    }
                    destroyGhost(int.Parse(btnArray[currentIndex + trajectory].Tag.ToString().Substring(2,1)));
                    score += 25;
                }
                else {              
                gameOver = true;
              }
            }
            else {
                currentIndex += trajectory;

                if(btnArray[currentIndex].Tag == "1") {
                    score+=1;
                    orbs--;
                    ScoreLabel.Text = score.ToString();
                    if (orbs == 0) {
                        nextStage();
                    }
                }
                else if(btnArray[currentIndex].Tag == "3") {
                    AIVulnerable = 10;
                }
            }
            btnArray[currentIndex].Tag = "Player";
        }

        private void movement_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == 'w') {
                trajectory = -16;
            }
            else if (e.KeyChar == 'a') {
                trajectory = -1;
            }
            else if (e.KeyChar == 's') {
                trajectory = 16;
            }
            else if (e.KeyChar == 'd') {
                trajectory = 1;
            }
        }

        ///Allows the game to restart without having to relaunch the entire application.
        private void continuebutton_Click(object sender, EventArgs e) {
            //hide highscore board
            GameOverLabel.Visible = false;
            HighscorePanel.Visible = false;            
            NewHighscorePanel.Visible = false;
            continuebutton.Visible = false;
            exitbutton.Visible = false;
            Playagainlabel.Visible = false;
            gameOver = false;
            despawn = 0;

            score = 0;
            orbs = 110;
            currentIndex = 120;
            trajectory = 0;

            //delete current ghosts
            for(int i = 0; i<ghosts.Length; i++) {
                ghosts[i] = null;
            }

            //replace orbs / clean up tags
            foreach(Button btn in btnArray) {
                if(btn.TabIndex == 71 || btn.TabIndex == 72 || btn.TabIndex == 73 || btn.TabIndex == 87 ||
                    btn.TabIndex == 120 || btn.TabIndex == 88 || btn.TabIndex == 89 || btn.TabIndex == 56) {
                    btn.Tag = "";
                }
                else if (btn.BackColor == Color.Black) {
                    if (btn.TabIndex == 45 || btn.TabIndex == 146 ||
                        btn.TabIndex == 182 || btn.TabIndex == 156) {
                        btn.Tag = "3";
                        btn.BackgroundImage = Properties.Resources.orb2;
                    }
                    else {
                        btn.Tag = "1";
                        btn.BackgroundImage = Properties.Resources.orb;
                    }
                }
            }

            //recreate/reset ghosts
            ghosts[0] = new Ghost(71, 10);
            ghosts[1] = new Ghost(72, 0);
            ghosts[2] = new Ghost(73, 20);
            ghosts[3] = new Ghost(87, 30);
            ghosts[4] = new Ghost(88, 40);
            ghosts[5] = new Ghost(89, 50);

            //distinct labels
            for (int i = 0; i < ghosts.Length; i++) {
                switch (i) {
                    case 0:
                        btnArray[ghosts[i].getIndex()].BackgroundImage = Properties.Resources.Ghost1;
                        btnArray[ghosts[i].getIndex()].Tag = "AI0";
                        break;
                    case 1:
                        btnArray[ghosts[i].getIndex()].BackgroundImage = Properties.Resources.Ghost2;
                        btnArray[ghosts[i].getIndex()].Tag = "AI1";
                        break;
                    case 2:
                        btnArray[ghosts[i].getIndex()].BackgroundImage = Properties.Resources.Ghost3;
                        btnArray[ghosts[i].getIndex()].Tag = "AI2";
                        break;
                    case 3:
                        btnArray[ghosts[i].getIndex()].BackgroundImage = Properties.Resources.Ghost4;
                        btnArray[ghosts[i].getIndex()].Tag = "AI3";
                        break;
                    case 4:
                        btnArray[ghosts[i].getIndex()].BackgroundImage = Properties.Resources.Ghost5;
                        btnArray[ghosts[i].getIndex()].Tag = "AI4";
                        break;
                    case 5:
                        btnArray[ghosts[i].getIndex()].BackgroundImage = Properties.Resources.Ghost6;
                        btnArray[ghosts[i].getIndex()].Tag = "AI5";
                        break;
                }
            }
        }

        /// Exits the application when the button is clicked. Button shown on game over. 
        private void exitbutton_Click(object sender, EventArgs e) {
            timer.Stop();
            animationTimer.Stop();
            Application.Exit();
        }
        
        /// Simple helper function that stops the game and displays game over menus when called.
        private void endGame() {
            exitbutton.Visible = true;
            continuebutton.Visible = true;
            Playagainlabel.Visible = true;
            Boolean newHighScore = false;

            //check for new highscore
            for(int i = 0; i < 5; i++) {
                if(score >= highScores[i].getScore()) {
                    newHighScore = true;
                }
            }

            if(newHighScore) {
                //display new highscore menu
                NewHighscorePanel.Visible = true;
                NewHighscorePanel.BringToFront();
                NewHighscoreLabel.Visible = true;
                NewHighScoreTextBox.Visible = true;
                ConfirmUserInput.Visible = true;
            }
            else {
                //populate highscore board
                HighscoreName1.Text = highScores[0].getName();
                HighscoreName2.Text = highScores[1].getName();
                HighscoreName3.Text = highScores[2].getName();
                HighscoreName4.Text = highScores[3].getName();
                HighscoreName5.Text = highScores[4].getName();
                Highscore1.Text = highScores[0].getScore().ToString();
                Highscore2.Text = highScores[1].getScore().ToString();
                Highscore3.Text = highScores[2].getScore().ToString();
                Highscore4.Text = highScores[3].getScore().ToString();
                Highscore5.Text = highScores[4].getScore().ToString();

                //display highscore board
                GameOverLabel.Visible = true;
                HighscorePanel.Visible = true;
                HighscorePanel.BringToFront();
                HighscoreTable.Visible = true;
                HighscoreLabel.Visible = true;
                HighscoreName1.Visible = true;
                HighscoreName2.Visible = true;
                HighscoreName3.Visible = true;
                HighscoreName4.Visible = true;
                HighscoreName5.Visible = true;
                Highscore1.Visible = true;
                Highscore2.Visible = true;
                Highscore3.Visible = true;
                Highscore4.Visible = true;
                Highscore5.Visible = true;

                String[] temp = new string[5];

                //write to file
                for (int i = 0; i < 5; i++) {
                    temp[i] = highScores[i].getName() + " " + highScores[i].getScore().ToString();
                }

                File.WriteAllLines("C:\\Users\\" + Environment.UserName + "\\Desktop\\Pacman_Highscores.txt", temp);
            }
        }

        /// Helper function to identify, remove, and replace a ghost object in the ghosts[] array
        private void destroyGhost(int index) {
            btnArray[ghosts[index].getIndex()].BackgroundImage = null;
            btnArray[ghosts[index].getIndex()].Tag = "";
            ghosts[index] = null;
            switch (index) {
                case 0:
                    ghosts[index] = new Ghost(71, 10);
                    break;
                case 1:
                    ghosts[index] = new Ghost(72, 0);
                    break;
                case 2:
                    ghosts[index] = new Ghost(73, 20);
                    break;
                case 3:
                    ghosts[index] = new Ghost(87, 30);
                    break;
                case 4:
                    ghosts[index] = new Ghost(88, 40);
                    break;
                case 5:
                    ghosts[index] = new Ghost(89, 50);
                    break;
            }
        }

        /// Function to reset the game board for the next stage once all orbs are collected.
        /// Similar to the restart function but the score is maintained
        /// lot of reused code here, could I combine the two functions?
        private void nextStage() {
            currentIndex = 120;
            trajectory = 0;
            orbs = 110;

            //delete current ghosts
            for(int i = 0; i<ghosts.Length; i++) {
                ghosts[i] = null;
            }

            //repopulate orbs
            foreach (Button btn in btnArray) {
                if (btn.TabIndex == 71 || btn.TabIndex == 72 || btn.TabIndex == 73 || btn.TabIndex == 87 ||
                    btn.TabIndex == 120 || btn.TabIndex == 88 || btn.TabIndex == 89 || btn.TabIndex == 56) {
                    btn.Tag = "";
                }
                else if (btn.BackColor == Color.Black) {
                    if (btn.TabIndex == 45 || btn.TabIndex == 146 ||
                        btn.TabIndex == 182 || btn.TabIndex == 156) {
                        btn.Tag = "3";
                        btn.BackgroundImage = Properties.Resources.orb2;
                    }
                    else {
                        btn.Tag = "1";
                        btn.BackgroundImage = Properties.Resources.orb;
                    }
                }
            }

            //recreate ghosts
            ghosts[0] = new Ghost(71, 10);
            ghosts[1] = new Ghost(72, 0);
            ghosts[2] = new Ghost(73, 20);
            ghosts[3] = new Ghost(87, 30);
            ghosts[4] = new Ghost(88, 40);
            ghosts[5] = new Ghost(89, 50);

            //distinct labels
            for (int i = 0; i < ghosts.Length; i++) {
                switch (i) {
                    case 0:
                        btnArray[ghosts[i].getIndex()].Tag = "AI0";
                        break;
                    case 1:
                        btnArray[ghosts[i].getIndex()].Tag = "AI1";
                        break;
                    case 2:
                        btnArray[ghosts[i].getIndex()].Tag = "AI2";
                        break;
                    case 3:
                        btnArray[ghosts[i].getIndex()].Tag = "AI3";
                        break;
                    case 4:
                        btnArray[ghosts[i].getIndex()].Tag = "AI4";
                        break;
                    case 5:
                        btnArray[ghosts[i].getIndex()].Tag = "AI5";
                        break;
                }
            }
        }

        private void ConfirmUserInput_Click(object sender, EventArgs e) {
            String userInput = "";
            Regex regex = new Regex("[0-9]");
            if (NewHighScoreTextBox.Text != null) {
                userInput = NewHighScoreTextBox.Text;

                if (regex.IsMatch(userInput)) {
                    UserInputErrorLabel.Text = "Error: no numbers allowed";
                    UserInputErrorLabel.Visible = true;
                }
                else if (userInput.Contains(" ")) {
                    UserInputErrorLabel.Text = "Error: no spaces allowed";
                    UserInputErrorLabel.Visible = true;
                }
                else if (userInput.Length < 1) {
                    UserInputErrorLabel.Text = "Error: please enter a name";
                    UserInputErrorLabel.Visible = true;
                }
                else {
                    //add new highscore to list
                    highScores[4] = new Highscore(NewHighScoreTextBox.Text, score);

                    Array.Sort(highScores, Highscore.SortScoreAcending());

                    //close new highscore menu
                    NewHighscoreLabel.Visible = false;
                    NewHighscorePanel.Visible = false;
                    NewHighScoreTextBox.Visible = false;
                    ConfirmUserInput.Visible = false;

                    //populate highscore board
                    HighscoreName1.Text = highScores[0].getName();
                    HighscoreName2.Text = highScores[1].getName();
                    HighscoreName3.Text = highScores[2].getName();
                    HighscoreName4.Text = highScores[3].getName();
                    HighscoreName5.Text = highScores[4].getName();
                    Highscore1.Text = highScores[0].getScore().ToString();
                    Highscore2.Text = highScores[1].getScore().ToString();
                    Highscore3.Text = highScores[2].getScore().ToString();
                    Highscore4.Text = highScores[3].getScore().ToString();
                    Highscore5.Text = highScores[4].getScore().ToString();

                    //display highscore board
                    GameOverLabel.Visible = true;
                    HighscorePanel.Visible = true;
                    HighscorePanel.BringToFront();
                    HighscoreTable.Visible = true;
                    HighscoreLabel.Visible = true;
                    HighscoreName1.Visible = true;
                    HighscoreName2.Visible = true;
                    HighscoreName3.Visible = true;
                    HighscoreName4.Visible = true;
                    HighscoreName5.Visible = true;
                    Highscore1.Visible = true;
                    Highscore2.Visible = true;
                    Highscore3.Visible = true;
                    Highscore4.Visible = true;
                    Highscore5.Visible = true;

                    String[] temp = new string[5];

                    //write to file
                    for (int i = 0; i < 5; i++) {
                        temp[i] = highScores[i].getName() + " " + highScores[i].getScore().ToString();
                    }

                    File.WriteAllLines("C:\\Users\\" + Environment.UserName + "\\Desktop\\Pacman_Highscores.txt", temp);
                }
            }
        }

        /// Helper function, clears the error code message that displays when the user puts an invalid name for a new highscore.
        /// Makes communicating errors easier to understand / removes confusion for multiple bad entries
        private void NewHighScoreTextBox_TextChanged(object sender, EventArgs e) {
            UserInputErrorLabel.Visible = false;
        }
    }
}

//future updates / ideas
//database for highscore / non-local leaderboard
//new game board or even randomly generated game board? Logic will work as long as there is flow (no dead ends or split board)
