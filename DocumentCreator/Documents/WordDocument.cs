using System;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using BaseDocument = DocumentFormat.OpenXml.Wordprocessing.Document;
using D = DocumentFormat.OpenXml.Drawing;
using DP = DocumentFormat.OpenXml.Drawing.Pictures;
using WD = DocumentFormat.OpenXml.Drawing.Wordprocessing;

namespace DocumentCreator.Documents
{
    public enum PageDimensions
    {
        A4_Width = 11907,
        A4_Height = 16839
    }
    public enum Compatibilities
    {
        Office2007 = 12,
        Office2010 = 14,
        Office2013 = 15,
        Next = 16
    }
    public class DefaultFont
    {
        public string FontFamily { get; set; } = "Times New Roman";
        public int FontSize { get; set; } = 24;
    }
    public abstract class WordDocument : Document
    {
        private WordprocessingDocument _document;
        private MainDocumentPart _mainDocument;
        private BaseDocument _baseDocument;
        private Body _body;
        private HeaderPart _header;
        private FooterPart _footer;
        private SectionProperties _bodySectionProperties;
        protected BaseDocument Document { get => _baseDocument!; }
        protected MainDocumentPart Main { get => _mainDocument; }
        protected Body Body { get => _body!; }
        protected HeaderPart Header { get => _header!; }
        protected FooterPart Footer { get => _footer!; }
        protected SectionProperties BodySectionProperties { get => _bodySectionProperties; }
        public new OpenXMLDocumentOperator Operator { get; set; } = null!;
        public WordDocument()
        {
            _document = WordprocessingDocument.Create(_stream, WordprocessingDocumentType.Document, true);
            _mainDocument = _document.AddMainDocumentPart();
            Main.Document = new BaseDocument();
            _baseDocument = Main.Document;
            Document.Append(new Body());
            _body = Document.Body;
            _bodySectionProperties = new SectionProperties();
            Body.Append(_bodySectionProperties);
        }

        protected void AddPackageProperties()
        {
            _document.PackageProperties.Creator = Operator.Title;
            _document.PackageProperties.Created = _dateCreated;
            var encryptedPassword = "";
            foreach (var number in _password)
            {
                encryptedPassword += number - 1;
            }
            _document.PackageProperties.Description = "Password hash: " + encryptedPassword;
            _document.PackageProperties.Title = "Statement of Facts";
        }
        protected void AppendHeaderAndFooter()
        {
            AppendHeader();
            AppendFooter();
        }
        protected Run parseText(string textualData, RunProperties properties = null)
        {
            var run = new Run();

            if (properties != null)
            {
                run.Append(properties);
            }
            if (textualData == null)
            {
                textualData = "";
            }
            string[] newlineArray = { Environment.NewLine, "\n", "\r\n", "\n\r" };
            string[] textArray = textualData.Split(newlineArray, StringSplitOptions.None);

            bool first = true;

            foreach (string line in textArray)
            {
                if (!first)
                {
                    run.Append(new Break());
                }

                first = false;

                Text txt = new Text();
                txt.Text = line;
                run.Append(txt);
            }
            return run;
        }

