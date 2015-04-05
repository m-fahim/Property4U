# Property4U
Property4U is an Open Source Enterprise Real Estate 2.0 solution. 

##Introduction:
Property4U is an Open Source Enterprise Real Estate 2.0 solution completely equipped with Buying, Selling, Renting and Bidding features. It also provides Developer APIs with complete documentation. 

##System Requirements:
  - Visual Studio 2013 Update 2
  - Microsoft .NET Framework 4.5
  - IIS 7 or higher
  - Microsoft® SQL Server 2012 Express

##System Specifications:
  - ASP.NET MVC 5
  - ASP.NET WEB API 2.2
  - ASP.NET Identity 2.0
  - ASP.NET SignalR
  - Bootstrap 3
  - EntityFramework 6
  - Json.NET
  - Quartz.NET

##Demo:
  - FrontEnd: http://property4u.somee.com/
  - BackEnd: http://property4u.somee.com/Account/Login

#Getting Started
  - Download Property4U source code.
  - Unzip Property4U and copy it the Projects folder of Visual Studio 2013.
  
    ###Build Project
- Open Project in Visual Studio 2013.
- Open **ControlDesk > Index.cshtml** file under Views folder of project solution.
- Build project **Build > Build Solution** or **Ctrl+Shift+B**.
- Go to **Debug** menu and select **Start Debugging** or **F5**.
- When project is loaded in browser it prompts with **Login Screen**.
- Enter **Admin** Username and Password from **Passwords.txt** provided in project source.
- After successful Login, system takes you the **Configuration** page.

    ###Load Sample Data
- Choose **Load** from **Configuration** **Core > Sample Data**.
- Proceed by accepting the "Alert Message" regarding database reset and wait until successful **Data Load** notification appears
- Logout from system for applying changes.
- Remember to delete **Sample_Data.bak** from **Content > System > Sample_Data.bak** after loading samples.

##Security Provisions:
- **Change** default usernames and passwords provided with the **SAMPLE DATA**.
- Remember to delete **Sample_Data.bak** from **Content > System > Sample_Data.bak** after loading samples.


##Developers
- Usman Tahir
- mfahim
- Waheeb Islam

##Credits
- https://www.google.com.pk/
- http://www.asp.net/mvc/overview/getting-started
- http://stackoverflow.com
- http://www.binarytheme.com/asp-net-free-admin-template-siminta/
- http://shapebootstrap.net/item/e-shopper-best-free-ecommerce-html-template/
- http://getbootstrap.com/
- http://fortawesome.github.io/Font-Awesome/
- https://github.com/TroyGoode/PagedList
- http://www.newtonsoft.com/json
- http://jquery.com/
- http://jqueryui.com/
- http://sachinchoolur.github.io/lightGallery/
- http://knockoutjs.com/
- http://www.quartz-scheduler.net/
- http://dillinger.io/

##FAQ’s

####Can make it work with http?
  - Yes, Right Click **Project Solution > Properties > Web > Project Url >** Place your url **http://localhost:*/**.
  - Select **Project > Properties Window >** SSL Enabled to **“False” >** Place **http://localhost:*/** in **Url**.
  - Remove **[RequireHttps]** from **FrontEnd** and **ControlDesk** Controllers.
  - Now **Clean** and **Build Solution** to apply Changes.

####Social Login Integration is available?
  - Yes, firstly complete **Build Project** and **Load Sample Data** steps.
  - Login using **Admin** account.
  - Under Configuration > Core > Fill Facebook and Google IDs and Secret Keys.
  - Now Clean and Build Solution to apply Changes.

####How to publish project on shared hosting?
  - **Clean** and **Build Solution**.
  - Under **Build** menu > **Publish Property4U**.
  - Select **Custom** to create **Profile** by given name to it.
  - **Publish Web > Publish Method** “FTP”.
  - Fill **FTP Details** provided by hosting provider and select Validate.
  - After successful **Validation** select **Publish** and wait until it Uploading files using FTP.
  - Create **App_Data folder** in root of **File Manager** of **Control Panel** if missing **> Upload XMLDocument.xml** from project solution to App_Data. 
  - Create new **MSSQL Database**.
  - Upload and Attach **Property4U-2-1-beta.mdf** database file located in project **App_Data folder** to newly created MSSQL Database.
  - Open **Web.Config** and replace **connectionString="*”** with your hosting provider one. 
