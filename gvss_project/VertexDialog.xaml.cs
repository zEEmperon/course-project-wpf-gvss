using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace gvss_project
{
    public partial class VertexDialog : Window
    {
        public enum VertexDialogResult {JustClosed, AddViolation, AddAuxiliaryParam};

        private const uint defWindowHeight = 150;
        private const uint addViolationWindowHeight = 605;
        private const uint addAuxiliaryParamWindowHeight = 280;
        private const uint addRepetativeViolationWindowHeight = 280;

        public string AuxiliaryParam { get; private set; }
        public uint ConnectedVertexID { get; private set; }

        public float FailureRateViolation { get; private set; }
        public char CapLetViolation { get; private set; }
        public uint IndexViolation { get; private set; }
        public string DescriptionViolation { get; private set; }
        public SensorArrowSign ArrowViolation { get; private set; }
        public SensorControlAgent ControlAgentViolation { get; private set; }
        public SensorControlMethod ControlMethodViolation { get; private set; }
        public SensorRequestBodies RequestBody { get; set; }
        public SensorRequestPlacementMethod RequestPlacementMethod { get; set; }

        public string ViolationUniqueCode { get; private set; }

        public VertexDialogResult Result { get; private set; }
        private enum ChooseAction { Violation, AuxiliaryParam, None};

        private List<RadioButton> chooseActionRBs;
        private ChooseAction actionRB;
        private SensorArrowSign checkedArrowRB;
        private List<RadioButton> uniqueCodeRadioButtons;
        private Border violationBorder;
        private uint addViolationSize;

        private List<VertexNode> potentialVertexes;
        private List<string> allSensors;
        private GVSSMode mode;
        public VertexDialog(List<string> allSensors, List<VertexNode> potentialVertexes, List<string> allAuxiliaryParams, List<string> allViolations, GVSSMode mode)
        {
            InitializeComponent();
            LocalizeWindow();

            this.mode = mode;
            this.Height = defWindowHeight;

            this.allSensors = allSensors;
            this.potentialVertexes = potentialVertexes;

            if (mode == GVSSMode.RepetativeViolations)
            {
                violationBorder = addRepetativeViolationBorder;
                addViolationSize = addRepetativeViolationWindowHeight;
            }
            else if (mode == GVSSMode.UniqueViolations) {

                violationBorder = addViolationBorder;
                addViolationSize = addViolationWindowHeight;
            }

            violationRB.Tag = ChooseAction.Violation;
            auxiliaryParamRB.Tag = ChooseAction.AuxiliaryParam;
            actionRB = ChooseAction.None;

            chooseActionRBs = new List<RadioButton>();
            chooseActionRBs.Add(violationRB);
            chooseActionRBs.Add(auxiliaryParamRB);

            foreach (string auxiliary in allAuxiliaryParams) {
                bool isAlreadyExist = false;
                foreach (string violation in allViolations)
                {
                    if (violation == auxiliary) {
                        isAlreadyExist = true;
                        break;
                    }
                }
                if (!isAlreadyExist) {
                    ComboBoxItem sensor = new ComboBoxItem();
                    sensor.Tag = auxiliary;
                    sensor.Content = auxiliary;
                    repetativeViolationsCB.Items.Add(sensor);
                }
            }
            repetativeViolationsCB.SelectedIndex = 0;
            setAddRepetativeViolationButton();



            foreach (string sensorName in allAuxiliaryParams)
            {
                ComboBoxItem sensor = new ComboBoxItem();
                sensor.Tag = sensorName;
                sensor.Content = sensorName;
                auxiliaryParamCB.Items.Add(sensor);

            }
            auxiliaryParamCB.SelectedIndex = 0;
            if (auxiliaryParamCB.Items.Count == 0)
            {
                addAuxiliaryParamToVertex.IsEnabled = false;
                loadPotentialVertexes("");
            }
            else {
                loadPotentialVertexes((string)(((ComboBoxItem)(auxiliaryParamCB.SelectedItem)).Tag));
            }

            InputCorrector.InitializeRelatedToSensorCBs(controlMethodCB, controlAgentCB, requestBodyCB, requestPlacementMethodCB);

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

            borderAroundUniqueCodePreview.Background = (SolidColorBrush)this.TryFindResource("disabledBlue");

        }
        private void chooseRB_Click(object sender, RoutedEventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if ((ChooseAction)rb.Tag == actionRB) {
                rb.IsChecked = false;
                actionRB = ChooseAction.None;

                violationBorder.Visibility = Visibility.Collapsed;
                addAuxiliaryParamBorder.Visibility = Visibility.Collapsed;
                this.Height = defWindowHeight;

            }
            else
            {
                actionRB = (ChooseAction)rb.Tag;
                if (actionRB == ChooseAction.Violation)
                {
                    this.Height = addViolationSize;
                    violationBorder.Visibility = Visibility.Visible;
                    addAuxiliaryParamBorder.Visibility = Visibility.Collapsed;
                }
                else if (actionRB == ChooseAction.AuxiliaryParam) {
                    this.Height = addAuxiliaryParamWindowHeight;
                    violationBorder.Visibility = Visibility.Collapsed;
                    addAuxiliaryParamBorder.Visibility = Visibility.Visible;

                }
            }
        }
        private void arrowRB_Click(object sender, RoutedEventArgs e)
        {
            InputCorrector.arrowRB_Click(sender, e, ref checkedArrowRB);
            setViolationButton();
        }
        private void setViolationButton()
        {
            InputCorrector.setButton(addViolationButton,
                 indexTxtBox,
                 capLetTxtBox,
                 addViolationDescription,
                 allSensors,
                 uniqueCodeRadioButtons,
                 addViolationInfo,
                 uniqueViolationFailureTB);
        }
        private void addViolationButton_Click(object sender, RoutedEventArgs e)
        {
            SensorArrowSign arrowSign = SensorArrowSign.None;
            foreach (RadioButton rb in uniqueCodeRadioButtons)
            {
                if (rb.IsChecked == true)
                {
                    arrowSign = (SensorArrowSign)rb.Tag;
                    break;
                }
            }

            ControlMethodViolation = (SensorControlMethod)(((ComboBoxItem)(controlMethodCB.SelectedItem)).Tag);
            ControlAgentViolation = (SensorControlAgent)(((ComboBoxItem)(controlAgentCB.SelectedItem)).Tag);
            RequestBody = (SensorRequestBodies)(((ComboBoxItem)(requestBodyCB.SelectedItem)).Tag);
            RequestPlacementMethod = (SensorRequestPlacementMethod)(((ComboBoxItem)(requestPlacementMethodCB.SelectedItem)).Tag);
            CapLetViolation = capLetTxtBox.Text[0];
            IndexViolation = UInt16.Parse(indexTxtBox.Text);
            DescriptionViolation = addViolationDescription.Text;
            ArrowViolation = arrowSign;
            FailureRateViolation = float.Parse(uniqueViolationFailureTB.Text);
            

            Result = VertexDialogResult.AddViolation;

            this.DialogResult = true;
            this.Close();          
        }
        private void loadPotentialVertexes(string exceptUniqueCode) {
            string selectedItemVertexID = "----";
            if (potentialVertexesCB.Items.Count != 0) {
                selectedItemVertexID = ((VertexNode)((ComboBoxItem)potentialVertexesCB.SelectedItem).Tag).VertexID.ToString();
            }           
            potentialVertexesCB.Items.Clear();

            int selectedIndex = 0;

            foreach (VertexNode v in potentialVertexes) {
                if (v.UniqueCode != exceptUniqueCode) {
                    ComboBoxItem vertex = new ComboBoxItem();
                    vertex.Tag = v;
                    vertex.Content = "Вершина №" + v.VertexID + " (" + v.UniqueCode + ")";
                    potentialVertexesCB.Items.Add(vertex);
                    if (v.VertexID.ToString() == selectedItemVertexID) {
                        selectedIndex = potentialVertexesCB.Items.Count - 1;
                    }
                }
            }
            potentialVertexesCB.SelectedIndex = selectedIndex;

            if (potentialVertexesCB.Items.Count == 0 || auxiliaryParamCB.Items.Count == 0)
            {
                addAuxiliaryParamToVertex.IsEnabled = false;
            }
            else if (potentialVertexesCB.Items.Count > 0 && auxiliaryParamCB.Items.Count > 0) {
                addAuxiliaryParamToVertex.IsEnabled = true;
            }
        }
        private void auxiliaryParamCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            loadPotentialVertexes((string)((ComboBoxItem)e.AddedItems[0]).Tag);
        }
        private void addAuxiliaryParamToVertex_Click(object sender, RoutedEventArgs e)
        {
            AuxiliaryParam = (string)(((ComboBoxItem)auxiliaryParamCB.SelectedItem).Tag);
            ConnectedVertexID = ((VertexNode)(((ComboBoxItem)potentialVertexesCB.SelectedItem).Tag)).VertexID;

            Result = VertexDialogResult.AddAuxiliaryParam;

            this.DialogResult = true;
            this.Close();
        }
        private void addRepetativeViolationButton_Click(object sender, RoutedEventArgs e)
        {
            ViolationUniqueCode = (string)((ComboBoxItem)(repetativeViolationsCB.SelectedItem)).Tag;
            FailureRateViolation = float.Parse(failureRateTB.Text);
            Result = VertexDialogResult.AddViolation;

            this.DialogResult = true;
            this.Close();
        }
        private void setAddRepetativeViolationButton() {
            if (repetativeViolationsCB.Items.Count != 0 && InputCorrector.isFailureRateTextBoxValid(failureRateTB))
                addRepetativeViolationButton.IsEnabled = true;
            else
                addRepetativeViolationButton.IsEnabled = false;
        }
        private void failureRateTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            string name = ((TextBox)(sender)).Name;
            if (name == "failureRateTB")
            {
                setAddRepetativeViolationButton();
            }
            else if (name == "uniqueViolationFailureTB") {
                setViolationButton();
            }
            
        }
        private void uniqueViolationFailureTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            InputCorrector.FailureRateTB_PreviewTextInput(sender, e);
            setViolationButton();
        }
        private void textBoxWithoutSpaceKey_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            InputCorrector.textBoxWithoutSpaceKey_PreviewKeyDown(sender, e);
        }
        private void addViolationDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            setViolationButton();
        }
        private void nameParam_TextChanged(object sender, TextChangedEventArgs e)
        {
            InputCorrector.changeSensorNameRepresentation(capLetTxtBox,
                indexTxtBox,
                namePreviewLabel,
                uniqueCodeRadioButtons,
                borderAroundUniqueCodePreview,
                uniqueCodePreviewDock, ref checkedArrowRB);
            setViolationButton();

        }
        private void capLetTxtBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            InputCorrector.CapitalLetterTB_PreviewTextInput(sender, e);
        }
        private void indexTxtBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            InputCorrector.IndexTB_PreviewTextInput(sender, e);
        }
        private void failureRateTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            InputCorrector.FailureRateTB_PreviewTextInput(sender, e);
            setAddRepetativeViolationButton();
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
        private void LocalizeWindow()
        {
            this.Title = DefaultSettings.getWindowTitleText(WindowTitles.Add);
            addAuxiliaryParamToVertex.Content = DefaultSettings.getButtonText(Buttons.Add);
            addViolationButton.Content = DefaultSettings.getButtonText(Buttons.Add);
            addRepetativeViolationButton.Content = DefaultSettings.getButtonText(Buttons.Add);
            chooseActionLabel.Content = DefaultSettings.getLabelText(Labels.ChooseAction);
            violationRB.Content = DefaultSettings.getWordValue(Words.AddViolation);
            auxiliaryParamRB.Content = DefaultSettings.getWordValue(Words.AddAuxiliaryParamToVertex);
            addViolationLabel.Content = DefaultSettings.getLabelText(Labels.AddViolation);
            violationLabel.Text = DefaultSettings.getLabelText(Labels.Violation);
            failureRateLabel.Text = DefaultSettings.getLabelText(Labels.FailureRate);
            addUniqueViolationLabel.Content = DefaultSettings.getLabelText(Labels.AddViolation);
            letterLabel.Text = DefaultSettings.getLabelText(Labels.Letter);
            indexLabel.Text = DefaultSettings.getLabelText(Labels.Index);
            requestBodyLabel.Text = DefaultSettings.getLabelText(Labels.RequestBody);
            requestPlacementMethodLabel.Text = DefaultSettings.getLabelText(Labels.RequestPlacementMethod);
            failureRateUniqueLabel.Text = DefaultSettings.getLabelText(Labels.FailureRate);
            descriptionLabel.Text = DefaultSettings.getLabelText(Labels.Description);
            addAuxiliaryParamToVertexLabel.Content = DefaultSettings.getLabelText(Labels.AddAuxiliaryParamToVertex);
            auxiliaryParamLabel.Text = DefaultSettings.getLabelText(Labels.AuxiliaryParam);
            controlMethodLabel.Text = DefaultSettings.getLabelText(Labels.ControlMethod);           
        }
    }
}
