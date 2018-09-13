using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System.Net.Http;
using Acr.UserDialogs;

namespace TestCustomVision
{
    public partial class MainPage : ContentPage
    {
        private MediaFile _foto;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void ElegirClick(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            var foto = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions());

            _foto = foto;
            ImgSource.Source = FileImageSource.FromFile(foto.Path);
        }

        private async void TomarFoto(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            var foto = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions()
            {
                Directory = "clasificador",
                Name = "sourse.jpg"
            });
            _foto = foto;
            ImgSource.Source = FileImageSource.FromFile(foto.Path);
        }

        private async void AnalizarFoto(object sender, EventArgs e)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Prediction-Key", "d20c03142343439d8598d1cf03558421");
            const string endpoint = "https://southcentralus.api.cognitive.microsoft.com/customvision/v2.0/Prediction/1cd65429-17d7-4a80-a31e-a57023de206f/image?iterationId=3daf1c6d-2b6a-4548-a826-2ed598188d94";

            var contentStream = new StreamContent(_foto.GetStream());

            var response = await httpClient.PostAsync(endpoint, contentStream);

            if (!response.IsSuccessStatusCode)
            {
                UserDialogs.Instance.Toast("A ocurrido un error");
                return;
            }

            var jason = await response.Content.ReadAsStringAsync();


        }
    }
}
