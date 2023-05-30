using shop.Classes;
using System;
using System.Collections.Generic;
using System.Windows;
using Window = System.Windows.Window;
using Word = Microsoft.Office.Interop.Word;

namespace shop.View
{
    /// <summary>
    /// Логика взаимодействия для Bucket.xaml
    /// </summary>
    public partial class Bucket : Window
    {
        public List<Clothes> listBasketClothes;

        public double sumOrder;

        public double sumCard;

        private double finalSumCard;
        public Bucket(List<Clothes> GlistBasketClothes, double summCard)
        {
            listBasketClothes = GlistBasketClothes;
            sumCard = summCard;
            InitializeComponent();
            addInDataGrid();
        }

        protected override void OnActivated(EventArgs e)
        {
            sumOrder = 0;
            for (int i = 0; i < listBasketClothes.Count; i++)
            {
                sumOrder += listBasketClothes[i].End;
            }

            tb_summOrder.Text = "Сумма заказа: " + sumOrder.ToString("0.00");
            finalSumCard = sumCard - sumOrder;
            tb_fromCardCreate.Text = "Остаток на карте: " + finalSumCard.ToString("0.00");
        }

        private void butMainMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void butCreateOrder_Click(object sender, RoutedEventArgs e)
        {
            View.CreateOrderWindow createOrderWindow = new CreateOrderWindow(finalSumCard); //создание объекта окна
            this.Hide();
            createOrderWindow.ShowDialog(); //Показать модальное дополнительное
        }

        public void addInDataGrid()
        {
            dataGridProducts.ItemsSource = null;
            dataGridProducts.ItemsSource = listBasketClothes;

            for (int i = 0; i < listBasketClothes.Count; i++)
            {
                listBasketClothes[i].End = listBasketClothes[i].Discount * listBasketClothes[i].Count;
            }
        }

