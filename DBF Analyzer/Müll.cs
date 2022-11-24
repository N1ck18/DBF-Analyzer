using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBF_Analyzer
{
    internal class Müll
    {

        //    decimal rowDecimal = decimal.Parse(Math.Pow(0.1, int.Parse(extendedTable.Rows[j][2].ToString())).ToString());
        //    decimal decimalValue = 0.0m * rowDecimal;
        //    for (int k = 0; k < rowSize; k++)
        //    {
        //        //Автоматизируем сдвиг байт, т.к. не знаем сколько точно будет байт в числе
        //        if (buffer[index] != 0x00)
        //        {
        //            decimalValue += buffer[index + k] * 0x10 ^ (k * 0x02);
        //        }

        ////Автоматизируем сдвиг байт, т.к. не знаем сколько точно будет байт в числе
        //if (buffer[index] != 0x00)
        //{                                        
        //    intValue += buffer[index + k] * 0x10 ^ (k * 0x02);
        //}
        //else break;



        //Множественным fs.Read
        //
        //ВНИМАНИE!!! по тестам работает медленнее!!!!
        //
        //fs.Position = 0x20; //32
        //buffer = new byte[12];
        //string typeColumn;
        //string columnName = "";

        ////fs.Position смещается при перебирании байтов, поэтому прибавляем 20 байт а не 32 (12+20=32)
        //for (int j = 0; j < columnCount; j++)
        //{
        //    fs.Read(buffer, 0, buffer.Length);
        //    for (int i = 0; i < buffer.Length; i++)
        //    {
        //        if (buffer[i] == 0x00 || i == buffer.Length - 1)
        //        {
        //            columnName += Encoding.Default.GetString(buffer, 0, i);
        //            typeColumn = Encoding.Default.GetString(buffer, 11, 1);
        //            table.Columns.Add(columnName, Type.GetType("System.String"));
        //            columnName = "";
        //            fs.Position += 20;
        //            break;
        //        }
        //    }
        //}


    }
}
