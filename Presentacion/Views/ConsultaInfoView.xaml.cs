using System;
using System.Windows;
using ApplicationLogic.DTOs;

namespace Presentacion.Views
{
    public partial class ConsultaInfoView : Window
    {
        public ConsultaInfoView(ConsultaDTO consulta)
        {
            InitializeComponent();
            CargarDatos(consulta);
        }

        private void CargarDatos(ConsultaDTO consulta)
        {
            if (consulta == null) return;

            TxtFecha.Text = consulta.Fecha.ToString("dd/MM/yyyy HH:mm");
            TxtPrecio.Text = consulta.Precio.ToString("C");
            TxtTratamiento.Text = consulta.TratamientoNombre;
            TxtPaciente.Text = consulta.PacienteNombre;
            TxtDentista.Text = consulta.DentistaNombre;
            TxtNotas.Text = consulta.Notas;
        }

        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
