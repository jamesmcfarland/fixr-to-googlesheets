using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;


class GoogleSheetsHelper
{
    private SheetsService service;
    public GoogleSheetsHelper(string pathToKeyFile, string applicationName)
    {

        GoogleCredential credential = GoogleCredential.FromFile("key.json").CreateScoped(new string[] { SheetsService.Scope.Spreadsheets });

        service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = applicationName,
        });

    }

    public List<List<String>> getSpreadsheet(string sheet, string range, bool sanitise = false)
    {

        SpreadsheetsResource.ValuesResource.GetRequest request =
                service.Spreadsheets.Values.Get(sheet, range);

        ValueRange response = request.Execute();


        List<List<String>> data = new List<List<string>>();
        if (sanitise)
        {

            foreach (IList<Object> o in response.Values)
            {
                List<String> sanitised = new List<String>();
                foreach (Object o1 in o)
                {
                    sanitised.Add((string)o1.Sanitise());
                }
                data.Add(sanitised);
            }
        }
        else
        {
            foreach (List<object> objList in response.Values)
            {
                List<String> strList = new List<string>();
                foreach (object o in objList)
                {
                    strList.Add((string)o);
                }
                data.Add(strList);
            }

        }

        return data;
    }

    public bool writeSpreadsheet(string sheet, string range, List<List<Object>> values)
    {

        try
        {
            List<IList<object>> converted = new List<IList<object>>();
            values.ForEach(value=>converted.Add(value));

            ValueRange body = new ValueRange()
            {
                Values = converted
            };

            SpreadsheetsResource.ValuesResource.AppendRequest request = service.Spreadsheets.Values.Append(body, sheet, range);
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            AppendValuesResponse result = request.Execute();

            return true;
        }
        catch { return false; }
    }

    




}