using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forward_ICE.Engines
{
    internal class ICE : Engine
    {
        public override event Signal CheckState;

        private bool _isWorking;
        private bool _isTimeLimitOver;
        private bool _isMaxReached;
        private double _workingTime;
        private double _maxN;
        private double _maxV;

        private double _T;

        private double _I;
        private List<double> _M;
        private List<double> _V;
        private double _TOverheat;
        private double _HM;
        private double _HV;
        private double _C;

        public bool Logging { get; set; } = false;
        public override double T {  get { return _T; } }

        public override double TOverheat {  get { return _TOverheat; } }

        public override double WorkingTime { get { return _workingTime; } }

        public override double MaxN { get { return _maxN; } }

        public override double MaxV { get { return _maxV; } }

        public override bool IsTimeLimitOver { get { return _isTimeLimitOver; } }

        public override bool IsWorking { get { return _isWorking; } }

        public override bool IsMaxReached { get { return _isMaxReached; } }

        public override void Start(double TEnv, double precision, double timeLimit)
        {
            if (_M.Count != _V.Count)
            {
                throw new ArgumentException("Размерности массивов крутящего момента M и скорости V не совпадают");
            }

            double a;
            double VH;
            double VC;
            double N;
            double V;
            double M;

            _maxN = 0;
            _maxV = 0;
            _workingTime = 0;

            if (TEnv > _TOverheat)
            {
                return;
            }

            _isWorking = true;
            _isTimeLimitOver = false;
            _isMaxReached = false;
            _T = TEnv;

            for (int i = 0; i < _M.Count; i++)
            {
                V = _V[i];
                M = _M[i];

                while (V < _V[i + 1])
                {

                    if (_workingTime > timeLimit)
                    {
                        _isWorking = false;
                        _isTimeLimitOver = true;
                    }

                    if (!_isWorking)
                    {
                        return;
                    }

                    a = M / _I;
                    VH = M * _HM + V * V * _HV;
                    VC = _C * (TEnv - _T);

                    _T += VH + VC;
                    V += a / precision;
                    M = (V - _V[i]) * (_M[i + 1] - _M[i]) / (_V[i + 1] - _V[i]) + _M[i];

                    N = M * V / 1000;
                    if (N > _maxN)
                    {
                        _maxN = N;
                        _maxV = V;
                    }
                    else
                    {
                        _isMaxReached = true;
                    }

                    _workingTime += 1.0 / precision;

                    if (Logging)
                    {
                        Console.WriteLine($"V[{i + 1}]={_V[i + 1]}; V={V}; " +
                            $"M[{i + 1}]={_M[i + 1]}; M={M}; " +
                            $"N={N}; MaxN={_maxN}; MaxV={_maxV}; " +
                            $"T={_T}; T_o={_TOverheat}; Time={_workingTime}");
                    }

                    CheckState.Invoke();
                }
            }
        }

        public override void Stop()
        {
            _isWorking = false;
        }

        public ICE(string path)
        {
            string line;

            using (var file = new StreamReader(path))
            {
                line = file.ReadLine().Replace(',', '.');
                _I = double.Parse(line, CultureInfo.InvariantCulture);

                line = file.ReadLine().Replace(',', '.');
                _M = line.Split().Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

                line = file.ReadLine().Replace(',', '.');
                _V = line.Split().Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();

                line = file.ReadLine().Replace(',', '.');
                _TOverheat = double.Parse(line, CultureInfo.InvariantCulture);

                line = file.ReadLine().Replace(',', '.');
                _HM = double.Parse(line, CultureInfo.InvariantCulture);

                line = file.ReadLine().Replace(',', '.');
                _HV = double.Parse(line, CultureInfo.InvariantCulture);

                line = file.ReadLine().Replace(',', '.');
                _C = double.Parse(line, CultureInfo.InvariantCulture);
            }
        }

        public ICE(
            double I,
            List<double> M,
            List<double> V,
            double TOverheat,
            double HM,
            double HV,
            double C
            )
        {
            _I = I;
            _M = M;
            _V = V;
            _TOverheat = TOverheat;
            _HM = HM;
            _HV = HV;
            _C = C;
        }
    }
}
