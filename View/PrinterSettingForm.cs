using System;
using System.Windows.Forms;
using System.Drawing.Printing;
using HwModule.Settings;
using HwModule.Controller;

namespace HwModule.View
{
    public partial class PrinterSettingForm : Form
    {
        private PrintController printController;

        public PrinterSettingForm(PrintController printController)
        {
            InitializeComponent();
            this.printController = printController;
            LoadPrinters();
            LoadSettings();
        }

        private void LoadPrinters()
        {
            PrinterSelect.Items.Clear();
            foreach (string p in PrinterSettings.InstalledPrinters)
                PrinterSelect.Items.Add(p);
        }

        private void LoadSettings()
        {
            var s = ApplicationSetting.Instance;

            // 공통
            ChkDefaultPrinter.Checked = s.UseDefaultPrinter;
            UpdatePrinterSelectEnabled();

            // 프린터 1
            if (PrinterSelect.Items.Contains(s.PrinterSetting.PrinterName))
                PrinterSelect.SelectedItem = s.PrinterSetting.PrinterName;

            IsLandscapeY.Checked = s.PrintPaperSetting.IsLandscape;
            IsLandscapeN.Checked = !s.PrintPaperSetting.IsLandscape;
            LocationX.Text  = s.PrintLayoutSetting.LocationX.ToString();
            LocationY.Text  = s.PrintLayoutSetting.LocationY.ToString();
            PaperWidth.Text = s.PrintPaperSetting.PaperWidth.ToString();
            FontSize.Text   = s.PrintLayoutSetting.FontSize.ToString();
        }

        private void UpdatePrinterSelectEnabled()
        {
            bool useDefault = ChkDefaultPrinter.Checked;
            PrinterSelect.Enabled = !useDefault;
            LblPrinterName.Enabled = !useDefault;
        }

        private void ChkDefaultPrinter_CheckedChanged(object sender, EventArgs e)
            => UpdatePrinterSelectEnabled();

        private void btnSave_Click(object sender, EventArgs e)
        {
            var s = ApplicationSetting.Instance;

            s.UseDefaultPrinter = ChkDefaultPrinter.Checked;

            if (!s.UseDefaultPrinter && PrinterSelect.SelectedItem != null)
                s.PrinterSetting.PrinterName = PrinterSelect.SelectedItem.ToString();

            s.PrintPaperSetting.IsLandscape  = IsLandscapeY.Checked;
            s.PrintLayoutSetting.LocationX   = int.TryParse(LocationX.Text, out int lx) ? lx : 0;
            s.PrintLayoutSetting.LocationY   = int.TryParse(LocationY.Text, out int ly) ? ly : 0;
            s.PrintPaperSetting.PaperWidth   = int.TryParse(PaperWidth.Text, out int pw) ? pw : 80;
            s.PrintLayoutSetting.FontSize    = int.TryParse(FontSize.Text, out int fs) ? fs : 9;

            s.SaveToFile();
            MessageBox.Show("설정이 저장되었습니다.", "HwModule v0", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnCancel_Click(object sender, EventArgs e)
            => this.Close();

        private void btnTestGoods_Click(object sender, EventArgs e)
        {
            try { printController.PrintGoodsReceiptTest(); }
            catch (Exception ex) { MessageBox.Show("테스트 오류: " + ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnTestTicket_Click(object sender, EventArgs e)
        {
            try { printController.PrintTest1(""); }
            catch (Exception ex) { MessageBox.Show("테스트 오류: " + ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
    }
}
