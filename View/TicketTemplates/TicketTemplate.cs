using HwModule.Utils;
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

namespace HwModule.View.TicketTemplates
{
    public class TicketTemplate : UserControl
    {
        public Thickness DocMargin { get; set; }
        public SolidColorBrush DocBackground { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        //static TicketTemplate()
        //{
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(TicketTemplate), new FrameworkPropertyMetadata(typeof(TicketTemplate)));
        //}

        public void Init()
        {
            this.DataContext = this;

            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()) == false)
                return;

            //디자인 타임 샘플 데이터
            DocBackground = new SolidColorBrush(Colors.LightBlue);
            SetTestData();
        }

        public void SetMargin(double leftMargin, double topMargin)
        {
            var left = 0d;
            var top = 0d;

            //파라미터 마진 단위가 mm 이기 때문에 픽셀로 변환
            left = Math.Round(leftMargin.MillimeterToPixel(), 2);
            top = Math.Round(topMargin.MillimeterToPixel(), 2);

            DocMargin = new Thickness(left, top, 0, 0);
        }

        public virtual void SetTestData()
        {

        }
    }
}
