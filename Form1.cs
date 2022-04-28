namespace PacMan {

    /* BUGS
     * Pacman flashes when stopped, hits a wall, etc. 
     * No Idle animation 
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


        //movement clock
        private void TimerEventProcessor(Object anObject, EventArgs eventArgs) {
            if (trajectory != 0) {
                btnArray[currentIndex].BackgroundImage = null;
                btnArray[currentIndex].Tag = "";
            }
            move();
            AIMove();
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
                }
                animation = false;
            }
            else {
                btnArray[currentIndex].BackgroundImage = Properties.Resources.Closed;
                animation = true;
            }
        }

        public Form1() {
            InitializeComponent();

            timer.Interval = 300;
            //timer.Start();
            timer.Tick += new EventHandler(TimerEventProcessor);

            AnimationTimer.Interval = 150;
            //AnimationTimer.Start();
            AnimationTimer.Tick += new EventHandler(TimerEventProcessor2);

            score = 0;

            currentIndex = 121;
            trajectory = 0;
            btnArray = new Button[256];
            flowLayoutPanel1.Controls.CopyTo(btnArray, 0);

            ghosts = new Ghost[2]; //[6]

            ghosts[0] = new Ghost(71);
            ghosts[1] = new Ghost(72);
            //ghosts[2] = new Ghost(73);
            //ghosts[3] = new Ghost(87);
            //ghosts[4] = new Ghost(88);
            //ghosts[5] = new Ghost(89);
            timer.Start();
            AnimationTimer.Start();
        }

        private void AIMove() {
            Boolean check = false;

            //every tick, calculate all valid moves
            //randomly select a move from the pool (3 max choices, often only 1 choice)

            //error because length is 6, but not filled. change temporarily while testing
            for (int i = 0; i<ghosts.Length; i++){
                Random random = new Random();
                List<int> validMoves = new List<int>();

                btnArray[ghosts[i].getIndex()].BackgroundImage = null;
                btnArray[ghosts[i].getIndex()].Tag = "";

                if (btnArray[ghosts[i].getIndex() + 1].BackColor != Color.DarkSlateBlue && ghosts[i].getTrajectory() != -1) {
                    validMoves.Add(1);
                }
                if (btnArray[ghosts[i].getIndex() - 1].BackColor != Color.DarkSlateBlue && ghosts[i].getTrajectory() != +1) {
                    validMoves.Add(-1);
                }
                if (btnArray[ghosts[i].getIndex() + 16].BackColor != Color.DarkSlateBlue && ghosts[i].getTrajectory() != -16) {
                    validMoves.Add(16);
                }
                if (btnArray[ghosts[i].getIndex() - 16].BackColor != Color.DarkSlateBlue && ghosts[i].getTrajectory() != +16) {
                    validMoves.Add(-16);
                }

                ghosts[i].update(validMoves[random.Next(0, validMoves.Count)]);
                btnArray[ghosts[i].getIndex()].BackgroundImage = Properties.Resources.Icon;
                //validMoves.Clear(); //clears automatically since it goes out of scope?
            }
        }

        private Boolean move() {
            Boolean runGame = true;

            if (btnArray[currentIndex + trajectory].BackColor == Color.DarkSlateBlue) {
                trajectory = 0;
            }
            else if (btnArray[currentIndex].Tag == "AI") {
                //enemy collision
                runGame = false;
                Application.Exit();
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

        //placeholder / old code
        private void ExitButton_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        //placeholder / old code
        private void RestartButton_Click(object sender, EventArgs e) {
            //GameOverPanel.Visible = false;
            foreach (Button btn in btnArray) {
                btn.BackColor = Color.PeachPuff;
            }
            trajectory = 16;
            ScoreLabel.Text = "0";
            currentIndex = 18;

            timer.Start();

        }
    }
}