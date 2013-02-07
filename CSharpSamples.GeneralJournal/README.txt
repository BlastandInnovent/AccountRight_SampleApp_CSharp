
 AccountRight API sample: General Journal module

 This sample web application demonstrates how you can call and use the 
 General Journal module API.


 == System Requirements =========================


* The API server Host must be set up on the local computer or network. 

* To deploy this sample web application, Web Deploy (msdeploy.exe) must be 
  installed on the computer that runs the .cmd file. For information about
  how to install Web Deploy, visit http://go.microsoft.com/?linkid=9278654

* IIS must be enabled on the computer where the web sample application 
  will be installed.

* The IIS Website on which the web sample application will be installed 
  needs to use the .NET 4.0 application pool, not .NET 4.0 classic pool.

* ASP.NET MVC 3 must be installed. For information about how to install 
  ASP.NET MVC 3, visit http://go.microsoft.com/fwlink/?LinkID=208140


 == Installation ================================


1.Extract the zip file to a folder on your computer. 
  
  The zip file contains the following files:

  * api_sample.deploy-readme.txt (readme file to help you deploy 
    the web application)

  * api_sample.deploy.cmd (command file for deployment)

  * api_sample.SetParameters.xml (xml config file for the deployment 
    command)

  * api_sample.SourceManifest.xml (xml config file for system config
    during deployment)

  * api_sample.zip (web application archive)

  * src.zip (Visual Studio 2012 project and source code for the 
    sample web application). 

2. Open a Command Prompt window as an administrator. 

3. Run api_sample.deploy.cmd at the command prompt:

   api_sample.deploy.cmd /Y

4. Start the IIS Manager (Control Panel > Administrative Tools).  

5. Expand Sites/Default Web Site, and then select GeneralJournal.

6. Double-click the Application Settings icon. 

7. Double-click WebApiUrl, enter the API URL into the Value field, and 
   then click OK. 

8. Start Internet Explorer, and go to http://localhost/GeneralJournal/ 
   to test the web application. 

------------------------------------------------

NOTE: To View the source code, extract the src.zip archive and then load
      CSharpSamples.sln into Visual Studio .net 2012. 

