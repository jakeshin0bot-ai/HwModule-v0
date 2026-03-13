namespace HwModule.View
{
    partial class PrinterSettingForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.ChkDefaultPrinter = new System.Windows.Forms.CheckBox();
            this.LblPrinterName    = new System.Windows.Forms.Label();
            this.PrinterSelect     = new System.Windows.Forms.ComboBox();
            this.IsLandscapeY      = new System.Windows.Forms.RadioButton();
            this.IsLandscapeN      = new System.Windows.Forms.RadioButton();
            this.LocationX         = new System.Windows.Forms.TextBox();
            this.LocationY         = new System.Windows.Forms.TextBox();
            this.PaperWidth        = new System.Windows.Forms.TextBox();
            this.FontSize          = new System.Windows.Forms.TextBox();
            this.btnSave           = new System.Windows.Forms.Button();
            this.btnCancel         = new System.Windows.Forms.Button();
            this.btnTestGoods      = new System.Windows.Forms.Button();
            this.btnTestTicket     = new System.Windows.Forms.Button();
            this.SuspendLayout();

            int lx = 20, w = 200, h = 24, pad = 32;
            int y = 15;

            // Title
            var lblTitle = new System.Windows.Forms.Label();
            lblTitle.Text = "HwModule v0 — 프린터 설정";
            lblTitle.Font = new System.Drawing.Font("굴림", 10f, System.Drawing.FontStyle.Bold);
            lblTitle.Location = new System.Drawing.Point(lx, y); lblTitle.Size = new System.Drawing.Size(300, 22);
            this.Controls.Add(lblTitle); y += pad + 4;

            // 기본 프린터 체크박스
            this.ChkDefaultPrinter.Text = "Windows 기본 프린터 사용 (드라이버 미설치 시 권장)";
            this.ChkDefaultPrinter.Location = new System.Drawing.Point(lx, y);
            this.ChkDefaultPrinter.Size = new System.Drawing.Size(330, h);
            this.ChkDefaultPrinter.CheckedChanged += new System.EventHandler(this.ChkDefaultPrinter_CheckedChanged);
            this.Controls.Add(this.ChkDefaultPrinter); y += pad;

            // 프린터 선택
            this.LblPrinterName.Text = "프린터 선택:";
            this.LblPrinterName.Location = new System.Drawing.Point(lx, y + 3);
            this.LblPrinterName.Size = new System.Drawing.Size(80, h);
            this.Controls.Add(this.LblPrinterName);

            this.PrinterSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PrinterSelect.Location = new System.Drawing.Point(lx + 90, y);
            this.PrinterSelect.Size = new System.Drawing.Size(240, h);
            this.Controls.Add(this.PrinterSelect); y += pad;

            // 구분선
            var sep1 = new System.Windows.Forms.Label();
            sep1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            sep1.Location = new System.Drawing.Point(lx, y); sep1.Size = new System.Drawing.Size(340, 2);
            this.Controls.Add(sep1); y += 10;

            // 용지 방향
            var lblOri = new System.Windows.Forms.Label();
            lblOri.Text = "용지 방향:";
            lblOri.Location = new System.Drawing.Point(lx, y + 3); lblOri.Size = new System.Drawing.Size(80, h);
            this.Controls.Add(lblOri);

            this.IsLandscapeN.Text = "세로(Portrait)";
            this.IsLandscapeN.Location = new System.Drawing.Point(lx + 90, y);
            this.IsLandscapeN.Size = new System.Drawing.Size(120, h);
            this.Controls.Add(this.IsLandscapeN);

            this.IsLandscapeY.Text = "가로(Landscape)";
            this.IsLandscapeY.Location = new System.Drawing.Point(lx + 215, y);
            this.IsLandscapeY.Size = new System.Drawing.Size(130, h);
            this.Controls.Add(this.IsLandscapeY); y += pad;

            // 용지 가로
            AddLabelTextBox("용지 가로(mm):", lx, y, ref this.PaperWidth, w - 80); y += pad;
            // 여백 X
            AddLabelTextBox("여백 X(px):", lx, y, ref this.LocationX, w - 80); y += pad;
            // 여백 Y
            AddLabelTextBox("여백 Y(px):", lx, y, ref this.LocationY, w - 80); y += pad;
            // 폰트 크기
            AddLabelTextBox("폰트 크기:", lx, y, ref this.FontSize, w - 80); y += pad + 4;

            // 구분선
            var sep2 = new System.Windows.Forms.Label();
            sep2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            sep2.Location = new System.Drawing.Point(lx, y); sep2.Size = new System.Drawing.Size(340, 2);
            this.Controls.Add(sep2); y += 10;

            // 테스트 버튼
            this.btnTestGoods.Text = "현장구매 테스트 출력";
            this.btnTestGoods.Location = new System.Drawing.Point(lx, y);
            this.btnTestGoods.Size = new System.Drawing.Size(155, 30);
            this.btnTestGoods.Click += new System.EventHandler(this.btnTestGoods_Click);
            this.Controls.Add(this.btnTestGoods);

            this.btnTestTicket.Text = "티켓 테스트 출력";
            this.btnTestTicket.Location = new System.Drawing.Point(lx + 165, y);
            this.btnTestTicket.Size = new System.Drawing.Size(155, 30);
            this.btnTestTicket.Click += new System.EventHandler(this.btnTestTicket_Click);
            this.Controls.Add(this.btnTestTicket); y += 40;

            // 저장/취소
            this.btnSave.Text = "저장";
            this.btnSave.Location = new System.Drawing.Point(lx + 160, y);
            this.btnSave.Size = new System.Drawing.Size(80, 30);
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            this.Controls.Add(this.btnSave);

            this.btnCancel.Text = "닫기";
            this.btnCancel.Location = new System.Drawing.Point(lx + 250, y);
            this.btnCancel.Size = new System.Drawing.Size(80, 30);
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            this.Controls.Add(this.btnCancel); y += 45;

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(380, y);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PrinterSettingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HwModule v0 설정";
            this.ResumeLayout(false);
        }

        private void AddLabelTextBox(string label, int lx, int y, ref System.Windows.Forms.TextBox tb, int tbWidth)
        {
            var lbl = new System.Windows.Forms.Label();
            lbl.Text = label;
            lbl.Location = new System.Drawing.Point(lx, y + 3);
            lbl.Size = new System.Drawing.Size(90, 20);
            this.Controls.Add(lbl);
            tb = new System.Windows.Forms.TextBox();
            tb.Location = new System.Drawing.Point(lx + 95, y);
            tb.Size = new System.Drawing.Size(tbWidth, 22);
            this.Controls.Add(tb);
        }

        private System.Windows.Forms.CheckBox ChkDefaultPrinter;
        private System.Windows.Forms.Label    LblPrinterName;
        private System.Windows.Forms.ComboBox PrinterSelect;
        private System.Windows.Forms.RadioButton IsLandscapeY;
        private System.Windows.Forms.RadioButton IsLandscapeN;
        private System.Windows.Forms.TextBox  LocationX;
        private System.Windows.Forms.TextBox  LocationY;
        private System.Windows.Forms.TextBox  PaperWidth;
        private System.Windows.Forms.TextBox  FontSize;
        private System.Windows.Forms.Button   btnSave;
        private System.Windows.Forms.Button   btnCancel;
        private System.Windows.Forms.Button   btnTestGoods;
        private System.Windows.Forms.Button   btnTestTicket;
    }
}
