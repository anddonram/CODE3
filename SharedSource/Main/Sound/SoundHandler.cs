using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.Sound;

namespace Codigo.Sound
{
    /**
     * <summary>
     * This class handles all sounds loading. Sounds are played in AnimationBehavior
     * </summary>
     */
    class SoundHandler : Component
    {
        /**<summary>
         * All sounds' ids belonging to each action
         * </summary>
         */
        public Dictionary<ActionEnum, List<int>> sounds { get; private set; }

        /**
         * <summary>
         * The sound bank
         * </summary>
         */
        public SoundBank soundBank { get; private set; }

        protected override void Initialize()
        {
            base.Initialize();
            soundBank = new SoundBank(Assets);
            sounds = new Dictionary<ActionEnum, List<int>>();
            WaveServices.SoundPlayer.StopAllSounds();
            // Sound.wav was added through Wave Visual Editor
            //Add here the new sounds
            AddNewSound(ActionEnum.Chop, WaveContent.Assets.Sounds.Chopping_wav);
            AddNewSound(ActionEnum.Build, WaveContent.Assets.Sounds.Build_wav);
            AddNewSound(ActionEnum.Fight, WaveContent.Assets.Sounds.Fighting_wav);
            AddNewSound(ActionEnum.Train, WaveContent.Assets.Sounds.Fighting_wav);
            AddNewSound(ActionEnum.Heal, WaveContent.Assets.Sounds.Healing_wav);
            AddNewSound(ActionEnum.Enter, WaveContent.Assets.Sounds.Enter_wav);
            AddNewSound(ActionEnum.Exit, WaveContent.Assets.Sounds.Exit_wav);
            AddNewSound(ActionEnum.Move, WaveContent.Assets.Sounds.Move1_wav);
            AddNewSound(ActionEnum.Move, WaveContent.Assets.Sounds.Move2_wav);
            

            //Finally, register the bank
            WaveServices.SoundPlayer.RegisterSoundBank(soundBank);
        }
        /**
         * <summary>
         * Adds a new sound for a specific action to the bank
         * </summary>
         */
        private void AddNewSound(ActionEnum key, string value)
        {

            int id = AddSoundToBank(soundBank, value);

            if (!sounds.ContainsKey(key))
            {
                sounds.Add(key, new List<int>());
            }
            sounds[key].Add(id);
        }
        /**
         * <summary>
         * Adds a new sound to the bank and returns the assigned id for that song
         * </summary>
         */
        private int AddSoundToBank(SoundBank soundBank, string s)
        {
            var sound = new SoundInfo(s);
            soundBank.Add(sound);
            return sound.SoundId;
        }
    }
}
