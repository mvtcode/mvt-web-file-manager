var objList; // = new Array();
function genLink() {
    var sLink = $('#HD_CurrentFolder').val();
    $('#curentFolder').text(sLink);

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
    var url = "/ajax/command.ashx?cmd=getlist&fd=" + fd;
    $.getJSON(url + "&format=json&jsoncallback=?",
    function (result) {
        $('#ListItem').html('loading...');
        if (fd == '') {
            $('#HD_CurrentFolder').val('Root');
        }
        else {
            $('#HD_CurrentFolder').val(fd);
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
                sHtml += '<li class="name">' + val.name + '</li>';
            }
            sHtml += '<li class="size">' + val.length + '</li>';
            sHtml += '<li class="create">' + ShowFormatDate(val.DateCreate) + '</li>';
            sHtml += '<li class="modify">' + ShowFormatDate(val.DateEdit) + '</li>';
            /**/
            sHtml += '<li class="TitleActive">';
            if (val.type == "folder") {
                sHtml += '<img src="/images/icon/zipIcon.png" title="zip folder" />';
                sHtml += '<img src="/images/icon/folder_rename.png" title="rename" />';
                sHtml += '<img src="/images/icon/folder_move.png" title="move folder to other folder" />';
                sHtml += '<img src="/images/icon/folder_download.png" title="zip and download folder" />';
                sHtml += '<img src="/images/icon/folder_option.png" title="option" />';
                sHtml += '<img src="/images/icon/folder_delete.png" title="delete forder" />';
            }
            else {
                sHtml += '<img src="/images/icon/file_rename.png" title="rename" />';
                sHtml += '<img src="/images/icon/file-move.png" title="move" />';
                sHtml += '<img src="/images/icon/file_edit.png" title="edit text" />';
                sHtml += '<img src="/images/icon/file_download.png" title="download file" />';
                sHtml += '<img src="/images/icon/file_delete.png" title="delete file" />';
            }
            sHtml += '</li>';
            /**/
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
            //"Zip": { name: "Zip", icon: "Zip", cssover: "popup-button", href: "#popup_confirm" },
            "Rename": { name: "Rename", icon: "RenameFolder", cssover: "popup-button", href: "#popup_Rename" },
            "Move": { name: "Move", icon: "MoveFolder" },
            "Delete": { name: "Delete", icon: "DeleteFolder", cssover: "popup-button", href: "#popup_confirm" },
            //"Download": { name: "Download", icon: "DownloadFolder" },
            "line": "---------",
            "Setting": { name: "Setting", icon: "SettingFolder" }
        }
    });
});

function EventContextFolder(KeyEvent, id) {
    var obj = getItemList(id);
    $('#HD_ID').val(id);
    switch (KeyEvent) {
        case "Open":
            ctFolderOpen(obj);
            break;
        //        case "Zip": 
        //            ctFolderZip(obj); 
        //            break; 
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
        case "Setting":
            ctFolderSetting(obj);
            break;
        default:
    }
}

function ctFolderOpen(obj) {
    GetList(obj.path);
}
//function ctFolderZip(obj) {
//    
//}
function ctFolderRename(obj) {
    $('#Extension').text('');
    $('#txtFileName').val(obj.name);
}
function ctMove(obj) {
    $('#fileMove').text(obj.name);
    $('#BT_Show_Move').click();
}
function ctDelete(obj) {
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
        selector: '#ListItem .itemfile .name',
        callback: function (key, options) {
            var id = $(this).parent().attr('id');
            EventContextFile(key, id);
        },
        items: {
            "Download": { name: "Download", icon: "DownloadFile" },
            "Rename": { name: "Rename", icon: "RenameFile", cssover: "popup-button", href: "#popup_Rename" },
            "Move": { name: "Move", icon: "MoveFile" },
            "Delete": { name: "Delete", icon: "DeleteFile", cssover: "popup-button", href: "#popup_confirm" }
        }
    });
});

function EventContextFile(KeyEvent, id) {
    var obj = getItemList(id);
    $('#HD_ID').val(id);
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
        default:
    }
}

function ctFileRename(obj) {
    $('#Extension').text('.' + obj.type);
    $('#txtFileName').val(getOnlynameFile(obj.name));
}
function ctFileDownload(obj) {
    window.open("/ajax/command.ashx?cmd=download&id=" + obj.id);
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
            if (sList == '')
                sList = $(this).parent().parent().attr('id');
            else {
                sList += ';' + $(this).parent().parent().attr('id');
            }
        }
    });
    if (sList != '')
    //$('#BT_Call_Delete #BT_Call_Move').button("disable");
        $('#BT_Call_Delete').button("enable");
    else {
        $('#BT_Call_Delete').button("disable");
    }
    $('#HD_ListID').val(sList);
}
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
$(document).ready(function () {
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

    $('#BT_Show_Move').click(function () {
        $('#txtFolderMove').val('');
        //if ($('#treeview').html() == 0) {
        $.ajax({
            url: '/ajax/command.ashx?cmd=treeview',
            success: function (data) {
                $('#treeview').html(data);
                BuildTreeview();
            }
        });
        //}
    });

    $('.popup-button').showPopup({
        top: 200, //khoảng cách popup cách so với phía trên
        closeButton: ".close_popup, .bt_cancel", //khai báo nút close cho popup
        scroll: false, //cho phép scroll khi mở popup, mặc định là không cho phép
        //outClose: false, //click ra ngoài là close
        onClose: function () {
            $('#HD_ID').val('');
        }
    });

    $('.popup').draggable({ handle: ".popup-header" });

    $(".button").button();
});
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function BuildTreeview() {
    $("#treeview").treeview({
        animated: "fast",
        collapsed: true,
        unique: true,
        //persist: "cookie",
        toggle: function () {
            window.console && console.log("%o was toggled", this);
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
        'formData': { 'fd': $('#HD_CurrentFolder').val(), 'vkl': 'vcc' },
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
        var oInfo = getItemList($('#HD_ID').val());
        $('#txtStatusRename').text('processing...');
        $.getJSON('/ajax/command.ashx?cmd=rename&id=' + $('#HD_ID').val() + '&newname=' + $('#txtFileName').val()
                    + '&type=' + oInfo.type + '&isFile=' + oInfo.isFile + '&format=json&jsoncallback=?',
            function (data) {
                if (data != null) {
                    if (data.error !=null) {
                        $('#txtStatusRename').text(data.error);
                        return;
                    }
                    else {
                        $('#txtStatusRename').text('rename ok!');
                    }
                }
                else {
                    $('#txtStatusRename').text('Not connection!');
                }
            }
        );
    });
});
///////////////////delete//////////////////////

///////////////////move//////////////////////

///////////////////setting//////////////////////