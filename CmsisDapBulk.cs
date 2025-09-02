using Microsoft.VisualBasic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Devices.Enumeration;
using Windows.Devices.Usb;
using Windows.Storage.Streams;

internal class CmsisDapBulk
{
    UsbDevice? _UsbDevice = null;
    private UsbBulkOutPipe? _BulkOutPipe = null;
    private UsbBulkInPipe? _BulkInPipe = null;

    public UInt32 MaxPacketSize;

    private static async Task<String?> GetStringDescriptorAsync(UsbDevice device, UInt32 index, UInt32 languageId = 0x0409)
    {
        var setupPacket = new UsbSetupPacket
        {
            RequestType = new UsbControlRequestType
            {
                ControlTransferType = UsbControlTransferType.Standard,
                Direction = UsbTransferDirection.In, // device to host
                Recipient = UsbControlRecipient.Device,
            },
            Request = 0x06,         // 描述符
            Value = 0x0300 + index, // 描述符序号
            Index = languageId,     // 语言，默认英语
            Length = device.DeviceDescriptor.MaxPacketSize0 // 端点0包长
        };

        var buffer = new Windows.Storage.Streams.Buffer(device.DeviceDescriptor.MaxPacketSize0);
        var stringDescriptorBuffer = await device.SendControlInTransferAsync(setupPacket, buffer);
        var reader = DataReader.FromBuffer(stringDescriptorBuffer);

        Byte descriptorLength = reader.ReadByte();
        Byte descriptorType = reader.ReadByte();
        if ((descriptorType != 0x03) || (descriptorLength < 2))
        {
            return null;
        }

        /* 字符串 */
        Byte[] stringDescriptorData = new Byte[reader.UnconsumedBufferLength];
        reader.ReadBytes(stringDescriptorData);
        var serialNumber = Encoding.Unicode.GetString(stringDescriptorData);

        return serialNumber;
    }

    public async Task<String[]?> GetDeviceList()
    {
        List<String> dapList = new List<String>();

        /* 筛选设备 */
        UsbDeviceClass dapUsbClass = new()
        {
            ClassCode = 0xFF, // 必须为0xFF
            SubclassCode = 0x00,
            ProtocolCode = 0x00
        };
        String aqs = UsbDevice.GetDeviceClassSelector(dapUsbClass);

        /* 找到所有符合条件的设备 */
        var deviceList = await DeviceInformation.FindAllAsync(aqs, null);
        if ((deviceList == null) || (deviceList.Count == 0))
        {
            return null;
        }

        /* 逐个设备检查 */
        foreach (var deviceItem in deviceList)
        {
            /* 检查设备名是否包含"CMSIS-DAP" */
            if (!deviceItem.Name.Contains("CMSIS-DAP"))
            {
                continue;
            }

            if (deviceItem.IsEnabled == false)
            {
                continue;
            }

            UsbDevice? usbDevice = null;
            try
            {
                usbDevice = await UsbDevice.FromIdAsync(deviceItem.Id);
            }
            catch (Exception exception)
            {
                continue;
            }

            if (usbDevice == null)
            {
                continue;
            }

            /* 通过控制传输获取字符串描述符3，即序列号 */
            var serialNumberString = await GetStringDescriptorAsync(usbDevice, 3);
            if (serialNumberString == null)
            {
                usbDevice.Dispose();
                continue;
            }

            dapList.Add($"{deviceItem.Name}[{serialNumberString}]");
            usbDevice.Dispose();
        }

        if (dapList.Count == 0)
        {
            return null;
        }
        return dapList.ToArray();
    }

    public async Task<int> OpenAsync(UInt32 Vid, UInt32 Pid, String? SerialNumber = null)
    {
        /* 筛选设备 */
        String aqs;
        if (Vid * Pid == 0)
        {
            UsbDeviceClass dapUsbClass = new()
            {
                ClassCode = 0xFF, // 必须为0xFF
                SubclassCode = 0x00,
                ProtocolCode = 0x00
            };
            aqs = UsbDevice.GetDeviceClassSelector(dapUsbClass);
        }
        else
        {
            aqs = UsbDevice.GetDeviceSelector(Vid, Pid);
        }

        /* 找到所有符合条件的设备 */
        var deviceList = await DeviceInformation.FindAllAsync(aqs, null);
        if ((deviceList == null) || (deviceList.Count == 0))
        {
            return -1;
        }

        /* 逐个设备检查 */
        foreach (var deviceItem in deviceList)
        {
            /* 检查设备名是否包含"CMSIS-DAP" */
            if (!deviceItem.Name.Contains("CMSIS-DAP"))
            {
                continue;
            }

            if (deviceItem.IsEnabled == false)
            {
                continue;
            }

            UsbDevice? usbDevice = null;
            try
            {
                usbDevice = await UsbDevice.FromIdAsync(deviceItem.Id);
            }
            catch (Exception exception)
            {
                continue;
            }

            if (usbDevice == null)
            {
                continue;
            }

            /* 通过控制传输获取字符串描述符3，即序列号 */
            var serialNumberString = await GetStringDescriptorAsync(usbDevice, 3);
            if (serialNumberString == null)
            {
                continue;
            }

            /* 选择默认接口 */
            var defaultInterface = usbDevice.DefaultInterface;
            MaxPacketSize = defaultInterface.BulkOutPipes[0].EndpointDescriptor.MaxPacketSize;

            /* 必须有足够的管道 */
            if ((defaultInterface.BulkInPipes.Count == 0) || (defaultInterface.BulkOutPipes.Count == 0))
            {
                continue;
            }

            /* 需要匹配序列号 */
            if (SerialNumber != null)
            {
                if (!serialNumberString.Equals(SerialNumber))
                {
                    continue; // 序列号不匹配
                }
            }

            _UsbDevice = usbDevice;

            /* 储存读写通道 */
            _BulkOutPipe = defaultInterface.BulkOutPipes[0];
            _BulkInPipe = defaultInterface.BulkInPipes[0];

            /* 清理数据 */
            _BulkInPipe.FlushBuffer();

            //_BulkOutPipe.WriteOptions = UsbWriteOptions.AutoClearStall;
            //_BulkInPipe.ReadOptions = UsbReadOptions.AutoClearStall;

            return 0;
        }

        return -1;
    }

    public async Task<Byte[]?> TransferAsync(Byte[] OutData)
    {
        /* 包长超标 */
        if (OutData.Length > MaxPacketSize)
        {
            return null; // 下位机可能不支持自动分包
        }

        /* 管道无效 */
        if ((_BulkOutPipe == null) || (_BulkInPipe == null))
        {
            return null;
        }

        _BulkInPipe.FlushBuffer(); // 清除接收区数据

        /* 写 */
        try
        {
            await _BulkOutPipe.OutputStream.WriteAsync(OutData.AsBuffer());
        }
        catch (Exception exception)
        {
            return null;
        }

        /* 读 */
        DataReader reader = new(_BulkInPipe.InputStream);
        try
        {
            await reader.LoadAsync(MaxPacketSize);
        }
        catch (Exception exception)
        {
            reader.Dispose();
            return null;
        }

        var inData = reader.DetachBuffer().ToArray();
        reader.Dispose();
        return inData;
    }

    public void Close()
    {
        _UsbDevice?.Dispose();
        _BulkOutPipe = null;
        _BulkInPipe = null;

        MaxPacketSize = 0;
    }
}
