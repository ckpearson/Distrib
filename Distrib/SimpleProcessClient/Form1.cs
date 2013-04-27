using Distrib.Communication;
using RemoteProcessing.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleProcessClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private SimpleProcNode_CommsProxy _proxy;

        private void btnConnect_Click(object sender, EventArgs e)
        {
            _proxy = new SimpleProcNode_CommsProxy(
                new TcpOutgoingCommsLink<ISimpleProcNode_Comms>(
                    IPAddress.Parse(txtAddress.Text),
                    Convert.ToInt32(txtPort.Text),
                    new XmlCommsMessageReaderWriter(
                        new BinaryFormatterCommsMessageFormatter())));

            btnSet.Enabled = false;
            txtAddress.Enabled = false;
            txtPort.Enabled = false;
            grpOperations.Enabled = true;
        }

        private void btnSayHello_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSayHello.Text))
            {
                MessageBox.Show("You need to type who to say hello to!");
                txtSayHello.Focus();
                return;
            }

            try
            {
                var def = _proxy.GetJobDefinitions()
                        .FirstOrDefault(jd => jd.Name == "Say Hello");

                if (def == null)
                {
                    MessageBox.Show("No job definition could be found called 'Say Hello'", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                lblSayHelloResponse.Text = _proxy.SayHello(txtSayHello.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("{0}\n{1}",
                    ex.Message, ex.GetBaseException().Message), "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
