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
//---------------------------------------------------------------------------------
namespace devMobile.IoT.nanoFramework
{
    using System;
    using System.Device.I2c;
    using System.Threading;

    public class Program
    {
        public static void Main()
        {
            I2cConnectionSettings i2cConnectionSettings = new(1, 0x40);

            // i2cDevice.Dispose
            I2cDevice i2cDevice = I2cDevice.Create(i2cConnectionSettings);

            while (true)
            {
                byte[] readBuffer = new byte[3] { 0, 0, 0 };

                // First temperature
                i2cDevice.WriteByte(0xF3);

                //Thread.Sleep(50); // no go -46.8
                //Thread.Sleep(60);
                Thread.Sleep(70);
                //Thread.Sleep(90);
                //Thread.Sleep(110);

                i2cDevice.Read(readBuffer);

                ushort temperatureRaw = (ushort)(readBuffer[0] << 8);
                temperatureRaw += readBuffer[1];

                //Debug.WriteLine($"Raw {temperatureRaw}");

                double temperature = temperatureRaw * (175.72 / 65536.0) - 46.85;

                // Then read the Humidity
                i2cDevice.WriteByte(0xF5);

                // No delay 46%
                //Thread.Sleep(50); // 52%
                //Thread.Sleep(60); // 53%
                Thread.Sleep(70); // 54%
                //Thread.Sleep(90); // 54%
                //Thread.Sleep(110); // 54% 

                i2cDevice.Read(readBuffer);

                ushort humidityRaw = (ushort)(readBuffer[0] << 8);
                humidityRaw += readBuffer[1];

                //Debug.WriteLine($"Raw {humidityRaw}");

                double humidity = humidityRaw * (125.0 / 65536.0) - 6.0;

                Console.WriteLine($"{DateTime.UtcNow:HH:mm:ss} Temperature:{temperature:F1}°C Humidity:{humidity:F0}%");

                Thread.Sleep(1000);
            }
        }
    }
}
