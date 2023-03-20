//---------------------------------------------------------------------------------
// Copyright (c) March 2023, devMobile Software
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// nanoff --platform ESP32 --serialport COM7 --update
// nanoff --target ST_STM32F769I_DISCOVERY --update 
//
//---------------------------------------------------------------------------------
namespace devMobile.IoT.nanoFramework.Sensirion
{
    using System;
    using System.Device.I2c;
    using System.Threading;

    //using nanoFramework.Hardware.Esp32;

    class Program
    {
        static void Main(string[] args)
        {
            const int busId = 1;

            //Configuration.SetPinFunction(Gpio.IO04, DeviceFunction.I2C1_DATA);
            //Configuration.SetPinFunction(Gpio.IO05, DeviceFunction.I2C1_CLOCK);

            I2cConnectionSettings i2cConnectionSettings = new(busId, Sht20.DefaultI2cAddress);

            using I2cDevice i2cDevice = I2cDevice.Create(i2cConnectionSettings);
            {
                using (Sht20 sht20 = new Sht20(i2cDevice))
                {
                    sht20.Reset();

                    while (true)
                    {
                        double temperature = sht20.Temperature();
                        double humidity = sht20.Humidity();
#if HEATER_ON_OFF
					    sht20.HeaterOn();
					    Console.WriteLine($"{DateTime.Now:HH:mm:ss} HeaterOn:{sht20.IsHeaterOn()}");
#endif
                        Console.WriteLine($"{DateTime.UtcNow:HH:mm:ss} Temperature:{temperature:F1}°C Humidity:{humidity:F0}% HeaterOn:{sht20.IsHeaterOn()}");
#if HEATER_ON_OFF
					    sht20.HeaterOff();
					    Console.WriteLine($"{DateTime.Now:HH:mm:ss} HeaterOn:{sht20.IsHeaterOn()}");
#endif
                        Thread.Sleep(1000);
                    }
                }
            }
        }
    }
}