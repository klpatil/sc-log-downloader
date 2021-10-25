<%@ page language="C#" autoeventwireup="true" codefile="SCLD.aspx.cs" inherits="SCBasics.Tools.SCLD" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" lang="en">

<head runat="server" id="Head1">
    <!-- Required meta tags -->
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.2/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-uWxY/CJNBR+1zjPWmfnSnVxwRheevXITnMqoEIeG1LJrdI0GlVs/9cVSyPYXdcSF" crossorigin="anonymous">

    <title>Sitecore Log Downloader(SCLD)</title>
</head>
<body>

    <!-- Option 1: Bootstrap Bundle with Popper -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.1.2/dist/js/bootstrap.bundle.min.js" integrity="sha384-kQtW33rZJAHjgefvhyyzcGF3C5TFyBQBA13V1RKPf4uH+bwyzQxZ6CmMZHmNBEfJ" crossorigin="anonymous"></script>


    <form id="form1" runat="server">
        <div class="container-fluid">
            <div class="row">
                <div class="col">
                    <hr />
                    <div class="alert alert-info" role="alert">
                        <asp:Label ID="lblMessage" runat="server" Visible="false" />
                    </div>
                    <hr />

                </div>
            </div>
            <div class="row">
                <div class="col-8">
                    <div class="alert alert-primary" role="alert">
                        To download log file(s) from specific instance log folder. Please select Instance log folder from here : =>
                    </div>
                </div>

                <div class="col-2">
                    <asp:DropDownList ID="ddllogFileType" runat="server" OnSelectedIndexChanged="logFileType_SelectedIndexChanged"
                        AutoPostBack="True">
                    </asp:DropDownList>
                     <asp:Button ID="btnLoadCurrentInstance" runat="server" Text="Auto Select Current Instance"
                        OnClick="btnLoadCurrentInstance_Click"
                        CssClass="btn btn-link" Visible="true" />
                </div>
                <div class="col-2">
                   <asp:Button ID="btnDownloadZip" runat="server" Text="Download Selected File(s)" OnClick="btnDownload_Click"
                        CssClass="btn btn-primary" Visible="true" />
                </div>
                <hr />
            </div>

            <div class="row">
                <div class="col">
                    <asp:Panel ID="mainPanel" runat="server" Width="100%" ScrollBars="Auto" Height="600px">
                        <asp:Table ID="tblData" runat="server" BorderWidth="1" CssClass="table table-striped">
                        </asp:Table>
                    </asp:Panel>
                </div>
            </div>
          
        </div>
    </form>
</body>
</html>
