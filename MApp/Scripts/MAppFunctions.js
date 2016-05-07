﻿function renderMenues() {
    var issueId = viewModelJs.Issue.Id;

    if (viewModelJs.Issue.Status == "FINISHED") {
        status = 5
    } else if (viewModelJs.Issue.Status == "EVALUATING") {
        status = 4
    } else if (viewModelJs.Issue.Status == "BRAINSTORMING2") {
        status = 3
    } else if (viewModelJs.Issue.Status == "BRAINSTORMING1") {
        status = 2
    } else {
        status = 1
    }

    if (status < 6) {
        var menuElem = document.getElementById("creating-menu")
        for (i = 0; i < menuElem.childNodes.length; i++) {
            if (menuElem.childNodes[i].nodeName == "A") {
                menuElem.childNodes[i].innerHTML = 'Issue Overview';
                menuElem.childNodes[i].href = "/Issue/Creating?issueId=" + issueId;
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
        menuElem.childNodes[0].href = "/Issue/Evaluating?issueId=" + issueId;
        menuElem = document.getElementById("evaluating-menu")
        menuElem.childNodes[0].href = "/Issue/finished?issueId=" + issueId;
    }
    if (status < 5) {

        var menuElem = document.getElementById("finished-menu")
        menuElem.className = "disabled"
    }
    if (status < 4) {
        var menuElem = document.getElementById("evaluating-menu")
        menuElem.className = "disabled"
    }
    if (status < 3) {
        var menuElem = document.getElementById("criteriarating-menu")
        menuElem.className = "disabled"

    }
    if (status < 2) {
        var menuElem = document.getElementById("alternatives-menu")
        menuElem.className = "disabled"
        menuElem = document.getElementById("criteriafinding-menu")
        menuElem.className = "disabled"
    }
}