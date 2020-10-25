﻿using mod_add.Datos.Contexto;
using mod_add.Datos.Implementaciones;
using mod_add.Datos.Infraestructura;
using mod_add.Datos.Interfaces;
using mod_add.Datos.Modelos;
using mod_add.Enums;
using SRLibrary.SR_Context;
using SRLibrary.SR_DAO;
using SRLibrary.SR_DTO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Migrations;
using System.Linq;

namespace mod_add.ViewModels
{
    public class EliminarProductosViewModel : ViewModelBase
    {
        private readonly DatabaseFactory dbf;
        private readonly IProductoEliminarServicio _productoEliminarServicio;
        public EliminarProductosViewModel()
        {
            dbf = new DatabaseFactory();
            _productoEliminarServicio = new ProductoEliminarServicio(dbf);

            ProductosEliminar = new ObservableCollection<ProductoEliminacion>();

            ObtenerProductosSR();
            ObtenerProductosEliminar();
            GenerarListado();
        }

        public TipoRespuesta Guardar()
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                try
                {
                    foreach (var productoEliminar in ProductosEliminar.Where(x => x.Eliminar || x.Guardado))
                    {
                        context.ProductosEliminar.AddOrUpdate(productoEliminar);
                        context.SaveChanges();
                    }

                    App.ProductosEliminar = context.ProductosEliminar.Where(x => x.Eliminar).ToList();

                    return TipoRespuesta.HECHO;
                }
                catch
                {
                    return TipoRespuesta.ERROR;
                }
            }
        }

        public void ObtenerProductosSR()
        {
            using (SoftRestaurantDBContext context = new SoftRestaurantDBContext())
            {
                SR_productos_DAO productos_DAO = new SR_productos_DAO(context);

                Productos = productos_DAO.GetAll();
            }
        }

        public void ObtenerProductosEliminar()
        {
            ProductosEliminables = _productoEliminarServicio.GetAll().ToList();
        }

        public void GenerarListado()
        {
            foreach (var producto in Productos)
            {
                bool eliminar = false;
                int id = 0;
                bool guardado = false;

                if (ProductosEliminables.Count > 0)
                {
                    var productoEliminable = ProductosEliminables.Where(x => x.Clave == producto.idproducto).FirstOrDefault();

                    if (productoEliminable != null)
                    {
                        id = productoEliminable.Id;
                        eliminar = productoEliminable.Eliminar;
                        guardado = true;
                    }
                }

                var productoEliminar = new ProductoEliminacion
                {
                    Clave = producto.idproducto,
                    Grupo = producto.idgrupo,
                    Descripcion = producto.descripcion,
                    Eliminar = eliminar,
                    Guardado = guardado
                };

                if (id > 0)
                {
                    productoEliminar.Id = id;
                }

                ProductosEliminar.Add(productoEliminar);
            }
        }

        private List<ProductoEliminacion> ProductosEliminables { get; set; }
        private List<SR_productos> Productos { get; set; }

        private ObservableCollection<ProductoEliminacion> _ProductosEliminar;
        public ObservableCollection<ProductoEliminacion> ProductosEliminar
        {
            get { return _ProductosEliminar; }
            set
            {
                _ProductosEliminar = value;
                OnPropertyChanged(nameof(ProductosEliminar));
            }
        }
    }
}
