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
    /// Interaction logic for ShowTime.xaml
    /// </summary>
    public partial class ShowTime : Window
    {
        private string time(double value) {
            return value.ToString() + " сек.";
        }
        public ShowTime(Event e)
        {
            InitializeComponent();
            LocalizeWindow();

            header.Content = DefaultSettings.getLabelText(Labels.Event) + " №" + e.ID.ToString();

            double originalTime = e.getOriginalTime();
            double requestTime = e.getRequestTime();
            double irrelevantTime = e.getIrrelevantTime();
            double agentTimeInfluence = e.getAgentTimeInfluence();
            double findControlAgentTime = e.getFindControlAgentTime();

            double totalTime = originalTime + requestTime + irrelevantTime + agentTimeInfluence + findControlAgentTime;

            informationQuantityTB.Text = e.getInformationDifficulty().ToString() + " д.e.";
            originalTimeTB.Text = time(originalTime);
            requestTimeTB.Text = time(requestTime);
            irrelevantInformationTimeTB.Text = time(irrelevantTime);
            agentInfluenceTimeTB.Text = time(agentTimeInfluence);
            findControlAgentTimeTB.Text = time(findControlAgentTime);
            totalTimeTB.Text = time(totalTime);


        }

        private void LocalizeWindow() {
            this.Title = DefaultSettings.getWindowTitleText(WindowTitles.EventDetectionTime);
            informationQuantityLabel.Text = DefaultSettings.getLabelText(Labels.InformationQuantity);
            originalTimeLabel.Text = DefaultSettings.getLabelText(Labels.OriginalTime);
            requestTimeLabel.Text = DefaultSettings.getLabelText(Labels.RequestTime);
            irrelevantInformationLabel.Text = DefaultSettings.getLabelText(Labels.IrrelevantInformationTime);
            agentTimeInfluenceLabel.Text = DefaultSettings.getLabelText(Labels.AgentTimeInfluence);
            findControlAgentTimeLabel.Text = DefaultSettings.getLabelText(Labels.FindControlAgentTime);
            totalTimeLabel.Text = DefaultSettings.getLabelText(Labels.TotalTime);
        }
    }
}
