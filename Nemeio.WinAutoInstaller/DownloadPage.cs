using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Nemeio.WinAutoInstaller.Controllers;
using Nemeio.WinAutoInstaller.Models;

namespace Nemeio.WinAutoInstaller
{
    public partial class DownloadPage : Form
    {
        private const string DownloadText = "Downloading";

        private DownloadController _downloadController;
        private ErrorMessageProvider _errorMessageProvider;

        public DownloadPage()
        {
            InitializeComponent();
            Load += Form1_Load;
            FormClosing += DownloadPage_FormClosing;
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            PB_DownloadProgress.Minimum = 0;
            PB_DownloadProgress.Maximum = 100;

            But_Cancel.Click += But_Cancel_Click;

            _errorMessageProvider = new ErrorMessageProvider();

            var keyboardId = WmiHelper.GetCurrentKeyboardIdentifier();
            if (string.IsNullOrWhiteSpace(keyboardId))
            {
                // hack to allow to use from dev standpoint with a fake keyboard Id)
                keyboardId = SettingsHelper.ReadSetting("FakeKeyboardId");
                if (string.IsNullOrWhiteSpace(keyboardId))
                {
                    var errorMessage = "Current auto installer is not running from expected CDrom drive or keyboard Identifier is not recognized.\n\nPlease run from keyboard mounted CDRom drive or contact support";
                    MessageBox.Show(errorMessage, "Nemeio", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Logger.Instance.LogException(null, errorMessage);
                    Close();
                    return;
                }
            }

            var message = $"Do you want to download Nemeio\nfor keyboard <{keyboardId}>?";
            var messageResponse = MessageBox.Show(message, "Nemeio", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (messageResponse == DialogResult.Yes)
            {
                _downloadController = new DownloadController();
                _downloadController.DownloadProgressChanged += DownloadController_DownloadProgressChanged;
                _downloadController.DownloadFinished += DownloadController_DownloadFinished;

                Task.Run(async () => await _downloadController.StartAsync());
            }
            else
            {
                Logger.Instance.LogInformation("User cancel download");
                Close();
            }
        }

        private void DownloadPage_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_downloadController != null)
            {
                _downloadController.DownloadProgressChanged -= DownloadController_DownloadProgressChanged;
                _downloadController.DownloadFinished -= DownloadController_DownloadFinished;
                _downloadController.Stop();
                _downloadController = null;
            }
        }

        private void But_Cancel_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void DownloadController_DownloadProgressChanged(object sender, EventArgs.InstallerDownloadProgressChangedEventArgs e)
        {
            if (this.IsHandleCreated)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    var percent = e.Percent;
                    PB_DownloadProgress.Value = percent;
                    Lab_Header.Text = $"{DownloadText} {_downloadController.CurrentVersion} [{_downloadController.CurrentArchitecture}] ... ({percent}%)";
                });
            }
        }

        private void DownloadController_DownloadFinished(object sender, EventArgs.InstallerDownloadFinishedEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                if (e.ErrorCode == ErrorCode.WinAutoInstallerSuccess)
                {
                    Process.Start(e.DownloadPath.AbsolutePath);
                    Close();
                }
                else
                {
                    var errorMessage = _errorMessageProvider.GetErrorMessage(e.ErrorCode);
                    MessageBox.Show(
                        errorMessage, 
                        "Nemeio", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Error, 
                        MessageBoxDefaultButton.Button1
                    );
                    Close();
                }
            });
        }
    }
}
