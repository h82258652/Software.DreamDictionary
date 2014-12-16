using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.SpeechRecognition;
using Windows.Storage;

namespace 周公解梦
{
    public class VoiceCommandHelper
    {
#if WINDOWS_PHONE_APP
        public static  Windows.ApplicationModel.Activation.VoiceCommandActivatedEventArgs StartVoiceCommandArgs = null;
#endif

        public static async Task InstallVoiceCommandIfFirstLaunch()
        {
#if WINDOWS_PHONE_APP
            if (VoiceCommandManager.InstalledCommandSets.Count <= 0)
            {
                StorageFile storageFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///VoiceCommand.xml"));
                await Windows.Media.SpeechRecognition.VoiceCommandManager.InstallCommandSetsFromStorageFileAsync(storageFile);
            }
#endif
        }
    }
}
