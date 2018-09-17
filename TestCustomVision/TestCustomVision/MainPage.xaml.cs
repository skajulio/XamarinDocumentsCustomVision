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
using Newtonsoft.Json;

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
            //await CrossMedia.Current.Initialize();

            //var foto = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions());

            //_foto = foto;
            //ImgSource.Source = FileImageSource.FromFile(foto.Path);

            await CrossMedia.Current.Initialize();

            var foto = await CrossMedia.Current
                .PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions());

            _foto = foto;
            ImgSource.Source = FileImageSource.FromFile(foto.Path);
        }

        private async void TomarFoto(object sender, EventArgs e)
        {
            //await CrossMedia.Current.Initialize();

            //if(!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            //{
            //    await DisplayAlert("Sin Cámara", "No hay Cámara Disponibe", "Ok");
            //    return;
            //}

            //var foto = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            //{
            //    SaveToAlbum = true,
            //    Directory = "clasificador",
            //    Name = "sourse.jpg",
            //});

            //if (foto == null)
            //    return;

            //await DisplayAlert("File Location", foto.Path, "OK");

            //_foto = foto;
            //ImgSource.Source = ImageSource.FromFile(foto.Path);

            //await CrossMedia.Current.Initialize();

            //var foto = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions()
            //{
            //    Directory = "clasificator",
            //    Name = "source.jpg"
            //});
            //_foto = foto;
            //ImgSource.Source = FileImageSource.FromFile(foto.Path);

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("Sin Cámara", "No hay Cámara Disponibe", "Ok");
                return;
            }


        }

        private async void AnalizarFoto(object sender, EventArgs e)
        {
            //var httpClient = new HttpClient();
            //httpClient.DefaultRequestHeaders.Add("Prediction-Key", "d20c03142343439d8598d1cf03558421");
            //const string endpoint = "https://southcentralus.api.cognitive.microsoft.com/customvision/v2.0/Prediction/1cd65429-17d7-4a80-a31e-a57023de206f/image?iterationId=3daf1c6d-2b6a-4548-a826-2ed598188d94";

            //var contentStream = new StreamContent(_foto.GetStream());

            //var response = await httpClient.PostAsync(endpoint, contentStream);

            //if (!response.IsSuccessStatusCode)
            //{
            //    UserDialogs.Instance.Toast("A ocurrido un error");
            //    return;
            //}

            //var jason = await response.Content.ReadAsStringAsync();

            const string endpoint = "https://southcentralus.api.cognitive.microsoft.com/customvision/v2.0/Prediction/1cd65429-17d7-4a80-a31e-a57023de206f/image?iterationId=d4f53db4-5ab1-40a2-ac50-628e39491cf7";
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Prediction-Key", "d20c03142343439d8598d1cf03558421");

            var contentStream = new StreamContent(_foto.GetStream());

            using (Acr.UserDialogs.UserDialogs.Instance.Loading("Cargando..."))
            {
                using (Acr.UserDialogs.UserDialogs.Instance.Loading("Uploading..."))
                {
                    var response = await httpClient.PostAsync(endpoint, contentStream);

                    if (!response.IsSuccessStatusCode)
                    {
                        UserDialogs.Instance.Toast("Un error ha ocurrido.");
                        return;
                    }

                    var json = await response.Content.ReadAsStringAsync();

                    var prediction = JsonConvert.DeserializeObject<PredictionResponse>(json);

                    var tag = prediction.predictions.First();

                    Resultado.Text = $"{tag.tagName} - {tag.probability:p0}";
                    Precision.Progress = tag.probability;

                }
            }


        }
    }


    public class PredictionResponse
    {
        public string id { get; set; }
        public string project { get; set; }
        public string iteration { get; set; }
        public DateTime created { get; set; }
        public Prediction[] predictions { get; set; }
    }

    public class Prediction
    {
        public float probability { get; set; }
        public string tagId { get; set; }
        public string tagName { get; set; }
    }

}
