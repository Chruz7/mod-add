using mod_add.Datos.Modelos;
using mod_add.Enums;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace mod_add.Utils
{
    public static class Exportar
    {
        public static TipoRespuesta Excel(List<BitacoraModificacion> bitacora)
        {
            if (bitacora.Count == 0) return TipoRespuesta.SIN_REGISTROS;
            DateTime fecha = DateTime.Now;
            string path = ConfiguracionLocalServicio.ReadSetting("PATH-EXPORTACION-EXCEL");
            string fechaString = fecha.ToString("yyyyMMddHHmmss");
            string pathArchivo = $@"{path}\bitacora-{fechaString}.xlsx";

            try
            {
                SLDocument sl = new SLDocument();

                sl.SetCellValue(1, 1, "TIPO AJUSTE");
                sl.SetCellValue(1, 2, "FECHA DE PROCESO");
                sl.SetCellValue(1, 3, "FECHA INICIAL (VENTA)");
                sl.SetCellValue(1, 4, "FECHA FINAL (VENTA)");
                sl.SetCellValue(1, 5, "TOTAL CUENTAS");
                sl.SetCellValue(1, 6, "CUENTAS MODIFICADAS");
                sl.SetCellValue(1, 7, "IMPORTE ANTERIOR");
                sl.SetCellValue(1, 8, "IMPORTE NUEVO");
                sl.SetCellValue(1, 9, "DIFERENCIA %");

                int i = 2;
                foreach (var resgitro in bitacora)
                {
                    sl.SetCellValue(i, 1, $"{resgitro.TipoAjuste}");
                    sl.SetCellValue(i, 2, resgitro.FechaProceso);
                    sl.SetCellValue(i, 3, resgitro.FechaInicialVenta);
                    sl.SetCellValue(i, 4, resgitro.FechaFinalVenta);
                    sl.SetCellValue(i, 5, resgitro.TotalCuentas);
                    sl.SetCellValue(i, 6, resgitro.CuentasModificadas);
                    sl.SetCellValue(i, 7, resgitro.ImporteAnterior);
                    sl.SetCellValue(i, 8, resgitro.ImporteNuevo);
                    sl.SetCellValue(i, 9, resgitro.Diferencia);
                    i++;
                }
                sl.SaveAs(pathArchivo);

                return TipoRespuesta.HECHO;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
                return TipoRespuesta.ERROR;
            }
        }
    }
}
