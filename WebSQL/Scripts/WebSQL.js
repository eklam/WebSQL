/// <reference path="_references.js" />

var treeData;
var autoCompleteOpen = false;
var data = null;
var dataArray = null;
var arrCounter = 0;
var refreshInProgress = false;
var refreshQueued = false;

function Login(server, username, password)
{
    $('#btnLogin').prop('disabled', true);
    $('#btnLogin').html('Please wait...');
    $.post('../../Home/LoginToServer/', { ServerName: server, UserName: username, Password: password }, function (response) {
        if (response.IsSuccess)
        {
            $('#myModal').modal('hide');
            RenderPageAfterLogin(response);
        }
        else
        {
            alert(response.ExceptionText);
        }
        $('#btnLogin').prop('disabled', false);
        $('#btnLogin').html('Login');
    });

}

function RenderPageAfterLogin(response) {
    $('#objTree').jstree({
        json_data: {
            data: eval(response.AdditionalData.JSTree)
        },
        plugins: ['themes', 'json_data'],
        themes: { theme: 'classic' }
    }).css('position', 'fixed').css('height', '90%').css('overflow', 'scroll').css('width', '290px');
    treeData = response.AdditionalData.ServerInfo;

    $('#txtDatabase').val('master');
    DisableDatabaseBox(true);
    StartSignalR();
}

function StartSignalR() {
    // Start the connection.
    var tsignalr = $.connection.myConnection;
    $.connection.hub.start().done(function () {
        
        //tNotification.server.addStaffIdToCohort(staffId());
        //  tNotification.server.registerUser();
    });
    /// end test notification
    tsignalr.client.RefreshResults = function (jsonResult) {
       
        var response = jsonResult.Data;
        if (data == null)
        {
            $('#statusBar').html('Loading initial grid...');
            data = response.Data;
            dataArray = new Array(10000);
            arrCounter = 0;
            dataArray[arrCounter] = data;
            var scope = angular.element($('#resultsGrid')).scope();
            scope.myData = eval(response.Data);
            scope.myCols = eval(response.ColModel);
       
           scope.$apply();

        }
        else 
        {

            arrCounter++;
            dataArray[arrCounter] = response.Data;
            setTimeout(function () {
                $('#statusBar').html('Grid has ' + ((arrCounter + 1) * 1000) + ' rows.');
            }, 1000);
            //var scope = angular.element($('#resultsGrid')).scope();
            //scope.myData = eval(response.Data);
            //scope.$apply();
            //data = data.substr(0, data.length - 1) + ',' + response.Data.substr(1, response.Data.length - 1);
            //$('#tblResults').jqGrid('clearGridData');
            //if (!refreshQueued) {
            //    if (refreshInProgress)
            //        refreshQueued = true;
            if (response.IsLastRecordSet)
            {
                //    setTimeout(function () {
                refreshInProgress = true;
                var sCompile = new Date();
                $('#statusBar').html('Loading grid...');
                var tdata = dataArray[0];
                // var arrCounterSnapshot = arrCounter;
                for (var dai = 1; dai <= arrCounter; dai++)
                    tdata = tdata.substr(0, tdata.length - 1) + ',' + dataArray[dai].substr(1, dataArray[dai].length - 1);
                var scope = angular.element($('#resultsGrid')).scope();
                scope.myData = eval(tdata);
                scope.$apply();
                var fCompile = new Date();
                var elapsedTime = new Date();
                elapsedTime.setTime(fCompile.getTime() - sCompile.getTime());
                $('#statusBar').html('Grid loaded in ' + elapsedTime.getSeconds() + ' secs.');
                refreshInProgress = false;
                refreshQueued = false;
                //    }, 5000);
            
            }
        }

        
    }
}



function RefreshResults(json)
{
    alert(json);
}

function DisableDatabaseBox(doDisable) {
    if (doDisable) {
        $('#txtDatabase').attr('disabled', 'disabled');
        $('#btnChangeDb').show();
        $('#btnSaveDb').hide();
    }
    else
    {
        $('#txtDatabase').removeAttr('disabled');
        $('#btnChangeDb').hide();
        $('#btnSaveDb').show();
    }

}


function LoginAuth() {
    $('#btnExecute').html('Please wait...');
    $.post('../../Home/LoginToServerAuth/', function (response) {
        if (response.IsSuccess) {
            RenderPageAfterLogin(response);
        }
        else {
            alert(response.ExceptionText);
        }
        $('#btnExecute').html('Execute');
    });
}
function clickDefaultButton(e, buttonid) {
    var evt = e ? e : window.event;
    var bt = document.getElementById(buttonid);
    if (bt) {
        if (evt.keyCode == 13) {
            bt.click();
            return false;
        }
    }
}
$(document).ready(function () {
    $('#txtPassword').keydown(function (e) {
        clickDefaultButton(e, 'btnLogin');
    });

    $('#txtDatabase').keydown(function (e) {
        clickDefaultButton(e, 'btnSaveDb');
    });
    $('#txtPassword').focus();
    
    $('#btnExecute').click(function () {
        var sql = $('#txtSQL').val();
        var database = $('#txtDatabase').val();
        data = null;
        // $('#tblResults').jqGrid('clearGridData');
        $.post('../../Home/RunSQL2/', { Database: database, SQL: sql }, function (response) {
            
          

        });
    });



    $('#btnChangeDb').click(function () {
        DisableDatabaseBox(false);
        $('#txtDatabase').focus();
    });

    $('#btnLogOff').click(function () {
        $.post('../../Home/LogOff/', {}, function (response) {
            if (response.IsSuccess) {
                window.location.reload(true);
            }
            else {
                alert('Error logging off.');
            }
        });
    });
     
    $('#btnSaveDb').click(function () {

        DisableDatabaseBox(true);
    });

    $('#txtSQL')
      // don't navigate away from the field on tab when selecting an item
      .bind('keydown', function (event) {
          if (event.keyCode === $.ui.keyCode.TAB &&
              $(this).data("ui-autocomplete").menu.active) {
              event.preventDefault();
          }
          else if (event.keyCode == 190) // periods
          {
              $('#txtSQL').trigger('keydown.autocomplete');
          }
      })
        .bind('autocompleteopen', function (event, ui) { autoCompleteOpen = true; })
        .bind('autocompleteclose', function (event, ui) { autoCompleteOpen = false; } )
      .autocomplete({
          autoFocus: true,
          source: function (request, response) {
              response(GetAutoComplete(request.term));
              
              
          },
          search: function () {
              // custom minLength
              //var term = extractLast(this.value);
              //if (term.length < 2) {
              //    return false;
              //}
          },
          focus: function () {
              // prevent value inserted on focus
              return false;
          },
          select: function (event, ui) {
              var caretPos = $('#txtSQL').caret();
              var fulltext = this.value;
              var termstocaret = split(this.value.substr(0, caretPos));
              // remove the current input
              termstocaret.pop();
              // add the selected item
              termstocaret.push(ui.item.value);
              // add placeholder to get the comma-and-space at the end
              termstocaret.push('');
              var textToCaret = termstocaret.join(' ');
              if (textToCaret.length > 0 && textToCaret[textToCaret.length - 1] == ' ')
                  textToCaret = textToCaret.substr(0, textToCaret.length - 1);
              var val = textToCaret + fulltext.substr(caretPos);
              //if (val.length > 0 && val[val.length - 1] != ' ')
              //    val = val + ' ';
              this.value = val.substring(0, val.length);
              $('#txtSQL').caret(textToCaret.length);
              return false;
          }
      });
});

