using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HwModule.Utils;

namespace HwModule.View.TicketTemplates
{
    /// <summary>
    /// TicketTemp.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class GoodsReceipt : TicketTemplate, INotifyPropertyChanged
    {

        // 캠핑장 정보
        public string CampName { get; set; }
        public string BusiNo { get; set; }
        public string MasterName { get; set; }
        public string Address { get; set; }
        public string Tel { get; set; }

        // 거래정보
        public string AuthDate { get; set; }
        public string AuthCode { get; set; }
        public string CardName { get; set; }
        public string CardQuota { get; set; }
        public string PayMethod { get; set; }
        public string MaskingNum { get; set; }

        // 이용 사이트 정보
        public List<Dictionary<string, string>> GoodsOrderDetailList { get; set; }
     

        // 상품정보

        // 이용금액 정보
        public string TotalAmt { get; set; }
        public string TotalPay { get; set; }
        public string Vat { get; set; }
        public string DcName { get; set; }
        public string TotalDcAmt { get; set; }
        public string EtcDcAmt { get; set; }
        public string OrderStatus { get; set; }

        public string DisplayTotalAmt 
        { 
            get
            {
                if(EtcDcAmt != null)
                {
                    return (int.Parse(TotalAmt) + int.Parse(EtcDcAmt)).ToString();
                }
                return TotalAmt;
            }
        }
        public string DisplayTotalPay
        {
            get
            {
                if (Vat != null)
                {
                    return (int.Parse(TotalPay) - int.Parse(Vat)).ToString();
                }
                return TotalPay;
            }
        }
        public string DisplayCardInfo
        {
            get
            {
                if(PayMethod == "CARD")
                {
                    if(CardQuota != "00")
                    {
                        return MaskingNum + "(" + CardName + "/" + CardQuota + "개월" +  ")";
                    }
                    else
                    {
                        return MaskingNum + "(" + CardName + "/일시불" + ")";
                    }
                }
                else
                {
                    return MaskingNum + "(" + CardName + ")";
                }
            }
        }


        public GoodsReceipt()
        {
            InitializeComponent();
            DataContext = this; // 데이터 컨텍스트 설정
            Init();
        }

        public override void SetTestData()
        {
            CampName = "송지호 오토 캠핑장 입장권";
            BusiNo = "220-01-00123";
            MasterName = "대표자명";
            Address = "강원특별자치도 춘천시 춘천순환로 609, 4";
            Tel = "070-1234-1234";

            AuthDate = "24.01.31 14:30:57";
            AuthCode = "2231132321";
            CardName = "삼성카드";
            CardQuota = "일시불";

            TotalAmt = "30000";
            TotalPay = "30000";
            Vat = "0";
            DcName = "";
            TotalDcAmt = "0";
            EtcDcAmt = "0";
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
