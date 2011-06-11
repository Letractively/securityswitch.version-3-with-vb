Imports System
Imports System.Configuration
Imports System.Text.RegularExpressions

Namespace SecuritySwitch.Configuration

	''' <summary>
	''' The possible ways to ignore HTTP handlers.
	''' </summary>
	Public Enum IgnoreHandlers
		''' <summary>
		''' Indicates that the module should ignore the built-in HTTP handlers.
		''' <list type="bullet">
		'''		<item>Trace.axd</item>
		'''		<item>WebResource.axd</item>
		''' </list>
		''' </summary>
		BuiltIn

		''' <summary>
		''' Indicates that the module should ignore all files with extensions corresponding
		''' to the standard for HTTP handlers. Currently, that is .axd files.
		''' </summary>
		WithStandardExtensions

		''' <summary>
		''' Indicates that the module will not ignore handlers unless specifically 
		''' specified in the files or directories entries.
		''' </summary>
		''' <remarks>
		''' This was the default behavior prior to version 3.1.
		''' </remarks>
		None

	End Enum

	''' <summary>
	''' The different modes supported for the &lt;securitySwitch&gt; configuration section.
	''' </summary>
	Public Enum Mode

		''' <summary>
		''' Indicates that web page security is on and all requests should be monitored.
		''' </summary>
		[On]

		''' <summary>
		''' Only remote requests are to be monitored.
		''' </summary>
		RemoteOnly

		''' <summary>
		''' Only local requests are to be monitored.
		''' </summary>
		LocalOnly

		''' <summary>
		''' Web page security is off and no monitoring should occur.
		''' </summary>
		Off

	End Enum


	''' <summary>
	''' The different modes for bypassing security warnings.
	''' </summary>
	Public Enum SecurityWarningBypassMode

		''' <summary>
		''' Always bypass security warnings when switching to an unencrypted page.
		''' </summary>
		AlwaysBypass

		''' <summary>
		''' Only bypass security warnings when switching to an unencrypted page if the proper query parameter is present.
		''' </summary>
		BypassWithQueryParam

		''' <summary>
		''' Never bypass security warnings when switching to an unencrypted page.
		''' </summary>
		NeverBypass

	End Enum


	''' <summary>
	''' Contains the settings of a &lt;securitySwitch&gt; configuration section.
	''' </summary>
	Public Class Settings
		Inherits ConfigurationSection

		''' <summary>
		''' Creates an instance of Settings.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

#Region "Properties"

		''' <summary>
		''' Gets or sets the name of the query parameter that will indicate to the module to bypass
		''' any security warning if WarningBypassMode = BypassWithQueryParam.
		''' </summary>
		<ConfigurationProperty("bypassQueryParamName")> _
		Public Property BypassQueryParamName() As String
			Get
				Return CStr(Me("bypassQueryParamName"))
			End Get
			Set(ByVal value As String)
				Me("bypassQueryParamName") = value
			End Set
		End Property

		''' <summary>
		''' Gets or sets the path to a URI for encrypted redirections, if any.
		''' </summary>
		<ConfigurationProperty("encryptedUri"), RegexStringValidator("^(?:|(?:https://)?[\w\-][\w\.\-,]*(?:\:\d+)?(?:/[\w\.\-]+)*/?)$")> _
		Public Property EncryptedUri() As String
			Get
				Return CStr(Me("encryptedUri"))
			End Get
			Set(ByVal value As String)
				If Not String.IsNullOrEmpty(value) Then
					Me("encryptedUri") = value
				Else
					Me("encryptedUri") = Nothing
				End If
			End Set
		End Property

		''' <summary>
		''' Gets or sets a flag indicating how to ignore HTTP handlers, if at all.
		''' </summary>
		<ConfigurationProperty("ignoreHandlers", DefaultValue:=IgnoreHandlers.BuiltIn)> _
		Public Property IgnoreHandlers() As IgnoreHandlers
			Get
				Return CType(Me("ignoreHandlers"), IgnoreHandlers)
			End Get
			Set(ByVal value As IgnoreHandlers)
				Me("ignoreHandlers") = value
			End Set
		End Property

		''' <summary>
		''' Gets or sets a flag indicating whether or not to maintain the current path when redirecting
		''' to a different host.
		''' </summary>
		<ConfigurationProperty("maintainPath", DefaultValue:=True)> _
		Public Property MaintainPath() As Boolean
			Get
				Return CBool(Me("maintainPath"))
			End Get
			Set(ByVal value As Boolean)
				Me("maintainPath") = value
			End Set
		End Property

		''' <summary>
		''' Gets or sets the mode indicating how the secure switch settings are handled.
		''' </summary>
		<ConfigurationProperty("mode", DefaultValue:=Mode.On)> _
		Public Property Mode() As Mode
			Get
				Return CType(Me("mode"), Mode)
			End Get
			Set(ByVal value As Mode)
				Me("mode") = value
			End Set
		End Property

		''' <summary>
		''' Gets the collection of directory settings read from the configuration section.
		''' </summary>
		<ConfigurationProperty("directories")> _
		Public ReadOnly Property Directories() As DirectorySettingCollection
			Get
				Return CType(Me("directories"), DirectorySettingCollection)
			End Get
		End Property

		''' <summary>
		''' Gets the collection of file settings read from the configuration section.
		''' </summary>
		<ConfigurationProperty("files")> _
		Public ReadOnly Property Files() As FileSettingCollection
			Get
				Return CType(Me("files"), FileSettingCollection)
			End Get
		End Property

		''' <summary>
		''' Gets or sets the path to a URI for unencrypted redirections, if any.
		''' </summary>
		<ConfigurationProperty("unencryptedUri"), RegexStringValidator("^(?:|(?:http://)?[\w\-][\w\.\-,]*(?:\:\d+)?(?:/[\w\.\-]+)*/?)$")> _
		Public Property UnencryptedUri() As String
			Get
				Return CStr(Me("unencryptedUri"))
			End Get
			Set(ByVal value As String)
				If Not String.IsNullOrEmpty(value) Then
					Me("unencryptedUri") = value
				Else
					Me("unencryptedUri") = Nothing
				End If
			End Set
		End Property

		''' <summary>
		''' Gets or sets the bypass mode indicating whether or not to bypass security warnings
		''' when switching to a unencrypted page.
		''' </summary>
		<ConfigurationProperty("warningBypassMode", DefaultValue:=SecurityWarningBypassMode.BypassWithQueryParam)> _
		Public Property WarningBypassMode() As SecurityWarningBypassMode
			Get
				Return CType(Me("warningBypassMode"), SecurityWarningBypassMode)
			End Get
			Set(ByVal value As SecurityWarningBypassMode)
				Me("warningBypassMode") = value
			End Set
		End Property

		''' <summary>
		''' This property is for internal use and is not meant to be set.
		''' </summary>
		<ConfigurationProperty("xmlns")> _
		Public Property Xmlns() As String
			Get
				Return CStr(Me("xmlns"))
			End Get
			Set(ByVal value As String)
				Me("xmlns") = value
			End Set
		End Property

#End Region

	End Class

End Namespace