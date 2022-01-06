using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;


Console.WriteLine("Starting...\nWorking directory:");
Console.WriteLine(Environment.CurrentDirectory);



var settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("settings.json"));
if (settings == null) settings = new Settings();

if (settings.email == "" || settings.password == "" || settings.eventId == "" || settings.googleSheetsApplicationName == "" || settings.googleSheetsMemberSpreadsheetID == "" || settings.googleSheetsKeyfilePath == "" || settings.googleSheetsMemberSpreadsheetQuery == "" || settings.googleSheetsReducedFIXRSheetName == "" || settings.googleSheetsFIXRSheetName == "" || settings.googleSheetsSUListSheetName == "" || settings.runMins == "")
{
    Console.WriteLine("Settings file contains empty settings\nplease fix it, or modify the code to support whatever changes you have made!\nQuitting...");
    Environment.Exit(1);
}

if (!File.Exists("key.json"))
{
    Console.WriteLine("Google keyfile not present (key.json)\nplease see the below link on how to configure a service account\nhttps://developers.google.com/workspace/guides/create-credentials#service-account");
    Environment.Exit(1);
}

string downloadDir = Path.Combine(Environment.CurrentDirectory, "Downloads");

ChromeDriver driver = null;

try
{


    while (true)
    {
        Console.WriteLine($"Starting run at {DateTime.Now.ToLongTimeString()} \nConfiguring chrome...");

        new DriverManager().SetUpDriver(new ChromeConfig(), VersionResolveStrategy.MatchingBrowser);
        var chromeOptions = new ChromeOptions();
        chromeOptions.AddUserProfilePreference("download.folderList", 2);
        chromeOptions.AddUserProfilePreference("download.default_directory", downloadDir);
        chromeOptions.AddUserProfilePreference("download.prompt_for_download", false);
        chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");
        chromeOptions.AddArgument("--headless");

        Console.WriteLine("Starting chrome");
        driver = new ChromeDriver(chromeOptions);
        driver.Navigate().GoToUrl("https://organiser.fixr.co/login?next=/accounts");




        Console.WriteLine("Setting up download directory...");
        if (!Directory.Exists(downloadDir))
        {
            Directory.CreateDirectory(downloadDir);
        }

        foreach (string path in Directory.GetFiles(downloadDir))
        {
            File.Delete(path);
        }

        Console.WriteLine("Logging in to FIXR...");
        var emailInput = driver.FindElement(By.Name("email"));
        var passwordInput = driver.FindElement(By.Name("password"));
        emailInput.SendKeys(settings.email);
        passwordInput.SendKeys(settings.password);
        emailInput.Submit();

        Thread.Sleep(2000);

        Console.WriteLine("Logged in");
        var divMyAccounts = driver.FindElement(By.ClassName("sc-gKckTs"));

        divMyAccounts.Click();
        Thread.Sleep(1000);
        driver.Navigate().GoToUrl(String.Format("https://organiser.fixr.co/events/{0}/attendees", settings.eventId));
        Thread.Sleep(1000);
        var btnCSVDownload = driver.FindElement(By.ClassName("sc-gKckTs"));
        Console.WriteLine("Downloading CSV");
        btnCSVDownload.Click();
        var divDownload = driver.FindElement(By.ClassName("sc-kLwgWK"));
        divDownload.Click();
        Thread.Sleep(3000);
        Console.WriteLine("Done on FIXR.co! Quitting chrome..");
        driver.Quit();

        string csvDir = Path.Combine(downloadDir, "attendees.csv");

        File.Move(Directory.GetFiles(downloadDir)[0], csvDir);




        Console.WriteLine("Processing data...");
        string[] csvData = File.ReadAllLines(csvDir);


        List<QCSFixrAttendeeData> fixrData = new List<QCSFixrAttendeeData>();

        bool first = true;

        foreach (string s in csvData)
        {
            //Skip the title line
            if (first)
            {
                first = false;
                continue;
            }

            //Process each line into an object

            //Split by the csv deliminator (comma , )
            string[] s1 = s.Split(",");
            fixrData.Add(new QCSFixrAttendeeData()
            {
                FirstName = s1[0],
                LastName = s1[1],
                Email = s1[2],
                DOB = s1[3],
                Mobile = s1[4],
                SoldAt = s1[5],
                EntryStatus = s1[6],
                PeopleIn = s1[7],
                EntryTime = s1[8],
                EventID = s1[9],
                EventName = s1[10],
                TicketTypeID = s1[11],
                TicketTypeName = s1[12],
                TicketTypeCategory = s1[13],
                Price = s1[14],
                Currency = s1[15],
                TicketReference = s1[16],
                Course = s1[17],
                QUBEmail = s1[18],
                StudentNo = s1[19],
                Year = s1[20],

            });
        }

        Console.WriteLine("CSV Parse done, starting google sheets API");
        /*

        Now that we have processed the FIXR data into an object, we now need to compare our list against the existing list, in order to remove pre-exisitng entries. 
        To do this for QCS, we need to connect up to the Google Sheets API, and request our member spreadsheet down. 
        This will be done in a similar fashion to the Minecraft Whitelisting solution (jamesmcfarland/qcs-whitelist on github ) 

        */


        //Setup the google sheets API
        GoogleSheetsHelper googleSheetsHelper = new GoogleSheetsHelper(settings.googleSheetsKeyfilePath, settings.googleSheetsApplicationName);

        //Get the current member list
        Console.WriteLine("Getting current members...");
        List<List<String>> currentMembers = googleSheetsHelper.getSpreadsheet(settings.googleSheetsMemberSpreadsheetID, settings.googleSheetsMemberSpreadsheetQuery, true);

        List<String[]> currentMemberStudentNosAndEmails = new List<String[]>();

        //Create a list of student numbers
        Console.WriteLine("Generating checklist...");
        currentMembers.ForEach(member =>
        {
            currentMemberStudentNosAndEmails.Add(new string[] { member[0], member[(member[0] == "not provided") ? 2 : 1] });
        });

        //Now, work out which of the FIXR attendees are new, and need to be added to the spreadsheet. 
        //To do this, create a new list of FIXR attendees who's student number is NOT in the current list
        //If student number is "not provided" 

        Console.WriteLine("Removing existing entries...");
        List<QCSFixrAttendeeData> newAttendees = new List<QCSFixrAttendeeData>();

        foreach (QCSFixrAttendeeData attendeeData in fixrData)
        {
            //The LINQ Find method returns null if no data is found, so we can use this to try match the student number
            if (currentMemberStudentNosAndEmails.Find(studentData =>
            {
                if (studentData[0] == "" || studentData[0] == "not provided") return attendeeData.Email == studentData[1];
                else return attendeeData.StudentNo == studentData[0];
            }) == null)
            {
                newAttendees.Add(attendeeData);
            }
        }

        //Now, we simply need to write the new data to the google sheet

        //Get each QCSFixrAttendeeData's various list representation (this is custom for my implentation)

        Console.WriteLine("Generating new data to be added to spreadsheets");
        //SU List
        List<List<object>> attendeesAsSUList = new List<List<object>>();
        newAttendees.ForEach(newAttendee => attendeesAsSUList.Add(newAttendee.asSUList()));
        //FIXR List
        List<List<object>> attendeesAsFIXRList = new List<List<object>>();
        newAttendees.ForEach(newAttendee => attendeesAsFIXRList.Add(newAttendee.asFIXRList()));
        //FIXR (Reduced) List
        List<List<object>> attendeesAsFIXRReducedList = new List<List<object>>();
        newAttendees.ForEach(newAttendee => attendeesAsFIXRReducedList.Add(newAttendee.asFIXRReducedDetailList()));


        Console.WriteLine("Writing spreadsheets...");
        if (!googleSheetsHelper.writeSpreadsheet(settings.googleSheetsMemberSpreadsheetID, settings.googleSheetsSUListSheetName, attendeesAsSUList))
            Console.WriteLine("Error on SU");
        if (!googleSheetsHelper.writeSpreadsheet(settings.googleSheetsMemberSpreadsheetID, settings.googleSheetsFIXRSheetName, attendeesAsFIXRList))
            Console.WriteLine("Error on FIXR");
        if (!googleSheetsHelper.writeSpreadsheet(settings.googleSheetsMemberSpreadsheetID, settings.googleSheetsReducedFIXRSheetName, attendeesAsFIXRReducedList))
            Console.WriteLine("Error on FIXR Reduced");

        Console.WriteLine("done at " + DateTime.Now.ToLongTimeString());

        await Task.Delay(Convert.ToInt16(settings.runMins) * 6000);

    }
}
catch (Exception e)
{
    Console.WriteLine("Exception occured in program. Waiting 1/2 of runMins before trying again. " + e.StackTrace);
    try
    {
        driver.Quit();

    }
    catch
    {
        Console.WriteLine("Unable to stop driver. It may already be closed.");
    }
    finally
    {

        Console.WriteLine("Driver closed.");
    }
}
finally
{
    await Task.Delay(Convert.ToInt16(settings.runMins) * 30000);
}