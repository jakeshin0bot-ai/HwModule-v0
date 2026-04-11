using System;
using System.Net;
using System.Threading;
using System.Text;
using System.IO;
using System.Windows.Forms;
using WinFormsApp = System.Windows.Forms.Application;
using NLog;
using HwModule.Properties;
using HwModule.Core;
using V3PrintData = HwModule.Core.V3PrintData;
using HwModule.Devices.Printer;
using HwModule.Settings;
using HwModule.Controller;
using HwModule.View;
using Newtonsoft.Json;

namespace NotifyIconApp
{
    static class Program
    {
        static HttpListener listener;
        static GeneralPrinter printer1;
        static GeneralPrinter printer2;
        static PrintController printController;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static PrinterSettingForm printerSettingFormInstance;

        [STAThread]
        static void Main()
        {
            WinFormsApp.EnableVisualStyles();
            WinFormsApp.SetCompatibleTextRenderingDefault(false);

            // WPF PrintDialog 사용을 위한 WPF Application 인스턴스 생성
            if (System.Windows.Application.Current == null)
            {
                new System.Windows.Application { ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown };
            }

            var settings = ApplicationSetting.Instance;

            printer1 = new GeneralPrinter(settings.PrinterSetting, settings.PrintPaperSetting, settings.PrintLayoutSetting);
            printer2 = new GeneralPrinter(settings.PrinterSetting2, settings.PrintPaperSetting2, settings.PrintLayoutSetting2);

            printController = new PrintController(printer1, printer2);

            string url = "http://localhost:7311/";
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            UIThreadHelper.Initialize();

            using (var noti = new NotifyIcon())
            {
                noti.Text = "HwModule v0";
                noti.Icon = Resources.test_icon;

                noti.ContextMenuStrip = new ContextMenuStrip();
                noti.ContextMenuStrip.Items.Add("설정", null, Setting_Clicked);
                noti.ContextMenuStrip.Items.Add("종료", null, Exit_Clicked);
                noti.DoubleClick += new EventHandler(Double_Clicked);
                noti.Visible = true;

                ThreadPool.QueueUserWorkItem(ListenForRequests);
                WinFormsApp.Run();
            }
        }

        private static void ListenForRequests(object state)
        {
            while (true)
            {
                try
                {
                    HttpListenerContext context = listener.GetContext();
                    ThreadPool.QueueUserWorkItem(HandleRequest, context);
                    logger.Info("Received request.");
                }
                catch (HttpListenerException ex)
                {
                    logger.Error(ex, "HttpListener stopped: {0}", ex.Message);
                    break;
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "ListenForRequests error: {0}", ex.Message);
                }
            }
        }

