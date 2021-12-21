using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace gvss_project
{
    public struct Connection {
        public uint v1ID { get; private set; }
        public uint v2ID { get; private set; }

        public Connection(uint v1, uint v2) {
            v1ID = v1;
            v2ID = v2;
        }
    }
    public partial class MainWindow : Window
    {
        private GVSS g;
        private uint btnWidth = 50;
        private uint btnHeight = 50;
        private bool isPressed = false;
        private bool particularEventIsSelected = false;
        private Point topLeft;
        private Point pos;
        private int indexCounter = 3;
        private ContextMenu contextMenu;
        private SensorArrowSign checkedArrowRB;
        private GVSSMode checkedModeRB;
        private LanguageMode currentLanguage;
        private List<RadioButton> uniqueCodeRadioButtons;
        private List<RadioButton> modeRadioButtons;
        private List<RadioButton> languageModeRadioButtons;
        private bool initializationFinished;
        private readonly Point mainInitialPos;

        private MessageBoxResult warningMB(string text) {
            return MessageBox.Show(text, DefaultSettings.getWindowTitleText(WindowTitles.WarningMB),
                            MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
        }
        private void errorMB(string text) {
            MessageBox.Show(text, DefaultSettings.getWindowTitleText(WindowTitles.ErrorMB), MessageBoxButton.OK, MessageBoxImage.Error);
        }
        private void programmaticallySetGVSSMode(GVSSMode mode) {
            foreach (RadioButton modeRB in modeRadioButtons)
            {
                if ((GVSSMode)(modeRB.Tag) == mode)
                {
                    checkedModeRB = mode;
                    modeRB.IsChecked = true;
                    break;
                }
            }
        }
        private void clearInterface() {
            load_sensorEditCB();
            load_chooseEventCB();
            clearRegisterParamFields();
        }

        private bool SaveGVSS() {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "GVSS Files (*.gvss)|*.gvss";
            sfd.InitialDirectory = Environment.CurrentDirectory;
            if (sfd.ShowDialog() == true) {
                BinarySerialization.WriteToBinaryFile<GVSS>(sfd.FileName, g);
                return true;
            }
            return false;
        }
        private void LoadGVSS() {

            if (endWithGVSS(DefaultSettings.getMessageBoxText(MessageBoxMessages.SaveGVSSBeforeLoad)) == true) {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "GVSS Files (*.gvss)|*.gvss| MathLAB Files (*.asv)|*.asv";
                ofd.InitialDirectory = Environment.CurrentDirectory;
                if (ofd.ShowDialog() == true)
                {
                    string fileName = ofd.FileName;
                    string fileExt = System.IO.Path.GetExtension(fileName);

                    if (fileExt == ".asv")
                    {
                        List<double> doubleNumbers = new List<double>();
                        try
                        {
                            doubleNumbers = FindYArray(fileName);
                        }
                        catch (Exception)
                        {
                            errorMB(DefaultSettings.getErrorMessageText(ErrorMessages.WhileASVFileCorrupted));
                            return;
                        }

                        MathLABResultWindow window = new MathLABResultWindow(doubleNumbers);
                        window.Owner = this;
                        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        window.ShowDialog();

                    }
                    else if (fileExt == ".gvss") {
                        List<uint> oldVertexes = g.getVertexesIDs();

                        try
                        {
                            GVSS newGVSS = BinarySerialization.ReadFromBinaryFile<GVSS>(fileName);
                            this.g = newGVSS;
                        }
                        catch (Exception)
                        {
                            errorMB(DefaultSettings.getErrorMessageText(ErrorMessages.WhileGVSSFileCorrupted));
                            return;
                        }

                        removeVertexes(oldVertexes);
                        clearInterface();

                        GVSSMode newMode = g.getMode();

                        if (newMode != checkedModeRB)
                        {
                            programmaticallySetGVSSMode(newMode);
                        }

                        List<VertexNode> newVertexes = g.getVertexes(false);
                        foreach (VertexNode v in newVertexes)
                        {
                            spawnVertex(g.getType(v.VertexID), v);
                            List<uint> childVertexes = g.getVertexChilds(v.VertexID);
                            foreach (uint cv in childVertexes)
                            {
                                float probability = -1f;
                                if (v.VertexID == 1)
                                {
                                    probability = g.getEdgeWeight(v.VertexID, cv);
                                }
                                spawnEdge(v.VertexID, cv, probability);
                            }
                        }
                    }              
                }
            }    
        }
        private void SavePhoto()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PNG Files (*.png)|*.png";
            sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (sfd.ShowDialog() == true)
            {
                RenderVisualService.RenderToPNGFile(canvas, sfd.FileName);
            }
        }

        private void addViolation(char capLet, uint index, string description, SensorControlMethod controlMethod, SensorControlAgent controlAgent, SensorRequestBodies requestBody, SensorRequestPlacementMethod requestPlacementMethod, SensorArrowSign arrowSign, Point centerPos, float probability) {
            VertexNode v = g.addViolation(capLet, index, description, controlMethod, controlAgent, arrowSign, requestBody, requestPlacementMethod, centerPos,probability);
            spawnViolation(v, probability);
            fillManagePanel();
            load_chooseEventCB();
        }
        private void addViolation(string auxiliaryCode, Point centerPos, float probability) {
            VertexNode v = g.addViolation(auxiliaryCode, centerPos,probability);
            spawnViolation(v, probability);
            fillManagePanel();
            load_chooseEventCB();
        }
        private void spawnViolation(VertexNode v, float probability) {
            spawnVertex("Violation", v);
            spawnEdge(g.getMain().VertexID, v.VertexID, probability);
        }
        private void rollUpOrExpandStackPanel(StackPanel s, Border b, Visibility v) {

            uint rolledUpHeight;
            uint expandedHeight;

            if (s == manageStackPanel)
            {
                rolledUpHeight = 33;
                expandedHeight = 370;
            }
            else
                throw new UnexpectedError();

            if (v == Visibility.Collapsed)
            {
                s.Visibility = Visibility.Collapsed;
                b.Height = rolledUpHeight;
                b.CornerRadius = new CornerRadius(15, 15, 0, 0);

            }
            else if (v == Visibility.Visible) {
                s.Visibility = Visibility.Visible;
                b.Height = expandedHeight;
                b.CornerRadius = new CornerRadius(15, 15, 15, 15);
            }
        }
        private string load_sensorEditCB(string uniqueCode = "") {

            bool selectExplicit = false;
            ComboBoxItem cbi = new ComboBoxItem();
            if (uniqueCode != "" && g.isSensorInGVSS(uniqueCode))
                selectExplicit = true;

            sensorsEditCB.Items.Clear();
            List<string> allSensors = g.getAllSensors();
            foreach (string sensor in allSensors) {
                ComboBoxItem c = new ComboBoxItem();
                c.Tag = sensor;
                c.Content = sensor;
                sensorsEditCB.Items.Add(c);
                if (selectExplicit && uniqueCode == sensor)
                    cbi = c;
            }

            if (selectExplicit)
                sensorsEditCB.SelectedItem = cbi;
            else
                sensorsEditCB.SelectedIndex = 0;

            if (sensorsEditCB.Items.Count == 0)
            {
                editButton.IsEnabled = false;
                deleteButton.IsEnabled = false;
                rollUpOrExpandStackPanel(manageStackPanel,manageBorder,Visibility.Collapsed);
                return "";
            }
            else {
                editButton.IsEnabled = true;
                deleteButton.IsEnabled = true;
                rollUpOrExpandStackPanel(manageStackPanel, manageBorder, Visibility.Visible);
                return (string)(((ComboBoxItem)(sensorsEditCB.SelectedItem)).Tag);
            }
        }
        private void load_chooseEventCB(int eID = 0) {

            bool selectExplicit = false;

            if (eID > 0 && eID <= chooseEventCB.Items.Count - 1) {
                selectExplicit = true;
            }

            chooseEventCB.Items.Clear();
            uint numOfTechLines = (uint)((ComboBoxItem)techLinesCB.SelectedItem).Tag;
            OperatorLevel operatorLevel = (OperatorLevel)((ComboBoxItem)operatorLevelCB.SelectedItem).Tag;
            InformationRepresentationMethod informationRepresentationMethod = (InformationRepresentationMethod)((ComboBoxItem)informationRepresentationMethodCB.SelectedItem).Tag; 
            List <Event> events = g.getEvents(numOfTechLines, operatorLevel, informationRepresentationMethod);

            ComboBoxItem c = new ComboBoxItem();
            c.Content = DefaultSettings.getWordValue(Words.All);
            c.Tag = (int)0;
            chooseEventCB.Items.Add(c);

            ComboBoxItem cbi = new ComboBoxItem();

            foreach (Event e in events)
            {
                c = new ComboBoxItem();
                c.Tag = e;
                c.Content = e.ID + " (вершина #" + e.VertexPath[0]+")";
                chooseEventCB.Items.Add(c);
                if (selectExplicit && eID == e.ID)
                    cbi = c;
            }
            if (selectExplicit)
                chooseEventCB.SelectedItem = cbi;
            else
                chooseEventCB.SelectedIndex = 0;
        }
        private void connectAuxiliaryParamAndViolation(uint violationID, string auxiliaryParamCode, Point centerPosOfAuxiliaryParamVertex) {
            VertexNode v2 = g.connect(violationID, auxiliaryParamCode, centerPosOfAuxiliaryParamVertex);
            spawnVertex("Auxiliary", v2);
            spawnEdge(violationID, v2.VertexID);
            fillManagePanel();
            load_chooseEventCB();
        }
        private Point getVertexTopLeft(uint vID) {
            Point vPos = g.getVertexCenterPos(vID);
            vPos.X -= btnWidth / 2;
            vPos.Y -= btnHeight / 2;
            return vPos;
        }
        private void spawnVertex(string style, VertexNode tag) {

            Point vertexPosition = getVertexTopLeft(tag.VertexID);

            Button newBtn = new Button { Width = btnWidth, Height = btnHeight };
            newBtn.Style = Resources[style] as Style;

            newBtn.PreviewMouseDown += NewBtn_MouseDown;
            newBtn.PreviewMouseUp += NewBtn_MouseUp;
            newBtn.PreviewMouseMove += NewBtn_MouseMove;

            newBtn.Tag = tag;
            newBtn.Content = tag.UniqueCode;
            if (tag.UniqueCode != "Main") {
                newBtn.ContextMenu = contextMenu;
            }
            if (tag.UniqueCode != "Main")
                newBtn.Click += NewBtn_Click;
            Panel.SetZIndex(newBtn, 2);

            Canvas.SetLeft(newBtn, vertexPosition.X);
            Canvas.SetTop(newBtn, vertexPosition.Y);
            canvas.Children.Add(newBtn);

            spawnVertexNum(tag.VertexID, new Point(vertexPosition.X, vertexPosition.Y));

        }
        private void NewBtn_Click(object sender, RoutedEventArgs e)
        {
            fillManagePanel(((VertexNode)(((Button)sender).Tag)).UniqueCode);
        }
        private void spawnVertexNum(uint vID, Point pos) {

            Label num = new Label();
            num.Content = '#' + vID.ToString();
            int textLen = num.Content.ToString().Length;
            Panel.SetZIndex(num, 3);
            num.Foreground = (SolidColorBrush)this.TryFindResource("mainButtonBlue");
            num.FontWeight = FontWeights.Bold;
            num.Tag = vID;
            num.FontSize = 10;
            num.Width = btnWidth;
            num.Padding = new Thickness(btnWidth / 2 - 3 - ((textLen-2)*4), 5, 0, 0);
            num.IsHitTestVisible = false;

            Canvas.SetLeft(num, pos.X);
            Canvas.SetTop(num, pos.Y);
            canvas.Children.Add(num);

        }
        private void moveVertexNum(uint vID, Point pos) {
            Label l = new Label();
            foreach (UIElement uI in canvas.Children) {
                if (uI.GetType() == typeof(Label)) {
                    if ((uint)(((Label)uI).Tag) == vID) {
                        l = (Label)uI;
                        break;
                    }

                }
            }
            if (l == new Label())
                throw new NotFoundException();
            Canvas.SetLeft(l, pos.X);
            Canvas.SetTop(l, pos.Y);
        }
        private void setVertexNumIndex(uint vID, int index)
        {
            Label l = new Label();
            foreach (UIElement uI in canvas.Children)
            {
                if (uI.GetType() == typeof(Label))
                {
                    if ((uint)(((Label)uI).Tag) == vID)
                    {
                        l = (Label)uI;
                        break;
                    }

                }
            }
            if (l == new Label())
                throw new NotFoundException();
            Panel.SetZIndex(l, index);
        }
        private void NewBtn_MouseMove(object sender, MouseEventArgs e)
        {
            if (isPressed) {
                Button b = (Button)sender;
                Point pos1 = e.GetPosition(canvas);

                double canvasHeight = canvas.ActualHeight;
                double canvasWidth = canvas.ActualWidth;

                double left = topLeft.X + (pos1.X - pos.X);
                double top = topLeft.Y + (pos1.Y - pos.Y);


                if (left + b.Width > canvasWidth)
                    left = canvasWidth - b.Width;
                else if (left < 0)
                    left = 0;

                if (top + b.Height > canvasHeight)
                    top = canvasHeight - b.Height;

                else if (top < 0)
                    top = 0;

                Canvas.SetLeft(b, left);
                Canvas.SetTop(b, top);
                uint vID = ((VertexNode)b.Tag).VertexID;
                g.setVertexCenterPos(vID, new Point(left + (btnWidth / 2), top + (btnHeight / 2)));
                redrawEdges(vID);
                moveVertexNum(vID, new Point(left, top));

            }
        }
        private void NewBtn_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
                isPressed = false;
        }
        private void NewBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {    
            if (e.LeftButton == MouseButtonState.Pressed) {
                if (particularEventIsSelected)
                    exitParticularEvent();
                isPressed = true;
                Button b = (Button)sender;
                Panel.SetZIndex(b, ++indexCounter);
                setVertexNumIndex(((VertexNode)b.Tag).VertexID, ++indexCounter);
                topLeft = new Point(Canvas.GetLeft(b), Canvas.GetTop(b));
                pos = e.GetPosition(canvas);
            }


        }
        private void spawnEdge(uint v1ID, uint v2ID, float probability = -1f) {

            bool setToolTip = false;
            if (probability != -1f && v1ID == g.getMain().VertexID) {
                setToolTip = true;
            }

            Line line = new Line();
            Point v1Pos = g.getVertexCenterPos(v1ID);
            Point v2Pos = g.getVertexCenterPos(v2ID);

            line.X1 = v1Pos.X;
            line.Y1 = v1Pos.Y;
            line.X2 = v2Pos.X;
            line.Y2 = v2Pos.Y;
            line.Cursor = Cursors.Arrow;
            line.StrokeThickness = 2;
            line.Stroke = (Brush)new BrushConverter().ConvertFrom("#00358a");
            line.Tag = new Connection(v1ID, v2ID);
            Panel.SetZIndex(line, 1);

            if (setToolTip) {
                line.ToolTip = new ToolTip { Content = probability.ToString() };
            }

            canvas.Children.Add(line);
        }
        private void redrawEdges(uint vertexThatHaveNewPos) {
            Line l;
            Connection c;
            foreach (UIElement uIElement in canvas.Children) {
                if (uIElement.GetType() == typeof(Line)) {
                    l = (Line)uIElement;
                    c = (Connection)l.Tag;
                    if (c.v1ID == vertexThatHaveNewPos)
                    {
                        Point p = g.getVertexCenterPos(vertexThatHaveNewPos);
                        l.X1 = p.X;
                        l.Y1 = p.Y;
                    }
                    else if (c.v2ID == vertexThatHaveNewPos) {
                        Point p = g.getVertexCenterPos(vertexThatHaveNewPos);
                        l.X2 = p.X;
                        l.Y2 = p.Y;
                    }
                }
            }
        }
        private void create_GVSS() {
            g = new GVSS(mainInitialPos);
            spawnVertex("Main", g.getMain());
            programmaticallySetGVSSMode(g.getMode());

        }
        public MainWindow()
        {
            initializationFinished = false;

            InitializeComponent();

            particularEventIsSelected = false;

            InputCorrector.InitializeRelatedToSensorCBs(controlMethodCB, controlAgentCB, requestBodyCB, requestPlacementMethodCB);
            InputCorrector.InitializeTechLinesComboBox(techLinesCB, 4);
            ParameterValueHelper.InitializeComboBoxAndSelectIndex(operatorLevelCB, typeof(OperatorLevel), 3);
            ParameterValueHelper.InitializeComboBoxAndSelectIndex(informationRepresentationMethodCB, typeof(InformationRepresentationMethod));

            techLinesCB.SelectedIndex = 3;

            uniqueCodeRadioButtons = new List<RadioButton>();

            uniqueCodeRadioButtons.Add(arrowUpRB);
            uniqueCodeRadioButtons.Add(arrowDownRB);
            uniqueCodeRadioButtons.Add(arrowLeftRB);
            uniqueCodeRadioButtons.Add(arrowRightRB);

            arrowUpRB.Tag = SensorArrowSign.Up;
            arrowRightRB.Tag = SensorArrowSign.Right;
            arrowLeftRB.Tag = SensorArrowSign.Left;
            arrowDownRB.Tag = SensorArrowSign.Down;
            checkedArrowRB = SensorArrowSign.None;

            modeRadioButtons = new List<RadioButton>();

            uniqueViolationsModeRB.Tag = GVSSMode.UniqueViolations;
            repetativeViolationModeRB.Tag = GVSSMode.RepetativeViolations;

            modeRadioButtons.Add(uniqueViolationsModeRB);
            modeRadioButtons.Add(repetativeViolationModeRB);

            repetativeViolationModeRB.IsChecked = true;
            checkedModeRB = (GVSSMode)repetativeViolationModeRB.Tag;

            languageModeRadioButtons = new List<RadioButton>();
            currentLanguage = DefaultSettings.LanguageMode;

            languageModeRadioButtons.Add(ruLanRB);
            ruLanRB.Tag = LanguageMode.Russian;

            languageModeRadioButtons.Add(ukrLanRB);
            ukrLanRB.Tag = LanguageMode.Ukrainian;

            foreach (RadioButton rb in languageModeRadioButtons) {
                if ((LanguageMode)rb.Tag == currentLanguage) {
                    rb.IsChecked = true;
                    break;
                }
            }

            borderAroundUniqueCodePreview.Background = (SolidColorBrush)this.TryFindResource("disabledBlue");

            contextMenu = new ContextMenu();
            MenuItem edit = new MenuItem();
            edit.Header = DefaultSettings.getContextMenuHeaderText(ContextMenuHeaders.Edit);
            edit.Click += ContextMenuClick;
            edit.Icon = new Image { Source = new BitmapImage(new Uri("/edit.png", UriKind.Relative)), Stretch = Stretch.Fill };
            contextMenu.Items.Add(edit);

            MenuItem delete = new MenuItem();
            delete.Header = DefaultSettings.getContextMenuHeaderText(ContextMenuHeaders.DeleteVertex);
            delete.Click += ContextMenuClick;
            delete.Icon = new Image { Source = new BitmapImage(new Uri("/delete.png", UriKind.Relative)), Stretch = Stretch.Fill };
            contextMenu.Items.Add(delete);

            mainInitialPos = new Point(370, 50);
            create_GVSS();
         
            load_chooseEventCB();
            setUpInterface();

            initializationFinished = true;
            

        }
        private void ContextMenuClick(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            if (mi != null)
            {
                ContextMenu cm = mi.Parent as ContextMenu;
                if (cm != null)
                {
                    Button btn = cm.PlacementTarget as Button;
                    if (btn != null)
                    {
                        exitParticularEvent();
                        if ((string)mi.Header == DefaultSettings.getContextMenuHeaderText(ContextMenuHeaders.DeleteVertex))
                        {                         
                            if (warningMB(DefaultSettings.getMessageBoxText(MessageBoxMessages.DeleteVertex)) == MessageBoxResult.Yes)
                            {
                                deleteVertex(((VertexNode)(btn.Tag)).VertexID);
                            }
                        }
                        else if ((string)mi.Header == DefaultSettings.getContextMenuHeaderText(ContextMenuHeaders.Edit)) {
                            edit(((VertexNode)(btn.Tag)).UniqueCode);
                        }

                    }
                }
            }
        }
        private void addVertexOnCanvas(object sender, MouseButtonEventArgs e)
        {

            if (particularEventIsSelected)
            {
                canvas.Cursor = Cursors.Cross;
                exitParticularEvent();
            }
            else {
                Point curCursorPos = e.GetPosition(canvas);
                VertexDialog vd = new VertexDialog(g.getAllSensors(), g.getVertexes(), g.getAvailableParams(), g.getAllViolations(), this.checkedModeRB);
                vd.Owner = this;
                vd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                if (vd.ShowDialog() == true)
                {
                    VertexDialog.VertexDialogResult res = vd.Result;
                    if (res == VertexDialog.VertexDialogResult.AddViolation)
                    {
                        if (this.checkedModeRB == GVSSMode.UniqueViolations)
                        {
                            addViolation(vd.CapLetViolation, vd.IndexViolation, vd.DescriptionViolation, vd.ControlMethodViolation, vd.ControlAgentViolation, vd.RequestBody, vd.RequestPlacementMethod,vd.ArrowViolation, curCursorPos, vd.FailureRateViolation);
                        }
                        else if (this.checkedModeRB == GVSSMode.RepetativeViolations)
                        {
                            addViolation(vd.ViolationUniqueCode, curCursorPos,vd.FailureRateViolation);
                        }

                    }
                    else if (res == VertexDialog.VertexDialogResult.AddAuxiliaryParam)
                    {
                        try
                        {
                            connectAuxiliaryParamAndViolation(vd.ConnectedVertexID, vd.AuxiliaryParam, curCursorPos);
                        }
                        catch (Exception)
                        {
                            errorMB(DefaultSettings.getErrorMessageText(ErrorMessages.WhileAddRepetative));
                        }
                    }
                    fillManagePanel();
                }
            }        
        }
        private void arrowRB_Click(object sender, RoutedEventArgs e)
        {
            InputCorrector.arrowRB_Click(sender, e, ref checkedArrowRB);
            setRegisterAuxiliaryParamButton();
        }
        private void clearRegisterParamFields() {
            capLetTxtBox.Text = "";
            indexTxtBox.Text = "";
            auxiliaryParamRegisterDescription.Text = "";
            controlMethodCB.SelectedIndex = 0;
            controlAgentCB.SelectedIndex = 0;
        }
        private void registerButton_Click(object sender, RoutedEventArgs e)
        {
            SensorArrowSign arrowSign = SensorArrowSign.None;
            foreach (RadioButton rb in uniqueCodeRadioButtons) {
                if (rb.IsChecked == true) {
                    arrowSign = (SensorArrowSign)rb.Tag;
                    break;
                }
            }

            SensorControlMethod cm = (SensorControlMethod)(((ComboBoxItem)(controlMethodCB.SelectedItem)).Tag);
            SensorControlAgent ca = (SensorControlAgent)(((ComboBoxItem)(controlAgentCB.SelectedItem)).Tag);
            SensorRequestBodies requestBodies = (SensorRequestBodies)(((ComboBoxItem)(requestBodyCB.SelectedItem)).Tag);
            SensorRequestPlacementMethod rpm = (SensorRequestPlacementMethod)(((ComboBoxItem)(requestPlacementMethodCB.SelectedItem)).Tag);

            g.addAuxiliaryParam(capLetTxtBox.Text[0], UInt16.Parse(indexTxtBox.Text), auxiliaryParamRegisterDescription.Text,
                cm, ca, arrowSign, requestBodies, rpm);

            fillManagePanel();
            clearRegisterParamFields();
        }
        private void removeVertexes(List<uint> listOfVertexesToDelete) {
            List<UIElement> toRemove = new List<UIElement>();

            foreach (UIElement element in canvas.Children)
            {
                if (element.GetType() == typeof(Button))
                {
                    VertexNode node = (VertexNode)(((Button)element).Tag);
                    foreach (uint v in listOfVertexesToDelete)
                    {
                        if (v == node.VertexID)
                        {
                            toRemove.Add(element);
                            break;
                        }
                    }
                }
                else if (element.GetType() == typeof(Line))
                {
                    Connection connection = (Connection)(((Line)element).Tag);
                    foreach (uint v in listOfVertexesToDelete)
                    {
                        if (v == connection.v1ID)
                        {
                            toRemove.Add(element);
                        }
                        else if (v == connection.v2ID)
                        {
                            toRemove.Add(element);
                        }
                    }

                }
                else if (element.GetType() == typeof(Label))
                {
                    uint id = (uint)(((Label)element).Tag);
                    foreach (uint v in listOfVertexesToDelete)
                    {
                        if (v == id)
                        {
                            toRemove.Add(element);
                            break;
                        }
                    }
                }
            }

            foreach (UIElement o in toRemove)
            {
                canvas.Children.Remove(o);
            }

            fillManagePanel();
            load_chooseEventCB();

        }
        private void deleteVertex(uint vertexID) {
            List<uint> list = g.delete(vertexID);
            removeVertexes(list);
        }
        private void photoButton_Click(object sender, RoutedEventArgs e)
        {
            exitParticularEvent();
            SavePhoto();
            
        }
        private void modeRB_Click(object sender, RoutedEventArgs e)
        {
            GVSSMode newMode = (GVSSMode)((RadioButton)sender).Tag;
            if (newMode != checkedModeRB)
            {
                exitParticularEvent();
                bool ifNeedToChangeMode = true;
                if (newMode == GVSSMode.UniqueViolations && !g.isNull())
                {
                    if (warningMB(DefaultSettings.getMessageBoxText(MessageBoxMessages.ChangeMode)) == MessageBoxResult.No)
                    {
                        ifNeedToChangeMode = false;
                    }
                }

                if (ifNeedToChangeMode)
                {
                    List<uint> deletedVertexes = g.setMode(newMode);
                    if (newMode == GVSSMode.UniqueViolations && deletedVertexes.Count != 0)
                    {
                        removeVertexes(deletedVertexes);
                    }
                    checkedModeRB = newMode;
                }
                else
                {
                    foreach (RadioButton radio in modeRadioButtons)
                    {
                        if (radio.IsChecked == false)
                        {
                            radio.IsChecked = true;
                            break;
                        }
                    }
                }
            }
        }
        private void loadManageSensorFields(string uniqueCode) {
            InputCorrector.setSensorInfoTextBlocks(
                g.getSensor(uniqueCode),
                controlMethodInfo,
                controlAgentInfo,
                requestBodyInfo,
                requestPlacementMethodInfo,
                descriptionInfo,
                requestBodyInfoBorder,
                requestPlacementMethodInfoBorder,
                controlAgentInfoTBlock
                );
        }
        private void fillManagePanel(string uniqueCode = "") {
            string code = uniqueCode;
            if (uniqueCode == "" && sensorsEditCB.Items.Count != 0) {
                code = (string)(((ComboBoxItem)(sensorsEditCB.SelectedItem)).Tag);
            }

            code = load_sensorEditCB(code);
            if (code != "") {

                loadManageSensorFields(code);
            }
        }
        private void sensorsEditCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).Items.Count != 0)
                loadManageSensorFields((string)(((ComboBoxItem)(((ComboBox)sender).SelectedItem)).Tag));
        }
        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            exitParticularEvent();
            if (warningMB(DefaultSettings.getMessageBoxText(MessageBoxMessages.FullDeleteParam)) == MessageBoxResult.Yes) {

                removeVertexes(g.deleteAll((string)(((ComboBoxItem)(sensorsEditCB.SelectedItem)).Tag)));
                fillManagePanel("---");
                setRegisterAuxiliaryParamButton();
            }

        }
        private void edit(string uniqueCode)
        {
            EditDialog ed = new EditDialog(g.getSensor(uniqueCode));
            ed.Owner = this;
            ed.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            if (ed.ShowDialog() == true)
            {
                g.editSensor(ed.UniqueCode, ed.ControlAgent, ed.ControlMethod, ed.RequestBody, ed.RequestPlacementMethod,  ed.Description);
                fillManagePanel();
            }
        }
        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            exitParticularEvent();
            edit((string)(((ComboBoxItem)(sensorsEditCB.SelectedItem)).Tag));
        }
        private void drawParticularEvent(uint eventID) {

            List<uint> vList = new List<uint>();
            if (eventID == 0) {

                makeAllElementsOnCanvasVisible();
                return;
            }

            foreach (ComboBoxItem cbi in chooseEventCB.Items) {
                if (cbi.Tag.GetType() == typeof(Event)) {
                    Event e = (Event)(cbi.Tag);
                    if (e.ID == eventID)
                    {
                        vList.AddRange(e.VertexPath);
                        break;
                    }
                }               
            }

            if (vList == new List<uint>()) {
                throw new UnexpectedError();
            }

            foreach (UIElement element in canvas.Children) {
                bool makeHidden = true;

                if (element.GetType() == typeof(Button))
                {
                    VertexNode node = (VertexNode)(((Button)element).Tag);
                    foreach (uint v in vList)
                    {
                        if (v == node.VertexID)
                        {
                            makeHidden = false;
                            break;
                        }
                    }
                    
                }
                else if (element.GetType() == typeof(Line))
                {
                    Connection connection = (Connection)(((Line)element).Tag);
                    foreach (uint v in vList)
                    {
                        bool entered = false;
                        if (v == connection.v1ID)
                        {
                            entered = true;
                            foreach (uint v2 in vList) {
                                if (v2 == connection.v2ID) {
                                    makeHidden = false;
                                    break;
                                }
                            }
                        }
                        else if (v == connection.v2ID)
                        {
                            entered = true;
                            foreach (uint v2 in vList)
                            {
                                if (v2 == connection.v1ID)
                                {
                                    makeHidden = false;
                                    break;
                                }
                            }
                        }
                        if (entered) {
                            break;
                        }
                    }

                }
                else if (element.GetType() == typeof(Label))
                {
                    uint id = (uint)(((Label)element).Tag);
                    foreach (uint v in vList)
                    {
                        if (v == id)
                        {
                            makeHidden = false;
                            break;
                        }
                    }
                }

                if (makeHidden)
                {
                    element.Visibility = Visibility.Collapsed;
                }
                else {
                    element.Visibility = Visibility.Visible;
                }
            }

            particularEventIsSelected = true;
            canvas.Cursor = Cursors.Arrow;
           
        }
        private void makeAllElementsOnCanvasVisible() {
            canvas.Cursor = Cursors.Cross;
            particularEventIsSelected = false;
            foreach (UIElement element in canvas.Children) {
                element.Visibility = Visibility.Visible;
            }
        }
        private void chooseEventCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem cbi = (ComboBoxItem)(chooseEventCB.SelectedItem);
            uint eventID;

            if (cbi == null)
                return;

            if (cbi.Tag.GetType() == typeof(int))
            {
                eventID = 0;
                setEventsFieldsEnabled(false);
            }
            else {
                eventID = ((Event)cbi.Tag).ID;
                setEventsFieldsEnabled(true);
            }

            drawParticularEvent(eventID);

        }
        private void exitParticularEvent() {
            chooseEventCB.SelectedIndex = 0;
            particularEventIsSelected = false;
        }
        private void saveGVSS_Click(object sender, RoutedEventArgs e)
        {
            exitParticularEvent();
            SaveGVSS();
        }
        private void loadGVSS_Click(object sender, RoutedEventArgs e)
        {
            exitParticularEvent();
            LoadGVSS();
        }
        private void setEventsFieldsEnabled(bool value) {
            getTimeButton.IsEnabled = value;
        }
        private void indexTxtBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            InputCorrector.IndexTB_PreviewTextInput(sender, e);
        }
        private void capLetTxtBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            InputCorrector.CapitalLetterTB_PreviewTextInput(sender, e);
        }
        private void textBoxWithoutSpaceKey_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            InputCorrector.textBoxWithoutSpaceKey_PreviewKeyDown(sender, e);
        }
        private void auxiliaryParamRegisterDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            setRegisterAuxiliaryParamButton();
        }
        private void setRegisterAuxiliaryParamButton()
        {
            InputCorrector.setButton(registerButton,
                indexTxtBox,
                capLetTxtBox,
                auxiliaryParamRegisterDescription,
                g.getAllSensors(),
                uniqueCodeRadioButtons,
                registerAuxiliaryParamInfo);
        }
        private void nameParam_TextChanged(object sender, TextChangedEventArgs e)
        {
            InputCorrector.changeSensorNameRepresentation(capLetTxtBox,
                indexTxtBox,
                namePreviewLabel,
                uniqueCodeRadioButtons,
                borderAroundUniqueCodePreview,
                uniqueCodePreviewDock, ref checkedArrowRB);
            setRegisterAuxiliaryParamButton();

        }
        private void controlMethodCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InputCorrector.ControlMethodCB_SelectionChanged(sender, e, 
                controlAgentTBlock, 
                requestBodyCB, 
                requestBodyBorder,
                requestPlacementMethodCB,
                requestPlacementMethodBorder);
        }
        private void requestBodyCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InputCorrector.requestBodyCB_SelectionChanged(sender, e, requestPlacementMethodCB);
        }
        private void getTimeButton_Click(object sender, RoutedEventArgs e)
        {
            Event ev = (Event)(((ComboBoxItem)(chooseEventCB.SelectedItem)).Tag);
            ShowTime show = new ShowTime(ev);
            show.Owner = this;
            show.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            show.ShowDialog();

        }
        private void settingsGVSSCB_SelectionChanged(object sender, SelectionChangedEventArgs e){
            if (initializationFinished) {
                load_chooseEventCB(chooseEventCB.SelectedIndex);
            }                  
        }
        private void newGVSSButton_Click(object sender, RoutedEventArgs e)
        {
            if (endWithGVSS("Хотите ли вы сохранить эту ГВСС перед созданием новой?") == true) {
                removeVertexes(g.getVertexesIDs());
                create_GVSS();
                clearInterface();
            }
                
        }
        private bool endWithGVSS(string warningMessageText) {
            if (!g.isNull())
            {
                if (warningMB(warningMessageText) == MessageBoxResult.Yes)
                {
                    if (SaveGVSS() == false)
                        return false;
                }
            }
            return true;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (endWithGVSS(DefaultSettings.getMessageBoxText(MessageBoxMessages.DoYouWantToSaveGVSSBeforeQuit)) == false)
                e.Cancel = true;         
        }
        private List<double> FindYArray(string filePath)
        {
            if (File.Exists(filePath))
            {
                string txt = File.ReadAllText(filePath);
                txt = txt.Replace("\r\n", " ");
                Regex resultPattern = new Regex(@"y1=\[.*?\]");
                Regex numPattern = new Regex(@"(\s\d+\.\d+)");

                List<double> doubleValues = new List<double>();
                foreach (Match m in numPattern.Matches(resultPattern.Match(txt).Value))
                {
                    doubleValues.Add(Double.Parse(m.Value));
                }
                return doubleValues;
            }
            throw new UnexpectedError();
        }

        private void lanModeRB_Click(object sender, RoutedEventArgs e)
        {
            LanguageMode newLan = (LanguageMode)((RadioButton)sender).Tag;
            if (newLan != currentLanguage)
            {
                currentLanguage = newLan;
                DefaultSettings.LanguageMode = newLan;
                setUpInterface();
            }
        }

        private void setUpInterface() {
            setUpButtons();
            setUpLabels();
            setUpToolTips();
            setUpContextMenu();
            setUpComboBoxes();
            fillManagePanel();
            setRegisterAuxiliaryParamButton();
        }
        private void setUpButtons() {
            registerButton.Content = DefaultSettings.getButtonText(Buttons.Register);
            editButton.Content = DefaultSettings.getButtonText(Buttons.Edit);
            deleteButton.Content = DefaultSettings.getButtonText(Buttons.Delete);
            getTimeButton.Content = DefaultSettings.getButtonText(Buttons.GetTime);
        }
        private void setUpLabels() {
            lanModeLabel.Content = DefaultSettings.getLabelText(Labels.LanguageMode);
            registerAuxiliaryParamLabel.Content = DefaultSettings.getLabelText(Labels.RegisterAuxiliaryParam);
            letterLabel.Text = DefaultSettings.getLabelText(Labels.Letter);
            indexLabel.Text = DefaultSettings.getLabelText(Labels.Index);
            requestBodyLabel.Text = DefaultSettings.getLabelText(Labels.RequestBody);
            requestPlacementMethodLabel.Text = DefaultSettings.getLabelText(Labels.RequestPlacementMethod);
            descriptionLabel.Text = DefaultSettings.getLabelText(Labels.Description);
            managmentLabel.Content = DefaultSettings.getLabelText(Labels.Managment);
            auxiliaryParamOrViolationLabel.Content = DefaultSettings.getLabelText(Labels.AuxiliaryParamAndViolation);
            controlMethodInfoLabel.Text = DefaultSettings.getLabelText(Labels.ControlMethod);
            controlMethodLabel.Text = DefaultSettings.getLabelText(Labels.ControlMethod);
            requestBodyInfoLabel.Text = DefaultSettings.getLabelText(Labels.RequestBody);
            requestPlacementMethodInfoLabel.Text = DefaultSettings.getLabelText(Labels.RequestPlacementMethod);
            descriptionInfoLabel.Text = DefaultSettings.getLabelText(Labels.Description);
            GVSSSetUpLabel.Content = DefaultSettings.getLabelText(Labels.GVSSSettings);
            numOfTechLinesLabel.Text = DefaultSettings.getLabelText(Labels.NumOfTechLines);
            operatorLevelLabel.Text = DefaultSettings.getLabelText(Labels.OperatorLevel);
            informationRepresentationMethodLabel.Text = DefaultSettings.getLabelText(Labels.InformationRepresentationMethod);
            eventLabel.Content = DefaultSettings.getLabelText(Labels.Event);
            chooseEventLabel.Content = DefaultSettings.getLabelText(Labels.ChooseEvent);

            SensorControlMethod cm = (SensorControlMethod)(((ComboBoxItem)(controlMethodCB.SelectedItem)).Tag);

            if (cm == SensorControlMethod.Auto)
            {
                controlAgentTBlock.Text = DefaultSettings.getLabelText(Labels.ControlAgentAuto);

            }
            else if (cm == SensorControlMethod.Request)
            {
                controlAgentTBlock.Text = DefaultSettings.getLabelText(Labels.ControlAgentRequest);
            }

        }
        private void setUpToolTips() {
            uniqueViolationsModeRB.ToolTip = new TextBlock { Text = DefaultSettings.getToolTipText(ToolTips.ViolationsAreUnique), Width = 180, FontSize = 14, FontWeight = FontWeights.DemiBold, TextWrapping = TextWrapping.Wrap};
            repetativeViolationModeRB.ToolTip = new TextBlock { Text = DefaultSettings.getToolTipText(ToolTips.ViolationsCanBeParams), Width = 170, FontSize = 14, FontWeight = FontWeights.DemiBold, TextWrapping = TextWrapping.Wrap };

            saveGVSS.ToolTip = DefaultSettings.getToolTipText(ToolTips.SaveGVSS);
            loadGVSS.ToolTip = DefaultSettings.getToolTipText(ToolTips.LoadGVSS);
            newGVSSButton.ToolTip = DefaultSettings.getToolTipText(ToolTips.CreateNewGVSS);
            photoButton.ToolTip = DefaultSettings.getToolTipText(ToolTips.SavePhoto);
        }
        private void setUpContextMenu() {
            foreach (UIElement uIElement in canvas.Children) {
                if (uIElement.GetType() == typeof(Button)) {
                    Button b = (Button)uIElement;
                    if (b.Style == Resources["Main"] as Style)
                        continue;
                    ((MenuItem)b.ContextMenu.Items[0]).Header = DefaultSettings.getContextMenuHeaderText(ContextMenuHeaders.Edit);
                    ((MenuItem)b.ContextMenu.Items[1]).Header = DefaultSettings.getContextMenuHeaderText(ContextMenuHeaders.DeleteVertex);
                }
            }
        }
        private void setUpComboBoxes() {
            InputCorrector.LocalizeComboBoxes(controlMethodCB, controlAgentCB, requestBodyCB, requestPlacementMethodCB, informationRepresentationMethodCB);
            ((ComboBoxItem)chooseEventCB.Items[0]).Content = DefaultSettings.getWordValue(Words.All);
        }
    }
}
