(function (app, $) {

    app.GridViewModel = function (options) {
        var self = this;

        /*
        * private properties
        */
        var _criteria = options.Criteria || { PageSize: 10, PageIndex: 1, SortExpression: "", SortDirection: "" };
        var _dataSourceUrl = options.DataSourceUrl;
        var _targetDom = options.TargetDom;
        var _isInlineEdit = options.InlineEdit || false;
        var _originalEditObject = null;
        var _insertRowFocusedControlIndex = options.InsertRowFocusedControlIndex || 0;


        /*
        * observable properties
        */
        self.totalPages = ko.observable(0);
        self.totalRecords = ko.observable();

        self.currentPageIndex = ko.observable(1);
        self.selectedRowIndex = ko.observable(0);

        self.listDataItems = ko.observableArray([]);
        self.listNewItems = ko.observableArray();
        self.listEditItems = ko.observableArray();
        self.CustomResults = ko.observableArray([]);

        self.numPageSize = [10, 20, 30, 40, 50];
        self.selectedPageSize = ko.observable(_criteria.PageSize);

        //editing row index
        self.editingRowIndex = ko.observable(-1); // init as not edit

        self.sortOrder = ko.observable("ASC");
        self.sortExpression = ko.observable();

        self.isCreateNewInline = ko.observable(false);
        self.isEditInline = ko.observable(false);

        // Edit in Cell Mode
        self.f2 = ko.observable(false);

        self.noError = ko.observable(true);

        self.isBusy = ko.observable(false);

        $(document).ajaxComplete(function () {
            if ($.active <= 1) {
                self.isBusy(false);
            }
        });

        $(document).ajaxStart(function () {
            if (!self.isBusy()) self.isBusy(true);
        });

        /*
        * computed functions
        */
        self.numPages = ko.computed(function () {
            var arr = [];
            for (var i = 0; i < self.totalPages() ; i++) {
                arr[i] = i + 1;
            }
            return arr;
        });

        //current page index change
        self.currentPageIndexChange = ko.computed(function () {
            if (self.currentPageIndex() != null && _criteria.PageIndex !== self.currentPageIndex()) {
                //check if list of new or edited items exist
                if (SKTIS.Checker.modifiedDataExistsForAjax([self.listNewItems, self.listEditItems]) == true) {
                    //revert curentPageIndex value to before changed
                    self.currentPageIndex(_criteria.PageIndex);
                    return;
                } else {
                    self.listNewItems.removeAll();
                    self.listEditItems.removeAll();
                }

                _criteria.PageIndex = self.currentPageIndex();
                loadData();
                self.selectedRowIndex(0);
            }
        });

        self.selectedPageSizeChange = ko.computed(function () {
            if (self.selectedPageSize() != null && _criteria.PageSize !== self.selectedPageSize()) {
                //check if list of new or edited items exist
                if (SKTIS.Checker.modifiedDataExistsForAjax([self.listNewItems, self.listEditItems]) == true) {
                    //revert selectedPageSize value to before changed
                    self.selectedPageSize(_criteria.PageSize);
                    return;
                } else {
                    self.listNewItems.removeAll();
                    self.listEditItems.removeAll();
                }

                _criteria.PageSize = self.selectedPageSize();
                _criteria.PageIndex = 1; self.currentPageIndex(1);
                
                loadData();
                
                self.selectedRowIndex(0);
            }
        });

        /*
        *  public functions
        */
        self.selectRow = function (index) {
            self.selectedRowIndex(index);
            return true;
        }

        self.moveSelectedUp = function () {
            var index = self.selectedRowIndex();

            if (index > 0) {
                index--;
                self.selectedRowIndex(index);
            }
        }

        self.moveSelectedDown = function () {
            var index = self.selectedRowIndex();
            if (index < self.listDataItems().length - 1) {
                index++;
                self.selectedRowIndex(index);
            }
        }


        self.nextPage = function () {
            var index = self.currentPageIndex();
            if (index < self.totalPages()) {
                index++;
                self.currentPageIndex(index);
            }
        }

        self.prevPage = function () {
            var index = self.currentPageIndex();
            if (index > 1) {
                index--;
                self.currentPageIndex(index);
            }
        }

        self.enterPage = function (index) {
            self.currentPageIndex(index);
        }

        self.firstPage = function () {
            var index = self.currentPageIndex();
            if (index > 1) {
                index = 1;
                self.currentPageIndex(index);
            }
        }

        self.lastPage = function () {
            var index = self.currentPageIndex();
            if (index !== self.totalPages() && self.totalPages() > 0) {
                self.currentPageIndex(self.totalPages());
            }
        }

        self.nextPageSize = function () {
            for (var i = 0; i < self.numPageSize.length - 1; i++) {
                if (self.numPageSize[i] === self.selectedPageSize()) {
                    self.selectedPageSize(self.numPageSize[i + 1]);
                    break;
                }
            }
        }
        self.prevPageSize = function () {
            for (var i = 1; i < self.numPageSize.length; i++) {
                if (self.numPageSize[i] === self.selectedPageSize()) {
                    self.selectedPageSize(self.numPageSize[i - 1]);
                    break;
                }
            }
        }

        self.sortBy = function (expression, callback) {
            //check if list of new or edited items exist
            if (SKTIS.Checker.modifiedDataExistsForAjax([self.listNewItems, self.listEditItems]) == true) {
                return;
            } else {
                self.listNewItems.removeAll();
                self.listEditItems.removeAll();
            }

            if (self.sortExpression() === expression)
                self.sortOrder(self.sortOrder() === "ASC" ? "DESC" : "ASC");
            else
                self.sortOrder('ASC');
            self.sortExpression(expression);

            _criteria.SortExpression = self.sortExpression();
            _criteria.SortOrder = self.sortOrder();

            if (isFunction(callback)) {
                loadData(callback);
            } else {
                loadData();
            }
        }

        function isFunction(functionToCheck) {
            var getType = {};
            return functionToCheck && getType.toString.call(functionToCheck) === '[object Function]';
        }

        self.sort = function (expression, sortdirection) {
            self.sortExpression(expression);
            self.sortOrder(sortdirection);

            _criteria.SortExpression = self.sortExpression();
            _criteria.SortOrder = self.sortOrder();
            loadData();
        }

        self.selectedItem = function () {
            if (self.listDataItems().length > 0 && self.selectedRowIndex() >= 0)
                return ko.mapping.toJS(self.listDataItems()[self.selectedRowIndex()]);
            else
                return null;
        }

        self.updateSelectedItem = function (data) {
            self.listDataItems.replace(self.listDataItems()[self.selectedRowIndex()], data);
        }

        self.createNewItem = function (data) {
            if (self.listDataItems() == null)
                self.listDataItems([]);
            self.listDataItems.push(data);
        }

        self.onDelete = null;

        self.deleteItem = function (item) {
            self.listDataItems.remove(item);
            var index = self.selectedRowIndex();
            if (index === self.listDataItems().length) {
                index--;
                self.selectedRowIndex(index);
            }

            if (self.onDelete != null)
                self.onDelete(item);
        }

        //search function
        self.search = function (criteria, callback) {
            if (criteria != null) {
                _criteria.PageIndex = 1;
                $.extend(_criteria, criteria);
                loadData(callback);
                self.currentPageIndex(1);
                self.selectedRowIndex(0);
            }
        }

        self.active = function (isActive) {
        }

        self.enableInlineEdit = function (enable) {
            _isInlineEdit = enable;
        }

        self.setDataSourceUrl = function (url) {
            _dataSourceUrl = url;
        }

        /*
        * Inline edit functions
        */
        self.mapping = null;
        self.applyValidationRules = null;
        self.editInline = function (index, data, event) {
            if (!_isInlineEdit || self.editingRowIndex() !== -1)
                return;

            self.editingRowIndex(index);
            _originalEditObject = data;

            var obj = self.mapping == null ? ko.mapping.fromJS(data) : ko.mapping.fromJS(data, self.mapping);

            if (self.applyValidationRules != null) {
                self.applyValidationRules(obj);
                self.errors = ko.validation.group(obj);
            }

            self.listDataItems.replace(self.listDataItems()[index], obj);
        }

        self.editInline2 = function (index, data, event) {

            self.editingRowIndex(index);
            _originalEditObject = data;

            var obj = self.mapping == null ? ko.mapping.fromJS(data) : ko.mapping.fromJS(data, self.mapping);

            if (self.applyValidationRules != null) {
                self.applyValidationRules(obj);
                self.errors = ko.validation.group(obj);
            }

            self.listDataItems.replace(self.listDataItems()[index], obj);
        }

        self.editInline3 = function (index, data, event) {
            self.isEditInline(true);

            if(typeof event != 'undefined'){
                if ($(event.target).is('a.newTabLink')) {
                    window.open($(event.target).attr('href'), $(event.target).attr('target'));
                    return;
                }
                if ($(event.target).is('td.hasModal') || $(event.target).is('span.hasModal')) {
                    return;
                }
            }

            SKTIS.Helper.Log('Begin editInLine3');
            if (!_isInlineEdit || self.editingRowIndex() !== -1) {
                if (self.noError())
                    self.saveInline(self.selectedRowIndex(), self.listDataItems()[self.selectedRowIndex()]);
            }

            SKTIS.Helper.Log('Begin editInLine3');
            if (!_isInlineEdit || self.editingRowIndex() !== -1) {
                if (self.noError())
                    self.saveInline(self.selectedRowIndex(), self.listDataItems()[self.selectedRowIndex()]);
            }

            if (self.errors == null || self.errors().length <= 0) {
                self.editingRowIndex(-1);
                self.editingRowIndex(index);
                _originalEditObject = data;
                var obj = self.mapping == null ? ko.mapping.fromJS(data) : ko.mapping.fromJS(data, self.mapping);
                self.listDataItems.replace(self.listDataItems()[index], obj);
            }
            // Force to render selectpicker
            $('.selectpicker').selectpicker('render');
            SKTIS.Helper.Log('End editInLine3');
        }
        
        self.editInlineWithCallback = function (index, cb, data, event) {
            if (typeof cb == 'function') {
                cb(data, index);
            }
            self.isEditInline(true);
            
            if (typeof event != 'undefined') {
                if ($(event.target).is('a.newTabLink')) {
                    window.open($(event.target).attr('href'), $(event.target).attr('target'));
                    return;
                }
                if ($(event.target).is('td.hasModal') || $(event.target).is('span.hasModal')) {
                    return;
                }
            }

            SKTIS.Helper.Log('Begin editInLine3');
            if (!_isInlineEdit || self.editingRowIndex() !== -1) {
                if (self.noError())
                    self.saveInline(self.selectedRowIndex(), self.listDataItems()[self.selectedRowIndex()]);
            }

            SKTIS.Helper.Log('Begin editInLine3');
            if (!_isInlineEdit || self.editingRowIndex() !== -1) {
                if (self.noError())
                    self.saveInline(self.selectedRowIndex(), self.listDataItems()[self.selectedRowIndex()]);
            }

            if (self.errors == null || self.errors().length <= 0) {
                self.editingRowIndex(-1);
                self.editingRowIndex(index);
                _originalEditObject = data;
                var obj = self.mapping == null ? ko.mapping.fromJS(data) : ko.mapping.fromJS(data, self.mapping);
                self.listDataItems.replace(self.listDataItems()[index], obj);
            }
            // Force to render selectpicker
            $('.selectpicker').selectpicker('render');
            SKTIS.Helper.Log('End editInLine3');

        }

        self.editInlineCurrentSelectedRow = function () {
            if (_isInlineEdit) {
                self.editInline(self.selectedRowIndex(), self.listDataItems()[self.selectedRowIndex()]);
            }
        }

        self.onAfterSave = null;
        self.saveInline = function (index, data) {
            SKTIS.Helper.Log('Begin saveInline');
            var value = ko.mapping.toJS(data);
            if (self.onSaveInline != null)
                self.onSaveInline(value);

            self.editingRowIndex(-1);

            if (self.isCreateNewInline()) {
                self.listNewItems().push(value);
            } else {
                var indexBefore = index;
                if (self.listNewItems.length > 0) {
                    indexBefore = index - self.listNewItems.length;
                }
                if (indexBefore < 0) {
                    indexBefore = index - self.listNewItems.length;
                    if (self.listNewItems.hasOwnProperty(indexBefore)) {
                        self.listNewItems.replace(self.listNewItems()[indexBefore], value);
                    } else {
                        self.listNewItems()[indexBefore] = value;
                    }
                } else {
                    if (self.listEditItems.hasOwnProperty(indexBefore)) {
                        self.listEditItems.replace(self.listEditItems()[indexBefore], value);
                    } else {
                        var changeAvailable = false;
                        $.each(data, function (k, v) {
                            if (ko.isObservable(v))
                                if (v._versionNumber > 1)
                                    changeAvailable = true;
                        });
                        if (changeAvailable)
                            self.listEditItems()[indexBefore] = value;
                    }
                }

            }

            self.isCreateNewInline(false);

            self.listDataItems.replace(self.listDataItems()[index], value);

            if (self.onAfterSave != null)
                self.onAfterSave();
            if ($('tr.insertRow input').length > 0)
                $('tr.insertRow input').get(_insertRowFocusedControlIndex).focus();
            else {
                $('.btn-primary').get(_insertRowFocusedControlIndex).focus();
            }
            self.errors = null;
            SKTIS.Helper.Log('End saveInline');
        }

        self.onSaveInline2 = null;
        self.saveInline2 = function (index, data) {
            if (_isInlineEdit == true && self.errors().length === 0) {
                var value = ko.mapping.toJS(data);
                if (self.onSaveInline2 != null) {
                    //SAVE BUT ALLOW SOME VALIDATION HANDLING FIRST AND SHOW ERRORS IF ANY
                    if (!self.onSaveInline2(value, data)) {
                        return false;
                    }
                }

                self.editingRowIndex(-1);
                self.isCreateNewInline(false);

                self.listDataItems.replace(self.listDataItems()[index], value);

                if (self.onAfterSave != null)
                    self.onAfterSave();
            }
            else if (self.errors().length > 0) {
                if (self.onSaveInline2 != null) {
                    var value = ko.mapping.toJS(data);
                    if (!self.onSaveInline2(value, data)) {
                        self.errors.showAllMessages();
                        return false;
                    }
                }

                self.errors.showAllMessages();
            }
        }

        self.saveInlineEditingRow = function () {
            if (_isInlineEdit) {
                self.saveInline(self.editingRowIndex(),
                                self.selectedItem());
            }
        }

        self.saveInlineEditingRow2 = function () {
            if (_isInlineEdit) {
                return self.saveInline2(self.editingRowIndex(),
                                self.selectedItem());
            }
        }


        self.saveInlineWithPopulatedValue = function (value) {
            if (_isInlineEdit) {
                if (self.onSaveInline != null)
                    self.onSaveInline(value);

                self.listDataItems.replace(self.listDataItems()[self.editingRowIndex()], value);

                self.editingRowIndex(-1);
                self.isCreateNewInline(false);
            }
        }

        // delegate for inline saving
        self.onSaveInline = null;

        self.cancelInline = function (item) {
            SKTIS.Helper.Log('Begin cancelInline');
            if (_isInlineEdit) {
                if (!self.isCreateNewInline()) {
                    if (_originalEditObject != null)
                        self.listDataItems.replace(self.listDataItems()[self.editingRowIndex()], _originalEditObject);

                    self.editingRowIndex(-1);
                    self.errors = null;
                }
                else {
                    if (item == null)
                        item = self.selectedItem();
                    self.listDataItems.remove(item);
                    self.editingRowIndex(-1);
                    self.selectedRowIndex(0);
                    self.isCreateNewInline(false);

                    //remove validation for item
                    self.errors = null;
                }
            }
            SKTIS.Helper.Log('End cancelInline');
        }

        self.addNewInline = function (data) {
            if (_isInlineEdit && self.editingRowIndex() === -1) {

                //apply validation rules
                var obsvObj = self.mapping == null ? ko.mapping.fromJS(data) : ko.mapping.fromJS(data, self.mapping);

                self.errors = ko.validation.group(obsvObj);
                //if (self.applyValidationRules != null) self.applyValidationRules(obsvObj);

                //self.lastPage();
                self.listDataItems.push(obsvObj);
                self.editingRowIndex(self.listDataItems().length - 1);
                self.selectedRowIndex(self.editingRowIndex());
                self.isCreateNewInline(true);

                return obsvObj;
            }
        }

        self.addNewInline2 = function (data) {
            SKTIS.Helper.Log('Begin addNewInline2');
            if (!_isInlineEdit || self.editingRowIndex() !== -1) {
                SKTIS.Helper.Log('if addNewInline2');
                self.saveInline(self.selectedRowIndex(), self.listDataItems()[self.selectedRowIndex()]);

                if (self.errors().length === 0) {
                    //apply validation rules
                    var obsvObj = self.mapping == null ? ko.mapping.fromJS(data) : ko.mapping.fromJS(data, self.mapping);

                    self.errors = ko.validation.group(obsvObj);
                    //if (self.applyValidationRules != null) self.applyValidationRules(obsvObj);

                    //self.lastPage();
                    self.listDataItems.unshift(obsvObj);
                    //self.editingRowIndex(0);
                    self.selectedRowIndex(self.editingRowIndex());
                    self.isCreateNewInline(true);

                    return obsvObj;
                }
                else if (self.errors().length > 0) self.errors.showAllMessages();
            }
            else {
                SKTIS.Helper.Log('else addNewInline2');
                //apply validation rules
                var obsvObj = self.mapping == null ? ko.mapping.fromJS(data) : ko.mapping.fromJS(data, self.mapping);
                self.applyValidationRules(obsvObj);

                self.errors = ko.validation.group(obsvObj);
                //if (self.applyValidationRules != null) self.applyValidationRules(obsvObj);

                if (self.errors().length != 0) {
                    $.each(self.errors(), function (k, v) {
                        SKTIS.Helper.Notification(v);
                    });
                    self.cancelInline();
                    return false;
                }

                //self.lastPage();
                self.listDataItems.unshift(obsvObj);
                self.editingRowIndex(0);

                self.selectedRowIndex(self.editingRowIndex());
                self.isCreateNewInline(true);

                self.saveInline(self.editingRowIndex(), self.listDataItems()[self.selectedRowIndex()]);
                if (typeof self.onAfterInsert == 'function') {
                    self.onAfterInsert(data);
                    SKTIS.Helper.Log('onAfterInsert');
                }
                return obsvObj;
            }
            //}
            SKTIS.Helper.Log('End addNewInline2');
        }

        self.addNewInlineWorkerAssignment = function (data) {
            SKTIS.Helper.Log('Begin addNewInlineWorkerAssignment');
            if (!_isInlineEdit || self.editingRowIndex() !== -1) {
                SKTIS.Helper.Log('if addNewInlineWorkerAssignment');
                self.saveInline(self.selectedRowIndex(), self.listDataItems()[self.selectedRowIndex()]);

                if (self.errors().length === 0) {
                    //apply validation rules
                    var obsvObj = self.mapping == null ? ko.mapping.fromJS(data) : ko.mapping.fromJS(data, self.mapping);

                    self.errors = ko.validation.group(obsvObj);
                    //if (self.applyValidationRules != null) self.applyValidationRules(obsvObj);

                    //self.lastPage();
                    self.listDataItems.unshift(obsvObj);
                    //self.editingRowIndex(0);
                    self.selectedRowIndex(self.editingRowIndex());
                    self.isCreateNewInline(true);

                    return obsvObj;
                }
                else if (self.errors().length > 0) self.errors.showAllMessages();
            }
            else {
                SKTIS.Helper.Log('else addNewInlineWorkerAssignment');
                //apply validation rules
                var obsvObj = self.mapping == null ? ko.mapping.fromJS(data) : ko.mapping.fromJS(data, self.mapping);
                var status = true;
                self.applyValidationRules(obsvObj);

                self.errors = ko.validation.group(obsvObj);
                //if (self.applyValidationRules != null) self.applyValidationRules(obsvObj);

                if (self.errors().length != 0) {
                    $.each(self.errors(), function (k, v) {
                        SKTIS.Helper.Notification(v);
                    });
                    self.cancelInline();
                    return false;
                }

                //self.lastPage();
                self.listDataItems.unshift(obsvObj);
                self.editingRowIndex(0);
                $.each(self.listDataItems(), function (i, v) {
                    if (i != 0) {
                        if (ko.isObservable(v.EmployeeID)) {
                            var employee = v.EmployeeID();
                        } else {
                            var employee = v.EmployeeID;
                        }
                        if (ko.isObservable(v.StartDate) && ko.isObservable(v.EndDate)) {
                            var start_date = v.StartDate();
                            var end_date = v.EndDate();
                        } else {
                            var start_date = v.StartDate;
                            var end_date = v.EndDate;
                        }
                        
                        if ((employee == data.EmployeeID()) && (data.StartDate() >= start_date && data.EndDate() <= end_date)) {
                            status = false;
                            SKTIS.Helper.Notification('<strong>Employee id ' + employee + '</strong> with <strong>start date ' + start_date + '</strong> to <strong>end date ' + end_date + '</strong> is already assignment', 'warning');
                        }
                    }
                });
                
                self.selectedRowIndex(self.editingRowIndex());
                self.isCreateNewInline(true);

                self.saveInline(self.editingRowIndex(), self.listDataItems()[self.selectedRowIndex()]);
                if (typeof self.onAfterInsert == 'function') {
                    self.onAfterInsert(data);
                    SKTIS.Helper.Log('onAfterInsert');
                }

                if (status == false) {
                    self.listDataItems.shift();
                }
                
                return obsvObj;
            }
            //}
            SKTIS.Helper.Log('End addNewInlineWorkerAssignment');
        }


        self.addNewInlineHoliday = function (data) {
            SKTIS.Helper.Log('Begin addNewInlineHoliday');
            if (!_isInlineEdit || self.editingRowIndex() !== -1) {
                SKTIS.Helper.Log('if addNewInlineHoliday');
                self.saveInline(self.selectedRowIndex(), self.listDataItems()[self.selectedRowIndex()]);

                //console.log(self.selectedRowIndex());
                //console.log(self.listDataItems());
                //console.log(self.selectedRowIndex());

                if (self.errors().length === 0) {
                    //        //apply validation rules
                    //        var obsvObj = self.mapping == null ? ko.mapping.fromJS(data) : ko.mapping.fromJS(data, self.mapping);

                    //        self.errors = ko.validation.group(obsvObj);
                    //        //if (self.applyValidationRules != null) self.applyValidationRules(obsvObj);

                    //        //self.lastPage();
                    //        self.listDataItems.unshift(obsvObj);
                    //        self.editingRowIndex(0);
                    //        self.selectedRowIndex(self.editingRowIndex());
                    //        self.isCreateNewInline(true);

                    //return obsvObj;
                }
                //    else if (self.errors().length > 0) self.errors.showAllMessages();
            }
            else {
                //apply validation rules
                var obsvObj = self.mapping == null ? ko.mapping.fromJS(data) : ko.mapping.fromJS(data, self.mapping);
                self.applyValidationRules(obsvObj);

                self.errors = ko.validation.group(obsvObj);
                if (self.errors().length != 0) {
                    $.each(self.errors(), function (k, v) {
                        SKTIS.Helper.Notification(v);
                    });
                    self.cancelInline();
                    return false;
                }

                var locationCodes = data.LocationCode();
                var copydata = data;

                for (var i = 0; i < locationCodes.length; i++) {
                    copydata.LocationCode = ko.observable(locationCodes[i]);
                    SKTIS.Helper.Log('else addNewInlineHoliday');

                    obsvObj = self.mapping == null ? ko.mapping.fromJS(copydata) : ko.mapping.fromJS(copydata, self.mapping);
                    self.listDataItems.unshift(obsvObj);
                    self.editingRowIndex(0);

                    self.selectedRowIndex(self.editingRowIndex());
                    self.isCreateNewInline(true);

                    self.saveInline(self.editingRowIndex(), self.listDataItems()[self.selectedRowIndex()]);
                    if (typeof self.onAfterInsert == 'function') {
                        self.onAfterInsert(copydata);
                        SKTIS.Helper.Log('onAfterInsert');
                    }
                }
                return obsvObj;
            }
            SKTIS.Helper.Log('End addNewInlineHoliday');
        }

        self.getEditingRowData = function () {
            if (_isInlineEdit && self.listDataItems != null)
                return ko.mapping.toJS(self.listDataItems[self.editingRowIndex]);
        }

        /*
        *  css functions
        */
        self.sortCss = function (column, expression, order) {
            if (column === expression) {
                if (order === 'ASC')
                    return 'sorting_asc';
                else
                    return 'sorting_desc';
            } else {
                return 'sorting';
            }
        }
        //self.sortCss = function (column, expression, order) {
        //    if (column === expression) {
        //        if (order === 'ASC')
        //            return 'ui-icon sorting_asc';
        //        else
        //            return 'ui-icon sorting_desc';
        //    } else {
        //        return 'ui-icon sorting';
        //    }
        //}

        self.reset = function () {
            self.listDataItems([]);
            //self.active(false);
            self.editingRowIndex(-1);

            self.totalPages(0);
            self.totalRecords(-1);

            self.currentPageIndex(1);
            self.selectedRowIndex(0);

            //self.selectedPageSize = ko.observable(_criteria.PageSize);
            self.isCreateNewInline = ko.observable(false);
            self.errors = null;
        }

        self.reload = function () {
            loadData();
        };

        /*
        * private functions
        */
        function loadData(callback) {
            //load data
            $.ajax({
                url: _dataSourceUrl,
                type: 'POST',
                contentType: 'application/json',
                data: ko.toJSON(_criteria),
                cache: false,
                success: function (data) {
                    if (data != null) {
                        if (data.Results != null) {
                            var convertObjectToObervable = [];
                            $.each(data.Results, function (k, v) {
                                var obj = self.mapping == null ? ko.mapping.fromJS(v) : ko.mapping.fromJS(v, self.mapping);
                                SKTIS.Helper.Log(obj);
                                convertObjectToObervable.push(obj);
                            });
                            self.listDataItems(convertObjectToObervable);
                            if (data.Results.length <= 0)
                                if (callback) {
                                    var response = { 'status': 'Empty', 'message': 'Data Empty' };
                                    callback(response);
                                }
                        }
                        else {
                            self.listDataItems([]);
                            if (callback) {
                                var response = { 'status': 'Empty', 'message': 'Data Empty' };
                                callback(response);
                            }
                        }
                        
                        // Custom Model
                        if (data.CustomResults != null) {
                            var customResultsObj = [];
                            $.each(data.CustomResults, function (k, v) {
                                var obj = self.customresultmapping == null ? ko.mapping.fromJS(v) : ko.mapping.fromJS(v, self.customresultmapping);
                                customResultsObj.push(obj);
                            });
                            self.CustomResults(customResultsObj);
                            SKTIS.Helper.Log(customResultsObj);
                        }                        

                        self.totalPages(data.TotalPages);
                        self.totalRecords(data.TotalRecords);

                        self.active(true);
                        self.errors = null;

                        if (callback) {
                            var response = { 'status': 'OK', 'message': 'Data loaded successfully' };
                            callback(response);
                        }

                        // fix messy selectpicker
                        $('.selectpicker').selectpicker('render');
                    }

                    // to fix lost focus on IE after load data
                    if ($('div.tab-content > div.active').length > 0) {
                        if ($('div.tab-content > div.active').find("button.btn-primary").length > 0) {
                            //console.log($('div.tab-content > div.active').find("button.btn-primary").get(0));
                            $('div.tab-content > div.active').find("button.btn-primary").get(0).focus(function () {
                                //console.log('focus');
                            });
                        }
                    } else if ($('.btn-primary').length > 0) {
                        $('.btn-primary').get(0).focus();
                    }
                },
                error: function (xhr, status, error) {
                    var response = { 'status': 'KO', 'message': error };
                    if (callback) callback(response);
                }
            });

            // jump to top of the page
            $("html, body").animate({ scrollTop: 0 }, "slow");
        }

        self.keydown = function (data, event) {
            if (event.keyCode == '113') {
                self.f2(true);
                $(event.target).attr('style', 'border-color: #66afe9;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset, 0 0 8px rgba(255, 50, 0, 0.6);outline: 0 none;');
            }

            var that = $(event.target);
            var td = that.parent();
            var tr = $('tbody');
            var $focused = $(':focus');
            var isInput = $focused.is('input');
            if (isInput) {
                var col = $focused.parents('td')
                var colNumber = col.parents('tr').find('td').index(col);
            }
            var input = $("input")[0];
            var ind = self.editingRowIndex();
            if (!self.f2()) {
                // Up
                if (event.keyCode == '38') {
                    self.triggerValidation(data);
                    self.editInline3(ind - 1, self.listDataItems()[ind - 1]);
                    
                    // For IE you need to use a settimeout function due to it being lazy
                    //tr.parent().find('input').first().css("background-color", "red");
                    setTimeout(function () { tr.parent().find('input').first().focus(); }, 10);
                }
                // Down
                if (event.keyCode == '40') {
                    self.triggerValidation(data);
                    self.editInline3(ind + 1, self.listDataItems()[ind + 1]);
                    
                    // For IE you need to use a settimeout function due to it being lazy
                    //tr.parent().find('input').first().css("background-color", "red");
                    setTimeout(function () { tr.parent().find('input').first().focus(); }, 10);
                }
                // Left
                if (event.keyCode == '37') {
                    var prevInput = td.prev().find('input');
                    if (prevInput.length == 0) {
                        self.triggerValidation(data);
                        self.editInline3(ind - 1, self.listDataItems()[ind - 1]);
                        $(':focus').parent().parent().children().last().find('input').focus(); // Force to focus to last td
                    }
                    td.prev().find('input').focus();
                }
                // Right
                if (event.keyCode == '39') {
                    var nextInput = td.next().find('input');
                    if (nextInput.length == 0) {
                        self.triggerValidation(data);
                        self.editInline3(ind + 1, self.listDataItems()[ind + 1]);
                    }
                    td.next().find('input').focus();
                }
            }
            return true;
        }

        self.triggerValidation = function (data) {
            var _oldValues = ko.mapping.toJS(data);
            if (self.applyValidationRules != null) {
                self.applyValidationRules(data);
                self.errors = ko.validation.group(data);
            }
            if (self.errors().length != 0) {
                $.each(self.errors(), function (k, v) {
                    SKTIS.Helper.Notification(v);
                });
                // Rollback observable values
                for (var i in data) {
                    if (ko.isObservable(data[i]) && ['ResponseType', 'Message', 'errors'].indexOf(i) < 0)
                        if (!data[i].isValid())
                            data[i](_oldValues[i]);
                }
                self.noError(false);
                return false;
            }
        }
    }
})(this.app = this.app || {}, jQuery);

