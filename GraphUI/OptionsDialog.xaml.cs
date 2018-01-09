using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GraphUI.Properties;

namespace GraphUI
{
    /// <summary>
    /// Options logic
    /// </summary>
    public partial class OptionsDialog : Window
    {
        #region Data Members

        /// <summary>
        /// The singleton instance
        /// </summary>
        public static OptionsDialog Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Enables / disables archiving controls in options dialog
        /// </summary>
        public bool ArchiveGraphs
        {
            get
            {
                return Settings.Default.ArchiveEnabled;
            }
            set
            {
                Settings.Default.ArchiveEnabled =
                ArchivePathTextBox.IsEnabled    =
                ArchivePathButton.IsEnabled     =
                FolderSizeTextBox.IsEnabled     =
                Radio_800X600.IsEnabled         =
                Radio_1280X1024.IsEnabled       =
                Radio_1920X1080.IsEnabled       =
                RadioCustom.IsEnabled           =
                CustomWidth.IsEnabled           =
                CustomHeight.IsEnabled          = value;
            }
        }

        /// <summary>
        /// Enables / disables history controls
        /// </summary>
        public bool KeepUiHistory
        {
            get
            {
                return Settings.Default.HistoryEnabled;
            }
            set
            {
                Settings.Default.HistoryEnabled  =
                NumPagesHistoryTextBox.IsEnabled =
                AutoNavigateCheckBox.IsEnabled   = value;
            }
        }

        #endregion Data Members

        #region Public Methods

        /// <summary>
        /// Initializes the options dialog with values stored in settings
        /// </summary>
        public OptionsDialog()
        {
            InitializeComponent();

            Left = Settings.Default.OptionsLeft;
            Top = Settings.Default.OptionsTop;

            ArchiveGraphs = Settings.Default.ArchiveEnabled;

            ArchivePathTextBox.Text = Settings.Default.ArchiveLocation;

            FolderSizeTextBox.Text = Settings.Default.MaxArchiveSize.ToString();

            if (Settings.Default.ImageWidth == 800 && Settings.Default.ImageHeight == 600)
            {
                Radio_800X600.IsChecked = true;
            }
            else if (Settings.Default.ImageWidth == 1280 && Settings.Default.ImageHeight == 1024)
            {
                Radio_1280X1024.IsChecked = true;
            }
            else if (Settings.Default.ImageWidth == 1920 && Settings.Default.ImageHeight == 1080)
            {
                Radio_1920X1080.IsChecked = true;
            }
            else
            {
                RadioCustom.IsChecked = true;
                CustomWidth.Text = Settings.Default.ImageWidth.ToString();
                CustomHeight.Text = Settings.Default.ImageHeight.ToString();
            }

            KeepUiHistory = Settings.Default.HistoryEnabled;

            NumPagesHistoryTextBox.Text = Settings.Default.NumPagesHistory.ToString();

            AutoNavigateCheckBox.IsChecked = Settings.Default.AutoNavigate;

            Instance = this;
        }

        #endregion Public Methods

        #region Event Handlers

        /// <summary>
        /// Close the window
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Event args</param>
        private void OnExitOptions(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Saves the options
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Event args</param>
        private void OnSaveOptions(object sender, RoutedEventArgs e)
        {
            Settings.Default.ArchiveLocation = ArchivePathTextBox.Text;
            Settings.Default.MaxArchiveSize = int.Parse(FolderSizeTextBox.Text);

            if (Radio_800X600.IsChecked == true)
            {
                Settings.Default.ImageWidth = 800;
                Settings.Default.ImageHeight = 600;
            }
            else if ( Radio_1280X1024.IsChecked == true)
            {
                Settings.Default.ImageWidth = 1280;
                Settings.Default.ImageHeight = 1024;
            }
            else if (Radio_1920X1080.IsChecked == true)
            {
                Settings.Default.ImageWidth = 1920;
                Settings.Default.ImageHeight = 1080;
            }
            else
            {
                Settings.Default.ImageWidth = int.Parse(CustomWidth.Text);
                Settings.Default.ImageHeight = int.Parse(CustomHeight.Text);
            }

            Settings.Default.HistoryEnabled = KeepUiHistory;

            Settings.Default.NumPagesHistory = int.Parse(NumPagesHistoryTextBox.Text);

            Settings.Default.AutoNavigate = (bool)AutoNavigateCheckBox.IsChecked;

            Settings.Default.Save();

            MainWindow.Instance.UpdateHistory();

            Close();
        }

        /// <summary>
        /// Save state information
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Event args</param>
        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.Default.OptionsLeft = Left;
            Settings.Default.OptionsTop = Top;
            Settings.Default.Save();
        }

        /// <summary>
        /// Only allow numeric input
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Event args</param>
        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex(@"[^0-9]").IsMatch(e.Text);
        }

        /// <summary>
        /// Remove spaces from dimension value
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Event args</param>
        private void OnTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (sender.GetType() == typeof (TextBox))
            {
                ((TextBox)sender).Text = Regex.Replace(((TextBox)sender).Text, @"\s+", "");
            }
        }

        /// <summary>
        /// Sets the archive path
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Event args</param>
        private void OnSetArchivePath(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ArchivePathTextBox.Text = dialog.SelectedPath;
            }
        }

        #endregion Event Handlers
    }
}
