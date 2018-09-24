namespace GameCreator.Camera
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

	public class ICameraMotorTypeEditor : Editor 
	{
		private const string PROP_LOOKAT = "lookAt";

        // PROPERTIES: ----------------------------------------------------------------------------

        private bool initialize = true;

		// SEALED METHODS: ------------------------------------------------------------------------

        public bool PaintInspectorMotor(Transform cameraMotorTransform)
		{
            serializedObject.Update();
            if (this.initialize)
            {
                this.OnSubEnable();
                this.initialize = false;
            }

			bool forceRepaint = this.OnSubInspectorGUI(cameraMotorTransform);

			serializedObject.ApplyModifiedProperties();
			return forceRepaint;
		}

        public bool PaintSceneMotor(Transform cameraMotorTransform)
        {
            if (this.initialize)
            {
                this.OnSubEnable();
                this.initialize = false;
            }

            return this.OnSubSceneGUI(cameraMotorTransform);
        }

		// VIRTUAL METHODS: -----------------------------------------------------------------------

		public virtual void OnCreate(Transform cameraMotorTransform) {}

        protected virtual void OnSubEnable() 
        { 
            return; 
        }

        protected virtual bool OnSubInspectorGUI(Transform cameraMotorTransform)
        {
            return false;
        }

		public virtual bool OnSubSceneGUI(Transform cameraMotorTransform) 
		{ 
			return false; 
		}

        public virtual bool ShowPreviewCamera()
        {
            return true;
        }
    }
}