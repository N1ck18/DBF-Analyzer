using DBF;
using DBF_Analyzer_WPF.Infrastructure.Commands;
using DBF_Analyzer_WPF.Services;
using DBF_Analyzer_WPF.ViewModels.Base;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DBF_Analyzer_WPF.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        // Основные

        #region Title - Заголовок окна
        /// <summary>
        /// Заголовок окна
        /// </summary>
        private string _Title = "DBF Анализатор";
        public string Title
        {
            get => _Title;
            set => Set(ref _Title, value);
        }
        #endregion

        #region ColumnType - тип ячейки в StatusBar 
        /// <summary>
        /// Тип ячейки
        /// </summary>
        private string _ColumnType = "";
        public string ColumnType
        {
            get => _ColumnType;
            set => Set(ref _ColumnType, value);

            //get { return _ColumnType; }
            //set { Set(ref _ColumnType, value); }
        }
        #endregion

        #region RecordCount - количество записей в StatusBar
        /// <summary>
        /// Количество записей
        /// </summary>
        private int _RecordCount = 0;
        public int RecordCount
        {
            get => _RecordCount;
            set => Set(ref _RecordCount, value);
        }
        #endregion

        #region LoadBar : int - полоска загрузки
        /// <summary>
        /// Полоска загрузки
        /// </summary>
        private int _LoadBar = 30;
        /// <summary>
        /// Полоска загрузки
        /// </summary>
        public int LoadBar
        {
            get => _LoadBar;
            set => Set(ref _LoadBar, value);
        }

        #endregion

        #region DelimiterCount - количество знаков после запятой
        /// <summary>
        /// Количество знаков после запятой
        /// </summary>
        private int _DelimiterCount = 0;
        public int DelimiterCount
        {
            get => _DelimiterCount;
            set => Set(ref _DelimiterCount, value);
        }
        #endregion

        #region InControll - столбец находится в контроле
        /// <summary>
        /// Столбец находится в контроле
        /// </summary>
        private bool _InControll = false;
        public bool InControll
        {
            get => _InControll;
            set => Set(ref _InControll, value);
        }
        #endregion

        // Комманды

        #region Close Application
        public ICommand CloseApplicationCommand { get; }
        private bool CanCloseApplicationCommandExecute(object p) => true;
        private void OnCloseApplicationCommandExecuted(object p)
        {
            Application.Current.Shutdown();
        }
        #endregion

        #region Load File
        public ICommand OpenFileCommand { get; }

        DataSet set = new();
        DataTable columnTable = new();
        DataTable workTable = new();
        DataTable headerTable = new();

        // Таблица заголовка файла
        private DataView _HeaderView;
        public DataView HeaderView
        {
            get => _HeaderView;
            set => Set(ref _HeaderView, value);
        }

        // Таблица толбцов
        private DataView _ColumnView;
        public DataView ColumnView
        {
            get => _ColumnView;
            set => Set(ref _ColumnView, value);
        }

        // Основная таблица данных
        private DataView _View;
        public DataView View
        {
            get => _View;
            set => Set(ref _View, value);
        }

        private bool CanOpenFileCommandExecute(object p) => true;
        private void OnOpenFileCommandExecuted(object p)
        {
            OpenFileDialog openFile = new();
            if (openFile.ShowDialog() == true)
            {
                try
                {
                    set = DBF_Lib.LoadFile(openFile.FileName);
                }
                // Тут надо сделать обработчик ошибок
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                    //Console.WriteLine(ex);
                    //return;
                }
                set = DBF_Lib.LoadFile(openFile.FileName);
                headerTable = set.Tables[2];
                columnTable = set.Tables[1];
                workTable = set.Tables[0];

                // индексируем таблицы
                workTable = IndexTable(workTable);
                columnTable = IndexTable(columnTable);

                // присваиваем данные для окна
                RecordCount = workTable.Rows.Count;

                // прикручиваем view модель
                HeaderView = headerTable.DefaultView;
                ColumnView = columnTable.DefaultView;
                View = workTable.DefaultView; //view модель, через него передаём ItemSource

                //WorkTable.Visibility = Visibility.Visible;                
                //DataContext = this; //все данные передаются в окно через этот параметр
                //ColumnTable.Visibility = Visibility.Visible;
            }
        }
        #endregion

        #region Анализ файла
        public ICommand AnalyzeButtonCommand { get; }
        private bool CanAnalyzeButtonCommandExecute(object p) => true;
        private void OnAnalyzeButtonCommandExecuted(object p)
        {
            
            //bool result = (true && false) || false;
            //string colName = "UKL";
            //string 

            //if (workTable.Columns.Contains(colName))
            //{
            //    if (workTable.Rows[0][colName] )
            //    {

            //    }
            //    workTable.Rows[0][colName] 
            //}
            workTable.Rows[0][1] = 5;
            workTable.AcceptChanges();
        }
        #endregion


        // Обработчики

        #region Колонка индексов
        private static DataTable IndexTable(DataTable dataTable)
        {
            dataTable.Columns.Add("№", typeof(string)).SetOrdinal(0);
            for (int i = 1; i <= dataTable.Rows.Count; i++)
                dataTable.Rows[i - 1][0] = i.ToString();
            dataTable.AcceptChanges();
            return dataTable;
        }
        #endregion


        public MainWindowViewModel()
        {
            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);
            OpenFileCommand = new LambdaCommand(OnOpenFileCommandExecuted, CanOpenFileCommandExecute);
            AnalyzeButtonCommand = new LambdaCommand(OnAnalyzeButtonCommandExecuted, CanAnalyzeButtonCommandExecute);
        }


    }
}
