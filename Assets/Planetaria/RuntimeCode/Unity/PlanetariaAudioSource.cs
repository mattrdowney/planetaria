using System.Collections.Generic;
using UnityEngine;

/*
namespace Planetaria
{
    public sealed class PlanetariaAudioSource : MonoBehaviour // TODO: instead of differences being temporal, the AudioSource should provide sound spatialization.
    {
        private void Awake()
        {
            audio_source = this.GetOrAddComponent<AudioSource>();
            audio_source.rolloffMode = AudioRolloffMode.Logarithmic;
            if (!audio_listener.exists)
            {
                audio_listener = Transform.FindObjectOfType<AudioListener>().transform;
            }
        }

        public AudioClip clip
        {
            get
            {
                return audio_source.clip;
            }
            set
            {
                audio_source.clip = value;
            }
        }

        public float volume
        {
            get
            {
                return audio_source.volume;
            }
            set
            {
                audio_source.volume = value;
            }
        }

        public float time
        {
            get
            {
                return internal_time;
            }
            set
            {
                internal_time = Time.time + value;
                if (value >= delay)
                {
                    audio_source.time = value;
                }
                //Debug.Assert()
            }
        }
        
        private float delay;
        private float internal_time;
        private AudioSource audio_source;

        private static optional<Transform> audio_listener;
        private static SortedList<float, PlanetariaAudioSource> sounds;
    }
}
*/

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
