namespace GameCreator.ModuleManager
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Networking;
    using UnityEditor;

    public abstract class ModuleManagerStore
    {
        public class RequestData
        {
            public UnityWebRequest request;
            public string error = "";
            public UnityAction<bool, string> callback;

            public RequestData(UnityWebRequest request, UnityAction<bool, string> callback)
            {
                this.request = request;
                this.callback = callback;
            }
        }

        public enum StoreRequestStatus
        {
            None,
            Requesting,
            Error,
            Complete
        }

        private const string STORE_API_URI = "https://store.gamecreator.io/api/catalogue/";

        // JSON CLASSES: --------------------------------------------------------------------------

        [System.Serializable]
        public class ReceivedData
        {
            public Version version = new Version(0, 0, 0);
            public Catalogue[] catalogue = new Catalogue[0];
        }

        [System.Serializable]
        public class Catalogue
        {
            public string mid = "";
            public string name = "";
            public string description = "";
            public string version = "";
            public string date = "";
            public bool beta = false;
            public Tag[] tags = new Tag[0];
        }

        [System.Serializable]
        public class Tag
        {
            public string name = "";
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public static StoreRequestStatus REQUEST_STATUS = StoreRequestStatus.None;
        public static RequestData REQUEST_DATA;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static void LoadStoreModules()
        {
            REQUEST_STATUS = StoreRequestStatus.Requesting;
            UnityWebRequest request = UnityWebRequest.Get(STORE_API_URI);
            REQUEST_DATA = new RequestData(request, CallbackAPIStoreComplete);

            EditorApplication.update += StoreRequestUpdate;

            #if UNITY_2017_2_OR_NEWER
            REQUEST_DATA.request.SendWebRequest();
            #else
            REQUEST_DATA.request.Send();
            #endif
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static void StoreRequestUpdate()
        {
            if (!REQUEST_DATA.request.isDone) return;

            if (REQUEST_DATA.request.isNetworkError || REQUEST_DATA.request.isHttpError)
            {
                REQUEST_DATA.callback.Invoke(true, REQUEST_DATA.request.error);
            }
            else if (REQUEST_DATA.request.responseCode == 200)
            {
                REQUEST_DATA.callback.Invoke(false, REQUEST_DATA.request.downloadHandler.text);
            }
            else
            {
                REQUEST_DATA.callback.Invoke(true, "Unhandled error");
            }

            EditorApplication.update -= ModuleManagerStore.StoreRequestUpdate;
        }

        private static void CallbackAPIStoreComplete(bool isError, string content)
        {
            if (isError)
            {
                REQUEST_STATUS = StoreRequestStatus.Error;
                REQUEST_DATA.error = content;
            }
            else
            {
                ReceivedData dataReceived = new ReceivedData();
                EditorJsonUtility.FromJsonOverwrite(content, dataReceived);

                if (dataReceived != null)
                {
                    List<Module> modules = new List<Module>();
                    for (int i = 0; i < dataReceived.catalogue.Length; ++i)
                    {
                        Module module = new Module();
                        module.moduleID = dataReceived.catalogue[i].mid;
                        module.displayName = dataReceived.catalogue[i].name;
                        module.description = dataReceived.catalogue[i].description;
                        module.version = new Version(dataReceived.catalogue[i].version);
                        module.tags = new string[dataReceived.catalogue[i].tags.Length];
                        for (int j = 0; j < dataReceived.catalogue[i].tags.Length; ++j)
                        {
                            module.tags[j] = dataReceived.catalogue[i].tags[j].name;
                        }

                        modules.Add(module);
                    }

                    ModuleManager.AddStoreModules(modules.ToArray());
                    REQUEST_STATUS = StoreRequestStatus.Complete;
                }
                else
                {
                    REQUEST_STATUS = StoreRequestStatus.Error;
                    REQUEST_DATA.error = "Unable to parse server response";
                }
            }

            if (ModuleManagerWindow.WINDOW != null) ModuleManagerWindow.WINDOW.Repaint();
        }
    }
}