function renderMenues() {
    var issueId = viewModelJs.Issue.Id;
    var issue = viewModelJs.Issue
    doLinks(issue);
    if (viewModel.Issue.Status() == "CLOSED") {
        status = 7
    }else if (viewModel.Issue.Status() == "FINISHED") {
        status = 6
    }else if (viewModel.Issue.Status() == "DECIDING") {
        status = 5
    } else if (viewModel.Issue.Status() == "EVALUATING") {
        status = 4
    } else if (viewModel.Issue.Status() == "BRAINSTORMING2") {
        status = 3
    } else if (viewModel.Issue.Status() == "BRAINSTORMING1") {
        status = 2
    } else {
        status = 1
    }

    if (status == 5 || status == 6 || status == 7) {
        var menuElem = document.getElementById("finished-menu")
        menuElem.className = "enabled"
        document.getElementById("anchor-finished").addEventListener('click', function (e) {
            window.location.href = "/Issue/Decision?issueId=" + issueId
        })

        document.getElementById('anchor-criteriafinding').innerText = 'Criteria'
        document.getElementById('anchor-evaluating').innerText = 'Alternatives'
    }
    if (status < 5) {
        document.getElementById('anchor-criteriafinding').innerText = 'Criteria'

        var menuElem = document.getElementById("finished-menu")
        menuElem.className = "disabled"
        $("#anchor-finished").on('click', function (e) {
            e.preventDefault();
        });

        menuElem = document.getElementById("evaluating-menu")
        menuElem.className = "enabled"
        document.getElementById("anchor-evaluating").addEventListener('click', function (e) {
            window.location.href = "/Issue/Evaluation?issueId=" + issueId
        })
    }
    if (status < 4) {
        var menuElem = document.getElementById("evaluating-menu")
        menuElem.className = "disabled"
        $("#anchor-evaluating").on('click', function (e) {
            e.preventDefault();
        });

        menuElem = document.getElementById("criteriarating-menu")
        menuElem.className = "enabled"
        if ($("#anchor-criteriarating").hasClass('disabled')) {
            $("#anchor-criteriarating").removeClass('disabled')
        }

        menuElem = document.getElementById("evaldr-menu")
        if (!$("#anchor-evaluatingdr").hasClass('enabled')) {
            !$("#anchor-evaluatingdr").addClass('enabled')
        }
        if ($("#anchor-evaluatingdr").hasClass('disabled')) {
            $("#anchor-evaluatingdr").removeClass('disabled')
        }
        if (menuElem.className == "disabled") {
            menuElem.className = "enabled";
        }

        document.getElementById("anchor-criteriarating").addEventListener('click', function (e) {
            window.location.href = "/Issue/CriteriaRating?issueId=" + issueId
        })
    }
    if (status < 3) {
        var menuElem = document.getElementById("criteriarating-menu")
        menuElem.className = "disabled"
        $("#anchor-criteriarating").on('click', preventDefaultClick);

        menuElem = document.getElementById("alternatives-menu")
        menuElem.className = "enabled"
        menuElem = document.getElementById("criteriafinding-menu")
        menuElem.className = "enabled"
        document.getElementById('anchor-criteriafinding').innerText = 'Criteria'

        menuElem = document.getElementById("brainstorming-menu")
        if (!$("#anchor-brainstorming").hasClass('enabled')) {
            !$("#anchor-brainstorming").addClass('enabled')
        }
        if ($("#anchor-brainstorming").hasClass('disabled')){
            $("#anchor-brainstorming").removeClass('disabled')
        }
        if (menuElem.className == "disabled") {
            menuElem.className = "enabled";
        }

        menuElem = document.getElementById("evaldr-menu")
        menuElem.className = "disabled"
        $("#anchor-evaluatingdr").addClass('disabled')
    }
    if (status < 2) {
        var menuElem = document.getElementById("alternatives-menu")
        menuElem.className = "disabled"
        menuElem = document.getElementById("criteriafinding-menu")
        menuElem.className = "disabled"
        menuElem = document.getElementById("brainstorming-menu")
        menuElem.className = "disabled"
        $("#anchor-brainstorming").addClass('disabled')
    }
}

