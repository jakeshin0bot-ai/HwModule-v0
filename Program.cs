using System;
using System.Net;
using System.Threading;
using System.Text;
using System.IO;
using System.Windows.Forms;
using NLog;
using HwModule.Properties;
using HwModule.Core;
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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

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
                Application.Run();
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

                if (request.HttpMethod == "POST")
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
                else
                {
                    responseString = "method not allowed";
                    buffer = Encoding.UTF8.GetBytes(responseString);
                }

                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
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
        private static void Exit_Clicked(object sender, EventArgs e) => Application.Exit();

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
