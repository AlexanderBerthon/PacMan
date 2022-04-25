namespace PacMan {

    /* BUGS
     * Pacman flashes when stopped, hits a wall, etc. 
     * 
    */

    public partial class Form1 : Form {
        //global variables :(
        int currentIndex;
        int trajectory;
        int score; 
        Random random = new Random();
        Button[] btnArray;
        Boolean animation = true;

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer AnimationTimer = new System.Windows.Forms.Timer();


        /// cells with borders are walls, if player collides with a wall, stop, no movement but game continues
        /// empty cells get an orb
        /// player moves cell to cell, clearing orbs, earning points
        /// every cell the player moves to gets background overriden with player pic, then empty when they leave.
        /// every cell the enemy moves to gets background overriden with enemy pic, then replaced when they leave.


        private void TimerEventProcessor(Object anObject, EventArgs eventArgs) {
            if (trajectory != 0) {
                btnArray[currentIndex].BackgroundImage = null;
                btnArray[currentIndex].Tag = "";
            }
            move();
        }

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
            timer.Start();
            timer.Tick += new EventHandler(TimerEventProcessor);

            AnimationTimer.Interval = 150;
            AnimationTimer.Start();
            AnimationTimer.Tick += new EventHandler(TimerEventProcessor2);

            score = 0;

            currentIndex = 121;
            trajectory = 0;
            btnArray = new Button[256];
            flowLayoutPanel1.Controls.CopyTo(btnArray, 0);

        }

        private Boolean move() {
            Boolean runGame = true;

            if (btnArray[currentIndex + trajectory].BackColor == Color.DarkSlateBlue) {
                trajectory = 0;
            }
            else if (btnArray[currentIndex].Tag == "AI") {
                //enemy collision
                runGame = false;
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

        private void ExitButton_Click(object sender, EventArgs e) {
            Application.Exit();
        }

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