*** Version 2.x (For .NET 1.1) ***

Congratulations!

In order for the SecureSwitchModule to work properly, you'll need to configure either your web application or the web server. The web application configuration file is in the application's virtual root with the name web.config. If you'd like to enable the module for the entire server, the machine.config file should be edited. It is found in your Windows (WinNT) directory under Microsoft.NET/Framework/vX.X.XXXX/Config/machine.config. You may also need to register the assembly in the Global Assembly Cache and include the version, culture and publickeytoken in the machine.config file for machine-level operation.

Please, add the following lines to the configuration file of your choice:

<configuration>
	...
	<configSections>
		...
		<section 
			name="secureSwitch" 
			type="SecureSwitch.SecureSwitchSectionHandler, SecureSwitch" allowDefinition="MachineToApplication" 
			allowLocation="false" />
	</configSections>
	
	...
	
	<system.web>
		...
		<httpModules>
			...
			<add 
				name="SecureSwitch" 
				type="SecureSwitch.SecureSwitchModule, SecureSwitch" />
		</httpModules>
		...
	</system.web>
	...
</configuration>

--------------------------------------------------------------

To enable the security module in a web application, add the following section to your web.config file:

<configuration>
	...
	<!--
	secureSwitch
		This section will redirect any matching pages to the HTTPS protocol for SSL security
		and, if needed, redirect any non-matching pages (or pages matching an entry marked secure="false") 
		to the HTTP protocol to remove the security and encryption.

		Set mode="On", "RemoteOnly" or "LocalOnly" to enable web page security; 
		"Off" to disable (default = "On").
		
		"On": Security is enabled and all requests are monitored.
		"RemoteOnly": Only requests from remote clients are monitored.
		"LocalOnly": Only requests from the local server are monitored.
		"Off": No requests are monitored.
		
		Set encryptedUri to a specific URI to indicate where to redirect to when the module decides that 
		security is needed. Likewise, set unencryptedUri for times the module decides that security is
		not needed.
		
		Set maintainPath="False" to prevent the module from maintaining the current path
		when redirecting to the specified host paths (default = "True").
		
		Set warningBypassMode="AlwaysBypass" to always bypass security warnings;
		"NeverBypass" to never bypass the warnings (default = "BypassWithQueryParam").
		
		"AlwaysBypass": Always bypass security warnings when switching to an unencrypted page.
		"BypassWithQueryParam": Only bypass security warnings when switching to an unencrypted page if the 
			proper query parameter is present.
		"NeverBypass": Never bypass security warnings when switching to an unencrypted page.
		
		Set bypassQueryParamName to the name of a query parameter that will indicate to the module to bypass
		any security warning if warningBypassMode="BypassWithQueryParam" (default = "BypassSecurityWarning").
		
		Set ignoreHandlers to one of the following values in order to instruct the module on ignoring 
		HTTP handlers (default = "BuiltIn").
		
		"BuiltIn": The built-in HTTP handlers should be ignored. Currently, these are Trace.axd and WebResource.axd.
		"WithStandardExtensions": All files that have an extension that corresponds to standard HTTP handlers should 
			be ignored. Currently, that is .axd files.
		"None": No HTTP handlers should be ignored unless specifically specified in the files or directories entries.
		
		- Add <directory> tags for each directory to secure.
		- Add <file> tags for each file to secure.
		- Both tags expect a "path" attribute to the directory or file that should be evaluated.
			Specify "/" as the directory path in order to denote the application root (not the site root).
		- Both tags may include a "secure" attribute indicating whether or not to secure the 
			directory or file (default = "True"). Possible values are "True" to force security, 
			"False" to force insecurity and "Ignore" to ignore the file or directory and do nothing.
		- <directory> tags may include a "recurse" attribute. If "True", all files in any sub-directories
			are included (default = "False").
	-->
	<secureSwitch mode="On">
		<file path="Default.aspx" secure="False" />
		<file path="Lib/PopupCalendar.aspx" secure="Ignore" />
		<file path="Members/ViewStatistics.aspx" />
		<file path="Admin/MoreAdminStuff.aspx" secure="False" />
		<directory path="/" />
		<directory path="Admin" />
		<directory path="Members/Secure" recurse="True" />
	</secureSwitch>

	...
	
	<system.web>
		...
	</system.web>
	
	...	
</configuration>

Change any attributes, directory tags and file tags to suit the needs of your web application.
