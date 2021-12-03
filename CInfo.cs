using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CTRX
{
    [Serializable]
    public class CInfo
    {
        private uint acces;
        private uint id;
        private string name;
        private string text;

        public CInfo()
        {
            Acces = 0;
            Id = 0;
            Name = "" ?? throw new ArgumentNullException(nameof(name));
            Text = "" ?? throw new ArgumentNullException(nameof(text));
        }

        public CInfo(uint acces, uint id, string name, string text)
        {
            Acces = acces;
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }

        public uint Acces { get => acces; set => acces = value; }
        public uint Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Text { get => text; set => text = value; }
    }
}