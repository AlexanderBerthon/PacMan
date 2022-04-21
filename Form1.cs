namespace PacMan {
    public partial class Form1 : Form {
        //global variables :(
        int currentIndex;
        int trajectory;
        int score; 
        Random random = new Random();
        Button[] btnArray;

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        /// cells with borders are walls, if player collides with a wall, stop, no movement but game continues
        /// empty cells get an orb
        /// player moves cell to cell, clearing orbs, earning points
        /// every cell the player moves to gets background overriden with player pic, then empty when they leave.
        /// every cell the enemy moves to gets background overriden with enemy pic, then replaced when they leave.


        private void TimerEventProcessor(Object anObject, EventArgs eventArgs) {
            //reset event clock
            timer.Stop();
            if (move()) {
                timer.Start();
            }
            else {
                ScoreLabel.Text = "Final Score: " + score;
                //GameOverPanel.Visible = true;
            }
        }

        public Form1() {
            InitializeComponent();

            timer.Interval = 250;
            timer.Start();
            timer.Tick += new EventHandler(TimerEventProcessor);

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
                btnArray[currentIndex].BackColor = Color.Black; //don't need once you convert to image
                currentIndex += trajectory;

                //if point gathered
                if (btnArray[currentIndex].Tag == "1") {
                    btnArray[currentIndex].BackgroundImage = null;
                    btnArray[currentIndex].Tag = "0";
                    score++;
                    ScoreLabel.Text = score.ToString();
                    this.Refresh();
                }
                btnArray[currentIndex].Tag = "Player";
                //placeholder, convert to pacman later
                btnArray[currentIndex].BackColor = Color.Yellow;

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

        private void button4_Click(object sender, EventArgs e) {

        }
    }
}