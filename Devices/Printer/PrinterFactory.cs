using HwModule.Core;
using HwModule.Settings;

namespace HwModule.Devices.Printer
{
    public class PrinterFactory : IFactory<IPrinter>
    {
        public IPrinter Create(params object[] args)
        {
            var printerSetting = args[0] as PrinterSetting;
            var printPaperSetting = args[1] as PrintPaperSetting;
            var printLayoutSetting = args[2] as PrintLayoutSetting;

            if (printerSetting.PrinterKind == PrinterKind.General)
                return new GeneralPrinter(printerSetting, printPaperSetting, printLayoutSetting);
            else
                throw new FactoryException("프린터 장비 생성중 오류 발생");
        }
    }
}
