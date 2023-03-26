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
// nanoff --target ST_STM32F769I_DISCOVERY --update 
// nanoff --platform ESP32 --serialport COM7 --update
//
//---------------------------------------------------------------------------------
//#define ST_STM32F769I_DISCOVERY 
#define  SPARKFUN_ESP32_THING_PLUS
namespace devMobile.IoT.Device.Sht20
{
    using System;
    using System.Device.I2c;
    using System.Threading;

#if SPARKFUN_ESP32_THING_PLUS
    using nanoFramework.Hardware.Esp32;
#endif

    public class Program
    {
        public static void Main()
        {
#if SPARKFUN_ESP32_THING_PLUS
            Configuration.SetPinFunction(Gpio.IO23, DeviceFunction.I2C1_DATA);
            Configuration.SetPinFunction(Gpio.IO22, DeviceFunction.I2C1_CLOCK);
#endif
            I2cConnectionSettings i2cConnectionSettings = new(1, 0x40);

            // i2cDevice.Dispose
            I2cDevice i2cDevice = I2cDevice.Create(i2cConnectionSettings);

            while (true)
            {
                /*
                byte[] writeBuffer = new byte[3] { 0xF3, 0, 0 };
                byte[] readBuffer = new byte[3] { 0, 0, 0 };

                i2cDevice.WriteRead(writeBuffer, readBuffer);
                */

                byte[] readBuffer = new byte[3] { 0, 0, 0 };
 
                // First temperature
                //i2cDevice.WriteByte(0xF3);
                if (i2cDevice.WriteByte(0xF3).Status != I2cTransferStatus.FullTransfer)
                {
                    Console.WriteLine($"Temperature write failed");
                }

                //Thread.Sleep(50); // no go -46.8
                //Thread.Sleep(60);
                Thread.Sleep(70);
                //Thread.Sleep(90);
                //Thread.Sleep(110);

                // i2cDevice.Read(readBuffer)
                if ( i2cDevice.Read(readBuffer).Status != I2cTransferStatus.FullTransfer )
                {
                    Console.WriteLine($"Temperature read failed");
                }

                ushort temperatureRaw = (ushort)(readBuffer[0] << 8);
                temperatureRaw += readBuffer[1];

                //Console.WriteLine($"Raw {temperatureRaw}");

                double temperature = temperatureRaw * (175.72 / 65536.0) - 46.85;


                // Then read the Humidity
                i2cDevice.WriteByte(0xF5);

                //Thread.Sleep(50);  
                //Thread.Sleep(60);  
                Thread.Sleep(70);
                //Thread.Sleep(90);  
                //Thread.Sleep(110);   

                if (i2cDevice.Read(readBuffer).Status != I2cTransferStatus.FullTransfer)
                {
                    Console.WriteLine($"Temperature read failed");
                }

                ushort humidityRaw = (ushort)(readBuffer[0] << 8);
                humidityRaw += readBuffer[1];

                //Console.WriteLine($"Raw {humidityRaw}");

                double humidity = humidityRaw * (125.0 / 65536.0) - 6.0;

                //Console.WriteLine($"{DateTime.UtcNow:HH:mm:ss} Temperature:{temperature:F1}°C");
                //Console.WriteLine($"{DateTime.UtcNow:HH:mm:ss} Humidity:{humidity:F0}%");
                Console.WriteLine($"{DateTime.UtcNow:HH:mm:ss} Temperature:{temperature:F1}°C Humidity:{humidity:F0}%");

                Thread.Sleep(1000);
            }
        }
    }
}
