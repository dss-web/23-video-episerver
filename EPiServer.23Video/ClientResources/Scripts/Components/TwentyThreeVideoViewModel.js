/*This file is part of 23 Video content provider for EPiServer.
23 Video content provider for EPiServer is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
23 Video content provider for EPiServer is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.
You should have received a copy of the GNU Lesser General Public License along with 23 Video content provider for EPiServer. If not, see http://www.gnu.org/licenses/. */

define([
// dojo
    "dojo/_base/array",
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/topic",
        "dojo/dom-class",
    "dojo/Stateful",
    "dojo/when",
//epi
    "epi",
    "epi/dependency",

    "epi/shell/ClipboardManager",
    "epi/shell/command/_CommandProviderMixin",
    "epi/shell/selection",
    "epi/shell/TypeDescriptorManager",
//epi-cms
    "epi-cms/_ContentContextMixin",
    "epi-cms/_MultilingualMixin",
    "epi-cms/contentediting/_ContextualContentContextMixin",
    "epi-cms/core/ContentReference",
    "epi-cms/widget/ContentForestStoreModel",
        "epi-cms/widget/viewmodel/MultipleFileUploadViewModel",
    "epi-cms/widget/MultipleFileUpload",
    "epi-cms/widget/UploadUtil",
        "epi/shell/widget/dialog/Dialog",
//command
    "epi-cms/asset/command/NewFolder",
    "epi-cms/asset/command/RenameSelectedFolder",
    "epi-cms/widget/CreateCommandsMixin",
    "epi-cms/command/NewContent",
    "epi-cms/command/CopyContent",
    "epi-cms/command/CutContent",
    "epi-cms/command/DeleteContent",
    "epi-cms/command/PasteContent",
    "epi-cms/command/TranslateContent",
    "epi-cms/component/command/ViewTrash",
    "epi-cms/component/command/ChangeContext",
    "epi-cms/command/UploadContent",

        // resource
    "epi/i18n!epi/cms/nls/twentythreevideo.media"
],

function (
// dojo
    array,
    declare,
    lang,
    topic,
        domClass,
    Stateful,
    when,
// epi
    epi,
    dependency,

    ClipboardManager,
    _CommandProviderMixin,
    Selection,
    TypeDescriptorManager,

//epi-cms
    _ContentContextMixin,
    _MultilingualMixin,
    _ContextualContentContextMixin,
    ContentReference,
    ContentForestStoreModel,
       MultipleFileUploadViewModel,
    MultipleFileUpload,
    UploadUtil,
    Dialog,
//command
    NewFolderCommand,
    RenameSelectedFolderCommand,
    CreateCommandsMixin,
    NewContentCommand,
    CopyContentCommand,
    CutContentCommand,
    DeleteContentCommand,
    PasteContentCommand,
    TranslateContentCommand,
    ViewTrashCommand,
    ChangeContextCommand,
    UploadContentCommand,

    // resources
    resources
) {

    // module:
    //      epi-cms/widget/viewmodel/HierarchicalListViewModel
    // summary:
    //      Handles search and tree to list browsing widgets.

    return declare("TwentyThreeVideoViewModel", [Stateful, _ContentContextMixin, _ContextualContentContextMixin, _MultilingualMixin, CreateCommandsMixin], {

        // menuType: [readonly] Object
        //      Enum used with commands to set in what context they should be available.
        menuType: { ROOT: 1, TREE: 2, LIST: 4 },

        // searchArea: [readonly] String
        //      Used with the search component when querying to scope the search.
        searchArea: "",
        // searchRoots: [readonly] String
        //      Used with the search component to set the roots to search in.
        searchRoots: "",

        // clipboardManager: [const] ClipboardManager
        //      Used to handle copy-paste operations with the commands.
        clipboardManager: null,

        // selection: [const] Selection
        //      Used to handle currently selected items for the commands.
        selection: null,

        // commands: [readonly] _Command[]
        //      Used to handle currently selected items for the commands.
        commands: null,

        // createCommands: [readonly] _Command[]
        //      Used to handle currently selected items for the commands.
        createCommands: null,

        // createHierarchyCommands: [readonly] _Command[]
        //      Used to handle currently selected items for the commands.
        createHierarchyCommands: null,

        // pseudoContextualCommands: [readonly] _Command[]
        //      Used to handle currently selected items for the commands.
        pseudoContextualCommands: null,

        // currentTreeItem:  ContentReference
        //      The currently selected tree item.
        currentTreeItem: null,

        // currentListItem:  ContentReference
        //      The currently selected list item.
        currentListItem: null,

        // listQuery: Query
        //      Query object holding parameters to get the children.
        //      of the current tree item
        listQuery: null,

        // listQueryOptions: Object
        //      Extra paramaters needed to query the store for the list items.
        listQueryOptions: null,

        // showAllLanguages: Boolean
        //      Indicates if to query for items only in current language context or not.
        showAllLanguages: true,

        // treeStoreModelClass: [const] Function
        //      Class to use as model for the tree.
        treeStoreModelClass: ContentForestStoreModel,

        // treeStoreModel: [const] TreeStoreModel
        //      TreeStoreModel instance.
        treeStoreModel: null,

        // store: [const] Dojo/Store
        //      Store instance used for all server queries.
        store: null,

        // storeKey: [const] String
        //      Key to resolve store from dependency.
        storeKey: "epi.cms.content.light",

        // mainNavigationTypes: String[]
        //      Which types to filter for tree queries. Also used with trash command.
        mainNavigationTypes: null,

        // containedTypes: [const] String[]
        //       Which types to filter for list queries.
        containedTypes: null,

        _showAllLanguagesSetter: function (value) {
            this.showAllLanguages = value;
            this._updateListQuery(this.currentTreeItem);
            this.treeStoreModel.set("showAllLanguages", value);
        },

        _currentTreeItemSetter: function (value) {
            this.currentTreeItem = value;
            this._updateListQuery(value);
        },

        postscript: function (args) {
            this.inherited(arguments);
            this.contentRepositoryDescriptors = this.contentRepositoryDescriptors || dependency.resolve("epi.cms.contentRepositoryDescriptors");

            declare.safeMixin(this, this.contentRepositoryDescriptors[args.repositoryKey]);

            this.clipboardManager = this.clipboardManager || new ClipboardManager();
            this.selection = this.selection || new Selection();
            this.store = this.store || dependency.resolve("epi.storeregistry").get(this.storeKey);

            this._setupTreeStoreModel();

            this._setupCommands();
            this.set("commands", this._commandRegistry.toArray());

            this._setupSearchRoots();

            this.set("listQueryOptions", this.treeStoreModel._queryOptions);
        },

        startup: function () {
            // summary:
            //      Allows the view model to start reacting to external input.
            // tags:
            //      protected

            this.inherited(arguments);

            this._setupSelection();
        },

        getCommand: function (commandName) {
            // summary:
            //      Gets a command by command name
            // tags:
            //      protected

            return this._commandRegistry[commandName] ? this._commandRegistry[commandName].command : null;
        },

        contentContextChanged: function (context, callerData) {
            // summary:
            //      Called when the currently loaded content changes. I.e. a new content data object is loaded into the preview area.
            //      Override _ContextContextMixin.contentContextChanged
            // tags:
            //      protected

            this._setupSearchRoots();

            if (!this._isSupportedContent(context)) {
                return;
            }

            var self = this,
                oldListRef = self.get("currentListItem"),
                previousSelection = self.treeStoreModel.get("previousSelection"),
                listRef = ContentReference.toContentReference(context.id);

            when(self.store.get(listRef.createVersionUnspecificReference()), function (currentContent) {
                var contextParentLink = previousSelection && self.hasContextual(previousSelection.selectedAncestors) && currentContent.assetsFolderLink != null ? currentContent.assetsFolderLink : context.parentLink,
                    treeRef = ContentReference.toContentReference(previousSelection ? contextParentLink : self.roots[0]);

                // Just want to store the current selected content when the context changed.
                // So that, do not need to wait any action from the store.
                self.set("editingListItem", listRef);

                if (callerData && callerData.forceReload || !ContentReference.compareIgnoreVersion(oldListRef, listRef)) {
                    when(self.store.get(treeRef.createVersionUnspecificReference()), function (model) {
                        self.treeStoreModel && when(self.treeStoreModel.canExpandTo(model), function (canExpand) {
                            if (canExpand) {
                                self.set("currentTreeItem", treeRef);
                                self.set("currentListItem", listRef);

                                self._updateCommands(model, self.menuType.LIST);
                            }
                        });
                    });
                }
            });
        },

        onSearch: function (metadata) {
            // summary:
            //      Handles changes as a result of a user search
            // metadata:
            //      Seatch result
            // tags:
            //      public

            when(this.store.get(metadata.id), lang.hitch(this, function (model) {
                var currentListItem = ContentReference.toContentReference(model.contentLink);
                this.set("editingListItem", currentListItem);
                this.set("currentListItem", currentListItem);
                this.set("currentTreeItem", ContentReference.toContentReference(model.parentLink));
                this._updateCommands(model, this.menuType.LIST);
            }));
        },

        onTreeItemSelected: function (model, isRoot) {
            // summary:
            //      Handles changes to the slection in the tree and updates the viev model
            // tags:
            //      public

            var oldRef = this.get("currentTreeItem"),
                newRef = ContentReference.toContentReference(model.contentLink),
                menuType = isRoot ? this.menuType.ROOT : this.menuType.TREE;

            this._updateCommands(model, menuType);

            if (!ContentReference.compareIgnoreVersion(oldRef, newRef) && ContentReference.emptyContentReference != newRef) {
                this.set("currentTreeItem", newRef);
            }
        },

        onListItemSelected: function (model) {
            // summary:
            //      Handles changes to the slection in the list and updates the viev model
            // tags:
            //      public

            var oldRef = this.get("currentListItem"),
                newRef = ContentReference.toContentReference(model.contentLink);

            this._updateCommands(model, this.menuType.LIST);

            if (!ContentReference.compareIgnoreVersion(oldRef, newRef)) {
                this.set("currentListItem", newRef);
            }
        },

        //onListItemUpdated: function (updatedItems) {
        //    // summary:
        //    //      Refresh the editing media if it have a new version
        //    // updatedItems: [Array]
        //    //      Collection of the updated item
        //    // tags:
        //    //      public, extension
        //},

        _isSupportedContent: function (/*Object*/content) {
            // summary:
            //      Indicates whether the given content is a type contained by this widget.
            // content:
            //      Object to validate
            // tags:
            //      private
            return !!(content && content.id) && this.containedTypes.some(function (type) {
                return TypeDescriptorManager.isBaseTypeIdentifier(content.dataType, type);
            });
        },

        _setupSelection: function () {
            // summary:
            //      Get target tree item and list item for selection
            // tags:
            //      protected

            when(this.getCurrentContext(), lang.hitch(this, function (ctx) {

                var rootId;

                if (!this._isContentContext(ctx) || !this._isSupportedContent(ctx)) {
                    // TODO: Fix this hack of always using the first item in roots
                    rootId = ContentReference.toContentReference(this.roots[0]).toString();
                    when(this.store.get(rootId), lang.hitch(this, function (model) {
                        this.onTreeItemSelected(model, true);
                    }));
                } else {
                    this.contentContextChanged(ctx, null);
                }
            }));
        },

        _setupTreeStoreModel: function () {
            // summary:
            //      Creates an configures the treeStoreModel.
            // tags:
            //      protected

            var treeModel = new this.treeStoreModelClass({
                store: this.store,
                roots: this.roots,
                typeIdentifiers: this.mainNavigationTypes,
                containedTypes: this.containedTypes,
                notAllowToCopy: this.preventCopyingFor,
                notAllowToDelete: this.preventDeletionFor,
                notSupportContextualContents: this.preventContextualContentFor,
                onAddDelegate: lang.hitch(this, function (node) {
                    var targetNode = dijit.getEnclosingWidget(node.domNode),
                        target = targetNode && targetNode.item,
                        canExecute = (typeof (this.treeStoreModel.canEdit) === "function") && this.treeStoreModel.canEdit(target);

                    if (canExecute) {
                        node.edit();


                        // Publish the children change AFTER the user has change the name of the new folder or canceled the editor
                        // REMARK: We need to do this after the change otherwise the editing will be canceled because the children change replaced the child items in the tree
                        // TODO: This should be handled by the tree model
                        var publish = function () {
                            topic.publish("/epi/cms/contentdata/childrenchanged", target.parentLink);
                        };

                        var handle = node.on("rename", function () {
                            handle.remove();

                            publish();
                        });

                        var cancelHandle = node.on("cancelEdit", function () {
                            cancelHandle.remove();

                            publish();
                        });

                    }
                }),
                onRefreshRoots: lang.hitch(this, this._setupSearchRoots),
                additionalQueryOptions: {
                    sort: this._getSortSettings()
                }
            });

            this.set("treeStoreModel", treeModel);
        },

        _getSortSettings: function () {
            // summary:
            //      Returns the list of sort criteria.
            // tags:
            //      protected

            return [{ attribute: "name", descending: false }];
        },

        _setupSearchRoots: function () {
            // summary:
            //      Creates and configures the treeStoreModel.
            // tags:
            //      protected

            when(this.getCurrentContent(), lang.hitch(this, function (currentContentItem) {
                var roots = this.roots instanceof Array && this.roots.length > 0 ? lang.clone(this.roots) : [],
                    assetsFolderLink = currentContentItem && currentContentItem.assetsFolderLink;

                if (assetsFolderLink && typeof this._getPseudoContextualContent === "function" && assetsFolderLink != this._getPseudoContextualContent()) {
                    roots.push(assetsFolderLink);
                }

                this.set("searchRoots", roots.join(","));
            }));
        },

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

            var menuType = this.menuType;

            var settings = {
                category: "context",
                model: this.treeStoreModel,
                selection: this.selection,
                clipboard: this.clipboardManager
            };

            this.createHierarchyCommands = {};
            var index = 1;



            // this.createCommands = this.getCreateCommands(index);

            var commands = {
                rename: {
                    command: new RenameSelectedFolderCommand({ category: "context" }),
                    isAvailable: menuType.TREE,
                    order: 10
                },


                edit: {
                    command: new ChangeContextCommand({
                        category: "context",
                        forceContextChange: true
                    }),
                    isAvailable: this.menuType.LIST,
                    order: 3
                },
                translate: {
                    command: new TranslateContentCommand(),
                    isAvailable: this.menuType.TREE | this.menuType.LIST
                },

                uploadDefault: {
                    command: new UploadContentCommand(lang.mixin({
                        iconClass: "epi-iconUpload",
                        label: resources.linktocreateitem,
                        viewModel: this
                    }, settings)),
                    isAvailable: this.menuType.ROOT | this.menuType.TREE,
                },
                //upload: {
                //    command: new UploadContentCommand(lang.mixin({
                //        category: "context",
                //        iconClass: "epi-iconUpload",
                //        label: resources.linktocreateitem,
                //        viewModel: this
                //    }, settings)),
                //    isAvailable: this.menuType.ROOT | this.menuType.TREE,
                //    order: 2
                //},
                trash: {
                    command: new ViewTrashCommand({ typeIdentifiers: this.mainNavigationTypes }),
                    order: 60
                },
                sort: function () {
                    var commands = [];
                    for (var key in this) {
                        if (key !== "toArray" && key !== "sort" && this.hasOwnProperty(key)) {
                            var index = this[key].order;
                            if (!index) {
                                index = 100;
                            }
                            commands.push([index, this[key].command]);
                        }
                    }

                    commands.sort(function (a, b) {
                        return a[0] - b[0];
                    });

                    return commands;
                },
                toArray: function () {
                    var sortedCommand = this.sort();
                    var commands = [];
                    array.forEach(sortedCommand, function (key) {
                        commands.push(key[1]);
                    });

                    return commands;
                }
            };

            this._commandRegistry = lang.mixin(this._commandRegistry, this.createHierarchyCommands, this.createCommands, commands);

            this.pseudoContextualCommands = this._getPseudoContextualCommands();

            this.pseudoContextualCommands.push(this._commandRegistry.uploadDefault.command);
            //   this.pseudoContextualCommands.push(this._commandRegistry.upload.command);
        },

        _updateListQuery: function (itemRef) {
            // summary:
            //      Creates a new query and updates the listQuery property.
            // tags:
            //      protected

            var id,
                query = null;

            // remove all mainNavigationTypes from containedTypes, to avoid of displaying Folder in Content List
            var contentTypes = array.filter(this.containedTypes, function (item) {
                return this.mainNavigationTypes.indexOf(item) < 0;
            }, this);

            if (itemRef) {
                id = itemRef.toString(),
                query = this._createListChildrenQuery(id, this.showAllLanguages, contentTypes);
            }

            this.set("listQuery", query);
        },

        _createListChildrenQuery: function (id, showAllLanguages, contentTypes) {
            return { referenceId: id, query: "getchildren", allLanguages: showAllLanguages, typeIdentifiers: contentTypes };
        },

        _updateSelection: function (model) {
            // summary:
            //      Updates the selection manager to the current model. Used when commands execute.
            // tags:
            //      protected

            this.selection.set("data", model ? [{ type: "epi.cms.contentdata", data: model }] : []);
        },

        _updateCommands: function (model, menuType) {
            // summary:
            //      Updates the current model for all commands needing this.
            // tags:
            //      protected

            this._updateSelection(model);

            if (typeof this.treeStoreModel.isSupportedType === "function" && this.treeStoreModel.isSupportedType(model.typeIdentifier)) {
                this._updateTreeContextCommandModels(model);
                this.decoratePseudoContextualCommands(this.pseudoContextualCommands);
            }

            this._commandRegistry.translate.command.set("model", model);
            this._commandRegistry.translate.command.set("executeDelegate", null);
            this._commandRegistry.edit.command.set("model", model);

            // Set custom availability last in case the command has
            // default logic for changing this in the model change handling
            this._updateCommandAvailability(menuType);
        },

        //_updateTreeContextCommandModels: function (model) {
        //    // summary:
        //    //      Update model of commands in case selected content is a navigation node.
        //    // tags:
        //    //      private

        //    this._updateCreateCommandModels(model);

        //    this._commandRegistry.rename.command.set("model", model);


        //    this._commandRegistry.uploadDefault.command.set("model", model);
        //    this._commandRegistry.upload.command.set("model", model);
        //},

        _updateCreateCommandModels: function (model) {
            // summary:
            //      Update model of create commands.
            // tags:
            //      protected

            var commands = this.createCommands;
            for (var key in commands) {
                commands[key].command.set("model", model);
            }
        },

        _getPseudoContextualCommands: function () {
            // summary:
            //      Get commands to decorates
            // returns: [Array]
            //      Array of command object that each is instance of "epi.shell.command._Command" class
            // tags:
            //      private

            var key,
                commands = [];

            for (key in this.createCommands) {
                commands.push(this.createCommands[key].command);
            }

            for (key in this.createHierarchyCommands) {
                commands.push(this.createHierarchyCommands[key].command);
            }

            // commands.push(this._commandRegistry.paste.command);

            return commands;
        },

        _updateCommandAvailability: function (menuType) {
            // summary:
            //      Updates the availability of the command dependant on which menu types it is registered for.
            // tags:
            //      protected

            var registry = this._commandRegistry,
                isAvailableFlags;

            for (var key in registry) {
                if (key !== "toArray" && registry.hasOwnProperty(key)) {
                    isAvailableFlags = registry[key].isAvailable;
                    if (isAvailableFlags) {
                        var command = registry[key].command;
                        this._updateAvailabilityForSpecificCommand(command, menuType, isAvailableFlags);
                    }
                }
            }
        },

        _updateAvailabilityForSpecificCommand: function (command, menuType, isAvailableFlags) {
            // we don't want to update availability of TranslateCommand because her model knows better when to be available
            if (!command.isInstanceOf(TranslateContentCommand)) {
                command.set("isAvailable", !!(isAvailableFlags & menuType));
            }
        },

        _updateTreeContextCommandModels: function (model) {
            // summary:
            //      Update model of commands in case selected content is folder
            // tags:
            //      private

            this.inherited(arguments);

            this._commandRegistry.uploadDefault.command.set("model", model);
            //this._commandRegistry.upload.command.set("model", model);
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
                autofocus: !!window.FileReader,
                //autofocus: UploadUtil.supportNativeDndFiles(), // Only autofocus if not using flash.
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