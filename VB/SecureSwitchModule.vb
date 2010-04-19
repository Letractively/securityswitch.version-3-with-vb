Imports System.Configuration
Imports System.Web
Imports SecureSwitch.Configuration

Namespace SecureSwitch

	''' <summary>
	''' The SecureSwitchModule class hooks the application's BeginRequest event
	''' in order to request the current page securely if specified in the 
	''' configuration file.
	''' </summary>
	Public Class SecureSwitchModule
		Implements IHttpModule

		''' <summary>
		''' Initializes an instance of this class.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Disposes of any resources used.
		''' </summary>
		Public Sub Dispose() Implements IHttpModule.Dispose
			' No resources were used.
		End Sub

		''' <summary>
		''' Occurs just before the SecureSwitchModule evaluates the current request.
		''' </summary>
		Public Event BeforeEvaluateRequest As BeforeEvaluateRequestEventHandler

		''' <summary>
		''' Initializes the module by hooking the application's BeginRequest event if indicated by the config settings.
		''' </summary>
		''' <param name="Application">The HttpApplication this module is bound to.</param>
		Public Sub Init(ByVal context As HttpApplication) Implements IHttpModule.Init
			If Not context Is Nothing Then
				' Get the settings for the SecureSwitch section
				Dim Settings As Settings = CType(ConfigurationSettings.GetConfig("SecureSwitch"), Settings)
				If Not Settings Is Nothing AndAlso Settings.Mode <> Mode.Off Then
					' Store the settings in application state for quick access on each request
					context.Application("Settings") = Settings

					' Add a reference to the Application_ProcessRequest handler for the application's
					' AcquireRequestState event.
					' * This ensures that the session ID is available for cookie-less session processing.
					AddHandler context.AcquireRequestState, AddressOf Me.Application_ProcessRequest
				End If
			End If
		End Sub

		''' <summary>
		''' Raises the BeforeEvaluateRequest event.
		''' </summary>
		''' <param name="e">The EvaluateRequestEventArgs used for the event.</param>
		Protected Sub OnBeforeEvaluateRequest(ByVal e As EvaluateRequestEventArgs)
			' Raise the event.
			RaiseEvent BeforeEvaluateRequest(Me, e)
		End Sub

		''' <summary>
		''' Process this request by evaluating it appropriately.
		''' </summary>
		''' <param name="source">The source of the event.</param>
		''' <param name="e">EventArgs passed in.</param>
		Private Sub Application_ProcessRequest(ByVal source As Object, ByVal e As EventArgs)
			' Cast the source as an HttpApplication instance.
			Dim Context As HttpApplication = Nothing
			If TypeOf source Is HttpApplication Then
				Context = CType(source, HttpApplication)
			End If
			
			If Not Context Is Nothing Then
				' Retrieve the settings from application state.
				Dim Settings As Settings = CType(Context.Application("Settings"), Settings)

				' Call the BeforeEvaluateRequest event and check if a subscriber indicated to cancel the 
				' evaluation of the current request.
				Dim Args As New EvaluateRequestEventArgs(Context, Settings)
				OnBeforeEvaluateRequest(Args)

				If Not Args.CancelEvaluation Then
					' Evaluate the response against the settings.
					Dim Secure As SecurityType = RequestEvaluator.Evaluate(Context.Request, Settings, False)

					' Take appropriate action.
					If Secure = SecurityType.Secure Then
						SslHelper.RequestSecurePage(Settings)
					ElseIf Secure = SecurityType.Insecure Then
						SslHelper.RequestUnsecurePage(Settings)
					End If
				End If
			End If
		End Sub

	End Class


	''' <summary>
	''' Represents the method that handles the event raised just before a request is evaluated by 
	''' the SecureSwitchModule.
	''' </summary>
	''' <param name="sender">The SecureSwitchModule that is the source of the event.</param>
	''' <param name="e">An EvaluateRequestEventArgs that contains the event data.</param>
	Public Delegate Sub BeforeEvaluateRequestEventHandler(ByVal sender As Object, ByVal e As EvaluateRequestEventArgs)

End Namespace