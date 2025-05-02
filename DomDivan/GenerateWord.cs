using DomDivan.Models;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.ObjectModel;

namespace DomDivan;

public class GenerateWord
{
    public static void GenerateWordReceipt(ObservableCollection<CartView> order)
    {
        using (var doc = WordprocessingDocument.Create("C:\\Users\\gghgg\\Desktop\\vs studio\\Диплом\\Заказ_1.docx", WordprocessingDocumentType.Document))
        {
            var mainPart = doc.AddMainDocumentPart();
            mainPart.Document = new Document(new Body());

            var body = mainPart.Document.Body;
            body.AppendChild(new Paragraph(new Run(new Text($"Чек заказа №1"))));

            // Создаем таблицу с товарами
            var table = new Table();
            var headerRow = new TableRow();
            headerRow.Append(new TableCell(new Paragraph(new Run(new Text("№")))));
            headerRow.Append(new TableCell(new Paragraph(new Run(new Text("Наименование")))));
            headerRow.Append(new TableCell(new Paragraph(new Run(new Text("Вариация")))));
            // ... остальные заголовки
            table.Append(headerRow);

            for (int i = 0; i < order.Count; i++)
            {
                var row = new TableRow();
                row.Append(new TableCell(new Paragraph(new Run(new Text((i + 1).ToString())))));
                row.Append(new TableCell(new Paragraph(new Run(new Text(order[i].Title)))));
                row.Append(new TableCell(new Paragraph(new Run(new Text($"{order[i].Color}, {order[i].Cloth}")))));
                // ... остальные столбцы
                table.Append(row);
            }

            var TotalRow = new TableRow();
            TotalRow.Append(new TableCell(new Paragraph(new Run(new Text("Итого:")))));
            TotalRow.Append(new TableCell(new Paragraph(new Run(new Text(order.Sum(o=>o.Quantity*o.DiscountPrice).ToString())))));
            table.Append(TotalRow);

            body.AppendChild(table);
            doc.Save();
        }
    }
}
