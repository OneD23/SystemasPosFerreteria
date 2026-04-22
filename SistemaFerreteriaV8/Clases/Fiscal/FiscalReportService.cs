using SistemaFerreteriaV8.Clases;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFerreteriaV8.Clases.Fiscal
{
    /// <summary>
    /// Genera archivos base para reportes fiscales RD (607 y 608) a partir de facturas del sistema.
    /// Nota: estos reportes son un preformato para facilitar carga/revisión.
    /// </summary>
    public class FiscalReportService
    {
        public async Task<string> ExportarReporte607CsvAsync(DateTime desde, DateTime hasta, string carpetaSalida, string rncEmisor)
        {
            var facturas = await Factura.ListarTodasAsync(incluirEliminadas: false);
            var ventas = facturas
                .Where(f => f != null)
                .Where(f => f.Fecha.Date >= desde.Date && f.Fecha.Date <= hasta.Date)
                .Where(f => !f.Cotizacion && !f.Eliminada)
                .Where(f => !string.IsNullOrWhiteSpace(f.NFC))
                .OrderBy(f => f.Fecha)
                .ThenBy(f => f.Id)
                .ToList();

            Directory.CreateDirectory(carpetaSalida);
            var periodo = desde.ToString("yyyyMM");
            var filePath = Path.Combine(carpetaSalida, $"DGII_607_{periodo}.csv");

            var sb = new StringBuilder();
            sb.AppendLine("RNC_CEDULA,PERIODO,NCF,NCF_MODIFICADO,TIPO_ID_CLIENTE,RNC_CEDULA_CLIENTE,FECHA_COMPROBANTE,FECHA_RETENCION,MONTO_FACTURADO,ITBIS_FACTURADO,ITBIS_RETENIDO,RETENCION_RENTA,TIPO_INGRESO,FORMA_PAGO");

            foreach (var f in ventas)
            {
                var rncCliente = SoloDigitos(f.RNC);
                var tipoId = string.IsNullOrWhiteSpace(rncCliente) ? "" : (rncCliente.Length >= 9 ? "1" : "2");
                var ncf = LimpiarNcf(f.NFC);

                sb.AppendLine(string.Join(",",
                    EscapeCsv(SoloDigitos(rncEmisor)),
                    EscapeCsv(f.Fecha.ToString("yyyyMM")),
                    EscapeCsv(ncf),
                    EscapeCsv(""),
                    EscapeCsv(tipoId),
                    EscapeCsv(rncCliente),
                    EscapeCsv(f.Fecha.ToString("yyyyMMdd")),
                    EscapeCsv(""),
                    EscapeCsv(FormatDecimal(f.Total)),
                    EscapeCsv("0.00"),
                    EscapeCsv("0.00"),
                    EscapeCsv("0.00"),
                    EscapeCsv("01"),
                    EscapeCsv(MapFormaPago(f.MetodoDePago))));
            }

            await File.WriteAllTextAsync(filePath, sb.ToString(), Encoding.UTF8);
            return filePath;
        }

        public async Task<string> ExportarReporte608CsvAsync(DateTime desde, DateTime hasta, string carpetaSalida, string rncEmisor)
        {
            var facturas = await Factura.ListarTodasAsync(incluirEliminadas: true);
            var anuladas = facturas
                .Where(f => f != null && f.Eliminada)
                .Where(f => !string.IsNullOrWhiteSpace(f.NFC))
                .Where(f => (f.FechaEliminacion ?? f.Fecha).Date >= desde.Date && (f.FechaEliminacion ?? f.Fecha).Date <= hasta.Date)
                .OrderBy(f => f.FechaEliminacion ?? f.Fecha)
                .ThenBy(f => f.Id)
                .ToList();

            Directory.CreateDirectory(carpetaSalida);
            var periodo = desde.ToString("yyyyMM");
            var filePath = Path.Combine(carpetaSalida, $"DGII_608_{periodo}.csv");

            var sb = new StringBuilder();
            sb.AppendLine("RNC_CEDULA,PERIODO,NCF,FECHA_ANULACION,TIPO_ANULACION,MONTO,COMENTARIO");

            foreach (var f in anuladas)
            {
                var fechaAnulacion = f.FechaEliminacion ?? f.Fecha;
                var tipoAnulacion = MapTipoAnulacion(f.MotivoEliminacion);

                sb.AppendLine(string.Join(",",
                    EscapeCsv(SoloDigitos(rncEmisor)),
                    EscapeCsv(fechaAnulacion.ToString("yyyyMM")),
                    EscapeCsv(LimpiarNcf(f.NFC)),
                    EscapeCsv(fechaAnulacion.ToString("yyyyMMdd")),
                    EscapeCsv(tipoAnulacion),
                    EscapeCsv(FormatDecimal(f.Total)),
                    EscapeCsv((f.MotivoEliminacion ?? string.Empty).Trim())));
            }

            await File.WriteAllTextAsync(filePath, sb.ToString(), Encoding.UTF8);
            return filePath;
        }

        private static string MapFormaPago(string metodo)
        {
            var value = (metodo ?? string.Empty).Trim().ToLowerInvariant();
            if (value.Contains("efectivo")) return "01";
            if (value.Contains("cheque") || value.Contains("transfer")) return "02";
            if (value.Contains("tarjeta")) return "03";
            if (value.Contains("credito")) return "04";
            return "05"; // permuta/otros
        }

        private static string MapTipoAnulacion(string motivo)
        {
            var value = (motivo ?? string.Empty).Trim().ToLowerInvariant();
            if (value.Contains("error") && value.Contains("ncf")) return "01";
            if (value.Contains("error") && value.Contains("cliente")) return "02";
            if (value.Contains("devol")) return "03";
            if (value.Contains("producto")) return "04";
            if (value.Contains("servicio")) return "05";
            return "06"; // otros
        }

        private static string LimpiarNcf(string ncf)
        {
            if (string.IsNullOrWhiteSpace(ncf)) return string.Empty;
            return ncf.Replace("NCF:", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace(" ", string.Empty)
                .Trim();
        }

        private static string SoloDigitos(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            return new string(value.Where(char.IsDigit).ToArray());
        }

        private static string FormatDecimal(double value)
            => value.ToString("0.00", CultureInfo.InvariantCulture);

        private static string EscapeCsv(string value)
        {
            value ??= string.Empty;
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            }

            return value;
        }
    }
}
