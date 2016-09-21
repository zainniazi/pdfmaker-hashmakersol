using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;
using System.IO;
using System.Linq;

namespace HashMakerSol.PDFMaker
{
    public class PdfBytes
    {
        private Rectangle _pageSize;

        public Rectangle DocPageSize
        {
            get { return (_pageSize ?? PageSize.A4); }
            set { _pageSize = value; }
        }

        public byte[] GetPdfBytesArray(string html)
        {
            html = Constants.FormatImageLinks(html);

            using (var ms = new MemoryStream())
            using (var document = new Document(DocPageSize, 10, 10, 5, 0))
            {
                using (PdfWriter writer = PdfWriter.GetInstance(document, ms))
                {
                    document.Open();
                    using (TextReader xmlString = new StringReader(html))
                    {
                        //Set factories
                        HtmlPipelineContext htmlContext = new HtmlPipelineContext(null);
                        htmlContext.SetTagFactory(Tags.GetHtmlTagProcessorFactory());
                        //Set css
                        ICSSResolver cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(false);
                        foreach (var item in Constants.GetCSSLinks(html).ToList())
                            cssResolver.AddCssFile(item, true);

                        //Export
                        IPipeline pipeline = new CssResolverPipeline(cssResolver, new HtmlPipeline(htmlContext, new PdfWriterPipeline(document, writer)));
                        var worker = new XMLWorker(pipeline, true);
                        var xmlParse = new XMLParser(true, worker);
                        xmlParse.Parse(xmlString);
                        xmlParse.Flush();
                    }
                    document.Close();
                }
                return ms.ToArray();
            }
        }
    }
}