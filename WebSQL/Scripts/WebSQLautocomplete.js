/// <reference path="_references.js" />
var collection = [
  '[no suggestions]'
];

function GetAutoComplete(term) {
    var caretPos = $('#txtSQL').caret();
    
    var lastKeyword = GetLastKeyword(term, caretPos);
    var lastword = ExtractLastWord(term, caretPos);
    var currentWord = GetCurrentWord(term, caretPos);

    
    if (lastword == 'from')
        return ProcessTableKeyword(term, currentWord);
    else if (lastword == 'join')
        return ProcessTableKeyword(term, currentWord);
    else if (lastKeyword == 'on' || lastKeyword == 'where' || lastKeyword == 'select') {
        var indexOfDot = currentWord.indexOf('.');
        if (indexOfDot != -1) {
            var wordBeforeDot = currentWord.substr(0, indexOfDot);
            return ProcessColumns(term, wordBeforeDot, currentWord);
        }
        else {
            return ProcessShortcut(term, currentWord);
        }
    }
    else
        return ProcessFilter(term, collection, lastword);
    
}

function GetLastKeyword(term, caretPos) {
    var arr = GetArr(term.substr(0, caretPos));
    
    var currentWord = '';
    do {
        currentWord = arr.pop();
        if (currentWord == null)
            return '';
        switch (currentWord.toLowerCase()) {
            case 'select':
                return currentWord;
            case 'from':
                return currentWord;
            case 'join':
                return currentWord;
            case 'on':
                return currentWord;
            case 'where':
                return currentWord;
        }
    } while (currentWord.length > 0);
    return '';
}

function ProcessColumns(term, shortcutName, currentWord) {
    var shortcuts = GetShortcuts(splitAllWS(term));
    var table = Enumerable.From(shortcuts).Where('$.Shortcut == "' + shortcutName + '"').Select('$.Table').FirstOrDefault(null);
    var cols = FetchColumns($('#txtDatabase').val(), table);
    for (var i = 0; i < cols.length; i++) {
        cols[i] = shortcutName + '.' + cols[i];
    }
    return ProcessFilter(term, cols, currentWord);
}

function FetchColumns(database, tableName)
{
    var db = Enumerable.From(treeData.DatabaseList)
                    .Where('$.DatabaseName == "' + database + '"')
                    .FirstOrDefault(null);
    var cols = new Array();
    if (db == null)
        return cols;
    var tbl = Enumerable.From(db.Tables).Where('$.Name == "' + tableName + '"').FirstOrDefault(null);
    if (tbl == null)
        return cols;
    cols = Enumerable.From(tbl.Columns).ToArray();
    if (cols.length > 0)
        return Enumerable.From(cols).Select('$.Name').ToArray();

    $('#statusBar').html('Fetching columns...');
    $.ajax({
        type: 'POST',
        url: '../../Home/FetchColumns/',
        data: { Database: database, Table: tableName },
        success: function (response) {
            tbl.Columns = response;
            cols = response;
            $('#statusBar').html('Columns refreshed for table ' + tableName);
            if (!autoCompleteOpen)
            {
                $('#txtSQL').autocomplete('search', $('#txtSQL').val());
            }
        },
        dataType: 'json',
        async: true
    });
    
    return Enumerable.From(cols).Select('$.Name').ToArray();
}

function GetLastCharacter(term) {
    if (term.length == 0)
        return '';
    return term.substr(term.length - 1, 1);
}

function ProcessShortcut(term, currentWord) {
    var shortcuts = GetShortcuts( splitAllWS(term));
    var shortcutNames = Enumerable.From(shortcuts).Select('$.Shortcut').ToArray();
    return ProcessFilter(term, shortcutNames, currentWord);

}

function RemoveAllWhiteSpace(str) {
    return str.replace(/\s/g, '');
}

function GetShortcuts(term) {
    var tableList = GetTableList();
    var shortcutList = [];
    for (var i = 0; i < term.length; i++) {
        var termStr = RemoveAllWhiteSpace(term[i]);
        if (Enumerable.From(tableList).Contains(termStr)) {
            // want to make sure we are not at the last item on the list
            if (i + 1 < term.length) {
                var shortcut = RemoveAllWhiteSpace(term[i + 1]);
                shortcutList.push({ Shortcut: shortcut, Table: termStr });
            }
        }
    }
    return shortcutList;
}

function GetTableList() {
    var currentDb = $('#txtDatabase').val();
    var db = Enumerable.From(treeData.DatabaseList)
                    .Where('$.DatabaseName == "' + currentDb + '"')
                    .FirstOrDefault(null);

    var tableList = Enumerable.From(db.Tables).Select('$.Name').ToArray();
    return tableList;
}
function ProcessTableKeyword(term, lastword) {
    
    var tableList = GetTableList();
    return ProcessFilter(term, tableList, lastword);
}

function ProcessFilter(term, array, lastword) {
    // if starting a new word, show array, otherwise, filter based on current input
    if (term.length > 1 && term[term.length - 1] == ' ')
        return array;
    return $.ui.autocomplete.filter(array, lastword);
}

function split(val) {
    //return val.split(/\s/);
    return val.split(' ');
}

function splitAllWS(val) {
    return val.split(/\s/);
    
}

function GetArr(term) {
    return term.match(/\S+/g);
}

function GetCurrentWord(term, caretPos) {
    return GetArr(term.substr(0, caretPos)).pop();
}

function ExtractLastWord(term, caretPos) {
    // return split(term).pop();
    var arr = GetArr(term.substr(0, caretPos));

    if (arr.length < 1)
        return '';

    if (term[term.length - 1] == ' ' || arr.length == 1)
        return GetCurrentWord(term);

    return arr[arr.length - 2];
    
}