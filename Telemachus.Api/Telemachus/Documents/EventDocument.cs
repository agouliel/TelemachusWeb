using System;
using System.Collections.Generic;
using DocumentCreator.Documents;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using Telemachus.Business.Models.Events;

namespace Telemachus.Documents
{
    public class EventDocument : WordDocument
    {
        private DocumentViewModel _doc { get; set; }
        public EventDocument(string contentPath, DocumentViewModel doc)
        {
            _doc = doc;
            FileName = $"{doc.VesselName} - Statement of facts - {DateTime.Now.ToString("dddd, dd MMMM yyyy")}";
            Operator = new OpenXMLDocumentOperator(_doc.Operator, contentPath);
            Operator.Department = "qsm";
            Version = Compatibilities.Office2010;
            DefaultFont = new DefaultFont()
            {
                FontFamily = "Arial",
                FontSize = 18
            };
            AddHeaderAndFooter();
            PageMargins = new int[] { 720, 720 };
            PageSize = new PageDimensions[] { PageDimensions.A4_Width, PageDimensions.A4_Height };
            //var rows = new List<List<string>>();
            //var rnd = new Random();
            //for (var i = 0; i < 50; i++)
            //{
            //    var values = new List<string>();
            //    for (var j = 0; j < headers.Count; j++)
            //    {
            //        var faker = new Bogus.Faker();
            //        switch (j)
            //        {
            //            case 0:
            //                values.Add(string.Join(" ", faker.Lorem.Words(rnd.Next(3, 5))));
            //                break;
            //            case 1:
            //                var dateTime = faker.Date.Between(DateTime.ParseExact("01/01/2023", "d", null), DateTime.ParseExact("05/01/2023", "d", null));
            //                values.Add(dateTime.ToString("dd/MM/yy"));
            //                values.Add(dateTime.ToString("HH:mm"));
            //                break;
            //            case 3:
            //                var numOfValues = rnd.Next(1, 10);
            //                numOfValues = numOfValues > 8 ? 1 : 0;
            //                values.Add(faker.Lorem.Sentences(numOfValues));
            //                break;
            //            default:
            //                break;
            //        }
            //    }
            //    rows.Add(values);
            //}
            AppendHeadTable();
            Body.Append(Break());
            var headers = new List<string>(new string[] { "SOF", "DD/MM/YY", "Local time", "Remarks" });
            AppendFactsTable(headers);
            Body.Append(Break());
            AppendRemarksTable();
            Body.Append(Break());
            Body.Append(Break());
            AppendSignTable();
            AppendHeaderAndFooter();
            AddPackageProperties();
            SetReadOnly(true);
        }
        protected TableCell CreateHeadCell(string value = " ", bool strong = false, string offset = "")
        {
            var tableCellProperties = new TableCellProperties(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Top },
                new TableCellBorders(TableBorderProps()));
            if (!string.IsNullOrEmpty(offset))
            {
                tableCellProperties.Append(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = offset });
            }
            else
            {
                tableCellProperties.Append(new TableCellWidth { Type = TableWidthUnitValues.Auto });
            }
            var tableCell = new TableCell(tableCellProperties);
            var paragraphPropertites = new ParagraphProperties(new SpacingBetweenLines() { Line = "220", LineRule = LineSpacingRuleValues.Exact, AfterAutoSpacing = false, Before = "0", After = "0" });
            var runProperties = new RunProperties(new FontSize() { Val = "20" });
            if (strong)
            {
                runProperties.Append(new Bold());
                tableCell.Append(new Paragraph(paragraphPropertites, new Run(runProperties, new Text(value))));
            }
            else
            {
                tableCell.Append(new Paragraph(paragraphPropertites, new Run(runProperties, new Text(value))));
            }
            return tableCell;
        }
        private void AppendSignTable()
        {
            var table = new Table();
            var tableProperties = new TableProperties(
                //new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Pct },
                //new TableStyle() { Val = "TableGrid" },
                new TableJustification() { Val = TableRowAlignmentValues.Center },
                new TableWidth() { Width = "4500", Type = TableWidthUnitValues.Pct },
                new TableLayout() { Type = TableLayoutValues.Fixed },
                new TableCellMarginDefault(
                new TopMargin() { Width = "0", Type = TableWidthUnitValues.Dxa },
                new StartMargin() { Width = "110", Type = TableWidthUnitValues.Dxa },
                new BottomMargin() { Width = "0", Type = TableWidthUnitValues.Dxa },
                new EndMargin() { Width = "110", Type = TableWidthUnitValues.Dxa }),
                new TableCellSpacing() { Width = "30", Type = TableWidthUnitValues.Dxa }
            );
            table.Append(tableProperties);
            var tableRow = new TableRow();
            for (var i = 0; i < 3; i++)
            {
                tableRow.Append(new TableCell(new TableCellProperties(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Bottom }), new Paragraph(new ParagraphProperties(new ParagraphBorders(new BottomBorder()
                {
                    Val =
                    new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 8
                }), new Justification() { Val = JustificationValues.Center }, new SpacingBetweenLines() { Before = "0", After = "0" }), new Run(new RunProperties(new FontSize() { Val = "20" }), new Text("")))));
            }
            table.Append(tableRow);
            tableRow = new TableRow();
            for (var i = 0; i < 3; i++)
            {
                var text = "";
                switch (i)
                {
                    case 0:
                        text = "Terminal/Shipper/Consignee";
                        break;
                    case 1:
                        text = "Agent";
                        break;
                    case 2:
                        text = "Master";
                        break;
                }
                tableRow.Append(new TableCell(new TableCellProperties(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Top }), new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "250", LineRule = LineSpacingRuleValues.Exact, Before = "0", After = "0" }, new Justification() { Val = JustificationValues.Center }), new Run(new RunProperties(new Bold(), new FontSize() { Val = "20" }), new Text(text)))));
            }
            table.Append(tableRow);
            Body.Append(table);
        }
        private void AppendRemarksTable()
        {
            var table = new Table();
            var tableProperties = new TableProperties(
                new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Pct },
                new TableStyle() { Val = "TableGrid" },
                new TableCellMarginDefault(
                new TopMargin() { Width = "100", Type = TableWidthUnitValues.Dxa },
                new StartMargin() { Width = "100", Type = TableWidthUnitValues.Dxa },
                new BottomMargin() { Width = "100", Type = TableWidthUnitValues.Dxa },
                new EndMargin() { Width = "100", Type = TableWidthUnitValues.Dxa })
            );
            table.Append(tableProperties);
            var tableRow = new TableRow(new TableCell(new TableCellProperties(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Bottom }, new TableCellMargin(
                        new TopMargin() { Width = "100", Type = TableWidthUnitValues.Dxa },
                        new BottomMargin() { Width = "100", Type = TableWidthUnitValues.Dxa }
                        )), new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "300", LineRule = LineSpacingRuleValues.Exact, AfterAutoSpacing = false, Before = "0", After = "0" }), new Run(new RunProperties(new Bold(), new FontSize() { Val = "20" }), new Text("Remarks (delays, stoppages etc.)")))));
            table.Append(tableRow);
            var tableCell = new TableCell(new TableCellProperties(new TableCellBorders(TableBorderProps())));
            tableCell.Append(new Paragraph(parseText(_doc.Remarks)));
            for (var i = 0; i < 2; i++)
            {
                tableCell.Append(Break());
            }
            tableRow = new TableRow(tableCell);
            table.Append(tableRow);
            Body.Append(table);
        }
        protected void AppendHeadTable()
        {
            var table = new Table();
            var tableProperties = new TableProperties(
                new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Pct },
                new TableStyle() { Val = "TableGrid" },
                new TableCellMarginDefault(
                new TopMargin() { Width = "100", Type = TableWidthUnitValues.Dxa },
                new StartMargin() { Width = "100", Type = TableWidthUnitValues.Dxa },
                new BottomMargin() { Width = "100", Type = TableWidthUnitValues.Dxa },
                new EndMargin() { Width = "100", Type = TableWidthUnitValues.Dxa })
            );
            table.Append(tableProperties);
            var tableRow = new TableRow();
            tableRow.Append(CreateHeadCell("Vessel:", true, "950"));
            tableRow.Append(CreateHeadCell(_doc.VesselName));
            tableRow.Append(CreateHeadCell("Date:", true, "550"));
            tableRow.Append(CreateHeadCell(_doc.FormattedDate));
            table.Append(tableRow);
            tableRow = new TableRow();
            tableRow.Append(CreateHeadCell("Operation / Grade:", true, "950"));
            tableRow.Append(CreateHeadCell(_doc.OperationGrade));
            tableRow.Append(CreateHeadCell("Voyage:", true, "550"));
            tableRow.Append(CreateHeadCell(_doc.Voyage));
            table.Append(tableRow);
            tableRow = new TableRow();
            tableRow.Append(CreateHeadCell("Port:", true, "950"));
            tableRow.Append(CreateHeadCell(_doc.PortName));
            tableRow.Append(CreateHeadCell("Terminal:", true, "550"));
            tableRow.Append(CreateHeadCell(_doc.Terminal));
            table.Append(tableRow);
            Body.Append(table);
        }
        protected void AppendFactsTable(List<string> headers)
        {
            var table = new Table();
            var tableProperties = new TableProperties(
                new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Pct },
                new TableStyle() { Val = "TableGrid" },
                new TableCellMarginDefault(
                new TopMargin() { Width = "50", Type = TableWidthUnitValues.Dxa },
                new StartMargin() { Width = "100", Type = TableWidthUnitValues.Dxa },
                new BottomMargin() { Width = "50", Type = TableWidthUnitValues.Dxa },
                new EndMargin() { Width = "100", Type = TableWidthUnitValues.Dxa })
            );
            table.Append(tableProperties);
            table.Append(CreateFactRow(headers, true));

            foreach (var e in _doc.Facts)
            {
                var rowValues = new List<string>
                {
                    e.EventTypeId != 16 ? e.Name : e.CustomEventName,
                    !e.HiddenDate ? e.DateFormatted : "",
                    !e.HiddenDate ? e.TimeFormatted : "",
                    e.Remarks
                };
                table.Append(CreateFactRow(rowValues));
            }
            for (var i = 0; i < 8; i++)
            {
                table.Append(CreateFactRow(new List<string>() { " ", " ", " ", " " }));
            }
            Body.Append(table);
        }
        private TableRow CreateFactRow(List<string> values, bool isHeader = false)
        {
            var tableRow = new TableRow();
            for (var i = 0; i < values.Count; i++)
            {
                var tableCellProperties = new TableCellProperties(new TableCellVerticalAlignment() { Val = isHeader ? TableVerticalAlignmentValues.Bottom : TableVerticalAlignmentValues.Top });
                if (isHeader)
                {
                    tableCellProperties.Append(new TableCellMargin(
                        new TopMargin() { Width = "100", Type = TableWidthUnitValues.Dxa },
                        new BottomMargin() { Width = "100", Type = TableWidthUnitValues.Dxa }
                        ));
                }
                switch (i)
                {
                    case 0:
                        tableCellProperties.Append(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "2000" });
                        break;
                    case 1:
                        tableCellProperties.Append(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "500" });
                        break;
                    case 2:
                        tableCellProperties.Append(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "700" });
                        break;
                    default:
                        tableCellProperties.Append(new TableCellWidth { Type = TableWidthUnitValues.Auto });
                        break;
                }
                if (!isHeader)
                {
                    tableCellProperties.Append(new TableCellBorders(TableBorderProps()));
                }
                var tableCell = new TableCell(tableCellProperties);
                var paragraphPropertites = new ParagraphProperties(new SpacingBetweenLines() { Line = "190", LineRule = LineSpacingRuleValues.Exact, AfterAutoSpacing = false, Before = "0", After = "0" });
                if (i > 0 && (i < values.Count - 1))
                {
                    paragraphPropertites.Append(new Justification() { Val = JustificationValues.Center });
                }
                var runProperties = new RunProperties();

                if (isHeader || i == 0) runProperties.Append(new Bold());
                if (isHeader) runProperties.Append(new FontSize() { Val = "20" });
                tableCell.Append(new Paragraph(paragraphPropertites, parseText(values[i], runProperties)));
                tableRow.Append(tableCell);
            }
            return tableRow;
        }
        private OpenXmlElement[] TableBorderProps(uint size = 4)
        {
            return new OpenXmlElement[]
            {
                new TopBorder()
                {
                    Val =
                    new EnumValue<BorderValues>(BorderValues.Single),
                    Size = size
                },
                new BottomBorder()
                {
                    Val =
                    new EnumValue<BorderValues>(BorderValues.Single),
                    Size = size
                },
                new LeftBorder()
                {
                    Val =
                    new EnumValue<BorderValues>(BorderValues.Single),
                    Size = size
                },
                new RightBorder()
                {
                    Val =
                    new EnumValue<BorderValues>(BorderValues.Single),
                    Size = size
                },
                new InsideHorizontalBorder()
                {
                    Val =
                    new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 0
                },
                new InsideVerticalBorder()
                {
                    Val =
                    new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 0
                }
            };
        }
    }
}
