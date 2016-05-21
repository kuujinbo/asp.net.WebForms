<%@ Page Language="C#" AutoEventWireup="false" codefile="DatatablesServer.aspx.cs" Inherits="ControlExamples.DatatablesServer" %>
<asp:content id='content1' contentplaceholderid='PageContent' runat='server'>

<h1>DataTables Test</h1>

<div style='margin:10px 0;'>
    <button class="btn btn-success" value='create' data-create-url='/approve'>Create</button>
    <button class="btn btn-primary" value='rollover' data-batch-url='/rollover'>Rollover</button>
    <button class="btn btn-success" value='approve' data-batch-url='/approve'>Approve</button>
    <button class="btn btn-danger" value='disapprove' data-batch-url='/disapprove'>Disapprove</button>
</div>

<table id="example" class="display table">
<thead><tr>
<th style='text-align: center !important;padding:4px !important'><input id="check-all" type="checkbox" value="1" /></th>
<th>Name</th>
<th>Position</th>
<th>Office</th>
<th>Extension</th>
<th >Start date</th>
<th>Salary</th>
<th></th>
</tr></thead>
 
<tfoot><tr>
<th></th>
<th></th>
<th></th>
<th></th>
<th></th>
<th></th>
<th></th>
<th></th>
</tr></tfoot>
</table>

<style type="text/css">
tfoot { display: table-header-group;}
tfoot th { margin:0 !important; padding:2px 4px !important;}
.dataTable { width:100% !important; }
/*table.dataTable,
table.dataTable td {
  -webkit-box-sizing: content-box;
  -moz-box-sizing: content-box;
  box-sizing: content-box;
}*/
.pointer {cursor: pointer; }
.green {color: green; }
.red {color: red; }
/* bootstrap override */ 
.dataTable > tbody > tr > td, .table > tbody > tr > th, .table > tfoot > tr > td, .table > tfoot > tr > th, .table > thead > tr > td, .table > thead > tr > th {
    padding: 5px !important;
}
.link-icons { 
    margin-right: 4px !important;
    cursor: pointer;
}
.search-icons {
    background:#f1c860;
    border:1px solid #eaeaea;
    font-size:1.2em !important;
    text-align:center !important; 
    vertical-align:middle !important;
    /*font-weight:bold;*/
    padding: 4px !important;
    margin-right: 4px !important;
    cursor: pointer;
}

.dataTables_processing {
    position: absolute !important;
    top: 40% !important;
    z-index: 9999 !important;
}
h1.dataTablesLoading
{
    background-color:#fff !important;
    padding: 8px 76px !important;
    border: 2px solid #eaeaea;
}
.spin-infinite {
    -webkit-animation: spin-infinite .8s infinite linear;
    -moz-animation: spin-infinite 1s infinite linear;
    -o-animation: spin-infinite 1s infinite linear;
    animation: spin-infinite 1s infinite linear;
}

@-moz-keyframes spin-infinite {
  from {-moz-transform: rotate(0deg);}  
  to {-moz-transform: rotate(360deg);}
}

@-webkit-keyframes spin-infinite {
  from {-webkit-transform: rotate(0deg);}
  to { -webkit-transform: rotate(360deg);}
}

@keyframes spin-infinite {
  from {transform: rotate(0deg);}
  to {transform: rotate(360deg);}
}

</style>

<script type="text/ecmascript">
document.onkeypress = function (e) {
    if ((e.which === 13) && (e.target.type === 'text')) { return false; }
};

var tableId = '#example';
var checkAllSelector = '#check-all';
var checkboxSelector = 'tbody input[type="checkbox"]';
var checkboxChecked = 'input[type="checkbox"]:checked';
var checkboxUnchecked = 'input[type="checkbox"]:not(:checked)';
var dataTablesLoading = "<h1 class='dataTablesLoading'>Loading data <span class='glyphicon glyphicon-refresh spin-infinite' /></h1>"

