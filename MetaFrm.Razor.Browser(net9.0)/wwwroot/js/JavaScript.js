function ElementHide(elementId, isHide) {
    if (isHide)
        document.getElementById(elementId).style.display = "none";
    else
        document.getElementById(elementId).style.display = "";
}

function ElementFocus(elementId) {
    document.getElementById(elementId).focus();
}

function ModalClose(dataBsTarget) {
    var myModalEl = document.getElementById(dataBsTarget);
    var modal = bootstrap.Modal.getInstance(myModalEl)
    modal.hide();
}

window.getDimensions = function () {
    return {
        width: window.innerWidth,
        height: window.innerHeight
    };
};

function setCookie(c_name, value, exdays) {
    var exdate = new Date();
    exdate.setDate(exdate.getDate() + exdays);
    var c_value = escape(value) + ((exdays == null) ? "" : "; expires=" + exdate.toUTCString());
    document.cookie = c_name + "=" + c_value;
}

function getCookie(c_name) {
    var i, x, y, ARRcookies = document.cookie.split(";");
    for (i = 0; i < ARRcookies.length; i++) {
        x = ARRcookies[i].substr(0, ARRcookies[i].indexOf("="));
        y = ARRcookies[i].substr(ARRcookies[i].indexOf("=") + 1);
        x = x.replace(/^\s+|\s+$/g, "");
        if (x == c_name) {
            return unescape(y);
        }
    }
}

var isExpanded = false;
function LayoutMenuInit() {
    document.documentElement.setAttribute('class', 'light-style layout-menu-fixed layout-navbar-fixed');
    isExpanded = false;
}
function LayoutMenuExpande() {
    if (!isExpanded) {
        //document.documentElement.setAttribute('class', 'light-style layout-menu-fixed layout-transitioning layout-menu');
        document.documentElement.setAttribute('class', 'light-style layout-menu-fixed layout-navbar-fixed layout-menu-expanded');
        isExpanded = true;
    }
    else {
        document.documentElement.setAttribute('class', 'light-style layout-menu-fixed layout-navbar-fixed');
        isExpanded = false;
    }
}
function window_open(url, target, width, height) {
    var leftpos = screen.width / 2 - (width / 2);
    var toppos = screen.height / 2 - (height / 2);

    var winopts = "width=" + width + ", height=" + height + ", toolbar=no,status=no,statusbar=no,menubar=no,scrollbars=no,resizable=no";
    var position = ",left=" + leftpos + ", top=" + toppos;
    window.open(url, target, winopts + position);
}

function SetViewportScale(width, scale) {
    var viewportmeta = document.querySelector('meta[name="viewport"]');
    if (viewportmeta) {
        if (scale === undefined || scale === null || scale < 0.25 || scale > 10) {
        }
        else {
            if (width === undefined || width === null || width == "")
            {
                width = "device-width";
            }
            viewportmeta.content = "width=" + width + ", initial-scale=" + scale + ", minimum-scale=0.3, maximum-scale=5.0, user-scalable=no, viewport-fit=cover";
        }
    }
}