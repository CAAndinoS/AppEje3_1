using AppEje3_1.Models;
using AppEje3_1.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppEje3_1.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageList : ContentPage
    {

        public PageList()
        {
            InitializeComponent();
            CargarAlumnos();
        }

        // Manejar el evento Invoked del SwipeItem para eliminar el alumno
        private async void SwipeItem_Invoked(object sender, EventArgs e)
        {
            var alumno = (Alumnos)((SwipeItem)sender).CommandParameter;

            // Implementa la lógica para eliminar el alumno utilizando el repositorio
            PerfilRepositorio _perfilRepositorio = new PerfilRepositorio();
            await _perfilRepositorio.EliminarAlumno(alumno.id);

            // Actualizar la lista de alumnos después de eliminar
            CargarAlumnos();
        }
        private void SwipeItem_Actualizar_Invoked(object sender, EventArgs e)
        {
            var alumno = (Alumnos)((SwipeItem)sender).CommandParameter;

            // Abrir la página de edición con el alumno seleccionado como parámetro
            Navigation.PushAsync(new PageActualizar(alumno));
        }
        private async void CargarAlumnos()
        {
            // Obtener todos los alumnos desde el repositorio
            PerfilRepositorio _perfilRepositorio = new PerfilRepositorio();
            var alumnos = await _perfilRepositorio.ObtenerAlumnos();

            // Establecer los alumnos como la fuente de datos del ListView
            ListViewAlumnos.ItemsSource = alumnos;
        }

        private void ListViewAlumnos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }
    }
}