
# fixr-to-googlesheets
A handy tool for translating [FIXR](https://fixr.co) attendee data into Google Sheets

#### Please understand that this code will NOT work out of the box for you! (unless your setup on google sheets and FIXR is exactly the same as the one this was written to work with. Most of the code WILL however work, just with some tweaking. See the "modifying" section below! 

## Prerequisites
1. Configure a service account on Google Cloud ([Instructions](https://developers.google.com/workspace/guides/create-credentials#service-account))
2. Make a FIXR account and event
3. Copy the ```key.json``` file into the root directory of the project
4. Make yourself familiar with the Google Sheets API ([Link](https://developers.google.com/sheets/api))
5. Edit the settings.json file and fill out all of the settings
    - You will need to get the ID for the FIXR event, this can be found in the URL address bar
        - e.g. for `https://organiser.fixr.co/events/12345678`, `12345678` is the event ID
    - You will also  need to get the ID for the Google Sheet you plan on using, his can be found in the URL address bar also:
        -  e.g for `https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms`, `1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms` is the spreadsheet ID
6. Install the .NET SDK (version 6+), NodeJS and PM2
    - Download the .NET SDK from [here](https://dotnet.microsoft.com/en-us/). Make sure you download the SDK, not the runtime!
    - I reccommend using [nvm](https://github.com/nvm-sh/nvm_) to manage NodeJS if you're running on a unix system
    - If you're on windows, use [nvm-windows](https://github.com/coreybutler/nvm-windows)
    - To install PM2 (make sure node.js is installed first!) run `npm install pm2@latest -g`

## Installation

Clone the repository using Git

```bash
git clone https://jamesmcfarland/fixr-to-googlesheets
```
At this point, copy in the key.json file and fill out the settings.json file
#### This step should not be attempted until BOTH `key.json` and `settings.json` have been filled out
Fix the permissions on, then run `update.sh` which updates the code, builds it, then runs it using PM2

```bash
chmod a+x update.sh
./update.sh
```
If you don't want to use PM2, you can run the project with the following command
```bash
dotnet run fixr
```

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

This will be updated by me as the needs for the tool change, however feel free to fork and improve it in any way, and if you need any help modifying the code for your needs, feel free to open an issue and reach out.

## License
Licensed under the [GNU General Public License v3.0](https://choosealicense.com/licenses/gpl-3.0/)
