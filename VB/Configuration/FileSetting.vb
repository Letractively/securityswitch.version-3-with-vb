Namespace SecureSwitch.Configuration

	''' <summary>
	''' The FileSetting class represents an file entry in the &lt;secureSwitch&gt;
	''' configuration section.
	''' </summary>
	Public Class FileSetting
		Inherits ItemSetting

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
		Public Sub New(ByVal path As String, ByVal secure As SecurityType)
			MyBase.New(path, secure)
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
	''' The FileSettingCollection class houses a collection of FileSetting instances.
	''' </summary>
	Public Class FileSettingCollection
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
		Default Public ReadOnly Property Item(ByVal index As Integer) As FileSetting
			Get
				Return CType(List(index), FileSetting)
			End Get
		End Property

		''' <summary>
		''' Adds the item to the collection.
		''' </summary>
		''' <param name="item">The item to add.</param>
		Public Function Add(ByVal item As FileSetting) As Integer
			Return List.Add(item)
		End Function

		''' <summary>
		''' Inserts an item into the collection at the specified index.
		''' </summary>
		''' <param name="index">The index to insert the item at.</param>
		''' <param name="item">The item to insert.</param>
		Public Sub Insert(ByVal index As Integer, ByVal item As FileSetting)
			List.Insert(index, item)
		End Sub

		''' <summary>
		''' Removes an item from the collection.
		''' </summary>
		''' <param name="item">The item to remove.</param>
		Public Sub Remove(ByVal item As FileSetting)
			List.Remove(item)
		End Sub

	End Class

End Namespace