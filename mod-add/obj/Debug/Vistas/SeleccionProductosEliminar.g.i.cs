﻿#pragma checksum "..\..\..\Vistas\SeleccionProductosEliminar.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "0404EAC85171E8AFC4536015D9E52EE3183031809E8281F66AEE4420AF7FBCBA"
//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
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
using mod_add.Vistas;


namespace mod_add.Vistas {
    
    
    /// <summary>
    /// SeleccionProductosEliminar
    /// </summary>
    public partial class SeleccionProductosEliminar : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 29 "..\..\..\Vistas\SeleccionProductosEliminar.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button SeleccionarGrupo;
        
        #line default
        #line hidden
        
        
        #line 59 "..\..\..\Vistas\SeleccionProductosEliminar.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid ProductosEliminar;
        
        #line default
        #line hidden
        
        
        #line 85 "..\..\..\Vistas\SeleccionProductosEliminar.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Cancelar;
        
        #line default
        #line hidden
        
        
        #line 114 "..\..\..\Vistas\SeleccionProductosEliminar.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Aceptar;
        
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
            System.Uri resourceLocater = new System.Uri("/mod-add;component/vistas/seleccionproductoseliminar.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Vistas\SeleccionProductosEliminar.xaml"
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
            this.SeleccionarGrupo = ((System.Windows.Controls.Button)(target));
            
            #line 30 "..\..\..\Vistas\SeleccionProductosEliminar.xaml"
            this.SeleccionarGrupo.Click += new System.Windows.RoutedEventHandler(this.SeleccionarGrupo_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.ProductosEliminar = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 3:
            this.Cancelar = ((System.Windows.Controls.Button)(target));
            
            #line 86 "..\..\..\Vistas\SeleccionProductosEliminar.xaml"
            this.Cancelar.Click += new System.Windows.RoutedEventHandler(this.Cancelar_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.Aceptar = ((System.Windows.Controls.Button)(target));
            
            #line 115 "..\..\..\Vistas\SeleccionProductosEliminar.xaml"
            this.Aceptar.Click += new System.Windows.RoutedEventHandler(this.Aceptar_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

