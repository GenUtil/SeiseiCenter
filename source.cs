using System;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
class RandomCodeGenerator
{
    static Random rnd = new Random();
    static string GENIE_CHARS = "APZLGITYEOXUKSVN";
    static int?[] GENIE_MAP_6 = { null, 13, 14, 15, 16, 21, 22, 23, 4, 9, 10, 11, 12, 17, 18, 19, 0, 5, 6, 7, 20, 1, 2, 3 };
    static int?[] GENIE_MAP_8 = { null, 13, 14, 15, 16, 21, 22, 23, 4, 9, 10, 11, 12, 17, 18, 19, 0, 5, 6, 7, 28, 1, 2, 3, 24, 29, 30, 31, 20, 25, 26, 27 };
    static string SETTINGS_FILE = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        "seiseisenta_settings.txt"
    );
    static string DEFAULT_TEMPLATE = "{ADDR} {VAL}";
    static TextBox outputBox;
    static TextBox entryCount, entryAddrLen, entryValLen, entryCmpLen;
    static TextBox entryMinVal, entryMaxVal;
    static CheckBox chkCmp;
    static TextBox templateBox;
    static RadioButton modeNormal, modeGenie;
    static FlowLayoutPanel flowAddrRanges;
    static List<AddrRangeControls> addrRangeList = new List<AddrRangeControls>();
    class AddrRangeControls
    {
        public Panel Panel;
        public TextBox MinAddr;
        public TextBox MaxAddr;
        public Button RemoveBtn;
    }
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Form form = new Form();
        form.Text = "コード生成センター v1.1.03";
        form.Width = 700; form.Height = 700;
        form.MinimumSize = new System.Drawing.Size(650, 600);
        form.FormClosing += (s, e) => SaveSettings();
        try { form.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch {}
        Panel panelMode = new Panel() { Dock = DockStyle.Top, Height = 35 };
        modeNormal = new RadioButton() { Text = "通常モード", AutoSize = true, Checked = true, Left = 10, Top = 8 };
        modeGenie = new RadioButton() { Text = "Game Genieモード", AutoSize = true, Left = 130, Top = 8 };
        modeNormal.CheckedChanged += (s, e) => UpdateUIState();
        panelMode.Controls.AddRange(new Control[] { modeNormal, modeGenie });
        GroupBox groupAddr = new GroupBox() { Text = "アドレス範囲", Dock = DockStyle.Top, Height = 130 };
        flowAddrRanges = new FlowLayoutPanel() { Dock = DockStyle.Fill, AutoScroll = true, FlowDirection = FlowDirection.TopDown, WrapContents = false };
        Button btnAddRange = new Button() { Text = "＋ 範囲を追加", Dock = DockStyle.Bottom, Height = 25 };
        btnAddRange.Click += (s, e) => AddAddrRangeRow("0x0000", "0xFFFF");
        groupAddr.Controls.Add(flowAddrRanges);
        groupAddr.Controls.Add(btnAddRange);
        Panel panelConfig = new Panel() { Dock = DockStyle.Top, Height = 70, Padding = new Padding(5) };
        Label lblValRange = new Label() { Text = "値範囲:", Left = 10, Top = 10, Width = 50 };
        entryMinVal = CreateTextBox(65, 8, 80, "0x00");
        Label lblToVal = new Label() { Text = "~", Left = 150, Top = 10, Width = 15 };
        entryMaxVal = CreateTextBox(170, 8, 80, "0xFF");
        Label lblCount = new Label() { Text = "行数:", Left = 10, Top = 40, Width = 40 };
        entryCount = CreateTextBox(55, 38, 40, "10");
        Label lblAddr = new Label() { Text = "ADDR桁:", Left = 105, Top = 40, Width = 55 };
        entryAddrLen = CreateTextBox(165, 38, 30, "9");
        Label lblVal = new Label() { Text = "VAL桁:", Left = 205, Top = 40, Width = 45 };
        entryValLen = CreateTextBox(255, 38, 30, "2");
        Label lblCmp = new Label() { Text = "CMP桁:", Left = 300, Top = 40, Width = 50 };
        entryCmpLen = CreateTextBox(355, 38, 30, "2");
        chkCmp = new CheckBox() { Text = "CMP使用", Left = 400, Top = 38, AutoSize = true };
        panelConfig.Controls.AddRange(new Control[] {
            lblValRange, entryMinVal, lblToVal, entryMaxVal,
            lblCount, entryCount, lblAddr, entryAddrLen, lblVal, entryValLen, lblCmp, entryCmpLen, chkCmp
        });
        Panel panelTemplate = new Panel() { Dock = DockStyle.Top, Height = 65 };
        Label lblTemplate = new Label() { Text = "テンプレート ({ADDR}, {VAL}, {CMP})", Left = 10, Top = 5, AutoSize = true };
        templateBox = new TextBox() { Left = 10, Top = 25, Width = 660, Height = 35, Multiline = true, ScrollBars = ScrollBars.Vertical, Text = DEFAULT_TEMPLATE };
        SetupTextBoxEvents(templateBox);
        panelTemplate.Controls.AddRange(new Control[] { lblTemplate, templateBox });
        Panel panelButtons = new Panel() { Dock = DockStyle.Top, Height = 40 };
        Button btnGenerate = new Button() { Text = "生成", Left = 10, Top = 5, Width = 100, Height = 30 };
        btnGenerate.Click += (s, e) => GenerateCodes();
        Button btnCopy = new Button() { Text = "コピー", Left = 120, Top = 5, Width = 100, Height = 30 };
        panelButtons.Controls.AddRange(new Control[] { btnGenerate, btnCopy });
        outputBox = new TextBox() { Multiline = true, ScrollBars = ScrollBars.Vertical, Font = new Font("Consolas", 10), Dock = DockStyle.Fill };
        SetupTextBoxEvents(outputBox);
        form.Controls.Add(outputBox);
        form.Controls.Add(panelButtons);
        form.Controls.Add(panelTemplate);
        form.Controls.Add(panelConfig);
        form.Controls.Add(groupAddr);
        form.Controls.Add(panelMode);
        LoadSettings();
        if (addrRangeList.Count == 0) AddAddrRangeRow("0x0000", "0xFFFF");
        UpdateUIState();
        Application.Run(form);
    }
    static TextBox CreateTextBox(int x, int y, int w, string text)
    {
        TextBox tb = new TextBox() { Left = x, Top = y, Width = w, Text = text };
        SetupTextBoxEvents(tb);
        return tb;
    }
    static void SetupTextBoxEvents(TextBox tb)
    {
        tb.KeyDown += (s, e) => {
            if (e.Control && e.KeyCode == Keys.A) {
                ((TextBox)s).SelectAll();
                e.SuppressKeyPress = true;
            }
        };
        tb.KeyPress += (s, e) => {
            if (e.KeyChar == (char)127) e.Handled = true;
        };
    }
    static void AddAddrRangeRow(string min, string max)
    {
        Panel p = new Panel() { Width = 550, Height = 28 };
        TextBox tbMin = CreateTextBox(0, 2, 110, min);
        Label lbl = new Label() { Text = "~", Left = 115, Top = 5, Width = 15 };
        TextBox tbMax = CreateTextBox(135, 2, 110, max);
        Button btnRem = new Button() { Text = "×", Left = 250, Top = 1, Width = 25, Height = 24 };
        var row = new AddrRangeControls { Panel = p, MinAddr = tbMin, MaxAddr = tbMax, RemoveBtn = btnRem };
        btnRem.Click += (s, e) => {
            flowAddrRanges.Controls.Remove(p);
            addrRangeList.Remove(row);
        };
        p.Controls.AddRange(new Control[] { tbMin, lbl, tbMax, btnRem });
        addrRangeList.Add(row);
        flowAddrRanges.Controls.Add(p);
    }
    static void UpdateUIState()
    {
        bool normal = modeNormal.Checked;
        templateBox.Enabled = normal;
        entryCmpLen.Enabled = normal;
        chkCmp.Enabled = normal;
        foreach (var row in addrRangeList) {
            row.MinAddr.Enabled = normal;
            row.MaxAddr.Enabled = normal;
            row.RemoveBtn.Enabled = normal;
        }
        entryMinVal.Enabled = normal;
        entryMaxVal.Enabled = normal;
    }
    static void GenerateCodes()
    {
        outputBox.Clear();
        int count = 10, addrLen = 9, valLen = 2, cmpLen = 2;
        int.TryParse(entryCount.Text, out count);
        int.TryParse(entryAddrLen.Text, out addrLen);
        int.TryParse(entryValLen.Text, out valLen);
        int.TryParse(entryCmpLen.Text, out cmpLen);
        var ranges = new List<Tuple<long, long>>();
        foreach (var row in addrRangeList)
        {
            try
            {
                long min = Convert.ToInt64(row.MinAddr.Text.Replace("0x", ""), 16);
                long max = Convert.ToInt64(row.MaxAddr.Text.Replace("0x", ""), 16);
                if (min <= max) ranges.Add(new Tuple<long, long>(min, max));
            }
            catch { }
        }
        if (ranges.Count == 0 && modeNormal.Checked)
        {
            MessageBox.Show("有効なアドレス範囲を設定してください。");
            return;
        }
        long minVal = 0, maxVal = 0xFF;
        try { minVal = Convert.ToInt64(entryMinVal.Text.Replace("0x", ""), 16); } catch { }
        try { maxVal = Convert.ToInt64(entryMaxVal.Text.Replace("0x", ""), 16); } catch { }
        StringBuilder sb = new StringBuilder();
        if (modeNormal.Checked)
        {
            long cmpRange = (long)Math.Pow(16, cmpLen) - 1;
            string template = string.IsNullOrEmpty(templateBox.Text) ? DEFAULT_TEMPLATE : templateBox.Text;
            for (int i = 0; i < count; i++)
            {
                var targetRange = ranges[rnd.Next(ranges.Count)];
                long randomAddr = RandomLong(targetRange.Item1, targetRange.Item2);
                long randomVal = RandomLong(minVal, maxVal);
                long randomCmp = RandomLong(0, cmpRange);
                string addrStr = randomAddr.ToString("X" + addrLen);
                string valStr = randomVal.ToString("X" + valLen);
                string cmpStr = randomCmp.ToString("X" + cmpLen);
                string code = template.Replace("{ADDR}", addrStr).Replace("{VAL}", valStr).Replace("{CMP}", chkCmp.Checked ? cmpStr : "");
                sb.AppendLine(code);
            }
        }
        else
        {
            long gMinA = ranges.Count > 0 ? ranges[0].Item1 : 0;
            long gMaxA = ranges.Count > 0 ? ranges[0].Item2 : 0xFFFF;
            for (int i = 0; i < count; i++)
            {
                int addr = rnd.Next((int)Math.Min(gMinA, int.MaxValue), (int)Math.Min(gMaxA, int.MaxValue) + 1);
                int val = rnd.Next((int)minVal, (int)maxVal + 1);
                int cmp = rnd.Next(0, 0xFF + 1);
                sb.AppendLine(rnd.NextDouble() < 0.5 ? Encode6(addr, val) : Encode8(addr, val, cmp));
            }
        }
        outputBox.Text = sb.ToString();
        SaveSettings();
    }
    static long RandomLong(long minValue, long maxValue)
    {
        if (minValue >= maxValue) return minValue;
        byte[] buf = new byte[8];
        rnd.NextBytes(buf);
        long longRand = BitConverter.ToInt64(buf, 0);
        return (Math.Abs(longRand % (maxValue - minValue + 1)) + minValue);
    }
    static void CopyToClipboard()
    {
        if (!string.IsNullOrEmpty(outputBox.Text)) {
            Clipboard.SetText(outputBox.Text);
            MessageBox.Show("コピーしました。");
        }
    }
    static void SaveSettings()
    {
        try
        {
            using (StreamWriter sw = new StreamWriter(SETTINGS_FILE, false, Encoding.UTF8))
            {
                sw.WriteLine("Mode=" + (modeNormal.Checked ? "normal" : "genie"));
                sw.WriteLine("Count=" + entryCount.Text);
                sw.WriteLine("AddrLen=" + entryAddrLen.Text);
                sw.WriteLine("ValLen=" + entryValLen.Text);
                sw.WriteLine("CmpLen=" + entryCmpLen.Text);
                sw.WriteLine("UseCmp=" + chkCmp.Checked);
                sw.WriteLine("Template=" + templateBox.Text.Replace("\r", "").Replace("\n", "\\n"));
                sw.WriteLine("MinVal=" + entryMinVal.Text);
                sw.WriteLine("MaxVal=" + entryMaxVal.Text);
                var ranges = addrRangeList.Select(r => r.MinAddr.Text + "," + r.MaxAddr.Text);
                sw.WriteLine("AddrRanges=" + string.Join("|", ranges));
            }
        } catch { }
    }
    static void LoadSettings()
    {
        try
        {
            if (!File.Exists(SETTINGS_FILE)) return;
            var lines = File.ReadAllLines(SETTINGS_FILE);
            foreach (var line in lines)
            {
                var kv = line.Split(new char[] { '=' }, 2);
                if (kv.Length != 2) continue;
                switch (kv[0])
                {
                    case "Mode": modeNormal.Checked = kv[1] == "normal"; break;
                    case "Count": entryCount.Text = kv[1]; break;
                    case "AddrLen": entryAddrLen.Text = kv[1]; break;
                    case "ValLen": entryValLen.Text = kv[1]; break;
                    case "CmpLen": entryCmpLen.Text = kv[1]; break;
                    case "UseCmp": chkCmp.Checked = kv[1] == "True"; break;
                    case "Template": templateBox.Text = kv[1].Replace("\\n", "\r\n"); break;
                    case "MinVal": entryMinVal.Text = kv[1]; break;
                    case "MaxVal": entryMaxVal.Text = kv[1]; break;
                    case "AddrRanges":
                        flowAddrRanges.Controls.Clear();
                        addrRangeList.Clear();
                        foreach (var pair in kv[1].Split('|')) {
                            var m = pair.Split(',');
                            if (m.Length == 2) AddAddrRangeRow(m[0], m[1]);
                        }
                        break;
                }
            }
        } catch { }
    }
    static string Encode6(int addr, int val)
    {
        int[] bits = new int[24];
        Array.Copy(ToBits(addr, 16), 0, bits, 0, 16);
        Array.Copy(ToBits(val, 8), 0, bits, 16, 8);
        int[] shuffled = GenieShuffle(bits, GENIE_MAP_6);
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < 24; i += 4) sb.Append(GENIE_CHARS[FromBits(shuffled, i, 4)]);
        return sb.ToString();
    }
    static string Encode8(int addr, int val, int cmp)
    {
        int[] bits = new int[32];
        Array.Copy(ToBits(addr, 16), 0, bits, 0, 16);
        Array.Copy(ToBits(val, 8), 0, bits, 16, 8);
        Array.Copy(ToBits(cmp, 8), 0, bits, 24, 8);
        int[] shuffled = GenieShuffle(bits, GENIE_MAP_8);
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < 32; i += 4) sb.Append(GENIE_CHARS[FromBits(shuffled, i, 4)]);
        return sb.ToString();
    }
    static int[] ToBits(int value, int length)
    {
        int[] bits = new int[length];
        for (int i = 0; i < length; i++) bits[i] = (value & (1 << (length - 1 - i))) != 0 ? 1 : 0;
        return bits;
    }
    static int FromBits(int[] bits, int start, int len)
    {
        int v = 0;
        for (int i = start; i < start + len; i++) v = (v << 1) | bits[i];
        return v;
    }
    static int[] GenieShuffle(int[] bits, int?[] map)
    {
        int[] result = new int[bits.Length];
        for (int i = 0; i < map.Length; i++)
            if (map[i].HasValue && map[i].Value < bits.Length) result[i] = bits[map[i].Value];
        return result;
    }
}
