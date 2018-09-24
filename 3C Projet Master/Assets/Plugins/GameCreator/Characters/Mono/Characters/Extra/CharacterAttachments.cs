namespace GameCreator.Characters
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    public class CharacterAttachments : MonoBehaviour
    {
        [System.Serializable]
        public class EventData
        {
            public GameObject attachment;
            public HumanBodyBones bone;
            public bool isDestroy;
        }

        [System.Serializable]
        public class AttachmentEvent : UnityEvent<EventData>
        { }

        // PROPERTIES: ----------------------------------------------------------------------------

        private Animator animator;
        public Dictionary<HumanBodyBones, List<GameObject>> attachments { get; private set; }

        public AttachmentEvent onAttach = new AttachmentEvent();
        public AttachmentEvent onDetach = new AttachmentEvent();

        // INITIALIZERS: --------------------------------------------------------------------------

        public void Setup(Animator animator)
        {
            this.animator = animator;
            this.attachments = new Dictionary<HumanBodyBones, List<GameObject>>();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Attach(HumanBodyBones bone, GameObject instance, Vector3 position, Quaternion rotation, Space space = Space.Self)
        {
            if (!this.attachments.ContainsKey(bone)) this.attachments.Add(bone, new List<GameObject>());

            if (string.IsNullOrEmpty(instance.scene.name))
            {
                instance = Instantiate(instance, Vector3.zero, Quaternion.identity);
            }

            instance.transform.SetParent(this.animator.GetBoneTransform(bone));

            switch (space)
            {
                case Space.Self :
                    instance.transform.localPosition = position;
                    instance.transform.localRotation = rotation;
                    break;

                case Space.World:
                    instance.transform.position = position;
                    instance.transform.rotation = rotation;
                    break;
            }

            this.attachments[bone].Add(instance);
            if (this.onAttach != null)
            {
                this.onAttach.Invoke(new EventData
                {
                    attachment = instance,
                    bone = bone
                });
            }
        }

        public List<GameObject> Detach(HumanBodyBones bone)
        {
            return this.DetachOrDestroy(bone, false);
        }

        public bool Detach(GameObject instance)
        {
            return this.DetachOrDestroy(instance, false);
        }

        public void Remove(HumanBodyBones bone)
        {
            this.DetachOrDestroy(bone, true);
        }

        public void Remove(GameObject instance)
        {
            this.DetachOrDestroy(instance, true);
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private List<GameObject> DetachOrDestroy(HumanBodyBones bone, bool destroy)
        {
            List<GameObject> objects = new List<GameObject>();
            if (this.attachments.ContainsKey(bone))
            {
                objects = new List<GameObject>(this.attachments[bone]);
                this.attachments.Remove(bone);

                for (int i = 0; i < objects.Count; ++i)
                {
                    if (objects[i] != null)
                    {
                        objects[i].transform.SetParent(null);

                        if (this.onDetach != null)
                        {
                            this.onDetach.Invoke(new EventData
                            {
                                attachment = objects[i],
                                bone = bone,
                                isDestroy = destroy
                            });
                        }

                        if (destroy) Destroy(objects[i]);
                    }
                }
            }

            return objects;
        }

        private bool DetachOrDestroy(GameObject instance, bool destroy)
        {
            foreach (KeyValuePair<HumanBodyBones, List<GameObject>> item in this.attachments)
            {
                if (item.Value == null) continue;
                if (this.attachments[item.Key].Remove(instance))
                {
                    instance.transform.SetParent(null);

                    if (this.onDetach != null)
                    {
                        this.onDetach.Invoke(new EventData
                        {
                            attachment = instance,
                            bone = item.Key,
                            isDestroy = destroy
                        });
                    }

                    if (destroy) Destroy(instance);

                    if (this.attachments[item.Key].Count == 0)
                    {
                        this.attachments.Remove(item.Key);
                    }

                    return true;
                }
            }

            return false;
        }
    }
}