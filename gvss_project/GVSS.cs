using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace gvss_project
{
    [Serializable]
    public enum GVSSMode { UniqueViolations, RepetativeViolations }

    [Serializable]
    public class VertexNode
    {
        public uint VertexID { get; private set; }
        public string UniqueCode { get; private set; }

        public VertexNode(uint vID, string uniqueCode) {
            VertexID = vID;
            UniqueCode = uniqueCode;
        }
    }

    [Serializable]
    public class GVSS
    {
        [Serializable]
        private enum VertexType { Main, Violation, AuxiliaryParam };

        [Serializable]
        private class Vertex
        {
            [Serializable]
            private class Edge
            {
                public float EdgeWeight { get; set; }
                public uint ConnectedVertexID { get; set; }
                public Edge(uint conVerID, float edgeWeight)
                {

                    EdgeWeight = edgeWeight;
                    ConnectedVertexID = conVerID;

                }
            }

            //properties and fields
            public Point CenterPos { get; set; }
            public uint ID { get; private set; }
            public VertexType Type { get; private set; }
            public int AdjVCount {
                get {
                    return edges.Count;
                }
            }
            public uint Level { get; private set; }
            private List<Edge> edges;
            //additional information corresponding to the task

            public Vertex() { }
            public Vertex(uint vID, VertexType type, uint level, Point centerPos)
            {

                ID = vID;
                edges = new List<Edge>();
                Type = type;
                Level = level;
                CenterPos = centerPos;

            }
            public void addEdge(uint anotherVertexID, float edgeWeight)
            {

                if (anotherVertexID != this.ID)
                    edges.Add(new Edge(anotherVertexID, edgeWeight));

            }
            public void deleteEdge(uint anotherVertexID)
            {

                if (anotherVertexID != this.ID)
                {
                    foreach (Edge edge in edges)
                    {
                        if (edge.ConnectedVertexID == anotherVertexID)
                        {
                            edges.Remove(edge);
                            break;

                        }
                    }
                }

            }
            public void setEdgeWeight(uint anotherVertexID, float edgeWeight)
            {

                foreach (Edge edge in edges)
                {
                    if (edge.ConnectedVertexID == anotherVertexID)
                    {
                        edges[edges.IndexOf(edge)].EdgeWeight = edgeWeight;
                        break;
                    }
                }

            }
            public List<uint> getAdjacentVertexes()
            {

                List<uint> adjVs = new List<uint>();
                foreach (Edge edge in edges)
                {
                    adjVs.Add(edge.ConnectedVertexID);
                }
                return adjVs;

            }
            public bool isConnectedWith(uint anotherVertexID)
            {

                if (anotherVertexID != this.ID)
                {
                    foreach (Edge edge in edges)
                    {
                        if (anotherVertexID == edge.ConnectedVertexID)
                        {
                            return true;
                        }
                    }
                }
                return false;

            }
            public float getEdgeWeight(uint anotherVertex)
            {
                foreach (Edge edge in edges)
                {
                    if (edge.ConnectedVertexID == anotherVertex)
                    {
                        return edge.EdgeWeight;
                    }
                }
                return -1;
            }
            public uint getParentVertex() {
                if (ID != 1)
                {
                    return edges[0].ConnectedVertexID;
                }
                else
                    throw new UnexpectedError();
            }
        }

        [Serializable]
        private struct Correspondence
        {
            public string SensorCode { get; private set; }
            public uint VertexID { get; private set; }
            public uint VertexIndex { get; private set; }

            public Correspondence(string sensorCode, uint vertexID, uint vertexIndex)
            {
                SensorCode = sensorCode;
                VertexID = vertexID;
                VertexIndex = vertexIndex;

            }

        }

        //fields and properties

        private List<Vertex> vertexes;
        private List<(Sensor, uint)> violations;
        private List<Sensor> auxiliaryParams;
        private List<uint> deletedVertexesList;
        private List<Correspondence> correspondences;
        private uint vertexID_counter;
        private readonly uint mainVertexID;
        private GVSSMode mode;

        public string ProductionAreaName { get; private set; }
        public int Size { get { return vertexes.Count; } }

        //private methods
        private Vertex getVertex(uint vID) {
            foreach (Vertex v in vertexes)
            {
                if (v.ID == vID)
                    return v;
            }
            throw new NotFoundException();
        }
        private uint getSensorsCount() {
            if (mode == GVSSMode.RepetativeViolations)
            {
                return (uint)auxiliaryParams.Count;
            }
            else if (mode == GVSSMode.UniqueViolations)
            {
                return (uint)(auxiliaryParams.Count + violations.Count);
            }
            else throw new UnexpectedError();
        }
        private bool isViolation(uint vID) {
            if (getVertex(vID).Type == VertexType.Violation)
            {
                return true;
            }
            else {
                return false;
            }
        }
        private uint addVertex(VertexType type, uint level, Point centerPos) {
            uint id = ++vertexID_counter;
            vertexes.Add(new Vertex(id, type, level, centerPos));
            return id;
        }
        private void deleteVertex(uint vertexID) {//
            if (isVertexInGraph(vertexID))
            {
                List<uint> connectedVertexes = new List<uint>();
                foreach(Vertex v in vertexes)
                {
                    if (v.ID == vertexID)
                    {
                        connectedVertexes = v.getAdjacentVertexes();
                        deletedVertexesList.Add(v.ID);
                        vertexes.Remove(v);
                        break;
                    }
                }
                if (connectedVertexes.Count > 0) {
                    foreach (Vertex v1 in vertexes)
                    {
                        foreach (uint v2 in connectedVertexes)
                        {
                            if (v1.ID == v2)
                            {
                                v1.deleteEdge(vertexID);
                            }
                        }
                    }
                }
            }
        }
        private void addEdge(uint firstV, uint secondV, float edgeWeight) {
            if (isVertexInGraph(firstV) && isVertexInGraph(secondV) && !doesTheEdgeExist(firstV, secondV))
            {
                uint counter = 0;
                foreach (Vertex v in vertexes)
                {
                    if (v.ID == firstV)
                    {
                        v.addEdge(secondV, edgeWeight);
                        counter++;
                    }
                    else if (v.ID == secondV)
                    {
                        v.addEdge(firstV, edgeWeight);
                        counter++;
                    }
                    if (counter == 2)
                        break;
                }
            }
        }
        private bool isVertexInGraph(uint vID) {
            foreach (Vertex vertex in vertexes)
            {
                if (vertex.ID == vID)
                    return true;
            }
            return false;
        }
        private bool doesTheEdgeExist(uint firstV, uint secondV) {
            foreach(Vertex v in vertexes)
            {
                if ((v.ID == firstV && v.isConnectedWith(secondV))||
                    (v.ID == secondV && v.isConnectedWith(firstV)))
                {
                    return true;
                }
            }
            return false;
        }      
        private Sensor registerSensor(char captionLetter, uint index, string desription, SensorControlMethod controlMethod, SensorControlAgent controlAgent, SensorArrowSign arrowSign, SensorRequestBodies requestBody, SensorRequestPlacementMethod requestPlacementMethod) {
            Sensor s = new Sensor();
            s.CapitalLetter = captionLetter;
            s.Index = (int)index;
            s.Description = desription;
            s.ControlMethod = controlMethod;
            s.ArrowSign = arrowSign;
            s.ControlAgent = controlAgent;
            s.RequestBody = requestBody;
            s.RequestPlacementMethod = requestPlacementMethod;

            if (s.isValid() && isSensorNameAvailable(index, s.CapitalLetter, s.ArrowSign))
                return s;
            else throw new InvalidSensor();
        }
        private Sensor getAuxiliaryParam(string code) {
            foreach (Sensor s in auxiliaryParams) {
                if (s.UniqueCode == code)
                    return s;
            }
            throw new NotFoundException();
        }
        private Correspondence getCorrespondence(uint vID) {
            foreach (Correspondence c in correspondences) {
                if (c.VertexID == vID)
                    return c;
            }
            throw new NotFoundException();
        }
        private bool doesTheAuxiliaryParamExist(string code) {
            foreach (Sensor s in auxiliaryParams)
            {
                if (s.UniqueCode == code)
                    return true;
            }
            return false;
        }
        private bool doesTheSensorRepeat(string sensorCode, uint connectedVertexID) {

            List<uint> path = getWayUp(connectedVertexID);
            foreach (uint v in path) {
                if (getUniqueCode(v) == sensorCode)
                    return true;
            }

            List<uint> vertexesAround = getAdjacentVertexes(connectedVertexID);
            uint level = getLevel(connectedVertexID);
            foreach (uint v in vertexesAround) {
                if (getLevel(v) > level && sensorCode == getUniqueCode(v))
                    return true;
            }

            return false;
        
        }
        private void deleteCorrespondence(uint vID) { 
            foreach(Correspondence c in correspondences){
                if (c.VertexID == vID) {
                    correspondences.Remove(c);
                    return;
                }
            }
            throw new NotFoundException();
        }
        private bool doesTheViolationExist(string uniqueCode) {
            foreach ((Sensor,uint) v in violations)
            {
                if (v.Item1.UniqueCode == uniqueCode) {
                    return true;
                }      
            }
            return false;
        }
        private void deleteViolation(uint vID) {
            foreach ((Sensor, uint) node in violations) {
                if (node.Item2 == vID) {
                    violations.Remove(node);
                    return;
                }
            }
            throw new NotFoundException();
        }
        private uint getLevel(uint vID) {
            foreach (Vertex v in vertexes) {
                if (v.ID == vID) {
                    return v.Level;
                }
            }
            throw new NotFoundException();
        }
        private void deleteAuxiliary(string sensorCode) {
            if (doesTheAuxiliaryParamExist(sensorCode)) {
                foreach (Sensor s in auxiliaryParams) {
                    if (s.UniqueCode == sensorCode) {
                        auxiliaryParams.Remove(s);
                        break;
                    }
                }
            }
        }
        private void deleteProc(uint curVertexID)   {

            List<uint> adjV = getAdjacentVertexes(curVertexID);
            foreach (uint v in adjV) {
                if (getLevel(v) > getLevel(curVertexID)) {
                    deleteProc(v);
                }
            }
            if (getVertex(curVertexID).Type == VertexType.Violation) {
                string aP = getCorrespondence(curVertexID).SensorCode;
                if (mode == GVSSMode.RepetativeViolations && !doesTheAuxiliaryParamExist(aP)) {
                    deleteAuxiliary(aP);
                }
                deleteViolation(curVertexID);
            }
                 
            deleteCorrespondence(curVertexID);
            deleteVertex(curVertexID);
            return;
        
        }
        private List<uint> getWayUp(uint startVertexID) {

            uint curVertex = startVertexID;
            List<uint> path = new List<uint>();
            bool completed = false;

            while (!completed) {

                path.Add(curVertex);
                List<uint> adjVertexes = getAdjacentVertexes(curVertex);
                uint level = getLevel(curVertex);
                foreach (uint v in adjVertexes) {
                    if (level > getLevel(v)) {
                        if (v == mainVertexID)
                            completed = true;
                        else {
                            curVertex = v;
                        }
                        break;
                        
                    }

                }
            
            }
            return path;
        
        }
        private int getNumberVerticiesInSubtree(uint vID) {
            if (isVertexInGraph(vID) && vID != mainVertexID)
            {
                uint parentID = getVertex(vID).getParentVertex();
                uint parentLevel = getLevel(parentID);
                List<uint> verticies = getAdjacentVertexes(parentID);

                int counter = 0;

                foreach (uint v in verticies) {
                    if (getLevel(v) > parentLevel) {
                        ++counter;
                    }
                }
                return counter;
            }
            else {
                throw new InvalidArgument();
            }
        }
        private float getViolationInformationQuantity(uint vID, uint numOfTechLines) {
            if (isViolation(vID))
            {
                float sum = 0;
                foreach ((Sensor, uint) violation in violations) {
                    sum += getEdgeWeight(mainVertexID, violation.Item2);
                }

                return getEdgeWeight(mainVertexID, vID) / sum;
            }
            else
                throw new InvalidArgument();
        }
        private uint getDampedDeviceTotal() {
            uint count = 0;

            foreach (Sensor s in auxiliaryParams) {
                if (s.ControlAgent == SensorControlAgent.HighlyDampedDevice || s.ControlAgent == SensorControlAgent.WeakDampedDevice) {
                    count++;
                }
            }
            if (mode == GVSSMode.UniqueViolations) {
                foreach ((Sensor, uint) v in violations) {
                    if (v.Item1.ControlAgent == SensorControlAgent.HighlyDampedDevice || v.Item1.ControlAgent == SensorControlAgent.WeakDampedDevice) {
                        count++;
                    }
                }
            }

            return count;
        }

        //public methods

        public GVSS(Point mainCenterPos, GVSSMode mode = GVSSMode.RepetativeViolations)
        {
            this.mode = mode;
            this.vertexes = new List<Vertex>();
            this.violations = new List<(Sensor,uint)>();
            this.auxiliaryParams = new List<Sensor>();
            this.correspondences = new List<Correspondence>();
            this.vertexID_counter = 0;
            this.ProductionAreaName = "Прозводственная система";

            //creating main vertex
            mainVertexID = this.addVertex(VertexType.Main, 0, mainCenterPos);
            correspondences.Add(new Correspondence("Main", mainVertexID, 0));
        }
        public VertexNode addViolation(char captionLetter, uint index, string description, SensorControlMethod controlMethod, SensorControlAgent controlAgent, SensorArrowSign arrowSign, SensorRequestBodies requestBody, SensorRequestPlacementMethod requestPlacementMethod, Point centerPos, float probability) {

            Sensor s = registerSensor(captionLetter, index, description, controlMethod, controlAgent, arrowSign, requestBody, requestPlacementMethod);
            uint level = 1;
            uint vID = addVertex(VertexType.Violation, level, centerPos);
            violations.Add((s, vID));
            if (mode == GVSSMode.RepetativeViolations)
                auxiliaryParams.Add(s);
            correspondences.Add(new Correspondence(s.UniqueCode, vID, (uint)(vertexes.Count - 1)));
            
            addEdge(mainVertexID, vID, probability);
            return new VertexNode(vID, s.UniqueCode);

        }
        public VertexNode addViolation(string auxiliaryCode, Point centerPos, float probability) {

            if (mode == GVSSMode.RepetativeViolations)
            {
                if (doesTheAuxiliaryParamExist(auxiliaryCode) && !doesTheViolationExist(auxiliaryCode))
                {
                    
                    uint level = 1;
                    uint vID = addVertex(VertexType.Violation, level, centerPos);
                    Sensor s = getAuxiliaryParam(auxiliaryCode);
                    violations.Add((s, vID));
                    correspondences.Add(new Correspondence(s.UniqueCode, vID, (uint)(vertexes.Count - 1)));

                    addEdge(mainVertexID, vID, probability);
                    return new VertexNode(vID, s.UniqueCode);
                }
                else
                {
                    throw new InvalidArgument();
                }
            }
            else {
                throw new UnexpectedError();
            }
            
            
        }
        public string addAuxiliaryParam(char captionLetter, uint index, string description, SensorControlMethod controlMethod, SensorControlAgent controlAgent, SensorArrowSign arrowSign, SensorRequestBodies requestBody, SensorRequestPlacementMethod requestPlacementMethod) {
            Sensor s = registerSensor(captionLetter, index, description, controlMethod, controlAgent, arrowSign,requestBody, requestPlacementMethod);
            auxiliaryParams.Add(s);
            return s.UniqueCode;
        }
        public VertexNode connect(uint upperVertex, string auxiliaryVertexCode, Point centerPos) {
            if (upperVertex == mainVertexID
                || !isVertexInGraph(upperVertex)
                || !doesTheAuxiliaryParamExist(auxiliaryVertexCode))
                throw new InvalidArgument();
            else if (doesTheSensorRepeat(auxiliaryVertexCode, upperVertex))
                throw new SensorRepeatsException();
            
            Sensor s = getAuxiliaryParam(auxiliaryVertexCode);
            uint level = vertexes[(int)getCorrespondence(upperVertex).VertexIndex].Level+1;
            uint vID = addVertex(VertexType.AuxiliaryParam, level, centerPos);
            correspondences.Add(new Correspondence(s.UniqueCode, vID, (uint)(vertexes.Count - 1)));

            float probability = 1.0f; //count probability
            addEdge(upperVertex, vID, probability);
            return new VertexNode(vID,s.UniqueCode);

        }
        public List<uint> delete(uint vID) {
            if (vID != mainVertexID && isVertexInGraph(vID)) {
                deletedVertexesList = new List<uint>();
                deleteProc(vID);
                return deletedVertexesList;
            }
            else
                throw new InvalidArgument();
        }
        public bool isSensorNameAvailable(uint index, char capitalLetter, SensorArrowSign arrowSign)  {
            Sensor s = new Sensor();
            s.Index = (int)index;
            s.CapitalLetter = capitalLetter;
            s.ArrowSign = arrowSign;

            foreach ((Sensor, uint) value in violations) {
                if (value.Item1 == s)
                    return false;         
            }

            foreach (Sensor value in auxiliaryParams) {
                if (value == s)
                    return false;
            }

            return true;
        }
        public string getUniqueCode(uint vID) {
            if (isVertexInGraph(vID)) {
                foreach (Correspondence c in correspondences)
                {
                    if (c.VertexID == vID)
                        return c.SensorCode;
                }
                throw new UnexpectedError();
            }
            else
                throw new InvalidArgument();
        
        }
        public List<VertexNode> getVertexes(bool withoutMain = true)
        {
            List<VertexNode> vList = new List<VertexNode>();
            foreach (Correspondence c in correspondences)
            {
                if (withoutMain && c.SensorCode == "Main") {
                    continue;
                }
                vList.Add(new VertexNode(c.VertexID, c.SensorCode));
            }
            return vList;
        }
        public List<uint> getVertexesIDs() {
            List<uint> vList = new List<uint>();
            foreach (Correspondence c in correspondences)
            {
                vList.Add(c.VertexID);
            }
            return vList;
        }
        public List<uint> getAdjacentVertexes(uint vertexID)
        {
            foreach (Vertex v in vertexes)
            {
                if (v.ID == vertexID)
                {
                    return v.getAdjacentVertexes();
                }
            }
            return new List<uint>();
        }
        public List<string> getAvailableParams() {
            List<string> list = new List<string>();
            foreach (Sensor s in auxiliaryParams) {
                list.Add(s.UniqueCode);
            }
            return list;

        }
        public List<Event> getEvents(uint numOfTechLines = 1, OperatorLevel operatorLevel = OperatorLevel.Third, InformationRepresentationMethod informationRepresentationMethod = InformationRepresentationMethod.MnemonicDiagram) {

            if (numOfTechLines < 1)
                throw new InvalidArgument();

            List<Event> events = new List<Event>();
            List<uint> path = new List<uint>();
            uint eventCounter = 1;

            foreach (Vertex v in vertexes) {
                if (v.AdjVCount == 1 && v.Type != VertexType.Main && v.Type != VertexType.Violation) {
                    path = getWayUp(v.getAdjacentVertexes()[0]);
                    path.Insert(0, v.ID);
                    path.Add(mainVertexID);

                    Event e = new Event(eventCounter++, path, numOfTechLines, getSensorsCount(), getViolationInformationQuantity(path[path.Count - 2],numOfTechLines), operatorLevel, getDampedDeviceTotal(), informationRepresentationMethod);

                    foreach (uint tempV in path) {
                        if (tempV != mainVertexID) {

                            Sensor s = getSensor(getCorrespondence(tempV).SensorCode);
                            if (s.ControlMethod == SensorControlMethod.Request)
                            {
                                e.addRequestSensor(s);
                            }
                            else if (s.ControlMethod == SensorControlMethod.Auto) {
                                e.addAutoSensor(s);
                            }


                            if (tempV != path[path.Count - 2]) {
                                e.addVertexAndSubverticiesNumber(new Event.VertexAndNumOfSubverticies((int)tempV, getNumberVerticiesInSubtree(tempV)));
                            }
                        }

                    }

                    events.Add(e);
                }
            }

            return events;
        }
        public VertexNode getMain() {
            foreach (Correspondence c in correspondences) {
                if (c.VertexID == mainVertexID) {
                    return new VertexNode(c.VertexID, c.SensorCode);
                }
            }
            throw new UnexpectedError();
        }
        public Point getVertexCenterPos(uint vID) {

            foreach (Vertex v in vertexes) {
                if (v.ID == vID)
                    return v.CenterPos;
            }
            throw new NotFoundException();
        
        }
        public void setVertexCenterPos(uint vID, Point pos) {
            foreach (Vertex v in vertexes)
            {
                if (v.ID == vID) {
                    v.CenterPos = pos;
                    return;
                }     
            }
            throw new NotFoundException();
        }
        public List<string> getAllSensors() {
            List<string> allSensors = new List<string>();

            foreach ((Sensor, uint) value in violations)
            {
                allSensors.Add(value.Item1.UniqueCode);
            }

            foreach (Sensor value in auxiliaryParams)
            {
                allSensors.Add(value.UniqueCode);
            }

            if (allSensors.Count == 0)
                return allSensors;
            return allSensors.Distinct().ToList();
        
        }
        public List<uint> setMode(GVSSMode newMode) {

            if (newMode == this.mode) {
                return new List<uint>();
            }

            List<uint> vertexesToRemove = new List<uint>();

            if (newMode == GVSSMode.RepetativeViolations)
            {
                //были уникальные - стали повторяющиеся
                foreach ((Sensor, uint) violation in violations) {
                    auxiliaryParams.Add(violation.Item1);
                }


            }
            else if (newMode == GVSSMode.UniqueViolations) 
            {
                //были повторяющиеся - стали уникальными
                // remove from auxiliary params, delete vertexes
                
                foreach ((Sensor, uint) violation in violations) {
                    foreach (Sensor auxiliary in auxiliaryParams) {
                        if (violation.Item1.UniqueCode == auxiliary.UniqueCode) {
                            auxiliaryParams.Remove(auxiliary);
                            break;
                        }
                    }
                }

                List<uint> toRemoveVertexes = new List<uint>();
                
                foreach((Sensor, uint) violation in violations) {
                    foreach (Correspondence c in correspondences)
                    {
                        if (c.SensorCode == violation.Item1.UniqueCode && getVertex(c.VertexID).Type == VertexType.AuxiliaryParam) {
                            toRemoveVertexes.Add(c.VertexID);
                        }
                    }
                }

                foreach (uint v in toRemoveVertexes) {
                    vertexesToRemove.AddRange(delete(v));
                }
            }

            this.mode = newMode;
            if (vertexesToRemove.Count != 0)
            {
                return vertexesToRemove.Distinct().ToList();
            }
            else return new List<uint>();
            

        }
        public List<string> getAllViolations() {

            List<string> v = new List<string>();
            foreach ((Sensor, uint) viol in violations) {
                v.Add(viol.Item1.UniqueCode);
            }
            return v;

        }
        public bool isSensorInGVSS(string uniqueCode) {
            bool isIn = false;

            foreach (Sensor s in auxiliaryParams)
            {
                if (s.UniqueCode == uniqueCode) {
                    isIn = true;
                    break;
                }
            }

            if (!isIn && mode == GVSSMode.UniqueViolations) {
                foreach ((Sensor, uint) v in violations) {
                    if (v.Item1.UniqueCode == uniqueCode) {
                        isIn = true;
                        break;
                    }
                }
            }

            return isIn;
        }
        public Sensor getSensor(string uniqueCode) {

            if (isSensorInGVSS(uniqueCode))
            {
                foreach (Sensor sens in auxiliaryParams) {
                    if (sens.UniqueCode == uniqueCode)
                        return sens;
                }
                if (mode == GVSSMode.UniqueViolations) {
                    foreach ((Sensor, uint) v in violations) {
                        if (v.Item1.UniqueCode == uniqueCode) {
                            return v.Item1;
                        }
                    }
                }
            }
            throw new InvalidArgument();
        }
        public List<uint> deleteAll(string uniqueCode) {
            if (isSensorInGVSS(uniqueCode))
            {
                List<uint> toRemove = new List<uint>();
                foreach (Correspondence c in correspondences) {
                    if (c.SensorCode == uniqueCode) {
                        toRemove.Add(c.VertexID);
                    }
                }

                List<uint> deletedVertexesList = new List<uint>();

                foreach (uint v in toRemove) {
                    deletedVertexesList.AddRange(delete(v));
                }

                deleteAuxiliary(uniqueCode);
                if (deletedVertexesList.Count != 0)
                    return deletedVertexesList.Distinct().ToList();
                return deletedVertexesList;
            }
            else {
                throw new InvalidArgument();
            }
        }
        public List<uint> getVertexChilds(uint vID) {
            if (isVertexInGraph(vID))
            {
                Vertex v = getVertex(vID);
                uint vLevel = v.Level;
                List<uint> adjVertexes = v.getAdjacentVertexes();
                foreach (uint adjV in adjVertexes) {
                    if (getLevel(adjV) < vLevel) {
                        adjVertexes.Remove(adjV);
                        break;
                    }
                }
                return adjVertexes;
            }
            else {
                throw new InvalidArgument();
            }
        }
        public void editSensor(string uniqueCode, SensorControlAgent controlAgent, SensorControlMethod controlMethod, SensorRequestBodies requestBody, SensorRequestPlacementMethod requestPlacementMethod, string description) {
            foreach ((Sensor, uint) v in violations) {
                if (v.Item1.UniqueCode == uniqueCode) {
                    v.Item1.ControlMethod = controlMethod;
                    v.Item1.ControlAgent = controlAgent;
                    v.Item1.Description = description;
                    v.Item1.RequestBody = requestBody;
                    v.Item1.RequestPlacementMethod = requestPlacementMethod;
                    break;
                }
            }
            foreach (Sensor a in auxiliaryParams)
            {
                if (a.UniqueCode == uniqueCode)
                {
                    a.ControlMethod = controlMethod;
                    a.ControlAgent = controlAgent;
                    a.Description = description;
                    a.RequestBody = requestBody;
                    a.RequestPlacementMethod = requestPlacementMethod;
                }
            }
        }
        public string getType(uint vID) {

            if (isVertexInGraph(vID))
            {
                if (vID == mainVertexID)
                    return "Main";

                foreach ((Sensor, uint) v in violations)
                {
                    if (v.Item2 == vID)
                        return "Violation";
                }

                return "Auxiliary";
            }
            else throw new InvalidArgument();
           


        }
        public float getEdgeWeight(uint firstV, uint secondV)
        {
            if (doesTheEdgeExist(firstV, secondV))
            {
                foreach (Vertex v in vertexes)
                {
                    if (v.ID == firstV)
                    {
                        return v.getEdgeWeight(secondV);
                    }
                    else if (v.ID == secondV)
                    {
                        return v.getEdgeWeight(firstV);
                    }
                }
            }
            return -1;
        }
        public GVSSMode getMode() {
            return mode;
        }
        public bool isNull() {
            return vertexes.Count == 1 && correspondences.Count == 1 && auxiliaryParams.Count == 0 && violations.Count == 0;
        }

    }
}
