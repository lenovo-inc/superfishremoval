using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SuperFishRemovalTool.Utilities;

namespace SuperFishRemovalTool
{
    /// <summary>
    /// Summary result of 'sum' of all operations performed
    /// </summary>
    public enum OverallResult
    {
        /// <summary>
        /// Invalid state
        /// </summary>
        None = 0,
        /// <summary>
        /// No superfish items were found
        /// </summary>
        NoItemsFound = 1,
        /// <summary>
        /// Superfish items were found, and all were removed
        /// </summary>
        ItemsFoundAndRemoved = 2,
        /// <summary>
        /// Superfish items were found, but not all were removed
        /// </summary>
        ItemsFoundButNotRemoved = 3,
        /// <summary>
        /// An unexpected error occured while trying to remove one or many items
        /// </summary>
        Error = 4,
    }

    public partial class MainScreen : Form
    {
        public MainScreen()
        {
            InitializeComponent();
            this.Text = Localizer.Get().UtilityName; // Window title
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.PrepareBlankState();
        }

        #region UI Toggles

        private void PrepareBlankState()
        {
            try
            {
                var stringTable = Localizer.Get();
                this.IsDoingWork = false;
                this.Text = stringTable.UtilityName;
                this.IntroLabel.Text = stringTable.UtilityAbout;
                this.RemoveButton.BackColor = Constants.Colors.ButtonBackground;
                this.RemoveButton.ForeColor = Constants.Colors.ButtonForeground;
                this.RemoveButton.Text = stringTable.Remove;
                this.RestartNowButton.BackColor = Constants.Colors.ButtonBackground;
                this.RestartNowButton.ForeColor = Constants.Colors.ButtonForeground;
                this.RestartNowButton.Text = stringTable.RestartNow;
                this.RestartLaterButton.BackColor = Constants.Colors.ButtonBackground;
                this.RestartLaterButton.ForeColor = Constants.Colors.ButtonForeground;
                this.RestartLaterButton.Text = stringTable.RestartLater;
                this.MoreInfoLabel.Text = stringTable.MoreInformationText;

                this.ManualRemovalLinkLabel.Text = stringTable.ManualRemovalInstructionsText;
                this.ManualRemovalLinkLabel.Links.Clear();
                this.ManualRemovalLinkLabel.Links.Add(new LinkLabel.Link() { LinkData = stringTable.ManualRemovalInstructionsLink });

                this.SecurityAdvisoryLinkLabel.Text = stringTable.LenovoSecurityAdvisoryText;
                this.SecurityAdvisoryLinkLabel.Links.Clear();
                this.SecurityAdvisoryLinkLabel.Links.Add(new LinkLabel.Link() { LinkData = stringTable.LenovoSecurityAdvisoryLink });

                this.LenovoStatementLinkLabel.Text = stringTable.LenovoStatementText;
                this.LenovoStatementLinkLabel.Links.Clear();
                this.LenovoStatementLinkLabel.Links.Add(new LinkLabel.Link() { LinkData = stringTable.LenovoStatementLink });

                this.LicenseAgreementLinkLabel.Text = stringTable.LicenseAgreementText;
                this.LicenseAgreementLinkLabel.Links.Clear();
                this.LicenseAgreementLinkLabel.Links.Add(new LinkLabel.Link() { LinkData = stringTable.LicenseAgreementLink });
                
                this.OverallResultLabel.ResetText();
                this.ResultsFlowPanel.Controls.Clear();
                this.MainProgressBar.Value = 0;
                this.UpdateRestartButtons(OverallResult.None);
                this.UpdateVersionField();
            }
            catch (Exception ex)
            {
                Logging.Logger.Log(ex, "Exception while preparing base state of UI");
            }

        }

        /// <summary>
        /// When doing work, display the progress bar and disable buttons
        /// </summary>
        private bool IsDoingWork
        {
            get
            {
                return _isDoingWork;
            }
            set
            {
                _isDoingWork = value;
                // Helper variables to make the remainder more readable
                bool isVisibleWhileWorking = _isDoingWork;
                bool isNotVisibleWhileWorking = !_isDoingWork;
                bool isEnabledWhileWorking = _isDoingWork;
                bool isNotEnabledWhilewWorking = !_isDoingWork;

                this.RemoveButton.BackColor = (value) ? Constants.Colors.DisabledBackgroundColor : Constants.Colors.ButtonBackground;

                this.MainProgressBar.Visible = isVisibleWhileWorking;
                this.RemoveButton.Enabled = isNotVisibleWhileWorking;
                this.RestartNowButton.Enabled = isNotEnabledWhilewWorking;
                this.RestartLaterButton.Enabled = isNotEnabledWhilewWorking;
            }
        }
        private bool _isDoingWork;

