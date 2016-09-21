This Package Contains a light weight PDFMaker which is faster than others in market and hastle free. 
Others have issues with most of the hosting where those sites doesn't allow them to run.

You can make PDF by simply returning return new PdfResult() in ASP.NET MVC which will 
show a PDF in the browser or you can use PdfBytes class to get html converted to bytes of pdf.
 
There are multiple overloads available for you to select an data object or a action other than your current one.

Moreover you can pass parameters to specify master layout if any, document size, File Download Name. 

Example: 
using HashMakerSol.PDFMaker;
return new PdfResult( * Centers) { 
      ** MasterViewName = "_PDFLayout", 
     *** DocPageSize = PageSize.A4.Rotate()
}; 

PdfBytes pdfBytes = new PdfBytes() { 
   *** DocPageSize = PageSize.A4
};

pdfBytes.GetPdfBytesArray("<h1>Hello</h1>");
 *  Centers : Is your Object of data. 
**  MasterViewName : Layout of your Action. 
*** DocPageSize : This is to specify size and orientation both. Chose a size and at end call rotate for Landscape or 
Leave as it is for Horizontal.

http://www.hashmakersol.com/
