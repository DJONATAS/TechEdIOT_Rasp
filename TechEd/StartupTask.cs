using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using System.Net;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409
// https://ms-iot.github.io/content/en-US/win10/samples/PinMappingsRPi2.htm

namespace TechEd
{
    public sealed class StartupTask : IBackgroundTask
    {
        private const int liquidificador_PIN = 13;
        private const int lampada_PIN = 12;

        private GpioPin liquidificador;
        private GpioPin lampada;
        private GpioPinValue lampadaValue = GpioPinValue.Low;
        private GpioPinValue liquidificadorValue = GpioPinValue.High;

        private string liquidificadorResult;
        private string lampadaResult;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            initGPIO();

            while (true)
            {
                RequestLiquidificador();
                RequestLampada();
            }         
        }

        private void initGPIO()
        {
            var gpio = GpioController.GetDefault();            
            liquidificador = gpio.OpenPin(liquidificador_PIN);
            liquidificador.SetDriveMode(GpioPinDriveMode.Output);

            lampada = gpio.OpenPin(lampada_PIN);
            lampada.SetDriveMode(GpioPinDriveMode.Output);
        }

        private async void RequestLiquidificador()
        {
            var result = Task.Run(() => GetResponseLiquidificador()).Result;

            if (result != liquidificadorResult)
            {
                if (result.Contains("0"))
                {
                    liquidificador.Write(GpioPinValue.High);
                }
                else
                {
                    liquidificador.Write(GpioPinValue.Low);
                }
            }

        }

        private async void RequestLampada()
        {
            var result = Task.Run(() => GetResponseLampada()).Result;

            if (result != lampadaResult)
            {
                if (result.Contains("0"))
                {                    
                    lampada.Write(GpioPinValue.High);
                }
                else
                {
                    lampada.Write(GpioPinValue.Low);
                }
            }
        }

        private async Task<string> GetResponseLiquidificador()
        {
            var httpClient = new HttpClient();

            var response = await httpClient.GetAsync("http://techediot.azurewebsites.net/api/Liquidificador");
            var contents = await response.Content.ReadAsStringAsync();

            return contents;
        }

        private async Task<string> GetResponseLampada()
        {
            var httpClient = new HttpClient();

            var response = await httpClient.GetAsync("http://techediot.azurewebsites.net/api/Lampada");
            var contents = await response.Content.ReadAsStringAsync();

            return contents;
        }


        private void Mudar()
        {
            lampada.Write(GpioPinValue.High);
            lampada.Write(GpioPinValue.Low);

            liquidificador.Write(GpioPinValue.High);
            liquidificador.Write(GpioPinValue.Low);

            lampada.Write(GpioPinValue.High);
            lampada.Write(GpioPinValue.Low);

            liquidificador.Write(GpioPinValue.High);
            liquidificador.Write(GpioPinValue.Low);

        }
    }
}
