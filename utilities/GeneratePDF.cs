using System;
using QuestPDF;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

using SMIS.models;

/// <summary>
/// Overall, This Code Base Will Generate PDFs 
/// This Feature Able To Get All Student In The List
/// And Put The Information All In Printable PDF
/// It Uses QuestPDF With Community License
/// </summary>

namespace SMIS.utilities
{
    public sealed class GeneratePDF : IDocument
    {
        private readonly IEnumerable<Student> _students;
        public GeneratePDF(IEnumerable<Student> students)
        {
            _students = students;    
        }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(0.5f, Unit.Inch);

                page.Header().Text("STUDENT INFORMATION TABLE")
                    .FontFamily("Times New Roman")
                    .FontSize(20)
                    .ExtraBold()
                    .AlignCenter();

                page.Content().Element(ComposeContent);
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.PaddingVertical(40).Column(col =>
            {
                col.Item().Element(ComposeTable);
            });
        }

        private void ComposeTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(col =>
                {
                    col.RelativeColumn(); // Student's ID
                    col.RelativeColumn(4); // Student's Name
                    col.RelativeColumn(); // Student's Gender
                    col.RelativeColumn(); // Student's Age
                    col.RelativeColumn(); // Student's Strand
                    col.RelativeColumn(4); // Student's Address
                });

                table.Header(head =>
                {
                    head.Cell().Element(StyleHeader).AlignCenter().Text("ID");
                    head.Cell().Element(StyleHeader).AlignCenter().Text("NAME");
                    head.Cell().Element(StyleHeader).AlignCenter().Text("SEX");
                    head.Cell().Element(StyleHeader).AlignCenter().Text("AGE");
                    head.Cell().Element(StyleHeader).AlignCenter().Text("STRAND");
                    head.Cell().Element(StyleHeader).AlignCenter().Text("ADDRESS");

                    foreach (var student in _students) 
                    {
                        table.Cell().Element(StyleData).AlignCenter().Text(student.id.ToString());
                        table.Cell().Element(StyleData).AlignCenter().Text(student.name.ToUpperInvariant());
                        table.Cell().Element(StyleData).AlignCenter().Text(student.sex.ToString());
                        table.Cell().Element(StyleData).AlignCenter().Text(student.age.ToString());
                        table.Cell().Element(StyleData).AlignCenter().Text(student.strand.ToString());
                        table.Cell().Element(StyleData).AlignCenter().Text(student.address.ToUpperInvariant());
                    }
                });
            });
        }

        private static IContainer StyleHeader(IContainer container)
        {
            return container.DefaultTextStyle(x => x.FontSize(14).Bold())
                .PaddingVertical(10)
                .BorderBottom(1)
                .BorderColor(Colors.Black);
        }

        private static IContainer StyleData(IContainer container)
        {
            return container.DefaultTextStyle(x => x.FontSize(11).SemiBold())
                .PaddingVertical(5)
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Darken3);
        }
    }
}