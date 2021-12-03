using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CTRX
{
    [Serializable]
    public class CParam<T> : CInfo
    {
        private T value;
        private T valMin;
        private T valMax;
        private T valRef;
        private string unit;

        public T Value { get => value; set => this.value = value; }
        public T ValMin { get => valMin; set => valMin = value; }
        public T ValMax { get => valMax; set => valMax = value; }
        public T ValRef { get => valRef; set => valRef = value; }
        public string Unit { get => unit; set => unit = value; }


        public CParam(T value, T valMin, T valMax, T valRef, string unit)
        {
            Value = value;
            ValMin = valMin;
            ValMax = valMax;
            ValRef = valRef;
            Unit = unit ?? throw new ArgumentNullException(nameof(unit));
        }

        public CParam(uint acces, uint id, string name, string text) : base(acces, id, name, text)
        {
            Acces = acces;
            Id = id;
            Name = name;
            Text = text;
        }

        public CParam()
        {
        }
    }
}