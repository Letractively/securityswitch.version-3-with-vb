Imports System.Collections.Specialized

Namespace SecureSwitch.Configuration

	''' <summary>
	''' The different modes supported for the &lt;secureSwitch&gt; configuration section.
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
	''' Settings contains the settings of a SecureSwitch configuration section.
	''' </summary>
	Public Class Settings

		' Fields
		Private _bypassQueryParamName As String = "BypassSecurityWarning"
		Private _encryptedUri As String = String.Empty
		Private _maintainPath As Boolean = True
		Private _mode As Mode = Mode.On
		Private _directories As DirectorySettingCollection
		Private _files As FileSettingCollection
		Private _unencryptedUri As String = String.Empty
		Private _warningBypassMode As SecurityWarningBypassMode = SecurityWarningBypassMode.BypassWithQueryParam

#Region " Properties "

		''' <summary>
		''' Gets or sets the name of the query parameter that will indicate to the module to bypass
		''' any security warning if WarningBypassMode = BypassWithQueryParam.
		''' </summary>
		Public Property BypassQueryParamName() As String
			Get
				Return _bypassQueryParamName
			End Get
			Set(ByVal Value As String)
				_bypassQueryParamName = Value
			End Set
		End Property

		''' <summary>
		''' Gets or sets the path to a URI for encrypted redirections, if any.
		''' </summary>
		Public Property EncryptedUri() As String
			Get
				Return _encryptedUri
			End Get
			Set(ByVal Value As String)
				If Not Value Is Nothing AndAlso Value.Length > 0 Then
					_encryptedUri = Value
				Else
					_encryptedUri = String.Empty
				End If
			End Set
		End Property

		''' <summary>
		''' Gets or sets a flag indicating whether or not to maintain the current path when redirecting
		''' to a different host.
		''' </summary>
		Public Property MaintainPath() As Boolean
			Get
				Return _maintainPath
			End Get
			Set(ByVal Value As Boolean)
				_maintainPath = Value
			End Set
		End Property

		''' <summary>
		''' Gets or sets the mode indicating how the secure web page settings handled.
		''' </summary>
		Public Property Mode() As Mode
			Get
				Return _mode
			End Get
			Set(ByVal Value As Mode)
				_mode = Value
			End Set
		End Property

		''' <summary>
		''' Gets the collection of directories read from the configuration section.
		''' </summary>
		Public ReadOnly Property Directories() As DirectorySettingCollection
			Get
				Return _directories
			End Get
		End Property

		''' <summary>
		''' Gets the collection of files read from the configuration section.
		''' </summary>
		Public ReadOnly Property Files() As FileSettingCollection
			Get
				Return _files
			End Get
		End Property

		''' <summary>
		''' Gets or sets the path to a URI for unencrypted redirections, if any.
		''' </summary>
		Public Property UnencryptedUri() As String
			Get
				Return _unencryptedUri
			End Get
			Set(ByVal Value As String)
				If Not Value Is Nothing AndAlso Value.Length > 0 Then
					_unencryptedUri = Value
				Else
					_unencryptedUri = String.Empty
				End If
			End Set
		End Property

		''' <summary>
		''' Gets or sets the bypass mode indicating whether or not to bypass security warnings
		''' when switching to a unencrypted page.
		''' </summary>
		Public Property WarningBypassMode() As SecurityWarningBypassMode
			Get
				Return _warningBypassMode
			End Get
			Set(ByVal Value As SecurityWarningBypassMode)
				_warningBypassMode = Value
			End Set
		End Property

#End Region

		''' <summary>
		''' The default constructor creates the needed lists.
		''' </summary>
		Public Sub New()
			' Create the collections
			_directories = New DirectorySettingCollection
			_files = New FileSettingCollection
		End Sub

	End Class

End Namespace