Imports System
Imports System.Web
Imports SecuritySwitch.Configuration

Namespace SecuritySwitch

	''' <summary>
	''' Defines the arguments used for the EvaluateRequest event.
	''' </summary>
	''' <remarks></remarks>
	Public Class EvaluateRequestEventArgs

		' Fields.
		Private _application As HttpApplication
		Private _cancelEvaluation As Boolean = False
		Private _settings As Settings


		''' <summary>
		''' Gets the HttpApplication used to evaluate the request.
		''' </summary>
		Public ReadOnly Property Application() As HttpApplication
			Get
				Return _application
			End Get
		End Property

		''' <summary>
		''' Gets or sets a flag indicating whether or not to cancel the evaluation.
		''' </summary>
		Public Property CancelEvaluation() As Boolean
			Get
				Return _cancelEvaluation
			End Get
			Set(ByVal value As Boolean)
				_cancelEvaluation = value
			End Set
		End Property

		''' <summary>
		''' Gets the Settings used to evaluate the request.
		''' </summary>
		Public ReadOnly Property Settings() As Settings
			Get
				Return _settings
			End Get
		End Property

		''' <summary>
		''' Creates an instance of EvaluateRequestEventArgs with an instance of Settings.
		''' </summary>
		''' <param name="application">The HttpApplication for the current context.</param>
		''' <param name="settings">An instance of Settings used for the evaluation of the request.</param>
		Public Sub New(ByVal application As HttpApplication, ByVal settings As Settings)
			MyBase.New()
			_application = application
			_settings = settings
		End Sub

	End Class

End Namespace