        /// <summary>
        /// Update restart buttons visibility and ableness based on the overall result
        /// </summary>
        /// <param name="result"></param>
        private void UpdateRestartButtons(OverallResult result)
        {
            bool areButtonsEnabled = false;
            switch (result)
            {
                case OverallResult.None:
                    areButtonsEnabled = false;
                    break;
                case OverallResult.NoItemsFound:
                    areButtonsEnabled = false;
                    break;
                case OverallResult.ItemsFoundAndRemoved:
                    areButtonsEnabled = true;
                    break;
                case OverallResult.ItemsFoundButNotRemoved:
                    areButtonsEnabled = false;
                    break;
                case OverallResult.Error:
                    areButtonsEnabled = false;
                    break;
                default:
                    break;
            }

            this.RestartLaterButton.Visible = areButtonsEnabled;
            this.RestartLaterButton.Enabled = areButtonsEnabled;
            this.RestartNowButton.Visible = areButtonsEnabled;
            this.RestartNowButton.Enabled = areButtonsEnabled;
        }

        /// <summary>
        /// Translate an overall result into what the end user will see as the result
        /// </summary>
        /// <param name="result"></param>
        private void UpdateOverallStatusLabel(OverallResult result)
        {
            var stringTable = Localizer.Get();
            string newText = string.Empty;
            switch (result)
            {
                case OverallResult.None:
                    goto case OverallResult.Error;
                case OverallResult.NoItemsFound:
                    newText = stringTable.OverallStatusNotOnSystem;
                    break;
                case OverallResult.ItemsFoundAndRemoved:
                    newText = stringTable.OverallStatusAppRemoved;
                    break;
                case OverallResult.ItemsFoundButNotRemoved:
                    goto case OverallResult.Error;
                case OverallResult.Error:
                    newText = stringTable.OverallStatusError;
                    break;
                default:
                    break;
            }
            this.OverallResultLabel.Text = newText;
        }

        /// <summary>
        /// Adds a line item into the results for a particular scan result
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool AddLabelBasedOnIndividualResult(Utilities.FixResult result)
        {
            bool error = false;
            try
            {
                var stringTable = Localizer.Get();
                string text = String.Empty;
                if (result == null || result.DidFail)
                {
                    text = stringTable.Error;
                }
                else
                {

                    if (result.DidExist)
                    {
                        if (result.WasRemoved)
                        {
                            text = stringTable.ResultFoundAndRemoved;
                        }
                        else
                        {
                            text = stringTable.ResultFoundButNotRemoved;
                            error = true;
                        }
                    }
                    else
                    {
                        text = stringTable.ResultNotFound;
                    }

                }

                string completeText = String.Format("{0} - {1}", text, result.NameOfItem);
                this.ResultsFlowPanel.Controls.Add(GenerateResultLabel(completeText));

            }
            catch (Exception ex)
            {
                Logging.Logger.Log(ex, "Exception while adding result label");
            }
            return error;
        }

        /// <summary>
        /// Displays the current version of the executable
        /// </summary>
        private void UpdateVersionField()
        {
            try
            {
                var version = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();
                this.VersionLabel.Text = String.Format("{0}: {1}", Localizer.Get().Version, version);
            }
            catch(Exception ex)
            {
                Logging.Logger.Log(ex, "Exception while updating version field");
            }
        }
        #endregion UI Toggles

        /// <summary>
        /// Starts the process of running the removal agents
        /// </summary>
        private void StartRunningRemovalAgents()
        {
            try
            {
                bool areAnyBrowsersRunning = BrowserDetector.AreAnyWebBrowsersRunning();
                if (areAnyBrowsersRunning)
                {
                    MessageBox.Show(Localizer.Get().CloseWebBrowsers, Localizer.Get().UtilityName);
                }

                this.OverallResultLabel.ResetText();
                this.ResultsFlowPanel.Controls.Clear();
                this.MainProgressBar.Value = 0;
                this.PrepareBlankState();
                this.IsDoingWork = true;
                this.InitializeBackgroundWorker();

            }
            catch (Exception ex)
            {
                Logging.Logger.Log(ex, "Exception while running removal agents");
            }
        }



        private void InitializeBackgroundWorker()
        {
            this.MainProgressBar.Value = 5;
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.DoWork += (sender, e) =>
                {
                    var bgWorker = sender as BackgroundWorker;
                    if(bgWorker != null)
                    {
                        OverallResult overallResult = OverallResult.None;
                        var stringTable = Localizer.Get();


                        var agents = GetSuperfishRemovalAgents().ToList();
                        if(agents != null && agents.Any())
                        {
                            foreach(var agent in agents)
                            {
                                try
                                {
                                    double percentageComplete = ( (double)(agents.IndexOf(agent) + 1) / (double)agents.Count) * 100;
                                    var removalResult = TryToExecuteRemoval(agent);
                                    bgWorker.ReportProgress(Convert.ToInt32(percentageComplete), removalResult);
                                    overallResult = CalculateSingleResult(removalResult);
                                    System.Threading.Thread.Sleep(1000); // Some of the agents perform very quickly.  Slow it down to show each step
                                }
                                catch(Exception ex)
                                {
                                    Logging.Logger.Log(Logging.LogSeverity.Error, ex.Message);
                                }
                            }
                        }

                        bgWorker.ReportProgress(100, overallResult);
                        System.Threading.Thread.Sleep(500); // Let the user see the final 100% before hiding progress bar
                    }
                };


            backgroundWorker.ProgressChanged += (sender, e) =>
            {
                if (e != null)
                {
                    if (e.UserState == null)
                    {
                        this.UpdateOverallStatusLabel(OverallResult.Error);
                    }
                    else
                    {
                        this.MainProgressBar.Value = e.ProgressPercentage;
                        var result = e.UserState as Utilities.FixResult;
                        if (result != null)
                        {
                            this.AddLabelBasedOnIndividualResult(result);
                        }
                        else
                        {
                            var overallStatus = (OverallResult)e.UserState;
                            if (overallStatus != OverallResult.None)
                            {
                                this.UpdateOverallStatusLabel(overallStatus);
                                this.UpdateRestartButtons(overallStatus);
                            }
                        }
                    }
                }
            };
            backgroundWorker.RunWorkerCompleted += (sender, e) =>
            {
                this.IsDoingWork = false;
                this.MainProgressBar.Value = 0;
            };

            backgroundWorker.RunWorkerAsync(); // Start!
        }

