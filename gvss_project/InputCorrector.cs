using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace gvss_project
{
    //public enum TextBoxType { Description, FailureRate, None};
    public static class InputCorrector
    {
        private enum Errors { Name, NotFilled, AllEmpty, None}
        private static ErrorWhileInput getError(Errors error) {
            SolidColorBrush redBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9c0b4a"));
            if (error == Errors.Name)
                return new ErrorWhileInput { Message = DefaultSettings.getErrorMessageText(ErrorMessages.TheNameExistsAlready), Brush = redBrush, isBtnEnabled = false };
            else if(error == Errors.NotFilled)
                return new ErrorWhileInput { Message = DefaultSettings.getErrorMessageText(ErrorMessages.NotAllFieldsAreFilled), Brush = redBrush, isBtnEnabled = false };
            else if (error == Errors.AllEmpty)
                return new ErrorWhileInput { Message = "", isBtnEnabled = false };
            else
                return new ErrorWhileInput { Message = "", isBtnEnabled = true };
        }
        private struct ErrorWhileInput {
            public string Message;
            public Brush Brush;
            public bool isBtnEnabled;
        }

        private static bool isArrowRBSelected(List<RadioButton> arrowRadioButtons, ref SensorArrowSign resultArrowSign) {
            foreach (RadioButton rb in arrowRadioButtons)
            {
                if (rb.IsChecked == true)
                {
                    resultArrowSign = (SensorArrowSign)rb.Tag;
                    return true;
                }
            }
            return false;
        }
        private static bool isMatch(string text, string pattern) {
            return System.Text.RegularExpressions.Regex.IsMatch(text, pattern);
        }
        private static bool isFloatValue(string text)
        {
            if (text.Length == 1 && text[0] == '0')
                return false;
            if (text.Length >= 2 && text[0] == '0' && text[1] != '.')
                return false;
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"^\d*\.?\d+$");
            return regex.IsMatch(text);
        }
        private static void setRequestCBs(bool isEnabled, ComboBox requestBodyCB, Border requestBodyBorder, ComboBox requestPlacementMethodCB, Border requestPlacementMethodBorder) {
            Brush brush;
            if (isEnabled)
            {
                brush = (SolidColorBrush)App.Current.TryFindResource("defBlue");
                ParameterValueHelper.InitializeComboBoxAndSelectIndex(requestBodyCB, typeof(SensorRequestBodies));

            }
            else {
                brush = (SolidColorBrush)App.Current.TryFindResource("disabledBlue");
                ParameterValueHelper.InitializeComboBoxWithNoneAndSelectIndex(requestBodyCB, typeof(SensorRequestBodies));
            }
                

            requestBodyBorder.Background = brush;
            requestPlacementMethodBorder.Background = brush; 
            requestBodyCB.IsEnabled = isEnabled;
            requestPlacementMethodCB.IsEnabled = isEnabled;
        }

        private static bool isFailureRateTextBoxINullOrValid(TextBox textBox) {
            return (textBox == null) || (textBox.Text.Length != 0 && isFloatValue(textBox.Text)); 
        }
        private static bool isFailureRateTextBoxINullOrNotValid(TextBox textBox)
        {
            return (textBox == null) || !(textBox.Text.Length != 0 && isFloatValue(textBox.Text));
        }
        public static bool isFailureRateTextBoxValid(TextBox textBox) {
            return textBox.Text.Length != 0 && isFloatValue(textBox.Text);
        }

        public static void InitializeRelatedToSensorCBs(ComboBox controlMethodCB, ComboBox controlAgentCB, ComboBox requestBodyCB, ComboBox requestPlacementMethodCB) {
            ParameterValueHelper.InitializeComboBox(controlMethodCB, typeof(SensorControlMethod));
            ParameterValueHelper.InitializeComboBox(controlAgentCB, typeof(SensorControlAgent));
            ParameterValueHelper.InitializeComboBoxWithNone(requestBodyCB, typeof(SensorRequestBodies));
            ParameterValueHelper.InitializeComboBoxWithNone(requestPlacementMethodCB, typeof(SensorRequestPlacementMethod));

            controlAgentCB.SelectedIndex = 0;
            controlMethodCB.SelectedIndex = 0;
            
            requestBodyCB.SelectedIndex = 0;
            requestPlacementMethodCB.SelectedIndex = 0;

        }
        public static void textBoxWithoutSpaceKey_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;

        }
        public static void IndexTB_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            if ((e.Text == "0" && ((TextBox)sender).Text.Length == 0) || !isMatch(e.Text, "^[0-9]"))
            {
                e.Handled = true;

            }
        }
        public static void CapitalLetterTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!isMatch(e.Text, "^[АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюяІіЇїa-zA-Z]"))
            {
                e.Handled = true;

            }
            else {
                ((TextBox)sender).Text = e.Text[0].ToString().ToUpper();
            }    
        }
        public static void FailureRateTB_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            if (!isMatch(e.Text, "^[.0-9]"))
            {
                e.Handled = true;

            }
        }
        public static void changeSensorNameRepresentation(TextBox capitalLetterTextBox, TextBox indexTextBox, Label namePreviewLabel, List<RadioButton> arrowRadioButtons, Border backgroundBorder, DockPanel uniqueCodePreviewDock, ref SensorArrowSign checkedArrowRB) {
            if (capitalLetterTextBox.Text.Length > 0 && indexTextBox.Text.Length > 0)
            {
                foreach (RadioButton rb in arrowRadioButtons)
                {
                    rb.IsEnabled = true;
                }
                backgroundBorder.Background = (SolidColorBrush)App.Current.TryFindResource("blueBorderBrush");

                int length = indexTextBox.Text.Length;
                int width = 0;
                int paddingLeftOffset = 0;

                if (length == 1)
                {
                    paddingLeftOffset = 8;
                    width = 58;
                }
                else if (length == 2)
                {
                    paddingLeftOffset = 5;
                    width = 64;
                }
                else if (length == 3)
                {
                    paddingLeftOffset = 2;
                    width = 70;
                }

                uniqueCodePreviewDock.Width = width;
                namePreviewLabel.Padding = new Thickness(paddingLeftOffset, 0, 0, 0);
                namePreviewLabel.Content = Computations.getCapLetAndIndex(capitalLetterTextBox.Text[0], Int32.Parse(indexTextBox.Text));

            }
            else
            {
                int width = 56;
                namePreviewLabel.Content = "";
                uniqueCodePreviewDock.Width = width;

                backgroundBorder.Background = (SolidColorBrush)App.Current.TryFindResource("disabledBlue");

                foreach (RadioButton rb in arrowRadioButtons)
                {
                    rb.IsChecked = false;
                    rb.IsEnabled = false;
                }

                checkedArrowRB = SensorArrowSign.None;
            }
        }
        public static void arrowRB_Click(object sender, RoutedEventArgs e, ref SensorArrowSign checkedArrowRB)
        {
            RadioButton rb = (RadioButton)sender;
            SensorArrowSign rbArrow = (SensorArrowSign)rb.Tag;
            if (rbArrow == checkedArrowRB)
            {
                rb.IsChecked = false;
                checkedArrowRB = SensorArrowSign.None;
            }
            else
            {
                checkedArrowRB = (SensorArrowSign)rb.Tag;
            }
        }
        public static void setButton(Button button, TextBox indexTextBox, TextBox capitalLetterTextBox, TextBox descriptionTextBox, List<string> sensors,List<RadioButton> arrowRadioButtons, TextBlock resultTextBlock, TextBox failureTextBox = null) {

            SensorArrowSign arrowSign = SensorArrowSign.None;
            Errors error = Errors.NotFilled;

            bool isArrowSelected = isArrowRBSelected(arrowRadioButtons, ref arrowSign);
            bool isIndexTBValid = indexTextBox.Text.Length != 0;
            bool isCapitalLetterTBValid = capitalLetterTextBox.Text.Length != 0;
            bool isDescriptionTBValid = descriptionTextBox.Text.Length != 0;

            bool isFailureRateTBNullOrValid = isFailureRateTextBoxINullOrValid(failureTextBox);
            bool isFailureRateTBNullOrNotValid = isFailureRateTextBoxINullOrNotValid(failureTextBox);
            bool isAllEmpty = !isArrowSelected && !isIndexTBValid && !isCapitalLetterTBValid && !isDescriptionTBValid;

            if (isArrowSelected && isIndexTBValid && isCapitalLetterTBValid)
            {
                bool isSensorNameAvailable = true;
                string curCode = Computations.getUniqueCode(capitalLetterTextBox.Text[0], UInt16.Parse(indexTextBox.Text), arrowSign);

                foreach (string ucode in sensors)
                {
                    if (ucode == curCode)
                    {
                        isSensorNameAvailable = false;
                        break;
                    }
                }


                if (!isSensorNameAvailable)
                    error = Errors.Name;
                else if (isDescriptionTBValid && isFailureRateTBNullOrValid)
                    error = Errors.None;
                else if (!isDescriptionTBValid || !isFailureRateTBNullOrValid)
                    error = Errors.NotFilled;

            }
            else if (isAllEmpty && isFailureRateTBNullOrNotValid)
                error = Errors.AllEmpty;

            ErrorWhileInput errorWhileInput = getError(error);

            resultTextBlock.Text = errorWhileInput.Message;
            resultTextBlock.Foreground = errorWhileInput.Brush;
            button.IsEnabled = errorWhileInput.isBtnEnabled;
        }
        public static void ControlMethodCB_SelectionChanged(
            object sender, 
            SelectionChangedEventArgs e,
            TextBlock labelTextBlock,
            ComboBox requestBodyCB,            
            Border requestBodyBorder,
            ComboBox requestPlacementMethodCB,
            Border requestPlacementMethodBorder) {
            
            SensorControlMethod cm = (SensorControlMethod)(((ComboBoxItem)(((ComboBox)sender).SelectedItem)).Tag);

            string labelText = "";

            bool isRequest = false;

            if (cm == SensorControlMethod.Auto)
            {
                labelText = DefaultSettings.getLabelText(Labels.ControlAgentAuto);
                
            }
            else if (cm == SensorControlMethod.Request)
            {
                labelText = DefaultSettings.getLabelText(Labels.ControlAgentRequest);
                isRequest = true;
            }
            else
                throw new UnexpectedError();

            labelTextBlock.Text = labelText;
            setRequestCBs(isRequest, requestBodyCB, requestBodyBorder, requestPlacementMethodCB, requestPlacementMethodBorder);

        }

        public static void requestBodyCB_SelectionChanged(object sender, SelectionChangedEventArgs e, ComboBox requestPlacementMethodCB) {
            if (((ComboBox)sender).Items.Count != 0)
            {
                SensorRequestBodies rb = (SensorRequestBodies)(((ComboBoxItem)(((ComboBox)sender).SelectedItem)).Tag);
                ParameterValueHelper.InitializeRequestPlacementMethod(requestPlacementMethodCB, rb);
            }
            else {
                ParameterValueHelper.InitializeComboBoxWithNoneAndSelectIndex(requestPlacementMethodCB, typeof(SensorRequestPlacementMethod));
            }

        }
        public static void setSensorInfoTextBlocks(
            Sensor s,
            TextBlock controlMethodInfo,
            TextBlock controlAgentInfo,
            TextBlock requestBodyInfo,
            TextBlock requestPlacementMethodInfo,
            TextBlock descriptionInfo,
            Border requestBodyInfoBorder,
            Border requestPlacementMethodInfoBorder,
            TextBlock controlAgentInfoTBlock)
        {
            controlAgentInfo.Text = DefaultSettings.getControlAgentText(s.ControlAgent);
            controlMethodInfo.Text = DefaultSettings.getControlMethodText(s.ControlMethod);

            if (s.ControlMethod == SensorControlMethod.Request)
            {
                Brush b = (SolidColorBrush)App.Current.TryFindResource("defBlue");
                controlAgentInfoTBlock.Text = DefaultSettings.getLabelText(Labels.ControlAgentRequest);
                requestBodyInfoBorder.Background = b;
                requestPlacementMethodInfoBorder.Background = b;
                requestBodyInfo.Text = DefaultSettings.getRequestBodyText(s.RequestBody);
                requestPlacementMethodInfo.Text = DefaultSettings.getRequestPlacementMethodText(s.RequestPlacementMethod);
            }
            else {
                Brush b = (SolidColorBrush)App.Current.TryFindResource("disabledBlue");
                controlAgentInfoTBlock.Text = DefaultSettings.getLabelText(Labels.ControlAgentAuto);
                requestBodyInfoBorder.Background = b;
                requestPlacementMethodInfoBorder.Background = b;
                requestBodyInfo.Text = "";
                requestPlacementMethodInfo.Text = "";
            }


            descriptionInfo.Text = s.Description;

        }

        public static void InitializeTechLinesComboBox(ComboBox techLinesCB, int numOfTechLines)
        {
            if (numOfTechLines > 0)
            {
                techLinesCB.Items.Clear();
                foreach (uint i in Enumerable.Range(1, numOfTechLines))
                {
                    ComboBoxItem cbi = new ComboBoxItem();
                    cbi.Tag = i;
                    cbi.Content = i.ToString();
                    techLinesCB.Items.Add(cbi);
                }
                techLinesCB.SelectedIndex = 0;

            }

        }
        public static void LocalizeComboBox(ComboBox cb, Type innerType) {
            if (cb.Items.Count == 0)
                return;

            if (innerType == typeof(SensorControlMethod))
            {
                foreach (ComboBoxItem cbi in cb.Items) {
                    cbi.Content = DefaultSettings.getControlMethodText((SensorControlMethod)cbi.Tag);
                }
            }
            else if (innerType == typeof(SensorControlAgent)) 
            {
                foreach (ComboBoxItem cbi in cb.Items)
                {
                    cbi.Content = DefaultSettings.getControlAgentText((SensorControlAgent)cbi.Tag);
                }
            }
            else if (innerType == typeof(SensorRequestPlacementMethod))
            {
                foreach (ComboBoxItem cbi in cb.Items)
                {
                    cbi.Content = DefaultSettings.getRequestPlacementMethodText((SensorRequestPlacementMethod)cbi.Tag);
                }
            }

            else if (innerType == typeof(SensorRequestBodies))
            {
                foreach (ComboBoxItem cbi in cb.Items)
                {
                    cbi.Content = DefaultSettings.getRequestBodyText((SensorRequestBodies)cbi.Tag);
                }
            }
            else if (innerType == typeof(InformationRepresentationMethod))
            {
                foreach (ComboBoxItem cbi in cb.Items)
                {
                    cbi.Content = DefaultSettings.getInformationRepresentationMethodText((InformationRepresentationMethod)cbi.Tag);
                }
            }

        }
        public static void LocalizeComboBoxes(ComboBox controlMethod, ComboBox controlAgent, ComboBox requestBody, ComboBox requestPlacementMethod, ComboBox informationRepresentationMethod) {
            LocalizeComboBox(controlMethod, typeof(SensorControlMethod));
            LocalizeComboBox(controlAgent, typeof(SensorControlAgent));
            LocalizeComboBox(requestBody, typeof(SensorRequestBodies));
            LocalizeComboBox(requestPlacementMethod, typeof(SensorRequestPlacementMethod));
            LocalizeComboBox(informationRepresentationMethod, typeof(InformationRepresentationMethod));
        }
    }
}
