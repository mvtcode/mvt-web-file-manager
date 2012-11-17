<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="WebFileManager._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <link href="Styles/popup.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.9.1.custom.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery.treeview.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/popup.js" type="text/javascript"></script>
    <script src="Scripts/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>
    <%--<script src="Scripts/jquery.cookie.js" type="text/javascript"></script>--%>
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
        .m4a
        {
            text-indent: 10px;
            background: url('') left no-repeat;
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
        $(document).ready(function () {
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

            $("tr td input[type=checkbox]").change(function () {
                if ($(this).is(':checked')) {
                    $(this).parent("td").parent("tr").addClass("select");
                } else {
                    $(this).parent("td").parent("tr").removeClass("select");
                }
            });

            //            $('td').filter(function () {
            //                //return (this.text().lastIndexOf('.m4a')>0);
            //                alert(this.text());
            //            }).addClass('m4a');
            $('tr').find('td:eq(1)').addClass('m4a');

            //////////////////////

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

            function SetUpdate(sFile) {
                $('#MainContent_HD_File').val(sFile);
                return false;
            }

            function SetDelete(sFile) {
                $('#MainContent_HD_File').val(sFile);
                $('#fileDelete').text(sFile);
                alert(sFile);
                alert($('#MainContent_HD_File').val());
                return false;
            }

            function SetMove(sFile) {
                $('#MainContent_HD_File').val(sFile);
                return false;
            }

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
                //alert(this.toString());
                //alert($(this).attr('title'));
                $('#txtFolderMove').val($(this).attr('title'));
            });

            $('#treeview').find("span").attr('style', "cursor:pointer;cursor: hand").addClass("folders");

            //$('#treeview').find("li").find("span").bind("onclick", function () { return false; });
        });
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div>
        <asp:Button ID="Button1" runat="server" Text="Delete" CssClass="button" />
        <asp:Button ID="Button6" runat="server" Text="New folder" CssClass="button" />
        <asp:Button ID="Button3" runat="server" Text="Move" CssClass="button" />
        <asp:Button ID="Button2" runat="server" Text="Upload" CssClass="button popup-button"
            href="#popup_Upload" />
        <asp:Button ID="Button4" runat="server" Text="UnZip" CssClass="button" />
        <asp:Button ID="Button5" runat="server" Text="Download" CssClass="button" />
    </div>
    <div class="folder">
        C:\Users\Admin\Desktop\DJ\
    </div>
    <div style="clear: both">
    </div>
    <asp:GridView ID="GV_File" DataKeyNames="FullName" runat="server" AutoGenerateColumns="False"
        PageSize="20" HeaderStyle-Font-Size="12px" RowStyle-Font-Size="12px" EnableTheming="False"
        EmptyDataText="Không tìm thấy bản ghi nào" CssClass="tblList" Width="100%" AllowPaging="True"
        OnPageIndexChanging="GV_File_PageIndexChanging" OnRowCommand="GV_File_RowCommand"
        OnRowDataBound="GV_File_RowDataBound">
        <PagerStyle VerticalAlign="Middle" />
        <HeaderStyle HorizontalAlign="Center" />
        <RowStyle Font-Size="12px"></RowStyle>
        <Columns>
            <asp:TemplateField HeaderText="Select">
                <ItemTemplate>
                    <asp:CheckBox ID="CheckBox1" runat="server" />
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="5%" />
            </asp:TemplateField>
            <asp:BoundField HeaderText="FileName" DataField="name">
                <ItemStyle Width="50%" />
            </asp:BoundField>
            <asp:BoundField DataField="length" HeaderText="File size">
                <ItemStyle Width="10%" />
            </asp:BoundField>
            <asp:BoundField DataField="DateCreate" HeaderText="Date create">
                <ItemStyle Width="10%" />
            </asp:BoundField>
            <asp:BoundField DataField="DateEdit" HeaderText="Date modify">
                <ItemStyle Width="10%" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Thao tác">
                <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                    Font-Underline="False" HorizontalAlign="Center" Width="15%" />
                <ItemTemplate>
                    <asp:ImageButton ID="imgbtDelete" runat="server" CommandName="Delete" ImageUrl="/images/action_delete.gif"
                        CssClass="popup-button1" href="#popup_confirm" />
                    <asp:ImageButton ID="imgbtEdit" runat="server" CommandName="Edit" CommandArgument='<%# Eval("FullName") %>'
                        ImageUrl="/images/edit.gif" CssClass="popup-button" href="#popup_Rename" />
                    <asp:ImageButton ID="imgbtMove" runat="server" CommandName="move" ImageUrl="/images/action_delete.gif"
                        CssClass="popup-button" href="#popup-move" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <div id="popup_confirm" class="popup">
        <div class="popup-header">
            <h2>
                Confirm delete file</h2>
            <p id="fileDelete">
                (file name)</p>
        </div>
        <div style="text-align: center; padding: 10px 0px;">
            <asp:Button ID="BT_Delete" runat="server" Text="Delete" CssClass="button" />
            <asp:Button ID="BT_Confirm_Cancel" runat="server" Text="Cancel" CssClass="bt_cancel button"
                OnClientClick="javascript:return false;" />
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
            <asp:Button ID="BT_Sublmit" runat="server" Text="Submit" CssClass="button" />
            <asp:Button ID="BT_Cancel" runat="server" Text="Cancel" CssClass="bt_cancel button"
                OnClientClick="javascript:return false;" />
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
                <%=sTree%>
                <%--<li><span title="123">Item 2</span>
                    <ul>
                        <li><span>Item 2.0</span>
                            <ul>
                                <li><span>Item 2.0.0</span>
                                    <ul>
                                        <li><span>Item 2.0.0.0</span></li>
                                        <li><span>Item 2.0.0.1</span></li>
                                    </ul>
                                </li>
                            </ul>
                        </li>
                        <li><span>Item 2.1</span>
                            <ul>
                                <li><span>Item 2.1.0</span>
                                    <ul>
                                        <li><span>Item 2.1.0.0</span></li>
                                    </ul>
                                </li>
                                <li><span>Item 2.1.1</span>
                                    <ul>
                                        <li><span>Item 2.1.1.0</span></li>
                                        <li><span>Item 2.1.1.1</span></li>
                                        <li><span>Item 2.1.1.2</span></li>
                                    </ul>
                                </li>
                                <li><span>Item 2.1.2</span>
                                    <ul>
                                        <li><span>Item 2.1.2.0</span></li>
                                        <li><span>Item 2.1.2.1</span></li>
                                        <li><span>Item 2.1.2.2</span></li>
                                    </ul>
                                </li>
                            </ul>
                        </li>
                    </ul>
                </li>--%>
            </ul>
        </div>
    <%--<div style="height: 250px;overflow-Y: auto;padding: 0px 0px 10px 10px;border: 1px solid #CCC">
            <asp:TreeView ID="TreeView1" runat="server" ExpandDepth="2" ShowLines="True">
                <Nodes>
                    <asp:TreeNode Text="New Node" Value="New Node">
                        <asp:TreeNode Text="New Node" Value="New Node">
                            <asp:TreeNode Text="New Node" Value="New Node" Selected="True"></asp:TreeNode>
                            <asp:TreeNode Text="New Node" Value="New Node"></asp:TreeNode>
                        </asp:TreeNode>
                        <asp:TreeNode Text="New Node" Value="New Node"></asp:TreeNode>
                    </asp:TreeNode>
                    <asp:TreeNode Text="New Node" Value="New Node">
                        <asp:TreeNode Text="New Node" Value="New Node"></asp:TreeNode>
                    </asp:TreeNode>
                    <asp:TreeNode Text="New Node" Value="New Node">
                        <asp:TreeNode Text="New Node" Value="New Node">
                            <asp:TreeNode Text="New Node" Value="New Node">
                                <asp:TreeNode Text="New Node" Value="New Node"></asp:TreeNode>
                                <asp:TreeNode Text="New Node" Value="New Node"></asp:TreeNode>
                            </asp:TreeNode>
                        </asp:TreeNode>
                        <asp:TreeNode Text="New Node" Value="New Node"></asp:TreeNode>
                    </asp:TreeNode>
                </Nodes>
                <NodeStyle ImageUrl="~/images/action_check.gif" />
            </asp:TreeView>
        </div>--%>
    <div style="padding-top: 10px; text-align: center">
        <input type="text" id="txtFolderMove" style="width: 380px;" />
    </div>
    <div style="text-align: center; padding: 10px 0px;">
        <asp:Button ID="BT_Move" runat="server" Text="Move" CssClass="button" />
        <asp:Button ID="BT_Move_Cancel" runat="server" Text="Cancel" CssClass="bt_cancel button"
            OnClientClick="javascript:return false;" />
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
            <asp:Button ID="BT_Upload" runat="server" Text="Upload" CssClass="button" />
            <asp:Button ID="BT_Upload_Cancel" runat="server" Text="Cancel" CssClass="bt_cancel button"
                OnClientClick="javascript:return false;" />
        </div>
    </div>
    <input type="hidden" id="HD_File" runat="server" value="" />
</asp:Content>
