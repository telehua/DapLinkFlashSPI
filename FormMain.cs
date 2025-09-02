using System.Security.Cryptography;
using System.Windows.Forms;
using System.Text.Json;
using System.Threading.Tasks;

namespace DapLinkFlashSPI
{
    public partial class FormMain : Form
    {
        FlashTargetList? flashTargetList = null;
        FlashTargetConfig? flashTargetConfig = null;

        Byte[]? fileTempBuff = null;

        public FormMain()
        {
            InitializeComponent();
        }

        // 目标配置项
        public class FlashTargetConfig
        {
            public String Name { get; set; }
            public String Manufacturer { get; set; }
            public UInt32 ChipSize { get; set; }
            public UInt32 PageSize { get; set; }
            public UInt32 AddrSize { get; set; }
            public UInt32 AddrExtCmd { get; set; }
            public UInt32 ReadCmd { get; set; }
            public UInt32 PageProgramCmd { get; set; }
            public UInt32 WriteEnableCmd { get; set; }
            public UInt32 WriteDisableCmd { get; set; }
            public UInt32 ChipEraseCmd { get; set; }
            public UInt32 ReadStatusCmd { get; set; }
            public UInt32 BusyBit { get; set; }
        }

        // 目标配置列表
        public class FlashTargetList
        {
            public IList<FlashTargetConfig>? Target { get; set; }
        }

        // 页面加载事件
        private void FormMain_Load(object sender, EventArgs e)
        {
            // 关闭所有按钮
            groupBoxOperation.Enabled = false;
            groupBoxTargetSelect.Enabled = false;
            groupDeviceSelect.Enabled = false;

            // 默认速度选择
            comboBoxDeviceSpeed.SelectedIndex = 4;

            // 加载配置文件
            String configFile = "ConfigDapLinkFlashSPI.json";
            if (!File.Exists(configFile))
            {
                toolStripStatusLabelMain.Text = "错误";
                textBoxLogView.AppendText("找不到配置文件\r\n");
                return;
            }

            textBoxLogView.AppendText($"找到配置文件\r\n");
            var cfg = File.ReadAllText(configFile);
            // 解析JSON文件
            try
            {
                flashTargetList = JsonSerializer.Deserialize<FlashTargetList>(cfg);
            }
            catch (Exception ex)
            {
                toolStripStatusLabelMain.Text = "错误";
                textBoxLogView.AppendText($"配置文件解析失败\r\n");
                return;
            }

            if (flashTargetList == null || flashTargetList.Target == null)
            {
                toolStripStatusLabelMain.Text = "错误";
                textBoxLogView.AppendText($"配置文件解析失败\r\n");
                return;
            }

            // 将可选芯片写入选择列表
            comboBoxTargetNameSelect.Items.Clear();
            foreach (var item in flashTargetList.Target)
            {
                if (item.Name == null)
                {
                    textBoxLogView.AppendText($"配置文件解析失败\r\n");
                    return;
                }
                comboBoxTargetNameSelect.Items.Add(item.Name);
            }

            toolStripStatusLabelMain.Text = "就绪";
            textBoxLogView.AppendText($"配置文件解析成功\r\n");

            // FLASH型号选择
            comboBoxTargetNameSelect.SelectedIndex = 0;

            // 打开所有按钮
            groupBoxOperation.Enabled = true;
            groupBoxTargetSelect.Enabled = true;
            groupDeviceSelect.Enabled = true;
        }

