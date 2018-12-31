using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Planetaria;

namespace DebrisNoirs
{
    public class ScoreKeeper : PlanetariaMonoBehaviour
    {
        public void reset_clock()
        {
            score = 0;
        }

        public void start_clock()
        {
            state = StopwatchState.Started;
            
            player_score_text.rectTransform.anchoredPosition = new Vector3(0, 30, 0);
            highscore_text.rectTransform.anchoredPosition = new Vector3(0, -30, 0);
        }

        public void stop_clock()
        {
            state = StopwatchState.Stopped;
            if (score > highscore) // General commentary. Scores in this game are sort of bound by battery life, which is amusing.
            {
                PlayerPrefs.SetInt("highscore", score);
                highscore_text.text = score.ToString();
                highscore = score;
            }

            player_score_text.rectTransform.anchoredPosition = new Vector3(0, 300, 0);
            highscore_text.rectTransform.anchoredPosition = new Vector3(0, -300, 0);
            score_display_time = 10f;
        }

        private void OnValidate()
        {
            player_score_text = this.gameObject.internal_game_object.transform.Find("PlayerScore").GetComponent<Text>();
            highscore_text = this.gameObject.internal_game_object.transform.Find("HighScore").GetComponent<Text>();
            score = 0;
        }

        private void Start()
        {
            highscore = 0;
            if (PlayerPrefs.HasKey("highscore"))
            {
                highscore = PlayerPrefs.GetInt("highscore");
            }
            highscore_text.text = highscore.ToString();
        }

        public void add(int points)
        {
            if (state == StopwatchState.Started) // if the player is still alive
            {
                score += points; // add the points of the debris (like 10, 20, or 30)
                player_score_text.text = score.ToString(); // print only when something changes
                if (score > highscore) // if there is a new highscore
                {
                    highscore = score; // set the new highscore
                    highscore_text.text = highscore.ToString(); // and print it on screen
                    last_autosave += Time.deltaTime;
                    if (last_autosave >= 0) // TODO: see if this needs to be slower
                    {
                        PlayerPrefs.SetInt("highscore", score); // save the score (e.g. in case the user's battery dies)
                        last_autosave = 0;
                    }
                }
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (score_display_time > 0)
            {
                score_display_time -= Time.deltaTime;
                if (score_display_time <= 0)
                {
                    player_score_text.rectTransform.anchoredPosition = new Vector3(0, 30, 0);
                    highscore_text.rectTransform.anchoredPosition = new Vector3(0, -30, 0);
                }
            }
        }

        protected override void OnConstruction() { }
        protected override void OnDestruction() { }

        private enum StopwatchState { Stopped = 0, Started = 1 }

        [SerializeField] [HideInInspector] private Text player_score_text;
        [SerializeField] [HideInInspector] private Text highscore_text;

        [NonSerialized] [HideInInspector] private StopwatchState state = StopwatchState.Started;
        [NonSerialized] [HideInInspector] int score;
        [NonSerialized] [HideInInspector] int highscore;
        [NonSerialized] [HideInInspector] float score_display_time;
        [NonSerialized] [HideInInspector] float last_autosave = 0;
    }
}

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.