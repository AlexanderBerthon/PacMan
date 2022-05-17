namespace PacMan {

    /* BUGS
     * Pacman flashes when stopped, hits a wall, etc. 
     * ghost collision is kind of necessary.. otherwise they like stack up and dissapear and it looks janky
     * especially at the start
     * 
     * when ghosts overlap, orb gets destroyed.
     *      
     * went into stuck
     * 1 tick no movement
     * next tick moved to open spot but left nothing
     *
    */

    public partial class Form1 : Form {
        //global variables :(
        int currentIndex;
        int trajectory;
        int score;
        Ghost[] ghosts;
        Button[] btnArray;
        Boolean animation = true;

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer AnimationTimer = new System.Windows.Forms.Timer();


        /// cells with borders are walls, if player collides with a wall, stop, no movement but game continues
        /// empty cells get an orb
        /// player moves cell to cell, clearing orbs, earning points
        /// every cell the player moves to gets background overriden with player pic, then empty when they leave.
        /// every cell the enemy moves to gets background overriden with enemy pic, then replaced when they leave.

        public Form1() {
            InitializeComponent();

            timer.Interval = 300;
            timer.Tick += new EventHandler(TimerEventProcessor);

            AnimationTimer.Interval = 150;
            AnimationTimer.Tick += new EventHandler(TimerEventProcessor2);

            score = 0;

            currentIndex = 121;
            trajectory = 0;
            btnArray = new Button[256];
            flowLayoutPanel1.Controls.CopyTo(btnArray, 0);

            ghosts = new Ghost[6]; //[6] = number of ghosts

            ghosts[0] = new Ghost(71, 10); //start index, delay(# of game tick cycles)
            ghosts[1] = new Ghost(72, 0);
            ghosts[2] = new Ghost(73, 20);
            ghosts[3] = new Ghost(87, 30);
            ghosts[4] = new Ghost(88, 40);
            ghosts[5] = new Ghost(89, 50);

            for (int i = 0; i < ghosts.Length; i++) {
                btnArray[ghosts[i].getIndex()].Tag = "AI 0";
            }


            timer.Start();
            AnimationTimer.Start();
        }


        //movement clock
        private void TimerEventProcessor(Object anObject, EventArgs eventArgs) {
            if (trajectory != 0) {
                btnArray[currentIndex].BackgroundImage = null;
                btnArray[currentIndex].Tag = "0"; //""
            }
            move();
            AIMove();
            //testing pls remove after
            /*
            foreach(Button btn in btnArray){
                if (btn.Tag == "AI") {
                    btn.BackColor = Color.Firebrick;
                }
                else if(btn.BackColor == Color.DarkSlateBlue){
                    //don't do anything
                }
                else {
                    btn.BackColor = Color.Black;
                }
            }
            */
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
            /*
            //Random random = new Random();
            for (int i = 0; i < ghosts.Length; i++) {
                if (animation){
                    btnArray[ghosts[i].getIndex()].BackgroundImage = Properties.Resources.Ghost1;
                }
                else {
                    btnArray[ghosts[i].getIndex()].BackgroundImage = Properties.Resources.Ghost2;
                }
            }
            */
        }



        private void AIMove() {
            //every tick, calculate all valid moves
            //randomly select a move from the pool (3 max choices, often only 1 choice)
            Random random = new Random();
            List<int> validMoves = new List<int>();
            Boolean replaceOrb = false;

            for (int i = 0; i < ghosts.Length; i++) {
                if (ghosts[i].getDelay() == 0) {
                    if (btnArray[ghosts[i].getIndex()].Tag.ToString().Contains("1")) {
                        btnArray[ghosts[i].getIndex()].BackgroundImage = Properties.Resources.orb;
                        btnArray[ghosts[i].getIndex()].Tag = "1";
                    }
                    else {
                        btnArray[ghosts[i].getIndex()].Tag = "0";
                        btnArray[ghosts[i].getIndex()].BackgroundImage = null;
                    }

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

                    if (validMoves.Count > 0) {
                        int choice = random.Next(0, validMoves.Count);
                        if(btnArray[ghosts[i].getIndex() + validMoves[choice]].Tag == "1") {
                            replaceOrb = true;
                        }
                        ghosts[i].update(validMoves[choice]);
                    }
                    else {
                        ghosts[i].stuck(true);
                    }
                }
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
                if (replaceOrb) {
                    btnArray[ghosts[i].getIndex()].Tag = "AI 1";
                }
                else if (ghosts[i].stuck()) {
                    if (btnArray[ghosts[i].getIndex()].Tag.ToString().Contains("1")){
                        btnArray[ghosts[i].getIndex()].Tag = "AI 1"; //might be a problem..
                    }
                    else {
                        btnArray[ghosts[i].getIndex()].Tag = "AI 0"; //might be a problem..
                    }
                }
                else {
                    btnArray[ghosts[i].getIndex()].Tag = "AI 0";
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

        private Boolean move() {
            Boolean runGame = true;

            if (btnArray[currentIndex + trajectory].BackColor == Color.DarkSlateBlue) {
                trajectory = 0;
            }
            else if (btnArray[currentIndex].Tag.ToString().Contains("AI")) {
                //enemy collision
                //runGame = false;
                //exitbutton.Visible = true;
                //Gameoverlabel.Visible = true;
                //continuebutton.Visible = true;
                //Playagainlabel.Visible = true;
                //timer.Stop();
                //AnimationTimer.Stop();
            }
            else {
                currentIndex += trajectory;
                //if point gathered
                if (btnArray[currentIndex].Tag == "1") {
                    score++;
                    ScoreLabel.Text = score.ToString();
                }
                btnArray[currentIndex].Tag = "Player";
            }

            return runGame;
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
            Gameoverlabel.Visible = false;
            continuebutton.Visible = false;
            exitbutton.Visible = false;
            Playagainlabel.Visible = false;
        }

        private void exitbutton_Click(object sender, EventArgs e) {
            Application.Exit();
        }
    }
}