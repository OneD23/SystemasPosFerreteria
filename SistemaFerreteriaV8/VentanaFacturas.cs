using SistemaFerreteriaV8.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemaFerreteriaV8
{
    public partial class VentanaFacturas : Form
    {

        private void ConfigurarVistaProfesional()
        {
            ListaDeFacturas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            ListaDeFacturas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            ListaDeFacturas.MultiSelect = false;
            ListaDeFacturas.AllowUserToAddRows = false;
            ListaDeFacturas.AllowUserToDeleteRows = false;
            ListaDeFacturas.RowHeadersVisible = false;
            ListaDeFacturas.BorderStyle = BorderStyle.None;
            ListaDeFacturas.BackgroundColor = System.Drawing.Color.FromArgb(20, 20, 20);
            ListaDeFacturas.EnableHeadersVisualStyles = false;

            ListaDeFacturas.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(35, 35, 35);
            ListaDeFacturas.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            ListaDeFacturas.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);

            ListaDeFacturas.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(18, 18, 18);
            ListaDeFacturas.DefaultCellStyle.ForeColor = System.Drawing.Color.WhiteSmoke;
            ListaDeFacturas.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            ListaDeFacturas.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            ListaDeFacturas.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10);

            ListaDeFacturas.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(28, 28, 28);

            Column2.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
            Column6.DefaultCellStyle.Format = "C2";
            Paginacion.Text = "Página 1 de 1";
        }

        private const int pageSize = 50;
        private int currentPage = 1;
        private int totalPages = 1;
        private int searchVersion = 0;
        private bool isInitializing = true;

        private ProgressBar progressBarLoading;
        private CheckBox chkMostrarEliminadas;

        public VentanaFacturas()
        {
            InitializeComponent();
            SistemaFerreteriaV8.Clases.ThemeManager.ApplyToForm(this);
            AutoScroll = true;

            progressBarLoading = new ProgressBar
            {
                Name = "progressBarLoading",
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 30,
                Visible = false,
                Width = 100,
                Height = 20,
                Left = 508,
                Top = 85
            };

            this.Controls.Add(progressBarLoading);
            this.Controls.SetChildIndex(progressBarLoading, 0);
            chkMostrarEliminadas = new CheckBox
            {
                Text = "Mostrar eliminadas",
                ForeColor = System.Drawing.Color.White,
                BackColor = System.Drawing.Color.Transparent,
                AutoSize = true,
                Left = 390,
                Top = 42
            };
            chkMostrarEliminadas.CheckedChanged += async (_, __) => await ReiniciarYRecargarAsync();
            groupBox1.Controls.Add(chkMostrarEliminadas);
            ConfigurarVistaProfesional();

            button1.Click += async (_, __) => await CambiarPaginaAsync(-1);
            button2.Click += async (_, __) => await CambiarPaginaAsync(1);
            buttonEliminarPorFecha.Click += async (_, __) => await EliminarFacturasPorFechaAsync();
            Fecha1.ValueChanged += async (_, __) => await ReiniciarYRecargarAsync();
            Fecha2.ValueChanged += async (_, __) => await ReiniciarYRecargarAsync();

            OrganizarLayoutFacturas();
            Resize += (_, __) => OrganizarLayoutFacturas();
        }

        private async void VentanaFacturas_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            Fecha1.Value = DateTime.Today.AddMonths(-1);
            Fecha2.Value = DateTime.Now;

            isInitializing = false;
            await CargarPaginaAsync();
        }

        private async Task ReiniciarYRecargarAsync()
        {
            if (isInitializing)
                return;

            currentPage = 1;
            await CargarPaginaAsync();
        }

        private async Task CambiarPaginaAsync(int delta)
        {
            var nuevaPagina = currentPage + delta;
            if (nuevaPagina < 1 || nuevaPagina > totalPages)
                return;

            currentPage = nuevaPagina;
            await CargarPaginaAsync();
        }

        private async Task CargarPaginaAsync()
        {
            progressBarLoading.Visible = true;

            try
            {
                string tipoFiltro = comboBox1.Text;
                string termino = Id.Text?.Trim();

                var resultado = await Factura.ListarFacturasPaginadasAsync(
                    Fecha1.Value,
                    Fecha2.Value,
                    currentPage,
                    pageSize,
                    tipoFiltro,
                    termino,
                    chkMostrarEliminadas?.Checked == true);

                var lista = resultado.Facturas;
                var total = resultado.Total;

                totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
                if (currentPage > totalPages)
                {
                    currentPage = totalPages;
                }

                var empleadosIds = lista
                    .Select(f => f.IdEmpleado)
                    .Where(id => !string.IsNullOrWhiteSpace(id))
                    .Distinct()
                    .ToList();

                var empleadosMap = new Dictionary<string, string>();
                foreach (var empleadoId in empleadosIds)
                {
                    var empleado = await Empleado.BuscarAsync(empleadoId);
                    empleadosMap[empleadoId] = empleado?.Nombre ?? string.Empty;
                }

                ListaDeFacturas.Rows.Clear();
                foreach (var item in lista)
                {
                    var empleadoNombre = string.IsNullOrWhiteSpace(item.IdEmpleado)
                        ? string.Empty
                        : (empleadosMap.TryGetValue(item.IdEmpleado, out var nombre) ? nombre : string.Empty);

                    ListaDeFacturas.Rows.Add(
                        item.Id,
                        item.Fecha,
                        item.NombreCliente,
                        item.TipoFactura,
                        (item.Description ?? string.Empty) + (item.Informacion ?? string.Empty),
                        empleadoNombre,
                        item.Total,
                        item.Enviar,
                        item.Paga);
                }

                CantidadFactura.Text = total.ToString();
                Paginacion.Text = $"Página {currentPage} de {totalPages}";
            }
            finally
            {
                progressBarLoading.Visible = false;
            }
        }

        private async Task EliminarFacturasPorFechaAsync()
        {
            var desde = Fecha1.Value.Date;
            var hasta = Fecha2.Value.Date;
            if (hasta < desde)
            {
                MessageBox.Show("La fecha 'Hasta' no puede ser menor que 'Desde'.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmacion = MessageBox.Show(
                $"Se eliminarán (lógicamente) las facturas entre {desde:dd/MM/yyyy} y {hasta:dd/MM/yyyy}.\n\n¿Desea continuar?",
                "Eliminar facturas por fecha",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmacion != DialogResult.Yes)
                return;

            buttonEliminarPorFecha.Enabled = false;
            progressBarLoading.Visible = true;
            try
            {
                int total = await Factura.EliminarPorRangoFechasAsync(
                    desde,
                    hasta,
                    $"Eliminación por rango de fechas ({desde:yyyy-MM-dd} a {hasta:yyyy-MM-dd}) desde VentanaFacturas");

                MessageBox.Show(
                    total > 0
                        ? $"Se eliminaron {total} factura(s) dentro del rango."
                        : "No se encontraron facturas activas para eliminar en ese rango.",
                    "Eliminar facturas por fecha",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                await ReiniciarYRecargarAsync();
            }
            finally
            {
                progressBarLoading.Visible = false;
                buttonEliminarPorFecha.Enabled = true;
            }
        }

        private void OrganizarLayoutFacturas()
        {
            const int margenLateral = 16;
            const int separacion = 12;
            const int topTitulo = 38;

            int areaAncho = Math.Max(980, ClientSize.Width - (margenLateral * 2));
            int xInicial = margenLateral;

            label1.Location = new System.Drawing.Point(Math.Max(xInicial, (ClientSize.Width / 2) - (label1.Width / 2)), topTitulo);

            int topFiltros = 118;
            label4.Location = new System.Drawing.Point(xInicial, topFiltros + 4);
            Fecha1.Width = 150;
            Fecha1.Location = new System.Drawing.Point(label4.Right + 8, topFiltros);

            label5.Location = new System.Drawing.Point(Fecha1.Right + separacion + 6, topFiltros + 4);
            Fecha2.Width = 150;
            Fecha2.Location = new System.Drawing.Point(label5.Right + 8, topFiltros);

            buttonEliminarPorFecha.Size = new System.Drawing.Size(250, 34);
            buttonEliminarPorFecha.Location = new System.Drawing.Point(Fecha2.Right + separacion + 4, topFiltros - 1);

            int filtroAncho = Math.Min(370, Math.Max(300, areaAncho / 3));
            groupBox1.Size = new System.Drawing.Size(filtroAncho, 84);

            int xGrupoIdeal = (ClientSize.Width - margenLateral - groupBox1.Width);
            int yGrupo = topFiltros - 35;
            if (buttonEliminarPorFecha.Right + separacion > xGrupoIdeal)
            {
                yGrupo = topFiltros + 44;
            }
            groupBox1.Location = new System.Drawing.Point(xGrupoIdeal, yGrupo);

            int topGrid = Math.Max(groupBox1.Bottom, buttonEliminarPorFecha.Bottom) + 14;
            int bottomZona = 120;
            ListaDeFacturas.Location = new System.Drawing.Point(xInicial, topGrid);
            ListaDeFacturas.Size = new System.Drawing.Size(areaAncho, Math.Max(250, ClientSize.Height - topGrid - bottomZona));

            int yBottom = ListaDeFacturas.Bottom + 20;
            button1.Location = new System.Drawing.Point(xInicial + (areaAncho / 6), yBottom);
            button2.Location = new System.Drawing.Point(xInicial + areaAncho - (areaAncho / 6) - button2.Width, yBottom);
            Paginacion.Location = new System.Drawing.Point((ClientSize.Width / 2) - 60, yBottom + 9);

            label2.Location = new System.Drawing.Point(xInicial + areaAncho - 220, yBottom - 42);
            CantidadFactura.Location = new System.Drawing.Point(label2.Right + 8, yBottom - 42);
        }

        private async void Id_TextChanged(object sender, EventArgs e)
        {
            if (isInitializing)
                return;

            searchVersion++;
            int versionActual = searchVersion;

            await Task.Delay(250);
            if (versionActual != searchVersion)
                return;

            currentPage = 1;
            await CargarPaginaAsync();
        }

        private async void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isInitializing)
                return;

            Id.Text = string.Empty;
            currentPage = 1;
            await CargarPaginaAsync();
        }

        private async void ListaDeFacturas_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || ListaDeFacturas[0, e.RowIndex]?.Value == null) return;
            if (!int.TryParse(ListaDeFacturas[0, e.RowIndex].Value.ToString(), out int id)) return;

            var facturaActiva = await Factura.BuscarAsync(id);

            if (facturaActiva != null)
            {
                var factura = new VentanaFactura() { Factura = facturaActiva };
                factura.Show();
            }
            else
            {
                MessageBox.Show("La factura no fue encontrada.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
