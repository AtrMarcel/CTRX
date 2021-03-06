using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace CTRX
{
    public class CLaser : CPost
    {
        private CLaserCycle laserCycle;

        public CLaserCycle LaserCycle { get => laserCycle; set => laserCycle = value; }

        public override int MajEntrees() { return 1; }

        public override int MajCycle() { return 1; }

        public override int MajSorties() { return 1; }

        public override void Run(Object source, ElapsedEventArgs e) { }

        public override int Reset() { return 1; }

        public override int Load() { return 1; }

        public override int Save() { return 1; }
    }

    public class CLaserCycle : CCycleStatus
    {
    }
}