var tableCallbacks = function() {
    return {
        clearCheckAll: function() {
            // ajax call only updates tbody
            document.querySelector(checkAllSelector).checked = false;
        },
        getSelectedRowIds: function(table) {
            var selectedIds = [];
            table.rows().every(function (rowIdx, tableLoop, rowLoop) {
                var cb = this.node().querySelector(checkboxChecked);
                if (cb !== null && cb.checked) {
                    selectedIds.push(this.data()[0]);
                }
            });
            return selectedIds;
        },
        buttons: function(e) {
            e.preventDefault();
            var ids = tableCallbacks.getSelectedRowIds(this.table);
            var msg = 'event handler for ['
                + this.value
                + '] button'

            // TODO: add XHR or redirect to batch callback
            if (ids.length > 0) {
                if (this.dataset.batchUrl) {
                    msg += ' with record IDs:\n'
                        + ids + '\nwill be sent to URL: ' + this.dataset.batchUrl;
                } else if (this.dataset.createUrl) {
                    msg += ' with record IDs:\n'
                        + ids + '\nwill be sent to URL: ' + this.dataset.batchUrl;
                }
            } else {
                msg += '\nNO ROWS SELECTED\n'
            }
            alert(msg);

            return false;
        },
        checkAll: function(e) {
            if (this.checked) {
                var nodes = document.querySelectorAll(checkboxUnchecked);
                for (i = 0; i < nodes.length; ++i) nodes[i].click();
            } else {
                var nodes = document.querySelectorAll(checkboxChecked);
                for (i = 0; i < nodes.length; ++i) nodes[i].click();
            }
        },
        footerSearchFocusin: function (e) {
            var nodes = document.querySelectorAll('input[type=text]');
        },
        textSearchKeyup: function (e) {   // submit on ENTER key
            if (e.which === 13) {
                // SEARCH PARAMS __MUST__ BE EXPLICITLY CLEARED
                this.table.search('').columns().search('');
                // now we can search 
                this.table.column(this.dataset.columnNumber).search(this.value).draw();
                tableCallbacks.clearCheckAll();
            }
        },
        tableClick: function(e) {
            var target = e.target;
            // single checkbox click
            if (target.type === 'checkbox') {
                var row = target.parentNode.parentNode;
                if (row && row.tagName.toLowerCase() === 'tr') {
                    if (target.checked) {
                        row.classList.add('selected');
                    } else {
                        row.classList.remove('selected');
                    }
                }
            }
            // edit & delete links
            else if (target.tagName.toLowerCase() === 'span' && target.classList.contains('glyphicon')) {
                var row = target.parentNode.parentNode;
                if (target.classList.contains('glyphicon-remove-circle')) {
                    // TODO: ajax call to delete record from datatbase...
                    alert('CLICKED DELETE with record id: ' + this.table.row(row).data()[0]);

                    // send XHR to request updated view
                    this.table.row(row).remove().draw();
                    tableCallbacks.clearCheckAll();

                }
                else if (target.classList.contains('glyphicon-edit')) {
                    // TODO: redirect to edit view
                    alert('CLICKED EDIT with record id: ' + this.table.row(row).data()[0]);
                }
            }
        },

        addTableListeners: function (table) {
            // action buttons
            var buttons = document.querySelectorAll('button.btn');
            for (i = 0 ; i < buttons.length ; i++) {
                buttons[i].table = table;
                buttons[i].addEventListener('click', tableCallbacks.buttons, false);
            }

            // 'check all' checkbox
            var checkAll = document.querySelector(checkAllSelector);
            checkAll.table = table;
            checkAll.addEventListener('click', tableCallbacks.checkAll, false);

            // datatable clicks
            var tableClick = document.querySelector(tableId);
            tableClick.table = table;
            tableClick.addEventListener('click', tableCallbacks.tableClick, false);

            // inject search icons 
            var footers = document.querySelectorAll(tableId + ' tfoot th');
            footers[footers.length - 1].innerHTML = 
                "<span class='green search-icons glyphicon glyphicon-search'></span>"
                + "<span class='search-icons glyphicon glyphicon-remove'></span>";

            /* inject search textboxes:
               first column checkbox, last column edit & delete icon links */
            for (var i = 1; i < footers.length - 1; i++) {
                footers[i].innerHTML = "<input style='width:100% !important;display: block !important;'"
                    + " data-column-number='" + i + "'"
                    + " class='form-control' type='text' placeholder='Search' />";
            }

            // do search
            var footerSearch = document.querySelector(tableId + ' tfoot');
            footerSearch.addEventListener('focusin', tableCallbacks.footerSearchFocusin, false);
            var textSearch = document.querySelectorAll(tableId + ' tfoot input[type=text]');
            for (i = 0 ; i < textSearch.length ; i++) {
                textSearch[i].table = table;
                textSearch[i].addEventListener('keyup', tableCallbacks.textSearchKeyup, false);
            }
        }
    }
}();


$(document).ready(function() {
    var table = $(tableId).DataTable({
        processing: true,
        serverSide: true,
        searchDelay: 750,
        deferRender: true,
        dom: 'lrtip',
        pagingType: 'full_numbers',
        autoWidth: true,
        order: [[1, 'asc']],
        language: {
            processing: dataTablesLoading,
            paginate: {
                previous: "<span class='glyphicon glyphicon-chevron-left' title='PREVIOUS' />",
                next: "<span class='glyphicon glyphicon-chevron-right'  title='NEXT' />",
                first: "<span class='glyphicon glyphicon-fast-backward' title='FIRST' />",
                last: "<span class='glyphicon glyphicon-fast-forward' title='LAST' />"
            }
        },
        ajax: {
            url: 'DatatablesServer.ashx',
            error: function (jqXHR, responseText, errorThrown) {
                // need to explicitly hide on error
                document.querySelector('div.dataTables_processing').style.display = 'none';
                console.log(errorThrown);
            }
        },
        columnDefs: [{
            targets: 0,
            searchable: false,
            orderable: false,
            width: '2%',
            render: function(data, type, full, meta) { return "<input type='checkbox'>"; }
        },
        {
            targets: 7,
            searchable: false,
            orderable: false,
            width: '10%',
            render: function(data, type, full, meta) {
                return "<span class='link-icons glyphicon glyphicon-edit green'></span>"
                + " <span class='link-icons glyphicon glyphicon-remove-circle red'></span>";
            }
        }]
    });
    tableCallbacks.addTableListeners(table);
});
</script>
</asp:content>