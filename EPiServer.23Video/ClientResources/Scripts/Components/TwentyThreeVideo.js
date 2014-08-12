define([
// dojo
    "dojo/_base/declare",
    "dojo/_base/lang",

    "dojo/dom-class",
    "dojo/dom-style",

    "dojo/on",
    "dojo/topic",
// epi-cms
    "twentythree/components/TwentyThreeVideoViewModel",
    "epi-cms/widget/HierarchicalList",
// Resources
    "epi/i18n!epi/cms/nls/episerver.cms.widget.hierachicallist",
    "epi/i18n!epi/cms/nls/twentythreevideo.media"
],

function (
// dojo
    declare,
    lang,

    domClass,
    domStyle,

    on,
    topic,
// epi
    TwentyThreeVideoiewModel,
    HierarchicalList,
// Resources
    hierarchicalListResources,
    resources
) {

    // module:
    //      epi-cms/component/Media
    // summary:
    //      Media management component
    // tags:
    //      public

    return declare("TwentyThreeVideo", [HierarchicalList], {

        res: resources,

        // enableDndFileDropZone: [public] Boolean
        //      Flag to indicate this widget allowed to show drop zone widget ("epi-cms/widget/FilesUploadDropZone" that care about native DnD files from browser) or not.
        enableDndFileDropZone: true,

        // showCreateContentArea: [public] Boolean
        //      Flag to indicate this widget allowed to show create content area by default or not.
        showCreateContentArea: true,

        modelClassName: TwentyThreeVideoiewModel,

        noDataMessage: resources.nocontent,

        // hierarchicalListClass: [readonly] String
        //      The CSS class to be used on the content list.
        hierarchicalListClass: "epi-mediaList",

        // createContentText: [public] String
        //      Upload file text
        createContentText: resources.dropareatitle,

        postCreate: function () {

            this.inherited(arguments);

            this.list.set("noDataMessage", this.res.nocontent);

            this.own(
                this.model.getCommand("upload").watch("canExecute", lang.hitch(this, function (name, oldValue, newValue) {
                    this._toggleCreateContentArea(newValue);
                }))
            );
        },

        // =======================================================================
        // List setup

        _setupList: function () {
            // summary:
            //      List setup
            // tags:
            //      protected

            this.inherited(arguments);

            domClass.add(this.list.grid.domNode, "epi-thumbnailContentList");

            //this.own(
            //    on(this.list, "createItemAction", lang.hitch(this, function () {
            //        this.model._commandRegistry.uploadDefault.command.execute();
            //    }))
            //);
        }

    });

});