function preventDefaultClick(e) {
    e.preventDefault();
}

function doLinks(issue) {
    if (issue.Id == -1)
        return;
    issueId = issue.Id
    var menuElem = document.getElementById("creating-menu")
    for (i = 0; i < menuElem.childNodes.length; i++) {
        if (menuElem.childNodes[i].nodeName == "A") {
            menuElem.childNodes[i].innerHTML = 'Define';
            menuElem.childNodes[i].href = "/Issue/Creating?issueId=" + issueId;
            break;
        }
    }

    menuElem = document.getElementById("finished-menu")
    for (i = 0; i < menuElem.childNodes.length; i++) {
        if (menuElem.childNodes[i].nodeName == "A") {
            menuElem.childNodes[i].href = "/Issue/Decision?issueId=" + issueId;
            if (issue.Status == "FINISHED" || issue.Status == "CLOSED")
                menuElem.childNodes[i].innerHTML = 'Decide';
            break;
        }
    }

    menuElem = document.getElementById("alternatives-menu")
    menuElem.childNodes[0].href = "/Issue/BrAlternatives?issueId=" + issueId;
    menuElem = document.getElementById("criteriafinding-menu")
    menuElem.childNodes[0].href = "/Issue/BrCriteria?issueId=" + issueId;
    menuElem = document.getElementById("criteriarating-menu")
    menuElem.childNodes[0].href = "/Issue/CriteriaRating?issueId=" + issueId;
    menuElem = document.getElementById("evaluating-menu")
    for (i = 0; i < menuElem.childNodes.length; i++) {
        if (menuElem.childNodes[i].nodeName == "A") {
            menuElem.childNodes[i].href = "/Issue/Evaluation?issueId=" + issueId;
            break;
        }
    }
}

//mark comments as read/seen
//type can be Issue, Alternative, Criterion
function onShowCommentsBtnClick(type) {
    var param = {
        issueId: viewModel.Issue.Id(),
        type: type
    }
    $.ajax({
        url: '/Issue/MarkCommentsAsRead',
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        async: true,
        processData: false,
        data: JSON.stringify(param),
        complete: function (r) {
            if (r.status != 200) {
                alert("Error happend!");
            }
        }
    })
}

//SignalR user added to issue
function userAddedToIssue(notificationHub) {
    notificationHub.client.userAddedToIssue = function (issue, accessRights, userId) {
        for (var i = 0; i < accessRights.length; i++) {
            if (accessRights[i].UserId == viewModel.UserId()) {
                $.notify({
                    icon: 'glyphicon glyphicon-info-sign',
                    title: 'New Issue',
                    message: 'You were added to issue: ' + issue.Title,
                    url: '/Issue/Creating?issueId=' + issue.Id
                }, {
                    delay: notDelayLong,
                    type: 'info',
                    placement: notPlacementCorner,
                    animate: notAnimate
                });
            }
        }
    }
}

//SignalR issue to next stage
function nextStageNotification(notificationHub) {
    notificationHub.client.nextStage = function (issueId, status, userId) {
        if (viewModel.UserId() == userId) {
            return;
        }
        viewModel.Issue.Status(status);
        renderMenues()
        $.notify({
            icon: 'glyphicon glyphicon-info-sign',
            title: 'Status updated',
            message: 'Issue was put to next stage!',
        }, {
            delay: notDelay,
            type: 'info',
            placement: notPlacementCorner,
            animate: notAnimate
        });
    }
}

//comment warning minimum 3 length
function commentWarning() {
    $.notify({
        icon: 'glyphicon glyphicon-warning-sign',
        title: 'System Notificaion',
        message: 'Comment must minimum be 3 characters long!'
    }, {
        delay: notDelay,
        type: 'warning',
        placement: notPlacement,
        animate: notAnimate
    });
}