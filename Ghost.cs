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
        Boolean isStuck;

        public Ghost(int startIndex, int startDelay) {
            index = startIndex;
            delay = startDelay;
            isStuck = false;
            trajectory = -16;
        }

        public void update(int trajectory) {
            this.trajectory = trajectory;
            index += trajectory;
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
