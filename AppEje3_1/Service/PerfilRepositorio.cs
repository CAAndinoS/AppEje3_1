using AppEje3_1.Models;
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppEje3_1.Service
{
    public class PerfilRepositorio
    {

        static string webapikey = "AIzaSyCST3t0SHGxmFe5fas2jJJYdEuABRJ_aOY";
        
        public static FirebaseClient firebaseperfil = new FirebaseClient("https://appeje3punto1-default-rtdb.firebaseio.com/");


        public async Task Guardar_Alumno(Alumnos c)
        {
            // Obtener el valor actual del contador autoincremental desde Firebase
            int nuevoId = await ObtenerNuevoId();

            // Asignar el id autoincremental al objeto Alumnos
            c.id = nuevoId;

            // Guardar el alumno en Firebase usando el ID como representante
            await firebaseperfil
                .Child("alumnos")
                .Child(c.id.ToString()) // Utilizamos el ID como el representante
                .PutAsync(c);

            // Incrementar el contador en Firebase después de guardar el nuevo alumno
            await IncrementarContador();
        }

        public async Task<int> ObtenerNuevoId()
        {
            try
            {
                // Obtener el valor actual del contador desde Firebase
                var contadorData = await firebaseperfil
                    .Child("contador")
                    .OnceSingleAsync<int>();

                int nuevoId = contadorData; // No es necesario el operador ??

                return nuevoId;
            }
            catch (Firebase.Database.FirebaseException ex)
            {
                // Si ocurre una excepción es porque el nodo "contador" no existe
                // Creamos el nodo "contador" con un valor inicial de 1
                await firebaseperfil
                    .Child("contador")
                    .PutAsync(1);

                // Devolvemos el valor inicial del id (1)
                return 1;
            }
        }

        private async Task IncrementarContador()
        {
            // Incrementar el contador en 1 en Firebase
            var contadorData = await firebaseperfil
                .Child("contador")
                .OnceSingleAsync<int>();

            int nuevoContador = contadorData + 1;

            await firebaseperfil
                .Child("contador")
                .PutAsync(nuevoContador);
        }

        // Método para obtener todos los alumnos almacenados en Firebase
        public async Task<List<Alumnos>> ObtenerAlumnos()
        {
            var alumnos = await firebaseperfil
                .Child("alumnos")
                .OnceAsync<Alumnos>();

            return alumnos.Select(alumno => alumno.Object).ToList();
        }

        // Método para eliminar un alumno
        public async Task EliminarAlumno(int  alumnoId)
        {
            await firebaseperfil
              .Child("alumnos")
              .Child(alumnoId.ToString())
              .DeleteAsync();
        }

        public async Task Actualizar_Alumno(Alumnos c)
        {
            // Utilizar el método PutAsync para actualizar el alumno en Firebase
            await firebaseperfil
                .Child("alumnos")
                .Child(c.id.ToString())
                .PutAsync(c);
        }
    }
}
