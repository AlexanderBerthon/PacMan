using System;

namespace PacMan {

    internal class HighscoreComp : IComparer<Highscore> {
        public int Compare(Highscore x, Highscore y) {
            if (x.getScore().CompareTo(y.getScore()) != 0) {
                return x.getScore().CompareTo(y.getScore());
            }
            else {
                return 0;
            }
        }
    }
}