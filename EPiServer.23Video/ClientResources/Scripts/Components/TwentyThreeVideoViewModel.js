define( [
// dojo
    "dojo/_base/array",
    "dojo/_base/declare",
    "dojo/_base/lang",

    "dojo/dom-class",

    "dojo/when",
// epi
    "epi",
    "epi/shell/widget/dialog/Dialog",

    "epi-cms/core/ContentReference",

    "epi-cms/widget/ContextualContentForestStoreModel",
    "epi-cms/widget/viewmodel/HierarchicalListViewModel",
    "epi-cms/widget/viewmodel/MultipleFileUploadViewModel",
    "epi-cms/widget/MultipleFileUpload",
    "epi-cms/widget/UploadUtil",

    "epi-cms/command/UploadContent",
    "epi-cms/command/EditImage",
    "epi-cms/command/DownloadMedia",
// resource
    "epi/i18n!epi/cms/nls/twentythreevideo.media"
],

function (
// dojo
    array,
    declare,
    lang,

    domClass,

    when,
// epi
    epi,
    Dialog,

    ContentReference,

    ContextualContentForestStoreModel,
    HierarchicalListViewModel,
    MultipleFileUploadViewModel,
    MultipleFileUpload,
    UploadUtil,

    UploadContentCommand,
    EditImageCommand,
    DownloadCommand,
// resources
    resources
) {

    // module:
    //      epi-cms/component/viewmodel/MediaViewModel
    // summary:
    //      Handles search and tree to list browsing widgets.
    // tags:
    //      public

    return declare("TwentyThreeVideoViewModel", [HierarchicalListViewModel], {

        // treeStoreModelClass: [const] Function
        //      Class to use as model for the tree.

        // commented out to remove "for this page"
       // treeStoreModelClass: ContextualContentForestStoreModel,

        // Dialog widget for uploading new media
        _dialog: null,

        _getTypesToCreate: function () {
            // No create commands for media since upload is used instead.
            return [];
        },

        _setupCommands: function () {
            // summary:
            //      Creates and registers the commands used.
            // tags:
            //      protected

            this.inherited(arguments);

            var settings = {
                selection: this.selection,
                model: this
            };

            var customCommands = {
                uploadDefault: {
                    command: new UploadContentCommand(lang.mixin({
                        iconClass: "epi-iconPlus",
                        label: resources.command.label,
                        resources: resources,
                        viewModel: this
                    }, settings))
                },
                upload: {
                    command: new UploadContentCommand(lang.mixin({
                        category: "context",
                        iconClass: "epi-iconUpload",
                        label: resources.linktocreateitem,
                        viewModel: this
                    }, settings)),
                    isAvailable: this.menuType.ROOT | this.menuType.TREE,
                    order: 2
                },
               
            };

            this._commandRegistry = lang.mixin(this._commandRegistry, customCommands);

            this.pseudoContextualCommands.push(this._commandRegistry.uploadDefault.command);
            this.pseudoContextualCommands.push(this._commandRegistry.upload.command);
        },

        _updateTreeContextCommandModels: function (model) {
            // summary:
            //      Update model of commands in case selected content is folder
            // tags:
            //      private

            this.inherited(arguments);

            this._commandRegistry.uploadDefault.command.set("model", model);
            this._commandRegistry.upload.command.set("model", model);
        },

        upload: function (/*Array*/fileList, /*String?*/targetId, /*Boolean?*/createAsLocalAsset) {
            // summary:
            //      Upload multiple files.
            // fileList: [Array]
            //      List files to upload.
            //      When null, only show upload form to select files for uploading.
            //      Otherwise, upload files in list.
            // targetId: [String?]
            //      Parent content id
            // createAsLocalAsset: [Boolean?]
            // tags:
            //      protected

            // only create diaglog if it is not available, otherwise, re-use it.
            var uploader = new MultipleFileUpload({
                model: new MultipleFileUploadViewModel({
                    store: this.get("store"),
                    query: this.get("listQuery")
                })
            });

            uploader.on("beforeUploaderChange", lang.hitch(this, function () {
                this._uploading = true;
            }));

            // close multiple files upload dialog when stop uploading
            uploader.on("close", lang.hitch(this, function (uploading) {
                this._dialog && (uploading ? this._dialog.hide() : this._dialog.destroy());
            }));

            // Reload current folder of tree, to reflect changes
            uploader.on("uploadComplete", lang.hitch(this, function (/*Array*/uploadFiles) {
                // Set current tree item again to reload items in list.
                if (uploader.createAsLocalAsset) {
                    when(this.treeStoreModel && typeof this.treeStoreModel.refreshRoots === "function" && this.treeStoreModel.refreshRoots(this), lang.hitch(this, function () {
                        // Turn-off createAsLocalAsset
                        uploader.set("createAsLocalAsset", false);
                        // Update uploading directory after create a new real one local asset folder for the given content
                        uploader.set("uploadDirectory", this.get("currentTreeItem").id);
                        // Update content list query after create a new real one local asset folder for the given content
                        uploader.model.set("query", this.get("listQuery"));
                    }));
                } else {
                    this.onListItemUpdated(uploadFiles);
                    this.set("currentTreeItem", this.get("currentTreeItem"));
                }

                if (this._dialog && !this._dialog.open) {
                    this._dialog.destroy();
                }

                this._uploading = false;
            }));

            this._dialog = new Dialog({
                title: resources.linktocreateitem,
                content: uploader,
                autofocus: UploadUtil.supportNativeDndFiles(), // Only autofocus if not using flash.
                defaultActionsVisible: false,
                closeIconVisible: false
            });

            domClass.add(this._dialog.domNode, "epi-multiFileUploadDialog");

            // only show close button for multiple files upload dialog
            this._dialog.definitionConsumer.add({
                name: "close",
                label: epi.resources.action.close,
                action: function () {
                    uploader.close();
                }
            });

            this._dialog.resize({ w: 700 });
            this._dialog.show();

            var selectedContent = createAsLocalAsset ? this.selection.data[0].data : this.store.get(targetId);
            when(selectedContent, lang.hitch(this, function (content) {
                // Update breadcumb on upload dialog.
                this._buildBreadcrumb(content, uploader);

                // Set destination is current tree item.
                uploader.set("uploadDirectory", targetId || this.get("currentTreeItem").id);
                uploader.set("createAsLocalAsset", createAsLocalAsset);

                uploader.upload(fileList);
            }));
        },

        onListItemUpdated: function (updatedItems) {
            // summary:
            //      Refresh the editing media if it have a new version
            // updatedItems: [Array]
            //      Collection of the updated item. In this case, they are files.
            // tags:
            //      public, extension

            var store = this.store;

            return when(this.getCurrentContext(), function (currentContext) {
                var contentWithoutVersion = (new ContentReference(currentContext.id)).createVersionUnspecificReference().toString();

                return when(store.get(contentWithoutVersion), function (currentContent) {
                    var editingMedia = array.filter(updatedItems, function (updatedItem) {
                        return currentContent.name.toLowerCase() === updatedItem.fileName.toLowerCase();
                    })[0];
                    return editingMedia ? currentContent : null;
                });
            });
        },

        _buildBreadcrumb: function (contentItem, uploader) {
            // summary:
            //      Build breadcrumb for the provided content
            // contentItem: Object
            //      The provided content
            // uploader: Object
            //      The multiple file upload control
            // tags:
            //      private

            if (!uploader) {
                return;
            }


            // Do not add more items when current content is sub root
            if (this.treeStoreModel.isTypeOfRoot(contentItem)) {
                uploader.set("breadcrumb", [contentItem]);
                return;
            }

            this.treeStoreModel.getAncestors(contentItem.contentLink, lang.hitch(this, function (ancestors) {
                var ancestor,
                    paths = [contentItem];

                for (var i = ancestors.length - 1; i >= 0; i--) {
                    ancestor = ancestors[i];
                    paths.unshift(ancestor);

                    // Break after first sub root or context root
                    if (this.treeStoreModel.isTypeOfRoot(ancestor)) {
                        break;
                    }
                }

                uploader.set("breadcrumb", paths);
            }));
        }

    });

});