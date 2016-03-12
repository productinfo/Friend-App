﻿using System;
using System.Diagnostics;
using System.Threading;
using Windows.Devices.Geolocation;
using Windows.Devices.Sms;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Friend_s.Services;
using GalaSoft.MvvmLight.Command;
using Tweetinvi;
using Tweetinvi.Core.Credentials;

namespace Friend_s.ViewModel
{
    public class SosPageViewModel : BaseViewModel
    {
        private static SmsDevice2 _device;
        public delegate void CallingInfoDelegate();
        public event CallingInfoDelegate CellInfoUpdateCompleted;
        public event CallingInfoDelegate ActivePhoneCallStateChanged;
        private CancellationTokenSource _cts = null;
        private static string _latitude;
        private static string _longitude;

        public string SosPageText { get; set; }

        public SosPageViewModel()
        {
            TimerStarterCommand = new RelayCommand(TimerStarter);
            SosCommand = new RelayCommand(SosCommandMethod);
        }

        public RelayCommand TimerStarterCommand { get; set; }
        public RelayCommand SosCommand { get; set; }

        private void TimerStarter()
        {
            SosPageText += "Timer Started \n";
            RaisePropertyChanged(()=>SosPageText);
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(20);
            timer.Tick += timer_Tick;
            //timer.Start();

            if (!Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
                return;
            var spineClass = new SpineClass();
            spineClass.InitializeCallingInfoAsync();
            Caller();
        }

        private void timer_Tick(object sender, object e)
        {
            LocationAccesser();

            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
                return;
            //TODO: Get messaging permission
            
            MessageSender();
            
        }


        private static string _phonenumber;
        private static string _phonename;


        private async void LocationAccesser()
        {
            SosPageText += "Trying to get location... \n";
            try
            {
                var accessStatus = await Geolocator.RequestAccessAsync();

                switch (accessStatus)
                {
                    case GeolocationAccessStatus.Allowed:

                        // If DesiredAccuracy or DesiredAccuracyInMeters are not set (or value is 0), DesiredAccuracy.Default is used.
                        var geolocator = new Geolocator { DesiredAccuracy = PositionAccuracy.High };
                        // Carry out the operation
                        var pos = await geolocator.GetGeopositionAsync();
                        _latitude = pos.Coordinate.Point.Position.Latitude.ToString();
                        _longitude = pos.Coordinate.Point.Position.Longitude.ToString();
                        var location = new BasicGeoposition
                        {
                            Latitude = pos.Coordinate.Point.Position.Latitude,
                            Longitude = pos.Coordinate.Point.Position.Longitude
                        };
                        SosPageText += "Location accessed... \n" + _latitude + "\n" + _longitude ;

                        break;
                    case GeolocationAccessStatus.Denied:
                        Debug.WriteLine("Access Denied!");
                        SosPageText += "Access Denied \n";
                        break;

                    case GeolocationAccessStatus.Unspecified:
                        Debug.WriteLine("Unspecified");
                        SosPageText += "Unknown error \n";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            RaisePropertyChanged(() => SosPageText);
        }

        private async void MessageSender()
        {
            if (_device == null)
            {
                try
                {
                    _device = SmsDevice2.GetDefault();
                }
                catch (Exception ex)
                {
                    //textBox.Text = ex.Message;
                    return;
                }

            }
            //if (_device == null) return;

            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey("FirstContactNumber")) return;
            _phonenumber = ApplicationData.Current.LocalSettings.Values["FirstContactNumber"] as string;
            var msg = new SmsTextMessage2
            {
                To = _phonenumber,
                Body = "I am in need of help. My coordinates are\n Latitude:" + _latitude + "Longitude \n" + _longitude
            };
            var result = await _device.SendMessageAndGetResultAsync(msg);
            SosPageText += "Sending Message.... \n";

            if (!result.IsSuccessful)
            {
                SosPageText += "Message Sending Failed \n";
                return;
            }
            var msgStr = "";
            msgStr += "Text message sent, To: " + _phonenumber;
            SosPageText += msgStr+"\n";
            RaisePropertyChanged(()=>SosPageText);
        }

        private async void Caller()
        {
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey("FirstContactNumber"))
            {
                SosPageText += "Contacts not assigned \n";
                return;
            }
            _phonenumber = ApplicationData.Current.LocalSettings.Values["FirstContactNumber"] as string;

            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey("FirstContactName"))
            {
                return;
            }
            _phonename = ApplicationData.Current.LocalSettings.Values["FirstContactName"] as string;

            if ((SpineClass.CurrentPhoneLine != null))
            {
                SosPageText += "Calling... \n";
                SpineClass.CurrentPhoneLine.Dial(_phonenumber, _phonename);
            }
            else
            {
                SosPageText += "No line found to place the call. No SIM or system not a mobile phone \n";
            }
            RaisePropertyChanged(()=>SosPageText);
        }

        private async void TwitterPoster()
        {
            SosPageText += "Checking credentials... \n";
            var vault = new PasswordVault();
            var credentialList = vault.FindAllByUserName("Twitter");
            if (credentialList.Count <= 0)
            {
                SosPageText += "Twitter not configured \n";
                return;
            }
            var credential = vault.Retrieve("Friend", "Twitter");
            SosPageText += "Credentials Retrieved \n";

            // Set up your credentials (https://apps.twitter.com)
            Auth.SetUserCredentials("CONSUMER_KEY", "CONSUMER_SECRET", "ACCESS_TOKEN", "ACCESS_TOKEN_SECRET");

            //TODO: Publish the Tweet with location on your Timeline
            Tweet.PublishTweet("I need help at"+_latitude+_longitude);

            SosPageText += "Publishing Tweet... \n";

            RaisePropertyChanged(()=>SosPageText);
        }

        private void SosCommandMethod()
        {
            LocationAccesser();
            MessageSender();
            //Caller();
            //TwitterPoster();
        }
    }
}
