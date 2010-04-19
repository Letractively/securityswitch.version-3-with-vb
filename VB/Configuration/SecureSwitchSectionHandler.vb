Imports System.Collections.Specialized
Imports System.Configuration
Imports System.Text.RegularExpressions
Imports System.Xml
Imports SecureSwitch.Configuration

Namespace SecureSwitch

	''' <summary>
	''' The exception thrown for any errors while reading the SecureSwitch 
	''' section of a configuration file.
	''' </summary>
	Public Class SecureSwitchSectionException
		Inherits System.Configuration.ConfigurationException

		''' <summary>
		''' Initializes an instance of this exception with specified parameters.
		''' </summary>
		''' <param name="Message">The message to display to the client when the exception is thrown.</param>
		''' <param name="Node">The XmlNode that contains the error.</param>
		Public Sub New(ByVal message As String, ByVal node As XmlNode)
			MyBase.new(message, node)
		End Sub

	End Class


	''' <summary>
	''' SecureSwitchSectionHandler reads any <SecureSwitch> section from a configuration file.
	''' </summary>
	Public Class SecureSwitchSectionHandler
		Implements IConfigurationSectionHandler

		''' <summary>
		''' Parses the XML configuration section and returns the settings.
		''' </summary>
		''' <param name="parent">
		'''		The configuration settings in a corresponding parent 
		'''		configuration section.
		''' </param>
		''' <param name="configContext">
		'''		An HttpConfigurationContext when Create is called from the ASP.NET 
		'''		configuration system. Otherwise, this parameter is reserved and is 
		'''		a null reference (Nothing in Visual Basic).
		''' </param>
		''' <param name="section">
		'''		The XmlNode that contains the configuration information from the 
		'''		configuration file. Provides direct access to the XML contents of 
		'''		the configuration section.
		''' </param>
		''' <returns>
		'''		Returns a Settings instance initialized with the 
		'''		read configuration settings.
		'''	</returns>
		Public Function Create(ByVal parent As Object, ByVal configContext As Object, ByVal section As System.Xml.XmlNode) As Object Implements System.Configuration.IConfigurationSectionHandler.Create
			' Create a Settings object for the settings in this section
			Dim Settings As New Settings

			' Read the general settings
			ReadGeneralSettings(section, Settings)

			' Traverse the child nodes
			For Each Node As XmlNode In section.ChildNodes
				If Node.NodeType = System.Xml.XmlNodeType.Comment Then
					' Skip comment nodes (thanks to dcbrower on CodeProject for pointing this out)

				ElseIf Node.Name.ToLower() = "directory" Then
					' This is a directory path node
					Settings.Directories.Add(ReadDirectoryItem(Node))
				ElseIf Node.Name.ToLower() = "file" Then
					' This is a file path node
					Settings.Files.Add(ReadFileItem(Node))
				Else
					' Throw an exception for this unrecognized node
					Throw New SecureSwitchSectionException(String.Format("'{0}' is not an acceptable setting.", Node.Name), Node)
				End If
			Next

			' Return the settings
			Return Settings
		End Function

		''' <summary>
		''' Reads general settings from the SecureSwitch section into the given Settings instance.
		''' </summary>
		''' <param name="section">The XmlNode to read from.</param>
		''' <param name="settings">The Settings instance to set.</param>
		Protected Sub ReadGeneralSettings(ByVal section As XmlNode, ByVal settings As Settings)
			' Get the mode attribute
			If Not section.Attributes("mode") Is Nothing Then
				Dim ModeValue As String = section.Attributes("mode").Value.Trim()
				If [Enum].IsDefined(GetType(Mode), ModeValue) Then
					settings.Mode = CType([Enum].Parse(GetType(Mode), ModeValue), Mode)
				Else
					Throw New SecureSwitchSectionException("Invalid value for the 'mode' attribute.", section)
				End If
			End If

			' Get the secureHostPath attribute
			If Not section.Attributes("encryptedUri") Is Nothing Then
				settings.EncryptedUri = section.Attributes("encryptedUri").Value
			End If

			' Get the insecureHostPath attribute
			If Not section.Attributes("unencryptedUri") Is Nothing Then
				settings.UnencryptedUri = section.Attributes("unencryptedUri").Value
			End If

			' Validate that if either encryptedUri or unencryptedUri are set, both must be set
			If (settings.EncryptedUri.Length > 0 And settings.UnencryptedUri.Length = 0) Or (settings.UnencryptedUri.Length > 0 And settings.EncryptedUri.Length = 0) Then
				Throw New SecureSwitchSectionException("You must specify both 'encryptedUri' and 'unencryptedUri', or neither.", section)
			End If

			' Get the maintainPath attribute
			If Not section.Attributes("maintainPath") Is Nothing Then
				Dim Value As String = section.Attributes("maintainPath").Value.ToLower()
				settings.MaintainPath = (Value = "true" OrElse Value = "yes" OrElse Value = "on")
			End If

			' Get the warningBypassMode attribute
			If Not section.Attributes("warningBypassMode") Is Nothing Then
				Dim WarningBypassModeValue As String = section.Attributes("warningBypassMode").Value.Trim()
				If [Enum].IsDefined(GetType(SecurityWarningBypassMode), WarningBypassModeValue) Then
					settings.WarningBypassMode = CType([Enum].Parse(GetType(SecurityWarningBypassMode), WarningBypassModeValue), SecurityWarningBypassMode)
				Else
					Throw New SecureSwitchSectionException("Invalid value for the 'warningBypassMode' attribute.", section)
				End If
			End If

			' Get the bypassQueryParamName attribute
			If Not section.Attributes("bypassQueryParamName") Is Nothing Then
				settings.BypassQueryParamName = section.Attributes("bypassQueryParamName").Value
			End If
		End Sub

		''' <summary>
		''' Reads the typical attributes for a ItemSetting from the configuration node.
		''' </summary>
		''' <param name="node">The XmlNode to read from.</param>
		''' <param name="item">The ItemSetting to set values for.</param>
		Protected Sub ReadChildItem(ByVal node As XmlNode, ByVal item As ItemSetting)
			' Set the item only if the node has a valid path attribute value
			If Not node.Attributes("path") Is Nothing AndAlso node.Attributes("path").Value.Trim().Length > 0 Then
				' Get the value of the path attribute
				item.Path = node.Attributes("path").Value.Trim().ToLower()

				' Remove leading and trailing "/" characters.
				If (item.Path.Length > 0) Then
					item.Path = item.Path.Trim("/"c)
				End If

				' Check for a secure attribute
				If Not node.Attributes("secure") Is Nothing Then
					Dim SecureValue As String = node.Attributes("secure").Value.Trim()
					If [Enum].IsDefined(GetType(SecurityType), SecureValue) Then
						item.Secure = CType([Enum].Parse(GetType(SecurityType), SecureValue), SecurityType)
					Else
						Throw New SecureSwitchSectionException("Invalid value for the 'secure' attribute.", node)
					End If
				End If
			Else
				' Throw an exception for the missing Path attribute
				Throw New SecureSwitchSectionException("'path' attribute not found.", node)
			End If
		End Sub

		''' <summary>
		''' Reads a directory item from the configuration node and returns a new instance of DirectorySetting.
		''' </summary>
		''' <param name="node">The XmlNode to read from.</param>
		''' <returns>A DirectorySetting initialized with values read from the node.</returns>
		Protected Function ReadDirectoryItem(ByVal node As XmlNode) As DirectorySetting
			' Create a DirectorySetting instance
			Dim Item As New DirectorySetting

			' Read the typical attributes
			ReadChildItem(node, Item)

			' Check for a recurse attribute
			If Not node.Attributes("recurse") Is Nothing Then
				Item.Recurse = (node.Attributes("recurse").Value.ToLower() = "true")
			End If

			Return Item
		End Function

		''' <summary>
		''' Reads a file item from the configuration node and returns a new instance of FileSetting.
		''' </summary>
		''' <param name="node">The XmlNode to read from.</param>
		''' <returns>A FileSetting initialized with values read from the node.</returns>
		Protected Function ReadFileItem(ByVal node As XmlNode) As FileSetting
			' Create a FileSetting instance
			Dim Item As New FileSetting

			' Read the typical attributes
			ReadChildItem(node, Item)

			Return Item
		End Function

	End Class

End Namespace