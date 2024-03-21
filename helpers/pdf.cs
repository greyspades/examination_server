using Aspose.Pdf;
namespace PDFHelpers;
public class PdfToHtmlConverter
{
    public static string ConvertPdfToHtml(string pdfFilePath)
    {
        // Open the PDF document
        Document pdfDocument = new Document(pdfFilePath);

        // Create HTML save options
        HtmlSaveOptions htmlOptions = new HtmlSaveOptions();
        htmlOptions.PartsEmbeddingMode = HtmlSaveOptions.PartsEmbeddingModes.EmbedAllIntoHtml;
        htmlOptions.LettersPositioningMethod = HtmlSaveOptions.LettersPositioningMethods.UseEmUnitsAndCompensationOfRoundingErrorsInCss;
        
        // Convert PDF to HTML
        using (MemoryStream htmlStream = new MemoryStream())
        {
            pdfDocument.Save(htmlStream, htmlOptions);
            return System.Text.Encoding.UTF8.GetString(htmlStream.ToArray());
        }
    }
}