        /// <summary>
        /// Restarts the user's device
        /// </summary>
        private void RestartDevice()
        {
            try
            {
                ProcessStarter.StartWithoutWindow("shutdown.exe", "-r -t 5", true);
            }
            catch(Exception ex)
            {
                Logging.Logger.Log(ex, "Exception while restarting device");
            }
            Application.Exit();
        }

        #region events
        private void RemoveButton_Click(object sender, EventArgs e)
        {
            
            StartRunningRemovalAgents();
        }


        private void RestartNowButton_Click(object sender, EventArgs e)
        {
            try
            {
                RestartDevice();
            }
            catch(Exception ex)
            {
                Logging.Logger.Log(ex, "Exception while handling restart button click ");
            }
        }

        private void RestartLaterButton_Click(object sender, EventArgs e)
        {
            try
            {
                Application.Exit();
            }
            catch (Exception ex)
            {
                Logging.Logger.Log(ex, "Exception while handling restart later click");
            }
        }




        private void LearnMoreLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            HandleLinkClick(e);
        }

        private void ManualRemovalLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            HandleLinkClick(e);
        }

        private void SecurityAdvisoryLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            HandleLinkClick(e);
        }

        private void LenovoStatementLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            HandleLinkClick(e);
        }

        private void LicenseAgreementLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            HandleLinkClick(e);
        }

        #endregion events

        #region Private Static methods
        private static IEnumerable<Utilities.ISuperfishDetector> GetSuperfishRemovalAgents()
        {
            return new List<Utilities.ISuperfishDetector>()
            {
                       new Utilities.ApplicationUtility(),
                       new Utilities.CertificateUtility(),
                       new Utilities.RegistryUtility(),
                       new Utilities.FilesDetector(),
                       new Utilities.MozillaCertificateUtility(),
            };
        }


        private static Utilities.FixResult TryToExecuteRemoval(Utilities.ISuperfishDetector detector)
        {
            Utilities.FixResult result = null;
            try
            {
                result = detector.RemoveItem();
            }
            catch (Exception ex)
            {
                Logging.Logger.Log(ex, "Exception while trying to invoke superfish detetctor ");
            }
            return result;
        }


        private static void HandleLinkClick(LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (e != null && e.Link != null)
                {
                    // Send the URL to the operating system.
                    System.Diagnostics.Process.Start(e.Link.LinkData as string);
                }
            }
            catch (Exception ex)
            {
                Logging.Logger.Log(ex, "Exception while handling link label click");
            }
        }

        /// <summary>
        /// Creates a UI Label instance with the provided text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static Label GenerateResultLabel(string text)
        {
            return new Label()
            {
                Text = String.Format(text),
                AutoSize = false,
                Width = 400,
                Margin = new Padding(0, 3, 0, 2),
                Font = new System.Drawing.Font("Microsoft Sans Serif", 10F),
            };
        }


        /// <summary>
        /// Translates a single removal result into an overall result for displaying to the user
        /// </summary>
        /// <param name="fixResult"></param>
        /// <returns></returns>
        private static OverallResult CalculateSingleResult(Utilities.FixResult fixResult)
        {
            OverallResult result = OverallResult.None;
            if (fixResult == null || fixResult.DidFail)
            {
                result = OverallResult.Error;
            }
            else
            {
                if (fixResult.DidExist)
                {
                    result = fixResult.WasRemoved ? OverallResult.ItemsFoundAndRemoved : OverallResult.ItemsFoundButNotRemoved;
                }
                else
                {
                    result = OverallResult.NoItemsFound;
                }
            }
            return result;
        }

        /// <summary>
        /// Prioritizes previous results with a current result to determine which one to keep
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="mostRecent"></param>
        /// <returns></returns>
        private static OverallResult CalculateMergedResult(OverallResult previous, OverallResult mostRecent)
        {
            var mostRelevant = previous;
            if (previous > mostRecent)
            {
                mostRelevant = previous;
            }
            else
            {
                mostRelevant = mostRecent;
            }
            return mostRelevant;

        }
        #endregion Private Static methods


    }
}