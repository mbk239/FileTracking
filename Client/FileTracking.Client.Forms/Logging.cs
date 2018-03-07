using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileTracking.Client.Forms
{
    public partial class frmLogging : Form
    {

        public string LoggingText
        {
            get
            {
                return txtLog.Text;
            }
            set
            {
                txtLog.Text += value + Environment.NewLine;
            }
        }

        public frmLogging()
        {
            InitializeComponent();
            txtLog.Text = "";       
        }
    }
}
