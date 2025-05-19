
// 多语言支持
window._R = F.getResource;
// 初始化多语言资源
F.setResources({
    "TEST": "測試"
   
});

// 表格列 - 用户状态
function renderStatus(value, params) {
    value = parseInt(value, 10);

    var badgeType, badgeText;
    switch (value) {
        case 1:
            badgeType = 'success';
            badgeText = '优秀';
            break;
        case 2:
            badgeType = 'processing';
            badgeText = '良好';
            break;
        case 3:
            badgeType = 'warning';
            badgeText = '补考';
            break;
        case 4:
            badgeType = 'error';
            badgeText = '重修';
            break;
    }

    // 添加动画效果
    var badgeEl = $('<div>').addClass('f-badge');
    if (badgeType === 'success') {
        badgeEl.addClass('f-badge-animation-processing');
    } else if (badgeType === 'error') {
        badgeEl.addClass('f-badge-animation-fade');
    }

    return $('<div>')
        .addClass('f-badge-tag')
        .addClass('f-badge-type-' + badgeType)
        .append(badgeEl)
        .append($('<span>').text(badgeText));
}

// 表格列 - 用户基本信息
function renderUserProfile(value, params) {
    var rowAttrs = params.rowData.attrs;
    var desc = renderGender(rowAttrs['Gender']) + ' · 入学年份：' + rowAttrs['EntranceYear'];

    return $('<div>')
        .addClass('user-profile')
        .append($('<div>').addClass('avatar')
            .append($('<img>').attr('src', rowAttrs['Avatar'])))
        .append($('<div>').addClass('name-desc')
            .append($('<div>').addClass('name').text(rowAttrs['Name']))
            .append($('<div>').addClass('desc').text(desc)));
}

// 表格列 - 评分
function renderRate1(value) {
    return F.rateHtml(value, {
        readonly: true,
        iconFont: 'f-iconfont-thumbs-up'
    });
}



// 文本 - 性别
function renderGender(value, params) {
    return (value == 1 || value === '男') ? _R('Male') : _R('Female');
}

function renderOPNO(value, params) {
    return (value == 1 || value === '測試') ? _R('TEST') : _R('TEST');
}

// 超链接标签 - 所学专业
function renderMajor(value, params) {
    var encodedValue = F.htmlEncode(value);
    return $('<a>', {
        'target': '_blank',
        'href': 'http://gsa.ustc.edu.cn/search?q=' + F.urlEncode(value),
        'data-qtip': encodedValue,
        'html': encodedValue
    });
}

// 图片标签 - 分组
function renderGroup(value, params) {
    return $('<img>', {
        'class': 'f-grid-imagefield',
        'alt': 'Group',
        'src': F.baseUrl + 'res/images/16/' + value + '.png'
    });
}

// HTML - 行扩展列
function renderExpander(value, params) {
    var rowData = params.rowData;
    return $('<div>').addClass('expander')
        .append($('<p>').html('<strong>' + _R('NamePrefix') + '</strong>' + F.htmlEncode(rowData.values['Name'])))
        .append($('<p>').html('<strong>' + _R('IntroPrefix') + '</strong>' + value));
}

// 超链接标签 - 删除图标
function renderDeleteAction(value, params) {
    return $('<a>').addClass('action-btn delete').attr('href', 'javascript:;')
        .append($('<img>').addClass('f-grid-cell-icon').attr('src', F.baseUrl + 'res/icon/delete.png').attr('alt', 'Delete'));
}


// 公共方法 - 显示通知框
function showNotify(content) {
    // 消息正文可能会比较长，所以不显示前面的图标（messageIcon: ''）
    F.notify({
        message: content,
        target: '_top',
        header: false,
        messageIcon: '',
        positionX: 'center',
        positionY: 'top'
    });
}

// 公共方法 - 通过消息框展示表格选中的行
function notifySelectedRows(gridId) {
    var grid = F(gridId);

    if (!grid.hasSelection()) {
        F.alert(_R('NoSelectionMessage'));
        return;
    }

    var genderColumn = grid.getColumn('Gender');
    var majorColumn = grid.getColumn('Major');

    var table = $('<table>').addClass('result');
    var tr = $('<tr>').appendTo(table);
    if (grid.idField) {
        tr.append('<th>' + _R('GridRowId') + '</th>');
    }
    if (grid.textField) {
        tr.append('<th>' + _R('GridRowText') + '</th>');
    }
    if (genderColumn) {
        tr.append('<th>' + _R('GridGender') + '</th>');
    }
    if (majorColumn) {
        tr.append('<th>' + _R('GridMajor') + '</th>');
    }

    $.each(grid.getSelectedRows(true), function (index, row) {
        tr = $('<tr>').appendTo(table);
        if (grid.idField) {
            tr.append($('<td>').text(row.id));
        }
        if (grid.textField) {
            tr.append($('<td>').text(row.text));
        }
        if (genderColumn) {
            var genderValue = row.values['Gender'];
            // 兼容单元格值为HTML片段的情况（FineUIPro） 
            if (F.product == 'FineUIPro') {
                genderValue = $(genderValue).text();
            } else {
                genderValue = genderValue == 1 ? _R('Male') : _R('Female');
            }
            tr.append($('<td>').text(genderValue));
        }
        if (majorColumn) {
            var majorValue = row.values['Major'];
            // 兼容单元格值为HTML片段的情况（FineUIPro） 
            if (F.product == 'FineUIPro') {
                majorValue = $(majorValue).text();
            }
            tr.append($('<td>').text(majorValue));
        }
    });

    showNotify('<raw>' + table[0].outerHTML + '</raw>');
}

