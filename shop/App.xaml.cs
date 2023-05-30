using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Exel = Microsoft.Office.Interop.Excel; //подключение excel

namespace shop
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        //Данные для авторизации администратора
        public static string Login = "admin";
        public static string Password = "shop";

        //Данные для работы с Excel
        public static Exel.Application excelApp; //запуск excel
        public static Exel.Workbook excelBook; //отдельная книга
        public static Exel.Worksheet excelSheet; //один лист
        public static Exel.Range excelCells; //ячейки листа

        //Пути к файлам приложения
        public static string pathExe = Environment.CurrentDirectory; //к файлу exe
        public static string fileMenu = pathExe + @"\PriceList-1.xlsx"; //к файлу Exel
    }
}
