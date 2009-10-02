using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Threading;

namespace MCDB2BIN
{
    public sealed class PerformanceTimer
    {
        [DllImport("KERNEL32")]
        private static extern bool QueryPerformanceCounter(out long pCount);

        [DllImport("KERNEL32")]
        private static extern bool QueryPerformanceFrequency(out long pFrequency);

        private bool mActive = false;
        private long mStarted = 0;
        private long mStopped = 0;
        private long mFrequency = 0;
        private double mDuration = 0;

        public PerformanceTimer()
        {
            if (QueryPerformanceFrequency(out mFrequency) == false) throw new Win32Exception();
        }

        public void Reset() { mDuration = 0; }

        public void Unpause()
        {
            if (!mActive)
            {
                mActive = true;
                QueryPerformanceCounter(out mStarted);
            }
        }

        public void Pause()
        {
            if (mActive)
            {
                QueryPerformanceCounter(out mStopped);
                mActive = false;
                mDuration += (double)(mStopped - mStarted) / (double)mFrequency;
            }
        }

        public double Duration { get { return mDuration; } }
    }
}
