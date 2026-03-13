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
    public partial class TicketTemp : TicketTemplate, INotifyPropertyChanged
    {
        // 입점정보
        public string FranName { get; set; }
        public string MastName { get; set; }
        public string MastTel { get; set; }
        public string BusiNum { get; set; }
        public string Address { get; set; }
        public string ReceiptCont { get; set; }

        // 거래정보
        public string AuthDate { get; set; }
        public string AuthCode { get; set; }
        public string CardName { get; set; }
        public string CardQuota { get; set; }
        public string PayMethod { get; set; }
        public string MaskingNum { get; set; }
        public string PrintDate { get; set; }

        // 이용 사이트 정보
        public List<Dictionary<string, string>> OrderDetailList { get; set; }

        // 이용금액 정보
        public string OrderNum { get; set; }
        public string TicketName { get; set; }
        public string TotalCnt { get; set; }
        public string TotalAmt { get; set; }
        public string TotalPayCnt { get; set; }
        public string TotalPay { get; set; }
        public string TotalSaleCnt { get; set; }
        public string TotalSaleAmt { get; set; }
        public string TotalCancCnt { get; set; }
        public string TotalCancAmt { get; set; }
        public string TotalAddAmt { get; set; }
        public string TotalPrevAmt { get; set; }
        // orderCnacelInfo canc_amt
        public string BeforeTotalCancAmt { get; set; }
        // 온라인 취소 시 직전 부분취소금액 
        public string PrevCancAmt { get; set; }
        public string Vat { get; set; }
        public string OrderState { get; set; }

        public string DisplayTitle
        {
            get
            {
                if(PayMethod == "CARD")
                {
                    if (OrderState == "1")
                    {
                        return "카드승인";
                    }
                    else
                    {
                        return "카드취소";
                    }
                } else 
                {
                    if(OrderState == "1")
                    {
                        return "현금승인";
                    } else
                    {
                        return "현금취소";
                    }
                }
            }
        }

        public string DisplaySubCheck { get; set; } 
        public string DisplaySubTitle
        {
            get
            {
                if(DisplaySubCheck == "Y")
                {
                    return "-고객용-";
                } else
                {
                    return "-보관용-";
                }
            }
        }

        public string DisplayPayKind
        {
            get
            {
                if(OrderState == "1")
                {
                    return "거래금액";
                } else if(OrderState == "2")
                {
                    return "취소금액";
                } else
                {
                    return "";
                }
            }
        }
        
        public string DisplayPayKindAmt
        {
            get
            {
                if (OrderState == "1")
                {
                    return DisplayTotalPay;
                }
                else if (OrderState == "2")
                {
                    return BeforeTotalCancAmt;
                }
                else
                {
                    return "";
                }
            }
        }

        public string DisplayTotalCancAmt
        {
            get
            {
                if(OrderState == "1")
                {
                    return TotalCancAmt;
                } else 
                {
                    return "0";
                }
            }
        }

        public string DisplayAuthDate
        {
            get
            {
                if(AuthDate != null && AuthDate != "")
                {
                    return AuthDate.Substring(0, 4) + "." + AuthDate.Substring(4, 2) + "." + AuthDate.Substring(6, 2) + " " + AuthDate.Substring(8, 2) + ":" + AuthDate.Substring(10, 2) + ":" + AuthDate.Substring(12, 2);
                } else
                {
                    return "일시오류";
                }
                
            }
        }

        public Boolean VatFlag
        {
            get
            {
                if(PayMethod == "CASH" && OrderState == "1")
                {
                    return true;
                } else
                {
                    return false;
                }
            }
        }

        public string DisplayPeyMethod
        {
            get
            {
                if (PayMethod == "CARD")
                {
                    return "카드";
                }  else
                {
                    return "현금";
                }
            }
        }
        public string DisplayTotalAmt 
        { 
            get
            {
                if (TotalCancAmt != null && OrderState != "2")
                {
                    return (int.Parse(TotalAmt) - int.Parse(TotalCancAmt)).ToString();
                } else if (PrevCancAmt != "0" && OrderState == "2")
                {
                    return (int.Parse(TotalAmt) - int.Parse(PrevCancAmt)).ToString();
                } 
                return TotalAmt;
            }
        }
        public string DisplayOrgTotalAmt
        {
            get
            {
                if(TotalPrevAmt != "0")
                {
                    return TotalPrevAmt;
                } else
                {
                    return TotalAmt;
                }
            }
        }
        public string DisplayTotalPay
        {
            get
            {
                //if (Vat != null)
                if (TotalCancAmt != null)
                {
                    return (int.Parse(TotalPay) - int.Parse(TotalCancAmt)).ToString();
                }
                return TotalPay;
            }
        }
        public string DisplayTotalCnt
        {
            get
            {
                if(TotalCancCnt != null)
                {
                    return (int.Parse(TotalCnt) - int.Parse(TotalCancCnt)).ToString();
                }
                return TotalCnt;
                
            }
        }
        public string DisplayCardInfo
        {
            get
            {
                if(PayMethod == "CARD")
                {
                    if(MaskingNum != "" && MaskingNum != null)
                    {
                        if (CardQuota != "00")
                        {
                            return MaskingNum + "(" + CardName + "/" + CardQuota + "개월" + ")";
                        }
                        else
                        {
                            return MaskingNum + "(" + CardName + "/일시불" + ")";
                        }
                    } else
                    {
                        return "";
                    }
                    
                }
                else
                {
                    if(MaskingNum != "" && MaskingNum != null)
                    {
                        return MaskingNum + "(" + CardName + ")";
                    } else
                    {
                        return "";
                    }
                    
                }
            }
        }

        public TicketTemp()
        {
            InitializeComponent();
            DataContext = this; // 데이터 컨텍스트 설정
            Init();
        }

        public override void SetTestData()
        {
            OrderNum = "0000123456";
            TicketName = "티켓명 7회차 15:30 ~ 16:30";
            PrintDate = "2024.06.17 12:40:33";

            FranName = "파주도시관광공사 (임진각)";
            BusiNum = "220-01-00123";
            MastName = "대표자명";
            Address = "강원특별자치도 춘천시 춘천순환로 609, 4";
            MastTel = "070-1234-1234";
            ReceiptCont = "■ 영수증은 타인에게 양도 할 수 없습니다.\r\n\r\n  [주의사항]\r\n- 셔틀버스 출발 10분 전에는 이용 요금이 반환 되지 않습니다.\r\n- 검문소에서 신분증 제시와 군 통제에 잘 따라야 합니다.\r\n- 원활한 진행을 위해 반드시 출발 시간을 준수해주세요.\r\n- 지정된 장소 이외는 출입할 수 없습니다.\r\n- 제 3땅굴 내부, 영상관, 차량 이동 구간, 군사 시설은 사진 촬영할 수 없습니다.\r\n- 땅굴 내부 관람 시 휠체어, 유모차는 이용할 수 없습니다.";

            AuthDate = "24.01.31 14:30:57";
            AuthCode = "2231132321";
            CardName = "삼성카드";
            CardQuota = "일시불";

            TotalCnt = "3";
            TotalAmt = "430000";
            TotalPay = "230000";
            Vat = "0";
        }
       

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
