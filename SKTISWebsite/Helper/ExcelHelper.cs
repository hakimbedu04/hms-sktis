using DocumentFormat.OpenXml.Spreadsheet;
using SpreadsheetLight;

namespace SKTISWebsite.Helper
{
    public static class ExcelHelper
    {
        public static SLStyle GetDefaultExcelStyle(SLDocument slDoc)
        {
            SLStyle style = slDoc.CreateStyle();
            style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            style.Font.FontName = "Calibri";
            style.Font.FontSize = 10;
            return style;
        }
    }
}
