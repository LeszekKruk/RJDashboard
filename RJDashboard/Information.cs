using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RJDashboard
{
    public partial class Information : MetroForm
    {
        public Information(string errorDescription, string errorSolution)
        {
            InitializeComponent();
            lblErrorType.Text = errorDescription;
            lblErrorSolution.Text = errorSolution;
        }

        private void tileClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
