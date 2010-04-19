Imports System
Imports System.Globalization
Imports System.Web
Imports System.Web.Configuration
Imports SecureSwitch.Configuration

Namespace SecureSwitch

	''' <summary>
	''' Represents an evaluator for requests that 
	''' </summary>
	Public NotInheritable Class RequestEvaluator

		''' <summary>
		''' Evaluates a given request against specified settings for the type of security action required
		''' to fulfill the request properly.
		''' </summary>
		''' <param name="request">The request to evaluate.</param>
		''' <param name="settings">The settings to evaluate against.</param>
		''' <param name="forceEvaluation">
		''' A flag indicating whether or not to force evaluation, despite the mode set.
		''' </param>
		''' <returns>A SecurityType value for the appropriate action.</returns>
		Public Shared Function Evaluate(ByVal request As HttpRequest, ByVal settings As Settings, ByVal forceEvaluation As Boolean) As SecurityType
			' Initialize the result to Ignore.
			Dim Result As SecurityType = SecurityType.Ignore

			' Determine if this request should be ignored based on the settings' Mode.
			Dim MustEvaluateRequest As Boolean = (forceEvaluation OrElse RequestMatchesMode(request, settings.Mode))
			DebugHelper.Output(If(MustEvaluateRequest, "Evaluating request...", "Evaluation of request skipped."))
			If MustEvaluateRequest Then
				' Make sure the request shouldn't be ignored as a HTTP handler.
				If settings.IgnoreHandlers = IgnoreHandlers.BuiltIn AndAlso Not IsBuiltInHandlerRequest(request) OrElse _
				 settings.IgnoreHandlers = IgnoreHandlers.WithStandardExtensions AndAlso Not IsStandardHandlerRequest(request) OrElse _
				 settings.IgnoreHandlers = IgnoreHandlers.None Then
					' Get the relative file path of the current request from the application root.
					Dim RelativeFilePath As String = HttpUtility.UrlDecode(request.Url.AbsolutePath).Remove(0, request.ApplicationPath.Length).ToLower()
					If RelativeFilePath.StartsWith("/") Then
						' Remove any leading "/".
						RelativeFilePath = RelativeFilePath.Substring(1)
					End If

					' Get the relative directory of the current request by removing the last segment of the RelativeFilePath.
					Dim RelativeDirectory As String = String.Empty
					Dim i As Integer = RelativeFilePath.LastIndexOf("/"c)
					If i >= 0 Then
						RelativeDirectory = RelativeFilePath.Substring(0, i)
					End If

					' Determine if there is a matching file path for the current request.
					i = settings.Files.IndexOf(RelativeFilePath)
					If (i >= 0) Then
						Result = settings.Files(i).Secure
						DebugHelper.Output("Request matches file: {0} - {1}.", settings.Files(i).Secure, settings.Files(i).Path)
					Else
						' Try to find a matching directory path.
						Dim j As Integer = -1
						i = 0
						While i < settings.Directories.Count
							' Try to match the beginning of the directory if recursion is allowed (partial match).
							If (settings.Directories(i).Recurse AndAlso RelativeDirectory.StartsWith(settings.Directories(i).Path, StringComparison.CurrentCultureIgnoreCase) OrElse _
							 RelativeDirectory.Equals(settings.Directories(i).Path, StringComparison.CurrentCultureIgnoreCase)) AndAlso _
							 (j = -1 OrElse settings.Directories(i).Path.Length > settings.Directories(j).Path.Length) Then
								' First or longer partial match found (deepest recursion is the best match).
								j = i
							End If

							i += 1
						End While

						If j > -1 Then
							' Indicate a match for a partially matched directory allowing recursion.
							Result = settings.Directories(j).Secure
							DebugHelper.Output("Request matches directory: {0} - {1}.", settings.Directories(j).Secure, settings.Directories(j).Path)
						Else
							' No match indicates an insecure result.
							Result = SecurityType.Insecure
							DebugHelper.Output("Request does not match anything.")
						End If
					End If
				End If
			End If

			Return Result
		End Function

		''' <summary>
		''' Evaluates a given request against configured settings for the type of security action required
		''' to fulfill the request properly.
		''' </summary>
		''' <param name="request">The request to evaluate.</param>
		''' <returns>A SecurityType value for the appropriate action.</returns>
		Public Shared Function Evaluate(ByVal request As HttpRequest) As SecurityType
			' Get the settings for the SecureSwitch section.
			Dim Settings As Settings = TryCast(WebConfigurationManager.GetSection("SecureSwitch"), Settings)

			Return Evaluate(request, Settings, False)
		End Function

		''' <summary>
		''' Determines if the specified request is for one of the built-in HTTP handlers.
		''' </summary>
		''' <param name="request">The HttpRequest to test.</param>
		''' <returns>True if the request is for a built-in HTTP handler; false otherwise.</returns>
		Private Shared Function IsBuiltInHandlerRequest(ByVal request As HttpRequest) As Boolean
			' Get the file name of the request.
			Dim FileName As String = request.Url.Segments(request.Url.Segments.Length - 1)
			Return ( _
			 String.Compare(FileName, "trace.axd", True, CultureInfo.InvariantCulture) = 0 OrElse _
			 String.Compare(FileName, "webresource.axd", True, CultureInfo.InvariantCulture) = 0 _
			)
		End Function

		''' <summary>
		''' Determines if the specified request is for a standard HTTP handler (.axd).
		''' </summary>
		''' <param name="request">The HttpRequest to test.</param>
		''' <returns>True if the request is for a standard HTTP handler (.axd); false otherwise.</returns>
		Private Shared Function IsStandardHandlerRequest(ByVal request As HttpRequest) As Boolean
			Dim Path As String = request.Url.AbsolutePath
			Return ( _
			 Path.EndsWith(".axd", True, CultureInfo.InvariantCulture) OrElse _
			 Path.EndsWith(".ashx", True, CultureInfo.InvariantCulture) OrElse _
			 Path.EndsWith(".asmx/js", True, CultureInfo.InvariantCulture) OrElse _
			 Path.EndsWith(".asmx/jsdebug", True, CultureInfo.InvariantCulture) _
			)
		End Function

		''' <summary>
		''' Tests the given request to see if it matches the specified mode.
		''' </summary>
		''' <param name="request">A HttpRequest to test.</param>
		''' <param name="mode">The Mode used in the test.</param>
		''' <returns>
		'''		Returns true if the request matches the mode as follows:
		'''		<list type="disc">
		'''			<item>If mode is On.</item>
		'''			<item>If mode is set to RemoteOnly and the request is from a computer other than the server.</item>
		'''			<item>If mode is set to LocalOnly and the request is from the server.</item>
		'''		</list>
		'''	</returns>
		Private Shared Function RequestMatchesMode(ByVal request As HttpRequest, ByVal mode As Mode) As Boolean
			Select Case mode
				Case Mode.On
					Return True

				Case Mode.RemoteOnly
					Return (request.ServerVariables("REMOTE_ADDR") <> request.ServerVariables("LOCAL_ADDR"))

				Case Mode.LocalOnly
					Return (request.ServerVariables("REMOTE_ADDR") = request.ServerVariables("LOCAL_ADDR"))

				Case Else
					Return False
			End Select
		End Function

	End Class

End Namespace