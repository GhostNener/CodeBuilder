﻿#pragma checksum "..\..\MainWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "E6BDCE5DD4006CAD5A6FB27895A0BAFD"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.34014
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
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


namespace CodeBuilder {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 25 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tbConnStr;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnConnect;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tbNamespace;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cbTables;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cbDAL;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnGenerateCode;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tbSelectPath;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnExport;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnExportALL;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ColumnDefinition gridLeft;
        
        #line default
        #line hidden
        
        
        #line 53 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ColumnDefinition gridRight;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tbModel;
        
        #line default
        #line hidden
        
        
        #line 63 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tbDAL;
        
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
            System.Uri resourceLocater = new System.Uri("/CodeBuilder;component/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\MainWindow.xaml"
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
            
            #line 4 "..\..\MainWindow.xaml"
            ((CodeBuilder.MainWindow)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            
            #line 4 "..\..\MainWindow.xaml"
            ((CodeBuilder.MainWindow)(target)).Closed += new System.EventHandler(this.Window_Closed);
            
            #line default
            #line hidden
            return;
            case 2:
            this.tbConnStr = ((System.Windows.Controls.TextBox)(target));
            
            #line 25 "..\..\MainWindow.xaml"
            this.tbConnStr.KeyDown += new System.Windows.Input.KeyEventHandler(this.tbConnStr_KeyDown);
            
            #line default
            #line hidden
            return;
            case 3:
            this.btnConnect = ((System.Windows.Controls.Button)(target));
            
            #line 26 "..\..\MainWindow.xaml"
            this.btnConnect.Click += new System.Windows.RoutedEventHandler(this.btnConnect_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.tbNamespace = ((System.Windows.Controls.TextBox)(target));
            
            #line 40 "..\..\MainWindow.xaml"
            this.tbNamespace.GotFocus += new System.Windows.RoutedEventHandler(this.tbNamespace_GotFocus);
            
            #line default
            #line hidden
            
            #line 40 "..\..\MainWindow.xaml"
            this.tbNamespace.LostFocus += new System.Windows.RoutedEventHandler(this.tbNamespace_LostFocus);
            
            #line default
            #line hidden
            
            #line 40 "..\..\MainWindow.xaml"
            this.tbNamespace.PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(this.tbNamespace_PreviewMouseDown);
            
            #line default
            #line hidden
            return;
            case 5:
            this.cbTables = ((System.Windows.Controls.ComboBox)(target));
            
            #line 41 "..\..\MainWindow.xaml"
            this.cbTables.GotFocus += new System.Windows.RoutedEventHandler(this.cbTables_GotFocus);
            
            #line default
            #line hidden
            return;
            case 6:
            this.cbDAL = ((System.Windows.Controls.ComboBox)(target));
            
            #line 42 "..\..\MainWindow.xaml"
            this.cbDAL.GotFocus += new System.Windows.RoutedEventHandler(this.cbTables_GotFocus);
            
            #line default
            #line hidden
            return;
            case 7:
            this.btnGenerateCode = ((System.Windows.Controls.Button)(target));
            
            #line 43 "..\..\MainWindow.xaml"
            this.btnGenerateCode.Click += new System.Windows.RoutedEventHandler(this.btnGenerateCode_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.tbSelectPath = ((System.Windows.Controls.TextBox)(target));
            
            #line 44 "..\..\MainWindow.xaml"
            this.tbSelectPath.PreviewMouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.tbSelectPath_PreviewMouseDoubleClick);
            
            #line default
            #line hidden
            return;
            case 9:
            this.btnExport = ((System.Windows.Controls.Button)(target));
            
            #line 45 "..\..\MainWindow.xaml"
            this.btnExport.Click += new System.Windows.RoutedEventHandler(this.btnExport_Click);
            
            #line default
            #line hidden
            return;
            case 10:
            this.btnExportALL = ((System.Windows.Controls.Button)(target));
            
            #line 46 "..\..\MainWindow.xaml"
            this.btnExportALL.Click += new System.Windows.RoutedEventHandler(this.btnExportALL_Click);
            
            #line default
            #line hidden
            return;
            case 11:
            this.gridLeft = ((System.Windows.Controls.ColumnDefinition)(target));
            return;
            case 12:
            this.gridRight = ((System.Windows.Controls.ColumnDefinition)(target));
            return;
            case 13:
            this.tbModel = ((System.Windows.Controls.TextBox)(target));
            
            #line 56 "..\..\MainWindow.xaml"
            this.tbModel.PreviewMouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.TextBox_PreviewMouseDoubleClick);
            
            #line default
            #line hidden
            return;
            case 14:
            
            #line 59 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.btnCopyModel);
            
            #line default
            #line hidden
            return;
            case 15:
            this.tbDAL = ((System.Windows.Controls.TextBox)(target));
            
            #line 63 "..\..\MainWindow.xaml"
            this.tbDAL.PreviewMouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.TextBox_PreviewMouseDoubleClick);
            
            #line default
            #line hidden
            return;
            case 16:
            
            #line 66 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.btnCopyDAL);
            
            #line default
            #line hidden
            return;
            case 17:
            
            #line 73 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.TextBlock)(target)).MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.TextBlock_MouseDown);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

