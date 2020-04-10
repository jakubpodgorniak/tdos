using System.Collections.Generic;
using System.Linq;

namespace TDOS.MG.Utils
{
    public class FrameRateCounter
    {
        public FrameRateCounter(int bufferSize)
        {
            this.bufferSize = bufferSize;
            deltaTimesBuffer = new Queue<float>(bufferSize);
        }

        public float CurrentFramesPerSecond { get; private set; }

        public float AvarageFramesPerSecond { get; private set; }

        public long TotalFrames { get; private set; }

        public float TotalSecond { get; private set; }

        public void Update(float deltaTime)
        {
            CurrentFramesPerSecond = 1.0f / deltaTime;

            deltaTimesBuffer.Enqueue(CurrentFramesPerSecond);

            if (deltaTimesBuffer.Count > bufferSize)
            {
                deltaTimesBuffer.Dequeue();

                AvarageFramesPerSecond = deltaTimesBuffer.Average();
            }
            else
            {
                AvarageFramesPerSecond = CurrentFramesPerSecond;
            }

            TotalFrames++;
            TotalSecond += deltaTime;
        }

        private readonly int bufferSize;

        private readonly Queue<float> deltaTimesBuffer;
    }
}
