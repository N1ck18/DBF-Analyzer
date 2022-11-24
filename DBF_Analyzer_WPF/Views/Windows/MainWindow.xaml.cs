using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
using System.Xml;
using DBF;
using Microsoft.Win32;

namespace DBF_Analyzer_WPF.Views.Windows
{
    public partial class MainWindow : Window
    {
        #region Values OFF
        //DataTable columnTable = new();
        //DataTable workTable = new();
        //DataSet set = new();
        //public DataView HeaderView { get; set; }
        //public DataView ColumnView { get; set; }
        //public DataView View { get; set; }
        #endregion

        #region DataGrid properties
        /*
             EnableRowVirtualization="True" EnableColumnVirtualization = "true"
             Background="Transparent" BorderThickness="0" BorderBrush="#DFDFDF"
             ScrollViewer.CanContentScroll="true" ScrollViewer.PanningMode="Both"
             VerticalGridLinesBrush="#DFDFDF" HorizontalGridLinesBrush="#DFDFDF"
             HeadersVisibility="Column" ColumnWidth="150" CanUserAddRows="False" CanUserDeleteRows="False" AutoGenerateColumns="False"
             AlternationCount="2"
             Sorting="SortHandler" SelectionChanged="RowDidChange" PreparingCellForEdit="CellDidBeginditing" CellEditEnding="CellDidEndEditing"
             PreviewKeyDown="Keydown"
             VirtualizingPanel.IsVirtualizing="True" VirtualizingPanel.VirtualizationMode="Recycling" VirtualizingPanel.IsContainerVirtualizable="True" VirtualizingPanel.ScrollUnit="Pixel"
             VirtualizingPanel.CacheLengthUnit="Pixel" VirtualizingPanel.IsVirtualizingWhenGrouping="True"
        */
        #endregion

        public MainWindow()
        {
            InitializeComponent();
        }

        #region Close OFF
        //private void Close(object sender, RoutedEventArgs e)
        //{
        //    Close();
        //}
        #endregion

        #region Open file OFF
        // Загружаем DBF файл в представление
        //private void Open_File(object sender, RoutedEventArgs e)
        //{
        //    OpenFileDialog openFile = new();
        //    if (openFile.ShowDialog() == true)
        //    {
        //        columnTable.Dispose();
        //        workTable.Dispose();
        //        set.Dispose();

        //        try
        //        {
        //            set = DBF_Lib.LoadFile(openFile.FileName);
        //        }
        //        catch (Exception)
        //        {
        //            throw new Exception("Ошибка при загрузке файла");
        //        }
        //        set = DBF_Lib.LoadFile(openFile.FileName);
        //        columnTable = set.Tables[1];
        //        workTable = set.Tables[0];
        //        //HeaderView = headerTable.DefaultView;
        //        ColumnView = columnTable.DefaultView;
        //        ColumnTable.Visibility = Visibility.Visible;
        //        View = workTable.DefaultView; //view модель, через него передаём ItemSource
        //        WorkTable.Visibility = Visibility.Visible;
        //        Edit_Off();
        //        DataContext = this; //все данные передаются в окно через этот параметр                
        //    }
        //}
        #endregion

        #region Edit text buttons
        // Кнопки переключения возможности редактирования текста, по-умолчанию false
        private void Edit_On(object sender, RoutedEventArgs e)
        {
            if (WorkTable != null)
            {
                WorkTable.IsReadOnly = false;
                WorkTable.Columns[0].IsReadOnly = true;
            }
        }
        private void Edit_Off(object sender, RoutedEventArgs e)
        {
            if (WorkTable != null)
                WorkTable.IsReadOnly = true;
        }
        //private void Edit_Off() //Перегрузка для подгрузку файла, сбрасывает значение до дефолта
        //{
        //    edit.IsChecked = false;
        //    if (WorkTable != null)
        //        WorkTable.IsReadOnly = true;
        //}
        #endregion

        // Приводим формат даты в русскую
        private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if ((string)e.Column.Header == "№")
            {
                e.Column.CellStyle = new Style(typeof(DataGridCell));
                e.Column.CellStyle.Setters.Add(new Setter(BackgroundProperty, new SolidColorBrush(Colors.LightGray)));
            }            
            if (e.PropertyType == typeof(DateTime))
            {
                ((DataGridTextColumn)e.Column).Binding.StringFormat = "dd.MM.yyyy";
            }
        }
    }
}
