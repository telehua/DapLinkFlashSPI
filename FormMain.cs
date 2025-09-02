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

        // Ŀ��������
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

        // Ŀ�������б�
        public class FlashTargetList
        {
            public IList<FlashTargetConfig>? Target { get; set; }
        }

        // ҳ������¼�
        private void FormMain_Load(object sender, EventArgs e)
        {
            // �ر����а�ť
            groupBoxOperation.Enabled = false;
            groupBoxTargetSelect.Enabled = false;
            groupDeviceSelect.Enabled = false;

            // Ĭ���ٶ�ѡ��
            comboBoxDeviceSpeed.SelectedIndex = 4;

            // ���������ļ�
            String configFile = "ConfigDapLinkFlashSPI.json";
            if (!File.Exists(configFile))
            {
                toolStripStatusLabelMain.Text = "����";
                textBoxLogView.AppendText("�Ҳ��������ļ�\r\n");
                return;
            }

            textBoxLogView.AppendText($"�ҵ������ļ�\r\n");
            var cfg = File.ReadAllText(configFile);
            // ����JSON�ļ�
            try
            {
                flashTargetList = JsonSerializer.Deserialize<FlashTargetList>(cfg);
            }
            catch (Exception ex)
            {
                toolStripStatusLabelMain.Text = "����";
                textBoxLogView.AppendText($"�����ļ�����ʧ��\r\n");
                return;
            }

            if (flashTargetList == null || flashTargetList.Target == null)
            {
                toolStripStatusLabelMain.Text = "����";
                textBoxLogView.AppendText($"�����ļ�����ʧ��\r\n");
                return;
            }

            // ����ѡоƬд��ѡ���б�
            comboBoxTargetNameSelect.Items.Clear();
            foreach (var item in flashTargetList.Target)
            {
                if (item.Name == null)
                {
                    textBoxLogView.AppendText($"�����ļ�����ʧ��\r\n");
                    return;
                }
                comboBoxTargetNameSelect.Items.Add(item.Name);
            }

            toolStripStatusLabelMain.Text = "����";
            textBoxLogView.AppendText($"�����ļ������ɹ�\r\n");

            // FLASH�ͺ�ѡ��
            comboBoxTargetNameSelect.SelectedIndex = 0;

            // �����а�ť
            groupBoxOperation.Enabled = true;
            groupBoxTargetSelect.Enabled = true;
            groupDeviceSelect.Enabled = true;
        }

        // ��ȡ��ť����¼�
        private async void ButtonDump_Click(object sender, EventArgs e)
        {
            if (comboBoxDeviceSelect.SelectedIndex < 0)
            {
                textBoxLogView.AppendText($"��ѡ���豸\r\n");
                return;
            }

            var nameAndSerialNumber = comboBoxDeviceSelect.Items[comboBoxDeviceSelect.SelectedIndex];
            String? s = nameAndSerialNumber?.ToString();

            if (s == null)
            {
                textBoxLogView.AppendText("��ˢ���б�\r\n");
                return;
            }
            String[] stringList = s.Split('[', ']');
            if (stringList == null || stringList.Length < 2)
            {
                textBoxLogView.AppendText("��ˢ���б�\r\n");
                return;
            }
            String deviceDame = stringList[0];
            String serialNumber = stringList[1];
            textBoxLogView.AppendText($"����ʹ��{deviceDame}\r\n");

            CmsisDap dap = new CmsisDap();
            DapSpiFlash flash = new DapSpiFlash();

            Int32 result = await dap.OpenAsync(0, 0, serialNumber);
            if (result != 0)
            {
                textBoxLogView.AppendText("�豸��ʧ��\r\n");
                return;
            }

            if (flashTargetConfig == null)
            {
                textBoxLogView.AppendText("��������\r\n");
                return;
            }

            // ��JTAG�ӿ�
            await dap.Connect(CmsisDap.Port.JTAG);
            await dap.Speed(10 * 1000000);

            // �ر����а�ť
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

            toolStripStatusLabelMain.Text = "����";
            textBoxLogView.AppendText($"��ȡ��......\r\n");
            toolStripProgressBarMain.Value = 0;

            Byte[] buffer = new Byte[flashTargetConfig.ChipSize];
            fileTempBuff = buffer;
            //for (UInt32 i = 0; i < fileTempBuff.Length; i++)
            //{
            //    fileTempBuff[i] = 0xFF;
            //}

            // ��ȡ����
            Boolean readError = false;
            for (UInt32 i = 0; i < flashTargetConfig.ChipSize; i += flashTargetConfig.PageSize)
            {
                // ������
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
                textBoxLogView.AppendText($"��ȡʧ��\r\n");
            }
            else
            {
                textBoxLogView.AppendText($"��ȡ�ɹ�\r\n");
            }

            toolStripStatusLabelMain.Text = "����";

            toolStripProgressBarMain.Value = 100;
            dap.Close();

            // �����а�ť
            groupBoxOperation.Enabled = true;
            groupBoxTargetSelect.Enabled = true;
            groupDeviceSelect.Enabled = true;
        }

        // ���ļ�����¼�
        private void ToolStripMenuItemFileOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new()
            {
                Multiselect = false,
                Filter = "Binary Image|*.bin"
            };

            if (ofd.ShowDialog() != DialogResult.OK)
            {
                textBoxLogView.AppendText($"ȡ�����ļ�\r\n");
                return;
            }

            textBoxLogView.AppendText($"���ļ�{ofd.FileName}\r\n");

            var fileData = File.ReadAllBytes(ofd.FileName);
            if (fileData == null)
            {
                textBoxLogView.AppendText($"��ȡ�ļ�ʧ��\r\n");
                return;
            }

            textBoxLogView.AppendText($"���ļ���ȡ 0x{fileData.Length:X} Bytes\r\n");
            fileTempBuff = fileData;

            ofd.Dispose();
        }

        // �����ļ�����¼�
        private void ToolStripMenuItemFileSave_Click(object sender, EventArgs e)
        {
            if (fileTempBuff == null)
            {
                textBoxLogView.AppendText($"�����ݣ�����ʧ��\r\n");
                return;
            }

            SaveFileDialog sfd = new()
            {
                Filter = "Binary|*.bin"
            };

            if (sfd.ShowDialog() != DialogResult.OK)
            {
                textBoxLogView.AppendText($"ȡ�������ļ�\r\n");
                sfd.Dispose();
                return;
            }

            var f = File.Create(sfd.FileName, fileTempBuff.Length);
            textBoxLogView.AppendText($"���浽�ļ�{sfd.FileName}\r\n");
            f.Write(fileTempBuff);
            f.Close();
            textBoxLogView.AppendText($"д���ļ� 0x{fileTempBuff.Length:X} Bytes\r\n");
            sfd.Dispose();
        }

        // ��������¼�
        private void ToolStripMenuItemHelpAbout_Click(object sender, EventArgs e)
        {
            FormAbout formAbout = new();
            formAbout.ShowDialog();
            formAbout.Dispose();
        }

        // оƬ�л��¼�
        private void ComboBoxTargetNameSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (flashTargetList == null || flashTargetList.Target == null)
            {
                return;
            }
            // �л�Ŀ��FLASH����
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

                textBoxLogView.AppendText($"������{list.Length}���豸\r\n");
                comboBoxDeviceSelect.SelectedIndex = 0;
            }
            else
            {
                textBoxLogView.AppendText($"�޿����豸\r\n");
                comboBoxDeviceSelect.SelectedIndex = -1;
                comboBoxDeviceSelect.Text = String.Empty;
            }
        }

        private async void buttonProgram_Click(object sender, EventArgs e)
        {
            if (fileTempBuff == null || fileTempBuff.Length == 0)
            {
                textBoxLogView.AppendText($"�����ݿ��ã�����ļ�\r\n");
                return;
            }

            if (comboBoxDeviceSelect.SelectedIndex < 0)
            {
                textBoxLogView.AppendText($"��ѡ���豸\r\n");
                return;
            }

            var nameAndSerialNumber = comboBoxDeviceSelect.Items[comboBoxDeviceSelect.SelectedIndex];
            String? s = nameAndSerialNumber?.ToString();

            if (s == null)
            {
                textBoxLogView.AppendText("��ˢ���б�\r\n");
                return;
            }
            String[] stringList = s.Split('[', ']');
            if (stringList == null || stringList.Length < 2)
            {
                textBoxLogView.AppendText("��ˢ���б�\r\n");
                return;
            }
            String deviceDame = stringList[0];
            String serialNumber = stringList[1];
            textBoxLogView.AppendText($"����ʹ��{deviceDame}\r\n");

            CmsisDap dap = new CmsisDap();
            DapSpiFlash flash = new DapSpiFlash();

            Int32 result = await dap.OpenAsync(0, 0, serialNumber);
            if (result != 0)
            {
                textBoxLogView.AppendText("�豸��ʧ��\r\n");
                return;
            }

            if (flashTargetConfig == null)
            {
                textBoxLogView.AppendText("��������\r\n");
                return;
            }

            // ��JTAG�ӿ�
            await dap.Connect(CmsisDap.Port.JTAG);
            await dap.Speed(10 * 1000000);

            // �ر����а�ť
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

            toolStripStatusLabelMain.Text = "����";
            textBoxLogView.AppendText($"д����......\r\n");
            toolStripProgressBarMain.Value = 0;

            // д������
            Boolean readError = false;
            UInt32 dataSize = (fileTempBuff.Length < flashTargetConfig.ChipSize) ? (UInt32)fileTempBuff.Length : flashTargetConfig.ChipSize;
            for (UInt32 i = 0; i < dataSize; i += flashTargetConfig.PageSize)
            {
                // ������
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
                textBoxLogView.AppendText($"д��ʧ��\r\n");
            }
            else
            {
                textBoxLogView.AppendText($"д��ɹ���0x{dataSize:X} Bytes\r\n");
            }
            toolStripStatusLabelMain.Text = "����";

            toolStripProgressBarMain.Value = 100;
            dap.Close();

            // �����а�ť
            groupBoxOperation.Enabled = true;
            groupBoxTargetSelect.Enabled = true;
            groupDeviceSelect.Enabled = true;
        }

        private async void buttonErase_Click(object sender, EventArgs e)
        {
            if (comboBoxDeviceSelect.SelectedIndex < 0)
            {
                textBoxLogView.AppendText($"��ѡ���豸\r\n");
                return;
            }

            var nameAndSerialNumber = comboBoxDeviceSelect.Items[comboBoxDeviceSelect.SelectedIndex];
            String? s = nameAndSerialNumber?.ToString();

            if (s == null)
            {
                textBoxLogView.AppendText("��ˢ���б�\r\n");
                return;
            }
            String[] stringList = s.Split('[', ']');
            if (stringList == null || stringList.Length < 2)
            {
                textBoxLogView.AppendText("��ˢ���б�\r\n");
                return;
            }
            String deviceDame = stringList[0];
            String serialNumber = stringList[1];
            textBoxLogView.AppendText($"����ʹ��{deviceDame}\r\n");

            CmsisDap dap = new CmsisDap();
            DapSpiFlash flash = new DapSpiFlash();

            Int32 result = await dap.OpenAsync(0, 0, serialNumber);
            if (result != 0)
            {
                textBoxLogView.AppendText("�豸��ʧ��\r\n");
                return;
            }

            if (flashTargetConfig == null)
            {
                textBoxLogView.AppendText("��������\r\n");
                return;
            }

            // ��JTAG�ӿ�
            await dap.Connect(CmsisDap.Port.JTAG);
            await dap.Speed(10 * 1000000);

            // �ر����а�ť
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

            toolStripStatusLabelMain.Text = "����";
            textBoxLogView.AppendText($"ȫƬ������......\r\n");
            toolStripProgressBarMain.Value = 0;

            await flash.EraseChip(dap);

            toolStripProgressBarMain.Value = 100;
            dap.Close();

            toolStripStatusLabelMain.Text = "����";
            textBoxLogView.AppendText($"�����ɹ�\r\n");

            // �����а�ť
            groupBoxOperation.Enabled = true;
            groupBoxTargetSelect.Enabled = true;
            groupDeviceSelect.Enabled = true;
        }

        private async void buttonCheckEmpty_Click(object sender, EventArgs e)
        {
            if (comboBoxDeviceSelect.SelectedIndex < 0)
            {
                textBoxLogView.AppendText($"��ѡ���豸\r\n");
                return;
            }

            var nameAndSerialNumber = comboBoxDeviceSelect.Items[comboBoxDeviceSelect.SelectedIndex];
            String? s = nameAndSerialNumber?.ToString();

            if (s == null)
            {
                textBoxLogView.AppendText("��ˢ���б�\r\n");
                return;
            }
            String[] stringList = s.Split('[', ']');
            if (stringList == null || stringList.Length < 2)
            {
                textBoxLogView.AppendText("��ˢ���б�\r\n");
                return;
            }
            String deviceDame = stringList[0];
            String serialNumber = stringList[1];
            textBoxLogView.AppendText($"����ʹ��{deviceDame}\r\n");

            CmsisDap dap = new CmsisDap();
            DapSpiFlash flash = new DapSpiFlash();

            Int32 result = await dap.OpenAsync(0, 0, serialNumber);
            if (result != 0)
            {
                textBoxLogView.AppendText("�豸��ʧ��\r\n");
                return;
            }

            if (flashTargetConfig == null)
            {
                textBoxLogView.AppendText("��������\r\n");
                return;
            }

            // ��JTAG�ӿ�
            await dap.Connect(CmsisDap.Port.JTAG);
            await dap.Speed(10 * 1000000);

            // �ر����а�ť
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

            toolStripStatusLabelMain.Text = "����";
            textBoxLogView.AppendText($"��ȡ��......\r\n");
            toolStripProgressBarMain.Value = 0;

            // ��ȡ����
            Boolean readError = false;
            for (UInt32 i = 0; i < flashTargetConfig.ChipSize; i += flashTargetConfig.PageSize)
            {
                // ������
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
                        textBoxLogView.AppendText($"�����󣬵�ַ: 0x{(i + j):X}\r\n");
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
                textBoxLogView.AppendText($"���ʧ��\r\n");
            }
            else
            {
                textBoxLogView.AppendText($"���ɹ�\r\n");
            }

            toolStripStatusLabelMain.Text = "����";

            toolStripProgressBarMain.Value = 100;
            dap.Close();

            // �����а�ť
            groupBoxOperation.Enabled = true;
            groupBoxTargetSelect.Enabled = true;
            groupDeviceSelect.Enabled = true;
        }

        private async void buttonMatch_Click(object sender, EventArgs e)
        {
            if (fileTempBuff == null || fileTempBuff.Length == 0)
            {
                textBoxLogView.AppendText($"�����ݿ���\r\n");
                return;
            }

            if (comboBoxDeviceSelect.SelectedIndex < 0)
            {
                textBoxLogView.AppendText($"��ѡ���豸\r\n");
                return;
            }

            var nameAndSerialNumber = comboBoxDeviceSelect.Items[comboBoxDeviceSelect.SelectedIndex];
            String? s = nameAndSerialNumber?.ToString();

            if (s == null)
            {
                textBoxLogView.AppendText("��ˢ���б�\r\n");
                return;
            }
            String[] stringList = s.Split('[', ']');
            if (stringList == null || stringList.Length < 2)
            {
                textBoxLogView.AppendText("��ˢ���б�\r\n");
                return;
            }
            String deviceDame = stringList[0];
            String serialNumber = stringList[1];
            textBoxLogView.AppendText($"����ʹ��{deviceDame}\r\n");

            CmsisDap dap = new CmsisDap();
            DapSpiFlash flash = new DapSpiFlash();

            Int32 result = await dap.OpenAsync(0, 0, serialNumber);
            if (result != 0)
            {
                textBoxLogView.AppendText("�豸��ʧ��\r\n");
                return;
            }

            if (flashTargetConfig == null)
            {
                textBoxLogView.AppendText("��������\r\n");
                return;
            }

            // ��JTAG�ӿ�
            await dap.Connect(CmsisDap.Port.JTAG);
            await dap.Speed(10 * 1000000);

            // �ر����а�ť
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

            toolStripStatusLabelMain.Text = "����";
            textBoxLogView.AppendText($"��ȡ��......\r\n");
            toolStripProgressBarMain.Value = 0;

            // ��ȡ����
            Boolean readError = false;
            UInt32 dataSize = (fileTempBuff.Length < flashTargetConfig.ChipSize) ? (UInt32)fileTempBuff.Length : flashTargetConfig.ChipSize;
            textBoxLogView.AppendText($"�Ա�����0x{dataSize:X} Bytes\r\n");

            for (UInt32 i = 0; i < dataSize; i += flashTargetConfig.PageSize)
            {
                // ������
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
                        textBoxLogView.AppendText($"У����󣬵�ַ: 0x{(i + j):X}\r\n");
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
                textBoxLogView.AppendText($"У��ʧ��\r\n");
            }
            else
            {
                textBoxLogView.AppendText($"У��ɹ�\r\n");
            }

            toolStripStatusLabelMain.Text = "����";

            toolStripProgressBarMain.Value = 100;
            dap.Close();

            // �����а�ť
            groupBoxOperation.Enabled = true;
            groupBoxTargetSelect.Enabled = true;
            groupDeviceSelect.Enabled = true;
        }
    }
}
