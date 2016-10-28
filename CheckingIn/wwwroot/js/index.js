function addprofile(a, b) {
    var d = '<tr><td class="text-right">' + a + '</td><td class="text-left">' + b + '</td></tr>';
    $('#profile').append(d);
}


function addwarn(date, info) {
    var d = '<tr><td class="text-left">' + date + '</td><td class="text-left">' + info + '</td></tr>';
    $('#warn').append(d);
}

function adddata(name, date, intime, outtime,info) {
    var d = '<tr><td class="text-left">' + name + '</td><td class="text-left">' + date + '</td><td class="text-left">' + intime + '</td><td class="text-left">' + outtime + '</td><td class="text-left">' + info + '</td></tr>';
    $('#data').append(d);
}

function getdata(name) {
    $.get("getdata?name=" + name, function (data, status) {

        var d = JSON.parse(data);
        var profile = d.profile;
        for (var i in profile) {
            addprofile(i, profile[i]);
        }


        var warns = JSON.parse(d.warns);
        for (var i in warns) {
            addwarn(warns[i].date, warns[i].info);
        }
        var datas = JSON.parse(d.data);
        for (var i in datas) {
            adddata(name, datas[i].date, datas[i].intime, datas[i].outtime,datas[i].info);
        }
    });
}
//获取url中的参数
function getUrlParam(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)"); //构造一个含有目标参数的正则表达式对象
    var r = window.location.search.substr(1).match(reg);  //匹配目标参数
    if (r != null) return decodeURI(r[2]);
    return null; //返回参数值
}


$().ready(function () {
    var name = getUrlParam("name");
    getdata(name);
    $("#t").text(name + " 考勤分析报表");

})