        // 读取按钮点击事件
        private async void ButtonDump_Click(object sender, EventArgs e)
        {
            if (comboBoxDeviceSelect.SelectedIndex < 0)
            {
                textBoxLogView.AppendText($"请选择设备\r\n");
                return;
            }

            var nameAndSerialNumber = comboBoxDeviceSelect.Items[comboBoxDeviceSelect.SelectedIndex];
            String? s = nameAndSerialNumber?.ToString();

            if (s == null)
            {
                textBoxLogView.AppendText("请刷新列表\r\n");
                return;
            }
            String[] stringList = s.Split('[', ']');
            if (stringList == null || stringList.Length < 2)
            {
                textBoxLogView.AppendText("请刷新列表\r\n");
                return;
            }
            String deviceDame = stringList[0];
            String serialNumber = stringList[1];
            textBoxLogView.AppendText($"正在使用{deviceDame}\r\n");

            CmsisDap dap = new CmsisDap();
            DapSpiFlash flash = new DapSpiFlash();

            Int32 result = await dap.OpenAsync(0, 0, serialNumber);
            if (result != 0)
            {
                textBoxLogView.AppendText("设备打开失败\r\n");
                return;
            }

            if (flashTargetConfig == null)
            {
                textBoxLogView.AppendText("参数错误\r\n");
                return;
            }

            // 打开JTAG接口
            await dap.Connect(CmsisDap.Port.JTAG);
            await dap.Speed(10 * 1000000);

            // 关闭所有按钮
            groupBoxOperation.Enabled = false;
            groupBoxTargetSelect.Enabled = false;
            groupDeviceSelect.Enabled = false;

            flash.ChipSize = flashTargetConfig.ChipSize;
            flash.PageSize = flashTargetConfig.PageSize;
            flash.AddrSize = flashTargetConfig.AddrSize;
            flash.AddrExtCmd = flashTargetConfig.AddrExtCmd;
            flash.ReadCmd = flashTargetConfig.ReadCmd;
            flash.PageProgramCmd = flashTargetConfig.PageProgramCmd;
            flash.WriteEnableCmd = flashTargetConfig.WriteEnableCmd;
            flash.WriteDisableCmd = flashTargetConfig.WriteDisableCmd;
            flash.ChipEraseCmd = flashTargetConfig.ChipEraseCmd;
            flash.ReadStatusCmd = flashTargetConfig.ReadStatusCmd;
            flash.BusyBit = flashTargetConfig.BusyBit;

            if (flash.AddrSize > 3)
            {
                await flash.EnableExtAddr(dap);
            }

            toolStripStatusLabelMain.Text = "运行";
            textBoxLogView.AppendText($"读取中......\r\n");
            toolStripProgressBarMain.Value = 0;

            Byte[] buffer = new Byte[flashTargetConfig.ChipSize];
            fileTempBuff = buffer;
            //for (UInt32 i = 0; i < fileTempBuff.Length; i++)
            //{
            //    fileTempBuff[i] = 0xFF;
            //}

            // 读取数据
            Boolean readError = false;
            for (UInt32 i = 0; i < flashTargetConfig.ChipSize; i += flashTargetConfig.PageSize)
            {
                // 进度条
                toolStripProgressBarMain.Value = (Int32)(((float)i / flashTargetConfig.ChipSize) * 100);

                var d = await flash.ReadMass(dap, i, flashTargetConfig.PageSize);
                if (d == null)
                {
                    readError = true;
                    break;
                }
                d.CopyTo(fileTempBuff, i);
            }

            if (readError)
            {
                textBoxLogView.AppendText($"读取失败\r\n");
            }
            else
            {
                textBoxLogView.AppendText($"读取成功\r\n");
            }

            toolStripStatusLabelMain.Text = "就绪";

            toolStripProgressBarMain.Value = 100;
            dap.Close();

            // 打开所有按钮
            groupBoxOperation.Enabled = true;
            groupBoxTargetSelect.Enabled = true;
            groupDeviceSelect.Enabled = true;
        }

        // 打开文件点击事件
        private void ToolStripMenuItemFileOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new()
            {
                Multiselect = false,
                Filter = "Binary Image|*.bin"
            };

            if (ofd.ShowDialog() != DialogResult.OK)
            {
                textBoxLogView.AppendText($"取消打开文件\r\n");
                return;
            }

            textBoxLogView.AppendText($"打开文件{ofd.FileName}\r\n");

            var fileData = File.ReadAllBytes(ofd.FileName);
            if (fileData == null)
            {
                textBoxLogView.AppendText($"读取文件失败\r\n");
                return;
            }

            textBoxLogView.AppendText($"从文件读取 0x{fileData.Length:X} Bytes\r\n");
            fileTempBuff = fileData;

            ofd.Dispose();
        }

        // 保存文件点击事件
        private void ToolStripMenuItemFileSave_Click(object sender, EventArgs e)
        {
            if (fileTempBuff == null)
            {
                textBoxLogView.AppendText($"无数据，保存失败\r\n");
                return;
            }

            SaveFileDialog sfd = new()
            {
                Filter = "Binary|*.bin"
            };

            if (sfd.ShowDialog() != DialogResult.OK)
            {
                textBoxLogView.AppendText($"取消保存文件\r\n");
                sfd.Dispose();
                return;
            }

            var f = File.Create(sfd.FileName, fileTempBuff.Length);
            textBoxLogView.AppendText($"保存到文件{sfd.FileName}\r\n");
            f.Write(fileTempBuff);
            f.Close();
            textBoxLogView.AppendText($"写入文件 0x{fileTempBuff.Length:X} Bytes\r\n");
            sfd.Dispose();
        }

