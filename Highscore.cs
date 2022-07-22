using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan {
    internal class Highscore{
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

        public static IComparer SortScoreAcending() {
            return (IComparer)new SortScoreAcendingHelper();
        }
    }
}
