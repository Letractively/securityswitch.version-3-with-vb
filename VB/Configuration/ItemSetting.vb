Imports System.Collections

Namespace SecureSwitch.Configuration

	''' <summary>
	''' Indicates the type of security for a ItemSetting.
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
	''' The ItemSettingComparer class implements the IComparer interface to compare.
	''' </summary>
	Public Class ItemSettingComparer
		Implements IComparer

		''' <summary>
		''' Compares the two objects as string and ItemSetting or both ItemSetting 
		''' by the Path property.
		''' </summary>
		''' <param name="x">First object to compare.</param>
		''' <param name="y">Second object to compare.</param>
		''' <returns></returns>
		Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare
			' Check the type of the parameters
			If Not TypeOf x Is ItemSetting AndAlso Not TypeOf x Is String Then
				' Throw an exception for the first argument
				Throw New ArgumentException("Parameter must be a ItemSetting or a String.", "x")
			ElseIf Not TypeOf y Is ItemSetting AndAlso Not TypeOf y Is String Then
				' Throw an exception for the second argument
				Throw New ArgumentException("Parameter must be a ItemSetting or a String.", "y")
			End If

			' Initialize the path variables
			Dim xPath As String = String.Empty
			Dim yPath As String = String.Empty

			' Get the path for x
			If TypeOf x Is ItemSetting Then
				xPath = CType(x, ItemSetting).Path
			Else
				xPath = CType(x, String)
			End If

			' Get the path for y
			If TypeOf y Is ItemSetting Then
				yPath = CType(y, ItemSetting).Path
			Else
				yPath = CType(y, String)
			End If

			' Compare the paths, ignoring case
			Return String.Compare(xPath, yPath, True)
		End Function

	End Class


	''' <summary>
	''' The ItemSetting class is the base class that represents entries in the &lt;secureSwitch&gt;
	''' configuration section.
	''' </summary>
	Public Class ItemSetting
		' Fields
		Private _secure As SecurityType = SecurityType.Secure
		Private _path As String = String.Empty

		''' <summary>
		''' Gets or sets the type of security for this directory or file.
		''' </summary>
		Public Property Secure() As SecurityType
			Get
				Return _secure
			End Get
			Set(ByVal Value As SecurityType)
				_secure = Value
			End Set
		End Property

		''' <summary>
		''' Gets or sets the path of this directory or file.
		''' </summary>
		Public Property Path() As String
			Get
				Return _path
			End Get
			Set(ByVal Value As String)
				_path = Value
			End Set
		End Property

		''' <summary>
		''' Creates an instance of this class.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Creates an instance with initial values.
		''' </summary>
		''' <param name="path">The relative path to the directory or file.</param>
		''' <param name="ignore">A flag to ignore security for the directory or file.</param>
		Public Sub New(ByVal path As String, ByVal secure As SecurityType)
			' Initialize the path and secure properties
			Me._path = path
			Me._secure = secure
		End Sub

		''' <summary>
		''' Creates an instance with an initial path value.
		''' </summary>
		''' <param name="path">The relative path to the directory or file.</param>
		Public Sub New(ByVal path As String)
			Me.New(path, SecurityType.Secure)
		End Sub

	End Class

	''' <summary>
	''' The ItemSettingCollection class houses a collection of ItemSetting instances.
	''' </summary>
	Public Class ItemSettingCollection
		Inherits CollectionBase

		''' <summary>
		''' Initialize an instance of this collection.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Returns the index of a specified item in the collection.
		''' </summary>
		''' <param name="Item">The item to find.</param>
		''' <returns>Returns the index of the item.</returns>
		Public Function IndexOf(ByVal item As ItemSetting) As Integer
			Return List.IndexOf(item)
		End Function

		''' <summary>
		''' Returns the index of an item with the specified path in the collection.
		''' </summary>
		''' <param name="path">The path of the item to find.</param>
		''' <returns>Returns the index of the item with the path.</returns>
		Public Function IndexOf(ByVal path As String) As Integer
			' Create a comparer for sorting and searching
			Dim Comparer As New ItemSettingComparer
			InnerList.Sort(Comparer)
			Return InnerList.BinarySearch(path, Comparer)
		End Function

	End Class

End Namespace