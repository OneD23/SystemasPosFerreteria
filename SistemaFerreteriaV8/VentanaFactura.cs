using Microsoft.VisualBasic;
using SistemaFerreteriaV8.Clases;
using SistemaFerreteriaV8.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemaFerreteriaV8
{
    public partial class VentanaFactura : Form
    {
        public Factura Factura { get; set; }
        private readonly Label lblEstado = new Label { AutoSize = true, Visible = false };

        public VentanaFactura()
        {
            InitializeComponent();
            SistemaFerreteriaV8.Clases.ThemeManager.ApplyToForm(this);
            ModernizarFormulario();
            ConfigurarAtajosTeclado();
            AjustarLayoutFactura();
            Resize += (_, _) => AjustarLayoutFactura();
        }

        private void ModernizarFormulario()
        {
            UiConsistencia.AplicarFormularioBase(this);
            UiConsistencia.AplicarBotonPrimario(Actualizar);
            UiConsistencia.AplicarBotonPeligro(Eliminar);
            UiConsistencia.AplicarBotonAccion(button1);
            UiConsistencia.AplicarBotonExito(button2);

            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 41, 59);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.FromArgb(15, 23, 42);
            dataGridView1.GridColor = Color.FromArgb(226, 232, 240);
            dataGridView1.RowTemplate.Height = Math.Max(dataGridView1.RowTemplate.Height, 26);

            UiConsistencia.AplicarStatusLabel(lblEstado, Math.Max(button1.Bottom, button2.Bottom) + 10);
            Controls.Add(lblEstado);
        }

        private void ConfigurarAtajosTeclado()
        {
            KeyPreview = true;
            KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    Close();
                    e.SuppressKeyPress = true;
                }
            };

            IdFactura.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    RegistrarFactura();
                }
            };
        }

        private void AjustarLayoutFactura()
        {
            const int margen = 16;
            const int separacion = 8;
            int anchoUtil = ClientSize.Width - (margen * 2);

            Titulo.Location = new Point((ClientSize.Width - Titulo.Width) / 2, 18);
            DireccionNegocio.Location = new Point((ClientSize.Width - DireccionNegocio.Width) / 2, Titulo.Bottom + separacion);
            tel.Location = new Point((ClientSize.Width - tel.Width) / 2, DireccionNegocio.Bottom + separacion);
            label7.Location = new Point((ClientSize.Width / 2) - 50, tel.Bottom + separacion);
            RNC.Location = new Point(label7.Right + 6, label7.Top);

            TipoFactura.Location = new Point(margen, label7.Bottom + 28);

            int yInfo = TipoFactura.Bottom + 12;
            label4.Location = new Point(margen, yInfo);
            NFC.Location = new Point(label4.Right + 6, yInfo);
            label9.Location = new Point(margen, yInfo + 24);
            Valido.Location = new Point(label9.Right + 6, yInfo + 24);

            label8.Location = new Point(ClientSize.Width - 190, yInfo);
            IdFactura.Location = new Point(label8.Right + 6, yInfo - 2);
            IdFactura.Width = 90;

            label10.Location = new Point(margen, yInfo + 52);
            Fecha.Location = new Point(label10.Right + 6, yInfo + 52);
            label12.Location = new Point(margen, yInfo + 80);
            RNCCliente.Location = new Point(label12.Right + 6, yInfo + 80);
            label14.Location = new Point(margen, yInfo + 108);
            Cliente.Location = new Point(label14.Right + 6, yInfo + 108);
            label11.Location = new Point(margen, yInfo + 136);
            Direccion.Location = new Point(label11.Right + 6, yInfo + 136);

            dataGridView1.Location = new Point(margen, yInfo + 168);
            dataGridView1.Size = new Size(anchoUtil, Math.Max(170, ClientSize.Height - dataGridView1.Top - 150));

            int yPie = dataGridView1.Bottom + 8;
            label13.Location = new Point(ClientSize.Width - 118, yPie);
            total.Location = new Point(ClientSize.Width - 78, yPie);

            int wBtn = 120;
            int hBtn = 44;
            int yBtns = ClientSize.Height - hBtn - 16;
            button2.SetBounds(margen, yBtns, wBtn, hBtn);
            button1.SetBounds(button2.Right + 20, yBtns, wBtn, hBtn);
            Actualizar.SetBounds(button1.Right + 20, yBtns, wBtn, hBtn);
            Eliminar.SetBounds(Actualizar.Right + 20, yBtns, wBtn, hBtn);
        }

        private void MostrarEstado(string mensaje, bool error = false)
        {
            UiConsistencia.MostrarEstado(lblEstado, mensaje, error);
        }

        private void VentanaFactura_Load(object sender, EventArgs e)
        {
            if (Factura != null)
            {
                Configuraciones config = new Configuraciones().ObtenerPorId(1);

                Titulo.Text = config?.Nombre ?? "";
                DireccionNegocio.Text = config?.Direccion ?? "";
                tel.Text = config?.Telefono ?? "";

                string serie = "B02";
                if (Factura.TipoFactura == "Comprobante Fiscal")
                    serie = "B01";
                else if (Factura.TipoFactura == "Comprobante Gubernamental")
                    serie = "B15";

                IdFactura.Text = Factura.Id.ToString() ?? "";
                NFC.Text = serie + (Factura.NFC ?? "");
                Valido.Text = config?.FechaExpiracion.ToString() ?? "";
                RNC.Text = config?.RNC ?? "";

                TipoFactura.Text = Factura.TipoFactura ?? "";
                RNCCliente.Text = Factura.RNC ?? "";
                Cliente.Text = Factura.NombreCliente ?? "";
                Direccion.Text = Factura.Direccion ?? "";
                Fecha.Text = Factura.Fecha != null ? Factura.Fecha.ToString() : "";

                if (Factura.Productos != null)
                {
                    foreach (var item in Factura.Productos)
                    {
                        dataGridView1.Rows.Add(item.Cantidad, item.Producto?.Nombre, item.Precio, item.Cantidad * item.Precio);
                    }
                }
                total.Text = Factura.Total.ToString("c2");
            }

            AjustarLayoutFactura();
        }

        private async void Eliminar_Click(object sender, EventArgs e)
        {
            if (Factura == null)
                return;

            if (MessageBox.Show("¿Está seguro que desea eliminar esta factura?", "Aviso", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                string motivo = Interaction.InputBox("Escriba la razón de eliminación de la factura:", "Motivo obligatorio", "");
                if (string.IsNullOrWhiteSpace(motivo))
                {
                    MessageBox.Show("Debe indicar una razón para eliminar la factura.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Form1 frmPrincipal = WinFormsApp.OpenForms["Form1"] as Form1;
                Empleado usuarioActual = frmPrincipal?.EmpleadoActivo;

                Factura.MotivoEliminacion = motivo.Trim();
                Factura.FechaEliminacion = DateTime.Now;
                if (usuarioActual != null)
                {
                    Factura.EliminadaPorId = usuarioActual.Id.ToString();
                    Factura.EliminadaPorNombre = usuarioActual.Nombre;
                }
                else
                {
                    Factura.EliminadaPorId = "N/A";
                    Factura.EliminadaPorNombre = "Usuario no identificado";
                }

                await Factura.EliminarFacturaAsync();
                MostrarEstado("Factura eliminada correctamente.");
                MessageBox.Show(
                    $"Factura eliminada correctamente.\nMotivo: {Factura.MotivoEliminacion}\nUsuario: {Factura.EliminadaPorNombre}",
                    "Factura eliminada",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Actualiza una factura con los datos del grid, solo si ya existe.
        /// </summary>
        public async void RegistrarFactura()
        {
            if (string.IsNullOrWhiteSpace(IdFactura.Text))
                return;

            List<ListProduct> listaProducto = new List<ListProduct>();

            foreach (DataGridViewRow item in dataGridView1.Rows)
            {
                if (item.Cells[0]?.Value == null || item.Cells[1]?.Value == null || item.Cells[2]?.Value == null)
                    continue;

                string nombre = item.Cells[1].Value?.ToString() ?? "0";
                string cantidadStr = item.Cells[0].Value?.ToString() ?? "0";
                string precioStr = item.Cells[2].Value?.ToString() ?? "0";

                if (!double.TryParse(cantidadStr, out double cantidad) ||
                    !double.TryParse(precioStr, out double precio))
                    continue;

                var lookupProducto = await AppServices.Product.FindByNameAsync(nombre);
                var productoActual = lookupProducto.Product ?? new Productos { Nombre = nombre };

                var productos = new ListProduct()
                {
                    Producto = productoActual,
                    Cantidad = cantidad,
                    Precio = precio
                };
                // IMPORTANTE:
                // Aquí NO se debe tocar inventario (vendido/cantidad), porque esta ventana
                // se usa para visualizar/reimprimir/editar factura existente.
                // El ajuste de stock solo debe ocurrir en el flujo de venta original.
                listaProducto.Add(productos);
            }

            // Busca la factura por ID y la actualiza
            var factura = await Factura.BuscarAsync(int.Parse(IdFactura.Text));

            if (factura != null)
            {
                factura.NombreCliente = Cliente.Text;
                var caja = await Caja.BuscarPorClaveAsync("estado", "true");
                factura.NombreEmpresa = caja?.Id ?? "";
                // factura.IdCliente = ... // si necesitas usarlo
                // factura.Fecha = ... // si necesitas actualizar la fecha
                factura.Productos = listaProducto;
                if (double.TryParse(total.Text, out double totalValue))
                    factura.Total = totalValue;
                // factura.Descuentos = ... // si usas descuentos
                // factura.Description = ... // si tienes descripción
                // factura.Direccion = ... // si tienes dirección
                // factura.MetodoDePago = ... // si necesitas actualizar método
                // factura.Paga = ... // si es necesario
                // factura.Enviar = ... // si es necesario
                // factura.tipoFactura = ... // si es necesario

                await factura.ActualizarFacturaAsync();
                MostrarEstado("Factura actualizada correctamente.");
                MessageBox.Show("Factura actualizada correctamente.", "Actualización", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MostrarEstado("No se encontró la factura para actualizar.", true);
                MessageBox.Show("No se encontró la factura para actualizar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Actualizar_Click(object sender, EventArgs e)
        {
            RegistrarFactura();
        }

        private void IdFactura_TextChanged(object sender, EventArgs e) { }

        private void button1_Click(object sender, EventArgs e)
        {
            // Genera un conduce (reporte de entrega) en PDF
            List<ListProduct> listaProducto = new List<ListProduct>();

            foreach (DataGridViewRow item in dataGridView1.Rows)
            {
                if (item.Cells[0]?.Value == null || item.Cells[1]?.Value == null || item.Cells[2]?.Value == null)
                    continue;

                string nombre = item.Cells[1].Value?.ToString() ?? "0";
                string cantidadStr = item.Cells[0].Value?.ToString() ?? "0";
                string precioStr = item.Cells[2].Value?.ToString() ?? "0";

                if (!double.TryParse(cantidadStr, out double cantidad) ||
                    !double.TryParse(precioStr, out double precio))
                    continue;

                var productos = new ListProduct()
                {
                    Producto = new Productos() { Nombre = nombre },
                    Cantidad = cantidad,
                    Precio = precio
                };

                listaProducto.Add(productos);
            }
            var n = new Reportes() { FacturaActiva = Factura, Productos = listaProducto };
            n.GenerarConducePDF();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            switch (Factura.TipoFactura)
            {
                case "Consumo":
                    Factura.GenerarFacturaAsync();
                    break;
                case "Comprobante Fiscal":
                    Factura.GenerarFacturaComprobante();
                    break;
                case "Comprobante Gubernamental":
                    Factura.GenerarFacturaGubernamental();
                    break;
                default:
                    Factura.GenerarFactura1();
                    break;
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            double totalActivo1 = 0;
            foreach (DataGridViewRow item in dataGridView1.Rows)
            {
                if (item?.Cells[0]?.Value == null || item.Cells[2]?.Value == null)
                    continue;

                if (double.TryParse(item.Cells[0].Value.ToString(), out double cantidad) &&
                    double.TryParse(item.Cells[2].Value.ToString(), out double precio))
                {
                    item.Cells[3].Value = cantidad * precio;
                    totalActivo1 += cantidad * precio;
                }
            }
            total.Text = totalActivo1.ToString("c2");
        }
    }
}
