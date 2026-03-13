using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Printing;
using System.Drawing.Printing;
using HwModule.Core;
using HwModule.Devices.Printer;
using HwModule.Settings;
using HwModule.View.TicketTemplates;
using Newtonsoft.Json;
using NLog;

namespace HwModule.Controller
{
    /// <summary>
    /// HwModule v0 PrintController
    /// - v2 기반: PrintTicket / PrintSeat / PrintAddPay
    /// - v1 추가: PrintGoodsReceipt (campInfo 구조, 현장구매 영수증)
    /// </summary>
    public class PrintController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private GeneralPrinter printer1;
        private GeneralPrinter printer2;

        public PrintController(GeneralPrinter printer1, GeneralPrinter printer2)
        {
            this.printer1 = printer1;
            this.printer2 = printer2;
        }

        // ── 공통: PrintDialog 생성 (설정에 따라 기본/지정 프린터) ─────────
        private System.Windows.Controls.PrintDialog CreatePrintDialog(GeneralPrinter printer)
        {
            var pd = new System.Windows.Controls.PrintDialog();
            pd.PrintTicket.PageOrientation =
                printer.PaperSetting.IsLandscape ? PageOrientation.Landscape : PageOrientation.Portrait;

            var settings = ApplicationSetting.Instance;
            if (!settings.UseDefaultPrinter && !string.IsNullOrWhiteSpace(printer.PrinterSetting.PrinterName))
            {
                try
                {
                    pd.PrintQueue = new PrintQueue(new PrintServer(), printer.PrinterSetting.PrinterName);
                    logger.Info("프린터 지정: {0}", printer.PrinterSetting.PrinterName);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "지정 프린터 오류 — 기본 프린터로 대체: {0}", ex.Message);
                }
            }
            else
            {
                logger.Info("Windows 기본 프린터 사용");
            }
            return pd;
        }

        // ── v1: 현장구매 영수증 (GoodsReceipt, campInfo 구조) ─────────────
        public void PrintGoodsReceipt(dynamic jsonData)
        {
            logger.Info("PrintGoodsReceipt 시작");
            var template = new GoodsReceipt();
            var pd = CreatePrintDialog(printer1);

            template.CampName    = Convert.ToString(jsonData.campInfo.campName);
            template.BusiNo      = Convert.ToString(jsonData.campInfo.busiNo);
            template.MasterName  = Convert.ToString(jsonData.campInfo.masterName);
            template.Address     = Convert.ToString(jsonData.campInfo.address);
            template.Tel         = Convert.ToString(jsonData.campInfo.tel);

            template.AuthDate    = Convert.ToString(jsonData.printParams.AUTHDATE);
            template.AuthCode    = Convert.ToString(jsonData.printParams.AUTHCODE);
            template.PayMethod   = Convert.ToString(jsonData.printParams.PAYMETHOD);
            template.CardName    = Convert.ToString(jsonData.printParams.CARDNAME);
            template.CardQuota   = Convert.ToString(jsonData.printParams.CARDQUOTA ?? "00");
            template.MaskingNum  = Convert.ToString(jsonData.printParams.MASKINGNUM);
            template.Vat         = Convert.ToString(jsonData.printParams.VAT      ?? "0");
            template.TotalAmt    = Convert.ToString(jsonData.printParams.TOTAL_AMT ?? "0");
            template.TotalPay    = Convert.ToString(jsonData.printParams.TOTAL_PAY ?? "0");
            template.TotalDcAmt  = Convert.ToString(jsonData.printParams.DC_AMT    ?? "0");
            template.EtcDcAmt    = Convert.ToString(jsonData.printParams.ETC_DC_AMT ?? "0");
            template.OrderStatus = Convert.ToString(jsonData.printParams.ORDER_STATUS ?? "1");

            template.GoodsOrderDetailList = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(
                Convert.ToString(jsonData.printParams.goodsOrderDetailList));

            template.SetMargin(printer1.LayoutSetting.LocationX, printer1.LayoutSetting.LocationY);
            template.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            template.Arrange(new Rect(new Point(0, 0), template.DesiredSize));
            template.UpdateLayout();

            IDocumentPaginatorSource src = template.Doc;
            pd.PrintDocument(src.DocumentPaginator, "현장구매영수증");
            logger.Info("PrintGoodsReceipt 완료");
        }

        public void PrintGoodsReceiptTest()
        {
            logger.Info("PrintGoodsReceiptTest 시작");
            var template = new GoodsReceipt();
            var pd = CreatePrintDialog(printer1);

            template.CampName   = "송지호 오토 캠핑장";
            template.BusiNo     = "220-01-00123";
            template.MasterName = "대표자명";
            template.Address    = "강원특별자치도 고성군";
            template.Tel        = "033-1234-5678";
            template.AuthDate   = "20260313153000";
            template.AuthCode   = "12345678";
            template.PayMethod  = "CARD";
            template.CardName   = "삼성카드";
            template.CardQuota  = "00";
            template.MaskingNum = "4124-23**-****-1234";
            template.Vat        = "0";
            template.TotalAmt   = "30000";
            template.TotalPay   = "30000";
            template.TotalDcAmt = "0";
            template.EtcDcAmt   = "0";
            template.OrderStatus = "1";
            template.GoodsOrderDetailList = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string> { ["GOODS_NAME"]="A구역", ["TOTAL_CNT"]="1", ["TOTAL_PAY"]="30000" }
            };

            template.SetMargin(printer1.LayoutSetting.LocationX, printer1.LayoutSetting.LocationY);
            template.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            template.Arrange(new Rect(new Point(0, 0), template.DesiredSize));
            template.UpdateLayout();

            IDocumentPaginatorSource src = template.Doc;
            pd.PrintDocument(src.DocumentPaginator, "영수증테스트");
            logger.Info("PrintGoodsReceiptTest 완료");
        }

        // ── v2: 티켓 출력 (TicketTemp, franInfo 구조) ─────────────────────
        public void PrintTicket(dynamic jsonData)
        {
            logger.Info("PrintTicket 시작");
            var template = new TicketTemp();
            var pd = CreatePrintDialog(printer1);

            template.FranName    = Convert.ToString(jsonData.franInfo.franName);
            template.BusiNum     = Convert.ToString(jsonData.franInfo.busiNum);
            template.MastName    = Convert.ToString(jsonData.franInfo.mastName);
            template.Address     = Convert.ToString(jsonData.franInfo.address);
            template.MastTel     = Convert.ToString(jsonData.franInfo.mastTel);
            template.ReceiptCont = Convert.ToString(jsonData.franInfo.receiptCont ?? "");

            template.AuthDate    = Convert.ToString(jsonData.printParams.AUTHDATE);
            template.AuthCode    = Convert.ToString(jsonData.printParams.AUTHCODE);
            template.PayMethod   = Convert.ToString(jsonData.printParams.PAYMETHOD);
            template.CardName    = Convert.ToString(jsonData.printParams.CARDNAME);
            template.CardQuota   = Convert.ToString(jsonData.printParams.CARDQUOTA ?? "00");
            template.MaskingNum  = Convert.ToString(jsonData.printParams.MASKINGNUM);
            template.Vat         = Convert.ToString(jsonData.printParams.VAT ?? "0");

            long ts = long.Parse(Convert.ToString(jsonData.printParams.PRINT_DATE));
            DateTime kst = TimeZoneInfo.ConvertTimeFromUtc(
                DateTimeOffset.FromUnixTimeMilliseconds(ts).UtcDateTime,
                TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time"));
            template.PrintDate = kst.ToString("yyyy.MM.dd HH:mm:ss");

            template.OrderNum    = Convert.ToString(jsonData.printParams.ORDER_NUM);
            template.TicketName  = Convert.ToString(jsonData.printParams.TICKET_NAME);
            template.TotalCnt    = Convert.ToString(jsonData.printParams.TOTAL_CNT    ?? "0");
            template.TotalAmt    = Convert.ToString(jsonData.printParams.TOTAL_AMT    ?? "0");
            template.TotalPay    = Convert.ToString(jsonData.printParams.TOTAL_PAY    ?? "0");
            template.TotalCancCnt = Convert.ToString(jsonData.printParams.TOTAL_CANC_CNT ?? "0");
            template.TotalCancAmt = Convert.ToString(jsonData.printParams.TOTAL_CANC_AMT ?? "0");
            template.TotalSaleCnt = Convert.ToString(jsonData.printParams.TOTAL_SALE_CNT ?? "0");
            template.TotalSaleAmt = Convert.ToString(jsonData.printParams.TOTAL_SALE_AMT ?? "0");
            template.TotalPrevAmt = Convert.ToString(jsonData.printParams.TOTAL_PREV_AMT ?? "0");
            template.OrderState  = Convert.ToString(jsonData.printParams.ORDER_STATE ?? "1");
            template.DisplaySubCheck      = Convert.ToString(jsonData.printParams.SUB_TITLE_CHECK ?? "N");
            template.BeforeTotalCancAmt   = Convert.ToString(jsonData.printParams.BEFORE_TOTAL_CANC_AMT ?? "0");
            template.PrevCancAmt          = Convert.ToString(jsonData.printParams.PREV_CANC_AMT ?? "0");

            template.OrderDetailList = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(
                Convert.ToString(jsonData.printParams.orderDetailList));

            template.SetMargin(printer1.LayoutSetting.LocationX, printer1.LayoutSetting.LocationY);
            template.Measure(new Size((double)printer1.PaperSetting.PaperWidth, (double)printer1.PaperSetting.PaperHeight));
            template.Arrange(new Rect(new Point(0, 0), template.DesiredSize));
            template.UpdateLayout();

            IDocumentPaginatorSource src = template.Doc;
            pd.PrintDocument(src.DocumentPaginator, "티켓출력");
            logger.Info("PrintTicket 완료");
        }

        // ── v2: 좌석 출력 ──────────────────────────────────────────────────
        public void PrintSeat(dynamic jsonData)
        {
            logger.Info("PrintSeat 시작");
            var template = new SeatTemp();
            var pd = CreatePrintDialog(printer1);

            template.ReceiptCont   = Convert.ToString(jsonData.franInfo.receiptCont ?? "");
            template.PlaceName     = Convert.ToString(jsonData.printParams.PLACE_NAME);
            template.TicketName    = Convert.ToString(jsonData.printParams.TICKET_NAME);
            template.OrgTicketName = Convert.ToString(jsonData.printParams.ORG_TICKET_NAME);
            template.TotalCnt      = Convert.ToString(jsonData.printParams.TOTAL_CNT ?? "0");
            template.TotalCancCnt  = Convert.ToString(jsonData.printParams.TOTAL_CANC_CNT ?? "0");
            template.TurnTime      = Convert.ToString(jsonData.printParams.TURN_TIME);
            template.MoveWayName   = Convert.ToString(jsonData.printParams.MOVE_WAY_NM);
            template.MoveMethodName = Convert.ToString(jsonData.printParams.MOVE_METHOD_NM);
            template.TypeName      = Convert.ToString(jsonData.printParams.TYPE_NAME);
            template.UseDate       = Convert.ToString(jsonData.printParams.USE_DATE);

            template.OrderSeatList = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(
                Convert.ToString(jsonData.printParams.orderSeatList));

            template.SetMargin(printer1.LayoutSetting.LocationX, printer1.LayoutSetting.LocationY);
            template.Measure(new Size((double)printer1.PaperSetting.PaperWidth, (double)printer1.PaperSetting.PaperHeight));
            template.Arrange(new Rect(new Point(0, 0), template.DesiredSize));
            template.UpdateLayout();

            IDocumentPaginatorSource src = template.Doc;
            pd.PrintDocument(src.DocumentPaginator, "좌석출력");
            logger.Info("PrintSeat 완료");
        }

        // ── v2: 추가결제 출력 ──────────────────────────────────────────────
        public void PrintAddPay(dynamic jsonData)
        {
            logger.Info("PrintAddPay 시작");
            var template = new AddPayTemp();
            var pd = CreatePrintDialog(printer1);

            template.FranName    = Convert.ToString(jsonData.franInfo.franName);
            template.BusiNum     = Convert.ToString(jsonData.franInfo.busiNum);
            template.MastName    = Convert.ToString(jsonData.franInfo.mastName);
            template.Address     = Convert.ToString(jsonData.franInfo.address);
            template.MastTel     = Convert.ToString(jsonData.franInfo.mastTel);
            template.ReceiptCont = Convert.ToString(jsonData.franInfo.receiptCont ?? "");

            template.AuthDate   = Convert.ToString(jsonData.printParams.AUTHDATE);
            template.AuthCode   = Convert.ToString(jsonData.printParams.AUTHCODE);
            template.PayMethod  = Convert.ToString(jsonData.printParams.PAYMETHOD);
            template.CardName   = Convert.ToString(jsonData.printParams.CARDNAME);
            template.CardQuota  = Convert.ToString(jsonData.printParams.CARDQUOTA ?? "00");
            template.MaskingNum = Convert.ToString(jsonData.printParams.MASKINGNUM);
            template.Amt        = Convert.ToString(jsonData.printParams.AMT ?? "0");
            template.Vat        = Convert.ToString(jsonData.printParams.VAT ?? "0");
            template.PayAmt     = Convert.ToString(jsonData.printParams.PAY_AMT ?? "0");
            template.CancAmt    = Convert.ToString(jsonData.printParams.CANC_AMT ?? "0");
            template.OrderNum   = Convert.ToString(jsonData.printParams.ORDER_NUM);
            template.TicketName = Convert.ToString(jsonData.printParams.TICKET_NAME);
            template.OrderState = Convert.ToString(jsonData.printParams.ORDER_STATE ?? "1");
            template.DisplaySubCheck = Convert.ToString(jsonData.printParams.SUB_TITLE_CHECK ?? "N");

            template.OrderAddDetailList = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(
                Convert.ToString(jsonData.printParams.orderAddDetailList));

            template.SetMargin(printer1.LayoutSetting.LocationX, printer1.LayoutSetting.LocationY);
            template.Measure(new Size((double)printer1.PaperSetting.PaperWidth, (double)printer1.PaperSetting.PaperHeight));
            template.Arrange(new Rect(new Point(0, 0), template.DesiredSize));
            template.UpdateLayout();

            IDocumentPaginatorSource src = template.Doc;
            pd.PrintDocument(src.DocumentPaginator, "추가결제출력");
            logger.Info("PrintAddPay 완료");
        }

        // ── v3: 통합 분기 (/print 엔드포인트) ─────────────────────────────
        public void PrintV3(V3PrintData data)
        {
            logger.Info("PrintV3 시작 printType={0}", data.PrintType);
            switch ((data.PrintType ?? "receipt").ToLower())
            {
                case "ticket":
                    PrintTicketV3(data);
                    break;
                case "both":
                    PrintReceiptV3(data);
                    PrintTicketV3(data);
                    break;
                case "receipt":
                default:
                    PrintReceiptV3(data);
                    break;
            }
        }

        // ── v3: 영수증 출력 (printer1, GoodsReceipt 재활용) ───────────────
        public void PrintReceiptV3(V3PrintData data)
        {
            logger.Info("PrintReceiptV3 시작");
            var settings = ApplicationSetting.Instance;
            var template = new GoodsReceipt();
            var pd = CreatePrintDialog(printer1);

            var f = data.Facility    ?? new V3Facility();
            var r = data.Reservation ?? new V3Reservation();
            var p = data.Payment     ?? new V3Payment();

            template.CampName    = f.Name    ?? "";
            template.BusiNo      = f.BusiNo  ?? "";
            template.MasterName  = f.CeoName ?? "";
            template.Address     = f.Address ?? "";
            template.Tel         = f.Tel     ?? "";

            // 결제 정보
            string authDate14 = (p.AuthDate ?? "").Replace("-", "").Replace(":", "").Replace(" ", "");
            template.AuthDate   = authDate14.Length >= 14 ? authDate14.Substring(0, 14) : authDate14;
            template.AuthCode   = p.AuthCode   ?? "";
            template.PayMethod  = p.Method     ?? "";
            template.CardName   = p.CardName   ?? "";
            template.CardQuota  = p.Quota      ?? "00";
            template.MaskingNum = p.MaskingNum ?? "";
            template.Vat        = "0";
            template.TotalAmt   = p.TotalAmt.ToString();
            template.TotalPay   = p.TotalPay.ToString();
            template.TotalDcAmt = p.DcAmt.ToString();
            template.EtcDcAmt   = "0";
            template.OrderStatus = "1";

            // 상세 목록
            template.GoodsOrderDetailList = (data.Items ?? new List<V3Item>()).Select(i =>
                new Dictionary<string, string>
                {
                    ["GOODS_NAME"] = i.RoomName ?? "",
                    ["TOTAL_CNT"]  = i.Qty.ToString(),
                    ["TOTAL_PAY"]  = i.Price.ToString()
                }).ToList();

            template.SetMargin(printer1.LayoutSetting.LocationX, printer1.LayoutSetting.LocationY);
            template.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            template.Arrange(new Rect(new Point(0, 0), template.DesiredSize));
            template.UpdateLayout();

            IDocumentPaginatorSource src = template.Doc;
            pd.PrintDocument(src.DocumentPaginator, "영수증(v3)");
            logger.Info("PrintReceiptV3 완료");
        }

        // ── v3: 티켓 출력 (printer2, TicketV3, 가로형) ────────────────────
        public void PrintTicketV3(V3PrintData data)
        {
            logger.Info("PrintTicketV3 시작");
            var settings = ApplicationSetting.Instance;

            // ticketFooter: JSON 우선, 없으면 로컬 설정값
            if (data.Facility != null && string.IsNullOrWhiteSpace(data.Facility.TicketFooter))
                data.Facility.TicketFooter = settings.TicketFooterText;

            // 출력 시점 타임스탬프 채우기
            if (data.PrintedAt == 0)
                data.PrintedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var template = new TicketV3();
            template.SetData(data);

            // 가로형 출력 설정
            var pd = CreatePrintDialog(printer2);
            pd.PrintTicket.PageOrientation = System.Printing.PageOrientation.Landscape;

            template.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            template.Arrange(new Rect(new Point(0, 0), template.DesiredSize));
            template.UpdateLayout();

            IDocumentPaginatorSource src = template.Doc;
            pd.PrintDocument(src.DocumentPaginator, "이용권(v3)");
            logger.Info("PrintTicketV3 완료");
        }

        // ── v2: 테스트 출력 ────────────────────────────────────────────────
        public void PrintTest1(string str)
        {
            logger.Info("PrintTest1 시작");
            var template = new TicketTemp();
            var pd = CreatePrintDialog(printer1);

            template.OrderNum   = "0000123456";
            template.TicketName = "테스트 티켓 1회차 15:30~16:30";
            template.PrintDate  = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");
            template.FranName   = "임팩시스 테스트";
            template.BusiNum    = "000-00-00000";
            template.MastName   = "대표자명";
            template.Address    = "강원특별자치도 춘천시";
            template.MastTel    = "033-0000-0000";
            template.ReceiptCont = "■ 테스트 출력입니다.";
            template.AuthDate   = "20260101120000";
            template.AuthCode   = "12345678";
            template.CardName   = "삼성카드";
            template.CardQuota  = "00";
            template.TotalCnt   = "1";
            template.TotalAmt   = "10000";
            template.TotalPay   = "10000";
            template.Vat        = "0";
            template.OrderState = "1";
            template.TotalCancAmt = "0";
            template.TotalCancCnt = "0";
            template.TotalPrevAmt = "0";
            template.BeforeTotalCancAmt = "0";
            template.PrevCancAmt = "0";
            template.DisplaySubCheck = "N";

            template.SetMargin(printer1.LayoutSetting.LocationX, printer1.LayoutSetting.LocationY);
            template.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            template.Arrange(new Rect(new Point(0, 0), template.DesiredSize));
            template.UpdateLayout();

            IDocumentPaginatorSource src = template.Doc;
            pd.PrintDocument(src.DocumentPaginator, "티켓테스트");
            logger.Info("PrintTest1 완료");
        }
    }
}
