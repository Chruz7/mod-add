using mod_add.Datos.Modelos;
using mod_add.Enums;
using SpreadsheetLight;
using SRLibrary.SR_DTO;
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

        public static TipoRespuesta Excel(List<SR_bitacorafiscal> bitacora)
        {
            if (bitacora.Count == 0) return TipoRespuesta.SIN_REGISTROS;
            DateTime fecha = DateTime.Now;
            string path = ConfiguracionLocalServicio.ReadSetting("PATH-EXPORTACION-EXCEL");
            string fechaString = fecha.ToString("yyyyMMddHHmmss");
            string pathArchivo = $@"{path}\bitacora-{fechaString}.xlsx";

            try
            {
                SLDocument sl = new SLDocument();

                SLStyle styleColumn = sl.CreateStyle();
                styleColumn.SetHorizontalAlignment(DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center);
                styleColumn.SetVerticalAlignment(DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Center);
                styleColumn.SetWrapText(true);

                SLStyle styleColumn2 = sl.CreateStyle();
                styleColumn2.SetHorizontalAlignment(DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Right);
                styleColumn2.SetWrapText(true);

                sl.SetColumnStyle(1, styleColumn);
                sl.SetColumnStyle(2, styleColumn);
                sl.SetColumnStyle(3, styleColumn);
                sl.SetColumnStyle(4, styleColumn);
                sl.SetColumnStyle(5, styleColumn);
                sl.SetColumnStyle(9, styleColumn);

                styleColumn.FormatCode = "#,##0.00";
                sl.SetColumnStyle(8, styleColumn);

                styleColumn2.FormatCode = "#,##0.0000";
                sl.SetColumnStyle(6, styleColumn2);
                sl.SetColumnStyle(7, styleColumn2);

                sl.SetCellStyle(1, 6, styleColumn);
                sl.SetCellStyle(1, 7, styleColumn);

                sl.SetColumnWidth(1, 24);
                sl.SetColumnWidth(2, 24);
                sl.SetColumnWidth(3, 24);
                sl.SetColumnWidth(4, 15);
                sl.SetColumnWidth(5, 15);
                sl.SetColumnWidth(6, 15);
                sl.SetColumnWidth(7, 15);
                sl.SetColumnWidth(8, 15);
                sl.SetColumnWidth(9, 15);

                sl.SetCellValue(1, 1, "FECHA DE PROCESO");
                sl.SetCellValue(1, 2, "FECHA INICIAL (VENTA)");
                sl.SetCellValue(1, 3, "FECHA FINAL (VENTA)");
                sl.SetCellValue(1, 4, $"TOTAL CUENTAS");
                sl.SetCellValue(1, 5, "CUENTAS MODIFICADAS");
                sl.SetCellValue(1, 6, "IMPORTE ANTERIOR");
                sl.SetCellValue(1, 7, "IMPORTE NUEVO");
                sl.SetCellValue(1, 8, "DIFERENCIA %");
                sl.SetCellValue(1, 9, "TIPO DE ELIMINACION");
                //sl.SetCellValue(1, 10, "MODO DE ELIMINACION");

                int i = 2;
                foreach (var resgitro in bitacora)
                {
                    sl.SetCellValue(i, 1, resgitro.fecha.Value.ToString());
                    sl.SetCellValue(i, 2, resgitro.fechainicial.Value.ToString());
                    sl.SetCellValue(i, 3, resgitro.fechafinal.Value.ToString());
                    sl.SetCellValue(i, 4, resgitro.cuentastotal.Value);
                    sl.SetCellValue(i, 5, resgitro.cuentasmodificadas.Value);
                    sl.SetCellValue(i, 6, resgitro.importeanterior.Value);
                    sl.SetCellValue(i, 7, resgitro.importenuevo.Value);
                    sl.SetCellValue(i, 8, resgitro.diferencia.Value);
                    sl.SetCellValue(i, 9, resgitro.tipo);
                    //sl.SetCellValue(i, 10, resgitro.tipo);
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
