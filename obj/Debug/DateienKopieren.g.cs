﻿#pragma checksum "..\..\DateienKopieren.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "AEAEB122562994CFE6917DB68A9FE13EF534C8EB59D135B885013E70B2A8A367"
//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

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


namespace HeosUpdateCreator {
    
    
    /// <summary>
    /// DateienKopieren
    /// </summary>
    public partial class DateienKopieren : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 48 "..\..\DateienKopieren.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button buttonKopieren;
        
        #line default
        #line hidden
        
        
        #line 60 "..\..\DateienKopieren.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border borderButtonAbbrechen;
        
        #line default
        #line hidden
        
        
        #line 69 "..\..\DateienKopieren.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button buttonAbbrechen;
        
        #line default
        #line hidden
        
        
        #line 89 "..\..\DateienKopieren.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button buttonWeiter;
        
        #line default
        #line hidden
        
        
        #line 154 "..\..\DateienKopieren.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelOriginPath;
        
        #line default
        #line hidden
        
        
        #line 163 "..\..\DateienKopieren.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid dataGridSource;
        
        #line default
        #line hidden
        
        
        #line 195 "..\..\DateienKopieren.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelDestinationPath;
        
        #line default
        #line hidden
        
        
        #line 204 "..\..\DateienKopieren.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid dataGridTarget;
        
        #line default
        #line hidden
        
        
        #line 227 "..\..\DateienKopieren.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelCopyInProgress;
        
        #line default
        #line hidden
        
        
        #line 237 "..\..\DateienKopieren.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelCopyProgressPercent;
        
        #line default
        #line hidden
        
        
        #line 247 "..\..\DateienKopieren.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelCopyFileInfo;
        
        #line default
        #line hidden
        
        
        #line 257 "..\..\DateienKopieren.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ProgressBar copyProgressBar;
        
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
            System.Uri resourceLocater = new System.Uri("/HeosUpdateCreator;component/dateienkopieren.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\DateienKopieren.xaml"
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
            this.buttonKopieren = ((System.Windows.Controls.Button)(target));
            
            #line 51 "..\..\DateienKopieren.xaml"
            this.buttonKopieren.Click += new System.Windows.RoutedEventHandler(this.buttonKopieren_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.borderButtonAbbrechen = ((System.Windows.Controls.Border)(target));
            return;
            case 3:
            this.buttonAbbrechen = ((System.Windows.Controls.Button)(target));
            
            #line 72 "..\..\DateienKopieren.xaml"
            this.buttonAbbrechen.Click += new System.Windows.RoutedEventHandler(this.buttonAbbrechen_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.buttonWeiter = ((System.Windows.Controls.Button)(target));
            
            #line 92 "..\..\DateienKopieren.xaml"
            this.buttonWeiter.Click += new System.Windows.RoutedEventHandler(this.buttonWeiter_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.labelOriginPath = ((System.Windows.Controls.Label)(target));
            return;
            case 6:
            this.dataGridSource = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 7:
            this.labelDestinationPath = ((System.Windows.Controls.Label)(target));
            return;
            case 8:
            this.dataGridTarget = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 9:
            this.labelCopyInProgress = ((System.Windows.Controls.Label)(target));
            return;
            case 10:
            this.labelCopyProgressPercent = ((System.Windows.Controls.Label)(target));
            return;
            case 11:
            this.labelCopyFileInfo = ((System.Windows.Controls.Label)(target));
            return;
            case 12:
            this.copyProgressBar = ((System.Windows.Controls.ProgressBar)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
