using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forward_ICE.Engines
{
    internal abstract class Engine
    {
        public delegate void Signal();
        public abstract event Signal CheckState;

        public abstract double T {  get; }
        public abstract double TOverheat { get; }
        public abstract double WorkingTime { get; }
        public abstract double MaxN { get; }
        public abstract double MaxV { get; }
        public abstract bool IsWorking { get; }
        public abstract bool IsTimeLimitOver {  get; }
        public abstract bool IsMaxReached { get; }

        public abstract void Start(double TEnv, double precision, double timeLimit);
        public abstract void Stop();
    }
}
