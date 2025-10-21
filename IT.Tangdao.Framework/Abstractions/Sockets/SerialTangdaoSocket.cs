using IT.Tangdao.Framework.Abstractions.SocketMessages;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.EventArg;
using IT.Tangdao.Framework.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Sockets
{
    public class SerialTangdaoSocket : TangdaoSocketBase
    {
        private SerialPort _serialPort;
        private CancellationTokenSource _receiveCts;

        public SerialTangdaoSocket(NetMode mode, ITangdaoUri uri) : base(mode, uri)
        {
            _receiveCts = new CancellationTokenSource();
        }

        public override async Task<bool> ConnectAsync()
        {
            try
            {
                return await Task.Run(() =>
                {
                    _serialPort = new SerialPort
                    {
                        PortName = Uri.ComPort,
                        BaudRate = Uri.BaudRate,
                        Parity = Parity.None,
                        DataBits = 8,
                        StopBits = StopBits.One,
                        Handshake = Handshake.None
                    };

                    // 订阅串口事件
                    _serialPort.DataReceived += SerialPort_DataReceived;
                    _serialPort.ErrorReceived += SerialPort_ErrorReceived;

                    _serialPort.Open();
                    IsConnected = true;

                    Console.WriteLine($"串口连接成功: {Uri.ComPort}@{Uri.BaudRate}bps");

                    return true;
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
                return false;
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (_serialPort.BytesToRead > 0)
                {
                    byte[] buffer = new byte[_serialPort.BytesToRead];
                    int bytesRead = _serialPort.Read(buffer, 0, buffer.Length);
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    OnMessageReceived(message);
                }
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
            }
        }

        private void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            OnErrorOccurred(new Exception($"串口错误: {e.EventType}"));
        }

        public override async Task SendAsync(string message)
        {
            if (!IsConnected || _serialPort == null || !_serialPort.IsOpen) return;

            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                _serialPort.Write(data, 0, data.Length);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
            }
        }

        public override async Task<string> ReceiveAsync()
        {
            if (!IsConnected || _serialPort == null || !_serialPort.IsOpen)
                return string.Empty;

            try
            {
                // 串口通常是事件驱动的，这里实现一个简单的同步接收
                await Task.Delay(100); // 等待数据到达
                if (_serialPort.BytesToRead > 0)
                {
                    byte[] buffer = new byte[_serialPort.BytesToRead];
                    int bytesRead = _serialPort.Read(buffer, 0, buffer.Length);
                    return Encoding.UTF8.GetString(buffer, 0, bytesRead);
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
                return string.Empty;
            }
        }

        public override async Task DisconnectAsync()
        {
            _receiveCts?.Cancel();

            if (_serialPort != null)
            {
                _serialPort.DataReceived -= SerialPort_DataReceived;
                _serialPort.ErrorReceived -= SerialPort_ErrorReceived;

                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                }
                _serialPort.Dispose();
            }

            IsConnected = false;
            await Task.CompletedTask;
        }

        // 串口特有的方法
        public void SetSerialSettings(int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
                _serialPort.BaudRate = baudRate;
                _serialPort.Parity = parity;
                _serialPort.DataBits = dataBits;
                _serialPort.StopBits = stopBits;
                _serialPort.Open();
            }
        }
    }
}