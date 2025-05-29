using DomDivan.Models;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace DomDivan;

public class GenerateWord
{
    public static void GenerateWordReceipt(ObservableCollection<CartView> orderItems, Order order)
    {
        string filePath = $"C:\\Users\\gghgg\\Desktop\\vs studio\\Диплом\\Заказ_{order.Id}.docx";

        using (var doc = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
        {
            var mainPart = doc.AddMainDocumentPart();
            mainPart.Document = new Document(new Body());

            var body = mainPart.Document.Body;

            // Добавляем заголовок чека
            var centeredParagraph = CreateParagraphWithProperties($"Заказ №{order.Id} от {DateTime.Now.ToString("dd.MM.yyyy")}г.", true, 20);
            centeredParagraph.ParagraphProperties = new ParagraphProperties()
            {
                Justification = new Justification() { Val = JustificationValues.Center }
            };
            body.AppendChild(centeredParagraph);
            body.AppendChild(CreateParagraphWithProperties("Магазин \"Dom Divan\"", false, 14));
            body.AppendChild(CreateParagraphWithProperties("ИП \"Полянская Е.В.\"", false, 14));
            body.AppendChild(CreateParagraphWithProperties("ИНН 263504326995", false, 14));
            body.AppendChild(CreateParagraphWithProperties("ОГРНИП 305263503300157", false, 14));
            body.AppendChild(CreateParagraphWithProperties("Номер телефона 88008888888", false, 14));

            // Добавляем заголовок "Товары"
            body.AppendChild(new Paragraph(new Run(new Text("")))); // Пустая строка
            centeredParagraph = CreateParagraphWithProperties("Товары", true, 16);
            centeredParagraph.ParagraphProperties = new ParagraphProperties()
            {
                Justification = new Justification() { Val = JustificationValues.Center }
            };
            body.AppendChild(centeredParagraph);

            // Создаем таблицу с товарами
            var table = new Table();

            // Настройки таблицы
            var tableProperties = new TableProperties(
                new TableBorders(
                    new TopBorder() { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder() { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder() { Val = BorderValues.Single, Size = 4 },
                    new RightBorder() { Val = BorderValues.Single, Size = 4 },
                    new InsideHorizontalBorder() { Val = BorderValues.Single, Size = 4 },
                    new InsideVerticalBorder() { Val = BorderValues.Single, Size = 4 }
                ),
                new TableWidth() { Width = "100%", Type = TableWidthUnitValues.Pct }
            );
            table.AppendChild(tableProperties);

            // Заголовки таблицы
            var headerRow = new TableRow();
            headerRow.Append(CreateTableCell("№", true));
            headerRow.Append(CreateTableCell("Название", true));
            headerRow.Append(CreateTableCell("Вариация", true));
            headerRow.Append(CreateTableCell("Цена, руб.", true));
            headerRow.Append(CreateTableCell("Кол-во, шт", true));
            headerRow.Append(CreateTableCell("Скидка, %", true));
            headerRow.Append(CreateTableCell("Сумма, руб.", true));
            table.Append(headerRow);

            // Добавляем товары
            for (int i = 0; i < orderItems.Count; i++)
            {
                var row = new TableRow();
                row.Append(CreateTableCell((i + 1).ToString(), false));
                row.Append(CreateTableCell(orderItems[i].Title, false));
                row.Append(CreateTableCell($"{orderItems[i].TitleVariant}", false));
                row.Append(CreateTableCell(orderItems[i].DiscountPrice.ToString("N2"), false));
                row.Append(CreateTableCell(orderItems[i].Quantity.ToString(), false));
                row.Append(CreateTableCell(orderItems[i].Discount?.ToString(), false));
                row.Append(CreateTableCell((orderItems[i].Quantity * orderItems[i].DiscountPrice).ToString("N2"), false));
                table.Append(row);
            }

            body.AppendChild(table);

            // Добавляем итоговую информацию
            body.AppendChild(new Paragraph(new Run(new Text("")))); // Пустая строка
            body.AppendChild(CreateParagraphWithProperties($"Итого: {orderItems.Sum(o => o.Quantity * o.DiscountPrice).ToString("N2")} руб.", false, 14));
            body.AppendChild(CreateParagraphWithProperties($"Всего товаров: {orderItems.Sum(o => o.Quantity)} шт, на сумму: {orderItems.Sum(o => o.Quantity * o.DiscountPrice).ToString("N2")} руб.", false, 14));
            //адрес и дата доставки, ФИО покупателя
            body.AppendChild(CreateParagraphWithProperties($"ФИО покупателя: {order.CustomerName}", false, 14));
            body.AppendChild(CreateParagraphWithProperties($"Номер телефона покупателя: {order.PhoneNumber}", false, 14));
            body.AppendChild(CreateParagraphWithProperties($"Адрес доставки: {order.DeliveryAddress}", false, 14));
            body.AppendChild(CreateParagraphWithProperties($"Дата доставки: {order.DeliveryDate.ToString("dd.MM.yyyy")}г.", false, 14));
            body.AppendChild(CreateParagraphWithProperties($"Продавец ___________________", false, 14));

            doc.Save();

            Process.Start(new ProcessStartInfo(filePath)
            {
                UseShellExecute = true
            });
        }
    }

    private static Paragraph CreateParagraphWithProperties(string text, bool isBold, int fontSize)
    {
        var paragraph = new Paragraph();
        var run = new Run();

        var runProperties = new RunProperties();
        if (isBold)
        {
            runProperties.Append(new Bold());
        }
        runProperties.Append(new FontSize() { Val = (fontSize * 2).ToString() }); // В OpenXML размер в удвоенных пунктах

        run.Append(runProperties);
        run.Append(new Text(text));

        paragraph.Append(run);
        return paragraph;
    }

    private static TableCell CreateTableCell(string text, bool isHeader)
    {
        var cell = new TableCell();
        var paragraph = new Paragraph();
        var run = new Run();

        if (isHeader)
        {
            run.Append(new RunProperties(new Bold()));
        }

        run.Append(new Text(text));
        paragraph.Append(run);
        cell.Append(paragraph);

        // Настройки ячейки
        var cellProperties = new TableCellProperties(
            new TableCellWidth() { Type = TableWidthUnitValues.Auto }
        );
        cell.Append(cellProperties);

        return cell;
    }
}