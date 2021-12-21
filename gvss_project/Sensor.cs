using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gvss_project
{
    [Serializable]
    public class Sensor
    {
        private char _capitalLetter;
        public char CapitalLetter
        {
            get
            {
                return _capitalLetter;
            }
            set
            {
                if (Char.IsLetter(value))
                {
                    _capitalLetter = Char.ToUpper(value);
                }
                else
                {
                    throw new InvalidArgument();
                }

            }
        }
        public int Index { get; set; }
        public string Description { get; set; }
        public uint NumOfRequestInputKeys { get; set; }
        public SensorControlMethod ControlMethod { get; set; }
        public SensorControlAgent ControlAgent { get; set; }
        public SensorArrowSign ArrowSign { get; set; }
        public SensorRequestBodies RequestBody { get; set; }
        public SensorRequestPlacementMethod RequestPlacementMethod{ get; set; }
        public string UniqueCode {
            get {
                int u = 0x02b9b + (int)ArrowSign;
                string str = CapitalLetter.ToString();
                foreach (char c in Index.ToString()) {
                    str += ((char)(0x2080 + Int32.Parse(c.ToString()))).ToString();                
                }
                str += ((char)u).ToString();
                return str;
            }
        }
        public bool isValid() {

            return _capitalLetter != '0' &&
                Index != -1 &&
                ArrowSign != SensorArrowSign.None &&
                ControlMethod != SensorControlMethod.None &&
                ControlAgent != SensorControlAgent.None;
        }
        public static bool operator ==(Sensor s1, Sensor s2) {
            return (s1.CapitalLetter == s2.CapitalLetter)
                && (s1.Index == s2.Index)
                && (s1.ArrowSign == s2.ArrowSign);
        }
        public static bool operator !=(Sensor s1, Sensor s2) { 
            return (s1.CapitalLetter != s2.CapitalLetter)
                || (s1.Index != s2.Index)
                || (s1.ArrowSign != s2.ArrowSign);

        }
        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else {
                Sensor s = (Sensor)obj;
                return (CapitalLetter == s.CapitalLetter)
                && (Index == s.Index)
                && (ArrowSign == s.ArrowSign);
            }
        }
        public Sensor() {

            _capitalLetter = '0';
            Index = -1;
            Description = "";
            ControlMethod = SensorControlMethod.None;
            ArrowSign = SensorArrowSign.None;
            ControlAgent = SensorControlAgent.None;
            RequestBody = SensorRequestBodies.None;
            RequestPlacementMethod = SensorRequestPlacementMethod.None;
            NumOfRequestInputKeys = 3;

        }
        
    }
}
