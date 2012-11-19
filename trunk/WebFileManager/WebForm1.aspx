<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="WebFileManager.WebForm1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="Styles/Site.css" rel="stylesheet" type="text/css" />
    <link href="Styles/Styles.css" rel="stylesheet" type="text/css" />
    <script src="/Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <link href="Styles/popup.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.9.1.custom.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery.treeview.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/popup.js" type="text/javascript"></script>
    <script src="Scripts/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>
    <script src="Scripts/jquery.treeview.js" type="text/javascript"></script>
    <style>
        .chan
        {
            background-color: #AAAAAA;
        }
        .le
        {
            background-color: #ffffff;
        }
        .select
        {
            background-color: khaki;
        }
        .over
        {
            background: #DDDDDD;
        }
        .center
        {
            text-align: center;
            margin: 0 auto;
            margin-left: auto;
            margin-right: auto;
            width: 960px;
        }
        .buttonzz
        {
            font-weight: bold;
            color: #FEF4E9;
            border: solid 1px #DA7C0C;
            background: #F78D1D;
            background: -webkit-gradient(linear, left top, left bottom, from(#FAA51A), to(#F47A20));
            background: -moz-linear-gradient(top, #FAA51A, #F47A20);
            filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='#faa51a', endColorstr='#f47a20');
            display: inline-block;
            outline: none;
            cursor: pointer;
            text-align: center;
            text-decoration: none;
            font: 12px/100% Arial, Helvetica, sans-serif;
            padding: .5em 2em .55em;
            text-shadow: 0 1px 1px rgba(0, 0, 0, .3);
            -webkit-border-radius: .5em;
            -moz-border-radius: .5em;
            border-radius: .5em;
            -webkit-box-shadow: 0 1px 2px rgba(0, 0, 0, .2);
            -moz-box-shadow: 0 1px 2px rgba(0,0,0,.2);
            box-shadow: 0 1px 2px rgba(0, 0, 0, .2);
        }
        .folder
        {
            clear: both;
            font-weight: bold;
            color: #01ABFA;
            border: 1px solid #FBCB09;
            background: #FDF5CE;
            display: inline-block;
            outline: none;
            text-align: left;
            text-decoration: none;
            margin: 15px 0;
            padding: 6px 4px;
            text-shadow: 0 1px 1px rgba(0, 0, 0, .3);
            -webkit-box-shadow: 0 1px 2px rgba(0, 0, 0, .2);
            -moz-box-shadow: 0 1px 2px rgba(0,0,0,.2);
            box-shadow: 0 1px 2px rgba(0, 0, 0, .2);
            width: 99%;
        }
    </style>
    <script language="javascript" type="text/javascript">
        function GetList(fd) {
            var url = "/ajax/command.ashx?cmd=getlist&fd=" + fd;
            $.getJSON(url + "&format=json&jsoncallback=?",
                function (result) {
                    $('#ListItem').html('loading...');
                    if (result != null && result.error != null) {
                        alert(result.error);
                    }
                    else {
                        if (result != null && result.length > 0) {
                            var sHtml = '';
                            $.each(result, function (i, val) {
                                if (val.type == "folder") {
                                    sHtml+='<ul class="listfolder">';
                                }
                                else {
                                    sHtml+='<ul class="itemfile">';
                                }
                                sHtml+='<li class="TitleSelect"><input type="checkbox" id="selected_' + i + '" /></li>';
                                sHtml+='<li class="name">' + val.name + '</li>';
                                sHtml+='<li class="size">' + val.length + '</li>';
                                sHtml += '<li class="create">' + ShowFormatDate(val.DateCreate) + '</li>';
                                sHtml += '<li class="modify">' + ShowFormatDate(val.DateEdit) + '</li>';
                                sHtml+='<li class="TitleActive">';
                                if (val.type == "folder") {
                                    sHtml+='<img src="images/icon/zipIcon.png" title="zip folder" />';
                                    sHtml+='<img src="images/icon/folder_rename.png" title="rename" />';
                                    sHtml+='<img src="images/icon/folder_move.png" title="move folder to other folder" />';
                                    sHtml+='<img src="images/icon/folder_download.png" title="zip and download folder" />';
                                    sHtml+='<img src="images/icon/folder_option.png" title="option" />';
                                    sHtml+='<img src="images/icon/folder_delete.png" title="delete forder" />';
                                }
                                else {
                                    sHtml+='<img src="images/icon/file_rename.png" title="rename" />';
                                    sHtml+='<img src="images/icon/file-move.png" title="move" />';
                                    sHtml+='<img src="images/icon/file_edit.png" title="edit text" />';
                                    sHtml+='<img src="images/icon/file_download.png" title="download file" />';
                                    sHtml+='<img src="images/icon/file_delete.png" title="delete file" />';
                                }
                                sHtml+='</li>';
                                sHtml+='</ul>';
                            });
                            $('#ListItem').html(sHtml);
                        }
                    }
                });
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
        
        $(document).ready(function () {
            GetList('');
            //$.getJSON('/ajax/command.ashx?cmd=getlist&format=json&jsoncallback=?', function (data) {
            //    alert(data);
            //});


            $('.popup-button').showPopup({
                top: 200, //khoảng cách popup cách so với phía trên
                closeButton: ".close_popup, .bt_cancel", //khai báo nút close cho popup
                scroll: false, //cho phép scroll khi mở popup, mặc định là không cho phép
                //outClose: false, //click ra ngoài là close
                onClose: function () {
                    //sự kiện cho phép gọi sau khi đóng popup, cho phép chúng ta gọi 1 số sự kiện khi đóng popup, bạn có thể để null ở đây
                }
            });

            $('.popup-button').click(function () { return false; });

            $('.popup').draggable({ handle: ".popup-header" });

            $(".button").button();

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
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="page">
        <div class="header">
            <div class="title">
                <h1>
                    File manager .Net
                </h1>
            </div>
            <div class="clear hideSkiplink">
            </div>
        </div>
        <div class="main">
            <div>
                <input type="submit" name="" value="Delete" id="MainContent_Button1" class="button" />
                <input type="submit" name="" value="New folder" id="MainContent_Button6" class="button" />
                <input type="submit" name="" value="Move" id="MainContent_Button3" class="button" />
                <input type="submit" name="" value="Upload" id="MainContent_Button2" class="button popup-button"
                    href="#popup_Upload" />
                <input type="submit" name="" value="UnZip" id="MainContent_Button4" class="button" />
                <input type="submit" name="" value="Download" id="MainContent_Button5" class="button" />
            </div>
            <div class="folder">
                C:\Users\Admin\Desktop\DJ\
            </div>
            <div style="clear: both">
            </div>
            <div id="MainGrid">
                <ul class="title">
                    <li class="TitleSelect">Select</li>
                    <li class="name">FileName</li>
                    <li class="size">File size</li>
                    <li class="create">Date create</li>
                    <li class="modify">Date modify</li>
                    <li class="TitleActive">Active</li>
                </ul>
                <div id="ListItem" class="listitem">
                    <ul class="listfolder">
                        <li class="TitleSelect">
                            <input type="checkbox" id="selected" /></li>
                        <li class="name">FileName</li>
                        <li class="size">File size</li>
                        <li class="create">Date create</li>
                        <li class="modify">Date modify</li>
                        <li class="TitleActive">
                            <img src="images/icon/zipIcon.png" title="zip folder" />
                            <img src="images/icon/folder_rename.png" title="rename" />
                            <img src="images/icon/folder_move.png" title="move folder to other folder" />
                            <img src="images/icon/folder_download.png" title="zip and download folder" />
                            <img src="images/icon/folder_option.png" title="option" />
                            <img src="images/icon/folder_delete.png" title="delete forder" />
                        </li>
                    </ul>
                    <ul class="itemfile">
                        <li class="TitleSelect">
                            <input type="checkbox" id="Checkbox1" /></li>
                        <li class="name">FileName</li>
                        <li class="size">File size</li>
                        <li class="create">Date create</li>
                        <li class="modify">Date modify</li>
                        <li class="TitleActive">
                            <img src="images/icon/file_rename.png" title="rename" />
                            <img src="images/icon/file-move.png" title="move" />
                            <img src="images/icon/file_edit.png" title="edit text" />
                            <img src="images/icon/file_download.png" title="download file" />
                            <img src="images/icon/file_delete.png" title="delete file" />
                        </li>
                    </ul>
                </div>
            </div>
            <div id="popup_confirm" class="popup">
                <div class="popup-header">
                    <h2>
                        Confirm delete file</h2>
                    <p id="fileDelete">
                        (file name)</p>
                </div>
                <div style="text-align: center; padding: 10px 0px;">
                    <input type="submit" name="ctl00$MainContent$BT_Delete" value="Delete" id="MainContent_BT_Delete"
                        class="button" />
                    <input type="submit" name="ctl00$MainContent$BT_Confirm_Cancel" value="Cancel" onclick="javascript:return false;"
                        id="MainContent_BT_Confirm_Cancel" class="bt_cancel button" />
                </div>
            </div>
            <div id="popup_Rename" class="popup">
                <div class="popup-header">
                    <h2>
                        Rename file</h2>
                    <a class="close_popup" href="javascript:void(0)"></a>
                </div>
                <div class="info_popup">
                    <input type="text" id="txtFileName" />
                </div>
                <div style="text-align: center; padding-bottom: 10px;">
                    <input type="submit" name="ctl00$MainContent$BT_Sublmit" value="Submit" id="MainContent_BT_Sublmit"
                        class="button" />
                    <input type="submit" name="ctl00$MainContent$BT_Cancel" value="Cancel" onclick="javascript:return false;"
                        id="MainContent_BT_Cancel" class="bt_cancel button" />
                </div>
            </div>
            <div id="popup-move" class="popup">
                <div class="popup-header">
                    <h2>
                        Move file</h2>
                    <p>
                        (file name)</p>
                    <p>
                        select folder</p>
                    <a class="close_popup" href="javascript:void(0)"></a>
                </div>
                <div style="height: 250px; overflow-y: auto; padding: 0px 0px 10px 10px; border: 1px solid #CCC">
                    <ul id="treeview" class="filetree">
                        <li><span title="Root" class="open">Root</span><ul>
                            <li><span title="Root\Ape">Ape</span></li><li><span title="Root\DJ">DJ</span></li><li>
                                <span title="Root\dts">dts</span></li><li><span title="Root\nhac trong the">nhac trong
                                    the</span></li><li><span title="Root\PhamTruong-Dance">PhamTruong-Dance</span></li><li>
                                        <span title="Root\Tong Hop">Tong Hop</span></li><li><span title="Root\WAV">WAV</span><ul>
                                            <li><span title="Root\WAV\爱琴海2010DJ嗨曲[APE+CUE]">爱琴海2010DJ嗨曲[APE+CUE]</span></li><li>
                                                <span title="Root\WAV\爱车慢摇英文版[WAV+CUE]">爱车慢摇英文版[WAV+CUE]</span></li></ul>
                                        </li>
                        </ul>
                        </li>
                    </ul>
                </div>
                <div style="padding-top: 10px; text-align: center">
                    <input type="text" id="txtFolderMove" style="width: 380px;" />
                </div>
                <div style="text-align: center; padding: 10px 0px;">
                    <input type="submit" name="ctl00$MainContent$BT_Move" value="Move" id="MainContent_BT_Move"
                        class="button" />
                    <input type="submit" name="ctl00$MainContent$BT_Move_Cancel" value="Cancel" onclick="javascript:return false;"
                        id="MainContent_BT_Move_Cancel" class="bt_cancel button" />
                </div>
            </div>
            <div id="popup_Upload" class="popup">
                <div class="popup-header">
                    <h2>
                        Upload file</h2>
                    <a class="close_popup" href="javascript:void(0)"></a>
                </div>
                <div class="info_popup">
                    <input type="text" id="Text2" />
                </div>
                <div style="text-align: center; padding-bottom: 10px;">
                    <input type="submit" name="ctl00$MainContent$BT_Upload" value="Upload" id="MainContent_BT_Upload"
                        class="button" />
                    <input type="submit" name="ctl00$MainContent$BT_Upload_Cancel" value="Cancel" onclick="javascript:return false;"
                        id="MainContent_BT_Upload_Cancel" class="bt_cancel button" />
                </div>
            </div>
            <input name="ctl00$MainContent$HD_File" type="hidden" id="MainContent_HD_File" />
        </div>
        <div class="clear">
        </div>
    </div>
    <div class="footer">
        chán chết chẳng có cái j
    </div>
    </form>
</body>
</html>
