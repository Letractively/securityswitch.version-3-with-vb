Namespace SecureSwitch.Configuration

	''' <summary>
	''' The DirectorySetting class represents a directory entry in the &lt;secureSwitch&gt;
	''' configuration section.
	''' </summary>
	Public Class DirectorySetting
		Inherits ItemSetting

		' Fields
		Private _recurse As Boolean = False

		''' <summary>
		''' Gets or sets a flag indicating whether or not to include all files in any sub-directories 
		''' when evaluating a request.
		''' </summary>
		Public Property Recurse() As Boolean
			Get
				Return _recurse
			End Get
			Set(ByVal Value As Boolean)
				_recurse = Value
			End Set
		End Property

		''' <summary>
		''' Creates an instance with default values.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Creates an instance with initial values.
		''' </summary>
		''' <param name="path">The relative path to the directory or file.</param>
		''' <param name="ignore">A flag to ignore security for the directory or file.</param>
		Public Sub New(ByVal path As String, ByVal secure As SecurityType, ByVal recurse As Boolean)
			MyBase.New(path, secure)
			Me._recurse = recurse
		End Sub

		''' <summary>
		''' Creates an instance with an initial path value.
		''' </summary>
		''' <param name="path">The relative path to the directory or file.</param>
		Public Sub New(ByVal path As String)
			MyBase.New(path)
		End Sub

	End Class

	''' <summary>
	''' The DirectorySettingCollection class houses a collection of DirectorySetting instances.
	''' </summary>
	Public Class DirectorySettingCollection
		Inherits ItemSettingCollection

		''' <summary>
		''' Initialize an instance of this collection.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Indexer for the collection.
		''' </summary>
		Default Public ReadOnly Property Item(ByVal index As Integer) As DirectorySetting
			Get
				Return CType(List(index), DirectorySetting)
			End Get
		End Property

		''' <summary>
		''' Adds the item to the collection.
		''' </summary>
		''' <param name="item">The item to add.</param>
		Public Function Add(ByVal item As DirectorySetting) As Integer
			Return List.Add(item)
		End Function

		''' <summary>
		''' Inserts an item into the collection at the specified index.
		''' </summary>
		''' <param name="index">The index to insert the item at.</param>
		''' <param name="item">The item to insert.</param>
		Public Sub Insert(ByVal index As Integer, ByVal item As DirectorySetting)
			List.Insert(index, item)
		End Sub

		''' <summary>
		''' Removes an item from the collection.
		''' </summary>
		''' <param name="item">The item to remove.</param>
		Public Sub Remove(ByVal item As DirectorySetting)
			List.Remove(item)
		End Sub

	End Class

End Namespace