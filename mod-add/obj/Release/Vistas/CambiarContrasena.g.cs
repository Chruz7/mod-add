﻿#pragma checksum "..\..\..\Vistas\CambiarContrasena.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "665BAE7E964F8F84EA786C6B6E2E583859B6C8C2813693F7258D99C58EBE52BB"
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
    /// CambiarContrasena
    /// </summary>
    public partial class CambiarContrasena : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 42 "..\..\..\Vistas\CambiarContrasena.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.PasswordBox ContrasenaActual;
        
        #line default
        #line hidden
        
        
        #line 57 "..\..\..\Vistas\CambiarContrasena.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.PasswordBox ContrasenaNueva;
        
        #line default
        #line hidden
        
        
        #line 65 "..\..\..\Vistas\CambiarContrasena.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Cancelar;
        
        #line default
        #line hidden
        
        
        #line 92 "..\..\..\Vistas\CambiarContrasena.xaml"
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
            System.Uri resourceLocater = new System.Uri("/mod-add;component/vistas/cambiarcontrasena.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Vistas\CambiarContrasena.xaml"
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
            this.ContrasenaActual = ((System.Windows.Controls.PasswordBox)(target));
            return;
            case 2:
            this.ContrasenaNueva = ((System.Windows.Controls.PasswordBox)(target));
            return;
            case 3:
            this.Cancelar = ((System.Windows.Controls.Button)(target));
            
            #line 66 "..\..\..\Vistas\CambiarContrasena.xaml"
            this.Cancelar.Click += new System.Windows.RoutedEventHandler(this.Cancelar_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.Aceptar = ((System.Windows.Controls.Button)(target));
            
            #line 93 "..\..\..\Vistas\CambiarContrasena.xaml"
            this.Aceptar.Click += new System.Windows.RoutedEventHandler(this.Aceptar_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
