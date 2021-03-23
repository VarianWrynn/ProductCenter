define(function (require, exports, module) {

    var $ = jQuery = require('./jquery');
    var ui = require('./jquery.ui');
    //var fileupload = require('./jquery.fileupload');
    var fileupload_ui = require('./jquery.fileupload-ui');
    var HMAutoModel = require('./SpecialJS/HMAuto');
   
    /*
     * jQuery File Upload Plugin JS Example 6.5.1
     * https://github.com/blueimp/jQuery-File-Upload
     *
     * Copyright 2010, Sebastian Tschan
     * https://blueimp.net
     *
     * Licensed under the MIT license:
     * http://www.opensource.org/licenses/MIT
     */

    /*jslint nomen: true, unparam: true, regexp: true */
    /*global $, window, document */

    $(document).ready(function () {

        'use strict';
        HMAutoModel.MediaHM.AutoHM();


        fileupload_ui();
        // Initialize the jQuery File Upload widget:
        $('#fileupload').fileupload();

        $('#fileupload').fileupload('option', {
            //maxFileSize: 1024*1024,
            maxFileSize: 52000000,
            resizeMaxWidth: 1920,
            resizeMaxHeight: 1200
        }).bind('fileuploadsubmit', function (e, data) {
            var inputs = data.context.find(':input');
            if (inputs.filter('[required][value=""]').first().focus().length) {
                return false;
            }
            //formData: { HMNUM: $("#hiddenAutoHM").val() }
            //data.formData = inputs.serializeArray();
        });
    });// end of $(document).ready
});