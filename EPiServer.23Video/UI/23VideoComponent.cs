using EPiServer.Shell;
using EPiServer.Shell.ViewComposition;

namespace EPiServer._23Video.UI
{
    /// <summary>
    /// Component that provides a YouTube integration.
    /// </summary>
    [Component]
    public class YouTubeComponent : ComponentDefinitionBase
    {
        public YouTubeComponent()
            : base("epi-cms.component.Media")
        {
            Categories = new string[] { "content" };
            Title = "23 video";
            Description = "List content from 23 Video";
            SortOrder = 1000;
            PlugInAreas = new[] { PlugInArea.AssetsDefaultGroup };
            Settings.Add(new Setting("repositoryKey", _23VideoRepositoryDescriptor.RepositoryKey));
        }
    }
}