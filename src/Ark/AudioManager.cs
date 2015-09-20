#region Using Statements
using Microsoft.Xna.Framework.Audio;
#endregion

namespace Ark
{
    static class AudioManager
    {
        /// <summary>
        /// Creates a new SoundEffectInstance for a SoundEffect.
        /// </summary>
        /// <param name="effect">Provides a loaded sound resource.</param>
        /// <param name="instance">Provides a single playing, paused, or stopped instance of a sound effect.</param>
        /// <returns></returns>
        public static SoundEffectInstance CreateInstance(SoundEffect effect, SoundEffectInstance instance)
        {
            return instance = effect.CreateInstance();
        }

        /// <summary>
        /// Plays a non-looping SoundEffect.
        /// </summary>
        /// <param name="effect">Provides a loaded sound resource.</param>
        public static void PlaySoundEffect(SoundEffect effect)
        {
            effect.Play();
        }

        /// <summary>
        /// Plays a non-looping SoundEffectInstance.
        /// </summary>
        /// <param name="instance">Provides a single playing, paused, or stopped instance of a sound effect.</param>
        public static void PlaySoundEffect(SoundEffectInstance instance)
        {
            instance.Play();
        }

        /// <summary>
        /// Plays a non-looing SoundEffect whos volume, pitch and pan can be manipulated.
        /// </summary>
        /// <param name="effect">Provides a single playing, paused, or stopped instance of a sound effect.</param>
        /// <param name="volume">Used to increase or decrease the volume of a sound effect between 0 and 1.</param>
        /// <param name="pitch">Used to increase the pitch of a sound effect, a value of -1 decreases the pitch by an octave 
        /// and 1 increase the pitch by an octave. The default value of a sound effect is 0.</param>
        /// <param name="pan">Used to determine which speaker audio should play from when using a stereo system. -1 plays audio
        /// on the left speaker only, 1 on the right speaker and 0 on both.</param>
        public static void PlaySoundEffect(SoundEffect effect, float volume, float pitch,
            float pan)
        {
            effect.Play(volume, pitch, pan);
        }

        /// <summary>
        /// Plays a non-looing SoundEffectInstance whos volume, pitch and pan can be manipulated.
        /// </summary>
        /// <param name="instance">Provides a single playing, paused, or stopped instance of a sound effect.</param>
        /// <param name="volume">Used to increase or decrease the volume of a sound effect between 0 and 1.</param>
        /// <param name="pitch">Used to increase the pitch of a sound effect, a value of -1 decreases the pitch by an octave 
        /// and 1 increase the pitch by an octave. The default value of a sound effect is 0.</param>
        /// <param name="pan">Used to determine which speaker audio should play from when using a stereo system. -1 plays audio
        /// on the left speaker only, 1 on the right speaker and 0 on both.</param>
        public static void PlaySoundEffect(SoundEffectInstance instance, float volume, float pitch,
            float pan)
        {
            instance.Volume = volume;
            instance.Pitch = pitch;
            instance.Pan = pan;
            instance.Play();
        }

        /// <summary>
        /// Plays a SoundEffectInstance which can be looped and whos volume, pitch and pan can be manipulated.
        /// </summary>
        /// <param name="instance">Provides a single playing, paused, or stopped instance of a sound effect.</param>
        /// <param name="volume">Used to increase or decrease the volume of a sound effect between 0 and 1.</param>
        /// <param name="pitch">Used to increase the pitch of a sound effect, a value of -1 decreases the pitch by an octave 
        /// and 1 increase the pitch by an octave. The default value of a sound effect is 0.</param>
        /// <param name="pan">Used to determine which speaker audio should play from when using a stereo system. -1 plays audio
        /// on the left speaker only, 1 on the right speaker and 0 on both.</param>
        /// <param name="isLooped"></param>
        public static void PlaySoundEffect(SoundEffectInstance instance, float volume, float pitch,
            float pan, bool isLooped)
        {
            instance.Volume = volume;
            instance.Pitch = pitch;
            instance.Pan = pan;
            instance.IsLooped = isLooped;
            instance.Play();
        }

        /// <summary>
        /// Pauses a SoundEffectInstance. 
        /// </summary>
        /// <param name="instance">Provides a single playing, paused, or stopped instance of a sound effect.</param>
        public static void PauseSoundEffect(SoundEffectInstance instance)
        {
            instance.Pause();
        }

        /// <summary>
        /// Resumes playback for a SoundEffectInstance. 
        /// </summary>
        /// <param name="instance">Provides a single playing, paused, or stopped instance of a sound effect.</param>
        public static void ResumeSoundEffect(SoundEffectInstance instance)
        {
            instance.Resume();
        }

        /// <summary>
        /// Stops playing a SoundEffectInstance. 
        /// </summary>
        /// <param name="instance">Provides a single playing, paused, or stopped instance of a sound effect.</param>
        public static void StopSoundEffect(SoundEffectInstance instance)
        {
            instance.Stop();
        }

        /// <summary>
        /// Gets the current state (playing, paused, or stopped) of the SoundEffectInstance. 
        /// </summary>
        /// <param name="instance">Provides a single playing, paused, or stopped instance of a sound effect.</param>
        /// <returns></returns>
        public static SoundState CheckSoundState(SoundEffectInstance instance)
        {
            return instance.State;
        }
    }
}