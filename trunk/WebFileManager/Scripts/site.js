var objList;
var CurrentFolder;
var CurrentId;
var ListID;
function genLink() {
    var sLink = CurrentFolder;
    $('#curentFolder').text(sLink);
    document.title = sLink + ' - .NET File managerment';
    var k = sLink.lastIndexOf('/');
    if (sLink == 'Root') {
        $('#upfd').css('display', 'none');
    }
    else {
        $('#upfd').css('display', 'block');
    }
    if (k >= 0) sLink = sLink.substring(0, k);

    $('#upfd li >a').attr("onclick", "GetList(\'" + sLink + "\')");
}
function GetList(fd) {
    ListID = '';
    CurrentId = '';
    $('#BT_Call_Move').button("disable");
    $('#BT_Call_Delete').button("disable");
    $('#BT_Call_Zip').button("disable");
    $('#BT_Call_Download').button("disable");
    var url = "/ajax/command.ashx?cmd=getlist&fd=" + fd;
    $.getJSON(url + "&format=json&jsoncallback=?",
    function (result) {
        $('#ListItem').html('loading...');
        if (fd == '') {
            CurrentFolder = 'Root';
        }
        else {
            CurrentFolder = fd;
        }
        genLink();
        if (result != null && result.error != null) {
            alert(result.error);
        }
        else {
            objList = result;
            biuldGrid(objList);
        }
    });
}

function biuldGrid(olist) {
    if (olist != null && olist.length > 0) {
        var sHtml = '';
        $.each(olist, function (i, val) {
            if (val.type == "folder") {
                sHtml += '<ul class="listfolder" id="' + val.id + '">';
            }
            else {
                sHtml += '<ul class="itemfile" id="' + val.id + '">';
            }
            sHtml += '<li class="TitleSelect"><input type="checkbox" id="selected_' + i + '" onclick="getListCheck();" /></li>';
            if (val.type == "folder") {
                sHtml += '<li class="name"><a href="javascript:void(0);" onclick="GetList(\'' + val.path + '\')">' + val.name + '</a></li>';
            }
            else {
                sHtml += '<li class="name ' + val.type + '">' + val.name + '</li>';
            }
            sHtml += '<li class="size">' + val.length + '</li>';
            sHtml += '<li class="create">' + ShowFormatDate(val.DateCreate) + '</li>';
            sHtml += '<li class="modify">' + ShowFormatDate(val.DateEdit) + '</li>';
            sHtml += '</ul>';
        });
        $('#ListItem').html(sHtml);
    }
    else {
        $('#ListItem').html('no item');
    }
}

function ShowFormatDateTime(oDate) {
    if (oDate == null) return "";
    return oDate.getHours() + ':' +
                oDate.getMinutes() + ':' +
                    oDate.getSeconds() + ' ' +
                        oDate.getDate() + '/' +
                            (oDate.getMonth() + 1) + '/' +
                                oDate.getFullYear();
}

