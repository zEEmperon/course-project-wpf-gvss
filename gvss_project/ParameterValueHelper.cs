using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace gvss_project
{
    public static class ParameterValueHelper
    {
        public static readonly Dictionary<OperatorLevel, string> OperatorLevels = new Dictionary<OperatorLevel, string>() {
            { OperatorLevel.Second, "II"},
            { OperatorLevel.Third, "III"},
            { OperatorLevel.Forth, "IV"},
            { OperatorLevel.Fifth, "V"}
        };      

        private static ComboBoxItem createComboBoxItem(string content, object tag)
        {
            ComboBoxItem cbi = new ComboBoxItem();
            cbi.Tag = tag;
            cbi.Content = content;
            return cbi;
        }
        public static void InitializeComboBox(ComboBox cb, Type enumType)
        {
            if (enumType == typeof(SensorControlMethod))
            {
                cb.Items.Clear();
                foreach (SensorControlMethod cm in Enum.GetValues(typeof(SensorControlMethod)))
                {
                    if (cm == SensorControlMethod.None)
                        continue;
                    cb.Items.Add(createComboBoxItem(DefaultSettings.getControlMethodText(cm), cm));
                }
            }
            else if (enumType == typeof(SensorControlAgent))
            {
                cb.Items.Clear();
                foreach (SensorControlAgent ca in Enum.GetValues(typeof(SensorControlAgent)))
                {
                    if (ca == SensorControlAgent.None)
                        continue;
                    cb.Items.Add(createComboBoxItem(DefaultSettings.getControlAgentText(ca), ca));
                }
            }
            else if (enumType == typeof(InformationRepresentationMethod))
            {
                cb.Items.Clear();
                foreach (InformationRepresentationMethod irm in Enum.GetValues(typeof(InformationRepresentationMethod)))
                {
                    if (irm == InformationRepresentationMethod.None)
                        continue;
                    cb.Items.Add(createComboBoxItem(DefaultSettings.getInformationRepresentationMethodText(irm), irm));
                }
            }
            else if (enumType == typeof(SensorRequestBodies))
            {
                cb.Items.Clear();
                foreach (SensorRequestBodies rb in Enum.GetValues(typeof(SensorRequestBodies)))
                {
                    if (rb == SensorRequestBodies.None)
                        continue;
                    cb.Items.Add(createComboBoxItem(DefaultSettings.getRequestBodyText(rb), rb));
                }
            }
            else if (enumType == typeof(OperatorLevel)) {
                cb.Items.Clear();
                foreach (OperatorLevel ol in Enum.GetValues(typeof(OperatorLevel)))
                {
                    cb.Items.Add(createComboBoxItem(OperatorLevels[ol], ol));
                }
            }
            else
            {
                throw new InvalidArgument();
            }

        }
        public static void InitializeComboBoxAndSelectIndex(ComboBox cb, Type enumType, int index = 0)
        {
            InitializeComboBox(cb, enumType);
            cb.SelectedIndex = index;
        }
        public static void InitializeComboBoxWithNone(ComboBox cb, Type enumType)
        {
            if (enumType == typeof(SensorRequestBodies))
            {
                cb.Items.Clear();
                cb.Items.Add(createComboBoxItem("", SensorRequestBodies.None));
            }
            else if (enumType == typeof(SensorRequestPlacementMethod))
            {
                cb.Items.Clear();
                cb.Items.Add(createComboBoxItem("", SensorRequestPlacementMethod.None));
            }
            else
            {
                throw new InvalidArgument();
            }
        }
        public static void InitializeComboBoxWithNoneAndSelectIndex(ComboBox cb, Type enumType, int index = 0)
        {
            InitializeComboBoxWithNone(cb, enumType);
            cb.SelectedIndex = index;
        }
        public static void InitializeRequestPlacementMethod(ComboBox cb, SensorRequestBodies requestBody)
        {
            if (requestBody != SensorRequestBodies.None)
            {
                cb.Items.Clear();
            }
            else
            {
                return;
            }

            if (requestBody == SensorRequestBodies.ToggleSwitch || requestBody == SensorRequestBodies.Button)
            {
                cb.Items.Add(createComboBoxItem(DefaultSettings.getRequestPlacementMethodText(SensorRequestPlacementMethod.MnemonicDiagram), SensorRequestPlacementMethod.MnemonicDiagram));
                cb.Items.Add(createComboBoxItem(DefaultSettings.getRequestPlacementMethodText(SensorRequestPlacementMethod.GroupMethod), SensorRequestPlacementMethod.GroupMethod));
                cb.Items.Add(createComboBoxItem(DefaultSettings.getRequestPlacementMethodText(SensorRequestPlacementMethod.BlockDiagramMethod), SensorRequestPlacementMethod.BlockDiagramMethod));
                cb.Items.Add(createComboBoxItem(DefaultSettings.getRequestPlacementMethodText(SensorRequestPlacementMethod.UnorganizedMethod), SensorRequestPlacementMethod.UnorganizedMethod));
                cb.SelectedIndex = 0;
            }
            else if (requestBody == SensorRequestBodies.Dialer || requestBody == SensorRequestBodies.KeyboardDevice)
            {
                cb.Items.Add(createComboBoxItem(DefaultSettings.getRequestPlacementMethodText(SensorRequestPlacementMethod.Separately), SensorRequestPlacementMethod.Separately));
                cb.Items.Add(createComboBoxItem(DefaultSettings.getRequestPlacementMethodText(SensorRequestPlacementMethod.GroupMethod), SensorRequestPlacementMethod.GroupMethod));
                cb.SelectedIndex = 0;
            }
        }

    }
}
