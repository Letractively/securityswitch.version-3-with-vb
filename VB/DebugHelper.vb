Imports System.Web

''' <summary>
''' A helper class for debugging.
''' </summary>
Friend Class DebugHelper

	' The category to use when tracing.
	Const TraceCategory As String = "SecuritySwitchModule"

	' Holds a reference to the current trace context.
	Private Shared _trace As TraceContext
	Private Shared ReadOnly Property Trace() As TraceContext
		Get
			If _trace Is Nothing Then
				_trace = HttpContext.Current.Trace
			End If
			Return _trace
		End Get
	End Property


	Friend Shared Sub Output(ByVal message As String)
		Trace.Write(TraceCategory, message)
	End Sub

	Friend Shared Sub Output(ByVal formattedMessage As String, ByVal ParamArray args As Object())
		Trace.Write(TraceCategory, String.Format(formattedMessage, args))
	End Sub

	Friend Shared Sub OutputIf(ByVal condition As Boolean, ByVal message As String)
		If condition Then
			Trace.Write(TraceCategory, message)
		End If
	End Sub

	Friend Shared Sub OutputIf(ByVal condition As Boolean, ByVal formattedMessage As String, ByVal ParamArray args As Object())
		If condition Then
			Trace.Write(TraceCategory, String.Format(formattedMessage, args))
		End If
	End Sub

End Class