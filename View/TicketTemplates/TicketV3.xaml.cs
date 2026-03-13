using HwModule.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;

namespace HwModule.View.TicketTemplates
{
    /// <summary>
    /// TicketV3 — 가로형 이용권 티켓 (v3 전용)
    /// 용지: 148mm x 100mm (A6 가로), printer2 출력
    /// </summary>
    public partial class TicketV3 : TicketTemplate, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // ── 시설 정보 ──────────────────────────────────────────────
        public string FacilityName { get; set; }

        // ── 예약 정보 ──────────────────────────────────────────────
        public string OrderNum     { get; set; }
        public string GuestName    { get; set; }
        public string GuestPhone   { get; set; }
        public string CarNum       { get; set; } = "";
        public string AreaName     { get; set; }

        /// <summary>좌석 목록 줄 표시 (예: 분비나무(102호), 물푸레나무(108호))</summary>
        public string RoomListText { get; set; }

        /// <summary>이용기간 표시 (예: 2026.03.13(목) ~ 03.15(토))</summary>
        public string UsePeriod    { get; set; }

        // ── 한줄 알림 ──────────────────────────────────────────────
        public string TicketFooter { get; set; } = "";

        // ── 발권일시 ───────────────────────────────────────────────
        public string PrintedAtText { get; set; }

        // ── QR코드 ─────────────────────────────────────────────────
        public ImageSource QrCodeSource { get; private set; }

        public TicketV3()
        {
            InitializeComponent();
            Init();
        }

        /// <summary>예약 데이터를 세팅하고 QR 생성</summary>
        public void SetData(V3PrintData data)
        {
            var f  = data.Facility    ?? new V3Facility();
            var r  = data.Reservation ?? new V3Reservation();
            var items = data.Items    ?? new List<V3Item>();

            FacilityName  = f.Name    ?? "";
            TicketFooter  = f.TicketFooter ?? "";

            OrderNum      = r.OrderNum   ?? "";
            GuestName     = r.GuestName  ?? "";
            GuestPhone    = r.GuestPhone ?? "";
            CarNum        = r.CarNum     ?? "";
            AreaName      = r.AreaName   ?? "";

            // 좌석 목록: 구역 + 좌석명 조합
            var roomLines = items
                .Select(i => string.IsNullOrWhiteSpace(i.AreaName)
                    ? i.RoomName
                    : i.RoomName)
                .Distinct()
                .ToList();
            RoomListText = roomLines.Count > 0
                ? string.Join(", ", roomLines)
                : AreaName;

            // 이용 기간
            var dates = items.Select(i => i.UseDate).Where(d => !string.IsNullOrEmpty(d)).Distinct().OrderBy(d => d).ToList();
            if (dates.Count == 0 && !string.IsNullOrEmpty(r.UseDate))
                dates = new List<string> { r.UseDate };

            UsePeriod = BuildUsePeriod(dates);

            // 발권일시
            var printedAt = DateTimeOffset.FromUnixTimeMilliseconds(data.PrintedAt).ToLocalTime();
            PrintedAtText = printedAt.ToString("yyyy.MM.dd HH:mm");

            // QR 코드 생성
            QrCodeSource = GenerateQr(OrderNum);

            this.DataContext = this;
        }

        /// <summary>
        /// ZXing.Net으로 QR코드 BitmapSource 생성
        /// </summary>
        private static ImageSource GenerateQr(string text)
        {
            try
            {
                var writer = new BarcodeWriterPixelData
                {
                    Format = BarcodeFormat.QR_CODE,
                    Options = new QrCodeEncodingOptions
                    {
                        Width       = 220,
                        Height      = 220,
                        Margin      = 1,
                        ErrorCorrection = ZXing.QrCode.Internal.ErrorCorrectionLevel.M
                    }
                };
                var pixelData = writer.Write(text);
                var bitmap    = new WriteableBitmap(pixelData.Width, pixelData.Height, 96, 96, PixelFormats.Bgra32, null);
                bitmap.WritePixels(
                    new Int32Rect(0, 0, pixelData.Width, pixelData.Height),
                    pixelData.Pixels, pixelData.Width * 4, 0);
                bitmap.Freeze();
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>날짜 목록을 "2026.03.13(목) ~ 03.15(토)" 형식으로 변환</summary>
        private static string BuildUsePeriod(List<string> dates)
        {
            if (dates.Count == 0) return "-";
            string[] dayNames = { "일", "월", "화", "수", "목", "금", "토" };

            string Format(string d)
            {
                if (d.Length < 8) return d;
                if (!DateTime.TryParseExact(d, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var dt))
                    return d;
                return $"{dt.Year}.{dt.Month:D2}.{dt.Day:D2}({dayNames[(int)dt.DayOfWeek]})";
            }

            if (dates.Count == 1) return Format(dates[0]);

            // 마지막 날짜는 체크아웃일이면 월/일만
            var last = dates.Last();
            string lastFmt;
            if (DateTime.TryParseExact(last, "yyyyMMdd", null,
                System.Globalization.DateTimeStyles.None, out var lastDt))
                lastFmt = $"{lastDt.Month:D2}.{lastDt.Day:D2}({dayNames[(int)lastDt.DayOfWeek]})";
            else
                lastFmt = last;

            return $"{Format(dates.First())} ~ {lastFmt}";
        }

        public override void SetTestData()
        {
            FacilityName  = "백두대간생태수목원";
            OrderNum      = "KS20260313001";
            GuestName     = "홍 길 동";
            GuestPhone    = "010-1234-****";
            CarNum        = "12가 3456";
            AreaName      = "체험센터(4인실)";
            RoomListText  = "분비나무(102호), 물푸레나무(108호)";
            UsePeriod     = "2026.03.13(목) ~ 03.15(토)";
            TicketFooter  = "본 티켓은 이용 당일에만 유효합니다.";
            PrintedAtText = "2026.03.13 23:15";
            QrCodeSource  = GenerateQr("KS20260313001");
            this.DataContext = this;
        }
    }
}
