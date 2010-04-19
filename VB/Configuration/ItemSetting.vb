Imports System
Imports System.Configuration
Imports System.Globalization
Imports System.Text.RegularExpressions

Namespace SecureSwitch.Configuration

	''' <summary>
	''' Indicates the type of security for a file or directory.
	''' </summary>
	Public Enum SecurityType

		''' <summary>
		''' The item should be secure.
		''' </summary>
		Secure

		''' <summary>
		''' The item should be insecure.
		''' </summary>
		Insecure

		''' <summary>
		''' The item should be ignored.
		''' </summary>
		Ignore

	End Enum

	''' <summary>
	''' Represents an file or directory entry in the &lt;secureSwitch&gt;
	''' configuration section.
	''' </summary>
	Public MustInherit Class ItemSetting
		Inherits ConfigurationElement

#Region "Constructors"

		''' <summary>
		''' Creates an instance of ItemSetting.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Creates an instance with an initial path value.
		''' </summary>
		''' <param name="path">The relative path to the file.</param>
		Public Sub New(ByVal path As String)
			Me.New()
			Me.Path = path
		End Sub

		''' <summary>
		''' Creates an instance with initial values.
		''' </summary>
		''' <param name="path">The relative path to the file.</param>
		''' <param name="secure">The type of security for the file.</param>
		Public Sub New(ByVal path As String, ByVal secure As SecurityType)
			Me.New(path)
			Me.Secure = secure
		End Sub

#End Region

#Region "Properties"

		''' <summary>
		''' Gets or sets the path of this item.
		''' </summary>
		<ConfigurationProperty("path", IsRequired:=True, IsKey:=True)> _
		Public Overridable Property Path() As String
			Get
				Return CStr(Me("path"))
			End Get
			Set(ByVal value As String)
				Me("path") = value
			End Set
		End Property

		''' <summary>
		''' Gets or sets the type of security for this item.
		''' </summary>
		<ConfigurationProperty("secure", DefaultValue:=SecurityType.Secure)> _
		Public Property Secure() As SecurityType
			Get
				Return CType(Me("secure"), SecurityType)
			End Get
			Set(ByVal value As SecurityType)
				Me("secure") = value
			End Set
		End Property

#End Region

	End Class

	''' <summary>
	''' Represents a collection of ItemSetting objects.
	''' </summary>
	Public MustInherit Class ItemSettingCollection
		Inherits ConfigurationElementCollection

		''' <summary>
		''' Returns the index of a specified item in the collection.
		''' </summary>
		''' <param name="item">The item to find.</param>
		''' <returns>Returns the index of the item.</returns>
		Public Function IndexOf(ByVal item As ItemSetting) As Integer
			If Not item Is Nothing Then
				Return BaseIndexOf(item)
			Else
				Return -1
			End If
		End Function

		''' <summary>
		''' Returns the index of an item with the specified path in the collection.
		''' </summary>
		''' <param name="Path">The path of the item to find.</param>
		''' <returns>Returns the index of the item with the path.</returns>
		Public Function IndexOf(ByVal path As String) As Integer
			If path Is Nothing Then
				Throw New ArgumentNullException("path")
			Else
				Return Me.IndexOf(CType(BaseGet(path.ToLower(CultureInfo.InvariantCulture)), ItemSetting))
			End If
		End Function

	End Class

End Namespace