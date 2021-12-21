using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gvss_project
{
    public class Event
    {
        public class VertexAndNumOfSubverticies
        {
            public int VertexID { get; private set; }
            public int SubverticiesNumber { get; private set; }
            public VertexAndNumOfSubverticies(int vertexID, int subvertexesNumber)
            {

                if (vertexID <= 0 || subvertexesNumber < 0)
                {
                    throw new InvalidArgument();
                }
                VertexID = vertexID;
                SubverticiesNumber = subvertexesNumber;
            }
            public VertexAndNumOfSubverticies()
            {
                VertexID = -1;
                SubverticiesNumber = -1;
            }
            public bool isNull()
            {
                if (VertexID == -1 && SubverticiesNumber == -1)
                    return true;
                return false;
            }
        }

        private List<VertexAndNumOfSubverticies> vertices;
        private uint numOfTechLines;
        private float violationInformationQuantity;
        private uint sensorsCount;
        private uint dampedDeviceTotal;
        private OperatorLevel operatorLevel;
        private InformationRepresentationMethod informationRepresentationMethod;
        public List<uint> VertexPath { get; private set; }
        public List<Sensor> requestSensors;
        public List<Sensor> autoSensors;
        public uint ID { get; private set; }

        private VertexAndNumOfSubverticies isVertexAlreadyAdded(int vID)
        {
            foreach (VertexAndNumOfSubverticies vertex in vertices)
            {
                if (vertex.VertexID == vID)
                {
                    return vertex;
                }
            }
            throw new NotFoundException();
        }

        private double Log(double value)
        {
            return -Math.Log(value, 2);
        }
        private double Round(double value, int decimalNumbers = 2) {
            return Math.Round(value, decimalNumbers);
        }

        public Event(uint id, List<uint> vertexPath, uint numOfTechLines, uint sensorsCount, float violationInformationQuantity, OperatorLevel operatorLevel, uint dampedDeviceTotal, InformationRepresentationMethod informationRepresentationMethod)
        {
            vertices = new List<VertexAndNumOfSubverticies>();

            requestSensors = new List<Sensor>();
            autoSensors = new List<Sensor>();

            this.numOfTechLines = numOfTechLines;
            this.operatorLevel = operatorLevel;
            this.sensorsCount = sensorsCount;
            this.violationInformationQuantity = violationInformationQuantity;
            this.dampedDeviceTotal = dampedDeviceTotal;
            this.informationRepresentationMethod = informationRepresentationMethod;

            ID = id;
            VertexPath = vertexPath;
            
        }
        
        public void addRequestSensor(Sensor s)
        {
            if (s.ControlMethod == SensorControlMethod.Request)
                requestSensors.Add(s);
        }
        public void addAutoSensor(Sensor s) {
            if (s.ControlMethod == SensorControlMethod.Auto)
                autoSensors.Add(s);
        }

        public void addVertexAndSubverticiesNumber(VertexAndNumOfSubverticies node)
        {
            VertexAndNumOfSubverticies v = new VertexAndNumOfSubverticies();
            try
            {
                v = isVertexAlreadyAdded(node.VertexID);
                throw new InvalidArgument();
            }
            catch (NotFoundException)
            {
                vertices.Add(node);
            }
        }
        public VertexAndNumOfSubverticies getVertexAndSubverticiesNumber(int vID)
        {
            return isVertexAlreadyAdded(vID);
        }

        public double getInformationDifficulty()
        {
            double techLineInfo = Log(1.0 / numOfTechLines);
            double sum = techLineInfo + Log(violationInformationQuantity);

            foreach (VertexAndNumOfSubverticies v in vertices)
            {
                sum -= Log((float)v.SubverticiesNumber);
            }
            return Round(sum);
        }
        public double getOriginalTime()
        {
            double informationDifficulty = getInformationDifficulty();
            (double, double) ab = DefaultSettings.OperatorCoefs[operatorLevel];
            double time = ab.Item1 + ab.Item2 * informationDifficulty;
            return Round(DefaultSettings.IndformationRepresentationMethodCoefs[informationRepresentationMethod] * time);

        }
        public double getRequestTime() {
            double time = 0;
            double t1, t2;
            foreach (Sensor s in requestSensors) {
                t1 = DefaultSettings.RequestTime[(s.RequestBody, s.RequestPlacementMethod)];
                t2 = DefaultSettings.ControlAgentsTime[s.ControlAgent];
                if (s.RequestBody == SensorRequestBodies.Dialer || s.RequestBody == SensorRequestBodies.KeyboardDevice)
                {
                    t1 *= s.NumOfRequestInputKeys;
                }
                time += t1 + t2;
            }
            return Round(time);
        }
        public double getIrrelevantTime() {
            return Math.Round(0.2*(sensorsCount * 0.075),2,MidpointRounding.AwayFromZero);
        }
        public double getAgentTimeInfluence() {
            double time = 0;
            foreach (Sensor s in autoSensors) {
                time += DefaultSettings.ControlAgentsTime[s.ControlAgent];
            }
            return Round(time);
        }
        public double getFindControlAgentTime() {
            double time = 0;

            uint dampedDevicesInEvent = 0;

            foreach (Sensor s in requestSensors) {
                if (s.ControlAgent == SensorControlAgent.HighlyDampedDevice || s.ControlAgent == SensorControlAgent.WeakDampedDevice)
                    dampedDevicesInEvent++;
            }
            foreach (Sensor s in autoSensors)
            {
                if (s.ControlAgent == SensorControlAgent.HighlyDampedDevice || s.ControlAgent == SensorControlAgent.WeakDampedDevice)
                    dampedDevicesInEvent++;
            }

            if (dampedDevicesInEvent != 0) {
                time = 0.02 * dampedDeviceTotal + 1.7 + (0.1 * (dampedDevicesInEvent - 1));
            }

            return Round(time, 1);
        }
    }
}
