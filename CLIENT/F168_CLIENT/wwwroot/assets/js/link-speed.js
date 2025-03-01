var fastest = {},
    tim = {},
    t = {},
    lis = {},
    speed = {},
    isMobile = false,
    zxkfUrl = "";
const maxVal = 100,
    minVal = 90;
let listMain = [];

function getEndTime(e) { }

function getColorForNum(e) {
    return e <= 200 ? "#96a2c2" : e > 200 && e <= 400 ? "#96a2c2" : e > 400 && e <= 600 ? "#96a2c2" : "#96a2c2";
}

function getTextForNum(e) {
    return e <= 200 ? "Rất Nhanh" : e > 200 && e <= 400 ? "Nhanh" : e > 400 && e <= 600 ? "Chậm" : "Hơi Chậm";
}

function getSpeedTime() {
    listMain.forEach(((e, t) => {
        tim[t] = (new Date()).getTime();
        const n = "<img src=" + e.url + ' width="1" height="1" onerror="getDiffEndTime(' + t + ')">';
        $($("#lineCtrl").find(".val")[t]).html(n);
    }));
}

function getDiffEndTime(e) {
    const t = (new Date()).getTime();
    getIntervalNum(Math.min(800, parseInt((t - tim[e]) / 2)), $("#lineCtrl .progress-content")[e]);
    //getIntervalNum(415, $("#lineCtrl .progress-content")[e]);

}

function getIntervalNum(e, t) {
    let n;
    const r = $(t).find(".val")[0],
        i = $(t).find(".text")[0],
        o = $(t).find(".pointer")[0],
        a = $(t).find(".svg-list-bg")[0],
        s = Array.from($(a).find(".path-svg"));
    s.forEach((e => {
        e.setAttribute("style", "fill:#e8e8e8}");
    }));
    let l = 0;
    n && clearInterval(n), n = setInterval((() => {
        l++, t.style.color = getColorForNum(l), r.innerText = l, i.innerText = getTextForNum(l), o.style.transform = `rotate(${0.3275 * l - 121}deg)`;
        const a = Math.ceil(0.25 * l);
        s.slice(0, a).forEach((e => {
            e.setAttribute("style", `fill:${getColorForNum(l)}`);
        })), l === e && clearInterval(n);
    }));
}

function checkIsMobile() {
    var e = false;
    if (-1 != window.location.toString().indexOf("pref=padindex")) e = false;
    else {
        (/AppleWebKit.*Mobile/i.test(navigator.userAgent) || /MIDP|SymbianOS|NOKIA|SAMSUNG|LG|NEC|TCL|Alcatel|BIRD|DBTEL|Dopod|PHILIPS|HAIER|LENOVO|MOT-|Nokia|SonyEricsson|SIE-|Amoi|ZTE/.test(navigator.userAgent)) && window.location.href.indexOf("?mobile") < 0 && (e = true);
    }
    return e;
}

function BBOnlineService() {
    window.open(zxkfUrl, "Service", "width=1039,height=728,status=no,scrollbars=no");
}

function addFavorite() {
    var e = window.location,
        t = document.title,
        n = navigator.userAgent.toLowerCase();
    if (n.indexOf("360se") > -1) alert("Do hạn chế về chức năng của trình duyệt 360, vui lòng nhấn Ctrl+D để thu thập thủ công！");
    else if (n.indexOf("msie 8") > -1) window.external.AddToFavoritesBar(e, t);
    else if (document.all) try {
        window.external.addFavorite(e, t);
    } catch (e) {
        alert("Trình duyệt của bạn không hỗ trợ, vui lòng nhấn Ctrl+D để lưu thủ công!");
    } else window.sidebar ? window.sidebar.addPanel(t, e, "") : alert("Trình duyệt của bạn không hỗ trợ, vui lòng nhấn Ctrl+D để lưu thủ công!");
}

