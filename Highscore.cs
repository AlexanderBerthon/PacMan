using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan {
    internal class Highscore : IComparable {
        String username;
        int score;

        public Highscore(String name, int score) {
            this.username = name;
            this.score = score;
        }

        public int getScore() {
            return score;
        }

        public String getName() {
            return username;
        }

        private class SortScoreAcendingHelper : IComparer {
            int IComparer.Compare(object a, object b) {
                Highscore s1 = (Highscore)a;
                Highscore s2 = (Highscore)b;

                if(s1.score < s2.score) {
                    return 1;
                }

                if(s1.score > s2.score) {
                    return -1;
                }

                else {
                    return 0;
                }
            }
        }

        int IComparable.CompareTo(object obj) {
            Highscore s = (Highscore)obj;
            return String.Compare(this.username, s.username);
        }

        public static IComparer SortScoreAcending() {
            return (IComparer)new SortScoreAcendingHelper();
        }

        /*
        public int CompareTo(Highscore other) {
            // Compares Height, Length, and Width.
            if (this.score.CompareTo(other.score) != 0) {
                return this.score.CompareTo(other.score);
            }
            else {
                return 0;
            }
        }
        */
    }
}
