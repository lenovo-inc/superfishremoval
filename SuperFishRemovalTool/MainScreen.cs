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
    public enum OverallResult
    {
        None = 0,
        NoItemsFound = 1,
        ItemsFoundAndRemoved = 2,
        ItemsFoundButNotRemoed = 3,
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
                this.IsWorking = false;
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
                Console.WriteLine(ex.Message);
            }

        }


        private bool IsWorking
        {
            get
            {
                return true;
            }
            set
            {
                bool isVisibleWhileWorking = value;
                bool isNotVisibleWhileWorking = !value;
                bool isEnabledWhileWorking = value;
                bool isNotEnabledWhilewWorking = !value;

                this.RemoveButton.BackColor = (value) ? Constants.Colors.DisabledBackgroundColor : Constants.Colors.ButtonBackground;

                this.MainProgressBar.Visible = isVisibleWhileWorking;
                this.RemoveButton.Enabled = isNotVisibleWhileWorking;
                this.RestartNowButton.Enabled = isNotEnabledWhilewWorking;
                this.RestartLaterButton.Enabled = isNotEnabledWhilewWorking;
            }
        }

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
                case OverallResult.ItemsFoundButNotRemoed:
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
                case OverallResult.ItemsFoundButNotRemoed:
                    goto case OverallResult.Error;
                case OverallResult.Error:
                    newText = stringTable.OverallStatusError;
                    break;
                default:
                    break;
            }
            this.OverallResultLabel.Text = newText;
        }

        private bool UpdateLabelBasedOnResult(Utilities.FixResult result)
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
                this.ResultsFlowPanel.Controls.Add(GenerageResultLabel(completeText));

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return error;
        }

        private void UpdateVersionField()
        {
            try
            {
                var version = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();
                this.VersionLabel.Text = String.Format("{0}: {1}", Localizer.Get().Version, version);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion UI Toggles


        private void RunRemoveAgents()
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
                this.IsWorking = true;
                this.InitializeBackgroundWorker();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
                                    var removalResult = TryToExeuteRemoval(agent);
                                    bgWorker.ReportProgress(Convert.ToInt32(percentageComplete), removalResult);
                                    overallResult = CalculateSingleResult(removalResult);
                                    System.Threading.Thread.Sleep(1000);
                                }
                                catch(Exception ex)
                                {
                                    Console.Write(ex.Message);
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
                            this.UpdateLabelBasedOnResult(result);
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
                this.IsWorking = false;
                this.MainProgressBar.Value = 0;
            };

            backgroundWorker.RunWorkerAsync(); // Start!
        }


        private void Shutdown()
        {
            try
            {
                ProcessStarter.StartWithoutWindow("shutdown.exe", "-r -t 5", true);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Application.Exit();
        }

        #region events
        private void RemoveButton_Click(object sender, EventArgs e)
        {
            
            RunRemoveAgents();
        }


        private void RestartNowButton_Click(object sender, EventArgs e)
        {
            try
            {
                Shutdown();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
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
                Console.WriteLine(ex.Message);
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


        private static Utilities.FixResult TryToExeuteRemoval(Utilities.ISuperfishDetector detector)
        {
            Utilities.FixResult result = null;
            try
            {
                result = detector.RemoveItem();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
                Console.WriteLine(ex.Message);
            }
        }

        private static Label GenerageResultLabel(string text)
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
                    result = fixResult.WasRemoved ? OverallResult.ItemsFoundAndRemoved : OverallResult.ItemsFoundButNotRemoed;
                }
                else
                {
                    result = OverallResult.NoItemsFound;
                }
            }
            return result;
        }

        private static OverallResult CalculateMergedResult(OverallResult previous, OverallResult mostRecent)
        {
            var mostRelevent = previous;
            if (previous > mostRecent)
            {
                mostRelevent = previous;
            }
            else
            {
                mostRelevent = mostRecent;
            }
            return mostRelevent;

        }
        #endregion Private Static methods


    }
}