var headArr, mainArr;
var jsonLink = [
    "https://app88.fun/trangchu",
    "https://app88.fun/trangchu",
    "https://app88.fun/trangchu",
    "https://app88.fun/trangchu",
    "https://app88.fun/trangchu",
    "https://app88.fun/trangchu",
]
var jsonData = {
    "success": true,
    "msg": "ok",
    "code": 0,
    "t": {
        "mainPositionList": [
            {
                "name": null,
                "mainPositionList": [
                    {
                        "id": 75,
                        "name": "LINK 1",
                        "rowName": null,
                        "url": "https://app88.fun/trangchu",
                        "sort": 1,
                        "position": 2,
                        "type": 1,
                        "platformId": 31
                    },
                    {
                        "id": 76,
                        "name": "LINK 2",
                        "rowName": null,
                        "url": "https://app88.fun/trangchu",
                        "sort": 2,
                        "position": 2,
                        "type": 1,
                        "platformId": 31
                    },
                    {
                        "id": 77,
                        "name": "LINK 3",
                        "rowName": null,
                        "url": "https://app88.fun/trangchu",
                        "sort": 3,
                        "position": 2,
                        "type": 1,
                        "platformId": 31
                    },
                    {
                        "id": 131,
                        "name": "LINK 4",
                        "rowName": null,
                        "url": "https://app88.fun/trangchu",
                        "sort": 4,
                        "position": 2,
                        "type": 1,
                        "platformId": 31
                    }
                ]
            }
        ],
        "otherPositionList": [
            {
                "id": 112,
                "name": "Trực tuyến",
                "rowName": null,
                "url": "https://app88.fun/trangchu",
                "sort": null,
                "position": 3,
                "type": 3,
                "platformId": 31
            }
        ]
    }
}


function randomizeUrls(data, linkList) {
    var mainPositionList = data.t.mainPositionList[0].mainPositionList;
    for (var i = 0; i < mainPositionList.length; i++) {
        var randomIndex = Math.floor(Math.random() * linkList.length);
        mainPositionList[i].url = linkList[randomIndex];
        linkList.splice(randomIndex, 1); // Loại bỏ URL đã sử dụng để tránh trùng lặp
    }
}
randomizeUrls(jsonData, jsonLink);

