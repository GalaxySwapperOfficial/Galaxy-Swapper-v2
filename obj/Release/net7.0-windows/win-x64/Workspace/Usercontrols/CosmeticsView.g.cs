﻿#pragma checksum "..\..\..\..\..\..\Workspace\Usercontrols\CosmeticsView.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "3055A91191CCFC41553221465240C31183E34033"
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
    /// CosmeticsView
    /// </summary>
    public partial class CosmeticsView : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 14 "..\..\..\..\..\..\Workspace\Usercontrols\CosmeticsView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ScrollViewer Frontend_Viewer;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\..\..\..\..\Workspace\Usercontrols\CosmeticsView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.WrapPanel Frontend_Items;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\..\..\..\..\Workspace\Usercontrols\CosmeticsView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ScrollViewer Option_Viewer;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\..\..\..\..\Workspace\Usercontrols\CosmeticsView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.WrapPanel Option_Items;
        
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
            System.Uri resourceLocater = new System.Uri("/Galaxy Swapper v2;component/workspace/usercontrols/cosmeticsview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\..\Workspace\Usercontrols\CosmeticsView.xaml"
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
            
            #line 10 "..\..\..\..\..\..\Workspace\Usercontrols\CosmeticsView.xaml"
            ((Galaxy_Swapper_v2.Workspace.Usercontrols.CosmeticsView)(target)).Loaded += new System.Windows.RoutedEventHandler(this.CosmeticsView_Loaded);
            
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
            this.Option_Viewer = ((System.Windows.Controls.ScrollViewer)(target));
            return;
            case 5:
            this.Option_Items = ((System.Windows.Controls.WrapPanel)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

