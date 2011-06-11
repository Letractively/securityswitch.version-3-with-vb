Imports System
Imports System.Globalization
Imports System.Web
Imports SecuritySwitch.Configuration

Namespace SecuritySwitch

	''' <summary>
	''' Provides shared methods for ensuring that a page is rendered 
	''' securely via SSL or unsecurely.
	''' </summary>
	Public NotInheritable Class SslHelper

		' Protocol prefixes.
		Private Const UnsecureProtocolPrefix As String = "http://"
		Private Const SecureProtocolPrefix As String = "https://"

		''' <summary>
		''' Prevent creating an instance of this class.
		''' </summary>
		Private Sub New()
		End Sub

		''' <summary>
		''' Determines the secure page that should be requested if a redirect occurs.
		''' </summary>
		''' <param name="settings">The Settings to use in determining.</param>
		''' <param name="ignoreCurrentProtocol">
		''' A flag indicating whether or not to ingore the current protocol when determining.
		''' </param>
		''' <returns>A string containing the absolute URL of the secure page to redirect to.</returns>
		''' <exception cref="ArgumentNullException"></exception>
		Public Shared Function DetermineSecurePage(ByVal settings As Settings, ByVal ignoreCurrentProtocol As Boolean) As String
			If settings Is Nothing Then
				Throw New ArgumentNullException("settings")
			End If

			Dim Result As String = Nothing
			Dim Request As HttpRequest = HttpContext.Current.Request

			' Is this request already secure?
			Dim RequestPath As String = Request.Url.AbsoluteUri
			If ignoreCurrentProtocol OrElse RequestPath.StartsWith(UnsecureProtocolPrefix) Then
				' Is there a different URI to redirect to?
				If String.IsNullOrEmpty(settings.EncryptedUri) Then
					' Replace the protocol of the requested URL with "https".
					' * Account for cookieless sessions by applying the application modifier.
					Result = String.Concat( _
					 SecureProtocolPrefix, _
					 Request.Url.Authority, _
					 HttpContext.Current.Response.ApplyAppPathModifier(Request.Path), _
					 Request.Url.Query _
					)
				Else
					' Build the URL with the "https" protocol.
					Result = BuildUrl(True, settings.MaintainPath, settings.EncryptedUri, settings.UnencryptedUri)
				End If
			End If

			Return Result
		End Function

		''' <summary>
		''' Determines the unsecure page that should be requested if a redirect occurs.
		''' </summary>
		''' <param name="settings">The Settings to use in determining.</param>
		''' <param name="ignoreCurrentProtocol">
		''' A flag indicating whether or not to ingore the current protocol when determining.
		''' </param>
		''' <returns>A string containing the absolute URL of the unsecure page to redirect to.</returns>
		''' <exception cref="ArgumentNullException"></exception>
		Public Shared Function DetermineUnsecurePage(ByVal settings As Settings, ByVal ignoreCurrentProtocol As Boolean) As String
			If settings Is Nothing Then
				Throw New ArgumentNullException("settings")
			End If

			Dim Result As String = Nothing
			Dim Request As HttpRequest = HttpContext.Current.Request

			' Is this request secure?
			Dim RequestPath As String = HttpContext.Current.Request.Url.AbsoluteUri
			If ignoreCurrentProtocol OrElse RequestPath.StartsWith(SecureProtocolPrefix) Then
				' Is there a different URI to redirect to?
				If String.IsNullOrEmpty(settings.UnencryptedUri) Then
					' Replace the protocol of the requested URL with "http".
					' * Account for cookieless sessions by applying the application modifier.
					Result = String.Concat( _
					 UnsecureProtocolPrefix, _
					 Request.Url.Authority, _
					 HttpContext.Current.Response.ApplyAppPathModifier(Request.Path), _
					 Request.Url.Query _
					)
				Else
					' Build the URL with the "http" protocol.
					Result = BuildUrl(False, settings.MaintainPath, settings.EncryptedUri, settings.UnencryptedUri)
				End If
			End If

			Return Result
		End Function

		''' <summary>
		''' Requests the current page over a secure connection, if it is not already.
		''' </summary>
		''' <param name="settings">The Settings to use for this request.</param>
		Public Shared Sub RequestSecurePage(ByVal settings As Settings)
			' Determine the response path, if any.
			Dim ResponsePath As String = DetermineSecurePage(settings, False)
			DebugHelper.Output(If(Not String.IsNullOrEmpty(ResponsePath), "Requesting secured page (HTTPS).", "Request is already secure."))
			If Not String.IsNullOrEmpty(ResponsePath) Then
				' Redirect to the secure page.
				HttpContext.Current.Response.Redirect(ResponsePath, True)
			End If
		End Sub

		''' <summary>
		''' Requests the current page over an unsecure connection, if it is not already.
		''' </summary>
		''' <param name="settings">The Settings to use for this request.</param>
		''' <exception cref="ArgumentNullException"></exception>
		Public Shared Sub RequestUnsecurePage(ByVal settings As Settings)
			If settings Is Nothing Then
				Throw New ArgumentNullException("settings")
			End If

			' Determine the response path, if any.
			Dim ResponsePath As String = DetermineUnsecurePage(settings, False)
			DebugHelper.Output(If(Not String.IsNullOrEmpty(ResponsePath), "Requesting unsecured page (HTTP).", "Request is already unsecured."))
			If Not String.IsNullOrEmpty(ResponsePath) Then
				Dim Request As HttpRequest = HttpContext.Current.Request

				' Test for the need to bypass a security warning.
				Dim Bypass As Boolean
				If settings.WarningBypassMode = SecurityWarningBypassMode.AlwaysBypass Then
					Bypass = True
				ElseIf settings.WarningBypassMode = SecurityWarningBypassMode.BypassWithQueryParam AndAlso _
				  Not Request.QueryString(settings.BypassQueryParamName) Is Nothing Then
					Bypass = True

					' Remove the bypass query parameter from the URL.
					Dim NewPath As New System.Text.StringBuilder(ResponsePath)
					Dim i As Integer = ResponsePath.LastIndexOf(String.Format("?{0}=", settings.BypassQueryParamName))
					If i < 0 Then
						i = ResponsePath.LastIndexOf(String.Format("&{0}=", settings.BypassQueryParamName))
					End If
					NewPath.Remove(i, settings.BypassQueryParamName.Length + Request.QueryString(settings.BypassQueryParamName).Length + 1)

					' Remove any abandoned "&" character.
					If i >= NewPath.Length Then
						i = NewPath.Length - 1
					End If
					If NewPath(i) = "&"c Then
						NewPath.Remove(i, 1)
					End If

					' Remove any abandoned "?" character.
					i = NewPath.Length - 1
					If NewPath(i) = "?"c Then
						NewPath.Remove(i, 1)
					End If

					ResponsePath = NewPath.ToString()
				Else
					Bypass = False
				End If

				' Output a redirector for the needed page to avoid a security warning.
				Dim Response As HttpResponse = HttpContext.Current.Response
				If Bypass Then
					' Clear the current response.
					Response.Clear()

					' Add a refresh header to the response for the new path.
					Response.AddHeader("Refresh", String.Concat("0;URL=", ResponsePath))

					' Also, add JavaScript to replace the current location as backup.
					Response.Write("<html><head><title></title>")
					Response.Write("<!-- <script language=""javascript"">window.location.replace(""")
					Response.Write(ResponsePath)
					Response.Write(""")</script> -->")
					Response.Write("</head><body></body></html>")

					Response.End()
				Else
					' Redirect to the unsecure page.
					Response.Redirect(ResponsePath, True)
				End If
			End If
		End Sub

		''' <summary>
		''' Builds a URL from the given protocol and appropriate host path. The resulting URL 
		''' will maintain the current path if requested.
		''' </summary>
		''' <param name="secure">Is this to be a secure URL?</param>
		''' <param name="maintainPath">Should the current path be maintained during transfer?</param>
		''' <param name="encryptedUri">The URI to redirect to for encrypted requests.</param>
		''' <param name="unencryptedUri">The URI to redirect to for standard requests.</param>
		''' <returns></returns>
		Private Shared Function BuildUrl(ByVal secure As Boolean, ByVal maintainPath As Boolean, ByVal encryptedUri As String, ByVal unencryptedUri As String) As String
			' Clean the URIs.
			encryptedUri = CleanHostUri(CStr(IIf(String.IsNullOrEmpty(encryptedUri), unencryptedUri, encryptedUri)))
			unencryptedUri = CleanHostUri(CStr(IIf(String.IsNullOrEmpty(unencryptedUri), encryptedUri, unencryptedUri)))

			' Get the current request.
			Dim Request As HttpRequest = HttpContext.Current.Request

			' Prepare to build the needed URL.
			Dim Url As New System.Text.StringBuilder()

			' Host authority (e.g. secure.mysite.com/).
			If secure Then
				Url.Append(encryptedUri)
			Else
				Url.Append(unencryptedUri)
			End If

			If maintainPath Then
				' Append the current file path.
				Url.Append(Request.CurrentExecutionFilePath).Append(Request.Url.Query)
			Else
				' Append just the current page
				Dim CurrentUrl As String = Request.Url.AbsolutePath
				Url.Append(CurrentUrl.Substring(CurrentUrl.LastIndexOf("/"c) + 1)).Append(Request.Url.Query)
			End If

			' Replace any double slashes with a single slash.
			Url.Replace("//", "/")

			' Prepend the protocol.
			If secure Then
				Url.Insert(0, SecureProtocolPrefix)
			Else
				Url.Insert(0, UnsecureProtocolPrefix)
			End If

			Return Url.ToString()
		End Function

		''' <summary>
		''' Cleans a host path by stripping out any unneeded elements.
		''' </summary>
		''' <param name="uri">The host URI to validate.</param>
		''' <returns>Returns a string that is stripped as needed.</returns>
		Private Shared Function CleanHostUri(ByVal uri As String) As String
			Dim Result As String = String.Empty
			If Not String.IsNullOrEmpty(uri) Then
				' Ensure there is a protocol or a Uri cannot be constructed.
				If Not uri.StartsWith(UnsecureProtocolPrefix) AndAlso Not uri.StartsWith(SecureProtocolPrefix) Then
					uri = UnsecureProtocolPrefix + uri
				End If

				' Extract the authority and path to build a string suitable for our needs.
				Dim HostUri As New Uri(uri)
				Result = String.Concat(HostUri.Authority, HostUri.AbsolutePath)
				If Not Result.EndsWith("/") Then
					Result += "/"
				End If
			End If

			Return Result
		End Function

	End Class

End Namespace