var dataCtrl = {
    init: function (jsonData) {
        var e = this;
        var t = jsonData.t || jsonData;
        mainArr = t.mainPositionList || t.mainPositionMap;
        headArr = t.otherPositionList || t.otherPosition;
        for (var n in mainArr) {
            var r = mainArr[n].mainPositionList;
            for (var i in r) {
                r[i].positionName = r[i].name;
            }
            mainArr[n].list = r;
        }
        for (var n in headArr) {
            if (headArr[n].name) {
                headArr[n].positionName = headArr[n].name;
            }
        }
        window.isNewWin || (isNewWin = false);
        e.makeHeadDom();
        e.makeMainDom();
    },

    makeHeadDom: function () {
        for (var e = 0; e < headArr.length; e++) {
            var t = headArr[e];
            switch (t.type) {
                case 1:
                case 2:
                    break;
                case 3:
                    zxkfUrl = headArr[e].url, '<label><i></i><a onclick="dataCtrl.openHref(headArr[' + e + '].url)" rel="nofollow">' + t.name + "</a></label>";
                    break;
                case 4:
                case 7:
                    '<label><i></i><a onclick="dataCtrl.openHref(headArr[' + e + '].url)" rel="nofollow">' + t.name + "</a></label>";
                    break;
                case 5:
                    '<label class="live"><i></i><a onclick="dataCtrl.openHref(headArr[' + e + '].url)" rel="nofollow">' + t.name + "</a></label>";
            }
        }
    },
    makeMainDom: function () {
        let e = "";
        const t = Array(60).fill(0);
        mainArr.forEach((e => {
            listMain.push(...e.list);
        })), listMain.forEach((n => {
            e += `\n<div class="link-item">\n        <div class="speed-item">\n        <a href="${n.url}" href="#" target="_blank" rel="noopener noreferrer"
                    bis_size='{"x":403,"y":430,"w":138,"h":16,"abs_x":403,"abs_y":430}' target="_blank" class="progress-content link-speed">\n        <div class="svg-list-bg">`, t.forEach(((t, n) => {
                e += `\n            <span class="svg-item" style="transform: translate3d(-50%, 0, 0) rotate(${4.5 * n - 134}deg);">\n        <svg\n          xmlns="http://www.w3.org/2000/svg"\n          width="7"\n          height="14"\n          viewBox="0 0 8 16"\n        >\n          <g fill="none" fill-rule="evenodd">\n            <g>\n              <g>\n                <path\n                  d="M0 0H8V16H0z"\n                  transform="translate(-410.000000, -115.000000) translate(410.000000, 115.000000)"\n                />\n                <path\n                  class="path-svg"\n                  fill="#E8E8E8"\n                  d="M1.66 1.977l.533 12.258c.033.741.66 1.324 1.403 1.323.34 0 .678+.003 1.018+.01.742+.01 1.369-.565 1.399-1.306l.496-12.26c.03-.74-.546-1.365-1.287-1.38C4.462.607 3.705.602 2.942.61 2.2.62 1.626 1.24 1.66 1.977"\n                  transform="translate(-410.000000, -115.000000) translate(410.000000, 115.000000)"\n                />\n              </g>\n            </g>\n          </g>\n        </svg>\n      </span>\n            `;
            })), e += `\n        </div>\n          <div class="big-circle">\n            <div class="pointer"></div>\n            <div class="circle-p"></div>\n          </div>\n          <div class="number">\n            <p class="val"></p>\n            <p class="text"></p>\n          </div>\n        </a>\n        <div class="address">\n            <a href="${n.url}" target="_blank">${n.name}</a>\n        </div>\n      </div>\n    </div>\n        `;
        })), $("#lineCtrl").html(e), setTimeout((() => {
            getSpeedTime();
        }), 300), $(".reload_click_btn").on("click", getSpeedTime);
    },
    sortArr: function (e) {
        return e.sort((function (e, t) {
            var n = e.sort,
                r = t.sort;
            return n < r ? -1 : n > r ? 1 : 0;
        }));
    },
    randomArr: function (e) {
        return e.sort((function () {
            return Math.floor(10 * Math.random()) > 4 ? -1 : 1;
        }));
    },
    urlCut: function (e) {
        return e.replace(/https:\/\/|www.|http:\/\//g, "");
    },
    openHref: function (e) {
        isNewWin ? window.open(e) : location.href = e;
    }
};
$((function () {
    dataCtrl.init(jsonData);
}));

$(document).ready(function () {

    var videos = [
        [{ type: 'mp4', 'src': './video/clip1.mp4#t=0.75' }],
        [{ type: 'mp4', 'src': './video/clip2.mp4#t=0.75' }],
        [{ type: 'mp4', 'src': './video/clip3.mp4#t=0.75' }],
        [{ type: 'mp4', 'src': './video/clip4.mp4#t=0.75' }],
        [{ type: 'mp4', 'src': './video/clip5.mp4#t=0.75' }],
        [{ type: 'mp4', 'src': './video/clip6.mp4#t=0.75' }],
        [{ type: 'mp4', 'src': './video/clip7.mp4#t=0.75' }],
    ];
    var randomitem = videos[Math.floor(Math.random() * videos.length)];

    function videoadd(element, src, type) {
        var source = document.createElement('source');
        source.src = src;
        source.type = type;
        element.appendChild(source);
    }

    function newvideo(src) {
        var video = document.getElementById("randomVideo");
        videoadd(video, src, 'video/ogg');
        video.autoplay = true;
        video.load();
    }
    newvideo(randomitem[0].src)

    // Change Video when finished
    document.getElementById('randomVideo').addEventListener('ended', handler, false);
    function handler(e) {
        newvideo(randomitem[0].src)
    }
})

var e = document.getElementById('randomVideo');
e.addEventListener('play', () => {
    e.currentTime = 0;
}, { once: true });
