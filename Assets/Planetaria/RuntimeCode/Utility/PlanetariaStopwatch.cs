using System;
using UnityEngine;

public class PlanetariaStopwatch : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (state != StopwatchState.Stopped)
        {
            fraction += Time.deltaTime;
            if (fraction >= 1f)
            {
                fraction -= 1f; // This technically wouldn't work if the frames per second of update was <1.
                seconds += 1;
                if (seconds >= seconds_in_minute)
                {
                    seconds -= seconds_in_minute; // this is equivalent to seconds = 0
                    minutes += 1;
                    if (minutes >= minutes_in_hour)
                    {
                        minutes -= minutes_in_hour; // this is equivalent to minutes = 0
                        hours += 1;
                        if (hours >= hours_in_day)
                        {
                            hours -= hours_in_day; // this is equivalent to hours = 0
                            days += 1;
                            if (days >= days_in_year)
                            {
                                days -= days_in_year; // this is equivalent to days = 0
                                years += 1;
                            }
                        }
                    }
                }
            }
        }
    }

    public override string ToString()
    {
        string result = "";
        if (years > 0)
        {
            result += (years.ToString() + "-");
        }
        if (days > 0)
        {
            if (years > 0)
            {
                // print all of the digits in 365, if the year is being printed.
                result += (days.ToString().PadLeft(3, '0') + " "); // pad leading zeros
            }
            else
            {
                result += (days.ToString() + " ");
            }
        }
        if (hours > 0)
        {
            if (years > 0 || days > 0)
            {
                // print all of the digits in 24, if the year or day is being printed.
                result += (hours.ToString().PadLeft(2, '0') + ":");
            }
            else
            {
                result += (hours.ToString() + ":");
            }
        }
        if (minutes > 0)
        {
            if (years > 0 || days > 0 || hours > 0)
            {
                // print all of the digits in 60, if the year or day or hour is being printed.
                result += (minutes.ToString().PadLeft(2, '0') + ":");
            }
            else
            {
                result += (minutes.ToString() + ":");
            }
        }
        if (seconds >= 0) // always happens (unless, a bug exists that makes seconds negative).
        {
            if (years > 0 || days > 0 || hours > 0 || minutes > 0)
            {
                // print all of the digits in 60, if the year or day or hour or minutes is being printed.
                result += (seconds.ToString().PadLeft(2, '0') + ".");
            }
            else
            {
                result += (seconds.ToString().PadLeft(1,'0') + "."); // at the bare minimum, if you want to display something it should look like 0.00
            }
        }
        if (fraction >= 0) // always happens (unless, a bug exists that makes fraction negative).
        {
            int hundredths = Mathf.FloorToInt(fraction * 100);
            result += hundredths.ToString().PadRight(2, '0'); // this is the only time we pad on the right with zeros (trailing zeros).
        }
        return result;
    }

    private enum StopwatchState { Stopped = 0, Started = 1 }

    [NonSerialized] [HideInInspector] private StopwatchState state = StopwatchState.Stopped;

    [NonSerialized] [HideInInspector] private float fraction = 0;
    [NonSerialized] [HideInInspector] private int seconds = 0;
    [NonSerialized] [HideInInspector] private int minutes = 0;
    [NonSerialized] [HideInInspector] private int hours = 0;
    [NonSerialized] [HideInInspector] private int days = 0;
    [NonSerialized] [HideInInspector] private uint years = 0; // I'm being pedantic here for defined-behavior uint overflow (only an AI could play this for a year, much less 2^32 years, but hey, it's not quite the heat death of the universe).

    private const int seconds_in_minute = 60;
    private const int minutes_in_hour = 60;
    private const int hours_in_day = 24;
    private const int days_in_year = 365;
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