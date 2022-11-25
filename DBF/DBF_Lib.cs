using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace DBF
{
    //Вся информация по структуре взята из файла DBFheader.chm
    //
    public static class DBF_Lib
    {

        #region Проверка на валидность файла по заголовку
        private static bool CheckForDBF(string path)
        {
            //Загружаем в память заголовок, на всякий случай, из него будем генерировать файл на выходе
            FileStream fs = new(path, FileMode.Open, FileAccess.Read)
            {
                Position = 0x00 //8
            };

            // Читаем заголовок
            //
            var buffer = new byte[32];
            fs.Read(buffer, 0, buffer.Length);

            // Проверяем тип файла по первому байту
            //
            try
            {
                if (buffer[0] != 0x03)
                    switch (buffer[0])
                    {
                        case 0x04:
                            throw new Exception("D4, D5 (FS)");
                        case 0x05:
                            throw new Exception("D5, Fp (FS)");
                        case 0x43:
                            throw new Exception("FS с мемо-полем .dbv");
                        case 0xB3:
                            throw new Exception("FS с мемо-полями .dbv .dbt");
                        case 0x83:
                            throw new Exception("FS, D3, D4, D5, Fb, Fp, CL с мемо-полем .dbt");
                        case 0x8B:
                            throw new Exception("D4, D5 с мемо-полем .dbt формат D4");
                        case 0x8E:
                            throw new Exception("D4, D5 SQL-таблица");
                        case 0xF5:
                            throw new Exception("Fp с мемо полем .fmp");
                        default:
                            throw new Exception();
                    }
            }
            catch (Exception ex)
            {
                if (ex == null)
                    throw new Exception("Файл повреждён или не является файлом DBF");
                Console.WriteLine($"Файл является таблицей {0}. Продолжать можно на свой страх и риск!", ex);
            }

            // Проверка по заголовку
            //
            //            
            // Количество записей
            //
            //buffer = new byte[4];
            //fs.Position = 0x04; //4
            //fs.Read(buffer, 0, buffer.Length);
            int i; // адрес(индекс) байт, эквивалентно fs.Position если делать через fs.Read

            i = 4;
            int rowCount = buffer[i] + (buffer[i + 1] * 0x100) + (buffer[i + 2] * 0x10000) + (buffer[i + 3] * 0x1000000);

            // Размер заголовка
            //
            //buffer = new byte[2];
            //fs.Position = 0x08;
            //fs.Read(buffer, 0, buffer.Length);
            i = 8;
            int headerSize = buffer[i] + (buffer[i + 1] * 0x100);

            // Размер записи
            //
            //buffer = new byte[2];
            //fs.Position = 0x10;
            //fs.Read(buffer, 0, buffer.Length);
            i = 10;
            int rowLenght = buffer[i] + (buffer[i + 1] * 0x100);

            // Проверка последнего байта заголовка
            //
            buffer = new byte[1];
            fs.Position = headerSize - 1;
            fs.Read(buffer, 0, 1);
            if (buffer[0] != 0x0D)
                throw new Exception("Файл повреждёт, последний байт заголовка не 0x0D");

            // Проверка последнего байта файла
            //
            buffer = new byte[1];
            fs.Position = fs.Length - 1;
            fs.Read(buffer, 0, 1);
            if (buffer[0] != 0x1A)
                throw new Exception("Файл повреждёт, последний байт файла не 0x1A");

            // Проверка данных заголовка (без учёта содержимого записей)
            //
            if (fs.Length != (rowCount * rowLenght + headerSize) + 1) // 1 это последний байт 1А
                throw new Exception("Файл повреждёт, контрольная сумма количества записей, размера заголовка, размера записи не соответствует размеру файла");
            fs.Close();
            return true;
        }
        #endregion


        #region Загрузка заголовка в байт массиве
        public static byte[] LoadByteHeader(string path)
        {
            FileStream fs = new(path, FileMode.Open, FileAccess.Read);

            CheckForDBF(path);

            byte[] buffer = new byte[2];
            fs.Position = 0x08;
            fs.Read(buffer, 0, buffer.Length);
            int headerSize = buffer[0] + (buffer[1] * 0x100);

            buffer = new byte[headerSize];
            fs.Read(buffer, 0, buffer.Length);
            fs.Close();

            return buffer;

            // Заполняем таблицу заголовка
            //
            buffer = new byte[1];
            fs.Read(buffer, 0, buffer.Length);
            //fs.Position = 0x00;

            int headerLength = (buffer[1] * 0x100) + buffer[0];
            if (headerLength == 0)
                throw new Exception("Файл не формата DBF");
            buffer = new byte[headerLength];

            fs.Read(buffer, 0, headerLength);
            fs.Close();
            Console.WriteLine($"{BitConverter.ToString(buffer, headerLength - 2, 2)}");
            return buffer;
        }
        #endregion

        #region Загрузка заголовка в таблицу
        private static DataTable LoadHeader(string path)
        {
            DataTable headerTable = new();
            headerTable.Columns.Add("№", typeof(int));
            headerTable.Columns.Add("HEX", typeof(string));
            headerTable.Columns.Add("DECIMAL", typeof(int));
            headerTable.Columns.Add("Длинна байт", typeof(int));
            headerTable.Columns.Add("Описание", typeof(string));

            FileStream fs = new(path, FileMode.Open, FileAccess.Read)
            {
                Position = 0x00 //8
            };
            byte[] buffer = new byte[32];
            fs.Read(buffer, 0, buffer.Length);

            int i;
            // Количество записей
            i = 4;
            int rowCount = buffer[i] + (buffer[i + 1] * 0x100) + (buffer[i + 2] * 0x10000) + (buffer[i + 3] * 0x1000000);

            // Размер заголовка            
            i = 8;
            int headerSize = buffer[i] + (buffer[i + 1] * 0x100);

            // Размер записи            
            i = 10;
            int rowLenght = buffer[i] + (buffer[i + 1] * 0x100);

            string bufferString = "";
            for (int j = 16; j < 28; j++)
            {
                bufferString += buffer[j].ToString("X");
            }

            headerTable.Rows.Add(1, buffer[0].ToString("X"), buffer[0], 1, "Простая(наверное) таблица");
            headerTable.Rows.Add(2, buffer[1].ToString("X"), buffer[1], 1, "Год последнего обновления таблицы");
            headerTable.Rows.Add(3, buffer[2].ToString("X"), buffer[2], 1, "Месяц последнего обновления таблицы");
            headerTable.Rows.Add(4, buffer[3].ToString("X"), buffer[3], 1, "День последнего обновления таблицы");
            headerTable.Rows.Add(5, rowCount.ToString("X"), rowCount, 4, "Количество записей в таблице");
            headerTable.Rows.Add(6, headerSize.ToString("X"), headerSize, 2, "Размер заголовка в байтах");
            headerTable.Rows.Add(7, rowLenght.ToString("X"), rowLenght, 2, "Размер записи в байтах");
            headerTable.Rows.Add(8, buffer[12].ToString("X") + buffer[13].ToString("X"), buffer[12], 2, "Зарезервировано");
            headerTable.Rows.Add(9, buffer[14].ToString("X"), buffer[14], 1, "Транзакция (начало 0х01, конец/игнорирование 0х00)");
            headerTable.Rows.Add(10, buffer[15].ToString("X"), buffer[15], 1, "Нормальная видимость (0х00), закодированно (0х01)");
            headerTable.Rows.Add(11, bufferString, buffer[16], 12, "Использование многопользовательского окружения");
            headerTable.Rows.Add(12, buffer[28].ToString("X"), buffer[28], 1, "Индекс используется (0х01) или нет (0х00)");
            headerTable.Rows.Add(13, buffer[29].ToString("X"), buffer[29], 1, "Кодовая страница, если 0x65 то Russian MS-DOS 866");
            headerTable.Rows.Add(14, buffer[30].ToString("X") + buffer[31].ToString("X"), buffer[30], 2, "Зарезервировано");

            fs.Close();
            return headerTable;
        }
        #endregion

        #region Исправление null записей в numeric
        //Исплавляем записи с данными, не соответствующими формату столбца
        //пример: в Numeric строке в байт коде стоят символы, вместо 20 20 20 20 20 20 стоит 6E 75 6C 6C 20 20 (null__)
        //
        public static string fix_string()
        {
            string result = "";
            return result;
        }

        #endregion

        #region Загрузка файла, основной метод
        public static DataSet LoadFile(string path)
        {
            //Загружаем DBF файл, анализируем и составляем таблицу формата DataTable
            //Данная библиотека предназначена только для простой таблицы FS, D3, D4, D5, Fb, Fp, CL, первый байт должен быть 0x03
            //DBF:
            //Header - заголовок, первые 32 байта +
            //Поля, 32 байта на каждое поле - название(11 байт), тип(1 байт), адрес в памяти(4 байта), размер(1 байт), знаки после запятой(1 байт)
            //Заканчиваются байтом 0x0D
            //далее - поля записей(Records), размер записи в байтах 0x0A-0B заголовка(little endian, справа-налево!)
            //Заканчиваются байтом 0x1A
            //
            //Важные байты заголовка:
            //0x04 - RecordsCount, 4 bytes
            //0x08 - HeaderSize, 2 bytes
            //0x0A - RecordSize, 2 bytes
            //0x1D - Язык, 65 это Russian MS-DOS, 866, 1 byte

            //Запускаем проверку на DBF формат, выдаст ошибку если что-то не так
            //
            CheckForDBF(path);

            DataSet set = new();
            DataTable table = new();
            FileStream fs = new(path, FileMode.Open, FileAccess.Read);

            // Создаём таблицу 

            //Настройка отображения даты, чисел и кодировки!!!
            //
            CultureInfo dfi = new("ru-RU"); //Русский формат отображения даты, в файле всегда идёт американский формат ггггММдд
            CultureInfo nfi = new("en-US"); //Американский формат разделителя чисел - . вместо , DBF понимает только точку!!!
            //Устанавливаем пакет System.Text.Encoding.CodePages
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); //регистрируем кодовые страницы с установленного пакета            
                                                                           //Encoding.GetEncoding(866);

            //Считаем количество строк
            byte[] buffer = new byte[4];
            fs.Position = 0x04; //4
            fs.Read(buffer, 0, buffer.Length);
            int rowCount = buffer[0] + (buffer[1] * 0x100) + (buffer[2] * 0x10000) + (buffer[3] * 0x1000000);
            if (rowCount == 0)
                throw new Exception("Не удалось прочесть байты с количеством записей");

            //Считаем количество столбцов
            fs.Position = 0x08; //8
            buffer = new byte[2];
            fs.Read(buffer, 0, buffer.Length);
            int columnCount = (((buffer[1] * 0x100) + buffer[0]) - 32 - 1) / 32;
            if (columnCount == 0)
                throw new Exception("Не удалось создать ни одной колонки");

            //Проверка производительности
            var sw = new Stopwatch();
            sw.Start();

            //Создаём колонки, присваиваем имя и тип
            //
            fs.Position = 0x20; //32
            buffer = new byte[columnCount * 32];
            fs.Read(buffer, 0, buffer.Length);

            //Структура описания поля в заголовке, адреса со смещением относительно начала заголовка(32 байта)
            //
            //ColumnName                 //0x00 Имя поля, 11 байт
            //ColumnType                 //0x11 Тип поля, 1 байт
            //Вычисляется при выгрузке   //0x12 Адрес в памяти, нужно для записей, начинается с 0x01 и далее смещение идёт на размер поля(columnSize), 4 байта
            //Size                       //0x16 Размер поля, 1 байт
            //Decimal                    //0x17 Количество знаков после запятой, 1 байт            


            //Создаём дополнительную таблицу к основной со значениями размера поля и количества знаков после запятой для N формата
            //В одной таблице DataTabble это сделать нельзя никак
            #region Column Table
            DataTable extendedTable = new();
            extendedTable.Columns.Add("Name", Type.GetType("System.String")); // на всякий
            extendedTable.Columns.Add("Size", Type.GetType("System.Int32"));
            extendedTable.Columns.Add("Decimal", Type.GetType("System.Int32"));
            extendedTable.Columns.Add("Description", typeof(string));

            //СОЗДАЁМ ТАБЛИЦУ КОЛОНОК
            //            
            int index = 0;
            for (int j = 0; j < columnCount; j++)
            {
                DataRow extRow = extendedTable.NewRow();
                DataColumn dataColumn = new()
                {
                    ColumnName = "" //Обнуляем имя
                };
                extRow[1] = buffer[index + 16]; //Размер поля
                extRow[2] = buffer[index + 17]; //Количество знаков после запятой
                extRow[3] = ""; // Описание (служебный столбец), по-умолчанию пусто. Можно заполнить в основной программе используя словать, например

                // т.к. в заголовке, в отличии от записей, все не используемые символы записываются как 0x00 и идут строго после данных, мы можем просто их отбросить
                for (int i = 0; i < 11; i++) //Находим имя
                {
                    if (buffer[index + i] == 0x00 || i == 10)
                    {
                        dataColumn.ColumnName += Encoding.GetEncoding(866).GetString(buffer, index, i); //866 начинает работать после установки пакета и добавлении провайдера
                        break;
                    }
                }

                //Создаём колонки таблицы в зависимости от типа 
                string columnType = Encoding.GetEncoding(866).GetString(buffer, index + 11, 1); //Тип поля
                switch (columnType)
                {
                    case "D":
                        dataColumn.DataType = typeof(DateTime);
                        break;
                    case "F":
                        dataColumn.DataType = Type.GetType("System.Double");
                        break;
                    case "N":
                        if (int.Parse(extRow[2].ToString()) != 0)
                            dataColumn.DataType = Type.GetType("System.Decimal");
                        else
                        {
                            if (int.Parse(extRow[1].ToString()) > 9)
                            {
                                dataColumn.DataType = Type.GetType("System.Int64");
                            }
                            else dataColumn.DataType = Type.GetType("System.Int32");
                        }
                        break;
                    case "L":
                        dataColumn.DataType = Type.GetType("System.Boolean");
                        break;
                    case "C":
                        dataColumn.DataType = Type.GetType("System.String");
                        break;
                    default: // неизвестный формат, или не реализованный, тут либо писать в string либо выдавать ошибку    
                        throw new Exception("Этот формат не реализован в программе! Сожалеем");
                        //dataColumn.DataType = Type.GetType("System.String");
                        //break;
                }
                extRow[0] = dataColumn.ColumnName;
                extendedTable.Rows.Add(extRow);
                table.Columns.Add(dataColumn);
                index += 32; //смещение на 32 байта к началу следующей колонки                
            }
            #endregion
            //
            //ТАБЛИЦА СОЗДАНА

            sw.Stop();
            Console.WriteLine($"Time Spent: {sw.Elapsed}");
            sw.Restart(); //Перезапуск проверки

            #region Work Table
            //Заполняем таблицу данными
            //
            //
            //Считаем количество записей
            fs.Position = 0x04;
            buffer = new byte[4];
            fs.Read(buffer, 0, buffer.Length);
            int RecordsCount = buffer[3] * 0x1000000 + buffer[2] * 0x10000 + buffer[1] * 0x100 + buffer[0];
            // !!! байты в файле считываются слева - направо, но читаться должны справа - налево
            // пример: 1A 16 05 мы видим последовательность в файле, 3 байта, но полное число будет 05 16 1A, поэтому сдвигаем байты влево на позицию(0x100)
            //       0x16 * 0x100 = 0x1600, 0x05 * 0x10000 = 0x050000 => 0x1A + 0x1600 + 0x050000 = 0x05161A, что мы и должны получить
            // ещё можно реализовать методом сдвига битов влево на 8: 0x1A + 0x16 << 8 + 0x05 << 16 = 0x05161A

            //Считаем длину записи
            fs.Position = 0x0A; //10
            buffer = new byte[2];
            fs.Read(buffer, 0, buffer.Length);
            int RecordSize = buffer[0] + buffer[1] * 0x100;

            //Считыаем размер заголовка
            fs.Position = 0x08;
            buffer = new byte[2];
            fs.Read(buffer, 0, buffer.Length);
            int HeaderSize = (buffer[1] << 8) + buffer[0];

            fs.Position = HeaderSize;
            buffer = new byte[RecordSize * RecordsCount];
            fs.Read(buffer, 0, buffer.Length);
            index = 0;
            int rowSize; //размер поля, берём из расширенной таблицы (2-й столбец)

            fs.Close(); //Поидее больше не нужен, все данные в буфере

            //СОЗДАЁМ ТАБЛИЦУ            
            table.BeginLoadData();
            for (int j = 0; j < RecordsCount; j++) //отсчёт по количеству записей
            {
                DataRow row = table.NewRow();
                index += 1; //пропускаем первый байт строки, т.к. он служит разделителем между записями
                for (int i = 0; i < columnCount; i++) //отсчёт по количеству колонок
                {
                    rowSize = int.Parse(extendedTable.Rows[i][1].ToString()); //длина поля
                    string stringValue = Encoding.GetEncoding(866).GetString(buffer, index, rowSize); //устанавливаем кодировку чтения символов, читаем строку

                    switch (table.Columns[i].DataType.ToString())
                    {
                        //Всё записано в кодировке 866 Russian MS-DOS, при использовании ASCII кодировки энкодер понимает только латинский алфавит
                        //Проверяем на пустые значения, если верно то пишем DBNull.Value(!только так!) везде кроме C (string) формата
                        //
                        //
                        //ВНИМАНИЕ!!! проверка try catch идёт на поля не строкового формата. Иногда в любом другом формате можно встретить ячейки вида null                        
                        //пример: в Numeric строке в байт коде стоят символы 6E 75 6C 6C 20 20 (null__), вместо 20 20 20 20 20 20
                        //
                        case "System.DateTime":
                            if (stringValue.Replace(" ", "") == "") //Проверка на пустую строку                            
                                row[i] = DBNull.Value;
                            else
                                try
                                {
                                    row[i] = DateTime.ParseExact(stringValue, "yyyyMMdd", dfi);
                                }
                                catch (Exception ex)
                                {
                                    if (stringValue.Replace(" ", "") == "null")
                                    {
                                        row[i] = DBNull.Value;
                                    }
                                    else
                                        throw new Exception($"{ex.Message} \n Строка: {j}, Колонка: {table.Columns[i].ColumnName}, Значение: {stringValue}");
                                }
                            break;

                        case "System.Double":                                                     //ВНИМАНИЕ!!!!!!! не протестированно!
                            if (stringValue.Replace(" ", "") == "") //Проверка на пустую строку                            
                                row[i] = DBNull.Value;
                            else
                                try
                                {
                                    row[i] = double.Parse(stringValue, nfi);
                                }
                                catch (Exception ex)
                                {
                                    if (stringValue.Replace(" ", "") == "null")
                                    {
                                        row[i] = DBNull.Value;
                                    }
                                    else
                                        throw new Exception($"{ex.Message} \n Строка: {j}, Колонка: {table.Columns[i].ColumnName}, Значение: {stringValue}");
                                }
                            break;

                        case "System.Boolean":                                                    //ВНИМАНИЕ!!!!!!! не протестированно!                            
                            if (stringValue.Replace(" ", "") == "") //Проверка на пустую строку                            
                                row[i] = DBNull.Value;
                            else
                                try
                                {
                                    row[i] = bool.Parse(stringValue);
                                }
                                catch (Exception ex)
                                {
                                    if (stringValue.Replace(" ", "") == "null")
                                    {
                                        row[i] = DBNull.Value;
                                    }
                                    else
                                        throw new Exception($"{ex.Message} \n Строка: {j}, Колонка: {table.Columns[i].ColumnName}, Значение: {stringValue}");
                                }
                            break;

                        case "System.Int32":
                            if (stringValue.Replace(" ", "") == "") //Проверка на пустую строку
                                row[i] = DBNull.Value;
                            else
                                try
                                {
                                    // Отключено на всякий случай
                                    //row[i] = int.Parse(stringValue.Replace(".", ""), nfi);
                                    row[i] = int.Parse(stringValue, nfi);
                                }
                                catch (Exception ex)
                                {
                                    if (stringValue.Replace(" ", "") == "null")
                                    {
                                        row[i] = DBNull.Value;
                                    }
                                    else                                        
                                        throw new Exception($"{ex.Message} \n Строка: {j}, Колонка: {table.Columns[i].ColumnName}, Значение: {stringValue}");
                                }
                            break;

                        case "System.Int64":
                            if (stringValue.Replace(" ", "") == "") //Проверка на пустую строку
                                row[i] = DBNull.Value;
                            else
                                try
                                {
                                    // Отключено на всякий случай
                                    //row[i] = long.Parse(stringValue.Replace(".", ""), nfi);
                                    row[i] = long.Parse(stringValue, nfi);
                                }
                                catch (Exception ex)
                                {
                                    if (stringValue.Replace(" ", "") == "null")
                                    {
                                        row[i] = DBNull.Value;
                                    }
                                    else
                                        throw new Exception($"{ex.Message} \n Строка: {j}, Колонка: {table.Columns[i].ColumnName}, Значение: {stringValue}");
                                }                            
                            break;

                        case "System.Decimal":
                            if (stringValue.Replace(" ", "") == "") //Проверка на пустую строку
                                row[i] = DBNull.Value;
                            else
                                try
                                {
                                    row[i] = decimal.Parse(stringValue, nfi);
                                }
                                catch (Exception ex)
                                {
                                    if (stringValue.Replace(" ", "") == "null")
                                    {
                                        row[i] = DBNull.Value;
                                    }
                                    else
                                        throw new Exception($"{ex.Message} \n Строка: {j}, Колонка: {table.Columns[i].ColumnName}, Значение: {stringValue}");
                                }                            
                            break;

                        case "System.String":
                            if (stringValue.Replace(" ", "") == "") //Проверка на пустую строку
                                row[i] = "";
                            else
                            {
                                //Обрезаем бесполезные пробелы после данных
                                if (stringValue[^1] == ' ')
                                {
                                    int spaceIndex = 0;
                                    for (int k = 0; k < stringValue.Length; k++)
                                    {
                                        if (stringValue[k] == ' ' && (stringValue.Length - 1) == k)
                                        {
                                            stringValue = stringValue.Remove(spaceIndex);
                                            break;
                                        }
                                        if (stringValue[k] == ' ')
                                        {
                                            if (spaceIndex != 0)
                                                continue;
                                            spaceIndex = k;
                                        }
                                        else spaceIndex = 0;
                                    }
                                }
                                row[i] = stringValue;
                            }
                            break;
                        default: // неизвестный формат, или не реализованный, тут либо писать в string либо выдавать ошибку                            
                            //row[i] = stringValue;                            
                            throw new Exception($"Запись неизвестного формата \n Строка: {j}, Колонка: {table.Columns[i].ColumnName}, Значение: {stringValue}");                            
                            //break;
                    }
                    index += rowSize;
                }
                table.Rows.Add(row);
            }
            table.EndLoadData();

            #endregion            
            //ТАБЛИЦА СОЗДАНА

            sw.Stop();
            Console.WriteLine($"Time Spent: {sw.Elapsed}");
            set.Tables.Add(table);
            set.Tables.Add(extendedTable);
            set.Tables.Add(LoadHeader(path));

            return set;
        }
        #endregion
    }
}