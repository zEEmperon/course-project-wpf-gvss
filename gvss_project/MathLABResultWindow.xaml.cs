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
    public class EventDifficulty {
        public int EventID { get; set; }
        public string EventTime { 
            get {
                return _eventTime.ToString() + "  сек."; 
            } }

        private double _eventTime;

        public EventDifficulty(int id, double infDif) {
            EventID = id;
            _eventTime = infDif;
        }
    }

    public partial class MathLABResultWindow : Window
    {

        private const int ukrWidth = 320;
        private const int ruWidth = 420;
        public MathLABResultWindow(List<double> doubleNumbers)
        {
            InitializeComponent();
            LocalizeWindow();
            List<EventDifficulty> ed = new List<EventDifficulty>();

            int id = 1;

            foreach (double num in doubleNumbers)
            {
                ed.Add(new EventDifficulty(id++, num));
            }

            resultDataGrid.ItemsSource = ed;

        }

        private void LocalizeWindow()
        {
            this.Title = DefaultSettings.getWindowTitleText(WindowTitles.MathLABResults);

            this.Width = (DefaultSettings.LanguageMode == LanguageMode.Russian) ? ruWidth : ukrWidth;
            
            ((DataGridTextColumn)resultDataGrid.Columns[0]).Header = "№ " + DefaultSettings.getWordValue(Words.EventID);
            ((DataGridTextColumn)resultDataGrid.Columns[1]).Header = DefaultSettings.getWordValue(Words.EventTime);
        }

    }
}
