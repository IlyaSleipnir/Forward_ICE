using Forward_ICE.Engines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Forward_ICE.Stands
{
    internal class TestingStand
    {
        private Engine _engine;

        public Engine Engine {
            get { return _engine; }
            set { _engine = value; }
        }
        public TestingStand( Engine engine )
        {
            _engine = engine;
        }

        public double Watch()
        {
            while (_engine.T < _engine.TOverheat)
            {
                if (_engine.IsTimeLimitOver)
                {
                    break;
                }
            }
            _engine.Stop();

            return _engine.WorkingTime;
        }

        public void CheckOverheat()
        {
            if(_engine.T >= _engine.TOverheat || _engine.IsTimeLimitOver)
            {
                _engine.Stop();
            }
        }

        public double StartOverheatTesting(double TEnv, double precision, double timeLimit)
        {
            //using (AutoResetEvent resetEvent = new AutoResetEvent(false))
            //{
            //    ThreadPool.QueueUserWorkItem((state) => _engine.Start(TEnv, precision, timeLimit));
            //}

            //ThreadPool.QueueUserWorkItem((state) => _engine.Start(TEnv, precision, timeLimit));

            _engine.CheckState += CheckOverheat;
            _engine.Start(TEnv, precision, timeLimit);

            //while (_engine.T < _engine.TOverheat)
            //{
            //    if (_engine.IsTimeLimitOver)
            //    {
            //        break;
            //    }
            //}
            //_engine.Stop();

            _engine.CheckState -= CheckOverheat;
            return _engine.WorkingTime;


        }

        public void CheckMaxPower()
        {
            if (!_engine.IsMaxReached || _engine.IsTimeLimitOver)
            {
                _engine.Stop();
            }
        }

        public Dictionary<string, double> StartMaxPowerTesting(double TEnv, double precision, double timeLimit)
        { 

            _engine.CheckState += CheckOverheat;
            _engine.Start(TEnv, precision, timeLimit);

            _engine.CheckState -= CheckOverheat;
            return new Dictionary<string, double>
            {
                { "MaxN", _engine.MaxN },
                { "MaxV", _engine.MaxV }
            };


        }
    }
}
