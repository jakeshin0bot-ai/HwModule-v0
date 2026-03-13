using HwModule.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace HwModule.Settings
{
    /// <summary>
    /// HwModule v0 - 통합 설정 (v1 + v2 merged)
    /// 저장 위치: {실행파일폴더}/Settings/applicationSetting.json
    /// </summary>
    public class ApplicationSetting
    {
        private ApplicationSetting() { SetDefault(); }

        private static ApplicationSetting instance;
        public static ApplicationSetting Instance
        {
            get
            {
                if (instance == null) instance = LoadFromFile();
                return instance;
            }
        }

        // ── 프린터 1 (메인 프린터) ──────────────────────────────────
        public PrinterSetting     PrinterSetting     { get; set; }
        public PrintPaperSetting  PrintPaperSetting  { get; set; }
        public PrintLayoutSetting PrintLayoutSetting { get; set; }

        // ── 프린터 2 (보조 프린터 - 미사용 시 null 가능) ───────────
        public PrinterSetting     PrinterSetting2     { get; set; }
        public PrintPaperSetting  PrintPaperSetting2  { get; set; }
        public PrintLayoutSetting PrintLayoutSetting2 { get; set; }

        // ── 공통 옵션 ──────────────────────────────────────────────
        /// <summary>true: Windows 기본 프린터 사용 (PrinterName 무시)</summary>
        public bool UseDefaultPrinter { get; set; }

        // ── v3 출력 문구 ───────────────────────────────────────────
        /// <summary>영수증 하단 문구 (빈 문자열이면 미출력)</summary>
        public string ReceiptFooterText { get; set; } = "";

        /// <summary>티켓 한줄 알림 (서버 값 없을 때 로컬 기본값)</summary>
        public string TicketFooterText { get; set; } = "본 티켓은 이용 당일에만 유효합니다.";

        private static string filePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Settings", "applicationSetting.json");

        public void SaveToFile()
        {
            string dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            File.WriteAllText(filePath, JsonConvert.SerializeObject(this, Formatting.Indented), Encoding.UTF8);
        }

        private static ApplicationSetting LoadFromFile()
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<ApplicationSetting>(json);
            }
            var def = new ApplicationSetting();
            def.SaveToFile();
            return def;
        }

        private void SetDefault()
        {
            UseDefaultPrinter = true; // 기본값: Windows 기본 프린터 사용

            PrinterSetting = new PrinterSetting { PrinterKind = PrinterKind.General, PrinterName = "" };
            PrintPaperSetting  = new PrintPaperSetting  { Name = "default",  IsLandscape = false, PaperWidth = 80, PaperHeight = 0 };
            PrintLayoutSetting = new PrintLayoutSetting { LocationX = 0, LocationY = 0, FontSize = 9 };

            PrinterSetting2    = new PrinterSetting     { PrinterKind = PrinterKind.General, PrinterName = "" };
            PrintPaperSetting2  = new PrintPaperSetting { Name = "default2", IsLandscape = false, PaperWidth = 80, PaperHeight = 0 };
            PrintLayoutSetting2 = new PrintLayoutSetting { LocationX = 0, LocationY = 0, FontSize = 9 };
        }
    }

    public class PrinterSetting
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public PrinterKind PrinterKind { get; set; }
        /// <summary>Windows 프린터 이름 (UseDefaultPrinter=false일 때 사용)</summary>
        public string PrinterName { get; set; }
    }

    public class PrintPaperSetting
    {
        public string Name        { get; set; }
        public bool   IsLandscape { get; set; }
        /// <summary>용지 가로 (mm)</summary>
        public int PaperWidth  { get; set; }
        /// <summary>용지 세로 (mm, 0=자동)</summary>
        public int PaperHeight { get; set; }
    }

    public class PrintLayoutSetting
    {
        public int FontSize  { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public FontStyle FontStyle { get; set; }
        public int LocationX { get; set; }
        public int LocationY { get; set; }
    }
}
