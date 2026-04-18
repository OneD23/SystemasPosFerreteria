# Guía rápida: Reportes fiscales RD (DGII) en el sistema

Se agregó un servicio para generar **preformatos CSV** de reportes fiscales:

- **607 (Ventas / comprobantes emitidos)**
- **608 (Comprobantes anulados)**

## Archivos agregados

- `SistemaFerreteriaV8/Clases/Fiscal/FiscalReportService.cs`
- Métodos wrapper en `SistemaFerreteriaV8/Clases/Reportes.cs`

## Uso desde código

```csharp
var reportes = new Reportes();
var config = new Configuraciones().ObtenerPorId(1);

string carpeta = @"C:\ReportesFiscales";
var desde = new DateTime(2026, 4, 1);
var hasta = new DateTime(2026, 4, 30);

string archivo607 = await reportes.ExportarReporteFiscal607CsvAsync(desde, hasta, carpeta, config?.RNC ?? "");
string archivo608 = await reportes.ExportarReporteFiscal608CsvAsync(desde, hasta, carpeta, config?.RNC ?? "");
```

## Notas importantes

- Los archivos se generan como **preformato** para facilitar validación/carga.
- Columnas de impuestos/retenciones no presentes en el modelo actual se exportan en `0.00`.
- El campo de anulaciones (608) mapea el motivo de forma básica a un código de tipo de anulación.
- Antes de envío oficial a DGII, validar contenido y formato según versión vigente de norma/plantilla.

