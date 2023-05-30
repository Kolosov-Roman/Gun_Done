using System;
using System.Collections.Generic;
using System.IO;
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
using Exel = Microsoft.Office.Interop.Excel; //подключение excel


namespace shop
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow() //конструктор главного окна
        {
            InitializeComponent();
            try //обработка исключения
            {
                App.excelApp = new Exel.Application();  //создать объект Exel
                App.excelApp.Visible = false;   //не отображать Exel
            }
            catch 
            {
                MessageBox.Show("Установите MS Excel.");
                this.Close();
            }

            
        }

        // Завершить работу приложения
        private void butExit_Click(object sender, RoutedEventArgs e)
        {
            App.excelApp.Quit(); //Выйти из Excel
            //Уничтожить все COM-объекты
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(App.excelApp);
            //Заставляет сборщик мусора произвести сборку мусора
            GC.Collect();
            this.Close(); //Закрыть главное окно
        }

        // Пункт меню Прайс-лист
        private void butPriceList_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(App.fileMenu)) //проверка наличия документа
            {
                //открыть книгу Excel
                App.excelBook = App.excelApp.Workbooks.Open(App.fileMenu);
                App.excelApp.Visible = true; //сделать видимым Excel
            }
            else
            {
                MessageBox.Show("Файл спрайс-листом отсутствует.");
                this.Close();
            }
        }
       
        // Пункт меню Заказ
        private void butOrder_Click(object sender, RoutedEventArgs e)
        {
            Random rand = new Random();
            double sumCard = Math.Round(rand.NextDouble() * 30000, 2) + 10000;
            MessageBox.Show($"На вашей карте {sumCard}.");

            View.CreateOrderWindow createOrderWindow = new View.CreateOrderWindow(sumCard); //создание объекта окна для конструктора с параметром

            this.Hide();
            createOrderWindow.ShowDialog(); //Показать модальное дополнительное
            this.Show(); //После закрытия доп окна - показать главное

            

        }

        // Пункт меню Каталог
        private void butWorkWithCatalog_Click(object sender, RoutedEventArgs e)
        {
            View.Authorization authorization = new View.Authorization(); //создание объекта окна
            this.Hide();
            authorization.ShowDialog(); //Показать модальное дополнительное
            this.Show(); //После закрытия доп окна - показать главное

        }
    }
}
