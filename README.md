
# Table of Contents

1.  [Summary](#org04f108e)
2.  [Endpoints](#orgc7ba569)
3.  [Create, Install, and Update instructions](#orgea56b90)
    1.  [Create Web Deploy Package](#org02fb387)
    2.  [Installation](#org2bada33)
        1.  [Required Data](#org5f24c06)
        2.  [Instructions](#org1da815e)
    3.  [Upgrade](#org719d520)
        1.  [Required Data](#org620976c)
        2.  [Instructions](#org906abc4)
4.  [Configuration Options (Web.config)](#org7cc63f2)
5.  [How to verify EGen API Data](#orgc233eca)
6.  [Logging](#orgc4b8d80)


<a id="org04f108e"></a>

# Summary

The purpose of the EGen API is to take the gas burn summary data from
California ISO and insert it into the PG&E SharePoint site as a list.
This list can then be used by any of the EGen/DailyPlan Sharepoint
applications to display information to GasOps.


<a id="orgc7ba569"></a>

# Endpoints

-   **EGenAPI:** <https://>[fully qualified domain name]/api/gasburnsummary
-   **CMRI API:** <https://ws.caiso.com/sst/cmri/RetrieveGasBurnSummaryData_CMRIv1_DocAttach_AP>
-   **CMRI URL:** <https://portal.caiso.com/cmri/logon.do>


<a id="orgea56b90"></a>

# Create, Install, and Update instructions


<a id="org02fb387"></a>

## Create Web Deploy Package

1.  Create Web Deploy Package from Visual Studio
    -   Site Name (DeployIisAppPath): EGenAPI
    -   Package location: (somewhere on local computer, for example C:\Users\\[username]\desktop\egen)
2.  Create a .zip package from the package location
    1.  Right click on the package location
    2.  Click on "Send to"
    3.  Click on "Compressed (zipped) folder"


<a id="org2bada33"></a>

## Installation


<a id="org5f24c06"></a>

### Required Data

-   EGenAPI deployment package
-   PGE IIS Service account name
-   PGE IIS Service account password
-   SharePoint URL
-   SharePoint list name
-   SharePoint Username
-   SharePoint Domain
-   SharePoint password


<a id="org1da815e"></a>

### Instructions

1.  On the IIS Server, create EGenAPI website folder (for example, D:\EGenAPI)
2.  Using IIS Server manager create a website
    -   Site Name: EGenAPI
    -   Physical Path: Folder created in Step 2
        -   Connect as: PGE IIS service account
    -   IP Address: Use assigned IP Address
    -   Port: Use assigned port
    -   Host Name: use assigned hostname or no hostname
3.  Stop the EGenAPI website in IIS Manager
4.  Right click on the EGenAPI application pool
5.  Select "Advanced Settings"
6.  Change the identity to the PGE IIS Service account (you will need the domain\username and password)
7.  Open "Manage computer certificates"
8.  Import the CMRI certificate into the Personal key store
9.  Right click on "CMRIWEBSERVICE PRGTAPPDBSWC001x9401" under Personal\Certficates
10. Click on All Tasks -> Manage Private Keys
11. Add the PGE IIS Service account and give it read only permissions
    -   Additional information can be found here: <https://stackoverflow.com/questions/12106011/system-security-cryptography-cryptographicexception-keyset-does-not-exist>
12. Copy the web package over to the IIS server
13. Unzip the package to a folder of your choice
14. Open powershell as an administrator
15. Run: New-EventLog -LogName "Application" -Source "pge.gasops.egen.cmri.web"
16. Install the following web server features:
    -   Install-WindowsFeature -Name "Web-Asp-Net45"
    -   Upgrade the .Net Framework to 4.7.2
        -   <https://support.microsoft.com/en-us/help/4054530/microsoft-net-framework-4-7-2-offline-installer-for-windows>
        -   Run NDP472-KB4054530-x86-x64-AllOS-ENU.exe and follow the prompts
17. Change to the folder with the deployment files
18. Run the command file as a test first: .\Pge.GasOps.EGen.Cmri.Web.deploy.cmd /T
19. Correct any errors that show up
20. Once no errors show up run: .\Pge.GasOps.EGen.Cmri.Web.deploy.cmd /Y
21. Open notepad as an administrator
22. In notepad open Web.config in the EGenAPI website folder
23. Change the following values to match the environment:
    -   SharePointUrl - Value: the sharepoint url (for example, "<http://wolverine.example.com/sites/pge>")
    -   SharePointListName - Value: the sharepoint list to write caiso data to (for example, "cmri")
    -   SharePointUsername - Value: a user that can write to the sharepoint list above
    -   SharePointPassword - Value: the password for the SharePoint user listed above
    -   SharePointDomain - Value: the domain for the SharePoint user listed above
24. Save Web.config and close notepad
25. Start the EGenAPI website in IIS Manager


<a id="org719d520"></a>

## Upgrade


<a id="org620976c"></a>

### Required Data

-   EGenAPI upgrade package
-   EGenAPI website physical folder path


<a id="org906abc4"></a>

### Instructions

1.  **Critical**: Make a backup copy of the EGenAPI Web.config and move it outside of the EGenAPI physical folder path.
    -   This is used later on, as the upgrade process **overwrites** the old Web.config file
2.  Copy the EGenAPI upgrade package to the IIS Server
3.  Unzip the package to a folder of your choice
4.  Open powershell as an administrator
5.  Change to the folder the upgrade package was unzipped to
6.  Run the command file as a test first: .\Pge.GasOps.EGen.Cmri.Web.deploy.cmd /T
7.  Correct any errors that show up
8.  Once no errors show up run: .\Pge.GasOps.EGen.Cmri.Web.deploy.cmd /Y
9.  Copy the backup Web.config made in step one back into the EGenAPI website physical folder path.
10. Restart the EGenAPI website


<a id="org7cc63f2"></a>

# Configuration Options (Web.config)

-   **serilog:minimum-level:** This setting determines what level logging should show up in the event logs
    -   Possible values, organized from most inclusive to least:
        Verbose > Debug > Information > Warning > Error > Fatal
    -   Currently, leave this set to Verbose
-   **SharePointUrl:** The URL to the SharePoint site (for example, <http://wolverine/sites/pge>)
-   **SharePointListName:** The SharePoint list name to insert data into
-   **SharePointUsername:** The username that can insert data into the SharePoint list
-   **SharePointPassword:** The password to use for that SharePoint user
-   **SharePointDomain:** The domain the SharePointUser belongs to
-   **X509SubjectName:** The subject name for the certificate to use for the CMRI web service (for example, CMRIWEBSERVICE PRGTAPPDBSWC001x9401)
-   **X509ThumbPrint:** The thumbprint for cerficate to use for the CMRI web service (for example, 068ce8018aaedc13930edd5560edb2fd244fbb3f)
-   **CaisoWebServiceName:** What service name to register the certificate under
-   **PrecedingDaysToDelete:** Delete list items starting x days in the past and before


<a id="orgc233eca"></a>

# How to verify EGen API Data

1.  Go to the CMRI URL (<https://portal.caiso.com/cmri/logon.do>)
    -   You will need to install a client certificate issued by California
        ISO to use their web service.
2.  On the left side, hover over "Gas Burn"
3.  Click on "Gas Burn Summary"
4.  Edit the following fields:
    -   **Start Date:** Today (for example, 8/24/2018)
    -   **End Date:** Today + 3 days (for example, if today is 8/24/2018, then end date would be 8/27/2018)
    -   **Gas Company:** PGAE
    -   **Market:** DAM or 2DA depending on which market type you are verifying
    -   **Summary Level:** Gas Company
    -   **Execution Type:** IFM
5.  Click on "Apply"
6.  Here is a description of the data that comes up
    -   **Trade Date:** The day the summary is for
    -   **HE01 - HE24:** Hour Ending 0100 to 0000.  The number below this is the MMCF for that interval.
    -   **HE25:** Used during the transition from/to daylight saving time
7.  In a separate tab, open the "CMRI" SharePoint list
8.  Compare the data between the tabs to determine if the data matches, a key is listed below:
    -   The date and time shown in TradeIntervalEndTime will match up with
        the hour end time shown for the specified Trade date.  The MMCF
        value shown in the SharePoint list should match up with the HE
        value.  Make sure the DA in the SharePoint list matches up with the
        Market selected (DAM or 2DA).


<a id="orgc4b8d80"></a>

# Logging

The EGen API logs messages to the Application log in the Windows Event
Log from the pge.gasops.egen.cmri.web source on the server that IIS is
running.  I would suggest creating a custom view for only the
pge.gasops.egen.cmri.web source to make it easier to troubleshoot the
EGen API, if necessary.  All errors generated are logged to the event
log, as well as the original XML request header, the response data from
California ISO CMRI gas burn summary API, and changes to the sharepoint
list made by the EGen API are logged as well.

