﻿#pragma checksum "..\..\..\..\..\..\Workspace\Usercontrols\MiscView.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "C93F4EE0557118C12C24A6B32E8571B1F630A21E"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Galaxy_Swapper_v2.Workspace.Usercontrols;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
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


namespace Galaxy_Swapper_v2.Workspace.Usercontrols {
    
    
    /// <summary>
    /// MiscView
    /// </summary>
    public partial class MiscView : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 14 "..\..\..\..\..\..\Workspace\Usercontrols\MiscView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ScrollViewer Frontend_Viewer;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\..\..\..\..\Workspace\Usercontrols\MiscView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.WrapPanel Frontend_Items;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\..\..\..\..\Workspace\Usercontrols\MiscView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border FOVBorder;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\..\..\..\..\Workspace\Usercontrols\MiscView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image FOVIcon;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\..\..\..\..\Workspace\Usercontrols\MiscView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock FOVTextblock;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.9.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Galaxy Swapper v2;component/workspace/usercontrols/miscview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\..\Workspace\Usercontrols\MiscView.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.9.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 10 "..\..\..\..\..\..\Workspace\Usercontrols\MiscView.xaml"
            ((Galaxy_Swapper_v2.Workspace.Usercontrols.MiscView)(target)).Loaded += new System.Windows.RoutedEventHandler(this.MiscView_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.Frontend_Viewer = ((System.Windows.Controls.ScrollViewer)(target));
            return;
            case 3:
            this.Frontend_Items = ((System.Windows.Controls.WrapPanel)(target));
            return;
            case 4:
            this.FOVBorder = ((System.Windows.Controls.Border)(target));
            
            #line 16 "..\..\..\..\..\..\Workspace\Usercontrols\MiscView.xaml"
            this.FOVBorder.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.FOV_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.FOVIcon = ((System.Windows.Controls.Image)(target));
            return;
            case 6:
            this.FOVTextblock = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

