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
    /// SeatTemp.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SeatTemp : TicketTemplate, INotifyPropertyChanged
    { 
        public string PlaceName { get; set; }
        public string TicketName { get; set; }
        public string OrgTicketName { get; set; }
        public string TotalCnt { get; set; }
        public string TotalCancCnt { get; set; }
        public string MoveWayName { get; set; }
        public string MoveMethodName { get; set; }
        public string TypeName { get; set; }
        public string TurnTime { get; set; }
        public string UseDate { get; set; }
        public string ReceiptCont { get; set; }

        // 이용 사이트 정보
        public List<Dictionary<string, string>> OrderSeatList { get; set; }

        public SeatTemp()
        {
            InitializeComponent();
            DataContext = this; // 데이터 컨텍스트 설정
            Init();
        }

        public string DisplayTitle
        {
            get
            {
                return PlaceName + " 좌석정보";
            }
        }

        public string DisplayTotalCnt
        {
            get
            {
                if(TotalCancCnt != "" && TotalCancCnt != "0")
                {
                    return (int.Parse(TotalCnt) - int.Parse(TotalCancCnt)).ToString();
                }  else
                {
                    return TotalCnt;
                }
            }
        }
        public string DisplayTicketName
        {
            get
            {
                if(OrgTicketName != null && TurnTime != null)
                {
                    return OrgTicketName + " " + TurnTime.Substring(0, 2) + ":" + TurnTime.Substring(2, 2);
                } 
                else
                {
                    return "";
                }
            }
        }

        public string DisplayMoveCategory
        {
            get
            {
                if(MoveWayName != null && MoveMethodName != null && TypeName != null)
                {
                    return MoveWayName + " " + MoveMethodName + " " + "(" + TypeName + ")";
                }
                else
                {
                    return "";
                }
            }
        }

        public override void SetTestData()
        {
            PlaceName = "DMZ 평화관광";
            TicketName = "4월 7일 (1회차-땅굴행 도보) 16:40";
            TotalCnt = "3";
            
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
