
public class Settings
{
    public string email { get; set; }="";
    public string password { get; set; }="";
    public string eventId { get; set; }="";
    public string googleSheetsApplicationName { get; set; }="";
    public string googleSheetsMemberSpreadsheetID { get; set; }="";
    public string googleSheetsKeyfilePath { get; set; }="";
    public string googleSheetsMemberSpreadsheetQuery { get; set; }="";
    public string googleSheetsReducedFIXRSheetName { get; set; }="";
    public string googleSheetsFIXRSheetName { get; set; }="";
    public string googleSheetsSUListSheetName { get; set; }="";
    public string runMins { get; set; }="";

    public Settings(){}
}
