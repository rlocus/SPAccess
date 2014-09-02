﻿#pragma checksum "..\..\OpenSiteWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C24939D73556B57498EA902095F78249"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using SP2013Access.Validation;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace SP2013Access {
    
    
    /// <summary>
    /// OpenSiteWindow
    /// </summary>
    public partial class OpenSiteWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 1 "..\..\OpenSiteWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal SP2013Access.OpenSiteWindow OpenSite;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\OpenSiteWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox SiteUrlTextBox;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\OpenSiteWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox AuthenticationModeComboBox;
        
        #line default
        #line hidden
        
        
        #line 54 "..\..\OpenSiteWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox UseCurrentUserCredentialsCheckBox;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\OpenSiteWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox UserNameTextBox;
        
        #line default
        #line hidden
        
        
        #line 68 "..\..\OpenSiteWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.PasswordBox UserPasswordTextBox;
        
        #line default
        #line hidden
        
        
        #line 69 "..\..\OpenSiteWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button OKButton;
        
        #line default
        #line hidden
        
        
        #line 86 "..\..\OpenSiteWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CancelButton;
        
        #line default
        #line hidden
        
        
        #line 87 "..\..\OpenSiteWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label MessageLabel;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/SP2013Access;component/opensitewindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\OpenSiteWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.OpenSite = ((SP2013Access.OpenSiteWindow)(target));
            return;
            case 2:
            
            #line 25 "..\..\OpenSiteWindow.xaml"
            ((System.Windows.Controls.Grid)(target)).AddHandler(System.Windows.Controls.Validation.ErrorEvent, new System.EventHandler<System.Windows.Controls.ValidationErrorEventArgs>(this.OnValidationError));
            
            #line default
            #line hidden
            return;
            case 3:
            this.SiteUrlTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.AuthenticationModeComboBox = ((System.Windows.Controls.ComboBox)(target));
            
            #line 52 "..\..\OpenSiteWindow.xaml"
            this.AuthenticationModeComboBox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.AuthenticationModeComboBox_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 5:
            this.UseCurrentUserCredentialsCheckBox = ((System.Windows.Controls.CheckBox)(target));
            
            #line 54 "..\..\OpenSiteWindow.xaml"
            this.UseCurrentUserCredentialsCheckBox.Checked += new System.Windows.RoutedEventHandler(this.UseCurrentUserCredentialsCheckBox_Checked);
            
            #line default
            #line hidden
            
            #line 54 "..\..\OpenSiteWindow.xaml"
            this.UseCurrentUserCredentialsCheckBox.Unchecked += new System.Windows.RoutedEventHandler(this.UseCurrentUserCredentialsCheckBox_Unchecked);
            
            #line default
            #line hidden
            return;
            case 6:
            this.UserNameTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            this.UserPasswordTextBox = ((System.Windows.Controls.PasswordBox)(target));
            return;
            case 8:
            this.OKButton = ((System.Windows.Controls.Button)(target));
            
            #line 69 "..\..\OpenSiteWindow.xaml"
            this.OKButton.Click += new System.Windows.RoutedEventHandler(this.OK_OnClick);
            
            #line default
            #line hidden
            return;
            case 9:
            this.CancelButton = ((System.Windows.Controls.Button)(target));
            
            #line 86 "..\..\OpenSiteWindow.xaml"
            this.CancelButton.Click += new System.Windows.RoutedEventHandler(this.Cancel_OnClick);
            
            #line default
            #line hidden
            return;
            case 10:
            this.MessageLabel = ((System.Windows.Controls.Label)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