        // 帮助点击事件
        private void ToolStripMenuItemHelpAbout_Click(object sender, EventArgs e)
        {
            FormAbout formAbout = new();
            formAbout.ShowDialog();
            formAbout.Dispose();
        }

        // 芯片切换事件
        private void ComboBoxTargetNameSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (flashTargetList == null || flashTargetList.Target == null)
            {
                return;
            }
            // 切换目标FLASH配置
            textBoxTargetManufacturer.Text = flashTargetList.Target[
                comboBoxTargetNameSelect.SelectedIndex].Manufacturer;
            flashTargetConfig = flashTargetList.Target[
                comboBoxTargetNameSelect.SelectedIndex];
        }

        private async void ButtonDeviceFind_Click(object sender, EventArgs e)
        {
            CmsisDap dap = new CmsisDap();
            var list = await dap.GetDeviceList();
            dap.Close();

            comboBoxDeviceSelect.Items.Clear();

            if (list != null)
            {
                foreach (var item in list)
                {
                    comboBoxDeviceSelect.Items.Add(item);
                }

                textBoxLogView.AppendText($"搜索到{list.Length}个设备\r\n");
                comboBoxDeviceSelect.SelectedIndex = 0;
            }
            else
            {
                textBoxLogView.AppendText($"无可用设备\r\n");
                comboBoxDeviceSelect.SelectedIndex = -1;
                comboBoxDeviceSelect.Text = String.Empty;
            }
        }

        private async void buttonProgram_Click(object sender, EventArgs e)
        {
            if (fileTempBuff == null || fileTempBuff.Length == 0)
            {
                textBoxLogView.AppendText($"无数据可用，请打开文件\r\n");
                return;
            }

            if (comboBoxDeviceSelect.SelectedIndex < 0)
            {
                textBoxLogView.AppendText($"请选择设备\r\n");
                return;
            }

            var nameAndSerialNumber = comboBoxDeviceSelect.Items[comboBoxDeviceSelect.SelectedIndex];
            String? s = nameAndSerialNumber?.ToString();

            if (s == null)
            {
                textBoxLogView.AppendText("请刷新列表\r\n");
                return;
            }
            String[] stringList = s.Split('[', ']');
            if (stringList == null || stringList.Length < 2)
            {
                textBoxLogView.AppendText("请刷新列表\r\n");
                return;
            }
            String deviceDame = stringList[0];
            String serialNumber = stringList[1];
            textBoxLogView.AppendText($"正在使用{deviceDame}\r\n");

            CmsisDap dap = new CmsisDap();
            DapSpiFlash flash = new DapSpiFlash();

            Int32 result = await dap.OpenAsync(0, 0, serialNumber);
            if (result != 0)
            {
                textBoxLogView.AppendText("设备打开失败\r\n");
                return;
            }

            if (flashTargetConfig == null)
            {
                textBoxLogView.AppendText("参数错误\r\n");
                return;
            }

            // 打开JTAG接口
            await dap.Connect(CmsisDap.Port.JTAG);
            await dap.Speed(10 * 1000000);

            // 关闭所有按钮
            groupBoxOperation.Enabled = false;
            groupBoxTargetSelect.Enabled = false;
            groupDeviceSelect.Enabled = false;

            flash.ChipSize = flashTargetConfig.ChipSize;
            flash.PageSize = flashTargetConfig.PageSize;
            flash.AddrSize = flashTargetConfig.AddrSize;
            flash.AddrExtCmd = flashTargetConfig.AddrExtCmd;
            flash.ReadCmd = flashTargetConfig.ReadCmd;
            flash.PageProgramCmd = flashTargetConfig.PageProgramCmd;
            flash.WriteEnableCmd = flashTargetConfig.WriteEnableCmd;
            flash.WriteDisableCmd = flashTargetConfig.WriteDisableCmd;
            flash.ChipEraseCmd = flashTargetConfig.ChipEraseCmd;
            flash.ReadStatusCmd = flashTargetConfig.ReadStatusCmd;
            flash.BusyBit = flashTargetConfig.BusyBit;

            if (flash.AddrSize > 3)
            {
                await flash.EnableExtAddr(dap);
            }

            toolStripStatusLabelMain.Text = "运行";
            textBoxLogView.AppendText($"写入中......\r\n");
            toolStripProgressBarMain.Value = 0;

            // 写入数据
            Boolean readError = false;
            UInt32 dataSize = (fileTempBuff.Length < flashTargetConfig.ChipSize) ? (UInt32)fileTempBuff.Length : flashTargetConfig.ChipSize;
            for (UInt32 i = 0; i < dataSize; i += flashTargetConfig.PageSize)
            {
                // 进度条
                toolStripProgressBarMain.Value = (Int32)(((float)i / dataSize) * 100);

                UInt32 opSize = ((dataSize - i) < flashTargetConfig.PageSize) ? (dataSize - i) : flashTargetConfig.PageSize;
                var d = await flash.WriteMass(dap, i, fileTempBuff.Skip((Int32)i).Take((Int32)opSize).ToArray());
                if (d < 0)
                {
                    readError = true;
                    break;
                }
            }

            if (readError)
            {
                textBoxLogView.AppendText($"写入失败\r\n");
            }
            else
            {
                textBoxLogView.AppendText($"写入成功，0x{dataSize:X} Bytes\r\n");
            }
            toolStripStatusLabelMain.Text = "就绪";

            toolStripProgressBarMain.Value = 100;
            dap.Close();

            // 打开所有按钮
            groupBoxOperation.Enabled = true;
            groupBoxTargetSelect.Enabled = true;
            groupDeviceSelect.Enabled = true;
        }

        private async void buttonErase_Click(object sender, EventArgs e)
        {
            if (comboBoxDeviceSelect.SelectedIndex < 0)
            {
                textBoxLogView.AppendText($"请选择设备\r\n");
                return;
            }

            var nameAndSerialNumber = comboBoxDeviceSelect.Items[comboBoxDeviceSelect.SelectedIndex];
            String? s = nameAndSerialNumber?.ToString();

            if (s == null)
            {
                textBoxLogView.AppendText("请刷新列表\r\n");
                return;
            }
            String[] stringList = s.Split('[', ']');
            if (stringList == null || stringList.Length < 2)
            {
                textBoxLogView.AppendText("请刷新列表\r\n");
                return;
            }
            String deviceDame = stringList[0];
            String serialNumber = stringList[1];
            textBoxLogView.AppendText($"正在使用{deviceDame}\r\n");

            CmsisDap dap = new CmsisDap();
            DapSpiFlash flash = new DapSpiFlash();

            Int32 result = await dap.OpenAsync(0, 0, serialNumber);
            if (result != 0)
            {
                textBoxLogView.AppendText("设备打开失败\r\n");
                return;
            }

            if (flashTargetConfig == null)
            {
                textBoxLogView.AppendText("参数错误\r\n");
                return;
            }

            // 打开JTAG接口
            await dap.Connect(CmsisDap.Port.JTAG);
            await dap.Speed(10 * 1000000);

            // 关闭所有按钮
            groupBoxOperation.Enabled = false;
            groupBoxTargetSelect.Enabled = false;
            groupDeviceSelect.Enabled = false;

            flash.ChipSize = flashTargetConfig.ChipSize;
            flash.PageSize = flashTargetConfig.PageSize;
            flash.AddrSize = flashTargetConfig.AddrSize;
            flash.AddrExtCmd = flashTargetConfig.AddrExtCmd;
            flash.ReadCmd = flashTargetConfig.ReadCmd;
            flash.PageProgramCmd = flashTargetConfig.PageProgramCmd;
            flash.WriteEnableCmd = flashTargetConfig.WriteEnableCmd;
            flash.WriteDisableCmd = flashTargetConfig.WriteDisableCmd;
            flash.ChipEraseCmd = flashTargetConfig.ChipEraseCmd;
            flash.ReadStatusCmd = flashTargetConfig.ReadStatusCmd;
            flash.BusyBit = flashTargetConfig.BusyBit;

            if (flash.AddrSize > 3)
            {
                await flash.EnableExtAddr(dap);
            }

            toolStripStatusLabelMain.Text = "运行";
            textBoxLogView.AppendText($"全片擦除中......\r\n");
            toolStripProgressBarMain.Value = 0;

            await flash.EraseChip(dap);

            toolStripProgressBarMain.Value = 100;
            dap.Close();

            toolStripStatusLabelMain.Text = "就绪";
            textBoxLogView.AppendText($"擦除成功\r\n");

            // 打开所有按钮
            groupBoxOperation.Enabled = true;
            groupBoxTargetSelect.Enabled = true;
            groupDeviceSelect.Enabled = true;
        }

        private async void buttonCheckEmpty_Click(object sender, EventArgs e)
        {
            if (comboBoxDeviceSelect.SelectedIndex < 0)
            {
                textBoxLogView.AppendText($"请选择设备\r\n");
                return;
            }

            var nameAndSerialNumber = comboBoxDeviceSelect.Items[comboBoxDeviceSelect.SelectedIndex];
            String? s = nameAndSerialNumber?.ToString();

            if (s == null)
            {
                textBoxLogView.AppendText("请刷新列表\r\n");
                return;
            }
            String[] stringList = s.Split('[', ']');
            if (stringList == null || stringList.Length < 2)
            {
                textBoxLogView.AppendText("请刷新列表\r\n");
                return;
            }
            String deviceDame = stringList[0];
            String serialNumber = stringList[1];
            textBoxLogView.AppendText($"正在使用{deviceDame}\r\n");

            CmsisDap dap = new CmsisDap();
            DapSpiFlash flash = new DapSpiFlash();

            Int32 result = await dap.OpenAsync(0, 0, serialNumber);
            if (result != 0)
            {
                textBoxLogView.AppendText("设备打开失败\r\n");
                return;
            }

            if (flashTargetConfig == null)
            {
                textBoxLogView.AppendText("参数错误\r\n");
                return;
            }

            // 打开JTAG接口
            await dap.Connect(CmsisDap.Port.JTAG);
            await dap.Speed(10 * 1000000);

            // 关闭所有按钮
            groupBoxOperation.Enabled = false;
            groupBoxTargetSelect.Enabled = false;
            groupDeviceSelect.Enabled = false;

            flash.ChipSize = flashTargetConfig.ChipSize;
            flash.PageSize = flashTargetConfig.PageSize;
            flash.AddrSize = flashTargetConfig.AddrSize;
            flash.AddrExtCmd = flashTargetConfig.AddrExtCmd;
            flash.ReadCmd = flashTargetConfig.ReadCmd;
            flash.PageProgramCmd = flashTargetConfig.PageProgramCmd;
            flash.WriteEnableCmd = flashTargetConfig.WriteEnableCmd;
            flash.WriteDisableCmd = flashTargetConfig.WriteDisableCmd;
            flash.ChipEraseCmd = flashTargetConfig.ChipEraseCmd;
            flash.ReadStatusCmd = flashTargetConfig.ReadStatusCmd;
            flash.BusyBit = flashTargetConfig.BusyBit;

            if (flash.AddrSize > 3)
            {
                await flash.EnableExtAddr(dap);
            }

            toolStripStatusLabelMain.Text = "运行";
            textBoxLogView.AppendText($"读取中......\r\n");
            toolStripProgressBarMain.Value = 0;

            // 读取数据
            Boolean readError = false;
            for (UInt32 i = 0; i < flashTargetConfig.ChipSize; i += flashTargetConfig.PageSize)
            {
                // 进度条
                toolStripProgressBarMain.Value = (Int32)(((float)i / flashTargetConfig.ChipSize) * 100);

                var d = await flash.ReadMass(dap, i, flashTargetConfig.PageSize);
                if (d == null)
                {
                    readError = true;
                }

                for (UInt32 j = 0; j < d.Length; j++)
                {
                    if (d[j] != 0xFF)
                    {
                        textBoxLogView.AppendText($"检查错误，地址: 0x{(i + j):X}\r\n");
                        readError = true;
                        break;
                    }
                }

                if (readError)
                {
                    break;
                }
            }

            if (readError)
            {
                textBoxLogView.AppendText($"检查失败\r\n");
            }
            else
            {
                textBoxLogView.AppendText($"检查成功\r\n");
            }

            toolStripStatusLabelMain.Text = "就绪";

            toolStripProgressBarMain.Value = 100;
            dap.Close();

            // 打开所有按钮
            groupBoxOperation.Enabled = true;
            groupBoxTargetSelect.Enabled = true;
            groupDeviceSelect.Enabled = true;
        }

        private async void buttonMatch_Click(object sender, EventArgs e)
        {
            if (fileTempBuff == null || fileTempBuff.Length == 0)
            {
                textBoxLogView.AppendText($"无数据可用\r\n");
                return;
            }

            if (comboBoxDeviceSelect.SelectedIndex < 0)
            {
                textBoxLogView.AppendText($"请选择设备\r\n");
                return;
            }

            var nameAndSerialNumber = comboBoxDeviceSelect.Items[comboBoxDeviceSelect.SelectedIndex];
            String? s = nameAndSerialNumber?.ToString();

            if (s == null)
            {
                textBoxLogView.AppendText("请刷新列表\r\n");
                return;
            }
            String[] stringList = s.Split('[', ']');
            if (stringList == null || stringList.Length < 2)
            {
                textBoxLogView.AppendText("请刷新列表\r\n");
                return;
            }
            String deviceDame = stringList[0];
            String serialNumber = stringList[1];
            textBoxLogView.AppendText($"正在使用{deviceDame}\r\n");

            CmsisDap dap = new CmsisDap();
            DapSpiFlash flash = new DapSpiFlash();

            Int32 result = await dap.OpenAsync(0, 0, serialNumber);
            if (result != 0)
            {
                textBoxLogView.AppendText("设备打开失败\r\n");
                return;
            }

            if (flashTargetConfig == null)
            {
                textBoxLogView.AppendText("参数错误\r\n");
                return;
            }

            // 打开JTAG接口
            await dap.Connect(CmsisDap.Port.JTAG);
            await dap.Speed(10 * 1000000);

            // 关闭所有按钮
            groupBoxOperation.Enabled = false;
            groupBoxTargetSelect.Enabled = false;
            groupDeviceSelect.Enabled = false;

            flash.ChipSize = flashTargetConfig.ChipSize;
            flash.PageSize = flashTargetConfig.PageSize;
            flash.AddrSize = flashTargetConfig.AddrSize;
            flash.AddrExtCmd = flashTargetConfig.AddrExtCmd;
            flash.ReadCmd = flashTargetConfig.ReadCmd;
            flash.PageProgramCmd = flashTargetConfig.PageProgramCmd;
            flash.WriteEnableCmd = flashTargetConfig.WriteEnableCmd;
            flash.WriteDisableCmd = flashTargetConfig.WriteDisableCmd;
            flash.ChipEraseCmd = flashTargetConfig.ChipEraseCmd;
            flash.ReadStatusCmd = flashTargetConfig.ReadStatusCmd;
            flash.BusyBit = flashTargetConfig.BusyBit;

            if (flash.AddrSize > 3)
            {
                await flash.EnableExtAddr(dap);
            }

            toolStripStatusLabelMain.Text = "运行";
            textBoxLogView.AppendText($"读取中......\r\n");
            toolStripProgressBarMain.Value = 0;

            // 读取数据
            Boolean readError = false;
            UInt32 dataSize = (fileTempBuff.Length < flashTargetConfig.ChipSize) ? (UInt32)fileTempBuff.Length : flashTargetConfig.ChipSize;
            textBoxLogView.AppendText($"对比区域：0x{dataSize:X} Bytes\r\n");

            for (UInt32 i = 0; i < dataSize; i += flashTargetConfig.PageSize)
            {
                // 进度条
                toolStripProgressBarMain.Value = (Int32)(((float)i / dataSize) * 100);

                var d = await flash.ReadMass(dap, i, flashTargetConfig.PageSize);
                if (d == null)
                {
                    readError = true;
                    break;
                }

                for (UInt32 j = 0; j < d.Length; j++)
                {
                    if (d[j] != fileTempBuff[i + j])
                    {
                        textBoxLogView.AppendText($"校验错误，地址: 0x{(i + j):X}\r\n");
                        readError = true;
                        break;
                    }
                }

                if (readError)
                {
                    break;
                }
            }

            if (readError)
            {
                textBoxLogView.AppendText($"校验失败\r\n");
            }
            else
            {
                textBoxLogView.AppendText($"校验成功\r\n");
            }

            toolStripStatusLabelMain.Text = "就绪";

            toolStripProgressBarMain.Value = 100;
            dap.Close();

            // 打开所有按钮
            groupBoxOperation.Enabled = true;
            groupBoxTargetSelect.Enabled = true;
            groupDeviceSelect.Enabled = true;
        }
    }
}