        private static void HandleRequest(object state)
        {
            HttpListenerContext context = null;
            try
            {
                context = (HttpListenerContext)state;
                HttpListenerRequest request = context.Request;

                string requestUrl = request.Url.AbsolutePath.ToLower();
                dynamic postData = null;
                string responseString = "";
                byte[] buffer = null;

                // OPTIONS preflight 처리 (CORS)
                if (request.HttpMethod == "OPTIONS")
                {
                    buffer = Encoding.UTF8.GetBytes("");
                }
                else if (request.HttpMethod == "POST")
                {
                    using (StreamReader reader = new StreamReader(request.InputStream, Encoding.UTF8))
                    {
                        string jsonData = reader.ReadToEnd();
                        logger.Debug("Request [{0}] body: {1}", requestUrl, jsonData);
                        postData = JsonConvert.DeserializeObject(jsonData);
                    }

                    switch (requestUrl)
                    {
                        // ── v2 엔드포인트 ──────────────────────────────────
                        case "/ticketprint":
                            responseString = requestUrl + " success!";
                            buffer = Encoding.UTF8.GetBytes(responseString);
                            UIThreadHelper.RunOnUIThread(() => printController.PrintTicket(postData));
                            break;

                        case "/seatlistprint":
                            responseString = requestUrl + " success!";
                            buffer = Encoding.UTF8.GetBytes(responseString);
                            UIThreadHelper.RunOnUIThread(() => printController.PrintSeat(postData));
                            break;

                        case "/addpayprint":
                            responseString = requestUrl + " success!";
                            buffer = Encoding.UTF8.GetBytes(responseString);
                            UIThreadHelper.RunOnUIThread(() => printController.PrintAddPay(postData));
                            break;

                        case "/ticketprinttest":
                            responseString = requestUrl + " success!";
                            buffer = Encoding.UTF8.GetBytes(responseString);
                            UIThreadHelper.RunOnUIThread(() => printController.PrintTest1(""));
                            break;

                        // ── v3 엔드포인트 (통합, /print /receipt /ticket) ──
                        case "/print":
                        case "/receipt":
                        case "/ticket":
                        {
                            V3PrintData v3data = JsonConvert.DeserializeObject<V3PrintData>(
                                JsonConvert.SerializeObject(postData));

                            // 엔드포인트로 직접 접근 시 printType 강제 설정
                            if (requestUrl == "/receipt") v3data.PrintType = "receipt";
                            if (requestUrl == "/ticket")  v3data.PrintType = "ticket";

                            responseString = requestUrl + " success!";
                            buffer = Encoding.UTF8.GetBytes(responseString);
                            UIThreadHelper.RunOnUIThread(() => printController.PrintV3(v3data));
                            break;
                        }

                        // ── v1 엔드포인트 (캠핑/현장구매 영수증) ──────────
                        case "/goodsreceiptprint":
                            responseString = requestUrl + " success!";
                            buffer = Encoding.UTF8.GetBytes(responseString);
                            UIThreadHelper.RunOnUIThread(() => printController.PrintGoodsReceipt(postData));
                            break;

                        case "/goodsreceiptprinttest":
                            responseString = requestUrl + " success!";
                            buffer = Encoding.UTF8.GetBytes(responseString);
                            UIThreadHelper.RunOnUIThread(() => printController.PrintGoodsReceiptTest());
                            break;

                        // ── 공통 ──────────────────────────────────────────
                        case "/ping":
                            responseString = "isConnected";
                            buffer = Encoding.UTF8.GetBytes(responseString);
                            break;

                        default:
                            responseString = requestUrl + " fail!";
                            buffer = Encoding.UTF8.GetBytes(responseString);
                            logger.Warn("Unknown endpoint: {0}", requestUrl);
                            break;
                    }
                }
                else if (request.HttpMethod == "GET")
                {
                    switch (requestUrl)
                    {
                        case "/printerlist":
                        {
                            // 설치된 프린터 목록 반환
                            var printers = new Newtonsoft.Json.Linq.JArray();
                            foreach (string pName in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                            {
                                printers.Add(pName);
                            }
                            var resp = new Newtonsoft.Json.Linq.JObject();
                            resp["result"] = "SUCCESS";
                            resp["printerList"] = printers;
                            // 현재 설정된 프린터도 포함
                            var s = ApplicationSetting.Instance;
                            resp["printer1"] = s.PrinterSetting?.PrinterName ?? "";
                            resp["printer2"] = s.PrinterSetting2?.PrinterName ?? "";
                            resp["useDefaultPrinter"] = s.UseDefaultPrinter;
                            responseString = resp.ToString(Newtonsoft.Json.Formatting.None);
                            break;
                        }
                        case "/ping":
                            responseString = "isConnected";
                            break;
                        default:
                            responseString = requestUrl + " not found";
                            break;
                    }
                    buffer = Encoding.UTF8.GetBytes(responseString);
                }
                else if (request.HttpMethod == "POST" && requestUrl == "/printersetting")
                {
                    // 프린터 설정 저장
                    using (StreamReader reader = new StreamReader(request.InputStream, Encoding.UTF8))
                    {
                        string jsonData = reader.ReadToEnd();
                        dynamic settingData = JsonConvert.DeserializeObject(jsonData);
                        var s = ApplicationSetting.Instance;
                        if (settingData.printer1 != null)
                            s.PrinterSetting.PrinterName = Convert.ToString(settingData.printer1);
                        if (settingData.printer2 != null)
                            s.PrinterSetting2.PrinterName = Convert.ToString(settingData.printer2);
                        if (settingData.useDefaultPrinter != null)
                            s.UseDefaultPrinter = Convert.ToBoolean(settingData.useDefaultPrinter);
                        s.SaveToFile();
                        // printer 객체 갱신
                        UIThreadHelper.RunOnUIThread(() => {
                            printer1 = new HwModule.Devices.Printer.GeneralPrinter(s.PrinterSetting, s.PrintPaperSetting, s.PrintLayoutSetting);
                            printer2 = new HwModule.Devices.Printer.GeneralPrinter(s.PrinterSetting2, s.PrintPaperSetting2, s.PrintLayoutSetting2);
                            printController = new PrintController(printer1, printer2);
                        });
                        responseString = "{\"result\":\"SUCCESS\"}";
                    }
                    buffer = Encoding.UTF8.GetBytes(responseString);
                }
                else
                {
                    responseString = "method not allowed";
                    buffer = Encoding.UTF8.GetBytes(responseString);
                }

                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
                context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
                context.Response.Headers.Add("Access-Control-Allow-Credentials", "false");
                context.Response.ContentLength64 = buffer.Length;
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.OutputStream.Close();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "HandleRequest error: {0}", ex.Message);
                try
                {
                    if (context != null)
                    {
                        byte[] errBuf = Encoding.UTF8.GetBytes("error");
                        context.Response.ContentLength64 = errBuf.Length;
                        context.Response.OutputStream.Write(errBuf, 0, errBuf.Length);
                        context.Response.OutputStream.Close();
                    }
                }
                catch { }
            }
        }

        private static void Double_Clicked(object sender, EventArgs e) => OpenPrinterSettingForm();
        private static void Setting_Clicked(object sender, EventArgs e) => OpenPrinterSettingForm();
        private static void Exit_Clicked(object sender, EventArgs e) => WinFormsApp.Exit();

        private static void OpenPrinterSettingForm()
        {
            if (printerSettingFormInstance == null || printerSettingFormInstance.IsDisposed)
            {
                printerSettingFormInstance = new PrinterSettingForm(printController);
                printerSettingFormInstance.FormClosed += (s, a) => printerSettingFormInstance = null;
                printerSettingFormInstance.ShowDialog();
            }
            else
            {
                printerSettingFormInstance.Activate();
            }
        }
    }

    public static class UIThreadHelper
    {
        private static SynchronizationContext synchronizationContext;

        public static void Initialize()
        {
            synchronizationContext = SynchronizationContext.Current ?? new WindowsFormsSynchronizationContext();
        }

        public static void RunOnUIThread(Action action)
        {
            synchronizationContext.Post(_ => action.Invoke(), null);
        }
    }
}
