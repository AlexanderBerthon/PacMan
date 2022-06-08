namespace PacMan {

    /* BUGS
     * AI-Player collision is a little janky still, 1 time I was able to squeak through an AI and live
     * 
     * Player pac man animation flashes some times still, not easy on the eyes
     * 
     * Still some issues with collision, but much better
     * Collision occurs, but the ghost just keeps going, like it decides to move instead of deleting/recreating. I get points though so why isn't it deleting properly?
     * What is weird is that is works perfectly sometimes and then othertimes ignores logic
     * 
     *
     *
     * TAGS (getting complicated so making a key)
     * AI1 - AI + distinct identifier + " " + orb/no orb
     * e.g. "AI1 orb" is ghost one and needs orb replacement
     * 
     * Player = the player / pacman
     * 
     * 0 = empty space / no orb
     * 1 = full space / orb
     * 3 = special orb / power up
     *
     *
     *
    */

    public partial class Form1 : Form {
        //global variables :(
        int currentIndex;

        Boolean skip = true;

        int trajectory;
        int score;
        int orbs;
        Ghost[] ghosts;
        Button[] btnArray;
        Boolean animation = true;
        int AIVulnerable;

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer animationTimer = new System.Windows.Forms.Timer();

        /// cells with borders are walls, if player collides with a wall, stop, no movement but game continues
        /// empty cells get an orb
        /// player moves cell to cell, clearing orbs, earning points
        /// every cell the player moves to gets background overriden with player pic, then empty when they leave.
        /// every cell the enemy moves to gets background overriden with enemy pic, then replaced when they leave.

        public Form1() {
            InitializeComponent();

            timer.Interval = 300;
            timer.Tick += new EventHandler(TimerEventProcessor);

            animationTimer.Interval = 150;
            animationTimer.Tick += new EventHandler(TimerEventProcessor2);

            score = 0;
            orbs = 110; //might be off / add orbs-- when collision happens 

            AIVulnerable = 0;

            currentIndex = 120;
            trajectory = 0;
            btnArray = new Button[256];
            flowLayoutPanel1.Controls.CopyTo(btnArray, 0);

            ghosts = new Ghost[6]; //[6] = number of ghosts

            ghosts[0] = new Ghost(71, 30); //start index, delay(# of game tick cycles)
            ghosts[1] = new Ghost(72, 0);
            ghosts[2] = new Ghost(73, 60);
            ghosts[3] = new Ghost(87, 90);
            ghosts[4] = new Ghost(88, 120);
            ghosts[5] = new Ghost(89, 150);

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

            //testing
            Gameoverlabel.Visible = true;

            timer.Start();
            animationTimer.Start();
        }


        //movement clock
        private void TimerEventProcessor(Object anObject, EventArgs eventArgs) {
            
            if (trajectory != 0) {
                btnArray[currentIndex].BackgroundImage = null;
                btnArray[currentIndex].Tag = "0"; //""
            }
            move();
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
        }

        private void AIMove() {
            //every tick, calculate all valid moves
            //randomly select a move from the pool (3 max choices, often only 1 choice)
            Random random = new Random();
            List<int> validMoves = new List<int>();
            Boolean replaceOrb = false;

            for (int i = 0; i < ghosts.Length; i++) {
                //logic
                if (ghosts[i].getDelay() == 0) {
                    //replace orb if needed
                    if (btnArray[ghosts[i].getIndex()].Tag.ToString().Contains("orb")) {
                        btnArray[ghosts[i].getIndex()].BackgroundImage = Properties.Resources.orb;
                        btnArray[ghosts[i].getIndex()].Tag = "1";
                    }
                    else {
                        btnArray[ghosts[i].getIndex()].Tag = "0";
                        btnArray[ghosts[i].getIndex()].BackgroundImage = null;
                    }

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

                    //unstuck if needed
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

                    //move
                    if (validMoves.Count > 0) {
                        int choice = random.Next(0, validMoves.Count);

                        if (btnArray[ghosts[i].getIndex()].Tag == "Player" || btnArray[ghosts[i].getIndex() + validMoves[choice]].Tag == "Player") {
                            Gameoverlabel.Text = "AI Capture"; //testing
                            if (AIVulnerable > 0) {
                                destroyGhost(i);
                                validMoves.Clear(); //testing
                                score += 25;
                            }
                            else {
                                //enemy collision
                                exitbutton.Visible = true;
                                //Gameoverlabel.Visible = true;
                                continuebutton.Visible = true;
                                Playagainlabel.Visible = true;
                                timer.Stop();
                                animationTimer.Stop();
                            }
                        }
                        else {
                            ghosts[i].update(validMoves[choice]);
                        }
                    }
                    else {
                        ghosts[i].stuck(true);
                    }
                }
                //animation
                if (AIVulnerable > 0) {
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
                    btnArray[ghosts[i].getIndex()].BackgroundImage = Properties.Resources.Ghost7;
                }
                else {
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

                if (replaceOrb) {
                    btnArray[ghosts[i].getIndex()].Tag += " orb";
                }
                else if (ghosts[i].stuck()) {
                    if (btnArray[ghosts[i].getIndex()].Tag.ToString().Contains("orb")){
                        btnArray[ghosts[i].getIndex()].Tag += " orb";
                    }
                    else {
                        //do nothing
                        //btnArray[ghosts[i].getIndex()].Tag = "AI 0";
                    }
                }
                else {
                    //do nothing
                    //btnArray[ghosts[i].getIndex()].Tag = "AI 0";
                }

                validMoves.Clear();
                replaceOrb = false;
            }

            for (int i = 0; i < ghosts.Length; i++) {
                if(ghosts[i].getDelay() > 0){
                    ghosts[i].reduceDelay();
                }
            }
        }

        private void move() {
            if (btnArray[currentIndex + trajectory].BackColor == Color.DarkSlateBlue) {
                trajectory = 0;
            }

            
            else if (btnArray[currentIndex + trajectory].Tag.ToString().Contains("AI")) {
                Gameoverlabel.Text = "Player+Traj Collison";
                if (AIVulnerable > 0) {
                    //ghost capture
                    destroyGhost(int.Parse(btnArray[currentIndex + trajectory].Tag.ToString().Substring(2,1))); //this is disgusting
                    score += 200;
                }
                else {
                    //enemy collision
                    endGame();
                }
            }

            
            else {
                currentIndex += trajectory;
                //if point gathered
                if(btnArray[currentIndex].Tag == "1") {
                    score+=1;
                    orbs--;
                    ScoreLabel.Text = score.ToString();
                    if (orbs == 0) {
                        nextStage(); //might be a problem to leave this here
                    }
                }
                else if(btnArray[currentIndex].Tag == "3") {
                    AIVulnerable = 10;
                }
                //btnArray[currentIndex].Tag = "Player";
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

        private void continuebutton_Click(object sender, EventArgs e) {
            //Gameoverlabel.Visible = false;

    
            Gameoverlabel.Text = ""; //test

            continuebutton.Visible = false;
            exitbutton.Visible = false;
            Playagainlabel.Visible = false;
               
            score = 0;
            orbs = 110;
            currentIndex = 121;
            trajectory = 0;

            //delete current ghosts
            for(int i = 0; i<ghosts.Length; i++) {
                ghosts[i] = null;
            }

            foreach(Button btn in btnArray) {
                if(btn.TabIndex == 71 || btn.TabIndex == 72 || btn.TabIndex == 73 || btn.TabIndex == 87 ||
                    btn.TabIndex == 87 || btn.TabIndex == 88 || btn.TabIndex == 89 || btn.TabIndex == 56) {
                    btn.Tag = "";
                }
                else if (btn.BackColor == Color.Black) {
                    if (btn.TabIndex == 45 || btn.TabIndex == 51 || btn.TabIndex == 146 ||
                        btn.TabIndex == 182 || btn.TabIndex == 202 || btn.TabIndex == 156) {
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
            ghosts[0] = new Ghost(71, 30); //start index, delay(# of game tick cycles)
            ghosts[1] = new Ghost(72, 0);
            ghosts[2] = new Ghost(73, 60);
            ghosts[3] = new Ghost(87, 90);
            ghosts[4] = new Ghost(88, 120);
            ghosts[5] = new Ghost(89, 150);

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

            timer.Start();
            animationTimer.Start();
        }

        private void exitbutton_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        private void endGame() {
            exitbutton.Visible = true;
            //Gameoverlabel.Visible = true;
            continuebutton.Visible = true;
            Playagainlabel.Visible = true;
            timer.Stop();
            animationTimer.Stop();
        }

        private void destroyGhost(int index) {
            btnArray[ghosts[index].getIndex()].BackgroundImage = null;
            btnArray[ghosts[index].getIndex()].Tag = "";
            ghosts[index] = null;
            switch (index) {
                case 0:
                    ghosts[0] = new Ghost(71, 30); //use this one if the delay works?
                    break;
                case 1:
                    ghosts[1] = new Ghost(72, 0);
                    break;
                case 2:
                    ghosts[2] = new Ghost(73, 60);
                    break;
                case 3:
                    ghosts[3] = new Ghost(87, 90);
                    break;
                case 4:
                    ghosts[4] = new Ghost(88, 120);
                    break;
                case 5:
                    ghosts[5] = new Ghost(89, 150);
                    break;
            }
        }

        private void nextStage() {
            currentIndex = 121;
            trajectory = 0;
            orbs = 110;

            //delete current ghosts
            for(int i = 0; i<ghosts.Length; i++) {
                ghosts[i] = null;
            }

            //repopulate orbs
            foreach (Button btn in btnArray) {
                if (btn.TabIndex == 71 || btn.TabIndex == 72 || btn.TabIndex == 73 || btn.TabIndex == 87 ||
                    btn.TabIndex == 87 || btn.TabIndex == 88 || btn.TabIndex == 89 || btn.TabIndex == 56) {
                    btn.Tag = "";
                }
                else if (btn.BackColor == Color.Black) {
                    if (btn.TabIndex == 45 || btn.TabIndex == 51 || btn.TabIndex == 146 ||
                        btn.TabIndex == 182 || btn.TabIndex == 202 || btn.TabIndex == 156) {
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
            ghosts[0] = new Ghost(71, 30);
            ghosts[1] = new Ghost(72, 0);
            ghosts[2] = new Ghost(73, 60);
            ghosts[3] = new Ghost(87, 90);
            ghosts[4] = new Ghost(88, 120);
            ghosts[5] = new Ghost(89, 150);

            //tag ghosts
            for (int i = 0; i < ghosts.Count(); i++) {
                btnArray[ghosts[i].getIndex()].Tag = "AI 0";
            }

            timer.Start();
            animationTimer.Start();
        }

    }
}