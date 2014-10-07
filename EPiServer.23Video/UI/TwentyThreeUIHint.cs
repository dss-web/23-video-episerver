/*This file is part of 23 Video content provider for EPiServer.
23 Video content provider for EPiServer is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
23 Video content provider for EPiServer is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.
You should have received a copy of the GNU Lesser General Public License along with 23 Video content provider for EPiServer. If not, see http://www.gnu.org/licenses/. */
using System;
using EPiCode.TwentyThreeVideo.Models;
using EPiServer.Core;
using EPiServer.Shell;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace EPiCode.TwentyThreeVideo.UI
{
    [EditorDescriptorRegistration(TargetType = typeof(ContentReference), UIHint = "twentythreevideo")]
    public class TwentyThreeVideoEditorDescriptor : ContentReferenceEditorDescriptor<Video>
    {
        public override string RepositoryKey
        {
            get { return Constants.ProviderKey; }
        }
    }

    [UIDescriptorRegistration]
    public class TwentyThreeVideoUIDescriptor : UIDescriptor<Video>
    {
        public TwentyThreeVideoUIDescriptor()
        {
            ContainerTypes = new Type[] { typeof(VideoFolder) };
        }
    }

}
