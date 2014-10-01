using EPiServer.Shell;
using EPiServer.Shell.ViewComposition;

namespace EPiCode.TwentyThreeVideo.UI
{
    [Component]
    public class TwentyThreeVideoComponent : ComponentDefinitionBase
    {
        public TwentyThreeVideoComponent()
            : base("twentythree.components.TwentyThreeVideo")
        {
            Categories = new string[] { "content" };
            Title = "23 video";
            Description = "List content from 23 Video";
            SortOrder = 1000;
            PlugInAreas = new[] { PlugInArea.AssetsDefaultGroup };
            Settings.Add(new Setting("repositoryKey", TwentyThreeVideoRepositoryDescriptor.RepositoryKey));
        }
    }
}