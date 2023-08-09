using AppEje3_1.Models;
using AppEje3_1.Service;
using Plugin.Media;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppEje3_1.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageActualizar : ContentPage
    {
        PerfilRepositorio _perfilRepositorio = new PerfilRepositorio();
        Plugin.Media.Abstractions.MediaFile photo = null;
        private Alumnos alumno;
        public PageActualizar(Alumnos alumno)
        {
            InitializeComponent();
            this.alumno = alumno;
            CargarDatosAlumno();
        }
        private void CargarDatosAlumno()
        {
            // Cargar los datos del alumno en los campos de la página
            txtnombre.Text = alumno.nombres;
            txtapellidos.Text = alumno.apellidos;
            txtsexo.Text = alumno.sexo;
            txtdireccion.Text = alumno.direccion;
            // Puedes cargar otras propiedades del alumno aquí si las tienes en la clase Alumnos

            // Cargar la foto del alumno si existe
            if (!string.IsNullOrEmpty(alumno.foto))
            {
                byte[] fotobyte = Convert.FromBase64String(alumno.foto);
                var stream = new MemoryStream(fotobyte);
                Foto.Source = ImageSource.FromStream(() => stream);
            }
        }

        private string traeImagenToBase64()
        {
            if (photo != null)
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    Stream stream = photo.GetStream();
                    stream.CopyTo(memory);
                    byte[] fotobyte = memory.ToArray();

                    byte[] imagenescalada = obtener_imagen_escalada(fotobyte, 50, 500, 500); // Ajusta los valores de ancho y alto según tus necesidades

                    string base64String = Convert.ToBase64String(imagenescalada);
                    return base64String;
                }
            }
            return null;
        }

        private byte[] obtener_imagen_escalada(byte[] imagen, int compresion, int nuevoAncho, int nuevoAlto)
        {
            using (SKBitmap originalBitmap = SKBitmap.Decode(imagen))
            {
                SKImageInfo info = new SKImageInfo(nuevoAncho, nuevoAlto);
                using (SKBitmap scaledBitmap = originalBitmap.Resize(info, SKFilterQuality.High))
                {
                    using (SKData compressedData = scaledBitmap.Encode(SKEncodedImageFormat.Jpeg, compresion))
                    {
                        return compressedData.ToArray();
                    }
                }
            }
        }

        private async void btnFoto_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            var source = await Application.Current.MainPage.DisplayActionSheet(
                "Elige Una Opcion",
                "Cancelar",
                null,
                "Galeria",
                "Camara");

            if (source == "Cancelar")
            {
                photo = null;
                return;
            }
            if (source == "Camara")
            {
                photo = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "FotosAplicacion",
                    Name = "PhotoAlbum.jpg",
                    SaveToAlbum = true
                });
            }
            else
            {
                photo = await CrossMedia.Current.PickPhotoAsync();
            }

            if (photo != null)
            {
                Foto.Source = ImageSource.FromStream(() =>
                {
                    var stream = photo.GetStream();
                    return stream;
                });
            }
        }
        private async void Buttoactualizar_Clicked(object sender, EventArgs e)
        {
            if (txtdireccion.Text.Equals(""))
            {
                await DisplayAlert("Info", "Porfavor llenar el campo direccion", "Ok");
            }
            else if (txtapellidos.Text.Equals(""))
            {
                await DisplayAlert("Info", "Porfavor llenar el campo email", "Ok");
            }
            else if (txtnombre.Text.Equals(""))
            {
                await DisplayAlert("Info", "Porfavor llenar el campo nombre", "Ok");
            }
            else if (txtsexo.Text.Equals(""))
            {
                await DisplayAlert("Info", "Porfavor llenar el campo telefono", "Ok");
            }
            else { 
                // Actualizar los datos del alumno con los valores ingresados en los campos
                alumno.nombres = txtnombre.Text;
            alumno.apellidos = txtapellidos.Text;
            alumno.sexo = txtsexo.Text;
            alumno.direccion = txtdireccion.Text;

            // Verificar si hay una foto nueva seleccionada
            string nuevaFotoBase64 = traeImagenToBase64();
            if (!string.IsNullOrEmpty(nuevaFotoBase64))
            {
                // Si hay una foto nueva, actualizar el campo 'foto' del objeto alumno
                alumno.foto = nuevaFotoBase64;
            }

            // Utilizar el método Actualizar_Alumno para actualizar el alumno en Firebase
            await _perfilRepositorio.Actualizar_Alumno(alumno);

            // Volver a la página de lista después de guardar los cambios
            Navigation.PushAsync(new PageList());
            }
        }



    }
}