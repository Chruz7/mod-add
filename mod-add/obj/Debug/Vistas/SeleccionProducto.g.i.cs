﻿#pragma checksum "..\..\..\Vistas\SeleccionProducto.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "5A951986BB3136FE70C3A33E047F2ADD8112AA56793D1A4341F0945B2C656961"
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
    /// SeleccionProducto
    /// </summary>
    public partial class SeleccionProducto : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 38 "..\..\..\Vistas\SeleccionProducto.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox Grupos;
        
        #line default
        #line hidden
        
        
        #line 53 "..\..\..\Vistas\SeleccionProducto.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Buscador;
        
        #line default
        #line hidden
        
        
        #line 63 "..\..\..\Vistas\SeleccionProducto.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid Productos;
        
        #line default
        #line hidden
        
        
        #line 98 "..\..\..\Vistas\SeleccionProducto.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Cancelar;
        
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
            System.Uri resourceLocater = new System.Uri("/mod-add;component/vistas/seleccionproducto.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Vistas\SeleccionProducto.xaml"
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
            this.Grupos = ((System.Windows.Controls.ComboBox)(target));
            
            #line 40 "..\..\..\Vistas\SeleccionProducto.xaml"
            this.Grupos.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.Grupos_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 2:
            this.Buscador = ((System.Windows.Controls.TextBox)(target));
            
            #line 56 "..\..\..\Vistas\SeleccionProducto.xaml"
            this.Buscador.KeyUp += new System.Windows.Input.KeyEventHandler(this.Buscador_KeyUp);
            
            #line default
            #line hidden
            return;
            case 3:
            this.Productos = ((System.Windows.Controls.DataGrid)(target));
            
            #line 67 "..\..\..\Vistas\SeleccionProducto.xaml"
            this.Productos.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.Productos_MouseDoubleClick);
            
            #line default
            #line hidden
            return;
            case 4:
            this.Cancelar = ((System.Windows.Controls.Button)(target));
            
            #line 99 "..\..\..\Vistas\SeleccionProducto.xaml"
            this.Cancelar.Click += new System.Windows.RoutedEventHandler(this.Cancelar_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

