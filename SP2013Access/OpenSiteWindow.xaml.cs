using SharePoint.Remote.Access;
using SharePoint.Remote.Access.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SP2013Access
{
    /// <summary>
    /// Interaction logic for OpenSiteWindow.xaml
    /// </summary>
    public partial class OpenSiteWindow : Window
    {
        public class AuthenticationModeComboBoxItem
        {
            public string Title { get; set; }

            public AuthType Type { get; set; }
        }

        public string SiteUrl { get; set; }

        public string UserName { get; set; }

        public SPClientContext ClientContext { get; private set; }

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

            this.AuthenticationModeComboBox.ItemsSource = authenticationModes;
            this.AuthenticationModeComboBox.SelectedIndex = 0;
        }

        public OpenSiteWindow(RecentSite recentSite)
            : this()
        {
            var authenticationModes = (List<AuthenticationModeComboBoxItem>)this.AuthenticationModeComboBox.ItemsSource;

            if (recentSite != null)
            {
                this.SiteUrl = SiteUrlTextBox.Text = recentSite.Url;
                this.UserName = UserNameTextBox.Text = recentSite.UserName;
                UseCurrentUserCredentialsCheckBox.IsChecked = recentSite.UseCurrentUserCredentials;

                this.AuthenticationModeComboBox.SelectedIndex =
                    authenticationModes.Select(auth => auth.Type).ToList().IndexOf(recentSite.Authentication);
            }
            else
            {
                this.SiteUrl = SiteUrlTextBox.Text = "https://sharepoint";
            }
        }

        private void Cancel_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            //this.Close();
        }

        private void OK_OnClick(object sender, RoutedEventArgs e)
        {
            OKButton.IsEnabled = false;
            CancelButton.IsEnabled = false;
            SiteUrlTextBox.IsEnabled = false;
            AuthenticationModeComboBox.IsEnabled = false;

            if (!UseCurrentUserCredentialsCheckBox.IsEnabled)
            {
                UserNameTextBox.IsEnabled = false;
                UserPasswordTextBox.IsEnabled = false;
            }

            AuthType authType = ((AuthenticationModeComboBoxItem)this.AuthenticationModeComboBox.SelectedValue).Type;

            if (authType == AuthType.Default)
            {
                UseCurrentUserCredentialsCheckBox.IsEnabled = false;
            }

            var site = new RecentSite
            {
                Url = this.SiteUrlTextBox.Text,
                UserName = UserNameTextBox.Text,
                UseCurrentUserCredentials = UseCurrentUserCredentialsCheckBox.IsChecked ?? false,
                Authentication = ((AuthenticationModeComboBoxItem)this.AuthenticationModeComboBox.SelectedValue).Type
            };

            MessageLabel.Content = string.Format("Connecting to {0}", site.Url);

            try
            {
                this.ClientContext = new SPClientContext(
                    site.Url,
                    site.Authentication,
                    site.UserName,
                    UserPasswordTextBox.Password);
            }
            catch (Exception ex)
            {
                MessageLabel.Content = ex.Message;
                OKButton.IsEnabled = true;
                CancelButton.IsEnabled = true;
                SiteUrlTextBox.IsEnabled = true;
                AuthenticationModeComboBox.IsEnabled = true;

                if (!UseCurrentUserCredentialsCheckBox.IsEnabled)
                {
                    UserNameTextBox.IsEnabled = true;
                    UserPasswordTextBox.IsEnabled = true;
                }

                if (authType == AuthType.Default)
                {
                    UseCurrentUserCredentialsCheckBox.IsEnabled = true;
                }

                return;
            }
            //finally
            //{
            //}

            IPromise<object, Exception> promise = Utility.ExecuteAsync(this.ClientContext.ConnectAsync());

            promise.Done(() =>
            {
                MessageLabel.Content = "Done";
                Globals.Configuration.Add(this.ClientContext);
                Globals.Configuration.Save();

                CloseDialog();
            });

            promise.Fail((ex) =>
            {
                MessageLabel.Content = ex.Message;
                //MessageBox.Show(ex.Message, "Failed Connection", MessageBoxButton.OK, MessageBoxImage.Warning);
            });

            promise.Always(() =>
            {
                OKButton.IsEnabled = true;
                CancelButton.IsEnabled = true;
                SiteUrlTextBox.IsEnabled = true;
                AuthenticationModeComboBox.IsEnabled = true;

                if (!UseCurrentUserCredentialsCheckBox.IsEnabled)
                {
                    UserNameTextBox.IsEnabled = true;
                    UserPasswordTextBox.IsEnabled = true;
                }

                if (authType == AuthType.Default)
                {
                    UseCurrentUserCredentialsCheckBox.IsEnabled = true;
                }
            });
        }

        private void CloseDialog()
        {
            if (this.Owner != null && this.Owner.OwnedWindows.Count > 0)
            {
                this.DialogResult = true;
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
            AuthType authType = ((AuthenticationModeComboBoxItem)this.AuthenticationModeComboBox.SelectedValue).Type;

            if (authType == AuthType.Anonymous)
            {
                UseCurrentUserCredentialsCheckBox.IsEnabled = false;
                UserNameTextBox.IsEnabled = false;
                UserPasswordTextBox.IsEnabled = false;
            }
            else
            {
                if (authType == AuthType.Forms || authType == AuthType.SharePointOnline)
                {
                    UseCurrentUserCredentialsCheckBox.IsEnabled = false;
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
    }
}