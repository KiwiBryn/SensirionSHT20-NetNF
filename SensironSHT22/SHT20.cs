//---------------------------------------------------------------------------------
// Copyright (c) October 2021, devMobile Software
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
// Inspired by Arduino library https://github.com/RobTillaart/SHT2x thankyou Rob Tillart
//
//---------------------------------------------------------------------------------
namespace devMobile.IoT.nanoFramework.Sensirion
{
    using System;
    using System.Device.I2c;
    using System.Threading;

    public class Sht20 : IDisposable
    {
        public const byte DefaultI2cAddress = 0x40;
        private const byte UserRegisterRead = 0xE7;
        private const byte UserRegisterWrite = 0xE6;
        private const byte HeaterOnMask = 0x04;
        private const byte HeaterOffMask = 0xFb;
        private const byte ReadingWaitmSec = 70;
        private const byte TemperatureNoHold = 0xF3;
        private const byte HumidityNoHold = 0xF5;
        private const byte SoftReset = 0xFE;

        private I2cDevice _i2cDevice = null;

        public Sht20(I2cDevice i2cDevice)
        {
            SpanByte writeBuffer = new byte[1] { UserRegisterRead };
            SpanByte readBuffer = new byte[1] { 0 };
            _i2cDevice = i2cDevice ?? throw new ArgumentNullException(nameof(_i2cDevice));

            _i2cDevice.WriteRead(writeBuffer, readBuffer);

            if (readBuffer[0] == 0)
            {
                throw new ApplicationException("SHT20 device not found");
            }
        }


        public double Temperature()
        {
            byte[] readBuffer = new byte[3] { 0, 0, 0 };
            if (_i2cDevice == null)
            {
                throw new ArgumentNullException(nameof(_i2cDevice));
            }

            _i2cDevice.WriteByte(TemperatureNoHold);

            Thread.Sleep(ReadingWaitmSec);

            _i2cDevice.Read(readBuffer);

            ushort temperatureRaw = (ushort)(readBuffer[0] << 8);
            temperatureRaw += readBuffer[1];

            double temperature = temperatureRaw * (175.72 / 65536.0) - 46.85;

            return temperature;
        }

        public double Humidity()
        {
            byte[] readBuffer = new byte[3] { 0, 0, 0 };
            if (_i2cDevice == null)
            {
                throw new ArgumentNullException(nameof(_i2cDevice));
            }

            _i2cDevice.WriteByte(HumidityNoHold);

            Thread.Sleep(ReadingWaitmSec);

            _i2cDevice.Read(readBuffer);

            ushort humidityRaw = (ushort)(readBuffer[0] << 8);
            humidityRaw += readBuffer[1];

            double humidity = humidityRaw * (125.0 / 65536.0) - 6.0;

            return humidity;
        }

        public void Reset()
        {
            if (_i2cDevice == null)
            {
                throw new ArgumentNullException(nameof(_i2cDevice));
            }

            _i2cDevice.WriteByte(SoftReset);

            Thread.Sleep(70);
        }

        public void HeaterOn()
        {
            byte[] writeBufferRead = new byte[1] { UserRegisterRead };
            byte[] readBuffer = new byte[1] { 0 };
            if (_i2cDevice == null)
            {
                throw new ArgumentNullException(nameof(_i2cDevice));
            }

            _i2cDevice.WriteRead(writeBufferRead, readBuffer);

            byte[] writeBufferWrite = new byte[2] { UserRegisterWrite, (byte)(readBuffer[0] | HeaterOnMask) };

            _i2cDevice.Write(writeBufferWrite);
        }

        public bool IsHeaterOn()
        {
            byte[] writeBuffer = new byte[1] { UserRegisterRead };
            byte[] readBuffer = new byte[1] { 0 };
            if (_i2cDevice == null)
            {
                throw new ArgumentNullException(nameof(_i2cDevice));
            }

            _i2cDevice.WriteRead(writeBuffer, readBuffer);

            return ((readBuffer[0] & HeaterOnMask) == HeaterOnMask);
        }

        public void HeaterOff()
        {
            byte[] writeBufferRead = new byte[1] { UserRegisterRead };
            byte[] readBuffer = new byte[1] { 0 };
            if (_i2cDevice == null)
            {
                throw new ArgumentNullException(nameof(_i2cDevice));
            }

            _i2cDevice.WriteRead(writeBufferRead, readBuffer);

            byte[] writeBufferWrite = new byte[2] { UserRegisterWrite, (byte)(readBuffer[0] & HeaterOffMask) };

            _i2cDevice.Write(writeBufferWrite);
        }

        public void Dispose()
        {
            _i2cDevice?.Dispose();
            _i2cDevice = null!;
        }
    }
}

