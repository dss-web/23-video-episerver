using EPiCode.TwentyThreeVideo.Provider;

namespace EPiCode.TwentyThreeVideo.Data
{
    public class EditorGroupChannelMappingRepository
    {
        public string GetEditorGroupForChannel(string channelName)
        {
            if (Client.Settings.EditorGroupChannelMapping.ContainsKey(channelName))
                return Client.Settings.EditorGroupChannelMapping[channelName];
            
            return string.Empty;
        }
    }
}