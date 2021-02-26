using Dropbox.Api;
using Dropbox.Api.Files;
using Dropbox.Api.Stone;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Dropbox
{
    public partial class MainWindow : Window
    {
        private readonly DropboxClient cliente;
        string ruta = "/";

        public MainWindow()
        {
            InitializeComponent();


            cliente = Task.Run(async () => await Cliente()).GetAwaiter().GetResult();

            ListFolderResult lista = Task.Run(async () => await ListFolder(cliente, "")).GetAwaiter().GetResult();

            ListarName(lista);

            var tareaimagen = Task.Run(async () => await Download(cliente, "//Foto Principal")).GetAwaiter().GetResult();
            var bytesimagen = Task.Run(async () => await tareaimagen.GetContentAsByteArrayAsync()).GetAwaiter().GetResult();

            imagenMuestra.Source = Transformacionbytestoimagen(bytesimagen);


        }

        static async Task<DropboxClient> Cliente()
        {
            return new DropboxClient("// Clave");
        }

        // Lista las carpetas
        static async Task<ListFolderResult> ListFolder(DropboxClient cliente, string ruta)
        {
            return await cliente.Files.ListFolderAsync(ruta);
        }

        // Tarea Descarga ??? FALTA
        async Task<IDownloadResponse<FileMetadata>> Download(DropboxClient dbx, string ruta)
        {
            // cambiar por ruta
            return await dbx.Files.DownloadAsync(ruta);
        }

        // Actualiza ListView
        private void ListarName(ListFolderResult lista)
        {
            List<string> listaDatos = new List<string>();
            foreach (Metadata item in lista.Entries)
            {
                listaDatos.Add(item.Name);
            }
            listView.ItemsSource = null;
            listView.ItemsSource = listaDatos;
        }

        // Convierte Array bytes a ImagenSource
        private ImageSource Transformacionbytestoimagen(byte[] bytesimagen)
        {
            BitmapImage bitmapImage = new BitmapImage();
            MemoryStream ms = new MemoryStream(bytesimagen);
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = ms;
            bitmapImage.EndInit();
            return (ImageSource)bitmapImage;
        }

        private void DobleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            if (!listView.SelectedItem.ToString().EndsWith(".jpg") && !listView.SelectedItem.ToString().EndsWith(".png"))
            {
                var carpeta = listView.SelectedItem;
                ListFolderResult lista = Task.Run(async () => await ListFolder(cliente, ruta += carpeta)).GetAwaiter().GetResult();
                ListarName(lista);
            }

            else
            {
                Func<object> fichero = () => listView.SelectedItem;
                String nombreFichero = (String)this.Dispatcher.Invoke(fichero);
                var tareaimagen = Task.Run(async () => await Download(cliente, ruta + "/" + nombreFichero)).GetAwaiter().GetResult();
                var bytesimagen = Task.Run(() => tareaimagen.GetContentAsByteArrayAsync()).GetAwaiter().GetResult();
                imagenMuestra.Source = Transformacionbytestoimagen(bytesimagen);
            }
        }
    }
}
