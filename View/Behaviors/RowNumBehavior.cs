using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace SecurityTour.Pos.View.Behaviors
{
    public class RowNumBehavior
    {
        #region IsRowNumbers property

        public static readonly DependencyProperty IsRowNumbersProperty =
            DependencyProperty.RegisterAttached("IsRowNumbers", typeof(bool), typeof(RowNumBehavior),
            new FrameworkPropertyMetadata(false, OnRowNumbersChanged));

        private static void OnRowNumbersChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            DataGrid grid = source as DataGrid;
            if (grid == null)
                return;
            if ((bool)args.NewValue)
            {
                grid.LoadingRow += OnGridLoadingRow;
                grid.UnloadingRow += OnGridUnloadingRow;
            }
            else
            {
                grid.LoadingRow -= OnGridLoadingRow;
                grid.UnloadingRow -= OnGridUnloadingRow;
            }
        }

        private static void RefreshDataGridRowNumbers(object sender)
        {
            DataGrid grid = sender as DataGrid;
            if (grid == null)
                return;

            foreach (var item in grid.Items)
            {
                var row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromItem(item);
                if (row != null)
                    row.Header = row.GetIndex() + 1;
            }
        }

        private static void OnGridUnloadingRow(object sender, DataGridRowEventArgs e)
        {
            RefreshDataGridRowNumbers(sender);
        }

        private static void OnGridLoadingRow(object sender, DataGridRowEventArgs e)
        {
            RefreshDataGridRowNumbers(sender);
        }

        [AttachedPropertyBrowsableForType(typeof(DataGrid))]
        public static void SetIsRowNumbers(DependencyObject element, bool value)
        {
            element.SetValue(IsRowNumbersProperty, value);
        }

        public static bool GetIsRowNumbers(DependencyObject element)
        {
            return (bool)element.GetValue(IsRowNumbersProperty);
        }

        #endregion
    }
}
