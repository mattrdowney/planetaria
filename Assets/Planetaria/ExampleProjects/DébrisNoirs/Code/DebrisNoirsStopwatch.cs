using System;
using UnityEngine;
using UnityEngine.UI;
using Planetaria;

namespace DebrisNoirs
{
    public class DebrisNoirsStopwatch : PlanetariaMonoBehaviour
    {
        public void reset_clock()
        {
            time = new DebrisNoirsTime();
        }

        public void start_clock()
        {
            state = StopwatchState.Started;
            
            player_score.rectTransform.anchoredPosition = new Vector3(0, 30, 0);
            high_score.rectTransform.anchoredPosition = new Vector3(0, -30, 0);
        }

        public void stop_clock()
        {
            state = StopwatchState.Stopped;
            if (time > best_time) // General commentary. Scores in this game are sort of bound by battery life, which is amusing.
            {
                time.save("high_score");
                high_score.text = time.ToString();
                best_time = time;
            }

            player_score.rectTransform.anchoredPosition = new Vector3(0, 300, 0);
            high_score.rectTransform.anchoredPosition = new Vector3(0, -300, 0);
            high_score_display_time = 10f;
        }

        private void OnValidate()
        {
            player_score = this.gameObject.internal_game_object.transform.Find("PlayerScore").GetComponent<Text>();
            high_score = this.gameObject.internal_game_object.transform.Find("HighScore").GetComponent<Text>();
            time = new DebrisNoirsTime();
        }

        private void Start()
        {
            best_time = new DebrisNoirsTime("high_score");
            high_score.text = best_time.ToString();
        }

        // Update is called once per frame
        private void Update()
        {
            if (state != StopwatchState.Stopped)
            {
                time.increment(Time.deltaTime);
            }
            player_score.text = time.ToString();
            if (high_score_display_time > 0)
            {
                high_score_display_time -= Time.deltaTime;
                if (high_score_display_time <= 0)
                {
                    player_score.rectTransform.anchoredPosition = new Vector3(0, 30, 0);
                    high_score.rectTransform.anchoredPosition = new Vector3(0, -30, 0);
                }
            }
        }

        protected override void OnConstruction() { }
        protected override void OnDestruction() { }

        private enum StopwatchState { Stopped = 0, Started = 1 }

        [SerializeField] [HideInInspector] private Text player_score;
        [SerializeField] [HideInInspector] private Text high_score;

        [NonSerialized] [HideInInspector] private StopwatchState state = StopwatchState.Started;
        [NonSerialized] [HideInInspector] DebrisNoirsTime time;
        [NonSerialized] [HideInInspector] DebrisNoirsTime best_time;
        [NonSerialized] [HideInInspector] float high_score_display_time;
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