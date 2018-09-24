using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class GestureAnimation
{
    public const float TRANSITION = 0.25f;

    // PROPERTIES: --------------------------------------------------------------------------------

    protected PlayableGraph graph;
    protected AnimationMixerPlayable mixer;
    protected AnimationClipPlayable gesturePlayable;

    private bool gesturePlaying = false;
    private float gestureTransition = TRANSITION;
    private float gestureDuration = 0.0f;

    // INITIALIZERS: ------------------------------------------------------------------------------

    public Playable Setup(PlayableGraph graph, Playable input)
    {
        this.graph = graph;

        this.mixer = AnimationMixerPlayable.Create(this.graph, 2, true);
        this.mixer.ConnectInput(0, input, 0);
        this.mixer.ConnectInput(1, Playable.Null, 0);

        mixer.SetInputWeight(0, 1.0f);
        mixer.SetInputWeight(1, 0.0f);

        return this.mixer;
    }

    // PUBLIC METHODS: ----------------------------------------------------------------------------

    public virtual void PlayGesture(AnimationClip clip, float transition = TRANSITION)
    {
        if (clip == null) return;

        this.gesturePlayable = AnimationClipPlayable.Create(this.graph, clip);
        this.gesturePlayable.SetTime(0f);
        this.gesturePlayable.SetDuration(clip.length);

        this.graph.Disconnect(this.mixer, 1);
        this.graph.Connect(this.gesturePlayable, 0, this.mixer, 1);

        this.mixer.SetInputWeight(0, 1.0f);
        this.mixer.SetInputWeight(1, 0.0f);

        this.gesturePlaying = true;
        this.gestureDuration = clip.length;
        this.gestureTransition = transition;
    }

    public void StopGesture()
    {
        this.mixer.SetInputWeight(0, 1.0f);
        this.graph.Disconnect(this.mixer, 1);
        this.gesturePlaying = false;
    }

    public void Update()
    {
        if (this.gesturePlaying)
        {
            if (this.gesturePlayable.IsDone())
            {
                this.StopGesture();
                return;
            }
            else
            {
                float time = (float)this.gesturePlayable.GetTime();
                if (time + this.gestureTransition >= this.gestureDuration)
                {
                    float t = (this.gestureDuration - time) / this.gestureTransition;

                    t = Mathf.Clamp01(t);
                    mixer.SetInputWeight(0, 1.0f - t);
                    mixer.SetInputWeight(1, t);
                }
                else if (time <= this.gestureTransition)
                {
                    float t = time / this.gestureTransition;

                    t = Mathf.Clamp01(t);
                    mixer.SetInputWeight(0, 1.0f - t);
                    mixer.SetInputWeight(1, t);
                }
                else
                {
                    mixer.SetInputWeight(0, 0.0f);
                    mixer.SetInputWeight(1, 1.0f);
                }
            }
        }
    }
}