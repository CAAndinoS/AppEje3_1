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
    public partial class PageAlumno : ContentPage
    {
        PerfilRepositorio _perfilRepositorio = new PerfilRepositorio();
        Plugin.Media.Abstractions.MediaFile photo = null;
        public PageAlumno()
        {
            InitializeComponent();
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

        public async void UpdateMethod()
        {
            Alumnos alumno = new Alumnos
            {
                nombres = txtnombre.Text,
                apellidos = txtapellidos.Text,
                sexo = txtsexo.Text,
                direccion = txtdireccion.Text,
                foto = traeImagenToBase64()

            };

            // Guardar el alumno en Firebase
            await _perfilRepositorio.Guardar_Alumno(alumno);

            // Mostrar mensaje de éxito
            await DisplayAlert("Info", "Alumno guardado correctamente", "Ok");

            // Limpia los campos después de guardar
            txtnombre.Text = "";
            txtapellidos.Text = "";
            txtsexo.Text = "";
            txtdireccion.Text = "";
            Foto.Source = null;
            
        }

        private async void Buttoguardar_Clicked(object sender, EventArgs e)
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
            else if (traeImagenToBase64() == null)
            {
                await DisplayAlert("Info", "Porfavor tomar la fotografia", "Ok");
            }
            else
            {
                UpdateMethod();
            }
        }

        private async void Buttolista_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PageList());
        }
    }
}