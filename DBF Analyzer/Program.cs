using DBF_Analyzer;
using System;
using System.Data;
using System.Reflection;
using System.Text;

Console.WriteLine("Hello, World!");


//DBF.Append("c:\\Users\\it3\\source\\repos\\НИКОЛАЙ\\из МИС\\PAT.dbf");
//var header = DBF_new.LoadHeader("c:\\Users\\Nikolay\\source\\repos\\НИКОЛАЙ\\из МИС\\PAT.dbf");
DataSet _ = DBF_new.LoadFile("c:\\Users\\Nikolay\\source\\repos\\НИКОЛАЙ\\из МИС\\PAT.dbf");
//DataTable table = DBF_new.LoadFile("c:\\Users\\it3\\source\\repos\\НИКОЛАЙ\\из МИС\\PAT.dbf");


Console.ReadLine();