using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RJLogger;

namespace RJDashboard.Controls
{
    public partial class TextLogs : UserControl
    {
        private int _count = 0;
        private List<string> _listLogs;

        public int MaxLogs { get; set; }

        public TextLogs()
        {
            MaxLogs = 100;
            _listLogs = new List<string>();

            InitializeComponent();
        }        

        public void AddLog(string log, string error, string code)
        {
            try
            {
                _count += 1;

                txtLogs.AppendText(Environment.NewLine);
                txtLogs.AppendText($"- {_count} - -- - -- - -- - -- - -- - -- -");

                if (!string.IsNullOrEmpty(log))
                {                        
                    txtLogs.AppendText(Environment.NewLine);
                    txtLogs.AppendText(log);

                    _listLogs.Add(log);
                }

                if (!string.IsNullOrEmpty(error))
                {
                    txtLogs.AppendText(Environment.NewLine);
                    txtLogs.AppendText($"Error: {error}");

                    _listLogs.Add($"Error: {error}");
                }

                if (!string.IsNullOrEmpty(code))
                {
                    txtLogs.AppendText(Environment.NewLine);
                    txtLogs.AppendText($"Code: {code}");

                    _listLogs.Add($"Code: {code}");
                }

                if (_count > MaxLogs)
                    ClearLogs();
            }
            catch (Exception e)
            {
                txtLogs.Clear();
                _count = 0;

                AppLogger.GetLogger().Error("Błąd podczas dodawania loga.", e);
            }
        }

        private void ClearLogs()
        {
            try
            {
                _count = 0;
                txtLogs.Clear();
            }
            catch (Exception e)
            {
                AppLogger.GetLogger().Error("Błąd podczas czyszczenia logów", e);
            }
        }
    }
}