        public void butCheck_Click(object sender, RoutedEventArgs e)
        {
            //Создание чека заказа
            //Объявление необходимых величин
            Word.Application wordApp;       //Сервер Word
            Word.Document wordDoc;          //Документ Word
            Word.Paragraph wordPar;         //Абзац документа
            Word.Range wordRange;           //Текст абзаца
            Word.Table wordTable;           //Таблица 
            Word.InlineShape wordShape;     //Рисунок
                                            //Создание сервера Word
            try
            {
                wordApp = new Word.Application();
                wordApp.Visible = false;
            }
            catch
            {
                MessageBox.Show("Товарный чек в Word создать не удалось");
                return;
            }
            //Создание документа Word
            wordDoc = wordApp.Documents.Add();      //Добавить новый пустой документ
            wordDoc.PageSetup.Orientation = Word.WdOrientation.wdOrientPortrait; // Книжная

            //***Первый параграф – логотип
            wordPar = wordDoc.Paragraphs.Add();
            wordRange = wordPar.Range;
            wordShape = wordDoc.InlineShapes.AddPicture(Environment.CurrentDirectory + "/../../Resources/logo.jpg", Type.Missing, Type.Missing, wordRange);
            wordShape.Width = 100;
            wordShape.Height = 100;

            //***Второй параграф - дата и время заказа
            wordPar = wordDoc.Paragraphs.Add();
            wordRange = wordPar.Range;
            wordRange.Font.Color = Word.WdColor.wdColorBlack;
            wordRange.Font.Size = 14;
            wordRange.Font.Name = "Arial Black"; //Текст первого абзаца – заголовка документа
            wordRange.Text = "Дата и время заказа: " + DateTime.Now.ToString();
            wordRange.InsertParagraphAfter();

            //***Третий параграф - заголовок таблицы
            wordPar = wordDoc.Paragraphs.Add();
            wordPar.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            wordRange = wordPar.Range;
            wordRange.Font.Color = Word.WdColor.wdColorBlack;
            wordRange.Font.Size = 14;
            wordRange.Font.Name = "Arial Black";
            wordRange.Text = "Список заказанных блюд:";
            wordRange.InsertParagraphAfter();


            //***Четверный параграф - таблица
            wordRange = wordPar.Range;
            //Число строк в таблицы совпадает с число строк в таблице заказов формы
            wordTable = wordDoc.Tables.Add(wordRange, listBasketClothes.Count + 1, 5);
            wordTable.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            wordTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingleWavy;
            //Заголовков таблицы из ЭУ DataGrid
            Word.Range cellRange;
            for (int col = 1; col <= 5; col++)
            {
                cellRange = wordTable.Cell(1, col).Range;
                cellRange.Text = dataGridProducts.Columns[col - 1].Header.ToString();
            }
            //Можно выполнить заливку заголовка таблицы
            wordTable.Rows[1].Shading.ForegroundPatternColor = Word.WdColor.wdColorSkyBlue;
            wordTable.Rows[1].Shading.BackgroundPatternColorIndex = Word.WdColorIndex.wdBlue;
            wordTable.Rows[1].Range.Bold = 1;
            wordTable.Rows[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            //Заполнение ячеек таблицы из списка заказов
            wordPar.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            for (int row = 2; row <= listBasketClothes.Count + 1; row++)
            {
                wordRange.Font.Size = 12;
                wordRange.Font.Name = "Arial Black";
                cellRange = wordTable.Cell(row, 1).Range;
                cellRange.Text = listBasketClothes[row - 2].Name;
                wordRange.Font.Size = 12;
                wordRange.Font.Color = Word.WdColor.wdColorBlack;
                wordRange.Font.Name = "Arial Black";
                cellRange = wordTable.Cell(row, 2).Range;
                cellRange.Font.StrikeThrough = 1;
                cellRange.Text = listBasketClothes[row - 2].Cost.ToString();
                cellRange = wordTable.Cell(row, 3).Range;
                cellRange.Font.StrikeThrough = 0;
                cellRange.Text = listBasketClothes[row - 2].Discount.ToString();
                cellRange = wordTable.Cell(row, 4).Range;
                cellRange.Text = listBasketClothes[row - 2].Rating.ToString();
                cellRange = wordTable.Cell(row, 5).Range;
                cellRange.Text = listBasketClothes[row - 2].Count.ToString();
            }

            //***Пятый параграф - итоги
            wordPar = wordDoc.Paragraphs.Add();
            wordPar.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            wordRange = wordPar.Range;
            wordRange.Font.Color = Word.WdColor.wdColorSkyBlue;
            wordRange.Font.Size = 14;
            wordRange.Font.Name = "Arial Black";
            wordRange.Bold = 2;
            wordRange.Text = "\nСумма заказа: " + sumOrder.ToString("0.00") + " рублей.\n";
            wordRange.InsertParagraphAfter();

            //***Шестой параграф - печать
            wordPar = wordDoc.Paragraphs.Add();
            wordRange = wordPar.Range;
            wordShape = wordDoc.InlineShapes.AddPicture(Environment.CurrentDirectory + "/../../Resources/Shtamp.png", Type.Missing, Type.Missing, wordRange);
            wordShape.Width = 150;
            wordShape.Height = 150;

            //***Седьмой параграф - подписьТекст
            wordPar = wordDoc.Paragraphs.Add();
            wordPar.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            wordRange = wordPar.Range;
            wordRange.Font.Color = Word.WdColor.wdColorBlack;
            wordRange.Font.Size = 14;
            wordRange.Font.Name = "Arial Black";
            wordRange.Text = "\nПодпись:\n";
            wordRange.InsertParagraphAfter();

            //***Восьмой параграф - подпись
            wordPar = wordDoc.Paragraphs.Add();
            wordRange = wordPar.Range;
            wordShape = wordDoc.InlineShapes.AddPicture(Environment.CurrentDirectory + "/../../Resources/Podpis'.png", Type.Missing, Type.Missing, wordRange);
            wordShape.Width = 300;
            wordShape.Height = 100;

            wordApp.Visible = true;
            //Сохранение документа
            string fileName = Environment.CurrentDirectory + "/../../Чек от " + DateTime.Now.ToLongDateString() + ", " + DateTime.Now.ToString("HHч mmмин ssсек");
            wordDoc.SaveAs(fileName + ".docx");
            wordDoc.SaveAs(fileName + ".pdf", Word.WdExportFormat.wdExportFormatPDF);
            //Завершение работы с Word
            wordDoc.Close(true, null, null);                //Сначала закрыть документ
            wordApp.Quit();                     //Выход из Word
                                                //Вызвать свою подпрограмму убивания процессов
            releaseObject(wordPar);             //Уничтожить абзац
            releaseObject(wordDoc);             //Уничтожить документ
            releaseObject(wordApp);             //Удалить из Диспетчера задач
            butCreateOrder_Click(sender, e);
        }

        public void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Не могу освободить объект " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }

        private void Button_ClickPlus(object sender, RoutedEventArgs e)
        {
            Clothes productInOrder = (Clothes)dataGridProducts.SelectedItem;
            if (productInOrder.Discount < finalSumCard)
            {
                listBasketClothes[listBasketClothes.IndexOf(productInOrder)].Count++;
            }
            else
            {
                MessageBox.Show("У Вас недостаточно денег на карте.");
                return;
            }
            addInDataGrid();
            OnActivated(null);
        }

        private void Button_ClickMinus(object sender, RoutedEventArgs e)
        {
            Clothes productInOrder = (Clothes)dataGridProducts.SelectedItem;
            listBasketClothes[listBasketClothes.IndexOf(productInOrder)].Count--;
            if (listBasketClothes[listBasketClothes.IndexOf(productInOrder)].Count == 0)
            {
                listBasketClothes.Remove(productInOrder);
            }
            addInDataGrid();
            OnActivated(null);
        }

        private void Button_ClickDelete(object sender, RoutedEventArgs e)
        {
            Clothes productInOrder = (Clothes)dataGridProducts.SelectedItem;
            listBasketClothes.Remove(productInOrder);
            addInDataGrid();
            OnActivated(null);
        }
    }
}