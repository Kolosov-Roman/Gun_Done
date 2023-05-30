using shop.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

using Excel = Microsoft.Office.Interop.Excel;

namespace shop.View
{
    /// <summary>
    /// Логика взаимодействия для CreateOrderWindow.xaml
    /// </summary>
    public partial class CreateOrderWindow : Window
    {
        public double sumCard; //в величину передается значение
        public double sumOrder = 0;

        public List<Clothes> listClothes; //глобальный список одежды

        public List<Clothes> listBasketClothes = new List<Clothes>();

        List<Category> Category = new List<Category>();

        //конструктор с параметром - переданное из окна значение
        public CreateOrderWindow(double sumCard)
        {
            InitializeComponent();
            if (File.Exists(App.fileMenu)) //проверка наличия документа
            {
                //открыть книгу Excel
                App.excelBook = App.excelApp.Workbooks.Open(App.fileMenu);
                App.excelApp.Visible = false; //сделать видимым Excel
            }
            else
            {
                MessageBox.Show("Файл спрайс-листом отсутствует.");
                this.Close();
            }
            this.sumCard = sumCard; //инициализация
            tb_fromCard.Text += sumCard.ToString();
        }



        private void butMainMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void butCreateOrder_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            var rand = new Random();

            MessageBox.Show($"Сумма Вашего заказа составила {sumOrder}.");
            View.Bucket bucket = new Bucket(listBasketClothes, sumCard); //создание объекта окна
            bucket.Owner = this; //Указать владельца у дополнительного окна
            bucket.ShowDialog(); //Показать модальное дополнительное

            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Стереть все категории
            listCategory.Items.Clear();

            Category category = new Category();

            for (int i = 1; i <= App.excelBook.Worksheets.Count; i++)
            {
                category = new Category();
                category.Name = App.excelBook.Worksheets[i].Name;
                category.Photo = category.Photo = App.pathExe + $"/../../Resources/{category.Name}.png";
                Category.Add(category);
            }
            listCategory.ItemsSource = Category;
        }

        private void listCategory_SelectionChenged(object sender, SelectionChangedEventArgs e)
        {
            Category category = (Category)listCategory.SelectedItem;
            string categoryName = category.Name;

            listClothes = new List<Clothes>(); //Создать список одежды
            Clothes clothes; //объявить отдельную вещь

            foreach (Excel.Worksheet item in App.excelBook.Worksheets)
            {
                if (item.Name == categoryName)
                {
                    App.excelCells = item.Cells;
                }
            }

            //получить все заполненные ячейки листа в цикле
            for (int i = 1; i <= App.excelCells.Rows.Count; i++)
            {
                if (App.excelCells[i, 1].value2 == null)
                    break;

                clothes = new Clothes(); //создать отдельную вещь
                //заполнить поля объекта clothes из ячеек Excel
                clothes.Name = (string)App.excelCells.Cells[i, 1].Value2; //название вещи в объект
                clothes.Cost = (int)App.excelCells.Cells[i, 2].Value2; //название вещи в объект
                clothes.Discount = int.Parse(((double)clothes.Cost * (1.0 - (double)App.excelCells.Cells[i, 3].Value2 / 100.0)).ToString());
                clothes.Rating = (double)App.excelCells.Cells[i, 4].Value2; //название вещи в объект
                clothes.Photo = App.pathExe + $"/../../Resources/{categoryName}/{App.excelCells.Cells[i, 5].Value2}.png";
                clothes.Count = 0;
                clothes.End = 0;

                listClothes.Add(clothes); //занесение вещи в список
            }

            listViewClothes.ItemsSource = listClothes; //привязать список к элементу интерфейса
        }

        private void ButtonAddInBasket_Click(object sender, RoutedEventArgs e)
        {
            Grid parent = (Grid)(sender as Button).Parent;
            int i = listViewClothes.Items.IndexOf(parent.DataContext);

            if (sumOrder + listClothes[i].Discount < sumCard)
            {
                sumOrder += listClothes[i].Discount;
                listClothes[i].Count++;

                if (listClothes[i].Count <= 1)
                {
                    listBasketClothes.Add(listClothes[i]);
                }

                tb_summOrder.Text = $"Сумма заказа: {sumOrder.ToString("0.00")}";
            }
            else
            {
                MessageBox.Show("У Вас недостаточно денег на карте.");
            }
        }
    }
}