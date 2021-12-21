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
    /// <summary>
    /// Interaction logic for EditDialog.xaml
    /// </summary>
    /// 
    public partial class EditDialog : Window
    {
        public SensorControlMethod ControlMethod { get; private set; }
        public SensorControlAgent ControlAgent { get; private set; }
        public SensorRequestBodies RequestBody { get; set; }
        public SensorRequestPlacementMethod RequestPlacementMethod { get; set; }
        public string Description { get; private set; }
        public readonly string UniqueCode;

        private SensorControlMethod ControlMethodDef { get; set; }
        private SensorControlAgent ControlAgentDef { get; set; }
        private SensorRequestBodies RequestBodyDef { get; set; }
        private SensorRequestPlacementMethod RequestPlacementMethodDef { get; set; }
        private string DescriptionDef { get; set; }

        public EditDialog(Sensor sensor)
        {
            InitializeComponent();
            LocalizeWindow();

            UniqueCode = sensor.UniqueCode;

            ControlAgentDef = sensor.ControlAgent;
            ControlMethodDef = sensor.ControlMethod;
            DescriptionDef = sensor.Description;
            RequestPlacementMethodDef = sensor.RequestPlacementMethod;
            RequestBodyDef = sensor.RequestBody;

            InputCorrector.InitializeRelatedToSensorCBs(controlMethodCB, controlAgentCB, requestBodyCB, requestPlacementMethodCB);

            uniqueCodeLabel.Content = sensor.UniqueCode;

            foreach (ComboBoxItem cbi in controlMethodCB.Items) { 
                if((SensorControlMethod)(cbi.Tag) == sensor.ControlMethod){
                    controlMethodCB.SelectedItem = cbi;
                    break;
                }
            }

            foreach (ComboBoxItem cbi in controlAgentCB.Items){
                if ((SensorControlAgent)(cbi.Tag) == sensor.ControlAgent){
                    controlAgentCB.SelectedItem = cbi;
                    break;
                }
            }

            if (sensor.ControlMethod == SensorControlMethod.Request) {
                foreach (ComboBoxItem cbi in requestBodyCB.Items)
                {
                    if ((SensorRequestBodies)(cbi.Tag) == sensor.RequestBody)
                    {
                        requestBodyCB.SelectedItem = cbi;
                        break;
                    }
                }
                foreach (ComboBoxItem cbi in requestPlacementMethodCB.Items)
                {
                    if ((SensorRequestPlacementMethod)(cbi.Tag) == sensor.RequestPlacementMethod)
                    {
                        requestPlacementMethodCB.SelectedItem = cbi;
                        break;
                    }
                }
            }
            

            descriptionTB.Text = sensor.Description;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ControlMethod = (SensorControlMethod)(((ComboBoxItem)(controlMethodCB.SelectedItem)).Tag);
            ControlAgent = (SensorControlAgent)(((ComboBoxItem)(controlAgentCB.SelectedItem)).Tag);
            Description = descriptionTB.Text;
            RequestBody = (SensorRequestBodies)(((ComboBoxItem)(requestBodyCB.SelectedItem)).Tag);
            RequestPlacementMethod = (SensorRequestPlacementMethod)(((ComboBoxItem)(requestPlacementMethodCB.SelectedItem)).Tag);

            this.DialogResult = true;
            this.Close();

        }
        private void controlMethodCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InputCorrector.ControlMethodCB_SelectionChanged(sender, e,
                controlAgentTBlock,
                requestBodyCB,
                requestBodyBorder,
                requestPlacementMethodCB,
                requestPlacementMethodBorder);
            setButton();
        }
        private void requestBodyCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InputCorrector.requestBodyCB_SelectionChanged(sender, e, requestPlacementMethodCB);
            setButton();
        }
        private void cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            setButton();
        }
        private void setButton() {
            if (controlMethodCB.SelectedItem != null &&
                controlAgentCB.SelectedItem != null &&
                requestBodyCB.SelectedItem != null &&
                requestPlacementMethodCB.SelectedItem != null) {

                bool isControlMethodNew = ((SensorControlMethod)((ComboBoxItem)(controlMethodCB.SelectedItem)).Tag) != ControlMethodDef;
                bool isControlAgentNew = ((SensorControlAgent)((ComboBoxItem)(controlAgentCB.SelectedItem)).Tag) != ControlAgentDef;
                bool isRequestBodyNew = ((SensorRequestBodies)((ComboBoxItem)(requestBodyCB.SelectedItem)).Tag) != RequestBodyDef;
                bool isRequestPlacementMethodNew = ((SensorRequestPlacementMethod)((ComboBoxItem)(requestPlacementMethodCB.SelectedItem)).Tag) != RequestPlacementMethodDef;
                bool isDescriptionNew = DescriptionDef != descriptionTB.Text;

                if ((isControlMethodNew || isControlAgentNew || isRequestBodyNew || isRequestPlacementMethodNew || isDescriptionNew) && descriptionTB.Text.Length != 0)
                {
                    changeButton.IsEnabled = true;
                }
                else
                {
                    changeButton.IsEnabled = false;
                }

            }
            
        }
        private void LocalizeWindow()
        {
            this.Title = DefaultSettings.getWindowTitleText(WindowTitles.Edit);
            changeButton.Content = DefaultSettings.getButtonText(Buttons.Change);
            controlMethodLabel.Text = DefaultSettings.getLabelText(Labels.ControlMethod);
            requestBodyLabel.Text = DefaultSettings.getLabelText(Labels.RequestBody);
            requestPlacementMethodLabel.Text = DefaultSettings.getLabelText(Labels.RequestPlacementMethod);
            descriptionLabel.Text = DefaultSettings.getLabelText(Labels.Description);
        }

        private void descriptionTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            setButton();
        }
    }
}