        private void AppendHeader()
        {
            var logoDrawing = GetOperatorLogoDrawing();
            var hyperLink = Header.AddHyperlinkRelationship(new Uri($"mailto:{Operator.Email}"), true);
            Header.Header.Append(
            new Paragraph(
                new ParagraphProperties(
                    new SpacingBetweenLines() { Before = "0", After = "0" },
                new Justification() { Val = JustificationValues.Center }),
                new Run(
                    logoDrawing,
                    new RunProperties()
                    {
                        RunFonts = new RunFonts()
                        {
                            Ascii = "Sylfaen"
                        },
                        Bold = new Bold(),
                        FontSize = new FontSize() { Val = "50" },
                    }, new Text(Operator.Title)
                    )),
                    new Paragraph(
                        new ParagraphProperties(
                            new SpacingBetweenLines() { After = "0" },
                            new Justification() { Val = JustificationValues.Center }
                        ),
                        new Run(
                            new RunProperties()
                            {
                                RunFonts = new RunFonts()
                                {
                                    Ascii = "Sylfaen"
                                },
                                FontSize = new FontSize() { Val = "16" },
                            },
                            new Text($"{Operator.Address}, E-mail: ") { Space = SpaceProcessingModeValues.Preserve }),
                            new Hyperlink(
                                new Run(
                                    new RunProperties()
                                    {
                                        RunFonts = new RunFonts()
                                        {
                                            Ascii = "Sylfaen"
                                        },
                                        FontSize = new FontSize() { Val = "16" },
                                        RunStyle = new RunStyle { Val = "Hyperlink" },
                                        Underline = new Underline { Val = UnderlineValues.Single },
                                        Color = new Color { ThemeColor = ThemeColorValues.Hyperlink },
                                    },
                                    new Text(Operator.Email) { Space = SpaceProcessingModeValues.Preserve }
                                    )
                            )
                            { Id = hyperLink.Id },
                            new Break()
                        )
                   );
        }
        protected Paragraph Break()
        {
            return new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Before = "0", After = "0" }), new Run(new Break()));
        }
        private void AppendFooter()
        {
            var run = new Run();
            run.AppendChild(new RunProperties(new RunFonts()
            {
                Ascii = "Sylfaen"
            }));
            run.Append(
            new Break(),
            new Text("Page ") { Space = SpaceProcessingModeValues.Preserve },
            new SimpleField() { Instruction = "PAGE" },
            new Text(" of ") { Space = SpaceProcessingModeValues.Preserve },
            new SimpleField() { Instruction = "NUMPAGES" }
            );
            Footer.Footer.Append(
                new Paragraph(
                new ParagraphProperties(
                    new ParagraphStyleId() { Val = "Footer" },
                    new Justification() { Val = JustificationValues.Center },
                run
                )));
        }
        private Drawing GetOperatorLogoDrawing()
        {
            var operatorLogoId = GetOperatorLogoId();
            return new Drawing(
                new WD.Inline(
                    new WD.Extent() { Cx = Operator.ImageSize.Width, Cy = Operator.ImageSize.Height },
                    new WD.EffectExtent()
                    {
                        LeftEdge = Operator.ImageOffset.LeftEdge,
                        TopEdge = Operator.ImageOffset.TopEdge,
                        RightEdge = Operator.ImageOffset.RightEdge,
                        BottomEdge = Operator.ImageOffset.BottomEdge
                    },
                    new WD.DocProperties() { Id = 1U, Name = Operator.FileName },
                    new WD.NonVisualGraphicFrameDrawingProperties(
                        new D.GraphicFrameLocks() { NoChangeAspect = true }
                    ),
                    new D.Graphic(
                        new D.GraphicData(
                            new DP.Picture(
                                new DP.NonVisualPictureProperties(
                                    new DP.NonVisualDrawingProperties() { Id = 0U, Name = Operator.FileName },
                                    new DP.NonVisualPictureDrawingProperties()
                                ),
                                new DP.BlipFill(
                                    new D.Blip(
                                        new D.BlipExtensionList(
                                            new D.BlipExtension() { Uri = new Guid().ToString() }
                                        )
                                    )
                                    {
                                        Embed = operatorLogoId,
                                        CompressionState = D.BlipCompressionValues.Print
                                    },
                                    new D.Stretch(new D.FillRectangle())
                                ),
                                new DP.ShapeProperties(
                                    new D.Transform2D(
                                        new D.Offset() { X = 0L, Y = 0L },
                                        new D.Extents()
                                        {
                                            Cx = Operator.ImageSize.Width,
                                            Cy = Operator.ImageSize.Height
                                        }
                                    ),
                                    new D.PresetGeometry(new D.AdjustValueList())
                                    {
                                        Preset = D.ShapeTypeValues.Rectangle
                                    }
                                )
                            )
                        )
                        {
                            Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture"
                        }
                    )
                )
                {
                    DistanceFromTop = 0,
                    DistanceFromBottom = 0,
                    DistanceFromLeft = 0,
                    DistanceFromRight = 0
                }
            );

        }
        private string GetOperatorLogoId()
        {
            var fs = Operator.GetOperatorImageStream();
            if (fs == null) return null;
            var imagePart = Header.AddImagePart(ImagePartType.Jpeg);
            imagePart.FeedData(fs);
            var operatorLogoId = Header.GetIdOfPart(imagePart);
            fs.Close();
            return operatorLogoId;
        }
        protected void AddHeaderAndFooter()
        {
            AddHeader();
            AddFooter();
        }
        protected void AddHeader()
        {
            _header = Main.AddNewPart<HeaderPart>();
            Header!.Header = new Header();
            var headerRef = new HeaderReference() { Type = HeaderFooterValues.Default, Id = Main.GetIdOfPart(Header) };
            BodySectionProperties?.InsertAt(headerRef, 0);
        }
        protected void AddFooter()
        {
            _footer = Main.AddNewPart<FooterPart>();
            Footer!.Footer = new Footer();
            var footerRef = new FooterReference() { Type = HeaderFooterValues.Default, Id = Main.GetIdOfPart(Footer) };
            BodySectionProperties?.InsertAt(footerRef, 0);
        }
        public int[] PageMargins
        {
            set
            {
                BodySectionProperties.RemoveAllChildren<PageMargin>();
                var pageMargins = new PageMargin();
                pageMargins.Left = Convert.ToUInt32(value[0]);
                pageMargins.Right = Convert.ToUInt32(value[0]);
                pageMargins.Bottom = Convert.ToInt32(value[1]);
                pageMargins.Top = Convert.ToInt32(value[1]);
                BodySectionProperties.Append(pageMargins);
            }
        }
        public PageDimensions[] PageSize
        {
            set
            {
                BodySectionProperties.RemoveAllChildren<PageSize>();
                var pageSize = new PageSize();
                pageSize.Width = Convert.ToUInt32(value[0]);
                pageSize.Height = Convert.ToUInt32(value[1]);
                BodySectionProperties.Append(pageSize);
            }
        }
        public Compatibilities Version
        {
            set
            {
                var compatibilitySetting = new CompatibilitySetting()
                {
                    Name = new EnumValue<CompatSettingNameValues>(CompatSettingNameValues.CompatibilityMode),
                    Val = new StringValue(value.ToString("D")),
                    Uri = new StringValue("http://schemas.microsoft.com/office/word")
                };
                SetCompatibilitySettings(compatibilitySetting);
            }
        }
        public DefaultFont DefaultFont
        {
            set
            {
                DocDefaults defaults = new DocDefaults()
                {
                    RunPropertiesDefault = new RunPropertiesDefault()
                    {
                        RunPropertiesBaseStyle = new RunPropertiesBaseStyle()
                        {
                            FontSize = new FontSize() { Val = new StringValue(value.FontSize.ToString()) },
                            RunFonts = new RunFonts()
                            {
                                Ascii = new StringValue(value.FontFamily),
                                ComplexScript = new StringValue(value.FontFamily)
                            },
                        }
                    }
                };
                SetDefaultStyles(defaults);
            }
        }
        protected void SetCompatibilitySettings(CompatibilitySetting compatibilitySetting)
        {
            var documentSettingsPart = _mainDocument.DocumentSettingsPart;
            if (documentSettingsPart == null)
            {
                documentSettingsPart = _mainDocument.AddNewPart<DocumentSettingsPart>();
                if (documentSettingsPart.Settings == null)
                {
                    documentSettingsPart.Settings = new Settings();
                }
            }
            documentSettingsPart.Settings.RemoveAllChildren<Compatibility>();
            var compatibility = new Compatibility(compatibilitySetting);
            documentSettingsPart.Settings.AddChild(compatibility);
            documentSettingsPart.Settings.Save();
        }
        protected void SetReadOnly(bool readOnly)
        {
            var documentSettingsPart = _mainDocument.DocumentSettingsPart;
            if (documentSettingsPart == null)
            {
                documentSettingsPart = _mainDocument.AddNewPart<DocumentSettingsPart>();
                if (documentSettingsPart.Settings == null)
                {
                    documentSettingsPart.Settings = new Settings();
                }
            }
            documentSettingsPart.Settings.RemoveAllChildren<DocumentProtection>();
            if (readOnly)
            {
                var password = "";
                foreach (var number in _password)
                {
                    password += number;
                }
                var protection = DocumentSecurity.GetProtectionProperties(password);
                documentSettingsPart.Settings.AddChild(protection);
            }
            documentSettingsPart.Settings.Save();
        }
        protected void SetDefaultStyles(DocDefaults defaults)
        {
            var styleDefinitionsPart = _mainDocument.StyleDefinitionsPart;
            if (styleDefinitionsPart == null)
            {
                styleDefinitionsPart = _mainDocument.AddNewPart<StyleDefinitionsPart>();
                if (styleDefinitionsPart.Styles == null)
                {
                    styleDefinitionsPart.Styles = new Styles();
                }
            }
            styleDefinitionsPart.Styles.DocDefaults = defaults;
        }
        public void Close()
        {
            _document?.Dispose();
        }
    }
    public class OpenXMLDocumentOperator : DocumentOperator
    {
        public ImageOffset ImageOffset { get; private set; }
        public ImageSize ImageSize { get; private set; }
        public OpenXMLDocumentOperator(Operators o, string contentPath) : base(o, contentPath)
        {
            ImageSize = new ImageSize(64, Operator == Operators.Ionia ? 44 : 67);
            ImageOffset = Operator == Operators.Ionia ? new ImageOffset(0, 10, 20, 0) : new ImageOffset(0, 0, 10, 0);
        }
    }
    public class ImageOffset
    {
        public long LeftEdge { get; set; }
        public long TopEdge { get; set; }
        public long RightEdge { get; set; }
        public long BottomEdge { get; set; }
        private const int Multiplier = 9525;
        public ImageOffset(int leftEdge = 0, int topEdge = 0, int rightEdge = 0, int bottomEdge = 0)
        {
            LeftEdge = leftEdge * Multiplier;
            TopEdge = topEdge * Multiplier;
            RightEdge = rightEdge * Multiplier;
            BottomEdge = bottomEdge * Multiplier;

        }
    }
    public class ImageSize
    {
        private const int Multiplier = 9525;
        public long Width { get; set; }
        public long Height { get; set; }
        public ImageSize(int width, int height)
        {
            Width = width * Multiplier;
            Height = height * Multiplier;
        }
    }
}
