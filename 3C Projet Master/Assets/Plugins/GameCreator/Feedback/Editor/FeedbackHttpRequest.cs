namespace GameCreator.Feedback
{
	using System.Text;
	using System.Collections;
	using System.Collections.Generic;
	using System.Security.Cryptography;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.Networking;
	using UnityEditor;

	public abstract class FeedbackHttpRequest
	{
		private class RequestData
		{
			public UnityWebRequest request;
			public UnityAction<bool, string> callback;

			public RequestData(UnityWebRequest request, UnityAction<bool, string> callback)
			{
				this.request = request;
				this.callback = callback;
			}
		}

		[System.Serializable]
		public class Data
		{
			public string name = "";
			public string mail = "";
			public string feedback = "";
			public string content = "";

			public Data(string name, string mail, string feedback, string content)
			{
				this.name = WWW.EscapeURL(name);
				this.mail = WWW.EscapeURL(mail);
				this.feedback = WWW.EscapeURL(feedback);
				this.content = WWW.EscapeURL(content);
			}
		}

		private const string ERR_NO_INTERNET = "No internet connection";
		private const string URL = "https://game-creator-feedback-ycpcqoeo.appspot.com/";

		private static RequestData REQUEST_DATA;

		// PUBLIC METHODS: ---------------------------------------------------------------------------------------------

		public static void Request(Data data, UnityAction<bool, string> callback)
		{
			if (Application.internetReachability == NetworkReachability.NotReachable) 
			{
				callback.Invoke(false, ERR_NO_INTERNET);
				return;
			}

			WWWForm post = new WWWForm();
			post.AddField("ivpednoikpbtixjikrqy", data.name);
			post.AddField("wrzfvgnyhbuqkpacojkk", data.mail);
			post.AddField("rhoiwmbrbrsopubhfglc", data.feedback);
			post.AddField("lptyesmrftaavojjfwcz", data.content);

			UnityWebRequest request = UnityWebRequest.Post(URL, post);
			REQUEST_DATA = new RequestData(request, callback);

			EditorApplication.update += FeedbackHttpRequest.EditorUpdate;

			#if UNITY_2017_2_OR_NEWER
			REQUEST_DATA.request.SendWebRequest();
			#else
			REQUEST_DATA.request.Send();
			#endif
		}

		// PRIVATE METHODS: --------------------------------------------------------------------------------------------

		private static void EditorUpdate()
		{
			if (!REQUEST_DATA.request.isDone) return;

			if (REQUEST_DATA.request.responseCode == 200) 
			{
				REQUEST_DATA.callback.Invoke(false, REQUEST_DATA.request.downloadHandler.text);
			}
			else 
			{
				Debug.LogWarning(REQUEST_DATA.request.error);
				REQUEST_DATA.callback.Invoke(true, REQUEST_DATA.request.error);
			}
			
			EditorApplication.update -= FeedbackHttpRequest.EditorUpdate;
		}
	}
}