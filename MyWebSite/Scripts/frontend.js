function getXmlHttp() {
    try {
        return new ActiveXObject("Msxml2.XMLHTTP");
    } catch (e) {
        try {
            return new ActiveXObject("Microsoft.XMLHTTP");
        } catch (ee) {
        }
    }
    if (typeof XMLHttpRequest != 'undefined') {
        return new XMLHttpRequest();
    }
}

function ajaxSuccess(status, text) {
    if (status === 200) {
        var obj = JSON.parse(text);
        document.getElementById("all").textContent = obj.count;
        document.getElementById("today").textContent = obj.today;
        if (obj.last) document.getElementById("last").textContent = obj.last;
        document.cookie = "secret=" + obj.secret;
    }
}

function ajaxCounter(uid) {
    var xmlhttp = getXmlHttp();
    xmlhttp.open("GET", a ='/users/counter?uid=' + uid + '&r=' + Math.random());
    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState == 4) {
            ajaxSuccess(
                xmlhttp.status,
                xmlhttp.responseText
            );
        }
    }

    xmlhttp.send(null);
}

function whatTimeIsIt() {
    var xmlhttp = getXmlHttp();
    xmlhttp.open("GET", '/users/time' + '?r=' + Math.random());
    xmlhttp.onreadystatechange = function() {
        if (xmlhttp.readyState == 4) {
            timeAjaxSuccess(
                xmlhttp.status,
                xmlhttp.responseText
            );
        }
    };

    xmlhttp.send(null);
}

function timeAjaxSuccess(status, text) {
    if (status != 200) return;
    var obj = JSON.parse(text);
    document.getElementById("last").textContent = obj.now;
}

VK.Auth.getLoginStatus(function (response) {
    if (response.session) {
        document.cookie = "uid=" + response.session['mid'];
        ajaxCounter(response.session['mid']);
    } else {
        console.log(response.session);
        document.getElementById("all").textContent = 1;
        document.getElementById("today").textContent = 1;
        whatTimeIsIt();
    }
});

var resolution = document.createElement('div');
resolution.textContent = 'Разрешение ' + screen.width + 'x' + screen.height;
document.getElementById("counter").appendChild(resolution);

var loc = (document.location.href).match(/.*\/(.+)/)[1];
var elem = document.getElementById(loc);
if (elem != null) elem.classList.add("target");