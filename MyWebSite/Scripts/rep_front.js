function getCookie(name) {
    var matches = document.cookie.match(new RegExp(
        "(?:^|; )" + name.replace(/([\.$?*|{}\(\)\[\]\\\/\+^])/g, '\\$1') + "=([^;]*)"
    ));
    return matches ? decodeURIComponent(matches[1]) : undefined;
}

function checkSecret(button) {
    var secret = getCookie("secret");
    var uid = getCookie("uid");
    if (!secret || !uid) {
        alert('Включите cookie и обновите страницу!');
        return;
    }

    button.parentNode.elements["secret_box"].value = secret;
    button.parentNode.elements["uid_box"].value = uid;
}

function deleteReport(button) {
    button.parentNode.elements["type"].value = "delete";
}

function editSuccess(status, text, elem) {
    if (status == 200) {
        elem.outerHTML = text;
    }
}

function historySuccess(status, text, elem) {
    if (status == 200) {
        document.getElementById('edit_action').outerHTML = text;
    }
}

function historyChange(elem) {
    var parent = elem.parentNode;
    var uid = getCookie("uid");
    var id = parent.parentNode.children[0].textContent;
    var date = document.getElementById('history').value;
    var xmlhttp = getXmlHttp();
    xmlhttp.open("GET", '/users/history' + '?id=' + id + '&date=' + date + "&uid=" + uid);
    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState == 4) {
            historySuccess(
                xmlhttp.status,
                xmlhttp.responseText,
                parent
            );
        }
    };

    xmlhttp.send(null);
}

function editRep(elem) {
    var parent = elem.parentNode;
    var uid = getCookie("uid");
    var secret = getCookie("secret");
    var id = parent.children[1].textContent;
    var xmlhttp = getXmlHttp();
    xmlhttp.open("GET", '/users/EditReport' + '?uid=' + uid + '&secret=' + secret + '&id=' + id);
    xmlhttp.onreadystatechange = function() {
        if (xmlhttp.readyState == 4) {
            editSuccess(
                xmlhttp.status,
                xmlhttp.responseText,
                parent
            );
        }
    };

    xmlhttp.send(null);
}

function addEditButtons(href) {
    var elems = document.getElementsByClassName("report_item");
    for (var b = 0; b < elems.length; b++) {
        var elem = elems[b];
        if (elem.children[0].children[2].href == href) elem.children[4].style.display = 'block';
    }
}

VK.Auth.getLoginStatus(function (response) {
    if (response.session) {
        var uid = response.session['mid'];
        document.getElementById("report_action").style.display = "block";
        document.getElementById("not_auth_wrap").style.display = "none";
        VK.Api.call('users.get', { uids: uid, fields: ["photo_200"] }, function (r) {
            if (r.response) {
                document.getElementById("report_user_img").src = r.response[0].photo_200;
                document.getElementById("report_user_href").href = "http://vk.com/id" + uid;
                document.getElementById("report_user_href").textContent = r.response[0].first_name + ' ' + r.response[0].last_name;
                addEditButtons("http://vk.com/id" + uid);
            }
        });
    } else {
        document.getElementById("report_action").style.display = "none";
        document.getElementById("not_auth_wrap").style.display = "block";
    }
});