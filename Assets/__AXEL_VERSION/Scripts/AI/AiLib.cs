using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLib
{
    public class Memory
    {
        public string id;
        public int turn;

        public List<Moment> moments;

        public float strength;
        
        int goodMoments;
        int badMoments;

        /// <summary>
        /// A memory is a way to save a moment in what AI was placed in front of. Useful to get something to make the AI learns something.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="turn"></param>
        public Memory(string customId, int customTurn)
        {
            id = customId;
            turn = customTurn;
        }

        /// <summary>
        /// Allows to calculate strength of a memory. Useful to define what what the best learning of the AI.
        /// </summary>
        /// <param name="moments"></param>
        /// <returns></returns>
        public int CalculateTotalStrength(Moment[] moments)
        {
            for(int i = 0; i < moments.Length; i++)
            {
                if(moments[i].momentGenre == Moment.MomentGenre.Good)
                {
                    goodMoments++;
                }
                if(moments[i].momentGenre == Moment.MomentGenre.Bad)
                {
                    badMoments++;
                }
            }

            int totalMemoryStrength = goodMoments - badMoments;
            return totalMemoryStrength;
        }
    }

    public class Moment
    {
        public enum MomentGenre { Neutral, Good, Bad }
        public MomentGenre momentGenre;

        /// <summary>
        /// Define the specifical moment
        /// </summary>
        /// <param name="moment"></param>
        public Moment(MomentGenre moment)
        {
            momentGenre = moment;
        }
    }
}
