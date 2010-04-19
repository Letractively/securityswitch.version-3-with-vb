Imports System
Imports System.Configuration
Imports System.Globalization

Namespace SecureSwitch.Configuration

	''' <summary>
	''' Represents a directory entry in the &lt;secureSwitch&gt;
	''' configuration section.
	''' </summary>
	Public Class DirectorySetting
		Inherits ItemSetting

#Region "Constructors"

		''' <summary>
		''' Creates an instance of DirectorySetting.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Creates an instance with an initial path value.
		''' </summary>
		''' <param name="path">The relative path to the directory.</param>
		Public Sub New(ByVal path As String)
			MyBase.New(path)
		End Sub

		''' <summary>
		''' Creates an instance with initial values.
		''' </summary>
		''' <param name="path">The relative path to the directory.</param>
		''' <param name="secure">The type of security for the directory.</param>
		Public Sub New(ByVal path As String, ByVal secure As SecurityType)
			MyBase.New(path, secure)
		End Sub

		''' <summary>
		''' Creates an instance with initial values.
		''' </summary>
		''' <param name="path">The relative path to the directory or file.</param>
		''' <param name="secure">The type of security for the directory.</param>
		''' <param name="recurse">A flag indicating whether or not to recurse this directory 
		''' when evaluating security.</param>
		Public Sub New(ByVal path As String, ByVal secure As SecurityType, ByVal recurse As Boolean)
			Me.New(path, secure)
			Me.Recurse = recurse
		End Sub

#End Region

#Region "Properties"

		''' <summary>
		''' Gets or sets the path of this directory setting.
		''' </summary>
		<ConfigurationProperty("path", IsRequired:=True, IsKey:=True), RegexStringValidator("^(?:|/|[\w\-][\w\.\-,]*(?:/[\w\.\-]+)*/?)$")> _
		Public Overrides Property Path() As String
			Get
				Return MyBase.Path
			End Get
			Set(ByVal value As String)
				MyBase.Path = CleanPath(value)
			End Set
		End Property

		''' <summary>
		''' Gets or sets a flag indicating whether or not to include all files in any sub-directories 
		''' when evaluating a request.
		''' </summary>
		<ConfigurationProperty("recurse", DefaultValue:=False)> _
		Public Property Recurse() As Boolean
			Get
				Return CBool(Me("recurse"))
			End Get
			Set(ByVal value As Boolean)
				Me("recurse") = value
			End Set
		End Property

#End Region

		''' <summary>
		''' Overriden to "clean-up" any inconsistent, yet allowed, input.
		''' </summary>
		Protected Overrides Sub PostDeserialize()
			MyBase.PostDeserialize()
			Me("path") = CleanPath(Path)
		End Sub

		''' <summary>
		''' Cleans the specified path as needed.
		''' </summary>
		''' <param name="path">The path to clean.</param>
		''' <returns>A string containing the cleaned path value.</returns>
		Protected Function CleanPath(ByVal path As String) As String
			' Strip any trailing slash from the path.
			If path.EndsWith("/") Then
				Return path.Substring(0, path.Length - 1)
			Else
				Return path
			End If
		End Function

	End Class

	''' <summary>
	''' Represents a collection of DirectorySetting objects.
	''' </summary>
	Public Class DirectorySettingCollection
		Inherits ItemSettingCollection

		''' <summary>
		''' Gets the element name for this collection.
		''' </summary>
		Protected Overrides ReadOnly Property ElementName() As String
			Get
				Return "directories"
			End Get
		End Property

		''' <summary>
		''' Gets a flag indicating an exception should be thrown if a duplicate element 
		''' is added to the collection.
		''' </summary>
		Protected Overrides ReadOnly Property ThrowOnDuplicate() As Boolean
			Get
				Return True
			End Get
		End Property

		''' <summary>
		''' Gets the element at the specified index.
		''' </summary>
		''' <param name="index">The index to retrieve the element from.</param>
		''' <returns>The DirectorySetting located at the specified index.</returns>
		Default Public Shadows ReadOnly Property Item(ByVal index As Integer) As DirectorySetting
			Get
				Return CType(BaseGet(index), DirectorySetting)
			End Get
		End Property

		''' <summary>
		''' Gets the element with the specified path.
		''' </summary>
		''' <param name="path">The path of the element to retrieve.</param>
		''' <returns>The DirectorySetting with the specified path.</returns>
		Default Public Shadows ReadOnly Property Item(ByVal path As String) As DirectorySetting
			Get
				If path Is Nothing Then
					Throw New ArgumentNullException("path")
				Else
					Return CType(BaseGet(path.ToLower(CultureInfo.InvariantCulture)), DirectorySetting)
				End If
			End Get
		End Property

#Region "Collection Methods"

		''' <summary>
		''' Adds a DirectorySetting to the collection.
		''' </summary>
		''' <param name="fileSetting">An initialized DirectorySetting instance.</param>
		Public Sub Add(ByVal fileSetting As DirectorySetting)
			BaseAdd(fileSetting)
		End Sub

		''' <summary>
		''' Clears all file entries from the collection.
		''' </summary>
		Public Sub Clear()
			BaseClear()
		End Sub

		''' <summary>
		''' Removes the specified DirectorySetting from the collection, if it exists.
		''' </summary>
		''' <param name="fileSetting">A DirectorySetting to remove.</param>
		Public Sub Remove(ByVal fileSetting As DirectorySetting)
			Dim Index As Integer = MyBase.IndexOf(fileSetting)
			If (Index >= 0) Then
				BaseRemoveAt(Index)
			End If
		End Sub

		''' <summary>
		''' Removes a DirectorySetting from the collection with a matching path as specified.
		''' </summary>
		''' <param name="path">The path of a DirectorySetting to remove.</param>
		Public Sub Remove(ByVal path As String)
			Dim Index As Integer = MyBase.IndexOf(path)
			If (Index >= 0) Then
				BaseRemoveAt(Index)
			End If
		End Sub

		''' <summary>
		''' Removes the DirectorySetting from the collection at the specified index.
		''' </summary>
		''' <param name="index">The index of the DirectorySetting to remove.</param>
		Public Sub RemoveAt(ByVal index As Integer)
			BaseRemoveAt(index)
		End Sub

#End Region

		''' <summary>
		''' Creates a new element for this collection.
		''' </summary>
		''' <returns>A new instance of FileSetting.</returns>
		Protected Overrides Function CreateNewElement() As ConfigurationElement
			Return New DirectorySetting()
		End Function

		''' <summary>
		''' Gets the key for the specified element.
		''' </summary>
		''' <param name="element">An element to get a key for.</param>
		''' <returns>A string containing the Path of the DirectorySetting.</returns>
		Protected Overrides Function GetElementKey(ByVal element As ConfigurationElement) As Object
			If Not element Is Nothing Then
				Return CType(element, DirectorySetting).Path.ToLower(CultureInfo.InvariantCulture)
			Else
				Return Nothing
			End If
		End Function

	End Class

End Namespace