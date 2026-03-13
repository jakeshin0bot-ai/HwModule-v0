using System;
using System.Drawing;
using System.Drawing.Printing;
using HwModule.Settings;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace HwModule.Devices.Printer
{
    /// <summary>
    /// 일반적인 프린터 장비 (Builder의 Director 역할)
    /// </summary>
    public class GeneralPrinter : IPrinter
    {
        private PrintDocument printDocument;

        public PrinterSetting PrinterSetting { get; }
        public PrintPaperSetting PaperSetting { get; }
        public PrintLayoutSetting LayoutSetting { get; }

        public GeneralPrinter(PrinterSetting printerSetting, PrintPaperSetting printPaperSetting, PrintLayoutSetting printLayoutSetting)
        {
            PrinterSetting = printerSetting;
            PaperSetting = printPaperSetting;
            LayoutSetting = printLayoutSetting;

            printDocument = new PrintDocument
            {
                PrinterSettings = new PrinterSettings { PrinterName = PrinterSetting.PrinterName }
            };
        }

        public void Dispose()
        {
            printDocument.Dispose();
        }
    }
}
