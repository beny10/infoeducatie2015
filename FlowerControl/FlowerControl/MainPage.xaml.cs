using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using Windows.ApplicationModel.Background;
using Windows.UI.Popups;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace FlowerControl
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page,INotifyPropertyChanged
    {
        private int _temperature;
        private int _humidity;
        private int _lumina;
        private int _pragUdare;
        public int PragUdare
        {
            get
            {
                return _pragUdare;
            }
            set
            {
                _pragUdare = value;
                OnPropertyChanged("PragUdare");
            }
        }
        public int Lumina
        {
            get
            {
                return _lumina;
            }
            set
            {
                _lumina = value;
                OnPropertyChanged("Lumina");
            }
        }
        public int Humidity
        {
            get
            {
                return _humidity;
            }
            set
            {
                _humidity = value;
                OnPropertyChanged("Humidity");
            }
        }
        public int Temperature
        {
            get
            {
                return _temperature;
            }
            set
            {
                _temperature = value;
                OnPropertyChanged("Temperature");
            }
        }
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.DataContext = this;
            RefreshData();
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        public void  OnPropertyChanged(string name)
        {
            var handler=PropertyChanged;
            if(handler!=null)
            {
                handler(this,new PropertyChangedEventArgs(name));
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //
        }
        private async void GetTemperature()
        {
            int temperature = 0;
            try
            {
                HttpClient http = new System.Net.Http.HttpClient();
                TimeSpan span = new TimeSpan(0, 0, 0,0,100);
                http.Timeout = span;
                var s = await http.GetStringAsync("http://192.168.0.115/?cmd=temperature");
                temperature = Convert.ToInt32(s.ToString());
                Temperature = temperature;
            }
            catch
            {
                //
            }
        }
        private async void GetHumidity()
        {
            int humidity = 0;
            try
            {
                HttpClient http = new System.Net.Http.HttpClient();
                TimeSpan span = new TimeSpan(0, 0, 0, 0, 100);
                http.Timeout = span;
                var s = await http.GetStringAsync("http://192.168.0.115/?cmd=humidity");
                humidity = Convert.ToInt32(s.ToString());
                Humidity = humidity;
            }
            catch
            {
                //
            }
        }
        private async void GetPragUdare()
        {
            int prag = 0;
            try
            {
                HttpClient http = new System.Net.Http.HttpClient();
                TimeSpan span = new TimeSpan(0, 0, 0, 0, 100);
                http.Timeout = span;
                var s = await http.GetStringAsync("http://192.168.0.115/?cmd=prag-udare");
                prag = Convert.ToInt32(s.ToString());
                PragUdare = prag;
            }
            catch
            {
                //
            }
        }
        private async void MsgBOx(string message)
        {
            try
            {
                var dialog = new MessageDialog(message);
                dialog.ShowAsync();
            }
            catch
            {
                //
            }
        }
        private async void Uda()
        {
            try
            {
                MsgBOx("Pornire udare");
                HttpClient http = new System.Net.Http.HttpClient();
                TimeSpan span = new TimeSpan(0, 0, 0, 0, 1);
                http.Timeout = span;
                var s = await http.GetStringAsync("http://192.168.0.115/?cmd=uda");
            }
            catch (Exception ee)
            {
                MsgBOx(ee.Message);
            }
        }
        private async void PragUp()
        {
            try
            {
                HttpClient http = new System.Net.Http.HttpClient();
                TimeSpan span = new TimeSpan(0, 0, 0, 0, 100);
                http.Timeout = span;
                var s = await http.GetStringAsync("http://192.168.0.115/?cmd=up-prag");
            }
            catch (Exception ee)
            {
                MsgBOx(ee.Message);
            }
        }
        private async void PragDown()
        {
            try
            {
                HttpClient http = new System.Net.Http.HttpClient();
                TimeSpan span = new TimeSpan(0, 0, 0, 0, 1);
                http.Timeout = span;
                var s = await http.GetStringAsync("http://192.168.0.115/?cmd=down-prag");
            }
            catch (Exception ee)
            {
                MsgBOx(ee.Message);
            }
        }
        private async void StopUda()
        {
            try
            {
                MsgBOx("Oprire udare");
                HttpClient http = new System.Net.Http.HttpClient();
                TimeSpan span=new TimeSpan(0,0,5);
                http.Timeout = span;
                var s = await http.GetStringAsync("http://192.168.0.115/?cmd=stop-uda");
            }
            catch
            {
                //
            }
        }
        private async void GetLumina()
        {
            int lumina = 0;
            try
            {
                HttpClient http = new System.Net.Http.HttpClient();
                TimeSpan span = new TimeSpan(0, 0, 0,0,100);
                http.Timeout = span;
                var s = await http.GetStringAsync("http://192.168.0.115/?cmd=lumina");
                lumina = Convert.ToInt32(s.ToString());
                Lumina = lumina;
            }
            catch
            {
                //
            }
        }
        private void RefreshData()
        {
            GetTemperature();
            GetHumidity();
            GetLumina();
            GetPragUdare();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RefreshData();
            
            const string name = "MyExampleTrigger";

            if (BackgroundTaskRegistration.AllTasks.Any(task => task.Value.Name == name))
            {
                // One register it once
                return;
            }

            var builder = new BackgroundTaskBuilder();
            var trigger = new SystemTrigger(SystemTriggerType.TimeZoneChange, false);

            builder.Name = name;
            builder.TaskEntryPoint = typeof(Bg).FullName;
            builder.SetTrigger(trigger);

            var registration = builder.Register();
            registration.Completed += registration_Completed;
        }

        void registration_Completed(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            var toastTemplate = ToastTemplateType.ToastImageAndText01;

            var toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

            var toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode("apa"));
            /*var toastImageElements = toastXml.GetElementsByTagName("image");
            toastImageElements[0].AppendChild(toastXml.CreateTextNode("flowerIcon.png"));*/

            var toast = new ToastNotification(toastXml);

            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Uda();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            StopUda();
        }

        private void pragUp_Click(object sender, RoutedEventArgs e)
        {
            PragUp();
        }

        private void pragDown_Click(object sender, RoutedEventArgs e)
        {
            PragDown();
        }

    }
}