function ShowFormatDate(oDate) {
    if (oDate == null) return "";
    return oDate.getDate() + '/' +
                (oDate.getMonth() + 1) + '/' +
                    oDate.getFullYear();
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
$(function () {
    $.contextMenu({
        selector: '#ListItem .listfolder .name',
        callback: function (key, options) {
            var id = $(this).parent().attr('id');
            EventContextFolder(key, id);
        },
        items: {
            "Open": { name: "Open", icon: "Open" },
            "Zip": { name: "Zip", icon: "Zip", cssover: "popup-button", href: "#popup_Zip" },
            "Rename": { name: "Rename", icon: "RenameFolder", cssover: "popup-button", href: "#popup_Rename" },
            "Move": { name: "Move", icon: "MoveFolder" },
            "Delete": { name: "Delete", icon: "DeleteFolder", cssover: "popup-button", href: "#popup_confirm" },
            //"Download": { name: "Download", icon: "DownloadFolder" },
            "line": "---------",
            "Property": { name: "Property", icon: "SettingFolder", cssover: "popup-button", href: "#popup_Property" }
        }
    });
});

function EventContextFolder(KeyEvent, id) {
    var obj = getItemList(id);
    CurrentId = id;
    switch (KeyEvent) {
        case "Open":
            ctFolderOpen(obj);
            break;
        case "Zip": 
            ctFolderZip(obj); 
            break; 
        case "Rename":
            ctFolderRename(obj);
            break;
        case "Move":
            ctMove(obj);
            break;
        case "Delete":
            ctDelete(obj);
            break;
        //        case "Download": 
        //            ctFolderDownload(obj); 
        //            break; 
        case "Property":
            ctFolderSetting(obj);
            break;
        default:
    }
}

function ctFolderOpen(obj) {
    GetList(obj.path);
}
function ctFolderZip(obj) {
    $('#txtStatusZip').text('');
    $('#lbZipFolderName').text(obj.name);
    $('#txtZipName').val(getOnlynameFile(obj.name));
}
function ctFolderRename(obj) {
    $('#Extension').text('');
    $('#txtStatusRename').text('');
    $('#txtFileName').val(obj.name);
}
function ctMove(obj) {
    $('#fileMove').text(obj.name);
    $('#lbStatusMove').text('');
    $('#BT_Show_Move').click();
}
function ctDelete(obj) {
    $('#lbStatusDelete').text('');
    if (obj.type == "folder") {
        $('#fileDelete').text("Are you want delete folder " + obj.name + " and all content items?");
    }
    else {
        $('#fileDelete').text("Are you want delete file " + obj.name);
    }
}
//function ctFolderDownload(obj) {
//    
//}
function ctFolderSetting(obj) {
    $('#lbStatusProperty').text('');
    $('#lbFilenameProperty').text(obj.name);
    var sCapasity = obj.length;
    if(sCapasity!=null && sCapasity !='')
        $('#lbCapasity').text('Size: ' + obj.length);
    else
        $('#lbCapasity').text('');
    document.getElementById('CB_System').checked = obj.isSystem;
    document.getElementById('CB_Readonly').checked = obj.isReadOnly;
    document.getElementById('CB_Hidden').checked = obj.isHidden; 
    //BT_Property_Ok
    //BT_Property_Cancel
}

function getItemList(id) {
    for (var i = 0; i < objList.length; i++) {
        if (objList[i].id == id) {
            return objList[i];
        }
    }
    return null;
}
/////////////////context menu file//////////////////
$(function () {
    $.contextMenu({
        selector: '#ListItem .itemfile .name:not(.zip,.txt,.html,.htm)',
        callback: function (key, options) {
            var id = $(this).parent().attr('id');
            EventContextFile(key, id);
        },
        items: {
            "Download": { name: "Download", icon: "DownloadFile" },
            "Rename": { name: "Rename", icon: "RenameFile", cssover: "popup-button", href: "#popup_Rename" },
            "Move": { name: "Move", icon: "MoveFile" },
            "Delete": { name: "Delete", icon: "DeleteFile", cssover: "popup-button", href: "#popup_confirm" },
            "line": "---------",
            "Property": { name: "Property", icon: "SettingFolder", cssover: "popup-button", href: "#popup_Property" }
        }
    });
});

$(function () {
    $.contextMenu({
        selector: '#ListItem .itemfile .zip',
        callback: function (key, options) {
            var id = $(this).parent().attr('id');
            EventContextFile(key, id);
        },
        items: {
            "Download": { name: "Download", icon: "DownloadFile" },
            "Rename": { name: "Rename", icon: "RenameFile", cssover: "popup-button", href: "#popup_Rename" },
            "Move": { name: "Move", icon: "MoveFile" },
            "Delete": { name: "Delete", icon: "DeleteFile", cssover: "popup-button", href: "#popup_confirm" },
            "line": "---------",
            
            "OpenOrchive": { name: "View Zip File",icon: "Zip", cssover: "popup-button", href: "#popup-viewzip" },
            /*"Zip": { name: "Zip", icon: "Zip",
                items: {
                    "OpenOrchive": { name: "Open orchive", cssover: "popup-button", href: "#popup-viewzip" },
                    "ExtractHere": { name: "Extract here" },
                    "ExtractTo": { name: "Extract to..." }
                }
            },*/
            "line2": "---------",
            "Property": { name: "Property", icon: "SettingFolder", cssover: "popup-button", href: "#popup_Property" }
        }
    });
});

$(function () {
    $.contextMenu({
        selector: '#ListItem .itemfile .txt,#ListItem .itemfile html,#ListItem .itemfile .htm',
        callback: function (key, options) {
            var id = $(this).parent().attr('id');
            EventContextFile(key, id);
        },
        items: {
            "Download": { name: "Download", icon: "DownloadFile" },
            "Rename": { name: "Rename", icon: "RenameFile", cssover: "popup-button", href: "#popup_Rename" },
            "Move": { name: "Move", icon: "MoveFile" },
            "Delete": { name: "Delete", icon: "DeleteFile", cssover: "popup-button", href: "#popup_confirm" },
            "line": "---------",
            "EditText": { name: "Edit Text", icon: "edit", cssover: "popup-button", href: "#popup-editText" },
            "line2": "---------",
            "Property": { name: "Property", icon: "SettingFolder", cssover: "popup-button", href: "#popup_Property" }
        }
    });
});

function EventContextFile(KeyEvent, id) {
    var obj = getItemList(id);
    CurrentId = id;
    switch (KeyEvent) {
        case "Rename":
            ctFileRename(obj);
            break;
        case "Move":
            ctMove(obj);
            break;
        case "Delete":
            ctDelete(obj);
            break;
        case "Download":
            ctFileDownload(obj);
            break;
        case "OpenOrchive":
            ctViewZipFile(obj);
            break;
        case "ExtractHere":
            ctExtractHere(obj);
            break;
        case "ExtractTo":
            ctExtractTo(obj);
            break;
        case "EditText":
            ctEditText(obj);
            break;
        case "Property":
            ctFolderSetting(obj);
            break;
        default:
    }
}

function ctFileRename(obj) {
    $('#Extension').text('.' + obj.type);
    $('#txtStatusRename').text('');
    $('#txtFileName').val(getOnlynameFile(obj.name));
}
function ctFileDownload(obj) {
    window.open("/ajax/command.ashx?cmd=download&id=" + obj.id);
}

function ctViewZipFile(obj) {
    $('#lbnameZip').text(obj.name);
    $('#ListContentZip').html('');
    $('#lbStatusContentZipFile').text('');
    document.getElementById("CB_Overwrite1").checked = false;
    $('#lbStatusContentZipFile').text('Opening...');
    $('#BT_ExtractHere').button("disable");
    $.getJSON('/ajax/command.ashx?cmd=ViewZip&id=' + CurrentId + '&type=' + obj.type
        + '&isFile=' + obj.isFile + '&format=json&jsoncallback=?',
            function (data) {
                $('#BT_ExtractHere').button("enable");
                if (data != null) {
                    if (data.error != null) {
                        $('#lbStatusContentZipFile').text(data.error);
                        return;
                    }
                    else {
                        $('#lbStatusContentZipFile').text('');
                        showContentZip(data);
                    }
                }
                else {
                    $('#lbStatusContentZipFile').text('Not connection!');
                }
            });
}
function ctExtractHere(obj) {
    
}

function ctExtractTo(obj) {

}

function ctEditText(obj) {
    $('#lbStatusEditText').text('');
    $('#lbFileNameEditText').text(obj.name);
    $('#txtContentEditText').val('');
    $.ajax({
        url: '/ajax/command.ashx?cmd=editText&id=' + CurrentId,
        success: function (data) {
            $('#txtContentEditText').val(data);
        }
    });
}

function showContentZip(olist) {
    if (olist != null && olist.length > 0) {
        var sHtml = '';
        $.each(olist, function (i, val) {
            sHtml += '<li>';
            sHtml += '<span>' + val.name + '</span>';
            sHtml += '</li>';
        });
        $('#ListContentZip').html(sHtml);
    }
    else {
        $('#ListContentZip').html('no item');
    }
}

function getOnlynameFile(sfile) {
    var n = sfile.lastIndexOf('.');
    if (n >= 0) sfile = sfile.substring(0, n);
    return sfile;
}
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function getListCheck() {
    var sList = '';
    $('#ListItem input[type=checkbox]').is(function () {
        if ($(this).is(':checked')) {
            var id = $(this).parent().parent().attr('id');
            if (getItemList(id).isFile) id = 'T' + id;
            else id = 'F' + id;
            if (sList == '')
                sList = id;
            else {
                sList += ';' + id;
            }
        }
    });
    if (sList != '') {
        $('#BT_Call_Move').button("enable");
        $('#BT_Call_Delete').button("enable");
        $('#BT_Call_Zip').button("enable");
    }
    else {
        $('#BT_Call_Move').button("disable");
        $('#BT_Call_Delete').button("disable");
        $('#BT_Call_Zip').button("disable");
    }
    ListID = sList;
}
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
$(document).ready(function () {
    $('.popup').draggable({ handle: ".popup-header" });
    $(".button").button();
    GetList('');
    //$('tr:Odd').addClass('chan');
    //$('tr:Even').addClass('le');

    //            $("tr").mouseenter(function () {
    //                if (this.find('td:eq(0)').find('input[type=checkbox]').is(':checked')) {
    //                    //chawngr lamf j
    //                }
    //                else {
    //                    this.addClass('over');
    //                }
    //            }).mouseleave(function () {
    //                if (bool) {
    //                    $(this).removeClass("over");
    //                }
    //            });

    //            $("tr td input[type=checkbox]").change(function () {
    //                if ($(this).is(':checked')) {
    //                    $(this).parent("td").parent("tr").addClass("select");
    //                } else {
    //                    $(this).parent("td").parent("tr").removeClass("select");
    //                }
    //            });

    //            $('td').filter(function () {
    //                //return (this.text().lastIndexOf('.m4a')>0);
    //                alert(this.text());
    //            }).addClass('m4a');
    //$('tr').find('td:eq(1)').addClass('m4a');

    //////////////////////

    $('#BT_Call_Refresh').click(function() {
        GetList(CurrentFolder);
        loadTreeview();
    });

    $('#BT_Show_Move').click(function () {
        $('#txtFolderMove').val('');
        if ($('#treeview').html() == '') {
            loadTreeview();
        }
        else {
            BuildTreeview();
        }
    });

    $('.popup-button').showPopup({
        top: 200, //khoảng cách popup cách so với phía trên
        closeButton: ".close_popup, .bt_cancel", //khai báo nút close cho popup
        scroll: false, //cho phép scroll khi mở popup, mặc định là không cho phép
        //outClose: false, //click ra ngoài là close
        onClose: function () {
            CurrentId = ''; //'$('#HD_ID').val('');
        }
    });
});
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

function loadTreeview() {
    $.ajax({
        url: '/ajax/command.ashx?cmd=treeview',
        success: function (data) {
            $('#treeview').html(data);
            BuildTreeview();
        }
    });
}
function BuildTreeview() {
    $('.hitarea').unbind('click');
    $("#treeview").treeview({
        animated: "fast",
        collapsed: true,
        unique: true,
        //persist: "cookie",
        toggle: function () {
            //window.console && console.log("%o was toggled", this);
        }
    });

    $('#treeview').find("span").click(function () {
        $('#txtFolderMove').val($(this).attr('title'));
    });

    $('#treeview').find("span").attr('style', "cursor:pointer;cursor: hand").addClass("folders");
}
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function BuildUpload() {
    $("#file_upload").uploadify({
        'swf': '/uploadify/uploadify.swf',
        'uploader': '/ajax/command.ashx?cmd=upload',
        'formData': { 'fd': CurrentFolder, 'vkl': 'vcc' },
        'auto': false,
        'buttonText': 'BROWSE...',
        'queueID': 'some_file_queue',
        'onUploadStart': function (file) {
            $('#StatusUpload').text('Starting to upload ' + file.name);
        },
        'onUploadSuccess': function (file, data, response) {
            $('#StatusUpload').text('The file ' + file.name + ' was successfully uploaded with a response of ' + response + ':' + data);
        },
        'onQueueComplete': function () {
            $('#StatusUpload').text('Upload successfully');
            GetList(CurrentFolder);
            setTimeout(function () {
                $('#BT_Upload_Cancel').click();
            }, 1000);
        }
    });
    $('#StatusUpload').text('Select file for upload');
}

function destroyUpload() {
    $('#file_upload').uploadify('destroy');
    $('#some_file_queue').html('');
    $('#StatusUpload').text('');
}

///////////////////rename//////////////////////
$(document).ready(function () {
    $('#BT_Rename').click(function () {
        var nName = $.trim($('#txtFileName').val());
        if (nName == '') {
            $('#txtStatusRename').text('you must select folder');
            $('#txtFileName').focus();
            return;
        }
        if (!validateFileName(nName)) {
            $('#txtStatusRename').text('validate name');
            $('#txtFileName').focus();
            return;
        }
        var oInfo = getItemList(CurrentId);
        if ($('#txtFileName').val() == getOnlynameFile(oInfo.name)) {
            $('#BT_Cancel_Rename').click();
            return;
        }
        $('#txtStatusRename').text('processing...');
        $('#BT_Rename').button("disable");
        $.getJSON('/ajax/command.ashx?cmd=rename&id=' + CurrentId + '&newname=' + nName
                    + '&type=' + oInfo.type + '&isFile=' + oInfo.isFile + '&format=json&jsoncallback=?',
            function (data) {
                $('#BT_Rename').button("enable");
                if (data != null) {
                    if (data.error != null) {
                        $('#txtStatusRename').text(data.error);
                        return;
                    }
                    else {
                        $('#txtStatusRename').text('rename ok!');
                        GetList(CurrentFolder);
                        if (!oInfo.isFile) loadTreeview();
                        setTimeout(function () {
                            $('#BT_Cancel_Rename').click();
                        }, 1000);
                    }
                }
                else {
                    $('#txtStatusRename').text('Not connection!');
                }
            }
        );
    });
});

///////////////////new folder//////////////////////

$(document).ready(function () {
    $('#BT_Call_NewFolder').click(function () {
        $('#txtNewFolder').val('');
        $('#lbStatusNewFolder').text('');
    });
    
    $('#BT_NewFolder').click(function () {
        var sNew = $('#txtNewFolder').val();
        if (!validateFolderName(sNew)) {
            $('#lbStatusNewFolder').text('validate name');
            return;
        }
        else {
            $('#lbStatusNewFolder').text('processing...');
            $('#BT_NewFolder').button("disable");
            $.getJSON('/ajax/command.ashx?cmd=newFolder&fd=' + CurrentFolder + '&newname=' + sNew + '&format=json&jsoncallback=?',
            function (data) {
                $('#BT_NewFolder').button("enable");
                if (data != null) {
                    if (data.error != null) {
                        $('#lbStatusNewFolder').text(data.error);
                        return;
                    }
                    else {
                        $('#lbStatusNewFolder').text('create ok!');
                        GetList(CurrentFolder);
                        loadTreeview();
                        setTimeout(function () {
                            $('#BT_Cancel_NewFolder').click();
                        }, 1000);
                    }
                }
                else {
                    $('#lbStatusNewFolder').text('Not connection!');
                }
            }
        );
        }
    });
});

function validateFolderName(sname) {
    var regex = new RegExp("^[a-zA-Z0-9_ ]+$", "gi");
    return regex.test(sname);
}

function validateFileName(sname) {
    var regex = new RegExp("^[a-zA-Z0-9_ ()-]+$", "gi");
    return regex.test(sname);
}
///////////////////delete//////////////////////
$(document).ready(function () {
    $('#BT_Delete').click(function () {
        var oInfo = getItemList(CurrentId);
        $('#lbStatusDelete').text('processing...');
        $(this).button("disable");
        $.getJSON('/ajax/command.ashx?cmd=delete&id=' + CurrentId + '&isFile=' + oInfo.isFile + '&format=json&jsoncallback=?',
            function (data) {
                $('#BT_Delete').button("enable");
                if (data != null) {
                    if (data.error != null) {
                        $('#lbStatusDelete').text(data.error);
                        return;
                    } else {
                        $('#lbStatusDelete').text('delete ok!');
                        GetList(CurrentFolder);
                        if (!oInfo.isFile) loadTreeview();
                        setTimeout(function() {
                            $('#BT_Delete_Cancel').click();
                        }, 1000);
                    }
                } else {
                    $('#lbStatusDelete').text('Not connection!');
                }
            });
    });
});
///////////////////move//////////////////////
$(document).ready(function () {
    $('#BT_Move').click(function () {
        var nf = $.trim($('#txtFolderMove').val());
        if (nf == '') {
            $('#lbStatusMove').text('you must select folder');
            $('#txtFolderMove').focus();
            return;
        }
        var oInfo = getItemList(CurrentId);
        $('#lbStatusMove').text('processing...');
        $(this).button("disable");
        $.getJSON('/ajax/command.ashx?cmd=move&id=' + CurrentId + '&isFile=' + oInfo.isFile + '&fd=' + CurrentFolder + '&nfd=' + nf + '&format=json&jsoncallback=?',
        function (data) {
            if (data != null) {
                $('#BT_Move').button("enable");
                if (data.error != null) {
                    $('#lbStatusMove').text(data.error);
                    return;
                } else {
                    $('#lbStatusMove').text('move ok!');
                    GetList(CurrentFolder);
                    if (!oInfo.isFile) loadTreeview();
                    setTimeout(function () {
                        $('#BT_Move_Cancel').click();
                    }, 1000);
                }
            } else {
                $('#lbStatusMove').text('Not connection!');
            }
        });
    });
});

///////////////////zip folder//////////////////////

$(document).ready(function () {
    $('#BT_Zip').click(function () {
        var nName = $.trim($('#txtZipName').val());
        if (nName == '') {
            $('#txtStatusZip').text('you must enter name');
            $('#txtZipName').focus();
            return;
        }
        var oInfo = getItemList(CurrentId);
        $('#txtStatusZip').text('processing...');
        $('#BT_Zip').button("disable");
        $.getJSON('/ajax/command.ashx?cmd=ZipFolder&id=' + CurrentId + '&newname=' + nName
                    + '&type=' + oInfo.type + '&isFile=' + oInfo.isFile + '&format=json&jsoncallback=?',
            function (data) {
                $('#BT_Zip').button("enable");
                if (data != null) {
                    if (data.error != null) {
                        $('#txtStatusZip').text(data.error);
                        return;
                    }
                    else {
                        $('#txtStatusZip').text('zip ok!');
                        GetList(CurrentFolder);
                        //if (!oInfo.isFile) loadTreeview();
                        setTimeout(function () {
                            $('#BT_Zip_Cancel').click();
                        }, 1000);
                    }
                }
                else {
                    $('#txtStatusZip').text('Not connection!');
                }
            });
    });
});

///////////////////Extract zip//////////////////////
$(document).ready(function () {
    $('#BT_ExtractHere').click(function () {
        $('#lbStatusContentZipFile').text('Extracting...');
        $('#BT_ExtractHere').button("disable");
        $.getJSON('/ajax/command.ashx?cmd=ExtractHere&id=' + CurrentId + '&Overwrite=' + document.getElementById("CB_Overwrite1").checked + '&format=json&jsoncallback=?',
            function (data) {
                $('#BT_ExtractHere').button("enable");
                if (data != null) {
                    if (data.error != null) {
                        $('#lbStatusContentZipFile').text(data.error);
                        return;
                    }
                    else {
                        $('#lbStatusContentZipFile').text('Extract ok!');
                        GetList(CurrentFolder);
                        loadTreeview();
                        setTimeout(function () {
                            $('#BT_ContentZip_Cancel').click();
                        }, 1000);
                    }
                }
                else {
                    $('#lbStatusContentZipFile').text('Not connection!');
                }
            });
    });
});

///////////////////edit text//////////////////////
$(document).ready(function () {
    $('#BT_Save_EditText').click(function () {
        $('#lbStatusEditText').text('saving...');
        $('#BT_Save_EditText').button("disable");
        var s = $('#txtContentEditText').val();
        s = s.replace(/\r\n|\r|\n/g, "\\n");
        $.getJSON('/ajax/command.ashx?cmd=SaveText&id=' + CurrentId + '&Content=' + s + '&format=json&jsoncallback=?',
        function (data) {
            $('#BT_Save_EditText').button("enable");
            if (data != null) {
                if (data.error != null) {
                    $('#lbStatusEditText').text(data.error);
                    return;
                }
                else {
                    $('#lbStatusEditText').text('saved');
                    setTimeout(function () {
                        $('#BT_EditText_Cancel').click();
                    }, 1000);
                }
            }
            else {
                $('#lbStatusEditText').text('Not connection!');
            }
        });
    });
});