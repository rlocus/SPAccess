using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SharePoint.Remote.Access;
using SharePoint.Remote.Access.Helpers;

namespace SP2013Access
{
    /// <summary>
    ///     Interaction logic for OpenSiteWindow.xaml
    /// </summary>
    public partial class OpenSiteWindow : Window
    {
        public OpenSiteWindow()
        {
            InitializeComponent();

            var authenticationModes = new List<AuthenticationModeComboBoxItem>(
                new[]
                {
                    new AuthenticationModeComboBoxItem
                    {
                        Title = "Default",
                        Type = AuthType.Default
                    },
                    new AuthenticationModeComboBoxItem
                    {
                        Title = "SharePoint Online (Office 365)",
                        Type = AuthType.SharePointOnline
                    },
                    new AuthenticationModeComboBoxItem
                    {
                        Title = "Anonymous",
                        Type = AuthType.Anonymous
                    },
                    new AuthenticationModeComboBoxItem
                    {
                        Title = "Forms Based",
                        Type = AuthType.Forms
                    }
                });

            AuthenticationModeComboBox.ItemsSource = authenticationModes;
            AuthenticationModeComboBox.SelectedIndex = 0;
        }

        public OpenSiteWindow(RecentSite recentSite)
            : this()
        {
            var authenticationModes = (List<AuthenticationModeComboBoxItem>) AuthenticationModeComboBox.ItemsSource;

            if (recentSite != null)
            {
                SiteUrl = SiteUrlTextBox.Text = recentSite.Url;
                UserName = UserNameTextBox.Text = recentSite.UserName;
                UseCurrentUserCredentialsCheckBox.IsChecked = recentSite.UseCurrentUserCredentials;

                AuthenticationModeComboBox.SelectedIndex =
                    authenticationModes.Select(auth => auth.Type).ToList().IndexOf(recentSite.Authentication);
                UserPasswordTextBox.Focus();
            }
            else
            {
                SiteUrl = SiteUrlTextBox.Text = "https://sharepoint";
            }
        }

        public string SiteUrl { get; set; }
        public string UserName { get; set; }
        public SPClientContext ClientContext { get; private set; }

        private void Cancel_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            //this.Close();
        }

        private void OK_OnClick(object sender, RoutedEventArgs e)
        {
            var authType = ((AuthenticationModeComboBoxItem) AuthenticationModeComboBox.SelectedValue).Type;
            var site = new RecentSite
            {
                Url = SiteUrlTextBox.Text,
                UserName = UserNameTextBox.Text,
                UseCurrentUserCredentials = UseCurrentUserCredentialsCheckBox.IsChecked ?? false,
                Authentication = authType
            };

            MessageLabel.Content = string.Format("Connecting to {0} ...", site.Url);

            try
            {
                ClientContext = new SPClientContext(
                    site.Url,
                    site.Authentication,
                    site.UserName,
                    UserPasswordTextBox.Password);
            }
            catch (Exception ex)
            {
                MessageLabel.Content = ex.Message;
                return;
            }

            OKButton.IsEnabled = false;
            CancelButton.IsEnabled = false;
            SiteUrlTextBox.IsEnabled = false;
            AuthenticationModeComboBox.IsEnabled = false;
            UseCurrentUserCredentialsCheckBox.IsEnabled = false;
            UserNameTextBox.IsEnabled = false;
            UserPasswordTextBox.IsEnabled = false;

            var promise = Utility.ExecuteAsync(ClientContext.ConnectAsync());
            promise.Done(() =>
            {
                MessageLabel.Content = "Done";
                Globals.Configuration.Add(ClientContext);
                Globals.Configuration.Save();

                CloseDialog();
            });

            promise.Fail(ex => { MessageLabel.Content = ex.Message; });

            promise.Always(() =>
            {
                OKButton.IsEnabled = true;
                CancelButton.IsEnabled = true;
                SiteUrlTextBox.IsEnabled = true;
                AuthenticationModeComboBox.IsEnabled = true;
                SetVisiblility();
            });
        }

        private void CloseDialog()
        {
            if (Owner != null && Owner.OwnedWindows.Count > 0)
            {
                DialogResult = true;
            }
        }

        private void OnValidationError(object sender, ValidationErrorEventArgs e)
        {
            //if (e.Action == ValidationErrorEventAction.Added)
            //{
            //    MessageLabel.Content = e.Error.ErrorContent.ToString();
            //}
        }

        private void UseCurrentUserCredentialsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            UserNameTextBox.IsEnabled = false;
            UserPasswordTextBox.IsEnabled = false;
        }

        private void UseCurrentUserCredentialsCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            UserNameTextBox.IsEnabled = true;
            UserPasswordTextBox.IsEnabled = true;
        }

        private void AuthenticationModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetVisiblility();
        }

        private void SetVisiblility()
        {
            var authType = ((AuthenticationModeComboBoxItem) AuthenticationModeComboBox.SelectedValue).Type;

            if (authType == AuthType.Anonymous)
            {
                UseCurrentUserCredentialsCheckBox.IsEnabled = false;
                UseCurrentUserCredentialsCheckBox.IsChecked = false;
                UserNameTextBox.IsEnabled = false;
                UserPasswordTextBox.IsEnabled = false;
            }
            else
            {
                if (authType == AuthType.Forms || authType == AuthType.SharePointOnline)
                {
                    UseCurrentUserCredentialsCheckBox.IsEnabled = false;
                    UseCurrentUserCredentialsCheckBox.IsChecked = false;
                    UserNameTextBox.IsEnabled = true;
                    UserPasswordTextBox.IsEnabled = true;
                }
                else
                {
                    UseCurrentUserCredentialsCheckBox.IsEnabled = true;
                    if (UseCurrentUserCredentialsCheckBox.IsChecked == true)
                    {
                        UserNameTextBox.IsEnabled = false;
                        UserPasswordTextBox.IsEnabled = false;
                    }
                    else
                    {
                        UserNameTextBox.IsEnabled = true;
                        UserPasswordTextBox.IsEnabled = true;
                    }
                }
            }
        }

        public class AuthenticationModeComboBoxItem
        {
            public string Title { get; set; }
            public AuthType Type { get; set; }
        }
    }
}