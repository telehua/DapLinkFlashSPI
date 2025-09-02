using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace DapLinkFlashSPI
{
    internal class DapSpiFlash
    {
        private readonly Byte[] BitReverseTable = new Byte[]
        {
            0x00, 0x80, 0x40, 0xC0, 0x20, 0xA0, 0x60, 0xE0, 0x10, 0x90, 0x50, 0xD0, 0x30, 0xB0, 0x70, 0xF0,
            0x08, 0x88, 0x48, 0xC8, 0x28, 0xA8, 0x68, 0xE8, 0x18, 0x98, 0x58, 0xD8, 0x38, 0xB8, 0x78, 0xF8,
            0x04, 0x84, 0x44, 0xC4, 0x24, 0xA4, 0x64, 0xE4, 0x14, 0x94, 0x54, 0xD4, 0x34, 0xB4, 0x74, 0xF4,
            0x0C, 0x8C, 0x4C, 0xCC, 0x2C, 0xAC, 0x6C, 0xEC, 0x1C, 0x9C, 0x5C, 0xDC, 0x3C, 0xBC, 0x7C, 0xFC,
            0x02, 0x82, 0x42, 0xC2, 0x22, 0xA2, 0x62, 0xE2, 0x12, 0x92, 0x52, 0xD2, 0x32, 0xB2, 0x72, 0xF2,
            0x0A, 0x8A, 0x4A, 0xCA, 0x2A, 0xAA, 0x6A, 0xEA, 0x1A, 0x9A, 0x5A, 0xDA, 0x3A, 0xBA, 0x7A, 0xFA,
            0x06, 0x86, 0x46, 0xC6, 0x26, 0xA6, 0x66, 0xE6, 0x16, 0x96, 0x56, 0xD6, 0x36, 0xB6, 0x76, 0xF6,
            0x0E, 0x8E, 0x4E, 0xCE, 0x2E, 0xAE, 0x6E, 0xEE, 0x1E, 0x9E, 0x5E, 0xDE, 0x3E, 0xBE, 0x7E, 0xFE,
            0x01, 0x81, 0x41, 0xC1, 0x21, 0xA1, 0x61, 0xE1, 0x11, 0x91, 0x51, 0xD1, 0x31, 0xB1, 0x71, 0xF1,
            0x09, 0x89, 0x49, 0xC9, 0x29, 0xA9, 0x69, 0xE9, 0x19, 0x99, 0x59, 0xD9, 0x39, 0xB9, 0x79, 0xF9,
            0x05, 0x85, 0x45, 0xC5, 0x25, 0xA5, 0x65, 0xE5, 0x15, 0x95, 0x55, 0xD5, 0x35, 0xB5, 0x75, 0xF5,
            0x0D, 0x8D, 0x4D, 0xCD, 0x2D, 0xAD, 0x6D, 0xED, 0x1D, 0x9D, 0x5D, 0xDD, 0x3D, 0xBD, 0x7D, 0xFD,
            0x03, 0x83, 0x43, 0xC3, 0x23, 0xA3, 0x63, 0xE3, 0x13, 0x93, 0x53, 0xD3, 0x33, 0xB3, 0x73, 0xF3,
            0x0B, 0x8B, 0x4B, 0xCB, 0x2B, 0xAB, 0x6B, 0xEB, 0x1B, 0x9B, 0x5B, 0xDB, 0x3B, 0xBB, 0x7B, 0xFB,
            0x07, 0x87, 0x47, 0xC7, 0x27, 0xA7, 0x67, 0xE7, 0x17, 0x97, 0x57, 0xD7, 0x37, 0xB7, 0x77, 0xF7,
            0x0F, 0x8F, 0x4F, 0xCF, 0x2F, 0xAF, 0x6F, 0xEF, 0x1F, 0x9F, 0x5F, 0xDF, 0x3F, 0xBF, 0x7F, 0xFF
        };

        public UInt32 ChipSize;
        public UInt32 PageSize;
        public UInt32 AddrSize;
        public UInt32 AddrExtCmd;
        public UInt32 ReadCmd;
        public UInt32 PageProgramCmd;
        public UInt32 WriteEnableCmd;
        public UInt32 WriteDisableCmd;
        public UInt32 ChipEraseCmd;
        public UInt32 ReadStatusCmd;
        public UInt32 BusyBit;

        public async Task<Byte?> ReadByte(CmsisDap dap, UInt32 addr)
        {
            Byte[] req =
            [
                (Byte)CmsisDap.CmdId.JTAG_Sequence,
                0x04,
                // 拉高CS
                (8 & 0x3F) | (1 << 6) | (0 << 7),
                BitReverseTable[0xFF],
                // 命令及地址
                ((8 + 24) & 0x3F) | (0 << 6) | (0 << 7),
                BitReverseTable[ReadCmd],
                BitReverseTable[(Byte)(addr >> 16)],
                BitReverseTable[(Byte)(addr >> 8)],
                BitReverseTable[(Byte)addr],
                // 数据传输
                (8 & 0x3F) | (0 << 6) | (1 << 7),
                BitReverseTable[0xFF],           
                // 拉高CS
                (8 & 0x3F) | (1 << 6) | (0 << 7),
                BitReverseTable[0xFF],
            ];

            var res = await dap.Transfer(req);
            if (res == null ||
                res.Length != (2 + 5) ||
                res[0] != req[0] ||
                res[1] != 0)
            {
                return null;
            }

            return BitReverseTable[res.Last()];// 最后一字节
        }

        public async Task<Byte[]?> ReadMass(CmsisDap dap, UInt32 addr, UInt32 size)
        {
            List<Byte> data = [];

            UInt32 remainingData = size;
            while (remainingData > 0)
            {
                List<Byte> req = [];

                // 统计字节数和需要的JTAG序列数
                UInt32 nBytesThisOp = ((dap.PacketSize - (6 + 1 + AddrSize)) / 9) * 8;
                if (nBytesThisOp > remainingData)
                {
                    nBytesThisOp = remainingData;
                }
                remainingData -= nBytesThisOp;
                UInt32 nSeqThisOp = (nBytesThisOp + 7) / 8;// 需要的JTAG序列数

                // 命令头
                req.Add((Byte)CmsisDap.CmdId.JTAG_Sequence);
                req.Add((Byte)(nSeqThisOp + 3));

                // 拉高CS
                req.Add((8 & 0x3F) | (1 << 6) | (0 << 7));// TMS=1, no TDO
                req.Add(0xFF);

                // FLASH命令及地址
                req.Add((Byte)(((8 + AddrSize * 8) & 0x3F) | (0 << 6) | (0 << 7)));// TMS=0, no TDO
                req.Add(BitReverseTable[ReadCmd]);
                if (AddrSize > 3)
                {
                    req.Add(BitReverseTable[(Byte)(addr >> 24)]);
                }
                req.Add(BitReverseTable[(Byte)(addr >> 16)]);
                req.Add(BitReverseTable[(Byte)(addr >> 8)]);
                req.Add(BitReverseTable[(Byte)addr]);

                // 数据
                var nByteCount = nBytesThisOp;
                var nSeqCount = nSeqThisOp;
                while (nSeqCount > 0)
                {
                    if (nByteCount < 8)
                    {
                        req.Add((Byte)(((nByteCount * 8) & 0x3F) | (0 << 6) | (1 << 7)));// TMS=0, get TDO
                        Byte[] b = new Byte[nByteCount];
                        for (UInt32 i = 0; i < b.Length; i++)
                        {
                            b[i] = 0xFF;
                        }
                        req.AddRange(b);
                        nByteCount = 0;
                    }
                    else
                    {
                        req.Add((Byte)(((8 * 8) & 0x3F) | (0 << 6) | (1 << 7)));// TMS=0, get TDO
                        Byte[] b = new Byte[8];
                        for (UInt32 i = 0; i < b.Length; i++)
                        {
                            b[i] = 0xFF;
                        }
                        req.AddRange(b);
                        nByteCount -= 8;
                    }
                    nSeqCount--;
                }

                // 拉高CS
                req.Add((8 & 0x3F) | (1 << 6) | (0 << 7));// TMS=1, no TDO
                req.Add(0xFF);

                // 一次USB传输
                var res = await dap.Transfer(req.ToArray());
                if (res == null || res.Length != (2 + nBytesThisOp) || res[0] != req[0] || res[1] != 0)
                {
                    // 返回数据不符合预期
                    return null;
                }

                // 截取数据部分
                Byte[] tdoData = res.Skip(2).ToArray();
                for (UInt32 i = 0; i < tdoData.Length; i++)
                {
                    // 逐字节翻转，LSB->MSB
                    tdoData[i] = BitReverseTable[tdoData[i]];
                }

                // 添加到数据缓冲区
                data.AddRange(tdoData);
                addr += nBytesThisOp;
            }

            // 大小不一致
            if (data.Count != size)
            {
                return null;
            }
            return data.ToArray();
        }

        public async Task<Int32> WriteByte(CmsisDap dap, UInt32 addr, Byte data)
        {
            List<Byte> req = [];

            req.Add((Byte)CmsisDap.CmdId.JTAG_Sequence);
            req.Add(0x08);

            // 拉高CS
            req.Add((8 & 0x3F) | (1 << 6) | (0 << 7));// TMS=1, no TDO
            req.Add(BitReverseTable[0xFF]);

            // 写使能
            req.Add((8 & 0x3F) | (0 << 6) | (0 << 7));// TMS=0, no TDO
            req.Add(BitReverseTable[WriteEnableCmd]);

            // 拉高CS
            req.Add((8 & 0x3F) | (1 << 6) | (0 << 7));// TMS=1, no TDO
            req.Add(BitReverseTable[0xFF]);

            // FLASH命令及地址
            req.Add(((8 + 24) & 0x3F) | (0 << 6) | (0 << 7));// TMS=0, no TDO
            req.Add(BitReverseTable[PageProgramCmd]);
            if (AddrSize > 3)
            {
                req.Add(BitReverseTable[(Byte)(addr >> 24)]);
            }
            req.Add(BitReverseTable[(Byte)(addr >> 16)]);
            req.Add(BitReverseTable[(Byte)(addr >> 8)]);
            req.Add(BitReverseTable[(Byte)addr]);

            // 数据传输
            req.Add((8 & 0x3F) | (0 << 6) | (0 << 7));// TMS=0, no TDO
            req.Add(BitReverseTable[data]);

            // 拉高CS
            req.Add((8 & 0x3F) | (1 << 6) | (0 << 7));// TMS=1, no TDO
            req.Add(BitReverseTable[0xFF]);

            // 关写使能
            req.Add((8 & 0x3F) | (0 << 6) | (0 << 7));// TMS=0, no TDO
            req.Add(BitReverseTable[WriteDisableCmd]);

            // 拉高CS
            req.Add((8 & 0x3F) | (1 << 6) | (0 << 7));// TMS=1, no TDO
            req.Add(BitReverseTable[0xFF]);

            var res = await dap.Transfer(req.ToArray());
            if (res == null || res.Length != (2) || res[0] != req[0] || res[1] != 0)
            {
                return -1;
            }
            return 0;
        }

        public async Task<Int32> WriteMass(CmsisDap dap, UInt32 addr, Byte[] data)
        {
            // 允许写入
            await EnableWrite(dap);

            UInt32 writeData = 0;
            while (writeData < (UInt32)data.Length)
            {
                // 对齐整页地址
                UInt32 opSizeThisPage = PageSize - (addr % PageSize);
                if (opSizeThisPage > ((UInt32)data.Length - writeData))
                {
                    // 数据量小于单次操作长度
                    opSizeThisPage = ((UInt32)data.Length - writeData);
                }

                // 单次操作，可能需要多次USB传输
                while (opSizeThisPage > 0)
                {
                    List<Byte> req = [];

                    // 统计字节数和需要的JTAG序列数
                    var nBytesOnePacket = ((dap.PacketSize - (6 + 1 + AddrSize)) / 9) * 8;
                    if (nBytesOnePacket > opSizeThisPage)
                    {
                        nBytesOnePacket = opSizeThisPage;
                    }
                    opSizeThisPage -= nBytesOnePacket;
                    var nSeqOnePacket = (nBytesOnePacket + 7) / 8;// 需要的JTAG序列数

                    // 命令头
                    req.Add((Byte)CmsisDap.CmdId.JTAG_Sequence);
                    req.Add((Byte)(nSeqOnePacket + 3));// 数据 + CS[2]

                    // 拉高CS
                    req.Add((8 & 0x3F) | (1 << 6) | (0 << 7));// TMS=1, no TDO
                    req.Add(0xFF);

                    // FLASH命令及地址
                    req.Add((Byte)(((8 + AddrSize * 8) & 0x3F) | (0 << 6) | (0 << 7)));// TMS=0, no TDO
                    req.Add(BitReverseTable[PageProgramCmd]);
                    if (AddrSize > 3)
                    {
                        req.Add(BitReverseTable[(Byte)(addr >> 24)]);
                    }
                    req.Add(BitReverseTable[(Byte)((addr) >> 16)]);
                    req.Add(BitReverseTable[(Byte)((addr) >> 8)]);
                    req.Add(BitReverseTable[(Byte)(addr)]);

                    // 基地址
                    addr += nBytesOnePacket;

                    while (nSeqOnePacket > 0)
                    {
                        if (nBytesOnePacket < 8)
                        {
                            req.Add((Byte)(((nBytesOnePacket * 8) & 0x3F) | (0 << 6) | (0 << 7)));// TMS=0, no TDO
                            for (UInt32 i = 0; i < nBytesOnePacket; i++)
                            {
                                req.Add(BitReverseTable[data[writeData]]);
                                writeData++;
                            }
                            nBytesOnePacket = 0;
                        }
                        else
                        {
                            req.Add((Byte)(((8 * 8) & 0x3F) | (0 << 6) | (0 << 7)));// TMS=0, no TDO
                            for (UInt32 i = 0; i < 8; i++)
                            {
                                req.Add(BitReverseTable[data[writeData]]);
                                writeData++;
                            }
                            nBytesOnePacket -= 8;
                        }

                        nSeqOnePacket--;
                    }

                    // 拉高CS
                    req.Add((8 & 0x3F) | (1 << 6) | (0 << 7));// TMS=1, no TDO
                    req.Add(0xFF);

                    // 一次USB传输
                    var res = await dap.Transfer(req.ToArray());
                    if (res == null || res.Length != (2) || res[0] != req[0] || res[1] != 0)
                    {
                        // 返回数据不符合预期
                        return -1;
                    }

                    // 等待FLASH操作结束
                    Int32 timeOutCount = 0;
                    while (true)
                    {
                        var sr = await ReadSR(dap);
                        if ((sr != null) && ((sr & (1 << (Byte)BusyBit)) == 0))
                        {
                            break;
                        }

                        //Thread.Sleep(10);
                        timeOutCount++;

                        if (timeOutCount > 10000)
                        {
                            break;
                        }
                    }
                }
            }

            // 不允许写入
            await DisableWrite(dap);

            return 0;
        }

        public async Task<Int32> EnableWrite(CmsisDap dap)
        {
            Byte[] req =
            [
                (Byte)CmsisDap.CmdId.JTAG_Sequence,
                0x03,
                // 拉高CS
                (8 & 0x3F) | (1 << 6) | (0 << 7),
                BitReverseTable[0xFF],
                // 命令
                ((8) & 0x3F) | (0 << 6) | (0 << 7),
                BitReverseTable[WriteEnableCmd],        
                // 拉高CS
                (8 & 0x3F) | (1 << 6) | (0 << 7),
                BitReverseTable[0xFF],
            ];

            var res = await dap.Transfer(req);
            if (res == null ||
                res.Length != (2) ||
                res[0] != req[0] ||
                res[1] != 0)
            {
                return -1;
            }
            return 0;
        }

        public async Task<Int32> DisableWrite(CmsisDap dap)
        {
            Byte[] req =
            [
                (Byte)CmsisDap.CmdId.JTAG_Sequence,
                0x03,
                // 拉高CS
                (8 & 0x3F) | (1 << 6) | (0 << 7),
                BitReverseTable[0xFF],
                // 命令
                ((8) & 0x3F) | (0 << 6) | (0 << 7),
                BitReverseTable[WriteDisableCmd],        
                // 拉高CS
                (8 & 0x3F) | (1 << 6) | (0 << 7),
                BitReverseTable[0xFF],
            ];

            var res = await dap.Transfer(req);
            if (res == null ||
                res.Length != (2) ||
                res[0] != req[0] ||
                res[1] != 0)
            {
                return -1;
            }
            return 0;
        }

        public async Task<Byte?> ReadSR(CmsisDap dap)
        {
            Byte[] req =
            [
                (Byte)CmsisDap.CmdId.JTAG_Sequence,
                0x04,
                // 拉高CS
                (8 & 0x3F) | (1 << 6) | (0 << 7),
                BitReverseTable[0xFF],
                // 命令
                ((8) & 0x3F) | (0 << 6) | (0 << 7),
                BitReverseTable[ReadStatusCmd],
                // 数据
                ((8) & 0x3F) | (0 << 6) | (1 << 7),
                BitReverseTable[0xFF],
                // 拉高CS
                (8 & 0x3F) | (1 << 6) | (0 << 7),
                BitReverseTable[0xFF],
            ];

            var res = await dap.Transfer(req);
            if (res == null ||
                res.Length != (2 + 1) ||
                res[0] != req[0] ||
                res[1] != 0)
            {
                return null;
            }
            Byte sr = BitReverseTable[res.Last()];

            return sr;
        }

        public async Task<Int32?> EraseChip(CmsisDap dap)
        {
            // 允许写入
            await EnableWrite(dap);

            Byte[] req =
            [
                (Byte)CmsisDap.CmdId.JTAG_Sequence,
                0x03,
                // 拉高CS
                (8 & 0x3F) | (1 << 6) | (0 << 7),
                BitReverseTable[0xFF],
                // 命令
                ((8) & 0x3F) | (0 << 6) | (0 << 7),
                BitReverseTable[ChipEraseCmd],
                // 拉高CS
                (8 & 0x3F) | (1 << 6) | (0 << 7),
                BitReverseTable[0xFF],
            ];

            var res = await dap.Transfer(req);
            if (res == null ||
                res.Length != (2) ||
                res[0] != req[0] ||
                res[1] != 0)
            {
                return -1;
            }

            // 等待FLASH操作结束
            Int32 timeOutCount = 0;
            while (true)
            {
                var sr = await ReadSR(dap);
                if ((sr != null) && ((sr & (1 << (Byte)BusyBit)) == 0))
                {
                    break;
                }

                Thread.Sleep(100);
                timeOutCount++;

                if (timeOutCount > 50000)
                {
                    return -2;
                }
            }

            // 不允许写入
            await DisableWrite(dap);

            return 0;
        }

        public async Task<Int32?> EnableExtAddr(CmsisDap dap)
        {
            Byte[] req =
            [
                (Byte)CmsisDap.CmdId.JTAG_Sequence,
                0x03,
                // 拉高CS
                (8 & 0x3F) | (1 << 6) | (0 << 7),
                BitReverseTable[0xFF],
                // 命令
                ((8) & 0x3F) | (0 << 6) | (0 << 7),
                BitReverseTable[AddrExtCmd],
                // 拉高CS
                (8 & 0x3F) | (1 << 6) | (0 << 7),
                BitReverseTable[0xFF],
            ];

            var res = await dap.Transfer(req);
            if (res == null ||
                res.Length != (2) ||
                res[0] != req[0] ||
                res[1] != 0)
            {
                return -1;
            }
            return 0;
        }
    }
}
