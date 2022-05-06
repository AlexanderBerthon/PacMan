using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan {
    internal class Ghost {
        int index;
        int trajectory;
        int delay;
        Boolean orb;
        Boolean isStuck;

        public Ghost(int startIndex, int startDelay) {
            index = startIndex;
            delay = startDelay;
            isStuck = false;
            orb = false;
            trajectory = -16;
        }

        public void update(int trajectory) {
            this.trajectory = trajectory;
            index += trajectory;
        }

        public Boolean replaceOrb() {
            return orb;
        }

        public void replaceOrb(Boolean status) {
            orb = status;
        }

        //should never go backwards
        //should calculate every move possible and randomly choose between them, minus backwards movement. 
        //every time this is called?
        //might need to change logic in main form as well.

        public void changeTrajectory() {
            Random rng = new Random();

            //calculate new trajectory? is this possible? what if 2 walls? move off screen?
            int random = rng.Next(0, 2);
            trajectory = Math.Abs(trajectory);
            switch (trajectory) {
                case 1:
                    if(random == 0) {
                        trajectory = -16;
                    }
                    else {
                        trajectory = 16;
                    }
                    break;
                case 16:
                    if (random == 0) {
                        trajectory = -1;
                    }
                    else {
                        trajectory = 1;
                    }
                    break;
            }
        }

        public int getTrajectory() {
            return trajectory;
        }

        public int getIndex() {
            return index;
        }

        public int getDelay() {
            return delay;
        }

        public void reduceDelay() {
            if (delay > 0) {
                delay--;
            }
        }

        public Boolean stuck() {
            return isStuck;
        }

        public void stuck(Boolean status) {
            isStuck = status;
        }


    }


}
