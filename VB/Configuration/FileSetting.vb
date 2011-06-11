Imports System
Imports System.Configuration
Imports System.Globalization

Namespace SecuritySwitch.Configuration

	''' <summary>
	''' Represents an file entry in the &lt;securitySwitch&gt;
	''' configuration section.
	''' </summary>
	Public Class FileSetting
		Inherits ItemSetting

#Region "Constructors"

		''' <summary>
		''' Creates an instance of FileSetting.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Creates an instance with an initial path value.
		''' </summary>
		''' <param name="path">The relative path to the file.</param>
		Public Sub New(ByVal path As String)
			MyBase.New(path)
		End Sub

		''' <summary>
		''' Creates an instance with initial values.
		''' </summary>
		''' <param name="path">The relative path to the file.</param>
		''' <param name="secure">The type of security for the file.</param>
		Public Sub New(ByVal path As String, ByVal secure As SecurityType)
			MyBase.New(path, secure)
		End Sub

#End Region

		''' <summary>
		''' Gets or sets the path of this file setting.
		''' </summary>
		<ConfigurationProperty("path", IsRequired:=True, IsKey:=True), RegexStringValidator("^(?:|[\w\-][\w\.\-,]*(?:/[\w\.\-]+)*)$")> _
		Public Overrides Property Path() As String
			Get
				Return MyBase.Path
			End Get
			Set(ByVal value As String)
				MyBase.Path = value
			End Set
		End Property

	End Class

	''' <summary>
	''' Represents a collection of FileSetting objects.
	''' </summary>
	Public Class FileSettingCollection
		Inherits ItemSettingCollection

		''' <summary>
		''' Gets the element name for this collection.
		''' </summary>
		Protected Overrides ReadOnly Property ElementName() As String
			Get
				Return "files"
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
		''' <returns>The FileSetting located at the specified index.</returns>
		Default Public Shadows ReadOnly Property Item(ByVal index As Integer) As FileSetting
			Get
				Return CType(BaseGet(index), FileSetting)
			End Get
		End Property

		''' <summary>
		''' Gets the element with the specified path.
		''' </summary>
		''' <param name="path">The path of the element to retrieve.</param>
		''' <returns>The FileSetting with the specified path.</returns>
		Default Public Shadows ReadOnly Property Item(ByVal path As String) As FileSetting
			Get
				If path Is Nothing Then
					Throw New ArgumentNullException("path")
				Else
					Return CType(BaseGet(path.ToLower(CultureInfo.InvariantCulture)), FileSetting)
				End If
			End Get
		End Property

#Region "Collection Methods"

		''' <summary>
		''' Adds a FileSetting to the collection.
		''' </summary>
		''' <param name="fileSetting">An initialized FileSetting instance.</param>
		Public Sub Add(ByVal fileSetting As FileSetting)
			BaseAdd(fileSetting)
		End Sub

		''' <summary>
		''' Clears all file entries from the collection.
		''' </summary>
		Public Sub Clear()
			BaseClear()
		End Sub

		''' <summary>
		''' Removes the specified FileSetting from the collection, if it exists.
		''' </summary>
		''' <param name="fileSetting">A FileSetting to remove.</param>
		Public Sub Remove(ByVal fileSetting As FileSetting)
			Dim Index As Integer = MyBase.IndexOf(fileSetting)
			If (Index >= 0) Then
				BaseRemoveAt(Index)
			End If
		End Sub

		''' <summary>
		''' Removes a FileSetting from the collection with a matching path as specified.
		''' </summary>
		''' <param name="path">The path of a FileSetting to remove.</param>
		Public Sub Remove(ByVal path As String)
			Dim Index As Integer = MyBase.IndexOf(path)
			If (Index >= 0) Then
				BaseRemoveAt(Index)
			End If
		End Sub

		''' <summary>
		''' Removes the FileSetting from the collection at the specified index.
		''' </summary>
		''' <param name="index">The index of the FileSetting to remove.</param>
		Public Sub RemoveAt(ByVal index As Integer)
			BaseRemoveAt(index)
		End Sub

#End Region

		''' <summary>
		''' Creates a new element for this collection.
		''' </summary>
		''' <returns>A new instance of FileSetting.</returns>
		Protected Overrides Function CreateNewElement() As ConfigurationElement
			Return New FileSetting()
		End Function

		''' <summary>
		''' Gets the key for the specified element.
		''' </summary>
		''' <param name="element">An element to get a key for.</param>
		''' <returns>A string containing the Path of the FileSetting.</returns>
		Protected Overrides Function GetElementKey(ByVal element As ConfigurationElement) As Object
			If Not element Is Nothing Then
				Return CType(element, FileSetting).Path.ToLower(CultureInfo.InvariantCulture)
			Else
				Return Nothing
			End If
		End Function

	End Class

End Namespace