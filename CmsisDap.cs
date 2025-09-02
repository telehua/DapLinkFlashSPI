using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.ClosedCaptioning;
using static CmsisDap;

internal class CmsisDap
{
    private CmsisDapBulk _dap;

    public UInt32 PacketSize;

    // 设备能力标志位枚举
    private enum Capabilities
    {
        SWD = (1 << 0),
        JTAG = (1 << 1),
        SWO_UART = (1 << 2),
        SWO_Manchester = (1 << 3),
        AutoCmd = (1 << 4),
        TimeStamp = (1 << 5),
        SWO_StreamingTrace = (1 << 6),
        UART_CommunicationPort = (1 << 7),
        USB_COM_Port = (1 << 8),
    }

    // 设备端口类型枚举
    public enum Port
    {
        DEFAULT = 0,    // 默认
        SWD = 1,        // SWD
        JTAG = 2,       // JTAG
    }

    // 设备命令枚举
    public enum CmdId
    {
        Info = 0x00,                // 获取设备信息
        ID_DAP_HostStatus = 0x01,   // 设定LED状态
        Connect = 0x02,             // 打开接口
        Disconnect = 0x03,          // 关闭接口
        TransferConfigure = 0x04,   // 配置传输参数
        Transfer = 0x05,            // 传输
        TransferBlock = 0x06,       // 块传输
        TransferAbort = 0x07,       // 
        WriteABORT = 0x08,          // 写停止寄存器
        Delay = 0x09,               // 延时
        ResetTarget = 0x0A,         // 复位目标芯片
        SWJ_Pins = 0x10,            // 软件IO设置
        SWJ_Clock = 0x11,           // 时钟速度
        SWJ_Sequence = 0x12,        // 序列
        SWD_Configure = 0x13,       // SWD配置
        SWD_Sequence = 0x1D,        // SWD序列
        JTAG_Sequence = 0x14,       // JTAG序列
        JTAG_Configure = 0x15,      // JTAG配置
        JTAG_IDCODE = 0x16,         // JTAG读IDCODE
        SWO_Transport = 0x17,       // 
        SWO_Mode = 0x18,            // 
        SWO_Baudrate = 0x19,        // 
        SWO_Control = 0x1A,         // 
        SWO_Status = 0x1B,          // 
        SWO_ExtendedStatus = 0x1E,  // 
        SWO_Data = 0x1C,            // 
        UART_Transport = 0x1F,      // 
        UART_Configure = 0x20,      // 
        UART_Control = 0x22,        // 
        UART_Status = 0x23,         // 
        UART_Transfer = 0x21,       //
    }

    // Info命令子命令枚举
    public enum InfoId
    {
        Vendor = 0x01,              // 厂家
        Product = 0x02,             // 产品
        SerialNumber = 0x03,        // 序列号
        FwVersion = 0x04,           // 固件版本
        DeviceVendor = 0x05,        // 设备厂家
        DeviceName = 0x06,          // 设备名
        BoardVendor = 0x07,         // 板厂家
        BoardName = 0x08,           // 板名
        ProductFwVersion = 0x09,    // 产品版本
        Capabilities = 0xF0,        // 可用功能
        TimestampClock = 0xF1,      // 时间戳计数频率
        UART_RX_BufferSize = 0xFB,  // UART接收缓冲区大小
        UART_TX_BufferSize = 0xFC,  // UART发送缓冲区大小
        SWO_BufferSize = 0xFD,      // SWO缓冲区大小
        PacketCount = 0xFE,         // 包缓存个数
        PacketSize = 0xFF,          // 最大包长
    }

    // 构造函数
    public CmsisDap()
    {
        _dap = new CmsisDapBulk();
    }

    // 获取可用列表
    public async Task<String[]?> GetDeviceList()
    {
        return await _dap.GetDeviceList();
    }

    // 打开设备
    public async Task<int> OpenAsync(UInt32 Vid, UInt32 Pid, String? SerialNumber = null)
    {
        var r = await _dap.OpenAsync(Vid, Pid, SerialNumber);
        if (r != 0)
        {
            return -1;
        }

        // 读取基本数据
        PacketSize = _dap.MaxPacketSize;

        return r;
    }

    // 获取设备能力
    public async Task<Byte> GetCapabilities()
    {
        Byte[] req = new Byte[] {
            (Byte)CmdId.Info,
            (Byte)InfoId.Capabilities
        };

        var res = await _dap.TransferAsync(req);

        if ((res == null) || (res[0] != 0x00) || (res[1] != 2))
        {
            return 0;
        }

        return res[2];
    }

    // 获取设备能力
    public async Task<Byte> GetPacketCount()
    {
        Byte[] req = new Byte[] {
            (Byte)CmdId.Info,
            (Byte)InfoId.PacketCount
        };

        var res = await _dap.TransferAsync(req);

        if ((res == null) || (res[0] != 0x00) || (res[1] != 2))
        {
            return 0;
        }

        return res[2];
    }

    public async Task<Int32> Speed(UInt32 freq)
    {
        Byte[] req = new Byte[] {
            (Byte)CmdId.SWJ_Clock,
            (Byte)freq,
            (Byte)(freq >> 8),
            (Byte)(freq >> 16),
            (Byte)(freq >> 24),
        };

        var res = await _dap.TransferAsync(req);
        if (res == null || res.Length != 2 || res[0] != req[0] || res[1] != 0)
        {
            return -1;
        }
        return 0;
    }

    // 打开端口
    public async Task<Int32> Connect(Port port)
    {
        Byte[] req = new Byte[] {
            (Byte)CmdId.Connect,
            (Byte)port
        };

        var res = await _dap.TransferAsync(req);
        if (res == null || res.Length != 2 || res[0] != req[0] || res[1] != req[0])
        {
            return -1;
        }
        return 0;
    }

    // 打开端口
    public async Task<Int32> Disconnect(Port port)
    {
        Byte[] req = new Byte[] {
            (Byte)CmdId.Disconnect,
        };

        var res = await _dap.TransferAsync(req);
        if (res == null || res.Length != 2 || res[0] != req[0] || res[1] != 0)
        {
            return -1;
        }
        return 0;
    }

    // 单数据包传输
    public async Task<Byte[]?> Transfer(Byte[] req)
    {
        return await _dap.TransferAsync(req);
    }

    // 关闭设备
    public void Close()
    {
        _dap.Close();
    }
}
